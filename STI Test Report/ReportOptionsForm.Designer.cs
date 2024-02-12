namespace STI_Test_Report
{
    partial class ReportOptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            testPlans_groupBox = new GroupBox();
            label3 = new Label();
            testSuite_TextBox = new TextBox();
            label2 = new Label();
            noTestSteps_checkBox = new CheckBox();
            noTestResults_checkBox = new CheckBox();
            lastTestRun_checkBox = new CheckBox();
            notApplicable_checkBox = new CheckBox();
            notRun_checkBox = new CheckBox();
            create_Button = new Button();
            writeLogFile_checkBox = new CheckBox();
            testPlans_groupBox.SuspendLayout();
            SuspendLayout();
            // 
            // testPlans_groupBox
            // 
            testPlans_groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            testPlans_groupBox.Controls.Add(label3);
            testPlans_groupBox.Controls.Add(testSuite_TextBox);
            testPlans_groupBox.Controls.Add(label2);
            testPlans_groupBox.Controls.Add(noTestSteps_checkBox);
            testPlans_groupBox.Controls.Add(noTestResults_checkBox);
            testPlans_groupBox.Controls.Add(lastTestRun_checkBox);
            testPlans_groupBox.Controls.Add(notApplicable_checkBox);
            testPlans_groupBox.Controls.Add(notRun_checkBox);
            testPlans_groupBox.Location = new Point(27, 36);
            testPlans_groupBox.Margin = new Padding(4, 5, 4, 5);
            testPlans_groupBox.Name = "testPlans_groupBox";
            testPlans_groupBox.Padding = new Padding(4, 5, 4, 5);
            testPlans_groupBox.Size = new Size(782, 279);
            testPlans_groupBox.TabIndex = 2;
            testPlans_groupBox.TabStop = false;
            testPlans_groupBox.Text = " ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(22, 184);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(96, 25);
            label3.TabIndex = 10;
            label3.Text = "Test suites:";
            // 
            // testSuite_TextBox
            // 
            testSuite_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            testSuite_TextBox.Location = new Point(125, 184);
            testSuite_TextBox.Name = "testSuite_TextBox";
            testSuite_TextBox.Size = new Size(625, 31);
            testSuite_TextBox.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 64);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(54, 25);
            label2.TabIndex = 8;
            label2.Text = "Filter:";
            // 
            // noTestSteps_checkBox
            // 
            noTestSteps_checkBox.AutoSize = true;
            noTestSteps_checkBox.Location = new Point(326, 99);
            noTestSteps_checkBox.Name = "noTestSteps_checkBox";
            noTestSteps_checkBox.Size = new Size(143, 29);
            noTestSteps_checkBox.TabIndex = 7;
            noTestSteps_checkBox.Text = "No test steps";
            noTestSteps_checkBox.UseVisualStyleBackColor = true;
            // 
            // noTestResults_checkBox
            // 
            noTestResults_checkBox.AutoSize = true;
            noTestResults_checkBox.Location = new Point(326, 64);
            noTestResults_checkBox.Name = "noTestResults_checkBox";
            noTestResults_checkBox.Size = new Size(152, 29);
            noTestResults_checkBox.TabIndex = 6;
            noTestResults_checkBox.Text = "No test results";
            noTestResults_checkBox.UseVisualStyleBackColor = true;
            // 
            // lastTestRun_checkBox
            // 
            lastTestRun_checkBox.AutoSize = true;
            lastTestRun_checkBox.Location = new Point(128, 134);
            lastTestRun_checkBox.Name = "lastTestRun_checkBox";
            lastTestRun_checkBox.Size = new Size(134, 29);
            lastTestRun_checkBox.TabIndex = 5;
            lastTestRun_checkBox.Text = "Last test run";
            lastTestRun_checkBox.UseVisualStyleBackColor = true;
            // 
            // notApplicable_checkBox
            // 
            notApplicable_checkBox.AutoSize = true;
            notApplicable_checkBox.Location = new Point(128, 99);
            notApplicable_checkBox.Name = "notApplicable_checkBox";
            notApplicable_checkBox.Size = new Size(153, 29);
            notApplicable_checkBox.TabIndex = 4;
            notApplicable_checkBox.Text = "Not applicable";
            notApplicable_checkBox.UseVisualStyleBackColor = true;
            // 
            // notRun_checkBox
            // 
            notRun_checkBox.AutoSize = true;
            notRun_checkBox.Location = new Point(128, 64);
            notRun_checkBox.Name = "notRun_checkBox";
            notRun_checkBox.Size = new Size(99, 29);
            notRun_checkBox.TabIndex = 3;
            notRun_checkBox.Text = "Not run";
            notRun_checkBox.UseVisualStyleBackColor = true;
            // 
            // create_Button
            // 
            create_Button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            create_Button.Location = new Point(654, 348);
            create_Button.Name = "create_Button";
            create_Button.Size = new Size(107, 38);
            create_Button.TabIndex = 14;
            create_Button.Text = "Create";
            create_Button.UseVisualStyleBackColor = true;
            create_Button.Click += Create_Button_Click;
            // 
            // writeLogFile_checkBox
            // 
            writeLogFile_checkBox.AutoSize = true;
            writeLogFile_checkBox.Location = new Point(49, 354);
            writeLogFile_checkBox.Name = "writeLogFile_checkBox";
            writeLogFile_checkBox.Size = new Size(139, 29);
            writeLogFile_checkBox.TabIndex = 15;
            writeLogFile_checkBox.Text = "Write log file";
            writeLogFile_checkBox.UseVisualStyleBackColor = true;
            // 
            // ReportOptionsForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(822, 413);
            Controls.Add(writeLogFile_checkBox);
            Controls.Add(create_Button);
            Controls.Add(testPlans_groupBox);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ReportOptionsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Print Options";
            Shown += ReportOptionsForm_Shown;
            testPlans_groupBox.ResumeLayout(false);
            testPlans_groupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox testPlans_groupBox;
        private Label label3;
        private TextBox testSuite_TextBox;
        private Label label2;
        private CheckBox noTestSteps_checkBox;
        private CheckBox noTestResults_checkBox;
        private CheckBox lastTestRun_checkBox;
        private CheckBox notApplicable_checkBox;
        private CheckBox notRun_checkBox;
        private Button create_Button;
        private CheckBox writeLogFile_checkBox;
    }
}