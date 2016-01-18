using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_WebUpdater
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            bool RebuildFile = false;
            if (!File.Exists(Global.WebUpdaterFile))
            {
                CommonFunc.CreateVersionXmlFile(Global.WebUpdaterFile);
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "VersionInfo", "20150831001", "", "", "版本日期及資訊");
                RebuildFile = true;
            }
            Global.LocaleVerList = CommonFunc.ScanVersionXmlFile(Global.WebUpdaterFile);

            bool DownloadStatus = DownloadFile(Global.WebUpdaterTempFile, Global.WebUpdaterUrl, false);
            if (DownloadStatus)
            {
                if (File.Exists(Global.WebUpdaterTempFile))
                {
                    Global.RemoteVerList = CommonFunc.ScanVersionXmlFile(Global.WebUpdaterTempFile);
                    File.Delete(Global.WebUpdaterTempFile);

                    if (Convert.ToInt64(Global.RemoteVerList[0][1]) > Convert.ToInt64(Global.LocaleVerList[0][1]) || RebuildFile)
                    {
                        if (RebuildFile)
                        {
                            Task.Factory.StartNew(() => UpdateFileTask());
                        }
                        else
                        {
                            if (MessageBox.Show("你確定要更新檔案嗎?", "偵測到 CrazyKTV 版本更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Task.Factory.StartNew(() => UpdateFileTask());
                            }
                            else
                            {
                                label1.Text = "你的 CrazyKTV 還未更新至最新版本。";
                            }
                        }
                    }
                    else
                    {
                        label1.Text = "你的 CrazyKTV 已是最新版本。";
                    }
                }
            }
            else
            {
                File.Delete(Global.WebUpdaterTempFile);
                label1.Text = "暫時無法取得網路上的更新資料,請稍後再試。";
            }
        }

        private void UpdateFileTask()
        {
            string UnFolderFileArguments = "-y";

            this.BeginInvoke((Action)delegate()
            {
                progressBar2.Maximum = Global.RemoteVerList.Count;
            });

            List<string> LocaleNameList = new List<string>();
            foreach (List<string> list in Global.LocaleVerList)
            {
                LocaleNameList.Add(list[0]);
            }

            foreach (List<string> list in Global.RemoteVerList)
            {
                this.BeginInvoke((Action)delegate()
                {
                    label1.Text = "正在檢查更新檔案,請稍待...";
                    progressBar2.Value = Global.RemoteVerList.IndexOf(list) + 1;
                });

                int LocaleListIndex = LocaleNameList.IndexOf(list[0]);

                if (LocaleListIndex >= 0)
                {
                    if (Convert.ToInt64(list[1]) > Convert.ToInt64(Global.LocaleVerList[LocaleListIndex][1]))
                    {
                        this.BeginInvoke((Action)delegate ()
                        {
                            label1.Text = "正在下載 " + list[0] + " 更新檔案...";
                        });
                        CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, list[0], list[1], list[2], list[3], list[4]);
                        if (list[0] != "VersionInfo")
                        {
                            if (list[0] == "CrazySong.mdb" && File.Exists(Application.StartupPath + @"\CrazySong.mdb"))
                            {
                                list[0] = "CrazySongEmpty.mdb";
                            }
                            else
                            {
                                if (list[3] == "")
                                {
                                    DownloadFile(list[0], list[2], true);
                                }
                                else
                                {
                                    string FilePath = Application.StartupPath + @"\" + list[3];
                                    if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
                                    DownloadFile(FilePath + @"\" + list[0], list[2], true);
                                }
                            }
                        }
                        else
                        {
                            if (list[2] != "") UnFolderFileArguments = list[2];
                        }
                    }
                }
                else
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        label1.Text = "正在下載 " + list[0] + " 更新檔案...";
                    });

                    CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, list[0], list[1], list[2], list[3], list[4]);
                    if (list[0] != "VersionInfo")
                    {
                        if (list[0] == "CrazySong.mdb" && File.Exists(Application.StartupPath + @"\CrazySong.mdb"))
                        {
                            list[0] = "CrazySongEmpty.mdb";
                        }
                        else
                        {
                            if (list[3] == "")
                            {
                                DownloadFile(list[0], list[2], true);
                            }
                            else
                            {
                                string FilePath = Application.StartupPath + @"\" + list[3];
                                if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
                                DownloadFile(FilePath + @"\" + list[0], list[2], true);
                            }
                        }
                    }
                }
            }

            this.BeginInvoke((Action)delegate()
            {
                label1.Text = "正在解壓檔案,請稍待...";
            });

            List<string> FolderFileList = new List<string>()
            {
                Application.StartupPath + @"\Folder_BackGround.exe",
                Application.StartupPath + @"\Folder_BMP.exe",
                Application.StartupPath + @"\Folder_Codec.exe",
                Application.StartupPath + @"\Folder_Favorite.exe",
                Application.StartupPath + @"\Folder_Lang.exe",
                Application.StartupPath + @"\Folder_SongMgr.exe",
                Application.StartupPath + @"\Folder_Web.exe",
            };

            foreach (string file in FolderFileList)
            {
                if (File.Exists(file))
                {
                    Process p = Process.Start(file, UnFolderFileArguments);
                    p.WaitForExit();
                    File.Delete(file);
                }
            }

            this.BeginInvoke((Action)delegate ()
            {
                label1.Text = "已完成檔案更新。";
            });
        }

        private bool DownloadFile(string File, string Url, bool UseProgBar)
        {
            bool DownloadStatus = false;
            FileStream FStream = new FileStream(File, FileMode.Create);

            try
            {
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(Url);
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();

                long FileSize = Response.ContentLength;
                
                if (UseProgBar)
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        progressBar1.Maximum = (int)FileSize;
                    });
                }

                Stream DataStream = Response.GetResponseStream();
                byte[] Databuffer = new byte[8192];
                int CompletedLength = 0;
                long TotalDLByte = 0;

                while ((CompletedLength = DataStream.Read(Databuffer, 0, 8192)) > 0)
                {
                    TotalDLByte += CompletedLength;
                    FStream.Write(Databuffer, 0, CompletedLength);
                    if (UseProgBar)
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            progressBar1.Value = (int)TotalDLByte;
                        });
                    }
                }
                FStream.Close();
                DataStream.Close();
                Response.Close();
                DownloadStatus = true;
            }
            catch
            {
                FStream.Close();
                DownloadStatus = false;
            }
            return DownloadStatus;
        }



    }
}
