namespace GazeCalibration
{
	partial class FormTrackingEvaluation
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
			this.SuspendLayout();
			// 
			// labelStartText
			// 
			this.labelStartText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelStartText.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStartText.Location = new System.Drawing.Point(0, 0);
			this.labelStartText.Name = "labelStartText";
			this.labelStartText.Size = new System.Drawing.Size(887, 529);
			this.labelStartText.TabIndex = 1;
			this.labelStartText.Text = "Press Enter To Start Evaluation";
			this.labelStartText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timerGazeLost
			// 
			this.timerGazeLost.Enabled = true;
			this.timerGazeLost.Interval = 500;
			this.timerGazeLost.Tick += new System.EventHandler(this.timerGazeLost_Tick);
			// 
			// FormTrackingEvaluation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(887, 529);
			this.Controls.Add(this.labelStartText);
			this.DoubleBuffered = true;
			this.Name = "FormTrackingEvaluation";
			this.Text = "FormTrackingEvaluation";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTrackingEvaluation_FormClosing);
			this.Load += new System.EventHandler(this.FormTrackingEvaluation_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormTrackingEvaluation_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormTrackingEvaluation_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelStartText;
		private System.Windows.Forms.Timer timerGazeLost;
	}
}