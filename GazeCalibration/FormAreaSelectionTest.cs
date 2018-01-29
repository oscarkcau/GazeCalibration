using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace GazeCalibration
{
	public partial class FormAreaSelectionTest : Form
	{
		public class Settings
		{
			[Category("Gaze Tracking")] public int NumOfGazePositions { get; set; } = 10;
			[Category("Gaze Tracking")] public float AverageDecayRatio { get; set; } = 0.7f;
			[Category("Gaze Tracking")] public int GazeLostTime { get; set; } = 500; // ms
			[Category("Gaze Tracking")] public bool UpdateTrackingModel { get; set; } = true;

			[Category("Test")] public int NumOfTrials { get; set; } = 30;
			[Category("Test")] public int MinTrialDistance { get; set; } = 300;

			[Category("Target")] public int NumOfTargets { get; set; } = 100;
			[Category("Target")] public int TargetRadius { get; set; } = 6; // pixels
			[Category("Target")] public Color TargetColor { get; set; } = Color.LimeGreen;
			[Category("Target")] public Color TargetColor2 { get; set; } = Color.Red;
			[Category("Target")] public int MinDistance { get; set; } = 10; // pixels

			[Category("Gaze Region")] public int RegionLockTime { get; set; } = 300; // ms
			[Category("Gaze Region")] public int RegionLockSpeed { get; set; } = 100; // pixels
			[Category("Gaze Region")] public int RegionRadius { get; set; } = 100; // pixels
		}

		class Target
		{
			public Point Position { get; set; }
			public bool isSelected { get; set; } = false;
			public List<Target> AdjacentTargets { get; private set; } = new List<Target>();
			public float focusValue { get; set; } = 0f;

			public Target(Point pos)
			{
				Position = pos;
			}
		}

		// private enum
		enum GazeDisplayMode { None, Individal, Average };

		// private fields
		Random rand = new Random();
		Settings settings = new Settings();
		FormMain formMain = null;
		bool isTestStarted = false;
		bool isFirstGazeUpdate = true;
		bool isGazeLost = false;
		List<Target> targets = new List<Target>();
		Target goalTarget = null;
		Target currentTarget = null;
		int trialCount;
		Queue<(Point, Matrix<double>, long)> lastGazePositions = new Queue<(Point, Matrix<double>, long)>();
		PointF averagedGazePosition;
		Stopwatch stopwatch = new Stopwatch();
		GazeDisplayMode gazeDisplayMode = GazeDisplayMode.Individal;
		int mouseX, mouseY;

		// constructor
		public FormAreaSelectionTest(FormMain f)
		{
			InitializeComponent();

			this.formMain = f;
		}

		// event handlers
		private void FormAreaSelectionTest_Load(object sender, EventArgs e)
		{
			this.formMain.GazeUpdated += FormMain_GazeUpdated;
		}
		private void labelStartText_Click(object sender, EventArgs e)
		{
			this.labelStartText.Visible = false;

			StartTest();
		}
		private void FormAreaSelectionTest_Paint(object sender, PaintEventArgs e)
		{
			if (isTestStarted == false) return;

			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// clear window
			g.Clear(SystemColors.Control);

			// draw all targets
			float radius = settings.TargetRadius;
			float size = radius + radius;
			using (Brush b = new SolidBrush(settings.TargetColor))
			{
				foreach (Target p in targets)
				{
					if (p == goalTarget) continue;

					float x = p.Position.X;
					float y = p.Position.Y;

					float s = size + 4 * p.focusValue;
					g.FillEllipse(Brushes.Gray, x - radius - 2 * p.focusValue, y - radius - 2 * p.focusValue, s, s);

					if (p.focusValue < 4)
						g.FillEllipse(b, x - radius, y - radius, size, size);
					else
						g.FillEllipse(Brushes.Red, x - radius, y - radius, size, size);
				}
			}

			// draw goal target
			using (Brush b = new SolidBrush(settings.TargetColor2))
			{
				float x = goalTarget.Position.X;
				float y = goalTarget.Position.Y;
				g.FillEllipse(b, x - radius, y - radius, size, size);

			}

			int h = this.ClientSize.Height;
			int w = this.ClientSize.Width;
			int r = 5;

			if (gazeDisplayMode == GazeDisplayMode.Individal)
			{
				// draw recent gaze predictions
				foreach (var pair in lastGazePositions)
				{
					Point p = pair.Item1;
					// make sure drawing point within window
					int x = p.X.LimitToRange(0, w - 1);
					int y = p.Y.LimitToRange(0, h - 1);
					g.DrawLine(Pens.Red, x - r, y, x + r, y);
					g.DrawLine(Pens.Red, x, y - r, x, y + r);
				}
			}

			if (gazeDisplayMode == GazeDisplayMode.Average)
			{
				// make sure drawing point within window
				float x = averagedGazePosition.X.LimitToRange(0, w - 1);
				float y = averagedGazePosition.Y.LimitToRange(0, h - 1);
				g.DrawLine(Pens.Red, x - r, y, x + r, y);
				g.DrawLine(Pens.Red, x, y - r, x, y + r);
			}
		}
		private void FormAreaSelectionTest_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				FinishTrial();
			}

			else if (e.Button == MouseButtons.Right)
			{
				
			}
		}
		private void FormAreaSelectionTest_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				// press ESC to close the window
				case Keys.Escape:
					this.Close();
					break;

				// press number keys to set gaze display mode
				case Keys.D1:
					gazeDisplayMode = GazeDisplayMode.None;
					break;

				case Keys.D2:
					gazeDisplayMode = GazeDisplayMode.Individal;
					break;

				case Keys.D3:
					gazeDisplayMode = GazeDisplayMode.Average;
					break;

				// press key S to show settings dialog
				case Keys.S:
					FormSettings f = new FormSettings("Area Selection Settings", this.settings);
					f.ShowDialog(this);
					break;

				case Keys.V:
					timerVirtualGazeTimer.Enabled = !timerVirtualGazeTimer.Enabled;
					break;

			}
		}
		private void timerGazeLost_Tick(object sender, EventArgs e)
		{
			this.timerGazeLost.Stop();
			this.isGazeLost = true;

			this.Invalidate();
		}

		// gaze update handler
		private void FormMain_GazeUpdated(object o, FormMain.GazeEventArgs e)
		{
			// add new gaze position to queue
			lastGazePositions.Enqueue((e.PredictedGaze, e.EyeFeature, this.stopwatch.ElapsedMilliseconds));

			// remove oldest position if more than the given number
			while (lastGazePositions.Count > settings.NumOfGazePositions)
			{
				lastGazePositions.Dequeue();
			}

			// compute the averaged gaze position
			if (isFirstGazeUpdate)
			{
				averagedGazePosition = e.PredictedGaze;
				isFirstGazeUpdate = false;
			}
			else
			{
				averagedGazePosition =
					averagedGazePosition
					.Multiply(settings.AverageDecayRatio)
					.Add(
						e.PredictedGaze.Multiply(1.0f - settings.AverageDecayRatio)
					);
			}

			// set timer and gaze lost flag
			this.timerGazeLost.Stop();
			this.timerGazeLost.Interval = settings.GazeLostTime;
			this.timerGazeLost.Start();
			this.isGazeLost = false;

			// remember to request redrawing
			this.Invalidate();
		}


		// main procideures
		private void GenerateTargets()
		{
			targets.Clear();

			int margin = settings.MinDistance + settings.TargetRadius;
			int w = this.ClientSize.Width;
			int h = this.ClientSize.Height;
			int minDis = settings.MinDistance + settings.TargetRadius * 2;
			for (int i = 0; i < settings.NumOfTargets; i++)
			{
				Point pos;

				while (true)
				{
					pos = new Point(
						rand.Next(margin, w - margin),
						rand.Next(margin, h - margin)
						);
					// ensure trial distance
					if (targets.Count > 0 &&
						targets.Last().Position.DistanceFrom(pos) < settings.MinTrialDistance)
						continue;
					// ensure no two targets are too close to each other
					if (targets.Exists(p => p.Position.DistanceFrom(pos) < minDis) == false)
						break;
				}

				targets.Add(new Target(pos));
			}
		}
		private void StartTest()
		{
			isTestStarted = true;
			trialCount = 0;
		
			stopwatch.Start();

			GenerateTargets();

			StartTrial();
		}
		private void FinishTest()
		{
			// todo: summary and record trial record
		}
		private void StartTrial()
		{
			this.goalTarget = targets[trialCount % targets.Count];

			trialCount++;

			this.Invalidate();
		}
		private void FinishTrial()
		{
			// todo: summary and record trial record

			if (trialCount < settings.NumOfTrials)
			{
				StartTrial();
			}
			else
			{
				FinishTest();
			}
		}

		private void timerVirtualGazeTimer_Tick(object sender, EventArgs e)
		{
			// add new gaze position to queue
			Point p = new Point(mouseX + rand.Next(-100, 100), mouseY + rand.Next(-100, 100));
			lastGazePositions.Enqueue((p, null, this.stopwatch.ElapsedMilliseconds));

			// remove oldest position if more than the given number
			while (lastGazePositions.Count > settings.NumOfGazePositions)
			{
				lastGazePositions.Dequeue();
			}

			// compute the averaged gaze position
			if (isFirstGazeUpdate)
			{
				averagedGazePosition = p;
				isFirstGazeUpdate = false;
			}
			else
			{
				averagedGazePosition =
					averagedGazePosition
					.Multiply(settings.AverageDecayRatio)
					.Add(
						p.Multiply(1.0f - settings.AverageDecayRatio)
					);
			}

			// set timer and gaze lost flag
			this.timerGazeLost.Stop();
			this.timerGazeLost.Interval = settings.GazeLostTime;
			this.timerGazeLost.Start();
			this.isGazeLost = false;

			foreach (Target t in targets)
			{
				double dis = p.DistanceFrom(t.Position);
				double focus = Math.Max(1 - (dis / 200), 0);
				t.focusValue *= 0.8f;
				t.focusValue += (float)focus;

				//if (currentTarget == null &&
				//	t.focusValue > )
			}

			// remember to request redrawing
			this.Invalidate();
		}

		private void FormAreaSelectionTest_MouseMove(object sender, MouseEventArgs e)
		{
			this.mouseX = e.X;
			this.mouseY = e.Y;
		}
	}
}
