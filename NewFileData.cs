namespace LessonLab
{
	static class NewFileData
	{
		public static void CatchNewFileCreated(int unit, int lesson, string title, string language)
		{
			if (System.Windows.Forms.Application.OpenForms["MainForm"] != null)
			{
				(System.Windows.Forms.Application.OpenForms["MainForm"] as MainForm).CatchNewFile(unit, lesson, title, language);
			}
		}
	}
}
