using System;
using System.Runtime;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO.Packaging;

namespace LessonLab
{
	/*
	
	Lesson Code is a sequence of two characters that defines the version and type of the lesson within the saved file

	This is a list of first and second characters and their respective purposes and definitions.

	First character (type of lesson):
		L (uppercase l)		-	Regular input-output lesson type; finished
		l (lowercase l)		-	Regular input-output lesson type; unfinished, since has placeholder tasks

	Second character (version of the lesson file: LessonLab versions producing, Shabyt versions supporting):
		-	S (uppercase s)		-	LessonLab: [beta - v3.0];		Shabyt:	0.1a
		-	a (lowercase a)		-	LessonLab: v3.1;				Shabyt: 0.1a
				'AM|Audio Match' added
		-	b (lowercase b)		-	LessonLab: [v3.2 - v...);		Shabyt:	[0.2a - ...)
				'AW|Translate, Rus→Qaz' and 'PW|Translate, Qaz→Rus' merged into 'TR|Translate'
				'FT|Fill with Translations' added
				'BQ|Basic Question' added

	*/

	public partial class MainForm : Form
	{
		float panelWidth = 0.4f;

		string LessonCode = "Lb";

		readonly string[] SupportedLessonCodes = ["Lb", "lb"];
		readonly string[] UpdatableLessonCodes = ["La", "la", "LS"];

		int unit, lesson;
		string title;
		string lang;
		bool IsFileOpened = false;

		int editIndex = -1;
		bool outputBoxCanBeClicked = true;

		int sizeX1, sizeY1, sizeXDelta, sizeYDelta;
		int PanelWidth1, PanelWidthDelta;

		readonly List<string> TaskList = [];

		public MainForm()
		{
			InitializeComponent();

			TabComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			LessonTabControl.Location = new Point(0, -25);
			foreach (TabPage Tab in LessonTabControl.TabPages)
				TabComboBox.Items.Add(Tab.Text);
			TabComboBox.SelectedIndex = 0;
			PanelWidth1 = SplitContainer.Panel2.Width;
			RestartButton.Location = new Point(13, 10);
		}



		//	------------------------------------------------------
		//	======================================================
		//	#Task			Task specific methods:
		//	======================================================
		//	------------------------------------------------------



		/*	GR|Given Material
			gr|[Paragraph][Paragraph]...*/
		private void AddTaskGR_Click(object sender, EventArgs e)
		{
			string task = "gr|";
			foreach (string g in GivenInputBoxGR.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
				task += $"[{g.Trim()}]";
			GivenInputBoxGR.Text = null;
			AddTask(task, "addedGR");
			AddTaskGR.Text = "Қосу";
		}



		/*	TB|Table
			tb|<ColumnCount>|[PretextParagraph][PretextParagraph]...|
				[ColumnName;ColumnName;...<0:0>;<0:1>;...;<1:0>;...;]|[PosttextParagraph][PosttextParagraph]... */
		private void AddTaskTB_Click(object sender, EventArgs e)
		{
			if (GivenTableDataGridViewTB.RowCount > 1 && GivenTableDataGridViewTB.ColumnCount > 1)
			{
				string task = $"tb|{GivenTableDataGridViewTB.ColumnCount}|";
				foreach (string g in GivenPretextInputBoxTB.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
					task += $"[{g.Trim()}]";
				task += "|[";
				for (int i = 0; i < GivenTableDataGridViewTB.RowCount; i++)         //i == y
					for (int j = 0; j < GivenTableDataGridViewTB.ColumnCount; j++)  //j == x
						task += $"{GivenTableDataGridViewTB[j, i].Value};";
				task = task.Remove(task.Length - 1);
				task += "]|";
				foreach (string g in GivenPosttextInputBoxTB.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
					task += $"[{g.Trim()}]";

				GivenPretextInputBoxTB.Text = GivenPosttextInputBoxTB.Text = null;
				GivenTableDataGridViewTB.Columns.Clear();
				ColumnNumLabelTB.Text = $"X: {GivenTableDataGridViewTB.ColumnCount}";
				AddTask(task, "addedTB");
				AddTaskTB.Text = "Қосу";
			}
		}
		private void AddColumnButtonTB_Click(object sender, EventArgs e)
		{
			DataGridViewColumn column = new()
			{
				SortMode = DataGridViewColumnSortMode.NotSortable,
				CellTemplate = new DataGridViewTextBoxCell(),
				Name = $"Column{GivenTableDataGridViewTB.ColumnCount + 1}"
			};
			ColumnNumLabelTB.Text = $"X: {GivenTableDataGridViewTB.ColumnCount + 1}";
			GivenTableDataGridViewTB.Columns.Add(column);
		}
		private void RemoveColumnButtonTB_Click(object sender, EventArgs e)
		{
			if (GivenTableDataGridViewTB.Columns.Count > 0)
				GivenTableDataGridViewTB.Columns.RemoveAt(GivenTableDataGridViewTB.Columns.Count - 1);
			ColumnNumLabelTB.Text = $"X: {GivenTableDataGridViewTB.ColumnCount}";
		}
		private void AddRowButtonTB_Click(object sender, EventArgs e)
		{
			if (GivenTableDataGridViewTB.ColumnCount > 0)
			{
				DataGridViewRow row = new();
				RowNumLabelTB.Text = $"Y: {GivenTableDataGridViewTB.RowCount + 1}";
				GivenTableDataGridViewTB.Rows.Add(row);
			}
		}
		private void RemoveRowButtonTB_Click(object sender, EventArgs e)
		{
			if (GivenTableDataGridViewTB.RowCount > 0)
				GivenTableDataGridViewTB.Rows.RemoveAt(GivenTableDataGridViewTB.RowCount - 1);
			RowNumLabelTB.Text = $"Y: {GivenTableDataGridViewTB.RowCount}";
		}



		/*	AR|Audio Material
			ar|AudioSource|[GivenWord][GivenWordTranslation][Example][ExampleTranslation]
			Audio source DOES NOT contain ".mp3" */
		private void AddTaskAR_Click(object sender, EventArgs e)
		{
			string task = $"ar|{AudioInputBoxAR.Text.Trim()}|[{GivenInputBoxAR.Text.Trim()}][{TranslationInputBoxAR.Text.Trim()}][{ExampleInputBoxAR.Text.Trim()}][{ExampleTranslationInputBoxAR.Text.Trim()}]";
			AddTask(task, "addedAR");
			AudioInputBoxAR.Text = GivenInputBoxAR.Text = TranslationInputBoxAR.Text = ExampleInputBoxAR.Text = ExampleTranslationInputBoxAR.Text = null;
			AddTaskAR.Text = "Қосу";
		}



		/*	TR|Translate
			tr|<style>|GivenPhrase={Answer}{altAnswer}{altAnswer}...
			style can be: "no" - no style OR 'i'/'f' + 's'/'p'	*/
		private void AddTaskTR_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(TranslationInputBoxTR.Text) && !string.IsNullOrEmpty(GivenInputBoxTR.Text))
			{
				string task = $"tr|{(IsStyleRequiredCheckBoxTR.Checked ?
					$"{(IsFormalCheckBoxTR.Checked ? 'f' : 'i')}{(IsPluralCheckBoxTR.Checked ? 'p' : 's')}" : "no")}|{GivenInputBoxTR.Text}=";
				List<string> phrases = new
					(TranslationInputBoxTR.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

				for (int i = 0; i < phrases.Count; i++)
					task += $"{{{Standardise(phrases[i])}}}";

				if (!string.IsNullOrEmpty(GivenInputBoxTR.Text) && !string.IsNullOrEmpty(TranslationInputBoxTR.Text))
					AddTask(task, "addedTR");

				TranslationInputBoxTR.Text = GivenInputBoxTR.Text = null;
				AddTaskTR.Text = "Қосу";
			}
		}
		private void IsMultiAnswerCheckBoxTR_CheckedChanged(object sender, EventArgs e)
		{
			if (IsMultiAnswerCheckBoxTR.Checked)
			{
				IsMultiAnswerCheckBoxTR.Location = new Point(8, TranslationGroupBoxTR.Height - 32);
				AddTaskTR.Location = new Point(AddTaskTR.Location.X, TranslationGroupBoxTR.Height - 36);
				TranslationInputBoxTR.Multiline = true;
				TranslationInputBoxTR.Size = new Size(TranslationInputBoxTR.Width, TranslationGroupBoxTR.Height - 87);
				TranslationGroupBoxTR.Text = "Переводы";
			}
			else
			{
				IsMultiAnswerCheckBoxTR.Location = new Point(8, 78);
				AddTaskTR.Location = new Point(AddTaskTR.Location.X, 72);
				TranslationInputBoxTR.Multiline = false;
				TranslationGroupBoxTR.Text = "Перевод";
			}
		}
		private void IsStyleRequiredCheckBoxTR_CheckedChanged(object sender, EventArgs e)
		{
			IsFormalCheckBoxTR.Enabled = IsPluralCheckBoxTR.Enabled = IsStyleRequiredCheckBoxTR.Checked;
		}



