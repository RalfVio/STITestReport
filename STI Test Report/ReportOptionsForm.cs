namespace STI_Test_Report
{
    public partial class ReportOptionsForm : Form
    {
        ReportOptions _reportOptions = null;
        public ReportOptionsForm()
        {
            InitializeComponent();
        }
        private void ReportOptionsForm_Shown(object sender, EventArgs e)
        {
            this.notRun_checkBox.Checked = Program.UserSettings.OptionNotRun;
            this.notApplicable_checkBox.Checked = Program.UserSettings.OptionNotApplicable;
            this.lastTestRun_checkBox.Checked = Program.UserSettings.OptionLastTestRun;
            this.noTestResults_checkBox.Checked = Program.UserSettings.OptionNoTestResults;
            this.noTestSteps_checkBox.Checked = Program.UserSettings.OptionNoTestSteps;
            this.testSuite_TextBox.Text = Program.UserSettings.GetTestSuiteIds() ?? "";
            this.writeLogFile_checkBox.Checked = Program.UserSettings.WriteLogFile;
        }

        public class ReportOptions
        {
            public bool NotRun { get; set; }
            public bool NotApplicable { get; set; }
            public bool LastTestRun { get; set; }
            public bool NoTestResults { get; set; }
            public bool NoTestSteps { get; set; }
            public bool WriteLogFile { get; set; }
            public List<int> TestSuiteIds { get; set; }

        }
        public ReportOptions GetReportOptions() => _reportOptions;

        private void Create_Button_Click(object sender, EventArgs e)
        {
            var testSuiteIds = new List<int>();
            if (this.testSuite_TextBox.Text.Trim() != "")
            {
                var tss = this.testSuite_TextBox.Text.Trim().Split(',');
                foreach (var ts in tss)
                {
                    if (!int.TryParse(ts.Trim(), out int testSuiteId))
                        return;
                    testSuiteIds.Add(testSuiteId);
                }
            }

            _reportOptions = new ReportOptions()
            {
                NotRun = this.notRun_checkBox.Checked,
                NotApplicable = this.notApplicable_checkBox.Checked,
                LastTestRun = this.lastTestRun_checkBox.Checked,
                NoTestResults = this.noTestResults_checkBox.Checked,
                NoTestSteps = this.noTestSteps_checkBox.Checked,
                TestSuiteIds = testSuiteIds,
                WriteLogFile = this.writeLogFile_checkBox.Checked,
            };

            Program.UserSettings.OptionNotRun = _reportOptions.NotRun;
            Program.UserSettings.OptionNotApplicable = _reportOptions.NotApplicable;
            Program.UserSettings.OptionLastTestRun = _reportOptions.LastTestRun;
            Program.UserSettings.OptionNoTestResults = _reportOptions.NoTestResults;
            Program.UserSettings.OptionNoTestSteps = _reportOptions.NoTestSteps;
            Program.UserSettings.TestSuiteIds = _reportOptions.TestSuiteIds;
            Program.UserSettings.WriteLogFile = _reportOptions.WriteLogFile;

            DialogResult = DialogResult.OK;
        }
    }
}
