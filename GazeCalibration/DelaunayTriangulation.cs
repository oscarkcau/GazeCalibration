using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GazeCalibration
{
	// Delaunay Triangulation class for 2D points
	class DelaunayTriangulation
	{
		#region Helper classes
		public class Vertex
		{
			// public properties
			public int Index { get; }
			public Vector Position { get; }
			public object Tag { get; set; }
			public bool IsBoundary { get; set; } = false;
			// constructors
			public Vertex(int index, Vector v, object tag=null) { Index = index; Position = v; Tag = tag; }
			public Vertex(int index, float x, float y, object tag = null) : this(index, new Vector(x, y), tag) { }
		}

		public class Face
		{
			// public properties
			public Vertex[] Vertices { get; } = new Vertex[3];
			public Face[] AdjacentFaces { get; } = new Face[3];
			public double[] Angles { get; } = new double[3];
			public (int, int, int) Key { get; }

			// constructor
			public Face(Vertex v1, Vertex v2, Vertex v3)
			{
				Vertices[0] = v1;
				Vertices[1] = v2;
				Vertices[2] = v3;
				AdjacentFaces[0] = null;
				AdjacentFaces[1] = null;
				AdjacentFaces[2] = null;
				Key = (Vertices[0].Index, Vertices[1].Index, Vertices[2].Index);
				Angles[0] = _Angle(v1.Position, v2.Position, v3.Position);
				Angles[1] = _Angle(v2.Position, v3.Position, v1.Position);
				Angles[2] = _Angle(v3.Position, v1.Position, v2.Position);

				double _Angle(Vector p1, Vector p2, Vector p3)
				{
					double p12 = (p2 - p1).Norm;
					double p13 = (p1 - p3).Norm;
					double p23 = (p2 - p3).Norm;

					return Math.Acos((p12 * p12 + p13 * p13 - p23 * p23) / (2 * p12 * p13));
				}
			}

			// public methods & operators
			public bool ContainsPoint(Vector v)
			{
				bool b1, b2, b3;

				b1 = _Sign(v, Vertices[0].Position, Vertices[1].Position) < 0.0f;
				b2 = _Sign(v, Vertices[1].Position, Vertices[2].Position) < 0.0f;
				b3 = _Sign(v, Vertices[2].Position, Vertices[0].Position) < 0.0f;

				return ((b1 == b2) && (b2 == b3));

				double _Sign(Vector p1, Vector p2, Vector p3)
				{
					// return the sign represents which side of the half-plane created by 
					// the edges (p2, p3) the point p1 is 
					return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
				}
			}
		}
		#endregion

		// public fields
		public List<Vertex> Vertices { get; } = new List<Vertex>();
		public Dictionary<(int, int, int), Face> Faces { get; } = new Dictionary<(int, int, int), Face>();

		// private fields
		private Queue<Face> unprocessQueue = new Queue<Face>();

		// constructor
		public DelaunayTriangulation(float left, float right, float top, float bottom)
		{
			Vertex v1 = _AddVertex(left, top);
			Vertex v2 = _AddVertex(left, bottom);
			Vertex v3 = _AddVertex(right, bottom);
			Vertex v4 = _AddVertex(right, top);

			Face f1 = _AddFace(v2, v3, v1);
			Face f2 = _AddFace(v4, v1, v3);

			f1.AdjacentFaces[0] = f2;
			f2.AdjacentFaces[0] = f1;
		}

		// private functions
		private Vertex _AddVertex(float x, float y, object tag = null)
		{
			// simply add vertex without any checking or edge flipping
			// only for internal use

			int index = this.Vertices.Count;
			Vertex v = new Vertex(index, x, y, tag);
			this.Vertices.Add(v);
			return v;
		}
		private Face _AddFace(Vertex v1, Vertex v2, Vertex v3)
		{
			// simply add face without any checking or edge flipping
			// only for internal use

			Face f = new Face(v1, v2, v3);
			this.Faces.Add(f.Key, f);
			return f;
		}
		private void _LinkFaces(Face f1, Face f2)
		{
			// set adjacent reference of 2 given adjacent faces
			// InvalidOperationException will be thrown if the given faces are not adjacent to each other

			if (f1 == null || f2 == null) return;

			var a1 = f1.Vertices;
			var a2 = f2.Vertices;
			int index1 = Array.IndexOf(a1, a1.Except(a2).First());
			int index2 = Array.IndexOf(a2, a2.Except(a1).First());

			f1.AdjacentFaces[index1] = f2;
			f2.AdjacentFaces[index2] = f1;
		}
		private void _FlipFaces()
		{
			// process all faces in unprocess queue
			while (unprocessQueue.Count > 0)
			{
				Face f1 = unprocessQueue.Dequeue();

				// check for all adjacent faces
				for (int index1 = 0; index1 < 3; index1++)
				{
					Face f2 = f1.AdjacentFaces[index1];
					if (f2 == null) continue;

					// find the opposite vertex in f2
					var a1 = f1.Vertices;
					var a2 = f2.Vertices;
					int index2 = Array.IndexOf(a2, a2.Except(a1).First());
					Debug.Assert(f1 == f2.AdjacentFaces[index2]);

					// skip if the two faces meet the Delaunay condition
					if (f1.Angles[index1] + f2.Angles[index2] <= Math.PI) continue;

					// get all 4 vertices of these 2 faces
					Vertex v1 = a1[index1];
					Vertex v2 = a1[(index1 + 1) % 3];
					Vertex v3 = a2[index2];
					Vertex v4 = a2[(index2 + 1) % 3];
					Debug.Assert(a1[(index1 + 1) % 3] == a2[(index2 + 2) % 3]);
					Debug.Assert(a1[(index1 + 2) % 3] == a2[(index2 + 1) % 3]);

					// create new flipped faces
					Face newFlipped1 = _AddFace(v1, v2, v3);
					Face newFlipped2 = _AddFace(v3, v4, v1);

					// link new faces to other faces
					_LinkFaces(newFlipped1, newFlipped2);
					_LinkFaces(newFlipped1, f1.AdjacentFaces[(index1 + 2) % 3]);
					_LinkFaces(newFlipped1, f2.AdjacentFaces[(index2 + 1) % 3]);
					_LinkFaces(newFlipped2, f2.AdjacentFaces[(index2 + 2) % 3]);
					_LinkFaces(newFlipped2, f1.AdjacentFaces[(index1 + 1) % 3]);

					// remove old faces
					Faces.Remove(f1.Key);
					Faces.Remove(f2.Key);

					// add new faces to unprocess queue
					unprocessQueue.Enqueue(newFlipped1);
					unprocessQueue.Enqueue(newFlipped2);

					// current face is removed, no need to check remaining adjacent faces
					break;
				}
			}
		}

		// public functions
		public int AddVertex(float x, float y, object tag = null)
		{
			// create new vertex
			Vertex newVertex = _AddVertex(x, y, tag);

			// find the face that contains the new vertex
			// InvalidOperationException will be thrown if no find
			Face oldFace = Faces.Values.First(_f => _f.ContainsPoint(newVertex.Position));

			// create new faces
			Face newFace0 = _AddFace(newVertex, oldFace.Vertices[1], oldFace.Vertices[2]);
			Face newFace1 = _AddFace(newVertex, oldFace.Vertices[2], oldFace.Vertices[0]);
			Face newFace2 = _AddFace(newVertex, oldFace.Vertices[0], oldFace.Vertices[1]);

			// update adjacent information
			_LinkFaces(newFace0, newFace1);
			_LinkFaces(newFace1, newFace2);
			_LinkFaces(newFace2, newFace0);
			_LinkFaces(newFace0, oldFace.AdjacentFaces[0]);
			_LinkFaces(newFace1, oldFace.AdjacentFaces[1]);
			_LinkFaces(newFace2, oldFace.AdjacentFaces[2]);

			// remove old face
			Faces.Remove(oldFace.Key);

			// add new faces to unprocedd queue
			unprocessQueue.Clear();
			unprocessQueue.Enqueue(newFace0);
			unprocessQueue.Enqueue(newFace1);
			unprocessQueue.Enqueue(newFace2);

			// flip faces to keep Delaunay property
			_FlipFaces();

			return newVertex.Index;
		}
		public void RemoveBoundingBox()
		{
			// get the bounding box vertices
			var boundingVertices = Vertices.GetRange(0, 4);

			// find all faces that contains bounding box vertices
			List<Face> facesToRemove = new List<Face>();
			foreach (Face f in Faces.Values)
			{
				if (f.Vertices.Any(v => boundingVertices.Contains(v)))
				{
					facesToRemove.Add(f);

					// also mark the boundary vertices
					foreach (Vertex v in f.Vertices)
						if(!boundingVertices.Contains(v))
							v.IsBoundary = true;
				}
			}

			// remove all selected faces
			foreach (Face f1 in facesToRemove)
			{
				foreach(Face f2 in f1.AdjacentFaces)
				{
					if (f2 == null) continue;
					
					// remember to update adjacent infromation
					for (int i = 0; i < 3; i++)
						if (f2.AdjacentFaces[i] == f1)
							f2.AdjacentFaces[i] = null;
				}

				// remove faces from Faces dictionary
				this.Faces.Remove(f1.Key);
			}
		}
	}
}