		/*	BQ|Basic Question
			bq|Question|Given={Answer}{altAnswer}{altAnswer}...	*/
		private void AddTaskBQ_Click(object sender, EventArgs e)
		{
			string task = $"bq|{QuestionInputBoxBQ.Text.Trim()}|{GivenInputBoxBQ.Text.Trim()}=";
			List<string> Answers = new
				(AnswerInputBoxBQ.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

			foreach (string text in Answers)
				task += $"{{{Standardise(text)}}}";

			AnswerInputBoxBQ.Text = GivenInputBoxBQ.Text = QuestionInputBoxBQ.Text = null;

			AddTask(task, "addedBQ");
			AddTaskBQ.Text = "Қосу";
		}
		private void IsMultiAnswerCheckBoxBQ_CheckedChanged(object sender, EventArgs e)
		{
			if (IsMultiAnswerCheckBoxBQ.Checked)
			{
				IsMultiAnswerCheckBoxBQ.Location = new Point(8, AnswerGroupBoxBQ.Height - 32);
				AddTaskBQ.Location = new Point(AddTaskBQ.Location.X, AnswerGroupBoxBQ.Height - 36);
				AnswerInputBoxBQ.Multiline = true;
				AnswerInputBoxBQ.Size = new Size(AnswerInputBoxBQ.Width, AnswerGroupBoxBQ.Height - 61);
				AnswerGroupBoxBQ.Text = "Дұрыс жауаптар";
			}
			else
			{
				IsMultiAnswerCheckBoxBQ.Location = new Point(8, 52);
				AddTaskBQ.Location = new Point(AddTaskBQ.Location.X, 46);
				AnswerInputBoxBQ.Multiline = false;
				AnswerGroupBoxBQ.Text = "Дұрыс жауап";
			}
		}




		/*	FG|Fill the Gaps
			fg|GivenPhrase={Answer}{Answer}...	*/
		private void AddTaskFG_Click(object sender, EventArgs e)
		{
			string task = $"fg|{GivenInputBoxFG.Text.Trim()}=";
			List<string> Answers = new
				(AnswerInputBoxFG.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

			foreach (string text in Answers)
				task += $"{{{Standardise(text)}}}";

			GivenInputBoxFG.Text = AnswerInputBoxFG.Text = null;

			AddTask(task, "addedFG");
			AddTaskFG.Text = "Қосу";
		}
		private void IsMultiAnswerCheckBoxFG_CheckedChanged(object sender, EventArgs e)
		{
			if (IsMultiAnswerCheckBoxFG.Checked)
				(IsMultiAnswerCheckBoxFG.Location, AddTaskFG.Location, AnswerInputBoxFG.Multiline, AnswerInputBoxFG.Size, AnswerGroupBoxFG.Text) =
					(new Point(8, AnswerGroupBoxFG.Height - 28),
					new Point(AddTaskFG.Location.X, AnswerGroupBoxFG.Height - 32), true,
					new Size(AnswerInputBoxFG.Width, AnswerGroupBoxFG.Height - 62),
					"Жауаптар");
			else
				(IsMultiAnswerCheckBoxFG.Location, AddTaskFG.Location, AnswerInputBoxFG.Multiline, AnswerGroupBoxFG.Text) =
					(new Point(8, 58), new Point(AddTaskFG.Location.X, 54), false, "Жауап");
		}



		/*	FT|Fill with Translation
			ft|Given|TranslationWithGap={Answer}{AltAnswer}{AltAnswer}...	*/
		private void AddTaskFT_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(AnswerInputBoxFT.Text) && !string.IsNullOrEmpty(GivenInputBoxFT.Text))
			{
				string task = $"ft|{GivenInputBoxFT.Text.Trim()}|{GapInputBoxFT.Text.Trim()}=";
				List<string> phrases = GetLines(AnswerInputBoxFT.Text, true);

				for (int i = 0; i < phrases.Count; i++)
					task += $"{{{phrases[i]}}}";

				if (!string.IsNullOrEmpty(GivenInputBoxFT.Text)
					&& !string.IsNullOrEmpty(AnswerInputBoxFT.Text) && !string.IsNullOrEmpty(GapInputBoxFT.Text))
					AddTask(task, "addedFT");

				AnswerInputBoxFT.Text = GivenInputBoxFT.Text = GapInputBoxFT.Text = null;
				AddTaskFT.Text = "Қосу";
			}
		}
		private void IsMultiAnswerCheckBoxFT_CheckedChanged(object sender, EventArgs e)
		{
			if (IsMultiAnswerCheckBoxFT.Checked)
			{
				IsMultiAnswerCheckBoxFT.Location = new Point(8, AnswerGroupBoxFT.Height - 32);
				AddTaskFT.Location = new Point(AddTaskFT.Location.X, AnswerGroupBoxFT.Height - 36);
				AnswerInputBoxFT.Multiline = true;
				AnswerInputBoxFT.Size = new Size(AnswerInputBoxFT.Width, AnswerGroupBoxFT.Height - 61);
				AnswerGroupBoxFT.Text = "Аудармалар";
			}
			else
			{
				IsMultiAnswerCheckBoxFT.Location = new Point(8, 52);
				AddTaskFT.Location = new Point(AddTaskFT.Location.X, 46);
				AnswerInputBoxFT.Multiline = false;
				AnswerGroupBoxFT.Text = "Аударма";
			}
		}



