using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        #region --- MainCfg 記錄無資料歌手 ---

        private void MainCfg_NonSingerDataLog_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            SingerMgr.CreateSongDataTable();
            Common_SwitchSetUI(false);

            MainCfg_Tooltip_Label.Text = "正在解析歌庫歌手資料,請稍待...";

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => MainCfg_NonSingerDataLogTask()));

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
            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, First(Song_SingerType) AS Song_SingerType, Count(Song_Singer) AS Song_SingerCount FROM ktv_Song GROUP BY Song_Singer HAVING (((First(Song_SingerType))<>10) AND ((Count(Song_Singer))>0)) ORDER BY First(Song_SingerType), First(Song_Singer)";
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
                        // 處理合唱歌曲中的特殊歌手名稱
                        foreach (string SpecialSingerName in SpecialStrlist)
                        {
                            Regex SpecialStrRegex = new Regex(SpecialSingerName, RegexOptions.IgnoreCase);
                            if (SpecialStrRegex.IsMatch(SingerName))
                            {
                                if (Singerlist.IndexOf(SpecialSingerName) < 0)
                                {
                                    // 查找資料庫預設歌手資料表
                                    if (SingerMgr.AllSingerLowCaseList.IndexOf(SpecialSingerName.ToLower()) < 0)
                                    {
                                        if (list.IndexOf(SpecialSingerName) < 0)
                                        {
                                            list.Add(SpecialSingerName);
                                            lock (LockThis) { Global.TotalList[1]++; }
                                        }
                                    }
                                    Singerlist.Add(SpecialSingerName);
                                    SingerName = Regex.Replace(SingerName, "&" + SpecialSingerName + "|" + SpecialSingerName + "&", "");
                                }
                            }
                        }

                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                        if (r.IsMatch(SingerName))
                        {
                            string[] singers = Regex.Split(SingerName, "&", RegexOptions.None);
                            foreach (string str in singers)
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
                        }
                        else
                        {
                            if (Singerlist.IndexOf(SingerName) < 0)
                            {
                                if (SingerMgr.AllSingerLowCaseList.IndexOf(SingerName.ToLower()) < 0)
                                {
                                    list.Add(SingerName);
                                    lock (LockThis) { Global.TotalList[1]++; }
                                }
                                Singerlist.Add(SingerName);
                            }
                        }
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

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => MainCfg_NonPhoneticsWordLogTask()));

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
