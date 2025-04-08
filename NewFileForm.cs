using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LessonLab
{
	public partial class NewFileForm : Form
	{
		public NewFileForm()
		{
			InitializeComponent();
			LanguageComboBox.SelectedIndex = 0;
			LanguageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void CreateNewFile_Click(object sender, EventArgs e)
		{
			if (TitleTextBox.Text.Length > 1)
			{
				NewFileData.CatchNewFileCreated(int.Parse(UnitUpDown.Value.ToString()), int.Parse(LessonUpDown.Value.ToString()),
					TitleTextBox.Text, LanguageComboBox.Text);
				Close();
			}
			else
			{
				MessageBox.Show("Enter a title!");
			}
		}
	}
}
