namespace LessonLab
{
	partial class NewFileForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewFileForm));
			LangsGroupBox = new System.Windows.Forms.GroupBox();
			LanguageComboBox = new System.Windows.Forms.ComboBox();
			TypeGroupBox = new System.Windows.Forms.GroupBox();
			RadioRadioButton = new System.Windows.Forms.RadioButton();
			LessonRadioButton = new System.Windows.Forms.RadioButton();
			idGroupBox = new System.Windows.Forms.GroupBox();
			TitleLabel = new System.Windows.Forms.Label();
			LessonUpDownLabel = new System.Windows.Forms.Label();
			UnitUpDownLabel = new System.Windows.Forms.Label();
			LessonUpDown = new System.Windows.Forms.NumericUpDown();
			UnitUpDown = new System.Windows.Forms.NumericUpDown();
			TitleTextBox = new System.Windows.Forms.TextBox();
			CreateNewFile = new System.Windows.Forms.Button();
			LangsGroupBox.SuspendLayout();
			TypeGroupBox.SuspendLayout();
			idGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)LessonUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)UnitUpDown).BeginInit();
			SuspendLayout();
			// 
			// LangsGroupBox
			// 
			LangsGroupBox.Controls.Add(LanguageComboBox);
			LangsGroupBox.ForeColor = System.Drawing.SystemColors.WindowText;
			LangsGroupBox.Location = new System.Drawing.Point(14, 14);
			LangsGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LangsGroupBox.Name = "LangsGroupBox";
			LangsGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LangsGroupBox.Size = new System.Drawing.Size(293, 57);
			LangsGroupBox.TabIndex = 4;
			LangsGroupBox.TabStop = false;
			LangsGroupBox.Text = "Тіл";
			// 
			// LanguageComboBox
			// 
			LanguageComboBox.FormattingEnabled = true;
			LanguageComboBox.Items.AddRange(new object[] { "Рус→Қаз" });
			LanguageComboBox.Location = new System.Drawing.Point(8, 23);
			LanguageComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LanguageComboBox.Name = "LanguageComboBox";
			LanguageComboBox.Size = new System.Drawing.Size(272, 23);
			LanguageComboBox.TabIndex = 0;
			LanguageComboBox.TabStop = false;
			LanguageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;
			// 
			// TypeGroupBox
			// 
			TypeGroupBox.Controls.Add(RadioRadioButton);
			TypeGroupBox.Controls.Add(LessonRadioButton);
			TypeGroupBox.ForeColor = System.Drawing.SystemColors.WindowText;
			TypeGroupBox.Location = new System.Drawing.Point(14, 77);
			TypeGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TypeGroupBox.Name = "TypeGroupBox";
			TypeGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TypeGroupBox.Size = new System.Drawing.Size(293, 57);
			TypeGroupBox.TabIndex = 5;
			TypeGroupBox.TabStop = false;
			TypeGroupBox.Text = "Түрі";
			// 
			// RadioRadioButton
			// 
			RadioRadioButton.AutoSize = true;
			RadioRadioButton.Enabled = false;
			RadioRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			RadioRadioButton.Location = new System.Drawing.Point(149, 22);
			RadioRadioButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RadioRadioButton.Name = "RadioRadioButton";
			RadioRadioButton.Size = new System.Drawing.Size(61, 19);
			RadioRadioButton.TabIndex = 1;
			RadioRadioButton.Text = "Радио";
			RadioRadioButton.UseVisualStyleBackColor = true;
			// 
			// LessonRadioButton
			// 
			LessonRadioButton.AutoSize = true;
			LessonRadioButton.Checked = true;
			LessonRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			LessonRadioButton.Location = new System.Drawing.Point(7, 23);
			LessonRadioButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LessonRadioButton.Name = "LessonRadioButton";
			LessonRadioButton.Size = new System.Drawing.Size(60, 19);
			LessonRadioButton.TabIndex = 0;
			LessonRadioButton.TabStop = true;
			LessonRadioButton.Text = "Сабақ";
			LessonRadioButton.UseVisualStyleBackColor = true;
			// 
			// idGroupBox
			// 
			idGroupBox.Controls.Add(TitleLabel);
			idGroupBox.Controls.Add(LessonUpDownLabel);
			idGroupBox.Controls.Add(UnitUpDownLabel);
			idGroupBox.Controls.Add(LessonUpDown);
			idGroupBox.Controls.Add(UnitUpDown);
			idGroupBox.Controls.Add(TitleTextBox);
			idGroupBox.ForeColor = System.Drawing.SystemColors.WindowText;
			idGroupBox.Location = new System.Drawing.Point(14, 141);
			idGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			idGroupBox.Name = "idGroupBox";
			idGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			idGroupBox.Size = new System.Drawing.Size(293, 138);
			idGroupBox.TabIndex = 6;
			idGroupBox.TabStop = false;
			// 
			// TitleLabel
			// 
			TitleLabel.AutoSize = true;
			TitleLabel.Location = new System.Drawing.Point(7, 74);
			TitleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			TitleLabel.Name = "TitleLabel";
			TitleLabel.Size = new System.Drawing.Size(57, 15);
			TitleLabel.TabIndex = 5;
			TitleLabel.Text = "Тақырып";
			// 
			// LessonUpDownLabel
			// 
			LessonUpDownLabel.AutoSize = true;
			LessonUpDownLabel.Location = new System.Drawing.Point(146, 18);
			LessonUpDownLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			LessonUpDownLabel.Name = "LessonUpDownLabel";
			LessonUpDownLabel.Size = new System.Drawing.Size(42, 15);
			LessonUpDownLabel.TabIndex = 4;
			LessonUpDownLabel.Text = "Сабақ";
			// 
			// UnitUpDownLabel
			// 
			UnitUpDownLabel.AutoSize = true;
			UnitUpDownLabel.Location = new System.Drawing.Point(7, 18);
			UnitUpDownLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			UnitUpDownLabel.Name = "UnitUpDownLabel";
			UnitUpDownLabel.Size = new System.Drawing.Size(41, 15);
			UnitUpDownLabel.TabIndex = 3;
			UnitUpDownLabel.Text = "Бөлім";
			// 
			// LessonUpDown
			// 
			LessonUpDown.Location = new System.Drawing.Point(149, 37);
			LessonUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LessonUpDown.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
			LessonUpDown.Name = "LessonUpDown";
			LessonUpDown.Size = new System.Drawing.Size(132, 21);
			LessonUpDown.TabIndex = 1;
			// 
			// UnitUpDown
			// 
			UnitUpDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			UnitUpDown.Location = new System.Drawing.Point(8, 37);
			UnitUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			UnitUpDown.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
			UnitUpDown.Name = "UnitUpDown";
			UnitUpDown.Size = new System.Drawing.Size(132, 21);
			UnitUpDown.TabIndex = 0;
			// 
			// TitleTextBox
			// 
			TitleTextBox.Font = new System.Drawing.Font("Arial", 15F);
			TitleTextBox.Location = new System.Drawing.Point(7, 92);
			TitleTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TitleTextBox.Name = "TitleTextBox";
			TitleTextBox.Size = new System.Drawing.Size(274, 30);
			TitleTextBox.TabIndex = 2;
			// 
			// CreateNewFile
			// 
			CreateNewFile.Image = Properties.Resources.AddIcon;
			CreateNewFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			CreateNewFile.Location = new System.Drawing.Point(14, 285);
			CreateNewFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CreateNewFile.Name = "CreateNewFile";
			CreateNewFile.Size = new System.Drawing.Size(293, 32);
			CreateNewFile.TabIndex = 7;
			CreateNewFile.Text = "Жаңа сабақты бастау";
			CreateNewFile.UseVisualStyleBackColor = true;
			CreateNewFile.Click += CreateNewFile_Click;
			// 
			// NewFileForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(321, 330);
			Controls.Add(CreateNewFile);
			Controls.Add(idGroupBox);
			Controls.Add(TypeGroupBox);
			Controls.Add(LangsGroupBox);
			Font = new System.Drawing.Font("Arial", 9F);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MaximumSize = new System.Drawing.Size(337, 369);
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(337, 369);
			Name = "NewFileForm";
			Text = "Жаңа сабақты бастау";
			TopMost = true;
			LangsGroupBox.ResumeLayout(false);
			TypeGroupBox.ResumeLayout(false);
			TypeGroupBox.PerformLayout();
			idGroupBox.ResumeLayout(false);
			idGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)LessonUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)UnitUpDown).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.GroupBox LangsGroupBox;
		private System.Windows.Forms.ComboBox LanguageComboBox;
		private System.Windows.Forms.GroupBox TypeGroupBox;
		private System.Windows.Forms.RadioButton RadioRadioButton;
		private System.Windows.Forms.RadioButton LessonRadioButton;
		private System.Windows.Forms.GroupBox idGroupBox;
		private System.Windows.Forms.NumericUpDown LessonUpDown;
		private System.Windows.Forms.NumericUpDown UnitUpDown;
		private System.Windows.Forms.TextBox TitleTextBox;
		private System.Windows.Forms.Button CreateNewFile;
		private System.Windows.Forms.Label TitleLabel;
		private System.Windows.Forms.Label LessonUpDownLabel;
		private System.Windows.Forms.Label UnitUpDownLabel;
	}
}