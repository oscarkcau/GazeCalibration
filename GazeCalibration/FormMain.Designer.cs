namespace GazeCalibration
{
	partial class FormMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.propertyGridSettings = new System.Windows.Forms.PropertyGrid();
			this.statusStripMain = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.imageBoxCapture = new Emgu.CV.UI.ImageBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonSaveDefaultSettings = new System.Windows.Forms.Button();
			this.buttonSaveSettings = new System.Windows.Forms.Button();
			this.buttonLoadSettings = new System.Windows.Forms.Button();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonStartCapture = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonGazeCalibration = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSelectionTest = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonSaveModel = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonLoadModel = new System.Windows.Forms.ToolStripButton();
			this.statusStripMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageBoxCapture)).BeginInit();
			this.panel1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGridSettings
			// 
			this.propertyGridSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridSettings.LineColor = System.Drawing.SystemColors.ControlDark;
			this.propertyGridSettings.Location = new System.Drawing.Point(0, 0);
			this.propertyGridSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.propertyGridSettings.Name = "propertyGridSettings";
			this.propertyGridSettings.Size = new System.Drawing.Size(300, 766);
			this.propertyGridSettings.TabIndex = 0;
			// 
			// statusStripMain
			// 
			this.statusStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelMain});
			this.statusStripMain.Location = new System.Drawing.Point(0, 901);
			this.statusStripMain.Name = "statusStripMain";
			this.statusStripMain.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
			this.statusStripMain.Size = new System.Drawing.Size(902, 30);
			this.statusStripMain.TabIndex = 1;
			this.statusStripMain.Text = "statusStrip1";
			// 
			// toolStripStatusLabelMain
			// 
			this.toolStripStatusLabelMain.Name = "toolStripStatusLabelMain";
			this.toolStripStatusLabelMain.Size = new System.Drawing.Size(179, 25);
			this.toolStripStatusLabelMain.Text = "toolStripStatusLabel1";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(893, 0);
			this.splitter1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(9, 901);
			this.splitter1.TabIndex = 2;
			this.splitter1.TabStop = false;
			// 
			// imageBoxCapture
			// 
			this.imageBoxCapture.BackColor = System.Drawing.Color.Black;
			this.imageBoxCapture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.imageBoxCapture.Location = new System.Drawing.Point(0, 0);
			this.imageBoxCapture.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.imageBoxCapture.Name = "imageBoxCapture";
			this.imageBoxCapture.Size = new System.Drawing.Size(893, 901);
			this.imageBoxCapture.TabIndex = 2;
			this.imageBoxCapture.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.propertyGridSettings);
			this.panel1.Controls.Add(this.buttonSaveDefaultSettings);
			this.panel1.Controls.Add(this.buttonSaveSettings);
			this.panel1.Controls.Add(this.buttonLoadSettings);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(902, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(300, 931);
			this.panel1.TabIndex = 5;
			// 
			// buttonSaveDefaultSettings
			// 
			this.buttonSaveDefaultSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonSaveDefaultSettings.Location = new System.Drawing.Point(0, 766);
			this.buttonSaveDefaultSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonSaveDefaultSettings.Name = "buttonSaveDefaultSettings";
			this.buttonSaveDefaultSettings.Size = new System.Drawing.Size(300, 55);
			this.buttonSaveDefaultSettings.TabIndex = 3;
			this.buttonSaveDefaultSettings.Text = "Save as Default Settings";
			this.buttonSaveDefaultSettings.UseVisualStyleBackColor = true;
			this.buttonSaveDefaultSettings.Click += new System.EventHandler(this.buttonSaveDefaultSettings_Click);
			// 
			// buttonSaveSettings
			// 
			this.buttonSaveSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonSaveSettings.Location = new System.Drawing.Point(0, 821);
			this.buttonSaveSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonSaveSettings.Name = "buttonSaveSettings";
			this.buttonSaveSettings.Size = new System.Drawing.Size(300, 55);
			this.buttonSaveSettings.TabIndex = 2;
			this.buttonSaveSettings.Text = "Save Settings";
			this.buttonSaveSettings.UseVisualStyleBackColor = true;
			this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
			// 
			// buttonLoadSettings
			// 
			this.buttonLoadSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonLoadSettings.Location = new System.Drawing.Point(0, 876);
			this.buttonLoadSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonLoadSettings.Name = "buttonLoadSettings";
			this.buttonLoadSettings.Size = new System.Drawing.Size(300, 55);
			this.buttonLoadSettings.TabIndex = 1;
			this.buttonLoadSettings.Text = "Load Settings";
			this.buttonLoadSettings.UseVisualStyleBackColor = true;
			this.buttonLoadSettings.Click += new System.EventHandler(this.buttonLoadSettings_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStartCapture,
            this.toolStripButtonGazeCalibration,
            this.toolStripButtonSelectionTest,
            this.toolStripSeparator1,
            this.toolStripButtonSaveModel,
            this.toolStripButtonLoadModel});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(893, 32);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonStartCapture
			// 
			this.toolStripButtonStartCapture.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStartCapture.Image")));
			this.toolStripButtonStartCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStartCapture.Name = "toolStripButtonStartCapture";
			this.toolStripButtonStartCapture.Size = new System.Drawing.Size(143, 29);
			this.toolStripButtonStartCapture.Text = "Start Capture";
			this.toolStripButtonStartCapture.Click += new System.EventHandler(this.toolStripButtonStartCapture_Click);
			// 
			// toolStripButtonGazeCalibration
			// 
			this.toolStripButtonGazeCalibration.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGazeCalibration.Image")));
			this.toolStripButtonGazeCalibration.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonGazeCalibration.Name = "toolStripButtonGazeCalibration";
			this.toolStripButtonGazeCalibration.Size = new System.Drawing.Size(168, 29);
			this.toolStripButtonGazeCalibration.Text = "Gaze Calibration";
			this.toolStripButtonGazeCalibration.Click += new System.EventHandler(this.toolStripButtonGazeCalibration_Click);
			// 
			// toolStripButtonSelectionTest
			// 
			this.toolStripButtonSelectionTest.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelectionTest.Image")));
			this.toolStripButtonSelectionTest.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSelectionTest.Name = "toolStripButtonSelectionTest";
			this.toolStripButtonSelectionTest.Size = new System.Drawing.Size(141, 29);
			this.toolStripButtonSelectionTest.Text = "SelectionTest";
			this.toolStripButtonSelectionTest.Click += new System.EventHandler(this.toolStripButtonSelectionTest_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
			// 
			// toolStripButtonSaveModel
			// 
			this.toolStripButtonSaveModel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveModel.Image")));
			this.toolStripButtonSaveModel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSaveModel.Name = "toolStripButtonSaveModel";
			this.toolStripButtonSaveModel.Size = new System.Drawing.Size(133, 29);
			this.toolStripButtonSaveModel.Text = "Save Model";
			this.toolStripButtonSaveModel.ToolTipText = "Save Regression Model";
			this.toolStripButtonSaveModel.Click += new System.EventHandler(this.toolStripButtonSaveModel_Click);
			// 
			// toolStripButtonLoadModel
			// 
			this.toolStripButtonLoadModel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadModel.Image")));
			this.toolStripButtonLoadModel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonLoadModel.Name = "toolStripButtonLoadModel";
			this.toolStripButtonLoadModel.Size = new System.Drawing.Size(135, 29);
			this.toolStripButtonLoadModel.Text = "Load Model";
			this.toolStripButtonLoadModel.ToolTipText = "Load Regression Model";
			this.toolStripButtonLoadModel.Click += new System.EventHandler(this.toolStripButtonLoadModel_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1202, 931);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.imageBoxCapture);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.statusStripMain);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Gaze Calibration";
			this.statusStripMain.ResumeLayout(false);
			this.statusStripMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageBoxCapture)).EndInit();
			this.panel1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGridSettings;
		private System.Windows.Forms.StatusStrip statusStripMain;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMain;
		private System.Windows.Forms.Splitter splitter1;
		private Emgu.CV.UI.ImageBox imageBoxCapture;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonSaveDefaultSettings;
		private System.Windows.Forms.Button buttonSaveSettings;
		private System.Windows.Forms.Button buttonLoadSettings;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonStartCapture;
        private System.Windows.Forms.ToolStripButton toolStripButtonGazeCalibration;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelectionTest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveModel;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadModel;
	}
}

