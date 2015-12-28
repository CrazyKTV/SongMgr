using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr", Global.SongAddSpecialStr);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode", Global.SongAddSongIdentificationMode);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode", Global.SongAddDupSongMode);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat", Global.SongAddEngSongNameFormat);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddUseCustomSongID", Global.SongAddUseCustomSongID);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEnableConvToTC", Global.SongAddEnableConvToTC);
                    break;
                case "取消更新":
                    SongAdd_Add_Button.Text = "加入歌庫";
                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_Tooltip_Label.Text = "已取消更新重複歌曲!";
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.AllowDrop = true;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_DragDrop_Label.Visible = (Global.SongMgrSongAddMode != "4") ? true : false;
                    Common_SwitchSetUI(true);
                    break;
                case "取消加入":
                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_DataGridView.Size = new Size(952, 296);
                    SongAdd_DataGridView.Location = new Point(23, 365);
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.AllowDrop = true;
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

        private void SongAdd_SongIdentificationMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongAdd_SongIdentificationMode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                    Global.SongAddSongIdentificationMode = SongAdd_SongIdentificationMode_ComboBox.SelectedValue.ToString();
                    break;
            }
        }

        private void SongAdd_DupSongMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(SongAdd_DupSongMode_ComboBox.SelectedValue.ToString())
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
                        if (SongAdd_Tooltip_Label.Text == "尚未輸入要加入的特殊歌手及歌曲名稱!") SongAdd_Tooltip_Label.Text = "";
                        dt = (DataTable)SongAdd_SpecialStr_ListBox.DataSource;
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = SongAdd_SpecialStr_TextBox.Text;
                        dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                        SongAdd_SpecialStr_TextBox.Text = "";

                        List<string> list = new List<string>();

                        foreach (DataRow row in dt.Rows)
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (row[column] != null)
                                {
                                    if (column.ColumnName == "Display")
                                    {
                                        list.Add(row[column].ToString());
                                    }
                                }
                            }
                        }
                        Global.SongAddSpecialStr = string.Join(",", list);
                    }
                    else
                    {
                        SongAdd_Tooltip_Label.Text = "尚未輸入要加入的特殊歌手及歌曲名稱!";
                    }
                    break;
                case "移除":
                    if (SongAdd_SpecialStr_ListBox.SelectedItem != null)
                    {
                        int index = int.Parse(SongAdd_SpecialStr_ListBox.SelectedIndex.ToString());
                        dt = (DataTable)SongAdd_SpecialStr_ListBox.DataSource;
                        dt.Rows.RemoveAt(index);

                        List<string> list = new List<string>();

                        foreach (DataRow row in dt.Rows)
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (row[column] != null)
                                {
                                    if (column.ColumnName == "Display")
                                    {
                                        list.Add(row[column].ToString());
                                    }
                                }
                            }
                        }
                        Global.SongAddSpecialStr = string.Join(",", list);
                    }
                    else
                    {
                        SongAdd_Tooltip_Label.Text = "已無可以刪除的特殊歌手及歌曲名稱!";
                    }
                    break;
            }
        }

        private void SongAdd_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void SongAdd_DragDrop(object sender, DragEventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            if (SongAdd_Tooltip_Label.Text == "要加入的歌曲檔案或資料夾不可與歌庫資料夾同目錄!") SongAdd_Tooltip_Label.Text = "";
            if (SongAdd_Tooltip_Label.Text == "要加入的歌曲檔案數量大於最小歌曲剩餘編號!") SongAdd_Tooltip_Label.Text = "";

            string[] drop = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> SupportFormat = new List<string>();
            SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));
            List<string> list = new List<string>();

            SongAdd_DataGridView.DataSource = null;

            foreach (string item in drop)
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
                    if (list.Count > Global.RemainingSongID)
                    {
                        SongAdd_Tooltip_Label.Text = "要加入的歌曲檔案數量大於最小歌曲剩餘編號!";
                    }
                    else
                    {
                        SongAdd_DataGridView.Size = new Size(952, 270);
                        SongAdd_DataGridView.Location = new Point(23, 23);
                        SongAdd_DataGridView.AllowDrop = false;
                        SongAdd_DataGridView.SelectionChanged -= new EventHandler(SongAdd_DataGridView_SelectionChanged);
                        SongAdd_DragDrop_Label.Visible = false;
                        SongAdd_Edit_GroupBox.Visible = true;
                        SongAdd_SongAddCfg_GroupBox.Visible = false;
                        SongAdd_SpecialStr_GroupBox.Visible = false;
                        SongAdd_DefaultSongInfo_GroupBox.Visible = false;
                        SongAdd_Save_Button.Text = "取消加入";
                        Common_SwitchSetUI(false);

                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongAdd_SongAnalysisTask(list)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            if (Global.SongMgrSongAddMode != "4")
                            {
                                this.BeginInvoke((Action)delegate ()
                                {
                                    SongAdd_DataGridView.SelectionChanged += new EventHandler(SongAdd_DataGridView_SelectionChanged);
                                    SongAdd_InitializeEditControl();
                                    SongAdd_DataGridView_SelectionChanged(new object(), new EventArgs());
                                });
                            }
                        });
                    }
                }
                else
                {
                    SongAdd_Tooltip_Label.Text = "要加入的歌曲檔案或資料夾沒有支援的影音檔格式!";
                }
            }
        }

        private void SongAdd_SongAnalysisTask(object file)
        {
            List<string> list = (List<string>)file;
            List<string> strlist = new List<string>();
            List<string> SongLangKeyWordList = new List<string>();
            List<string> SingerTypeKeyWordList = new List<string>();
            List<string> SongTrackKeyWordList = new List<string>();
            
            SongAnalysis.CreateSongDataTable();
            int total = 0;

            foreach (string str in Global.CrazyktvSongLangKeyWordList)
            {
                strlist = new List<string>(str.Split(','));
                foreach (string liststr in strlist)
                {
                    SongLangKeyWordList.Add(liststr);
                }
            }

            foreach (string str in Global.CrazyktvSingerTypeKeyWordList)
            {
                strlist = new List<string>(str.Split(','));
                foreach (string liststr in strlist)
                {
                    SingerTypeKeyWordList.Add(liststr);
                }
            }

            foreach (string str in Global.CrazyktvSongTrackKeyWordList)
            {
                strlist = new List<string>(str.Split(','));
                foreach (string liststr in strlist)
                {
                    SongTrackKeyWordList.Add(liststr);
                }
            }

            Parallel.ForEach(list, (str, loopState) =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                lock(LockThis)
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

            this.BeginInvoke((Action)delegate()
            {
                int sortindex = 0;
                Global.SongAddDT.DefaultView.Sort = "Song_Singer, Song_SongName";
                Global.SongAddDT = Global.SongAddDT.DefaultView.ToTable();
                SongAdd_DataGridView.DataSource = Global.SongAddDT;
                
                for (int i = 0; i < SongAdd_DataGridView.ColumnCount; i++)
                {
                    List<string> DataGridViewColumnName = SongAdd.GetDataGridViewColumnSet(SongAdd_DataGridView.Columns[i].Name);
                    SongAdd_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                    switch (SongAdd_DataGridView.Columns[i].HeaderText)
                    {
                        case "來源檔案路徑":
                            SongAdd_DataGridView.Columns[i].MinimumWidth = 640;
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

                SongAdd_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                SongAdd_DataGridView.Sort(SongAdd_DataGridView.Columns[sortindex], ListSortDirection.Ascending);

                if (Global.SongMgrSongAddMode != "4")
                {
                    SongAdd_DataGridView.AllowDrop = true;
                    Common_SwitchSetUI(true);
                    this.Activate();
                    SongAdd_DataGridView.Focus();
                    SongAdd_Add_Button.Enabled = SongAdd_CheckSongAddStatus();
                }

                Global.TimerEndTime = DateTime.Now;
                SongAdd_Tooltip_Label.Text = "總共分析 " + total + " 首歌曲, 共花費 "  +(long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成分析。";
                SongAnalysis.DisposeSongDataTable();
            });
        }

        private bool SongAdd_CheckSongAddStatus()
        {
            DataTable dt = new DataTable();
            dt = SongAdd_DataGridView.DataSource as DataTable;

            var query = from row in dt.AsEnumerable()
                        where row.Field<string>("Song_Lang").Equals("未知")
                        select row;
            if (query.Count<DataRow>() > 0) { return false; } else { return true; }
        }


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

        private void SongAdd_SongAddTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            this.BeginInvoke((Action)delegate()
            {
                Global.SongAddDT = new DataTable();
                Global.SongAddDT = SongAdd_DataGridView.DataSource as DataTable;
            });
            
            Global.SongAddValueList = new List<string>();
            Global.SongAddChorusSingerList = new List<string>();
            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };
            SongAddSong.CreateSongDataTable();

            int MaxDigitCode;
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
            CommonFunc.GetMaxSongId(MaxDigitCode);
            CommonFunc.GetNotExistsSongId(MaxDigitCode);

            int count = Global.SongAddDT.Rows.Count;

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

            foreach (string str in Global.SongAddValueList)
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
            Global.SongAddValueList.Clear();

            // 加入合唱歌手
            if (Global.SongAddChorusSingerList.Count > 0)
            {
                List<string> SingerList = new List<string>();
                List<string> SingerLowCaseList = new List<string>();

                Global.SingerDT = new DataTable();
                string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
                Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

                foreach (DataRow row in Global.SingerDT.AsEnumerable())
                {
                    SingerList.Add(row["Singer_Name"].ToString());
                    SingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                }

                foreach (string singer in Global.SongAddChorusSingerList)
                {
                    string singertype = "";
                    bool AddSinger = false;

                    if (Global.AllSingerLowCaseList.IndexOf(singer.ToLower()) >= 0)
                    {
                        singertype = Global.AllSingerTypeList[Global.AllSingerLowCaseList.IndexOf(singer.ToLower())];

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
            Global.SongAddChorusSingerList.Clear();
            SongAddConn.Close();

            List<int> TotalList = Global.TotalList;
            bool UpdateDupSong = false;
            switch (Global.SongAddDupSongMode)
            {
                case "1":
                    Global.DupSongAddDT.Dispose();
                    Global.DupSongAddDT = null;
                    break;
                case "2":
                    if (Global.DupSongAddDT.Rows.Count > 0) UpdateDupSong = true;
                    break;
                case "3":
                    SongAdd_SongUpdateTask();
                    Global.DupSongAddDT.Dispose();
                    Global.DupSongAddDT = null;
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

                    SongQuery_QueryFilter_ComboBox.SelectedValue = 1;
                    SongQuery_QueryType_ComboBox.SelectedValue = 4;
                    SongQuery_QueryType_ComboBox_SelectedIndexChanged(new ComboBox(), new EventArgs());

                    SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                    Common_RefreshSongType();
                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode));
                    Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode));
                    Task.Factory.StartNew(() => CommonFunc.GetRemainingSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.Enabled = true;

                    if (Global.SongMgrSongAddMode != "4")
                    {
                        SongAdd_DragDrop_Label.Visible = true;
                        Common_SwitchSetUI(true);
                    }

                    SongQuery_QueryType_ComboBox.SelectedValue = 1;
                    SongQuery_QueryValue_TextBox.Text = "";
                    MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
                }
                else
                {
                    SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                    SongAdd_Add_Button.Text = "更新歌庫";
                    SongAdd_Save_Button.Text = "取消更新";
                    SongAdd_Add_Button.Enabled = true;
                    SongAdd_Save_Button.Enabled = true;
                    SongAdd_DataGridView.DataSource = Global.DupSongAddDT;
                    SongAdd_DataGridView.AllowDrop = false;
                    SongAdd_DataGridView.Enabled = true;
                    Global.DupSongAddDT.Dispose();
                    Global.DupSongAddDT = null;
                }
                SongAddSong.DisposeSongDataTable();
            });
        }

        private void SongAdd_SongUpdateTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };

            if (Global.SongAddDupSongMode == "2")
            {
                Global.DupSongAddDT = new DataTable();
                Global.DupSongAddDT = SongAdd_DataGridView.DataSource as DataTable;
            }

            Global.SongAddValueList = new List<string>();

            int count = Global.DupSongAddDT.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                SongAddSong.StartUpdateSong(i);

                this.BeginInvoke((Action)delegate()
                {
                    SongAdd_Tooltip_Label.Text = "已成功搬移 " + Global.TotalList[3] + " 首重複歌曲,移除原有歌曲 " + Global.TotalList[4] + " 首...";
                });
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId, Song_Lang = @SongLang, Song_SingerType = @SongSingerType, Song_Singer = @SongSinger, Song_SongName = @SongSongName, Song_Track = @SongTrack, Song_SongType = @SongSongType, Song_Volume = @SongVolume, Song_WordCount = @SongWordCount, Song_PlayCount = @SongPlayCount, Song_MB = @SongMB, Song_CreatDate = @SongCreatDate, Song_FileName = @SongFileName, Song_Path = @SongPath, Song_Spell = @SongSpell, Song_SpellNum = @SongSpellNum, Song_SongStroke = @SongSongStroke, Song_PenStyle = @SongPenStyle, Song_PlayState = @SongPlayState";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id=@OldSongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in Global.SongAddValueList)
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

                    SongQuery_QueryFilter_ComboBox.SelectedValue = 1;
                    SongQuery_QueryType_ComboBox.SelectedValue = 4;
                    SongQuery_QueryType_ComboBox_SelectedIndexChanged(new ComboBox(), new EventArgs());

                    SongQuery_QueryStatus_Label.Text = SongAdd_Tooltip_Label.Text;
                    Common_RefreshSongType();
                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());

                    int MaxDigitCode;
                    if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
                    Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode));
                    Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode));
                    Task.Factory.StartNew(() => CommonFunc.GetRemainingSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                    SongAdd_Add_Button.Text = "加入歌庫";
                    SongAdd_Save_Button.Text = "儲存設定";
                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.AllowDrop = true;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_DragDrop_Label.Visible = (Global.SongMgrSongAddMode != "4") ? true : false;
                    Common_SwitchSetUI(true);

                    SongQuery_QueryType_ComboBox.SelectedValue = 1;
                    SongQuery_QueryValue_TextBox.Text = "";
                    MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
                });
                Global.DupSongAddDT.Dispose();
                Global.DupSongAddDT = null;
            }
        }

        private void SongAdd_UseCustomSongID_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongAddUseCustomSongID = SongAdd_UseCustomSongID_CheckBox.Checked.ToString();
        }

        private void SongAdd_EnableConvToTC_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongAddEnableConvToTC = SongAdd_EnableConvToTC_CheckBox.Checked.ToString();
        }

        #region --- 歌曲編輯 ---

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
                    if (SongSinger.ContainsAny(Global.CrtchorusSeparateList.ToArray()))
                    {
                        List<string> list = new List<string>(Global.SongAddSpecialStr.Split(',')).ConvertAll(str => str.ToLower());
                        if (list.IndexOf(SongSinger.ToLower()) < 0) SongAdd_EditSongSingerType_ComboBox.SelectedValue = 5;
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    if (SongSinger.ContainsAny(Global.CrtchorusSeparateList.ToArray()))
                    {
                        List<string> list = new List<string>(Global.SongAddSpecialStr.Split(',')).ConvertAll(str => str.ToLower());
                        if (list.IndexOf(SongSinger.ToLower()) < 0) SongAdd_EditSongSingerType_ComboBox.SelectedValue = 4;
                    }
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

                if (SelectedRowsCount > 1)
                {
                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        if (Global.SongAddMultiEditUpdateList[0])
                        {
                            string SongLang = ((DataRowView)SongAdd_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                            if (row.Cells["Song_Lang"].Value.ToString() != SongLang) row.Cells["Song_Lang"].Value = SongLang;
                        }

                        if (Global.SongAddMultiEditUpdateList[1])
                        {
                            string SongCreatDate = SongAdd_EditSongCreatDate_DateTimePicker.Value.ToString("yyyy/M/d") + " " + DateTime.Now.ToString("tt hh:mm:ss");
                            row.Cells["Song_CreatDate"].Value = SongCreatDate;
                        }

                        if (Global.SongAddMultiEditUpdateList[2])
                        {
                            string SongSinger = SongAdd_EditSongSinger_TextBox.Text;
                            row.Cells["Song_Singer"].Value = SongSinger;
                        }

                        if (Global.SongAddMultiEditUpdateList[3])
                        {
                            string SongSingerTypeStr = ((DataRowView)SongAdd_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                            string SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                            row.Cells["Song_SingerType"].Value = SongSingerType;
                        }

                        if (Global.SongAddMultiEditUpdateList[4])
                        {
                            string SongSongType = ((DataRowView)SongAdd_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                            row.Cells["Song_SongType"].Value = SongSongType;
                        }

                        if (Global.SongAddMultiEditUpdateList[5])
                        {
                            int SongTrack = (int)SongAdd_EditSongTrack_ComboBox.SelectedValue - 1;
                            row.Cells["Song_Track"].Value = SongTrack;
                        }

                        if (Global.SongAddMultiEditUpdateList[6])
                        {
                            string SongVolume = SongAdd_EditSongVolume_TextBox.Text;
                            row.Cells["Song_Volume"].Value = SongVolume;
                        }
                        row.Cells["Song_AddStatus"].Value = (row.Cells["Song_Lang"].Value.ToString() == "未知") ? "語系類別必須有值才能加歌!" : "";
                        row.Cells["Song_AddStatus"].Value = (row.Cells["Song_AddStatus"].Value.ToString() == "" && row.Cells["Song_SingerType"].Value.ToString() == "10") ? "此歌手尚未設定歌手資料!" : row.Cells["Song_AddStatus"].Value;
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        string SongLang = ((DataRowView)SongAdd_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        row.Cells["Song_Lang"].Value = SongLang;
                        row.Cells["Song_AddStatus"].Value = (SongLang == "未知") ? "語系類別必須有值才能加歌!" : "";

                        string SongCreatDate = SongAdd_EditSongCreatDate_DateTimePicker.Value.ToString("yyyy/M/d") + " " + DateTime.Now.ToString("tt hh:mm:ss");
                        row.Cells["Song_CreatDate"].Value = SongCreatDate;
                        row.Cells["Song_Singer"].Value = SongAdd_EditSongSinger_TextBox.Text;

                        string SongSingerTypeStr = ((DataRowView)SongAdd_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                        string SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                        row.Cells["Song_SingerType"].Value = SongSingerType;
                        row.Cells["Song_AddStatus"].Value = (row.Cells["Song_AddStatus"].Value.ToString() == "" && SongSingerType == "10") ? "此歌手尚未設定歌手資料!" : row.Cells["Song_AddStatus"].Value;

                        string SongSongName = SongAdd_EditSongSongName_TextBox.Text;
                        row.Cells["Song_SongName"].Value = SongSongName;
                        row.Cells["Song_SongType"].Value = ((DataRowView)SongAdd_EditSongSongType_ComboBox.SelectedItem)[0].ToString();

                        // 計算歌曲字數
                        List<string> SongWordCountList = new List<string>();
                        SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);

                        // 取得歌曲拼音
                        List<string> SongSpellList = new List<string>();
                        SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                        if (SongSpellList[2] == "") SongSpellList[2] = "0";

                        row.Cells["Song_Spell"].Value = SongSpellList[0];
                        row.Cells["Song_SpellNum"].Value = SongSpellList[1];
                        row.Cells["Song_SongStroke"].Value = SongSpellList[2];
                        row.Cells["Song_PenStyle"].Value = SongSpellList[3];
                        row.Cells["Song_WordCount"].Value = SongWordCountList[0];
                        row.Cells["Song_Track"].Value = SongAdd_EditSongTrack_ComboBox.SelectedValue;
                        row.Cells["Song_Volume"].Value = SongAdd_EditSongVolume_TextBox.Text;
                    }
                }
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

            SongAdd_EditSongSingerType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSingerType", MultiEdit);
            SongAdd_EditSongSingerType_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongSingerType_ComboBox.ValueMember = "Value";

            SongAdd_EditSongSongType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongType", MultiEdit);
            SongAdd_EditSongSongType_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongSongType_ComboBox.ValueMember = "Value";

            SongAdd_EditSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack", MultiEdit);
            SongAdd_EditSongTrack_ComboBox.DisplayMember = "Display";
            SongAdd_EditSongTrack_ComboBox.ValueMember = "Value";
        }

        #endregion



    }




    class SongAdd
    {
        public static DataTable GetDefaultSongInfo(string SongInfoType, bool MultiEdit)
        {
            List<string> list = new List<string>();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Display", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(int)));

            if (MultiEdit)
            {
                dt.Rows.Add(dt.NewRow());
                dt.Rows[dt.Rows.Count - 1][0] = "不變更";
                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
            }

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
                    list = Global.CrazyktvSongTrackWordList;
                    break;
                case "DefaultSongType":
                    string str = "";
                    if (Global.SongMgrSongType != "") { str = "無類別," + Global.SongMgrSongType; } else { str = "無類別"; }
                    list = new List<string>(str.Split(','));
                    break;
                case "SpecialStr":
                    if (Global.SongAddSpecialStr != "") { str = Global.SongAddSpecialStr; } else { str = "A-Lin"; }
                    list = new List<string>(str.Split(','));
                    break;
            }

            foreach (string s in list)
            {
                dt.Rows.Add(dt.NewRow());
                dt.Rows[dt.Rows.Count - 1][0] = s;
                dt.Rows[dt.Rows.Count - 1][1] = (SongInfoType == "DefaultSongTrack") ? dt.Rows.Count -1 : dt.Rows.Count;
            }
            return dt;
        }

        public static DataTable GetSongIdentificationModeList()
        {
            List<string> list = new List<string>();
            list = new List<string>() { "智能辨識模式", "歌手_歌名", "歌名_歌手", "歌曲編號_歌手_歌名" };

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Display", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(int)));
            
            foreach (string s in list)
            {
                dt.Rows.Add(dt.NewRow());
                dt.Rows[dt.Rows.Count - 1][0] = s;
                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
            }
            return dt;
        }

        public static DataTable GetDupSongModeList()
        {
            List<string> list = new List<string>();
            list = new List<string>() { "自動忽略重複歌曲", "手動處理重複歌曲", "檔案容量較大時自動取代" };

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Display", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(int)));

            foreach (string s in list)
            {
                dt.Rows.Add(dt.NewRow());
                dt.Rows[dt.Rows.Count - 1][0] = s;
                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
            }
            return dt;
        }

        #region --- 歌曲編輯下拉清單 ---

        public static DataTable GetEditSongLangList(bool MultiEdit)
        {
            DataTable list = new DataTable();
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

        #endregion

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Song_Id":
                    list = new List<string>() { "歌曲編號", "0", "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", "120", "none" };
                    break;
                case "Song_SingerType":
                    list = new List<string>() { "歌手類別", "120", "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", "160", "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", "270", "none" };
                    break;
                case "Song_Track":
                    list = new List<string>() { "歌曲聲道", "140", "none" };
                    break;
                case "Song_SongType":
                    list = new List<string>() { "歌曲類別", "120", "none" };
                    break;
                case "Song_Volume":
                    list = new List<string>() { "歌曲音量", "120", "3" };
                    break;
                case "Song_WordCount":
                    list = new List<string>() { "歌曲字數", "120", "2" };
                    break;
                case "Song_PlayCount":
                    list = new List<string>() { "點播次數", "0", "9" };
                    break;
                case "Song_MB":
                    list = new List<string>() { "歌曲大小", "120", "7" };
                    break;
                case "Song_CreatDate":
                    list = new List<string>() { "加歌日期", "140", "none" };
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
                    list = new List<string>() { "來源檔案路徑", "640", "none" };
                    break;
                case "Song_SortIndex":
                    list = new List<string>() { "排序索引", "0", "none" };
                    break;
                case "Song_AddStatus":
                    list = new List<string>() { "加歌狀況", "240", "none" };
                    break;
            }
            return list;
        }







    }



}