		/*	AQ|Audio Question
			aq|Question|AudioSource={Answer}{altAnswer}{altAnswer}...
			Audio source DOES NOT contain ".mp3"	*/
		private void AddTaskAQ_Click(object sender, EventArgs e)
		{
			string task = $"aq|{QuestionInputBoxAQ.Text.Trim()}|{AudioInputBoxAQ.Text.Trim()}=";
			List<string> Answers = GetLines(AnswerInputBoxAQ.Text, true);

			foreach (string text in Answers)
				task += $"{{{text}}}";

			AudioInputBoxAQ.Text = AnswerInputBoxAQ.Text = QuestionInputBoxAQ.Text = null;

			AddTask(task, "addedAQ");
			AddTaskAQ.Text = "Қосу";
		}
		private void IsMultiAnswerCheckBoxAQ_CheckedChanged(object sender, EventArgs e)
		{
			if (IsMultiAnswerCheckBoxAQ.Checked)
			{
				IsMultiAnswerCheckBoxAQ.Location = new Point(10, AnswerGroupBoxAQ.Height - 28);
				AddTaskAQ.Location = new Point(AddTaskAQ.Location.X, AnswerGroupBoxAQ.Height - 32);
				AnswerInputBoxAQ.Multiline = true;
				AnswerInputBoxAQ.Size = new Size(AnswerInputBoxAQ.Width, AnswerGroupBoxAQ.Height - 62);
				AnswerGroupBoxAQ.Text = "Жауаптар";
			}
			else
			{
				IsMultiAnswerCheckBoxAQ.Location = new Point(10, 60);
				AddTaskAQ.Location = new Point(AddTaskAQ.Location.X, 54);
				AnswerInputBoxAQ.Multiline = false;
				AnswerGroupBoxAQ.Text = "Жауап";
			}
		}



		/*	SW|Select Emoji
			sw|GivenWord={CorrectWord|Emoji}{IncorrectWord|Emoji}{IncorrectWord|Emoji}...	
			Emoji names are taken from emojipedia.org	*/
		private void AddTaskSW_Click(object sender, EventArgs e)
		{
			string task = $"sw|{GivenInputBoxSW.Text.Trim()}=";
			List<string> Words = GetLines(IncorrectInputBoxSW.Text);
			List<string> Emojis = GetLines(EmojiInputBoxSW.Text);

			Words.Insert(0, CorrectInputBoxSW.Text.Trim());

			if (Words.Count == Emojis.Count)
			{
				for (int i = 0; i < Words.Count; i++)
					task += $"{{{Words[i].Trim()}|{Emojis[i].Trim()}}}";

				AddTask(task, "addedSW");

				AddTaskSW.Text = "Қосу";
				IncorrectInputBoxSW.Text = CorrectInputBoxSW.Text = GivenInputBoxSW.Text = EmojiInputBoxSW.Text = null;
			}
			else
				EmojiGroupBoxSW.Text = "Эмодзи саны барлық жауаптар санымен сәйкес болуы керек!";
		}


		//TODO:	Wherever is needed, put GetLines() methods

		/*	AM|Audio Match
			am|Given={CorrectAnswer}{IncorrectAnswer}{IncorrectAnswer}...
			*/
		private void AddTaskAM_Click(object sender, EventArgs e)
		{
			List<string> IncorrectAnswers = GetLines(IncorrectInputBoxAM.Text);

			string task = $"am|{GivenInputBoxAM.Text.Trim()}={{{CorrectInputBoxAM.Text.Trim()}}}";
			foreach (string g in IncorrectAnswers)
				task += $"{{{g.Trim()}}}";

			IncorrectInputBoxAM.Text = CorrectInputBoxAM.Text = GivenInputBoxAM.Text = null;

			AddTask(task, "addedAM");
			AddTaskAM.Text = "Қосу";
		}



