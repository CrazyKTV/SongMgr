using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CrazyKTV_SongMgr.NativeMethods;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void SongAdd_Save_Button_Click(object sender, EventArgs e)
        {
            switch (SongAdd_Save_Button.Text)
            {
                case "儲存設定":
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongLang", Global.SongAddDefaultSongLang);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSingerType", Global.SongAddDefaultSingerType);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongTrack", Global.SongAddDefaultSongTrack);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongType", Global.SongAddDefaultSongType);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongVolume", Global.SongAddDefaultSongVolume);

                    string SongQuerySqlStr = "select * from ktv_SongMgr where Config_Type = 'SpecialStr' order by Config_Value";
                    string SongAddSpecialStr = "";
                    using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongQuerySqlStr, ""))
                    {
                        List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));
                        List<string> SpecialStrLowCaselist = SpecialStrlist.ConvertAll(str => str.ToLower());

                        foreach (string SpecialStrLowCase in SpecialStrLowCaselist)
                        {
                            var query = from row in dt.AsEnumerable()
                                        where row["Config_Value"].ToString().ToLower().Equals(SpecialStrLowCase)
                                        select row;

                            if (query.Count<DataRow>() == 0) SongAddSpecialStr += SpecialStrlist[SpecialStrLowCaselist.IndexOf(SpecialStrLowCase)] + "|";
                        }
                    }
                    SongAddSpecialStr = Regex.Replace(SongAddSpecialStr, @"\|$", "");

                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr", SongAddSpecialStr);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode", Global.SongAddSongIdentificationMode);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode", Global.SongAddDupSongMode);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat", Global.SongAddEngSongNameFormat);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddUseCustomSongID", Global.SongAddUseCustomSongID);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEnableConvToTC", Global.SongAddEnableConvToTC);
                    break;
                case "取消更新":
                    SongAdd_Add_Button.Text = "加入歌庫";
                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(296 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(365 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_Edit_GroupBox.Visible = false;
                    SongAdd_SongAddCfg_GroupBox.Visible = true;
                    SongAdd_SpecialStr_GroupBox.Visible = true;
                    SongAdd_DefaultSongInfo_GroupBox.Visible = true;
                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_Tooltip_Label.Text = "已取消更新重複歌曲!";
                    SongAdd_DragDrop_Label.Visible = (Global.SongMgrSongAddMode != "4") ? true : false;
                    Common_SwitchSetUI(true);
                    break;
                case "取消加入":
                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(296 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(365 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_Edit_GroupBox.Visible = false;
                    SongAdd_SongAddCfg_GroupBox.Visible = true;
                    SongAdd_SpecialStr_GroupBox.Visible = true;
                    SongAdd_DefaultSongInfo_GroupBox.Visible = true;
                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_Tooltip_Label.Text = "已取消加入歌曲!";
                    SongAdd_DragDrop_Label.Visible = (Global.SongMgrSongAddMode != "4") ? true : false;
                    break;
            }
        }

        #region --- SongAdd 控制項事件 ---

        private void SongAdd_SongIdentificationMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongAdd_SongIdentificationMode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                    Global.SongAddSongIdentificationMode = SongAdd_SongIdentificationMode_ComboBox.SelectedValue.ToString();
                    break;
            }
        }

        private void SongAdd_DupSongMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongAdd_DupSongMode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                    Global.SongAddDupSongMode = SongAdd_DupSongMode_ComboBox.SelectedValue.ToString();
                    break;
            }
        }

        private void SongAdd_EngSongNameFormat_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongAddEngSongNameFormat = SongAdd_EngSongNameFormat_CheckBox.Checked.ToString();
        }

        private void SongAdd_UseCustomSongID_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongAddUseCustomSongID = SongAdd_UseCustomSongID_CheckBox.Checked.ToString();
        }

        private void SongAdd_EnableConvToTC_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongAddEnableConvToTC = SongAdd_EnableConvToTC_CheckBox.Checked.ToString();
        }

        private void SongAdd_SpecialStr_ListBox_Enter(object sender, EventArgs e)
        {
            SongAdd_Tooltip_Label.Text = "";
            SongAdd_SpecialStr_Button.Text = "移除";
        }

        private void SongAdd_SpecialStr_TextBox_Enter(object sender, EventArgs e)
        {
            if (SongAdd_Tooltip_Label.Text != "此項目的值含有非法字元!") SongAdd_Tooltip_Label.Text = "";
            SongAdd_SpecialStr_Button.Text = "加入";
        }

        private void SongAdd_SpecialStr_Button_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            switch (SongAdd_SpecialStr_Button.Text)
            {
                case "加入":
                    if (SongAdd_SpecialStr_TextBox.Text != "")
                    {
                        List<string> SpecialStrLowCaselist = new List<string>();

                        string SongQuerySqlStr = "select * from ktv_SongMgr where Config_Type = 'SpecialStr' order by Config_Value";
                        using (DataTable SpecialStrDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongQuerySqlStr, ""))
                        {
                            foreach (DataRow row in SpecialStrDT.AsEnumerable())
                            {
                                SpecialStrLowCaselist.Add(row["Config_Value"].ToString().ToLower());
                            }
                        }

                        if (SpecialStrLowCaselist.IndexOf(SongAdd_SpecialStr_TextBox.Text.ToLower()) >= 0)
                        {
                            SongAdd_Tooltip_Label.Text = "要加入的特殊歌手及歌曲名稱已內建!";
                        }
                        else
                        {
                            if (SongAdd_SpecialStr_ListBox.SelectedItems.Count > 0)
                            {
                                dt = (DataTable)SongAdd_SpecialStr_ListBox.DataSource;

                                foreach (DataRow row in dt.AsEnumerable())
                                {
                                    if (row["Display"].ToString().ToLower() == SongAdd_SpecialStr_TextBox.Text.ToLower())
                                    {
                                        SongAdd_Tooltip_Label.Text = "要加入的特殊歌手及歌曲名稱已存在!";
                                        return;
                                    }
                                }

                                dt.Rows.Add(dt.NewRow());
                                dt.Rows[dt.Rows.Count - 1][0] = SongAdd_SpecialStr_TextBox.Text;
                                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                            }
                            else
                            {
                                using (DataTable NewDT = new DataTable())
                                {
                                    NewDT.Columns.Add(new DataColumn("Display", typeof(string)));
                                    NewDT.Columns.Add(new DataColumn("Value", typeof(int)));

                                    NewDT.Rows.Add(NewDT.NewRow());
                                    NewDT.Rows[NewDT.Rows.Count - 1][0] = SongAdd_SpecialStr_TextBox.Text;
                                    NewDT.Rows[NewDT.Rows.Count - 1][1] = NewDT.Rows.Count;
                                    dt = NewDT.Copy();
                                }
                                SongAdd_SpecialStr_ListBox.DataSource = dt;
                                SongAdd_SpecialStr_ListBox.DisplayMember = "Display";
                                SongAdd_SpecialStr_ListBox.ValueMember = "Value";
                            }
                            Global.SongAddSpecialStr += (Global.SongAddSpecialStr == "") ? SongAdd_SpecialStr_TextBox.Text : "|" + SongAdd_SpecialStr_TextBox.Text;
                            SongAdd_SpecialStr_TextBox.Text = "";
                            SongAdd_Tooltip_Label.Text = "已成功加入特殊歌手及歌曲名稱!";
                        }
                    }
                    else
                    {
                        SongAdd_Tooltip_Label.Text = "尚未輸入要加入的特殊歌手及歌曲名稱!";
                    }
                    break;
                case "移除":
                    if (SongAdd_SpecialStr_ListBox.SelectedItem != null)
                    {
                        string RemoveStr = ((DataRowView)SongAdd_SpecialStr_ListBox.SelectedItem)[0].ToString();
                        int index = int.Parse(SongAdd_SpecialStr_ListBox.SelectedIndex.ToString());
                        dt = (DataTable)SongAdd_SpecialStr_ListBox.DataSource;
                        dt.Rows.RemoveAt(index);
                        Global.SongAddSpecialStr = Regex.Replace(Global.SongAddSpecialStr, @"\|" + RemoveStr, "", RegexOptions.IgnoreCase);
                        SongAdd_Tooltip_Label.Text = "已成功移除特殊歌手及歌曲名稱!";
                    }
                    else
                    {
                        SongAdd_Tooltip_Label.Text = "已無可以刪除的特殊歌手及歌曲名稱!";
                    }
                    break;
            }
        }

        private void SongAdd_DefaultSongInfo_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedValue.ToString() != "System.Data.DataRowView")
            {
                switch (((ComboBox)sender).Name)
                {
                    case "SongAdd_DefaultSongLang_ComboBox":
                        Global.SongAddDefaultSongLang = ((ComboBox)sender).SelectedValue.ToString();
                        break;
                    case "SongAdd_DefaultSingerType_ComboBox":
                        Global.SongAddDefaultSingerType = ((ComboBox)sender).SelectedValue.ToString();
                        break;
                    case "SongAdd_DefaultSongTrack_ComboBox":
                        Global.SongAddDefaultSongTrack = ((ComboBox)sender).SelectedValue.ToString();
                        break;
                    case "SongAdd_DefaultSongType_ComboBox":
                        Global.SongAddDefaultSongType = ((ComboBox)sender).SelectedValue.ToString();
                        break;
                }
            }
        }

        private void SongAdd_DefaultSongVolume_TextBox_Validated(object sender, EventArgs e)
        {
            Global.SongAddDefaultSongVolume = SongAdd_DefaultSongVolume_TextBox.Text;
        }

        #endregion

        #region --- SongAdd 分析歌曲 ---

        private void SongAdd_ElevatedDragDrop(object sender, ElevatedDragDropArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            if (SongAdd_Tooltip_Label.Text == "要加入的歌曲檔案或資料夾不可與歌庫資料夾同目錄!") SongAdd_Tooltip_Label.Text = "";
            if (SongAdd_Tooltip_Label.Text == "要加入的歌曲檔案數量大於最小歌曲剩餘編號!") SongAdd_Tooltip_Label.Text = "";

            List<string> droplist = new List<string>();
            if (e.HWnd == SongAdd_DataGridView.Handle || e.HWnd == SongAdd_DragDrop_Label.Handle)
            {
                foreach (string file in e.Files)
                {
                    droplist.Add(file);
                }
            }

            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));
            List<string> list = new List<string>();

            SongAdd_DataGridView.DataSource = null;

            foreach (string item in droplist)
            {
                if (item.Contains(Global.SongMgrDestFolder) && Global.SongMgrSongAddMode != "3")
                {
                    SongAdd_Tooltip_Label.Text = "要加入的歌曲檔案或資料夾不可與歌庫資料夾同目錄!";
                    break;
                }
                else if (Directory.Exists(item))
                {
                    DirectoryInfo dir = new DirectoryInfo(item);
                    FileInfo[] Files = dir.GetFiles("*", SearchOption.AllDirectories).Where(p => SupportFormat.Contains(p.Extension.ToLower())).ToArray();
                    foreach (FileInfo f in Files)
                    {
                        list.Add(f.FullName);
                    }
                }
                else if (File.Exists(item))
                {
                    FileInfo f = new FileInfo(item);
                    foreach (string s in SupportFormat)
                    {
                        if (f.Extension.ToLower() == s) list.Add(item);
                    }
                }
            }

            if (SongAdd_Tooltip_Label.Text != "要加入的歌曲檔案或資料夾不可與歌庫資料夾同目錄!")
            {
                if (list.Count > 0)
                {
                    SongAdd_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(270 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.SelectionChanged -= new EventHandler(SongAdd_DataGridView_SelectionChanged);
                    SongAdd_DragDrop_Label.Visible = false;
                    SongAdd_Edit_GroupBox.Visible = true;
                    SongAdd_SongAddCfg_GroupBox.Visible = false;
                    SongAdd_SpecialStr_GroupBox.Visible = false;
                    SongAdd_DefaultSongInfo_GroupBox.Visible = false;
                    SongAdd_Save_Button.Text = "取消加入";
                    SongAdd_InitializeEditControl();
                    Common_SwitchSetUI(false);

                    var tasks = new List<Task>()
                    {
                        Task.Factory.StartNew(() => SongAdd_SongAnalysisTask(list))
                    };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        if (Global.SongMgrSongAddMode != "4")
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                SongAdd_DataGridView.SelectionChanged += new EventHandler(SongAdd_DataGridView_SelectionChanged);
                                SongAdd_DataGridView_SelectionChanged(new object(), new EventArgs());
                            });
                        }
                    });
                }
                else
                {
                    SongAdd_Tooltip_Label.Text = "要加入的歌曲檔案或資料夾沒有支援的影音檔格式!";
                }
            }
        }

        private void SongAdd_SongAnalysisTask(List<string> filelist)
        {
            List<string> strlist = new List<string>();
            List<string> SongLangKeyWordList = new List<string>();
            List<string> SingerTypeKeyWordList = new List<string>();
            List<string> SongTrackKeyWordList = new List<string>();

            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };
            SongAnalysis.CreateSongDataTable();
            int total = 0;

            foreach (string str in Global.CrazyktvSongLangKeyWordList)
            {
                strlist = new List<string>(str.Split(','));
                foreach (string liststr in strlist)
                {
                    SongLangKeyWordList.Add(liststr);
                }
                strlist.Clear();
            }

            foreach (string str in Global.CrazyktvSingerTypeKeyWordList)
            {
                strlist = new List<string>(str.Split(','));
                foreach (string liststr in strlist)
                {
                    SingerTypeKeyWordList.Add(liststr);
                }
                strlist.Clear();
            }

            foreach (string str in Global.CrazyktvSongTrackKeyWordList)
            {
                strlist = new List<string>(str.Split(','));
                foreach (string liststr in strlist)
                {
                    SongTrackKeyWordList.Add(liststr);
                }
                strlist.Clear();
            }

            if (Global.SongAddDefaultSongTrack == "6")
            {
                foreach(string str in filelist)
                {
                    lock (LockThis)
                    {
                        total++;
                    }

                    SongAnalysis.SongInfoAnalysis(str, SongLangKeyWordList, SingerTypeKeyWordList, SongTrackKeyWordList);

                    this.BeginInvoke((Action)delegate()
                    {
                        SongAdd_Tooltip_Label.Text = "正在分析第 " + total + " 首歌曲...";
                        if (Global.SongMgrSongAddMode == "4")
                        {
                            SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                            SongMgrCfg_Tooltip_Label.Text = SongAdd_Tooltip_Label.Text;
                        }
                    });
                }
            }
            else
            {
                Parallel.ForEach(filelist, (str, loopState) =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    lock (LockThis)
                    {
                        total++;
                    }

                    SongAnalysis.SongInfoAnalysis(str, SongLangKeyWordList, SingerTypeKeyWordList, SongTrackKeyWordList);

                    this.BeginInvoke((Action)delegate()
                    {
                        SongAdd_Tooltip_Label.Text = "正在分析第 " + total + " 首歌曲...";
                        if (Global.SongMgrSongAddMode == "4")
                        {
                            SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                            SongMgrCfg_Tooltip_Label.Text = SongAdd_Tooltip_Label.Text;
                        }
                    });
                });
            }

            this.BeginInvoke((Action)delegate()
            {
                int sortindex = 0;
                SongAnalysis.SongAnalysisDT.DefaultView.Sort = "Song_Singer, Song_SongName";
                SongAnalysis.SongAnalysisDT = SongAnalysis.SongAnalysisDT.DefaultView.ToTable();
                SongAdd_DataGridView.DataSource = SongAnalysis.SongAnalysisDT;
                
                for (int i = 0; i < SongAdd_DataGridView.ColumnCount; i++)
                {
                    List<string> DataGridViewColumnName = SongAdd.GetDataGridViewColumnSet(SongAdd_DataGridView.Columns[i].Name);
                    SongAdd_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                    switch (SongAdd_DataGridView.Columns[i].HeaderText)
                    {
                        case "來源檔案路徑":
                            int ColumnWidth_640 = Convert.ToInt32(640 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor);
                            SongAdd_DataGridView.Columns[i].MinimumWidth = ColumnWidth_640;
                            SongAdd_DataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            break;
                    }

                    if (DataGridViewColumnName[1].ToString() == "0")
                    {
                        SongAdd_DataGridView.Columns[i].Visible = false;
                    }

                    if (DataGridViewColumnName[2].ToString() != "none")
                    {
                        ((DataGridViewTextBoxColumn)SongAdd_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                    }

                    SongAdd_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                    if (SongAdd_DataGridView.Columns[i].HeaderText == "排序索引") sortindex = i;
                }
                SongAdd_DataGridView.Sort(SongAdd_DataGridView.Columns[sortindex], ListSortDirection.Ascending);

                if (Global.SongMgrSongAddMode != "4")
                {
                    Common_SwitchSetUI(true);
                    this.Activate();
                    SongAdd_DataGridView.Focus();
                    SongAdd_Add_Button.Enabled = SongAdd_CheckSongAddStatus();
                }

                Global.TimerEndTime = DateTime.Now;

                if (SongAdd.RemainingSongIdCountStr != "")
                {
                    SongAdd_Tooltip_Label.Text = SongAdd.RemainingSongIdCountStr;
                }
                else
                {
                    SongAdd_Tooltip_Label.Text = "已成功分析 " + (total - Global.TotalList[0]) + " 首歌曲,忽略 " + Global.TotalList[0] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成分析。";
                }

                SongLangKeyWordList.Clear();
                SingerTypeKeyWordList.Clear();
                SongTrackKeyWordList.Clear();
                SongAnalysis.DisposeSongDataTable();
            });
        }

        private bool SongAdd_CheckSongAddStatus()
        {
            SongAdd.RemainingSongIdCountStr = "";
            using (DataTable dt = SongAdd_DataGridView.DataSource as DataTable)
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals("未知")
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    SongAnalysis.SongAnalysisError = true;
                    return false;
                }
                else
                {
                    foreach (string langstr in Global.CrazyktvSongLangList)
                    {
                        var langquery = from row in dt.AsEnumerable()
                                        where row.Field<string>("Song_Lang").Equals(langstr)
                                        select row;

                        if (Global.CrazyktvSongLangList.IndexOf(langstr) >= 0)
                        {
                            int LangIndex = Global.CrazyktvSongLangList.IndexOf(langstr);
                            int SongCount = langquery.Count<DataRow>() + 1;

                            if (SongCount > Global.RemainingSongIdCountList[LangIndex])
                            {
                                SongAdd.RemainingSongIdCountStr = langstr + "歌曲的剩餘歌曲編號已不夠所加入的" + langstr + "歌曲使用!";
                                SongAdd_Edit_GroupBox.Enabled = false;
                                SongAnalysis.SongAnalysisError = true;
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
        }

        #endregion

        #region --- SongAdd 加入/更新歌曲按鈕點擊事件 ---

        private void SongAdd_Add_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            SongAdd_Add_Button.Enabled = false;
            SongAdd_DataGridView.Enabled = false;
            Common_SwitchSetUI(false);
            switch(SongAdd_Add_Button.Text)
            {
                case "加入歌庫":
                    Task.Factory.StartNew(SongAdd_SongAddTask);
                    break;
                case "更新歌庫":
                    Task.Factory.StartNew(SongAdd_SongUpdateTask);
                    break;
            }
        }

        #endregion

        #region --- SongAdd 加入歌曲 ---

        private void SongAdd_SongAddTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            this.BeginInvoke((Action)delegate()
            {
                SongAddSong.SongAddDT = new DataTable();
                SongAddSong.SongAddDT = SongAdd_DataGridView.DataSource as DataTable;
            });

            SongAddSong.SongAddValueList = new List<string>();
            SongAddSong.ChorusSingerList = new List<string>();
            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };
            SongAddSong.CreateSongDataTable();

            CommonFunc.GetMaxSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6);
            CommonFunc.GetNotExistsSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6);

            int count = SongAddSong.SongAddDT.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                SongAddSong.StartAddSong(i);

                this.BeginInvoke((Action)delegate()
                {
                    SongAdd_Tooltip_Label.Text = "已成功加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首...";
                    if (Global.SongMgrSongAddMode == "4")
                    {
                        SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                        SongMgrCfg_Tooltip_Label.Text = SongAdd_Tooltip_Label.Text;
                    }
                });
            }

            OleDbConnection SongAddConn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState";
            string sqlValuesStr = "@SongId, @SongLang, @SongSingerType, @SongSinger, @SongSongName, @SongTrack, @SongSongType, @SongVolume, @SongWordCount, @SongPlayCount, @SongMB, @SongCreatDate, @SongFileName, @SongPath, @SongSpell, @SongSpellNum, @SongSongStroke, @SongPenStyle, @SongPlayState";
            string SongAddSqlStr = "insert into ktv_Song ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(SongAddSqlStr, SongAddConn);

            OleDbCommand singercmd = new OleDbCommand();
            sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
            string SingerAddSqlStr = "insert into ktv_Singer ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            singercmd = new OleDbCommand(SingerAddSqlStr, SongAddConn);

            List<string> valuelist = new List<string>();
            List<string> NotExistsSingerId = new List<string>();
            NotExistsSingerId = CommonFunc.GetNotExistsSingerId("ktv_Singer", Global.CrazyktvDatabaseFile);
            int MaxSingerId = CommonFunc.GetMaxSingerId("ktv_Singer", Global.CrazyktvDatabaseFile) + 1;
            string NextSingerId = "";
            List<string> spelllist = new List<string>();
            List<string> singeraddedlist = new List<string>();

            foreach (string str in SongAddSong.SongAddValueList)
            {
                valuelist = new List<string>(str.Split('|'));

                var AddSongDBTask = Task.Factory.StartNew(() =>
                {
                    cmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                    cmd.Parameters.AddWithValue("@SongLang", valuelist[1]);
                    cmd.Parameters.AddWithValue("@SongSingerType", valuelist[2]);
                    cmd.Parameters.AddWithValue("@SongSinger", valuelist[3]);
                    cmd.Parameters.AddWithValue("@SongSongName", valuelist[4]);
                    cmd.Parameters.AddWithValue("@SongTrack", valuelist[5]);
                    cmd.Parameters.AddWithValue("@SongSongType", valuelist[6]);
                    cmd.Parameters.AddWithValue("@SongVolume", valuelist[7]);
                    cmd.Parameters.AddWithValue("@SongWordCount", valuelist[8]);
                    cmd.Parameters.AddWithValue("@SongPlayCount", valuelist[9]);
                    cmd.Parameters.AddWithValue("@SongMB", valuelist[10]);
                    cmd.Parameters.AddWithValue("@SongCreatDate", valuelist[11]);
                    cmd.Parameters.AddWithValue("@SongFileName", valuelist[12]);
                    cmd.Parameters.AddWithValue("@SongPath", valuelist[13]);
                    cmd.Parameters.AddWithValue("@SongSpell", valuelist[14]);
                    cmd.Parameters.AddWithValue("@SongSpellNum", valuelist[15]);
                    cmd.Parameters.AddWithValue("@SongSongStroke", valuelist[16]);
                    cmd.Parameters.AddWithValue("@SongPenStyle", valuelist[17]);
                    cmd.Parameters.AddWithValue("@SongPlayState", valuelist[18]);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        Global.TotalList[3]++;
                    }
                    catch
                    {
                        Global.TotalList[0]--;
                        Global.TotalList[2]++;
                        Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                        Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "加入歌曲時發生未知的錯誤: " + str;
                        Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                    }
                    cmd.Parameters.Clear();
                });

                var AddSingerDBTask = Task.Factory.StartNew(() =>
                {
                    if (valuelist[19] == "1")
                    {
                        string addstatus = "";
                        if (singeraddedlist.Count == 0)
                        {
                            singeraddedlist.Add(valuelist[3]);
                        }
                        else
                        {
                            if (singeraddedlist.IndexOf(valuelist[3]) >= 0) addstatus = "added";
                        }

                        if (addstatus != "added")
                        {
                            singeraddedlist.Add(valuelist[3]);
                            if (NotExistsSingerId.Count > 0)
                            {
                                NextSingerId = NotExistsSingerId[0];
                                NotExistsSingerId.RemoveAt(0);
                            }
                            else
                            {
                                NextSingerId = MaxSingerId.ToString();
                                MaxSingerId++;
                            }

                            spelllist = new List<string>();
                            spelllist = CommonFunc.GetSongNameSpell(valuelist[3]);

                            singercmd.Parameters.AddWithValue("@SingerId", NextSingerId);
                            singercmd.Parameters.AddWithValue("@SingerName", valuelist[3]);
                            singercmd.Parameters.AddWithValue("@SingerType", valuelist[2]);
                            singercmd.Parameters.AddWithValue("@SingerSpell", spelllist[0]);
                            singercmd.Parameters.AddWithValue("@SingerStrokes", spelllist[2]);
                            singercmd.Parameters.AddWithValue("@SingerSpellNum", spelllist[1]);
                            singercmd.Parameters.AddWithValue("@SingerPenStyle", spelllist[3]);

                            try
                            {
                                singercmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                                Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "加入歌手至 ktv_Singer 時發生錯誤: " + valuelist[3];
                                Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                            }
                            singercmd.Parameters.Clear();
                        }
                    }
                });

                Task.WaitAll(AddSongDBTask, AddSingerDBTask);
                this.BeginInvoke((Action)delegate()
                {
                    SongAdd_Tooltip_Label.Text = "正在將第 " + Global.TotalList[3] + " 首歌曲寫入資料庫,請稍待...";
                    if (Global.SongMgrSongAddMode == "4")
                    {
                        SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                        SongMgrCfg_Tooltip_Label.Text = SongAdd_Tooltip_Label.Text;
                    }
                });
            }
            SongAddSong.SongAddValueList.Clear();
            singeraddedlist.Clear();
            
            // 加入合唱歌手
            if (SongAddSong.ChorusSingerList.Count > 0)
            {
                List<string> SingerList = new List<string>();
                List<string> SingerLowCaseList = new List<string>();

                string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
                using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, ""))
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        SingerList.Add(row["Singer_Name"].ToString());
                        SingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                    }
                }

                foreach (string singer in SongAddSong.ChorusSingerList)
                {
                    string singertype = "";
                    bool AddSinger = false;

                    if (SongAddSong.AllSingerDataLowCaseList.IndexOf(singer.ToLower()) >= 0)
                    {
                        singertype = SongAddSong.AllSingerDataTypeList[SongAddSong.AllSingerDataLowCaseList.IndexOf(singer.ToLower())];
                    }
                    else
                    {
                        singertype = (Convert.ToInt32(Global.SongAddDefaultSingerType) - 1).ToString();
                    }

                    if (SingerList.Count > 0)
                    {
                        if (SingerLowCaseList.IndexOf(singer.ToLower()) < 0)
                        {
                            AddSinger = true;
                        }
                    }
                    else
                    {
                        AddSinger = true;
                    }

                    if (AddSinger)
                    {
                        if (NotExistsSingerId.Count > 0)
                        {
                            NextSingerId = NotExistsSingerId[0];
                            NotExistsSingerId.RemoveAt(0);
                        }
                        else
                        {
                            NextSingerId = MaxSingerId.ToString();
                            MaxSingerId++;
                        }

                        spelllist = new List<string>();
                        spelllist = CommonFunc.GetSongNameSpell(singer);

                        singercmd.Parameters.AddWithValue("@SingerId", NextSingerId);
                        singercmd.Parameters.AddWithValue("@SingerName", singer);
                        singercmd.Parameters.AddWithValue("@SingerType", singertype);
                        singercmd.Parameters.AddWithValue("@SingerSpell", spelllist[0]);
                        singercmd.Parameters.AddWithValue("@SingerStrokes", spelllist[2]);
                        singercmd.Parameters.AddWithValue("@SingerSpellNum", spelllist[1]);
                        singercmd.Parameters.AddWithValue("@SingerPenStyle", spelllist[3]);

                        try
                        {
                            singercmd.ExecuteNonQuery();
                            Global.TotalList[4]++;
                        }
                        catch
                        {
                            Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "加入合唱歌手至 ktv_Singer 時發生錯誤: " + valuelist[3];
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                        }
                        singercmd.Parameters.Clear();
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongAdd_Tooltip_Label.Text = "正在檢查並加入第 " + Global.TotalList[4] + " 位合唱歌手,請稍待...";
                        if (Global.SongMgrSongAddMode == "4")
                        {
                            SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                            SongMgrCfg_Tooltip_Label.Text = SongAdd_Tooltip_Label.Text;
                        }
                    });
                }
                SingerList.Clear();
                SingerLowCaseList.Clear();
            }
            SongAddSong.ChorusSingerList.Clear();
            SongAddConn.Close();

            List<int> TotalList = Global.TotalList;
            bool UpdateDupSong = false;
            switch (Global.SongAddDupSongMode)
            {
                case "1":
                    SongAddSong.DupSongAddDT.Dispose();
                    SongAddSong.DupSongAddDT = null;
                    break;
                case "2":
                    if (SongAddSong.DupSongAddDT.Rows.Count > 0) UpdateDupSong = true;
                    break;
                case "3":
                    SongAdd_SongUpdateTask();
                    SongAddSong.DupSongAddDT.Dispose();
                    SongAddSong.DupSongAddDT = null;
                    break;
            }

            this.BeginInvoke((Action)delegate()
            {
                Global.TimerEndTime = DateTime.Now;
                if (!UpdateDupSong)
                {
                    switch (Global.SongAddDupSongMode)
                    {
                        case "1":
                            SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                            break;
                        case "2":
                            SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                            break;
                        case "3":
                            SongAdd_Tooltip_Label.Text = "總共加入 " + TotalList[0] + " 首歌曲,加入重複歌曲 " + Global.TotalList[0] + " 首,失敗 " + TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                            break;
                    }

                    if (CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType") != Global.SongMgrSongType)
                    {
                        CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType", Global.SongMgrSongType);
                    }
                    SongMgrCfg_SongType_ListBox.DataSource = SongMgrCfg.GetSongTypeList();
                    SongMgrCfg_SongType_ListBox.DisplayMember = "Display";
                    SongMgrCfg_SongType_ListBox.ValueMember = "Value";
                    Common_RefreshSongType();

                    Common_QueryAddSong(100);
                    SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                    
                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    Task.Factory.StartNew(() => CommonFunc.GetMaxSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));
                    Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));
                    Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(296 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(365 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_Edit_GroupBox.Visible = false;
                    SongAdd_SongAddCfg_GroupBox.Visible = true;
                    SongAdd_SpecialStr_GroupBox.Visible = true;
                    SongAdd_DefaultSongInfo_GroupBox.Visible = true;
                    SongAdd_Add_Button.Enabled = false;

                    if (Global.SongMgrSongAddMode != "4")
                    {
                        SongAdd_DragDrop_Label.Visible = true;
                        Common_SwitchSetUI(true);
                    }
                    MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
                }
                else // 手動處理重歌曲
                {
                    SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                    SongAdd_Add_Button.Text = "更新歌庫";
                    SongAdd_Save_Button.Text = "取消更新";
                    SongAdd_Add_Button.Enabled = true;
                    SongAdd_Save_Button.Enabled = true;
                    SongAdd_DataGridView.DataSource = SongAddSong.DupSongAddDT;
                    SongAdd_DataGridView.Enabled = true;

                    SongAddSong.DupSongAddDT.Dispose();
                    SongAddSong.DupSongAddDT = null;
                }
                SongAddSong.DisposeSongDataTable();
            });
        }

        #endregion

        #region --- SongAdd 更新歌曲 ---

        private void SongAdd_SongUpdateTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };

            if (Global.SongAddDupSongMode == "2")
            {
                SongAddSong.DupSongAddDT = new DataTable();
                SongAddSong.DupSongAddDT = SongAdd_DataGridView.DataSource as DataTable;
            }

            SongAddSong.SongAddValueList = new List<string>();

            int count = SongAddSong.DupSongAddDT.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                SongAddSong.StartUpdateSong(i);

                this.BeginInvoke((Action)delegate()
                {
                    SongAdd_Tooltip_Label.Text = "已成功處理 " + Global.TotalList[3] + " 首重複歌曲,移除原有歌曲 " + Global.TotalList[4] + " 首...";
                });
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId, Song_Lang = @SongLang, Song_SingerType = @SongSingerType, Song_Singer = @SongSinger, Song_SongName = @SongSongName, Song_Track = @SongTrack, Song_SongType = @SongSongType, Song_Volume = @SongVolume, Song_WordCount = @SongWordCount, Song_PlayCount = @SongPlayCount, Song_MB = @SongMB, Song_CreatDate = @SongCreatDate, Song_FileName = @SongFileName, Song_Path = @SongPath, Song_Spell = @SongSpell, Song_SpellNum = @SongSpellNum, Song_SongStroke = @SongSongStroke, Song_PenStyle = @SongPenStyle, Song_PlayState = @SongPlayState";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @OldSongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in SongAddSong.SongAddValueList)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongLang", valuelist[1]);
                cmd.Parameters.AddWithValue("@SongSingerType", valuelist[2]);
                cmd.Parameters.AddWithValue("@SongSinger", valuelist[3]);
                cmd.Parameters.AddWithValue("@SongSongName", valuelist[4]);
                cmd.Parameters.AddWithValue("@SongTrack", valuelist[5]);
                cmd.Parameters.AddWithValue("@SongSongType", valuelist[6]);
                cmd.Parameters.AddWithValue("@SongVolume", valuelist[7]);
                cmd.Parameters.AddWithValue("@SongWordCount", valuelist[8]);
                cmd.Parameters.AddWithValue("@SongPlayCount", valuelist[9]);
                cmd.Parameters.AddWithValue("@SongMB", valuelist[10]);
                cmd.Parameters.AddWithValue("@SongCreatDate", valuelist[11]);
                cmd.Parameters.AddWithValue("@SongFileName", valuelist[12]);
                cmd.Parameters.AddWithValue("@SongPath", valuelist[13]);
                cmd.Parameters.AddWithValue("@SongSpell", valuelist[14]);
                cmd.Parameters.AddWithValue("@SongSpellNum", valuelist[15]);
                cmd.Parameters.AddWithValue("@SongSongStroke", valuelist[16]);
                cmd.Parameters.AddWithValue("@SongPenStyle", valuelist[17]);
                cmd.Parameters.AddWithValue("@SongPlayState", valuelist[18]);
                cmd.Parameters.AddWithValue("@OldSongId", valuelist[0]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis) { Global.TotalList[0]++; }
                }
                catch
                {
                    lock (LockThis) { Global.TotalList[2]++; }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【加歌頁面】更新重複歌曲資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                    this.BeginInvoke((Action)delegate()
                    {
                        SongAdd_Tooltip_Label.Text = "更新重複歌曲資料有誤,請回報操作記錄裡的內容!";
                    });
                }
                cmd.Parameters.Clear();
            }
            conn.Close();
            SongAddSong.SongAddValueList.Clear();

            if (Global.SongAddDupSongMode == "2")
            {
                this.BeginInvoke((Action)delegate()
                {
                    Global.TimerEndTime = DateTime.Now;
                    SongAdd_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首重複歌曲,移除原有歌曲 " + Global.TotalList[4] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成更新。";

                    if (CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType") != Global.SongMgrSongType)
                    {
                        CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType", Global.SongMgrSongType);
                    }
                    SongMgrCfg_SongType_ListBox.DataSource = SongMgrCfg.GetSongTypeList();
                    SongMgrCfg_SongType_ListBox.DisplayMember = "Display";
                    SongMgrCfg_SongType_ListBox.ValueMember = "Value";
                    Common_RefreshSongType();

                    Common_QueryAddSong(100);
                    SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                    
                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());

                    int MaxDigitCode;
                    if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
                    Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode));
                    Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode));
                    Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                    SongAdd_Add_Button.Text = "加入歌庫";
                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(296 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(365 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_Edit_GroupBox.Visible = false;
                    SongAdd_SongAddCfg_GroupBox.Visible = true;
                    SongAdd_SpecialStr_GroupBox.Visible = true;
                    SongAdd_DefaultSongInfo_GroupBox.Visible = true;
                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_DragDrop_Label.Visible = (Global.SongMgrSongAddMode != "4") ? true : false;
                    Common_SwitchSetUI(true);

                    MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
                });
                SongAddSong.DupSongAddDT.Dispose();
                SongAddSong.DupSongAddDT = null;
            }
        }

        #endregion

        #region --- SongAdd 歌曲編輯 ---

        private void SongAdd_EditSongLang_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (SongAdd_EditSongLang_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongAddMultiEditUpdateList[0] = (SongAdd_EditSongLang_ComboBox.Text != "不變更") ? true : false;
                    }
                }
            }
        }

        private void SongAdd_EditSongCreatDate_DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddMultiEditUpdateList[1] = true;
                }
            }
        }

        private void SongAdd_EditSongSinger_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;
                string SongSinger = SongAdd_EditSongSinger_TextBox.Text;

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddMultiEditUpdateList[2] = (SongAdd_EditSongSinger_TextBox.Text != "") ? true : false;
                }
            }
        }

        private void SongAdd_EditSongSingerType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (SongAdd_EditSongSingerType_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongAddMultiEditUpdateList[3] = (SongAdd_EditSongSingerType_ComboBox.Text != "不變更") ? true : false;
                    }
                }
            }
        }

        private void SongAdd_EditSongSongName_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount == 1)
                {
                    string SongSongName = SongAdd_EditSongSongName_TextBox.Text;
                    // 計算歌曲字數
                    List<string> SongWordCountList = new List<string>();
                    SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                    SongAdd_EditSongWordCount_TextBox.Text = SongWordCountList[0];

                    // 取得歌曲拼音
                    List<string> SongSpellList = new List<string>();
                    SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                    SongAdd_EditSongSpell_TextBox.Text = SongSpellList[0];
                }
            }
        }

        private void SongAdd_EditSongSongType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (SongAdd_EditSongSongType_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongAddMultiEditUpdateList[4] = (SongAdd_EditSongSongType_ComboBox.Text != "不變更") ? true : false;
                    }
                }
            }
        }

        private void SongAdd_EditSongTrack_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (SongAdd_EditSongTrack_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongAddMultiEditUpdateList[5] = (SongAdd_EditSongTrack_ComboBox.Text != "不變更") ? true : false;
                    }
                }
            }
        }

        private void SongAdd_EditSongTrack_Button_Click(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount == 1)
                {
                    List<string> list = new List<string>(Global.SongAddDataGridViewSelectList[0].Split('|'));

                    string SongId = SongAdd_EditSongId_TextBox.Text;
                    string SongLang = ((DataRowView)SongAdd_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                    string SongSinger = SongAdd_EditSongSinger_TextBox.Text;
                    string SongSongName = SongAdd_EditSongSongName_TextBox.Text;
                    string SongTrack = SongAdd_EditSongTrack_ComboBox.SelectedValue.ToString();
                    string SongFilePath = list[19];

                    List<string> PlayerSongInfoList = new List<string>() { SongId, SongLang, SongSinger, SongSongName, SongTrack, SongFilePath, "0", "SongAdd" };

                    Global.PlayerUpdateSongValueList = new List<string>();
                    PlayerForm newPlayerForm = new PlayerForm(this, PlayerSongInfoList);
                    newPlayerForm.Show();
                    this.Hide();
                }
            }
        }

        private void SongAdd_EditSongVolume_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddMultiEditUpdateList[6] = (SongAdd_EditSongVolume_TextBox.Text != "") ? true : false;
                }
            }
        }

        private void SongAdd_EditSongPlayCount_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddMultiEditUpdateList[7] = (SongAdd_EditSongPlayCount_TextBox.Text != "") ? true : false;
                }
            }
        }

        private void SongAdd_EditApplyChanges_Button_Click(object sender, EventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (Global.SongAddDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;
                List<string> UpdateList = new List<string>();

                SongAdd_Tooltip_Label.Text = "正在更新歌曲資料,請稍待...";
                Common_SwitchSetUI(false);

                string SongAddStatus;
                string SongLang;
                string SongSingerType;
                string SongSinger;
                string SongSongName;
                string SongTrack;
                string SongSongType;
                string SongVolume;
                string SongWordCount;
                string SongCreatDate;
                string SongSpell;
                string SongSpellNum;
                string SongSongStroke;
                string SongPenStyle;
                string SongSrcPath;

                if (SelectedRowsCount > 1)
                {
                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        SongAddStatus = row.Cells["Song_AddStatus"].Value.ToString();
                        SongLang = row.Cells["Song_Lang"].Value.ToString();
                        SongSingerType = row.Cells["Song_SingerType"].Value.ToString();
                        SongSinger = row.Cells["Song_Singer"].Value.ToString();
                        SongSongName = row.Cells["Song_SongName"].Value.ToString();
                        SongTrack = row.Cells["Song_Track"].Value.ToString();
                        SongSongType = row.Cells["Song_SongType"].Value.ToString();
                        SongVolume = row.Cells["Song_Volume"].Value.ToString();
                        SongWordCount = row.Cells["Song_WordCount"].Value.ToString();
                        SongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();
                        SongSpell = row.Cells["Song_Spell"].Value.ToString();
                        SongSpellNum = row.Cells["Song_SpellNum"].Value.ToString();
                        SongSongStroke = row.Cells["Song_SongStroke"].Value.ToString();
                        SongPenStyle = row.Cells["Song_PenStyle"].Value.ToString();
                        SongSrcPath = row.Cells["Song_SrcPath"].Value.ToString();

                        if (Global.SongAddMultiEditUpdateList[0])
                        {
                            SongLang = ((DataRowView)SongAdd_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        }

                        if (Global.SongAddMultiEditUpdateList[1])
                        {
                            SongCreatDate = SongAdd_EditSongCreatDate_DateTimePicker.Value.ToString();
                        }

                        if (Global.SongAddMultiEditUpdateList[2])
                        {
                            SongSinger = SongAdd_EditSongSinger_TextBox.Text;
                        }

                        if (Global.SongAddMultiEditUpdateList[3])
                        {
                            string SongSingerTypeStr = ((DataRowView)SongAdd_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                            SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                        }

                        if (Global.SongAddMultiEditUpdateList[4])
                        {
                            string SongSongTypeStr = ((DataRowView)SongAdd_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                            SongSongType = (SongSongTypeStr != "無類別") ? SongSongTypeStr : "";
                        }

                        if (Global.SongAddMultiEditUpdateList[5])
                        {
                            SongTrack = ((int)SongAdd_EditSongTrack_ComboBox.SelectedValue - 1).ToString();
                        }

                        if (Global.SongAddMultiEditUpdateList[6])
                        {
                            SongVolume = SongAdd_EditSongVolume_TextBox.Text;
                        }
                        SongAddStatus = (SongLang == "未知") ? "語系類別必須有值才能加歌!" : "";
                        SongAddStatus = (SongAddStatus == "" && SongSingerType == "10") ? "此歌手尚未設定歌手資料!" : SongAddStatus;

                        UpdateList.Add(SongAddStatus + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongCreatDate + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongSrcPath);
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        SongLang = ((DataRowView)SongAdd_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        SongAddStatus = (SongLang == "未知") ? "語系類別必須有值才能加歌!" : "";

                        string SongSingerTypeStr = ((DataRowView)SongAdd_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                        SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                        SongAddStatus = (SongAddStatus == "" && SongSingerType == "10") ? "此歌手尚未設定歌手資料!" : SongAddStatus;

                        SongSinger = SongAdd_EditSongSinger_TextBox.Text;
                        SongSongName = SongAdd_EditSongSongName_TextBox.Text;
                        SongTrack = SongAdd_EditSongTrack_ComboBox.SelectedValue.ToString();

                        string SongSongTypeStr = ((DataRowView)SongAdd_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                        SongSongType = (SongSongTypeStr != "無類別") ? SongSongTypeStr : "";

                        SongVolume = SongAdd_EditSongVolume_TextBox.Text;

                        // 計算歌曲字數
                        List<string> SongWordCountList = new List<string>();
                        SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                        SongWordCount = SongWordCountList[0];

                        SongCreatDate = SongAdd_EditSongCreatDate_DateTimePicker.Value.ToString();

                        // 取得歌曲拼音
                        List<string> SongSpellList = new List<string>();
                        SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                        if (SongSpellList[2] == "") SongSpellList[2] = "0";

                        SongSpell = SongSpellList[0];
                        SongSpellNum = SongSpellList[1];
                        SongSongStroke = SongSpellList[2];
                        SongPenStyle = SongSpellList[3];
                        SongSrcPath = row.Cells["Song_SrcPath"].Value.ToString();

                        UpdateList.Add(SongAddStatus + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongCreatDate + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongSrcPath);
                    }
                }

                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Global.SongAddDataGridViewRestoreSelectList = new List<string>();
                Global.SongAddDataGridViewRestoreCurrentRow = SongAdd_DataGridView.CurrentRow.Cells["Song_SrcPath"].Value.ToString();
                SongAdd_DataGridView.Sorted -= new EventHandler(SongAdd_DataGridView_Sorted);

                bool EnabledButton = true;

                using (DataTable UpdateDT = (DataTable)SongAdd_DataGridView.DataSource)
                {
                    List<string> valuelist;
                    foreach (string UpdateStr in UpdateList)
                    {
                        valuelist = new List<string>(UpdateStr.Split('|'));
                        Global.SongAddDataGridViewRestoreSelectList.Add(valuelist[14]);

                        var query = from row in UpdateDT.AsEnumerable()
                                    where row["Song_SrcPath"].ToString() == valuelist[14]
                                    select row;

                        foreach (DataRow row in query)
                        {
                            row["Song_AddStatus"] = valuelist[0];
                            row["Song_Lang"] = valuelist[1];
                            row["Song_SingerType"] = valuelist[2];
                            row["Song_Singer"] = valuelist[3];
                            row["Song_SongName"] = valuelist[4];
                            row["Song_Track"] = valuelist[5];
                            row["Song_SongType"] = valuelist[6];
                            row["Song_Volume"] = valuelist[7];
                            row["Song_WordCount"] = valuelist[8];
                            row["Song_CreatDate"] = valuelist[9];
                            row["Song_Spell"] = valuelist[10];
                            row["Song_SpellNum"] = valuelist[11];
                            row["Song_SongStroke"] = valuelist[12];
                            row["Song_PenStyle"] = valuelist[13];
                            row["Song_SrcPath"] = valuelist[14];
                        }
                        if (valuelist[0] == "語系類別必須有值才能加歌!") EnabledButton = false;
                        valuelist.Clear();

                        Global.TotalList[0]++;
                        SongAdd_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 筆資料...";
                    }
                    UpdateList.Clear();
                }

                SongAdd_DataGridView.Sorted += new EventHandler(SongAdd_DataGridView_Sorted);
                SongAdd_DataGridView_Sorted(new object(), new EventArgs());

                SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddDataGridViewSelectList = new List<string>();

                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        string SelSongId = row.Cells["Song_Id"].Value.ToString();
                        string SelSongLang = row.Cells["Song_Lang"].Value.ToString();
                        int SelSongSingerType = Convert.ToInt32(row.Cells["Song_SingerType"].Value);
                        string SelSongSinger = row.Cells["Song_Singer"].Value.ToString();
                        string SelSongSongName = row.Cells["Song_SongName"].Value.ToString();
                        int SelSongTrack = Convert.ToInt32(row.Cells["Song_Track"].Value);
                        string SelSongSongType = row.Cells["Song_SongType"].Value.ToString();
                        string SelSongVolume = row.Cells["Song_Volume"].Value.ToString();
                        string SelSongWordCount = row.Cells["Song_WordCount"].Value.ToString();
                        string SelSongPlayCount = row.Cells["Song_PlayCount"].Value.ToString();
                        string SelSongMB = row.Cells["Song_MB"].Value.ToString();
                        string SelSongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();
                        string SelSongFileName = row.Cells["Song_FileName"].Value.ToString();
                        string SelSongPath = row.Cells["Song_Path"].Value.ToString();
                        string SelSongSpell = row.Cells["Song_Spell"].Value.ToString();
                        string SelSongSpellNum = row.Cells["Song_SpellNum"].Value.ToString();
                        string SelSongSongStroke = row.Cells["Song_SongStroke"].Value.ToString();
                        string SelSongPenStyle = row.Cells["Song_PenStyle"].Value.ToString();
                        string SelSongPlayState = row.Cells["Song_PlayState"].Value.ToString();
                        string SelSongSrcPath = row.Cells["Song_SrcPath"].Value.ToString();

                        string SelectValue = SelSongId + "|" + SelSongLang + "|" + SelSongSingerType + "|" + SelSongSinger + "|" + SelSongSongName + "|" + SelSongTrack + "|" + SelSongSongType + "|" + SelSongVolume + "|" + SelSongWordCount + "|" + SelSongPlayCount + "|" + SelSongMB + "|" + SelSongCreatDate + "|" + SelSongFileName + "|" + SelSongPath + "|" + SelSongSpell + "|" + SelSongSpellNum + "|" + SelSongSongStroke + "|" + SelSongPenStyle + "|" + SelSongPlayState + "|" + SelSongSrcPath;
                        Global.SongAddDataGridViewSelectList.Add(SelectValue);
                    }
                }

                SongAdd_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆資料。";
                SongAdd_Add_Button.Enabled = (EnabledButton) ? true : false;
                Common_SwitchSetUI(true);
            }
        }

        private void SongAdd_InitializeEditControl()
        {
            SongAdd_EditSongId_TextBox.Text = "";
            SongAdd_EditSongLang_ComboBox.SelectedValue = 1;
            SongAdd_EditSongCreatDate_DateTimePicker.Value = DateTime.Now;
            SongAdd_EditSongSinger_TextBox.Text = "";
            SongAdd_EditSongSingerType_ComboBox.SelectedValue = 1;
            SongAdd_EditSongSongName_TextBox.Text = "";
            SongAdd_EditSongSongType_ComboBox.SelectedValue = 1;
            SongAdd_EditSongSpell_TextBox.Text = "";
            SongAdd_EditSongWordCount_TextBox.Text = "";
            SongAdd_EditSongSrcPath_TextBox.Text = "";
            SongAdd_EditSongTrack_ComboBox.SelectedValue = 1;
            SongAdd_EditSongVolume_TextBox.Text = "";
            SongAdd_EditSongPlayCount_TextBox.Text = "";

            SongAdd_EditSongId_TextBox.Enabled = false;
            SongAdd_EditSongLang_ComboBox.Enabled = false;
            SongAdd_EditSongCreatDate_DateTimePicker.Enabled = false;
            SongAdd_EditSongSinger_TextBox.Enabled = false;
            SongAdd_EditSongSingerType_ComboBox.Enabled = false;
            SongAdd_EditSongSongName_TextBox.Enabled = false;
            SongAdd_EditSongSongType_ComboBox.Enabled = false;
            SongAdd_EditSongSpell_TextBox.Enabled = false;
            SongAdd_EditSongWordCount_TextBox.Enabled = false;
            SongAdd_EditSongSrcPath_TextBox.Enabled = false;
            SongAdd_EditSongTrack_ComboBox.Enabled = false;
            SongAdd_EditSongTrack_Button.Enabled = false;
            SongAdd_EditSongVolume_TextBox.Enabled = false;
            SongAdd_EditSongPlayCount_TextBox.Enabled = false;
            SongAdd_EditApplyChanges_Button.Enabled = false;
        }

        private void SongAdd_GetSongEditComboBoxList(bool MultiEdit)
        {
            Global.SongAddMultiEdit = MultiEdit;
            SongAdd_EditSongLang_ComboBox.DataSource = SongAdd.GetEditSongLangList(MultiEdit);
            SongAdd_EditSongLang_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongLang_ComboBox.ValueMember = "Value";

            SongAdd_EditSongSingerType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSingerType", MultiEdit, true);
            SongAdd_EditSongSingerType_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongSingerType_ComboBox.ValueMember = "Value";

            SongAdd_EditSongSongType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongType", MultiEdit, true);
            SongAdd_EditSongSongType_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongSongType_ComboBox.ValueMember = "Value";

            SongAdd_EditSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack", MultiEdit, true);
            SongAdd_EditSongTrack_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongTrack_ComboBox.ValueMember = "Value";
        }

        #endregion

    }


    class SongAdd
    {
        public static string RemainingSongIdCountStr = string.Empty;

        #region --- SongAdd 加歌頁面下拉清單 ---

        public static DataTable GetDefaultSongInfo(string SongInfoType, bool MultiEdit, bool SongEditComboBox)
        {
            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add(new DataColumn("Display", typeof(string)));
                dt.Columns.Add(new DataColumn("Value", typeof(int)));

                if (MultiEdit)
                {
                    dt.Rows.Add(dt.NewRow());
                    dt.Rows[dt.Rows.Count - 1][0] = "不變更";
                    dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                }

                List<string> list = new List<string>();

                switch (SongInfoType)
                {
                    case "DefaultSongLang":
                        foreach (string langstr in Global.CrazyktvSongLangList)
                        {
                            list.Add(langstr);
                        }
                        list.Add("未知");
                        break;
                    case "DefaultSingerType":
                        foreach (string SingerTypeStr in Global.CrazyktvSingerTypeList)
                        {
                            if (SingerTypeStr != "未使用")
                            {
                                dt.Rows.Add(dt.NewRow());
                                dt.Rows[dt.Rows.Count - 1][0] = SingerTypeStr;
                                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                            }
                        }
                        break;
                    case "DefaultSongTrack":
                        list.AddRange(Global.CrazyktvSongTrackWordList);
                        break;
                    case "DefaultSongType":
                        string str = "";
                        if (Global.SongMgrSongType != "") { str = "無類別," + Global.SongMgrSongType; } else { str = "無類別"; }
                        list = new List<string>(str.Split(','));
                        break;
                    case "SpecialStr":
                        if (Global.SongAddSpecialStr != "")
                        {
                            string SongAddSpecialStr = Global.SongAddSpecialStr;
                            if (Global.SongAddSpecialStr.IndexOf("|") < 0)
                            {
                                SongAddSpecialStr = "";
                                Global.SongAddSpecialStr = Global.SongAddSpecialStr.Replace(",", "|");
                                string SongQuerySqlStr = "select * from ktv_SongMgr where Config_Type = 'SpecialStr' order by Config_Value";
                                
                                using (DataTable SpecialStrDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongQuerySqlStr, ""))
                                {
                                    List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));
                                    List<string> SpecialStrLowCaselist = SpecialStrlist.ConvertAll(SpecialStr => SpecialStr.ToLower());

                                    foreach (string SpecialStrLowCase in SpecialStrLowCaselist)
                                    {
                                        var query = from row in SpecialStrDT.AsEnumerable()
                                                    where row["Config_Value"].ToString().ToLower().Equals(SpecialStrLowCase)
                                                    select row;

                                        if (query.Count<DataRow>() == 0) SongAddSpecialStr += SpecialStrlist[SpecialStrLowCaselist.IndexOf(SpecialStrLowCase)] + "|";
                                    }
                                }
                                SongAddSpecialStr = Regex.Replace(SongAddSpecialStr, @"\|$", "");
                                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr", SongAddSpecialStr);
                            }
                            str = SongAddSpecialStr;
                        }
                        else
                        {
                            str = "";
                        }
                        if (str != "") list = new List<string>(str.Split('|'));
                        break;
                }

                if (list.Count > 0)
                {
                    foreach (string s in list)
                    {
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = s;
                        dt.Rows[dt.Rows.Count - 1][1] = (SongInfoType == "DefaultSongTrack") ? dt.Rows.Count - 1 : dt.Rows.Count;
                    }

                    if (!SongEditComboBox)
                    {
                        if (SongInfoType == "DefaultSongTrack")
                        {
                            dt.Rows.Add(dt.NewRow());
                            dt.Rows[dt.Rows.Count - 1][0] = "自動偵測 (緩慢)";
                            dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count - 1;
                        }
                    }
                }
                list.Clear();
                return dt;
            }
        }

        public static DataTable GetSongIdentificationModeList()
        {
            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add(new DataColumn("Display", typeof(string)));
                dt.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> list = new List<string>();
                list = new List<string>() { "智能辨識模式", "歌手_歌名", "歌名_歌手", "歌曲編號_歌手_歌名", @"歌手\歌名" };

                foreach (string s in list)
                {
                    dt.Rows.Add(dt.NewRow());
                    dt.Rows[dt.Rows.Count - 1][0] = s;
                    dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                }
                list.Clear();
                return dt;
            }
        }

        public static DataTable GetDupSongModeList()
        {
            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add(new DataColumn("Display", typeof(string)));
                dt.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> list = new List<string>();
                list = new List<string>() { "自動忽略重複歌曲", "手動處理重複歌曲", "檔案容量較大時自動取代" };

                foreach (string s in list)
                {
                    dt.Rows.Add(dt.NewRow());
                    dt.Rows[dt.Rows.Count - 1][0] = s;
                    dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                }
                list.Clear();
                return dt;
            }
        }

        #endregion

        #region --- 歌曲編輯下拉清單 ---

        public static DataTable GetEditSongLangList(bool MultiEdit)
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                if (MultiEdit)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = "不變更";
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }

                foreach (string str in Global.CrazyktvSongLangList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }

                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = "未知";
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                return list;
            }
        }

        #endregion

        #region --- SongAdd 歌曲列表欄位設定 ---

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            string ColumnWidth_120 = Convert.ToInt32((120 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_140 = Convert.ToInt32((140 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_160 = Convert.ToInt32((160 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_240 = Convert.ToInt32((240 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_270 = Convert.ToInt32((270 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_640 = Convert.ToInt32((640 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Song_Id":
                    list = new List<string>() { "歌曲編號", "0", "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", ColumnWidth_120, "none" };
                    break;
                case "Song_SingerType":
                    list = new List<string>() { "歌手類別", ColumnWidth_120, "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", ColumnWidth_160, "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", ColumnWidth_270, "none" };
                    break;
                case "Song_Track":
                    list = new List<string>() { "歌曲聲道", ColumnWidth_140, "none" };
                    break;
                case "Song_SongType":
                    list = new List<string>() { "歌曲類別", ColumnWidth_120, "none" };
                    break;
                case "Song_Volume":
                    list = new List<string>() { "歌曲音量", ColumnWidth_120, "3" };
                    break;
                case "Song_WordCount":
                    list = new List<string>() { "歌曲字數", ColumnWidth_120, "2" };
                    break;
                case "Song_PlayCount":
                    list = new List<string>() { "點播次數", "0", "9" };
                    break;
                case "Song_MB":
                    list = new List<string>() { "歌曲大小", ColumnWidth_120, "7" };
                    break;
                case "Song_CreatDate":
                    list = new List<string>() { "加歌日期", ColumnWidth_140, "none" };
                    break;
                case "Song_FileName":
                    list = new List<string>() { "檔案名稱", "0", "none" };
                    break;
                case "Song_Path":
                    list = new List<string>() { "歌曲路徑", "0", "none" };
                    break;
                case "Song_Spell":
                    list = new List<string>() { "歌曲拼音", "0", "none" };
                    break;
                case "Song_SpellNum":
                    list = new List<string>() { "手機輸入", "0", "none" };
                    break;
                case "Song_SongStroke":
                    list = new List<string>() { "歌曲筆劃", "0", "none" };
                    break;
                case "Song_PenStyle":
                    list = new List<string>() { "筆形順序", "0", "none" };
                    break;
                case "Song_PlayState":
                    list = new List<string>() { "播放狀態", "0", "none" };
                    break;
                case "Song_SrcPath":
                    list = new List<string>() { "來源檔案路徑", ColumnWidth_640, "none" };
                    break;
                case "Song_SortIndex":
                    list = new List<string>() { "排序索引", "0", "none" };
                    break;
                case "Song_AddStatus":
                    list = new List<string>() { "加歌狀況", ColumnWidth_240, "none" };
                    break;
            }
            return list;
        }
        
        #endregion

    }
}
