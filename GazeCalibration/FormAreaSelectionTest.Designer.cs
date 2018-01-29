namespace GazeCalibration
{
	partial class FormAreaSelectionTest
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
			this.labelStartText = new System.Windows.Forms.Label();
			this.timerGazeLost = new System.Windows.Forms.Timer(this.components);
			this.timerVirtualGazeTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// labelStartText
			// 
			this.labelStartText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelStartText.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStartText.Location = new System.Drawing.Point(0, 0);
			this.labelStartText.Name = "labelStartText";
			this.labelStartText.Size = new System.Drawing.Size(968, 646);
			this.labelStartText.TabIndex = 1;
			this.labelStartText.Text = "Click To Start Test";
			this.labelStartText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelStartText.Click += new System.EventHandler(this.labelStartText_Click);
			// 
			// timerGazeLost
			// 
			this.timerGazeLost.Interval = 500;
			this.timerGazeLost.Tick += new System.EventHandler(this.timerGazeLost_Tick);
			// 
			// timerVirtualGazeTimer
			// 
			this.timerVirtualGazeTimer.Tick += new System.EventHandler(this.timerVirtualGazeTimer_Tick);
			// 
			// FormAreaSelectionTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(968, 646);
			this.Controls.Add(this.labelStartText);
			this.DoubleBuffered = true;
			this.Name = "FormAreaSelectionTest";
			this.Text = "List Selection Test";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormAreaSelectionTest_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormAreaSelectionTest_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAreaSelectionTest_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormAreaSelectionTest_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormAreaSelectionTest_MouseMove);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelStartText;
		private System.Windows.Forms.Timer timerGazeLost;
		private System.Windows.Forms.Timer timerVirtualGazeTimer;
	}
}