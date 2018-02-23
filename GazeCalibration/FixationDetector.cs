using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GazeCalibration
{       // public enum
	public enum FixationState { NotFocusing, Focusing, Locked };

	public class FixationDetector
	{
		// public event
		public event EventHandler FixationLocked;
		public event EventHandler FixationReleased;
		public event EventHandler RemoveGazePosition;

		// public setting properties
		[Category("Gaze Tracking")] public float AverageDecayRatio { get; set; } = 0.7f;
		[Category("Gaze Tracking")] public int GazeLostTime { get; set; } = 300; // ms
		[Category("Fixation Region")] public int RegionLockSamples { get; set; } = 3;
		[Category("Fixation Region")] public int RegionLockTime { get; set; } = 10; // ms
		[Category("Fixation Region")] public int RegionLockRadius { get; set; } = 100; // pixels
		[Category("Fixation Region")] public int RegionReleaseRadius { get; set; } = 100; // pixels

		// public properties
		[XmlIgnore] [Browsable(false)] public Vector AveragedGazePosition { get; private set; }
		[XmlIgnore] [Browsable(false)] public Queue<(Point gazePosition, Matrix<double> feature, long time)>
			LastGazePositions { get; } = new Queue<(Point, Matrix<double>, long)>();
		[XmlIgnore] [Browsable(false)] public FixationState FixationState { get; private set; } = FixationState.NotFocusing;
		[XmlIgnore] [Browsable(false)] public double FixationRegionRadius { get; private set; }
		[XmlIgnore] [Browsable(false)] public long ElapsedMilliseconds { get => stopwatch.ElapsedMilliseconds; }

		// private fields
		Stopwatch stopwatch = new Stopwatch();
		bool isFirstGazeUpdate = true;
		CancellationTokenSource fixationCancellation = new CancellationTokenSource();

		// constructor
		public FixationDetector() { stopwatch.Start(); }

		// main procedures
		public async void AddGazePosition(Point gazePosition, Matrix<double> feature)
		{
			// add new gaze position to queue
			LastGazePositions.Enqueue((gazePosition, feature, this.stopwatch.ElapsedMilliseconds));

			// compute the averaged gaze position
			if (isFirstGazeUpdate)
			{
				// use the new gaze position directly for first time
				AveragedGazePosition = gazePosition;
				isFirstGazeUpdate = false;
			}
			else
			{
				// otherwise compute a moving average with decay weighting
				AveragedGazePosition *= AverageDecayRatio;
				AveragedGazePosition += (Vector)gazePosition * (1.0f - AverageDecayRatio);
			}

			// update fixation region radius
			this.FixationRegionRadius = LastGazePositions
				.Select(item => (AveragedGazePosition - item.gazePosition).Norm)
				.Average();

			// update the fixation mode based on the fixation region radius
			UpdateFixationMode();

			// schedule to remove feature after specified time 
			await Task.Delay(this.GazeLostTime);
			this.LastGazePositions.Dequeue();

			// raise remove event
			RemoveGazePosition?.Invoke(this, EventArgs.Empty);
		}
		private async void UpdateFixationMode()
		{
			switch (this.FixationState)
			{
				case FixationState.NotFocusing:
					// if has small enough region size and enough number of recent gaze positions
					if (this.FixationRegionRadius < this.RegionLockRadius &&
						this.LastGazePositions.Count > this.RegionLockSamples)
					{
						// move the Focusing mode
						this.FixationState = FixationState.Focusing;
						try
						{
							// wait for specified time before moving to Locked mode
							await Task.Delay(this.RegionLockTime, fixationCancellation.Token);
							this.FixationState = FixationState.Locked;
							// raise OnFixationLocked event
							FixationLocked?.Invoke(this, EventArgs.Empty);
						}
						catch
						{
							// delay canceled, abort to move to Locked mode (and stay in NotFocusing mode)
						}
					}
					break;

				case FixationState.Focusing:
					// if the fixation region is greater than specified size
					if (this.FixationRegionRadius >= this.RegionLockRadius)
					{
						this.FixationState = FixationState.NotFocusing;
						fixationCancellation.Cancel();
						fixationCancellation.Dispose();
						fixationCancellation = new CancellationTokenSource();
					}
					break;

				case FixationState.Locked:
					// return to NotFocusing mode if region size is even bigger
					if (this.FixationRegionRadius >= this.RegionReleaseRadius)
					{
						this.FixationState = FixationState.NotFocusing;
						// raise OnFixationReleased event
						FixationReleased?.Invoke(this, EventArgs.Empty);
					}
					break;
			}
		}
	}
}
