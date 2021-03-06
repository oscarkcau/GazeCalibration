﻿using System;
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
using System.Xml.Serialization;
using System.IO;

namespace GazeCalibration
{
	public partial class FormMain : Form
	{
		public class Settings
		{
			[Category("Face Detection")] public double FaceDection_ScaleFactor { get; set; } = 1.1;
			[Category("Face Detection")] public int FaceDection_MinNeighbors { get; set; } = 10;
			[Category("Face Detection")] public int FaceDection_MinSize { get; set; } = 160;

			[Category("Eye Detection")] public double EyeDection_ScaleFactor { get; set; } = 1.1;
			[Category("Eye Detection")] public int EyeDection_MinNeighbors { get; set; } = 10;
			[Category("Eye Detection")] public int EyeDection_MinSize { get; set; } = 40;

			[Category("Pupil Detection")] public bool EnablePupilDection { get; set; } = false;
			[Category("Pupil Detection")] public double PupilDection_ScaleFactor { get; set; } = 1.1;
			[Category("Pupil Detection")] public int PupilDection_MinScale { get; set; } = 10;
			[Category("Pupil Detection")] public int PupilDection_OffsetFactor { get; set; } = 4;

			[Category("Feature Extraction")] public int FeatureExtraction_PatchWidth { get; set; } = 10;
			[Category("Feature Extraction")] public int FeatureExtraction_PatchHeight { get; set; } = 6;

			[Category("Display")] public bool ShowCapturedImage { get; set; } = true;
			[Category("Display")] public bool ShowFaceBox { get; set; } = true;
			[Category("Display")] public bool ShowEyeBox { get; set; } = true;
			[Category("Display")] public bool ShowPupilBox { get; set; } = true;
			[Category("Display")] public bool ShowFeatures { get; set; } = true;

			[Category("Camera")] public int CameraIndex { get; set; } = 0;

			public Settings() { }
		}

		public class GazeEventArgs : EventArgs
		{
			public Matrix<double> EyeFeature { get; set; }
			public Point PredictedGaze { get; set; }
		}
		public delegate void GazeUpdatedHandler(object o, GazeEventArgs e);
		public event GazeUpdatedHandler GazeUpdated;

		// private fields for capture eye feature
		Settings settings = new Settings();
		VideoCapture capture = null;
		Mat frame = new Mat();
		Mat processedFrame = new Mat();
		List<Rectangle> faces = new List<Rectangle>();
		List<Rectangle> eyes = new List<Rectangle>();
		List<Rectangle> pupils = new List<Rectangle>();
		List<Mat> features = new List<Mat>();
		string faceFileName = "haarcascade_frontalface_default.xml";
		string eyeFileName = "haarcascade_eye.xml";
		CascadeClassifier faceClassifier = null;
		CascadeClassifier eyeClassifier = null;

		// local caches 
		Mat _concatedFeature = new Mat();
		Mat _concatedFeatureDouble = new Mat();
		Mat _scaledDown = new Mat();
		Mat _scaledUp = new Mat();

		// public Properties for calibration
		public RidgeRegression RegressionX { get; private set; } = null;
		public RidgeRegression RegressionY { get; private set; } = null;

		// constructor
		public FormMain()
		{
			InitializeComponent();

			LoadDefaultSettings();

			this.propertyGridSettings.SelectedObject = settings;

			int n = (settings.FeatureExtraction_PatchWidth * settings.FeatureExtraction_PatchHeight) * 2 + 5;
			RegressionX = new RidgeRegression(n);
			RegressionY = new RidgeRegression(n);

			faceClassifier = new CascadeClassifier(faceFileName);
			eyeClassifier = new CascadeClassifier(eyeFileName);
		}

