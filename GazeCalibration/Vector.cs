using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GazeCalibration
{
	public struct Vector
	{
		// public properties
		public float X { get; set; }
		public float Y { get; set; }
		public float Norm { get { return (float)Math.Sqrt(X * X + Y * Y); } }

		// constructor
		public Vector(float x, float y) { X = x; Y = y; }

		// public methods & operators
		public static Vector operator +(Vector v1, Vector v2)
		{
			return new Vector(v1.X + v2.X, v1.Y + v2.Y);
		}
		public static Vector operator -(Vector v1, Vector v2)
		{
			return new Vector(v1.X - v2.X, v1.Y - v2.Y);
		}
		public static Vector operator *(Vector v, float d)
		{
			return new Vector(v.X * d, v.Y * d);
		}
		public static Vector operator /(Vector v, float d)
		{
			return new Vector(v.X / d, v.Y / d);
		}
		public float Dot(Vector v)
		{
			return this.X * v.X + this.Y * v.Y;
		}
		public Vector Normalize()

		{
			return this / Norm;
		}
		public static implicit operator Vector(Point p)
		{
			return new Vector(p.X, p.Y);
		}
		public static implicit operator Vector(PointF p)
		{
			return new Vector(p.X, p.Y);
		}
		public static implicit operator PointF(Vector v)
		{
			return new PointF(v.X, v.Y);
		}
	}
}
