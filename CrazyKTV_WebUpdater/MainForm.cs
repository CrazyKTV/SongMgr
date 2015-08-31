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
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "VersionInfo", "20150831001", "", "版本日期及資訊");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV.exe", "539", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTV.exe", "CrazyKTV 主程式");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_BackGround.exe", "128", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTV_BackGround.exe", "CrazyKTV 建圖程式");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_SongMgr.exe", "132", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTV_SongMgr.exe", "CrazyKTV 加歌程式");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_ConfigTool.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTV_ConfigTool.exe", "CrazyKTV 設定工具");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTVEX.exe", "103", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTVEX.exe", "CrazyKTV 前導工具");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazySong.mdb", "103", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazySong.mdb", "CrazyKTV 空白資料庫");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "README_使用說明.doc", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/README_使用說明.doc", "CrazyKTV 使用說明");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CassiniDev4-lib.dll", "4017", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CassiniDev4-lib.dll", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "DirectShowLib-2005.dll", "210", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/DirectShowLib-2005.dll", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "MediaFoundation.dll", "200", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/MediaFoundation.dll", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Newtonsoft.Json.dll", "350", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Newtonsoft.Json.dll", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "SharpDX.dll", "263", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/SharpDX.dll", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "SharpDX.Direct3D9.dll", "263", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/SharpDX.Direct3D9.dll", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_BackGround.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_BackGround.exe", "背景圖檔資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_BMP.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_BMP.exe", "前導畫面資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Codec.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_Codec.exe", "影音解碼器資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Favorite.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_Favorite.exe", "我的最愛背景圖檔資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Lang.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_Lang.exe", "語系檔資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_SongMgr.exe", "103", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_SongMgr.exe", "加歌程式相關資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Web.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/Folder_Web.exe", "Web 遠端遙控資料夾");
                RebuildFile = true;
            }

            Global.LocaleVerList = new List<List<string>>();
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "VersionInfo"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_BackGround.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_SongMgr.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_ConfigTool.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CrazyKTVEX.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CrazySong.mdb"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "README_使用說明.doc"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "CassiniDev4-lib.dll"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "DirectShowLib-2005.dll"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "MediaFoundation.dll"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Newtonsoft.Json.dll"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "SharpDX.dll"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "SharpDX.Direct3D9.dll"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_BackGround.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_BMP.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_Codec.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_Favorite.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_Lang.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_SongMgr.exe"));
            Global.LocaleVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterFile, "Folder_Web.exe"));

            bool DownloadStatus = DownloadFile(Global.WebUpdaterTempFile, Global.WebUpdaterUrl, false);
            if (DownloadStatus)
            {
                if (File.Exists(Global.WebUpdaterTempFile))
                {
                    Global.RemoteVerList = new List<List<string>>();
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "VersionInfo"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CrazyKTV.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CrazyKTV_BackGround.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CrazyKTV_SongMgr.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CrazyKTV_ConfigTool.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CrazyKTVEX.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CrazySong.mdb"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "README_使用說明.doc"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "CassiniDev4-lib.dll"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "DirectShowLib-2005.dll"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "MediaFoundation.dll"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Newtonsoft.Json.dll"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "SharpDX.dll"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "SharpDX.Direct3D9.dll"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_BackGround.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_BMP.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_Codec.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_Favorite.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_Lang.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_SongMgr.exe"));
                    Global.RemoteVerList.Add(CommonFunc.LoadVersionXmlFile(Global.WebUpdaterTempFile, "Folder_Web.exe"));
                    File.Delete(Global.WebUpdaterTempFile);

                    if (Convert.ToInt64(Global.RemoteVerList[0][1]) > Convert.ToInt64(Global.LocaleVerList[0][1]) || RebuildFile)
                    {
                        if (RebuildFile)
                        {
                            Task.Factory.StartNew(() => UpdateFileTask(RebuildFile));
                        }
                        else
                        {
                            if (MessageBox.Show("你確定要更新檔案嗎?", "偵測到 CrazyKTV 版本更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Task.Factory.StartNew(() => UpdateFileTask(RebuildFile));
                            }
                        }
                    }
                    else
                    {
                        label1.Text = "你的 CrazyKTV 已是最新版本。";
                    }
                }
            }



        }

        private void UpdateFileTask(bool RebuildFile)
        {
            this.BeginInvoke((Action)delegate()
            {
                progressBar2.Maximum = Global.RemoteVerList.Count;
            });

            foreach (List<string> list in Global.RemoteVerList)
            {
                this.BeginInvoke((Action)delegate()
                {
                    label1.Text = "正在檢查更新檔案,請稍待...";
                    progressBar2.Value = Global.RemoteVerList.IndexOf(list) + 1;
                });

                if (Convert.ToInt64(list[1]) > Convert.ToInt64(Global.LocaleVerList[Global.RemoteVerList.IndexOf(list)][1]) || RebuildFile)
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        label1.Text = "正在下載 " + list[0] + " 更新檔案...";
                    });
                    CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, list[0], list[1], list[2], list[3]);
                    if (list[0] != "VersionInfo") DownloadFile(list[0], list[2], true);
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
                    Process p = Process.Start(file, "/S");
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