		/*	SP|Select Phrase 
			sp|QuestionPhrase|GivenText={CorrectAnswer}{IncorrectAnswer}{IncorrectAnswer}...	
			Select phrase that answers the question	*/
		private void AddTaskSP_Click(object sender, EventArgs e)
		{
			List<string> IncorrectAnswers = new
				(IncorrectInputBoxSP.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

			string task = $"sp|{QuestionInputBoxSP.Text.Trim()}|{GivenInputBoxSP.Text.Trim()}={{{CorrectInputBoxSP.Text.Trim()}}}";
			foreach (string g in IncorrectAnswers)
				task += $"{{{g.Trim()}}}";

			IncorrectInputBoxSP.Text = CorrectInputBoxSP.Text = GivenInputBoxSP.Text = QuestionInputBoxSP.Text = null;

			AddTask(task, "addedSP");
			AddTaskSP.Text = "Қосу";
		}



		/*	AS|Audio Select
			as|Question|AudioSource={CorrectAnswer}{IncorrectAnswer}{IncorrectAnswer}...
			Select audio that answers the question
			Audio source DOES NOT contain ".mp3"	*/
		private void AddTaskAS_Click(object sender, EventArgs e)
		{
			List<string> IncorrectAnswers = new
				(IncorrectInputBoxAS.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

			string task = $"as|{QuestionInputBoxAS.Text.Trim()}|{AudioInputBoxAS.Text.Trim()}={{{CorrectInputBoxAS.Text.Trim()}}}";
			foreach (string g in IncorrectAnswers)
				task += $"{{{g.Trim()}}}";

			IncorrectInputBoxAS.Text = CorrectInputBoxAS.Text = AudioInputBoxAS.Text = QuestionInputBoxAS.Text = null;

			AddTask(task, "addedAS");
			AddTaskAS.Text = "Қосу";
		}



		/*	TF|True/False
				tf|GivenPhrase|Question=<t/f>	*/
		private void AddTaskTF_Click(object sender, EventArgs e)
		{
			string task = $"tf|{GivenInputBoxTF.Text.Trim()}|{QuestionInputBoxTF.Text.Trim()}={(IsTrueCheckBoxTF.Checked ? 't' : 'f')}";
			AddTask(task, "addedTF");
			AddTaskTF.Text = "Қосу";
			GivenInputBoxTF.Text = QuestionInputBoxTF.Text = null;
		}



		/*	ph|Placeholder
			ph|Text
			*/
		private void AddTaskPH_Click(object sender, EventArgs e)
		{
			string task = $"ph|{PlaceholderInputBox.Text}";
			if (LessonCode[0] == 'L')
				LessonCode = $"l{LessonCode[1]}";

			PlaceholderInputBox.Text = null;
			AddTask(task, "addedPH");
			AddTaskPH.Text = "Қосу";
		}



		/*	ed|Edit	*/
		private void EditButton_Click(object sender, EventArgs e)
		{
			string lessonType = SelectTaskComboBox.Text[..2];
			string material = SelectTaskComboBox.Text[3..];
			bool isPlaceholder = lessonType == "ph";

			OpenLessonButton.Enabled = NewLessonButton.Enabled = false;
			EditButton.Enabled = RemoveTaskButton.Enabled =
				PushIntoButton.Enabled = SwapButton.Enabled = false;
			ActiveForm.BackColor = Color.FromArgb(123, 148, 140);
			editIndex = SelectTaskComboBox.SelectedIndex;
			outputBoxCanBeClicked = true;


			if (!isPlaceholder)
			{
				for (int i = 0; i < LessonTabControl.TabPages.Count; i++)
					if (LessonTabControl.TabPages[i].Name[..2] == lessonType)
						LessonTabControl.SelectedIndex = i;

				TabComboBox.Enabled = false;

				string[] segments; //split elements that were priorly covered in {} or []
				int divIndex, equalSignIndex;

				switch (lessonType)
				{
					case "gr":
						GivenInputBoxGR.Text = null;
						AddTaskGR.Text = "Өңдеу";

						segments = GetMatches(material, '[');
						for (int i = 0; i < segments.Length; i++)
							GivenInputBoxGR.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						break;

					case "tb":
						AddTaskTB.Text = "Өңдеу";
						string[] materialFragments = material.Split("|");

						int columnNum = int.Parse(materialFragments[0]);
						string pretextMaterial = materialFragments[1];
						segments = GetMatches(pretextMaterial, '[');
						for (int i = 0; i < segments.Length; i++)
							GivenPretextInputBoxTB.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");

						string[] tableValues = materialFragments[2][1..^1].Split(';');

						GivenTableDataGridViewTB.ColumnCount = columnNum;
						GivenTableDataGridViewTB.RowCount = tableValues.Length / columnNum;

						for (int row = 0; row < GivenTableDataGridViewTB.Rows.Count; row++)
							for (int column = 0; column < GivenTableDataGridViewTB.Columns.Count; column++)
								GivenTableDataGridViewTB.Rows[row].Cells[column].Value = tableValues[row * columnNum + column];

						string posttextMaterial = materialFragments[3];
						segments = GetMatches(posttextMaterial, '[');
						for (int i = 0; i < segments.Length; i++)
							GivenPosttextInputBoxTB.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						break;

					case "ar":
						AddTaskAR.Text = "Өңдеу";
						divIndex = material.IndexOf('|');

						segments = GetMatches(material[(divIndex + 1)..], '[');
						AudioInputBoxAR.Text = material[..divIndex];

						GivenInputBoxAR.Text = segments[0];
						TranslationInputBoxAR.Text = segments[1];
						ExampleInputBoxAR.Text = segments[2];
						ExampleTranslationInputBoxAR.Text = segments[3];
						break;

					case "tr":
						AddTaskTR.Text = "Өңдеу";
						if (material[..2] != "no")
						{
							IsStyleRequiredCheckBoxTR.Checked = true;
							IsFormalCheckBoxTR.Checked = material[0] == 'f';
							IsPluralCheckBoxTR.Checked = material[1] == 'p';
						}
						else
							IsStyleRequiredCheckBoxTR.Checked = IsFormalCheckBoxTR.Checked =
								IsPluralCheckBoxTR.Checked = false;

						equalSignIndex = material.IndexOf('=');
						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						GivenInputBoxTR.Text = material[3..equalSignIndex];

						IsMultiAnswerCheckBoxTR.Checked = segments.Length > 1;
						for (int i = 0; i < segments.Length; i++)
							TranslationInputBoxTR.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						break;

					case "ft":
						AddTaskFT.Text = "Өңдеу";

						divIndex = material.IndexOf('|');
						equalSignIndex = material.IndexOf('=');
						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						GivenInputBoxFT.Text = material[..divIndex];
						GapInputBoxFT.Text = material[(divIndex + 1)..equalSignIndex];

						for (int i = 0; i < segments.Length; i++)
							AnswerInputBoxFT.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						break;

					case "fg":
						AddTaskFG.Text = "Өңдеу";
						equalSignIndex = material.IndexOf('=');
						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						GivenInputBoxFG.Text = material[..equalSignIndex];

						IsMultiAnswerCheckBoxFG.Checked = segments.Length > 1;
						for (int i = 0; i < segments.Length; i++)
							AnswerInputBoxFG.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						break;

					case "aq":
						AddTaskAQ.Text = "Өңдеу";
						divIndex = material.IndexOf('|');
						equalSignIndex = material.IndexOf('=');

						QuestionInputBoxAQ.Text = material[..divIndex];
						AudioInputBoxAQ.Text = material[(divIndex + 1)..equalSignIndex];

						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						IsMultiAnswerCheckBoxAQ.Checked = segments.Length > 1;
						for (int i = 0; i < segments.Length; i++)
							AnswerInputBoxAQ.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						break;

					case "sw":
						AddTaskSW.Text = "Өңдеу";
						equalSignIndex = material.IndexOf('=');
						GivenInputBoxSW.Text = material[..equalSignIndex];
						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						for (int i = 0; i < segments.Length; i++)
						{
							divIndex = segments[i].IndexOf('|');

							if (i != 0)
								IncorrectInputBoxSW.Text += segments[i][..divIndex] + (i == segments.Length - 1 ? "" : "\r\n");
							else
								CorrectInputBoxSW.Text = segments[0][..divIndex];

							EmojiInputBoxSW.Text += segments[i][(divIndex + 1)..] + (i == segments.Length - 1 ? "" : "\r\n");
						}
						break;

					case "am":
						AddTaskAM.Text = "Өңдеу";
						equalSignIndex = material.IndexOf('=');
						GivenInputBoxAM.Text = material[..equalSignIndex];
						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						for (int i = 1; i < segments.Length; i++)
							IncorrectInputBoxAM.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						CorrectInputBoxAM.Text = segments[0];
						break;

					case "sp":
						AddTaskSP.Text = "Өңдеу";
						divIndex = material.IndexOf('|');
						equalSignIndex = material.IndexOf('=');

						QuestionInputBoxSP.Text = material[..divIndex];
						GivenInputBoxSP.Text = material[(divIndex + 1)..equalSignIndex];

						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						for (int i = 1; i < segments.Length; i++)
							IncorrectInputBoxSP.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						CorrectInputBoxSP.Text = segments[0];
						break;

					case "as":
						AddTaskAS.Text = "Өңдеу";
						divIndex = material.IndexOf('|');
						equalSignIndex = material.IndexOf('=');

						QuestionInputBoxAS.Text = material[..divIndex];
						AudioInputBoxAS.Text = material[(divIndex + 1)..equalSignIndex];

						segments = GetMatches(material[(equalSignIndex + 1)..], '{');
						for (int i = 1; i < segments.Length; i++)
							IncorrectInputBoxAS.Text += segments[i] + (i == segments.Length - 1 ? "" : "\r\n");
						CorrectInputBoxAS.Text = segments[0];
						break;

					case "tf":
						AddTaskTF.Text = "Өңдеу";
						divIndex = material.IndexOf('|');

						GivenInputBoxTF.Text = material[..divIndex];
						QuestionInputBoxTF.Text = material[(divIndex + 1)..^2];
						IsTrueCheckBoxTF.Checked = material[^1] == 't';
						break;

					default:
						Crash();
						break;
				}
			}
		}
		private void SelectTaskComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectTaskComboBox.Text[..2] == "ph")
				EditButton.Text = "Бастау";
			else
				EditButton.Text = "Өңдеу";
		}
		private void PushIntoButton_Click(object sender, EventArgs e)
		{
			if (SelectTaskComboBox.Text != SwapTaskComboBox.Text
				&& !string.IsNullOrEmpty(SelectTaskComboBox.Text)
				&& !string.IsNullOrEmpty(SwapTaskComboBox.Text))
			{
				bool isFinished = true;

				int pushIntoIndex = TaskList.IndexOf(SwapTaskComboBox.Text);
				string task = SelectTaskComboBox.Text;
				TaskList.Remove(TaskList[TaskList.IndexOf(SelectTaskComboBox.Text)]);
				TaskList.Insert(pushIntoIndex, task);


				OutputListBox.Items.Clear();
				foreach (string text in TaskList)
				{
					OutputListBox.Items.Add(text);
					if (text[..2] == "ph")
						isFinished = false;
				}
				SelectTaskComboBox.Items.Clear();
				foreach (string text in TaskList)
					SelectTaskComboBox.Items.Add(text);
				SwapTaskComboBox.Items.Clear();
				foreach (string text in TaskList)
					SwapTaskComboBox.Items.Add(text);

				NumerationListBox.Items.Clear();
				for (int i = 1; i <= TaskList.Count; i++)
					NumerationListBox.Items.Add(i);

				SaveData(isFinished, "pushed");
			}
		}
		private void SwapButton_Click(object sender, EventArgs e)
		{
			if (SelectTaskComboBox.Text != SwapTaskComboBox.Text
				&& !string.IsNullOrEmpty(SelectTaskComboBox.Text)
				&& !string.IsNullOrEmpty(SwapTaskComboBox.Text))
			{
				(TaskList[TaskList.IndexOf(SelectTaskComboBox.Text)], TaskList[TaskList.IndexOf(SwapTaskComboBox.Text)]) = (TaskList[TaskList.IndexOf(SwapTaskComboBox.Text)], TaskList[TaskList.IndexOf(SelectTaskComboBox.Text)]);
				bool isFinished = true;

				OutputListBox.Items.Clear();
				foreach (string text in TaskList)
				{
					OutputListBox.Items.Add(text);
					if (text[..2] == "ph")
						isFinished = false;
				}
				SelectTaskComboBox.Items.Clear();
				foreach (string text in TaskList)
					SelectTaskComboBox.Items.Add(text);
				SwapTaskComboBox.Items.Clear();
				foreach (string text in TaskList)
					SwapTaskComboBox.Items.Add(text);

				NumerationListBox.Items.Clear();
				for (int i = 1; i <= TaskList.Count; i++)
					NumerationListBox.Items.Add(i);

				SaveData(isFinished, "swapped");
			}
		}
		private void RemoveTaskButton_Click(object sender, EventArgs e)
		{
			bool isFinished = true;

			string TextToRemove = null;
			foreach (string text in TaskList)
				if (text == SelectTaskComboBox.Text)
					TextToRemove = text;
			TaskList.Remove(TextToRemove);
			SelectTaskComboBox.Items.Remove(TextToRemove);
			SwapTaskComboBox.Items.Remove(TextToRemove);
			OutputListBox.Items.Remove(TextToRemove);
			SelectTaskComboBox.Text = null;
			SwapTaskComboBox.Text = null;
			NumerationListBox.Items.Clear();
			for (int i = 1; i <= TaskList.Count; i++)
				NumerationListBox.Items.Add(i);

			foreach (string text in TaskList)
				if (text[..2] == "ph")
					isFinished = false;

			SaveData(isFinished, "removed");
		}
		private void OutputListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//RE: What is this fr
			if (outputBoxCanBeClicked && editIndex == -1)
			{
				SelectTaskComboBox.SelectedIndex = OutputListBox.SelectedIndex;
				LessonTabControl.SelectedIndex = LessonTabControl.TabPages.IndexOf(EDPage);
				TabComboBox.SelectedIndex = TabComboBox.Items.IndexOf("ed|Edit");
			}
			else
				outputBoxCanBeClicked = true;
		}



