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
using System.Threading;

namespace GazeCalibration
{
	public partial class FormAreaSelectionTest : Form
	{
		class Settings
		{
			[Category("Test")] public int NumOfTrials { get; set; } = 30;
			[Category("Test")] public int MinTrialDistance { get; set; } = 300;

			[Category("Target")] public int NumOfTargets { get; set; } = 100;
			[Category("Target")] public int TargetRadius { get; set; } = 6; // pixels
			[Category("Target")] public Color TargetColor { get; set; } = Color.LimeGreen;
			[Category("Target")] public Color TargetColor2 { get; set; } = Color.Red;
			[Category("Target")] public Color TargetColor3 { get; set; } = Color.Orange;
			[Category("Target")] public int MinDistance { get; set; } = 10; // pixels
			[Category("Target")] public double SliverTriangleThreshold { get; set; } = Math.PI / 4; // radian
			[Category("Target")] public bool ShowTargetAdjacency { get; set; } = false;

			[Category("Fixation")] public bool ShowFixationRegion { get; set; } = false;
			[Category("Fixation")] [TypeConverter(typeof(ExpandableObjectConverter))] public FixationDetector FixationDetector { get; set; }
			
			[Category("Local Selection Region")] public int LocalSelectionRegionRadius { get; set; } = 150;
		}

		class Target
		{
			public Point Position { get; set; }
			public bool IsSelected { get; set; } = false;
			public List<Target> AdjacentTargets { get; private set; } = new List<Target>();

			public Target(Point pos)
			{
				Position = pos;
			}
		}

		// private enum
		enum GazeDisplayMode { None, Individal, Average };
		enum SelectionMethod { MouseWheel, MouseMovement };

		// private fields
		Random rand = new Random();
		Settings settings = new Settings();
		FormMain formMain = null;
		SelectionMethod selectionMethod = SelectionMethod.MouseMovement;

		// private fields for selection task
		bool isTestStarted = false;
		int trialCount;
		List<Target> targets = new List<Target>();
		Target goalTarget = null;
		Target currentTarget = null;
		Vector localSelectionRegionCenter;
		List<Target> localTargets = new List<Target>();
		int localSelectionIndex;
		Vector mouseDisplacement;

		// private fields for fixation detection
		FixationDetector fixationDetector = new FixationDetector();
		GazeDisplayMode gazeDisplayMode = GazeDisplayMode.Individal;

		// private fields for virtual gaze simulation
		int mouseX, mouseY;
		int lastMouseX, lastMouseY;

		// constructor
		public FormAreaSelectionTest(FormMain f)
		{
			InitializeComponent();

			this.formMain = f;

			this.settings.FixationDetector = this.fixationDetector;
		}

		// event handlers
		private void FormAreaSelectionTest_Load(object sender, EventArgs e)
		{
			this.formMain.GazeUpdated += FormMain_GazeUpdated;

			this.MouseWheel += FormAreaSelectionTest_MouseWheel;

			this.fixationDetector.RemoveGazePosition += delegate { this.Invalidate(); };
			this.fixationDetector.FixationLocked += FixationDetector_FixationLocked;
			this.fixationDetector.FixationReleased += FixationDetector_FixationReleased;
		}
		private void FormAreaSelectionTest_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.formMain.GazeUpdated -= FormMain_GazeUpdated;
		}
		private void FormAreaSelectionTest_Paint(object sender, PaintEventArgs e)
		{
			if (isTestStarted == false) return;

			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// clear window
			g.Clear(SystemColors.Control);

			DrawFixation(g);

			if(settings.ShowTargetAdjacency) DrawTargetAdjacency(g);

			DrawTargets(g);

			DrawGazePosition(g);
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
		private void FormAreaSelectionTest_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				FinishTrial();
			}

