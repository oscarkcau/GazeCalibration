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
	public partial class FormCalibration : Form
	{
		// private fields
		private static readonly Object lockObject = new Object();
		FormMain formMain;
		FormMain.Feature lastFeature = null;
		List<Point> clickedPoints = new List<Point>();
        float[] gridLinePositions = { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };

		// constructor
		public FormCalibration(FormMain main)
		{
			InitializeComponent();

			formMain = main;
		}

		// control event handlers
		private void FormCalibration_MouseClick(object sender, MouseEventArgs e)
		{
			Point p = new Point(e.X, e.Y);

			// add click sample if feature exist
			lock (lockObject)
			{
				if (lastFeature != null)
				{
					clickedPoints.Add(p);
					formMain.AddClickSample(p, lastFeature);
				}
			}

			// request to redraw window
			this.Invalidate();
		}
		private void FormCalibration_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// clear window
			g.Clear(SystemColors.Control);

            // draw grid lines
            int h = this.ClientSize.Height;
            int w = this.ClientSize.Width;
            foreach (float pos in gridLinePositions)
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
				//g.DrawLine(Pens.Blue, p.X - r, p.Y, p.X + r, p.Y);
				//g.DrawLine(Pens.Blue, p.X, p.Y - r, p.X, p.Y + r);
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

		// public methods
		public void UpdateGazeCaptureState(FormMain.Feature feature)
		{
			// update last feature
			lock (lockObject)
			{
				this.lastFeature = feature;
			}

			// request to redraw window
			this.Invalidate();
		}
	}

	// helper extension class
	public static class InputExtensions {
		public static int LimitToRange(
			this int value, int inclusiveMinimum, int inclusiveMaximum) {
			if (value < inclusiveMinimum) { return inclusiveMinimum; }
			if (value > inclusiveMaximum) { return inclusiveMaximum; }
			return value;
		}
	}
}
