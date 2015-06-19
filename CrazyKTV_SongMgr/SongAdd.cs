using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainFrom : Form
    {
        private void SongAdd_Save_Button_Click(object sender, EventArgs e)
        {
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongLang", Global.SongAddDefaultSongLang);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSingerType", Global.SongAddDefaultSingerType);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongTrack", Global.SongAddDefaultSongTrack);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongType", Global.SongAddDefaultSongType);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongVolume", Global.SongAddDefaultSongVolume);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr", Global.SongAddSpecialStr);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode", Global.SongAddSongIdentificationMode);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode", Global.SongAddDupSongMode);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat", Global.SongAddEngSongNameFormat);
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

        private void SongAdd_RefreshDefaultSongType()
        {
            int i = Convert.ToInt32(SongAdd_DefaultSongType_ComboBox.SelectedValue);
            string SongTypeStr = "";
            DataTable dt = new DataTable();

            if (i != 1)
            {
                dt = (DataTable)SongAdd_DefaultSongType_ComboBox.DataSource;

                var query = from row in dt.AsEnumerable()
                            where row.Field<int>("Value").Equals(i)
                            select row;
                
                if (query.Count<DataRow>() > 0)
                {
                    foreach(DataRow row in query)
                    {
                        SongTypeStr = row["Display"].ToString();
                    }
                }
            }
             
            dt = SongAdd.GetDefaultSongInfo("DefaultSongType");
            if (i != 1)
            {
                var typequery = from row in dt.AsEnumerable()
                                where row.Field<string>("Display").Equals(SongTypeStr)
                                select row;

                if (typequery.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in typequery)
                    {
                        i = Convert.ToInt32(row["Value"]);
                    }
                }
            }

            SongAdd_DefaultSongType_ComboBox.DataSource = dt;
            SongAdd_DefaultSongType_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongType_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongType_ComboBox.SelectedValue = i;
        }

        private void SongAdd_DefaultSongVolume_TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                SongAdd_Tooltip_Label.Text = "此項目的值不能為空白!";
                e.Cancel = true;
            }
            else
            {
                if (int.Parse(((TextBox)sender).Text) > 100)
                {
                    SongAdd_Tooltip_Label.Text = "此項目只能輸入 0 ~ 100 的值!";
                    e.Cancel = true;
                }
                else
                {
                    Global.SongAddDefaultSongVolume = ((TextBox)sender).Text;
                    SongAdd_Tooltip_Label.Text = "";
                }
            }
        }

        private void SongAdd_SpecialStr_ListBox_Enter(object sender, EventArgs e)
        {
            SongAdd_Tooltip_Label.Text = "";
            SongAdd_SpecialStr_Button.Text = "移除";
        }

        private void SongAdd_SpecialStr_TextBox_Enter(object sender, EventArgs e)
        {
            SongAdd_Tooltip_Label.Text = "";
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
                if (item.Contains(Global.SongMgrDestFolder))
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
                        SongAdd_DragDrop_Label.Visible = false;
                        SongAdd_DataGridView.AllowDrop = false;
                        Common_SwitchSetUI(false);
                        Task.Factory.StartNew(SongAdd_SongAnalysisTask, list);
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
            List<string> SongLangIDList = new List<string>();
            SongAnalysis.CreateSongDataTable();
            int total = 0;

            foreach (string str in Global.CrazyktvSongLangIDList)
            {
                strlist = new List<string>(str.Split(','));
                foreach(string liststr in strlist)
                {
                    SongLangIDList.Add(liststr);
                }
            }

            Parallel.ForEach(list, (str, loopState) =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                lock(LockThis)
                {
                    total++;
                }

                SongAnalysis.SongInfoAnalysis(str, SongLangIDList);

                this.BeginInvoke((Action)delegate()
                {
                    SongAdd_Tooltip_Label.Text = "正在分析第 " + total + " 首歌曲...";
                });
            });

            this.BeginInvoke((Action)delegate()
            {
                int sortindex = 0;
                SongAdd_DataGridView.DataSource = Global.SongAddDT;
                
                for (int i = 0; i < SongAdd_DataGridView.ColumnCount; i++)
                {
                    List<string> DataGridViewColumnName = SongAdd.GetDataGridViewColumnSet(SongAdd_DataGridView.Columns[i].Name);
                    SongAdd_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                    switch (SongAdd_DataGridView.Columns[i].HeaderText)
                    {
                        case "來源檔案路徑":
                            SongAdd_DataGridView.Columns[i].MinimumWidth = 320;
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
                    SongAdd_DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;


                    if (SongAdd_DataGridView.Columns[i].HeaderText == "排序索引") sortindex = i;
                }

                SongAdd_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                SongAdd_DataGridView.Sort(SongAdd_DataGridView.Columns[sortindex], ListSortDirection.Ascending);

                SongAdd_DataGridView.AllowDrop = true;
                Common_SwitchSetUI(true);
                this.Activate();
                SongAdd_DataGridView.Focus();

                SongAdd_Add_Button.Enabled = SongAdd_CheckSongAddStatus();
                
                Global.TimerEndTime = DateTime.Now;
                SongAdd_Tooltip_Label.Text = "總共分析 " + total.ToString() + " 首歌曲, 共花費 "  +(long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成分析。";
            });
            SongAnalysis.DisposeSongDataTable();
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
            Global.SongAddDT = new DataTable();
            Global.SongAddDT = SongAdd_DataGridView.DataSource as DataTable;
            Global.SongAddValueList = new List<string>();
            Global.SongAddChorusSingerList = new List<string>();
            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };
            SongAddSong.CreateSongDataTable();

            if (Global.SongMgrMaxDigitCode == "1")
            {
                CommonFunc.GetMaxSongId(5);
                CommonFunc.GetNotExistsSongId(5);
            }
            else
            {
                CommonFunc.GetMaxSongId(6);
                CommonFunc.GetNotExistsSongId(6);
            }

            int count = Global.SongAddDT.Rows.Count;

            for (int i = 0; i < count; i++)
            //Parallel.For(0, count, (i, loopState) =>
                {
                    SongAddSong.StartAddSong(i);

                    this.BeginInvoke((Action)delegate()
                    {
                        SongAdd_Tooltip_Label.Text = "已成功加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首...";
                    });
                }//);


            OleDbConnection SongAddConn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState";
            string sqlValuesStr = "@SongId, @SongLang, @SongSingerType, @SongSinger, @SongSongName, @SongTrack, @SongSongType, @SongVolume, @SongWordCount, @SongPlayCount, @SongMB, @SongCreatDate, @SongFileName, @SongPath, @SongSpell, @SongSpellNum, @SongSongStroke, @SongPenStyle, @SongPlayState";
            string SongAddSqlStr = "insert into ktv_Song ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(SongAddSqlStr, SongAddConn);

            OleDbCommand singercmd = new OleDbCommand();
            OleDbCommand allsingercmd = new OleDbCommand();
            sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
            string SingerAddSqlStr = "insert into ktv_Singer ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            string AllSingerAddSqlStr = "insert into ktv_AllSinger ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            singercmd = new OleDbCommand(SingerAddSqlStr, SongAddConn);
            allsingercmd = new OleDbCommand(AllSingerAddSqlStr, SongAddConn);

            List<string> valuelist = new List<string>();
            List<string> NotExistsSingerId = new List<string>();
            NotExistsSingerId = CommonFunc.GetNotExistsSingerId("ktv_Singer", Global.CrazyktvDatabaseFile);
            List<string> NotExistsAllSingerId = new List<string>();
            NotExistsAllSingerId = CommonFunc.GetNotExistsSingerId("ktv_AllSinger", Global.CrazyktvDatabaseFile);
            int MaxSingerId = CommonFunc.GetMaxSingerId("ktv_Singer", Global.CrazyktvDatabaseFile) + 1;
            int MaxAllSingerId = CommonFunc.GetMaxSingerId("ktv_AllSinger", Global.CrazyktvDatabaseFile) + 1;
            string NextSingerId = "";
            List<string> spelllist = new List<string>();
            List<string> singeraddedlist = new List<string>();
            List<string> allsingeraddedlist = new List<string>();

            foreach(string str in Global.SongAddValueList)
            {
                valuelist = new List<string>(str.Split('*'));

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

                if (valuelist[19] == "1")
                {
                    string addstatus = "";
                    if (singeraddedlist.Count == 0)
                    {
                        singeraddedlist.Add(valuelist[3]);
                    }
                    else
                    {
                        foreach(string singerstr in singeraddedlist)
                        {
                            if(singerstr == valuelist[3])
                            {
                                addstatus = "added";
                                break;
                            }
                        }
                        if (addstatus != "added") singeraddedlist.Add(valuelist[3]);
                    }

                    if (addstatus != "added")
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

                if (valuelist[20] == "1")
                {
                    string addstatus = "";
                    if (allsingeraddedlist.Count == 0)
                    {
                        allsingeraddedlist.Add(valuelist[3]);
                    }
                    else
                    {
                        foreach (string singerstr in allsingeraddedlist)
                        {
                            if (singerstr == valuelist[3])
                            {
                                addstatus = "added";
                                break;
                            }
                        }
                        if (addstatus != "added") allsingeraddedlist.Add(valuelist[3]);
                    }

                    if (addstatus != "added")
                    {
                        if (NotExistsAllSingerId.Count > 0)
                        {
                            NextSingerId = NotExistsAllSingerId[0];
                            NotExistsAllSingerId.RemoveAt(0);
                        }
                        else
                        {
                            NextSingerId = MaxAllSingerId.ToString();
                            MaxAllSingerId++;
                        }

                        spelllist = new List<string>();
                        spelllist = CommonFunc.GetSongNameSpell(valuelist[3]);

                        allsingercmd.Parameters.AddWithValue("@SingerId", NextSingerId);
                        allsingercmd.Parameters.AddWithValue("@SingerName", valuelist[3]);
                        allsingercmd.Parameters.AddWithValue("@SingerType", valuelist[2]);
                        allsingercmd.Parameters.AddWithValue("@SingerSpell", spelllist[0]);
                        allsingercmd.Parameters.AddWithValue("@SingerStrokes", spelllist[2]);
                        allsingercmd.Parameters.AddWithValue("@SingerSpellNum", spelllist[1]);
                        allsingercmd.Parameters.AddWithValue("@SingerPenStyle", spelllist[3]);

                        try
                        {
                            allsingercmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "加入歌手至 ktv_AllSinger 時發生錯誤: " + valuelist[3];
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                        }
                        allsingercmd.Parameters.Clear();
                    }
                }

                this.BeginInvoke((Action)delegate()
                {
                    if (Global.SongAddValueList.IndexOf(str) > 0)
                    {
                        SongAdd_Tooltip_Label.Text = "正在將第 " + Global.SongAddValueList.IndexOf(str) + " 首歌曲寫入資料庫,請稍待...";
                    }
                });
            }
            Global.SongAddValueList.Clear();

            // 加入合唱歌手
            if (Global.SongAddChorusSingerList.Count > 0)
            {
                Global.SingerDT = new DataTable();
                string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
                Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

                Global.AllSingerDT = new DataTable();
                string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
                Global.AllSingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongAllSingerQuerySqlStr, "");

                foreach (string singer in Global.SongAddChorusSingerList)
                {
                    string singertype = "";
                    // 查找資料庫所有歌手表
                    var querysingerall = from row in Global.AllSingerDT.AsEnumerable()
                                         where row.Field<string>("Singer_Name").ToLower().Equals(singer.ToLower())
                                         select row;

                    if (querysingerall.Count<DataRow>() != 0)
                    {
                        foreach (DataRow row in querysingerall)
                        {
                            singertype = row["Singer_Type"].ToString();
                            break;
                        }

                        // 查找資料庫歌手表
                        var querysinger = from row in Global.SingerDT.AsEnumerable()
                                          where row.Field<string>("Singer_Name").ToLower().Equals(singer.ToLower())
                                          select row;

                        if (querysinger.Count<DataRow>() == 0)
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
                        if (Global.SongAddChorusSingerList.IndexOf(singer) > 0)
                        {
                            SongAdd_Tooltip_Label.Text = "正在檢查並加入第 " + Global.SongAddChorusSingerList.IndexOf(singer) + " 位合唱歌手,請稍待...";
                        }
                    });
                }
            }
            Global.SongAddChorusSingerList.Clear();
            SongAddConn.Close();

            if (Global.SongAddDupSongMode == "3") 
            {
                SongAdd_SongUpdateTask();
                Global.DupSongAddDT = new DataTable(); 
            }
                
            
            this.BeginInvoke((Action)delegate()
            {
                if (Global.DupSongAddDT.Rows.Count == 0)
                {
                    Global.TimerEndTime = DateTime.Now;

                    switch (Global.SongAddDupSongMode)
                    {
                        case "1":
                            SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                            break;
                        case "2":
                            SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
                            break;
                        case "3":
                            SongAdd_Tooltip_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成加歌。";
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
                    SongQuery_RefreshSongType();
                    SongAdd_RefreshDefaultSongType();
                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    Task.Factory.StartNew(() => CommonFunc.GetRemainingSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                    SongAdd_Add_Button.Enabled = false;
                    SongAdd_DataGridView.DataSource = null;
                    SongAdd_DataGridView.Enabled = true;
                    SongAdd_DragDrop_Label.Visible = true;
                    Common_SwitchSetUI(true);

                    SongQuery_QueryType_ComboBox.SelectedValue = 1;
                    SongQuery_QueryValue_TextBox.Text = "";
                    MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
                }
                else
                {
                    SongAdd_Add_Button.Text = "更新歌庫";
                    SongAdd_Add_Button.Enabled = true;
                    SongAdd_DataGridView.DataSource = Global.DupSongAddDT;
                    SongAdd_DataGridView.AllowDrop = false;
                    SongAdd_DataGridView.Enabled = true;
                }
            });
            SongAddSong.DisposeSongDataTable();
        }

        private void SongAdd_SongUpdateTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };

            switch (Global.SongAddDupSongMode)
            {
                case "2":
                    Global.DupSongAddDT = new DataTable();
                    Global.DupSongAddDT = SongAdd_DataGridView.DataSource as DataTable;
                    break;
            }

            Global.SongAddValueList = new List<string>();

            int count = Global.DupSongAddDT.Rows.Count;

            for (int i = 0; i < count; i++)
            //Parallel.For(0, count, (i, loopState) =>
            {
                SongAddSong.StartUpdateSong(i);

                this.BeginInvoke((Action)delegate()
                {
                    SongAdd_Tooltip_Label.Text = "已成功搬移 " + Global.TotalList[3] + " 首重複歌曲,移除原有歌曲 " + Global.TotalList[4] + " 首...";
                });
            }//);

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId, Song_Lang = @SongLang, Song_SingerType = @SongSingerType, Song_Singer = @SongSinger, Song_SongName = @SongSongName, Song_Track = @SongTrack, Song_SongType = @SongSongType, Song_Volume = @SongVolume, Song_WordCount = @SongWordCount, Song_PlayCount = @SongPlayCount, Song_MB = @SongMB, Song_CreatDate = @SongCreatDate, Song_FileName = @SongFileName, Song_Path = @SongPath, Song_Spell = @SongSpell, Song_SpellNum = @SongSpellNum, Song_SongStroke = @SongSongStroke, Song_PenStyle = @SongPenStyle, Song_PlayState = @SongPlayState";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id=@OldSongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in Global.SongAddValueList)
            {
                valuelist = new List<string>(str.Split('*'));

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


            this.BeginInvoke((Action)delegate()
            {
                Global.TimerEndTime = DateTime.Now;

                switch (Global.SongAddDupSongMode)
                {
                    case "2":
                        SongAdd_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首重複歌曲,移除原有歌曲 " + Global.TotalList[4] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成更新。";
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
                SongQuery_RefreshSongType();
                SongAdd_RefreshDefaultSongType();
                Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                Task.Factory.StartNew(() => CommonFunc.GetRemainingSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                SongAdd_Add_Button.Text = "加入歌庫";
                SongAdd_Add_Button.Enabled = false;
                SongAdd_DataGridView.DataSource = null;
                SongAdd_DataGridView.AllowDrop = true;
                SongAdd_DataGridView.Enabled = true;
                SongAdd_DragDrop_Label.Visible = true;
                Common_SwitchSetUI(true);

                SongQuery_QueryType_ComboBox.SelectedValue = 1;
                SongQuery_QueryValue_TextBox.Text = "";
                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
            });
        }



    }




    class SongAdd
    {
        public static DataTable GetDefaultSongInfo(string SongInfoType)
        {
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
                    list = new List<string>() { "男歌星", "女歌星", "樂團", "外國男", "外國女", "外國樂團", "其它", "新進歌星" };
                    break;
                case "DefaultSongTrack":
                    list = new List<string>() { "右聲道 / 音軌2", "左聲道 / 音軌1", "音軌3", "音軌4", "音軌5" };
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

        public static DataTable GetSongIdentificationModeList()
        {
            List<string> list = new List<string>();
            list = new List<string>() { "智慧辨識模式", "歌手_歌名", "歌名_歌手", "歌曲編號_歌手_歌名" };

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
                    list = new List<string>() { "語系類別", "100", "none" };
                    break;
                case "Song_SingerType":
                    list = new List<string>() { "歌手類別", "100", "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", "150", "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", "200", "none" };
                    break;
                case "Song_Track":
                    list = new List<string>() { "歌曲聲道", "140", "none" };
                    break;
                case "Song_SongType":
                    list = new List<string>() { "歌曲類別", "100", "none" };
                    break;
                case "Song_Volume":
                    list = new List<string>() { "歌曲音量", "100", "3" };
                    break;
                case "Song_WordCount":
                    list = new List<string>() { "歌曲字數", "100", "2" };
                    break;
                case "Song_PlayCount":
                    list = new List<string>() { "點播次數", "0", "9" };
                    break;
                case "Song_MB":
                    list = new List<string>() { "歌曲大小", "100", "7" };
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
                    list = new List<string>() { "來源檔案路徑", "320", "none" };
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
