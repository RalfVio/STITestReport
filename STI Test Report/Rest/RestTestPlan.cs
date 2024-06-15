using ADORest.BusinessObjects;
using Newtonsoft.Json.Linq;
using SQLite.BusinessObjects;

namespace ADORest
{
    public class RestTestPlan: RestBase, IDisposable
    {
        HttpClient _httpClient = null;

        public RestTestPlan(TeamProject teamProject)
            : base(teamProject)
        { }

        public void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();
        }

        #region Test Plan
        public async Task<List<TestPlan>> GetTestPlans()
        {
            if (_httpClient == null)
                _httpClient = GetClient();
            var ids = await GetFlatQueryResult($"select * from WorkItems where [System.WorkItemType]='Test Plan' and [System.TeamProject]='{_teamProject}'",_httpClient);
            var idBatches = SplitIdsIntoBatches(ids);
            var result = new List<TestPlan>();
            foreach (var idBatch in idBatches)
                result.AddRange(await GetTestPlans(idBatch));

            return result;
        }
        private async Task<List<TestPlan>> GetTestPlans(List<int> workItemIds)
        {
            //https://docs.microsoft.com/en-us/rest/api/vsts/wit/wiql/query%20by%20wiql
            //https://www.visualstudio.com/en-us/docs/integrate/api/wit/wiql

            const string queryFormat = "{0}/_apis/wit/WorkItems?ids={1}&fields=System.Id,System.Title,System.State,System.Reason,System.WorkItemType&api-version=1.0";
            string restUri = string.Format(queryFormat
                , _collectionUrl, SqlIds(workItemIds));

            List<TestPlan> result = null;
            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();

                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.value;
                    int dataItemsCount = resultJObject.count;
                    result = new List<TestPlan>();

                    for (int i = 0; i < dataItemsCount; i++)
                    {
                        int id = dataItemsJObject[i].id;
                        result.Add(new TestPlan()
                        {
                            Id = id,
                            Rev = dataItemsJObject[i].rev,
                            Url = dataItemsJObject[i].url,
                            State = dataItemsJObject[i].fields["System.State"],
                            Reason = dataItemsJObject[i].fields["System.Reason"],
                            Title = dataItemsJObject[i].fields["System.Title"],
                            WorkItemType = dataItemsJObject[i].fields["System.WorkItemType"],
                        });

                    }
                }
            }
            catch
            {
                //MessageBox.Show(ex.ToString());
                throw;
            }

            return result;
        }
        public async Task<TestPlan> GetTestPlan(int id) => (await GetTestPlans(new List<int> { id })).FirstOrDefault();
        #endregion 

        #region Test Suites/Points
        public async Task<List<TestSuite>> GetTestSuites(TestPlan testPlan)
        {
            var result = await GetTestSuites(testPlan, null);
            while (_continuationToken != null)
                result.AddRange(await GetTestSuites(testPlan, _continuationToken));
            return result;
        }
        public async Task<List<TestSuite>> GetTestSuites(TestPlan testPlan, string continuationToken)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/testplan/test%20%20suites/get%20test%20suites%20for%20plan?view=azure-devops-rest-6.0
            //GET https://dev.azure.com/{organization}/{project}/_apis/testplan/Plans/{planId}/suites?expand={expand}&continuationToken={continuationToken}&asTreeView={asTreeView}&api-version=6.0-preview.1

            string restQuery = $"{_collectionUrl}/{_teamProject}/_apis/testplan/Plans/{testPlan.Id}/suites?" + (string.IsNullOrEmpty(continuationToken) ? "" : $"continuationToken={continuationToken}&") + "api-version=5.0-preview.1";

            List<TestSuite> result = null;
            _continuationToken = null;

            if (_httpClient == null)
                _httpClient = GetClient();

            try
            {
                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restQuery))
                {
                    response.EnsureSuccessStatusCode();

                    SetContinuationToken(response.Headers);

                    string responseString = await response.Content.ReadAsStringAsync();
                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.value;

                    result = new List<TestSuite>();
                    int count = resultJObject.count;

                    for (int i = 0; i < count; i++)
                    {
                        int id = dataItemsJObject[i].id;
                        int parentId = 0;
                        dynamic parent = dataItemsJObject[i].parentSuite;
                        if (parent != null)
                            parentId = parent.id;
                        result.Add(new TestSuite()
                        {
                            Id = id,
                            Title = dataItemsJObject[i].name,
                            ParentId = parentId,
                            SuiteType = dataItemsJObject[i].suiteType,
                            State = dataItemsJObject[i].state,
                        });
                    }

                }
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw;
            }

            return result;
        }

        public async Task<List<TestPoint>> GetTestPoints(TestSuite suite, int testPlanId)
        {
            //https://www.visualstudio.com/en-us/docs/integrate/api/test/overview
            //https://Fabrikam-Fiber-inc.VisualStudio.com/DefaultCollection/fabrikam-fiber-tfvc/_apis/test/plans/1/suites/1/points?api-version=1.0
            const string restUriFormat = "{0}/{1}/_apis/test/plans/{2}/suites/{3}/points?api-version=1.0";

            string restUri = string.Format(restUriFormat, new object[] { _collectionUrl, _teamProject, testPlanId, suite.Id });

            List<TestPoint> result = null;
            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();
                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.value;
                    int count = resultJObject.count;
                    result = new List<TestPoint>();

                    for (int i = 0; i < count; i++)
                    {
                        int id = dataItemsJObject[i].id;
                        result.Add(new TestPoint()
                        {
                            Id = id,
                            SuiteId = suite.Id,
                            ConfigurationId = dataItemsJObject[i].configuration.id,
                            ConfigurationTitle = dataItemsJObject[i].configuration.name,
                            AssignedTo = dataItemsJObject[i].assignedTo.displayName,
                            AssignedToSid = dataItemsJObject[i].assignedTo.id,
                            Outcome = dataItemsJObject[i].outcome,
                            State = dataItemsJObject[i].state,
                            TestCaseId = dataItemsJObject[i].testCase.id,
                            TestCaseTitle = dataItemsJObject[i].testCase.name,
                            LastTestRunId = dataItemsJObject[i].lastTestRun == null ? 0 : dataItemsJObject[i].lastTestRun.id,
                        });
                    }
                }
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw;
            }

            return result;
        }
        #endregion

        #region Test runs
        //public async Task<List<TestRun>> GetTestRuns(TestPlan testPlan)
        //{
        //    const int top = 100;

        //    var result = new List<TestRun>();
        //    int skip = 0; int testRunsCount = 0;

        //    do
        //    {
        //        var testRuns = await GetTestRuns(testPlan, skip, top);
        //        testRunsCount = testRuns.Count;
        //        result.AddRange(testRuns);

        //        skip += testRunsCount;
        //    }
        //    while (testRunsCount == top);

        //    return result;
        //}

        public class TestRuns
        {
            public List<TestRun> Runs { get; set; }
            public int Count { get; set; } 
        }
        public async Task<TestRuns> GetTestRuns(TestPlan testPlan, int skip, int top)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/test/runs/list?view=azure-devops-rest-7.1
            string restUri = $"{_collectionUrl}/{_teamProject}/_apis/test/runs?planId={testPlan.Id}&includeRunDetails=true&$skip={skip}&$top={top}&api-version=7.1-preview.3";

            TestRuns result = null;
            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();
                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.value;
                    result = new TestRuns()
                    {

                        Runs = new List<TestRun>(),
                        Count = resultJObject.count,
                    };
                    for (int i = 0; i < result.Count; i++)
                    {
                        int id = dataItemsJObject[i].id;
                        string state = dataItemsJObject[i].state;
                        if (state != "255")
                            result.Runs.Add(new TestRun()
                            {
                                Id = dataItemsJObject[i].id,
                                Revision = dataItemsJObject[i].revision,
                                Title = dataItemsJObject[i].name,
                                PlanId = testPlan.Id,
                                IsAutomated = dataItemsJObject[i].isAutomated,
                                OwnerDisplayName = dataItemsJObject[i].owner.displayName,
                                OwnerSid = dataItemsJObject[i].owner.id,
                                StartedDate = dataItemsJObject[i].startedDate,
                                CompletedDate = dataItemsJObject[i].completedDate,
                                State = state,
                                BuildId = dataItemsJObject[i].build == null ? 0 : dataItemsJObject[i].build.id,
                            });
                    }

                }
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw;
            }

            return result;
        }
        #endregion

        #region Test results
        public async Task<List<TestStepResult>> GetTestStepResults(int runId)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/test/results/list?view=azure-devops-rest-6.1
            //GET https://dev.azure.com/{organization}/{project}/_apis/test/Runs/{runId}/results?detailsToInclude={detailsToInclude}&$skip={$skip}&$top={$top}&outcomes={outcomes}&api-version=6.1-preview.6

            string restUri = $"{_collectionUrl}/{_teamProject}/_apis/test/Runs/{runId}/results?detailsToInclude=iterations,workItems&api-version=6.1-preview.6";

            List<TestStepResult> result = null;

            string responseString;
            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    responseString = await response.Content.ReadAsStringAsync();

                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.value;
                    int count = resultJObject.count;

                    result = new List<TestStepResult>();

                    for (int i = 0; i < count; i++)
                    {
                        //resultId
                        int id = dataItemsJObject[i].id;
                        {
                            List<int> associatedWIIds = null;
                            if (dataItemsJObject[i].associatedBugs != null)
                            {
                                dynamic associatedBugsJObject = dataItemsJObject[i].associatedBugs;
                                int associatedBugsCount = associatedBugsJObject.Count;

                                associatedWIIds = new List<int>();
                                string associatedWIIdRaw;
                                for (int ii = 0; ii < associatedBugsCount; ii++)
                                {
                                    associatedWIIdRaw = associatedBugsJObject[ii].id;
                                    if (int.TryParse(associatedWIIdRaw, out int associatedWIId))
                                        associatedWIIds.Add(associatedWIId);
                                }
                            }
                            var testStepResult = new TestStepResult()
                            {
                                RunId = runId,
                                ResultId = dataItemsJObject[i].id,
                                StartedDate = dataItemsJObject[i].startedDate ?? new DateTime(1900, 1, 1),
                                CompletedDate = dataItemsJObject[i].completedDate ?? new DateTime(1900, 1, 1),
                                Outcome = dataItemsJObject[i].outcome ?? "",
                                Comment = dataItemsJObject[i].comment ?? "",
                                Revision = dataItemsJObject[i].revision,
                                State = dataItemsJObject[i].state ?? "",
                                TestCaseId = dataItemsJObject[i].testCase.id,
                                TestCaseTitle = dataItemsJObject[i].testCaseTitle ?? "",
                                TestCaseRevision = dataItemsJObject[i].testCaseRevision ?? 0,
                                TestPointId = dataItemsJObject[i].testPoint.id,
                                RunBy = dataItemsJObject[i].runBy == null ? "" : dataItemsJObject[i].runBy.displayName,
                                BuildId = dataItemsJObject[i].build == null ? 0 : dataItemsJObject[i].build.id,
                                BuildName = dataItemsJObject[i].build == null ? "" : dataItemsJObject[i].build.name,
                                AssociatedWIIds = associatedWIIds,
                            };
                            if (dataItemsJObject[i].iterationDetails != null && dataItemsJObject[i].iterationDetails.Count > 0)
                            {
                                dynamic iterationDetailsJObject = dataItemsJObject[i].iterationDetails;
                                int iterationDetailsCount = iterationDetailsJObject.Count;
                                testStepResult.Iterations = new List<TestResultIteration>();
                                for (int ii = 0; ii < iterationDetailsCount; ii++)
                                {
                                    var iteration = new TestResultIteration()
                                    {
                                        Outcome = iterationDetailsJObject[ii].outcome ?? "",
                                        Comment = iterationDetailsJObject[ii].comment ?? "",
                                        Id = iterationDetailsJObject[ii].id,
                                    };
                                    if (iterationDetailsJObject[ii].actionResults != null && iterationDetailsJObject[ii].actionResults.Count > 0)
                                    {
                                        dynamic actionResultsJObject = iterationDetailsJObject[ii].actionResults;
                                        iteration.ActionResultsJson = actionResultsJObject.ToString();
                                    }
                                    if (iterationDetailsJObject[ii].parameters != null && iterationDetailsJObject[ii].parameters.Count > 0)
                                    {
                                        dynamic parametersJObject = iterationDetailsJObject[ii].parameters;
                                        iteration.ParametersJson = parametersJObject.ToString();
                                    }
                                    testStepResult.Iterations.Add(iteration);
                                }
                            }
                            result.Add(testStepResult);
                        }
                    }
                }

            }
            catch { return null; }

            return result;
        }
        public async Task GetTestCaseData(TestStepResult testStepResult, DateTime startTime)
        {
            //https://learn.microsoft.com/en-us/rest/api/azure/devops/wit/work-items/get-work-item?view=azure-devops-rest-6.0&tabs=HTTP
            // GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{id}?fields={fields}&asOf={asOf}&$expand={$expand}&api-version=6.0

            string restUri = $"{_collectionUrl}/{_teamProject}/_apis/wit/workitems/{testStepResult.TestCaseId}?asOf={startTime:O}&api-version=6.0";

            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();

                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic fieldsJObject = resultJObject.fields;

                    testStepResult.TestCaseStepsXML = fieldsJObject["Microsoft.VSTS.TCM.Steps"] ?? "";
                    testStepResult.TestCaseState = fieldsJObject["System.State"] ?? "";
                    testStepResult.TestCaseRevision = resultJObject.rev;
                    testStepResult.TestCaseDescription = fieldsJObject["System.Description"] ?? "";
                }

            }
            catch { throw; } //(Exception ex) { OnMessage?.Invoke(this, new MessageEventArgs(ex)); }
        }

        public async Task GetTestCaseData(TestStepResult testStepResult)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/revisions/get?view=azure-devops-rest-6.0
            //GET https://dev.azure.com/{organization}/{project}/_apis/wit/workItems/{id}/revisions/{revisionNumber}?$expand={$expand}&api-version=6.0

            string restUri = $"{_collectionUrl}/{_teamProject}/_apis/wit/workitems/{testStepResult.TestCaseId}/revisions/{testStepResult.TestCaseRevision}?api-version=6.0";

            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();

                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic fieldsJObject = resultJObject.fields;

                    testStepResult.TestCaseStepsXML = fieldsJObject["Microsoft.VSTS.TCM.Steps"] ?? "";
                    testStepResult.TestCaseState = fieldsJObject["System.State"] ?? "";
                    testStepResult.TestCaseDescription = fieldsJObject["System.Description"] ?? "";
                }

            }
            catch { throw; } //(Exception ex) { OnMessage?.Invoke(this, new MessageEventArgs(ex)); }
        }

        public async Task<SharedSteps> GetSharedStepsData(int sharedStepsId, DateTime asOf, int stepId)
        {
            //https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work-items/get-work-item?view=azure-devops-rest-6.0
            //GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{id}?fields={fields}&asOf={asOf:yyyy-MM-ddT}&$expand={$expand}&api-version=6.0

            const string fields = "System.Title,Microsoft.VSTS.TCM.Steps";
            string restUri = $"{_collectionUrl}/{_teamProject}/_apis/wit/workitems/{sharedStepsId}?fields={fields}" + (asOf.Year < 2000 ? "" : $"&asOf={asOf:O}") + "&api-version=6.0";

            SharedSteps result = null;
            string responseString = null;
            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restUri))
                {
                    response.EnsureSuccessStatusCode();

                    responseString = await response.Content.ReadAsStringAsync();

                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic fieldsJObject = resultJObject.fields;

                    result = new SharedSteps()
                    {
                        Id = resultJObject.id,
                        Revision = resultJObject.rev,
                        Title = fieldsJObject["System.Title"] ?? "",
                        StepId = stepId,
                        SharedStepsXML = fieldsJObject["Microsoft.VSTS.TCM.Steps"] ?? "",
                    };
                }

            }
            catch { throw; } //(Exception ex) { OnMessage?.Invoke(this, new MessageEventArgs(ex)); }

            return result;
        }

        #endregion

        public async Task<List<WorkItem>> GetWorkItems(List<int> ids)
        {
            const string fields = "System.WorkItemType,System.State,System.Reason,System.Title,System.AssignedTo,System.CreatedDate"; //Microsoft.VSTS.Scheduling.StartDate,Microsoft.VSTS.Scheduling.FinishDate

            string idString = null; foreach (int id in ids) idString = (idString == null ? "" : idString + ",") + id.ToString();
            string restQuery = $"{_collectionUrl}/_apis/wit/workitems?ids={idString}&fields={fields}&api-version=1.0";

            List<WorkItem> result = null;
            try
            {
                if (_httpClient == null)
                    _httpClient = GetClient();

                _serviceCalls++;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                            restQuery))
                {
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();
                    dynamic resultJObject = JObject.Parse(responseString);
                    dynamic dataItemsJObject = resultJObject.value;
                    int count = resultJObject.count;

                    //[{"id":125986,"rev":37,"fields":{"System.AreaPath":"Philips.PIC\\PIIC_iX","System.TeamProject":"Philips.PIC","System.IterationPath":"Philips.PIC\\PIIC_iX\\Legacy\\Birdo","System.WorkItemType":"Test Plan","System.State":"Active","System.Reason":"New test plan","System.AssignedTo":"Gong, Yebin <CODE1\\usd43559>","System.CreatedDate":"2016-03-14T17:30:05.043Z","System.CreatedBy":"Gong, Yebin <CODE1\\usd43559>","System.ChangedDate":"2016-08-30T18:38:15.19Z","System.ChangedBy":"Hackett, Craig <CODE1\\310120513>","System.Title":"iX C.0 RfV Master DVIP","Microsoft.VSTS.Scheduling.StartDate":"2016-03-13T05:00:00Z","Microsoft.VSTS.Scheduling.FinishDate":"2016-06-29T04:00:00Z","System.Description":"<p>March 14, 2016: see attachment for the DVIP meeting and entry criteria.</p>\r\n<p>&nbsp;</p>\r\n<p>Mar 31, 2016: DVIP2</p>\r\n<p>&nbsp;</p>\r\n<p>April 18, 2016: DVIP3</p>\r\n<p>&nbsp;</p>\r\n<p>To see all the DVIPs, please view the attachments.</p>"},"url":"http://usdanrmmd1ms99a.code1.emi.philips.com:8080/tfs/Philips.Collection/_apis/wit/workItems/125986"}]
                    result = new List<WorkItem>();

                    for (int i = 0; i < count; i++)
                    {
                        int id = dataItemsJObject[i].id;

                        WorkItem workItem = new WorkItem()
                        {
                            Id = id,
                            WorkItemType = dataItemsJObject[i].fields["System.WorkItemType"],
                            State = dataItemsJObject[i].fields["System.State"],
                            Reason = dataItemsJObject[i].fields["System.Reason"],
                            Title = dataItemsJObject[i].fields["System.Title"],
                            AssignedTo = dataItemsJObject[i].fields["System.AssignedTo"],
                            CreatedDate = dataItemsJObject[i].fields["System.CreatedDate"],
                        };

                        result.Add(workItem);
                    }

                }
            }
            catch //(Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                throw;
            }

            return result;
        }

    }
}
