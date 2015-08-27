using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SongQuery_DataGridView.MakeDoubleBuffered(true);
            SongAdd_DataGridView.MakeDoubleBuffered(true);
            SingerMgr_DataGridView.MakeDoubleBuffered(true);
        }

        
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 歌庫版本資訊
            if (!File.Exists(Global.CrazyktvSongDBUpdateFile))
            {
                if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Update")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Update");
                CommonFunc.CreateConfigXmlFile(Global.CrazyktvSongDBUpdateFile);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SongDBVer", Global.CrazyktvSongDBVer);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SingerDBVer", Global.CrazyktvSingerDBVer);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "PhoneticsDBVer", Global.CrazyktvPhoneticsDBVer);
            }

            // 載入歌庫設定
            if (!File.Exists(Global.SongMgrCfgFile))
            {
                CommonFunc.CreateConfigXmlFile(Global.SongMgrCfgFile);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile", Global.CrazyktvDatabaseFile);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSupportFormat", Global.SongMgrSupportFormat);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder", Global.SongMgrDestFolder);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongAddMode", Global.SongMgrSongAddMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusMerge", Global.SongMgrChorusMerge);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode", Global.SongMgrMaxDigitCode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode", Global.SongMgrLangCode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType", Global.SongMgrSongType);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongInfoSeparate", Global.SongMgrSongInfoSeparate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusSeparate", Global.SongMgrChorusSeparate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongLang", Global.SongAddDefaultSongLang);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSingerType", Global.SongAddDefaultSingerType);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongTrack", Global.SongAddDefaultSongTrack);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongType", Global.SongAddDefaultSongType);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongVolume", Global.SongAddDefaultSongVolume);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr", Global.SongAddSpecialStr);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFolderStructure", Global.SongMgrFolderStructure);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFileStructure", Global.SongMgrFileStructure);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongTrackMode", Global.SongMgrSongTrackMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode", Global.SongAddSongIdentificationMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode", Global.SongAddDupSongMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangStr", string.Join(",", Global.CrazyktvSongLangList));
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord", string.Join("|", Global.CrazyktvSongLangKeyWordList));
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrBackupRemoveSong", Global.SongMgrBackupRemoveSong);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat", Global.SongAddEngSongNameFormat);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgAlwaysOnTop", Global.MainCfgAlwaysOnTop);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongDBConverterTabPage", Global.MainCfgHideSongDBConverterTabPage);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongAddResultTabPage", Global.MainCfgHideSongAddResultTabPage);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongLogTabPage", Global.MainCfgHideSongLogTabPage);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupRemoveSongDays", Global.MainCfgBackupRemoveSongDays);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "DBVerEnableDBVerUpdate", Global.DBVerEnableDBVerUpdate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "DBVerRebuildSingerData", Global.DBVerRebuildSingerData);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceEnableMultiSongPath", Global.SongMaintenanceEnableMultiSongPath);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceMultiSongPath", string.Join(",", Global.SongMaintenanceMultiSongPathList));
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddUseCustomSongID", Global.SongAddUseCustomSongID);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrCustomSingerTypeStructure", Global.SongMgrCustomSingerTypeStructure);
            }

            List<string> list = new List<string>()
            { 
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSupportFormat"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongAddMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusMerge"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongInfoSeparate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusSeparate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongLang"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSingerType"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongTrack"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongType"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongVolume"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFolderStructure"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFileStructure"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongTrackMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangStr"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrBackupRemoveSong"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgAlwaysOnTop"),
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SongDBVer"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongDBConverterTabPage"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongAddResultTabPage"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongLogTabPage"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupRemoveSongDays"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SingerMgrDefaultSingerDataTable"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "DBVerEnableDBVerUpdate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "DBVerRebuildSingerData"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceEnableMultiSongPath"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceMultiSongPath"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddUseCustomSongID"),
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SingerDBVer"),
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "PhoneticsDBVer"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrCustomSingerTypeStructure")
            };

            foreach (TabPage MainTabPage in MainTabControl.TabPages)
            {
                MainTabPage.Show();
            }
            
            if (list[21] != "") Global.CrazyktvSongLangList = new List<string>(list[21].Split(','));
            if (Global.CrazyktvSongLangList.Count < 10) Global.CrazyktvSongLangList = new List<string>() { "國語", "台語", "粵語", "日語", "英語", "客語", "原住民語", "韓語", "兒歌", "其它" };

            if (list[0] != "") Global.CrazyktvDatabaseFile = list[0];
            SongMgrCfg_DBFile_TextBox.Text = Global.CrazyktvDatabaseFile;

            if (list[1] != "") Global.SongMgrSupportFormat = list[1];
            SongMgrCfg_SupportFormat_TextBox.Text = Global.SongMgrSupportFormat;

            if (list[2] != "") Global.SongMgrDestFolder = list[2];
            SongMgrCfg_DestFolder_TextBox.Text = Global.SongMgrDestFolder;
            
            if (list[3] != "") Global.SongMgrSongAddMode = list[3];
            SongMgrCfg_SongAddMode_ComboBox.DataSource = SongMgrCfg.GetSongAddModeList();
            SongMgrCfg_SongAddMode_ComboBox.DisplayMember = "Display";
            SongMgrCfg_SongAddMode_ComboBox.ValueMember = "Value";
            SongMgrCfg_SongAddMode_ComboBox.SelectedValue = int.Parse(Global.SongMgrSongAddMode);

            if (list[4] != "") Global.SongMgrChorusMerge = list[4];
            SongMgrCfg_CrtchorusMerge_CheckBox.Checked = bool.Parse(Global.SongMgrChorusMerge);

            if (list[5] != "") Global.SongMgrMaxDigitCode = list[5];
            SongMgrCfg_MaxDigitCode_ComboBox.DataSource = SongMgrCfg.GetMaxDigitCodeList();
            SongMgrCfg_MaxDigitCode_ComboBox.DisplayMember = "Display";
            SongMgrCfg_MaxDigitCode_ComboBox.ValueMember = "Value";
            SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = int.Parse(Global.SongMgrMaxDigitCode);

            if (list[6] != "") { Global.SongMgrLangCode = list[6]; SongMgrCfg_LoadSongMgrLangCode(); } else { SongMgrCfg_RefreshSongMgrLangCode(); }

            if (list[7] != "") Global.SongMgrSongType = list[7];
            SongMgrCfg_SongType_ListBox.DataSource = SongMgrCfg.GetSongTypeList();
            SongMgrCfg_SongType_ListBox.DisplayMember = "Display";
            SongMgrCfg_SongType_ListBox.ValueMember = "Value";

            if (list[8] != "") Global.SongMgrSongInfoSeparate = list[8];
            SongMgrCfg_SongInfoSeparate_ComboBox.DataSource = SongMgrCfg.GetSongInfoSeparateList();
            SongMgrCfg_SongInfoSeparate_ComboBox.DisplayMember = "Display";
            SongMgrCfg_SongInfoSeparate_ComboBox.ValueMember = "Value";
            SongMgrCfg_SongInfoSeparate_ComboBox.SelectedValue = int.Parse(Global.SongMgrSongInfoSeparate);

            if (list[9] != "") Global.SongMgrChorusSeparate = list[9];
            SongMgrCfg_CrtchorusSeparate_ComboBox.DataSource = SongMgrCfg.GetCrtchorusSeparateList();
            SongMgrCfg_CrtchorusSeparate_ComboBox.DisplayMember = "Display";
            SongMgrCfg_CrtchorusSeparate_ComboBox.ValueMember = "Value";
            SongMgrCfg_CrtchorusSeparate_ComboBox.SelectedValue = int.Parse(Global.SongMgrChorusSeparate);

            if (list[10] != "") Global.SongAddDefaultSongLang = list[10];
            SongAdd_DefaultSongLang_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongLang");
            SongAdd_DefaultSongLang_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongLang_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongLang_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongLang);

            if (list[11] != "") Global.SongAddDefaultSingerType = list[11];
            SongAdd_DefaultSingerType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSingerType");
            SongAdd_DefaultSingerType_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSingerType_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSingerType_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSingerType);

            if (list[12] != "") Global.SongAddDefaultSongTrack = list[12];
            SongAdd_DefaultSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack");
            SongAdd_DefaultSongTrack_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongTrack_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongTrack_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongTrack);

            if (list[13] != "") Global.SongAddDefaultSongType = list[13];
            SongAdd_DefaultSongType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongType");
            SongAdd_DefaultSongType_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongType_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongType_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongType);

            if (list[14] != "") Global.SongAddDefaultSongVolume = list[14];
            SongAdd_DefaultSongVolume_TextBox.Text = Global.SongAddDefaultSongVolume;

            if (list[15] != "") Global.SongAddSpecialStr = list[15];
            SongAdd_SpecialStr_ListBox.DataSource = SongAdd.GetDefaultSongInfo("SpecialStr");
            SongAdd_SpecialStr_ListBox.DisplayMember = "Display";
            SongAdd_SpecialStr_ListBox.ValueMember = "Value";

            if (list[16] != "") Global.SongMgrFolderStructure = list[16];
            SongMgrCfg_FolderStructure_ComboBox.DataSource = SongMgrCfg.GetFolderStructureList();
            SongMgrCfg_FolderStructure_ComboBox.DisplayMember = "Display";
            SongMgrCfg_FolderStructure_ComboBox.ValueMember = "Value";
            SongMgrCfg_FolderStructure_ComboBox.SelectedValue = Global.SongMgrFolderStructure;

            if (list[17] != "") Global.SongMgrFileStructure = list[17];
            SongMgrCfg_FileStructure_ComboBox.DataSource = SongMgrCfg.GetFileStructureList();
            SongMgrCfg_FileStructure_ComboBox.DisplayMember = "Display";
            SongMgrCfg_FileStructure_ComboBox.ValueMember = "Value";
            SongMgrCfg_FileStructure_ComboBox.SelectedValue = Global.SongMgrFileStructure;

            if (list[18] != "") Global.SongMgrSongTrackMode = list[18];
            SongMgrCfg_SongTrackMode_CheckBox.Checked = bool.Parse(Global.SongMgrSongTrackMode);

            if (list[19] != "") Global.SongAddSongIdentificationMode = list[19];
            SongAdd_SongIdentificationMode_ComboBox.DataSource = SongAdd.GetSongIdentificationModeList();
            SongAdd_SongIdentificationMode_ComboBox.DisplayMember = "Display";
            SongAdd_SongIdentificationMode_ComboBox.ValueMember = "Value";
            SongAdd_SongIdentificationMode_ComboBox.SelectedValue = Global.SongAddSongIdentificationMode;

            if (list[20] != "") Global.SongAddDupSongMode = list[20];
            SongAdd_DupSongMode_ComboBox.DataSource = SongAdd.GetDupSongModeList();
            SongAdd_DupSongMode_ComboBox.DisplayMember = "Display";
            SongAdd_DupSongMode_ComboBox.ValueMember = "Value";
            SongAdd_DupSongMode_ComboBox.SelectedValue = Global.SongAddDupSongMode;

            if (list[22] != "")
            {
                Global.CrazyktvSongLangKeyWordList = new List<string>(list[22].Split('|'));
            }
            else
            {
                string str = CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangIDStr");
                Global.CrazyktvSongLangKeyWordList = new List<string>(str.Split('*'));
                if (Global.CrazyktvSongLangKeyWordList.Count < 10)
                {
                    Global.CrazyktvSongLangKeyWordList = new List<string>() { "國語,國", "台語,台,閩南,閩", "粵語,粵,廣東", "日語,日文,日", "英語,英文,英", "客語,客", "原住民語,民謠", "韓語,韓", "兒歌,兒", "其它" };
                }
                else
                {
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord", string.Join("|", Global.CrazyktvSongLangKeyWordList));
                    CommonFunc.RemoveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangIDStr");
                }
            }

            if (list[23] != "") Global.SongMgrBackupRemoveSong = list[23];
            SongMgrCfg_BackupRemoveSong_CheckBox.Checked = bool.Parse(Global.SongMgrBackupRemoveSong);

            if (list[24] != "") Global.SongAddEngSongNameFormat = list[24];
            SongAdd_EngSongNameFormat_CheckBox.Checked = bool.Parse(Global.SongAddEngSongNameFormat);

            if (list[25] != "") Global.MainCfgAlwaysOnTop = list[25];
            MainCfg_AlwaysOnTop_CheckBox.Checked = bool.Parse(Global.MainCfgAlwaysOnTop);

            if (list[26] != "") Global.CrazyktvSongDBVer = list[26];

            if (list[27] != "") Global.MainCfgHideSongDBConverterTabPage = list[27];
            MainCfg_HideSongDBConvTab_CheckBox.Checked = bool.Parse(Global.MainCfgHideSongDBConverterTabPage);

            if (list[28] != "") Global.MainCfgHideSongAddResultTabPage = list[28];
            MainCfg_HideSongAddResultTab_CheckBox.Checked = bool.Parse(Global.MainCfgHideSongAddResultTabPage);

            if (list[29] != "") Global.MainCfgHideSongLogTabPage = list[29];
            MainCfg_HideSongLogTab_CheckBox.Checked = bool.Parse(Global.MainCfgHideSongLogTabPage);

            if (list[30] != "") Global.MainCfgBackupRemoveSongDays = list[30];
            MainCfg_BackupRemoveSongDays_ComboBox.DataSource = MainCfg.GetBackupRemoveSongDaysList();
            MainCfg_BackupRemoveSongDays_ComboBox.DisplayMember = "Display";
            MainCfg_BackupRemoveSongDays_ComboBox.ValueMember = "Value";
            MainCfg_BackupRemoveSongDays_ComboBox.SelectedValue = Global.MainCfgBackupRemoveSongDays;

            if (list[31] != "") Global.SingerMgrDefaultSingerDataTable = list[31];
            SingerMgr_DefaultSingerDataTable_ComboBox.DataSource = SingerMgr.GetDefaultSingerDataTableList();
            SingerMgr_DefaultSingerDataTable_ComboBox.DisplayMember = "Display";
            SingerMgr_DefaultSingerDataTable_ComboBox.ValueMember = "Value";
            SingerMgr_DefaultSingerDataTable_ComboBox.SelectedValue = (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 2;

            if (list[32] != "") Global.DBVerEnableDBVerUpdate = list[32];
            SongMaintenance_EnableDBVerUpdate_CheckBox.Checked = bool.Parse(Global.DBVerEnableDBVerUpdate);

            if (list[33] != "") Global.DBVerRebuildSingerData = list[33];
            SongMaintenance_EnableRebuildSingerData_CheckBox.Checked = bool.Parse(Global.DBVerRebuildSingerData);

            if (list[34] != "") Global.SongMaintenanceEnableMultiSongPath = list[34];
            SongMaintenance_EnableMultiSongPath_CheckBox.Checked = bool.Parse(Global.SongMaintenanceEnableMultiSongPath);
            SongMaintenance_MultiSongPath_ListBox.Enabled = bool.Parse(Global.SongMaintenanceEnableMultiSongPath);
            SongMaintenance_MultiSongPath_Button.Enabled = bool.Parse(Global.SongMaintenanceEnableMultiSongPath);

            if (list[35] != "") Global.SongMaintenanceMultiSongPathList = new List<string>(list[35].Split(','));
            SongMaintenance_MultiSongPath_ListBox.DataSource = SongMaintenance.GetMultiSongPathList();
            SongMaintenance_MultiSongPath_ListBox.DisplayMember = "Display";
            SongMaintenance_MultiSongPath_ListBox.ValueMember = "Value";

            if (list[36] != "") Global.SongAddUseCustomSongID = list[36];
            SongAdd_UseCustomSongID_CheckBox.Checked = bool.Parse(Global.SongAddUseCustomSongID);

            if (list[37] != "") Global.CrazyktvSingerDBVer = list[37];

            if (list[38] != "") Global.CrazyktvPhoneticsDBVer = list[38];

            if (list[39] != "") Global.SongMgrCustomSingerTypeStructure = list[39];
            SongMgrCfg_SetCustomSingerTypeStructureCbox();



            // 建立歌曲操作記錄資料表
            Global.SongLogDT = new DataTable();
            Global.SongLogDT.Columns.Add(new DataColumn("Display", typeof(string)));
            Global.SongLogDT.Columns.Add(new DataColumn("Value", typeof(int)));

            // 檢查資料庫檔案是否為舊版資料庫
            SongDBUpdate_CheckDatabaseFile();

            // 歌曲查詢 - 載入下拉選單清單及設定
            SongQuery_QueryType_ComboBox.DataSource = SongQuery.GetSongQueryTypeList();
            SongQuery_QueryType_ComboBox.DisplayMember = "Display";
            SongQuery_QueryType_ComboBox.ValueMember = "Value";
            SongQuery_QueryType_ComboBox.SelectedValue = 1;

            SongQuery_QueryFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList();
            SongQuery_QueryFilter_ComboBox.DisplayMember = "Display";
            SongQuery_QueryFilter_ComboBox.ValueMember = "Value";
            SongQuery_QueryFilter_ComboBox.SelectedValue = 1;

            SongQuery_FuzzyQuery_CheckBox.Checked = bool.Parse(Global.SongQueryFuzzyQuery);
            SongQuery_SynonymousQuery_CheckBox.Checked = Global.SongQuerySynonymousQuery;

            SongQuery_ExceptionalQuery_ComboBox.DataSource = SongQuery.GetSongQueryExceptionalList();
            SongQuery_ExceptionalQuery_ComboBox.DisplayMember = "Display";
            SongQuery_ExceptionalQuery_ComboBox.ValueMember = "Value";
            SongQuery_ExceptionalQuery_ComboBox.SelectedValue = 1;

            // 歌手管理 - 載入下拉選單清單及設定
            SingerMgr_QueryType_ComboBox.DataSource = SingerMgr.GetSingerTypeList();
            SingerMgr_QueryType_ComboBox.DisplayMember = "Display";
            SingerMgr_QueryType_ComboBox.ValueMember = "Value";
            SingerMgr_QueryType_ComboBox.SelectedValue = 1;
            SingerMgr_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;

            SingerMgr_SingerAddType_ComboBox.DataSource = SingerMgr.GetSingerTypeList();
            SingerMgr_SingerAddType_ComboBox.DisplayMember = "Display";
            SingerMgr_SingerAddType_ComboBox.ValueMember = "Value";
            SingerMgr_SingerAddType_ComboBox.SelectedValue = 1;
            SingerMgr_SingerAddName_TextBox.ImeMode = ImeMode.OnHalf;

            // 歌庫轉換 - 載入下拉選單清單
            SongDBConverter_SrcDBType_ComboBox.DataSource = SongDBConverter.GetSrcDBTypeList();
            SongDBConverter_SrcDBType_ComboBox.DisplayMember = "Display";
            SongDBConverter_SrcDBType_ComboBox.ValueMember = "Value";
            SongDBConverter_SrcDBType_ComboBox.SelectedValue = 1;

            // 歌庫轉換 - 載入說明
            SongDBConverter_SetRtfText("來源資料庫: ", "請選擇你要轉換的點歌軟體歌曲資料庫檔案。" + Environment.NewLine);
            SongDBConverter_SetRtfText("資料庫類型: ", "請選擇你要轉換的點歌軟體類型。" + Environment.NewLine);
            SongDBConverter_SetRtfText("目的資料庫: ", "請選擇本工具所附的 CrazySongEmpty.mdb 空白資料庫檔案。" + Environment.NewLine);
            SongDBConverter_SetRtfText("重建資料夾: ", "當你完成歌庫轉換後,此功能會開啟,請選擇一個空白的資料夾。" + Environment.NewLine + "　　　　　  重建功能會依你在歌庫設定頁面裡的歌庫結構設定來重建所有檔案。" + Environment.NewLine);
            SongDBConverter_SetRtfText("重建方式:　 ", "可選擇使用複製或搬移檔案的方式來重建所有歌庫檔案。" + Environment.NewLine + "　　　　　  在同一磁碟代號使用搬移檔案的方式來重建歌庫檔案可加快重建速度。");

            MainTabControl_SelectedIndexChanged(new TabControl(), new EventArgs());

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(Global.CrazyktvDatabaseFile))
            {
                CommonFunc.CompactAccessDB("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Global.CrazyktvDatabaseFile + ";", Global.CrazyktvDatabaseFile);
            }
        }

        // 頁籤切換處理
        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MainTabControl.SelectedTab.Name)
            {
                case "SongQuery_TabPage":
                    SongQuery_QueryValue_TextBox.Focus();
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;

                    if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0 | Global.CrazyktvDatabaseMaxDigitCode == "Error" | !File.Exists(Global.CrazyktvDatabaseFile) | !Directory.Exists(Global.SongMgrDestFolder))
                    {
                        if (!File.Exists(Global.CrazyktvDatabaseFile)) SongQuery_QueryStatus_Label.Text = "資料庫檔案不存在!";
                        else if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0) SongQuery_QueryStatus_Label.Text = "資料庫檔案為舊版本!";
                        else if (Global.CrazyktvDatabaseMaxDigitCode == "Error") SongQuery_QueryStatus_Label.Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                        else if (!Directory.Exists(Global.SongMgrDestFolder)) SongQuery_QueryStatus_Label.Text = "歌庫資料夾不存在!";
                    }
                    else
                    {
                        if (SongQuery_QueryStatus_Label.Text == "資料庫檔案為舊版本!") SongQuery_QueryStatus_Label.Text = "";
                        else if (SongQuery_QueryStatus_Label.Text == "歌庫編碼混雜 5 及 6 位數編碼!") SongQuery_QueryStatus_Label.Text = "";
                        else if (SongQuery_QueryStatus_Label.Text == "資料庫檔案不存在!") SongQuery_QueryStatus_Label.Text = "";
                        else if (SongQuery_QueryStatus_Label.Text == "歌庫資料夾不存在!") SongQuery_QueryStatus_Label.Text = "";
                    }
                    break;
                case "SongAdd_TabPage":
                    if (Global.RemainingSongID < 100)
                    {
                        SongAdd_Tooltip_Label.Text = "注意: 已有語系的歌曲編號僅剩 " + Global.RemainingSongID + " 個編號可用!";
                    }

                    if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0 | Global.CrazyktvDatabaseMaxDigitCode == "Error" | !File.Exists(Global.CrazyktvDatabaseFile) | !Directory.Exists(Global.SongMgrDestFolder))
                    {
                        if (!File.Exists(Global.CrazyktvDatabaseFile)) SongAdd_Tooltip_Label.Text = "資料庫檔案不存在!";
                        else if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0) SongAdd_Tooltip_Label.Text = "資料庫檔案為舊版本!";
                        else if (Global.CrazyktvDatabaseMaxDigitCode == "Error") SongAdd_Tooltip_Label.Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                        else if (!Directory.Exists(Global.SongMgrDestFolder)) SongAdd_Tooltip_Label.Text = "歌庫資料夾不存在!";
                    }
                    else
                    {
                        if (SongAdd_Tooltip_Label.Text == "資料庫檔案為舊版本!") SongAdd_Tooltip_Label.Text = "";
                        else if (SongAdd_Tooltip_Label.Text == "歌庫編碼混雜 5 及 6 位數編碼!") SongAdd_Tooltip_Label.Text = "";
                        else if (SongAdd_Tooltip_Label.Text == "資料庫檔案不存在!") SongAdd_Tooltip_Label.Text = "";
                        else if (SongAdd_Tooltip_Label.Text == "歌庫資料夾不存在!") SongAdd_Tooltip_Label.Text = "";
                    }
                    break;
                case "SingerMgr_TabPage":
                    if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0 | Global.CrazyktvDatabaseMaxDigitCode == "Error" | !File.Exists(Global.CrazyktvDatabaseFile) | !Directory.Exists(Global.SongMgrDestFolder))
                    {
                        if (!File.Exists(Global.CrazyktvDatabaseFile)) SingerMgr_Tooltip_Label.Text = "資料庫檔案不存在!";
                        else if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0) SingerMgr_Tooltip_Label.Text = "資料庫檔案為舊版本!";
                        else if (Global.CrazyktvDatabaseMaxDigitCode == "Error") SingerMgr_Tooltip_Label.Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                        else if (!Directory.Exists(Global.SongMgrDestFolder)) SingerMgr_Tooltip_Label.Text = "歌庫資料夾不存在!";
                    }
                    else
                    {
                        if (SingerMgr_Tooltip_Label.Text == "資料庫檔案為舊版本!") SingerMgr_Tooltip_Label.Text = "";
                        else if (SingerMgr_Tooltip_Label.Text == "歌庫編碼混雜 5 及 6 位數編碼!") SingerMgr_Tooltip_Label.Text = "";
                        else if (SingerMgr_Tooltip_Label.Text == "資料庫檔案不存在!") SingerMgr_Tooltip_Label.Text = "";
                        else if (SingerMgr_Tooltip_Label.Text == "歌庫資料夾不存在!") SingerMgr_Tooltip_Label.Text = "";
                    }
                    break;
                case "SongMgrCfg_TabPage":
                    if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0 | Global.CrazyktvDatabaseMaxDigitCode == "Error" | !File.Exists(Global.CrazyktvDatabaseFile) | !Directory.Exists(Global.SongMgrDestFolder))
                    {
                        if (!File.Exists(Global.CrazyktvDatabaseFile)) SongMgrCfg_Tooltip_Label.Text = "資料庫檔案不存在!";
                        else if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0) SongMgrCfg_Tooltip_Label.Text = "資料庫檔案為舊版本!";
                        else if (Global.CrazyktvDatabaseMaxDigitCode == "Error") SongMgrCfg_Tooltip_Label.Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                        else if (!Directory.Exists(Global.SongMgrDestFolder)) SongMgrCfg_Tooltip_Label.Text = "歌庫資料夾不存在!";
                    }
                    else
                    {
                        if (SongMgrCfg_Tooltip_Label.Text == "資料庫檔案為舊版本!") SongMgrCfg_Tooltip_Label.Text = "";
                        else if (SongMgrCfg_Tooltip_Label.Text == "歌庫編碼混雜 5 及 6 位數編碼!") SongMgrCfg_Tooltip_Label.Text = "";
                        else if (SongMgrCfg_Tooltip_Label.Text == "資料庫檔案不存在!") SongMgrCfg_Tooltip_Label.Text = "";
                        else if (SongMgrCfg_Tooltip_Label.Text == "歌庫資料夾不存在!") SongMgrCfg_Tooltip_Label.Text = "";
                    }
                    break;
                case "SongMaintenance_TabPage":
                    if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0 | Global.CrazyktvDatabaseMaxDigitCode == "Error" | !File.Exists(Global.CrazyktvDatabaseFile) | !Directory.Exists(Global.SongMgrDestFolder))
                    {
                        if (!File.Exists(Global.CrazyktvDatabaseFile)) SongMaintenance_Tooltip_Label.Text = "資料庫檔案不存在!";
                        else if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0) SongMaintenance_Tooltip_Label.Text = "資料庫檔案為舊版本!";
                        else if (Global.CrazyktvDatabaseMaxDigitCode == "Error") SongMaintenance_Tooltip_Label.Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                        else if (!Directory.Exists(Global.SongMgrDestFolder)) SongMaintenance_Tooltip_Label.Text = "歌庫資料夾不存在!";
                    }
                    else
                    {
                        if (SongMaintenance_Tooltip_Label.Text == "資料庫檔案為舊版本!") SongMaintenance_Tooltip_Label.Text = "";
                        else if (SongMaintenance_Tooltip_Label.Text == "歌庫編碼混雜 5 及 6 位數編碼!") SongMaintenance_Tooltip_Label.Text = "";
                        else if (SongMaintenance_Tooltip_Label.Text == "資料庫檔案不存在!") SongMaintenance_Tooltip_Label.Text = "";
                        else if (SongMaintenance_Tooltip_Label.Text == "歌庫資料夾不存在!") SongMaintenance_Tooltip_Label.Text = "";
                    }

                    switch (SongMaintenance_TabControl.SelectedTab.Name)
                    {
                        case "SongMaintenance_CustomLang_TabPage":
                        case "SongMaintenance_MultiSongPath_TabPage":
                        case "SongMaintenance_DBVer_TabPage":
                            if (SongMaintenance_TabControl.Enabled == true) SongMaintenance_Save_Button.Enabled = true;
                            break;
                        default:
                            SongMaintenance_Save_Button.Enabled = false;
                            break;
                    }
                    break;
            }
        }




    }
}
