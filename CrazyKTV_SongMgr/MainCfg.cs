using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {

        #region --- MainCfg 控制項事件 ---

        private void MainCfg_Save_Button_Click(object sender, EventArgs e)
        {
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgAlwaysOnTop", Global.MainCfgAlwaysOnTop);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableAutoUpdate", Global.MainCfgEnableAutoUpdate);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongDBConverterTabPage", Global.MainCfgHideSongDBConverterTabPage);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongAddResultTabPage", Global.MainCfgHideSongAddResultTabPage);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongLogTabPage", Global.MainCfgHideSongLogTabPage);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideApplyCashboxIdButton", Global.MainCfgHideApplyCashboxIdButton);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupRemoveSongDays", Global.MainCfgBackupRemoveSongDays);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUIScale", Global.MainCfgUIScale);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUIFont", Global.MainCfgUIFont);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableUIScale", Global.MainCfgEnableUIScale);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUICustomScale", Global.MainCfgUICustomScale);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupDB", Global.MainCfgBackupDB);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupDBPath", Global.MainCfgBackupDBPath);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgPlayerCore", Global.MainCfgPlayerCore);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgPlayerOutput", Global.MainCfgPlayerOutput);
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

        private void MainCfg_HideButton_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbox = (CheckBox)sender;
            switch (cbox.Name)
            {
                case "MainCfg_HideApplyCashboxIdButton_CheckBox":
                    Global.MainCfgHideApplyCashboxIdButton = MainCfg_HideApplyCashboxIdButton_CheckBox.Checked.ToString();
                    if (cbox.Checked)
                    {
                        Cashbox_ApplyCashboxId_Button.Visible = false;
                    }
                    else
                    {
                        Cashbox_ApplyCashboxId_Button.Visible = true;
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

        private void MainCfg_UIScale_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainCfg_UIScale_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                Global.MainCfgUIScale = MainCfg_UIScale_ComboBox.SelectedValue.ToString();
                MainCfg_UIScale_TextBox.Enabled = (Global.MainCfgUIScale == "6") ? true : false;
            }
        }

        private void MainCfg_UIScale_TextBox_Validated(object sender, EventArgs e)
        {
            Global.MainCfgUICustomScale = MainCfg_UIScale_TextBox.Text;
        }

        private void MainCfg_EnableUIScale_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.MainCfgEnableUIScale = MainCfg_EnableUIScale_CheckBox.Checked.ToString();
            if (Global.MainCfgEnableUIScale == "True")
            {
                MainCfg_UIScale_ComboBox.Enabled = true;
                MainCfg_UIFont_ComboBox.Enabled = true;
                MainCfg_UIScale_Button.Enabled = true;
            }
            else
            {
                MainCfg_UIScale_ComboBox.Enabled = false;
                MainCfg_UIFont_ComboBox.Enabled = false;
                MainCfg_UIScale_Button.Enabled = false;
            }
        }

        private void MainCfg_UIScale_Button_Click(object sender, EventArgs e)
        {
            Common_ScalingUI();
        }

        private void MainCfg_BackupDB_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.MainCfgBackupDB = MainCfg_BackupDB_CheckBox.Checked.ToString();
        }

        private void MainCfg_BackupDB_Button_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (MainCfg_BackupDB_TextBox.Text != "") fbd.SelectedPath = MainCfg_BackupDB_TextBox.Text;
                if (fbd.ShowDialog() == DialogResult.OK && fbd.SelectedPath.Length > 0)
                {
                    Global.MainCfgBackupDBPath = fbd.SelectedPath;
                    MainCfg_BackupDB_TextBox.Text = fbd.SelectedPath;
                }
            }
        }

        private void MainCfg_PlayerCore_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Global.MainCfgPlayerCore = (((RadioButton)sender).Name == "MainCfg_PlayerCore_RadioButton1") ? "1" : "2";
            MainCfg_PlayerOutput_Panel.Enabled = (Global.MainCfgPlayerCore == "1") ? true : false;
        }

        private void MainCfg_PlayerOutput_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Global.MainCfgPlayerOutput = (((RadioButton)sender).Name == "MainCfg_PlayerOutput_RadioButton1") ? "1" : "2";
        }

        #endregion

        #region --- MainCfg 記錄無資料歌手 ---

        private void MainCfg_NonSingerDataLog_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            SingerMgr.CreateSongDataTable();
            Common_SwitchSetUI(false);

            MainCfg_Tooltip_Label.Text = "正在解析歌庫歌手資料,請稍待...";

            var tasks = new List<Task>() { Task.Factory.StartNew(() => MainCfg_NonSingerDataLogTask()) };

            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
            {
                Global.TimerEndTime = DateTime.Now;
                this.BeginInvoke((Action)delegate()
                {
                    MainCfg_Tooltip_Label.Text = "總共從歌庫歌曲解析出 " + Global.TotalList[0] + " 筆歌手資料,查詢到 " + Global.TotalList[1] + " 筆歌手無資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    Common_SwitchSetUI(true);
                });
                SingerMgr.DisposeSongDataTable();
            });
        }

        private void MainCfg_NonSingerDataLogTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Singerlist = new List<string>();
            List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));

            DataTable dt = new DataTable();
            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, First(Song_SingerType) AS Song_SingerType, Count(Song_Singer) AS Song_SingerCount FROM ktv_Song GROUP BY Song_Singer HAVING First(Song_SingerType)<=10 AND First(Song_SingerType)<>8 AND First(Song_SingerType)<>9 AND Count(Song_Singer)>0 ORDER BY First(Song_SingerType), First(Song_Singer)";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SingerQuerySqlStr, "");

            if (dt.Rows.Count > 0)
            {
                string SingerName = "";
                string SingerType = "";

                foreach (DataRow row in dt.AsEnumerable())
                {
                    SingerName = row["Song_Singer"].ToString();
                    SingerType = row["Song_SingerType"].ToString();

                    if (SingerType == "3")
                    {
                        List<string> slist = CommonFunc.GetChorusSingerList(SingerName);
                        foreach (string str in slist)
                        {
                            string ChorusSingerName = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                            if (Singerlist.IndexOf(ChorusSingerName) < 0)
                            {
                                // 查找資料庫預設歌手資料表
                                if (SingerMgr.AllSingerLowCaseList.IndexOf(ChorusSingerName.ToLower()) < 0)
                                {
                                    if (list.IndexOf(ChorusSingerName) < 0)
                                    {
                                        list.Add(ChorusSingerName);
                                        lock (LockThis) { Global.TotalList[1]++; }
                                    }
                                }
                                Singerlist.Add(ChorusSingerName);
                            }
                        }
                        slist.Clear();
                        slist = null;
                    }
                    else
                    {
                        if (Singerlist.IndexOf(SingerName) < 0)
                        {
                            if (SingerMgr.AllSingerLowCaseList.IndexOf(SingerName.ToLower()) < 0)
                            {
                                if (list.IndexOf(SingerName) < 0)
                                {
                                    list.Add(SingerName);
                                    lock (LockThis) { Global.TotalList[1]++; }
                                }
                            }
                            Singerlist.Add(SingerName);
                        }
                    }
                    lock (LockThis) { Global.TotalList[0]++; }
                    this.BeginInvoke((Action)delegate()
                    {
                        MainCfg_Tooltip_Label.Text = "已解析到 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                    });
                }
            }
            Singerlist.Clear();
            dt.Dispose();

            if (list.Count > 0)
            {
                foreach (string str in list)
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【記錄無資料歌手】未在預設歌手資料表的歌手: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }
        }

        #endregion

        #region --- MainCfg 記錄無拼音字 ---

        private void MainCfg_NonPhoneticsWordLog_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            SongMaintenance.CreateSongDataTable();
            Common_SwitchSetUI(false);

            var tasks = new List<Task>() { Task.Factory.StartNew(() => MainCfg_NonPhoneticsWordLogTask()) };

            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
            {
                Global.TimerEndTime = DateTime.Now;
                this.BeginInvoke((Action)delegate()
                {
                    MainCfg_Tooltip_Label.Text = "總共從 " + Global.TotalList[0] + " 首歌曲,查詢到 " + Global.TotalList[1] + " 筆無拼音資料的文字,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    Common_SwitchSetUI(true);
                });
                SongMaintenance.DisposeSongDataTable();
            });
        }

        private void MainCfg_NonPhoneticsWordLogTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            List<string> list = new List<string>();

            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_PlayCount, Song_FileName, Song_Path from ktv_Song order by Song_Id";
            using (DataTable SongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
            {
                string MatchWord = "";
                List<string> wordlist = new List<string>();

                Parallel.ForEach(SongDT.AsEnumerable(), (row, loopState) =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    MatchWord = row["Song_Singer"].ToString() + row["Song_SongName"].ToString();
                    MatchWord = Regex.Replace(MatchWord, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", "");

                    if (MatchWord != "")
                    {
                        MatchCollection CJKCharMatches = Regex.Matches(MatchWord, @"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d]|[\uac00-\ud7ff])");
                        if (CJKCharMatches.Count > 0)
                        {
                            foreach (Match m in CJKCharMatches)
                            {
                                if (wordlist.IndexOf(m.Value) < 0)
                                {
                                    // 查找資料庫拼音資料
                                    if (Global.PhoneticsWordList.IndexOf(m.Value) < 0)
                                    {
                                        if (list.IndexOf(m.Value) < 0)
                                        {
                                            list.Add(m.Value);
                                            lock (LockThis) { Global.TotalList[1]++; }
                                        }
                                    }
                                    wordlist.Add(m.Value);
                                }
                            }
                        }
                    }
                    lock (LockThis) { Global.TotalList[0]++; }
                    this.BeginInvoke((Action)delegate()
                    {
                        MainCfg_Tooltip_Label.Text = "正在解析第 " + Global.TotalList[0] + " 首歌曲的拼音資料,請稍待...";
                    });
                });
                wordlist.Clear();
            }

            if (list.Count > 0)
            {
                foreach (string str in list)
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【記錄無拼音字】未在拼音資料表的文字: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }
            list.Clear();
        }

        #endregion

    }

    class MainCfg
    {

        #region --- MainCfg 程式設定下拉清單 ---

        public static DataTable GetBackupRemoveSongDaysList()
        {
            using (DataTable list = new DataTable())
            {
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

        public static DataTable GetUIScaleList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> ItemList = new List<string>() { "66%", "80%", "100%", "125%", "150%", "自訂" };

                foreach (string str in ItemList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                ItemList.Clear();
                return list;
            }
        }

        public static DataTable GetUIFontList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                FontFamily[] fontFamilies;
                InstalledFontCollection installedFontCollection = new InstalledFontCollection();
                fontFamilies = installedFontCollection.Families;

                List<string> ItemList = new List<string>();
                foreach (FontFamily fontFamilie in fontFamilies)
                {
                    ItemList.Add(fontFamilie.Name);
                }

                foreach (string str in ItemList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                ItemList.Clear();
                return list;
            }
        }

        #endregion

    }
}
