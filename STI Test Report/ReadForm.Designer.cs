namespace STI_Test_Report
{
    partial class ReadForm
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
            groupBox1 = new GroupBox();
            readWorkItems_radioButton = new RadioButton();
            deleteRecords_CheckBox = new CheckBox();
            batchSize_Label = new Label();
            batchSize_TextBox = new TextBox();
            readTestResults_radioButton = new RadioButton();
            readTestRuns_radioButton = new RadioButton();
            label1 = new Label();
            testSuites_TextBox = new TextBox();
            createDB_checkBox = new CheckBox();
            readTestPoints_radioButton = new RadioButton();
            read_button = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(readWorkItems_radioButton);
            groupBox1.Controls.Add(deleteRecords_CheckBox);
            groupBox1.Controls.Add(batchSize_Label);
            groupBox1.Controls.Add(batchSize_TextBox);
            groupBox1.Controls.Add(readTestResults_radioButton);
            groupBox1.Controls.Add(readTestRuns_radioButton);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(testSuites_TextBox);
            groupBox1.Controls.Add(createDB_checkBox);
            groupBox1.Controls.Add(readTestPoints_radioButton);
            groupBox1.Location = new Point(12, 26);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(937, 451);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // readWorkItems_radioButton
            // 
            readWorkItems_radioButton.AutoSize = true;
            readWorkItems_radioButton.Location = new Point(25, 326);
            readWorkItems_radioButton.Name = "readWorkItems_radioButton";
            readWorkItems_radioButton.Size = new Size(168, 29);
            readWorkItems_radioButton.TabIndex = 9;
            readWorkItems_radioButton.TabStop = true;
            readWorkItems_radioButton.Text = "Read work items";
            readWorkItems_radioButton.UseVisualStyleBackColor = true;
            readWorkItems_radioButton.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // deleteRecords_CheckBox
            // 
            deleteRecords_CheckBox.AutoSize = true;
            deleteRecords_CheckBox.Enabled = false;
            deleteRecords_CheckBox.Location = new Point(346, 272);
            deleteRecords_CheckBox.Name = "deleteRecords_CheckBox";
            deleteRecords_CheckBox.Size = new Size(152, 29);
            deleteRecords_CheckBox.TabIndex = 8;
            deleteRecords_CheckBox.Text = "Delete records";
            deleteRecords_CheckBox.UseVisualStyleBackColor = true;
            // 
            // batchSize_Label
            // 
            batchSize_Label.AutoSize = true;
            batchSize_Label.Location = new Point(69, 273);
            batchSize_Label.Name = "batchSize_Label";
            batchSize_Label.Size = new Size(89, 25);
            batchSize_Label.TabIndex = 7;
            batchSize_Label.Text = "Batch size";
            // 
            // batchSize_TextBox
            // 
            batchSize_TextBox.Enabled = false;
            batchSize_TextBox.Location = new Point(184, 270);
            batchSize_TextBox.Name = "batchSize_TextBox";
            batchSize_TextBox.Size = new Size(105, 31);
            batchSize_TextBox.TabIndex = 6;
            // 
            // readTestResults_radioButton
            // 
            readTestResults_radioButton.AutoSize = true;
            readTestResults_radioButton.Location = new Point(25, 225);
            readTestResults_radioButton.Name = "readTestResults_radioButton";
            readTestResults_radioButton.Size = new Size(166, 29);
            readTestResults_radioButton.TabIndex = 5;
            readTestResults_radioButton.TabStop = true;
            readTestResults_radioButton.Text = "Read test results";
            readTestResults_radioButton.UseVisualStyleBackColor = true;
            readTestResults_radioButton.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // readTestRuns_radioButton
            // 
            readTestRuns_radioButton.AutoSize = true;
            readTestRuns_radioButton.Location = new Point(25, 160);
            readTestRuns_radioButton.Name = "readTestRuns_radioButton";
            readTestRuns_radioButton.Size = new Size(149, 29);
            readTestRuns_radioButton.TabIndex = 4;
            readTestRuns_radioButton.TabStop = true;
            readTestRuns_radioButton.Text = "Read test runs";
            readTestRuns_radioButton.UseVisualStyleBackColor = true;
            readTestRuns_radioButton.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 393);
            label1.Name = "label1";
            label1.Size = new Size(96, 25);
            label1.TabIndex = 3;
            label1.Text = "Test suites:";
            // 
            // testSuites_TextBox
            // 
            testSuites_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            testSuites_TextBox.Location = new Point(141, 393);
            testSuites_TextBox.Name = "testSuites_TextBox";
            testSuites_TextBox.Size = new Size(761, 31);
            testSuites_TextBox.TabIndex = 2;
            // 
            // createDB_checkBox
            // 
            createDB_checkBox.AutoSize = true;
            createDB_checkBox.Location = new Point(69, 91);
            createDB_checkBox.Name = "createDB_checkBox";
            createDB_checkBox.Size = new Size(202, 29);
            createDB_checkBox.TabIndex = 1;
            createDB_checkBox.Text = "Create new database";
            createDB_checkBox.UseVisualStyleBackColor = true;
            // 
            // readTestPoints_radioButton
            // 
            readTestPoints_radioButton.AutoSize = true;
            readTestPoints_radioButton.Location = new Point(25, 47);
            readTestPoints_radioButton.Name = "readTestPoints_radioButton";
            readTestPoints_radioButton.Size = new Size(165, 29);
            readTestPoints_radioButton.TabIndex = 0;
            readTestPoints_radioButton.TabStop = true;
            readTestPoints_radioButton.Text = "Read test points";
            readTestPoints_radioButton.UseVisualStyleBackColor = true;
            readTestPoints_radioButton.CheckedChanged += RadioButton_CheckedChanged;
            // 
            // read_button
            // 
            read_button.Enabled = false;
            read_button.Location = new Point(833, 483);
            read_button.Name = "read_button";
            read_button.Size = new Size(107, 38);
            read_button.TabIndex = 14;
            read_button.Text = "OK";
            read_button.UseVisualStyleBackColor = true;
            read_button.Click += read_button_Click;
            // 
            // ReadForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(983, 554);
            Controls.Add(read_button);
            Controls.Add(groupBox1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ReadForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Read Report Data";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private RadioButton readTestPoints_radioButton;
        private Button read_button;
        private Label label1;
        private TextBox testSuites_TextBox;
        private RadioButton readTestRuns_radioButton;
        private CheckBox createDB_checkBox;
        private RadioButton readTestResults_radioButton;
        private Label batchSize_Label;
        private TextBox batchSize_TextBox;
        private CheckBox deleteRecords_CheckBox;
        private RadioButton readWorkItems_radioButton;
    }
}