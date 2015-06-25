using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CrazyKTV_SongMgr
{
    public partial class MainFrom : Form
    {
        private void SongQuery_QueryType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongQuery_QueryType_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = true;
                    SongQuery_Clear_Button.Enabled = true;
                    SongQuery_QueryValue_TextBox.Focus();
                    break;
                case "2":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = true;
                    SongQuery_Clear_Button.Enabled = true;
                    SongQuery_QueryValue_TextBox.Focus();
                    break;
                case "3":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_QueryValue_TextBox.Focus();
                    break;
                case "4":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "100";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
                    SongQuery_DataGridView.Focus();
                    break;
                case "5":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "*";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
                    SongQuery_DataGridView.Focus();
                    break;
                case "6":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    
                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongType");
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "7":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;

                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SingerType");
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "8":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;

                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack");
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
            }
        }

        private void SongQuery_QueryValue_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (SongQuery_QueryType_ComboBox.SelectedValue.ToString())
            {
                case "1":

                    break;
                case "2":

                    break;
                case "3":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13)
                    {
                        e.Handled = true;
                    }
                    break;
                case "4":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13)
                    {
                        e.Handled = true;
                    }
                    break;
            }
            if ((int)e.KeyChar == 13)
            {
                SongQuery_Query_Button_Click(new Button(), new EventArgs());
            }
        }

        private void SongQuery_Query_Button_Click(object sender, EventArgs e)
        {
            Global.SongQueryQueryType = "SongQuery";
            SongQuery_EditMode_CheckBox.Enabled = true;

            SongQuery_DataGridView.DataSource = null;
            if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
            SongQuery_QueryStatus_Label.Text = "";
            string SongQueryStatusText = "";
            string SongQueryValue = "";

            if (File.Exists(Global.CrazyktvDatabaseFile))
            {
                string SongQueryType = "None";
                switch (SongQuery_QueryType_ComboBox.SelectedValue.ToString())
                {
                    case "1":
                        SongQueryType = "SongName";
                        SongQueryStatusText = SongQuery_QueryValue_TextBox.Text;
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        break;
                    case "2":
                        SongQueryType = "SingerName";
                        SongQueryStatusText = SongQuery_QueryValue_TextBox.Text;
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        break;
                    case "3":
                        SongQueryType = "SongID";
                        SongQueryStatusText = "歌曲編號中包含 " + SongQuery_QueryValue_TextBox.Text;
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        break;
                    case "4":
                        SongQueryType = "NewSong";
                        SongQueryStatusText = "新進歌曲";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        break;
                    case "5":
                        SongQueryType = "ChorusSong";
                        SongQueryStatusText = "合唱歌曲";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        break;
                    case "6":
                        SongQueryType = "SongType";
                        SongQueryStatusText = "歌曲類別為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQueryValue = SongQuery_QueryValue_ComboBox.Text;
                        break;
                    case "7":
                        SongQueryType = "SingerType";
                        SongQueryStatusText = "歌手類別為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQueryValue = Global.CrazyktvSingerTypeList.IndexOf(SongQuery_QueryValue_ComboBox.Text).ToString();
                        break;
                    case "8":
                        SongQueryType = "SongTrack";
                        SongQueryStatusText = "歌曲聲道為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQueryValue = SongQuery_QueryValue_ComboBox.SelectedValue.ToString();
                        Console.WriteLine(SongQueryValue);
                        break;
                }

                
                if (SongQueryValue == "")
                {
                    SongQuery_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                }
                else
                {
                    try
                    {
                        DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                        if (dt.Rows.Count == 0)
                        {
                            SongQuery_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                        }
                        else
                        {
                            if (SongQueryType == "SingerName" & Global.SongQueryFuzzyQuery == "False")
                            {
                                var query = from row in dt.AsEnumerable()
                                            where row.Field<string>("Song_Singer") != SongQuery_QueryValue_TextBox.Text
                                            select row;
                                if (query.Count<DataRow>() > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");

                                    foreach (DataRow row in query)
                                    {
                                        if (r.IsMatch(row["Song_Singer"].ToString()))
                                        {
                                            string RemoveThisRow = "True";
                                            string[] singers = Regex.Split(row["Song_Singer"].ToString(), "&", RegexOptions.None);
                                            foreach (string str in singers)
                                            {
                                                if (str == SongQuery_QueryValue_TextBox.Text) { RemoveThisRow = "False"; }
                                            }
                                            if (RemoveThisRow == "True") RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        }
                                    }
                                    
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                }
                            }

                            if (dt.Rows.Count == 0)
                            {
                                SongQuery_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                            }
                            else
                            {
                                SongQuery_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的歌曲。";

                                SongQuery_DataGridView.DataSource = dt;

                                for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
                                {
                                    List<string> DataGridViewColumnName = SongQuery.GetDataGridViewColumnSet(SongQuery_DataGridView.Columns[i].Name);
                                    SongQuery_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                    if (DataGridViewColumnName[1].ToString() == "0")
                                    {
                                        SongQuery_DataGridView.Columns[i].Visible = false;
                                    }

                                    if (DataGridViewColumnName[2].ToString() != "none")
                                    {
                                        ((DataGridViewTextBoxColumn)SongQuery_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                    }

                                    SongQuery_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                    SongQuery_DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                }

                                string SongFullPath = "";
                                int SongFullPathIndex = SongQuery_DataGridView.ColumnCount - 1;
                                SongQuery_DataGridView.Columns.Add("Song_FullPath", "檔案路徑");

                                SongQuery_DataGridView.Columns["Song_FullPath"].Width = 320;
                                SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = 320;
                                SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                {
                                    SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                    SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                }

                                SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                                SongQuery_DataGridView.Focus();
                                dt.Dispose();
                            }
                        }
                    }
                    catch
                    {
                        SongQuery_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                    }
                }
            }
        }

        private void SongQuery_EditMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                int MaxDigitCode;
                if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)));
                tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode)));

                SongQuery_DataGridView.EndEdit();
                SongQuery_QueryStatus_Label.Text = "已進入編輯模式...";
                SongQuery.CreateSongDataTable();
            }
            else
            {
                SongQuery_DataGridView.EndEdit();
                SongQuery_QueryStatus_Label.Text = "已進入檢視模式...";
                SongQuery.DisposeSongDataTable();
            }
            SongQuery_DataGridView.Focus();
        }

        private void SongQuery_SongUpdate(object SongUpdateDT)
        {
            DataTable dt = new DataTable();
            dt = (DataTable)SongUpdateDT;
            List<string> SongUpdateValueList = new List<string>();

            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState from ktv_Song order by Song_Id";
            Global.SongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            bool SongIdUpdate = false;

            this.BeginInvoke((Action)delegate()
            {
                SongQuery_QueryStatus_Label.Text = "";
            });

            foreach (DataRow row in dt.Rows)
            {
                int i = Convert.ToInt32(row["RowIndex"]);
                string OldSongId = row["SongId"].ToString();
                string OldSongLang = row["SongLang"].ToString();

                string SongId = SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                string SongLang = SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value.ToString();

                if (SongId == "")
                {
                    SongIdUpdate = true;
                    SongId = SongQuery.GetNextSongId(SongLang);
                    
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value = SongId;
                    });
                }

                int SongSingerType = Convert.ToInt32(SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Value);
                string SongSinger = SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value.ToString();
                string SongSongName = SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value.ToString();
                int SongTrack = Convert.ToInt32(SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Value);
                string SongSongType = SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Value.ToString();
                string SongVolume = SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Value.ToString();
                string SongWordCount = SongQuery_DataGridView.Rows[i].Cells["Song_WordCount"].Value.ToString();
                string SongPlayCount = SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Value.ToString();
                string SongMB = SongQuery_DataGridView.Rows[i].Cells["Song_MB"].Value.ToString();
                string SongCreatDate = SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Value.ToString();
                string SongFileName = SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                string SongPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString();
                string SongSpell = SongQuery_DataGridView.Rows[i].Cells["Song_Spell"].Value.ToString();
                string SongSpellNum = SongQuery_DataGridView.Rows[i].Cells["Song_SpellNum"].Value.ToString();
                string SongSongStroke = SongQuery_DataGridView.Rows[i].Cells["Song_SongStroke"].Value.ToString();
                string SongPenStyle = SongQuery_DataGridView.Rows[i].Cells["Song_PenStyle"].Value.ToString();
                string SongPlayState = SongQuery_DataGridView.Rows[i].Cells["Song_PlayState"].Value.ToString();

                string SongSingerStr = SongSinger;
                string SingerTypeStr = CommonFunc.GetSingerTypeStr(SongSingerType, 2, "null");
                string CrtchorusSeparate;
                string SongInfoSeparate;
                if (Global.SongMgrChorusSeparate == "1") { CrtchorusSeparate = "&"; } else { CrtchorusSeparate = "+"; }
                if (Global.SongMgrSongInfoSeparate == "1") { SongInfoSeparate = "_"; } else { SongInfoSeparate = "-"; }
                string SongTrackStr = CommonFunc.GetSongTrackStr(SongTrack - 1, 1, "null");

                bool DuplicateSong = false;
                bool MoveError = false;

                var query = from DupSongRow in Global.SongDT.AsEnumerable()
                            where DupSongRow.Field<string>("Song_Id") != OldSongId &&
                                  DupSongRow.Field<string>("Song_Lang").Equals(SongLang) &&
                                  DupSongRow.Field<string>("Song_Singer").ToLower().Equals(SongSinger.ToLower()) &&
                                  DupSongRow.Field<string>("Song_SongName").ToLower().Equals(SongSongName.ToLower())
                            select DupSongRow;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow DupSongRow in query)
                    {
                        if (DupSongRow["Song_SongType"] == null)
                        {
                            if (SongSongType == "")
                            {
                                DuplicateSong = true;
                            }
                        }
                        else
                        {
                            if (DupSongRow["Song_SongType"].ToString() == SongSongType)
                            {
                                DuplicateSong = true;
                            }
                        }
                        
                        if (DuplicateSong)
                        {
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】已有重複歌曲: " + DupSongRow["Song_Id"].ToString() + "|" + DupSongRow["Song_Lang"].ToString() + "|" + DupSongRow["Song_Singer"].ToString() + "|" + DupSongRow["Song_SongName"].ToString() + "|" + SongSongType;
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_QueryStatus_Label.Text = "已有重複歌曲,請參考操作記錄裡的內容!";
                            });
                            break;
                        } 
                    }
                }

                if (!DuplicateSong)
                {
                    // 更改欄位數值並搬移檔案
                    string SongSrcPath = Path.Combine(SongPath, SongFileName);
                    string SongExtension = Path.GetExtension(SongSrcPath);

                    if (SongSingerType == 3)
                    {
                        SongSingerStr = Regex.Replace(SongSinger, "[&+]", CrtchorusSeparate, RegexOptions.IgnoreCase);
                    }

                    switch (Global.SongMgrFolderStructure)
                    {
                        case "1":
                            if (Global.SongMgrChorusMerge == "True" & SongSingerType == 3)
                            {
                                SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\";
                            }
                            else
                            {
                                SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\" + SongSingerStr + @"\";
                            }
                            break;
                        case "2":
                            SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\";
                            break;
                    }

                    switch (Global.SongMgrFileStructure)
                    {
                        case "1":
                            if (SongSongType == "")
                            {
                                SongFileName = SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                        case "2":
                            if (SongSongType == "")
                            {
                                SongFileName = SongSongName + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongSongName + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                        case "3":
                            if (SongSongType == "")
                            {
                                SongFileName = SongId + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongId + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value = SongPath;
                        SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value = SongFileName;
                        SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongPath + SongFileName;
                    });

                    string SongDestPath = Path.Combine(SongPath, SongFileName);

                    if (File.Exists(SongSrcPath))
                    {
                        if (!Directory.Exists(SongPath)) Directory.CreateDirectory(SongPath);

                        if (File.Exists(SongDestPath))
                        {
                            if (SongSrcPath.ToLower() == SongDestPath.ToLower())
                            {
                                try
                                {
                                    FileAttributes attributes = File.GetAttributes(SongSrcPath);
                                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                    {
                                        attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                        File.SetAttributes(SongSrcPath, attributes);
                                    }

                                    File.Move(SongSrcPath, SongPath + "Temp_" + SongFileName);
                                    File.Move(SongPath + "Temp_" + SongFileName, SongDestPath);
                                }
                                catch
                                {
                                    MoveError = true;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (檔案唯讀或正在使用)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                                    });
                                }
                            }
                            else
                            {
                                MoveError = true;
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (歌庫裡已存在該首歌曲的檔案)";
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                this.BeginInvoke((Action)delegate()
                                {
                                    SongQuery_QueryStatus_Label.Text = "異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                                });
                            }
                        }
                        else
                        {
                            try
                            {
                                File.Move(SongSrcPath, SongDestPath);
                            }
                            catch
                            {
                                MoveError = true;
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (檔案唯讀或正在使用)";
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                this.BeginInvoke((Action)delegate()
                                {
                                    SongQuery_QueryStatus_Label.Text = "異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                                });
                            }
                        }
                    }
                    else
                    {
                        MoveError = true;
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (檔案不存在)";
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_QueryStatus_Label.Text = "異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                        });
                    }
                }

                
                if (MoveError | DuplicateSong)
                {
                    if (Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)] == Convert.ToInt32(SongId))
                    {
                        Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]--;
                    }

                    var SongDTQuery = from SongDTRow in Global.SongDT.AsEnumerable()
                                      where SongDTRow.Field<string>("Song_Id").Equals(OldSongId)
                                      select SongDTRow;

                    if (SongDTQuery.Count<DataRow>() > 0)
                    {
                        foreach (DataRow SongDTRow in SongDTQuery)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Style.ForeColor = Color.Red;
                                SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Style.ForeColor = Color.Red;

                                SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value = SongDTRow.Field<string>("Song_Id");
                                SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value = SongDTRow.Field<string>("Song_Lang");
                                SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Value = SongDTRow.Field<Int16>("Song_SingerType");
                                SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value = SongDTRow.Field<string>("Song_Singer");
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value = SongDTRow.Field<string>("Song_SongName");
                                SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Value = SongDTRow.Field<byte>("Song_Track");
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Value = SongDTRow.Field<string>("Song_SongType");
                                SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Value = SongDTRow.Field<byte>("Song_Volume");
                                SongQuery_DataGridView.Rows[i].Cells["Song_WordCount"].Value = SongDTRow.Field<byte>("Song_WordCount");
                                SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Value = SongDTRow.Field<int>("Song_PlayCount");
                                SongQuery_DataGridView.Rows[i].Cells["Song_MB"].Value = SongDTRow.Field<float>("Song_MB");
                                SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Value = SongDTRow.Field<DateTime>("Song_CreatDate");
                                SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value = SongDTRow.Field<string>("Song_FileName");
                                SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value = SongDTRow.Field<string>("Song_Path");
                                SongQuery_DataGridView.Rows[i].Cells["Song_Spell"].Value = SongDTRow.Field<string>("Song_Spell");
                                SongQuery_DataGridView.Rows[i].Cells["Song_SpellNum"].Value = SongDTRow.Field<string>("Song_SpellNum");
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongStroke"].Value = SongDTRow.Field<Int16>("Song_SongStroke");
                                SongQuery_DataGridView.Rows[i].Cells["Song_PenStyle"].Value = SongDTRow.Field<string>("Song_PenStyle");
                                SongQuery_DataGridView.Rows[i].Cells["Song_PlayState"].Value = SongDTRow.Field<byte>("Song_PlayState");
                            });
                        }
                    }
                }
                else
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Style.ForeColor = Color.Black;
                        SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Style.ForeColor = Color.Black;
                    });
                    string SongUpdateValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + OldSongId;
                    SongUpdateValueList.Add(SongUpdateValue);
                }
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId, Song_Lang = @SongLang, Song_SingerType = @SongSingerType, Song_Singer = @SongSinger, Song_SongName = @SongSongName, Song_Track = @SongTrack, Song_SongType = @SongSongType, Song_Volume = @SongVolume, Song_WordCount = @SongWordCount, Song_PlayCount = @SongPlayCount, Song_MB = @SongMB, Song_CreatDate = @SongCreatDate, Song_FileName = @SongFileName, Song_Path = @SongPath, Song_Spell = @SongSpell, Song_SpellNum = @SongSpellNum, Song_SongStroke = @SongSongStroke, Song_PenStyle = @SongPenStyle, Song_PlayState = @SongPlayState";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id=@OldSongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach(string str in SongUpdateValueList)
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
                cmd.Parameters.AddWithValue("@OldSongId", valuelist[19]);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "修改歌曲資料有誤,請回報操作記錄裡的內容!";
                    });
                }
                cmd.Parameters.Clear();
            }
            conn.Close();
            Global.SongDT.Dispose();

            if (SongIdUpdate)
            {
                int MaxDigitCode;
                if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)));
                tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode)));
                tasks.Add(Task.Factory.StartNew(() => Common_GetSongStatisticsTask()));
            }

            this.BeginInvoke((Action)delegate()
            {
                if (Global.SongLogDT.Rows.Count > 0) SongLog_TabPage.Text = "操作記錄 (" + Global.SongLogDT.Rows.Count + ")";
            });
        }

        private List<string> SongQuery_SongRemove(object SongIdlist, object SongFilelist)
        {
            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string SongRemoveSqlStr = "delete from ktv_Song where Song_Id=@SongId";
            cmd = new OleDbCommand(SongRemoveSqlStr, conn);

            List<string> RemoveSongIdlist = new List<string>();
            foreach (string str in (List<string>)SongFilelist)
            {
                int i = ((List<string>)SongFilelist).IndexOf(str);

                try
                {
                    if (File.Exists(str))
                    {
                        FileAttributes attributes = File.GetAttributes(str);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                            File.SetAttributes(str, attributes);
                        }

                        if (Global.SongMgrBackupRemoveSong == "True")
                        {
                            string SongFileName = Path.GetFileName(str);
                            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                            if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                            File.Move(str, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                        }
                        else
                        {
                            File.Delete(str);
                        }
                    }

                    cmd.Parameters.AddWithValue("@SongId", ((List<string>)SongIdlist)[i]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    RemoveSongIdlist.Add(((List<string>)SongIdlist)[i]);
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + str + " (唯讀或使用中)";
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                    });
                }
            }
            conn.Close();

            int MaxDigitCode;
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)));
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode)));
            tasks.Add(Task.Factory.StartNew(() => Common_GetSongStatisticsTask()));
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetRemainingSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6)));
            return RemoveSongIdlist;
        }

        private void SongQuery_FuzzyQuery_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SongQuery_FuzzyQuery_CheckBox.Checked == true)
            {
                Global.SongQueryFuzzyQuery = "True";
            }
            else
            {
                Global.SongQueryFuzzyQuery = "False";
            }
        }

        private void SongQuery_ExceptionalQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongQuery_ExceptionalQuery_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                    Global.SongQueryQueryType = "SongQuery";
                    SongQuery_EditMode_CheckBox.Enabled = true;
                    SongQuery_Query_Button.Enabled = false;

                    Common_SwitchSetUI(false);
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongQuery_ExceptionalQueryTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Common_SwitchSetUI(true);
                            SongQuery_Query_Button.Enabled = true;
                        });
                    });
                    break;
            }
        }

        private void SongQuery_ExceptionalQueryTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            this.BeginInvoke((Action)delegate()
            {
                SongQuery_DataGridView.DataSource = null;
                if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                SongQuery_QueryStatus_Label.Text = "";
                string SongQueryStatusText = "";
                string SongQueryValue = "";
                string SongQueryType = "None";

                if (File.Exists(Global.CrazyktvDatabaseFile))
                {
                    switch (SongQuery_ExceptionalQuery_ComboBox.SelectedValue.ToString())
                    {
                        case "1":
                            SongQueryType = "FileNotExists";
                            SongQueryValue = "NA";
                            SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                            break;
                        case "2":
                            SongQueryType = "SameFileSong";
                            SongQueryValue = "NA";
                            SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                            break;
                        case "3":
                            SongQueryType = "DuplicateSong";
                            SongQueryValue = "NA";
                            SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                            break;
                        case "4":
                            SongQueryType = "DuplicateSongIgnoreSinger";
                            SongQueryValue = "NA";
                            SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                            break;
                        case "5":
                            SongQueryType = "DuplicateSongIgnoreSongType";
                            SongQueryValue = "NA";
                            SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                            break;
                        case "6":
                            SongQueryType = "DuplicateSongOnlyChorusSinger";
                            SongQueryValue = "NA";
                            SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                            break;
                    }

                    if (SongQueryValue == "")
                    {
                        SongQuery_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                    }
                    else
                    {
                        SongQuery_QueryStatus_Label.Text = "查詢中,請稍待...";
                        try
                        {
                            DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                            if (dt.Rows.Count == 0)
                            {
                                SongQuery_QueryStatus_Label.Text = "查無異常歌曲,請重新查詢...";
                            }
                            else
                            {

                                switch (SongQuery_ExceptionalQuery_ComboBox.SelectedValue.ToString())
                                {
                                    case "1":
                                        List<int> RemoveRowsIdxlist = new List<int>();

                                        var query = from row in dt.AsEnumerable()
                                                    where File.Exists(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")))
                                                    select row;

                                        foreach (DataRow row in query)
                                        {
                                            RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        }

                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                        break;
                                }

                                SongQuery_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的異常歌曲。";

                                if (dt.Rows.Count > 0)
                                {
                                    SongQuery_DataGridView.DataSource = dt;

                                    for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
                                    {
                                        List<string> DataGridViewColumnName = SongQuery.GetDataGridViewColumnSet(SongQuery_DataGridView.Columns[i].Name);
                                        SongQuery_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                        if (DataGridViewColumnName[1].ToString() == "0")
                                        {
                                            SongQuery_DataGridView.Columns[i].Visible = false;
                                        }

                                        if (DataGridViewColumnName[2].ToString() != "none")
                                        {
                                            ((DataGridViewTextBoxColumn)SongQuery_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                        }

                                        SongQuery_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                        SongQuery_DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                    }

                                    string SongFullPath = "";
                                    int SongFullPathIndex = SongQuery_DataGridView.ColumnCount - 1;
                                    SongQuery_DataGridView.Columns.Add("Song_FullPath", "檔案路徑");

                                    SongQuery_DataGridView.Columns["Song_FullPath"].Width = 320;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = 320;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                    for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                    {
                                        SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                        SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                    }

                                    SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                                    SongQuery_DataGridView.Focus();
                                    dt.Dispose();
                                }
                            }
                        }
                        catch
                        {
                            SongQuery_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        }
                    }
                }
            });
        }

        private void SongQuery_QueryFilter_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongQuery_QueryFilter_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                Global.SongQueryFilter = SongQuery_QueryFilter_ComboBox.Text;
            }
        }

        private void SongQuery_GetFavoriteUserList()
        {
            SongQuery_FavoriteQuery_ComboBox.DataSource = CommonFunc.GetFavoriteUserList(0);
            SongQuery_FavoriteQuery_ComboBox.DisplayMember = "Display";
            SongQuery_FavoriteQuery_ComboBox.ValueMember = "Value";
            SongQuery_FavoriteQuery_ComboBox.SelectedValue = 1;
        }

        private void SongQuery_FavoriteQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Global.SongQueryFavoriteQuery == "False")
            {
                Global.SongQueryFavoriteQuery = "True";
            }
            else
            {
                if (SongQuery_FavoriteQuery_ComboBox.Text != "System.Data.DataRowView" & SongQuery_FavoriteQuery_ComboBox.Text != "無最愛用戶")
                {
                    Global.SongQueryQueryType = "FavoriteQuery";
                    SongQuery_EditMode_CheckBox.Checked = false;
                    SongQuery_EditMode_CheckBox.Enabled = false;
                    string UserId = "";

                    var query = from row in Global.FavoriteUserDT.AsEnumerable()
                                where row.Field<string>("User_Name").Equals(SongQuery_FavoriteQuery_ComboBox.Text)
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            UserId = row["User_Id"].ToString();
                            break;
                        }
                    }

                    DataTable dt = new DataTable();
                    string SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
                    dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                    List<string> list = new List<string>();

                    var dtquery = from row in dt.AsEnumerable()
                                  where row.Field<string>("User_Id").Equals(UserId)
                                  select row;

                    foreach (DataRow row in dtquery)
                    {
                        list.Add(row["Song_Id"].ToString());
                    }

                    dt.Dispose();
                    SongQuery_FavoriteQuery(list);
                }
            }
        }

        private void SongQuery_FavoriteQuery(List<string> SongIdList)
        {
            SongQuery_DataGridView.DataSource = null;
            if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
            SongQuery_QueryStatus_Label.Text = "";
            string SongQueryStatusText = "";
            string SongQueryValue = "";
            string SongQueryType = "None";

            if (File.Exists(Global.CrazyktvDatabaseFile))
            {
                SongQueryType = "FavoriteSong";
                SongQueryValue = "NA";
                SongQueryStatusText = SongQuery_FavoriteQuery_ComboBox.Text;

                if (SongQueryValue == "")
                {
                    SongQuery_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                }
                else
                {
                    SongQuery_QueryStatus_Label.Text = "查詢中,請稍待...";
                    try
                    {
                        DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                        if (dt.Rows.Count == 0)
                        {
                            SongQuery_QueryStatus_Label.Text = "查無最愛歌曲,請重新查詢...";
                        }
                        else
                        {
                            List<int> RemoveRowsIdxlist = new List<int>();

                            var query = from row in dt.AsEnumerable()
                                        where !SongIdList.Contains(row.Field<string>("Song_Id"))
                                        select row;

                            foreach (DataRow row in query)
                            {
                                RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                            }

                            for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                            {
                                dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                            }


                            SongQuery_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆屬於『" + SongQueryStatusText + "』的最愛歌曲。";

                            if (dt.Rows.Count > 0)
                            {
                                SongQuery_DataGridView.DataSource = dt;

                                for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
                                {
                                    List<string> DataGridViewColumnName = SongQuery.GetDataGridViewColumnSet(SongQuery_DataGridView.Columns[i].Name);
                                    SongQuery_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                    if (DataGridViewColumnName[1].ToString() == "0")
                                    {
                                        SongQuery_DataGridView.Columns[i].Visible = false;
                                    }

                                    if (DataGridViewColumnName[2].ToString() != "none")
                                    {
                                        ((DataGridViewTextBoxColumn)SongQuery_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                    }

                                    SongQuery_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                    SongQuery_DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                }

                                string SongFullPath = "";
                                int SongFullPathIndex = SongQuery_DataGridView.ColumnCount - 1;
                                SongQuery_DataGridView.Columns.Add("Song_FullPath", "檔案路徑");

                                SongQuery_DataGridView.Columns["Song_FullPath"].Width = 320;
                                SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = 320;
                                SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                {
                                    SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                    SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                }

                                SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                                SongQuery_DataGridView.Focus();
                                dt.Dispose();
                            }
                        }
                    }
                    catch
                    {
                        SongQuery_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                    }
                }
            }
        }

        private void SongQuery_FavoriteRemove(object SongIdlist, object UserId)
        {
            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string FavoriteRemoveSqlStr = "delete from ktv_Favorite where User_Id=@UserId and Song_Id=@SongId";
            cmd = new OleDbCommand(FavoriteRemoveSqlStr, conn);

            foreach (string str in (List<string>)SongIdlist)
            {
                cmd.Parameters.AddWithValue("@UserId", (string)UserId);
                cmd.Parameters.AddWithValue("@SongId", str);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            conn.Close();
        }

        private void SongQuery_FavoriteAdd(object SongIdlist, object UserId)
        {
            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            List<string> SongIdAddlist = (List<string>)SongIdlist;

            var query = from row in dt.AsEnumerable()
                        where row.Field<string>("User_Id").Equals((string)UserId)
                        select row;

            foreach (DataRow row in query)
            {
                if (SongIdAddlist.Contains(row["Song_Id"].ToString()))
                {
                    SongIdAddlist.Remove(row["Song_Id"].ToString());
                }
            }

            dt.Dispose();

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "User_Id, Song_Id";
            string sqlValuesStr = "@UserId, @SongId";
            string FavoriteAddSqlStr = "insert into ktv_Favorite ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(FavoriteAddSqlStr, conn);

            foreach (string str in SongIdAddlist)
            {
                cmd.Parameters.AddWithValue("@UserId", (string)UserId);
                cmd.Parameters.AddWithValue("@SongId", str);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            conn.Close();
        }

        private void SongQuery_RefreshSongType()
        {
            if (SongQuery_QueryType_ComboBox.SelectedValue.ToString() == "6")
            {
                SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongType");
                SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                SongQuery_QueryValue_ComboBox.SelectedValue = 1;
            }
        }

        private void SongQuery_Paste_Button_Click(object sender, EventArgs e)
        {
            SongQuery_QueryValue_TextBox.Text = Clipboard.GetText();
        }

        private void SongQuery_Clear_Button_Click(object sender, EventArgs e)
        {
            SongQuery_QueryValue_TextBox.Text = "";
        }


    }
    


    class SongQuery
    {
        public static void CreateSongDataTable()
        {
            string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
            Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongPhoneticsQuerySqlStr, "");
        }

        public static void DisposeSongDataTable()
        {
            Global.PhoneticsDT.Dispose();
        }

        public static string GetSongQuerySqlStr(string QueryType, string QueryValue)
        {
            string sqlCommonStr = " Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState ";
            string SongQuerySqlStr = "";
            string SongQueryOrderStr = " order by Song_Id";
            string SongQueryFilterStr = "";
            string QueryValueNarrow = QueryValue;
            string QueryValueWide = QueryValue;

            Regex HasWideChar = new Regex("[\x21-\x7E\xFF01-\xFF5E]");
            if (QueryType == "SongName" | QueryType == "SingerName")
            {
                if (Global.SongQueryFuzzyQuery == "True")
                {
                    if (HasWideChar.IsMatch(QueryValue))
                    {
                        QueryValueNarrow = CommonFunc.ConvToNarrow(QueryValue);
                        QueryValueWide = CommonFunc.ConvToWide(QueryValue);
                    }
                }

                Regex HasSymbols = new Regex("[']");
                if (HasSymbols.IsMatch(QueryValue))
                {
                    QueryValue = Regex.Replace(QueryValue, "[']", delegate(Match match)
                    {
                        string str = "' + \"" + match.ToString() + "\" + '";
                        return str;
                    });
                }

                if (HasSymbols.IsMatch(QueryValueNarrow))
                {
                    QueryValueNarrow = Regex.Replace(QueryValueNarrow, "[']", delegate(Match match)
                    {
                        string str = "' + \"" + match.ToString() + "\" + '";
                        return str;
                    });
                }
            }

            if (Global.SongQueryFilter != "全部")
            {
                SongQueryFilterStr = " and Song_Lang = '" + Global.SongQueryFilter + "'";
            }

            switch (QueryType)
            {
                case "SongName":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        if (HasWideChar.IsMatch(QueryValue))
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongName = '" + QueryValue + "'" + SongQueryFilterStr;
                    }
                    break;
                case "SingerName":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        if (HasWideChar.IsMatch(QueryValue))
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Singer = '" + QueryValue + "'" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('&" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "&'),0) <>0" + SongQueryFilterStr;
                    }
                    break;
                case "SongID":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Id like '%" + QueryValue + "%'" + SongQueryFilterStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Id = '" + QueryValue + "'";
                    }
                    break;
                case "NewSong":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select top " + QueryValue + sqlCommonStr + "from ktv_Song where Song_Lang = '" + Global.SongQueryFilter + "' order by Song_CreatDate desc, Song_Id desc";
                    }
                    else
                    {
                        SongQuerySqlStr = "select top " + QueryValue + sqlCommonStr + "from ktv_Song order by Song_CreatDate desc, Song_Id desc";
                    }
                    break;
                case "ChorusSong":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = 3" + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = 3" + SongQueryOrderStr;
                    }
                    break;
                case "SongType":
                    if (QueryValue == "無類別") QueryValue = "";
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongType = '" + QueryValue + "'" + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongType = '" + QueryValue + "'" + SongQueryOrderStr;
                    }
                    break;
                case "SingerType":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = " + QueryValue + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = " + QueryValue + SongQueryOrderStr;
                    }
                    break;
                case "SongTrack":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Track = " + QueryValue + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Track = " + QueryValue + SongQueryOrderStr;
                    }
                    break;
                case "FileNotExists":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
                case "SameFileSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_FileName) In (select Song_FileName from ktv_Song As Tmp group by Song_FileName, Song_Path HAVING Count(*)>1 and Song_FileName = ktv_Song.Song_FileName and Song_Path = ktv_Song.Song_Path))) order by Song_FileName";
                    break;
                case "DuplicateSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_Singer, Song_SongType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_Singer = ktv_Song.Song_Singer and Song_SongType = ktv_Song.Song_SongType))) order by Song_SongName";
                    break;
                case "DuplicateSongIgnoreSinger":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_SongType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_SongType = ktv_Song.Song_SongType))) order by Song_SongName";
                    break;
                case "DuplicateSongIgnoreSongType":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_Singer HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_Singer = ktv_Song.Song_Singer))) order by Song_SongName";
                    break;
                case "DuplicateSongOnlyChorusSinger":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_SongType, Song_SingerType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_SongType = ktv_Song.Song_SongType and Song_SingerType = 3))) order by Song_SongName";
                    break;
                case "FavoriteSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
            }

            return SongQuerySqlStr;
        }

        public static DataTable GetSongQueryTypeList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            List<string> ItemList = new List<string>() { "歌曲名稱", "歌手名稱", "歌曲編號", "新進歌曲", "合唱歌曲", "歌曲類別", "歌手類別", "歌曲聲道" };

            foreach (string str in ItemList)
            {
                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = str;
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
            }
            return list;
        }

        public static DataTable GetSongQueryValueList(string ValueType)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            List<string> valuelist = new List<string>();

            switch (ValueType)
            {
                case "SongType":
                    string str = "";
                    if (Global.SongMgrSongType != "") { str = "無類別," + Global.SongMgrSongType; } else { str = "無類別"; }
                    valuelist = new List<string>(str.Split(','));
                    foreach (string value in valuelist)
                    {
                        list.Rows.Add(list.NewRow());
                        list.Rows[list.Rows.Count - 1][0] = value;
                        list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                    }
                    break;
                case "SingerType":
                    foreach (string SingerTypeStr in Global.CrazyktvSingerTypeList)
                    {
                        if (SingerTypeStr != "未使用")
                        {
                            list.Rows.Add(list.NewRow());
                            list.Rows[list.Rows.Count - 1][0] = SingerTypeStr;
                            list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                        }
                    }
                    break;
                case "SongTrack":
                    if (Global.SongMgrSongTrackMode == "True")
                    {
                        valuelist = new List<string>() { "右聲道 / 音軌2", "左聲道 / 音軌1", "音軌3", "音軌4", "音軌5" };
                    }
                    else
                    {
                        valuelist = new List<string>() { "左聲道 / 音軌1", "右聲道 / 音軌2", "音軌3", "音軌4", "音軌5" };
                    }

                    foreach (string value in valuelist)
                    {
                        list.Rows.Add(list.NewRow());
                        list.Rows[list.Rows.Count - 1][0] = value;
                        list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                    }
                    break;
            }
            return list;
        }

        public static DataTable GetSongQueryFilterList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "全部";
            list.Rows[0][1] = 1;
            
            foreach (string str in Global.CrazyktvSongLangList)
            {
                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = str;
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
            }
            return list;
        }

        public static DataTable GetSongQueryExceptionalList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            List<string> ItemList = new List<string>() { "無檔案歌曲", "同檔案歌曲", "重複歌曲", "重複歌曲 (忽略歌手)", "重複歌曲 (忽略類別)", "重複歌曲 (合唱歌手)" };

            foreach (string str in ItemList)
            {
                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = str;
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
            }
            return list;
        }

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Song_Id":
                    list = new List<string>() { "歌曲編號", "100", "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", "100", "none" };
                    break;
                case "Song_SingerType":
                    list = new List<string>() { "歌手類別", "100", "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", "160", "none" };
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
                    list = new List<string>() { "點播次數", "100", "9" };
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
                    list = new List<string>() { "來源檔案路徑", "0", "none" };
                    break;
                case "Song_SortIndex":
                    list = new List<string>() { "排序索引", "0", "none" };
                    break;
                case "Song_AddStatus":
                    list = new List<string>() { "加歌狀況", "0", "none" };
                    break;
            }
            return list;
        }

        public static string GetSongTypeStr(int SongType)
        {
            List<string> list = new List<string>(Global.SongMgrSongType.Split(','));
            string Str = list[SongType];
            return Str;
        }

        public static string GetContextMenuStr(int ContextMenu, int ListType)
        {
            List<string> list;

            switch (ListType)
            {
                case 1:
                    list = new List<string>() { "開啟資料夾", "播放檔案", "刪除資料列" };
                    break;
                case 2:
                    list = new List<string>() { "開啟資料夾", "播放檔案" };
                    break;
                case 3:
                    list = new List<string>() { "開啟資料夾", "播放檔案", "從我的最愛移除" };
                    break;
                default:
                    list = new List<string>() { "刪除資料列" };
                    break;
            }

            string Str = list[ContextMenu];
            return Str;
        }

        public static string GetNextSongId(string SongLang)
        {
            string NewSongID = "";
            // 查詢歌曲編號有無斷號
            if (Global.NotExistsSongIdDT.Rows.Count != 0)
            {
                string RemoveRowindex = "";
                var Query = from row in Global.NotExistsSongIdDT.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals(SongLang)
                            orderby row.Field<string>("Song_Id")
                            select row;

                foreach (DataRow row in Query)
                {
                    NewSongID = row["Song_Id"].ToString();
                    RemoveRowindex = Global.NotExistsSongIdDT.Rows.IndexOf(row).ToString();
                    break;
                }
                if (RemoveRowindex != "")
                {
                    DataRow row = Global.NotExistsSongIdDT.Rows[Convert.ToInt32(RemoveRowindex)];
                    Global.NotExistsSongIdDT.Rows.Remove(row);
                }
            }

            // 若無斷號查詢各語系下個歌曲編號
            if (NewSongID == "")
            {
                string MaxDigitCode = "";
                switch (Global.SongMgrMaxDigitCode)
                {
                    case "1":
                        MaxDigitCode = "D5";
                        break;
                    case "2":
                        MaxDigitCode = "D6";
                        break;
                }

                foreach (string langstr in Global.CrazyktvSongLangList)
                {
                    if (langstr == SongLang)
                    {
                        int LangIndex = Global.CrazyktvSongLangList.IndexOf(langstr);
                        Global.MaxIDList[LangIndex]++;
                        NewSongID = Global.MaxIDList[LangIndex].ToString(MaxDigitCode);
                        break;
                    }
                }
            }
            return NewSongID;
        }

    }
}
