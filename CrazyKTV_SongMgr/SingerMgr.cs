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
    public partial class MainFrom : Form
    {
        private void SingerMgr_DefaultSingerDataTable_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SingerMgr_DefaultSingerDataTable_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    Global.SingerMgrDefaultSingerDataTable = "ktv_Singer";
                    break;
                case "2":
                    Global.SingerMgrDefaultSingerDataTable = "ktv_AllSinger";
                    break;
            }
            SingerMgr_DataGridView.DataSource = null;
            Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
        }

        private void SingerMgr_QueryValue_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_QueryTask("SingerName")));

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

        private void SingerMgr_Query_Button_Click(object sender, EventArgs e)
        {
            if (SingerMgr_QueryValue_TextBox.Text != "")
            {
                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_QueryTask("SingerName")));

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
                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_QueryTask("SingerType")));

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

        private void SingerMgr_QueryTask(object QueryType)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            this.BeginInvoke((Action)delegate()
            {
                string SingerType = Global.CrazyktvSingerTypeList.IndexOf(SingerMgr_QueryType_ComboBox.Text).ToString();
                string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
                string QueryValue = SingerMgr_QueryValue_TextBox.Text;
                string QueryValueNarrow = QueryValue;
                string QueryValueWide = QueryValue;
                string SingerQuerySqlStr = "";

                SingerMgr_DataGridView.DataSource = null;

                Regex HasWideChar = new Regex("[\x21-\x7E\xFF01-\xFF5E]");
                if ((string)QueryType == "SingerName")
                {
                    if (HasWideChar.IsMatch(QueryValue))
                    {
                        QueryValueNarrow = CommonFunc.ConvToNarrow(QueryValue);
                        QueryValueWide = CommonFunc.ConvToWide(QueryValue);
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

                if ((string)QueryType == "SingerName")
                {
                    if (HasWideChar.IsMatch(QueryValue))
                    {
                        SingerQuerySqlStr = "select " + sqlColumnStr + " from " + Global.SingerMgrDefaultSingerDataTable + " where InStr(1,LCase(Singer_Name),LCase('" + QueryValue + "'),0) <>0 or InStr(1,LCase(Singer_Name),LCase('" + QueryValueNarrow + "'),0) <>0 or InStr(1,LCase(Singer_Name),LCase('" + QueryValueWide + "'),0) <>0 order by Singer_Name";
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

                if (File.Exists(Global.CrazyktvDatabaseFile))
                {
                    try
                    {
                        DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SingerQuerySqlStr, "");
                        if (dt.Rows.Count == 0)
                        {
                            SingerMgr_Tooltip_Label.Text = "查無歌手,請重新查詢...";
                        }
                        else
                        {

                            if ((string)QueryType == "SingerName")
                            {
                                SingerMgr_Tooltip_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SingerMgr_QueryValue_TextBox.Text + "』的歌手。";
                            }
                            else
                            {
                                SingerMgr_Tooltip_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SingerMgr_QueryType_ComboBox.Text + "』的歌手。";
                            }

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
                                SingerMgr_DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                            }

                            SingerMgr_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                            SingerMgr_DataGridView.Columns["Singer_Type"].Width = 100;
                            SingerMgr_DataGridView.Columns["Singer_Type"].MinimumWidth = 100;
                            SingerMgr_DataGridView.Columns["Singer_Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dt.Dispose();
                        }
                    }
                    catch
                    {
                        SingerMgr_Tooltip_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                    }
                }
            });
        }

        private void SingerMgr_SingerUpdateTask(object SingerUpdateDT)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            DataTable dt = new DataTable();
            dt = (DataTable)SingerUpdateDT;
            List<string> UpdateValueList = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                int i = Convert.ToInt32(row["RowIndex"]);
                string OldSingerName = row["SingerName"].ToString();

                string SingerId = SingerMgr_DataGridView.Rows[i].Cells["Singer_Id"].Value.ToString();
                string SingerName = SingerMgr_DataGridView.Rows[i].Cells["Singer_Name"].Value.ToString();
                int SingerType = Convert.ToInt32(SingerMgr_DataGridView.Rows[i].Cells["Singer_Type"].Value);
                string SingerSpell = SingerMgr_DataGridView.Rows[i].Cells["Singer_Spell"].Value.ToString();
                string SingerStrokes = SingerMgr_DataGridView.Rows[i].Cells["Singer_Strokes"].Value.ToString();
                string SingerSpellNum = SingerMgr_DataGridView.Rows[i].Cells["Singer_SpellNum"].Value.ToString();
                string SingerPenStyle = SingerMgr_DataGridView.Rows[i].Cells["Singer_PenStyle"].Value.ToString();
                string SingerDTSingerId = "";

                var query = from SingerDTRow in (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.AllSingerDT.AsEnumerable() : Global.SingerDT.AsEnumerable()
                                  where SingerDTRow.Field<string>("Singer_Name").ToLower().Equals(OldSingerName.ToLower())
                                  select SingerDTRow;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow dtrow in query)
                    {
                        SingerDTSingerId = dtrow["Singer_Id"].ToString();
                        break;
                    }
                }

                string UpdateValue = SingerId + "*" + SingerName + "*" + SingerType + "*" + SingerSpell + "*" + SingerStrokes + "*" + SingerSpellNum + "*" + SingerPenStyle + "*" + SingerDTSingerId;
                UpdateValueList.Add(UpdateValue);
            }
            
            OleDbConnection conn = new OleDbConnection();
            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            string sqlColumnStr = "Singer_Id = @SingerId, Singer_Name = @SingerName, Singer_Type = @SingerType, Singer_Spell = @SingerSpell, Singer_Strokes = @SingerStrokes, Singer_SpellNum = @SingerSpellNum, Singer_PenStyle = @SingerPenStyle";
            string AllSingerUpdateSqlStr = "update ktv_AllSinger set " + sqlColumnStr + " where Singer_Id=@OldSingerId";
            string SingerUpdateSqlStr = "update ktv_Singer set " + sqlColumnStr + " where Singer_Id=@OldSingerId";

            OleDbCommand[] SingerUpdateCmds = 
            {
                new OleDbCommand(SingerUpdateSqlStr, conn),
                new OleDbCommand(AllSingerUpdateSqlStr, conn)
            };

            List<string> valuelist = new List<string>();

            foreach (string str in UpdateValueList)
            {
                valuelist = new List<string>(str.Split('*'));

                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerId", valuelist[0]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerName", valuelist[1]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerType", valuelist[2]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerSpell", valuelist[3]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerStrokes", valuelist[4]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerSpellNum", valuelist[5]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@SingerPenStyle", valuelist[6]);
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.AddWithValue("@OldSingerId", valuelist[0]);

                try
                {
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].ExecuteNonQuery();
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】更新" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "全部") + "歌手資料表時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                    this.BeginInvoke((Action)delegate()
                    {
                        SingerMgr_Tooltip_Label.Text = "修改歌手資料有誤,請回報操作記錄裡的內容!";
                    });
                }
                SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 0 : 1].Parameters.Clear();

                if (valuelist[7] != "")
                {
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerId", valuelist[7]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerName", valuelist[1]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerType", valuelist[2]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerSpell", valuelist[3]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerStrokes", valuelist[4]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerSpellNum", valuelist[5]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@SingerPenStyle", valuelist[6]);
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.AddWithValue("@OldSingerId", valuelist[7]);

                    try
                    {
                        SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].ExecuteNonQuery();
                    }
                    catch
                    {
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌手管理】更新" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "全部" : "歌庫") + "歌手資料表時發生錯誤: " + str;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                        this.BeginInvoke((Action)delegate()
                        {
                            SingerMgr_Tooltip_Label.Text = "修改歌手資料有誤,請回報操作記錄裡的內容!";
                        });
                    }
                    SingerUpdateCmds[(Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 0].Parameters.Clear();
                }
                
            }
            conn.Close();

            this.BeginInvoke((Action)delegate()
            {
                if (Global.SongLogDT.Rows.Count > 0) SongLog_TabPage.Text = "操作記錄 (" + Global.SongLogDT.Rows.Count + ")";
            });
        }

        private void SingerMgr_SingerRemoveTask(object SingerIdlist)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string SingerRemoveSqlStr = "delete from " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger") + " where Singer_Id=@SingerId";
            cmd = new OleDbCommand(SingerRemoveSqlStr, conn);

            foreach (string str in (List<string>)SingerIdlist)
            {
                cmd.Parameters.AddWithValue("@SingerId", str);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            conn.Close();
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

            var query = from row in (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.SingerDT.AsEnumerable() : Global.AllSingerDT.AsEnumerable()
                        where row.Field<string>("Singer_Name").ToLower().Equals(SingerName.ToLower())
                        select row;

            if (query.Count<DataRow>() > 0)
            {
                this.BeginInvoke((Action)delegate()
                {
                    SingerMgr_Tooltip_Label.Text = "歌手【" + SingerName + "】已在" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "全部") + "歌手資料庫中...";
                });
            }
            else
            {
                int MaxAllSingerId = CommonFunc.GetMaxSingerId((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger", Global.CrazyktvDatabaseFile) + 1;
                List<string> NotExistsAllSingerId = new List<string>();
                NotExistsAllSingerId = CommonFunc.GetNotExistsSingerId((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger", Global.CrazyktvDatabaseFile);

                string SingerId = "";
                if (NotExistsAllSingerId.Count > 0)
                {
                    SingerId = NotExistsAllSingerId[0];
                }
                else
                {
                    SingerId = MaxAllSingerId.ToString();
                }

                List<string> spelllist = new List<string>();
                spelllist = CommonFunc.GetSongNameSpell(SingerName);

                OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
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
                        SingerMgr_Tooltip_Label.Text = "已成功將歌手【" + SingerName + "】加入至" + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "歌庫" : "全部") + "歌手資料表。";
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

        private void SingerMgr_SingerExport_Button_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            string SingerQuerySqlStr = "";
            DataTable dt = new DataTable();

            string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            SingerQuerySqlStr = "select " + sqlColumnStr + " from " + ((Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? "ktv_Singer" : "ktv_AllSinger");
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SingerQuerySqlStr, "");

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

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
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
                        Common_SwitchSetUI(true);
                    });
                    SingerMgr.DisposeSongDataTable();
                });
            }
        }

        private void SingerMgr_NonSingerDataLog_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            SingerMgr.CreateSongDataTable();
            Common_SwitchSetUI(false);

            SingerMgr_Tooltip_Label.Text = "正在解析歌庫歌手資料,請稍待...";

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => SingerMgr_NonSingerDataLogTask()));

            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
            {
                Global.TimerEndTime = DateTime.Now;
                this.BeginInvoke((Action)delegate()
                {
                    SingerMgr_Tooltip_Label.Text = "總共從歌庫歌曲解析出 " + Global.TotalList[0] + " 位歌手,查詢到 " + Global.TotalList[1] + " 筆無資料的歌庫歌手,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    Common_SwitchSetUI(true);
                });
                SingerMgr.DisposeSongDataTable();
            });
        }

        private void SingerMgr_NonSingerDataLogTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Singerlist = new List<string>();
            List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, ",", RegexOptions.IgnoreCase));

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
                                    var query = from srow in Global.AllSingerDT.AsEnumerable()
                                                where srow.Field<string>("Singer_Name").ToLower().Equals(SpecialSingerName.ToLower())
                                                select srow;

                                    if (query.Count<DataRow>() == 0)
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
                                    var query = from srow in Global.AllSingerDT.AsEnumerable()
                                                where srow.Field<string>("Singer_Name").ToLower().Equals(ChorusSingerName.ToLower())
                                                select srow;
                                    
                                    if (query.Count<DataRow>() == 0)
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
                                var query = from srow in Global.AllSingerDT.AsEnumerable()
                                            where srow.Field<string>("Singer_Name").ToLower().Equals(SingerName.ToLower())
                                            select srow;

                                if (query.Count<DataRow>() == 0)
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
                    }
                    else
                    {
                        if (Singerlist.IndexOf(SingerName) < 0)
                        {
                            var query = from srow in Global.AllSingerDT.AsEnumerable()
                                        where srow.Field<string>("Singer_Name").ToLower().Equals(SingerName.ToLower())
                                        select srow;
                            
                            if (query.Count<DataRow>() == 0)
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
                        SingerMgr_Tooltip_Label.Text = "已解析到 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                    });
                }
            }
            Singerlist.Clear();
            dt.Dispose();

            if (list.Count > 0)
            {
                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【記錄無資料歌手】以下為無資料的歌手: " + string.Join(",", list);
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
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

        private void SingerMgr_SingerAddPaste_Button_Click(object sender, EventArgs e)
        {
            SingerMgr_SingerAddName_TextBox.Text = Clipboard.GetText();
        }

        private void SingerMgr_SingerAddClear_Button_Click(object sender, EventArgs e)
        {
            SingerMgr_SingerAddName_TextBox.Text = "";
        }
    }

    class SingerMgr
    {
        public static void CreateSongDataTable()
        {
            string SongSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_Singer";
            Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

            string SongAllSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_AllSinger";
            Global.AllSingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongAllSingerQuerySqlStr, "");

            string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
            Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongPhoneticsQuerySqlStr, "");
        }

        public static void DisposeSongDataTable()
        {
            Global.SingerDT.Dispose();
            Global.AllSingerDT.Dispose();
            Global.PhoneticsDT.Dispose();
        }

        public static DataTable GetDefaultSingerDataTableList()
        {
            List<string> SingerDTlist = new List<string>() { "歌庫歌手", "全部歌手" };
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

        public static DataTable GetSingerTypeList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

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














    }
}
