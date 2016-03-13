using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void SongDBUpdate_CheckDatabaseFile()
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
            bool Col_Langauage_KeyWord = false;
            bool Col_GodLiu = false;

            // 檢查資料庫表格及欄位
            if (File.Exists(Global.CrazyktvDatabaseFile) && File.Exists(Global.CrazyktvSongMgrDatabaseFile))
            {
                CrazyKTVDatabaseFile = true;
                SongMgrDatabaseFile = true;

                List<string> TableList = new List<string>() { "ktv_Favorite", "ktv_Langauage", "ktv_Phonetics", "ktv_Remote", "ktv_Singer", "ktv_SingerName", "ktv_Song", "ktv_Swan", "ktv_User", "ktv_Words"};
                List<string> CrazyktvDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvDatabaseFile, ""));
                foreach (string TableName in TableList)
                {
                    if (CrazyktvDBTableList.IndexOf(TableName) < 0) Global.CrazyktvDatabaseError = true;
                }

                TableList = new List<string>() { "ktv_AllSinger", "ktv_Cashbox", "ktv_Phonetics", "ktv_SongMgr", "ktv_Version" };
                List<string> SongMgrDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvSongMgrDatabaseFile, ""));
                foreach (string TableName in TableList)
                {
                    if (SongMgrDBTableList.IndexOf(TableName) < 0) Global.SongMgrDatabaseError = true;
                }

                if (!Global.CrazyktvDatabaseError)
                {
                    List<string> ktvLangauageColumnList = new List<string>(CommonFunc.GetOleDbColumnList(Global.CrazyktvDatabaseFile, "", "ktv_Langauage"));

                    if (CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0) TB_ktv_AllSinger = true;
                    if (CrazyktvDBTableList.IndexOf("ktv_Version") >= 0) TB_ktv_Version = true;
                    if (ktvLangauageColumnList.IndexOf("Langauage_KeyWord") >= 0) Col_Langauage_KeyWord = true;

                    List<string> GodLiuColumnList = new List<string>() { "Song_SongNameFuzzy", "Song_SingerFuzzy", "Song_FuzzyVer", "DLspace", "Epasswd", "imgpath", "cashboxsongid", "cashboxdat", "holidaysongid" };
                    List<string> ktvSongColumnList = new List<string>(CommonFunc.GetOleDbColumnList(Global.CrazyktvDatabaseFile, "", "ktv_Song"));

                    foreach (string ColumnName in GodLiuColumnList)
                    {
                        if (ktvSongColumnList.IndexOf(ColumnName) >= 0) Col_GodLiu = true;
                    }

                    if (TB_ktv_AllSinger || TB_ktv_Version || !Col_Langauage_KeyWord || Col_GodLiu) Global.CrazyktvDatabaseIsOld = true;
                }
            }

            // 檢查歌庫資料夾
            if (Directory.Exists(Global.SongMgrDestFolder)) SongMgrDestFolder = true;

            if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
            {
                if (!CrazyKTVDatabaseFile || !SongMgrDatabaseFile || Global.SongMgrDatabaseError || TB_ktv_AllSinger || TB_ktv_Version || !Col_Langauage_KeyWord || Col_GodLiu)
                { Common_SwitchDBVerErrorUI(false); }
                else { Global.CrazyktvDatabaseStatus = true; }
            }
            else
            {
                if (!CrazyKTVDatabaseFile || !SongMgrDatabaseFile || Global.SongMgrDatabaseError || !SongMgrDestFolder || TB_ktv_AllSinger || TB_ktv_Version || !Col_Langauage_KeyWord || Col_GodLiu)
                { Common_SwitchDBVerErrorUI(false); }
                else { Global.CrazyktvDatabaseStatus = true; }
            }

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
            else if (CrazyKTVDatabaseFile && !Global.CrazyktvDatabaseError && !Global.SongMgrDatabaseError && !Col_Langauage_KeyWord)
            {
                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongMaintenance_TabPage);
                SongMaintenance_TabControl.SelectedIndex = SongMaintenance_TabControl.TabPages.IndexOf(SongMaintenance_DBVer_TabPage);
                SongMaintenance_DBVerTooltip_Label.Text = "偵測到資料庫結構更動,開始進行更新...";
                var UpdateDBTask = Task.Factory.StartNew(() => SongDBUpdate_UpdateDatabaseFile("UpdateVersion"));
            }
            else if (CrazyKTVDatabaseFile && !Global.CrazyktvDatabaseError && !Global.SongMgrDatabaseError && TB_ktv_AllSinger)
            {
                MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongMaintenance_TabPage);
                SongMaintenance_TabControl.SelectedIndex = SongMaintenance_TabControl.TabPages.IndexOf(SongMaintenance_DBVer_TabPage);
                SongMaintenance_DBVerTooltip_Label.Text = "偵測到資料庫結構更動,開始進行更新...";
                var UpdateDBTask = Task.Factory.StartNew(() => SongDBUpdate_UpdateDatabaseFile("RemovektvAllSinger"));
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
                        Cashbox_UpdDate_Button.Enabled = Cashbox.GetUpdDateButtonEnableStatus();
                    });

                    if (Global.DBVerEnableDBVerUpdate == "True")
                    {
                        if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Update")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Update");
                        bool DownloadStatus = CommonFunc.DownloadFile(Application.StartupPath + @"\SongMgr\Update\UpdateDB.tmp", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/SongMgr/Update/UpdateDB.xml");
                        if (DownloadStatus)
                        {
                            Global.CrazyktvSongDBVer = CommonFunc.LoadConfigXmlFile(Application.StartupPath + @"\SongMgr\Update\UpdateDB.tmp", "SongDBVer");
                            CommonFunc.SaveConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SongDBVer", Global.CrazyktvSongDBVer);
                        }
                        File.Delete(Application.StartupPath + @"\SongMgr\Update\UpdateDB.tmp");
                    }

                    if (Global.DBVerEnableDBVerUpdate == "True")
                    {
                        if (File.Exists(Global.CrazyktvDatabaseFile))
                        {
                            if (Convert.ToDouble(Global.CrazyktvSongDBVer) > SongDBVer)
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    if (MessageBox.Show("你確定要更新歌庫版本嗎?", "偵測到歌庫版本更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        Common_SwitchDBVerErrorUI(false);
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
                if (!UpdateDBStatus) SongDBUpdate_UpdateFinish();
            }
        }

        private void SongDBUpdate_UpdateDatabaseFile(string UpdateType)
        {
            Global.TimerStartTime = DateTime.Now;
            OleDbConnection conn = new OleDbConnection();
            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand SongDBVerUpdatecmd = new OleDbCommand();
            OleDbCommand GodLiuColumnDropcmd = new OleDbCommand();
            string SongDBBackupFile = "";
            string SongDBVerUpdatecmdSqlStr = "";
            string GodLiuColumnDropSqlStr = "";
            bool UpdateError = false;

            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
            SongDBBackupFile = Application.StartupPath + @"\SongMgr\Backup\" + DateTime.Now.ToLongDateString() + "_CrazySong.mdb";
            File.Copy(Global.CrazyktvDatabaseFile, SongDBBackupFile, true);

            List<string> CrazyktvDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvDatabaseFile, ""));
            
            // 移除 ktv_AllSinger 資料表
            if (CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
            {
                try
                {
                    OleDbCommand[] cmds =
                    {
                        new OleDbCommand("drop table ktv_AllSinger", conn),
                    };

                    foreach (OleDbCommand cmd in cmds)
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    UpdateError = true;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongMaintenance_DBVerTooltip_Label.Text = "移除 ktv_AllSinger 資料表失敗,已還原為原本的資料庫檔案。";
                    });
                }
            }

            // 移除 ktv_Version 資料表
            if (CrazyktvDBTableList.IndexOf("ktv_Version") >= 0)
            {
                try
                {
                    OleDbCommand[] cmds =
                    {
                        new OleDbCommand("drop table ktv_Version", conn),
                    };

                    foreach (OleDbCommand cmd in cmds)
                    {
                        cmd.ExecuteNonQuery();
                    }
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

            if (!UpdateError)
            {
                DataTable dt = new DataTable();

                OleDbCommand[] Scmds =
                {
                    new OleDbCommand("select * from ktv_Song", conn),
                    new OleDbCommand("select * from ktv_Singer", conn),
                    new OleDbCommand("select * from ktv_Phonetics", conn),
                    new OleDbCommand("select * from ktv_Langauage", conn),
                };

                foreach (OleDbCommand cmd in Scmds)
                {
                    OleDbDataReader Reader = cmd.ExecuteReader();
                    if (dt.Rows.Count == 0)
                    {
                        dt = Reader.GetSchemaTable();
                    }
                    else
                    {
                        foreach (DataRow row in Reader.GetSchemaTable().Rows)
                        {
                            dt.ImportRow(row);
                        }
                    }
                    Reader.Close();
                }

                bool UpdateKtvSong = false;
                bool UpdateKtvSinger = false;
                bool UpdatePhonetics = false;
                bool UpdateLangauage = true;
                bool RemoveGodLiuColumn = false;

                List<string> GodLiuColumnlist = new List<string>();

                foreach (DataRow row in dt.AsEnumerable())
                {
                    switch (row["ColumnName"].ToString())
                    {
                        case "Song_SongName":
                            if (row["ColumnSize"].ToString() != "80") UpdateKtvSong = true;
                            break;
                        case "Song_Singer":
                            if (row["ColumnSize"].ToString() != "60") UpdateKtvSong = true;
                            break;
                        case "Song_Spell":
                            if (row["ColumnSize"].ToString() != "80") UpdateKtvSong = true;
                            break;
                        case "Song_FileName":
                            if (row["ColumnSize"].ToString() != "255") UpdateKtvSong = true;
                            break;
                        case "Song_SpellNum":
                            if (row["ColumnSize"].ToString() != "80") UpdateKtvSong = true;
                            break;
                        case "Song_PenStyle":
                            if (row["ColumnSize"].ToString() != "80") UpdateKtvSong = true;
                            break;
                        case "Singer_Name":
                        case "Singer_Spell":
                        case "Singer_SpellNum":
                        case "Singer_PenStyle":
                            if (row["ColumnSize"].ToString() != "60") UpdateKtvSinger = true;
                            break;
                        case "PenStyle":
                            if (row["ColumnSize"].ToString() != "40") UpdatePhonetics = true;
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
                            GodLiuColumnlist.Add(row["ColumnName"].ToString());
                            break;
                    }
                }
                dt.Dispose();
                dt = null;

                string UpdateSqlStr = "";
                OleDbCommand UpdateCmd = new OleDbCommand();

                if (UpdateKtvSong)
                {
                    UpdateSqlStr = "select * into ktv_Song_temp from ktv_Song";
                    UpdateCmd = new OleDbCommand(UpdateSqlStr, conn);
                    UpdateCmd.ExecuteNonQuery();

                    UpdateSqlStr = "delete * from ktv_Song";
                    UpdateCmd = new OleDbCommand(UpdateSqlStr, conn);
                    UpdateCmd.ExecuteNonQuery();

                    OleDbCommand[] Updatecmds =
                    {
                        new OleDbCommand("alter table ktv_Song alter column Song_SongName TEXT(80) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Song alter column Song_Singer TEXT(60) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Song alter column Song_Spell TEXT(80) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Song alter column Song_FileName TEXT(255) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Song alter column Song_SpellNum TEXT(80) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Song alter column Song_PenStyle TEXT(80) WITH COMPRESSION", conn)
                    };

                    try
                    {
                        foreach (OleDbCommand cmd in Updatecmds)
                        {
                            cmd.ExecuteNonQuery();
                        }

                        UpdateSqlStr = "insert into ktv_Song select * from ktv_Song_temp";
                        UpdateCmd = new OleDbCommand(UpdateSqlStr, conn);
                        UpdateCmd.ExecuteNonQuery();

                        UpdateSqlStr = "drop table ktv_Song_temp";
                        UpdateCmd = new OleDbCommand(UpdateSqlStr, conn);
                        UpdateCmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        UpdateError = true;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_DBVerTooltip_Label.Text = "更新歌曲資料表失敗,已還原為原本的資料庫檔案。";
                        });
                    }
                }

                if (UpdateKtvSinger)
                {
                    OleDbCommand[] Updatecmds =
                    {
                        new OleDbCommand("alter table ktv_Singer alter column Singer_Name TEXT(60) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Singer alter column Singer_Spell TEXT(60) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Singer alter column Singer_SpellNum TEXT(60) WITH COMPRESSION", conn),
                        new OleDbCommand("alter table ktv_Singer alter column Singer_PenStyle TEXT(60) WITH COMPRESSION", conn)
                    };

                    try
                    {
                        foreach (OleDbCommand cmd in Updatecmds)
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        UpdateError = true;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_DBVerTooltip_Label.Text = "更新歌手資料表失敗,已還原為原本的資料庫檔案。";
                        });

                    }
                }

                if (UpdatePhonetics)
                {
                    OleDbCommand[] Updatecmds =
                    {
                        new OleDbCommand("alter table ktv_Phonetics alter column PenStyle TEXT(40) WITH COMPRESSION", conn)
                    };

                    try
                    {
                        foreach (OleDbCommand cmd in Updatecmds)
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        UpdateError = true;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_DBVerTooltip_Label.Text = "更新拼音資料表失敗,已還原為原本的資料庫檔案。";
                        });
                    }
                }

                if (UpdateLangauage)
                {
                    OleDbCommand[] Updatecmds =
                    {
                        new OleDbCommand("alter table ktv_Langauage add column Langauage_KeyWord TEXT(255) WITH COMPRESSION", conn)
                    };

                    try
                    {
                        foreach (OleDbCommand cmd in Updatecmds)
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        UpdateError = true;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongMaintenance_DBVerTooltip_Label.Text = "更新語系資料表失敗,已還原為原本的資料庫檔案。";
                        });
                    }
                }

                if (RemoveGodLiuColumn)
                {
                    foreach (string GodLiuColumn in GodLiuColumnlist)
                    {
                        switch (GodLiuColumn)
                        {
                            case "Song_SongNameFuzzy":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column Song_SongNameFuzzy";
                                break;
                            case "Song_SingerFuzzy":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column Song_SingerFuzzy";
                                break;
                            case "Song_FuzzyVer":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column Song_FuzzyVer";
                                break;
                            case "DLspace":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column DLspace";
                                break;
                            case "Epasswd":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column Epasswd";
                                break;
                            case "imgpath":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column imgpath";
                                break;
                            case "cashboxsongid":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column cashboxsongid";
                                break;
                            case "cashboxdat":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column cashboxdat";
                                break;
                            case "holidaysongid":
                                GodLiuColumnDropSqlStr = "alter table ktv_Song drop column holidaysongid";
                                break;
                        }
                        GodLiuColumnDropcmd = new OleDbCommand(GodLiuColumnDropSqlStr, conn);
                        GodLiuColumnDropcmd.ExecuteNonQuery();
                    }
                }
                conn.Close(); 
            }

            if (UpdateError)
            {
                File.Copy(SongDBBackupFile, Global.CrazyktvDatabaseFile, true);
            }
            else
            {
                conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, "");
                SongDBVerUpdatecmdSqlStr = "update ktv_Version set SongDB = @SongDB where Id = @Id";
                SongDBVerUpdatecmd = new OleDbCommand(SongDBVerUpdatecmdSqlStr, conn);
                SongDBVerUpdatecmd.Parameters.AddWithValue("@SongDB", Global.CrazyktvSongDBVer);
                SongDBVerUpdatecmd.Parameters.AddWithValue("@Id", "1");
                SongDBVerUpdatecmd.ExecuteNonQuery();
                SongDBVerUpdatecmd.Parameters.Clear();
                conn.Close();

                CommonFunc.CompactAccessDB("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Global.CrazyktvDatabaseFile + ";", Global.CrazyktvDatabaseFile);

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

                this.BeginInvoke((Action)delegate()
                {
                    Global.TimerEndTime = DateTime.Now;

                    SongMaintenance_DBVer1Value_Label.Text = Global.CrazyktvSongDBVer + " 版";
                    Cashbox_UpdDateValue_Label.Text = (CultureInfo.CurrentCulture.Name == "zh-TW") ? Global.CashboxUpdDate.ToLongDateString() : Global.CashboxUpdDate.ToShortDateString();
                    Cashbox_UpdDate_Button.Enabled = Cashbox.GetUpdDateButtonEnableStatus();

                    SongMaintenance_DBVerTooltip_Label.Text = "";
                    SongMaintenance_Tooltip_Label.Text = "已完成歌庫版本更新,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                });
                SongDBUpdate_UpdateFinish();
            }
        }


        private void SongDBUpdate_UpdateFinish()
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
                    if (CrazyktvDBTableList.IndexOf(TableName) < 0) Global.CrazyktvDatabaseError = true;
                }

                TableList = new List<string>() { "ktv_AllSinger", "ktv_Cashbox", "ktv_Phonetics", "ktv_SongMgr", "ktv_Version" };
                List<string> SongMgrDBTableList = new List<string>(CommonFunc.GetOleDbTableList(Global.CrazyktvSongMgrDatabaseFile, ""));
                foreach (string TableName in TableList)
                {
                    if (SongMgrDBTableList.IndexOf(TableName) < 0) Global.SongMgrDatabaseError = true;
                }

                if (!Global.CrazyktvDatabaseError)
                {
                    List<string> ktvLangauageColumnList = new List<string>(CommonFunc.GetOleDbColumnList(Global.CrazyktvDatabaseFile, "", "ktv_Langauage"));

                    if (CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0) TB_ktv_AllSinger = true;
                    if (CrazyktvDBTableList.IndexOf("ktv_Version") >= 0) TB_ktv_Version = true;
                    if (ktvLangauageColumnList.IndexOf("Langauage_KeyWord") >= 0) Col_Langauage_KeyWord = true;

                    List<string> GodLiuColumnList = new List<string>() { "Song_SongNameFuzzy", "Song_SingerFuzzy", "Song_FuzzyVer", "DLspace", "Epasswd", "imgpath", "cashboxsongid", "cashboxdat", "holidaysongid" };
                    List<string> ktvSongColumnList = new List<string>(CommonFunc.GetOleDbColumnList(Global.CrazyktvDatabaseFile, "", "ktv_Song"));

                    foreach (string ColumnName in GodLiuColumnList)
                    {
                        if (ktvSongColumnList.IndexOf(ColumnName) >= 0) Col_GodLiu = true;
                    }

                    if (TB_ktv_AllSinger || TB_ktv_Version || !Col_Langauage_KeyWord || Col_GodLiu) Global.CrazyktvDatabaseIsOld = true;
                }
            }

            // 檢查歌庫資料夾
            if (Directory.Exists(Global.SongMgrDestFolder)) SongMgrDestFolder = true;

            if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
            {
                if (!CrazyKTVDatabaseFile || !SongMgrDatabaseFile || Global.SongMgrDatabaseError || TB_ktv_AllSinger || TB_ktv_Version || !Col_Langauage_KeyWord || Col_GodLiu)
                { Common_SwitchDBVerErrorUI(false); }
                else { Global.CrazyktvDatabaseStatus = true; }
            }
            else
            {
                if (!CrazyKTVDatabaseFile || !SongMgrDatabaseFile || Global.SongMgrDatabaseError || !SongMgrDestFolder || TB_ktv_AllSinger || TB_ktv_Version || !Col_Langauage_KeyWord || Col_GodLiu)
                { Common_SwitchDBVerErrorUI(false); }
                else { Global.CrazyktvDatabaseStatus = true; }
            }

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
                            Common_SwitchDBVerErrorUI(false);
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
                                Common_SwitchDBVerErrorUI(true);
                            }
                            else
                            {
                                if (SongMgrDestFolder) { Common_SwitchDBVerErrorUI(true); } else { Common_SwitchDBVerErrorUI(false); }
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
                            Common_SwitchDBVerErrorUI(true);
                        }
                        else
                        {
                            if (SongMgrDestFolder) { Common_SwitchDBVerErrorUI(true); } else { Common_SwitchDBVerErrorUI(false); }
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

                // 取得最小歌曲剩餘編號
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
        }

    }
}
