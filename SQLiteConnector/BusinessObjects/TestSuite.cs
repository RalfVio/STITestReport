using System;
using System.Collections.Generic;

using System.Linq;

namespace SQLite.BusinessObjects
{

    public class TestSuite:WorkItemBase
    {
        #region Properties

        public int ParentId { get; set; }
        public string SuitePath { get; set; }
        public string SuiteType { get; set; }


        #endregion

        #region Additional Properties
        public bool IsTopNode() { return ParentId < 1; }
        public List<TestPoint> TestPoints { get; set; }
        #endregion


        #region GeneralFunctions

        public static void SetSuitePath(ref List<TestSuite> testSuites)
        {
            if (testSuites.Count == 0)
                return;
            TestSuite topSuite = testSuites.Where(ts => ts.IsTopNode()).First();
            topSuite.SuitePath = "";

            foreach (TestSuite childSuite in testSuites.Where(ts => ts.ParentId == topSuite.Id))
                SetSuitePath(topSuite.SuitePath, childSuite, ref testSuites);
        }
        static void SetSuitePath(string suitePath, TestSuite testSuite, ref List<TestSuite> testSuites)
        {
            testSuite.SuitePath = (string.IsNullOrEmpty(suitePath)?"":suitePath + "/") 
                + testSuite.Title;
            foreach (TestSuite childSuite in testSuites.Where(ts => ts.ParentId == testSuite.Id))
                SetSuitePath(testSuite.SuitePath, childSuite, ref testSuites);
        }


        public override string ToString()
        {
            return String.Format("Test suite {1} ({0})", Id, Title);
        }
        #endregion

    }

}

