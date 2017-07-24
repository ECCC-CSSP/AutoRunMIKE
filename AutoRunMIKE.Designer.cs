namespace AutoRunMike
{
    partial class AutoRunMikeMzLaunchFileName
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.lblMzLaunchStyle = new System.Windows.Forms.Label();
            this.radioButtonRunMzLaunchNormal = new System.Windows.Forms.RadioButton();
            this.radioButtonRunMzLaunchMinimized = new System.Windows.Forms.RadioButton();
            this.radioButtonRunMzLaunchHidden = new System.Windows.Forms.RadioButton();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.lblRunning = new System.Windows.Forms.Label();
            this.butPause = new System.Windows.Forms.Button();
            this.butStart = new System.Windows.Forms.Button();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.dataGridViewScenarios = new System.Windows.Forms.DataGridView();
            this.richTextBoxStatus = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatusUpdate = new System.Windows.Forms.Label();
            this.lblStatusWorking = new System.Windows.Forms.Label();
            this.processMike = new System.Diagnostics.Process();
            this.timerMzLaunchStillRunning = new System.Windows.Forms.Timer(this.components);
            this.timerAutoRunMike = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panelCommand.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScenarios)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBoxStatus);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(662, 417);
            this.splitContainer1.SplitterDistance = 203;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelCommand);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelGrid);
            this.splitContainer2.Size = new System.Drawing.Size(662, 203);
            this.splitContainer2.SplitterDistance = 59;
            this.splitContainer2.TabIndex = 0;
            // 
            // panelCommand
            // 
            this.panelCommand.Controls.Add(this.lblMzLaunchStyle);
            this.panelCommand.Controls.Add(this.radioButtonRunMzLaunchNormal);
            this.panelCommand.Controls.Add(this.radioButtonRunMzLaunchMinimized);
            this.panelCommand.Controls.Add(this.radioButtonRunMzLaunchHidden);
            this.panelCommand.Controls.Add(this.lblCurrentFile);
            this.panelCommand.Controls.Add(this.lblRunning);
            this.panelCommand.Controls.Add(this.butPause);
            this.panelCommand.Controls.Add(this.butStart);
            this.panelCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCommand.Location = new System.Drawing.Point(0, 0);
            this.panelCommand.Name = "panelCommand";
            this.panelCommand.Size = new System.Drawing.Size(658, 55);
            this.panelCommand.TabIndex = 0;
            // 
            // lblMzLaunchStyle
            // 
            this.lblMzLaunchStyle.AutoSize = true;
            this.lblMzLaunchStyle.Location = new System.Drawing.Point(244, 15);
            this.lblMzLaunchStyle.Name = "lblMzLaunchStyle";
            this.lblMzLaunchStyle.Size = new System.Drawing.Size(106, 13);
            this.lblMzLaunchStyle.TabIndex = 7;
            this.lblMzLaunchStyle.Text = "MzLaunch.exe Style:";
            // 
            // radioButtonRunMzLaunchNormal
            // 
            this.radioButtonRunMzLaunchNormal.AutoSize = true;
            this.radioButtonRunMzLaunchNormal.Location = new System.Drawing.Point(356, 13);
            this.radioButtonRunMzLaunchNormal.Name = "radioButtonRunMzLaunchNormal";
            this.radioButtonRunMzLaunchNormal.Size = new System.Drawing.Size(58, 17);
            this.radioButtonRunMzLaunchNormal.TabIndex = 6;
            this.radioButtonRunMzLaunchNormal.Text = "Normal";
            this.radioButtonRunMzLaunchNormal.UseVisualStyleBackColor = true;
            // 
            // radioButtonRunMzLaunchMinimized
            // 
            this.radioButtonRunMzLaunchMinimized.AutoSize = true;
            this.radioButtonRunMzLaunchMinimized.Checked = true;
            this.radioButtonRunMzLaunchMinimized.Location = new System.Drawing.Point(426, 13);
            this.radioButtonRunMzLaunchMinimized.Name = "radioButtonRunMzLaunchMinimized";
            this.radioButtonRunMzLaunchMinimized.Size = new System.Drawing.Size(71, 17);
            this.radioButtonRunMzLaunchMinimized.TabIndex = 6;
            this.radioButtonRunMzLaunchMinimized.TabStop = true;
            this.radioButtonRunMzLaunchMinimized.Text = "Minimized";
            this.radioButtonRunMzLaunchMinimized.UseVisualStyleBackColor = true;
            // 
            // radioButtonRunMzLaunchHidden
            // 
            this.radioButtonRunMzLaunchHidden.AutoSize = true;
            this.radioButtonRunMzLaunchHidden.Location = new System.Drawing.Point(508, 13);
            this.radioButtonRunMzLaunchHidden.Name = "radioButtonRunMzLaunchHidden";
            this.radioButtonRunMzLaunchHidden.Size = new System.Drawing.Size(59, 17);
            this.radioButtonRunMzLaunchHidden.TabIndex = 6;
            this.radioButtonRunMzLaunchHidden.Text = "Hidden";
            this.radioButtonRunMzLaunchHidden.UseVisualStyleBackColor = true;
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Location = new System.Drawing.Point(68, 38);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(41, 13);
            this.lblCurrentFile.TabIndex = 5;
            this.lblCurrentFile.Text = "[empty]";
            // 
            // lblRunning
            // 
            this.lblRunning.AutoSize = true;
            this.lblRunning.Location = new System.Drawing.Point(12, 38);
            this.lblRunning.Name = "lblRunning";
            this.lblRunning.Size = new System.Drawing.Size(50, 13);
            this.lblRunning.TabIndex = 4;
            this.lblRunning.Text = "Running:";
            // 
            // butPause
            // 
            this.butPause.Location = new System.Drawing.Point(103, 7);
            this.butPause.Name = "butPause";
            this.butPause.Size = new System.Drawing.Size(65, 28);
            this.butPause.TabIndex = 0;
            this.butPause.Text = "Pause";
            this.butPause.UseVisualStyleBackColor = true;
            this.butPause.Click += new System.EventHandler(this.butPause_Click);
            // 
            // butStart
            // 
            this.butStart.Location = new System.Drawing.Point(15, 7);
            this.butStart.Name = "butStart";
            this.butStart.Size = new System.Drawing.Size(71, 28);
            this.butStart.TabIndex = 0;
            this.butStart.Text = "Start";
            this.butStart.UseVisualStyleBackColor = true;
            this.butStart.Click += new System.EventHandler(this.butStart_Click);
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.dataGridViewScenarios);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 0);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(658, 136);
            this.panelGrid.TabIndex = 0;
            // 
            // dataGridViewScenarios
            // 
            this.dataGridViewScenarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewScenarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewScenarios.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewScenarios.Name = "dataGridViewScenarios";
            this.dataGridViewScenarios.Size = new System.Drawing.Size(658, 136);
            this.dataGridViewScenarios.TabIndex = 0;
            this.dataGridViewScenarios.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridViewScenarios_ColumnAdded);
            this.dataGridViewScenarios.SelectionChanged += new System.EventHandler(this.dataGridViewScenarios_SelectionChanged);
            // 
            // richTextBoxStatus
            // 
            this.richTextBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxStatus.Location = new System.Drawing.Point(0, 29);
            this.richTextBoxStatus.Name = "richTextBoxStatus";
            this.richTextBoxStatus.Size = new System.Drawing.Size(658, 177);
            this.richTextBoxStatus.TabIndex = 0;
            this.richTextBoxStatus.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatusUpdate);
            this.panel1.Controls.Add(this.lblStatusWorking);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(658, 29);
            this.panel1.TabIndex = 1;
            // 
            // lblStatusUpdate
            // 
            this.lblStatusUpdate.AutoSize = true;
            this.lblStatusUpdate.ForeColor = System.Drawing.Color.Red;
            this.lblStatusUpdate.Location = new System.Drawing.Point(127, 8);
            this.lblStatusUpdate.Name = "lblStatusUpdate";
            this.lblStatusUpdate.Size = new System.Drawing.Size(41, 13);
            this.lblStatusUpdate.TabIndex = 1;
            this.lblStatusUpdate.Text = "[empty]";
            // 
            // lblStatusWorking
            // 
            this.lblStatusWorking.AutoSize = true;
            this.lblStatusWorking.ForeColor = System.Drawing.Color.Red;
            this.lblStatusWorking.Location = new System.Drawing.Point(11, 8);
            this.lblStatusWorking.Name = "lblStatusWorking";
            this.lblStatusWorking.Size = new System.Drawing.Size(55, 13);
            this.lblStatusWorking.TabIndex = 0;
            this.lblStatusWorking.Text = "Waiting ...";
            // 
            // processMike
            // 
            this.processMike.StartInfo.Domain = "";
            this.processMike.StartInfo.LoadUserProfile = false;
            this.processMike.StartInfo.Password = null;
            this.processMike.StartInfo.StandardErrorEncoding = null;
            this.processMike.StartInfo.StandardOutputEncoding = null;
            this.processMike.StartInfo.UserName = "";
            this.processMike.SynchronizingObject = this;
            this.processMike.Exited += new System.EventHandler(this.processMike_Exited);
            // 
            // timerMzLaunchStillRunning
            // 
            this.timerMzLaunchStillRunning.Interval = 10000;
            this.timerMzLaunchStillRunning.Tick += new System.EventHandler(this.timerMzLaunchStillRunning_Tick);
            // 
            // timerAutoRunMike
            // 
            this.timerAutoRunMike.Interval = 10000;
            this.timerAutoRunMike.Tick += new System.EventHandler(this.timerAutoRunMike_Tick);
            // 
            // AutoRunMikeMzLaunchFileName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 417);
            this.Controls.Add(this.splitContainer1);
            this.Name = "AutoRunMikeMzLaunchFileName";
            this.Text = "AutoRunMIKE application";
            this.Load += new System.EventHandler(this.AutoRunMike_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panelCommand.ResumeLayout(false);
            this.panelCommand.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScenarios)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panelCommand;
        private System.Windows.Forms.Button butStart;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.DataGridView dataGridViewScenarios;
        private System.Windows.Forms.RichTextBox richTextBoxStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatusUpdate;
        private System.Windows.Forms.Label lblStatusWorking;
        private System.Diagnostics.Process processMike;
        private System.Windows.Forms.Button butPause;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Timer timerMzLaunchStillRunning;
        private System.Windows.Forms.Label lblRunning;
        private System.Windows.Forms.RadioButton radioButtonRunMzLaunchHidden;
        private System.Windows.Forms.RadioButton radioButtonRunMzLaunchMinimized;
        private System.Windows.Forms.RadioButton radioButtonRunMzLaunchNormal;
        private System.Windows.Forms.Timer timerAutoRunMike;
        private System.Windows.Forms.Label lblMzLaunchStyle;
    }
}

