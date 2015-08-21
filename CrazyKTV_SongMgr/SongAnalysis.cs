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

        public static void CreateSongDataTable()
        {
            Global.SongAddDT = new DataTable();
            Global.SongAddDT.Columns.Add("Song_AddStatus", typeof(string));
            Global.SongAddDT.Columns.Add("Song_Id", typeof(string));
            Global.SongAddDT.Columns.Add("Song_Lang", typeof(string));
            Global.SongAddDT.Columns.Add("Song_SingerType", typeof(int));
            Global.SongAddDT.Columns.Add("Song_Singer", typeof(string));
            Global.SongAddDT.Columns.Add("Song_SongName", typeof(string));
            Global.SongAddDT.Columns.Add("Song_Track", typeof(int));
            Global.SongAddDT.Columns.Add("Song_SongType", typeof(string));
            Global.SongAddDT.Columns.Add("Song_Volume", typeof(int));
            Global.SongAddDT.Columns.Add("Song_WordCount", typeof(int));
            Global.SongAddDT.Columns.Add("Song_PlayCount", typeof(int));
            Global.SongAddDT.Columns.Add("Song_MB", typeof(float));
            Global.SongAddDT.Columns.Add("Song_CreatDate", typeof(DateTime));
            Global.SongAddDT.Columns.Add("Song_FileName", typeof(string));
            Global.SongAddDT.Columns.Add("Song_Path", typeof(string));
            Global.SongAddDT.Columns.Add("Song_Spell", typeof(string));
            Global.SongAddDT.Columns.Add("Song_SpellNum", typeof(string));
            Global.SongAddDT.Columns.Add("Song_SongStroke", typeof(int));
            Global.SongAddDT.Columns.Add("Song_PenStyle", typeof(string));
            Global.SongAddDT.Columns.Add("Song_PlayState", typeof(int));
            Global.SongAddDT.Columns.Add("Song_SrcPath", typeof(string));
            Global.SongAddDT.Columns.Add("Song_SortIndex", typeof(string));

            Global.SongAnalysisSingerList  = new List<string>();
            Global.SongAnalysisSingerLowCaseList = new List<string>();
            Global.SongAnalysisSingerTypeList = new List<string>();

            string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
            Global.AllSingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongAllSingerQuerySqlStr, "");

            foreach (DataRow row in Global.AllSingerDT.AsEnumerable())
            {
                Global.SongAnalysisSingerList.Add(row["Singer_Name"].ToString());
                Global.SongAnalysisSingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                Global.SongAnalysisSingerTypeList.Add(row["Singer_Type"].ToString());
            }

            string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
            Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

            foreach (DataRow row in Global.SingerDT.AsEnumerable())
            {
                if (Global.SongAnalysisSingerLowCaseList.IndexOf(row["Singer_Name"].ToString().ToLower()) < 0)
                {
                    Global.SongAnalysisSingerList.Add(row["Singer_Name"].ToString());
                    Global.SongAnalysisSingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                    Global.SongAnalysisSingerTypeList.Add(row["Singer_Type"].ToString());
                }
            }

            Global.PhoneticsWordList = new List<string>();
            Global.PhoneticsSpellList = new List<string>();
            Global.PhoneticsStrokesList = new List<string>();
            Global.PhoneticsPenStyleList = new List<string>();

            string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
            Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongPhoneticsQuerySqlStr, "");

            var query = from row in Global.PhoneticsDT.AsEnumerable()
                        where row.Field<Int16>("SortIdx") < 2
                        select row;

            foreach (DataRow row in query)
            {
                Global.PhoneticsWordList.Add(row["Word"].ToString());
                Global.PhoneticsSpellList.Add((row["Spell"].ToString()).Substring(0, 1));
                Global.PhoneticsStrokesList.Add(row["Strokes"].ToString());
                Global.PhoneticsPenStyleList.Add((row["PenStyle"].ToString()).Substring(0, 1));
            }
        }

        public static void DisposeSongDataTable()
        {
            Global.SongAnalysisSingerList.Clear();
            Global.SongAnalysisSingerLowCaseList.Clear();
            Global.SongAnalysisSingerTypeList.Clear();
            Global.PhoneticsWordList.Clear();
            Global.PhoneticsSpellList.Clear();
            Global.PhoneticsStrokesList.Clear();
            Global.PhoneticsPenStyleList.Clear();
            Global.SongAddDT.Dispose();
            Global.SingerDT.Dispose();
            Global.AllSingerDT.Dispose();
            Global.PhoneticsDT.Dispose();
        }

        public static void SongInfoAnalysis(string file, List<string> SongLangIDList)
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
            string SongSrcPath = Path.GetFullPath((string)file);
            string SongSortIndex = "";

            string SongLangStr = "";
            string SongSingerTypeStr = "";
            string SongTrackStr = "";
            string SongSongTypeStr = "";

            string SeparateStr = "[-_](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))";

            List<string> SongLangList = (List<string>) SongLangIDList;
            List<string> SingerTypeList = new List<string>() { "男歌星", "男", "女歌星", "女", "樂團", "團體", "團", "合唱", "對唱", "外國男", "外男", "外國女", "外女", "外國樂團", "外團", "未知" };
            List<string> SongTrackList = new List<string>() { "vl", "vr", "v3", "v4", "v5", "左", "右" };
            List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));
            List<string> SongSongTypeLowCaseList = SongSongTypeList.ConvertAll(str => str.ToLower());

            string FileNameRemoveStr = "";
            List<string> FileNameRemoveList = new List<string>();

            string DirStr = Path.GetDirectoryName((string)file);
            List<string> list = new List<string>();

            list = new List<string>(Regex.Split(((string)DirStr), @"\\", RegexOptions.None));
            foreach (string str in list)
            {
                if (SongSingerType != "" & SongSinger != "" & SongLang != "") break;
                string splitstr = Regex.Replace(str, @"%%|^\s*|\s*$", "");
                
                if(Global.SongAnalysisSingerLowCaseList.IndexOf(splitstr.ToLower()) >= 0)
                {
                    SongSinger = Global.SongAnalysisSingerList[Global.SongAnalysisSingerLowCaseList.IndexOf(splitstr.ToLower())];
                    if (SongSingerType == "") SongSingerType = Global.SongAnalysisSingerTypeList[Global.SongAnalysisSingerLowCaseList.IndexOf(splitstr.ToLower())];
                }

                // 查看資料夾名稱中有無語系類別
                if (SongLangList.IndexOf(splitstr) >= 0)
                {
                    SongLang = GetSongInfo("SongLang", splitstr);
                    SongLangStr = splitstr;
                }

                // 查看資料夾名稱中有無歌手類別
                if (SingerTypeList.IndexOf(splitstr) >= 0)
                {
                    if (SongSingerType == "" | GetSongInfo("SongSingerType", splitstr) == "3")
                    {
                        SongSingerType = GetSongInfo("SongSingerType", splitstr);
                        SongSingerTypeStr = splitstr;
                    }
                }
            }

            // 從檔案名稱查找歌曲資訊
            string FileStr = Path.GetFileNameWithoutExtension((string)file);

            // 特殊歌手及歌曲名稱處理
            list = new List<string>(Regex.Split(Global.SongAddSpecialStr, ",", RegexOptions.IgnoreCase));
            foreach (string str in list)
            {
                FileStr = Regex.Replace(FileStr, str, "%%" + str + "%%", RegexOptions.IgnoreCase);
            }

            // 歌手-歌名(語系-類別-聲道) 名稱處理
            FileStr = Regex.Replace(FileStr, @"[\{\(\[｛（［【].+?[_-].+?[】］）｝\]\)\}]", delegate (Match match)
            {
                string str = "%%" + match.ToString() + "%%";
                return str;
            });

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


            list = new List<string>(Regex.Split(((string)FileStrOpti), SeparateStr, RegexOptions.None));
            foreach (string str in list)
            {
                if (SongSingerType != "" & SongSinger != "" & SongLang != "" & SongTrack != "" & SongSongType != "") break;
                string splitstr = Regex.Replace(str, @"%%|^\s*|\s*$", "");

                if (SongSinger == "")
                {
                    if (Global.SongAnalysisSingerLowCaseList.IndexOf(splitstr.ToLower()) >= 0)
                    {
                        SongSinger = Global.SongAnalysisSingerList[Global.SongAnalysisSingerLowCaseList.IndexOf(splitstr.ToLower())];
                        if (SongSingerType == "") SongSingerType = Global.SongAnalysisSingerTypeList[Global.SongAnalysisSingerLowCaseList.IndexOf(splitstr.ToLower())];
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
                            List<string> BracketStrlist = new List<string>(Regex.Split(((string)MatchStr), SeparateStr, RegexOptions.None));
                            string BracketSeparateStr = (MatchStr.Contains("_")) ? "_" : "-";

                            foreach (string BracketStr in BracketStrlist)
                            {
                                if (SongLangList.IndexOf(BracketStr) >= 0)
                                {
                                    SongLang = GetSongInfo("SongLang", BracketStr);
                                    FileNameRemoveStr += ((BracketStrlist.IndexOf(BracketStr) == 0) ? BracketStr : BracketSeparateStr + BracketStr);
                                }
                                else if (SongTrackList.IndexOf(BracketStr.ToLower()) >= 0 || BracketStr.ToLower() == "l" || BracketStr.ToLower() == "r")
                                {
                                    SongTrack = GetSongInfo("SongTrack", BracketStr);
                                    FileNameRemoveStr += ((BracketStrlist.IndexOf(BracketStr) == 0) ? BracketStr : BracketSeparateStr + BracketStr);
                                }
                                else
                                {
                                    if (BracketStr.ContainsAny("合唱", "對唱")) SongSingerType = "3";
                                    SongSongType = BracketStr;
                                    FileNameRemoveStr += ((BracketStrlist.IndexOf(BracketStr) == 0) ? BracketStr : BracketSeparateStr + BracketStr);
                                }
                            }
                            FileNameRemoveList.Add(FileNameRemoveStr);
                        }
                        else
                        {
                            if (SongLangList.IndexOf(MatchStr) >= 0)
                            {
                                SongLang = GetSongInfo("SongLang", MatchStr);
                                FileNameRemoveList.Add(MatchStr);
                            }
                            else
                            {
                                if (matches.Count > 1)
                                {
                                    if (SongTrackList.IndexOf(MatchStr.ToLower()) >= 0 || MatchStr.ToLower() == "l" || MatchStr.ToLower() == "r")
                                    {
                                        SongTrack = GetSongInfo("SongTrack", MatchStr);
                                        FileNameRemoveList.Add(MatchStr);
                                    }
                                    else
                                    {
                                        if (MatchStr.ContainsAny("合唱", "對唱")) SongSingerType = "3";
                                        SongSongType = MatchStr;
                                        FileNameRemoveList.Add(MatchStr);
                                    }
                                }
                                else
                                {
                                    if (MatchStr.ContainsAny("合唱", "對唱")) SongSingerType = "3";
                                    if (SongSongTypeLowCaseList.IndexOf(MatchStr.ToLower()) >= 0)
                                    {
                                        SongSongType = SongSongTypeList[SongSongTypeLowCaseList.IndexOf(MatchStr.ToLower())];
                                        FileNameRemoveList.Add(MatchStr);
                                    }
                                }
                            }
                        }
                    }
                }

                // 查看檔案名稱中有無語系類別
                if (SongLang == "")
                {
                    if (SongLangList.IndexOf(splitstr) >= 0)
                    {
                        SongLang = GetSongInfo("SongLang", splitstr);
                        SongLangStr = splitstr;
                    }
                }

                // 查看檔案名稱中有無歌手類別
                if (SingerTypeList.IndexOf(splitstr) >= 0)
                {
                    if (SongSingerType == "" | GetSongInfo("SongSingerType", splitstr) == "3")
                    {
                        SongSingerType = GetSongInfo("SongSingerType", splitstr);
                        SongSingerTypeStr = splitstr;
                    }
                }

                // 查看檔案名稱中有無歌曲聲道
                if (SongTrack == "")
                {
                    if (SongTrackList.IndexOf(splitstr.ToLower()) >= 0)
                    {
                        SongTrack = GetSongInfo("SongTrack", splitstr);
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

                // 去除括號內要移除的字串
                if (FileNameRemoveList.Count > 0)
                {
                    foreach (string RemoveStr in FileNameRemoveList)
                    {
                        str = Regex.Replace(str, @"[\{\(\[｛（［【]" + RemoveStr + @"[】］）｝\]\)\}]", "", RegexOptions.IgnoreCase);
                    }
                }
                
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
                                    if (r.IsMatch(list[0])) SongSingerType = "3";
                                    SongSinger = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
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
                                SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                break;
                        }
                        break;
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
                                    if (r.IsMatch(list[1])) SongSingerType = "3";
                                    SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                }
                                else
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(list[1]))
                                    {
                                        SongSingerType = "3";
                                        SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                SongSongName = Regex.Replace(list[0], @"%%|^\s*|\s*$", "");
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
                                SongSongName = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
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
                                }
                                else
                                {
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                                    if (r.IsMatch(list[1]))
                                    {
                                        SongSingerType = "3";
                                        SongSinger = Regex.Replace(list[1], @"%%|^\s*|\s*$", "");
                                    }
                                }
                                SongSongName = Regex.Replace(list[2], @"%%|^\s*|\s*$", "");
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
                string str = CommonFunc.GetSingerTypeStr(int.Parse(Global.SongAddDefaultSingerType) - 1, 0, "null");
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
            if (SongSortIndex == "") SongSortIndex = "3";

            // 加歌狀態
            if (SongLang == "未知") SongAddStatus = "語系類別必須有值才能加歌!";
            if (SongAddStatus == "") { if (SongSingerType == "10") SongAddStatus = "此歌手尚未設定歌手資料!";}

            CreateDataRow(SongAddStatus, SongID, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongVolume, SongWordCount, SongPlayCount, SongMB, SongCreatDate, SongSpell, SongSpellNum, SongSongStroke, SongPenStyle, SongPlayState, SongSrcPath, SongSortIndex);
        }

        public static void CreateDataRow(string SongAddStatus, string SongID, string SongLang, string SongSingerType, string SongSinger, string SongSongName, string SongTrack, string SongSongType, string SongVolume, string SongWordCount, string SongPlayCount, float SongMB, DateTime SongCreatDate, string SongSpell,string SongSpellNum, string SongSongStroke, string SongPenStyle, string SongPlayState, string SongSrcPath, string SongSortIndex)
        {
            lock (LockThis)
            {
                DataRow row = Global.SongAddDT.NewRow();
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
                Global.SongAddDT.Rows.Add(row);
            }
        }


        public static string GetSongInfo(string SongInfoType, string SongInfoValue)
        {
            string infovalue = "";
            switch (SongInfoType)
            {
                case "SongLang":
                    List<string> list = new List<string>();
                    foreach (string str in Global.CrazyktvSongLangIDList)
                    {
                        list = new List<string>(str.Split(','));
                        foreach (string liststr in list)
                        {
                            if (SongInfoValue == liststr)
                            {
                                infovalue = Global.CrazyktvSongLangList[Global.CrazyktvSongLangIDList.IndexOf(str)];
                                break;
                            }
                        }
                        if (infovalue != "") break;
                    }
                    break;
                case "SongSingerType":
                    switch (SongInfoValue)
                    {
                        case "男歌星":
                        case "男":
                            infovalue = "0";
                            break;
                        case "女歌星":
                        case "女":
                            infovalue = "1";
                            break;
                        case "樂團":
                        case "團體":
                        case "團":
                            infovalue = "2";
                            break;
                        case "合唱":
                        case "對唱":
                            infovalue = "3";
                            break;
                        case "外國男":
                        case "外男":
                            infovalue = "4";
                            break;
                        case "外國女":
                        case "外女":
                            infovalue = "5";
                            break;
                        case "外國樂團":
                        case "外團":
                            infovalue = "6";
                            break;
                        case "未知":
                            infovalue = "7";
                            break;
                    }
                    break;
                case "SongTrack":
                    switch (SongInfoValue.ToLower())
                    {
                        case "vl":
                        case "l":
                        case "左":
                            if (Global.SongMgrSongTrackMode == "True") { infovalue = "2"; } else { infovalue = "1"; }
                            break;
                        case "vr":
                        case "r":
                        case "右":
                            if (Global.SongMgrSongTrackMode == "True") { infovalue = "1"; } else { infovalue = "2"; }
                            break;
                        case "v3":
                            infovalue = "3";
                            break;
                        case "v4":
                            infovalue = "4";
                            break;
                        case "v5":
                            infovalue = "5";
                            break;
                    }
                    break;
                
                    
                    
            }
            return infovalue;
        }

     


    }
}
