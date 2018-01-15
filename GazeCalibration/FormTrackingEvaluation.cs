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
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace GazeCalibration
{
	public partial class FormTrackingEvaluation : Form
	{
		// public setting class
		public class Settings
		{
			public int NumOfGazePositions { get; set; } = 10;
			public float AverageDecayRatio { get; set; } = 0.8f;
			public int GazeLostTime { get; set; } = 500;
			public bool UpdateTrackingModel { get; set; } = true;
			public int TargetRadius { get; set; } = 20;
			public Color TargetColor { get; set; } = Color.Green;
			public Color TargetColor2 { get; set; } = Color.Red;
			public float[] GridLinePositions { get; set; } = { 0.05f, 0.2f, 0.4f, 0.6f, 0.8f, 0.95f };
		}
		
		// private enum
		enum GazeDisplayMode { None, Individal, Average };

		// private field 
		Settings settings = new Settings();
		FormMain formMain = null;
		Queue<(Point, Matrix<double>, long)> lastGazePositions = new Queue<(Point, Matrix<double>,long)>();
		List<Point> testPoints;
		Point currentTestPoint;
		PointF averagedGazePosition;
		GazeDisplayMode gazeDisplayMode = GazeDisplayMode.Individal;
		bool isTestStated = false;
		bool isFirstGazeUpdate = true;
		bool isGazeLost = false;
		Stopwatch stopwatch = new Stopwatch();

		List<(Point, Point, long)> trackTrialRecords = new List<(Point, Point, long)>();

		// constructor
		public FormTrackingEvaluation(FormMain f)
		{
			InitializeComponent();

			this.formMain = f;
		}

		// event handlers
		private void FormTrackingEvaluation_Load(object sender, EventArgs e)
		{
			this.formMain.GazeUpdated += FormMain_GazeUpdated;
		}
		private void FormTrackingEvaluation_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.formMain.GazeUpdated -= FormMain_GazeUpdated;
		}
		private void FormTrackingEvaluation_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// clear window
			g.Clear(SystemColors.Control);

			// draw current test position
			using (Pen b = isGazeLost ? new Pen(settings.TargetColor2,2) : new Pen(settings.TargetColor,2))
			{
				float centerX = currentTestPoint.X;
				float centerY = currentTestPoint.Y;
				float radius = settings.TargetRadius;
				g.DrawEllipse(b, centerX - radius, centerY - radius,
							  radius + radius, radius + radius);
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
		private void FormTrackingEvaluation_KeyDown(object sender, KeyEventArgs e)
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
					FormSettings f = new FormSettings("Tracking Evaluation Settings", this.settings);
					f.ShowDialog(this);
					break;

				// press ENTER to start test
				case Keys.Enter:
					if (isTestStated == false)
					{
						StartTest();
					}
					break;
				
				// press SPACE to complete a Trial
				case Keys.Space:
					if (isTestStated)
					{
						FinishedTrial();

						if (testPoints.Count > 0)
							StartTrial();
						else
							FinishedTest();
					}
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
			if (lastGazePositions.Count > settings.NumOfGazePositions)
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

		// main procedure
		private void GenerateTestPositions()
		{
			int w = this.ClientSize.Width;
			int h = this.ClientSize.Height;

			// generate all points on grid
			this.testPoints = new List<Point>();
			foreach (float x in settings.GridLinePositions)
				foreach (float y in settings.GridLinePositions)
					testPoints.Add(new Point((int)(x * w), (int)(y * h)));

			// shuffle the list
			Random rand = new Random();
			for (int i=0; i<testPoints.Count; i++)
			{
				int index = rand.Next(i, testPoints.Count);
				Point tmp = testPoints[i];
				testPoints[i] = testPoints[index];
				testPoints[index] = tmp;
			}
		}
		private void StartTrial()
		{
			int n = testPoints.Count;

			currentTestPoint = testPoints[n - 1];
			testPoints.RemoveAt(n - 1);

			this.Invalidate();
		}
		private void FinishedTrial()
		{
			long currentTime = stopwatch.ElapsedMilliseconds;

			// record trial results
			// and add new samples to regression models
			foreach (var tuple in this.lastGazePositions)
			{
				Point gazePosition = tuple.Item1;
				long timeGap = currentTime - tuple.Item3;

				// only process if the gaze tracking occurs within given time period before key press event
				if (timeGap > settings.GazeLostTime) continue;
				
				// record trial result
				this.trackTrialRecords.Add((currentTestPoint, gazePosition, timeGap));

				// update tracking models
				if (settings.UpdateTrackingModel)
				{
					// todo: update regression tracking models
					formMain.AddClickSample(currentTestPoint, tuple.Item2);
				}
			}
		}
		private void StartTest()
		{
			this.labelStartText.Visible = false;
			this.isTestStated = true;

			this.stopwatch.Start();

			GenerateTestPositions();

			StartTrial();
		}
		private void FinishedTest()
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
					foreach (var pair in trackTrialRecords)
					{
						Point testPoint = pair.Item1;
						Point gazePosition = pair.Item2;
						writetext.WriteLine(testPoint.X + "," + testPoint.Y + "," + gazePosition.X + "," + gazePosition.Y);
					}
				}
			}

			// clos the window
			this.Close();
		}

	}
}
