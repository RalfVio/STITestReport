using System;
using System.Collections.Generic;

namespace SQLite.BusinessObjects
{

    public class TestRun
    {

        #region Properties

        public int Id { get; set;}
        public int Revision { get; set; }
        public int PlanId { get; set; }
        public string Title { get; set; }
        public string OwnerDisplayName { get; set; }
        public string OwnerSid { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string State { get; set; }
        public bool IsAutomated { get; set; }
        public int BuildId { get; set; }

        #endregion

        #region Additional Properties
        public List<TestStepResult> TestStepResults { get; set; }
        public string OwnerWithoutAD() { return OwnerDisplayName; }
        #endregion


        #region GeneralFunctions

        public override string ToString()
        {
            return String.Format("Test run {0}:{1}", Id, Title);
        }
        #endregion

    }

}
