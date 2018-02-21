using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GazeCalibration
{
	class VirtualMouse
	{
		// public properties
		public float X { get; set; }
		public float Y { get; set; }
		public Rectangle Region { get; }
		public float Scale { get; set; }

		// private fields
		Form form;
		bool firstMouseMove = true;
		Point regionCenter;
		float currentMouseX, currentMouseY;
		
		// constructor
		public VirtualMouse(Form form, int x, int y, int width, int height)
		{
			this.form = form;
			this.Region = new Rectangle(x, y, width, height);
			this.Scale = 1.0f;
			this.regionCenter = new Point(x + width / 2, height / 2);
			this.X = regionCenter.X;
			this.Y = regionCenter.Y;
		}

		public void MoveMouse(float x, float y)
		{
			// direct assign virtual cursor position for the first time
			if (firstMouseMove)
			{
				firstMouseMove = false;
				this.X = this.currentMouseX = x;
				this.Y = this.currentMouseY = y;
				return;
			}

			// update the virtual cursor position according system cursor movement
			float dx = x - currentMouseX;
			float dy = y - currentMouseY;
			this.X += dx * this.Scale;
			this.Y += dy * this.Scale;

			// ensure virtual cursor is within client window
			this.X.LimitToRange(this.Region.Left, this.Region.Right);
			this.Y.LimitToRange(this.Region.Top, this.Region.Bottom);

			// adjust system cursor position if flag is enabled
			Point p = this.form.PointToClient(regionCenter); // screen center point in client coordinate
			Cursor.Position = regionCenter;

			// remember to update current system cursor position
			currentMouseX = p.X;
			currentMouseY = p.Y;
		}
	}
}
