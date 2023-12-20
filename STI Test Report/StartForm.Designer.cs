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
            components = new System.ComponentModel.Container();
            main_menuStrip = new MenuStrip();
            file_ToolStripMenuItem = new ToolStripMenuItem();
            file_TeamProjects_toolStripMenuItem = new ToolStripMenuItem();
            file_Restart_toolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            file_Exit_StripMenuItem = new ToolStripMenuItem();
            dataToolStripMenuItem = new ToolStripMenuItem();
            dataOpenFolder_ToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            helpAbout_ToolStripMenuItem = new ToolStripMenuItem();
            testPlans_groupBox = new GroupBox();
            testPlan_label = new Label();
            read_button = new Button();
            open_linkLabel = new LinkLabel();
            testPlan_textBox = new TextBox();
            label1 = new Label();
            testPlan_comboBox = new ComboBox();
            testPlanBindingSource = new BindingSource(components);
            print_button = new Button();
            main_statusStrip = new StatusStrip();
            teamProject_toolStripStatusLabel = new ToolStripStatusLabel();
            status_toolStripStatusLabel = new ToolStripStatusLabel();
            pdf_SaveFileDialog = new SaveFileDialog();
            main_menuStrip.SuspendLayout();
            testPlans_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)testPlanBindingSource).BeginInit();
            main_statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // main_menuStrip
            // 
            main_menuStrip.ImageScalingSize = new Size(24, 24);
            main_menuStrip.Items.AddRange(new ToolStripItem[] { file_ToolStripMenuItem, dataToolStripMenuItem, helpToolStripMenuItem });
            main_menuStrip.Location = new Point(0, 0);
            main_menuStrip.Name = "main_menuStrip";
            main_menuStrip.Padding = new Padding(9, 3, 0, 3);
            main_menuStrip.Size = new Size(893, 35);
            main_menuStrip.TabIndex = 0;
            main_menuStrip.Text = "menuStrip1";
            // 
            // file_ToolStripMenuItem
            // 
            file_ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { file_TeamProjects_toolStripMenuItem, file_Restart_toolStripMenuItem, toolStripSeparator1, file_Exit_StripMenuItem });
            file_ToolStripMenuItem.Name = "file_ToolStripMenuItem";
            file_ToolStripMenuItem.Size = new Size(54, 29);
            file_ToolStripMenuItem.Text = "File";
            // 
            // file_TeamProjects_toolStripMenuItem
            // 
            file_TeamProjects_toolStripMenuItem.Name = "file_TeamProjects_toolStripMenuItem";
            file_TeamProjects_toolStripMenuItem.Size = new Size(223, 34);
            file_TeamProjects_toolStripMenuItem.Text = "Team projects";
            // 
            // file_Restart_toolStripMenuItem
            // 
            file_Restart_toolStripMenuItem.Name = "file_Restart_toolStripMenuItem";
            file_Restart_toolStripMenuItem.Size = new Size(223, 34);
            file_Restart_toolStripMenuItem.Text = "Restart";
            file_Restart_toolStripMenuItem.Click += file_Restart_toolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(220, 6);
            // 
            // file_Exit_StripMenuItem
            // 
            file_Exit_StripMenuItem.Name = "file_Exit_StripMenuItem";
            file_Exit_StripMenuItem.Size = new Size(223, 34);
            file_Exit_StripMenuItem.Text = "Exit";
            file_Exit_StripMenuItem.TextAlign = ContentAlignment.MiddleLeft;
            file_Exit_StripMenuItem.Click += FileExit_StripMenuItem_Click;
            // 
            // dataToolStripMenuItem
            // 
            dataToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dataOpenFolder_ToolStripMenuItem });
            dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            dataToolStripMenuItem.Size = new Size(65, 29);
            dataToolStripMenuItem.Text = "Data";
            // 
            // dataOpenFolder_ToolStripMenuItem
            // 
            dataOpenFolder_ToolStripMenuItem.Name = "dataOpenFolder_ToolStripMenuItem";
            dataOpenFolder_ToolStripMenuItem.Size = new Size(210, 34);
            dataOpenFolder_ToolStripMenuItem.Text = "Open folder";
            dataOpenFolder_ToolStripMenuItem.Click += dataOpenFolder_ToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { helpAbout_ToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 29);
            helpToolStripMenuItem.Text = "Help";
            // 
            // helpAbout_ToolStripMenuItem
            // 
            helpAbout_ToolStripMenuItem.Name = "helpAbout_ToolStripMenuItem";
            helpAbout_ToolStripMenuItem.Size = new Size(164, 34);
            helpAbout_ToolStripMenuItem.Text = "About";
            helpAbout_ToolStripMenuItem.Click += HelpAbout_ToolStripMenuItem_Click;
            // 
            // testPlans_groupBox
            // 
            testPlans_groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            testPlans_groupBox.Controls.Add(testPlan_label);
            testPlans_groupBox.Controls.Add(read_button);
            testPlans_groupBox.Controls.Add(open_linkLabel);
            testPlans_groupBox.Controls.Add(testPlan_textBox);
            testPlans_groupBox.Controls.Add(label1);
            testPlans_groupBox.Controls.Add(testPlan_comboBox);
            testPlans_groupBox.Controls.Add(print_button);
            testPlans_groupBox.Enabled = false;
            testPlans_groupBox.Location = new Point(21, 58);
            testPlans_groupBox.Margin = new Padding(4, 5, 4, 5);
            testPlans_groupBox.Name = "testPlans_groupBox";
            testPlans_groupBox.Padding = new Padding(4, 5, 4, 5);
            testPlans_groupBox.Size = new Size(859, 277);
            testPlans_groupBox.TabIndex = 1;
            testPlans_groupBox.TabStop = false;
            // 
            // testPlan_label
            // 
            testPlan_label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            testPlan_label.AutoSize = true;
            testPlan_label.Location = new Point(500, 101);
            testPlan_label.Name = "testPlan_label";
            testPlan_label.Size = new Size(54, 25);
            testPlan_label.TabIndex = 14;
            testPlan_label.Text = "Filter:";
            // 
            // read_button
            // 
            read_button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            read_button.Enabled = false;
            read_button.Location = new Point(457, 188);
            read_button.Name = "read_button";
            read_button.Size = new Size(107, 38);
            read_button.TabIndex = 13;
            read_button.Text = "Read";
            read_button.UseVisualStyleBackColor = true;
            read_button.Click += Read_button_Click;
            // 
            // open_linkLabel
            // 
            open_linkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            open_linkLabel.AutoSize = true;
            open_linkLabel.Location = new Point(609, 195);
            open_linkLabel.Margin = new Padding(4, 0, 4, 0);
            open_linkLabel.Name = "open_linkLabel";
            open_linkLabel.Size = new Size(56, 25);
            open_linkLabel.TabIndex = 12;
            open_linkLabel.TabStop = true;
            open_linkLabel.Text = "Open";
            open_linkLabel.Visible = false;
            open_linkLabel.LinkClicked += open_linkLabel_LinkClicked;
            // 
            // testPlan_textBox
            // 
            testPlan_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            testPlan_textBox.Location = new Point(581, 98);
            testPlan_textBox.Name = "testPlan_textBox";
            testPlan_textBox.Size = new Size(242, 31);
            testPlan_textBox.TabIndex = 11;
            testPlan_textBox.TextChanged += testPlan_textBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 63);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(85, 25);
            label1.TabIndex = 2;
            label1.Text = "Test plan:";
            // 
            // testPlan_comboBox
            // 
            testPlan_comboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            testPlan_comboBox.DataSource = testPlanBindingSource;
            testPlan_comboBox.DisplayMember = "TitleId";
            testPlan_comboBox.FormattingEnabled = true;
            testPlan_comboBox.Location = new Point(124, 58);
            testPlan_comboBox.Margin = new Padding(4, 5, 4, 5);
            testPlan_comboBox.Name = "testPlan_comboBox";
            testPlan_comboBox.Size = new Size(699, 33);
            testPlan_comboBox.TabIndex = 1;
            testPlan_comboBox.ValueMember = "ID";
            testPlan_comboBox.SelectedIndexChanged += testPlan_comboBox_SelectedIndexChanged;
            // 
            // testPlanBindingSource
            // 
            testPlanBindingSource.DataSource = typeof(SQLite.BusinessObjects.TestPlan);
            // 
            // print_button
            // 
            print_button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            print_button.Enabled = false;
            print_button.Location = new Point(716, 188);
            print_button.Margin = new Padding(4, 5, 4, 5);
            print_button.Name = "print_button";
            print_button.Size = new Size(107, 38);
            print_button.TabIndex = 0;
            print_button.Text = "Print";
            print_button.UseVisualStyleBackColor = true;
            print_button.Click += print_button_Click;
            // 
            // main_statusStrip
            // 
            main_statusStrip.ImageScalingSize = new Size(24, 24);
            main_statusStrip.Items.AddRange(new ToolStripItem[] { teamProject_toolStripStatusLabel, status_toolStripStatusLabel });
            main_statusStrip.Location = new Point(0, 364);
            main_statusStrip.Name = "main_statusStrip";
            main_statusStrip.Size = new Size(893, 32);
            main_statusStrip.TabIndex = 2;
            main_statusStrip.Text = "statusStrip1";
            // 
            // teamProject_toolStripStatusLabel
            // 
            teamProject_toolStripStatusLabel.Name = "teamProject_toolStripStatusLabel";
            teamProject_toolStripStatusLabel.Size = new Size(24, 25);
            teamProject_toolStripStatusLabel.Text = "...";
            // 
            // status_toolStripStatusLabel
            // 
            status_toolStripStatusLabel.Name = "status_toolStripStatusLabel";
            status_toolStripStatusLabel.Size = new Size(24, 25);
            status_toolStripStatusLabel.Text = "...";
            // 
            // pdf_SaveFileDialog
            // 
            pdf_SaveFileDialog.DefaultExt = "pdf";
            pdf_SaveFileDialog.Filter = "pdf|*.pdf|All|*.*";
            pdf_SaveFileDialog.Title = "Save pdf File";
            // 
            // StartForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(893, 396);
            Controls.Add(main_statusStrip);
            Controls.Add(testPlans_groupBox);
            Controls.Add(main_menuStrip);
            MainMenuStrip = main_menuStrip;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimumSize = new Size(559, 451);
            Name = "StartForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Test Report";
            FormClosing += StartForm_FormClosing;
            Shown += StartForm_Shown;
            main_menuStrip.ResumeLayout(false);
            main_menuStrip.PerformLayout();
            testPlans_groupBox.ResumeLayout(false);
            testPlans_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)testPlanBindingSource).EndInit();
            main_statusStrip.ResumeLayout(false);
            main_statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private BindingSource testPlanBindingSource;
        private StatusStrip main_statusStrip;
        private ToolStripStatusLabel teamProject_toolStripStatusLabel;
        private ToolStripMenuItem file_Restart_toolStripMenuItem;
        private TextBox testPlan_textBox;
        private ComboBox testPlan_comboBox;
        private ToolStripMenuItem file_TeamProjects_toolStripMenuItem;
        private SaveFileDialog pdf_SaveFileDialog;
        private ToolStripStatusLabel status_toolStripStatusLabel;
        private LinkLabel open_linkLabel;
        private Button read_button;
        private Label testPlan_label;
    }
}