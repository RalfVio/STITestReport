using ADORest.BusinessObjects;
using Newtonsoft.Json;

namespace STI_Test_Report
{
    public partial class StartForm : Form
    {
        bool _active = false;
        ADORest.RestTestPlan _adoRest = null;
        List<TestPlan> _testPlans = null;
        TeamProjects _teamProjects = null;
        string _reportFileName = null;
        public StartForm()
        {
            InitializeComponent();
            PrintControls(false);
        }

        #region Events

        #region Form
        private async void StartForm_Shown(object sender, EventArgs e)
        {
            this.Cursor= Cursors.WaitCursor;
            this.status_toolStripStatusLabel.Text = "Reading test plans";
            Application.DoEvents();

            _teamProjects = GetTeamProjects();
            if (_teamProjects == null)
            { 
                if(!File.Exists(Program.GetTeamProjectsFilePath()))
                {
                    _teamProjects = TeamProjects.CreateExample();

                    using (StreamWriter file = File.CreateText(Program.GetTeamProjectsFilePath()))
                    {
                        var serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                        serializer.Serialize(file, _teamProjects);
                    }

                }
            }
            
            if (_teamProjects != null)
            {
                for (int i = 0; i < _teamProjects.Projects.Count; i++)
                {
                    var menuItem = new ToolStripMenuItem()
                    {
                        Name = $"fileTeamProject{i}ToolStripMenuItem",
                        Text = _teamProjects.Projects[i].Title,
                        Tag = (i+1).ToString(),
                    };
                    this.file_TeamProjects_toolStripMenuItem.DropDownItems.Add(menuItem);
                    menuItem.Click += file_TeamProject_toolStripMenuItem_Click;
                }
               
            }

            var teamProject = _teamProjects.GetProject();
            teamProject_toolStripStatusLabel.Text = teamProject.Title;
            try
            {
                _adoRest = new ADORest.RestTestPlan(teamProject);
                _testPlans = await GetTestPlans(_adoRest);
                this.testPlan_comboBox.DataSource = _testPlans;
                this.testPlan_comboBox.SelectedIndex = -1;
                testPlans_groupBox.Enabled = _testPlans != null && _testPlans.Count > 0;
            }
            catch { }

            this.status_toolStripStatusLabel.Text = "";
            this.Cursor = Cursors.Default;
            _active = true;
        }
        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_adoRest != null)
                _adoRest.Dispose();
        }
        #endregion

        #region Menu
        private async void file_TeamProject_toolStripMenuItem_Click(object sender, EventArgs e)
        {
            _active = false;
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            if(!int.TryParse (((ToolStripItem)sender).Tag as string,out int id))
            {_active = true; this.Cursor = Cursors.Default; return; }

            _teamProjects.CurrentProject = id;
            var teamProject = _teamProjects.GetProject();
            teamProject_toolStripStatusLabel.Text = teamProject.Title;
            try
            {
                _adoRest = new ADORest.RestTestPlan(teamProject);
                _testPlans = await GetTestPlans(_adoRest);
                this.testPlan_comboBox.DataSource = null;
                this.testPlan_comboBox.DataSource = _testPlans;
                this.testPlan_comboBox.DisplayMember = "TitleId";
                this.testPlan_comboBox.SelectedIndex = -1;
                testPlans_groupBox.Enabled = _testPlans != null && _testPlans.Count > 0;
            }
            catch { }

            this.Cursor = Cursors.Default;
            _active = true;
        }
        private void file_Restart_toolStripMenuItem_Click(object sender, EventArgs e) => Application.Restart();
        private void FileExit_StripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void HelpAbout_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var f = new AboutBoxForm())
                f.ShowDialog();
        }
        #endregion
        private void testPlan_textBox_TextChanged(object sender, EventArgs e)
        {
            var testPlanFilter = ((TextBox)sender);

            _active = false;
            testPlan_comboBox.DataSource = null;
            if (testPlanFilter.Text == "")
                this.testPlan_comboBox.DataSource = _testPlans;
            else
                this.testPlan_comboBox.DataSource = _testPlans.Where(tp => tp.Title.Contains(testPlanFilter.Text, StringComparison.CurrentCultureIgnoreCase) || tp.Id.ToString() == testPlanFilter.Text).ToList();
            testPlan_comboBox.DisplayMember = "TitleId";
            testPlan_comboBox.ValueMember = "Id";
            this.testPlan_comboBox.SelectedIndex = -1;
            _active = true;
        }
        private void testPlan_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_active)
                return;
            var testPlan=((ComboBox)sender).SelectedItem as TestPlan;
            if (testPlan == null)
                return;

            string filePath = DBFilePath(testPlan.Id);
            PrintControls(System.IO.File.Exists(filePath));
        }

        #endregion

        private async void print_button_Click(object sender, EventArgs e)
        {
            var testPlanWorkItem = this.testPlan_comboBox.SelectedItem as TestPlan;
            if (testPlanWorkItem == null)
                return;

            var testSuiteIds = new List<int>();
            if (this.testSuite_TextBox.Text.Trim() != null)
            {
                var tss = this.testSuite_TextBox.Text.Trim().Split(',');
                foreach (var ts in tss)
                {
                    if (!int.TryParse(ts, out int testSuiteId))
                        return;
                    testSuiteIds.Add(testSuiteId);
                }
            }

            string dbFilePath = DBFilePath(testPlanWorkItem.Id);
            if (!System.IO.File.Exists(dbFilePath))
                return;

            pdf_SaveFileDialog.FileName = $"TestReport_{testPlanWorkItem.Id}.pdf";
            if (pdf_SaveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            bool optionNotRun = notRun_checkBox.Checked;
            bool optionNotApplicable = this.notApplicable_checkBox2.Checked;
            bool optionLastTestRun = this.lastTestRun_checkBox.Checked;
            bool optionNoTestResults = this.noTestResults_checkBox4.Checked;
            bool optionNoTestSteps=this.noTestSteps_checkBox.Checked;

            _reportFileName = pdf_SaveFileDialog.FileName;
            try
            {
                if (File.Exists(_reportFileName))
                    File.Delete(_reportFileName);
            }
            catch { this.status_toolStripStatusLabel.Text = "File is locked.";return;}

            this.status_toolStripStatusLabel.Text = "Creating file";
            this.open_linkLabel.Visible = false;

            this.Cursor = Cursors.WaitCursor;

            await Task.Run(() => CreateReport(_reportFileName, dbFilePath, optionNotRun, optionNotApplicable, optionLastTestRun, optionNoTestResults, optionNoTestSteps, testSuiteIds));
            
            this.open_linkLabel.Visible = File.Exists(_reportFileName);
            this.status_toolStripStatusLabel.Text = "Done";
            this.Cursor = Cursors.Default;
        }
        
        private void open_linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(string.IsNullOrEmpty(_reportFileName) || !File.Exists(_reportFileName))
                return;

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = _reportFileName,
                UseShellExecute = true,
            });
        }

   
        private void dataOpenFolder_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Program.GetApplicationDataFolder(),
                UseShellExecute = true,
            }); ;
        }

        #region Functions
        string DBFilePath(int testPlanId) => Program.GetApplicationDataFolder($"{_teamProjects.CurrentProject}_{testPlanId}.db");
        void PrintControls(bool enable)
        {
            notRun_checkBox.Enabled = enable;
            noTestSteps_checkBox.Enabled = enable;
            noTestResults_checkBox4.Enabled = enable;
            lastTestRun_checkBox.Enabled = enable;
            notApplicable_checkBox2.Enabled = enable;
            testSuite_TextBox.Enabled = enable;
            this.print_button.Enabled = enable;
            if (!enable)
                this.open_linkLabel.Visible = false;
        }
        void CreateReport(string pdfFilePath, string dbFilePath, bool optionNotRun, bool optionNotApplicable, bool optionLastTestRun, bool optionNoTestResults, bool optionNoTestSteps, List<int> testSuiteIds)
        {
            var sqlLiteBL = new SQLiteConnector.BL();
            sqlLiteBL.OpenDatabase(dbFilePath);
            var testPlan = sqlLiteBL.TestPlanRead();
            var pdfReport = new OutPdf.PdfSTITestReport();

            pdfReport.PrintStart(testPlan, 1, true);

            var testSuites = sqlLiteBL.TestSuitesRead(testSuiteIds);

            foreach (var testSuite in testSuites)
            {
                bool isNewTestSuite = true;
                foreach (var testPoint in testSuite.TestPoints)
                {
                    //if (!(testCaseIds == null || testCaseIds.Contains(testPoint.TestCaseId)))
                    //    continue;

                    if (testPoint.LastTestRunId <= 0)
                    {
                        if (optionNotRun)
                            continue;
                    }
                    else
                        switch (testPoint.Outcome)
                        {
                            case "NotApplicable":
                                if (optionNotApplicable)
                                    continue;
                                else
                                    break;
                        }

                    if (isNewTestSuite)
                    { pdfReport.Print(testSuite); isNewTestSuite = false; }

                    pdfReport.Print(testPoint);
                    if (optionNoTestResults)
                        continue;

                    var testResults = sqlLiteBL.TestResultsRead(testPoint, optionLastTestRun ? testPoint.LastTestRunId : (int?)null);
                    foreach (var testResult in testResults)
                    {
                        pdfReport.Print(testResult, null);
                        if (!optionNoTestSteps)
                        {
                            var steps = testResult.GetTestCaseSteps();
                            if (testResult.Iterations != null)
                                foreach (var iteration in testResult.Iterations)
                                {
                                    var actionResults = iteration.GetActionResults();
                                    var parameterResults = iteration.GetParameterResults();
                                    foreach (var step in steps)
                                    {
                                        switch (step.TypeOfStep)
                                        {
                                            case SQLite.BusinessObjects.Step.TypesOfStep.TestStep:
                                                var actionResult = actionResults.Where(ar => ar.StepIdentifier == step.GetStepIdentifier()).FirstOrDefault();
                                                step.Outcome = (actionResult == null ? "" : actionResult.Outcome);
                                                step.Comment = (actionResult == null ? "" : actionResult.ErrorMessage);

                                                if (parameterResults != null)
                                                    step.Parameters = parameterResults.Where(ar => ar.StepIdentifier == step.GetStepIdentifier()).ToList();
                                                pdfReport.Print(step);
                                                break;
                                            case SQLite.BusinessObjects.Step.TypesOfStep.SharedStepsRef:
                                                SQLite.BusinessObjects.SharedSteps sharedSteps = null;
                                                if (testResult.SharedSteps != null && (sharedSteps = testResult.SharedSteps.Where(sh => sh.Id == step.SharedStepsId).FirstOrDefault()) != null)
                                                {
                                                    step.Instructions = $"Shared Steps '{sharedSteps.Title}' ({sharedSteps.Id})";
                                                    pdfReport.Print(step);

                                                    var sharedStepsSteps = sharedSteps.GetSteps(step);

                                                    foreach (var sharedStepsStep in sharedStepsSteps)
                                                    {
                                                        var sharedStepsActionResult = actionResults.Where(ar => ar.StepIdentifier == sharedStepsStep.GetStepIdentifier()).FirstOrDefault();
                                                        sharedStepsStep.Outcome = (sharedStepsActionResult == null ? "" : sharedStepsActionResult.Outcome);
                                                        sharedStepsStep.Comment = (sharedStepsActionResult == null ? "" : sharedStepsActionResult.ErrorMessage);
                                                        pdfReport.Print(sharedStepsStep);
                                                    }
                                                }
                                                break;

                                        }
                                    }
                                }
                        }

                        var workItems = sqlLiteBL.TestResultAssociatedWorkItemsRead(testResult);
                        if (workItems != null)
                            foreach (var workItem in workItems)
                                pdfReport.Print(workItem);
                    }
                }
            }
            System.IO.MemoryStream stream = pdfReport.PrintEnd();

            sqlLiteBL.Dispose();

            if (stream == null)
                return;
            stream.Position = 0;
            stream.CopyTo(new FileStream(pdfFilePath, FileMode.Create));
        }

        public class TeamProjects
        {
            public TeamProjects() { Projects = new List<TeamProject>(); }
            public int CurrentProject { get; set; }
            public List<TeamProject> Projects { get; set; }
            public static TeamProjects CreateExample()
            {
                var result = new TeamProjects() { CurrentProject = 1 };
                result.Projects.Add(new TeamProject()
                {
                    Title = "Sample",
                    OrganizationUrl = "https://dev.azure.com/<your org>",
                    TeamProjectName = "<your team project>",
                    PAT = "<your personal access token>"
                });

                return result;
            }
            public TeamProject GetProject()
            {
                if (CurrentProject > 0 && CurrentProject <= Projects.Count)
                    return Projects[CurrentProject - 1];
                else
                {
                    this.CurrentProject = 1;
                    return this.Projects.FirstOrDefault();
                }
            }
        }
        TeamProjects GetTeamProjects()
        {
            string filePath = Program.GetTeamProjectsFilePath();
            if (!File.Exists(filePath))
                return null;

            return JsonConvert.DeserializeObject<TeamProjects>(File.ReadAllText(filePath));
        }
        static async Task<List<TestPlan>> GetTestPlans(ADORest.RestTestPlan adoRest)
        {
            List<TestPlan> result = null;
            result = await adoRest.GetTestPlans();
            return result.OrderBy(tp=>tp.Title).ToList();
        }

        #endregion

       
    }
}