namespace GazeCalibration
{
	partial class FormSettings
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
			this.propertyGridMain = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonClose = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGridMain
			// 
			this.propertyGridMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridMain.HelpVisible = false;
			this.propertyGridMain.Location = new System.Drawing.Point(0, 0);
			this.propertyGridMain.Name = "propertyGridMain";
			this.propertyGridMain.Size = new System.Drawing.Size(485, 489);
			this.propertyGridMain.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonClose);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 489);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(485, 70);
			this.panel1.TabIndex = 1;
			// 
			// buttonClose
			// 
			this.buttonClose.Location = new System.Drawing.Point(368, 14);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(105, 44);
			this.buttonClose.TabIndex = 0;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// FormSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 559);
			this.Controls.Add(this.propertyGridMain);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FormSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FormSettings";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGridMain;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonClose;
	}
}