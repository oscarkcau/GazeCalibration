using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GazeCalibration
{
	public partial class FormSelectionTest : Form
	{
		// public setting class
		public class Settings
		{
			public int NumOfTrials { get; set; } = 50;
			public int TargetRadius { get; set; } = 40;
			public int MinimumDistance { get; set; } = 200;
			public Color TargetColor { get; set; } = Color.Green;
			public Color TargetOverColor { get; set; } = Color.Red;
		}

		// private fields
		Random rand = new Random();
		FormMain formMain = null;
		Settings settings = null;
		int trialCount = 0;
		Point currentTarget;
		bool isTestStarted = false;
		bool isMouseOverTarget = false;

		// constructor
		public FormSelectionTest(FormMain form, Settings settings = null)
		{
			InitializeComponent();

			this.formMain = form;
			this.settings = settings ?? new Settings();

		}

		// event handlers
		private void labelStartText_Click(object sender, EventArgs e)
		{
			this.labelStartText.Visible = false;

			StartTest();
		}
		private void FormSelectionTest_MouseMove(object sender, MouseEventArgs e)
		{
			if (isTestStarted == false) return;

			Point mousePos = new Point(e.X, e.Y);

			isMouseOverTarget = (currentTarget.DistanceFrom(mousePos) < settings.TargetRadius);

			this.Invalidate();
		}
		private void FormSelectionTest_MouseDown(object sender, MouseEventArgs e)
		{
			// only handle left click
			if (e.Button != MouseButtons.Left) return;

			// if click on target
			if (isMouseOverTarget)
			{
				FinishTrial();

				// if more trial to go
				if (trialCount < settings.NumOfTrials)
				{
					// start a new trial 
					StartTrial();
				}
				else
				{
					// or else finish test at the end
					FinishTest();
				}
			}
		}
		private void FormSelectionTest_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// clear window
			g.Clear(SystemColors.Control);

			if (isTestStarted == false) return;

			// draw target
			using (Brush b = isMouseOverTarget ? new SolidBrush(settings.TargetColor) : new SolidBrush(settings.TargetOverColor))
			{
				float centerX = currentTarget.X;
				float centerY = currentTarget.Y;
				float radius = settings.TargetRadius;
				g.FillEllipse(b, centerX - radius, centerY - radius,
							  radius + radius, radius + radius);
			}
		}

		// main procedures
		private void StartTest()
		{
			isTestStarted = true;

			// start the first trial
			StartTrial();
		}
		private void FinishTest()
		{
			// todo: summary and record test record
		}
		private void StartTrial()
		{
			this.trialCount++;

			int margin = settings.TargetRadius;
			int w = this.ClientSize.Width;
			int h = this.ClientSize.Height;
			Point newTarget = new Point(
				rand.Next(margin, w - margin), 
				rand.Next(margin, h - margin)
				);

			if (trialCount > 1)
			while (newTarget.DistanceFrom(currentTarget) < settings.MinimumDistance)
			{
				newTarget = new Point(rand.Next(w), rand.Next(h));
			}

			currentTarget = newTarget;
		}
		private void FinishTrial()
		{
			// todo: summary and record trial record
		}
	}
}
