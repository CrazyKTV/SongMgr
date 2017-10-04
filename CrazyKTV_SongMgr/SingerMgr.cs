using System;
using System.Collections.Generic;
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
    public partial class MainForm : Form
    {
        #region --- SingerMgr 控制項事件 ---

        private void SingerMgr_QueryValue_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                string QueryValue = SingerMgr_QueryValue_TextBox.Text;
                string SingerType = Global.CrazyktvSingerTypeList.IndexOf(SingerMgr_QueryType_ComboBox.Text).ToString();

                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_QueryTask("SingerName", QueryValue, SingerType)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        SingerMgr_DataGridView.Focus();
                    });
                });
            }
        }

        private void SingerMgr_QueryPaste_Button_Click(object sender, EventArgs e)
        {
            SingerMgr_QueryValue_TextBox.Text = Clipboard.GetText();
        }

        private void SingerMgr_QueryClear_Button_Click(object sender, EventArgs e)
        {
            SingerMgr_QueryValue_TextBox.Text = "";
        }

        private void SingerMgr_DefaultSingerDataTable_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SingerMgr_DefaultSingerDataTable_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    Global.SingerMgrDefaultSingerDataTable = "ktv_Singer";
                    SingerMgr_DataGridView.DataSource = null;
                    SingerMgr_EditMode_CheckBox.Checked = false;
                    SingerMgr_EditMode_CheckBox.Enabled = false;
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    break;
                case "2":
                    Global.SingerMgrDefaultSingerDataTable = "ktv_AllSinger";
                    SingerMgr_DataGridView.DataSource = null;
                    SingerMgr_EditMode_CheckBox.Checked = false;
                    SingerMgr_EditMode_CheckBox.Enabled = false;
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    break;
            }
        }

        private void SingerMgr_Query_Button_Click(object sender, EventArgs e)
        {
            if (SingerMgr_QueryValue_TextBox.Text != "")
            {
                string QueryValue = SingerMgr_QueryValue_TextBox.Text;
                string SingerType = Global.CrazyktvSingerTypeList.IndexOf(SingerMgr_QueryType_ComboBox.Text).ToString();

                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_QueryTask("SingerName", QueryValue, SingerType)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        SingerMgr_DataGridView.Focus();
                    });
                });
            }
            else
            {
                SingerMgr_Tooltip_Label.Text = "必須輸入歌手名稱才能查詢...";
            }
        }

        private void SingerMgr_QueryType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SingerMgr_QueryType_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                string QueryValue = SingerMgr_QueryValue_TextBox.Text;
                string SingerType = Global.CrazyktvSingerTypeList.IndexOf(SingerMgr_QueryType_ComboBox.Text).ToString();

                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_QueryTask("SingerType", QueryValue, SingerType)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        SingerMgr_DataGridView.Focus();
                    });
                });
            }
        }

        private void SingerMgr_SingerLastName_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SingerMgr_SingerLastName_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    Global.SingerMgrLastNameSortMethod = "1";
                    break;
                case "2":
                    Global.SingerMgrLastNameSortMethod = "2";
                    break;
            }
        }

        #endregion

        #region --- SingerMgr 查詢歌手 ---

        private void SingerMgr_QueryTask(string QueryType, string QueryValue, string SingerType)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            string QueryValueNarrow = QueryValue;
            string QueryValueWide = QueryValue;
            string HasWideCharQueryValue = QueryValue;
            string SingerQuerySqlStr = "";

            Global.SongQueryHasWideChar = false;

            this.BeginInvoke((Action)delegate()
            {
                SingerMgr_DataGridView.DataSource = null;
            });
            
            Regex HasWideChar = new Regex("[\x21-\x7E\xFF01-\xFF5E]");
            if (QueryType == "SingerName")
            {
                if (HasWideChar.IsMatch(HasWideCharQueryValue))
                {
                    Global.SongQueryHasWideChar = true;
                    QueryValueNarrow = CommonFunc.ConvToNarrow(QueryValue);
                    QueryValueWide = CommonFunc.ConvToWide(QueryValue);
                    HasWideCharQueryValue = Regex.Replace(HasWideCharQueryValue, "[\x21-\x7E\xFF01-\xFF5E]", "", RegexOptions.IgnoreCase);
                    if (HasWideCharQueryValue == "" || HasWideCharQueryValue == " ") HasWideCharQueryValue = QueryValue;
                }

                Regex HasSymbols = new Regex("[']");
                if (HasSymbols.IsMatch(QueryValue))
                {
                    QueryValue = Regex.Replace(QueryValue, "[']", delegate (Match match)
                    {
                        string str = "'" + match.ToString();
                        return str;
                    });
                }

                if (HasSymbols.IsMatch(HasWideCharQueryValue))
                {
                    HasWideCharQueryValue = Regex.Replace(QueryValue, "[']", delegate (Match match)
                    {
                        string str = "'" + match.ToString();
                        return str;
                    });
                }

                if (HasSymbols.IsMatch(QueryValueNarrow))
                {
                    QueryValueNarrow = Regex.Replace(QueryValueNarrow, "[']", delegate (Match match)
                    {
                        string str = "'" + match.ToString();
                        return str;
                    });
                }
            }

            if (QueryType == "SingerName")
            {
                if (Global.SongQueryHasWideChar)
                {
                    SingerQuerySqlStr = "select " + sqlColumnStr + " from " + Global.SingerMgrDefaultSingerDataTable + " where InStr(1,LCase(Singer_Name),LCase('" + QueryValue + "'),0) <>0 or InStr(1,LCase(Singer_Name),LCase('" + QueryValueNarrow + "'),0) <>0 or InStr(1,LCase(Singer_Name),LCase('" + QueryValueWide + "'),0) <>0 or InStr(1,LCase(Singer_Name),LCase('" + HasWideCharQueryValue + "'),0) <>0 order by Singer_Name";
                }
                else
                {
                    SingerQuerySqlStr = "select " + sqlColumnStr + " from " + Global.SingerMgrDefaultSingerDataTable + " where InStr(1,LCase(Singer_Name),LCase('" + QueryValue + "'),0) <>0 order by Singer_Name";
                }
            }
            else
            {
                SingerQuerySqlStr = "select " + sqlColumnStr + " from " + Global.SingerMgrDefaultSingerDataTable + " where Singer_Type = '" + SingerType + "' order by Singer_Name";
            }

            if (Global.CrazyktvDatabaseStatus)
            {
                try
                {
                    string DatabaseFile = (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile;
                    using (DataTable dt = CommonFunc.GetOleDbDataTable(DatabaseFile, SingerQuerySqlStr, ""))
                    {
                        if (QueryType == "SingerName")
                        {
                            if (Global.GroupSingerLowCaseList.IndexOf(QueryValue.ToLower()) >= 0)
                            {
                                List<string> dtSingerIdList = new List<string>();
                                foreach (DataRow row in dt.AsEnumerable())
                                {
                                    dtSingerIdList.Add(row["Singer_Id"].ToString());
                                }

                                int i = Global.GroupSingerIdList[Global.GroupSingerLowCaseList.IndexOf(QueryValue.ToLower())];
                                List<string> list = new List<string>(Global.SingerGroupList[i].Split(','));
                                if (list.Count > 0)
                                {
                                    foreach (string GroupSingerName in list)
                                    {
                                        if (GroupSingerName.ToLower() != QueryValue.ToLower())
                                        {
                                            SingerQuerySqlStr = "select " + sqlColumnStr + " from " + Global.SingerMgrDefaultSingerDataTable + " where InStr(1,LCase(Singer_Name),LCase('" + GroupSingerName + "'),0) <>0 order by Singer_Name";
                                            using (DataTable SingerGroupDT = CommonFunc.GetOleDbDataTable(DatabaseFile, SingerQuerySqlStr, ""))
                                            {
                                                foreach (DataRow row in SingerGroupDT.Rows)
                                                {
                                                    if (dtSingerIdList.IndexOf(row["Singer_Id"].ToString()) < 0)
                                                    {
                                                        dt.ImportRow(row);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                dtSingerIdList.Clear();
                            }
                        }

                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                if (SingerMgr_EditMode_CheckBox.Checked) SingerMgr_EditMode_CheckBox.Checked = false;
                                SingerMgr_EditMode_CheckBox.Enabled = false;
                                SingerMgr_Tooltip_Label.Text = "查無歌手,請重新查詢...";
                            });
                        }
                        else
                        {
                            if (QueryType == "SingerName")
                            {
                                if (Global.SongQueryHasWideChar)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();
                                    QueryValue = Regex.Replace(QueryValue, "''", "'");

                                    var query = from row in dt.AsEnumerable()
                                                where !CommonFunc.ConvToNarrow(row.Field<string>("Singer_Name")).ToLower().Contains(CommonFunc.ConvToNarrow(QueryValue).ToLower())
                                                select row;

                                    if (query.Count<DataRow>() > 0)
                                    {
                                        foreach (DataRow row in query)
                                        {
                                            RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
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
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        if (SingerMgr_EditMode_CheckBox.Checked) SingerMgr_EditMode_CheckBox.Checked = false;
                                        SingerMgr_EditMode_CheckBox.Enabled = false;
                                        SingerMgr_Tooltip_Label.Text = "查無歌手,請重新查詢...";
                                    });
                                }
                                else
                                {
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SingerMgr_EditMode_CheckBox.Enabled = true;
                                        SingerMgr_Tooltip_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆歌手名稱包含『" + SingerMgr_QueryValue_TextBox.Text + "』的歌手。";
                                    });
                                }
                            }
                            else
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    SingerMgr_EditMode_CheckBox.Enabled = true;
                                    SingerMgr_Tooltip_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆歌手類別為『" + SingerMgr_QueryType_ComboBox.Text + "』的歌手。";
                                });
                            }

                            this.BeginInvoke((Action)delegate()
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    SingerMgr.ClearTooltipLabel = false;
                                    SingerMgr_DataGridView.DataSource = dt;

                                    for (int i = 0; i < SingerMgr_DataGridView.ColumnCount; i++)
                                    {
                                        List<string> DataGridViewColumnName = SingerMgr.GetDataGridViewColumnSet(SingerMgr_DataGridView.Columns[i].Name);
                                        SingerMgr_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                        if (DataGridViewColumnName[1].ToString() == "0")
                                        {
                                            SingerMgr_DataGridView.Columns[i].Visible = false;
                                        }

                                        if (DataGridViewColumnName[2].ToString() != "none")
                                        {
                                            ((DataGridViewTextBoxColumn)SingerMgr_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                        }

                                        SingerMgr_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                    }
                                    SingerMgr_DataGridView.Columns["Singer_Type"].Width = 100;
                                    SingerMgr_DataGridView.Columns["Singer_Type"].MinimumWidth = 100;
                                    SingerMgr_DataGridView.Columns["Singer_Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                }
                            });
                        }
                    }
                }
                catch
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        SingerMgr_Tooltip_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                    });
                }
            }
        }

        #endregion

        #region --- SingerMgr 編輯模式切換 ---

        private void SingerMgr_EditMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingerMgr_EditMode_CheckBox.Checked)
            {
                SingerMgr_Edit_GroupBox.Visible = true;
                SingerMgr_SingerAdd_GroupBox.Visible = false;
                SingerMgr_Manager_GroupBox.Visible = false;

                Global.SingerMgrMultiEdit = false;
                SingerMgr_InitializeEditControl();

                int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;
                SingerMgr_DataGridView_SelectionChanged(new object(), new EventArgs());
                if (SelectedRowsCount > 1) SingerMgr_DataGridView_MouseUp(new object(), null);

                SingerMgr_Tooltip_Label.Text = "已進入編輯模式...";
            }
            else
            {
                SingerMgr_EditMode_CheckBox.Enabled = (SingerMgr_DataGridView.RowCount == 0) ? false : true;
                SingerMgr_Edit_GroupBox.Visible = false;
                SingerMgr_SingerAdd_GroupBox.Visible = true;
                SingerMgr_Manager_GroupBox.Visible = true;

                SingerMgr_Tooltip_Label.Text = "已進入檢視模式...";
            }
            SingerMgr_DataGridView.Focus();
        }

        #endregion

        #region --- SingerMgr 更新歌手 ---

        private void SingerMgr_SingerUpdateTask(List<string> UpdateList, DataTable UpdateDT)
        {
            if (UpdateList.Count <= 0) return;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            OleDbConnection conn = new OleDbConnection();
            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");

            OleDbConnection singerconn = new OleDbConnection();
            singerconn = CommonFunc.OleDbOpenConn((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile, "");

            string sqlColumnStr = "Singer_Id = @SingerId, Singer_Name = @SingerName, Singer_Type = @SingerType, Singer_Spell = @SingerSpell, Singer_Strokes = @SingerStrokes, Singer_SpellNum = @SingerSpellNum, Singer_PenStyle = @SingerPenStyle";
            string AllSingerUpdateSqlStr = "update ktv_AllSinger set " + sqlColumnStr + " where Singer_Id = @OldSingerId";
            string SingerUpdateSqlStr = "update ktv_Singer set " + sqlColumnStr + " where Singer_Id = @OldSingerId";

            string SongSingerUpdateSqlStr = "";
            if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
            {
                SongSingerUpdateSqlStr = "update ktv_Song set Song_Singer = @SongSinger, Song_SingerType = @SongSingerType, Song_FileName = @SongFileName, Song_Path = @SongPath where Song_Id = @SongId";
            }
            else
            {
                SongSingerUpdateSqlStr = "update ktv_Song set Song_Singer = @SongSinger, Song_SingerType = @SongSingerType where Song_Id = @SongId";
            }

            OleDbCommand[] SingerUpdateCmds = 
            {
                new OleDbCommand(SingerUpdateSqlStr, singerconn),
                new OleDbCommand(AllSingerUpdateSqlStr, singerconn),
                new OleDbCommand(SongSingerUpdateSqlStr, conn)
            };

            List<string> valuelist;
            List<string> dtvaluelist;

            foreach (string str in UpdateList)
            {
                valuelist = new List<string>(str.Split('|'));

                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerId", valuelist[0]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerName", valuelist[1]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerType", valuelist[2]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerSpell", valuelist[3]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerStrokes", valuelist[4]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerSpellNum", valuelist[5]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerPenStyle", valuelist[6]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@OldSingerId", valuelist[7]);

                try
                {
                    // SingerName bug
                    bool SingerNameisEmpty = false;
                    if (valuelist[1] == "")
                    {
                        SingerNameisEmpty = true;
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】歌手名稱空白: 階段1 [" + str + "]";
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        lock (LockThis) { Global.TotalList[1]++; }

                        if (Global.SingerMgrSyncSongSinger == "True")
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首...";
                            });
                        }
                        else
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位...";
                            });
                        }
                    }
                    else
                    {
                        SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].ExecuteNonQuery();
                        lock (LockThis) { Global.TotalList[0]++; }

                        if (Global.SingerMgrSyncSongSinger == "True")
                        {
                            if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
                            {
                                string sqlCommonStr = " Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_SongType, Song_Track, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState ";
                                string SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Singer = '" + valuelist[8] + "' or InStr(1,LCase(Song_Singer),LCase('&" + valuelist[8] + "'),0) <>0 or InStr(1,LCase(Song_Singer),LCase('" + valuelist[8] + "&'),0) <>0";
                                List<string> SyncValuelist = new List<string>();

                                using (DataTable SyncDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
                                {
                                    if (SyncDT.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in SyncDT.AsEnumerable())
                                        {
                                            string SongId = row["Song_Id"].ToString();
                                            string SongLang = row["Song_Lang"].ToString();
                                            string OldSongSinger = row["Song_Singer"].ToString();
                                            string OldSongSingerType = row["Song_SingerType"].ToString();
                                            string SongSongName = row["Song_SongName"].ToString();
                                            string SongSongType = row["Song_SongType"].ToString();
                                            int SongTrack = Convert.ToInt32(row["Song_Track"].ToString());
                                            string OldSongFileName = row["Song_FileName"].ToString();
                                            string OldSongPath = row["Song_Path"].ToString();
                                            string SongSrcPath = Path.Combine(OldSongPath, OldSongFileName);

                                            string SingerName = valuelist[1];
                                            string SongSinger = "";
                                            int SongSingerType = Convert.ToInt32(valuelist[2]);

                                            if (OldSongSinger == valuelist[8])
                                            {
                                                if (OldSongSingerType != "3")
                                                {
                                                    SongSinger = SingerName;
                                                }
                                                else
                                                {
                                                    SongSinger = SingerName;
                                                    MatchCollection matches = Regex.Matches(SongSongName, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", RegexOptions.IgnoreCase);
                                                    if (matches.Count > 0)
                                                    {
                                                        foreach (Match match in matches)
                                                        {
                                                            if (match.Value.ContainsAny("合唱", "對唱"))
                                                            {
                                                                SongSingerType = 3;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (OldSongSingerType == "3")
                                                {
                                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                                    if (r.IsMatch(OldSongSinger))
                                                    {
                                                        string[] singers = Regex.Split(OldSongSinger, "&", RegexOptions.None);
                                                        foreach (string singer in singers)
                                                        {
                                                            if (singer == valuelist[8])
                                                            {
                                                                SongSinger = Regex.Replace(OldSongSinger, singer, SingerName);
                                                                SongSingerType = 3;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            // SingerName bug
                                            if (SongSinger == "")
                                            {
                                                SingerNameisEmpty = true;
                                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】歌手名稱空白: 階段2 [" + str + "]";
                                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                            }

                                            if (!SingerNameisEmpty)
                                            {
                                                string SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, OldSongFileName, OldSongPath, false, "", true);
                                                string SongPath = Path.GetDirectoryName(SongDestPath) + @"\";
                                                string SongFileName = Path.GetFileName(SongDestPath);

                                                bool MoveError = false;
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
                                                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】異動檔案時發生錯誤: " + SongSrcPath + " (檔案唯讀或正在使用)";
                                                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MoveError = true;
                                                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】異動檔案時發生錯誤: " + SongSrcPath + " (歌庫裡已存在該首歌曲的檔案)";
                                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
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
                                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】異動檔案時發生錯誤: " + SongSrcPath + " (檔案唯讀或正在使用)";
                                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MoveError = true;
                                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】異動檔案時發生錯誤: " + SongSrcPath + " (檔案不存在)";
                                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                                }

                                                if (!MoveError)
                                                {
                                                    string SyncValue = SongSinger + "|" + SongSingerType + "|" + SongFileName + "|" + SongPath + "|" + SongId;
                                                    SyncValuelist.Add(SyncValue);
                                                }
                                                else
                                                {
                                                    lock (LockThis) { Global.TotalList[3]++; }
                                                }
                                            }
                                            else
                                            {
                                                lock (LockThis) { Global.TotalList[3]++; }
                                            }
                                        }
                                    }
                                }

                                if (SyncValuelist.Count > 0)
                                {
                                    List<string> synclist = new List<string>();

                                    foreach (string syncstr in SyncValuelist)
                                    {
                                        synclist = new List<string>(syncstr.Split('|'));

                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongSinger", synclist[0]);
                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongSingerType", synclist[1]);
                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongFileName", synclist[2]);
                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongPath", synclist[3]);
                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongId", synclist[4]);

                                        try
                                        {
                                            SingerUpdateCmds[2].ExecuteNonQuery();
                                            lock (LockThis) { Global.TotalList[2]++; }

                                            this.BeginInvoke((Action)delegate ()
                                            {
                                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首...";
                                            });
                                        }
                                        catch
                                        {
                                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】同步歌曲的歌手資料時發生錯誤: " + syncstr;
                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                            lock (LockThis) { Global.TotalList[3]++; }

                                            this.BeginInvoke((Action)delegate ()
                                            {
                                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首...";
                                            });
                                        }
                                        SingerUpdateCmds[2].Parameters.Clear();
                                        synclist.Clear();
                                    }
                                }
                                SyncValuelist.Clear();
                            }
                            else // 不搬移模式
                            {
                                string sqlCommonStr = " Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_SongType, Song_Track, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState ";
                                string SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Singer = '" + valuelist[8] + "' or InStr(1,LCase(Song_Singer),LCase('&" + valuelist[8] + "'),0) <>0 or InStr(1,LCase(Song_Singer),LCase('" + valuelist[8] + "&'),0) <>0";
                                List<string> SyncValuelist = new List<string>();

                                using (DataTable SyncDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
                                {
                                    if (SyncDT.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in SyncDT.AsEnumerable())
                                        {
                                            string SongId = row["Song_Id"].ToString();
                                            string OldSongSinger = row["Song_Singer"].ToString();
                                            string OldSongSingerType = row["Song_SingerType"].ToString();
                                            string SongSongName = row["Song_SongName"].ToString();
                                            string SingerName = valuelist[1];
                                            string SongSingerType = valuelist[2];
                                            string SongSinger = "";

                                            if (OldSongSinger == valuelist[8])
                                            {
                                                if (OldSongSingerType != "3")
                                                {
                                                    SongSinger = SingerName;
                                                }
                                                else
                                                {
                                                    SongSinger = SingerName;
                                                    MatchCollection matches = Regex.Matches(SongSongName, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", RegexOptions.IgnoreCase);
                                                    if (matches.Count > 0)
                                                    {
                                                        foreach (Match match in matches)
                                                        {
                                                            if (match.Value.ContainsAny("合唱", "對唱"))
                                                            {
                                                                SongSingerType = "3";
                                                            }
                                                        }
                                                    }
                                                }
                                                // SingerName bug
                                                if (SongSinger == "")
                                                {
                                                    SingerNameisEmpty = true;
                                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】歌手名稱空白: 階段3 [" + str + "]";
                                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                                }
                                                else
                                                {
                                                    string SyncValue = SongSinger + "|" + SongSingerType + "|" + SongId;
                                                    SyncValuelist.Add(SyncValue);
                                                }
                                            }
                                            else
                                            {
                                                if (OldSongSingerType == "3")
                                                {
                                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                                    if (r.IsMatch(OldSongSinger))
                                                    {
                                                        string[] singers = Regex.Split(OldSongSinger, "&", RegexOptions.None);
                                                        foreach (string singer in singers)
                                                        {
                                                            if (singer == valuelist[8])
                                                            {
                                                                SongSinger = Regex.Replace(OldSongSinger, singer, SingerName);
                                                                SongSingerType = "3";
                                                                // SingerName bug
                                                                if (SongSinger == "")
                                                                {
                                                                    SingerNameisEmpty = true;
                                                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】歌手名稱空白: 階段4 [" + str + "]";
                                                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                                                }
                                                                else
                                                                {
                                                                    string SyncValue = SongSinger + "|" + SongSingerType + "|" + SongId;
                                                                    SyncValuelist.Add(SyncValue);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (SyncValuelist.Count > 0)
                                {
                                    List<string> synclist = new List<string>();

                                    foreach (string syncstr in SyncValuelist)
                                    {
                                        synclist = new List<string>(syncstr.Split('|'));

                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongSinger", synclist[0]);
                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongSingerType", synclist[1]);
                                        SingerUpdateCmds[2].Parameters.AddWithValue("@SongId", synclist[2]);

                                        try
                                        {
                                            SingerUpdateCmds[2].ExecuteNonQuery();
                                            lock (LockThis) { Global.TotalList[2]++; }

                                            this.BeginInvoke((Action)delegate ()
                                            {
                                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首...";
                                            });
                                        }
                                        catch
                                        {
                                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】同步歌曲的歌手資料時發生錯誤: " + str;
                                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                            lock (LockThis) { Global.TotalList[3]++; }

                                            this.BeginInvoke((Action)delegate ()
                                            {
                                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首...";
                                            });
                                        }
                                        SingerUpdateCmds[2].Parameters.Clear();
                                        synclist.Clear();
                                    }
                                }
                                SyncValuelist.Clear();
                            }
                        }
                        else
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位。";
                            });
                        }

                        this.BeginInvoke((Action)delegate ()
                        {
                            dtvaluelist = new List<string>(str.Split('|'));
                            Global.SingerMgrDataGridViewRestoreSelectList.Add(dtvaluelist[0]);

                            var query = from row in UpdateDT.AsEnumerable()
                                        where row["Singer_Id"].ToString() == dtvaluelist[7]
                                        select row;

                            foreach (DataRow row in query)
                            {
                                row["Singer_Id"] = dtvaluelist[0];
                                row["Singer_Name"] = dtvaluelist[1];
                                row["Singer_Type"] = dtvaluelist[2];
                                row["Singer_Spell"] = dtvaluelist[3];
                                row["Singer_Strokes"] = dtvaluelist[4];
                                row["Singer_SpellNum"] = dtvaluelist[5];
                                row["Singer_PenStyle"] = dtvaluelist[6];
                            }
                            dtvaluelist.Clear();
                        });
                    }
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】更新" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "預設") + "歌手資料表時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    lock (LockThis) { Global.TotalList[1]++; }

                    if (Global.SingerMgrSyncSongSinger == "True")
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首...";
                        });
                    }
                    else
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位...";
                        });
                    }
                }
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.Clear();
                valuelist.Clear();
            }
            conn.Close();
            singerconn.Close();

            this.BeginInvoke((Action)delegate()
            {
                if (Global.SongLogDT.Rows.Count > 0) SongLog_TabPage.Text = "操作記錄 (" + Global.SongLogDT.Rows.Count + ")";
            });
        }

        #endregion

        #region --- SingerMgr 移除歌手 ---

        private void SingerMgr_SingerRemoveTask(object SingerIdlist)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            OleDbConnection conn = new OleDbConnection();
            conn = CommonFunc.OleDbOpenConn((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile, "");
            
            OleDbCommand cmd = new OleDbCommand();
            string SingerRemoveSqlStr = "delete from " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger") + " where Singer_Id = @SingerId";
            cmd = new OleDbCommand(SingerRemoveSqlStr, conn);

            foreach (string str in (List<string>)SingerIdlist)
            {
                cmd.Parameters.AddWithValue("@SingerId", str);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                lock (LockThis) { Global.TotalList[0]++; }
            }
            conn.Close();
        }

        #endregion

        #region --- SingerMgr 新增歌手 ---
        private void SingerMgr_SingerAddPaste_Button_Click(object sender, EventArgs e)
        {
            SingerMgr_SingerAddName_TextBox.Text = Clipboard.GetText();
        }

        private void SingerMgr_SingerAddClear_Button_Click(object sender, EventArgs e)
        {
            SingerMgr_SingerAddName_TextBox.Text = "";
        }


        private void SingerMgr_SingerAdd_Button_Click(object sender, EventArgs e)
        {
            if (SingerMgr_SingerAddName_TextBox.Text != "")
            {
                SingerMgr.CreateSongDataTable();
                Common_SwitchSetUI(false);
                string SingerAddName = SingerMgr_SingerAddName_TextBox.Text;
                string SingerAddType = SingerMgr_SingerAddType_ComboBox.Text;

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerAddTask(SingerAddName, SingerAddType)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    });
                    SingerMgr.DisposeSongDataTable();
                });
            }
            else
            {
                SingerMgr_Tooltip_Label.Text = "必須輸入歌手名稱才能新增...";
            }
        }

        private void SingerMgr_SingerAddTask(object SingerAddName, object SingerAddType)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            string SingerName = (string)SingerAddName;
            string SingerType = Global.CrazyktvSingerTypeList.IndexOf((string)SingerAddType).ToString();

            int SingerExists = (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? SingerMgr.SingerLowCaseList.IndexOf(SingerName.ToLower()) : SingerMgr.AllSingerLowCaseList.IndexOf(SingerName.ToLower());

            if (SingerExists >= 0)
            {
                this.BeginInvoke((Action)delegate()
                {
                    SingerMgr_Tooltip_Label.Text = "歌手【" + SingerName + "】已在" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "預設") + "歌手資料庫裡!";
                });
            }
            else
            {
                int MaxAllSingerId = CommonFunc.GetMaxSingerId((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger", (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile) + 1;
                List<string> NotExistsAllSingerId = new List<string>();
                NotExistsAllSingerId = CommonFunc.GetNotExistsSingerId((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger", (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile);

                string SingerId = "";
                SingerId = (NotExistsAllSingerId.Count > 0) ? NotExistsAllSingerId[0] : MaxAllSingerId.ToString();

                List<string> spelllist = new List<string>();
                spelllist = CommonFunc.GetSongNameSpell(SingerName);

                OleDbConnection conn = CommonFunc.OleDbOpenConn((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile, "");
                OleDbCommand singercmd = new OleDbCommand();

                string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
                string sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
                string SingerAddSqlStr = "insert into " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger") + " ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
                singercmd = new OleDbCommand(SingerAddSqlStr, conn);

                singercmd.Parameters.AddWithValue("@SingerId", SingerId);
                singercmd.Parameters.AddWithValue("@SingerName", SingerName);
                singercmd.Parameters.AddWithValue("@SingerType", SingerType);
                singercmd.Parameters.AddWithValue("@SingerSpell", spelllist[0]);
                singercmd.Parameters.AddWithValue("@SingerStrokes", spelllist[2]);
                singercmd.Parameters.AddWithValue("@SingerSpellNum", spelllist[1]);
                singercmd.Parameters.AddWithValue("@SingerPenStyle", spelllist[3]);

                try
                {
                    singercmd.ExecuteNonQuery();
                    this.BeginInvoke((Action)delegate()
                    {
                        SingerMgr_Tooltip_Label.Text = "已成功將歌手【" + SingerName + "】加入至" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "預設") + "歌手資料表。";
                    });
                }
                catch
                {
                    Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "加入歌手至資料庫時發生錯誤: " + SingerName;
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                }
                singercmd.Parameters.Clear();
                conn.Close();
            }
        }

        #endregion

        #region --- SingerMgr 匯入/匯出歌手資料 ---

        private void SingerMgr_SingerExport_Button_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            string SingerQuerySqlStr = "";
            DataTable dt = new DataTable();

            string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            SingerQuerySqlStr = "select " + sqlColumnStr + " from " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger") + " order by Singer_Type, Singer_Name";
            dt = CommonFunc.GetOleDbDataTable((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile, SingerQuerySqlStr, "");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    list.Add(((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger") + "," + row["Singer_Id"].ToString() + "," + row["Singer_Name"].ToString() + "," + row["Singer_Type"].ToString() + "," + row["Singer_Spell"].ToString() + "," + row["Singer_Strokes"].ToString() + "," + row["Singer_SpellNum"].ToString() + "," + row["Singer_PenStyle"].ToString());
                }
            }

            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
            StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "Singer.txt" : "SingerAll.txt"));
            foreach (string str in list)
            {
                sw.WriteLine(str);
            }

            SingerMgr_Tooltip_Label.Text = @"已將歌手資料匯出至【SongMgr\Backup\" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "Singer.txt" : "SingerAll.txt") + "】檔案。";
            sw.Close();
            list.Clear();
            dt.Dispose();
            dt = null;
        }

        private void SingerMgr_SingerImport_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "Singer.txt" : "SingerAll.txt")))
            {
                if (SingerMgr_Tooltip_Label.Text == "歌手資料備份檔案不存在!") SingerMgr_Tooltip_Label.Text = "";
                if (MessageBox.Show("你確定要重置並匯入歌手資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    SingerMgr_Tooltip_Label.Text = "正在匯入歌手資料,請稍待...";

                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerImportTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                        this.BeginInvoke((Action)delegate()
                        {
                            SingerMgr_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 位歌手資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            else
            {
                SingerMgr_Tooltip_Label.Text = "歌手資料備份檔案不存在!";
            }
        }

        private void SingerMgr_SingerImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand singercmd = new OleDbCommand();

            conn = CommonFunc.OleDbOpenConn((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.CrazyktvDatabaseFile : Global.CrazyktvSongMgrDatabaseFile, "");
            string TruncateSqlStr = "delete * from " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger");
            singercmd = new OleDbCommand(TruncateSqlStr, conn);
            singercmd.ExecuteNonQuery();

            StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "Singer.txt" : "SingerAll.txt"), Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            string sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
            string SingerAddSqlStr = "insert into " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger") + " ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            singercmd = new OleDbCommand(SingerAddSqlStr, conn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, ",", RegexOptions.None));

                list[1] = (Addlist.IndexOf(AddStr) + 1).ToString();

                switch (list[0])
                {
                    case "ktv_Singer":
                    case "ktv_AllSinger":
                        singercmd.Parameters.AddWithValue("@SingerId", list[1]);
                        singercmd.Parameters.AddWithValue("@SingerName", list[2]);
                        singercmd.Parameters.AddWithValue("@SingerType", list[3]);
                        singercmd.Parameters.AddWithValue("@SingerSpell", list[4]);
                        singercmd.Parameters.AddWithValue("@SingerStrokes", list[5]);
                        singercmd.Parameters.AddWithValue("@SingerSpellNum", list[6]);
                        singercmd.Parameters.AddWithValue("@SingerPenStyle", list[7]);

                        singercmd.ExecuteNonQuery();
                        singercmd.Parameters.Clear();
                        lock (LockThis)
                        {
                            Global.TotalList[0]++;
                        }
                        break;
                }
                this.BeginInvoke((Action)delegate()
                {
                    SingerMgr_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                });
            }
            conn.Close();
        }

        #endregion

        #region --- SingerMgr 重建歌庫歌手 ---

        private void SingerMgr_RebuildSingerData_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要重建歌庫歌手資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SingerMgr.CreateSongDataTable();
                Common_SwitchSetUI(false);

                SingerMgr_Tooltip_Label.Text = "正在重建歌庫歌手資料,請稍待...";

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => Common_RebuildSingerDataTask("SingerMgr")));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SingerMgr_Tooltip_Label.Text = "總共重建 " + Global.TotalList[0] + " 位歌手資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                        Common_SwitchSetUI(true);
                    });
                    SingerMgr.DisposeSongDataTable();
                });
            }
        }

        #endregion

        #region --- SingerMgr 重建歌星姓氏 ---

        private void SingerMgr_SingerLastName_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            SingerMgr.CreateSongDataTable();
            Common_SwitchSetUI(false);

            SingerMgr_Tooltip_Label.Text = "正在重建歌星姓氏,請稍待...";

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => SingerMgr_RebuildSingerLastNameTask()));

            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
            {
                Global.TimerEndTime = DateTime.Now;
                this.BeginInvoke((Action)delegate()
                {
                    SingerMgr_Tooltip_Label.Text = "總共從歌庫歌曲解析出 " + Global.TotalList[0] + " 筆歌星姓氏,寫入 " + Global.TotalList[1] + " 列歌星姓氏資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    Common_SwitchSetUI(true);
                });
                SingerMgr.DisposeSongDataTable();
            });
        }

        private void SingerMgr_RebuildSingerLastNameTask()
        {
            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, First(Song_SingerType) AS Song_SingerType, Count(Song_Singer) AS Song_SingerCount FROM ktv_Song GROUP BY Song_Singer HAVING First(Song_SingerType)<=10 AND First(Song_SingerType)<>8 AND First(Song_SingerType)<>9 AND Count(Song_Singer)>0 ORDER BY First(Song_SingerType), First(Song_Singer)";
            using (DataTable SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SingerQuerySqlStr, ""))
            {
                if (SingerDT.Rows.Count > 0)
                {
                    List<string> SortItemList = new List<string>();

                    if (Global.SingerMgrLastNameSortMethod == "2")
                    {
                        string Bopomofo = "";
                        for (byte b = (byte)0x05; b <= (byte)0x29; b++)
                        {
                            Bopomofo = Encoding.Unicode.GetString(new byte[] { b, 0x31 });
                            SortItemList.Add(Bopomofo);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 40; i++)
                        {
                            SortItemList.Add(i.ToString());
                        }
                    }

                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add(new DataColumn("SingerSortItem", typeof(string)));
                        dt.Columns.Add(new DataColumn("SingerLastName", typeof(string)));

                        foreach (string SortItem in SortItemList)
                        {
                            dt.Rows.Add(dt.NewRow());
                            dt.Rows[dt.Rows.Count - 1][0] = SortItem;
                        }

                        string SingerLastName = "";
                        int SingerLastNameIndex;
                        List<string> SingerLastNameList = new List<string>();
                        List<string> SingerLastNameSpellList = new List<string>();

                        Regex r = new Regex(@"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d])");

                        foreach (DataRow row in SingerDT.AsEnumerable())
                        {
                            SingerLastName = row["Song_Singer"].ToString().Substring(0, 1);
                            if (r.IsMatch(SingerLastName))
                            {
                                if (SingerLastNameList.IndexOf(SingerLastName) < 0)
                                {
                                    SingerLastNameList.Add(SingerLastName);
                                    SingerLastNameSpellList = CommonFunc.GetSongNameSpell(SingerLastName);
                                    SingerLastNameIndex = SortItemList.IndexOf((Global.SingerMgrLastNameSortMethod == "2") ? SingerLastNameSpellList[0] : SingerLastNameSpellList[2]);
                                    if (SingerLastNameIndex >= 0) dt.Rows[SingerLastNameIndex][1] += SingerLastName;

                                    Global.TotalList[0]++;
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SingerMgr_Tooltip_Label.Text = "已從歌庫歌曲解析出 " + Global.TotalList[0] + " 筆歌星姓氏,請稍待...";
                                    });
                                }
                            }
                            SingerLastNameSpellList.Clear();
                        }
                        SingerLastNameList.Clear();

                        List<string> RebuildList = new List<string>();
                        RebuildList.Add("英文|ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ");
                        RebuildList.Add("數字|１２３４５６７８９０");

                        var query = from row in dt.AsEnumerable()
                                    where row["SingerLastName"].ToString() != ""
                                    select row;

                        if (query.Count<DataRow>() > 0)
                        {
                            foreach (DataRow row in query)
                            {
                                SingerLastName = row["SingerLastName"].ToString();
                                if (SingerLastName.Length > 26)
                                {
                                    MatchCollection CJKCharMatches = Regex.Matches(SingerLastName, @"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d]|[\uac00-\ud7ff]){26}");
                                    if (CJKCharMatches.Count > 0)
                                    {
                                        foreach (Match m in CJKCharMatches)
                                        {
                                            RebuildList.Add(row["SingerSortItem"].ToString() + ((Global.SingerMgrLastNameSortMethod == "1") ? "劃" : "") + "|" + m.Value);
                                            SingerLastName = Regex.Replace(SingerLastName, m.Value, "");
                                        }
                                        if (SingerLastName != "") RebuildList.Add(row["SingerSortItem"].ToString() + ((Global.SingerMgrLastNameSortMethod == "1") ? "劃" : "") + "|" + SingerLastName);
                                    }
                                    SingerLastNameList.Clear();
                                }
                                else
                                {
                                    RebuildList.Add(row["SingerSortItem"].ToString() + ((Global.SingerMgrLastNameSortMethod == "1") ? "劃" : "") + "|" + SingerLastName);
                                }
                            }
                        }

                        using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, ""))
                        {
                            string TruncateSqlStr = "delete * from ktv_SingerName";
                            using (OleDbCommand cmd = new OleDbCommand(TruncateSqlStr, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            string sqlColumnStr = "SingerName_Id, SingerName_Num, SingerName_Name";
                            string sqlValuesStr = "@SingerNameId, @SingerNameNum, @SingerNameName";
                            string SingerNameAddSqlStr = "insert into ktv_SingerName ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";

                            using (OleDbCommand cmd = new OleDbCommand(SingerNameAddSqlStr, conn))
                            {
                                List<string> list;
                                foreach (string RebuildStr in RebuildList)
                                {
                                    list = new List<string>(RebuildStr.Split('|'));

                                    cmd.Parameters.AddWithValue("@SingerNameId", RebuildList.IndexOf(RebuildStr) + 1);
                                    cmd.Parameters.AddWithValue("@SingerNameNum", list[0]);
                                    cmd.Parameters.AddWithValue("@SingerNameName", list[1]);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    list.Clear();

                                    Global.TotalList[1]++;
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SingerMgr_Tooltip_Label.Text = "已寫入 " + Global.TotalList[1] + " 列歌星姓氏資料,請稍待...";
                                    });
                                }
                            }
                        }
                        RebuildList.Clear();
                    }
                    SortItemList.Clear();
                }
            }
        }

        #endregion

        #region --- 歌手編輯 ---

        private void SingerMgr_EditSingerType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SingerMgr_EditMode_CheckBox.Checked == true)
            {
                if (SingerMgr_EditSingerType_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SingerMgrDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SingerMgrMultiEditUpdateList[0] = (SingerMgr_EditSingerType_ComboBox.Text != "不變更") ? true : false;
                    }
                }
            }
        }

        private void SingerMgr_EditSingerName_TextBox_Validated(object sender, EventArgs e)
        {
            if (SingerMgr_EditMode_CheckBox.Checked == true)
            {
                if (Global.SingerMgrDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount == 1)
                {
                    string SingerName = SingerMgr_EditSingerName_TextBox.Text;
                    // 取得歌手拼音
                    List<string> SingerSpellList = new List<string>();
                    SingerSpellList = CommonFunc.GetSongNameSpell(SingerName);
                    SingerMgr_EditSingerSpell_TextBox.Text = SingerSpellList[0];
                }
            }
        }

        private void SingerMgr_EditSyncSongSinger_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SingerMgrSyncSongSinger = SingerMgr_EditSyncSongSinger_CheckBox.Checked.ToString();
            if (Global.SongMgrInitializeStatus) CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SingerMgrSyncSongSinger", Global.SingerMgrSyncSongSinger);
        }

        private void SingerMgr_EditApplyChanges_Button_Click(object sender, EventArgs e)
        {
            if (SingerMgr_EditMode_CheckBox.Checked == true)
            {
                if (Global.SingerMgrDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;
                List<string> UpdateList = new List<string>();

                SingerMgr.CreateSongDataTable();
                SingerMgr_Tooltip_Label.Text = "正在更新歌手資料,請稍待...";

                string OldSingerId;
                string OldSingerName;
                string SingerId;
                string SingerName;
                string SingerType;
                string SingerSpell;
                string SingerSpellNum;
                string SingerStrokes;
                string SingerPenStyle;
                string CurrentRowSingerId = "";

                if (SelectedRowsCount > 1)
                {
                    Common_SwitchSetUI(false);
                    foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                    {
                        SingerId = row.Cells["Singer_Id"].Value.ToString();
                        OldSingerId = SingerId;
                        SingerName = row.Cells["Singer_Name"].Value.ToString();
                        OldSingerName = SingerName;
                        SingerType = row.Cells["Singer_Type"].Value.ToString();
                        SingerSpell = row.Cells["Singer_Spell"].Value.ToString();
                        SingerSpellNum = row.Cells["Singer_SpellNum"].Value.ToString();
                        SingerStrokes = row.Cells["Singer_Strokes"].Value.ToString();
                        SingerPenStyle = row.Cells["Singer_PenStyle"].Value.ToString();

                        if (Global.SingerMgrMultiEditUpdateList[0])
                        {
                            string SingerTypeStr = ((DataRowView)SingerMgr_EditSingerType_ComboBox.SelectedItem)[0].ToString();
                            SingerType = CommonFunc.GetSingerTypeStr(0, 1, SingerTypeStr);
                        }

                        if (SingerMgr_DataGridView.Rows.IndexOf(row) == SingerMgr_DataGridView.CurrentRow.Index)
                        {
                            CurrentRowSingerId = SingerId;
                        }

                        UpdateList.Add(SingerId + "|" + SingerName + "|" + SingerType + "|" + SingerSpell + "|" + SingerStrokes + "|" + SingerSpellNum + "|" + SingerPenStyle + "|" + OldSingerId + "|" + OldSingerName);
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                    {
                        SingerId = row.Cells["Singer_Id"].Value.ToString();
                        OldSingerId = SingerId;
                        OldSingerName = row.Cells["Singer_Name"].Value.ToString();
                        SingerName = SingerMgr_EditSingerName_TextBox.Text;

                        int SingerExists = (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? SingerMgr.SingerLowCaseList.IndexOf(SingerName.ToLower()) : SingerMgr.AllSingerLowCaseList.IndexOf(SingerName.ToLower());
                        if (SingerExists >= 0)
                        {
                            if (SingerId != ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? SingerMgr.SingerIdList[SingerExists] : SingerMgr.AllSingerIdList[SingerExists]))
                            {
                                SingerMgr_Tooltip_Label.Text = "歌手【" + SingerName + "】已在" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "預設") + "歌手資料庫裡!";
                                return;
                            }
                        }

                        CurrentRowSingerId = SingerId;
                        Common_SwitchSetUI(false);

                        string SingerTypeStr = ((DataRowView)SingerMgr_EditSingerType_ComboBox.SelectedItem)[0].ToString();
                        SingerType = CommonFunc.GetSingerTypeStr(0, 1, SingerTypeStr);

                        // 取得歌手拼音
                        List<string> SingerSpellList = new List<string>();
                        SingerSpellList = CommonFunc.GetSongNameSpell(SingerName);

                        SingerSpell = SingerSpellList[0];
                        SingerSpellNum = SingerSpellList[1];
                        SingerStrokes = SingerSpellList[2];
                        SingerPenStyle = SingerSpellList[3];

                        UpdateList.Add(SingerId + "|" + SingerName + "|" + SingerType + "|" + SingerSpell + "|" + SingerStrokes + "|" + SingerSpellNum + "|" + SingerPenStyle + "|" + OldSingerId + "|" + OldSingerName);
                    }
                }

                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Global.SingerMgrDataGridViewRestoreSelectList = new List<string>();
                Global.SingerMgrDataGridViewRestoreCurrentRow = CurrentRowSingerId;
                SingerMgr_DataGridView.Sorted -= new EventHandler(SingerMgr_DataGridView_Sorted);

                using (DataTable UpdateDT = (DataTable)SingerMgr_DataGridView.DataSource)
                {
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerUpdateTask(UpdateList, UpdateDT)));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            SingerMgr_DataGridView.Sorted += new EventHandler(SingerMgr_DataGridView_Sorted);
                            SingerMgr_DataGridView_Sorted(new object(), new EventArgs());

                            Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());

                            SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;

                            if (SelectedRowsCount > 1)
                            {
                                Global.SingerMgrDataGridViewSelectList = new List<string>();

                                foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                                {
                                    SingerId = row.Cells["Singer_Id"].Value.ToString();
                                    SingerType = row.Cells["Singer_Type"].Value.ToString();
                                    SingerName = row.Cells["Singer_Name"].Value.ToString();
                                    SingerSpell = row.Cells["Singer_Spell"].Value.ToString();
                                    SingerStrokes = row.Cells["Singer_Strokes"].Value.ToString();
                                    SingerSpellNum = row.Cells["Singer_SpellNum"].Value.ToString();
                                    SingerPenStyle = row.Cells["Singer_PenStyle"].Value.ToString();

                                    string SelectValue = SingerId + "|" + SingerType + "|" + SingerName + "|" + SingerSpell + "|" + SingerStrokes + "|" + SingerSpellNum + "|" + SingerPenStyle;
                                    Global.SingerMgrDataGridViewSelectList.Add(SelectValue);
                                }
                            }

                            if (Global.SingerMgrSyncSongSinger == "True")
                            {
                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位,同步歌曲 " + Global.TotalList[2] + " 首,失敗 " + Global.TotalList[3] + " 首。";
                            }
                            else
                            {
                                SingerMgr_Tooltip_Label.Text = "已成功更新 " + Global.TotalList[0] + " 位歌手資料,失敗 " + Global.TotalList[1] + " 位。";
                            }
                            Common_SwitchSetUI(true);
                        });
                        UpdateList.Clear();
                        SingerMgr.DisposeSongDataTable();
                    });
                }
            }
        }

        private void SingerMgr_InitializeEditControl()
        {
            SingerMgr_EditSingerId_TextBox.Text = "";
            SingerMgr_EditSingerType_ComboBox.SelectedValue = 1;
            SingerMgr_EditSingerName_TextBox.Text = "";
            SingerMgr_EditSingerSpell_TextBox.Text = "";

            SingerMgr_EditSingerId_TextBox.Enabled = false;
            SingerMgr_EditSingerType_ComboBox.Enabled = false;
            SingerMgr_EditSingerName_TextBox.Enabled = false;
            SingerMgr_EditSingerSpell_TextBox.Enabled = false;
            SingerMgr_EditApplyChanges_Button.Enabled = false;
        }

        private void SingerMgr_GetSingerEditComboBoxList(bool MultiEdit)
        {
            Global.SingerMgrMultiEdit = MultiEdit;
            SingerMgr_EditSingerType_ComboBox.DataSource = SingerMgr.GetSingerTypeList(MultiEdit);
            SingerMgr_EditSingerType_ComboBox.DisplayMember = "Display";
            SingerMgr_EditSingerType_ComboBox.ValueMember = "Value";
        }


        #endregion

        #region --- 歌手圖片 ---

        private void SingerMgr_EditSingerImg_Panel_DragEnter(object sender, DragEventArgs e)
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

        private void SingerMgr_EditSingerImg_Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (SingerMgr_Tooltip_Label.Text != "") SingerMgr_Tooltip_Label.Text = "";
            string[] drop = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> SupportFormat = new List<string>() { ".bmp", ".gif", ".jpg", ".png" };

            if (drop.Count<string>() > 1 || Directory.Exists(drop[0]))
            {
                SingerMgr_Tooltip_Label.Text = "一次僅能拖曳一個圖檔!";
            }
            else
            {
                if (SingerMgr_Tooltip_Label.Text == "一次僅能拖曳一個圖檔!") SingerMgr_Tooltip_Label.Text = "";
                foreach (string item in drop)
                {
                    if (File.Exists(item))
                    {
                        FileInfo f = new FileInfo(item);
                        if (f.Extension.ToLower().ContainsAny(SupportFormat.ToArray()))
                        {
                            string SingerName = SingerMgr_EditSingerName_TextBox.Text;
                            if (!Directory.Exists(Application.StartupPath + @"\Web\singerimg")) Directory.CreateDirectory(Application.StartupPath + @"\Web\singerimg");
                            string FilePath = Application.StartupPath + @"\Web\singerimg\" + SingerName + f.Extension.ToLower();
                            if (FilePath != item) SingerMgr_EditSingerImg_Panel.BackgroundImage.Dispose();

                            if (File.Exists(FilePath))
                            {
                                if (FilePath == item)
                                {
                                    SingerMgr_Tooltip_Label.Text = "拖曳的檔案與現用圖檔相同!";
                                }
                                else
                                {
                                    try
                                    {
                                        FileAttributes attributes = File.GetAttributes(FilePath);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(FilePath, attributes);
                                        }
                                        File.Delete(FilePath);

                                        if (item.ContainsAll(Path.GetTempPath(), ".bmp"))
                                        {
                                            FilePath = Application.StartupPath + @"\Web\singerimg\" + SingerName + ".jpg";
                                            Image tempimg = Image.FromFile(item);
                                            tempimg.Save(FilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                            tempimg.Dispose();
                                        }
                                        else
                                        {
                                            File.Copy(item, FilePath, true);
                                        }
                                        Image img = Image.FromFile(FilePath);
                                        Bitmap bmp = new Bitmap(img);
                                        img.Dispose();
                                        SingerMgr_EditSingerImg_Panel.BackColor = Color.Transparent;
                                        SingerMgr_EditSingerImg_Panel.BackgroundImage = bmp;
                                        SingerMgr_Tooltip_Label.Text = "已成功加入歌手圖片!";
                                    }
                                    catch
                                    {
                                        SingerMgr_Tooltip_Label.Text = "歌手圖檔無法變更!";
                                    }
                                }
                            }
                            else
                            {
                                if (item.ContainsAll(Path.GetTempPath(), ".bmp"))
                                {
                                    FilePath = Application.StartupPath + @"\Web\singerimg\" + SingerName + ".jpg";
                                    Image tempimg = Image.FromFile(item);
                                    tempimg.Save(FilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    tempimg.Dispose();
                                }
                                else
                                {
                                    File.Copy(item, FilePath, true);
                                }
                                Image img = Image.FromFile(FilePath);
                                Bitmap bmp = new Bitmap(img);
                                img.Dispose();
                                SingerMgr_EditSingerImg_Panel.BackColor = Color.Transparent;
                                SingerMgr_EditSingerImg_Panel.BackgroundImage = bmp;
                                SingerMgr_Tooltip_Label.Text = "已成功加入歌手圖片!";
                            }
                        }
                        else
                        {
                            SingerMgr_Tooltip_Label.Text = "拖曳的檔案不是圖檔!";
                        }
                    }
                }
            }
        }

        #endregion

    }

    class SingerMgr
    {
        public static bool ClearTooltipLabel = true;

        #region --- SingerMgr 建立資料表 ---

        public static List<string> SingerIdList;
        public static List<string> SingerList;
        public static List<string> SingerLowCaseList;
        public static List<string> SingerTypeList;
        public static List<string> AllSingerIdList;
        public static List<string> AllSingerList;
        public static List<string> AllSingerLowCaseList;
        public static List<string> AllSingerTypeList;

        public static void CreateSongDataTable()
        {
            SingerIdList = new List<string>();
            SingerList = new List<string>();
            SingerLowCaseList = new List<string>();
            SingerTypeList = new List<string>();

            string SongSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_Singer";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SingerIdList.Add(row["Singer_Id"].ToString());
                    SingerList.Add(row["Singer_Name"].ToString());
                    SingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                    SingerTypeList.Add(row["Singer_Type"].ToString());
                }
            }

            AllSingerIdList = new List<string>();
            AllSingerList = new List<string>();
            AllSingerLowCaseList = new List<string>();
            AllSingerTypeList = new List<string>();

            string SongAllSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_AllSinger";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongAllSingerQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    AllSingerIdList.Add(row["Singer_Id"].ToString());
                    AllSingerList.Add(row["Singer_Name"].ToString());
                    AllSingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                    AllSingerTypeList.Add(row["Singer_Type"].ToString());
                }
            }
        }

        public static void DisposeSongDataTable()
        {
            SingerIdList.Clear();
            SingerList.Clear();
            SingerLowCaseList.Clear();
            SingerTypeList.Clear();
            AllSingerIdList.Clear();
            AllSingerList.Clear();
            AllSingerLowCaseList.Clear();
            AllSingerTypeList.Clear();
            GC.Collect();
        }

        #endregion

        #region --- SingerMgr 取得下拉選單列表 ---

        public static DataTable GetDefaultSingerDataTableList()
        {
            List<string> SingerDTlist;

            #if DEBUG
                SingerDTlist = new List<string>() { "歌庫歌手", "預設歌手" };
            #else
                SingerDTlist = new List<string>() { "歌庫歌手" };
            #endif

            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            foreach (string str in SingerDTlist)
            {
                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = str;
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
            }
            return list;
        }

        public static DataTable GetSingerTypeList(bool MultiEdit)
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

            foreach (string str in Global.CrazyktvSingerTypeList)
            {
                if (str != "未使用")
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
            }
            return list;
        }

        public static DataTable GetSingerLastNameList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> Itemlist = new List<string>() { "以姓氏筆劃排序", "以姓氏注音排序" };

                foreach (string str in Itemlist)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
        }

        #endregion

        #region --- SingerMgr 歌手列表欄位設定 ---

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Singer_Id":
                    list = new List<string>() { "歌手編號", "0", "none" };
                    break;
                case "Singer_Name":
                    list = new List<string>() { "歌手名稱", "240", "none" };
                    break;
                case "Singer_Type":
                    list = new List<string>() { "歌手類別", "100", "none" };
                    break;
                case "Singer_Spell":
                    list = new List<string>() { "歌手拼音", "0", "none" };
                    break;
                case "Singer_Strokes":
                    list = new List<string>() { "歌手筆劃", "0", "none" };
                    break;
                case "Singer_SpellNum":
                    list = new List<string>() { "手機輸入", "0", "none" };
                    break;
                case "Singer_PenStyle":
                    list = new List<string>() { "筆形順序", "0", "none" };
                    break;
            }
            return list;
        }

        #endregion













    }
}
