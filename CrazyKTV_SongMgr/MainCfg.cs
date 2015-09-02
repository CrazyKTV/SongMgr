using System;
using System.Data;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void MainCfg_Save_Button_Click(object sender, EventArgs e)
        {
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgAlwaysOnTop", Global.MainCfgAlwaysOnTop);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableAutoUpdate", Global.MainCfgEnableAutoUpdate);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongDBConverterTabPage", Global.MainCfgHideSongDBConverterTabPage);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongAddResultTabPage", Global.MainCfgHideSongAddResultTabPage);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongLogTabPage", Global.MainCfgHideSongLogTabPage);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupRemoveSongDays", Global.MainCfgBackupRemoveSongDays);
        }

        private void MainCfg_AlwaysOnTop_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.MainCfgAlwaysOnTop = MainCfg_AlwaysOnTop_CheckBox.Checked.ToString();
            this.TopMost = MainCfg_AlwaysOnTop_CheckBox.Checked;
        }

        private void MainCfg_EnableAutoUpdate_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.MainCfgEnableAutoUpdate = MainCfg_EnableAutoUpdate_CheckBox.Checked.ToString();
        }

        private void MainCfg_HideTab_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbox = (CheckBox)sender;
            switch (cbox.Name)
            {
                case "MainCfg_HideSongDBConvTab_CheckBox":
                    Global.MainCfgHideSongDBConverterTabPage = MainCfg_HideSongDBConvTab_CheckBox.Checked.ToString();
                    if (cbox.Checked)
                    {
                        if (MainTabControl.TabPages.IndexOf(SongDBConverter_TabPage) >= 0)
                        {
                            SongDBConverter_TabPage.Hide();
                            MainTabControl.TabPages.Remove(SongDBConverter_TabPage);
                        }
                    }
                    else
                    {
                        if (MainTabControl.TabPages.IndexOf(SongDBConverter_TabPage) < 0)
                        {
                            MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(MainCfg_TabPage) + 1, SongDBConverter_TabPage);
                            SongDBConverter_TabPage.Show();
                        }
                    }
                    break;
                case "MainCfg_HideSongAddResultTab_CheckBox":
                    Global.MainCfgHideSongAddResultTabPage = MainCfg_HideSongAddResultTab_CheckBox.Checked.ToString();
                    if (cbox.Checked)
                    {
                        if (MainTabControl.TabPages.IndexOf(SongAddResult_TabPage) >= 0)
                        {
                            SongAddResult_TabPage.Hide();
                            MainTabControl.TabPages.Remove(SongAddResult_TabPage);
                        }
                    }
                    else
                    {
                        if (MainTabControl.TabPages.IndexOf(SongAddResult_TabPage) < 0)
                        {
                            MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(MainCfg_TabPage) + 1, SongAddResult_TabPage);
                            SongAddResult_TabPage.Show();
                        }
                    }
                    break;
                case "MainCfg_HideSongLogTab_CheckBox":
                    Global.MainCfgHideSongLogTabPage = MainCfg_HideSongLogTab_CheckBox.Checked.ToString();
                    if (cbox.Checked)
                    {
                        if (MainTabControl.TabPages.IndexOf(SongLog_TabPage) >= 0)
                        {
                            SongLog_TabPage.Hide();
                            MainTabControl.TabPages.Remove(SongLog_TabPage);
                        }
                    }
                    else
                    {
                        if (MainTabControl.TabPages.IndexOf(SongLog_TabPage) < 0)
                        {
                            MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(MainCfg_TabPage) + 1, SongLog_TabPage);
                            SongLog_TabPage.Show();
                        }
                    }
                    break;
            }
        }

        private void MainCfg_BackupRemoveSongDays_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainCfg_BackupRemoveSongDays_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                Global.MainCfgBackupRemoveSongDays = MainCfg_BackupRemoveSongDays_ComboBox.SelectedValue.ToString();
            }
        }




    }

    class MainCfg
    {
        public static DataTable GetBackupRemoveSongDaysList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            for (int i = 1; i < 31; i++)
            {
                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = i + " 天";
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
            }
            return list;
        }
    }
}