		// control event handlers
		private void buttonSaveDefaultSettings_Click(object sender, EventArgs e)
		{
			// show dialog
			DialogResult ret = MessageBox.Show(
				this,
				"Save current settings as default settings?",
				this.Text,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1
				);

			// save as default settings if OK was selected
			if (ret == DialogResult.OK)
			{
				string filename = AppDomain.CurrentDomain.BaseDirectory + @"\default_settings.xml";
				SaveSettings(filename);
			}
		}
		private void buttonSaveSettings_Click(object sender, EventArgs e)
		{
			SaveFileDialog d = new SaveFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			d.OverwritePrompt = true;

			if (d.ShowDialog(this) == DialogResult.OK)
			{
				SaveSettings(d.FileName);
			}
		}
		private void buttonLoadSettings_Click(object sender, EventArgs e)
		{
			OpenFileDialog d = new OpenFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			d.CheckFileExists = true;

			if (d.ShowDialog(this) == DialogResult.OK)
			{
				LoadSettings(d.FileName);
			}
		}
		private void toolStripButtonStartCapture_Click(object sender, EventArgs e)
		{
			// close currently opened webcam (if any)
			if (capture != null)
			{
				if (capture.IsOpened) capture.Stop();
				Application.Idle -= ProcessFrame;
				//capture.ImageGrabbed -= ProcessFrame;
				capture.Dispose();
				capture = null;
			}

			// start the webcam with specified camera index
			try
			{
				capture = new VideoCapture(settings.CameraIndex);
				Application.Idle += ProcessFrame;
				//capture.ImageGrabbed += ProcessFrame;
				capture.Start();
			}
			catch (NullReferenceException excpt)
			{
				MessageBox.Show(excpt.Message);
			}
		}
		private void toolStripButtonGazeCalibration_Click(object sender, EventArgs e)
		{
			FormCalibration f = new FormCalibration(this);
			f.Show();
		}
		private void toolStripButtonSelectionTest_Click(object sender, EventArgs e)
		{
			FormSelectionTest f = new FormSelectionTest(this);
			f.Show();
		}
		private void toolStripButtonTrackingEvaluation_Click(object sender, EventArgs e)
		{
			FormTrackingEvaluation f = new FormTrackingEvaluation(this);
			f.Show();
		}
		private void toolStripButtonAreaSelectionTest_Click(object sender, EventArgs e)
		{
			FormAreaSelectionTest f = new FormAreaSelectionTest(this);
			f.Show(this);
		}
		private void toolStripButtonSaveModel_Click(object sender, EventArgs e)
		{
			SaveFileDialog d = new SaveFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			d.OverwritePrompt = true;

			if (d.ShowDialog(this) == DialogResult.OK)
			{
				SaveRegressionModel(d.FileName);
			}
		}
		private void toolStripButtonLoadModel_Click(object sender, EventArgs e)
		{
			OpenFileDialog d = new OpenFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			d.CheckFileExists = true;

			if (d.ShowDialog(this) == DialogResult.OK)
			{
				LoadRegressionModel(d.FileName);
			}
		}

