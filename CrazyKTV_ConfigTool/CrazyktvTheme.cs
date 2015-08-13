using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrazyKTV_ConfigTool
{
    public partial class MainForm : Form
    {
        private void CrazyktvTheme_SaveCrazyktvCfgFile()
        {
            CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "AutoScreen", Global.CrazyktvAutoScreen);
            CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "ScreenDpi", Global.CrazyktvScreenDpi);
            CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "WinState", Global.CrazyktvWinState);
            CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "D3DButton", Global.CrazyktvD3DButton);
            CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "MainScreen", Global.CrazyktvMainScreen);
            CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "PlayScreen", Global.CrazyktvPlayScreen);
        }

        private void CrazyktvTheme_AutoScreen_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.CrazyktvAutoScreen = CrazyktvTheme_AutoScreen_CheckBox.Checked.ToString();
            CrazyktvTheme_SaveCrazyktvCfgFile();
        }

        private void CrazyktvTheme_ScreenDpi_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CrazyktvTheme_ScreenDpi_ComboBox.SelectedValue.ToString())
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "10":
                case "11":
                case "12":
                    Global.CrazyktvScreenDpi = CrazyktvTheme_ScreenDpi_ComboBox.SelectedValue.ToString();
                    CrazyktvTheme_SaveCrazyktvCfgFile();
                    break;
            }
        }

        private void CrazyktvTheme_WinState_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.CrazyktvWinState = CrazyktvTheme_WinState_CheckBox.Checked.ToString();
            CrazyktvTheme_SaveCrazyktvCfgFile();
        }

        private void CrazyktvTheme_D3DButton_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.CrazyktvD3DButton = CrazyktvTheme_D3DButton_CheckBox.Checked.ToString();
            CrazyktvTheme_SaveCrazyktvCfgFile();
        }

        private void CrazyktvTheme_MainScreen_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CrazyktvTheme_MainScreen_CheckBox.Checked)
            {
                Global.CrazyktvMainScreen = "0";
                Global.CrazyktvPlayScreen = "1";
            }
            else
            {
                Global.CrazyktvMainScreen = "1";
                Global.CrazyktvPlayScreen = "0";
            }
            CrazyktvTheme_SaveCrazyktvCfgFile();
        }

        private void CrazyktvTheme_ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CrazyktvTheme_ListView.SelectedItems.Count > 0)
            {
                CrazyktvTheme_PictureBox.Image.Dispose();
                if (!CrazyktvTheme_ApplyTheme_Button.Enabled)
                {
                    CrazyktvTheme_ApplyTheme_Button.Enabled = true;
                }

                CrazyktvTheme_PictureBox.Image.Dispose();

                ListView.SelectedListViewItemCollection selected = CrazyktvTheme_ListView.SelectedItems;
                string imagefile = selected[0].SubItems[1].Text;
                Bitmap bmpPic1 = new Bitmap(Image.FromFile(imagefile), 1024, 768);
                Bitmap bmpPic2 = new Bitmap(global::CrazyKTV_ConfigTool.Properties.Resources.tbasic, 1024, 768);
                Graphics g = Graphics.FromImage(bmpPic1);
                g.DrawImage(bmpPic2, new Point(0, 0));
                CrazyktvTheme_PictureBox.Image = new Bitmap(bmpPic1);

                g.Dispose();
                bmpPic1.Dispose();
                bmpPic2.Dispose();

                GC.Collect(2, GCCollectionMode.Forced);
            }
        }

        private void CrazyktvTheme_ApplyTheme_Button_Click(object sender, EventArgs e)
        {
            if (CrazyktvTheme_ListView.SelectedItems.Count > 0)
            {
                ListView.SelectedListViewItemCollection selected = CrazyktvTheme_ListView.SelectedItems;
                string sourceFileName = Path.GetFileName(selected[0].SubItems[1].Text);
                string targetFileName = "basic.jpg";
                string sourcePath = Path.GetDirectoryName(selected[0].SubItems[1].Text);
                string WinStateVar = "";
                string ScreenDpiVar = "";

                if (CrazyktvTheme_WinState_CheckBox.Checked)
                {
                    WinStateVar = @"\windows";
                }

                List<string> list = new List<string>();
                list = new List<string>() { "1024X768", "1920X1080", "1680X1050", "1600X1200", "1600X1024", "1440X900", "1366X768", "1280X1024", "1280X800", "1280X720", "800X600", "800X480", "640X480" };
                
                if (Global.CrazyktvAutoScreen == "True")
                {
                    if (Screen.AllScreens.Length < 2)
                    {
                        ScreenDpiVar = Screen.PrimaryScreen.Bounds.Width + "X" + Screen.PrimaryScreen.Bounds.Height;
                    }
                    else
                    {
                        if (CrazyktvTheme_MainScreen_CheckBox.Checked)
                        {
                            ScreenDpiVar = Screen.AllScreens[0].Bounds.Width + "X" + Screen.AllScreens[0].Bounds.Height;
                        }
                        else
                        {
                            ScreenDpiVar = Screen.AllScreens[1].Bounds.Width + "X" + Screen.AllScreens[1].Bounds.Height;
                        }
                    }
                }
                else
                {
                    ScreenDpiVar = list[Convert.ToInt32(Global.CrazyktvScreenDpi)];
                }

                string targetPath = Application.StartupPath + @"\BackGround\" + ScreenDpiVar + WinStateVar;
                string sourceFile = Path.Combine(sourcePath, sourceFileName);
                string destFile = Path.Combine(targetPath, targetFileName);

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                File.Copy(sourceFile, destFile, true);

                string CrazyKTV_BackGround_File = Application.StartupPath + @"\CrazyKTV_BackGround.exe";
                if (File.Exists(CrazyKTV_BackGround_File))
                {
                    Process p = Process.Start(CrazyKTV_BackGround_File, "MakeTheme");
                    p.WaitForExit();
                }
            }
        }

        private void CrazyktvTheme_GetFileList()
        {
            string path = Application.StartupPath + @"\我的最愛背景圖檔";
            if (Directory.Exists(path))
            {
                CrazyktvTheme_ListView.Columns.Add("佈景主題名稱", -2, HorizontalAlignment.Center);

                string[] ThemeFileExtensions = new[] { ".bmp", ".gif", ".jpg", ".png" };
                DirectoryInfo ThemeDirectory = new DirectoryInfo(path);
                FileInfo[] ThemeFiles = ThemeDirectory.GetFiles().Where(p => ThemeFileExtensions.Contains(p.Extension.ToLower())).ToArray();

                foreach (FileInfo f in ThemeFiles)
                {
                    string ThemeFileName = Path.GetFileNameWithoutExtension(f.ToString());
                    ListViewItem item = CrazyktvTheme_ListView.Items.Add(ThemeFileName);
                    item.SubItems.Add(f.FullName.ToString());
                }
                CrazyktvTheme_ListView.Scrollable = false;
                NativeMethods.ShowScrollBar((int)CrazyktvTheme_ListView.Handle, NativeMethods.SB_VERT, 1);
            }
        }


    }
    class CrazyktvTheme
    {
        public static DataTable GetScreenDpiList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "1024 x 768 (4:3)";
            list.Rows[0][1] = 0;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = "1920 x 1080 (16:9)";
            list.Rows[1][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[2][0] = "1680 x 1050 (16:10)";
            list.Rows[2][1] = 2;
            list.Rows.Add(list.NewRow());
            list.Rows[3][0] = "1600 x 1200 (4:3)";
            list.Rows[3][1] = 3;
            list.Rows.Add(list.NewRow());
            list.Rows[4][0] = "1600 x 1024 (16:10)";
            list.Rows[4][1] = 4;
            list.Rows.Add(list.NewRow());
            list.Rows[5][0] = "1440 x 900 (16:10)";
            list.Rows[5][1] = 5;
            list.Rows.Add(list.NewRow());
            list.Rows[6][0] = "1366 x 768 (16:9)";
            list.Rows[6][1] = 6;
            list.Rows.Add(list.NewRow());
            list.Rows[7][0] = "1280 x 1024 (5:4)";
            list.Rows[7][1] = 7;
            list.Rows.Add(list.NewRow());
            list.Rows[8][0] = "1280 x 800 (16:10)";
            list.Rows[8][1] = 8;
            list.Rows.Add(list.NewRow());
            list.Rows[9][0] = "1280 x 720 (16:9)";
            list.Rows[9][1] = 9;
            list.Rows.Add(list.NewRow());
            list.Rows[10][0] = "800 x 600 (4:3)";
            list.Rows[10][1] = 10;
            list.Rows.Add(list.NewRow());
            list.Rows[11][0] = "800 x 480 (5:3)";
            list.Rows[11][1] = 11;
            list.Rows.Add(list.NewRow());
            list.Rows[12][0] = "640 x 480 (4:3)";
            list.Rows[12][1] = 12;
            return list;
        }
    }
}
