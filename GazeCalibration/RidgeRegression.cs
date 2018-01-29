using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Xml.Serialization;
using System.IO;

namespace GazeCalibration 
{
	public class RidgeRegression {
		// public properties
		public int N { get; set; }
		public double Penalty { get; set; }
		public Matrix<double> ATA { get; set; }
		public Matrix<double> ATb { get; set; }
		public Matrix<double> W { get; set; }
		public Matrix<double> inv { get; set; }

		// constructor
		public RidgeRegression(int n, double penalty = 10e-5) 
		{
			this.N = n;
			this.Penalty = penalty;

			// initialize all matrices and vectors
			this.ATA = new Matrix<double>(n, n);
			ATA.SetIdentity(new MCvScalar(penalty)); // add regulation weights
			ATA[n-5, n-5] = 0; // bias term has no regulation

			this.ATb = new Matrix<double>(n, 1);
			ATb.SetZero();

			this.W = new Matrix<double>(n, 1);
			W.SetZero();

			this.inv = new Matrix<double>(n, n);
			inv.SetZero();
		}
		public RidgeRegression() { }

		public void AddFeature(Matrix<double> feature, double b, bool updateWeights = true) 
		{
			// Add one sample to regression model

			// FUCK! MulTransposed not working as expected
			//Matrix<double> vTv = new Matrix<double>(N, N);
			//CvInvoke.MulTransposed(feature, vTv, false);

			Matrix<double> vTv = feature.Transpose() * feature;

			this.ATA += vTv;
			this.ATb += feature.Transpose() * b;

			if (updateWeights)
			{
				UpdateWeights();
			}
		}

		public void AddFeatures(IEnumerable<Matrix<double>> features, double b, bool updateWeights = true)
		{
			// Add multiple samples to regression model

			// FUCK! MulTransposed not working as expected
			//Matrix<double> vTv = new Matrix<double>(N, N);
			//CvInvoke.MulTransposed(feature, vTv, false);

			foreach (var feature in features)
			{
				Matrix<double> vTv = feature.Transpose() * feature;

				this.ATA += vTv;
				this.ATb += feature.Transpose() * b;
			}

			if (updateWeights)
			{
				UpdateWeights();
			}
		}
		public void UpdateWeights()
		{
			CvInvoke.Invert(ATA, inv, DecompMethod.Cholesky);
			this.W = inv * ATb;
		}

		public double Predict(Matrix<double> feature) 
		{
			return this.W.DotProduct(feature.Transpose());
		}

		public void Save(string filename) 
		{
			XmlSerializer xs = new XmlSerializer(typeof(RidgeRegression));
			StreamWriter sw = new StreamWriter(filename);
			xs.Serialize(sw, this);
			sw.Close();
		}

		public static RidgeRegression Load(string filename) 
		{
			XmlSerializer xs = new XmlSerializer(typeof(RidgeRegression));
			StreamReader sr = new StreamReader(filename);
			RidgeRegression r = (RidgeRegression)xs.Deserialize(sr);
			sr.Close();

			return r;
		}
	}
}