		// main procedures
		private void ProcessFrame(object sender, EventArgs e)
		{
			// retrieve and handle new frame

			if (capture == null || capture.Ptr == IntPtr.Zero) return;

			capture.Retrieve(frame, 0);

			// detect face, eye, pupil and eye feature vector
			DetectFeatures();

			// show captured frame
			if (settings.ShowCapturedImage)
			{
				imageBoxCapture.Image = frame;
			}

			// check for current detection status 
			bool isDetectionOkay =
				faces.Count == 1 &&
				eyes.Count == 2 &&
				faces[0].UpperLeftRegion().ContainPoint(eyes[0].Center()) &&
				faces[0].UpperRightRegion().ContainPoint(eyes[1].Center());

			// prepare feature record and pass to calibration window
			GazeEventArgs record = null;
			if (isDetectionOkay)
			{
				// merge two feature vectors to one row vector
				CvInvoke.HConcat(features[0], features[1], _concatedFeature);

				// convert it into double type
				_concatedFeature.ConvertTo(_concatedFeatureDouble, DepthType.Cv64F);

				// convert it into double matrix and add padding values for constant term
				Point e1 = eyes[0].Center();
				Point e2 = eyes[1].Center();
				Point f = faces[0].Center();
				double f_width = faces[0].Width;
				double f_height = faces[0].Height;
				Matrix<double> padding = new Matrix<double>(
					new double[,] {
							{
								1,
								(e1.X - f.X) / f_width,
								(e1.Y - f.Y) / f_height,
								(e2.X - f.X) / f_width,
								(e2.Y - f.Y) / f_height
							}
					}
				);
				Matrix<double> featureVector = new Matrix<double>(_concatedFeature.Rows, _concatedFeature.Cols + 5);
				CvInvoke.HConcat(_concatedFeatureDouble, padding, featureVector);

				// also compute the predict value using current regression models
				int x = (int)RegressionX.Predict(featureVector);
				int y = (int)RegressionY.Predict(featureVector);

				// prepare feature record
				record = new GazeEventArgs
				{
					EyeFeature = featureVector,
					PredictedGaze = new Point((int)x, (int)y)
				};

				// show predicted gaze position
				this.toolStripStatusLabelMain.Text = x + " " + y;
			}

			// if calibration window is shown, prepare feature record and pass to it
			if (GazeUpdated != null && record != null)
			{
				GazeUpdated(this, record);
			}
		}
		private void DetectFeatures()
		{
			// clear previous detected boxes and feature vectors
			this.faces.Clear();
			this.eyes.Clear();
			this.pupils.Clear();
			this.features.Clear();

			// preprocess input frame - convert to gray scale and equalize histogram 
			CvInvoke.CvtColor(frame, processedFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
			CvInvoke.EqualizeHist(processedFrame, processedFrame);

			// detect faces
			Rectangle[] facesDetected = faceClassifier.DetectMultiScale(
				processedFrame,
				this.settings.FaceDection_ScaleFactor,
				this.settings.FaceDection_MinNeighbors,
				new Size(this.settings.FaceDection_MinSize, this.settings.FaceDection_MinSize)
				);
			// record detected faces
			faces.AddRange(facesDetected);

			// foreach detected face - detect eyes
			foreach (Rectangle f in facesDetected)
			{
				//Get the region of interest on the faces
				using (Mat faceRegion = new Mat(processedFrame, f))
				{
					// detect eyes
					Rectangle[] eyesDetected = eyeClassifier.DetectMultiScale(
						faceRegion,
						this.settings.EyeDection_ScaleFactor,
						this.settings.EyeDection_MinNeighbors,
						new Size(this.settings.EyeDection_MinSize, this.settings.EyeDection_MinSize)
						);

					// make sure the left eye has index 0, right eye has index 1
					if (eyesDetected.Count() == 2)
					{
						if (eyesDetected[0].X > eyesDetected[1].X)
						{
							Rectangle tmp = eyesDetected[0];
							eyesDetected[0] = eyesDetected[1];
							eyesDetected[1] = tmp;
						}
					}

					// record detected eye boxes
					foreach (Rectangle e in eyesDetected)
					{
						Rectangle eyeRect = e;
						eyeRect.Offset(f.X, f.Y);
						eyes.Add(eyeRect);
					}
				}
			}

			// for each eye, detect pupil and extract feature vector
			foreach (Rectangle eye in eyes)
			{
				// get sub image
				Mat eye_subimage = new Mat(processedFrame, eye);

				// detect and record pupil box
				if (settings.EnablePupilDection)
				{
					Rectangle pupil = FindPupil(eye_subimage);
					pupil.Offset(eye.X, eye.Y);
					pupils.Add(pupil);
				}

				// detect and record feature vector
				Mat feature = ComputeFeature(eye_subimage);
				this.features.Add(feature);

				// display feature vector
				if (this.settings.ShowFeatures)
				{
					CvInvoke.CvtColor(eye_subimage, eye_subimage, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);
					Mat roi = new Mat(frame, eye);
					eye_subimage.CopyTo(roi);
				}
			}

			// display detected boxes
			var color = new Bgr(Color.Red).MCvScalar;
			if (this.settings.ShowFaceBox)
				foreach (Rectangle face in faces)
					CvInvoke.Rectangle(frame, face, color, 2);

			if (this.settings.ShowEyeBox)
				foreach (Rectangle eye in eyes)
					CvInvoke.Rectangle(frame, eye, color, 2);

			if (this.settings.ShowPupilBox)
				foreach (Rectangle pupil in pupils)
					CvInvoke.Rectangle(frame, pupil, color, 1);
		}
		private Rectangle FindPupil(Mat image)
		{
			// build integral image
			Mat integral = new Mat();
			CvInvoke.Integral(image, integral);
			var integeralImage = integral.ToImage<Gray, int>();

			// search the maximum local contract region as pupil
			int w = integeralImage.Size.Width;
			int h = integeralImage.Size.Height;
			int minScale = this.settings.PupilDection_MinScale;
			int maxScale = integeralImage.Size.Height / 2;

			int maxDiff = 0;
			Rectangle maxRect = new Rectangle();
			int scale = minScale;
			// while scale within [minScale, maxScale]
			while (scale < maxScale)
			{
				int scale_power_2 = scale * scale;

				// for different offsets
				for (int offset = 0; offset < scale; offset += scale / this.settings.PupilDection_OffsetFactor)
				{
					// for all possible y in grid
					for (int y = scale + offset; y < h - scale * 2; y += scale)
					{
						// for all possible x in grid
						for (int x = scale + offset; x < w - scale * 2; x += scale)
						{
							// get the sums of center and the difference from surrounding grids
							int center = GetSum(integeralImage, x, y, scale, scale);
							int left = GetSum(integeralImage, x - scale, y, scale, scale) - center;
							int right = GetSum(integeralImage, x + scale, y, scale, scale) - center;
							int upper = GetSum(integeralImage, x, y - scale, scale, scale) - center;
							int bottom = GetSum(integeralImage, x, y + scale, scale, scale) - center;
							int upperLeft = GetSum(integeralImage, x - scale, y - scale, scale, scale) - center;
							int upperRight = GetSum(integeralImage, x + scale, y - scale, scale, scale) - center;
							int bottomLeft = GetSum(integeralImage, x - scale, y + scale, scale, scale) - center;
							int bottomRight = GetSum(integeralImage, x + scale, y + scale, scale, scale) - center;

							// compute the local contrast - the sum of differences between surrounding grid and center grid
							int diff = left + right + upper + bottom + upperLeft + upperRight + bottomLeft + bottomRight;
							diff /= scale_power_2;

							// record the values if it the current difference is maximum
							if (diff > maxDiff)
							{
								maxDiff = diff;
								maxRect = new Rectangle(x, y, scale, scale);
							}
						}
					}

					// increase the scale of grid
					scale = (int)(scale * this.settings.PupilDection_ScaleFactor);
				}
			}

			// return the rectangle with maximum contrast
			return maxRect;
		}
		private int GetSum(Image<Gray, int> img, int x, int y, int w, int h)
		{
			// return the sum of given region of a integeral image
			int a = img.Data[y, x, 0];
			int b = img.Data[y, x + w, 0];
			int c = img.Data[y + h, x, 0];
			int d = img.Data[y + h, x + w, 0];

			return a + d - b - c;
		}
		private Mat ComputeFeature(Mat image)
		{
			int patchWidth = settings.FeatureExtraction_PatchWidth;
			int patchHeight = settings.FeatureExtraction_PatchHeight;

			// based on the patch width and height, chop the middle part from the detected eye region
			int w = image.Size.Width;
			int h = (image.Size.Height * patchHeight) / patchWidth;
			int x = 0;
			int y = (image.Size.Height - h) / 2;
			Mat chopped = new Mat(image, new Rectangle(x, y, w, h));

			// scale the chopped region to patch size, then equalize its histogram
			CvInvoke.Resize(chopped, _scaledDown, new Size(patchWidth, patchHeight), 0, 0, Inter.Area);
			//CvInvoke.EqualizeHist(scaledDown, scaledDown);

			// scale up and copy to the original eye patch
			CvInvoke.Resize(_scaledDown, _scaledUp, chopped.Size, 0, 0, Inter.Nearest);
			_scaledUp.CopyTo(chopped);

			// return the flatted 1D Mat feature
			return _scaledDown.Reshape(1, 1);
		}

		// helper methods
		private void SaveSettings(string filename)
		{
			XmlSerializer xs = new XmlSerializer(typeof(Settings));
			StreamWriter sw = new StreamWriter(filename);
			xs.Serialize(sw, this.settings);
			sw.Close();
		}
		private void LoadSettings(string filename)
		{
			XmlSerializer xs = new XmlSerializer(typeof(Settings));
			StreamReader sr = new StreamReader(filename);
			this.settings = (Settings)xs.Deserialize(sr);
			this.propertyGridSettings.SelectedObject = this.settings;
			sr.Close();
		}
		private void LoadDefaultSettings()
		{
			string filename = AppDomain.CurrentDomain.BaseDirectory + @"\default_settings.xml";

			if (File.Exists(filename))
			{
				LoadSettings(filename);
			}
		}
		private void SaveRegressionModel(string filename)
		{
			RidgeRegression[] models = new RidgeRegression[] { RegressionX, RegressionY };

			XmlSerializer xs = new XmlSerializer(typeof(RidgeRegression[]));
			StreamWriter sw = new StreamWriter(filename);
			xs.Serialize(sw, models);
			sw.Close();
		}
		private void LoadRegressionModel(string filename)
		{
			XmlSerializer xs = new XmlSerializer(typeof(RidgeRegression[]));
			StreamReader sr = new StreamReader(filename);
			RidgeRegression[] models = (RidgeRegression[])xs.Deserialize(sr);
			sr.Close();

			RegressionX = models[0];
			RegressionY = models[1];
		}
	}

