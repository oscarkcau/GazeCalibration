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
			this.labelStartText = new System.Windows.Forms.Label();
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
			// FormSelectionTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(704, 448);
			this.Controls.Add(this.labelStartText);
			this.DoubleBuffered = true;
			this.Name = "FormSelectionTest";
			this.Text = "FormSelectionTest";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormSelectionTest_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormSelectionTest_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormSelectionTest_MouseMove);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelStartText;
	}
}