using ADORest.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STI_Test_Report
{
    public partial class ReadForm : Form
    {
        ReadReportData.Parameters _parameters = null;

        public ReadForm()
        {
            InitializeComponent();
            this.readTestPoints_radioButton.Checked = false;
            this.testSuites_TextBox.Text = Program.UserSettings.GetTestSuiteIds();
            if (Program.UserSettings.BatchSize > 0)
                this.batchSize_TextBox.Text = Program.UserSettings.BatchSize.ToString();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
                this.read_button.Enabled = true;

            this.testSuites_TextBox.Enabled = sender == readTestPoints_radioButton
                || sender == readTestResults_radioButton;
            this.batchSize_TextBox.Enabled = this.deleteRecords_CheckBox.Enabled = sender == readTestResults_radioButton;
        }

        private void read_button_Click(object sender, EventArgs e)
        {
            var testSuiteIds = new List<int>();
            if (this.testSuites_TextBox.Text.Trim() != "")
            {
                var tss = this.testSuites_TextBox.Text.Trim().Split(',');
                foreach (var ts in tss)
                {
                    if (!int.TryParse(ts.Trim(), out int testSuiteId))
                        return;
                    testSuiteIds.Add(testSuiteId);
                }
            }

            _parameters = new ReadReportData.Parameters()
            {
                CreateNewFile = createDB_checkBox.Checked,
                TestSuiteIds = testSuiteIds,
            };
            Program.UserSettings.TestSuiteIds = _parameters.TestSuiteIds;

            if (readTestPoints_radioButton.Checked)
            {
                _parameters.ProcessType = ReadReportData.Parameters.ProcessTypes.testPoints;
                DialogResult = DialogResult.OK;
            }
            else if (readTestRuns_radioButton.Checked)
            {
                _parameters.ProcessType = ReadReportData.Parameters.ProcessTypes.testRuns;
                DialogResult = DialogResult.OK;
            }
            else if (readTestResults_radioButton.Checked)
            {
                _parameters.ProcessType = ReadReportData.Parameters.ProcessTypes.testResults;
                _parameters.KeepRecords = !this.deleteRecords_CheckBox.Checked;
                if (this.batchSize_TextBox.Text.Trim() != "")
                {
                    if (!int.TryParse(this.batchSize_TextBox.Text.Trim(), out int batchSize))
                        return;
                    if (batchSize <= 0)
                        return;
                    _parameters.BatchSize = batchSize;
                    Program.UserSettings.BatchSize = batchSize;

                }
                else
                    _parameters.BatchSize = null;
                DialogResult = DialogResult.OK;
            }
            else if (readWorkItems_radioButton.Checked)
            {
                _parameters.ProcessType = ReadReportData.Parameters.ProcessTypes.workItems;
                DialogResult = DialogResult.OK;
            }
            else
                _parameters = null;
        }

        internal ReadReportData.Parameters GetReadParameters() => _parameters;


    }
}