	// helper extension class for Rectangle struct
	public static class ExtensionMethods
	{
		public static Point Center(this Rectangle r)
		{
			return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
		}
		public static bool ContainPoint(this Rectangle r, Point p)
		{
			return (
				p.X >= r.X &&
				p.X < r.X + r.Width &&
				p.Y >= r.Y &&
				p.Y < r.Y + r.Height
				);
		}
		public static Rectangle UpperLeftRegion(this Rectangle r)
		{
			return new Rectangle(r.X, r.Y, r.Width / 2, r.Height / 2);
		}
		public static Rectangle UpperRightRegion(this Rectangle r)
		{
			return new Rectangle(r.X + r.Width / 2, r.Y, r.Width / 2, r.Height / 2);
		}

		public static double DistanceFrom(this Point p1, Point p2)
		{
			float dx = p1.X - p2.X;
			float dy = p1.Y - p2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		public static double DistanceFrom(this PointF p1, PointF p2)
		{
			float dx = p1.X - p2.X;
			float dy = p1.Y - p2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public static int LimitToRange(this int value, int inclusiveMinimum, int inclusiveMaximum)
		{
			if (value < inclusiveMinimum) { return inclusiveMinimum; }
			if (value > inclusiveMaximum) { return inclusiveMaximum; }
			return value;
		}
		public static float LimitToRange(this float value, float inclusiveMinimum, float inclusiveMaximum)
		{
			if (value < inclusiveMinimum) { return inclusiveMinimum; }
			if (value > inclusiveMaximum) { return inclusiveMaximum; }
			return value;
		}

		public static float MapTo(this float x, float in_min, float in_max, float out_min, float out_max)
		{
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}
		public static double MapTo(this double x, double in_min, double in_max, double out_min, double out_max)
		{
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}
	}

} 
