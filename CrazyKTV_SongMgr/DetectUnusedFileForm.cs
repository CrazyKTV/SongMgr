using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class DetectUnusedFileForm : Form
    {
        public DetectUnusedFileForm()
        {
            InitializeComponent();
        }

        public DetectUnusedFileForm(Form ParentForm)
        {
            InitializeComponent();
            this.Owner = ParentForm;
            Global.DetectUnusedFilePathList = new List<string>();
            if (Global.SongMgrSongAddMode == "1" || Global.SongMgrSongAddMode == "2")
            {
                if (Global.SongMgrDestFolder != "" && Directory.Exists(Global.SongMgrDestFolder))
                    DetectUnusedFile_Path1_TextBox.Text = Global.SongMgrDestFolder;
            }
        }

        private void DetectUnusedFile_Button_Click(object sender, EventArgs e)
        {
            if (DetectUnusedFile_Path1_TextBox.Text != "") Global.DetectUnusedFilePathList.Add(DetectUnusedFile_Path1_TextBox.Text);
            if (DetectUnusedFile_Path2_TextBox.Text != "") Global.DetectUnusedFilePathList.Add(DetectUnusedFile_Path2_TextBox.Text);
            if (DetectUnusedFile_Path3_TextBox.Text != "") Global.DetectUnusedFilePathList.Add(DetectUnusedFile_Path3_TextBox.Text);
            if (DetectUnusedFile_Path4_TextBox.Text != "") Global.DetectUnusedFilePathList.Add(DetectUnusedFile_Path4_TextBox.Text);
            if (DetectUnusedFile_Path5_TextBox.Text != "") Global.DetectUnusedFilePathList.Add(DetectUnusedFile_Path5_TextBox.Text);
            this.Close();
        }

        private void DetectUnusedFileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Owner.Show();
            this.Owner.TopMost = (Global.MainCfgAlwaysOnTop == "True") ? true : false;
        }

        private void DetectUnusedFile_Path_Button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog opd = new FolderBrowserDialog();

            if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
            {
                switch (((Button)sender).Name)
                {
                    case "DetectUnusedFile_Path1_Button":
                        DetectUnusedFile_Path1_TextBox.Text = opd.SelectedPath;
                        break;
                    case "DetectUnusedFile_Path2_Button":
                        DetectUnusedFile_Path2_TextBox.Text = opd.SelectedPath;
                        break;
                    case "DetectUnusedFile_Path3_Button":
                        DetectUnusedFile_Path3_TextBox.Text = opd.SelectedPath;
                        break;
                    case "DetectUnusedFile_Path4_Button":
                        DetectUnusedFile_Path4_TextBox.Text = opd.SelectedPath;
                        break;
                    case "DetectUnusedFile_Path5_Button":
                        DetectUnusedFile_Path5_TextBox.Text = opd.SelectedPath;
                        break;
                }
            }
        }
    }
}