		//	----------------------------------------------------
		//	====================================================
		//	#Menu			Menu button methods:
		//	====================================================
		//	----------------------------------------------------



		private void NewLessonButton_Click(object sender, EventArgs e)
		{
			NewFileForm newFileForm = new();
			newFileForm.Show();
		}
		private void OpenLessonButton_Click(object sender, EventArgs e)
		{
			if (editIndex == -1)
			{
				TaskList.Clear();
				OutputListBox.Items.Clear();
				NumerationListBox.Items.Clear();
				SelectTaskComboBox.Items.Clear();
				SwapTaskComboBox.Items.Clear();

				OpenFileDialog dialog = new()
				{
					InitialDirectory = @"C:\Tasks",
					Filter = "LessonLab files (*.qlp)|*.qlp"
				};

				List<string> linesToBeOpened = [];

				dialog.ShowDialog();
				if (dialog.FileName != "")
					using (StreamReader sr = new(dialog.FileName))
					{
						string[] lines = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
						FileInfo fileInfo = new(dialog.FileName);
						if (fileInfo.Length > 0)
						{
							if (dialog.SafeFileName[..2] == "rq")
							{
								if (!string.IsNullOrEmpty(lines[0]))
								{


									if (SupportedLessonCodes.Contains(lines[0][..2]) && lines[0].Length > 4)
									{

										//When you open a new file, the data is saved on the old one, when not updated!!!
										lang = "RusToQaz";
										title = /*TitleTextBox.Text =*/ lines[0][3..];
										unit = int.Parse(dialog.SafeFileName[2..dialog.SafeFileName.IndexOf('-')]);
										lesson = int.Parse(dialog.SafeFileName[(dialog.SafeFileName.IndexOf('-') + 1)..dialog.SafeFileName.IndexOf('.')]);
										for (int i = 1; i < lines.Length; i++)
											if (!string.IsNullOrEmpty(lines[i]))
												linesToBeOpened.Add(lines[i]);

										TaskManagerGroupBox.Enabled = true;
										ActiveForm.Text = TaskManagerGroupBox.Text = $"QLP:LessonLab — {title} ({(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.qlp)";
										LessonTabControl.Enabled = true;
										ShowRestartButton();
									}
									else if (UpdatableLessonCodes.Contains(lines[0][..2]))
									{
										DialogResult dialogResult = MessageBox.Show($"'{lines[0][..2]}' is an unsupported lesson code\nDo you wish to update the file?\n(note: it might damage the file)",
											"Unsupported file", MessageBoxButtons.YesNo);
										if (dialogResult == DialogResult.Yes)
										{
											lang = "RusToQaz";
											title = lines[0][3..];
											unit = int.Parse(dialog.SafeFileName.Substring(2, dialog.SafeFileName.IndexOf('-') - 2));
											lesson = int.Parse(dialog.SafeFileName.Substring(dialog.SafeFileName.IndexOf('-') + 1, dialog.SafeFileName.IndexOf('.') - dialog.SafeFileName.IndexOf('-') - 1));

											if (char.IsLower(lines[0][1]))
												lines[0] = SupportedLessonCodes[1] + lines[0][2..];
											else
												lines[0] = SupportedLessonCodes[0] + lines[0][2..];
											for (int i = 1; i < lines.Length; i++)
											{
												if (lines[i].Length > 3)
												{
													if (lines[i][..2] == "aw" || lines[i][..2] == "pw")
													{
														lines[i] = "tr" + lines[i][2..];
														linesToBeOpened.Add(lines[i]);
													}
													else
													{
														linesToBeOpened.Add(lines[i]);
													}
												}
											}
											ActiveForm.Text = TaskManagerGroupBox.Text = $"QLP:LessonLab — {title} ({(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.qlp)";
											LessonTabControl.Enabled = true;
											ShowRestartButton();
										}
									}
									else
									{
										MessageBox.Show($"Error: '{lines[0][..2]}' is an unsupported lesson code");
									}
								}
								else
									MessageBox.Show("Error: Empty title");
							}
							else
								MessageBox.Show("Error: Invalid language");
						}
						else
							MessageBox.Show($"Error: '{lines[0][..2]}' is an obsolete file type and/or there is a lack of title");
						sr.Close();
					}
				else
					MessageBox.Show("Error: Empty file");

				foreach (string nextLine in linesToBeOpened)
					AddTask(nextLine, "opened");
			}
		}
		private void OpenDirectoryButton_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(@"C:\Tasks"))
				Directory.CreateDirectory(@"C:\Tasks");
			ProcessStartInfo psi = new(@"C:\Tasks") { UseShellExecute = true };
			Process.Start(psi);
		}
		private void TabComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			LessonTabControl.SelectedIndex = LessonTabControl.TabPages.Cast<TabPage>().ToList().FindIndex(tp => tp.Text == TabComboBox.Text);
		}
		private void SetLesson_Click()
		{
			if (title.Length > 1)
			{
				if (lang == "Рус→Қаз")
				{
					lang = "RusToQaz";
					TaskManagerGroupBox.Enabled = true;
					ActiveForm.Text = TaskManagerGroupBox.Text = $"QLP:LessonLab — {title} ({(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.qlp)";
					LessonTabControl.Enabled = true;
				}

				if (!Directory.Exists(@"C:\Tasks"))
					Directory.CreateDirectory(@"C:\Tasks");
				StreamWriter streamWriter = new($@"C:\Tasks\{(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.qlp");
				streamWriter.WriteLine($"{LessonCode}|{title}");
				streamWriter.Close();
				ShowRestartButton();
			}

			TaskList.Clear();
			OutputListBox.Items.Clear();
			NumerationListBox.Items.Clear();
			SelectTaskComboBox.Items.Clear();
			SwapTaskComboBox.Items.Clear();
		}
		private void ShowRestartButton()
		{
			RestartButton.Visible = true;
			NewLessonButton.Visible = OpenLessonButton.Visible = false;
		}
		private void RestartButton_Click(object sender, EventArgs e)
		{
			Application.Restart();
			Environment.Exit(0);
		}



		//	-----------------------------------------------------
		//	=====================================================
		//	#Data			Data control methods:
		//	=====================================================
		//	-----------------------------------------------------


		public void CatchNewFile(int CaughtUnit, int CaughtLesson, string CaughtTitle, string CaughtLanguage)
		{
			unit = CaughtUnit;
			lesson = CaughtLesson;
			title = CaughtTitle;
			lang = CaughtLanguage;
			SetLesson_Click();
		}
		void AddTask(string task, string action, bool silent = false)
		{
			bool isFinished = true;
			EditButton.Enabled = RemoveTaskButton.Enabled =
				PushIntoButton.Enabled = SwapButton.Enabled = true;
			ActiveForm.BackColor = Color.FromArgb(120, 198, 172);

			//RE: What does editIndex == -1 mean?
			if (editIndex == -1)
			{
				TaskList.Add(task);

				if (!silent)
				{
					OutputListBox.Items.Add(task);

					SelectTaskComboBox.Items.Clear();
					foreach (string text in TaskList)
					{
						SelectTaskComboBox.Items.Add(text);
						if (text[..2] == "ph")
							isFinished = false;
					}
					SwapTaskComboBox.Items.Clear();
					foreach (string text in TaskList)
						SwapTaskComboBox.Items.Add(text);

					NumerationListBox.Items.Clear();
					for (int i = 1; i <= TaskList.Count; i++)
						NumerationListBox.Items.Add(i);
				}
			}
			else
			{
				TaskList[editIndex] = task;
				OutputListBox.Items[editIndex] = task;

				SelectTaskComboBox.Items[editIndex] = task;

				SwapTaskComboBox.Items[editIndex] = task;

				SelectTaskComboBox.SelectedIndex = editIndex;
				LessonTabControl.SelectedIndex = LessonTabControl.TabPages.IndexOf(EDPage);
				TabComboBox.SelectedIndex = TabComboBox.Items.IndexOf("ed|Edit");

				TabComboBox.Enabled = OpenLessonButton.Enabled = NewLessonButton.Enabled = true;
			}

			if (!silent)
			{
				editIndex = -1;
				outputBoxCanBeClicked = false;
				OutputListBox.SelectedItems.Clear();
			}

			SaveData(isFinished, action);
		}
		private async void SaveData(bool isFinished, string action)
		{
			ActiveForm.Text = TaskManagerGroupBox.Text = $"QLP:LessonLab — {title} ({(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.qlp){(isFinished ? "" : " [UNFINISHED]")}";

			if (isFinished)
				if (LessonCode[0] == 'l')
					LessonCode = $"L{LessonCode[1]}";

			if (!Directory.Exists(@"C:\Tasks"))
				Directory.CreateDirectory(@"C:\Tasks");

			StreamWriter streamWriter =
				new($@"C:\Tasks\{(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.qlp");
			streamWriter.WriteLine($"{LessonCode}|{title}");

			//Creating a backup
			if (!Directory.Exists(@"C:\Tasks\Backup"))
				Directory.CreateDirectory(@"C:\Tasks\Backup");
			string formattedTime = DateTime.UtcNow.ToString().Replace('/', '.').Replace(':', '.');
			StreamWriter backUpWriter =
				new($@"C:\Tasks\Backup\{(lang == "RusToQaz" ? "rq" : "x")}{unit}-{lesson}.{action}.{formattedTime}.qlp");
			backUpWriter.WriteLine($"{LessonCode}|{title}");

			foreach (string g in TaskList)
				if (!string.IsNullOrEmpty(g))
				{
					streamWriter.WriteLine(g);
					backUpWriter.WriteLine(g);
				}

			streamWriter.Close();
			streamWriter.Dispose();
			await streamWriter.DisposeAsync();

			backUpWriter.Close();
			backUpWriter.Dispose();
			await backUpWriter.DisposeAsync();
		}



		//	-----------------------------------------------------
		//	=====================================================
		//	#Resize			Methods for resizing:
		//	=====================================================
		//	-----------------------------------------------------



		private void MainForm_ResizeBegin(object sender, EventArgs e)
		{
			sizeX1 = ActiveForm.Size.Width;
			sizeY1 = ActiveForm.Size.Height;
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			sizeXDelta = ActiveForm.Size.Width - sizeX1;
			sizeYDelta = ActiveForm.Size.Height - sizeY1;

			GetPanelWidth();
			SplitContainer.Size = new Size(ActiveForm.Width - 40, ActiveForm.Height - 55);
			//SplitContainer.SplitterDistance = Convert.ToInt32(SplitContainer.Width * (1f - panelWidth));

			OutputListBox.Size = new Size(OutputListBox.Width, ActiveForm.Height - 56);
			ResizeControl(OutputListBox, true, false, 0.6f);        //tf

			NumerationListBox.Size = new Size(NumerationListBox.Width, ActiveForm.Height - 56);

			ControllablePanel.Location = new Point(49 + OutputListBox.Width, 12);

			//ResizeInterior();
			ResizeControl(ControllablePanel, quotientX: panelWidth);

			ResizeControl(TaskManagerGroupBox, quotientX: panelWidth);
			ResizeControl(TabPanel, quotientX: panelWidth);
			ResizeControl(LessonTabControl, quotientX: panelWidth);
			ResizeControl(TabComboBox, quotientX: panelWidth);

			ResizeControl(NewLessonButton, yToChange: false, quotientX: panelWidth * 4f / 7f);
			ResizeControl(OpenLessonButton, yToChange: false, quotientX: panelWidth * 1f / 7f);
			ResizeControl(RestartButton, yToChange: false, quotientX: panelWidth * 5f / 7f);
			ResizeControl(OpenDirectoryButton, yToChange: false, quotientX: panelWidth * 2f / 7f);

			RelocateControl(OpenLessonButton, yToChange: false, quotientX: panelWidth * 4f / 7f);
			RelocateControl(OpenDirectoryButton, yToChange: false, quotientX: panelWidth * 5f / 7f);

			//ResizeControl(TopMenuSplitContainer, yToChange: true, quotientX: panelWidth);

			//GR
			ResizeControl(GivenGroupBoxGR, quotientX: panelWidth);
			ResizeControl(GivenInputBoxGR, quotientX: panelWidth);
			RelocateControl(AddTaskGR, quotientX: panelWidth);

			//TB
			//Vertical
			{
				ResizeControl([GivenGroupBoxTB, GivenPretextInputBoxTB, GivenPosttextInputBoxTB], false, quotientY: 0.5f);
				ResizeControl([GivenTableGroupBox, GivenTableDataGridViewTB], false, quotientY: 0.5f);
				RelocateControl([GivenTableGroupBox, AddTaskTB], false, quotientY: 0.5f);
			}
			{
				//Horizontal
				ResizeControl([GivenGroupBoxTB, GivenTableGroupBox], quotientX: panelWidth);
				RelocateControl([AddColumnButtonTB, ColumnNumLabelTB, RemoveColumnButtonTB, AddRowButtonTB, RowNumLabelTB, RemoveRowButtonTB, DividerTB, AddTaskTB],
					yToChange: false, quotientX: panelWidth);
				GivenTableDataGridViewTB.Size = new Size(GivenTableGroupBox.Width - 74, GivenTableDataGridViewTB.Height);
				ResizeControl([GivenPretextInputBoxTB, GivenPosttextInputBoxTB], yToChange: false, quotientX: 0.5f * panelWidth);
				RelocateControl(GivenPosttextInputBoxTB, yToChange: false, quotientX: 0.5f * panelWidth);
			}

			//AR
			ResizeControl([AudioGroupBoxAR,
				AudioInputBoxAR,
				GivenGroupBoxAR,
				GivenInputBoxAR,
				TranslationGroupBoxAR,
				TranslationInputBoxAR,
				ExampleGroupBoxAR,
				ExampleInputBoxAR,
				ExampleTranslationGroupBoxAR,
				ExampleTranslationInputBoxAR], yToChange: false, quotientX: panelWidth);
			RelocateControl(AddTaskAR, yToChange: false, quotientX: panelWidth);

			//TR
			ResizeControl([GivenGroupBoxTR, GivenInputBoxTR, TranslationGroupBoxTR, TranslationInputBoxTR], yToChange: false, quotientX: panelWidth);
			ResizeControl(TranslationGroupBoxTR, xToChange: false);
			RelocateControl(AddTaskTR, yToChange: false, quotientX: panelWidth);
			if (IsMultiAnswerCheckBoxTR.Checked)
			{
				ResizeControl(TranslationInputBoxTR, false);
				RelocateControl([AddTaskTR, IsMultiAnswerCheckBoxTR], false);
			}


			//BQ
			ResizeControl([GivenGroupBoxBQ, GivenInputBoxBQ, AnswerGroupBoxBQ, AnswerInputBoxBQ, QuestionInputBoxBQ, QuestionGroupBoxBQ], yToChange: false, quotientX: panelWidth);
			ResizeControl(AnswerGroupBoxBQ, xToChange: false);
			RelocateControl(AddTaskBQ, yToChange: false, quotientX: panelWidth);
			if (IsMultiAnswerCheckBoxBQ.Checked)
			{
				ResizeControl(AnswerInputBoxBQ, false);
				RelocateControl([AddTaskBQ, IsMultiAnswerCheckBoxBQ], false);
			}

			//FT
			ResizeControl([GivenGroupBoxFT, GivenInputBoxFT, AnswerGroupBoxFT, AnswerInputBoxFT, GapInputBoxFT, GapGroupBoxFT], yToChange: false, quotientX: panelWidth);
			ResizeControl(AnswerGroupBoxFT, xToChange: false);
			RelocateControl(AddTaskFT, yToChange: false, quotientX: panelWidth);
			if (IsMultiAnswerCheckBoxFT.Checked)
			{
				ResizeControl(AnswerInputBoxFT, false);
				RelocateControl([AddTaskFT, IsMultiAnswerCheckBoxFT], false);
			}

			//FG
			ResizeControl([GivenGroupBoxFG, GivenInputBoxFG, AnswerGroupBoxFG, AnswerInputBoxFG], yToChange: false, quotientX: panelWidth);
			ResizeControl(AnswerGroupBoxFG, xToChange: false);
			RelocateControl(AddTaskFG, yToChange: false, quotientX: panelWidth);
			if (IsMultiAnswerCheckBoxFG.Checked)
			{
				ResizeControl(AnswerInputBoxFG, false);
				RelocateControl([AddTaskFG, IsMultiAnswerCheckBoxFG], false);
			}

			//AQ
			ResizeControl([AudioGroupBoxAQ, AudioInputBoxAQ, QuestionGroupBoxAQ, QuestionInputBoxAQ, AnswerGroupBoxAQ, AnswerInputBoxAQ], yToChange: false, quotientX: panelWidth);
			ResizeControl(AnswerGroupBoxAQ, xToChange: false);
			RelocateControl(AddTaskAQ, yToChange: false, quotientX: panelWidth);
			if (IsMultiAnswerCheckBoxAQ.Checked)
			{
				ResizeControl(AnswerInputBoxAQ, false);
				RelocateControl([AddTaskAQ, IsMultiAnswerCheckBoxAQ], false);
			}

			//SW
			ResizeControl([IncorrectGroupBoxSW, IncorrectInputBoxSW, EmojiGroupBoxSW, EmojiInputBoxSW], xToChange: false, quotientY: 0.5f);
			RelocateControl([EmojiGroupBoxSW, GivenGroupBoxSW], xToChange: false, quotientY: 0.5f);
			RelocateControl(AddTaskSW, xToChange: false);

			ResizeControl([IncorrectGroupBoxSW, IncorrectInputBoxSW, CorrectGroupBoxSW, CorrectInputBoxSW, GivenGroupBoxSW, GivenInputBoxSW],
				yToChange: false, quotientX: panelWidth * 0.5f);
			ResizeControl([EmojiGroupBoxSW, EmojiInputBoxSW, AddTaskSW], yToChange: false, quotientX: panelWidth);

			RelocateControl([CorrectGroupBoxSW, GivenGroupBoxSW], yToChange: false, quotientX: panelWidth * 0.5f);


			//AM
			ResizeControl([IncorrectGroupBoxAM, IncorrectInputBoxAM], xToChange: false);
			ResizeControl([GivenGroupBoxAM,
				GivenInputBoxAM,
				CorrectGroupBoxAM,
				CorrectInputBoxAM,
				IncorrectGroupBoxAM,
				IncorrectInputBoxAM,
				AddTaskAM], yToChange: false, quotientX: panelWidth);
			RelocateControl(AddTaskAM, false);

			//SP
			ResizeControl([IncorrectGroupBoxSP, IncorrectInputBoxSP], xToChange: false);
			ResizeControl([GivenGroupBoxSP,
				GivenInputBoxSP,
				QuestionGroupBoxSP,
				QuestionInputBoxSP,
				CorrectGroupBoxSP,
				CorrectInputBoxSP,
				IncorrectGroupBoxSP,
				IncorrectInputBoxSP,
				AddTaskSP], yToChange: false, quotientX: panelWidth);
			RelocateControl(AddTaskSP, false);

			//AS
			ResizeControl([IncorrectGroupBoxAS, IncorrectInputBoxAS], xToChange: false);
			ResizeControl([AudioGroupBoxAS,
				AudioInputBoxAS,
				QuestionGroupBoxAS,
				QuestionInputBoxAS,
				CorrectGroupBoxAS,
				CorrectInputBoxAS,
				IncorrectGroupBoxAS,
				IncorrectInputBoxAS,
				AddTaskAS], yToChange: false, quotientX: panelWidth);
			RelocateControl(AddTaskAS, false);

			//TF
			ResizeControl([GivenGroupBoxTF, GivenInputBoxTF, QuestionGroupBoxTF, QuestionInputBoxTF], yToChange: false, quotientX: panelWidth);
			RelocateControl(AddTaskTF, yToChange: false, quotientX: panelWidth);

			//ph
			ResizeControl([PlaceholderGroupBox, PlaceholderInputBox], quotientX: panelWidth);
			ResizeControl(AddTaskPH, yToChange: false, quotientX: panelWidth);
			RelocateControl(AddTaskPH, xToChange: false);

			//ed
			ResizeControl([EditGroupBox, SelectTaskComboBox, SwapTaskComboBox], yToChange: false, quotientX: panelWidth);
			RelocateControl([EditButton, SwapButton, PushIntoButton], yToChange: false, quotientX: panelWidth);
		}

		void ResizeControl(Control container, bool xToChange = true, bool yToChange = true, float quotientX = 1f, float quotientY = 1f, bool isSplitMoved = false)
		{
			container.Size += new Size(
				xToChange ? (isSplitMoved ? PanelWidthDelta : (int)(sizeXDelta * quotientX)) : 0,
				yToChange ? (int)(sizeYDelta * quotientY) : 0);
		}

		void ResizeControl(Control[] containers, bool xToChange = true, bool yToChange = true, float quotientX = 1f, float quotientY = 1f, bool isSplitMoved = false)
		{
			foreach (Control container in containers)
				container.Size += new Size(
					xToChange ? (int)(sizeXDelta * quotientX) : 0,
					yToChange ? (int)(sizeYDelta * quotientY) : 0);
		}

		void GetPanelWidth()
		{
			//panelWidth = (float)SplitContainer.Panel2.Width / SplitContainer.Width;
		}

		private void SplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			PanelWidthDelta = SplitContainer.Panel2.Width - PanelWidth1;
			PanelWidth1 = SplitContainer.Panel1.Width;
			//panelWidth = (float)SplitContainer.Panel2.Width / SplitContainer.Width;
			//ActiveForm.Text = "moved: " + panelWidth.ToString();

			ResizeInterior();
		}

		void ResizeInterior()
		{
			//SplitContainer.Panel2.Width / SplitContainer.Width)
			//ResizeControl(ControllablePanel, isSplitMoved: true);

			//ResizeControl(TaskManagerGroupBox, isSplitMoved: true);
			//ResizeControl(TabPanel, isSplitMoved: true);
			//ResizeControl(LessonTabControl, isSplitMoved: true);
			//ResizeControl(TabComboBox, isSplitMoved: true);

			//GR
			//ResizeControl(GivenGroupBoxGR,	isSplitMoved: true);
			//ResizeControl(GivenInputBoxGR,	isSplitMoved: true);
			//RelocateControl(AddTaskGR,		isSplitMoved: true);
		}

		void RelocateControl(Control container, bool xToChange = true, bool yToChange = true, float quotientX = 1f, float quotientY = 1f)
		{
			container.Location = new Point(
				container.Location.X + (xToChange ? (int)(sizeXDelta * quotientX) : 0),
				container.Location.Y + (yToChange ? (int)(sizeYDelta * quotientY) : 0));
		}

		void RelocateControl(Control[] containers, bool xToChange = true, bool yToChange = true, float quotientX = 1f, float quotientY = 1f)
		{
			foreach (Control container in containers)
				container.Location = new Point(
					container.Location.X + (xToChange ? (int)(sizeXDelta * quotientX) : 0),
					container.Location.Y + (yToChange ? (int)(sizeYDelta * quotientY) : 0));
		}



		//	------------------------------------------------
		//	================================================
		//					Utility methods:
		//	================================================
		//	------------------------------------------------


		string[] GetMatches(string text, char filterCharacter)
		{

			List<string> matchesText = [];
			string filter = filterCharacter == '{' ? @"\{(.*?)\}" : (filterCharacter == '[' ? @"\[(.*?)\]" : "bruh");
			if (filter == "bruh")
				Crash();
			MatchCollection matches = Regex.Matches(text, filter);
			foreach (Match match in matches.Cast<Match>())
				matchesText.Add(match.Groups[1].Value);
			return matchesText.ToArray();
		}

		List<string> GetLines(string text, bool isStandardised = false, bool isDecapitalised = false)
		{
			//TODO:	Clean up this mess, in other words, both GetLines() and Standardise() may lower the texts, decide what to leave
			List<string> Lines = new
				(text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			if (isStandardised) Lines = Lines.Select(line => line = Standardise(line)).ToList();
			if (isDecapitalised) Lines = Lines.Select(line => line = line.ToLower()).ToList();
			return Lines.Where(line => !string.IsNullOrEmpty(line)).ToList();
		}

		void Crash()
		{
			int zero = 0;
			Text = $"{1 / zero}";
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Alt)
				e.SuppressKeyPress = true;
		}

		string Standardise(string text, bool isCapitalised = false)
		{
			string standardised = new string(text.Where(IsValidCharacter).ToArray()).Trim();
			string[] words = standardised.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			standardised = string.Join(" ", words);
			return isCapitalised ? standardised : standardised.ToLower();
		}

		bool IsValidCharacter(char textCharacter)
		{
			if (char.IsLetter(textCharacter) || textCharacter == ' ')
				return true;
			return false;
		}
	}
}
