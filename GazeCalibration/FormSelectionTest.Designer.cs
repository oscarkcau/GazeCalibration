namespace GazeCalibration
{
	partial class FormSelectionTest
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
			this.TimerVirtualGaze = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelStartText
			// 
			this.labelStartText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelStartText.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStartText.Location = new System.Drawing.Point(0, 0);
			this.labelStartText.Name = "labelStartText";
			this.labelStartText.Size = new System.Drawing.Size(704, 448);
			this.labelStartText.TabIndex = 0;
			this.labelStartText.Text = "Click To Start Test";
			this.labelStartText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelStartText.Click += new System.EventHandler(this.labelStartText_Click);
			// 
			// TimerVirtualGaze
			// 
			this.TimerVirtualGaze.Interval = 50;
			this.TimerVirtualGaze.Tick += new System.EventHandler(this.TimerVirtualGaze_Tick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// FormSelectionTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(704, 448);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelStartText);
			this.DoubleBuffered = true;
			this.Name = "FormSelectionTest";
			this.Text = "FormSelectionTest";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormSelectionTest_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSelectionTest_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormSelectionTest_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormSelectionTest_MouseMove);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelStartText;
		private System.Windows.Forms.Timer TimerVirtualGaze;
		private System.Windows.Forms.Label label1;
	}
}