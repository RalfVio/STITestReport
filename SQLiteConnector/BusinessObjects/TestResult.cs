using System;
using System.Collections.Generic;

namespace SQLite.BusinessObjects
{

    public class TestResult
    {
        public TestResult() { this.AssociatedBugs = new List<TestResultAssociatedBug>(); }
        #region Properties
        public int Id { get; set; }
        public int TestRunId { get; set; }
        public int ResultId { get; set; }
        public string Owner { get; set; }
        public string OwnerSid { get; set; }
        public int OwnerUserId { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Outcome { get; set; }
        public string State { get; set; }
        public string StateOutcome { get { return State == "Completed" ? Outcome : string.Format("{0} {1}", State, Outcome); } }
        public string RunBy { get; set; }
        public string RunBySid { get; set; }
        public int RunByUserId { get; set; }
        public int Revision { get; set; }
        public int TestCaseId { get; set; }
        public string TestCaseTitle { get; set; }
        public int TestCaseRevision { get; set; }
        public string TestCaseState { get; set; }
        public string TestCase { get { return string.Format("{0} {1}", TestCaseId, TestCaseTitle); } }
        public int ConfigurationId { get; set; }
        public string ConfigurationTitle { get; set; }
        public List<TestResultAssociatedBug> AssociatedBugs { get; set; }
        public string Build { get; set; }

        public string IterationComment { get; set; }



        #endregion

        #region Additional Properties
        public double? EstimatedEffort { get; set; }

        static System.Text.RegularExpressions.Regex _durationPattern = null;
        public double? TestDuration
        {
            get
            {
                if (_durationPattern == null)
                    _durationPattern = new System.Text.RegularExpressions.Regex(@"(\d{1,3}(\.\d{1,2})? ?h)|(\d{1,3} ?min)");

                if (string.IsNullOrWhiteSpace(IterationComment)) return null;
                System.Text.RegularExpressions.Match match = _durationPattern.Match(IterationComment);
                if (!match.Success)
                    return null;
                string durationString = IterationComment.Substring(match.Index, match.Length);

                // include decimal separator before the match string (.5h => match string 5h)
                if (match.Index > 0 && IterationComment.Substring(match.Index - 1, 1) == "." && durationString.Contains("h"))
                    durationString = IterationComment.Substring(match.Index - 1, match.Length + 1);

                double result = 0;
                if (durationString.Contains("h"))
                {
                    if (double.TryParse(durationString.Substring(0, durationString.IndexOf('h')).TrimEnd(), out result))
                        return result;
                }
                if (durationString.Contains("min"))
                {
                    if (double.TryParse(durationString.Substring(0, durationString.IndexOf("min")).TrimEnd(), out result))
                        return result / 60;
                }
                return null;
            }
        }

        public string OutcomeExternal() { return Outcome; }

        #endregion

        #region GeneralFunctions
        public bool IsError() => false;

        public override string ToString()
        {
            return String.Format("Test result {0}:{1} {2}", TestRunId, TestCaseId, TestCaseTitle);
        }
        #endregion

    }

    public class TestResultDetail : TestResult
    {
        const string _className = "TestResultDetail";

        public int TestPointId { get; set; }
        public int TestSuiteId { get; set; }
        public string TestSuiteName { get; set; }

    }

    public class TestResultAssociatedBug
    {

        #region Properties
        int _id = 0;
        public int Id { get { return _id; } }
        public int WorkItemId { get; set; }
        public int TestCaseId { get; set; }
        #endregion


    }
     
}
