using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrazyKTV_SongMgr
{
    class SongAnalysis
    {
        private static object LockThis = new object();

        #region --- SongAnalysis 建立資料表 ---

        public static DataTable SongAnalysisDT;
        public static List<string> SingerDataList;
        public static List<string> SingerDataLowCaseList;
        public static List<string> SingerDataTypeList;
        public static bool SongAnalysisCompleted;

        public static void CreateSongDataTable()
        {
            SongAnalysisCompleted = false;

            SongAnalysisDT = new DataTable();
            SongAnalysisDT.Columns.Add("Song_AddStatus", typeof(string));
            SongAnalysisDT.Columns.Add("Song_Id", typeof(string));
            SongAnalysisDT.Columns.Add("Song_Lang", typeof(string));
            SongAnalysisDT.Columns.Add("Song_SingerType", typeof(int));
            SongAnalysisDT.Columns.Add("Song_Singer", typeof(string));
            SongAnalysisDT.Columns.Add("Song_SongName", typeof(string));
            SongAnalysisDT.Columns.Add("Song_Track", typeof(int));
            SongAnalysisDT.Columns.Add("Song_SongType", typeof(string));
            SongAnalysisDT.Columns.Add("Song_Volume", typeof(int));
            SongAnalysisDT.Columns.Add("Song_WordCount", typeof(int));
            SongAnalysisDT.Columns.Add("Song_PlayCount", typeof(int));
            SongAnalysisDT.Columns.Add("Song_MB", typeof(float));
            SongAnalysisDT.Columns.Add("Song_CreatDate", typeof(DateTime));
            SongAnalysisDT.Columns.Add("Song_FileName", typeof(string));
            SongAnalysisDT.Columns.Add("Song_Path", typeof(string));
            SongAnalysisDT.Columns.Add("Song_Spell", typeof(string));
            SongAnalysisDT.Columns.Add("Song_SpellNum", typeof(string));
            SongAnalysisDT.Columns.Add("Song_SongStroke", typeof(int));
            SongAnalysisDT.Columns.Add("Song_PenStyle", typeof(string));
            SongAnalysisDT.Columns.Add("Song_PlayState", typeof(int));
            SongAnalysisDT.Columns.Add("Song_SrcPath", typeof(string));
            SongAnalysisDT.Columns.Add("Song_SortIndex", typeof(string));

            SingerDataList = new List<string>();
            SingerDataLowCaseList = new List<string>();
            SingerDataTypeList = new List<string>();

            string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongAllSingerQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SingerDataList.Add(row["Singer_Name"].ToString());
                    SingerDataTypeList.Add(row["Singer_Type"].ToString());
                }
            }

            string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    if (SingerDataLowCaseList.IndexOf(row["Singer_Name"].ToString().ToLower()) < 0)
                    {
                        SingerDataList.Add(row["Singer_Name"].ToString());
                        SingerDataTypeList.Add(row["Singer_Type"].ToString());
                    }
                }
            }
            SingerDataLowCaseList = SingerDataList.ConvertAll(str => str.ToLower());
        }

        public static void DisposeSongDataTable()
        {
            SingerDataList.Clear();
            SingerDataLowCaseList.Clear();
            SingerDataTypeList.Clear();
            SongAnalysisDT.Dispose();
            SongAnalysisDT = null;
            SongAnalysisCompleted = true;
            GC.Collect();
        }

        #endregion

        #region --- SongAnalysis 分析歌曲 ---

        public static void SongInfoAnalysis(string file, List<string> SongLangKeyWordList, List<string> SingerTypeKeyWordList, List<string> SongTrackKeyWordList)
        {
            string SongAddStatus = "";
            string SongID = "";
            string SongLang = "";
            string SongSingerType = "";
            string SongSinger = "";
            string SongSongName = "";
            string SongTrack = "";
            string SongSongType = "";
            string SongVolume = "";
            string SongWordCount = "";
            string SongPlayCount = "";
            float SongMB = 0;
            DateTime SongCreatDate = new DateTime();
            string SongSpell = "";
            string SongSpellNum = "";
            string SongSongStroke = "";
            string SongPenStyle = "";
            string SongPlayState = "";
            string SongSrcPath = Path.GetFullPath(file);
            string SongSortIndex = "";

            string SongLangStr = "";
            string SongSingerTypeStr = "";
            string SongTrackStr = "";
            string SongSongTypeStr = "";

            bool ChorusSong = false;

            string SeparateStr = "[-_](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))";
            string DirStr = Path.GetDirectoryName(file);

            List<string> list = new List<string>();
            list = new List<string>(Regex.Split(DirStr, @"\\", RegexOptions.None));

            foreach (string str in list)
            {
                if (SongSingerType != "" && SongSinger != "" && SongLang != "") break;
                string splitstr = Regex.Replace(str, @"%%|^\s*|\s*$", "");

                // 查看資料夾名稱中有無歌手
                if (SingerDataLowCaseList.IndexOf(splitstr.ToLower()) >= 0 && Global.SongAddSongIdentificationMode == "1")
                {
                    SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(splitstr.ToLower())];
                    SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(splitstr.ToLower())];
                }

                // 查看資料夾名稱中有無語系類別
                if (SongLangKeyWordList.IndexOf(splitstr) >= 0)
                {
                    SongLang = GetSongInfo("SongLang", splitstr);
                    SongLangStr = splitstr;
                }

                // 查看資料夾名稱中有無歌手類別
                if (SingerTypeKeyWordList.IndexOf(splitstr) >= 0)
                {
                    if (SongSingerType == "" || GetSongInfo("SongSingerType", splitstr) == "3")
                    {
                        SongSingerType = GetSongInfo("SongSingerType", splitstr);
                        SongSingerTypeStr = splitstr;
                        if (SongSingerType == "3") ChorusSong = true;
                    }
                }
            }
            list.Clear();

            // 從檔案名稱查找歌曲資訊
            string FileStr = Path.GetFileNameWithoutExtension(file);
            if (Global.SongAddEnableConvToTC == "True") FileStr = CommonFunc.ConvToTraditionalChinese(FileStr);

            // 特殊歌手及歌曲名稱處理
            list = new List<string>(Regex.Split(Global.SongAddSpecialStr, ",", RegexOptions.IgnoreCase));
            foreach (string str in list)
            {
                FileStr = Regex.Replace(FileStr, str, "%%" + str + "%%", RegexOptions.IgnoreCase);
            }
            list.Clear();

            // 歌手-歌名(語系-類別-聲道) 名稱處理
            MatchCollection BracketMatches = Regex.Matches(FileStr, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", RegexOptions.IgnoreCase);
            if (BracketMatches.Count > 0)
            {
                foreach (Match m in BracketMatches)
                {
                    if (m.Value.ContainsAny("-", "_"))
                    {
                        FileStr = FileStr.Replace(m.Value, "%%" + m.Value + "%%");
                    }
                }
            }

            string FileStrOpti = FileStr;
            if (SongSingerType != "")
            {
                FileStrOpti = Regex.Replace(FileStrOpti, "^" + SongSingerType + "[_-]|[_-]" + SongSingerType + "[_-]|[_-]" + SongSingerType + "$", "_", RegexOptions.IgnoreCase);
                FileStrOpti = Regex.Replace(FileStrOpti, "^[_]|[_]$", "", RegexOptions.IgnoreCase);
            }

            if (SongSinger != "")
            {
                FileStrOpti = Regex.Replace(FileStrOpti, "^" + SongSinger + "[_-]|[_-]" + SongSinger + "[_-]|[_-]" + SongSinger + "$", "_", RegexOptions.IgnoreCase);
                FileStrOpti = Regex.Replace(FileStrOpti, "^[_]|[_]$", "", RegexOptions.IgnoreCase);
            }

            if (SongLang != "")
            {
                FileStrOpti = Regex.Replace(FileStrOpti, "^" + SongLang + "[_-]|[_-]" + SongLang + "[_-]|[_-]" + SongLang + "$", "_", RegexOptions.IgnoreCase);
                FileStrOpti = Regex.Replace(FileStrOpti, "^[_]|[_]$", "", RegexOptions.IgnoreCase);
            }

            string FileNameRemoveStr = "";
            List<string> FileNameRemoveList = new List<string>();
            List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));
            List<string> SongSongTypeLowCaseList = SongSongTypeList.ConvertAll(str => str.ToLower());

            list = new List<string>(Regex.Split(FileStrOpti, SeparateStr, RegexOptions.None));
            foreach (string str in list)
            {
                if (SongSingerType != "" && SongSinger != "" && SongLang != "" && SongTrack != "" && SongSongType != "") break;
                string splitstr = Regex.Replace(str, @"%%|^\s*|\s*$", "");

                // 查看檔案名稱中有無歌手
                if (SongSinger == "")
                {
                    if (SingerDataLowCaseList.IndexOf(splitstr.ToLower()) >= 0)
                    {
                        SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(splitstr.ToLower())];
                        if (SongSingerType == "" || !ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(splitstr.ToLower())];
                    }
                }

                // 括號字串處理
                MatchCollection matches = Regex.Matches(splitstr, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string MatchStr = Regex.Replace(match.Value, @"^[\{\(\[｛（［【]|[】］）｝\]\)\}]$", "", RegexOptions.IgnoreCase);
                        if (MatchStr.ContainsAny("_", "-"))
                        {
                            List<string> BracketStrlist = new List<string>(Regex.Split(MatchStr, SeparateStr, RegexOptions.None));
                            string BracketSeparateStr = (MatchStr.Contains("_")) ? "_" : "-";

                            foreach (string BracketStr in BracketStrlist)
                            {
                                if (SongLangKeyWordList.IndexOf(BracketStr) >= 0)
                                {
                                    SongLang = GetSongInfo("SongLang", BracketStr);
                                    FileNameRemoveStr += ((BracketStrlist.IndexOf(BracketStr) == 0) ? BracketStr : BracketSeparateStr + BracketStr);
                                }
                                else if (SongTrackKeyWordList.IndexOf(BracketStr.ToLower()) >= 0 || BracketStr.ToLower() == "l" || BracketStr.ToLower() == "r")
                                {
                                    SongTrack = GetSongInfo("SongTrack", BracketStr.ToLower());
                                    FileNameRemoveStr += ((BracketStrlist.IndexOf(BracketStr) == 0) ? BracketStr : BracketSeparateStr + BracketStr);
                                }
                                else if (SongSongTypeLowCaseList.IndexOf(BracketStr.ToLower()) >= 0)
                                {
                                    SongSongType = SongSongTypeList[SongSongTypeLowCaseList.IndexOf(BracketStr.ToLower())];
                                    FileNameRemoveStr += ((BracketStrlist.IndexOf(BracketStr) == 0) ? BracketStr : BracketSeparateStr + BracketStr);
                                }
                                else if (BracketStr.ContainsAny("合唱", "對唱"))
                                {
                                    SongSingerType = "3";
                                    ChorusSong = true;
                                }
                            }
                            if (FileNameRemoveStr != "") FileNameRemoveList.Add(FileNameRemoveStr);
                        }
                        else
                        {
                            if (SongLangKeyWordList.IndexOf(MatchStr) >= 0)
                            {
                                SongLang = GetSongInfo("SongLang", MatchStr);
                                FileNameRemoveList.Add(MatchStr);
                            }
                            else if (SongTrackKeyWordList.IndexOf(MatchStr.ToLower()) >= 0 || MatchStr.ToLower() == "l" || MatchStr.ToLower() == "r")
                            {
                                SongTrack = GetSongInfo("SongTrack", MatchStr.ToLower());
                                FileNameRemoveList.Add(MatchStr);
                            }
                            else if (SongSongTypeLowCaseList.IndexOf(MatchStr.ToLower()) >= 0)
                            {
                                SongSongType = SongSongTypeList[SongSongTypeLowCaseList.IndexOf(MatchStr.ToLower())];
                                FileNameRemoveList.Add(MatchStr);
                            }
                            else if (MatchStr.ContainsAny("合唱", "對唱"))
                            {
                                SongSingerType = "3";
                                ChorusSong = true;
                            }
                        }
                    }
                }

                // 查看檔案名稱中有無語系類別
                if (SongLang == "")
                {
                    if (SongLangKeyWordList.IndexOf(splitstr) >= 0)
                    {
                        SongLang = GetSongInfo("SongLang", splitstr);
                        SongLangStr = splitstr;
                    }
                }

                // 查看檔案名稱中有無歌手類別
                if (SingerTypeKeyWordList.IndexOf(splitstr) >= 0)
                {
                    if (SongSingerType == "" || GetSongInfo("SongSingerType", splitstr) == "3")
                    {
                        SongSingerType = GetSongInfo("SongSingerType", splitstr);
                        SongSingerTypeStr = splitstr;
                        if (SongSingerType == "3") ChorusSong = true;
                    }
                }

                // 查看檔案名稱中有無歌曲聲道
                if (SongTrack == "")
                {
                    if (SongTrackKeyWordList.IndexOf(splitstr.ToLower()) >= 0)
                    {
                        SongTrack = GetSongInfo("SongTrack", splitstr.ToLower());
                        SongTrackStr = splitstr;
                    }
                }

                // 查看檔案名稱中有無歌曲類別
                if (SongSongType == "")
                {
                    if (SongSongTypeLowCaseList.IndexOf(splitstr.ToLower()) >= 0)
                    {
                        SongSongType = SongSongTypeList[SongSongTypeLowCaseList.IndexOf(splitstr.ToLower())];
                        SongSongTypeStr = splitstr;
                    }
                }
            }
            SongSongTypeList.Clear();
            SongSongTypeLowCaseList.Clear();

            // 套用預設語系類別
            if (SongLang == "")
            {
                SongLang = CommonFunc.GetSongLangStr(int.Parse(Global.SongAddDefaultSongLang) - 1, 0, "null");
            }

            // 套用預設歌曲聲道
            if (SongTrack == "")
            {
                SongTrack = Global.SongAddDefaultSongTrack;
            }

            // 套用預設歌曲類別
            if (SongSongType == "")
            {
                string str = "";
                if (Global.SongMgrSongType != "") { str = "無類別," + Global.SongMgrSongType; } else { str = "無類別"; }
                list = new List<string>(str.Split(','));
                if (list[int.Parse(Global.SongAddDefaultSongType) - 1] == "無類別")
                {
                    SongSongType = "";
                }
                else
                {
                    SongSongType = list[int.Parse(Global.SongAddDefaultSongType) - 1];
                }
                list.Clear();
            }

            // 套用預設歌曲音量
            if (SongVolume == "")
            {
                SongVolume = Global.SongAddDefaultSongVolume;
            }

            // 判斷歌手及歌曲名稱
            if (SongSongName == "")
            {
                list = new List<string>() { SongLangStr, SongSingerTypeStr, SongTrackStr, SongSongTypeStr };
                string str = FileStr;

                foreach (string s in list)
                {
                    str = Regex.Replace(str, "^" + s + "[_-]|[_-]" + s + "[_-]|[_-]" + s + "$", "_", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, "^[_-]|[_-]$", "", RegexOptions.IgnoreCase);
                }
                list.Clear();

                // 去除括號內要移除的字串
                if (FileNameRemoveList.Count > 0)
                {
                    foreach (string RemoveStr in FileNameRemoveList)
                    {
                        str = str.Replace(RemoveStr, "");
                        str = Regex.Replace(str, @"[\{\(\[｛（［【]" + @"[】］）｝\]\)\}]", "", RegexOptions.IgnoreCase);
                    }
                }
                FileNameRemoveList.Clear();

                list = new List<string>(Regex.Split(str, SeparateStr, RegexOptions.None));

                switch (Global.SongAddSongIdentificationMode)
                {
                    case "1":
                        switch (list.Count)
                        {
                            case 1:
                                if (SongSinger == "")
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(Path.GetFileName(DirStr))) SongSingerType = "3";
                                    SongSinger = Regex.Replace(Path.GetFileName(DirStr), @"%%|^\s*|\s*$", "");
                                }
                                SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                break;
                            default:
                                if (SongSinger == "")
                                {
                                    if (CommonFunc.IsSongId(list[0]))
                                    {
                                        SongID = list[0];
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[1])) SongSingerType = "3";
                                        SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    }
                                    else
                                    {
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[0])) SongSingerType = "3";
                                        SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                else
                                {
                                    if (CommonFunc.IsSongId(list[0]))
                                    {
                                        SongID = list[0];
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[1]))
                                        {
                                            SongSingerType = "3";
                                            SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                        }
                                    }
                                    else
                                    {
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[0]))
                                        {
                                            SongSingerType = "3";
                                            SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                        }
                                    }
                                }
                                if (string.Compare(SongSinger, list[1], true) == 0)
                                {
                                    if (CommonFunc.IsSongId(list[0]))
                                    {
                                        SongID = list[0];
                                        if (list.Count > 2)
                                        {
                                            SongSongName = Regex.Replace(list[2], @"%%|^\s*|\s*$", "");
                                        }
                                        else
                                        {
                                            SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                        }
                                    }
                                    else
                                    {
                                        SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                else
                                {
                                    SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                }
                                break;
                        }
                        break;
                    case "2":
                    case "3":
                        switch (list.Count)
                        {
                            case 1:
                                if (SongSinger == "")
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(Path.GetFileName(DirStr))) SongSingerType = "3";
                                    SongSinger = Regex.Replace(Path.GetFileName(DirStr), @"%%|^\s*|\s*$", "");
                                }
                                SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                break;
                            default:
                                if (SongSinger == "")
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    switch (Global.SongAddSongIdentificationMode)
                                    {
                                        case "2":
                                            if (r.IsMatch(list[0])) SongSingerType = "3";
                                            SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                            SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                            break;
                                        case "3":
                                            if (r.IsMatch(list[1])) SongSingerType = "3";
                                            SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                            SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                            break;
                                    }
                                }
                                else
                                {
                                    if (SongSinger == Regex.Replace(list[0], @"%%|^\s*|\s*$", ""))
                                    {
                                        SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[0])) SongSingerType = "3";

                                        if (SingerDataLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                                        {
                                            SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                            if (!ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                        }
                                        SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    }
                                    else
                                    {
                                        SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[1])) SongSingerType = "3";

                                        if (SingerDataLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                                        {
                                            SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                            if (!ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                        }
                                        SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                break;
                        }
                        break;
                    case "4":
                        switch (list.Count)
                        {
                            case 1:
                                if (SongSinger == "")
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(Path.GetFileName(DirStr))) SongSingerType = "3";
                                    SongSinger = Regex.Replace(Path.GetFileName(DirStr), @"%%|^\s*|\s*$", "");
                                }
                                SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                break;
                            case 2:
                                if (SongSinger == "")
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(list[0])) SongSingerType = "3";
                                    SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                    SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                }
                                else
                                {
                                    if (SongSinger == Regex.Replace(list[0], @"%%|^\s*|\s*$", ""))
                                    {
                                        SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[0])) SongSingerType = "3";

                                        if (SingerDataLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                                        {
                                            SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                            if (!ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                        }
                                        SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    }
                                    else
                                    {
                                        SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[1])) SongSingerType = "3";

                                        if (SingerDataLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                                        {
                                            SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                            if (!ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                        }
                                        SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                break;
                            default:
                                if (CommonFunc.IsSongId(list[0]))
                                {
                                    SongID = list[0];
                                }

                                if (SongSinger == "")
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(list[1])) SongSingerType = "3";
                                    SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    SongSongName = Regex.Replace(list[2], @"%%|^\s*|\s*$", "");
                                }
                                else
                                {
                                    if (SongSinger == Regex.Replace(list[1], @"%%|^\s*|\s*$", ""))
                                    {
                                        SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[1])) SongSingerType = "3";

                                        if (SingerDataLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                                        {
                                            SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                            if (!ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                        }
                                        SongSongName = Regex.Replace(list[2], @"%%|^\s*|\s*$", "");
                                    }
                                    else
                                    {
                                        SongSinger = Regex.Replace(list[2], @"%%|^\s*|\s*$", "");
                                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                        if (r.IsMatch(list[2])) SongSingerType = "3";

                                        if (SingerDataLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                                        {
                                            SongSinger = SingerDataList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                            if (!ChorusSong) SongSingerType = SingerDataTypeList[SingerDataLowCaseList.IndexOf(SongSinger.ToLower())];
                                        }
                                        SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                break;
                        }
                        break;
                }
            }

            // 台灣女孩問題處理
            if (SongSinger == "台灣女孩")
            {
                if (SongSongName == "羅百吉&寶貝")
                {
                    SongSinger = "羅百吉&寶貝";
                    SongSongName = "台灣女孩";
                    SongSingerType = "3";
                }
            }
            
            // 套用預設歌手類別
            if (SongSingerType == "")
            {
                string str = CommonFunc.GetSingerTypeStr(int.Parse(Global.SongAddDefaultSingerType) - 1, 3, "null");
                SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, str);
            }

            // 統一資料庫中合唱歌手分隔符號
            if (SongSingerType == "3")
            {
                Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                if (r.IsMatch(SongSinger)) SongSinger = Regex.Replace(SongSinger, "[&+]", "&", RegexOptions.IgnoreCase);
            }

            // 計算歌曲字數
            List<string> SongWordCountList = new List<string>();
            SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
            SongWordCount = SongWordCountList[0];

            if (Global.SongAddEngSongNameFormat == "True")
            {
                if (SongWordCountList[1] == "True")
                {
                    TextInfo ti = new CultureInfo("en-US",false).TextInfo;
                    SongSongName = ti.ToTitleCase(SongSongName.ToLower());
                }
            }

            // 點播次數初始值
            SongPlayCount = "0";

            // 計算歌曲大小
            FileInfo f = new FileInfo(file);
            SongMB = float.Parse(((f.Length / 1024f) / 1024f).ToString("F2"));

            // 取得加歌日期
            SongCreatDate = DateTime.Now;

            // 取得歌曲拼音
            List<string> SongSpellList = new List<string>();
            SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
            SongSpell = SongSpellList[0];
            SongSpellNum = SongSpellList[1];
            if (SongSpellList[2] == "") SongSpellList[2] = "0";
            SongSongStroke = SongSpellList[2];
            SongPenStyle = SongSpellList[3];

            // 播放狀態初始值
            SongPlayState = "0";

            // 排序索引
            if (SongLang == "未知") SongSortIndex = "1";
            if (SongSortIndex == "") { if (SongSingerType == "10") SongSortIndex = "2"; }
            if (SongSortIndex == "") { if (SongID != "") SongSortIndex = "3"; }
            if (SongSortIndex == "") SongSortIndex = "4";

            // 加歌狀態
            if (SongLang == "未知") SongAddStatus = "語系類別必須有值才能加歌!";
            if (SongAddStatus == "") { if (SongSingerType == "10") SongAddStatus = "此歌手尚未設定歌手資料!";}

            // 歌庫監視
            if (Global.SongMgrSongAddMode == "4")
            {
                if (SongLang == "未知")
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫監視】檔案結構中須有語系類別資訊才能加歌: " + SongSrcPath;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                else
                {
                    CreateDataRow(SongAddStatus, SongID, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongVolume, SongWordCount, SongPlayCount, SongMB, SongCreatDate, SongSpell, SongSpellNum, SongSongStroke, SongPenStyle, SongPlayState, SongSrcPath, SongSortIndex);
                }
            }
            else
            {
                CreateDataRow(SongAddStatus, SongID, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongVolume, SongWordCount, SongPlayCount, SongMB, SongCreatDate, SongSpell, SongSpellNum, SongSongStroke, SongPenStyle, SongPlayState, SongSrcPath, SongSortIndex);
            }
        }

        public static void CreateDataRow(string SongAddStatus, string SongID, string SongLang, string SongSingerType, string SongSinger, string SongSongName, string SongTrack, string SongSongType, string SongVolume, string SongWordCount, string SongPlayCount, float SongMB, DateTime SongCreatDate, string SongSpell,string SongSpellNum, string SongSongStroke, string SongPenStyle, string SongPlayState, string SongSrcPath, string SongSortIndex)
        {
            lock (LockThis)
            {
                DataRow row = SongAnalysisDT.NewRow();
                row["Song_AddStatus"] = SongAddStatus;
                row["Song_Id"] = SongID;
                row["Song_Lang"] = SongLang;
                row["Song_SingerType"] = SongSingerType;
                row["Song_Singer"] = SongSinger;
                row["Song_SongName"] = SongSongName;
                row["Song_Track"] = SongTrack;
                row["Song_SongType"] = SongSongType;
                row["Song_Volume"] = SongVolume;
                row["Song_WordCount"] = SongWordCount;
                row["Song_PlayCount"] = SongPlayCount;
                row["Song_MB"] = SongMB;
                row["Song_CreatDate"] = SongCreatDate;
                row["Song_Spell"] = SongSpell;
                row["Song_SpellNum"] = SongSpellNum;
                row["Song_SongStroke"] = SongSongStroke;
                row["Song_PenStyle"] = SongPenStyle;
                row["Song_PlayState"] = SongPlayState;
                row["Song_SrcPath"] = SongSrcPath;
                row["Song_SortIndex"] = SongSortIndex;
                SongAnalysisDT.Rows.Add(row);
            }
        }

        #endregion

        #region --- SongAnalysis 取得歌曲資訊 ---

        public static string GetSongInfo(string SongInfoType, string SongInfoValue)
        {
            string infovalue = "";
            List<string> list = new List<string>();

            switch (SongInfoType)
            {
                case "SongLang":
                    foreach (string str in Global.CrazyktvSongLangKeyWordList)
                    {
                        list = new List<string>(str.Split(','));
                        foreach (string liststr in list)
                        {
                            if (SongInfoValue == liststr)
                            {
                                infovalue = Global.CrazyktvSongLangList[Global.CrazyktvSongLangKeyWordList.IndexOf(str)];
                                break;
                            }
                        }
                        if (infovalue != "") break;
                    }
                    break;
                case "SongSingerType":
                    foreach (string str in Global.CrazyktvSingerTypeKeyWordList)
                    {
                        list = new List<string>(str.Split(','));
                        foreach (string liststr in list)
                        {
                            if (SongInfoValue == liststr)
                            {
                                infovalue = Global.CrazyktvSingerTypeKeyWordList.IndexOf(str).ToString();
                                break;
                            }
                        }
                        if (infovalue != "") break;
                    }
                    break;
                case "SongTrack":
                    foreach (string str in Global.CrazyktvSongTrackKeyWordList)
                    {
                        list = new List<string>(str.Split(','));
                        foreach (string liststr in list)
                        {
                            if (SongInfoValue == liststr)
                            {
                                infovalue = Global.CrazyktvSongTrackKeyWordList.IndexOf(str).ToString();
                                break;
                            }
                        }
                        if (infovalue != "") break;
                    }
                    break;
            }
            list.Clear();
            return infovalue;
        }

        #endregion

    }
}
