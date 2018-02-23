using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GazeCalibration
{
	public partial class FormSelectionTest : Form
	{
		public const UInt32 SPI_SETMOUSESPEED = 0x0071;

		[DllImport("User32.dll")]
		static extern Boolean SystemParametersInfo(
			UInt32 uiAction,
			UInt32 uiParam,
			UInt32 pvParam,
			UInt32 fWinIni);

		// public setting class
		public class Settings
		{
			[Category("Test")] public int NumOfTrials { get; set; } = 50;
			[Category("Target")] public int MinimumDistance { get; set; } = 200;
			[Category("Target")] [XmlElement(Type = typeof(XmlColor))] public Color TargetColor { get; set; } = Color.Green;
			[Category("Target")] [XmlElement(Type = typeof(XmlColor))] public Color TargetOverColor { get; set; } = Color.Red;
			[Category("Test Factor")] public int TargetRadius { get; set; } = 10;
			[Category("Test Factor")] public int DefaultMouseSpeed { get; set; } = 12;
			[Category("Test Factor")] public int MinimumMouseSpeed { get; set; } = 5;
			[Category("Test Factor")] public int MouseSpeedAdjustmentRadius { get; set; } = 200;
			[Category("Gaze Tracking")] public bool UpdateRegressionModel { get; set; } = true;
			[Category("Fixation")] [TypeConverter(typeof(ExpandableObjectConverter))] public FixationDetector FixationDetector { get; set; }
			[Category("Fixation")] public bool ShowFixitation { get; set; } = false;
		}

		enum GazeDisplayMode { None, Individal, Average };

		// private fields
		Random rand = new Random();
		FormMain formMain = null;
		Settings settings = null;
		FixationDetector fixationDetector = new FixationDetector();
		GazeDisplayMode gazeDisplayMode = GazeDisplayMode.Average;

		int trialCount = 0;
		int errorCount = 0;
		bool isTestStarted = false;
		bool isMouseOverTarget = false;
		Point startPoint, currentTarget;
		Point mouseLeftDownPoint, mouseRightDownPoint;
		long trialStartTime;
		List<(Point startPoint, Point TargetPoint, long time, int error)>
			trackTrialRecords = new List<(Point, Point, long, int)>();

		// constructor
		public FormSelectionTest(FormMain form)
		{
			InitializeComponent();

			formMain = form;
			settings = settings ?? new Settings();
			settings.FixationDetector = this.fixationDetector;

			formMain.GazeUpdated += FormMain_GazeUpdated;
			fixationDetector.FixationLocked += delegate { this.Invalidate(); };
			fixationDetector.FixationReleased += delegate { this.Invalidate(); };
			fixationDetector.RemoveGazePosition += delegate { this.Invalidate(); };
		}

		// event handlers
		private void labelStartText_Click(object sender, EventArgs e)
		{
			StartTest();
		}
		private void FormSelectionTest_MouseMove(object sender, MouseEventArgs e)
		{
			if (isTestStarted == false) return;
			if (Form.ActiveForm != this) return;

			// compute distances from cursor 
			PointF mousePos = new PointF(e.X, e.Y);
			double distFromTarget = mousePos.DistanceFrom(currentTarget);
			double distFromFixation = mousePos.DistanceFrom(fixationDetector.AveragedGazePosition);

			// check cursor over target
			bool oldValue = isMouseOverTarget;
			isMouseOverTarget = (distFromTarget < settings.TargetRadius);

			// update mouse speed
			int min = settings.MinimumMouseSpeed;
			int max = settings.DefaultMouseSpeed;
			int r = settings.MouseSpeedAdjustmentRadius;
			int sp = (int)distFromFixation.MapTo(0, r, min, max);
			sp = sp.LimitToRange(min, max);
			SystemParametersInfo(SPI_SETMOUSESPEED, 0, (uint)sp, 0);
			label1.Text = sp.ToString();

			// request redrawing if in need
			if (isMouseOverTarget != oldValue)
				this.Invalidate();
		}
		private void FormSelectionTest_MouseDown(object sender, MouseEventArgs e)
		{
			// record mouse click position
			if (e.Button == MouseButtons.Right) mouseRightDownPoint = new Point(e.X, e.Y);
			if (e.Button == MouseButtons.Left) mouseLeftDownPoint = new Point(e.X, e.Y);

			// only process left click when test started
			if (isTestStarted == false) return;
			if (e.Button != MouseButtons.Left) return;

			// if left click on target
			if (isMouseOverTarget)
			{
				FinishTrial();

				// start a new trial if more trial to go
				if (trialCount < settings.NumOfTrials)
					StartTrial();
				else
					FinishTest(); // or else finish test at the end

				this.Invalidate();
			}
			else // error click 
			{
				this.errorCount++;
			}
		}
		private void FormSelectionTest_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// clear window
			g.Clear(SystemColors.Control);

			if (isTestStarted == false) return;

			if (settings.ShowFixitation) DrawFixation(g);

			DrawTarget(g);

			DrawGazePosition(g);
		}
		private void FormSelectionTest_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					this.Close();
					break;

				case Keys.S:
					FormSettings f = new FormSettings("Selection Test Settings", this.settings);
					f.ShowDialog(this);
					break;

				case Keys.V:
					this.TimerVirtualGaze.Enabled = !this.TimerVirtualGaze.Enabled;
					break;

				case Keys.Q:
					this.gazeDisplayMode = GazeDisplayMode.None;
					break;

				case Keys.W:
					this.gazeDisplayMode = GazeDisplayMode.Average;
					break;

				case Keys.E:
					this.gazeDisplayMode = GazeDisplayMode.Individal;
					break;

				case Keys.R:
					this.settings.ShowFixitation = !this.settings.ShowFixitation;
					break;

			}
		}
		private void TimerVirtualGaze_Tick(object sender, EventArgs e)
		{
			Point p = new Point(
				(int)mouseRightDownPoint.X + rand.Next(-100, 100),
				(int)mouseRightDownPoint.Y + rand.Next(-100, 100)
				);

			fixationDetector.AddGazePosition(p, null);

			this.Invalidate();
		}
		private void FormMain_GazeUpdated(object o, FormMain.GazeEventArgs e)
		{
			this.fixationDetector.AddGazePosition(e.PredictedGaze, e.EyeFeature);

			this.Invalidate();
		}

		// main procedures
		private void StartTest()
		{
			labelStartText.Visible = false;
			isTestStarted = true;

			// start the first trial
			StartTrial();
		}
		private void FinishTest()
		{
			// save trial results to file in csv format
			// and close the window after saving

			// ask for folder location and filename 
			SaveFileDialog d = new SaveFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
			d.OverwritePrompt = true;

			// save all trial records
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				using (StreamWriter writetext = new StreamWriter(d.FileName))
				{
					foreach (var (start, target, time, err) in trackTrialRecords)
					{
						writetext.WriteLine(
							start.X + "," +
							start.Y + "," +
							target.X + "," +
							target.Y + "," +
							time + "," +
							err
							);
					}
				}
			}

			// clos the window
			this.Close();
		}
		private void StartTrial()
		{
			// increase trial count
			trialCount++;

			// reset error count
			errorCount = 0;

			// generate new target point
			int margin = settings.TargetRadius;
			int w = this.ClientSize.Width;
			int h = this.ClientSize.Height;
			Point newTarget = new Point(
				rand.Next(margin, w - margin), 
				rand.Next(margin, h - margin)
				);

			// make sure new target is far enough
			if (trialCount > 1)
			while (newTarget.DistanceFrom(currentTarget) < settings.MinimumDistance)
			{
				newTarget = new Point(rand.Next(w), rand.Next(h));
			}

			// update the start point and current target
			startPoint = currentTarget;
			currentTarget = newTarget;

			// update trial start time
			trialStartTime = fixationDetector.ElapsedMilliseconds;
		}
		private void FinishTrial()
		{
			// summarize and record trial record
			long trialFinishTime = fixationDetector.ElapsedMilliseconds;

			trackTrialRecords.Add(
				(startPoint, currentTarget, trialFinishTime - trialStartTime, errorCount)
				);

			// add features to regression models
			if (settings.UpdateRegressionModel)
			{
				foreach (var (_, feature, _) in fixationDetector.LastGazePositions)
				{
					if (feature == null) continue;
					formMain.RegressionX.AddFeature(feature, currentTarget.X, false);
					formMain.RegressionY.AddFeature(feature, currentTarget.Y, false);
				}
				formMain.RegressionX.UpdateWeights();
				formMain.RegressionY.UpdateWeights();
			}
		}
		private double Gaussian(double x, double sd)
		{
			// assume zero mean
			const double inv2PI = 0.39894228040143267793994605993438; // 1.0 / Math.Sqrt(2 * Math.PI);

			double factor = inv2PI / sd;
			double tmp = x / sd;
			double power = -0.5 * (tmp * tmp);

			return factor * Math.Exp(power);
		}
		private void DrawFixation(Graphics g)
		{
			// draw fixation region
			if (settings.ShowFixitation &&
				fixationDetector.FixationState == FixationState.Locked)
			{
				float r = (float)fixationDetector.FixationRegionRadius;
				PointF p = fixationDetector.AveragedGazePosition;
				g.FillEllipse(Brushes.LightGray,
					(p.X - r),
					(p.Y - r),
					(r + r),
					(r + r)
					);

				r = 200;
				g.DrawEllipse(Pens.Blue,
					(p.X - r),
					(p.Y - r),
					(r + r),
					(r + r)
					);
			}
		}
		private void DrawGazePosition(Graphics g)
		{
			int h = this.ClientSize.Height;
			int w = this.ClientSize.Width;

			if (gazeDisplayMode == GazeDisplayMode.Individal)
			{
				int r = 5;
				Pen pen = fixationDetector.FixationState == FixationState.Locked ? Pens.Green : Pens.Red;
				// draw recent gaze predictions
				foreach (var pair in fixationDetector.LastGazePositions)
				{
					Point p = pair.gazePosition;
					// make sure drawing point within window
					int x = p.X.LimitToRange(0, w - 1);
					int y = p.Y.LimitToRange(0, h - 1);
					g.DrawLine(pen, x - r, y, x + r, y);
					g.DrawLine(pen, x, y - r, x, y + r);
				}
			}
			else if (gazeDisplayMode == GazeDisplayMode.Average)
			{
				int r = 5;
				Pen pen = fixationDetector.FixationState == FixationState.Locked ? Pens.Green : Pens.Red;
				PointF p = fixationDetector.AveragedGazePosition;
				// make sure drawing point within window
				float x = p.X.LimitToRange(0, w - 1);
				float y = p.Y.LimitToRange(0, h - 1);
				g.DrawLine(pen, x - r, y, x + r, y);
				g.DrawLine(pen, x, y - r, x, y + r);
			}
		}
		private void DrawTarget(Graphics g)
		{
			using (Brush b = isMouseOverTarget ? new SolidBrush(settings.TargetColor) : new SolidBrush(settings.TargetOverColor))
			{
				float centerX = currentTarget.X;
				float centerY = currentTarget.Y;
				float radius = settings.TargetRadius;
				g.FillEllipse(b, centerX - radius, centerY - radius,
							  radius + radius, radius + radius);
			}
		}
	}
}
