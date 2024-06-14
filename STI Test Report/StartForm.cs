using Newtonsoft.Json;
using SQLite.BusinessObjects;
using System.Windows.Forms;

namespace STI_Test_Report
{
    public partial class StartForm : Form
    {
        bool _active = false;
        ADORest.RestTestPlan _adoRest = null;
        List<TestPlan> _testPlans = null;
        TestPlan _testPlan = null;
        TeamProjects _teamProjects = null;
        string _reportFileName = null;
        public StartForm()
        {
            InitializeComponent();
            PrintControls(false, false);
        }

        #region Events

        #region Form
        private async void StartForm_Shown(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.status_toolStripStatusLabel.Text = "Reading test plans";
            Application.DoEvents();

            _teamProjects = TeamProjects.GetTeamProjects();
            if (_teamProjects == null)
            {
                if (!File.Exists(Program.GetTeamProjectsFilePath()))
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
                        Tag = (i + 1).ToString(),
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
            this.testPlan_textBox.Focus();
            this.Cursor = Cursors.Default;
            _active = true;
            this.testPlan_textBox.Text = Program.UserSettings.TestPlanFilter ?? "";
            if (this.testPlan_textBox.Text != "")
                this.testPlan_comboBox.DroppedDown = true;
        }
        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_adoRest != null)
                _adoRest.Dispose();
            Program.UserSettingsSave();
        }
        #endregion

