using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    static class Program
    {
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
        public static string CrazyktvDatabaseFile = Application.StartupPath + @"\CrazySong.mdb";
        public static string CrazyktvSongDBVer = "1.00";
        public static string CrazyktvSongDBUpdateFile = Application.StartupPath + @"\SongMgr\Update\UpdateDB.xml";
        public static string CrazyktvDatabaseMaxDigitCode = "";
        public static string CrazyktvCfgFile = Application.StartupPath + @"\CrazyKTV.cfg";

        public static string SongMgrCfgFile = Application.StartupPath + @"\CrazyKTV_SongMgr.cfg";
        public static string SongMgrSupportFormat = ".avi;.flv;.dat;.mkv;.mp4;.mov;.mpg;.rmvb;.ts;.vob;.webm;.wmv";
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

        public static string SingerMgrDefaultSingerDataTable = "ktv_Singer";
        
        public static string SongAddDefaultSongLang = "11";
        public static string SongAddDefaultSingerType = "8";
        public static string SongAddDefaultSongTrack = "1";
        public static string SongAddDefaultSongType = "1";
        public static string SongAddDefaultSongVolume = "100";
        public static string SongAddSpecialStr = "A-Lin";
        public static string SongAddSongIdentificationMode = "1";
        public static string SongAddDupSongMode = "1";
        public static string SongAddEngSongNameFormat = "False";
        public static string SongAddUseCustomSongID = "False";

        public static string MainCfgAlwaysOnTop = "False";
        public static string MainCfgHideSongDBConverterTabPage = "False";
        public static string MainCfgHideSongAddResultTabPage = "True";
        public static string MainCfgHideSongLogTabPage = "True";
        public static string MainCfgBackupRemoveSongDays = "7";

        public static string DBVerEnableDBVerUpdate = "True";
        public static string DBVerRebuildSingerData = "False";
        public static string SongMaintenanceEnableMultiSongPath = "False";

        public static List<int> TotalList = new List<int>();
        public static List<int> MaxIDList = new List<int>();
        public static int RemainingSongID = 9999999;

        public static List<string> CrazyktvSongLangList = new List<string>() { "國語", "台語", "粵語", "日語", "英語", "客語", "原住民語", "韓語", "兒歌", "其它" };
        public static List<string> CrazyktvSongLangIDList = new List<string>() { "國語,國", "台語,台,閩南,閩", "粵語,粵,廣東", "日語,日文,日", "英語,英文,英", "客語,客", "原住民語,民謠", "韓語,韓", "兒歌,兒", "其它" };
        public static List<string> CrazyktvSingerTypeList = new List<string>() { "男歌星", "女歌星", "樂團", "合唱", "外國男", "外國女", "外國樂團", "其它", "未使用", "未使用", "新進歌星" };
        public static List<string> CrazyktvDBTableList = new List<string>();

        public static List<string> SongAnalysisSingerList = new List<string>();
        public static List<string> SongAnalysisSingerLowCaseList = new List<string>();
        public static List<string> SongAnalysisSingerTypeList = new List<string>();

        public static List<string> PhoneticsWordList = new List<string>();
        public static List<string> PhoneticsSpellList = new List<string>();
        public static List<string> PhoneticsPenStyleList = new List<string>();
        public static List<string> PhoneticsStrokesList = new List<string>();

        public static List<string> SongAddValueList = new List<string>();
        public static List<string> SongAddChorusSingerList = new List<string>();
        public static List<string> SongAddAllSongIDList = new List<string>();

        public static List<string> SongMaintenanceMultiSongPathList = new List<string>();
        public static List<string> SongDBConvValueList = new List<string>();
        public static List<string> SongDBConvJetktvLangList = new List<string>();
        public static List<string> SongDBConvJetktvPathList = new List<string>();

        public static DataTable SongDT = new DataTable();
        public static DataTable SongAddDT = new DataTable();
        public static DataTable SingerDT = new DataTable();
        public static DataTable AllSingerDT = new DataTable();
        public static DataTable PhoneticsDT = new DataTable();
        public static DataTable NotExistsSongIdDT = new DataTable();
        public static DataTable DuplicateSongDT = new DataTable();
        public static DataTable FailureSongDT = new DataTable();
        public static DataTable DupSongAddDT = new DataTable();
        public static DataTable SongSrcDT = new DataTable();
        public static DataTable SingerSrcDT = new DataTable();
        public static DataTable SongLogDT = new DataTable();
        public static DataTable SongStatisticsDT = new DataTable();
        public static DataTable FavoriteUserDT = new DataTable();

        public static DateTime TimerStartTime = new DateTime();
        public static DateTime TimerEndTime = new DateTime();

        public static string SongQueryQueryType = "SongQuery";
        public static string SongQueryFuzzyQuery = "True";
        public static bool SongQuerySynonymousQuery = true;
        public static string SongQueryFilter = "全部";

        public static string SongQueryDataGridViewValue = "";
        public static string SingerQueryDataGridViewValue = "";

        public static List<string> SynonymousWordList = new List<string>()
        {
            "阿|啊", "遊|游", "痴|癡", "姍|珊", "袂|抹|袜|抺|祙", "你|妳", "姊|姐", "未|末", "他|她", "秘|祕", "周|週", "已|己", "雙|双",
            "兩|二", "作|做", "密|蜜", "愈|越", "裡|裏", "腳|脚", "嘆|歎", "煙|菸|烟", "叉|义", "叨|叼", "妝|粧", "鉤|鈎", "沈|沉", "灑|洒",
            "無|嘸|毋|唔", "恒|恆", "那|哪", "甘|咁", "庄|莊", "漂|飄", "什|啥", "決|絕", "麥|嘜", "奈|耐", "果|菓", "的|得|の", "一|ㄧ",
            "逃|鼗", "喲|呦", "藥|葯", "只|衹|祇|袛|祗"
        };
    }
}
