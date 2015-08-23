using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    class SongAddSong
    {
        private static object LockThis = new object();

        public static void CreateSongDataTable()
        {
            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_Singer, Song_SongName, Song_SongType, Song_FileName, Song_Path from ktv_Song";
            Global.SongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            Global.SongAddAllSongIDList = new List<string>();
            foreach (DataRow row in Global.SongDT.AsEnumerable())
            {
                Global.SongAddAllSongIDList.Add(row["Song_Id"].ToString());
            }

            string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
            Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

            string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
            Global.AllSingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongAllSingerQuerySqlStr, "");

            Global.DuplicateSongDT = new DataTable();
            Global.DuplicateSongDT.Columns.Add(new DataColumn("Display", typeof(string)));
            Global.DuplicateSongDT.Columns.Add(new DataColumn("Value", typeof(int)));

            Global.FailureSongDT = new DataTable();
            Global.FailureSongDT.Columns.Add(new DataColumn("Display", typeof(string)));
            Global.FailureSongDT.Columns.Add(new DataColumn("Value", typeof(int)));

            Global.DupSongAddDT = new DataTable();
            Global.DupSongAddDT.Columns.Add("Song_AddStatus", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_Id", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_Lang", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_SingerType", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_Singer", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_SongName", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_Track", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_SongType", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_Volume", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_WordCount", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_PlayCount", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_MB", typeof(float));
            Global.DupSongAddDT.Columns.Add("Song_CreatDate", typeof(DateTime));
            Global.DupSongAddDT.Columns.Add("Song_FileName", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_Path", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_Spell", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_SpellNum", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_SongStroke", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_PenStyle", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_PlayState", typeof(int));
            Global.DupSongAddDT.Columns.Add("Song_SrcPath", typeof(string));
            Global.DupSongAddDT.Columns.Add("Song_SortIndex", typeof(string));
        }

        public static void DisposeSongDataTable()
        {
            Global.SongAddAllSongIDList.Clear();
            Global.SongDT.Dispose();
            Global.SongDT = null;
            Global.SingerDT.Dispose();
            Global.SingerDT = null;
            Global.AllSingerDT.Dispose();
            Global.AllSingerDT = null;
            Global.DupSongAddDT.Dispose();
            Global.DupSongAddDT = null;
        }

        public static void StartAddSong(int i)
        {
            // 判斷是否為重複歌曲
            string DuplicateSong = "";
            string DuplicateSongId = "";
            string DuplicateSongFileName = "";
            string DuplicateSongPath = "";
            float DuplicateSongMB = 0;
            List<string> ChorusSingerList = new List<string>() { "未有合唱歌手資料" };

            var query = from row in Global.SongDT.AsEnumerable()
                        where row.Field<string>("Song_Lang").Equals(Global.SongAddDT.Rows[i].Field<string>("Song_Lang")) &&
                              row.Field<string>("Song_Singer").ToLower().Equals(Global.SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower()) &&
                              row.Field<string>("Song_SongName").ToLower().Equals(Global.SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower())
                        select row;

            if (query.Count<DataRow>() == 0 & Global.SongAddDT.Rows[i].Field<int>("Song_SingerType") == 3)
            {
                Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                if (r.Matches(Global.SongAddDT.Rows[i].Field<string>("Song_Singer")).Count > 0)
                {
                    ChorusSingerList = new List<string>(Regex.Split(Global.SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower(), "&", RegexOptions.None));
                }

                query = from row in Global.SongDT.AsEnumerable()
                        where row.Field<string>("Song_Lang").Equals(Global.SongAddDT.Rows[i].Field<string>("Song_Lang")) &&
                              row.Field<string>("Song_Singer").ToLower().ContainsAll(ChorusSingerList.ToArray()) &&
                              row.Field<string>("Song_Singer").Length.Equals(Global.SongAddDT.Rows[i].Field<string>("Song_Singer").Length) &&
                              row.Field<string>("Song_SongName").ToLower().Equals(Global.SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower())
                        select row;
            }

            if (query.Count<DataRow>() == 0 && Global.SongQuerySynonymousQuery)
            {
                List<string> SynonymousSongNameList = new List<string>();
                SynonymousSongNameList = CommonFunc.GetSynonymousSongNameList(Global.SongAddDT.Rows[i].Field<string>("Song_SongName"));
                List<string> SynonymousSongNameLowCaseList = SynonymousSongNameList.ConvertAll(str => str.ToLower());

                if (SynonymousSongNameList.Count > 0)
                {
                    query = from row in Global.SongDT.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals(Global.SongAddDT.Rows[i].Field<string>("Song_Lang")) &&
                                  row.Field<string>("Song_Singer").ToLower().Equals(Global.SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower()) &&
                                  row.Field<string>("Song_SongName").ToLower().ContainsAny(SynonymousSongNameLowCaseList.ToArray())
                            select row;
                }
            }

            if (query.Count<DataRow>() > 0)
            {
                foreach (DataRow row in query)
                {
                    if (row["Song_SongType"] == null)
                    {
                        if (Global.SongAddDT.Rows[i].Field<string>("Song_SongType") == "")
                        {
                            DuplicateSong = "DuplicateSong";
                            DuplicateSongId = row.Field<string>("Song_Id");
                            DuplicateSongFileName = row.Field<string>("Song_FileName");
                            DuplicateSongPath = row.Field<string>("Song_Path");
                            string file = Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"));
                            if (File.Exists(file))
                            {
                                FileInfo f = new FileInfo(file);
                                DuplicateSongMB = float.Parse(((f.Length / 1024f) / 1024f).ToString("F2"));
                            }
                        }
                    }
                    else
                    {
                        if (row["Song_SongType"].ToString() == Global.SongAddDT.Rows[i].Field<string>("Song_SongType"))
                        {
                            DuplicateSong = "DuplicateSong";
                            DuplicateSongId = row.Field<string>("Song_Id");
                            DuplicateSongFileName = row.Field<string>("Song_FileName");
                            DuplicateSongPath = row.Field<string>("Song_Path");
                            string file = Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"));
                            if (File.Exists(file))
                            {
                                FileInfo f = new FileInfo(file);
                                DuplicateSongMB = float.Parse(((f.Length / 1024f) / 1024f).ToString("F2"));
                            }
                        }
                    }
                    if (DuplicateSong == "DuplicateSong") break;
                }
            }

            if (DuplicateSong == "DuplicateSong")
            {
                switch (Global.SongAddDupSongMode)
                {
                    case "1":
                        lock (LockThis)
                        {
                            Global.TotalList[1]++;
                            Global.DuplicateSongDT.Rows.Add(Global.DuplicateSongDT.NewRow());
                            Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][0] = Global.SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                            Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][1] = Global.DuplicateSongDT.Rows.Count;
                        }
                        break;
                    case "2":
                        DataRow row2 = Global.DupSongAddDT.NewRow();
                        row2["Song_AddStatus"] = "重複歌曲ID: " + DuplicateSongId;
                        row2["Song_Id"] = DuplicateSongId;
                        row2["Song_Lang"] = Global.SongAddDT.Rows[i].Field<string>("Song_Lang");
                        row2["Song_SingerType"] = Global.SongAddDT.Rows[i].Field<int>("Song_SingerType");
                        row2["Song_Singer"] = Global.SongAddDT.Rows[i].Field<string>("Song_Singer");
                        row2["Song_SongName"] = Global.SongAddDT.Rows[i].Field<string>("Song_SongName");
                        row2["Song_Track"] = Global.SongAddDT.Rows[i].Field<int>("Song_Track");
                        row2["Song_SongType"] = Global.SongAddDT.Rows[i].Field<string>("Song_SongType");
                        row2["Song_Volume"] = Global.SongAddDT.Rows[i].Field<int>("Song_Volume");
                        row2["Song_WordCount"] = Global.SongAddDT.Rows[i].Field<int>("Song_WordCount");
                        row2["Song_PlayCount"] = Global.SongAddDT.Rows[i].Field<int>("Song_PlayCount");
                        row2["Song_MB"] = Global.SongAddDT.Rows[i].Field<float>("Song_MB");
                        row2["Song_CreatDate"] = Global.SongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
                        row2["Song_FileName"] = DuplicateSongFileName;
                        row2["Song_Path"] = DuplicateSongPath;
                        row2["Song_Spell"] = Global.SongAddDT.Rows[i].Field<string>("Song_Spell");
                        row2["Song_SpellNum"] = Global.SongAddDT.Rows[i].Field<string>("Song_SpellNum");
                        row2["Song_SongStroke"] = Global.SongAddDT.Rows[i].Field<int>("Song_SongStroke");
                        row2["Song_PenStyle"] = Global.SongAddDT.Rows[i].Field<string>("Song_PenStyle");
                        row2["Song_PlayState"] = Global.SongAddDT.Rows[i].Field<int>("Song_PlayState");
                        row2["Song_SrcPath"] = Global.SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                        row2["Song_SortIndex"] = "1";
                        Global.DupSongAddDT.Rows.Add(row2);
                        break;
                    case "3":
                        if (Global.SongAddDT.Rows[i].Field<float>("Song_MB") > DuplicateSongMB)
                        {
                            DataRow row3 = Global.DupSongAddDT.NewRow();
                            row3["Song_AddStatus"] = "重複歌曲";
                            row3["Song_Id"] = DuplicateSongId;
                            row3["Song_Lang"] = Global.SongAddDT.Rows[i].Field<string>("Song_Lang");
                            row3["Song_SingerType"] = Global.SongAddDT.Rows[i].Field<int>("Song_SingerType");
                            row3["Song_Singer"] = Global.SongAddDT.Rows[i].Field<string>("Song_Singer");
                            row3["Song_SongName"] = Global.SongAddDT.Rows[i].Field<string>("Song_SongName");
                            row3["Song_Track"] = Global.SongAddDT.Rows[i].Field<int>("Song_Track");
                            row3["Song_SongType"] = Global.SongAddDT.Rows[i].Field<string>("Song_SongType");
                            row3["Song_Volume"] = Global.SongAddDT.Rows[i].Field<int>("Song_Volume");
                            row3["Song_WordCount"] = Global.SongAddDT.Rows[i].Field<int>("Song_WordCount");
                            row3["Song_PlayCount"] = Global.SongAddDT.Rows[i].Field<int>("Song_PlayCount");
                            row3["Song_MB"] = Global.SongAddDT.Rows[i].Field<float>("Song_MB");
                            row3["Song_CreatDate"] = Global.SongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
                            row3["Song_FileName"] = DuplicateSongFileName;
                            row3["Song_Path"] = DuplicateSongPath;
                            row3["Song_Spell"] = Global.SongAddDT.Rows[i].Field<string>("Song_Spell");
                            row3["Song_SpellNum"] = Global.SongAddDT.Rows[i].Field<string>("Song_SpellNum");
                            row3["Song_SongStroke"] = Global.SongAddDT.Rows[i].Field<int>("Song_SongStroke");
                            row3["Song_PenStyle"] = Global.SongAddDT.Rows[i].Field<string>("Song_PenStyle");
                            row3["Song_PlayState"] = Global.SongAddDT.Rows[i].Field<int>("Song_PlayState");
                            row3["Song_SrcPath"] = Global.SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                            row3["Song_SortIndex"] = "1";
                            Global.DupSongAddDT.Rows.Add(row3);
                        }
                        else
                        {
                            lock (LockThis)
                            {
                                Global.TotalList[1]++;
                                Global.DuplicateSongDT.Rows.Add(Global.DuplicateSongDT.NewRow());
                                Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][0] = Global.SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                                Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][1] = Global.DuplicateSongDT.Rows.Count;
                            }
                        }
                        break;
                }
            }
            else
            {
                string SongID = Global.SongAddDT.Rows[i].Field<string>("Song_Id"); ;
                string SongLang = Global.SongAddDT.Rows[i].Field<string>("Song_Lang");
                int SongSingerType = Global.SongAddDT.Rows[i].Field<int>("Song_SingerType");
                string SongSinger = Global.SongAddDT.Rows[i].Field<string>("Song_Singer");
                string SongSongName = Global.SongAddDT.Rows[i].Field<string>("Song_SongName");
                int SongTrack = Global.SongAddDT.Rows[i].Field<int>("Song_Track");
                string SongSongType = Global.SongAddDT.Rows[i].Field<string>("Song_SongType");
                int SongVolume = Global.SongAddDT.Rows[i].Field<int>("Song_Volume");
                int SongWordCount = Global.SongAddDT.Rows[i].Field<int>("Song_WordCount");
                int SongPlayCount = Global.SongAddDT.Rows[i].Field<int>("Song_PlayCount");
                float SongMB = Global.SongAddDT.Rows[i].Field<float>("Song_MB");
                DateTime SongCreatDate = Global.SongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
                string SongFileName = "";
                string SongPath = "";
                string SongSpell = Global.SongAddDT.Rows[i].Field<string>("Song_Spell");
                string SongSpellNum = Global.SongAddDT.Rows[i].Field<string>("Song_SpellNum");
                int SongSongStroke = Global.SongAddDT.Rows[i].Field<int>("Song_SongStroke");
                string SongPenStyle = Global.SongAddDT.Rows[i].Field<string>("Song_PenStyle");
                int SongPlayState = Global.SongAddDT.Rows[i].Field<int>("Song_PlayState");

                string SongAddSinger = "0";
                string SongAddAllSinger = "0";
                
                string SongSrcPath = Global.SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                string SongExtension = Path.GetExtension(SongSrcPath);


                // 判斷是否要加入歌手資料至歌手資料庫
                if (SongSingerType != 3)
                {
                    // 查找資料庫歌手表
                    var querysinger = from row in Global.SingerDT.AsEnumerable()
                                      where row.Field<string>("Singer_Name").ToLower().Equals(SongSinger.ToLower())
                                      select row;

                    if (querysinger.Count<DataRow>() == 0) SongAddSinger = "1";

                    /* 查找資料庫所有歌手表
                    var querysingerall = from row in Global.AllSingerDT.AsEnumerable()
                                         where row.Field<string>("Singer_Name").ToLower().Equals(SongSinger.ToLower())
                                         select row;

                    if (querysingerall.Count<DataRow>() == 0) SongAddAllSinger = "1";
                    */
                }
                else
                {
                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                    if (r.IsMatch(SongSinger))
                    {
                        string[] singers = Regex.Split(SongSinger, "&", RegexOptions.None);
                        foreach (string str in singers)
                        {
                            if(Global.SongAddChorusSingerList.IndexOf(str) == -1)
                            {
                                Global.SongAddChorusSingerList.Add(str);
                            }
                        }
                    }
                }

                // 判斷是否要新增歌曲類別
                if (SongSongType != "")
                {
                    Regex r = new Regex("^" + SongSongType + ",|," + SongSongType + ",|," + SongSongType + "$");
                    if (!r.IsMatch(Global.SongMgrSongType))
                    {
                        Global.SongMgrSongType = Global.SongMgrSongType + "," + SongSongType;
                    }
                }

                bool UseCustomSongID = false;
                if (Global.SongAddUseCustomSongID == "True")
                {
                    if (SongID != "")
                    {
                        List<string> StartIdlist = new List<string>();
                        StartIdlist = new List<string>(Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));

                        switch (Global.SongMgrMaxDigitCode)
                        {
                            case "1":
                                if (SongID.Length == 5 & Global.SongAddAllSongIDList.IndexOf(SongID) < 0)
                                {
                                    if (Global.CrazyktvSongLangList.IndexOf(SongLang) < 9)
                                    {
                                        if (Convert.ToInt32(SongID) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongID) < Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang) + 1]))
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(SongID) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongID) < 100000)
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                }
                                break;
                            case "2":
                                if (SongID.Length == 6 & Global.SongAddAllSongIDList.IndexOf(SongID) < 0)
                                {
                                    if (Global.CrazyktvSongLangList.IndexOf(SongLang) < 9)
                                    {
                                        if (Convert.ToInt32(SongID) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongID) < Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang) + 1]))
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(SongID) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongID) < 1000000)
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                
                // 查詢歌曲編號有無斷號
                if (!UseCustomSongID)
                {
                    SongID = "";
                    lock (LockThis)
                    {
                        if (Global.NotExistsSongIdDT.Rows.Count != 0)
                        {
                            string RemoveRowindex = "";
                            var Query = from row in Global.NotExistsSongIdDT.AsEnumerable()
                                        where row.Field<string>("Song_Lang").Equals(SongLang)
                                        orderby row.Field<string>("Song_Id")
                                        select row;

                            foreach (DataRow row in Query)
                            {
                                SongID = row["Song_Id"].ToString();
                                RemoveRowindex = Global.NotExistsSongIdDT.Rows.IndexOf(row).ToString();
                                break;
                            }
                            if (RemoveRowindex != "")
                            {
                                DataRow row = Global.NotExistsSongIdDT.Rows[Convert.ToInt32(RemoveRowindex)];
                                Global.NotExistsSongIdDT.Rows.Remove(row);
                            }
                        }
                    }

                    // 若無斷號查詢各語系下個歌曲編號
                    if (SongID == "")
                    {
                        string MaxDigitCode = "";
                        switch (Global.SongMgrMaxDigitCode)
                        {
                            case "1":
                                MaxDigitCode = "D5";
                                break;
                            case "2":
                                MaxDigitCode = "D6";
                                break;
                        }

                        foreach (string langstr in Global.CrazyktvSongLangList)
                        {
                            if (langstr == SongLang)
                            {
                                int LangIndex = Global.CrazyktvSongLangList.IndexOf(langstr);
                                Global.MaxIDList[LangIndex]++;
                                SongID = Global.MaxIDList[LangIndex].ToString(MaxDigitCode);
                                break;
                            }
                        }
                    }
                }

                string SongSingerStr = SongSinger;
                string SingerTypeStr = CommonFunc.GetSingerTypeStr(SongSingerType, 2, "null");
                string CrtchorusSeparate;
                string SongInfoSeparate;
                if (Global.SongMgrChorusSeparate == "1") { CrtchorusSeparate = "&"; } else { CrtchorusSeparate = "+"; }
                if (Global.SongMgrSongInfoSeparate == "1") { SongInfoSeparate = "_"; } else { SongInfoSeparate = "-"; }
                string SongTrackStr = CommonFunc.GetSongTrackStr(SongTrack -1, 1, "null");

                if (SongSingerType == 3)
                {
                    SongSingerStr = Regex.Replace(SongSinger, "[&+]", CrtchorusSeparate, RegexOptions.IgnoreCase);
                }

                if (Global.SongMgrSongAddMode == "3")
                {
                    SongPath = Path.GetDirectoryName(SongSrcPath) + @"\";
                    SongFileName = Path.GetFileName(SongSrcPath);
                }
                else
                {
                    switch (Global.SongMgrFolderStructure)
                    {
                        case "1":
                            if (Global.SongMgrChorusMerge == "True" & SongSingerType == 3)
                            {
                                SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\";
                            }
                            else
                            {
                                SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\" + SongSingerStr + @"\";
                            }
                            break;
                        case "2":
                            SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\";
                            break;
                    }

                    switch (Global.SongMgrFileStructure)
                    {
                        case "1":
                            if (SongSongType == "")
                            {
                                SongFileName = SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                        case "2":
                            if (SongSongType == "")
                            {
                                SongFileName = SongSongName + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongSongName + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                        case "3":
                            if (SongSongType == "")
                            {
                                SongFileName = SongID + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongID + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                    }
                }

                if (SongSinger.Length > 60)
                {
                    Global.TotalList[2]++;
                    Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "歌手名稱超過60字元: " + SongSrcPath;
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                }
                else if (SongSongName.Length > 80)
                {
                    Global.TotalList[2]++;
                    Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "歌曲名稱超過80字元: " + SongSrcPath;
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                }
                else
                {
                    bool FileIOError = false;
                    if (Global.SongMgrSongAddMode != "3")
                    {
                        string SongDestPath = Path.Combine(SongPath, SongFileName);

                        if (!Directory.Exists(SongPath))
                        {
                            Directory.CreateDirectory(SongPath);
                        }
                        try
                        {
                            switch (Global.SongMgrSongAddMode)
                            {
                                case "1":
                                    if (File.Exists(SongDestPath))
                                    {
                                        FileAttributes attributes = File.GetAttributes(SongDestPath);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(SongDestPath, attributes);
                                        }

                                        if (Global.SongMgrBackupRemoveSong == "True")
                                        {
                                            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                                            if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                            File.Move(SongDestPath, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                        }
                                        else
                                        {
                                            File.Delete(SongDestPath);
                                        }
                                    }
                                    File.Move(SongSrcPath, SongDestPath);
                                    break;
                                case "2":
                                    if (File.Exists(SongDestPath))
                                    {
                                        FileAttributes attributes = File.GetAttributes(SongDestPath);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(SongDestPath, attributes);
                                        }

                                        if (Global.SongMgrBackupRemoveSong == "True")
                                        {
                                            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                                            if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                            File.Move(SongDestPath, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                        }
                                    }
                                    File.Copy(SongSrcPath, SongDestPath, true);
                                    break;
                            }
                        }
                        catch
                        {
                            FileIOError = true;
                            lock (LockThis) { Global.TotalList[2]++; }
                            Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "檔案處理發生錯誤: " + SongDestPath + "(唯讀或使用中)";
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                        }
                    }

                    if (!FileIOError)
                    {
                        string SongAddValue = SongID + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongAddSinger + "|" + SongAddAllSinger;
                        Global.SongAddValueList.Add(SongAddValue);
                        lock (LockThis)
                        {
                            Global.SongAddAllSongIDList.Add(SongID);
                            Global.TotalList[0]++;
                        }
                    }
                }
            }
        }

        public static void StartUpdateSong(int i)
        {
            string SongId = Global.DupSongAddDT.Rows[i].Field<string>("Song_Id");
            string SongLang = Global.DupSongAddDT.Rows[i].Field<string>("Song_Lang");
            int SongSingerType = Global.DupSongAddDT.Rows[i].Field<int>("Song_SingerType");
            string SongSinger = Global.DupSongAddDT.Rows[i].Field<string>("Song_Singer");
            string SongSongName = Global.DupSongAddDT.Rows[i].Field<string>("Song_SongName");
            int SongTrack = Global.DupSongAddDT.Rows[i].Field<int>("Song_Track");
            string SongSongType = Global.DupSongAddDT.Rows[i].Field<string>("Song_SongType");
            int SongVolume = Global.DupSongAddDT.Rows[i].Field<int>("Song_Volume");
            int SongWordCount = Global.DupSongAddDT.Rows[i].Field<int>("Song_WordCount");
            int SongPlayCount = Global.DupSongAddDT.Rows[i].Field<int>("Song_PlayCount");
            float SongMB = Global.DupSongAddDT.Rows[i].Field<float>("Song_MB");
            DateTime SongCreatDate = Global.DupSongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
            string SongFileName = Global.DupSongAddDT.Rows[i].Field<string>("Song_FileName");
            string SongPath = Global.DupSongAddDT.Rows[i].Field<string>("Song_Path");
            string SongSpell = Global.DupSongAddDT.Rows[i].Field<string>("Song_Spell");
            string SongSpellNum = Global.DupSongAddDT.Rows[i].Field<string>("Song_SpellNum");
            int SongSongStroke = Global.DupSongAddDT.Rows[i].Field<int>("Song_SongStroke");
            string SongPenStyle = Global.DupSongAddDT.Rows[i].Field<string>("Song_PenStyle");
            int SongPlayState = Global.DupSongAddDT.Rows[i].Field<int>("Song_PlayState");

            string SongSrcPath = Global.DupSongAddDT.Rows[i].Field<string>("Song_SrcPath");
            string SongExtension = Path.GetExtension(SongSrcPath);

            // 判斷是否要新增歌曲類別
            if (SongSongType != "")
            {
                Regex r = new Regex("^" + SongSongType + ",|," + SongSongType + ",|," + SongSongType + "$");
                if (!r.IsMatch(Global.SongMgrSongType))
                {
                    Global.SongMgrSongType = Global.SongMgrSongType + "," + SongSongType;
                }
            }

            // 移除原有歌曲
            bool DeleteError = false;
            string oldfile = Path.Combine(SongPath, SongFileName);
            if (File.Exists(oldfile))
            {
                FileAttributes attributes = File.GetAttributes(oldfile);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                    File.SetAttributes(oldfile, attributes);
                }

                try
                {
                    if (Global.SongMgrBackupRemoveSong == "True")
                    {
                        if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                        if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                        File.Move(oldfile, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                    }
                    else
                    {
                        File.Delete(oldfile);
                    }
                }
                catch
                {
                    DeleteError = true;
                    lock (LockThis) { Global.TotalList[2]++; }
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【加歌頁面】重複歌曲原有檔案是唯讀檔或正在使用中: " + oldfile;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }
            else
            {
                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【加歌頁面】重複歌曲原有檔案不存在,已自動忽略移除原有檔案: " + oldfile;
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
            }

            if (!DeleteError)
            {
                lock (LockThis) { Global.TotalList[4]++; }
                string SongSingerStr = SongSinger;
                string SingerTypeStr = CommonFunc.GetSingerTypeStr(SongSingerType, 2, "null");
                string CrtchorusSeparate;
                string SongInfoSeparate;
                if (Global.SongMgrChorusSeparate == "1") { CrtchorusSeparate = "&"; } else { CrtchorusSeparate = "+"; }
                if (Global.SongMgrSongInfoSeparate == "1") { SongInfoSeparate = "_"; } else { SongInfoSeparate = "-"; }
                string SongTrackStr = CommonFunc.GetSongTrackStr(SongTrack - 1, 1, "null");

                if (SongSingerType == 3)
                {
                    SongSingerStr = Regex.Replace(SongSinger, "[&+]", CrtchorusSeparate, RegexOptions.IgnoreCase);
                }

                if (Global.SongMgrSongAddMode == "3")
                {
                    SongPath = Path.GetDirectoryName(SongSrcPath) + @"\";
                    SongFileName = Path.GetFileName(SongSrcPath);
                }
                else
                {
                    bool UseMultiSongPath = false;
                    string MultiSongPath = "";
                    if (Global.SongMaintenanceEnableMultiSongPath == "True" & SongPath.ContainsAny(Global.SongMaintenanceMultiSongPathList.ToArray()))
                    {
                        foreach (string str in Global.SongMaintenanceMultiSongPathList)
                        {
                            if (SongPath.Contains(str))
                            {
                                MultiSongPath = str;
                                UseMultiSongPath = true;
                                break;
                            }
                        }
                    }

                    switch (Global.SongMgrFolderStructure)
                    {
                        case "1":
                            if (Global.SongMgrChorusMerge == "True" & SongSingerType == 3)
                            {
                                if (UseMultiSongPath)
                                {
                                    SongPath = MultiSongPath + SongLang + @"\" + SingerTypeStr + @"\";
                                }
                                else
                                {
                                    SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\";
                                }
                            }
                            else
                            {
                                if (UseMultiSongPath)
                                {
                                    SongPath = MultiSongPath + SongLang + @"\" + SingerTypeStr + @"\" + SongSingerStr + @"\";
                                }
                                else
                                {
                                    SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\" + SongSingerStr + @"\";
                                }
                            }
                            break;
                        case "2":
                            if (UseMultiSongPath)
                            {
                                SongPath = MultiSongPath + SongLang + @"\" + SingerTypeStr + @"\";
                            }
                            else
                            {
                                SongPath = Global.SongMgrDestFolder + @"\" + SongLang + @"\" + SingerTypeStr + @"\";
                            }
                            break;
                    }

                    switch (Global.SongMgrFileStructure)
                    {
                        case "1":
                            if (SongSongType == "")
                            {
                                SongFileName = SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                        case "2":
                            if (SongSongType == "")
                            {
                                SongFileName = SongSongName + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongSongName + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                        case "3":
                            if (SongSongType == "")
                            {
                                SongFileName = SongId + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            else
                            {
                                SongFileName = SongId + SongInfoSeparate + SongSingerStr + SongInfoSeparate + SongSongName + SongInfoSeparate + SongSongType + SongInfoSeparate + SongTrackStr + SongExtension;
                            }
                            break;
                    }
                }


                if (SongSinger.Length > 60)
                {
                    Global.TotalList[2]++;
                    Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "歌手名稱超過60字元: " + SongSrcPath;
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                }
                else if (SongSongName.Length > 80)
                {
                    Global.TotalList[2]++;
                    Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "歌曲名稱超過80字元: " + SongSrcPath;
                    Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                }
                else
                {
                    bool FileIOError = false;
                    if (Global.SongMgrSongAddMode != "3")
                    {
                        string SongDestPath = Path.Combine(SongPath, SongFileName);

                        if (!Directory.Exists(SongPath))
                        {
                            Directory.CreateDirectory(SongPath);
                        }

                        try
                        {
                            switch (Global.SongMgrSongAddMode)
                            {
                                case "1":
                                    if (File.Exists(SongDestPath))
                                    {
                                        FileAttributes attributes = File.GetAttributes(SongDestPath);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(SongDestPath, attributes);
                                        }

                                        if (Global.SongMgrBackupRemoveSong == "True")
                                        {
                                            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                                            if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                            File.Move(SongDestPath, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                        }
                                        else
                                        {
                                            File.Delete(SongDestPath);
                                        }
                                    }
                                    File.Move(SongSrcPath, SongDestPath);
                                    break;
                                case "2":
                                    if (File.Exists(SongDestPath))
                                    {
                                        FileAttributes attributes = File.GetAttributes(SongDestPath);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(SongDestPath, attributes);
                                        }

                                        if (Global.SongMgrBackupRemoveSong == "True")
                                        {
                                            if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                                            if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                            File.Move(SongDestPath, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                        }
                                    }
                                    File.Copy(SongSrcPath, SongDestPath, true);
                                    break;
                            }
                        }
                        catch
                        {
                            FileIOError = true;
                            lock (LockThis) { Global.TotalList[2]++; }
                            Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "檔案處理發生錯誤: " + SongDestPath + " (唯讀或使用中)";
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                        }
                    }

                    if (!FileIOError)
                    {
                        string SongAddValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState;
                        Global.SongAddValueList.Add(SongAddValue);
                        lock (LockThis) { Global.TotalList[3]++; }
                    }
                }
            }
        }
























    }
}