        #region Menu
        private async void file_TeamProject_toolStripMenuItem_Click(object sender, EventArgs e)
        {
            _active = false;
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            if (!int.TryParse(((ToolStripItem)sender).Tag as string, out int id))
            { _active = true; this.Cursor = Cursors.Default; return; }

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
            if (!_active)
                return;

            var testPlanFilter = ((TextBox)sender);

            _active = false;
            testPlan_comboBox.DataSource = null;
            if (_testPlans != null)
            {
                if (testPlanFilter.Text == "")
                    this.testPlan_comboBox.DataSource = _testPlans;
                else
                    this.testPlan_comboBox.DataSource = _testPlans.Where(tp => tp.Title.Contains(testPlanFilter.Text, StringComparison.CurrentCultureIgnoreCase) || tp.Id.ToString() == testPlanFilter.Text).ToList();
            }
            testPlan_comboBox.DisplayMember = "TitleId";
            testPlan_comboBox.ValueMember = "Id";
            this.testPlan_comboBox.SelectedIndex = -1;
            Program.UserSettings.TestPlanFilter = this.testPlan_textBox.Text;
            _active = true;
        }
        private void testPlan_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_active)
                return;
            _testPlan = ((ComboBox)sender).SelectedItem as TestPlan;
            if (_testPlan == null)
            {
                PrintControls(false, false);
                return;
            }

            string filePath = DBFilePath();
            PrintControls(true, System.IO.File.Exists(filePath));
        }

        private async void print_button_Click(object sender, EventArgs e)
        {
            var testPlanWorkItem = this.testPlan_comboBox.SelectedItem as TestPlan;
            if (testPlanWorkItem == null)
                return;

            string dbFilePath = DBFilePath();
            if (!System.IO.File.Exists(dbFilePath))
                return;

            pdf_SaveFileDialog.FileName = $"TestReport_{testPlanWorkItem.Id}.pdf";
            if (pdf_SaveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            ReportOptionsForm.ReportOptions reportOptions = null;
            using (var f = new ReportOptionsForm())
            {
                if (f.ShowDialog() == DialogResult.OK)
                    reportOptions = f.GetReportOptions();
            }
            if (reportOptions == null)
                return;

            _reportFileName = pdf_SaveFileDialog.FileName;
            string logFilePath = Path.GetDirectoryName(_reportFileName) + "\\" + Path.GetFileNameWithoutExtension(_reportFileName) + ".txt";

            try
            {
                if (File.Exists(_reportFileName))
                    File.Delete(_reportFileName);
                if (File.Exists(logFilePath))
                    File.Delete(logFilePath);
            }
            catch { this.status_toolStripStatusLabel.Text = "File is locked."; return; }

            this.status_toolStripStatusLabel.Text = "Creating file";
            this.open_linkLabel.Visible = false;

            this.Cursor = Cursors.WaitCursor;

            await Task.Run(() => CreateReport(_reportFileName, logFilePath, dbFilePath, reportOptions));

            this.open_linkLabel.Visible = File.Exists(_reportFileName);
            this.status_toolStripStatusLabel.Text = "Done";
            this.Cursor = Cursors.Default;
        }

        private void open_linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(_reportFileName) || !File.Exists(_reportFileName))
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
        private async void Read_button_Click(object sender, EventArgs e)
        {
            var testPlanWorkItem = this.testPlan_comboBox.SelectedItem as TestPlan;
            if (testPlanWorkItem == null)
                return;

            ReadReportData.Parameters parameters = null;
            using (var f = new ReadForm())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    parameters = f.GetReadParameters();
                }
            }

            if (parameters == null)
                return;
            parameters.TestPlanId = testPlanWorkItem.Id;

            for (int i = 0; i < Math.Max(1, parameters.Repeats); i++)
                await ReadReportData(parameters);
        }
        #endregion

        SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        async Task ReadReportData(ReadReportData.Parameters parameters)
        {
            await _semaphore.WaitAsync();

            this.Cursor = Cursors.WaitCursor;
            _logWriter = new StreamWriter(GetFreeLogFilePath(), false, System.Text.Encoding.UTF8);
            _logWriter.WriteLine();
            _logWriter.WriteLine(parameters.LogHeader());
            _logWriter.WriteLine(""); _logWriter.WriteLine(new string('-', 80));
            var teamProject = _teamProjects.GetProject();
            if (_adoRest == null)
                _adoRest = new ADORest.RestTestPlan(teamProject);

            ReadReportData readData = null;
            try
            {
                readData = new ReadReportData();
                readData.WriteLog += WriteStatus;
                readData.WriteException += WriteException;

                await Task.Run(() => readData.Start(parameters, _adoRest, teamProject, DBFilePath()));
                //await readData.Start(parameters, _adoRest, teamProject, DBFilePath());
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
            finally
            {
                readData.WriteLog -= WriteStatus;
                readData.WriteException -= WriteException;
                _logWriter.WriteLine(new string('-', 80));
                _logWriter.Flush(); _logWriter.Dispose(); _logWriter = null;
                this.Cursor = Cursors.Default;
                this.print_button.Enabled = true;
                _semaphore.Release();
            }
        }

        #region Functions
        string DBFilePath() => Program.GetApplicationDataFolder($"{_teamProjects.CurrentProject}_{_testPlan.Id}.db");
        void PrintControls(bool enable, bool dbExists)
        {
            this.print_button.Enabled = enable && dbExists;
            this.read_button.Enabled = enable;
            if (!enable)
                this.open_linkLabel.Visible = false;
        }
        void CreateReport(string pdfFilePath, string logFilePath, string dbFilePath, ReportOptionsForm.ReportOptions reportOptions)
        {
            var sqlLiteBL = new SQLiteConnector.BL();
            sqlLiteBL.OpenDatabase(dbFilePath);
            var testPlan = sqlLiteBL.TestPlanRead();
            var pdfReport = new OutPdf.PdfSTITestReport();

            Dictionary<int, OutPdf.PdfSTITestReport.TestPointStatus> testPointStati = new();

            pdfReport.PrintStart(testPlan, 1, true);

            var testSuites = sqlLiteBL.TestSuitesRead(reportOptions.TestSuiteIds);

            foreach (var testSuite in testSuites)
            {
                bool isNewTestSuite = true;
                foreach (var testPoint in testSuite.TestPoints)
                {
                    //if (!(testCaseIds == null || testCaseIds.Contains(testPoint.TestCaseId)))
                    //    continue;

                    if (testPoint.LastTestRunId <= 0)
                    {
                        if (reportOptions.NotRun)
                            continue;
                    }
                    else
                        switch (testPoint.Outcome)
                        {
                            case "NotApplicable":
                                if (reportOptions.NotApplicable)
                                    continue;
                                else
                                    break;
                        }

                    if (isNewTestSuite)
                    { pdfReport.Print(testSuite); isNewTestSuite = false; }

                    pdfReport.Print(testPoint);
                    if (reportOptions.NoTestResults)
                        continue;

                    testPointStati.Add(testPoint.Id, new OutPdf.PdfSTITestReport.TestPointStatus() { 
                        TestCaseId = testPoint.TestCaseId,
                        TestCaseTitle=testPoint.TestCaseTitle,
                        SuitePath = testSuite.SuitePath,
                        PageNumber = pdfReport.GetPageNumber(),
                        NotRun = testPoint.LastTestRunId <= 0 
                    });

                    var testResults = sqlLiteBL.TestResultsRead(testPoint, reportOptions.LastTestRun ? testPoint.LastTestRunId : (int?)null);
                    foreach (var testResult in testResults)
                    {
                        pdfReport.Print(testResult, null);

                        testPointStati[testPoint.Id].RunId = testResult.RunId;
                        if (testResult.TestCaseState != "Approved")
                            testPointStati[testPoint.Id].TestCaseNotApproved = true;

                        if (!reportOptions.NoTestSteps)
                        {
                            var steps = testResult.GetTestCaseSteps();
                            if (steps == null || steps.Count == 0 || testResult.Iterations == null)
                                testPointStati[testPoint.Id].NoTestSteps = true;
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
                                                if (!step.IsPassedOrFailed())
                                                    testPointStati[testPoint.Id].NoTestSteps = true;

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
                                                        if (!sharedStepsStep.IsPassedOrFailed())
                                                            testPointStati[testPoint.Id].NoTestSteps = true;
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

            using (var fs = new FileStream(pdfFilePath, FileMode.Create))
            {
                stream.CopyTo(fs);
                fs.Flush();
                fs.Close();
            }

            if (reportOptions.WriteLogFile)
                using (var sw = new StreamWriter(logFilePath, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine($"Test Plan:\t{testPlan.TitleId}\r\n");
                    sw.WriteLine("Total test points:");
                    sw.WriteLine("Not applicable::");

                    int total = testPointStati.Count();
                    sw.WriteLine($"Selected test points:\t{total}");
                    int count = testPointStati.Count(tp => tp.Value.TestIsOk() && !tp.Value.NotRun);
                    sw.WriteLine($"Correct:\t{count} {((double)count / total):P1}");
                    count = testPointStati.Count(tp => !tp.Value.TestIsOk() && !tp.Value.NotRun);
                    sw.WriteLine($"Incorrect:\t{count} {((double)count / total):P1}");
                    count = testPointStati.Count(tp => tp.Value.TestCaseNotApproved && !tp.Value.NotRun);
                    sw.WriteLine($"  TC not approved:\t{count} {((double)count / total):P1}");
                    count = testPointStati.Count(tp => tp.Value.NoTestSteps && !tp.Value.NotRun);
                    sw.WriteLine($"  No step details:\t{count} {((double)count / total):P1}");
                    count = testPointStati.Count(tp => tp.Value.NotRun);
                    sw.WriteLine($"Not run:\t{count} {((double)count / total):P1}");

                    sw.WriteLine($"\r\n\r\nErrors:");
                    sw.WriteLine($"\r\nRun Id\tTest Case Id\tTitle\tSuite\tPage\tError");
                    foreach (var id in testPointStati.Keys)
                    {
                        var testPointStatus = testPointStati[id];
                        if (!testPointStatus.TestIsOk())
                            sw.WriteLine($"{(testPointStatus.RunId<=0?"":testPointStatus.RunId.ToString())}\t{testPointStatus.TestCaseId}\t{testPointStatus.TestCaseTitle}\t{testPointStatus.SuitePath}\t{testPointStatus.PageNumber}\t{testPointStatus.ErrorMessage()}");
                    }
                }
        }


        static async Task<List<TestPlan>> GetTestPlans(ADORest.RestTestPlan adoRest)
        {
            List<TestPlan> result = null;
            result = await adoRest.GetTestPlans();
            return result.OrderBy(tp => tp.Title).ToList();
        }

        #endregion

        #region Logging
       
        private void WriteStatus(object sender, LogEventArgs e)
        {
            this.status_toolStripStatusLabel.Text = e.Message;
            WriteLog(e.Message);
        }
        private void WriteException(object sender, LogExeptionArgs e)
        {
            this.status_toolStripStatusLabel.Text = $"Error: {e.Exception.Message}";
            WriteLog(e.Exception);
        }

        int _logMsgNo = 0;
        StreamWriter _logWriter = null;
        private void WriteLog(string msg) { _logMsgNo++; WriteLog(msg, (_logMsgNo % 10) == 0); }
        private void WriteLog(string msg, bool flush)
        {
            if (_logWriter == null)
                return;
            _logWriter.WriteLine(string.Format("{0:MM/dd/yyyy HH:mm:ss.ff}\t{1}", DateTime.Now, msg));
            if (flush) { _logWriter.Flush(); }
        }
        private void WriteLog(Exception exception)
        {
            if (_logWriter == null)
                return;
            _logWriter.WriteLine(string.Format("{0:MM/dd/yyyy HH:mm:ss.ff}\tException: {1}", DateTime.Now, exception.Message));
            _logWriter.Flush();
        }
        private string GetFreeLogFilePath()
        {
            string filePath = null;
            bool fileIsAvailable = true;
            int i = 0;
            do
            {
                filePath = Path.Combine(Program.GetLogFolder(), $"{_teamProjects.CurrentProject}_{_testPlan.Id}_{(i + 1):#00}.txt");
                fileIsAvailable = !File.Exists(filePath);

                i++;
            }
            while (!fileIsAvailable);

            return filePath;
        }

        #endregion
    }
}