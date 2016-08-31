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

        #region --- SongAddSong 建立資料表 ---

        public static DataTable SongAddDT;
        public static List<string> SongDataIdList;
        public static List<string> SongDataList;
        public static List<string> SongDataLowCaseList;
        public static List<string> SongDataFilePathList;
        public static List<string> SingerDataList;
        public static List<string> SingerDataLowCaseList;
        public static List<string> SingerDataTypeList;
        public static List<string> AllSingerDataList;
        public static List<string> AllSingerDataLowCaseList;
        public static List<string> AllSingerDataTypeList;

        public static DataTable DupSongAddDT;
        public static List<string> SongAddValueList;
        public static List<string> ChorusSingerList;

        public static void CreateSongDataTable()
        {
            SongDataIdList = new List<string>();
            SongDataList = new List<string>();
            SongDataLowCaseList = new List<string>();
            SongDataFilePathList = new List<string>();

            string SongDataSqlStr = "select Song_Id, Song_Lang, Song_Singer, Song_SongName, Song_SongType, Song_FileName, Song_Path from ktv_Song";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongDataSqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SongDataIdList.Add(row["Song_Id"].ToString());
                    SongDataList.Add(row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString() + "|" + row["Song_SongName"].ToString() + "|" + row["Song_SongType"].ToString());
                    SongDataLowCaseList.Add(row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString().ToLower() + "|" + row["Song_SongName"].ToString().ToLower() + "|" + row["Song_SongType"].ToString().ToLower());
                    SongDataFilePathList.Add(Path.Combine(row["Song_Path"].ToString(), row["Song_FileName"].ToString()));
                }
            }

            SingerDataList = new List<string>();
            SingerDataLowCaseList = new List<string>();
            SingerDataTypeList = new List<string>();

            string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SingerDataList.Add(row["Singer_Name"].ToString());
                    SingerDataTypeList.Add(row["Singer_Type"].ToString());
                }
            }
            SingerDataLowCaseList = SingerDataList.ConvertAll(str => str.ToLower());

            AllSingerDataList = new List<string>();
            AllSingerDataLowCaseList = new List<string>();
            AllSingerDataTypeList = new List<string>();

            string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongAllSingerQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    AllSingerDataList.Add(row["Singer_Name"].ToString());
                    AllSingerDataTypeList.Add(row["Singer_Type"].ToString());
                }
            }
            AllSingerDataLowCaseList = AllSingerDataList.ConvertAll(str => str.ToLower());

            Global.DuplicateSongDT = new DataTable();
            Global.DuplicateSongDT.Columns.Add(new DataColumn("Display", typeof(string)));
            Global.DuplicateSongDT.Columns.Add(new DataColumn("Value", typeof(int)));

            Global.FailureSongDT = new DataTable();
            Global.FailureSongDT.Columns.Add(new DataColumn("Display", typeof(string)));
            Global.FailureSongDT.Columns.Add(new DataColumn("Value", typeof(int)));

            DupSongAddDT = new DataTable();
            DupSongAddDT.Columns.Add("Song_AddStatus", typeof(string));
            DupSongAddDT.Columns.Add("Song_Id", typeof(string));
            DupSongAddDT.Columns.Add("Song_Lang", typeof(string));
            DupSongAddDT.Columns.Add("Song_SingerType", typeof(int));
            DupSongAddDT.Columns.Add("Song_Singer", typeof(string));
            DupSongAddDT.Columns.Add("Song_SongName", typeof(string));
            DupSongAddDT.Columns.Add("Song_Track", typeof(int));
            DupSongAddDT.Columns.Add("Song_SongType", typeof(string));
            DupSongAddDT.Columns.Add("Song_Volume", typeof(int));
            DupSongAddDT.Columns.Add("Song_WordCount", typeof(int));
            DupSongAddDT.Columns.Add("Song_PlayCount", typeof(int));
            DupSongAddDT.Columns.Add("Song_MB", typeof(float));
            DupSongAddDT.Columns.Add("Song_CreatDate", typeof(DateTime));
            DupSongAddDT.Columns.Add("Song_FileName", typeof(string));
            DupSongAddDT.Columns.Add("Song_Path", typeof(string));
            DupSongAddDT.Columns.Add("Song_Spell", typeof(string));
            DupSongAddDT.Columns.Add("Song_SpellNum", typeof(string));
            DupSongAddDT.Columns.Add("Song_SongStroke", typeof(int));
            DupSongAddDT.Columns.Add("Song_PenStyle", typeof(string));
            DupSongAddDT.Columns.Add("Song_PlayState", typeof(int));
            DupSongAddDT.Columns.Add("Song_SrcPath", typeof(string));
            DupSongAddDT.Columns.Add("Song_SortIndex", typeof(string));
        }

        public static void DisposeSongDataTable()
        {
            SongDataIdList.Clear();
            SongDataList.Clear();
            SongDataLowCaseList.Clear();
            SongDataFilePathList.Clear();
            SingerDataList.Clear();
            SingerDataLowCaseList.Clear();
            SingerDataTypeList.Clear();
            AllSingerDataList.Clear();
            AllSingerDataLowCaseList.Clear();
            AllSingerDataTypeList.Clear();

            SongAddDT.Dispose();
            SongAddDT = null;
            GC.Collect();
        }

        #endregion

        #region --- SongAddSong 加入歌曲 ---

        public static void StartAddSong(int i)
        {
            // 判斷是否為重複歌曲
            bool DuplicateSongStatus = false;
            int DuplicateSongInfoIndex = -1;
            string DuplicateSongId = "";
            float DuplicateSongMB = 0;

            if (SongDataLowCaseList.Count > 0)
            {
                string SongData = SongAddDT.Rows[i].Field<string>("Song_Lang") + "|" + SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower() + "|" + SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower() + "|" + SongAddDT.Rows[i].Field<string>("Song_SongType").ToLower();

                if (SongDataLowCaseList.IndexOf(SongData) >= 0)
                {
                    DuplicateSongInfoIndex = SongDataLowCaseList.IndexOf(SongData);
                    DuplicateSongStatus = true;
                }
                else
                {
                    if (Global.GroupSingerLowCaseList.IndexOf(SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower()) >= 0)
                    {
                        int SingerGroupId = Global.GroupSingerIdList[Global.GroupSingerLowCaseList.IndexOf(SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower())];
                        List<string> GroupSingerList = new List<string>(Global.SingerGroupList[SingerGroupId].Split(','));
                        if (GroupSingerList.Count > 0)
                        {
                            foreach (string GroupSingerName in GroupSingerList)
                            {
                                if (GroupSingerName.ToLower() != SongAddDT.Rows[i].Field<string>("Song_Singer").ToLower())
                                {
                                    SongData = SongAddDT.Rows[i].Field<string>("Song_Lang") + "|" + GroupSingerName.ToLower() + "|" + SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower() + "|" + SongAddDT.Rows[i].Field<string>("Song_SongType").ToLower();
                                }

                                if (SongDataLowCaseList.IndexOf(SongData) >= 0)
                                {
                                    DuplicateSongInfoIndex = SongDataLowCaseList.IndexOf(SongData);
                                    DuplicateSongStatus = true;
                                    break;
                                }
                            }
                            GroupSingerList.Clear();
                        }
                    }
                }

                if (!DuplicateSongStatus && SongAddDT.Rows[i].Field<int>("Song_SingerType") == 3)
                {
                    List<string> ChorusSongDatalist = new List<string>();
                    ChorusSongDatalist.Add(SongAddDT.Rows[i].Field<string>("Song_Lang"));
                    ChorusSongDatalist.Add(SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower());
                    if (SongAddDT.Rows[i].Field<string>("Song_SongType") != "") ChorusSongDatalist.Add(SongAddDT.Rows[i].Field<string>("Song_SongType").ToLower());

                    List<string> ChorusGroupSongDatalist = new List<string>();
                    ChorusGroupSongDatalist.Add(SongAddDT.Rows[i].Field<string>("Song_Lang"));
                    ChorusGroupSongDatalist.Add(SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower());
                    if (SongAddDT.Rows[i].Field<string>("Song_SongType") != "") ChorusGroupSongDatalist.Add(SongAddDT.Rows[i].Field<string>("Song_SongType").ToLower());
                    
                    // 處理合唱歌曲中的特殊歌手名稱
                    string ChorusSongSingerName = SongAddDT.Rows[i].Field<string>("Song_Singer");
                    int ChorusGroupSongSingerCount = 0;
                    bool MatchChorusGroupSongSinger = false;
                    List<string> ChorusSingerList = new List<string>();
                    List<string> ChorusGroupSingerList = new List<string>();
                    List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));

                    foreach (string SpecialSingerName in SpecialStrlist)
                    {
                        Regex SpecialStrRegex = new Regex("^" + SpecialSingerName + "&|&" + SpecialSingerName + "&|&" + SpecialSingerName + "$", RegexOptions.IgnoreCase);
                        if (SpecialStrRegex.IsMatch(ChorusSongSingerName))
                        {
                            if (ChorusSongDatalist.IndexOf(SpecialSingerName.ToLower()) < 0) ChorusSongDatalist.Add(SpecialSingerName.ToLower());
                            if (ChorusSingerList.IndexOf(SpecialSingerName.ToLower()) < 0) ChorusSingerList.Add(SpecialSingerName.ToLower());
                            if (ChorusGroupSingerList.IndexOf(SpecialSingerName.ToLower()) < 0) ChorusGroupSingerList.Add(SpecialSingerName.ToLower());
                            ChorusGroupSongSingerCount++;

                            ChorusSongSingerName = (ChorusSongSingerName != SpecialSingerName.ToLower()) ? Regex.Replace(ChorusSongSingerName, SpecialSingerName + "&|&" + SpecialSingerName + "$", "", RegexOptions.IgnoreCase) : "";
                        }
                    }
                    SpecialStrlist.Clear();
                    
                    if (ChorusSongSingerName != "")
                    {
                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                        if (r.IsMatch(ChorusSongSingerName))
                        {
                            string[] singers = Regex.Split(ChorusSongSingerName, "&", RegexOptions.None);
                            foreach (string str in singers)
                            {
                                string SingerStr = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                if (ChorusSongDatalist.IndexOf(SingerStr.ToLower()) < 0) ChorusSongDatalist.Add(SingerStr.ToLower());
                                if (ChorusSingerList.IndexOf(SingerStr.ToLower()) < 0) ChorusSingerList.Add(SingerStr.ToLower());

                                if (Global.GroupSingerLowCaseList.IndexOf(SingerStr.ToLower()) >= 0)
                                {
                                    int SingerGroupId = Global.GroupSingerIdList[Global.GroupSingerLowCaseList.IndexOf(SingerStr.ToLower())];
                                    List<string> GroupSingerList = new List<string>(Global.SingerGroupList[SingerGroupId].Split(','));
                                    if (GroupSingerList.Count > 0)
                                    {
                                        foreach (string GroupSingerName in GroupSingerList)
                                        {
                                            if (ChorusGroupSingerList.IndexOf(GroupSingerName.ToLower()) < 0)
                                            {
                                                ChorusGroupSingerList.Add(GroupSingerName.ToLower());
                                            }
                                        }
                                        ChorusGroupSongSingerCount++;
                                        MatchChorusGroupSongSinger = true;
                                        GroupSingerList.Clear();
                                    }
                                }
                                else
                                {
                                    if (ChorusGroupSingerList.IndexOf(SingerStr.ToLower()) < 0) ChorusGroupSingerList.Add(SingerStr.ToLower());
                                    ChorusGroupSongSingerCount++;
                                }
                            }
                        }
                        else
                        {
                            if (ChorusSongDatalist.IndexOf(ChorusSongSingerName.ToLower()) < 0) ChorusSongDatalist.Add(ChorusSongSingerName.ToLower());
                            if (ChorusSingerList.IndexOf(ChorusSongSingerName.ToLower()) < 0) ChorusSingerList.Add(ChorusSongSingerName.ToLower());
                            if (ChorusGroupSingerList.IndexOf(ChorusSongSingerName.ToLower()) < 0) ChorusGroupSingerList.Add(ChorusSongSingerName.ToLower());
                            ChorusGroupSongSingerCount++;
                        }
                    }

                    List<string> FindResultList = new List<string>();
                    if (!DuplicateSongStatus && ChorusSingerList.Count > 0)
                    {
                        FindResultList = SongDataLowCaseList.FindAll(SongInfo => SongInfo.ContainsAll(ChorusSongDatalist.ToArray()));
                        if (FindResultList.Count > 0)
                        {
                            foreach (string FindResult in FindResultList)
                            {
                                List<string> list = new List<string>(FindResult.Split('|'));

                                if (list[1].ContainsAll(ChorusSingerList.ToArray()) && list[2] == SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower())
                                {
                                    DuplicateSongInfoIndex = SongDataLowCaseList.IndexOf(FindResult);
                                    DuplicateSongStatus = true;
                                    break;
                                }
                                list.Clear();
                            }
                        }
                        FindResultList.Clear();
                    }

                    if (!DuplicateSongStatus && MatchChorusGroupSongSinger)
                    {
                        FindResultList = SongDataLowCaseList.FindAll(SongInfo => SongInfo.ContainsAll(ChorusGroupSongDatalist.ToArray()));
                        if (FindResultList.Count > 0)
                        {
                            foreach (string FindResult in FindResultList)
                            {
                                List<string> list = new List<string>(FindResult.Split('|'));

                                if (list[1].ContainsCount(ChorusGroupSongSingerCount, ChorusGroupSingerList.ToArray()) && list[2] == SongAddDT.Rows[i].Field<string>("Song_SongName").ToLower())
                                {
                                    DuplicateSongInfoIndex = SongDataLowCaseList.IndexOf(FindResult);
                                    DuplicateSongStatus = true;
                                    break;
                                }
                                list.Clear();
                            }
                        }
                        FindResultList.Clear();
                    }
                    ChorusGroupSingerList.Clear();
                    ChorusGroupSongDatalist.Clear();
                    ChorusSingerList.Clear();
                    ChorusSongDatalist.Clear();
                }
            }

            if (DuplicateSongStatus)
            {
                DuplicateSongId = SongDataIdList[DuplicateSongInfoIndex];
                string file = SongDataFilePathList[DuplicateSongInfoIndex];
                if (File.Exists(file))
                {
                    FileInfo f = new FileInfo(file);
                    DuplicateSongMB = float.Parse(((f.Length / 1024f) / 1024f).ToString("F2"));
                }

                switch (Global.SongAddDupSongMode)
                {
                    case "1":
                        lock (LockThis)
                        {
                            Global.TotalList[1]++;
                            Global.DuplicateSongDT.Rows.Add(Global.DuplicateSongDT.NewRow());
                            Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][0] = SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                            Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][1] = Global.DuplicateSongDT.Rows.Count;
                        }
                        break;
                    case "2":
                        DataRow row2 = DupSongAddDT.NewRow();
                        row2["Song_AddStatus"] = "重複歌曲ID: " + DuplicateSongId;
                        row2["Song_Id"] = DuplicateSongId;
                        row2["Song_Lang"] = SongAddDT.Rows[i].Field<string>("Song_Lang");
                        row2["Song_SingerType"] = SongAddDT.Rows[i].Field<int>("Song_SingerType");
                        row2["Song_Singer"] = SongAddDT.Rows[i].Field<string>("Song_Singer");
                        row2["Song_SongName"] = SongAddDT.Rows[i].Field<string>("Song_SongName");
                        row2["Song_Track"] = SongAddDT.Rows[i].Field<int>("Song_Track");
                        row2["Song_SongType"] = SongAddDT.Rows[i].Field<string>("Song_SongType");
                        row2["Song_Volume"] = SongAddDT.Rows[i].Field<int>("Song_Volume");
                        row2["Song_WordCount"] = SongAddDT.Rows[i].Field<int>("Song_WordCount");
                        row2["Song_PlayCount"] = SongAddDT.Rows[i].Field<int>("Song_PlayCount");
                        row2["Song_MB"] = SongAddDT.Rows[i].Field<float>("Song_MB");
                        row2["Song_CreatDate"] = SongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
                        row2["Song_FileName"] = Path.GetFileName(SongDataFilePathList[DuplicateSongInfoIndex]);
                        row2["Song_Path"] = Path.GetDirectoryName(SongDataFilePathList[DuplicateSongInfoIndex]) + @"\"; 
                        row2["Song_Spell"] = SongAddDT.Rows[i].Field<string>("Song_Spell");
                        row2["Song_SpellNum"] = SongAddDT.Rows[i].Field<string>("Song_SpellNum");
                        row2["Song_SongStroke"] = SongAddDT.Rows[i].Field<int>("Song_SongStroke");
                        row2["Song_PenStyle"] = SongAddDT.Rows[i].Field<string>("Song_PenStyle");
                        row2["Song_PlayState"] = SongAddDT.Rows[i].Field<int>("Song_PlayState");
                        row2["Song_SrcPath"] = SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                        row2["Song_SortIndex"] = "1";
                        DupSongAddDT.Rows.Add(row2);
                        break;
                    case "3":
                        if (SongAddDT.Rows[i].Field<float>("Song_MB") > DuplicateSongMB)
                        {
                            DataRow row3 = DupSongAddDT.NewRow();
                            row3["Song_AddStatus"] = "重複歌曲";
                            row3["Song_Id"] = DuplicateSongId;
                            row3["Song_Lang"] = SongAddDT.Rows[i].Field<string>("Song_Lang");
                            row3["Song_SingerType"] = SongAddDT.Rows[i].Field<int>("Song_SingerType");
                            row3["Song_Singer"] = SongAddDT.Rows[i].Field<string>("Song_Singer");
                            row3["Song_SongName"] = SongAddDT.Rows[i].Field<string>("Song_SongName");
                            row3["Song_Track"] = SongAddDT.Rows[i].Field<int>("Song_Track");
                            row3["Song_SongType"] = SongAddDT.Rows[i].Field<string>("Song_SongType");
                            row3["Song_Volume"] = SongAddDT.Rows[i].Field<int>("Song_Volume");
                            row3["Song_WordCount"] = SongAddDT.Rows[i].Field<int>("Song_WordCount");
                            row3["Song_PlayCount"] = SongAddDT.Rows[i].Field<int>("Song_PlayCount");
                            row3["Song_MB"] = SongAddDT.Rows[i].Field<float>("Song_MB");
                            row3["Song_CreatDate"] = SongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
                            row3["Song_FileName"] = Path.GetFileName(SongDataFilePathList[DuplicateSongInfoIndex]);
                            row3["Song_Path"] = Path.GetDirectoryName(SongDataFilePathList[DuplicateSongInfoIndex]) + @"\";
                            row3["Song_Spell"] = SongAddDT.Rows[i].Field<string>("Song_Spell");
                            row3["Song_SpellNum"] = SongAddDT.Rows[i].Field<string>("Song_SpellNum");
                            row3["Song_SongStroke"] = SongAddDT.Rows[i].Field<int>("Song_SongStroke");
                            row3["Song_PenStyle"] = SongAddDT.Rows[i].Field<string>("Song_PenStyle");
                            row3["Song_PlayState"] = SongAddDT.Rows[i].Field<int>("Song_PlayState");
                            row3["Song_SrcPath"] = SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                            row3["Song_SortIndex"] = "1";
                            DupSongAddDT.Rows.Add(row3);
                        }
                        else
                        {
                            lock (LockThis)
                            {
                                Global.TotalList[1]++;
                                Global.DuplicateSongDT.Rows.Add(Global.DuplicateSongDT.NewRow());
                                Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][0] = SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                                Global.DuplicateSongDT.Rows[Global.DuplicateSongDT.Rows.Count - 1][1] = Global.DuplicateSongDT.Rows.Count;
                            }
                        }
                        break;
                }
            }
            else
            {
                string SongId = SongAddDT.Rows[i].Field<string>("Song_Id"); ;
                string SongLang = SongAddDT.Rows[i].Field<string>("Song_Lang");
                int SongSingerType = SongAddDT.Rows[i].Field<int>("Song_SingerType");
                string SongSinger = SongAddDT.Rows[i].Field<string>("Song_Singer");
                string SongSongName = SongAddDT.Rows[i].Field<string>("Song_SongName");
                int SongTrack = SongAddDT.Rows[i].Field<int>("Song_Track");
                string SongSongType = SongAddDT.Rows[i].Field<string>("Song_SongType");
                int SongVolume = SongAddDT.Rows[i].Field<int>("Song_Volume");
                int SongWordCount = SongAddDT.Rows[i].Field<int>("Song_WordCount");
                int SongPlayCount = SongAddDT.Rows[i].Field<int>("Song_PlayCount");
                float SongMB = SongAddDT.Rows[i].Field<float>("Song_MB");
                DateTime SongCreatDate = SongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
                string SongFileName = "";
                string SongPath = "";
                string SongSpell = SongAddDT.Rows[i].Field<string>("Song_Spell");
                string SongSpellNum = SongAddDT.Rows[i].Field<string>("Song_SpellNum");
                int SongSongStroke = SongAddDT.Rows[i].Field<int>("Song_SongStroke");
                string SongPenStyle = SongAddDT.Rows[i].Field<string>("Song_PenStyle");
                int SongPlayState = SongAddDT.Rows[i].Field<int>("Song_PlayState");

                string SongAddSinger = "0";
                string SongAddAllSinger = "0";
                
                string SongSrcPath = SongAddDT.Rows[i].Field<string>("Song_SrcPath");
                string SongExtension = Path.GetExtension(SongSrcPath);

                string SingerName = SongSinger;
                // 判斷是否要加入歌手資料至歌手資料庫
                if (SongSingerType != 3)
                {
                    // 查找資料庫歌手表
                    if (SingerDataList.Count > 0)
                    {
                        if (SingerDataLowCaseList.IndexOf(SingerName.ToLower()) < 0)
                        {
                            SongAddSinger = "1";
                        }
                    }
                    else
                    {
                        SongAddSinger = "1";
                    }
                }
                else
                {
                    // 處理合唱歌曲中的特殊歌手名稱
                    List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));
                    foreach (string SpecialSingerName in SpecialStrlist)
                    {
                        Regex SpecialStrRegex = new Regex("^" + SpecialSingerName + "&|&" + SpecialSingerName + "&|&" + SpecialSingerName + "$", RegexOptions.IgnoreCase);
                        if (SpecialStrRegex.IsMatch(SingerName))
                        {
                            if (ChorusSingerList.IndexOf(SpecialSingerName) < 0)
                            {
                                ChorusSingerList.Add(SpecialSingerName);
                            }
                            SingerName = Regex.Replace(SingerName, SpecialSingerName + "&|&" + SpecialSingerName + "$", "", RegexOptions.IgnoreCase);
                        }
                    }

                    if (SingerName != "")
                    {
                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                        if (r.IsMatch(SingerName))
                        {
                            string[] singers = Regex.Split(SingerName, "&", RegexOptions.None);
                            foreach (string str in singers)
                            {
                                string SingerStr = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                if (ChorusSingerList.IndexOf(SingerStr) < 0)
                                {
                                    ChorusSingerList.Add(SingerStr);
                                }
                            }
                        }
                        else
                        {
                            if (ChorusSingerList.IndexOf(SingerName) < 0)
                            {
                                ChorusSingerList.Add(SingerName);
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
                    if (SongId != "")
                    {
                        List<string> StartIdlist = new List<string>();
                        StartIdlist = new List<string>(Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));

                        switch (Global.SongMgrMaxDigitCode)
                        {
                            case "1":
                                if (SongId.Length == 5 && SongDataIdList.IndexOf(SongId) < 0)
                                {
                                    if (Global.CrazyktvSongLangList.IndexOf(SongLang) < 9)
                                    {
                                        if (Convert.ToInt32(SongId) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongId) < Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang) + 1]))
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(SongId) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongId) < 100000)
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                }
                                break;
                            case "2":
                                if (SongId.Length == 6 && SongDataIdList.IndexOf(SongId) < 0)
                                {
                                    if (Global.CrazyktvSongLangList.IndexOf(SongLang) < 9)
                                    {
                                        if (Convert.ToInt32(SongId) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongId) < Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang) + 1]))
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(SongId) >= Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]) &&
                                        Convert.ToInt32(SongId) < 1000000)
                                        {
                                            UseCustomSongID = true;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                // 自訂編號
                if (UseCustomSongID)
                {
                    if (Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Count > 0)
                    {
                        if (Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].IndexOf(SongId) >= 0)
                        {
                            lock (LockThis)
                            {
                                Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Remove(SongId);
                            }
                        }
                    }

                    if (Convert.ToInt32(SongId) > Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)])
                    {
                        lock (LockThis)
                        {
                            Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)] = Convert.ToInt32(SongId);
                            GetUnUsedSongId(SongId, SongLang);
                        }
                    }
                }
                else //自動分配編號
                {
                    SongId = "";

                    // 查詢歌曲編號有無斷號
                    if (Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Count > 0)
                    {
                        SongId = Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)][0];
                        Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Remove(SongId);
                    }
                    
                    // 若無斷號查詢各語系下個歌曲編號
                    if (SongId == "")
                    {
                        string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";
                        Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]++;
                        SongId = Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)].ToString(MaxDigitCode);
                    }
                }

                SongPath = Path.GetDirectoryName(SongSrcPath) + @"\";
                SongFileName = Path.GetFileName(SongSrcPath);

                if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
                {
                    string SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongFileName, SongPath, false, "", false);
                    SongPath = Path.GetDirectoryName(SongDestPath) + @"\";
                    SongFileName = Path.GetFileName(SongDestPath);
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
                    if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
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
                                            CommonFunc.SetFileTime(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName, DateTime.Now);
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
                                            CommonFunc.SetFileTime(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName, DateTime.Now);
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
                        string SongAddValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongAddSinger + "|" + SongAddAllSinger;
                        SongAddSong.SongAddValueList.Add(SongAddValue);

                        lock (LockThis)
                        {
                            SongDataIdList.Add(SongId);
                            Global.TotalList[0]++;
                        }
                    }
                }
            }
        }

        #endregion

        #region --- SongAddSong 更新歌曲 ---

        public static void StartUpdateSong(int i)
        {
            string SongId = DupSongAddDT.Rows[i].Field<string>("Song_Id");
            string SongLang = DupSongAddDT.Rows[i].Field<string>("Song_Lang");
            int SongSingerType = DupSongAddDT.Rows[i].Field<int>("Song_SingerType");
            string SongSinger = DupSongAddDT.Rows[i].Field<string>("Song_Singer");
            string SongSongName = DupSongAddDT.Rows[i].Field<string>("Song_SongName");
            int SongTrack = DupSongAddDT.Rows[i].Field<int>("Song_Track");
            string SongSongType = DupSongAddDT.Rows[i].Field<string>("Song_SongType");
            int SongVolume = DupSongAddDT.Rows[i].Field<int>("Song_Volume");
            int SongWordCount = DupSongAddDT.Rows[i].Field<int>("Song_WordCount");
            int SongPlayCount = DupSongAddDT.Rows[i].Field<int>("Song_PlayCount");
            float SongMB = DupSongAddDT.Rows[i].Field<float>("Song_MB");
            DateTime SongCreatDate = DupSongAddDT.Rows[i].Field<DateTime>("Song_CreatDate");
            string SongFileName = DupSongAddDT.Rows[i].Field<string>("Song_FileName");
            string SongPath = DupSongAddDT.Rows[i].Field<string>("Song_Path");
            string SongSpell = DupSongAddDT.Rows[i].Field<string>("Song_Spell");
            string SongSpellNum = DupSongAddDT.Rows[i].Field<string>("Song_SpellNum");
            int SongSongStroke = DupSongAddDT.Rows[i].Field<int>("Song_SongStroke");
            string SongPenStyle = DupSongAddDT.Rows[i].Field<string>("Song_PenStyle");
            int SongPlayState = DupSongAddDT.Rows[i].Field<int>("Song_PlayState");
            string SongSrcPath = DupSongAddDT.Rows[i].Field<string>("Song_SrcPath");

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

            if (Global.SongMgrSongAddMode != "3")
            {
                string oldfile = Path.Combine(SongPath, SongFileName);

                if (File.Exists(oldfile) && oldfile != SongSrcPath)
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
                            CommonFunc.SetFileTime(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName, DateTime.Now);
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
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【加歌頁面】重複歌曲原有檔案不存在或為同檔案,已自動忽略移除原有檔案: " + oldfile;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }

            if (!DeleteError)
            {
                lock (LockThis) { Global.TotalList[4]++; }

                if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
                {
                    SongPath = Path.GetDirectoryName(SongSrcPath) + @"\";
                    SongFileName = Path.GetFileName(SongSrcPath);
                    string SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongFileName, SongPath, false, "", true);
                    SongPath = Path.GetDirectoryName(SongDestPath) + @"\";
                    SongFileName = Path.GetFileName(SongDestPath);
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
                    if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
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
                                            CommonFunc.SetFileTime(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName, DateTime.Now);
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
                                            CommonFunc.SetFileTime(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName, DateTime.Now);
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
                        SongAddSong.SongAddValueList.Add(SongAddValue);
                        lock (LockThis) { Global.TotalList[3]++; }
                    }
                }
            }
        }

        #endregion

        #region --- SongAddSong 取得未使用的編號 ---

        public static void GetUnUsedSongId(string MaxSongID, string SongLang)
        {
            string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";

            List<string> StartIdlist = new List<string>(Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));
            int iMin = Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(SongLang)]);
            int iMax = Convert.ToInt32(MaxSongID);

            Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Clear();

            for (int i = iMin; i < iMax; i++)
            {
                if (SongDataIdList.IndexOf(i.ToString(MaxDigitCode)) < 0)
                {
                    Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Add(i.ToString(MaxDigitCode));
                }
            }
        }

        #endregion

    }
}
