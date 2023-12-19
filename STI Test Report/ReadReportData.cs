using ADORest;
using ADORest.BusinessObjects;
using SQLite.BusinessObjects;
using System.Threading;

namespace STI_Test_Report
{
    public delegate void LogEventHandler(object sender, LogEventArgs e);
    
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    
    internal class ReadReportData
    {
        public event LogEventHandler WriteLog;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        #region Sub classes
        public class Parameters
        {
            public enum ProcessTypes { testPoints, testRuns, testResults, workItems }
            public ProcessTypes ProcessType { get; set; }
            public int TestPlanId { get; set; }
            public bool CreateNewFile { get; set; }
            public int? BatchSize { get; set; }
            public bool KeepRecords { get; set; }
            public List<int> TestSuiteIds { get; set; }

            public string LogHeader()
            {
                const string intend = "  ";
                var result = new System.Text.StringBuilder();
                switch (ProcessType)
                {
                    default:
                        result.AppendLine($"{intend}Command:         {Enum.GetName(typeof(ProcessTypes), ProcessType)}");
                        break;
                }
                result.AppendLine($"{intend}Create new file: {CreateNewFile}");
                result.AppendLine($"{intend}Test plan:       {TestPlanId}");
                result.AppendLine($"{intend}Keep records:    {KeepRecords}");
                if (BatchSize.HasValue)
                    result.AppendLine($"{intend}Bnatch size:     {BatchSize}");
                if (TestSuiteIds != null)
                    result.AppendLine($"{intend}Test suites:     {ADORest.RestBase.SqlIds(TestSuiteIds)}");

                return result.ToString();
            }
        }
        #endregion
        public async static Task Start(Parameters commandFile, ADORest.RestTestPlan adoRest, TeamProject teamProject, string dbFilePath, LogEventHandler logEventHandler )
        {
            await _semaphore.WaitAsync();

            ReadReportData readData = null;
            try
            {
                readData = new ReadReportData();
                readData.WriteLog += logEventHandler;
                await readData.Start(commandFile, adoRest, teamProject, dbFilePath );
            }
            finally 
            {
                readData.WriteLog -= logEventHandler;
                _semaphore.Release();
            }
            
        }

        public async Task Start(Parameters commandFile, ADORest.RestTestPlan adoRest, TeamProject teamProject, string dbFilePath)
        {
            if (!File.Exists(dbFilePath) || commandFile.CreateNewFile)
                File.Copy(Program.GetTemplateDBFilePath(), dbFilePath, true);

            var testPlan = await adoRest.GetTestPlan(commandFile.TestPlanId);

            using var sqlLiteBL = new SQLiteConnector.BL();
            sqlLiteBL.OpenDatabase(dbFilePath);
            sqlLiteBL.TestPlanSave(testPlan);

            switch (commandFile.ProcessType)
            {
                case ReadReportData.Parameters.ProcessTypes.testPoints:
                    await ReadTestPoints(testPlan, commandFile.TestSuiteIds, adoRest, sqlLiteBL);
                    break;
                case ReadReportData.Parameters.ProcessTypes.testRuns:
                    await ReadTestRuns(testPlan, adoRest, sqlLiteBL);
                    break;
                case ReadReportData.Parameters.ProcessTypes.testResults:
                    await ReadTestResults(testPlan, commandFile.BatchSize, commandFile.KeepRecords, commandFile.TestSuiteIds, adoRest, sqlLiteBL);
                    break;
                case ReadReportData.Parameters.ProcessTypes.workItems:
                    await ReadWorkItems(testPlan, adoRest, sqlLiteBL);
                    break;
            }
        }
        private async Task ReadTestPoints(SQLite.BusinessObjects.TestPlan testPlan, List<int> testSuiteIds, ADORest.RestTestPlan adoRest, SQLiteConnector.BL sqlLiteBL)
        {
            var startTime = DateTime.Now;
            var startServiceCalls = adoRest.GetServiceCalls();

            sqlLiteBL.TestSuitesDelete();

            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Reading test points for test plan {testPlan.Id}:{testPlan.Title}" });
            WriteLog?.Invoke(this, new LogEventArgs() { Message = "Reading test suites" });
            var testSuites = await adoRest.GetTestSuites(testPlan);
            SQLite.BusinessObjects.TestSuite.SetSuitePath(ref testSuites);

            WriteLog?.Invoke(this, new LogEventArgs() { Message = "Reading test points" });
            

            foreach (var suite in testSuites.OrderBy(ts => ts.SuitePath))
                sqlLiteBL.TestSuiteInsert(suite);

            List<TestSuite> testSuitesSorted = null;
            if (testSuiteIds != null && testSuiteIds.Count > 0)
                testSuitesSorted = testSuites.Where(ts=>testSuiteIds.Contains(ts.Id)).OrderBy(ts => ts.SuitePath).ToList();
            else
                testSuitesSorted = testSuites.OrderBy(ts => ts.SuitePath).ToList();


            for (int i=0;i<testSuitesSorted.Count;i++)
            {
                if((i+1)%25==0 || i==0 || i==testSuitesSorted.Count-1)
                {
                    WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Test suite {i+1}/{testSuitesSorted.Count} {testSuitesSorted[i].SuitePath}"});
                }
                
                var testPoints = await adoRest.GetTestPoints(testSuitesSorted[i], testPlan.Id);
                foreach (var testPoint in testPoints)
                    sqlLiteBL.TestPointInsert(testPoint);
            }

            sqlLiteBL.StatsSave("TestPoints", startTime, DateTime.Now, adoRest.GetServiceCalls() - startServiceCalls);
            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Done, {adoRest.GetServiceCalls() - startServiceCalls} service calls, processing time: {(DateTime.Now - startTime):mm\\:ss\\.f}" });
        }

