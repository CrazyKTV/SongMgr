using System;
using System.IO;
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
            // 載入歌庫設定
            if (!File.Exists(Global.WebUpdaterFile))
            {
                CommonFunc.CreateVersionXmlFile(Global.WebUpdaterFile);
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "VersionInfo", "20150831001", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm.cs", "版本日期及資訊");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV.exe", "539", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm.cs", "CrazyKTV 主程式");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_BackGround.exe", "128", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm.cs", "CrazyKTV 建圖程式");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_SongMgr.exe", "133", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm.cs", "CrazyKTV 加歌程式");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTV_ConfigTool.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 設定工具");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazyKTVEX.exe", "103", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 前導工具");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CrazySong.mdb", "103", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 空白資料庫");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "README_使用說明.doc", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 使用說明");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "CassiniDev4-lib.dll", "4017", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "DirectShowLib-2005.dll", "210", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "MediaFoundation.dll", "200", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Newtonsoft.Json.dll", "350", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "SharpDX.dll", "263", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "SharpDX.Direct3D9.dll", "263", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "CrazyKTV 相關組件");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_BackGround.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "背景圖檔資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_BMP.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "前導畫面資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Codec.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "影音解碼器資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Favorite.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "我的最愛背景圖檔資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Lang.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "語系檔資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_SongMgr.exe", "103", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "加歌程式相關資料夾");
                CommonFunc.SaveVersionXmlFile(Global.WebUpdaterFile, "Folder_Web.exe", "100", "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_SongMgr/MainForm2.cs", "Web 遠端遙控資料夾");
                
            }


        }


    }
}
