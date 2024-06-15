using SQLite.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteConnector
{
    public class BL : IDisposable
    {
        DAL_SQLite _sqLite = null;

        public void Dispose()
        {
            if (_sqLite != null)
            {
                _sqLite.CloseDatabase();
                _sqLite.Dispose();
            }
        }
        public void OpenDatabase(string filePath, bool dbCheck)
        {
            _sqLite = new DAL_SQLite();
            _sqLite.OpenDatabase(filePath);
            if (dbCheck)
            {
                DBCheck("TestResults", "TestCaseDescription", "TEXT");
            }
        }

        private void DBCheck(string tableName, string fieldName, string fieldType)
        {
            string sql = "select count(*)"
                + $"\r\nfrom pragma_table_info({DAL_SQLite.SQLText(tableName)})"
                + $"\r\nwhere name={DAL_SQLite.SQLText(fieldName)}";
            var fieldExists = _sqLite.ExecuteCommandValue(sql);

            if (fieldExists.HasValue && fieldExists.Value == 0)
            {
                sql = $"ALTER TABLE {tableName}"
                    + $"\r\nADD COLUMN {fieldName} {fieldType}";
                _sqLite.ExecuteCommand(sql);
            }
        }

        public void TestPlanSave(TestPlan testPlan)
        {
            string sql = "Update TestPlan"
            + "\r\nset Id=" + DAL_SQLite.SQLInt(testPlan.Id)
            + ",Title=" + DAL_SQLite.SQLText(testPlan.Title)
            + ",State=" + DAL_SQLite.SQLText(testPlan.State)
            + ",Reason=" + DAL_SQLite.SQLText(testPlan.Reason)
            + ",AssignedTo=" + DAL_SQLite.SQLText(testPlan.AssignedTo);
            int recs = _sqLite.ExecuteCommand(sql);

            if (recs > 0)
                return;

            sql = "Insert into TestPlan (Id,Title,State,Reason,AssignedTo)"
                + "\r\nValues"
                + "\r\n(" + DAL_SQLite.SQLInt(testPlan.Id)
                + "\r\n," + DAL_SQLite.SQLText(testPlan.Title)
                + "\r\n," + DAL_SQLite.SQLText(testPlan.State)
                + "\r\n," + DAL_SQLite.SQLText(testPlan.Reason)
                + "\r\n," + DAL_SQLite.SQLText(testPlan.AssignedTo)
                + ")";
            recs = _sqLite.ExecuteCommand(sql);
        }

        public TestPlan TestPlanRead()
        {
            string sql = "SELECT Id,Title,State,Reason,AssignedTo from TestPlan";
            if (!_sqLite.Rdr_Open(sql))
                return null;

            TestPlan result = null;

            try
            {
                if (_sqLite.Rdr_DataRead())
                {
                    result = new TestPlan() {
                        Id= _sqLite.Rdr_DataFieldInt(0),
                        Title = _sqLite.Rdr_DataFieldStr(1),
                        State = _sqLite.Rdr_DataFieldStr(2),
                        Reason = _sqLite.Rdr_DataFieldStr(3),
                        AssignedTo = _sqLite.Rdr_DataFieldStr(4),
                    };
                }

                _sqLite.Rdr_Close();
            }

            catch { result = null; _sqLite.Rdr_Close(); }

            return result;
        }
        public void TestSuitesDelete()
        {
            string sql = "DELETE FROM TestSuites";
            int recs = _sqLite.ExecuteCommand(sql);

            sql = "DELETE FROM TestPoints";
            recs = _sqLite.ExecuteCommand(sql);
        }
        public void TestSuiteInsert(TestSuite testSuite)
        {
            string sql = "INSERT INTO TestSuites (Id,ParentId,Title,SuitePath,SuiteType)"
            + "\r\nValues"

            + "(" + DAL_SQLite.SQLInt(testSuite.Id)
            + "," + (testSuite.ParentId <= 0 ? DAL_SQLite.SQLNull() : DAL_SQLite.SQLInt(testSuite.ParentId))
            + "," + DAL_SQLite.SQLText(testSuite.Title)
            + "," + DAL_SQLite.SQLText(testSuite.SuitePath)
            + "," + DAL_SQLite.SQLText(testSuite.SuiteType)
            + ")";

            int recs = _sqLite.ExecuteCommand(sql);
        }

        public List<TestSuite> TestSuitesRead(List<int> testSuiteIds)
        {
            string sql = "SELECT ts.Id,ts.ParentId,ts.Title,ts.SuitePath,ts.SuiteType"
                + ",tp.Id,tp.AssignedTo,tp.TestCaseId,tp.TestCaseTitle,tp.ConfigurationId,tp.ConfigurationTitle,tp.LastTestRunId,tp.State,tp.Outcome"
                 + "\r\nfrom TestSuites ts"
                + "\r\ninner join TestPoints tp on tp.TestSuiteId=ts.Id"
                + (testSuiteIds==null||testSuiteIds.Count==0?"":$"\r\nWHERE ts.Id in ({SqlIds(testSuiteIds)})")
                + "\r\norder by ts.SuitePath";
            if (!_sqLite.Rdr_Open(sql))
                return null;

            List<TestSuite> result = new List<TestSuite>();
            TestSuite testSuite = null;
            try
            {
                while (_sqLite.Rdr_DataRead())
                {
                    int testSuiteId = _sqLite.Rdr_DataFieldInt(0);
                    if (testSuite == null || testSuite.Id != testSuiteId)
                    {
                        testSuite = new TestSuite()
                        {
                            Id = testSuiteId,
                            ParentId = _sqLite.Rdr_DataFieldInt(1),
                            Title = _sqLite.Rdr_DataFieldStr(2),
                            SuitePath = _sqLite.Rdr_DataFieldStr(3),
                            SuiteType = _sqLite.Rdr_DataFieldStr(4),
                            TestPoints = new List<TestPoint>(),
                        };

                        result.Add(testSuite);
                    }

                    if(!_sqLite.Rdr_DataFieldIsNull(5))
                    {
                        testSuite.TestPoints.Add(new TestPoint()
                        {
                            Id = _sqLite.Rdr_DataFieldInt(5),
                            AssignedTo = _sqLite.Rdr_DataFieldStr(6),
                            TestCaseId = _sqLite.Rdr_DataFieldInt(7),
                            TestCaseTitle = _sqLite.Rdr_DataFieldStr(8),
                            ConfigurationId = _sqLite.Rdr_DataFieldInt(9),
                            ConfigurationTitle = _sqLite.Rdr_DataFieldStr(10),
                            LastTestRunId = _sqLite.Rdr_DataFieldInt(11),
                            State = _sqLite.Rdr_DataFieldStr(12),
                            Outcome = _sqLite.Rdr_DataFieldStr(13),
                        });
                    }
                       

                }

                _sqLite.Rdr_Close();
            }

            catch { result = null; _sqLite.Rdr_Close(); }

            return result;
        }
        public void TestPointInsert(TestPoint testPoint)
        {
            string sql = "INSERT INTO TestPoints (Id,TestSuiteId,AssignedTo,TestCaseId,TestCaseTitle,ConfigurationId,ConfigurationTitle,LastTestRunId,State,Outcome)"
            + "\r\nValues"
            + "(" + DAL_SQLite.SQLInt(testPoint.Id)
            + "," + DAL_SQLite.SQLInt(testPoint.SuiteId)
            + "," + DAL_SQLite.SQLText(testPoint.AssignedTo)
            + "," + DAL_SQLite.SQLInt(testPoint.TestCaseId)
            + "," + DAL_SQLite.SQLText(testPoint.TestCaseTitle)
            + "," + DAL_SQLite.SQLInt(testPoint.ConfigurationId)
            + "," + DAL_SQLite.SQLText(testPoint.ConfigurationTitle)
            + "," + (testPoint.LastTestRunId <= 0 ? DAL_SQLite.SQLNull() : DAL_SQLite.SQLInt(testPoint.LastTestRunId))
            + "," + DAL_SQLite.SQLText(testPoint.State)
            + "," + DAL_SQLite.SQLText(testPoint.Outcome)
            + ")";

            int recs = _sqLite.ExecuteCommand(sql);
        }

        public void TestRunsDelete()
        {
            string sql = "DELETE FROM TestRuns";
            int recs = _sqLite.ExecuteCommand(sql);
        }

        public void TestRunInsert(TestRun testRun)
        {
            string sql = "INSERT INTO TestRuns (Id,Revision,OwnerSid,OwnerDisplayName,StartedDate,CompletedDate,State,IsAutomated)"
            + "\r\nValues"
            + "(" + DAL_SQLite.SQLInt(testRun.Id)
            + "," + DAL_SQLite.SQLInt(testRun.Revision)
            + "," + DAL_SQLite.SQLText(testRun.OwnerSid)
            + "," + DAL_SQLite.SQLText(testRun.OwnerDisplayName)
            + "," + DAL_SQLite.SQLDate (testRun.StartedDate,true)
            + "," + DAL_SQLite.SQLDate(testRun.CompletedDate, true)
            + "," + DAL_SQLite.SQLText(testRun.State)
            + "," + DAL_SQLite.SQLBool(testRun.IsAutomated)
            + ")";

            int recs = _sqLite.ExecuteCommand(sql);
        }
        private List<int> TestRunsGetIds(int? batchSize)
        {
            if ((batchSize ?? 0) < 1)
                return _sqLite.ExecuteCommandValuesInt("select Id from TestRuns order by Id");
            else
                return _sqLite.ExecuteCommandValuesInt($"select Id from TestRuns where id not in (select RunId from TestResults) order by Id LIMIT {batchSize.Value}");
        }
        public List<int> TestRunsGetIds(int? batchSize, List<int> testSuiteIds)
        {
            if (testSuiteIds == null || testSuiteIds.Count == 0)
                return TestRunsGetIds(batchSize);

            if ((batchSize ?? 0) < 1)
                return _sqLite.ExecuteCommandValuesInt($"select Id from TestRuns where Id in (select LastTestRunId from testPoints where testsuiteid in ({SqlIds(testSuiteIds)})) order by Id");
            else
                return _sqLite.ExecuteCommandValuesInt($"select Id from TestRuns where id not in (select RunId from TestResults) and id in (select LastTestRunId from testPoints where testsuiteid in ({SqlIds(testSuiteIds)})) order by Id LIMIT {batchSize.Value}");
        }

        public int TestRunsGetRemaining(List<int> testSuiteIds)
        {
            if (testSuiteIds == null || testSuiteIds.Count == 0)
                return (int)_sqLite.ExecuteCommandValue("select count(*) from TestRuns where id not in (select RunId from TestResults)");
            else
                return (int)_sqLite.ExecuteCommandValue($"select count(*) from TestRuns where id not in (select RunId from TestResults) and id in (select LastTestRunId from testPoints where testsuiteid in ({SqlIds(testSuiteIds)}))");
        }

        public void TestResultsDelete()
        {
            string sql = "DELETE FROM TestResults";
            int recs = _sqLite.ExecuteCommand(sql);
            
            sql = "DELETE FROM TestResultIterations";
            recs = _sqLite.ExecuteCommand(sql);

            sql = "DELETE FROM TestResultAssociatedWIIds";
            recs = _sqLite.ExecuteCommand(sql);
        }
        public void TestResultInsert(TestStepResult testResult)
        {
            string sql = "INSERT INTO TestResults (RunId,ResultId,StartedDate,CompletedDate,Revision,State,Outcome,TestCaseId,TestCaseRevision,TestCaseTitle,TestCaseState,TestCaseStepsXML,TestCaseDescription,TestPointId,BuildId,BuildName,RunBy)"
            + "\r\nValues"
            + "(" + DAL_SQLite.SQLInt(testResult.RunId)
            + "," + DAL_SQLite.SQLInt(testResult.ResultId)
            + "," + DAL_SQLite.SQLDate(testResult.StartedDate, true)
            + "," + DAL_SQLite.SQLDate(testResult.CompletedDate, true)
            + "," + DAL_SQLite.SQLInt(testResult.Revision)
            + "," + DAL_SQLite.SQLText(testResult.State)
            + "," + DAL_SQLite.SQLText(testResult.Outcome)
            + "," + DAL_SQLite.SQLInt(testResult.TestCaseId)
            + "," + DAL_SQLite.SQLInt(testResult.TestCaseRevision)
            + "," + DAL_SQLite.SQLText(testResult.TestCaseTitle)
            + "," + DAL_SQLite.SQLText(testResult.TestCaseState)
            + "," + DAL_SQLite.SQLText(testResult.TestCaseStepsXML)
            + "," + DAL_SQLite.SQLText(testResult.TestCaseDescription)
            + "," + DAL_SQLite.SQLInt(testResult.TestPointId)
            + "," + DAL_SQLite.SQLInt(testResult.BuildId)
            + "," + DAL_SQLite.SQLText(testResult.BuildName)
            + "," + DAL_SQLite.SQLText(testResult.RunBy)
            + ")";

            int recs = _sqLite.ExecuteCommand(sql);
            if (recs != 1)
                throw new Exception("TestResultInsert");
            testResult.DBId = _sqLite.GetLastRowID();
            if (testResult.Iterations != null)
                foreach (var iteration in testResult.Iterations)
                {
                    sql = "INSERT INTO TestResultIterations (RunId,TestResultId,IterationId,Comment,ActionResults)"
                     + "\r\nValues"
                     + "(" + DAL_SQLite.SQLInt(testResult.RunId)
                     + "," + DAL_SQLite.SQLInt(testResult.DBId)
                     + "," + DAL_SQLite.SQLInt(iteration.Id)
                     + "," + DAL_SQLite.SQLText(iteration.Comment)
                     + "," + DAL_SQLite.SQLText(iteration.ActionResultsJson)
                     + ")";
                    recs = _sqLite.ExecuteCommand(sql);
                }

            if (testResult.AssociatedWIIds != null)
                foreach(var associatedWIId in testResult.AssociatedWIIds)
                {
                    sql = "INSERT INTO TestResultAssociatedWIIds (RunId,TestResultId,WorkItemId)"
                     + "\r\nValues"
                     + "(" + DAL_SQLite.SQLInt(testResult.RunId)
                     + "," + DAL_SQLite.SQLInt(testResult.DBId)
                     + "," + DAL_SQLite.SQLInt(associatedWIId)
                     + ")";
                    recs = _sqLite.ExecuteCommand(sql);
                }

        }

        public List<TestStepResult> TestResultsRead(TestPoint testPoint, int? runId)
        {
            string sql = "SELECT tres.Id,tres.RunId,tres.ResultId,tres.StartedDate,tres.CompletedDate,tres.State,tres.Outcome,tres.TestCaseId,tres.TestCaseStepsXML,tres.TestCaseDescription,tres.BuildName,tres.RunBy,tres.TestCaseState"
                + ",tr.Id,tr.OwnerDisplayName,tr.StartedDate,tr.CompletedDate,tr.State"
                + ",(exists(select * from TestResultSharedSteps st where st.TestResultId=tres.Id)) SharedSteps"
                + ",tres.Comment"
                + "\r\nfrom TestResults tres"
                + "\r\nleft join TestRuns tr on tr.Id=tres.RunId"
                + $"\r\nWhere tres.TestPointId={testPoint.Id}" + (runId==null?"": $" and tres.RunId={runId}")
                + "\r\norder by tres.RunId desc";

            if (!_sqLite.Rdr_Open(sql))
            {
                _sqLite.ExecuteCommand("ALTER TABLE TestResults\r\nADD COLUMN Comment TEXT;");
                if (!_sqLite.Rdr_Open(sql))
                    return null;
            }

            var result = new List<TestStepResult>();
            var dbIds = new List<int>();
            try
            {
                while (_sqLite.Rdr_DataRead())
                {
                    var testResult = new TestStepResult()
                    {
                        DBId = _sqLite.Rdr_DataFieldInt(0),
                        RunId = _sqLite.Rdr_DataFieldInt(1),
                        ResultId = _sqLite.Rdr_DataFieldInt(2),
                        StartedDate = _sqLite.Rdr_DataFieldDateTime(3).Value,
                        CompletedDate = _sqLite.Rdr_DataFieldDateTime(4).Value,
                        State = _sqLite.Rdr_DataFieldStr(5),
                        Outcome = _sqLite.Rdr_DataFieldStr(6),
                        TestCaseId = _sqLite.Rdr_DataFieldInt(7),
                        TestCaseStepsXML = _sqLite.Rdr_DataFieldStr(8),
                        TestCaseDescription = _sqLite.Rdr_DataFieldStr(9),
                        BuildName = _sqLite.Rdr_DataFieldStr(10),
                        RunBy = _sqLite.Rdr_DataFieldStr(11),
                        TestCaseState = _sqLite.Rdr_DataFieldStr(12),
                        HasSharedSteps = _sqLite.Rdr_DataFieldInt(18) > 0,
                        Comment=(_sqLite.Rdr_DataFieldIsNull(19)?"": _sqLite.Rdr_DataFieldStr(19))
                    };
                    if (!dbIds.Contains(testResult.DBId))
                        dbIds.Add(testResult.DBId);

                    if (!_sqLite.Rdr_DataFieldIsNull(13))
                    {
                        testResult.TestRun = new TestRun()
                        {
                            Id = _sqLite.Rdr_DataFieldInt(13),
                            OwnerDisplayName = _sqLite.Rdr_DataFieldStr(14),
                            StartedDate = _sqLite.Rdr_DataFieldDateTime(15),
                            CompletedDate = _sqLite.Rdr_DataFieldDateTime(16),
                            State = _sqLite.Rdr_DataFieldStr(17),
                        };
                    }

                    result.Add(testResult);
                }

                _sqLite.Rdr_Close();

                if (dbIds.Count > 0)
                {
                    string dbIdsWhere = null;
                    foreach (var dbId in dbIds)
                        dbIdsWhere = (dbIdsWhere == null ? "" : dbIdsWhere + ",")
                            + dbId;

                    sql = "SELECT TestResultId,IterationId,Outcome,Comment,ActionResults,ParametersJson"
                        + "\r\nfrom TestResultIterations"
                        + $"\r\n where TestResultId in ({dbIdsWhere})";

                    if (dbIdsWhere != null && _sqLite.Rdr_Open(sql))
                    {
                        while (_sqLite.Rdr_DataRead())
                        {
                            var dbTestResultId = _sqLite.Rdr_DataFieldInt(0);
                            var testResult = result.Where(r => r.DBId == dbTestResultId).FirstOrDefault();
                            if (testResult == null)
                                continue;

                            if (testResult.Iterations == null)
                                testResult.Iterations = new List<TestResultIteration>();

                            testResult.Iterations.Add(new TestResultIteration()
                            {
                                Id = _sqLite.Rdr_DataFieldInt(1),
                                Outcome = _sqLite.Rdr_DataFieldStr(2),
                                Comment = _sqLite.Rdr_DataFieldStr(3),
                                ActionResultsJson = _sqLite.Rdr_DataFieldStr(4),
                                ParametersJson = _sqLite.Rdr_DataFieldStr(5),
                            });
                        }

                        _sqLite.Rdr_Close();
                    }

                    sql = "SELECT Id,TestResultId,FileName,Data"
                        + "\r\nfrom TestResultEquipmentRecords"
                        + $"\r\n where TestResultId in ({dbIdsWhere})";

                    if (dbIdsWhere != null && _sqLite.Rdr_Open(sql))
                    {
                        while (_sqLite.Rdr_DataRead())
                        {
                            var dbTestResultId = _sqLite.Rdr_DataFieldInt(1);
                            var testResult = result.Where(r => r.DBId == dbTestResultId).FirstOrDefault();
                            if (testResult == null)
                                continue;
                            if (testResult.EquipmentRecords == null)
                                testResult.EquipmentRecords = new List<EquipmentRecord>();
                            testResult.EquipmentRecords.Add(new EquipmentRecord()
                            {
                                Id = _sqLite.Rdr_DataFieldInt(0),
                                FileName = _sqLite.Rdr_DataFieldStr(2),
                                Data = _sqLite.Rdr_DataFieldStr(3),
                            });
                        }
                        _sqLite.Rdr_Close();
                    }

                    foreach (var testResult in result.Where(tr => tr.HasSharedSteps))
                    {
                        if (!testResult.HasSharedSteps)
                            continue;
                        sql = "SELECT SharedStepsId,SharedStepsTitle,SharedStepsXML"
                            + "\r\nFROM TestResultSharedSteps"
                            + $"\r\nWHERE TestResultId={testResult.DBId}";

                        if (_sqLite.Rdr_Open(sql))
                        {
                            testResult.SharedSteps = new List<SharedSteps>();
                            while (_sqLite.Rdr_DataRead())
                            {
                                testResult.SharedSteps.Add(new SharedSteps()
                                {
                                    Id = _sqLite.Rdr_DataFieldInt(0),
                                    Title = _sqLite.Rdr_DataFieldStr(1),
                                    SharedStepsXML = _sqLite.Rdr_DataFieldStr(2),
                                });
                            }
                        }


                    }
                }
            }

            catch { result = null; _sqLite.Rdr_Close(); }



            return result;
        }

        public List<WorkItem> TestResultAssociatedWorkItemsRead(TestStepResult testResult)
        {
            string sql = "select wiIds.WorkItemId,wi.WorkItemType,wi.Title,wi.State,wi.Reason"
                + "\r\nfrom TestResultAssociatedWIIds wiIds"
                + "\r\nleft join WorkItems wi on wi.Id = wiIds.WorkItemId"
                + $"\r\nwhere wiIds.TestResultId = {testResult.DBId}"
                 + "\r\norder by wiIds.WorkItemId";
            if (!_sqLite.Rdr_Open(sql))
                return null;

            var result = new List<WorkItem>();
            try
            {
                while (_sqLite.Rdr_DataRead())
                {
                    result.Add(new WorkItem()
                    {
                        Id = _sqLite.Rdr_DataFieldInt(0),
                        WorkItemType = _sqLite.Rdr_DataFieldStr(1),
                        Title = _sqLite.Rdr_DataFieldStr(2),
                        State = _sqLite.Rdr_DataFieldStr(3),
                        Reason = _sqLite.Rdr_DataFieldStr(4),
                    });
                }
            }
            catch { result = null; _sqLite.Rdr_Close(); }
            return result;
        }

        public List<int> WorkItemsGetIds()
        {
            var result= _sqLite.ExecuteCommandValuesInt("select distinct testCaseId from TestPoints");

            var ids2 = _sqLite.ExecuteCommandValuesInt("select distinct WorkItemId from TestResultAssociatedWIIds");
            foreach (var id2 in ids2)
                if (!result.Contains(id2))
                    result.Add(id2);

            return result;
        }

        public List<WorkItem> TestCasesRead()
        {
            string sql = "select id,WorkItemType,Title from WorkItems"
                + "\r\nWhere Id in (select testCaseId from TestPoints)"
                + "\r\norder by Id";
            if (!_sqLite.Rdr_Open(sql))
                return null;

            var result = new List<WorkItem>();
            try
            {
                while (_sqLite.Rdr_DataRead())
                {
                    result.Add(new WorkItem()
                    {
                        Id = _sqLite.Rdr_DataFieldInt(0),
                        WorkItemType = _sqLite.Rdr_DataFieldStr(1),
                        Title = _sqLite.Rdr_DataFieldStr(2),
                    });
                }
            }
            catch { result = null; _sqLite.Rdr_Close(); }
            return result;
        }
        public void WorkItemsInsert(List<WorkItem> workItems)
        {
            string sql = "DELETE FROM WorkItems";
            int recs = _sqLite.ExecuteCommand(sql);

            foreach (var workItem in workItems)
            {
                sql = "Insert into WorkItems (Id,WorkItemType,Title,State,Reason,AssignedTo)"
                    + "\r\nValues"
                    + "\r\n(" + DAL_SQLite.SQLInt(workItem.Id)
                    + "\r\n," + DAL_SQLite.SQLText(workItem.WorkItemType)
                    + "\r\n," + DAL_SQLite.SQLText(workItem.Title)
                    + "\r\n," + DAL_SQLite.SQLText(workItem.State)
                    + "\r\n," + DAL_SQLite.SQLText(workItem.Reason)
                    + "\r\n," + DAL_SQLite.SQLText(workItem.AssignedTo)
                    + ")";
                recs = _sqLite.ExecuteCommand(sql);

            }
        }
        public void StatsSave(string statType, DateTime startTime, DateTime endTime, int serviceCalls)
        {
            var duration=DateTime.Now.Subtract(startTime).ToString(@"mm\:ss\.f");
            string sql = "UPDATE Stats"
                + "\r\nSET StartTime=" + DAL_SQLite.SQLDate(startTime, true)
                + "\r\n,EndTime=" + DAL_SQLite.SQLDate(endTime, true)
                + "\r\n,ServiceCalls=" + DAL_SQLite.SQLInt(serviceCalls)
                + "\r\n,Duration=" + DAL_SQLite.SQLText(duration)
                + "\r\nWhere [Type]=" + DAL_SQLite.SQLText(statType);
            int recs = _sqLite.ExecuteCommand(sql);
            if (recs > 0)
                return;

            sql = "INSERT INTO Stats (Type,StartTime,EndTime,ServiceCalls,Duration)"
            + "\r\nValues"
            + "(" + DAL_SQLite.SQLText(statType)
                + "," + DAL_SQLite.SQLDate(startTime, true)
                + "," + DAL_SQLite.SQLDate(endTime, true)
                + "," + DAL_SQLite.SQLInt(serviceCalls)
                + "," + DAL_SQLite.SQLText(duration)
                + ")";
            recs = _sqLite.ExecuteCommand(sql);
        }
        static string SqlIds(List<int> ids)
        {
            string result = null;
            foreach (int id in ids)
                result = (result == null ? "" : result + ",") + id.ToString();
            return result;
        }
    }
}
