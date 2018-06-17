using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void SongMonitor_CheckCurSong()
        {
            if (!Global.CrazyktvDatabaseStatus) return;

            if (Global.SongMgrSongAddMode != "4")
            {
                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_MonitorFolders_TabPage) >= 0)
                {
                    SongMgrCfg_MonitorFolders_TabPage.Hide();
                    SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_MonitorFolders_TabPage);
                }
            }

            if (Global.SongMgrSongAddMode == "4" && Global.SongMgrEnableMonitorFolders == "True")
            {
                Global.TimerStartTime = DateTime.Now;
                DateTime TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };
                Global.MTotalList = new List<int>() { 0, 0, 0, 0 };
                Common_SwitchSetUI(false);

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongMonitor_CheckCurSongTask()));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    DateTime TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,移除 " + Global.MTotalList[0] + " 首,共花費 " + (long)(TimerEndTime - TimerStartTime).TotalSeconds + " 秒完成監視。";
                        SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                        SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;

                        Common_SwitchSetUI(true);
                    });
                });
            }
        }

        private void SongMonitor_CheckCurSongTask()
        {
            SpinWait.SpinUntil(() => Global.InitializeSongData == true);
            this.BeginInvoke((Action)delegate()
            {
                SongQuery_QueryStatus_Label.Text = "正在檢查歌曲資料庫,請稍待...";
                SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
            });

            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_FileName, Song_Path from ktv_Song order by Song_Id";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            List<string> RemoveSongIdList = new List<string>();
            List<string> FileList = new List<string>();

            Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals(langstr)
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        if (!File.Exists(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))))
                        {
                            lock (LockThis)
                            {
                                RemoveSongIdList.Add(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")));
                            }
                        }
                        else
                        {
                            lock (LockThis)
                            {
                                FileList.Add(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")));
                            }
                        }
                    }
                }
            });
            dt.Dispose();
            dt = null;

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string SongRemoveSqlStr = "delete from ktv_Song where Song_Id = @SongId";
            cmd = new OleDbCommand(SongRemoveSqlStr, conn);

            if (RemoveSongIdList.Count > 0)
            {
                foreach (string str in RemoveSongIdList)
                {
                    List<string> list = new List<string>(str.Split('|'));
                    cmd.Parameters.AddWithValue("@SongId", list[0]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    Global.MTotalList[0]++;

                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "正在移除第 " + Global.MTotalList[0] + " 首檔案不存在的歌曲資料,請稍待...";
                        SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                        SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                    });

                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫監視】檔案不存在: " + list[0] + "|" + list[1];
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }
            conn.Close();

            this.BeginInvoke((Action)delegate()
            {
                SongQuery_QueryStatus_Label.Text = "正在檢查歌庫監視目錄,請稍待...";
                SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
            });

            List<string> NewFileList = new List<string>();
            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));

            bool EnableSongMonitor = false;
            Parallel.ForEach(Global.SongMgrMonitorFoldersList, (mpath, loopState) =>
            {
                if (mpath != "")
                {
                    EnableSongMonitor = true;
                    DirectoryInfo dir = new DirectoryInfo(mpath);
                    FileInfo[] Files = dir.GetFiles("*", SearchOption.AllDirectories).Where(p => SupportFormat.Contains(p.Extension.ToLower())).ToArray();
                    foreach (FileInfo f in Files)
                    {
                        if (FileList.IndexOf(f.FullName) < 0)
                        {
                            lock(LockThis)
                            {
                                NewFileList.Add(f.FullName);

                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫監視】偵測到新檔: " + f.FullName;
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                        }
                    }

                    if (!Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].EnableRaisingEvents)
                    {
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Path = mpath;
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].IncludeSubdirectories = true;
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Filter = "*.*";
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].InternalBufferSize = 8192;
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Created += new FileSystemEventHandler(WatcherOnCreated);
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Deleted += new FileSystemEventHandler(WatcherOnDeleted);
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Renamed += new RenamedEventHandler(WatcherOnRenamed);
                        Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].EnableRaisingEvents = true;
                    }
                }
            });

            if (NewFileList.Count > 0)
            {
                SongAdd_SongAnalysisTask(NewFileList);
                while (!SongAnalysis.SongAnalysisCompleted)
                {
                    Thread.Sleep(500);
                }

                if (!SongAnalysis.SongAnalysisError)
                {
                    SongAdd_SongAddTask();
                }
            }
            else
            {
                if (RemoveSongIdList.Count > 0)
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_QueryAddSong(100);
                    });

                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));
                }
            }

            if (EnableSongMonitor)
            {
                Global.SongMonitorDT = new DataTable();
                SongQuerySqlStr = "select Song_Id, Song_Lang, Song_FileName, Song_Path from ktv_Song order by Song_Id";
                Global.SongMonitorDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                this.BeginInvoke((Action)delegate()
                {
                    if (!Global.SongMonitorTimer.Enabled)
                    {
                        Global.SongMonitorTimer.Tick += new EventHandler(SongMonitorTimer_Tick);
                        Global.SongMonitorTimer.Interval = 1000;
                        Global.SongMonitorTimer.Start();
                    }
                });
            }
            RemoveSongIdList.Clear();
            FileList.Clear();
            NewFileList.Clear();
        }

        private void SongMonitor_SwitchSongMonitorWatcher()
        {
            if (!Global.CrazyktvDatabaseStatus) return;

            if (Global.SongMgrSongAddMode == "4" && Global.SongMgrEnableMonitorFolders == "True")
            {
                bool EnableSongMonitor = false;
                Parallel.ForEach(Global.SongMgrMonitorFoldersList, (mpath, loopState) =>
                {
                    if (mpath != "")
                    {
                        EnableSongMonitor = true;
                        if (!Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].EnableRaisingEvents)
                        {
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Path = mpath;
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].IncludeSubdirectories = true;
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Filter = "*.*";
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].InternalBufferSize = 8192;
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Created += new FileSystemEventHandler(WatcherOnCreated);
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Deleted += new FileSystemEventHandler(WatcherOnDeleted);
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].Renamed += new RenamedEventHandler(WatcherOnRenamed);
                            Global.SongMonitorWatcher[Global.SongMgrMonitorFoldersList.IndexOf(mpath)].EnableRaisingEvents = true;
                        }
                    }
                });

                if (EnableSongMonitor)
                {
                    if (Global.SongMonitorDT != null)
                    {
                        Global.SongMonitorDT.Dispose();
                        Global.SongMonitorDT = null;
                    }

                    Global.SongMonitorDT = new DataTable();
                    string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_FileName, Song_Path from ktv_Song order by Song_Id";
                    Global.SongMonitorDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                    if (!Global.SongMonitorTimer.Enabled)
                    {
                        Global.SongMonitorTimer.Tick += new EventHandler(SongMonitorTimer_Tick);
                        Global.SongMonitorTimer.Interval = 1000;
                        Global.SongMonitorTimer.Start();
                    }
                }
            }
            else
            {
                foreach (FileSystemWatcher fwatcher in Global.SongMonitorWatcher)
                {
                    if (fwatcher.EnableRaisingEvents)
                    {
                        fwatcher.Created -= new FileSystemEventHandler(WatcherOnCreated);
                        fwatcher.Deleted -= new FileSystemEventHandler(WatcherOnDeleted);
                        fwatcher.Renamed -= new RenamedEventHandler(WatcherOnRenamed);
                        fwatcher.EnableRaisingEvents = false;
                    }
                }

                if (Global.SongMonitorDT != null)
                {
                    Global.SongMonitorDT.Dispose();
                    Global.SongMonitorDT = null;
                }

                if (Global.SongMonitorTimer.Enabled)
                {
                    Global.SongMonitorTimer.Tick -= new EventHandler(SongMonitorTimer_Tick);
                    Global.SongMonitorTimer.Stop();
                }
            }
        }


        private static void WatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));
            if (SupportFormat.Contains(Path.GetExtension(e.FullPath).ToLower()))
            {
                if (Global.SongMonitorCreatedList.IndexOf(e.FullPath) < 0) Global.SongMonitorCreatedList.Add(e.FullPath);
            }
            else
            {
                if (Directory.Exists(e.FullPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(e.FullPath);
                    FileInfo[] Files = dir.GetFiles("*", SearchOption.AllDirectories).Where(p => SupportFormat.Contains(p.Extension.ToLower())).ToArray();
                    foreach (FileInfo f in Files)
                    {
                        if (Global.SongMonitorCreatedList.IndexOf(f.FullName) < 0) Global.SongMonitorCreatedList.Add(f.FullName);
                    }
                }
            }
            if (Global.SongMonitorCreatedList.Count > 0) Global.SongMonitorSTime = DateTime.Now;
        }


        private static void WatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));
            if (SupportFormat.Contains(Path.GetExtension(e.FullPath).ToLower()))
            {
                var query = from row in Global.SongMonitorDT.AsEnumerable()
                            where row.Field<string>("Song_Path").Equals(Path.GetDirectoryName(e.FullPath) + @"\") &&
                                  row.Field<string>("Song_FileName").Equals(Path.GetFileName(e.FullPath))
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        if (Global.SongMonitorDeletedList.IndexOf(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))) < 0) Global.SongMonitorDeletedList.Add(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")));
                        break;
                    }
                }
            }
            else
            {
                if (Path.GetExtension(e.FullPath) == "")
                {
                    var query = from row in Global.SongMonitorDT.AsEnumerable()
                                where row.Field<string>("Song_Path").Equals(e.FullPath + @"\") ||
                                      row.Field<string>("Song_Path").Contains(e.FullPath + @"\")
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            if (!File.Exists(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))))
                            {
                                if (Global.SongMonitorDeletedList.IndexOf(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))) < 0) Global.SongMonitorDeletedList.Add(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")));
                            }
                        }
                    }
                }
            }
            if (Global.SongMonitorDeletedList.Count > 0) Global.SongMonitorSTime = DateTime.Now;
        }


        private static void WatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));
            if (SupportFormat.Contains(Path.GetExtension(e.OldFullPath).ToLower()))
            {
                var query = from row in Global.SongMonitorDT.AsEnumerable()
                            where row.Field<string>("Song_Path").Equals(Path.GetDirectoryName(e.OldFullPath) + @"\") &&
                                  row.Field<string>("Song_FileName").Equals(Path.GetFileName(e.OldFullPath))
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        if (Global.SongMonitorDeletedList.IndexOf(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))) < 0) Global.SongMonitorDeletedList.Add(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")));
                        break;
                    }
                }
            }
            else
            {
                if (Path.GetExtension(e.OldFullPath) == "")
                {
                    var query = from row in Global.SongMonitorDT.AsEnumerable()
                                where row.Field<string>("Song_Path").Equals(e.OldFullPath + @"\") ||
                                      row.Field<string>("Song_Path").Contains(e.OldFullPath + @"\")
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            if (!File.Exists(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))))
                            {
                                if (Global.SongMonitorDeletedList.IndexOf(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))) < 0) Global.SongMonitorDeletedList.Add(row["Song_Id"].ToString() + "|" + Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")));
                            }
                        }
                    }
                }
            }

            if (SupportFormat.Contains(Path.GetExtension(e.FullPath).ToLower()))
            {
                if (Global.SongMonitorCreatedList.IndexOf(e.FullPath) < 0) Global.SongMonitorCreatedList.Add(e.FullPath);
            }
            else
            {
                if (Directory.Exists(e.FullPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(e.FullPath);
                    FileInfo[] Files = dir.GetFiles("*", SearchOption.AllDirectories).Where(p => SupportFormat.Contains(p.Extension.ToLower())).ToArray();
                    foreach (FileInfo f in Files)
                    {
                        if (Global.SongMonitorCreatedList.IndexOf(f.FullName) < 0) Global.SongMonitorCreatedList.Add(f.FullName);
                    }
                }
            }
            if (Global.SongMonitorDeletedList.Count > 0 || Global.SongMonitorCreatedList.Count > 0) Global.SongMonitorSTime = DateTime.Now;
        }


        private void SongMonitorTimer_Tick(object sender, EventArgs e)
        {
            if (Global.SongMonitorDeletedList.Count > 0 || Global.SongMonitorCreatedList.Count > 0)
            {
                Global.SongMonitorETime = DateTime.Now;
                if ((long)(Global.SongMonitorETime - Global.SongMonitorSTime).TotalSeconds >= 3)
                {
                    Global.SongMonitorTimer.Stop();
                    Global.TimerStartTime = DateTime.Now;
                    DateTime TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0, 0 };
                    Global.MTotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    SongQuery_QueryStatus_Label.Text = "正在同步檔案及資料庫,請稍待...";
                    SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                    SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;

                    var tasks = new List<Task>();

                    tasks.Add(Task.Factory.StartNew(() => SongMonitor_UpdateSongDBTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        DateTime TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_QueryStatus_Label.Text = "總共加入 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,移除 " + Global.MTotalList[0] + " 首,共花費 " + (long)(TimerEndTime - TimerStartTime).TotalSeconds + " 秒完成監視。";
                            SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                            SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;

                            if (Global.SongMonitorDT != null)
                            {
                                Global.SongMonitorDT.Dispose();
                                Global.SongMonitorDT = null;
                            }

                            Global.SongMonitorDT = new DataTable();
                            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_FileName, Song_Path from ktv_Song order by Song_Id";
                            Global.SongMonitorDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                            Common_SwitchSetUI(true);
                            Global.SongMonitorTimer.Start();
                        });
                    });
                }
            }
        }


        private void SongMonitor_UpdateSongDBTask()
        {
            List<int> CountList = new List<int>() { Global.SongMonitorDeletedList.Count, Global.SongMonitorCreatedList.Count };
            if (Global.SongMonitorDeletedList.Count > 0)
            {
                OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                OleDbCommand cmd = new OleDbCommand();
                string SongRemoveSqlStr = "delete from ktv_Song where Song_Id = @SongId";
                cmd = new OleDbCommand(SongRemoveSqlStr, conn);

                foreach (string str in Global.SongMonitorDeletedList)
                {
                    List<string> list = new List<string>(str.Split('|'));
                    cmd.Parameters.AddWithValue("@SongId", list[0]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    Global.MTotalList[0]++;

                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "正在移除第 " + Global.MTotalList[0] + " 首檔案不存在的歌曲資料,請稍待...";
                        SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                        SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                    });

                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫監視】檔案已刪除: " + list[0] + "|" + list[1];
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                Global.SongMonitorDeletedList.Clear();
                conn.Close();
            }

            if (Global.SongMonitorCreatedList.Count > 0)
            {
                SongAdd_SongAnalysisTask(Global.SongMonitorCreatedList);
                while (!SongAnalysis.SongAnalysisCompleted)
                {
                    Thread.Sleep(500);
                }

                if (!SongAnalysis.SongAnalysisError)
                {
                    SongAdd_SongAddTask();
                }

                this.BeginInvoke((Action)delegate()
                {
                    SongQuery_QueryStatus_Label.Text = "正在將變更的歌曲資料寫入操作記錄,請稍待...";
                    SongAdd_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                    SongMgrCfg_Tooltip_Label.Text = SongQuery_QueryStatus_Label.Text;
                });

                foreach (string filepath in Global.SongMonitorCreatedList)
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫監視】偵測到新檔: " + filepath;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                Global.SongMonitorCreatedList.Clear();
            }

            if (CountList[0] > 0 && CountList[1] == 0)
            {
                this.BeginInvoke((Action)delegate()
                {
                    Common_QueryAddSong(100);
                });

                Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));
            }
        }





    }





    class SongMonitor
    {
    }
}
