using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

            string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
            Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

            string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
            Global.AllSingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongAllSingerQuerySqlStr, "");

            string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
            Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongPhoneticsQuerySqlStr, "");
        }

        public static void DisposeSongDataTable()
        {
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
            string SongLangRemoveStr = "";
            string SongSongTypeRemoveStr = "";
            
            string SeparateStr = "[-_](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))";

            List<string> SongLangList = (List<string>) SongLangIDList;
            List<string> AnalysisList = new List<string>() { "男歌星", "男", "女歌星", "女", "樂團", "團體", "團", "合唱", "對唱", "外國男", "外男", "外國女", "外女", "外國樂團", "外團", "未知" };
            List<string> SongTrackList = new List<string>() { "vl", "vr", "v3", "v4", "v5" };
            List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));

            string DirStr = Path.GetDirectoryName((string)file);
            List<string> list = new List<string>();

            list = new List<string>(Regex.Split(((string)DirStr), @"\\", RegexOptions.None));
            foreach (string str in list)
            {
                if (SongSingerType != "" & SongSinger != "" & SongLang != "") break;
                string splitstr = Regex.Replace(str, @"%%|^\s*|\s*$", "");
                
                // 查找資料庫歌手表
                var query = from row in Global.SingerDT.AsEnumerable()
                            where row.Field<string>("Singer_Name").ToLower().Equals(splitstr.ToLower())
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        if (SongSingerType == "") SongSingerType = row["Singer_Type"].ToString();
                        SongSinger = row["Singer_Name"].ToString();
                        break;
                    }
                }

                // 查找資料庫所有歌手表
                if(SongSinger == "")
                {
                    var queryall = from row in Global.AllSingerDT.AsEnumerable()
                                   where row.Field<string>("Singer_Name").ToLower().Equals(splitstr.ToLower())
                                   select row;

                    if (queryall.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in queryall)
                        {
                            if (SongSingerType == "") SongSingerType = row["Singer_Type"].ToString();
                            SongSinger = row["Singer_Name"].ToString();
                            break;
                        }
                    }
                }

                // 查看資料夾名稱中有無語系類別
                foreach (string s in SongLangList)
                {
                    if (s == splitstr)
                    {
                        SongLang = GetSongInfo("SongLang", s);
                        SongLangStr = s;
                        if (SongLang != "") break;
                    }
                }

                // 查看資料夾名稱中有無歌手類別
                foreach (string s in AnalysisList)
                {
                    if (s == splitstr)
                    {
                        if (SongSingerType == "" | GetSongInfo("SongSingerType", s) == "3")
                        {
                            SongSingerType = GetSongInfo("SongSingerType", s);
                            SongSingerTypeStr = s;
                        }
                        break;
                    }
                }
            }

            string FileStr = Path.GetFileNameWithoutExtension((string)file);

            // 特殊歌手及歌曲名稱處理
            list = new List<string>(Regex.Split(Global.SongAddSpecialStr, ",", RegexOptions.IgnoreCase));
            foreach (string str in list)
            {
                FileStr = Regex.Replace(FileStr, str, "%%" + str + "%%", RegexOptions.IgnoreCase);
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


            list = new List<string>(Regex.Split(((string)FileStrOpti), SeparateStr, RegexOptions.None));
            foreach (string str in list)
            {
                if (SongSingerType != "" & SongSinger != "" & SongLang != "" & SongTrack != "" & SongSongType != "") break;
                string splitstr = Regex.Replace(str, @"%%|^\s*|\s*$", "");

                // 查找資料庫歌手表
                if (SongSinger == "")
                {
                    var query = from row in Global.SingerDT.AsEnumerable()
                                where row.Field<string>("Singer_Name").ToLower().Equals(splitstr.ToLower())
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            if (SongSingerType == "") SongSingerType = row["Singer_Type"].ToString();
                            SongSinger = row["Singer_Name"].ToString();
                            break;
                        }
                    }

                    // 查找資料庫所有歌手表
                    if (SongSinger == "")
                    {
                        var queryall = from row in Global.AllSingerDT.AsEnumerable()
                                       where row.Field<string>("Singer_Name").ToLower().Equals(splitstr.ToLower())
                                       select row;

                        if (queryall.Count<DataRow>() > 0)
                        {
                            foreach (DataRow row in queryall)
                            {
                                if (SongSingerType == "") SongSingerType = row["Singer_Type"].ToString();
                                SongSinger = row["Singer_Name"].ToString();
                                break;
                            }
                        }
                    }
                }
                
                // 查看檔案名稱中有無語系類別
                if (SongLang == "")
                {
                    foreach (string s in SongLangList)
                    {
                        if (s == splitstr)
                        {
                            SongLang = GetSongInfo("SongLang", s);
                            SongLangStr = s;
                            if (SongLang != "") break;
                        }
                        // 歌名[歌曲類別][語系] 處理
                        MatchCollection matches = Regex.Matches(splitstr, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", RegexOptions.IgnoreCase);
                        if (matches.Count > 0)
                        {
                            Match m = Regex.Match(splitstr, @"[\{\(\[｛（［【]" + s + @"[】］）｝\]\)\}]", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                SongLang = GetSongInfo("SongLang", s);
                                SongLangRemoveStr = s;

                                switch (matches.Count)
                                {
                                    case 2:
                                        foreach (Match match in matches)
                                        {
                                            if (match.Value != m.Value & match.Value == matches[0].Value)
                                            {
                                                SongSongType = Regex.Replace(match.Value, @"^[\{\(\[｛（［【]|[】］）｝\]\)\}]$", "", RegexOptions.IgnoreCase);
                                                SongSongTypeRemoveStr = SongSongType;
                                                if (SongSongTypeRemoveStr != "") break;
                                            }
                                        }
                                        break;
                                }
                            }
                            if (SongLang != "") break;
                        }
                    }
                }

                // 查看檔案名稱中有無歌手類別
                foreach (string s in AnalysisList)
                {
                    if (s == splitstr)
                    {
                        if (SongSingerType == "" | GetSongInfo("SongSingerType", s) == "3")
                        {
                            SongSingerType = GetSongInfo("SongSingerType", s);
                            SongSingerTypeStr = s;
                        }
                        break;
                    }
                }
                
                // 查看檔案名稱中有無歌曲聲道
                if (SongTrack == "")
                {
                    foreach (string s in SongTrackList)
                    {
                        if (string.Compare(s, splitstr, true) == 0)
                        {
                            SongTrack = GetSongInfo("SongTrack", s);
                            SongTrackStr = s;
                            if (SongTrack != "") break;
                        }
                    }
                }


                // 查看檔案名稱中有無歌曲類別
                if (SongSongType == "")
                {
                    foreach (string s in SongSongTypeList)
                    {
                        if (string.Compare(s, splitstr, true) == 0)
                        {
                            SongSongType = s;
                            SongSongTypeStr = s;
                            if (SongSongType != "") break;
                        }
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

                // 歌名[歌曲類別][語系] 處理
                if (SongLangRemoveStr != "")
                {
                    str = Regex.Replace(str, @"[\{\(\[｛（［【]" + SongLangRemoveStr + @"[】］）｝\]\)\}]", "", RegexOptions.IgnoreCase);
                }

                if (SongSongTypeRemoveStr != "")
                {
                    str = Regex.Replace(str, @"[\{\(\[｛（［【]" + SongSongTypeRemoveStr + @"[】］）｝\]\)\}]", "", RegexOptions.IgnoreCase);
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
                    switch (SongInfoValue)
                    {
                        case "vl":
                            if (Global.SongMgrSongTrackMode == "True") { infovalue = "2"; } else { infovalue = "1"; }
                            break;
                        case "vr":
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
