using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    static class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", @"SongMgr\Libs");
            AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", @"SongMgr\Libs");
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(null, new object[] { funsion.Invoke(AppDomain.CurrentDomain, null), "PRIVATE_BINPATH", @"SongMgr\Libs" });
        }

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6) NativeMethods.SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }


    class Global
    {
        public static float DPIScalingFactor = 1;

        public static string CrazyktvDatabaseFile = Application.StartupPath + @"\CrazySong.mdb";
        public static string CrazyktvSongMgrDatabaseFile = Application.StartupPath + @"\SongMgr\CrazySongMgr.mdb";
        public static string CrazyktvSongDBVer = "1.00";
        public static string CrazyktvSongDBUpdateFile = Application.StartupPath + @"\SongMgr\Update\UpdateDB.xml";
        public static string CrazyktvCfgFile = Application.StartupPath + @"\CrazyKTV.cfg";
        public static bool CrazyktvDatabaseStatus = false;
        public static bool CrazyktvDatabaseIsOld = false;
        public static bool CrazyktvDatabaseError = false;
        public static bool CrazyktvDatabaseMaxDigitCode = true;
        public static bool SongMgrDatabaseError = false;
        public static bool DatabaseUpdateFinished = false;
        public static bool InitializeSongData = false;

        public static string SongMgrVer = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString().Replace(".", "");
        public static string SongMgrCfgFile = Application.StartupPath + @"\CrazyKTV_SongMgr.cfg";
        public static string SongMgrSupportFormat = ".avi;.flv;.dat;.mkv;.mp4;.mov;.mpeg;.mpg;.rmvb;.ts;.vob;.webm;.wmv";
        public static string SongMgrDestFolder = "";
        public static string SongMgrSongAddMode = "1";
        public static string SongMgrChorusMerge = "False";
        public static string SongMgrMaxDigitCode = "2";
        public static string SongMgrLangCode = "";
        public static string SongMgrSongType = "";
        public static string SongMgrSongInfoSeparate = "1";
        public static string SongMgrChorusSeparate = "1";
        public static string SongMgrFolderStructure = "1";
        public static string SongMgrFileStructure = "1";
        public static string SongMgrSongTrackMode = "True";
        public static string SongMgrBackupRemoveSong = "False";
        public static string SongMgrCustomSingerTypeStructure = "1,1,1,1,1,1,1,2";
        public static string SongMgrEnableMonitorFolders = "False";
        public static string SongMgrSingerGroup = "";
        public static bool SongMgrInitializeStatus = false;
        public static List<string> SongMgrCustomSingerTypeStructureList = new List<string>() { "男", "女", "團", "合唱", "外男", "外女", "外團", "其他", "歌星姓氏", "全部歌星", "新進" };
        public static List<string> SongMgrMonitorFoldersList = new List<string>() { "", "", "", "", "" };

        public static string SingerMgrDefaultSingerDataTable = "ktv_Singer";
        public static string SingerMgrSyncSongSinger = "True";
        public static string SingerMgrLastNameSortMethod = "1";

        public static string SongAddDefaultSongLang = "11";
        public static string SongAddDefaultSingerType = "8";
        public static string SongAddDefaultSongTrack = "1";
        public static string SongAddDefaultSongType = "1";
        public static string SongAddDefaultSongVolume = "100";
        public static string SongAddSpecialStr = "";
        public static string SongAddSongIdentificationMode = "1";
        public static string SongAddDupSongMode = "1";
        public static string SongAddEngSongNameFormat = "False";
        public static string SongAddUseCustomSongID = "False";
        public static string SongAddEnableConvToTC = "False";

        public static string MainCfgAlwaysOnTop = "False";
        public static string MainCfgEnableAutoUpdate = "True";
        public static string MainCfgHideSongDBConverterTabPage = "False";
        public static string MainCfgHideSongAddResultTabPage = "True";
        public static string MainCfgHideSongLogTabPage = "True";
        public static string MainCfgHideApplyCashboxIdButton = "True";
        public static string MainCfgBackupRemoveSongDays = "7";
        public static string MainCfgUIScale = "3";
        public static string MainCfgUICustomScale = "100";
        public static string MainCfgUIFont = "Arial";
        public static string MainCfgEnableUIScale = "False";
        public static string MainCfgBackupDB = "False";
        public static string MainCfgBackupDBPath = "";

        public static string DBVerEnableDBVerUpdate = "True";
        public static string SongMaintenanceEnableMultiSongPath = "False";

        public static List<int> MTotalList = new List<int>();
        public static List<int> TotalList = new List<int>();
        public static List<int> MaxIDList = new List<int>();
        public static List<int> RemainingSongIdCountList = new List<int>();

        public static List<string> CrazyktvSongLangList = new List<string>() { "國語", "台語", "粵語", "日語", "英語", "客語", "原住民語", "韓語", "兒歌", "其它" };
        public static List<string> CrazyktvSongLangKeyWordList = new List<string>() { "國語,國", "台語,台,閩南,閩,臺語,臺", "粵語,粵,廣東", "日語,日文,日", "英語,英文,英", "客語,客", "原住民語,民謠", "韓語,韓", "兒歌,兒", "其它" };
        public static List<string> CrazyktvSingerTypeList = new List<string>() { "男歌星", "女歌星", "團體", "合唱", "外國男", "外國女", "外國團體", "其他", "未使用", "未使用", "新進歌星" };
        public static List<string> CrazyktvSingerTypeKeyWordList = new List<string>() { "男,男星,男歌星,男歌手", "女,女星,女歌星,女歌手", "團,團體,樂團", "合唱,對唱", "外男,外國男星,外國男歌星,外國男歌手", "外女,外國女星,外國女歌星,外國女歌手", "外團,外國團體,外國樂團", "未知,其他,其他歌星,其他歌手" };
        public static List<string> CrazyktvSongTrackList = new List<string>() { "V0", "VR", "VL", "V3", "V4", "V5" };
        public static List<string> CrazyktvSongTrackWordList = new List<string>() { "立體聲", "右聲道 / 音軌2", "左聲道 / 音軌1", "音軌3", "音軌4", "音軌5" };
        public static List<string> CrazyktvSongTrackKeyWordList = new List<string>() { "v0,立體", "vr,r,右", "vl,l,左", "v3", "v4", "v5" };

        public static List<string> SingerList = new List<string>();
        public static List<string> SingerLowCaseList = new List<string>();
        public static List<string> SingerTypeList = new List<string>();
        public static List<string> AllSingerList = new List<string>();
        public static List<string> AllSingerLowCaseList = new List<string>();
        public static List<string> AllSingerTypeList = new List<string>();

        public static List<string> SingerGroupList = new List<string>();
        public static List<int> GroupSingerIdList = new List<int>();
        public static List<string> GroupSingerLowCaseList = new List<string>();

        public static List<string> PhoneticsWordList = new List<string>();
        public static List<string> PhoneticsSpellList = new List<string>();
        public static List<string> PhoneticsPenStyleList = new List<string>();
        public static List<string> PhoneticsStrokesList = new List<string>();

        public static List<string> CashboxSongDataFullList = new List<string>();
        public static List<string> CashboxSongDataLangList = new List<string>();
        public static List<string> CashboxSongDataLowCaseList = new List<string>();
        public static List<string> CashboxSongDataFuzzyList = new List<string>();
        public static List<string> CashboxFullMatchSongList = new List<string>();
        public static List<string> CashboxFullAnalysisSongList = new List<string>();
        public static List<string> CashboxHaveSongList = new List<string>();

        public static List<string> SongMaintenanceMultiSongPathList = new List<string>();
        public static List<string> SongDBConvValueList = new List<string>();
        public static List<string> SongDBConvJetktvLangList = new List<string>();
        public static List<string> SongDBConvJetktvPathList = new List<string>();
        public static List<string> SongDBConvHK2SongPathList = new List<string>();

        public static DataTable SongDT = new DataTable();
        public static DataTable SingerDT = new DataTable();
        public static DataTable AllSingerDT = new DataTable();
        public static DataTable DuplicateSongDT = new DataTable();
        public static DataTable FailureSongDT = new DataTable();
        public static DataTable SongSrcDT = new DataTable();
        public static DataTable SingerSrcDT = new DataTable();
        public static DataTable SongPathSrcDT = new DataTable();
        public static DataTable ExtensionSrcDT = new DataTable();
        public static DataTable SongLogDT = new DataTable();
        public static DataTable FavoriteUserDT = new DataTable();
        public static DataTable CashboxNewSongDateDT = new DataTable();

        public static DateTime TimerStartTime = new DateTime();
        public static DateTime TimerEndTime = new DateTime();

        public static string SongQueryQueryType = "SongQuery";
        public static string SongQueryFuzzyQuery = "True";
        public static string SongQueryFilter = "全部";
        public static bool SongQuerySynonymousQuery = true;

        public static bool SongQueryMultiEdit;
        public static List<bool> SongQueryMultiEditUpdateList = new List<bool>() { false, false, false, false, false, false, false, false };

        /// <summary>
        /// <para>0: SongId | 1: SongLang | 2: SongSingerType | 3: SongSinger</para>
        /// <para>4: SongSongName | 5: SongTrack | 6: SongSongType | 7: SongVolume</para> 
        /// <para>8: SongWordCount | 9: SongPlayCount | 10: SongMB | 11: SongCreatDate</para> 
        /// <para>12: SongFileName | 13: SongPath | 14: SongSpell | 15: SongSpellNum</para> 
        /// <para>16: SongSongStroke | 17: SongPenStyle | 18: SongPlayState | 19: SongSrcPath</para>
        /// </summary>
        public static List<string> SongQueryDataGridViewSelectList = new List<string>();
        public static string SongQueryDataGridViewRestoreCurrentRow = "";
        public static List<string> SongQueryDataGridViewRestoreSelectList = new List<string>();

        public static bool SongAddMultiEdit;
        public static List<bool> SongAddMultiEditUpdateList = new List<bool>() { false, false, false, false, false, false, false, false };

        /// <summary>
        /// <para>0: SongId | 1: SongLang | 2: SongSingerType | 3: SongSinger</para>
        /// <para>4: SongSongName | 5: SongTrack | 6: SongSongType | 7: SongVolume</para> 
        /// <para>8: SongWordCount | 9: SongPlayCount | 10: SongMB | 11: SongCreatDate</para> 
        /// <para>12: SongFileName | 13: SongPath | 14: SongSpell | 15: SongSpellNum</para> 
        /// <para>16: SongSongStroke | 17: SongPenStyle | 18: SongPlayState | 19: SongSrcPath</para>
        /// </summary>
        public static List<string> SongAddDataGridViewSelectList = new List<string>();
        public static string SongAddDataGridViewRestoreCurrentRow = "";
        public static List<string> SongAddDataGridViewRestoreSelectList = new List<string>();

        public static bool SingerMgrMultiEdit;
        public static List<bool> SingerMgrMultiEditUpdateList = new List<bool>() { false };
        public static List<string> SingerMgrDataGridViewSelectList = new List<string>();
        public static string SingerMgrDataGridViewRestoreCurrentRow = "";
        public static List<string> SingerMgrDataGridViewRestoreSelectList = new List<string>();

        public static bool CashboxMultiEdit;
        public static List<bool> CashboxMultiEditUpdateList = new List<bool>() { false, false, false };

        /// <summary>
        /// <para>0: SongId | 1: SongLang | 2: SongSinger | 3: SongSongName | 4: SongCreatDate</para>
        /// </summary>
        public static List<string> CashboxDataGridViewSelectList = new List<string>();
        public static string CashboxDataGridViewRestoreCurrentRow = "";
        public static List<string> CashboxDataGridViewRestoreSelectList = new List<string>();

        public static List<string> PlayerUpdateSongValueList = new List<string>();
        public static bool PlayerInitialized = false;

        public static List<List<string>> UnusedSongIdList = new List<List<string>>();

        public static List<string> CrtchorusSeparateList = new List<string>() { "&", "+" };

        public static List<string> SynonymousWordList = new List<string>()
        {
            "阿|啊", "遊|游", "痴|癡", "姍|姗|珊", "袂|抹|袜|抺|祙", "你|妳", "姊|姐", "未|末", "他|她", "秘|祕", "周|週", "已|己", "雙|双",
            "兩|二", "作|做", "密|蜜", "愈|越", "裡|裏", "腳|脚", "嘆|歎", "煙|菸|烟", "叉|义", "叨|叼", "妝|粧", "鉤|鈎", "沈|沉", "灑|洒",
            "無|嘸|毋|唔", "恒|恆", "那|哪", "甘|咁", "庄|莊", "漂|飄", "什|甚|啥", "決|絕", "麥|嘜", "奈|耐", "果|菓", "的|得|の", "一|ㄧ",
            "逃|鼗", "喲|呦", "藥|葯", "只|衹|祇|袛|祗", "群|羣", "拼|拚", "麼|麽", "支|隻", "鬥|斗", "占,佔", "侯,候"
        };

        public static List<string> SingerTypeStructureList = new List<string>()
        {
            "男,男星,男歌星,男歌手",
            "女,女星,女歌星,女歌手",
            "團,團體,樂團",
            "合唱,對唱",
            "外男,外國男星,外國男歌星,外國男歌手",
            "外女,外國女星,外國女歌星,外國女歌手",
            "外團,外國團體,外國樂團",
            "未知,其他,其他歌星,其他歌手"
        };

        public static List<string> SongAnalysisExceptionSingerList = new List<string>() { "台灣女孩", "梁靜茹", "張懸", "莫文蔚" };
        public static List<string> SongAnalysisExceptionSongList = new List<string>()
        {
            "羅百吉&寶貝|台灣女孩",
            "梁靜茹|兒歌",
            "張懸|兒歌",
            "莫文蔚|台"
        };

        public static DataTable SongMonitorDT = new DataTable();

        public static FileSystemWatcher SongMonitorPath1 = new FileSystemWatcher();
        public static FileSystemWatcher SongMonitorPath2 = new FileSystemWatcher();
        public static FileSystemWatcher SongMonitorPath3 = new FileSystemWatcher();
        public static FileSystemWatcher SongMonitorPath4 = new FileSystemWatcher();
        public static FileSystemWatcher SongMonitorPath5 = new FileSystemWatcher();
        public static FileSystemWatcher[] SongMonitorWatcher =
        {
            SongMonitorPath1,
            SongMonitorPath2,
            SongMonitorPath3,
            SongMonitorPath4,
            SongMonitorPath5
        };

        public static Timer SongMonitorTimer = new Timer();
        public static DateTime SongMonitorSTime = new DateTime();
        public static DateTime SongMonitorETime = new DateTime();

        public static List<string> SongMonitorCreatedList = new List<string>();
        public static List<string> SongMonitorDeletedList = new List<string>();

        public static List<string> CashboxSongLangList = new List<string>() { "國語", "台語", "粵語", "日語", "英語", "客語", "韓語", "兒歌", "其它" };
        public static DateTime CashboxUpdDate = DateTime.Parse("2016/01/01");
        public static DataTable CashboxDT = new DataTable();
        public static bool CashboxFuzzyQuery = true;
        public static string CashboxQueryFilter = "全部";
        public static string CashboxNonSymbolList = "、|。|！|，|．|？";
        public static bool CashboxSynonymousQuery = true;
        public static bool CashboxHasWideChar = false;

        public static string RegexChorusSeparate = "[&+、]";

    }
}
