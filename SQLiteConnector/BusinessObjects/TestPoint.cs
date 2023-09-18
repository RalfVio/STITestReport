using System;
using System.Collections.Generic;

namespace SQLite.BusinessObjects
{
    public class TestPoint
    {
        #region Properties
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int SuiteId { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToSid { get; set; }
        public int AssignedToUserId { get; set;}
        public string AssignedToAccountName { get; set; }
        public string Outcome { get; set; }
        public int LastTestRunId { get; set; }
        public string State { get; set; }
        public string StateOutcome { get { return Outcome == "Unspecified" ? "Active" : Outcome; } }
        public int TestCaseId { get; set; }
        public string TestCaseTitle { get; set; }
        public string TestCase { get { return string.Format("{0} {1}", TestCaseId, TestCaseTitle); } }
        public int ConfigurationId { get; set; }
        public string ConfigurationTitle { get; set; }
        #endregion

  
        #region GeneralFunctions
        public override string ToString()
        {
            return String.Format("Test point: {0}-{1}", TestCaseId, ConfigurationTitle);
        }
        #endregion

    }
}
