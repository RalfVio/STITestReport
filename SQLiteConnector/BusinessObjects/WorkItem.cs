using System;
using System.Collections.Generic;

namespace SQLite.BusinessObjects
{
    public class WorkItem:WorkItemBase 
    {
        #region Properties

        public string CRType { get; set; }
        public string Url { get; set; }
        public string AreaPath { get; set; }
        public string IterationPath { get; set; }
        public double? EstimatedEffort { get; set; }
        public string VerificationOwner { get; set; }
        public DateTime CreatedDate { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? FinishDate { get; set; }

        #endregion

        #region Additional Properties

        #endregion

        #region GeneralFunctions
        #endregion
    }
    public class TestPlan : WorkItemBase
    {
        #region Properties


        #endregion

        #region Additional Properties

        #endregion

        #region GeneralFunctions
        #endregion
    }
    public abstract class WorkItemBase
    {
        #region Properties

        public int Id { set; get; }
        public string WorkItemType { get; set; }
        public int Rev { get; set; }
        public string State { get; set; }
        public string Reason { get; set; }

        public string Title { get; set; }
        public string AssignedTo { get; set; }

        #endregion

        #region Additional Properties

        #endregion

        #region GeneralFunctions

        public override string ToString() { return String.Format("{0} {1}:{2}", WorkItemType, Id, Title); }
        #endregion
    }
}
