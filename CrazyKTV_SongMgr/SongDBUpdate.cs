using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void SongMgrDB_CheckDatabaseFile()
        {
            Global.SongMgrDatabaseError = false;

            if (File.Exists(Global.CrazyktvSongMgrDatabaseFile))
            {
                List<string> TableList = new List<string>() { "ktv_AllSinger", "ktv_Cashbox", "ktv_Phonetics", "ktv_SongMgr", "ktv_Version" };
                List<string> SongMgrDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvSongMgrDatabaseFile, ""));
                foreach (string TableName in TableList)
                {
                    if (SongMgrDBTableList.IndexOf(TableName) < 0)
                    {
                        Global.SongMgrDatabaseError = true;
                        break;
                    }
                }
                TableList.Clear();
                TableList = null;
                SongMgrDBTableList.Clear();
                SongMgrDBTableList = null;
            }
        }

        private List<bool> GetCrazyktvDatabaseStatus()
        {
            Global.CrazyktvDatabaseStatus = false;
            Global.CrazyktvDatabaseIsOld = false;
            Global.CrazyktvDatabaseError = false;
            Global.SongMgrDatabaseError = false;
            Global.CrazyktvDatabaseMaxDigitCode = true;

            bool CrazyKTVDatabaseFile = false;
            bool SongMgrDatabaseFile = false;
            bool SongMgrDestFolder = false;
            bool TB_ktv_AllSinger = false;
            bool TB_ktv_Version = false;
            bool Col_Song_ReplayGain = false;
            bool Col_Langauage_KeyWord = false;
            bool Col_GodLiu = false;

            // 檢查資料庫表格及欄位
            if (File.Exists(Global.CrazyktvDatabaseFile) && File.Exists(Global.CrazyktvSongMgrDatabaseFile))
            {
                CrazyKTVDatabaseFile = true;
                SongMgrDatabaseFile = true;

                List<string> TableList = new List<string>() { "ktv_Favorite", "ktv_Langauage", "ktv_Phonetics", "ktv_Remote", "ktv_Singer", "ktv_SingerName", "ktv_Song", "ktv_Swan", "ktv_User", "ktv_Words" };
                List<string> CrazyktvDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvDatabaseFile, ""));
                foreach (string TableName in TableList)
                {
                    if (CrazyktvDBTableList.IndexOf(TableName) < 0)
                    {
                        Global.CrazyktvDatabaseError = true;
                        break;
                    }
                }
                TableList.Clear();
                TableList = null;

                TableList = new List<string>() { "ktv_AllSinger", "ktv_Cashbox", "ktv_Phonetics", "ktv_SongMgr", "ktv_Version" };
                List<string> SongMgrDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvSongMgrDatabaseFile, ""));
                foreach (string TableName in TableList)
                {
                    if (SongMgrDBTableList.IndexOf(TableName) < 0)
                    {
                        Global.SongMgrDatabaseError = true;
                        break;
                    }
                }
                TableList.Clear();
                TableList = null;

                if (!Global.CrazyktvDatabaseError)
                {
                    List<string> ktvLangauageColumnList = new List<string>(CommonFunc.GetOleDbColumnList(Global.CrazyktvDatabaseFile, "", "ktv_Langauage"));
                    if (ktvLangauageColumnList.IndexOf("Langauage_KeyWord") >= 0) Col_Langauage_KeyWord = true;
                    ktvLangauageColumnList.Clear();
                    ktvLangauageColumnList = null;

                    if (CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0) TB_ktv_AllSinger = true;
                    if (CrazyktvDBTableList.IndexOf("ktv_Version") >= 0) TB_ktv_Version = true;

                    List<string> GodLiuColumnList = new List<string>() { "Song_SongNameFuzzy", "Song_SingerFuzzy", "Song_FuzzyVer", "DLspace", "Epasswd", "imgpath", "cashboxsongid", "cashboxdat", "holidaysongid" };
                    List<string> ktvSongColumnList = new List<string>(CommonFunc.GetOleDbColumnList(Global.CrazyktvDatabaseFile, "", "ktv_Song"));
                    if (ktvSongColumnList.IndexOf("Song_ReplayGain") >= 0) Col_Song_ReplayGain = true;

                    foreach (string ColumnName in GodLiuColumnList)
                    {
                        if (ktvSongColumnList.IndexOf(ColumnName) >= 0) Col_GodLiu = true;
                    }
                    GodLiuColumnList.Clear();
                    GodLiuColumnList = null;
                    ktvSongColumnList.Clear();
                    ktvSongColumnList = null;
                    if (TB_ktv_AllSinger || TB_ktv_Version || !Col_Song_ReplayGain || !Col_Langauage_KeyWord || Col_GodLiu) Global.CrazyktvDatabaseIsOld = true;
                }
                CrazyktvDBTableList.Clear();
                CrazyktvDBTableList = null;
            }

            // 檢查歌庫資料夾
            if (Directory.Exists(Global.SongMgrDestFolder)) SongMgrDestFolder = true;

            if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
            {
                if (!CrazyKTVDatabaseFile || !SongMgrDatabaseFile || Global.SongMgrDatabaseError || TB_ktv_AllSinger || TB_ktv_Version || !Col_Song_ReplayGain || !Col_Langauage_KeyWord || Col_GodLiu)
                { Global.SongMgrDBVerErrorUIStatus = false; }
                else { Global.CrazyktvDatabaseStatus = true; }
            }
            else
            {
                if (!CrazyKTVDatabaseFile || !SongMgrDatabaseFile || Global.SongMgrDatabaseError || !SongMgrDestFolder || TB_ktv_AllSinger || TB_ktv_Version || !Col_Song_ReplayGain || !Col_Langauage_KeyWord || Col_GodLiu)
                { Global.SongMgrDBVerErrorUIStatus = false; }
                else { Global.CrazyktvDatabaseStatus = true; }
            }
            return new List<bool>() { CrazyKTVDatabaseFile, TB_ktv_Version, TB_ktv_AllSinger, SongMgrDestFolder };
        }

        private void SongDBUpdate_CheckDatabaseFile()
        {
            List<bool> list = GetCrazyktvDatabaseStatus();
            bool CrazyKTVDatabaseFile = list[0];
            bool TB_ktv_Version = list[1];
            bool TB_ktv_AllSinger = list[2];
            list.Clear();
            list = null;

            if (Global.CrazyktvDatabaseStatus)
            {
                var CheckDBUpdateTask = Task.Factory.StartNew(() => SongDBUpdate_CheckDatabaseVersion());
            }
            else if (CrazyKTVDatabaseFile && !Global.CrazyktvDatabaseError && !Global.SongMgrDatabaseError && TB_ktv_Version)
            {
                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongMaintenance_TabPage);
                SongMaintenance_TabControl.SelectedIndex = SongMaintenance_TabControl.TabPages.IndexOf(SongMaintenance_DBVer_TabPage);
                SongMaintenance_DBVerTooltip_Label.Text = "偵測到資料庫結構更動,開始進行更新...";
                var UpdateDBTask = Task.Factory.StartNew(() => SongDBUpdate_UpdateDatabaseFile("RemovektvVersion"));
            }
            else if (CrazyKTVDatabaseFile && !Global.CrazyktvDatabaseError && !Global.SongMgrDatabaseError && TB_ktv_AllSinger)
            {
                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongMaintenance_TabPage);
                SongMaintenance_TabControl.SelectedIndex = SongMaintenance_TabControl.TabPages.IndexOf(SongMaintenance_DBVer_TabPage);
                SongMaintenance_DBVerTooltip_Label.Text = "偵測到資料庫結構更動,開始進行更新...";
                var UpdateDBTask = Task.Factory.StartNew(() => SongDBUpdate_UpdateDatabaseFile("RemovektvAllSinger"));
            }
            else if (CrazyKTVDatabaseFile && !Global.CrazyktvDatabaseError && !Global.SongMgrDatabaseError && Global.CrazyktvDatabaseIsOld)
            {
                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongMaintenance_TabPage);
                SongMaintenance_TabControl.SelectedIndex = SongMaintenance_TabControl.TabPages.IndexOf(SongMaintenance_DBVer_TabPage);
                SongMaintenance_DBVerTooltip_Label.Text = "偵測到資料庫結構更動,開始進行更新...";
                var UpdateDBTask = Task.Factory.StartNew(() => SongDBUpdate_UpdateDatabaseFile("UpdateVersion"));
            }
            else
            {
                Global.DatabaseUpdateFinished = true;
            }
        }

        private void SongDBUpdate_CheckDatabaseVersion()
        {
            bool UpdateDBStatus = false;
            double SongDBVer = 1.00;
            string CashboxUpdDate = "";

            string VersionQuerySqlStr = "select * from ktv_Version";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, VersionQuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        SongDBVer = Convert.ToDouble(row["SongDB"]);
                        CashboxUpdDate = row["CashboxUpdDate"].ToString();
                    }
                    Global.CashboxUpdDate = DateTime.Parse(CashboxUpdDate);

                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_DBVer1Value_Label.Text = SongDBVer.ToString("F2") + " 版";
                        Cashbox_UpdDateValue_Label.Text = (CultureInfo.CurrentCulture.Name == "zh-TW") ? DateTime.Parse(CashboxUpdDate).ToLongDateString() : DateTime.Parse(CashboxUpdDate).ToShortDateString();
                    });

                    if (Global.DBVerEnableDBVerUpdate == "True")
                    {
                        if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Update")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Update");
                        
                        string url = "https://raw.githubusercontent.com/CrazyKTV/SongMgr/master/CrazyKTV_SongMgr/SongMgr/Update/UpdateDB.xml";
                        using (MemoryStream ms = CommonFunc.Download(url))
                        {
                            if (ms.Length > 0)
                            {
                                ms.Position = 0;
                                Global.CrazyktvSongDBVer = CommonFunc.LoadConfigXmlFile("", "SongDBVer", ms, true);

                                if (File.Exists(Global.CrazyktvDatabaseFile))
                                {
                                    if (Convert.ToDouble(Global.CrazyktvSongDBVer) > SongDBVer)
                                    {
                                        this.BeginInvoke((Action)delegate ()
                                        {
                                            if (MessageBox.Show("你確定要更新歌庫版本嗎?", "偵測到歌庫版本更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                            {
                                                Global.SongMgrDBVerErrorUIStatus = false;
                                                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongMaintenance_TabPage);
                                                SongMaintenance_TabControl.SelectedIndex = SongMaintenance_TabControl.TabPages.IndexOf(SongMaintenance_DBVer_TabPage);
                                                SongMaintenance_DBVerTooltip_Label.Text = "開始進行歌庫版本更新...";
                                                UpdateDBStatus = true;
                                                var UpdateDBTask = Task.Factory.StartNew(() => SongDBUpdate_UpdateDatabaseFile("UpdateVersion"));
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                if (!UpdateDBStatus) SongDBUpdate_UpdateFinish();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:必須檢閱 SQL 查詢中是否有安全性弱點")]
        private void SongDBUpdate_UpdateDatabaseFile(string UpdateType)
        {
            Global.TimerStartTime = DateTime.Now;

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, ""))
            {
                bool UpdateError = false;

                if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                string SongDBBackupFile = SongDBBackupFile = Application.StartupPath + @"\SongMgr\Backup\" + DateTime.Now.ToLongDateString() + "_CrazySong.mdb";
                File.Copy(Global.CrazyktvDatabaseFile, SongDBBackupFile, true);

                List<string> CrazyktvDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvDatabaseFile, ""));

                // 移除 ktv_AllSinger 資料表
                if (CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
                {
                    using (OleDbCommand cmd = new OleDbCommand("drop table ktv_AllSinger", conn))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        catch
                        {
                            UpdateError = true;
                            this.BeginInvoke((Action)delegate ()
                            {
                                SongMaintenance_DBVerTooltip_Label.Text = "移除 ktv_AllSinger 資料表失敗,已還原為原本的資料庫檔案。";
                            });
                        }
                    }
                }

                // 移除 ktv_Version 資料表
                if (CrazyktvDBTableList.IndexOf("ktv_Version") >= 0)
                {
                    using (OleDbCommand cmd = new OleDbCommand("drop table ktv_Version", conn))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        catch
                        {
                            UpdateError = true;
                            this.BeginInvoke((Action)delegate ()
                            {
                                SongMaintenance_DBVerTooltip_Label.Text = "移除 ktv_Version 資料表失敗,已還原為原本的資料庫檔案。";
                            });
                        }
                    }
                }
                CrazyktvDBTableList.Clear();
                CrazyktvDBTableList = null;

                if (!UpdateError)
                {
                    bool UpdateKtvSong = false;
                    bool UpdateKtvSinger = false;
                    bool UpdatePhonetics = false;
                    bool UpdateLangauage = true;
                    bool AddSongReplayGainColumn = true;
                    bool RemoveGodLiuColumn = false;
                    List<string> GodLiuColumnlist = new List<string>();

                    List<string> tablelist = new List<string>() { "ktv_Singer", "ktv_Phonetics", "ktv_Langauage" };
                    String[] Restrictions = new String[4];
                    Restrictions[2] = "ktv_Song";
                    using (DataTable dt = conn.GetSchema("Columns", Restrictions))
                    {
                        foreach (string tablename in tablelist)
                        {
                            Restrictions[2] = tablename;
                            using (DataTable tb = conn.GetSchema("Columns", Restrictions))
                            {
                                foreach (DataRow row in tb.Rows)
                                {
                                    dt.ImportRow(row);
                                }
                            }
                        }
                        tablelist.Clear();
                        tablelist = null;

                        foreach (DataRow row in dt.Rows)
                        {
                            switch (row["COLUMN_NAME"].ToString())
                            {
                                case "Song_SongName":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "80") UpdateKtvSong = true;
                                    break;
                                case "Song_Singer":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "60") UpdateKtvSong = true;
                                    break;
                                case "Song_Spell":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "80") UpdateKtvSong = true;
                                    break;
                                case "Song_FileName":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "255") UpdateKtvSong = true;
                                    break;
                                case "Song_SpellNum":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "80") UpdateKtvSong = true;
                                    break;
                                case "Song_PenStyle":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "80") UpdateKtvSong = true;
                                    break;
                                case "Song_ReplayGain":
                                    AddSongReplayGainColumn = false;
                                    break;
                                case "Singer_Name":
                                case "Singer_Spell":
                                case "Singer_SpellNum":
                                case "Singer_PenStyle":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "60") UpdateKtvSinger = true;
                                    break;
                                case "PenStyle":
                                    if (row["CHARACTER_MAXIMUM_LENGTH"].ToString() != "40") UpdatePhonetics = true;
                                    break;
                                case "Langauage_KeyWord":
                                    UpdateLangauage = false;
                                    break;
                                case "Song_SongNameFuzzy":
                                case "Song_SingerFuzzy":
                                case "Song_FuzzyVer":
                                case "DLspace":
                                case "Epasswd":
                                case "imgpath":
                                case "cashboxsongid":
                                case "cashboxdat":
                                case "holidaysongid":
                                    RemoveGodLiuColumn = true;
                                    GodLiuColumnlist.Add(row["COLUMN_NAME"].ToString());
                                    break;
                            }
                        }
                    }

                    string UpdateSqlStr = "";
                    if (UpdateKtvSong)
                    {
                        try
                        {
                            UpdateSqlStr = "select * into ktv_Song_temp from ktv_Song";
                            using (OleDbCommand cmd = new OleDbCommand(UpdateSqlStr, conn))
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }

                            UpdateSqlStr = "delete * from ktv_Song";
                            using (OleDbCommand cmd = new OleDbCommand(UpdateSqlStr, conn))
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }

                            List<string> cmdstrlist = new List<string>()
                            {
                                "alter table ktv_Song alter column Song_SongName TEXT(80) WITH COMPRESSION",
                                "alter table ktv_Song alter column Song_Singer TEXT(60) WITH COMPRESSION",
                                "alter table ktv_Song alter column Song_Spell TEXT(80) WITH COMPRESSION",
                                "alter table ktv_Song alter column Song_FileName TEXT(255) WITH COMPRESSION",
                                "alter table ktv_Song alter column Song_SpellNum TEXT(80) WITH COMPRESSION",
                                "alter table ktv_Song alter column Song_PenStyle TEXT(80) WITH COMPRESSION"
                            };

                            foreach (string cmdstr in cmdstrlist)
                            {
                                using (OleDbCommand cmd = new OleDbCommand(cmdstr, conn))
                                {
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }
                            }
                            cmdstrlist.Clear();
                            cmdstrlist = null;

                            UpdateSqlStr = "insert into ktv_Song select * from ktv_Song_temp";
                            using (OleDbCommand cmd = new OleDbCommand(UpdateSqlStr, conn))
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }

                            UpdateSqlStr = "drop table ktv_Song_temp";
                            using (OleDbCommand cmd = new OleDbCommand(UpdateSqlStr, conn))
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                        catch
                        {
                            UpdateError = true;
                            this.BeginInvoke((Action)delegate ()
                            {
                                SongMaintenance_DBVerTooltip_Label.Text = "更新歌曲資料表失敗,已還原為原本的資料庫檔案。";
                            });
                        }
                    }

                    if (UpdateKtvSinger)
                    {
                        List<string> cmdstrlist = new List<string>()
                        {
                            "alter table ktv_Singer alter column Singer_Name TEXT(60) WITH COMPRESSION",
                            "alter table ktv_Singer alter column Singer_Spell TEXT(60) WITH COMPRESSION",
                            "alter table ktv_Singer alter column Singer_SpellNum TEXT(60) WITH COMPRESSION",
                            "alter table ktv_Singer alter column Singer_PenStyle TEXT(60) WITH COMPRESSION"
                        };

                        try
                        {
                            foreach (string cmdstr in cmdstrlist)
                            {
                                using (OleDbCommand cmd = new OleDbCommand(cmdstr, conn))
                                {
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }
                            }
                            cmdstrlist.Clear();
                            cmdstrlist = null;
                        }
                        catch
                        {
                            UpdateError = true;
                            this.BeginInvoke((Action)delegate ()
                            {
                                SongMaintenance_DBVerTooltip_Label.Text = "更新歌手資料表失敗,已還原為原本的資料庫檔案。";
                            });

                        }
                    }

                    if (UpdatePhonetics)
                    {
                        using (OleDbCommand cmd = new OleDbCommand("alter table ktv_Phonetics alter column PenStyle TEXT(40) WITH COMPRESSION", conn))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            catch
                            {
                                UpdateError = true;
                                this.BeginInvoke((Action)delegate ()
                                {
                                    SongMaintenance_DBVerTooltip_Label.Text = "更新拼音資料表失敗,已還原為原本的資料庫檔案。";
                                });
                            }
                        }
                    }

                    if (UpdateLangauage)
                    {
                        using (OleDbCommand cmd = new OleDbCommand("alter table ktv_Langauage add column Langauage_KeyWord TEXT(255) WITH COMPRESSION", conn))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            catch
                            {
                                UpdateError = true;
                                this.BeginInvoke((Action)delegate ()
                                {
                                    SongMaintenance_DBVerTooltip_Label.Text = "更新語系資料表失敗,已還原為原本的資料庫檔案。";
                                });
                            }
                        }
                    }

                    if (AddSongReplayGainColumn)
                    {
                        using (OleDbCommand cmd = new OleDbCommand("alter table ktv_Song add column Song_ReplayGain DOUBLE", conn))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            catch
                            {
                                UpdateError = true;
                                this.BeginInvoke((Action)delegate ()
                                {
                                    SongMaintenance_DBVerTooltip_Label.Text = "加入 Song_ReplayGain 欄位失敗,已還原為原本的資料庫檔案。";
                                });
                            }
                        }
                    }

                    if (RemoveGodLiuColumn)
                    {
                        string cmdstr = string.Empty;
                        foreach (string GodLiuColumn in GodLiuColumnlist)
                        {
                            switch (GodLiuColumn)
                            {
                                case "Song_SongNameFuzzy":
                                    cmdstr = "alter table ktv_Song drop column Song_SongNameFuzzy";
                                    break;
                                case "Song_SingerFuzzy":
                                    cmdstr = "alter table ktv_Song drop column Song_SingerFuzzy";
                                    break;
                                case "Song_FuzzyVer":
                                    cmdstr = "alter table ktv_Song drop column Song_FuzzyVer";
                                    break;
                                case "DLspace":
                                    cmdstr = "alter table ktv_Song drop column DLspace";
                                    break;
                                case "Epasswd":
                                    cmdstr = "alter table ktv_Song drop column Epasswd";
                                    break;
                                case "imgpath":
                                    cmdstr = "alter table ktv_Song drop column imgpath";
                                    break;
                                case "cashboxsongid":
                                    cmdstr = "alter table ktv_Song drop column cashboxsongid";
                                    break;
                                case "cashboxdat":
                                    cmdstr = "alter table ktv_Song drop column cashboxdat";
                                    break;
                                case "holidaysongid":
                                    cmdstr = "alter table ktv_Song drop column holidaysongid";
                                    break;
                            }
                            using (OleDbCommand cmd = new OleDbCommand(cmdstr, conn))
                            {
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }
                                catch
                                {
                                    UpdateError = true;
                                    this.BeginInvoke((Action)delegate ()
                                    {
                                        SongMaintenance_DBVerTooltip_Label.Text = "新增 Song_ReplayGain 欄位失敗,已還原為原本的資料庫檔案。";
                                    });
                                }
                            }
                        }
                    }
                }

                if (UpdateError)
                {
                    File.Copy(SongDBBackupFile, Global.CrazyktvDatabaseFile, true);
                }
                else
                {
                    using (OleDbConnection mgrconn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
                    {
                        using (OleDbCommand cmd = new OleDbCommand("update ktv_Version set SongDB = @SongDB where Id = @Id", mgrconn))
                        {
                            cmd.Parameters.AddWithValue("@SongDB", Global.CrazyktvSongDBVer);
                            cmd.Parameters.AddWithValue("@Id", "1");
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }

                    string VersionQuerySqlStr = "select * from ktv_Version";
                    using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, VersionQuerySqlStr, ""))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                Global.CrazyktvSongDBVer = row["SongDB"].ToString();
                                Global.CashboxUpdDate = DateTime.Parse(row["CashboxUpdDate"].ToString());
                            }
                        }
                    }

                    this.BeginInvoke((Action)delegate ()
                    {
                        Global.TimerEndTime = DateTime.Now;

                        SongMaintenance_DBVer1Value_Label.Text = Global.CrazyktvSongDBVer + " 版";
                        Cashbox_UpdDateValue_Label.Text = (CultureInfo.CurrentCulture.Name == "zh-TW") ? Global.CashboxUpdDate.ToLongDateString() : Global.CashboxUpdDate.ToShortDateString();

                        SongMaintenance_DBVerTooltip_Label.Text = "";
                        SongMaintenance_Tooltip_Label.Text = "已完成歌庫版本更新,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    });
                    SongDBUpdate_UpdateFinish();
                }
            }
        }


        private void SongDBUpdate_UpdateFinish()
        {
            List<bool> list = GetCrazyktvDatabaseStatus();
            bool SongMgrDestFolder = list[3];
            list.Clear();
            list = null;

            if (Global.CrazyktvDatabaseStatus)
            {
                DataTable dt = new DataTable();
                string SongQuerySqlStr = "select Song_Id from ktv_Song";
                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");
                if (dt.Rows.Count > 0)
                {
                    var d5code = from row in dt.AsEnumerable()
                                 where row.Field<string>("Song_Id").Length == 5
                                 select row;

                    var d6code = from row in dt.AsEnumerable()
                                 where row.Field<string>("Song_Id").Length == 6
                                 select row;

                    int MaxDigitCode;
                    if (d5code.Count<DataRow>() > d6code.Count<DataRow>()) { MaxDigitCode = 5; } else { MaxDigitCode = 6; }

                    this.BeginInvoke((Action)delegate()
                    {
                        switch (MaxDigitCode)
                        {
                            case 5:
                                SongMgrCfg_MaxDigitCode_ComboBox.Enabled = false;
                                if (Global.SongMgrMaxDigitCode != "1")
                                {
                                    SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = 1;
                                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode", Global.SongMgrMaxDigitCode);
                                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode", Global.SongMgrLangCode);
                                }
                                break;
                            case 6:
                                SongMgrCfg_MaxDigitCode_ComboBox.Enabled = false;
                                if (Global.SongMgrMaxDigitCode != "2")
                                {
                                    SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = 2;
                                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode", Global.SongMgrMaxDigitCode);
                                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode", Global.SongMgrLangCode);
                                }
                                break;
                        }
                    });

                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("Song_Id").Length != MaxDigitCode
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Global.SongMgrDBVerErrorUIStatus = false;
                            SongMaintenance_CodeConvTo6_Button.Enabled = false;
                            SongMaintenance_CodeCorrect_Button.Enabled = true;
                            Global.CrazyktvDatabaseMaxDigitCode = false;
                            Global.CrazyktvDatabaseStatus = false;
                        });
                    }
                    else
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
                            {
                                Global.SongMgrDBVerErrorUIStatus = true;
                            }
                            else
                            {
                                if (SongMgrDestFolder) { Global.SongMgrDBVerErrorUIStatus = true; } else { Global.SongMgrDBVerErrorUIStatus = false; }
                            }

                            switch (Global.SongMgrMaxDigitCode)
                            {
                                case "1":
                                    SongMaintenance_CodeConvTo6_Button.Enabled = true;
                                    break;
                                case "2":
                                    SongMaintenance_CodeConvTo6_Button.Enabled = false;
                                    break;
                            }
                            SongMaintenance_CodeCorrect_Button.Enabled = false;
                            Global.CrazyktvDatabaseMaxDigitCode = true;
                        });
                    }
                }
                else // 空白資料庫
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
                        {
                            Global.SongMgrDBVerErrorUIStatus = true;
                        }
                        else
                        {
                            if (SongMgrDestFolder) { Global.SongMgrDBVerErrorUIStatus = true; } else { Global.SongMgrDBVerErrorUIStatus = false; }
                        }
                            
                        SongMaintenance_CodeConvTo6_Button.Enabled = false;
                        SongMaintenance_CodeCorrect_Button.Enabled = false;
                        Global.CrazyktvDatabaseMaxDigitCode = true;
                    });
                }
                dt.Dispose();
                dt = null;
            }

            if (Global.CrazyktvDatabaseStatus)
            {
                // 檢查是否有自訂語系
                Common_CheckSongLang();

                // 統計歌曲數量
                Task.Factory.StartNew(() => Common_GetSongStatisticsTask());

                // 統計歌手數量
                Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());

                // 檢查備份移除歌曲是否要刪除
                Task.Factory.StartNew(() => Common_CheckBackupRemoveSongTask());

                // 取得可用歌曲編號
                Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));

                this.BeginInvoke((Action)delegate()
                {
                    // 載入我的最愛清單
                    SongQuery_GetFavoriteUserList();

                    // 歌庫設定 - 載入下拉選單清單及設定
                    SongMgrCfg_SetLangLB();

                    // 歌庫維護 - 載入下拉選單清單及設定
                    SongMaintenance_GetFavoriteUserList();
                    SongMaintenance_SetCustomLangControl();
                });
            }
            Global.DatabaseUpdateFinished = true;
        }

    }
}
