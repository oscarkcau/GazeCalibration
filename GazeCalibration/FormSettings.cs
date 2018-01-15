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
	public partial class FormSettings : Form
	{
		public FormSettings(string title, object settings)
		{
			InitializeComponent();

			this.Text = title;
			this.propertyGridMain.SelectedObject = settings;
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
