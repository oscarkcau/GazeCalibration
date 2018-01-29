using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace GazeCalibration
{
	public partial class FormCalibration : Form
	{
		// settings class
		private class Settings
		{
			public float[] GridLinePositions { get; set; } = { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };
			public int GazeLostTime { get; set; } = 500;
		}

		// private fields
		Settings settings = new Settings();
		FormMain formMain;
		FormMain.GazeEventArgs lastFeature = null;
		List<Point> clickedPoints = new List<Point>();
		Queue<Matrix<double>> recentFeatures = new Queue<Matrix<double>>();
		
		// constructor
		public FormCalibration(FormMain main)
		{
			InitializeComponent();

			formMain = main;
		}

		// control event handlers
		private void FormCalibration_Load(object sender, EventArgs e)
		{
			// register gaze event handler
			formMain.GazeUpdated += FormMain_GazeUpdated;
		}
		private void FormCalibration_FormClosed(object sender, FormClosedEventArgs e)
		{
			// unregister gaze event handler
			formMain.GazeUpdated -= FormMain_GazeUpdated;
		}
		private void FormCalibration_MouseClick(object sender, MouseEventArgs e)
		{
			Point p = new Point(e.X, e.Y);

			// add click sample if feature exist
			if (lastFeature != null)
			{
				clickedPoints.Add(p);

				formMain.RegressionX.AddFeatures(recentFeatures, p.X);
				formMain.RegressionY.AddFeatures(recentFeatures, p.Y);
			}

			// request to redraw window
			this.Invalidate();
		}
		private void FormCalibration_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				// press ESC to close the window
				case Keys.Escape:
					this.Close();
					break;

				// press key S to show settings dialog
				case Keys.S:
					FormSettings f = new FormSettings("Calibration Settings", this.settings);
					f.ShowDialog(this);
					break;
			}
		}
		private void FormCalibration_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// clear window
			g.Clear(SystemColors.Control);

            // draw grid lines
            int h = this.ClientSize.Height;
            int w = this.ClientSize.Width;
            foreach (float pos in settings.GridLinePositions)
            {
                g.DrawLine(Pens.Green, 0, pos * h, w, pos * h);
                g.DrawLine(Pens.Green, pos * w, 0, pos * w, h);
            }

            // draw status of last eye detection and feature extraction
            Brush b = this.lastFeature != null ? Brushes.Green : Brushes.Red;
			float radius = 15;
			float centerX = (w / 2);
			float centerY = 30;
			g.FillEllipse(b, centerX - radius, centerY - radius,
						  radius + radius, radius + radius);

			// draw all sample click positions
			int r = 5;
			foreach (Point p in clickedPoints)
			{
                g.DrawEllipse(Pens.Blue, p.X - r, p.Y - r, r * 2, r * 2);
			}

			// draw gaze prediction
			if (this.lastFeature != null)
			{
				Point p = this.lastFeature.PredictedGaze;
				// make sure drawing point within window
				int x = p.X.LimitToRange(0, w - 1);
				int y = p.Y.LimitToRange(0, h - 1);
				g.DrawLine(Pens.Red, x - r, y, x + r, y);
				g.DrawLine(Pens.Red, x, y - r, x, y + r);
			}
		}

		// gaze event handler
		private async void FormMain_GazeUpdated(object o, FormMain.GazeEventArgs e)
		{
			// update last feature
			this.lastFeature = e;

			// store feature to queue
			recentFeatures.Enqueue(e.EyeFeature);

			// request to redraw window
			this.Invalidate();

			// schedule to remove feature after specified time 
			await Task.Delay(settings.GazeLostTime);
			recentFeatures.Dequeue();
		}

	}
}