        private async Task ReadTestRuns(SQLite.BusinessObjects.TestPlan testPlan, ADORest.RestTestPlan adoRest, SQLiteConnector.BL sqlLiteBL)
        {
            const int batchSize = 100;

            var startTime = DateTime.Now;
            var startServiceCalls = adoRest.GetServiceCalls();

            sqlLiteBL.TestRunsDelete();

            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Reading test runs for test plan {testPlan.Id}:{testPlan.Title}"});


            int skip = 0; int testRunsCount = 0; int batchNumber = 0;
            do
            {
                batchNumber++;
                WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Test runs, batch {batchNumber}" });
                var testRuns = await adoRest.GetTestRuns(testPlan, skip, batchSize);

                testRunsCount = testRuns.Count;
                skip += testRunsCount;

                foreach (var testRun in testRuns.Runs)
                    sqlLiteBL.TestRunInsert(testRun);
            }
            while (testRunsCount > 0);

            sqlLiteBL.StatsSave("TestRuns", startTime, DateTime.Now, adoRest.GetServiceCalls() - startServiceCalls);

            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"{skip + testRunsCount} test runs" });
            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Done, {adoRest.GetServiceCalls() - startServiceCalls} service calls, processing time: {(DateTime.Now - startTime):mm\\:ss\\.f}" });
        }
        private async Task ReadTestResults(SQLite.BusinessObjects.TestPlan testPlan, int? batchSize, bool keepRecords, List<int> testSuiteIds, ADORest.RestTestPlan adoRest, SQLiteConnector.BL sqlLiteBL)
        {
            var startTime = DateTime.Now;
            var startServiceCalls = adoRest.GetServiceCalls();

            if (!keepRecords)
                sqlLiteBL.TestResultsDelete();

            var runIds = sqlLiteBL.TestRunsGetIds(batchSize, testSuiteIds);
            if (runIds.Count == 0)
            {
                WriteLog?.Invoke(this, new LogEventArgs() { Message = "No run ids found" });
                return;
            }

            sqlLiteBL.TestPlanSave(testPlan);


            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Reading test results for test plan {testPlan.Id}:{testPlan.Title}" });

            for (int i = 0; i < runIds.Count; i++)
            {
                if (i == 0 || i == runIds.Count - 1 || (i + 1) % 25 == 0)
                    WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Test result for run {runIds[i]} ({i + 1}/{runIds.Count})" });

                var testStepResults = await adoRest.GetTestStepResults(runIds[i]);
                if (testStepResults != null)
                    foreach (var testResult in testStepResults)
                    {
                        if (testResult.TestCaseRevision > 0)
                            await adoRest.GetTestCaseData(testResult);
                        else
                            await adoRest.GetTestCaseData(testResult, testResult.StartedDate);
                        var testSteps = testResult.GetTestCaseSteps();
                        foreach (var testStep in testSteps)
                        {
                            if (testStep.TypeOfStep == SQLite.BusinessObjects.Step.TypesOfStep.SharedStepsRef
                                && (testResult.SharedSteps == null || !testResult.SharedSteps.Exists(st => st.Id == testStep.SharedStepsId)))
                            {
                                var sharedSteps = await adoRest.GetSharedStepsData(testStep.SharedStepsId, testResult.StartedDate, testStep.Id);
                                if (testResult.SharedSteps == null)
                                    testResult.SharedSteps = new List<SQLite.BusinessObjects.SharedSteps>();
                                testResult.SharedSteps.Add(sharedSteps);
                            }
                        }

                        sqlLiteBL.TestResultInsert(testResult);
                    }
            }

            sqlLiteBL.StatsSave("TestResults", startTime, DateTime.Now, adoRest.GetServiceCalls() - startServiceCalls);
            int remainingTestRuns = sqlLiteBL.TestRunsGetRemaining(testSuiteIds);
            //WriteLog?.Invoke(this, new LogEventArgs() { Message = $"{remainingTestRuns} remaining test runs"});
            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Done, {remainingTestRuns} remaining, {adoRest.GetServiceCalls() - startServiceCalls} service calls, processing time: {(DateTime.Now - startTime):mm\\:ss\\.f}"});
        }

        private async Task ReadWorkItems(SQLite.BusinessObjects.TestPlan testPlan, ADORest.RestTestPlan adoRest, SQLiteConnector.BL sqlLiteBL)
        {
            var startTime = DateTime.Now;
            var startServiceCalls = adoRest.GetServiceCalls();

            var ids = sqlLiteBL.WorkItemsGetIds();
            if (ids.Count == 0)
            {
                WriteLog?.Invoke(this, new LogEventArgs() { Message = "No work items found" });
                return;
            }

            sqlLiteBL.TestPlanSave(testPlan);

            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Reading work items for test plan {testPlan.Id}:{testPlan.Title}" });

            var idsBatches = RestBase.SplitIdsIntoBatches(ids);

            var workItems = new List<SQLite.BusinessObjects.WorkItem>();
            foreach (var idsBatch in idsBatches)
                workItems.AddRange(await adoRest.GetWorkItems(idsBatch));

            sqlLiteBL.WorkItemsInsert(workItems);

            sqlLiteBL.StatsSave("ReadWorkItems", startTime, DateTime.Now, adoRest.GetServiceCalls() - startServiceCalls);
            WriteLog?.Invoke(this, new LogEventArgs() { Message = $"Done, {adoRest.GetServiceCalls() - startServiceCalls} service calls, processing time: {(DateTime.Now - startTime):mm\\:ss\\.f}" });
        }

    }
}