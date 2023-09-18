namespace STI_Test_Report
{
    partial class StartForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.main_menuStrip = new System.Windows.Forms.MenuStrip();
            this.file_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.file_TeamProjects_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.file_Restart_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.file_Exit_StripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataOpenFolder_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpAbout_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testPlans_groupBox = new System.Windows.Forms.GroupBox();
            this.open_linkLabel = new System.Windows.Forms.LinkLabel();
            this.testPlan_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.testSuite_TextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.noTestSteps_checkBox = new System.Windows.Forms.CheckBox();
            this.noTestResults_checkBox4 = new System.Windows.Forms.CheckBox();
            this.lastTestRun_checkBox = new System.Windows.Forms.CheckBox();
            this.notApplicable_checkBox2 = new System.Windows.Forms.CheckBox();
            this.notRun_checkBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.testPlan_comboBox = new System.Windows.Forms.ComboBox();
            this.testPlanBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.print_button = new System.Windows.Forms.Button();
            this.testPlanBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.main_statusStrip = new System.Windows.Forms.StatusStrip();
            this.teamProject_toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.status_toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.pdf_SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.main_menuStrip.SuspendLayout();
            this.testPlans_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testPlanBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testPlanBindingSource)).BeginInit();
            this.main_statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // main_menuStrip
            // 
            this.main_menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.main_menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_ToolStripMenuItem,
            this.dataToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.main_menuStrip.Location = new System.Drawing.Point(0, 0);
            this.main_menuStrip.Name = "main_menuStrip";
            this.main_menuStrip.Size = new System.Drawing.Size(544, 24);
            this.main_menuStrip.TabIndex = 0;
            this.main_menuStrip.Text = "menuStrip1";
            // 
            // file_ToolStripMenuItem
            // 
            this.file_ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_TeamProjects_toolStripMenuItem,
            this.file_Restart_toolStripMenuItem,
            this.toolStripSeparator1,
            this.file_Exit_StripMenuItem});
            this.file_ToolStripMenuItem.Name = "file_ToolStripMenuItem";
            this.file_ToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.file_ToolStripMenuItem.Text = "File";
            // 
            // file_TeamProjects_toolStripMenuItem
            // 
            this.file_TeamProjects_toolStripMenuItem.Name = "file_TeamProjects_toolStripMenuItem";
            this.file_TeamProjects_toolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.file_TeamProjects_toolStripMenuItem.Text = "Team projects";
            // 
            // file_Restart_toolStripMenuItem
            // 
            this.file_Restart_toolStripMenuItem.Name = "file_Restart_toolStripMenuItem";
            this.file_Restart_toolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.file_Restart_toolStripMenuItem.Text = "Restart";
            this.file_Restart_toolStripMenuItem.Click += new System.EventHandler(this.file_Restart_toolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // file_Exit_StripMenuItem
            // 
            this.file_Exit_StripMenuItem.Name = "file_Exit_StripMenuItem";
            this.file_Exit_StripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.file_Exit_StripMenuItem.Text = "Exit";
            this.file_Exit_StripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.file_Exit_StripMenuItem.Click += new System.EventHandler(this.FileExit_StripMenuItem_Click);
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataOpenFolder_ToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.dataToolStripMenuItem.Text = "Data";
            // 
            // dataOpenFolder_ToolStripMenuItem
            // 
            this.dataOpenFolder_ToolStripMenuItem.Name = "dataOpenFolder_ToolStripMenuItem";
            this.dataOpenFolder_ToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.dataOpenFolder_ToolStripMenuItem.Text = "Open folder";
            this.dataOpenFolder_ToolStripMenuItem.Click += new System.EventHandler(this.dataOpenFolder_ToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpAbout_ToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpAbout_ToolStripMenuItem
            // 
            this.helpAbout_ToolStripMenuItem.Name = "helpAbout_ToolStripMenuItem";
            this.helpAbout_ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.helpAbout_ToolStripMenuItem.Text = "About";
            this.helpAbout_ToolStripMenuItem.Click += new System.EventHandler(this.HelpAbout_ToolStripMenuItem_Click);
            // 
            // testPlans_groupBox
            // 
            this.testPlans_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testPlans_groupBox.Controls.Add(this.open_linkLabel);
            this.testPlans_groupBox.Controls.Add(this.testPlan_textBox);
            this.testPlans_groupBox.Controls.Add(this.label3);
            this.testPlans_groupBox.Controls.Add(this.testSuite_TextBox);
            this.testPlans_groupBox.Controls.Add(this.label2);
            this.testPlans_groupBox.Controls.Add(this.noTestSteps_checkBox);
            this.testPlans_groupBox.Controls.Add(this.noTestResults_checkBox4);
            this.testPlans_groupBox.Controls.Add(this.lastTestRun_checkBox);
            this.testPlans_groupBox.Controls.Add(this.notApplicable_checkBox2);
            this.testPlans_groupBox.Controls.Add(this.notRun_checkBox);
            this.testPlans_groupBox.Controls.Add(this.label1);
            this.testPlans_groupBox.Controls.Add(this.testPlan_comboBox);
            this.testPlans_groupBox.Controls.Add(this.print_button);
            this.testPlans_groupBox.Enabled = false;
            this.testPlans_groupBox.Location = new System.Drawing.Point(15, 35);
            this.testPlans_groupBox.Name = "testPlans_groupBox";
            this.testPlans_groupBox.Size = new System.Drawing.Size(520, 248);
            this.testPlans_groupBox.TabIndex = 1;
            this.testPlans_groupBox.TabStop = false;
            this.testPlans_groupBox.Text = " Test Plans ";
            // 
            // open_linkLabel
            // 
            this.open_linkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.open_linkLabel.AutoSize = true;
            this.open_linkLabel.Location = new System.Drawing.Point(342, 211);
            this.open_linkLabel.Name = "open_linkLabel";
            this.open_linkLabel.Size = new System.Drawing.Size(36, 15);
            this.open_linkLabel.TabIndex = 12;
            this.open_linkLabel.TabStop = true;
            this.open_linkLabel.Text = "Open";
            this.open_linkLabel.Visible = false;
            this.open_linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.open_linkLabel_LinkClicked);
            // 
            // testPlan_textBox
            // 
            this.testPlan_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testPlan_textBox.Location = new System.Drawing.Point(203, 59);
            this.testPlan_textBox.Margin = new System.Windows.Forms.Padding(2);
            this.testPlan_textBox.Name = "testPlan_textBox";
            this.testPlan_textBox.Size = new System.Drawing.Size(293, 23);
            this.testPlan_textBox.TabIndex = 11;
            this.testPlan_textBox.TextChanged += new System.EventHandler(this.testPlan_textBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Test suites:";
            // 
            // testSuite_TextBox
            // 
            this.testSuite_TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testSuite_TextBox.Enabled = false;
            this.testSuite_TextBox.Location = new System.Drawing.Point(87, 166);
            this.testSuite_TextBox.Margin = new System.Windows.Forms.Padding(2);
            this.testSuite_TextBox.Name = "testSuite_TextBox";
            this.testSuite_TextBox.Size = new System.Drawing.Size(409, 23);
            this.testSuite_TextBox.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Filter:";
            // 
            // noTestSteps_checkBox
            // 
            this.noTestSteps_checkBox.AutoSize = true;
            this.noTestSteps_checkBox.Enabled = false;
            this.noTestSteps_checkBox.Location = new System.Drawing.Point(203, 115);
            this.noTestSteps_checkBox.Margin = new System.Windows.Forms.Padding(2);
            this.noTestSteps_checkBox.Name = "noTestSteps_checkBox";
            this.noTestSteps_checkBox.Size = new System.Drawing.Size(94, 19);
            this.noTestSteps_checkBox.TabIndex = 7;
            this.noTestSteps_checkBox.Text = "No test steps";
            this.noTestSteps_checkBox.UseVisualStyleBackColor = true;
            // 
            // noTestResults_checkBox4
            // 
            this.noTestResults_checkBox4.AutoSize = true;
            this.noTestResults_checkBox4.Enabled = false;
            this.noTestResults_checkBox4.Location = new System.Drawing.Point(203, 94);
            this.noTestResults_checkBox4.Margin = new System.Windows.Forms.Padding(2);
            this.noTestResults_checkBox4.Name = "noTestResults_checkBox4";
            this.noTestResults_checkBox4.Size = new System.Drawing.Size(101, 19);
            this.noTestResults_checkBox4.TabIndex = 6;
            this.noTestResults_checkBox4.Text = "No test results";
            this.noTestResults_checkBox4.UseVisualStyleBackColor = true;
            // 
            // lastTestRun_checkBox
            // 
            this.lastTestRun_checkBox.AutoSize = true;
            this.lastTestRun_checkBox.Enabled = false;
            this.lastTestRun_checkBox.Location = new System.Drawing.Point(89, 136);
            this.lastTestRun_checkBox.Margin = new System.Windows.Forms.Padding(2);
            this.lastTestRun_checkBox.Name = "lastTestRun_checkBox";
            this.lastTestRun_checkBox.Size = new System.Drawing.Size(90, 19);
            this.lastTestRun_checkBox.TabIndex = 5;
            this.lastTestRun_checkBox.Text = "Last test run";
            this.lastTestRun_checkBox.UseVisualStyleBackColor = true;
            // 
            // notApplicable_checkBox2
            // 
            this.notApplicable_checkBox2.AutoSize = true;
            this.notApplicable_checkBox2.Enabled = false;
            this.notApplicable_checkBox2.Location = new System.Drawing.Point(89, 115);
            this.notApplicable_checkBox2.Margin = new System.Windows.Forms.Padding(2);
            this.notApplicable_checkBox2.Name = "notApplicable_checkBox2";
            this.notApplicable_checkBox2.Size = new System.Drawing.Size(103, 19);
            this.notApplicable_checkBox2.TabIndex = 4;
            this.notApplicable_checkBox2.Text = "Not applicable";
            this.notApplicable_checkBox2.UseVisualStyleBackColor = true;
            // 
            // notRun_checkBox
            // 
            this.notRun_checkBox.AutoSize = true;
            this.notRun_checkBox.Enabled = false;
            this.notRun_checkBox.Location = new System.Drawing.Point(89, 94);
            this.notRun_checkBox.Margin = new System.Windows.Forms.Padding(2);
            this.notRun_checkBox.Name = "notRun_checkBox";
            this.notRun_checkBox.Size = new System.Drawing.Size(67, 19);
            this.notRun_checkBox.TabIndex = 3;
            this.notRun_checkBox.Text = "Not run";
            this.notRun_checkBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Test plan:";
            // 
            // testPlan_comboBox
            // 
            this.testPlan_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testPlan_comboBox.DataSource = this.testPlanBindingSource1;
            this.testPlan_comboBox.DisplayMember = "TitleId";
            this.testPlan_comboBox.FormattingEnabled = true;
            this.testPlan_comboBox.Location = new System.Drawing.Point(87, 35);
            this.testPlan_comboBox.Name = "testPlan_comboBox";
            this.testPlan_comboBox.Size = new System.Drawing.Size(409, 23);
            this.testPlan_comboBox.TabIndex = 1;
            this.testPlan_comboBox.ValueMember = "ID";
            this.testPlan_comboBox.SelectedIndexChanged += new System.EventHandler(this.testPlan_comboBox_SelectedIndexChanged);
            // 
            // testPlanBindingSource1
            // 
            this.testPlanBindingSource1.DataSource = typeof(ADORest.BusinessObjects.TestPlan);
            // 
            // print_button
            // 
            this.print_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.print_button.Enabled = false;
            this.print_button.Location = new System.Drawing.Point(420, 207);
            this.print_button.Name = "print_button";
            this.print_button.Size = new System.Drawing.Size(75, 23);
            this.print_button.TabIndex = 0;
            this.print_button.Text = "Print";
            this.print_button.UseVisualStyleBackColor = true;
            this.print_button.Click += new System.EventHandler(this.print_button_Click);
            // 
            // testPlanBindingSource
            // 
            this.testPlanBindingSource.DataSource = typeof(ADORest.BusinessObjects.TestPlan);
            // 
            // main_statusStrip
            // 
            this.main_statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.main_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teamProject_toolStripStatusLabel,
            this.status_toolStripStatusLabel});
            this.main_statusStrip.Location = new System.Drawing.Point(0, 295);
            this.main_statusStrip.Name = "main_statusStrip";
            this.main_statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.main_statusStrip.Size = new System.Drawing.Size(544, 22);
            this.main_statusStrip.TabIndex = 2;
            this.main_statusStrip.Text = "statusStrip1";
            // 
            // teamProject_toolStripStatusLabel
            // 
            this.teamProject_toolStripStatusLabel.Name = "teamProject_toolStripStatusLabel";
            this.teamProject_toolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
            this.teamProject_toolStripStatusLabel.Text = "...";
            // 
            // status_toolStripStatusLabel
            // 
            this.status_toolStripStatusLabel.Name = "status_toolStripStatusLabel";
            this.status_toolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
            this.status_toolStripStatusLabel.Text = "...";
            // 
            // pdf_SaveFileDialog
            // 
            this.pdf_SaveFileDialog.DefaultExt = "pdf";
            this.pdf_SaveFileDialog.Filter = "pdf|*.pdf|All|*.*";
            this.pdf_SaveFileDialog.Title = "Save pdf File";
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 317);
            this.Controls.Add(this.main_statusStrip);
            this.Controls.Add(this.testPlans_groupBox);
            this.Controls.Add(this.main_menuStrip);
            this.MainMenuStrip = this.main_menuStrip;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(398, 293);
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test Report";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartForm_FormClosing);
            this.Shown += new System.EventHandler(this.StartForm_Shown);
            this.main_menuStrip.ResumeLayout(false);
            this.main_menuStrip.PerformLayout();
            this.testPlans_groupBox.ResumeLayout(false);
            this.testPlans_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testPlanBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testPlanBindingSource)).EndInit();
            this.main_statusStrip.ResumeLayout(false);
            this.main_statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip main_menuStrip;
        private ToolStripMenuItem file_ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem file_Exit_StripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem helpAbout_ToolStripMenuItem;
        private GroupBox testPlans_groupBox;
        private Label label1;
        private Button print_button;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem dataOpenFolder_ToolStripMenuItem;
        private Label label3;
        private TextBox testSuite_TextBox;
        private Label label2;
        private CheckBox noTestSteps_checkBox;
        private CheckBox noTestResults_checkBox4;
        private CheckBox lastTestRun_checkBox;
        private CheckBox notApplicable_checkBox2;
        private CheckBox notRun_checkBox;
        private BindingSource testPlanBindingSource;
        private StatusStrip main_statusStrip;
        private ToolStripStatusLabel teamProject_toolStripStatusLabel;
        private ToolStripMenuItem file_Restart_toolStripMenuItem;
        private TextBox testPlan_textBox;
        private BindingSource testPlanBindingSource1;
        private ComboBox testPlan_comboBox;
        private ToolStripMenuItem file_TeamProjects_toolStripMenuItem;
        private SaveFileDialog pdf_SaveFileDialog;
        private ToolStripStatusLabel status_toolStripStatusLabel;
        private LinkLabel open_linkLabel;
    }
}