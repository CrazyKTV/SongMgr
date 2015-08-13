using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CrazyKTV_ConfigTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 取得佈景主題檔案清單
            CrazyktvTheme_GetFileList();

            //載入 CrazyKTV 設定
            if (!File.Exists(Global.CrazyktvCfgFile))
            {
                CommonFunc.CreateConfigXmlFile(Global.CrazyktvCfgFile);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "AutoScreen", Global.CrazyktvAutoScreen);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "ScreenDpi", Global.CrazyktvScreenDpi);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "WinState", Global.CrazyktvWinState);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "D3DButton", Global.CrazyktvD3DButton);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "MainScreen", Global.CrazyktvMainScreen);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvCfgFile, "PlayScreen", Global.CrazyktvPlayScreen);
            }

            List<string> list = new List<string>()
            {
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvCfgFile, "AutoScreen"), // 自動偵測點歌台螢幕大小 (AutoScreen)
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvCfgFile, "ScreenDpi"), // 自訂點歌台螢幕大小 (ScreenDpi)
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvCfgFile, "WinState"), // 使用視窗模式啟動點歌台 (WinState)
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvCfgFile, "D3DButton"), // 使用 3D 圖檔按鈕 (D3DButton)
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvCfgFile, "MainScreen"), // 點歌台螢幕 (MainScreen)
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvCfgFile, "PlayScreen") // 播放台螢幕 (PlayScreen)
            };

            if (list[0] != "") Global.CrazyktvAutoScreen = list[0];
            CrazyktvTheme_AutoScreen_CheckBox.Checked = bool.Parse(Global.CrazyktvAutoScreen);

            if (list[1] != "") Global.CrazyktvScreenDpi = list[1];
            CrazyktvTheme_ScreenDpi_ComboBox.DataSource = CrazyktvTheme.GetScreenDpiList();
            CrazyktvTheme_ScreenDpi_ComboBox.DisplayMember = "Display";
            CrazyktvTheme_ScreenDpi_ComboBox.ValueMember = "Value";
            CrazyktvTheme_ScreenDpi_ComboBox.SelectedValue = Global.CrazyktvScreenDpi;

            if (list[2] != "") Global.CrazyktvWinState = list[2];
            CrazyktvTheme_WinState_CheckBox.Checked = bool.Parse(Global.CrazyktvWinState);

            if (list[3] != "") Global.CrazyktvD3DButton = list[3];
            CrazyktvTheme_D3DButton_CheckBox.Checked = bool.Parse(Global.CrazyktvD3DButton);

            if (list[4] != "") Global.CrazyktvMainScreen = list[4];
            if (list[5] != "") Global.CrazyktvPlayScreen = list[5];
            if (Global.CrazyktvMainScreen == "1" & Global.CrazyktvPlayScreen == "0") { CrazyktvTheme_MainScreen_CheckBox.Checked = false; } else { CrazyktvTheme_MainScreen_CheckBox.Checked = true; }
            if (Screen.AllScreens.Length < 2) { CrazyktvTheme_MainScreen_CheckBox.Enabled = false; } else { CrazyktvTheme_MainScreen_CheckBox.Enabled = true; }

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            switch (MainTabControl.SelectedTab.Name)
            {
                case "CrazyktvTheme_TabPage":
                    CrazyktvTheme_ListView.Focus();
                    break;
            }
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MainTabControl.SelectedTab.Name)
            {
                case "CrazyktvTheme_TabPage":
                    CrazyktvTheme_ListView.Focus();
                    break;
            }
        }

    }
}
