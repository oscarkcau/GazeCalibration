using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GazeCalibration
{
	public partial class FormSettings : Form
	{
		private object settingObject = null;

		public FormSettings(string title, object settings)
		{
			InitializeComponent();

			this.Text = title;
			this.propertyGridMain.SelectedObject = settings;
			this.settingObject = settings;
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void buttonLoad_Click(object sender, EventArgs e)
		{
			// show dialog and get filename from user
			OpenFileDialog d = new OpenFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			d.CheckFileExists = true;

			if (d.ShowDialog(this) == DialogResult.OK)
			{
				// load setting from xml file
				Type type = this.settingObject.GetType();
				XmlSerializer xs = new XmlSerializer(type);
				StreamReader sr = new StreamReader(d.FileName);
				object source = xs.Deserialize(sr);
				sr.Close();

				// copy field values to current setting object
				object target = this.settingObject;
				foreach (var sourceProperty in type.GetProperties())
				{
					var targetProperty = type.GetProperty(sourceProperty.Name);
					targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
				}

				// refresh property grid
				this.propertyGridMain.Refresh();
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			// show dialog and get filename from user
			SaveFileDialog d = new SaveFileDialog();
			d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			d.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
			d.OverwritePrompt = true;

			if (d.ShowDialog(this) == DialogResult.OK)
			{
				// save setting object to xml file
				XmlSerializer xs = new XmlSerializer(this.settingObject.GetType());
				StreamWriter sw = new StreamWriter(d.FileName);
				xs.Serialize(sw, this.settingObject);
				sw.Close();
			}
		}
	}
}
