using ADORest.BusinessObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
