namespace GazeCalibration
{
	partial class FormCalibration
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
			this.SuspendLayout();
			// 
			// FormCalibration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 402);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "FormCalibration";
			this.Text = "FormCalibration";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCalibration_FormClosed);
			this.Load += new System.EventHandler(this.FormCalibration_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormCalibration_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormCalibration_MouseClick);
			this.ResumeLayout(false);

		}

		#endregion
	}
}