using Newtonsoft.Json;

namespace STI_Test_Report
{
    public class UserSettings
    {
        public string TestPlanFilter { get; set; }
        public int BatchSize { get; set; }
        public bool OptionNotRun { get; set; }
        public bool OptionNotApplicable { get; set; }
        public bool OptionLastTestRun { get; set; }
        public bool OptionNoTestResults { get; set; }
        public bool OptionNoTestSteps { get; set; }

        public List<int> TestSuiteIds { get; set; }
        public bool WriteLogFile { get; set; }

        public string GetTestSuiteIds()
        {
            if (TestSuiteIds == null || TestSuiteIds.Count == 0)
                return "";
            return ADORest.RestBase.SqlIds(TestSuiteIds);
        }
        public static UserSettings ReadUserSettings(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(filePath));
        }
        public void WriteUserSettings(string filePath)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, this);
            }
        }
    }

}