			else if (e.Button == MouseButtons.Right)
			{
				// nothing to preform yet
			}
		}
		private void FormAreaSelectionTest_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.selectionMethod == SelectionMethod.MouseMovement &&
				fixationDetector.FixationState == FixationState.Locked)
			{
				int dx = mouseX - lastMouseX;
				int dy = mouseY - lastMouseY;
				AddDragging(dx, dy);
			}

			this.lastMouseX = mouseX;
			this.lastMouseY = mouseY;
			this.mouseX = e.X;
			this.mouseY = e.Y;
		}
		private void FormAreaSelectionTest_MouseWheel(object sender, MouseEventArgs e)
		{
			if (fixationDetector.FixationState != FixationState.Locked) return;
			if (this.selectionMethod != SelectionMethod.MouseWheel) return;

			if (e.Delta < 0)
			{
				localSelectionIndex++;
			}
			else if (e.Delta > 0)
			{
				localSelectionIndex += this.localTargets.Count - 1;
			}
			localSelectionIndex %= this.localTargets.Count;

			this.currentTarget = localTargets.ElementAt(localSelectionIndex);
		}
		private void labelStartText_Click(object sender, EventArgs e)
		{
			this.labelStartText.Visible = false;

			StartTest();
		}
		private void timerVirtualGazeTimer_Tick(object sender, EventArgs e)
		{
			// add new gaze position to queue
			Point p = new Point(mouseX + rand.Next(-100, 100), mouseY + rand.Next(-100, 100));

			fixationDetector.AddGazePosition(p, null);

			this.Invalidate();
		}

		// gaze update handler
		private void FormMain_GazeUpdated(object o, FormMain.GazeEventArgs e)
		{
			fixationDetector.AddGazePosition(e.PredictedGaze, e.EyeFeature);

			this.Invalidate();
		}
		private void FixationDetector_FixationLocked(object sender, EventArgs e)
		{
			PrepareLocalTargets();
		}
		private void FixationDetector_FixationReleased(object sender, EventArgs e)
		{
			// nothing to preform yet
		}

		// main procideures for selection tasks
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

			FindTargetAdjacency();
		}
		private void FindTargetAdjacency()
		{
			// create Delaunay triangulation
			var triangulation = new DelaunayTriangulation(0, ClientSize.Width, 0, ClientSize.Height);
			foreach (Target t in targets)
			{
				triangulation.AddVertex(t.Position.X, t.Position.Y, t);
			}
			triangulation.RemoveBoundingBox(); // we do not need bounding vertices

			// generate target adjacency information
			foreach (var f in triangulation.Faces.Values)
			{
				for (int i = 0; i < 3; i++)
				{
					int j = (i + 1) % 3;
					Target t1 = f.Vertices[i].Tag as Target;
					Target t2 = f.Vertices[j].Tag as Target;
					if (!t1.AdjacentTargets.Contains(t2)) t1.AdjacentTargets.Add(t2);
					if (!t2.AdjacentTargets.Contains(t1)) t2.AdjacentTargets.Add(t1);
				}
			}

			// remove linkages when it is at sliver triangle
			foreach (var f in triangulation.Faces.Values)
			{
				for (int i = 0; i < 3; i++)
				{
					int j = (i + 1) % 3;
					if (f.Angles[i] + f.Angles[j] > settings.SliverTriangleThreshold) continue;

					Target t1 = f.Vertices[i].Tag as Target;
					Target t2 = f.Vertices[j].Tag as Target;
					t1.AdjacentTargets.Remove(t2);
					t2.AdjacentTargets.Remove(t1);
				}
			}
		}
		private void StartTest()
		{
			isTestStarted = true;
			trialCount = 0;
		
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

		// drawing procedures
		private void DrawFixation(Graphics g)
		{
			if (fixationDetector.FixationState == FixationState.Locked)
			{
				int r = settings.LocalSelectionRegionRadius;
				g.FillEllipse(Brushes.MistyRose,
					(float)(localSelectionRegionCenter.X - r),
					(float)(localSelectionRegionCenter.Y - r),
					(float)(r + r),
					(float)(r + r));
			}

			// draw fixation region
			if (settings.ShowFixationRegion)
			{
				double r = fixationDetector.FixationRegionRadius;
				PointF p = fixationDetector.AveragedGazePosition;
				g.FillEllipse(Brushes.LightGray, 
					(float)(p.X - r),
					(float)(p.Y - r),
					(float)(r + r),
					(float)(r + r));
			}
		}
		private void DrawTargets(Graphics g)
		{
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

					g.FillEllipse(b, x - radius, y - radius, size, size);
				}
			}

			// draw goal target
			if (goalTarget != null)
				using (Brush b = new SolidBrush(settings.TargetColor2))
				{
					float x = goalTarget.Position.X;
					float y = goalTarget.Position.Y;
					g.FillEllipse(b, x - radius, y - radius, size, size);

				}

			// draw current target
			if (fixationDetector.FixationState == FixationState.Locked && currentTarget != null)
				using (Brush b = new SolidBrush(settings.TargetColor3))
				{
					float x = currentTarget.Position.X;
					float y = currentTarget.Position.Y;
					g.FillEllipse(b, x - radius, y - radius, size, size);

				}
		}
		private void DrawGazePosition(Graphics g)
		{
			int h = this.ClientSize.Height;
			int w = this.ClientSize.Width;

			if (gazeDisplayMode == GazeDisplayMode.Individal)
			{
				int r = 5;
				// draw recent gaze predictions
				foreach (var pair in fixationDetector.LastGazePositions)
				{
					Point p = pair.gazePosition;
					// make sure drawing point within window
					int x = p.X.LimitToRange(0, w - 1);
					int y = p.Y.LimitToRange(0, h - 1);
					g.DrawLine(Pens.Red, x - r, y, x + r, y);
					g.DrawLine(Pens.Red, x, y - r, x, y + r);
				}
			}
			else if (gazeDisplayMode == GazeDisplayMode.Average)
			{
				int r = 5;
				PointF p = fixationDetector.AveragedGazePosition;
				// make sure drawing point within window
				float x = p.X.LimitToRange(0, w - 1);
				float y = p.Y.LimitToRange(0, h - 1);
				g.DrawLine(Pens.Red, x - r, y, x + r, y);
				g.DrawLine(Pens.Red, x, y - r, x, y + r);
			}
		}
		private void DrawTargetAdjacency(Graphics g)
		{
			foreach (Target t1 in targets)
			{
				foreach (Target t2 in t1.AdjacentTargets)
				{
					g.DrawLine(Pens.Gray, t1.Position.X, t1.Position.Y, t2.Position.X, t2.Position.Y);
				}
			}
		}

		// functions for supporting target selection
		private void PrepareLocalTargets()
		{
			// set local region center as current average gaze position
			PointF center = this.fixationDetector.AveragedGazePosition;

			// collect local targets within in region, ordered by Y coordinates
			var selectedTargets = targets
				.Where(t => center.DistanceFrom(t.Position) < settings.LocalSelectionRegionRadius)
				.OrderBy(t => t.Position.Y);

			// find the closest target and its index
			var (minTarget, minIndex, _) = selectedTargets
				.Select((t, i) => (t, i, d: center.DistanceFrom(t.Position)))
				.Aggregate((min, next) => min.d < next.d ? min : next);

			this.localSelectionRegionCenter = center;
			this.localTargets = selectedTargets.ToList();
			this.localSelectionIndex = minIndex;
			this.currentTarget = minTarget;
			this.mouseDisplacement = new Point(0, 0);
		}
		private void AddDragging(int dx, int dy)
		{
			// add to overall displcaement vector
			mouseDisplacement += new Vector(dx, dy);

			// if overall displacement excesses specified threshold
			if (mouseDisplacement.Norm > 20)
			{
				// find the adjacent target in the direction closest to the displacement vector
				PointF u = mouseDisplacement.Normalize();
				(Target maxTarget, float maxDot) = currentTarget.AdjacentTargets
					.Select(t => (t, dot: ((Vector)(t.Position) - currentTarget.Position).Normalize().Dot(u)))
					.Aggregate((max, next) => max.dot > next.dot ? max : next);
				
				// reset the displacement vector
				mouseDisplacement = new Vector(0, 0);

				// move to the found adjacent target only if dot product greater than given threshold
				if (maxDot > 0.2f)
				{
					this.currentTarget = maxTarget;

					// remember to request window redraw
					this.Invalidate();
				}
			}
		}
	}
}
