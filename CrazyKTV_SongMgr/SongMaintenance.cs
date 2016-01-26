using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
        private void SongMaintenance_Save_Button_Click(object sender, EventArgs e)
        {
            switch (SongMaintenance_Save_Button.Text)
            {
                case "儲存設定":
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "DBVerEnableDBVerUpdate", Global.DBVerEnableDBVerUpdate);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "DBVerRebuildSingerData", Global.DBVerRebuildSingerData);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceEnableMultiSongPath", Global.SongMaintenanceEnableMultiSongPath);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceMultiSongPath", string.Join(",", Global.SongMaintenanceMultiSongPathList));
                    break;
                case "更新語系":
                    Global.CrazyktvSongLangList = new List<string>();
                    Global.CrazyktvSongLangKeyWordList = new List<string>();
                    TextBox[] SongMaintenance_Lang_TextBox =
                    {
                        SongMaintenance_Lang1_TextBox,
                        SongMaintenance_Lang2_TextBox,
                        SongMaintenance_Lang3_TextBox,
                        SongMaintenance_Lang4_TextBox,
                        SongMaintenance_Lang5_TextBox,
                        SongMaintenance_Lang6_TextBox,
                        SongMaintenance_Lang7_TextBox,
                        SongMaintenance_Lang8_TextBox,
                        SongMaintenance_Lang9_TextBox,
                        SongMaintenance_Lang10_TextBox
                    };

                    for (int i = 0; i < SongMaintenance_Lang_TextBox.Count<TextBox>(); i++)
                    {
                        Global.CrazyktvSongLangList.Add(SongMaintenance_Lang_TextBox[i].Text);
                    }
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangStr", string.Join(",", Global.CrazyktvSongLangList));

                    TextBox[] SongMaintenance_LangIDStr_TextBox =
                    {
                        SongMaintenance_Lang1IDStr_TextBox,
                        SongMaintenance_Lang2IDStr_TextBox,
                        SongMaintenance_Lang3IDStr_TextBox,
                        SongMaintenance_Lang4IDStr_TextBox,
                        SongMaintenance_Lang5IDStr_TextBox,
                        SongMaintenance_Lang6IDStr_TextBox,
                        SongMaintenance_Lang7IDStr_TextBox,
                        SongMaintenance_Lang8IDStr_TextBox,
                        SongMaintenance_Lang9IDStr_TextBox,
                        SongMaintenance_Lang10IDStr_TextBox
                    };

                    for (int i = 0; i < SongMaintenance_LangIDStr_TextBox.Count<TextBox>(); i++)
                    {
                        Global.CrazyktvSongLangKeyWordList.Add(SongMaintenance_LangIDStr_TextBox[i].Text);
                    }
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord", string.Join("|", Global.CrazyktvSongLangKeyWordList));

                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongMaintenance_SongLangUpdateTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆自訂語系資料,失敗 " + Global.TotalList[1] + " 筆,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_RefreshSongLang();
                            Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                            Common_SwitchSetUI(true);
                        });
                    });
                    break;
            }
        }


        #region --- 拼音校正 ---

        private void SongMaintenance_SingerSpellCorrect_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要校正歌手拼音嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                SongMaintenance_Tooltip_Label.Text = "正在解析歌手的拼音資料,請稍待...";

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_SpellCorrectTask("ktv_Singer")));
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_SpellCorrectTask("ktv_AllSinger")));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), SpellCorrectEndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 位歌庫歌手及 " + Global.TotalList[1] + " 位預設歌手的拼音資料,失敗 " + Global.TotalList[2] + " 位,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                    SongMaintenance.DisposeSongDataTable();
                });
            }
        }

        private void SongMaintenance_SongSpellCorrect_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要校正歌曲拼音嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                SongMaintenance_Tooltip_Label.Text = "正在解析歌曲的拼音資料,請稍待...";

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_SpellCorrectTask("ktv_Song")));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), SpellCorrectEndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Global.TimerEndTime = DateTime.Now;
                        SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 首歌曲的拼音資料,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                        SongMaintenance.DisposeSongDataTable();
                    });
                });
            }
        }

        private void SongMaintenance_SpellCorrectTask(object TableName)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> SpellList = new List<string>();

            switch ((string)TableName)
            {
                case "ktv_Singer":
                    if (Global.SingerDT.Rows.Count > 0)
                    {
                        foreach (DataRow row in Global.SingerDT.AsEnumerable())
                        {
                            string str = "";
                            SpellList = new List<string>();

                            str = row["Singer_Id"].ToString() + "|";
                            str += row["Singer_Name"].ToString() + "|";
                            SpellList = CommonFunc.GetSongNameSpell(row["Singer_Name"].ToString());
                            if (SpellList[2] == "") SpellList[2] = "0";
                            str += SpellList[0] + "|" + SpellList[2] + "|" + SpellList[1] + "|" + SpellList[3];
                            list.Add(str);
                        }

                        OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                        OleDbCommand cmd = new OleDbCommand();
                        string sqlColumnStr = "Singer_Spell = @SingerSpell, Singer_Strokes = @SingerStrokes, Singer_SpellNum = @SingerSpellNum, Singer_PenStyle = @SingerPenStyle";
                        string SongUpdateSqlStr = "update ktv_Singer set " + sqlColumnStr + " where Singer_Id = @SingerId";
                        cmd = new OleDbCommand(SongUpdateSqlStr, conn);
                        List<string> valuelist = new List<string>();

                        foreach (string str in list)
                        {
                            valuelist = new List<string>(str.Split('|'));

                            cmd.Parameters.AddWithValue("@SingerSpell", valuelist[2]);
                            cmd.Parameters.AddWithValue("@SingerStrokes", valuelist[3]);
                            cmd.Parameters.AddWithValue("@SingerSpellNum", valuelist[4]);
                            cmd.Parameters.AddWithValue("@SingerPenStyle", valuelist[5]);
                            cmd.Parameters.AddWithValue("@SingerId", valuelist[0]);

                            try
                            {
                                lock (LockThis)
                                {
                                    Global.TotalList[0]++;
                                }
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                lock (LockThis)
                                {
                                    Global.TotalList[2]++;
                                }
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【拼音校正】更新資料庫時發生錯誤: " + str;
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                            cmd.Parameters.Clear();
                            
                            this.BeginInvoke((Action)delegate()
                            {
                                SongMaintenance_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 位歌庫歌手及 " + Global.TotalList[1] + " 位預設歌手的拼音資料,請稍待...";
                            });
                        }
                        conn.Close();
                        list.Clear();
                    }
                    break;
                case "ktv_AllSinger":
                    if (Global.AllSingerDT.Rows.Count > 0)
                    {
                        foreach (DataRow row in Global.AllSingerDT.AsEnumerable())
                        {
                            string str = "";
                            SpellList = new List<string>();

                            str = row["Singer_Id"].ToString() + "|";
                            str += row["Singer_Name"].ToString() + "|";
                            SpellList = CommonFunc.GetSongNameSpell(row["Singer_Name"].ToString());
                            if (SpellList[2] == "") SpellList[2] = "0";
                            str += SpellList[0] + "|" + SpellList[2] + "|" + SpellList[1] + "|" + SpellList[3];
                            list.Add(str);
                        }

                        OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, "");
                        OleDbCommand cmd = new OleDbCommand();
                        string sqlColumnStr = "Singer_Spell = @SingerSpell, Singer_Strokes = @SingerStrokes, Singer_SpellNum = @SingerSpellNum, Singer_PenStyle = @SingerPenStyle";
                        string SongUpdateSqlStr = "update ktv_AllSinger set " + sqlColumnStr + " where Singer_Id = @SingerId";
                        cmd = new OleDbCommand(SongUpdateSqlStr, conn);
                        List<string> valuelist = new List<string>();

                        foreach (string str in list)
                        {
                            valuelist = new List<string>(str.Split('|'));

                            cmd.Parameters.AddWithValue("@SingerSpell", valuelist[2]);
                            cmd.Parameters.AddWithValue("@SingerStrokes", valuelist[3]);
                            cmd.Parameters.AddWithValue("@SingerSpellNum", valuelist[4]);
                            cmd.Parameters.AddWithValue("@SingerPenStyle", valuelist[5]);
                            cmd.Parameters.AddWithValue("@SingerId", valuelist[0]);

                            try
                            {
                                lock (LockThis)
                                {
                                    Global.TotalList[1]++;
                                }
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                lock (LockThis)
                                {
                                    Global.TotalList[2]++;
                                }
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【拼音校正】更新資料庫時發生錯誤: " + str;
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                            cmd.Parameters.Clear();

                            this.BeginInvoke((Action)delegate()
                            {
                                SongMaintenance_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 位歌庫歌手及 " + Global.TotalList[1] + " 位預設歌手的拼音資料,請稍待...";
                            });
                        }
                        conn.Close();
                        list.Clear();
                    }
                    break;
                case "ktv_Song":
                    if (Global.SongDT.Rows.Count > 0)
                    {
                        foreach (DataRow row in Global.SongDT.AsEnumerable())
                        {
                            string str = "";
                            SpellList = new List<string>();

                            str = row["Song_Id"].ToString() + "|";
                            str += row["Song_SongName"].ToString() + "|";
                            SpellList = CommonFunc.GetSongNameSpell(row["Song_SongName"].ToString());
                            if (SpellList[2] == "") SpellList[2] = "0";
                            str += SpellList[0] + "|" + SpellList[1] + "|" + SpellList[2] + "|" + SpellList[3];
                            list.Add(str);
                            this.BeginInvoke((Action)delegate()
                            {
                                SongMaintenance_Tooltip_Label.Text = "正在解析第 " + list.Count + " 首歌曲的拼音資料,請稍待...";
                            });
                        }

                        OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                        OleDbCommand cmd = new OleDbCommand();
                        string sqlColumnStr = "Song_Spell = @SongSpell, Song_SpellNum = @SongSpellNum, Song_SongStroke = @SongSongStroke, Song_PenStyle = @SongPenStyle";
                        string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
                        cmd = new OleDbCommand(SongUpdateSqlStr, conn);
                        List<string> valuelist = new List<string>();

                        foreach (string str in list)
                        {
                            valuelist = new List<string>(str.Split('|'));

                            cmd.Parameters.AddWithValue("@SongSpell", valuelist[2]);
                            cmd.Parameters.AddWithValue("@SongSpellNum", valuelist[3]);
                            cmd.Parameters.AddWithValue("@SongSongStroke", valuelist[4]);
                            cmd.Parameters.AddWithValue("@SongPenStyle", valuelist[5]);
                            cmd.Parameters.AddWithValue("@SongId", valuelist[0]);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                lock (LockThis)
                                {
                                    Global.TotalList[0]++;
                                }
                            }
                            catch
                            {
                                lock (LockThis)
                                {
                                    Global.TotalList[1]++;
                                }
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【拼音校正】更新資料庫時發生錯誤: " + str;
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                            cmd.Parameters.Clear();

                            this.BeginInvoke((Action)delegate()
                            {
                                SongMaintenance_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 首歌曲的拼音資料,請稍待...";
                            });
                        }
                        conn.Close();
                        list.Clear();
                    }
                    break;
            }
        }


        #endregion


        #region --- 聲道互換 ---


        private void SongMaintenance_LRTrackExchange_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要互換左右聲道數值嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_LRTrackExchangeTask()));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), LRTrackExchangeEndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 首歌曲的聲道資料,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                    SongMaintenance.DisposeSongDataTable();
                });
            }
        }


        private void SongMaintenance_LRTrackExchangeTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();

            var query = from row in Global.SongDT.AsEnumerable()
                         where row.Field<byte>("Song_Track").Equals(1) ||
                               row.Field<byte>("Song_Track").Equals(2)
                         select row;

            foreach (DataRow row in query)
            {
                switch (row["Song_Track"].ToString())
                {
                    case "1":
                        list.Add("2" + "|" + row["Song_Id"].ToString());
                        break;
                    case "2":
                        list.Add("1" + "|" + row["Song_Id"].ToString());
                        break;
                }
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在解析第 " + list.Count + " 首歌曲的聲道資料,請稍待...";
                });
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Track = @SongTrack";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongTrack", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【互換左右聲道】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在轉換第 " + Global.TotalList[0] + " 首歌曲的聲道資料,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        #endregion


        #region --- 音量變更 ---

        private void SongMaintenance_VolumeChange_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要變更音量嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_VolumeChangeTask(SongMaintenance_VolumeChange_TextBox.Text)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), VolumeChangeEndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 首歌曲的音量資料,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                    SongMaintenance.DisposeSongDataTable();
                });
            }
        }


        private void SongMaintenance_VolumeChangeTask(object VolumeValue)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            
            foreach (DataRow row in Global.SongDT.AsEnumerable())
            {
                list.Add((string)VolumeValue + "|" + row["Song_Id"].ToString());
                
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在解析第 " + list.Count + " 首歌曲的音量資料,請稍待...";
                });
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Volume = @SongVolume";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongVolume", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【變更歌曲音量】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在轉換第 " + Global.TotalList[0] + " 首歌曲的音量資料,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        #endregion


        #region --- 編碼位數轉換 ---


        private void SongMaintenance_CodeConvTo6_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要轉換為 6 位數編碼嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                if (Global.SongMgrMaxDigitCode != "2") SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = 2;
                SongMgrCfg_GetSongMgrLangCode();

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_CodeConvTask()));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), CodeConvEndTask =>
                {
                    if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\Favorite.txt"))
                    {
                        Global.SongDT = new DataTable();
                        string SongQuerySqlStr = "select Song_Id, Song_Path, Song_SongName, Song_Singer, Song_Volume, Song_Track, Song_Lang, Song_FileName, Song_SingerType, Song_SongType from ktv_Song order by Song_Id";
                        Global.SongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                        var FavoriteImportTask = Task.Factory.StartNew(() => SongMaintenance_FavoriteImportTask());
                        FavoriteImportTask.Wait();

                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_GetFavoriteUserList();
                            SongMaintenance_GetFavoriteUserList();
                            if (Global.SongQueryQueryType == "FavoriteQuery")
                            {
                                Global.SongQueryQueryType = "SongQuery";
                                SongQuery_DataGridView.DataSource = null;
                                if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                                SongQuery_QueryStatus_Label.Text = "";
                            }
                        });
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        Global.TimerEndTime = DateTime.Now;
                        SongMaintenance_Tooltip_Label.Text = "總共轉換 " + Global.TotalList[2] + " 首歌曲的歌曲編號,失敗 " + Global.TotalList[3] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        SongDBUpdate_CheckDatabaseFile();
                        Common_SwitchSetUI(true);
                        SongMaintenance.DisposeSongDataTable();
                    });
                });
            }
        }

        private void SongMaintenance_CodeConvTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            SongMaintenance_FavoriteExportTask();

            string MaxDigitCode = "";
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = "D5"; } else { MaxDigitCode = "D6"; }

            List<string> list = new List<string>();
            List<string> StrIdlist = new List<string>();
            StrIdlist = new List<string>(Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));
            List<int> Idlist = new List<int>();
            Idlist = StrIdlist.Select(s => Convert.ToInt32(s)).ToList();

            if (Global.SongDT.Rows.Count > 0)
            {
                foreach (DataRow row in Global.SongDT.AsEnumerable())
                {
                    string str = "";
                    if (CommonFunc.GetSongLangStr(0, 0, row["Song_Lang"].ToString()) != "-1")
                    {
                        int LangIndex = Convert.ToInt32(CommonFunc.GetSongLangStr(0, 0, row["Song_Lang"].ToString()));
                        string NewSongId = Idlist[LangIndex].ToString(MaxDigitCode);
                        Idlist[LangIndex]++;

                        str = row["Song_Id"].ToString() + "|";
                        str += NewSongId + "|";
                        str += row["Song_SongName"].ToString() + "|";
                        str += row["Song_Lang"].ToString();
                        list.Add(str);
                    }
                    else
                    {
                        lock (LockThis)
                        {
                            Global.TotalList[3]++;
                        }
                        str = row["Song_Id"].ToString() + "|";
                        str += row["Song_SongName"].ToString() + "|";
                        str += row["Song_Lang"].ToString();

                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【編碼位數轉換】此首歌曲的語系資料不正確: " + str;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "正在配發第 " + Global.SongDT.Rows.IndexOf(row) + " 首歌曲的歌曲編號,請稍待...";
                    });
                }
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @OldSongId and Song_SongName = @SongSongName and Song_Lang = @SongLang";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);
                cmd.Parameters.AddWithValue("@OldSongId", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongSongName", valuelist[2]);
                cmd.Parameters.AddWithValue("@SongLang", valuelist[3]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[2]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[3]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【編碼位數轉換】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在轉換第 " + Global.TotalList[2] + " 首歌曲的歌曲編號,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        private void SongMaintenance_CodeCorrect_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要校正編碼位數嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                var d5code = from row in Global.SongDT.AsEnumerable()
                             where row.Field<string>("Song_Id").Length == 5
                             select row;

                var d6code = from row in Global.SongDT.AsEnumerable()
                             where row.Field<string>("Song_Id").Length == 6
                             select row;

                if (d5code.Count<DataRow>() > d6code.Count<DataRow>())
                {
                    if (Global.SongMgrMaxDigitCode != "1") SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = 1;
                    CommonFunc.GetMaxSongId(5);
                    CommonFunc.GetNotExistsSongId(5);
                }
                else
                {
                    if (Global.SongMgrMaxDigitCode != "2") SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = 2;
                    CommonFunc.GetMaxSongId(6);
                    CommonFunc.GetNotExistsSongId(6);
                }
                SongMgrCfg_GetSongMgrLangCode();

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_CodeCorrectTask()));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), CodeCorrectEndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Global.TimerEndTime = DateTime.Now;
                        SongMaintenance_Tooltip_Label.Text = "總共校正 " + Global.TotalList[0] + " 首歌曲的歌曲編號,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        SongDBUpdate_CheckDatabaseFile();
                        Common_SwitchSetUI(true);
                        SongMaintenance.DisposeSongDataTable();
                    });
                });
            }
        }


        private void SongMaintenance_CodeCorrectTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            int CorrectDigitCode = 0;

            if (Global.SongMgrMaxDigitCode == "1") { CorrectDigitCode = 5; } else { CorrectDigitCode = 6; }

            List<string> list = new List<string>();

            if (Global.SongDT.Rows.Count > 0)
            {
                var query = from row in Global.SongDT.AsEnumerable()
                            where row.Field<string>("Song_Id").Length != CorrectDigitCode
                            select row;

                foreach (DataRow row in query)
                {
                    string str = "";
                    if (CommonFunc.GetSongLangStr(0, 0, row["Song_Lang"].ToString()) != "-1")
                    {
                        string LangStr = row["Song_Lang"].ToString();
                        string NewSongId = SongMaintenance.GetNextSongId(LangStr);

                        str = row["Song_Id"].ToString() + "|";
                        str += NewSongId + "|";
                        str += row["Song_SongName"].ToString() + "|";
                        str += row["Song_Lang"].ToString();
                        list.Add(str);
                    }
                    else
                    {
                        lock (LockThis)
                        {
                            Global.TotalList[1]++;
                        }
                        str = row["Song_Id"].ToString() + "|";
                        str += row["Song_SongName"].ToString() + "|";
                        str += row["Song_Lang"].ToString();

                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【編碼位數轉換】此首歌曲的語系資料不正確: " + str;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "正在配發第 " + Global.SongDT.Rows.IndexOf(row) + " 首歌曲的歌曲編號,請稍待...";
                    });
                }
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @OldSongId and Song_SongName = @SongSongName and Song_Lang = @SongLang";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);
                cmd.Parameters.AddWithValue("@OldSongId", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongSongName", valuelist[2]);
                cmd.Parameters.AddWithValue("@SongLang", valuelist[3]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【編碼位數轉換】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在轉換第 " + Global.TotalList[0] + " 首歌曲的歌曲編號,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        #endregion


        #region --- 歌曲路徑變更 ---


        private void SongMaintenance_SongPathChange_Button_Click(object sender, EventArgs e)
        {
            if (SongMaintenance_SongPathChange_Button.Text == "瀏覽")
            {
                FolderBrowserDialog opd = new FolderBrowserDialog();
                if (SongMaintenance_DestSongPath_TextBox.Text != "") opd.SelectedPath = SongMaintenance_DestSongPath_TextBox.Text;

                if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
                {
                    SongMaintenance_DestSongPath_TextBox.Text = opd.SelectedPath + @"\";
                    SongMaintenance_SongPathChange_Button.Text = "變更";
                }
            }
            else
            {
                if (SongMaintenance_SrcSongPath_TextBox.Text == "")
                {
                    SongMaintenance_Tooltip_Label.Text = "你尚未輸入【原始路徑】!";
                }
                else
                {
                    if (MessageBox.Show("你確定要變更歌曲路徑嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Global.TimerStartTime = DateTime.Now;
                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        SongMaintenance.CreateSongDataTable();
                        Common_SwitchSetUI(false);

                        string SrcSongPath = SongMaintenance_SrcSongPath_TextBox.Text;
                        string DestSongPath = SongMaintenance_DestSongPath_TextBox.Text;

                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongMaintenance_SongPathChangeTask(SrcSongPath, DestSongPath)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            Global.TimerEndTime = DateTime.Now;
                            this.BeginInvoke((Action)delegate()
                            {
                                SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 首歌曲的歌曲路徑資料,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                                SongMaintenance_SrcSongPath_TextBox.Text = "";
                                SongMaintenance_DestSongPath_TextBox.Text = "";
                                SongMaintenance_SongPathChange_Button.Text = "瀏覽";
                                Common_SwitchSetUI(true);
                            });
                            SongMaintenance.DisposeSongDataTable();
                        });
                    }
                }
            }
        }


        private void SongMaintenance_SongPathChangeTask(object ObjSrcSongPath, object ObjDestSongPath)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            string SongPath = "";
            string SrcSongPath = (string)ObjSrcSongPath;
            string DestSongPath = (string)ObjDestSongPath;
            List<string> list = new List<string>();

            var query = from row in Global.SongDT.AsEnumerable()
                        where row.Field<string>("Song_Path").ToLower().Contains(SrcSongPath.ToLower())
                        select row;

            if (query.Count<DataRow>() > 0)
            {
                foreach (DataRow row in query)
                {
                    SongPath = row["Song_Path"].ToString().Replace(SrcSongPath, DestSongPath);
                    list.Add(SongPath + "|" + row["Song_Id"].ToString());

                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "正在解析第 " + list.Count + " 首歌曲的歌曲路徑資料,請稍待...";
                    });
                }
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Path = @SongPath";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongPath", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【變更歌曲路徑】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在轉換第 " + Global.TotalList[0] + " 首歌曲的歌曲路徑資料,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        #endregion


        #region --- 播放次數 ---


        private void SongMaintenance_PlayCountExport_Button_Click(object sender, EventArgs e)
        {
            SongMaintenance.CreateSongDataTable();
            List<string> list = new List<string>();

            if (Global.SongDT.Rows.Count > 0)
            {
                foreach (DataRow row in Global.SongDT.AsEnumerable())
                {
                    if (row["Song_PlayCount"].ToString() != "")
                    {
                        if (Convert.ToInt32(row["Song_PlayCount"].ToString()) > 0)
                        {
                            list.Add("ktv_Song|" + row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString() + "|" + row["Song_SongName"].ToString() + "|" + row["Song_PlayCount"].ToString());
                        }
                    }
                }
            }

            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
            StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\PlayCount.txt");
            foreach (string str in list)
            {
                sw.WriteLine(str);
            }

            SongMaintenance_Tooltip_Label.Text = @"已將播放次數資料匯出至【SongMgr\Backup\PlayCount.txt】檔案。";
            sw.Close();
            SongMaintenance.DisposeSongDataTable();
        }


        private void SongMaintenance_PlayCountImport_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\PlayCount.txt"))
            {
                if (SongMaintenance_Tooltip_Label.Text == @"【SongMgr\Backup\PlayCount.txt】播放次數備份檔案不存在!") SongMaintenance_Tooltip_Label.Text = "";
                if (MessageBox.Show("你確定要匯入播放次數資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    SongMaintenance.CreateSongDataTable();
                    Common_SwitchSetUI(false);

                    SongMaintenance_Tooltip_Label.Text = "正在匯入播放次數資料,請稍待...";

                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongMaintenance_PlayCountImportTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), PlayCountImportEndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 筆播放次數資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                            SongMaintenance.DisposeSongDataTable();
                        });
                    });
                }
            }
            else
            {
                SongMaintenance_Tooltip_Label.Text = @"【SongMgr\Backup\PlayCount.txt】播放次數備份檔案不存在!";
            }
        }


        private void SongMaintenance_PlayCountImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();
            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");

            StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\PlayCount.txt", Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string sqlColumnStr = "Song_PlayCount = @SongPlayCount";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));
                switch (list[0])
                {
                    case "ktv_Song":
                        var query = from row in Global.SongDT.AsEnumerable()
                                    where row.Field<string>("Song_Lang").Equals(list[1]) &&
                                          row.Field<string>("Song_Singer").Equals(list[2]) &&
                                          row.Field<string>("Song_SongName").Equals(list[3])
                                    select row;

                        if (query.Count<DataRow>() > 0)
                        {
                            foreach (DataRow row in query)
                            {
                                string SongId = row["Song_Id"].ToString();
                                cmd.Parameters.AddWithValue("@SongPlayCount", list[4]);
                                cmd.Parameters.AddWithValue("@SongId", SongId);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                lock (LockThis)
                                {
                                    Global.TotalList[0]++;
                                }
                                break;
                            }
                        }
                        break;
                }
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 首歌曲的播放次數資料,請稍待...";
                });
            }
            conn.Close();
        }


        private void SongMaintenance_PlayCountReset_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要重置播放次數嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_PlayCountResetTask()));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), PlayCountResetEndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "總共重置 " + Global.TotalList[0] + " 首歌曲的播放次數,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                    SongMaintenance.DisposeSongDataTable();
                });
            }
        }


        private void SongMaintenance_PlayCountResetTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();

            foreach (DataRow row in Global.SongDT.AsEnumerable())
            {
                list.Add("0" + "|" + row["Song_Id"].ToString());

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在解析第 " + list.Count + " 首歌曲的播放次數資料,請稍待...";
                });
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_PlayCount = @SongPlayCount";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongPlayCount", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【重置播放次數】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在重置第 " + Global.TotalList[0] + " 首歌曲的播放次數資料,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        #endregion


        #region --- 雜項 ---


        private void SongMaintenance_SongWordCountCorrect_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要校正歌曲字數嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                SongMaintenance.CreateSongDataTable();
                Common_SwitchSetUI(false);

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_WordCountCorrectTask()));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 首歌曲的歌曲字數,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                    SongMaintenance.DisposeSongDataTable();
                });
            }
        }


        private void SongMaintenance_WordCountCorrectTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> SongWordCountList = new List<string>();

            foreach (DataRow row in Global.SongDT.AsEnumerable())
            {
                SongWordCountList = CommonFunc.GetSongWordCount(row["Song_SongName"].ToString());
                string SongWordCount = SongWordCountList[0];

                list.Add(SongWordCount + "|" + row["Song_Id"].ToString());

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在解析第 " + list.Count + " 首歌曲的字數資料,請稍待...";
                });
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_WordCount = @SongWordCount";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in list)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongWordCount", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongId", valuelist[1]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【字數校正】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在轉換第 " + Global.TotalList[0] + " 首歌曲的字數資料,請稍待...";
                });
            }
            conn.Close();
            list.Clear();
        }


        private void SongMaintenance_RemoveEmptyDirs_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要移除空資料夾嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Common_SwitchSetUI(false);

                SongMaintenance_Tooltip_Label.Text = "正在解析空資料夾,請稍待...";

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMaintenance_RemoveEmptyDirsTask(Global.SongMgrDestFolder, false)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        if (Global.TotalList[0] == 0)
                        {
                            SongMaintenance_Tooltip_Label.Text = "恭喜！在你的歌庫資料夾裡沒有發現任何空白資料夾。";
                        }
                        else
                        {
                            SongMaintenance_Tooltip_Label.Text = "總共移除 " + Global.TotalList[0] + " 個空資料夾,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        }
                        Common_SwitchSetUI(true);
                    });
                });
            }
        }


        private void SongMaintenance_RemoveEmptyDirsTask(string dir, bool stepBack)
        {
            string RootDir = Global.SongMgrDestFolder;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Directory.GetFileSystemEntries(dir).Length > 0)
            {
                if (!stepBack)
                {
                    foreach (string subdir in Directory.GetDirectories(dir))
                    {
                        SongMaintenance_RemoveEmptyDirsTask(subdir, false);
                    }
                }
            }
            else
            {
                DirectoryInfo dirinfo = new DirectoryInfo(dir);
                if ((dirinfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    dirinfo.Attributes = dirinfo.Attributes & ~FileAttributes.ReadOnly;
                }

                try
                {
                    Directory.Delete(dir);
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【移除空資料夾】以下為已移除的空資料夾: " + dir;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    lock (LockThis) { Global.TotalList[0]++; }
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【移除空資料夾】無法移除因資料夾已被占用: " + dir;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string prevDir = dir.Substring(0, dir.LastIndexOf("\\"));
                if (RootDir.Length <= prevDir.Length) SongMaintenance_RemoveEmptyDirsTask(prevDir, true);

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "已移除掉 " + Global.TotalList[0] + " 個空資料夾,請稍待...";
                });
            }
        }


        private void SongMaintenance_CompactAccessDB_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Global.CrazyktvDatabaseFile))
            {
                if (MessageBox.Show("你確定要壓縮並修復資料庫嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                    File.Copy(Global.CrazyktvDatabaseFile, Application.StartupPath + @"\SongMgr\Backup\" + DateTime.Now.ToLongDateString() + "_Compact_CrazySong.mdb", true);

                    Common_SwitchSetUI(false);
                    CommonFunc.CompactAccessDB("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Global.CrazyktvDatabaseFile + ";", Global.CrazyktvDatabaseFile);
                    Common_SwitchSetUI(true);
                }
            }
        }


        #endregion


        #region --- 遙控設定 ---


        private void SongMaintenance_RemoteCfgExport_Button_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            string RemoteQuerySqlStr = "";
            DataTable dt = new DataTable();

            string sqlColumnStr = "Remote_Id, Remote_Subject, Remote_Controler, Remote_Controler2, Remote_Name";
            RemoteQuerySqlStr = "select " + sqlColumnStr + " from ktv_Remote";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, RemoteQuerySqlStr, "");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    list.Add("ktv_Remote," + row["Remote_Id"].ToString() + "," + row["Remote_Subject"].ToString() + "," + row["Remote_Controler"].ToString() + "," + row["Remote_Controler2"].ToString() + "," + row["Remote_Name"].ToString());
                }
            }

            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
            StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\Remote.txt");
            foreach (string str in list)
            {
                sw.WriteLine(str);
            }

            SongMaintenance_Tooltip_Label.Text = @"已將遙控設定匯出至【SongMgr\Backup\Remote.txt】檔案。";
            sw.Close();
            list.Clear();
            dt.Dispose();
        }


        private void SongMaintenance_RemoteCfgImport_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\Remote.txt"))
            {
                if (SongMaintenance_Tooltip_Label.Text == @"【SongMgr\Backup\Remote.txt】遙控設定備份檔案不存在!") SongMaintenance_Tooltip_Label.Text = "";
                if (MessageBox.Show("你確定要重置並匯入遙控設定嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    SongMaintenance_Tooltip_Label.Text = "正在匯入遙控設定,請稍待...";

                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongMaintenance_RemoteCfgImportTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 項遙控設定,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            else
            {
                SongMaintenance_Tooltip_Label.Text = @"【SongMgr\Backup\Remote.txt】遙控設定備份檔案不存在!";
            }
        }


        private void SongMaintenance_RemoteCfgImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();
            OleDbCommand Versioncmd = new OleDbCommand();

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            string TruncateSqlStr = "delete * from ktv_Remote";
            cmd = new OleDbCommand(TruncateSqlStr, conn);
            cmd.ExecuteNonQuery();

            StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\Remote.txt", Encoding.UTF8); ;

            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string sqlColumnStr = "Remote_Id, Remote_Subject, Remote_Controler, Remote_Controler2, Remote_Name";
            string sqlValuesStr = "@RemoteId, @RemoteSubject, @RemoteControler, @RemoteControler2, @RemoteName";
            string RemoteAddSqlStr = "insert into ktv_Remote ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(RemoteAddSqlStr, conn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, ",", RegexOptions.None));
                switch (list[0])
                {
                    case "ktv_Remote":
                        cmd.Parameters.AddWithValue("@RemoteId", list[1]);
                        cmd.Parameters.AddWithValue("@RemoteSubject", list[2]);
                        cmd.Parameters.AddWithValue("@RemoteControler", list[3]);
                        cmd.Parameters.AddWithValue("@RemoteControler2", list[4]);
                        cmd.Parameters.AddWithValue("@RemoteName", list[5]);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        lock (LockThis) { Global.TotalList[0]++; }
                        break;
                }
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 項遙控設定資料,請稍待...";
                });
            }
            conn.Close();
        }


        #endregion


        #region --- 拼音資料 ---


        private void SongMaintenance_PhoneticsExport_Button_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            string PhoneticsQuerySqlStr = "";
            DataTable dt = new DataTable();

            string sqlColumnStr = "Word, Code, Spell, PenStyle, SortIdx, Strokes";
            PhoneticsQuerySqlStr = "select " + sqlColumnStr + " from ktv_Phonetics order by Code, SortIdx";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, PhoneticsQuerySqlStr, "");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    //list.Add("ktv_Phonetics," + row["Word"].ToString() + "," + CommonFunc.GetWordUnicode(row["Word"].ToString()) + "," + row["Spell"].ToString() + "," + row["PenStyle"].ToString() + "," + row["SortIdx"].ToString() + "," + row["Strokes"].ToString());
                    list.Add("ktv_Phonetics," + row["Word"].ToString() + "," + row["Code"].ToString() + "," + row["Spell"].ToString() + "," + row["PenStyle"].ToString() + "," + row["SortIdx"].ToString() + "," + row["Strokes"].ToString());
                }
            }

            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
            StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\Phonetics.txt");
            foreach (string str in list)
            {
                sw.WriteLine(str);
            }

            SongMaintenance_Tooltip_Label.Text = @"已將拼音資料匯出至【SongMgr\Backup\Phonetics.txt】檔案。";
            sw.Close();
            list.Clear();
            dt.Dispose();
        }


        private void SongMaintenance_PhoneticsImport_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\Phonetics.txt"))
            {
                if (SongMaintenance_Tooltip_Label.Text == @"【SongMgr\Backup\Phonetics.txt】拼音資料備份檔案不存在!") SongMaintenance_Tooltip_Label.Text = "";
                if (MessageBox.Show("你確定要重置並匯入拼音資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    SongMaintenance_Tooltip_Label.Text = "正在匯入拼音資料,請稍待...";

                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongMaintenance_PhoneticsImportTask(false)));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 項拼音資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            else
            {
                SongMaintenance_Tooltip_Label.Text = @"【SongMgr\Backup\Phonetics.txt】拼音資料備份檔案不存在!";
            }
        }


        private void SongMaintenance_PhoneticsImportTask(bool Update)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();
            OleDbCommand Versioncmd = new OleDbCommand();

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            string TruncateSqlStr = "delete * from ktv_Phonetics";
            cmd = new OleDbCommand(TruncateSqlStr, conn);
            cmd.ExecuteNonQuery();

            StreamReader sr;
            if (Update)
            {
                sr = new StreamReader(Application.StartupPath + @"\SongMgr\Update\UpdatePhoneticsDB.txt", Encoding.UTF8);
            }
            else
            {
                sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\Phonetics.txt", Encoding.UTF8);
            }

            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string sqlColumnStr = "Word, Code, Spell, PenStyle, SortIdx, Strokes";
            string sqlValuesStr = "@Word, @Code, @Spell, @PenStyle, @SortIdx, @Strokes";
            string RemoteAddSqlStr = "insert into ktv_Phonetics ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(RemoteAddSqlStr, conn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, ",", RegexOptions.None));
                switch (list[0])
                {
                    case "ktv_Version":
                        string VersionSqlStr = "PhoneticsDB = @PhoneticsDB";
                        string VersionUpdateSqlStr = "update ktv_Version set " + VersionSqlStr + " where Id = @Id";
                        Versioncmd = new OleDbCommand(VersionUpdateSqlStr, conn);
                        Versioncmd.Parameters.AddWithValue("@PhoneticsDB", list[1]);
                        Versioncmd.Parameters.AddWithValue("@Id", "1");
                        Versioncmd.ExecuteNonQuery();
                        Versioncmd.Parameters.Clear();
                        break;
                    case "ktv_Phonetics":
                        cmd.Parameters.AddWithValue("@Word", list[1]);
                        cmd.Parameters.AddWithValue("@Code", list[2]);
                        cmd.Parameters.AddWithValue("@Spell", list[3]);
                        cmd.Parameters.AddWithValue("@PenStyle", list[4]);
                        cmd.Parameters.AddWithValue("@SortIdx", list[5]);
                        cmd.Parameters.AddWithValue("@Strokes", list[6]);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        lock (LockThis) { Global.TotalList[0]++; }
                        break;
                }

                this.BeginInvoke((Action)delegate()
                {

                    SongMaintenance_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 項拼音資料,請稍待...";
                });
            }
            conn.Close();
        }
        
        #endregion

        #region --- 歌庫結構重建 ---

        private void SongMaintenance_RebuildSongStructure_Button_Click(object sender, EventArgs e)
        {
            if (SongMaintenance_RebuildSongStructure_Button.Text == "瀏覽")
            {
                FolderBrowserDialog opd = new FolderBrowserDialog();
                if (SongMaintenance_RebuildSongStructure_TextBox.Text != "") opd.SelectedPath = SongMaintenance_RebuildSongStructure_TextBox.Text;

                if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
                {
                    if (Directory.GetFiles(opd.SelectedPath, "*", SearchOption.AllDirectories).Count() == 0)
                    {
                        if (SongMaintenance_Tooltip_Label.Text == "請選擇一個空白的資料夾!") SongMaintenance_Tooltip_Label.Text = "";
                        SongMaintenance_RebuildSongStructure_TextBox.Text = opd.SelectedPath;
                        SongMaintenance_RebuildSongStructure_Button.Text = "重建";
                    }
                    else
                    {
                        SongMaintenance_Tooltip_Label.Text = "請選擇一個空白的資料夾!";
                    }
                }
            }
            else
            {
                if (SongMaintenance_RebuildSongStructure_TextBox.Text == "")
                {
                    SongMaintenance_Tooltip_Label.Text = "你尚未選擇【重建資料夾】!";
                }
                else
                {
                    if (MessageBox.Show("你確定要重建歌庫結構嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Global.TimerStartTime = DateTime.Now;
                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        SongMaintenance.CreateSongDataTable();
                        Common_SwitchSetUI(false);

                        string RebuildSongPath = SongMaintenance_RebuildSongStructure_TextBox.Text;

                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongMaintenance_RebuildSongStructureTask(RebuildSongPath, Global.SongMgrSongAddMode)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            Global.TimerEndTime = DateTime.Now;
                            this.BeginInvoke((Action)delegate()
                            {
                                SongMaintenance_Tooltip_Label.Text = "總共重建 " + Global.TotalList[0] + " 首歌曲,忽略 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成重建。";
                                SongMaintenance_RebuildSongStructure_TextBox.Text = "";
                                SongMaintenance_RebuildSongStructure_Button.Text = "瀏覽";

                                Global.SongMgrDestFolder = RebuildSongPath;
                                SongMgrCfg_DestFolder_TextBox.Text = RebuildSongPath;
                                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder", Global.SongMgrDestFolder);

                                // 統計歌曲數量
                                Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                                Common_SwitchSetUI(true);
                            });
                            SongMaintenance.DisposeSongDataTable();
                        });
                    }
                }
            }
        }


        private void SongMaintenance_RebuildSongStructureTask(string RebuildSongPath, string RebuildMode)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            List<string> RebuildSongFileValueList = new List<string>();

            foreach (DataRow row in Global.SongDT.Rows)
            {
                string SongId = row["Song_Id"].ToString();
                string SongLang = row["Song_Lang"].ToString();
                int SongSingerType = Convert.ToInt32(row["Song_SingerType"]);
                string SongSinger = row["Song_Singer"].ToString();
                string SongSongName = row["Song_SongName"].ToString();
                int SongTrack = Convert.ToInt32(row["Song_Track"]);
                string SongSongType = row["Song_SongType"].ToString();
                string SongFileName = row["Song_FileName"].ToString();
                string SongPath = row["Song_Path"].ToString();

                bool HasInvalidChar = false;
                Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
                if (r.IsMatch(SongLang)) HasInvalidChar = true;
                if (r.IsMatch(SongSinger)) HasInvalidChar = true;
                if (r.IsMatch(SongSongName)) HasInvalidChar = true;
                if (r.IsMatch(SongSongType)) HasInvalidChar = true;

                if (SongSingerType < 0 | SongSingerType > 10)
                {
                    SongSingerType = 10;
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構重建】此首歌曲歌手類別數值錯誤,已自動將其數值改為10: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                if (SongTrack < 0 | SongTrack > 5)
                {
                    SongTrack = 0;
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構重建】此首歌曲聲道數值錯誤,已自動將其數值改為0: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                // 重建歌檔
                string SongSrcPath = Path.Combine(SongPath, SongFileName);
                string SongDestPath = "";

                if (!HasInvalidChar)
                {
                    SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongFileName, SongPath, true, RebuildSongPath); ;
                    SongPath = Path.GetDirectoryName(SongDestPath) + @"\";
                    SongFileName = Path.GetFileName(SongDestPath);
                }

                bool FileIOError = false;
                if (File.Exists(SongSrcPath) & !HasInvalidChar)
                {
                    if (!Directory.Exists(SongPath)) Directory.CreateDirectory(SongPath);

                    try
                    {
                        switch (RebuildMode)
                        {
                            case "1":
                                FileAttributes attributes = File.GetAttributes(SongSrcPath);
                                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                {
                                    attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                    File.SetAttributes(SongSrcPath, attributes);
                                }

                                if (File.Exists(SongDestPath)) File.Delete(SongDestPath);
                                File.Move(SongSrcPath, SongDestPath);
                                break;
                            case "2":
                                File.Copy(SongSrcPath, SongDestPath, true);
                                break;
                        }
                    }
                    catch
                    {
                        FileIOError = true;
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構重建】檔案處理發生錯誤: " + SongSrcPath + " (唯讀或使用中)";
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                }
                else
                {
                    FileIOError = true;
                    if (HasInvalidChar)
                    {
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構重建】檔案處理發生錯誤: " + SongId + "|" + SongLang + "|" + SongSinger + "|" + SongSongName + "|" + SongSongType + " (含有非法字元)";
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                    else
                    {
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構重建】此首歌曲檔案不存在,已自動忽略重建檔案: " + SongId + "|" + SongSrcPath;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                    lock (LockThis) { Global.TotalList[1]++; }
                }

                if (!FileIOError)
                {
                    string RebuildSongFileValue = SongId + "|" + SongSingerType + "|" + SongTrack + "|" + SongFileName + "|" + SongPath;
                    RebuildSongFileValueList.Add(RebuildSongFileValue);
                    lock (LockThis) { Global.TotalList[0]++; }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_Tooltip_Label.Text = "已成功將 " + Global.TotalList[0] + " 首歌曲重建至重建資料夾,請稍待...";
                    });
                }
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id = @SongId, Song_SingerType = @SongSingerType, Song_Track = @SongTrack, Song_FileName = @SongFileName, Song_Path = @SongPath";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);

            List<string> valuelist = new List<string>();

            foreach (string str in RebuildSongFileValueList)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongSingerType", valuelist[1]);
                cmd.Parameters.AddWithValue("@SongTrack", valuelist[2]);
                cmd.Parameters.AddWithValue("@SongFileName", valuelist[3]);
                cmd.Parameters.AddWithValue("@SongPath", valuelist[4]);
                cmd.Parameters.AddWithValue("@SongId", valuelist[0]);

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis) { Global.TotalList[3]++; }
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】寫入重建檔案路徑至資料庫時發生錯誤: " + valuelist[0] + "|" + valuelist[3] + "|" + valuelist[4];
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    lock (LockThis)
                    {
                        Global.TotalList[0]--;
                        Global.TotalList[2]++;
                    }
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[3] + " 首歌曲的資料庫資料,請稍待...";
                });
            }
            RebuildSongFileValueList.Clear();
            conn.Close();
        }


        #endregion


        #region --- 我的最愛 ---

        private void SongMaintenance_GetFavoriteUserList()
        {
            SongMaintenance_Favorite_ListBox.DataSource = CommonFunc.GetFavoriteUserList(1);
            SongMaintenance_Favorite_ListBox.DisplayMember = "Display";
            SongMaintenance_Favorite_ListBox.ValueMember = "Value";
            SongMaintenance_Favorite_ListBox.SelectedValue = 1;
        }

        private void SongMaintenance_Favorite_ListBox_Enter(object sender, EventArgs e)
        {
            SongMaintenance_Tooltip_Label.Text = "";
            SongMaintenance_Favorite_Button.Text = "移除";
        }

        private void SongMaintenance_Favorite_TextBox_Enter(object sender, EventArgs e)
        {
            SongMaintenance_Favorite_TextBox.ImeMode = ImeMode.OnHalf;
            SongMaintenance_Tooltip_Label.Text = "";
            SongMaintenance_Favorite_Button.Text = "加入";
        }

        private void SongMaintenance_Favorite_Button_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string SongQuerySqlStr = "";
            string UserId = "";
            string UserName = "";

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();

            switch (SongMaintenance_Favorite_Button.Text)
            {
                case "加入":
                    if (SongMaintenance_Favorite_TextBox.Text != "")
                    {
                        if (SongMaintenance_Tooltip_Label.Text == "尚未輸入要加入的最愛用戶名稱!") SongMaintenance_Tooltip_Label.Text = "";
                        int AddUser = 1;
                        dt = (DataTable)SongMaintenance_Favorite_ListBox.DataSource;

                        if (dt.Rows.Count > 0)
                        {
                            if (SongMaintenance_Tooltip_Label.Text == "已有此最愛用戶名稱!") SongMaintenance_Tooltip_Label.Text = "";
                            foreach (DataRow row in dt.AsEnumerable())
                            {
                                if (row["Display"].ToString() == SongMaintenance_Favorite_TextBox.Text)
                                {
                                    SongMaintenance_Tooltip_Label.Text = "已有此最愛用戶名稱!";
                                    dt = new DataTable();
                                    AddUser = 0;
                                    break;
                                }
                            }
                        }
                        if (AddUser != 0)
                        {
                            SongQuerySqlStr = "select User_Id, User_Name from ktv_User";
                            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                            for (int i = 1; i < 999; i++)
                            {
                                int AddUserID = 1;
                                foreach (DataRow row in dt.AsEnumerable())
                                {
                                    if (row["User_Id"].ToString() == "U" + i.ToString("D3"))
                                    {
                                        AddUserID = 0;
                                        break;
                                    }
                                }

                                if (AddUserID != 0)
                                {
                                    UserId = "U" + i.ToString("D3");
                                    UserName = SongMaintenance_Favorite_TextBox.Text;
                                    break;
                                }
                            }
                            if (UserId != "")
                            {
                                conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                                string sqlColumnStr = "User_Id, User_Name";
                                string sqlValuesStr = "@UserId, @UserName";
                                string UserAddSqlStr = "insert into ktv_User ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
                                cmd = new OleDbCommand(UserAddSqlStr, conn);

                                cmd.Parameters.AddWithValue("@UserId", UserId);
                                cmd.Parameters.AddWithValue("@UserName", UserName);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                conn.Close();
                                SongMaintenance_Favorite_TextBox.Text = "";

                                SongQuery_GetFavoriteUserList();
                                SongMaintenance_GetFavoriteUserList();
                                if (Global.SongQueryQueryType == "FavoriteQuery")
                                {
                                    Global.SongQueryQueryType = "SongQuery";
                                    SongQuery_DataGridView.DataSource = null;
                                    if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                                    SongQuery_QueryStatus_Label.Text = "";
                                }
                            }
                            else
                            {
                                SongMaintenance_Tooltip_Label.Text = "最愛用戶數量已滿!";
                            }
                        }
                    }
                    else
                    {
                        SongMaintenance_Tooltip_Label.Text = "尚未輸入要加入的最愛用戶名稱!";
                    }
                    break;
                case "移除":
                    UserName = SongMaintenance_Favorite_ListBox.Text;
                    if (UserName == "")
                    {
                        SongMaintenance_Tooltip_Label.Text = "已無最愛用戶可移除!";
                    }
                    else
                    {
                        if (SongMaintenance_Tooltip_Label.Text == "已無最愛用戶可移除!") SongMaintenance_Tooltip_Label.Text = "";
                        if (MessageBox.Show("你確定要移除此最愛用戶【" + UserName + "】及其所有最愛歌曲嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            SongQuerySqlStr = "select User_Id, User_Name from ktv_User";
                            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                            var query = from row in dt.AsEnumerable()
                                        where row.Field<string>("User_Name").Equals(UserName)
                                        select row;

                            if (query.Count<DataRow>() > 0)
                            {
                                foreach (DataRow row in query)
                                {
                                    UserId = row["User_Id"].ToString();
                                    break;
                                }
                            }

                            if (UserId != "")
                            {
                                conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                                string UserRemoveSqlStr = "delete from ktv_User where User_Id = @UserId and User_Name = @UserName";
                                cmd = new OleDbCommand(UserRemoveSqlStr, conn);

                                cmd.Parameters.AddWithValue("@UserId", UserId);
                                cmd.Parameters.AddWithValue("@UserName", UserName);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                                List<string> list = new List<string>();

                                var dtquery = from row in dt.AsEnumerable()
                                              where row.Field<string>("User_Id").Equals(UserId)
                                              select row;

                                if (dtquery.Count<DataRow>() > 0)
                                {
                                    foreach (DataRow row in dtquery)
                                    {
                                        list.Add(row["Song_Id"].ToString());
                                    }


                                    string FavoriteRemoveSqlStr = "delete from ktv_Favorite where User_Id = @UserId and Song_Id = @SongId";
                                    cmd = new OleDbCommand(FavoriteRemoveSqlStr, conn);

                                    foreach (string SongId in list)
                                    {
                                        cmd.Parameters.AddWithValue("@UserId", UserId);
                                        cmd.Parameters.AddWithValue("@SongId", SongId);
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                    }
                                }
                                conn.Close();
                                SongQuery_GetFavoriteUserList();
                                SongMaintenance_GetFavoriteUserList();
                                if (Global.SongQueryQueryType == "FavoriteQuery")
                                {
                                    Global.SongQueryQueryType = "SongQuery";
                                    SongQuery_DataGridView.DataSource = null;
                                    if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                                    SongQuery_QueryStatus_Label.Text = "";
                                }
                            }
                        }
                    }
                    break;
            }
            dt.Dispose();
        }


        private void SongMaintenance_FavoriteExport_Button_Click(object sender, EventArgs e)
        {
            SongMaintenance.CreateSongDataTable();

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => SongMaintenance_FavoriteExportTask()));

            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
            {
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = @"已將我的最愛資料匯出至【SongMgr\Backup\Favorite.txt】檔案。";
                    SongMaintenance.DisposeSongDataTable();
                });
            });
        }

        public static void SongMaintenance_FavoriteExportTask()
        {
            List<string> list = new List<string>();

            string SongQuerySqlStr = "select User_Id, User_Name from ktv_User";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        list.Add("ktv_User|" + row["User_Id"].ToString() + "|" + row["User_Name"].ToString());
                    }
                }
            }

            SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        var query = from QueryRow in Global.SongDT.AsEnumerable()
                                    where QueryRow.Field<string>("Song_Id").Equals(row["Song_Id"].ToString())
                                    select QueryRow;

                        if (query.Count<DataRow>() > 0)
                        {
                            foreach (DataRow songrow in query)
                            {
                                list.Add("ktv_Favorite|" + row["User_Id"].ToString() + "|" + songrow["Song_Lang"].ToString() + "|" + songrow["Song_Singer"].ToString() + "|" + songrow["Song_SongName"].ToString());
                                break;
                            }
                        }
                    }
                }
            }

            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
            StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\Favorite.txt");
            foreach (string str in list)
            {
                sw.WriteLine(str);
            }
            sw.Close();
        }

        private void SongMaintenance_FavoriteImport_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\Favorite.txt"))
            {
                if (SongMaintenance_Tooltip_Label.Text == @"【SongMgr\Backup\Favorite.txt】我的最愛備份檔案不存在!") SongMaintenance_Tooltip_Label.Text = "";
                if (MessageBox.Show("你確定要重置並匯入我的最愛嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    SongMaintenance.CreateSongDataTable();
                    Common_SwitchSetUI(false);

                    SongMaintenance_Tooltip_Label.Text = "正在匯入我的最愛,請稍待...";

                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongMaintenance_FavoriteImportTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), FavoriteImportEndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 位最愛用戶及 " + Global.TotalList[1] + " 首最愛歌曲,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);

                            SongQuery_GetFavoriteUserList();
                            SongMaintenance_GetFavoriteUserList();
                            if (Global.SongQueryQueryType == "FavoriteQuery")
                            {
                                Global.SongQueryQueryType = "SongQuery";
                                SongQuery_DataGridView.DataSource = null;
                                if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                                SongQuery_QueryStatus_Label.Text = "";
                            }
                            SongMaintenance.DisposeSongDataTable();
                        });
                    });
                }
            }
            else
            {
                SongMaintenance_Tooltip_Label.Text = @"【SongMgr\Backup\Favorite.txt】我的最愛備份檔案不存在!";
            }
        }


        private void SongMaintenance_FavoriteImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand Ucmd = new OleDbCommand();
            OleDbCommand Fcmd = new OleDbCommand();

            string TruncateSqlStr = "";

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            TruncateSqlStr = "delete * from ktv_User";
            Ucmd = new OleDbCommand(TruncateSqlStr, conn);
            Ucmd.ExecuteNonQuery();

            TruncateSqlStr = "delete * from ktv_Favorite";
            Fcmd = new OleDbCommand(TruncateSqlStr, conn);
            Fcmd.ExecuteNonQuery();

            StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\Favorite.txt", Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string UserColumnStr = "User_Id, User_Name";
            string UserValuesStr = "@UserId, @UserName";
            string UserAddSqlStr = "insert into ktv_User ( " + UserColumnStr + " ) values ( " + UserValuesStr + " )";
            Ucmd = new OleDbCommand(UserAddSqlStr, conn);

            string FavoriteColumnStr = "User_Id, Song_Id";
            string FavoriteValuesStr = "@UserId, @SongId";
            string FavoriteAddSqlStr = "insert into ktv_Favorite ( " + FavoriteColumnStr + " ) values ( " + FavoriteValuesStr + " )";
            Fcmd = new OleDbCommand(FavoriteAddSqlStr, conn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));
                switch (list[0])
                {
                    case "ktv_User":
                        Ucmd.Parameters.AddWithValue("@UserId", list[1]);
                        Ucmd.Parameters.AddWithValue("@UserName", list[2]);
                        Ucmd.ExecuteNonQuery();
                        Ucmd.Parameters.Clear();
                        lock (LockThis)
                        {
                            Global.TotalList[0]++;
                        }
                        break;
                    case "ktv_Favorite":
                        var query = from row in Global.SongDT.AsEnumerable()
                                    where row.Field<string>("Song_Lang").Equals(list[2]) &&
                                          row.Field<string>("Song_Singer").Equals(list[3]) &&
                                          row.Field<string>("Song_SongName").Equals(list[4])
                                    select row;

                        if (query.Count<DataRow>() > 0)
                        {
                            foreach (DataRow row in query)
                            {
                                string SongId = row["Song_Id"].ToString();
                                Fcmd.Parameters.AddWithValue("@UserId", list[1]);
                                Fcmd.Parameters.AddWithValue("@SongId", SongId);
                                Fcmd.ExecuteNonQuery();
                                Fcmd.Parameters.Clear();
                                lock (LockThis)
                                {
                                    Global.TotalList[1]++;
                                }
                                break;
                            }
                        }
                        break;
                }
            }
            conn.Close();
        }


        #endregion


        #region --- 自訂語系 ---

        private void SongMaintenance_SetCustomLangControl()
        {
            TextBox[] SongMaintenance_Lang_TextBox =
            {
                SongMaintenance_Lang1_TextBox,
                SongMaintenance_Lang2_TextBox,
                SongMaintenance_Lang3_TextBox,
                SongMaintenance_Lang4_TextBox,
                SongMaintenance_Lang5_TextBox,
                SongMaintenance_Lang6_TextBox,
                SongMaintenance_Lang7_TextBox,
                SongMaintenance_Lang8_TextBox,
                SongMaintenance_Lang9_TextBox,
                SongMaintenance_Lang10_TextBox
            };

            for (int i = 0; i < SongMaintenance_Lang_TextBox.Count<TextBox>(); i++)
            {
                SongMaintenance_Lang_TextBox[i].Text = Global.CrazyktvSongLangList[i];
            }

            TextBox[] SongMaintenance_LangIDStr_TextBox =
            {
              SongMaintenance_Lang1IDStr_TextBox,
              SongMaintenance_Lang2IDStr_TextBox,
              SongMaintenance_Lang3IDStr_TextBox,
              SongMaintenance_Lang4IDStr_TextBox,
              SongMaintenance_Lang5IDStr_TextBox,
              SongMaintenance_Lang6IDStr_TextBox,
              SongMaintenance_Lang7IDStr_TextBox,
              SongMaintenance_Lang8IDStr_TextBox,
              SongMaintenance_Lang9IDStr_TextBox,
              SongMaintenance_Lang10IDStr_TextBox
            };

            for (int i = 0; i < SongMaintenance_LangIDStr_TextBox.Count<TextBox>(); i++)
            {
                SongMaintenance_LangIDStr_TextBox[i].Text = Global.CrazyktvSongLangKeyWordList[i];
            }
        }


        private void SongMaintenance_TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMaintenance_TabControl.SelectedTab.Name)
            {
                case "SongMaintenance_CustomLang_TabPage":
                    SongMaintenance_Save_Button.Text = "更新語系";
                    SongMaintenance_Save_Button.Enabled = true;
                    break;
                case "SongMaintenance_MultiSongPath_TabPage":
                    SongMaintenance_Save_Button.Text = "儲存設定";
                    SongMaintenance_Save_Button.Enabled = true;
                    break;
                case "SongMaintenance_DBVer_TabPage":
                    SongMaintenance_Save_Button.Text = "儲存設定";
                    if (SongMaintenance_TabControl.Enabled == true) SongMaintenance_Save_Button.Enabled = true;
                    break;
                default:
                    SongMaintenance_Save_Button.Text = "儲存設定";
                    SongMaintenance_Save_Button.Enabled = false;
                    break;
            }
        }


        private void SongMaintenance_SongLangUpdateTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();

            if (File.Exists(Application.StartupPath + @"\SongMgr\Custom.lang"))
            {
                if (!Directory.Exists(Application.StartupPath + @"\Lang")) Directory.CreateDirectory(Application.StartupPath + @"\Lang");
                File.Copy(Application.StartupPath + @"\SongMgr\Custom.lang", Application.StartupPath + @"\Lang\Custom.lang", true);

                if (File.Exists(Application.StartupPath + @"\Lang\Custom.lang"))
                {
                    StreamReader sr = new StreamReader(Application.StartupPath + @"\Lang\Custom.lang", Encoding.Unicode);
                    while (!sr.EndOfStream)
                    {
                        list.Add(sr.ReadLine());
                    }
                    sr.Close();

                    list[2] = string.Join(",", Global.CrazyktvSongLangList);

                    StreamWriter sw = new StreamWriter(Application.StartupPath + @"\Lang\Custom.lang", false, Encoding.Unicode);
                    foreach (string str in list)
                    {
                        sw.WriteLine(str);
                    }
                    sw.Close();

                    CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "Language", "Custom");
                }
            }

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Langauage_Name = @LangauageName, Langauage_KeyWord = @LangauageKeyWord";
            string SongUpdateSqlStr = "update ktv_Langauage set " + sqlColumnStr + " where Langauage_Id = @LangauageId";
            cmd = new OleDbCommand(SongUpdateSqlStr, conn);

            foreach (string str in Global.CrazyktvSongLangList)
            {
                cmd.Parameters.AddWithValue("@LangauageName", str);
                cmd.Parameters.AddWithValue("@LangauageKeyWord", Global.CrazyktvSongLangKeyWordList[Global.CrazyktvSongLangList.IndexOf(str)]);
                cmd.Parameters.AddWithValue("@LangauageId", Global.CrazyktvSongLangList.IndexOf(str));

                try
                {
                    cmd.ExecuteNonQuery();
                    lock (LockThis)
                    {
                        Global.TotalList[0]++;
                    }
                }
                catch
                {
                    lock (LockThis)
                    {
                        Global.TotalList[1]++;
                    }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【自訂語系】更新資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                cmd.Parameters.Clear();
            }
            conn.Close();
        }


        #endregion


        #region --- 多重歌庫 ---


        private void SongMaintenance_EnableMultiSongPath_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongMaintenanceEnableMultiSongPath = SongMaintenance_EnableMultiSongPath_CheckBox.Checked.ToString();
            switch (Global.SongMaintenanceEnableMultiSongPath)
            {
                case "True":
                    SongMaintenance_MultiSongPath_ListBox.Enabled = true;
                    SongMaintenance_MultiSongPath_Button.Enabled = true;
                    break;
                case "False":
                    SongMaintenance_MultiSongPath_ListBox.Enabled = false;
                    SongMaintenance_MultiSongPath_Button.Enabled = false;
                    break;
            }
        }


        private void SongMaintenance_MultiSongPath_ListBox_Enter(object sender, EventArgs e)
        {
            SongMaintenance_MultiSongPath_Button.Text = "移除";
        }


        private void SongMaintenance_MultiSongPath_TextBox_Enter(object sender, EventArgs e)
        {
            if (SongMaintenance_MultiSongPath_TextBox.Text != "")
            {
                SongMaintenance_MultiSongPath_Button.Text = "加入";
            }
            else
            {
                SongMaintenance_MultiSongPath_Button.Text = "瀏覽";
            }
        }


        private void SongMaintenance_MultiSongPath_Button_Click(object sender, EventArgs e)
        {
            switch (SongMaintenance_MultiSongPath_Button.Text)
            {
                case "瀏覽":
                    FolderBrowserDialog opd = new FolderBrowserDialog();
                    if (SongMaintenance_MultiSongPath_TextBox.Text != "") opd.SelectedPath = SongMaintenance_MultiSongPath_TextBox.Text;

                    if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
                    {
                        SongMaintenance_MultiSongPath_TextBox.Text = opd.SelectedPath + @"\";
                        SongMaintenance_MultiSongPath_Button.Text = "加入";
                    }
                    break;
                case "加入":
                    DataTable dt = (DataTable)SongMaintenance_MultiSongPath_ListBox.DataSource;
                    if (SongMaintenance_MultiSongPath_ListBox.SelectedItems.Count > 0)
                    {
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = SongMaintenance_MultiSongPath_TextBox.Text;
                        dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                    }
                    else
                    {
                        using (DataTable NewDT = new DataTable())
                        {
                            NewDT.Columns.Add(new DataColumn("Display", typeof(string)));
                            NewDT.Columns.Add(new DataColumn("Value", typeof(int)));

                            NewDT.Rows.Add(NewDT.NewRow());
                            NewDT.Rows[NewDT.Rows.Count - 1][0] = SongMaintenance_MultiSongPath_TextBox.Text;
                            NewDT.Rows[NewDT.Rows.Count - 1][1] = NewDT.Rows.Count;
                            dt = NewDT.Copy();
                        }
                        SongMaintenance_MultiSongPath_ListBox.DataSource = dt;
                        SongMaintenance_MultiSongPath_ListBox.DisplayMember = "Display";
                        SongMaintenance_MultiSongPath_ListBox.ValueMember = "Value";
                    }
                    SongMaintenance_MultiSongPath_TextBox.Text = "";
                    SongMaintenance_MultiSongPath_Button.Text = "瀏覽";

                    SongMaintenance_MultiSongPath_ListBox.DataSource = dt;
                    SongMaintenance_MultiSongPath_ListBox.DisplayMember = "Display";
                    SongMaintenance_MultiSongPath_ListBox.ValueMember = "Value";

                    Global.SongMaintenanceMultiSongPathList = new List<string>();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.AsEnumerable())
                        {
                            if (row["Display"].ToString() != "")
                            {
                                Global.SongMaintenanceMultiSongPathList.Add(row["Display"].ToString());
                            }
                        }
                    }
                    break;
                case "移除":
                    if (SongMaintenance_MultiSongPath_ListBox.SelectedItem != null)
                    {
                        int index = SongMaintenance_MultiSongPath_ListBox.SelectedIndex;
                        dt = (DataTable)SongMaintenance_MultiSongPath_ListBox.DataSource;
                        dt.Rows.RemoveAt(index);

                        Global.SongMaintenanceMultiSongPathList = new List<string>();

                        foreach (DataRow row in dt.AsEnumerable())
                        {
                            if (row["Display"].ToString() != "")
                            {
                                Global.SongMaintenanceMultiSongPathList.Add(row["Display"].ToString());
                            }
                        }
                    }
                    else
                    {
                        SongMaintenance_Tooltip_Label.Text = "已無可以刪除的多重歌庫資料夾路徑!";
                    }
                    break;
            }
        }


        #endregion


        #region --- 歌庫版本 ---

        private void SongMaintenance_EnableDBVerUpdate_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.DBVerEnableDBVerUpdate = SongMaintenance_EnableDBVerUpdate_CheckBox.Checked.ToString();
        }


        private void SongMaintenance_EnableRebuildSingerData_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.DBVerRebuildSingerData = SongMaintenance_EnableRebuildSingerData_CheckBox.Checked.ToString();
        }

        private void SongMaintenance_SingerImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbConnection singerconn = new OleDbConnection();
            OleDbCommand Versioncmd = new OleDbCommand();
            OleDbCommand allsingercmd = new OleDbCommand();

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            singerconn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, "");
            string TruncateSqlStr = "delete * from ktv_AllSinger";
            allsingercmd = new OleDbCommand(TruncateSqlStr, singerconn);
            allsingercmd.ExecuteNonQuery();

            StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Update\UpdateSingerDB.txt", Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            string sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
            string AllSingerAddSqlStr = "insert into ktv_AllSinger ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            allsingercmd = new OleDbCommand(AllSingerAddSqlStr, singerconn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, ",", RegexOptions.None));
                switch (list[0])
                {
                    case "ktv_Version":
                        string VersionSqlStr = "SingerDB = @SingerDB";
                        string VersionUpdateSqlStr = "update ktv_Version set " + VersionSqlStr + " where Id = @Id";
                        Versioncmd = new OleDbCommand(VersionUpdateSqlStr, conn);

                        Versioncmd.Parameters.AddWithValue("@SingerDB", list[1]);
                        Versioncmd.Parameters.AddWithValue("@Id", "1");
                        Versioncmd.ExecuteNonQuery();
                        Versioncmd.Parameters.Clear();
                        break;
                    case "ktv_AllSinger":
                        allsingercmd.Parameters.AddWithValue("@SingerId", list[1]);
                        allsingercmd.Parameters.AddWithValue("@SingerName", list[2]);
                        allsingercmd.Parameters.AddWithValue("@SingerType", list[3]);
                        allsingercmd.Parameters.AddWithValue("@SingerSpell", list[4]);
                        allsingercmd.Parameters.AddWithValue("@SingerStrokes", list[5]);
                        allsingercmd.Parameters.AddWithValue("@SingerSpellNum", list[6]);
                        allsingercmd.Parameters.AddWithValue("@SingerPenStyle", list[7]);

                        allsingercmd.ExecuteNonQuery();
                        allsingercmd.Parameters.Clear();
                        lock (LockThis) { Global.TotalList[0]++; }
                        break;
                }
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                });
            }
            conn.Close();
            singerconn.Close();
        }

        private void SongMaintenance_CashboxImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Addlist = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            OleDbConnection Cashboxconn = new OleDbConnection();
            OleDbCommand Versioncmd = new OleDbCommand();
            OleDbCommand Cashboxcmd = new OleDbCommand();

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            Cashboxconn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, "");
            string TruncateSqlStr = "delete * from ktv_Cashbox";
            Cashboxcmd = new OleDbCommand(TruncateSqlStr, Cashboxconn);
            Cashboxcmd.ExecuteNonQuery();

            StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Update\UpdateCashboxDB.txt", Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                Addlist.Add(sr.ReadLine());
            }
            sr.Close();

            string sqlColumnStr = "Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate";
            string sqlValuesStr = "@CashboxId, @SongLang, @SongSinger, @SongSongName, @SongCreatDate";
            string CashboxAddSqlStr = "insert into ktv_Cashbox ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            Cashboxcmd = new OleDbCommand(CashboxAddSqlStr, Cashboxconn);

            foreach (string AddStr in Addlist)
            {
                list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));

                switch (list[0])
                {
                    case "ktv_Version":
                        string VersionSqlStr = "CashboxDB = @CashboxDB";
                        string VersionUpdateSqlStr = "update ktv_Version set " + VersionSqlStr + " where Id = @Id";
                        Versioncmd = new OleDbCommand(VersionUpdateSqlStr, conn);

                        Versioncmd.Parameters.AddWithValue("@CashboxDB", list[1]);
                        Versioncmd.Parameters.AddWithValue("@Id", "1");
                        Versioncmd.ExecuteNonQuery();
                        Versioncmd.Parameters.Clear();
                        break;
                    case "ktv_UpdDate":
                        Global.CashboxUpdDate = DateTime.Parse(list[1]);
                        string CashboxUpdDateSqlStr = "CashboxUpdDate = @CashboxUpdDate";
                        string CashboxUpdDateUpdateSqlStr = "update ktv_Version set " + CashboxUpdDateSqlStr + " where Id = @Id";
                        Versioncmd = new OleDbCommand(CashboxUpdDateUpdateSqlStr, conn);

                        Versioncmd.Parameters.AddWithValue("@CashboxUpdDate", list[1]);
                        Versioncmd.Parameters.AddWithValue("@Id", "1");
                        Versioncmd.ExecuteNonQuery();
                        Versioncmd.Parameters.Clear();
                        break;
                    case "ktv_Cashbox":
                        Cashboxcmd.Parameters.AddWithValue("@CashboxId", list[1]);
                        Cashboxcmd.Parameters.AddWithValue("@SongLang", list[2]);
                        Cashboxcmd.Parameters.AddWithValue("@SongSinger", list[3]);
                        Cashboxcmd.Parameters.AddWithValue("@SongSongName", list[4]);
                        Cashboxcmd.Parameters.AddWithValue("@SongCreatDate", list[5]);
                        Cashboxcmd.ExecuteNonQuery();
                        Cashboxcmd.Parameters.Clear();
                        lock (LockThis) { Global.TotalList[0]++; }
                        break;
                }
                this.BeginInvoke((Action)delegate()
                {
                    SongMaintenance_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 首錢櫃資料,請稍待...";
                });
            }
            conn.Close();
            Cashboxconn.Close();
        }

        #endregion


    }




    class SongMaintenance
    {
        public static void CreateSongDataTable()
        {
            Global.SongDT = new DataTable();
            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_PlayCount, Song_FileName, Song_Path from ktv_Song order by Song_Id";
            Global.SongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            Global.SingerList = new List<string>();
            Global.SingerLowCaseList = new List<string>();
            Global.SingerTypeList = new List<string>();

            Global.SingerDT = new DataTable();
            string SongSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_Singer";
            Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

            foreach (DataRow row in Global.SingerDT.AsEnumerable())
            {
                Global.SingerList.Add(row["Singer_Name"].ToString());
                Global.SingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                Global.SingerTypeList.Add(row["Singer_Type"].ToString());
            }

            Global.AllSingerList = new List<string>();
            Global.AllSingerLowCaseList = new List<string>();
            Global.AllSingerTypeList = new List<string>();

            Global.AllSingerDT = new DataTable();
            string SongAllSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_AllSinger";
            Global.AllSingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongAllSingerQuerySqlStr, "");

            foreach (DataRow row in Global.AllSingerDT.AsEnumerable())
            {
                Global.AllSingerList.Add(row["Singer_Name"].ToString());
                Global.AllSingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                Global.AllSingerTypeList.Add(row["Singer_Type"].ToString());
            }
        }

        public static void DisposeSongDataTable()
        {
            Global.SingerList.Clear();
            Global.SingerLowCaseList.Clear();
            Global.SingerTypeList.Clear();
            Global.AllSingerList.Clear();
            Global.AllSingerLowCaseList.Clear();
            Global.AllSingerTypeList.Clear();
            Global.SongDT.Dispose();
            Global.SongDT = null;
            Global.SingerDT.Dispose();
            Global.SingerDT = null;
            Global.AllSingerDT.Dispose();
            Global.AllSingerDT = null;
        }

        public static DataTable GetMultiSongPathList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            if (Global.SongMaintenanceMultiSongPathList.Count > 0)
            {
                foreach (string s in Global.SongMaintenanceMultiSongPathList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = s;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
            }
            return list;
        }

        public static string GetNextSongId(string SongLang)
        {
            string NewSongID = "";

            // 查詢歌曲編號有無斷號
            if (Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Count > 0)
            {
                NewSongID = Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)][0];
                Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Remove(NewSongID);
            }

            // 若無斷號查詢各語系下個歌曲編號
            if (NewSongID == "")
            {
                string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";
                Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]++;
                NewSongID = Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)].ToString(MaxDigitCode);
            }
            return NewSongID;
        }


    }
}
