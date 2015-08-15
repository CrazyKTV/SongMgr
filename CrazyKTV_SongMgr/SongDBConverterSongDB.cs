using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrazyKTV_SongMgr
{
    class SongDBConverterSongDB
    {
        private static object LockThis = new object();

        public static void CreateSongDataTable(int SongSrcDBType, string SongSrcDBFile, string SongDestDBFile)
        {
            string SongQuerySqlStr = "";
            switch (SongSrcDBType)
            {
                case 1:
                    SongQuerySqlStr = "select Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_PlayState from ktv_Song";
                    Global.SongSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");
                    break;
                case 2:
                    SongQuerySqlStr = "select Song_ID, Song_Type, Song_Singer, Song_SingerList, Song_Title, Song_Channel, Song_Volume, Song_Count, Song_CreateDate, Song_FileName, Song_Path from Tbl_Song";
                    Global.SongSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "tmwcmgumbonqd");

                    SongQuerySqlStr = "select Singer_ID, Singer_Sex, Singer_Name from Tbl_Singer";
                    Global.SingerSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "tmwcmgumbonqd");
                    break;
            }

            string SongAllSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_AllSinger";
            Global.AllSingerDT = CommonFunc.GetOleDbDataTable(SongDestDBFile, SongAllSingerQuerySqlStr, "");

            Global.PhoneticsWordList = new List<string>();
            Global.PhoneticsSpellList = new List<string>();
            Global.PhoneticsStrokesList = new List<string>();
            Global.PhoneticsPenStyleList = new List<string>();

            string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
            Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(SongDestDBFile, SongPhoneticsQuerySqlStr, "");

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
            Global.PhoneticsWordList.Clear();
            Global.PhoneticsSpellList.Clear();
            Global.PhoneticsStrokesList.Clear();
            Global.PhoneticsPenStyleList.Clear();
            Global.SongSrcDT.Dispose();
            Global.AllSingerDT.Dispose();
            Global.PhoneticsDT.Dispose();
        }

        public static void StartConvFromOldDB(int i)
        {
            bool ConvStatus = true;
            string SongId = Global.SongSrcDT.Rows[i]["Song_Id"].ToString();
            string SongLang = Global.SongSrcDT.Rows[i]["Song_Lang"].ToString();
            string SongSongName = Global.SongSrcDT.Rows[i]["Song_SongName"].ToString();
            string SongSinger = Global.SongSrcDT.Rows[i]["Song_Singer"].ToString();
            string SongSongType = Global.SongSrcDT.Rows[i]["Song_SongType"].ToString();
            string SongFileName = Global.SongSrcDT.Rows[i]["Song_FileName"].ToString();
            string SongPath = Global.SongSrcDT.Rows[i]["Song_Path"].ToString();
            string file = Path.Combine(SongPath, SongFileName);

            List<string> list = new List<string>();
            list = new List<string>() { SongId, SongLang, SongSongName, SongSinger };

            foreach(string str in list)
            {
                if(str == "")
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】已忽略轉換此首歌曲,因為此首歌曲資料不全: " + SongId + "|" + SongLang + "|" + SongSongName + "|" + SongSinger;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    lock (LockThis) Global.TotalList[1]++;
                    ConvStatus = false;
                    break;
                }
            }

            if(ConvStatus == true)
            {
                string SongSingerType = "";
                if (!Global.SongSrcDT.Rows[i]["Song_SingerType"].Equals(DBNull.Value))
                {
                    SongSingerType = Global.SongSrcDT.Rows[i]["Song_SingerType"].ToString();
                }
                else
                {
                    SongSingerType = "10";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌手類別數值為空值,已自動將其數值改為10: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongTrack = "";
                if (!Global.SongSrcDT.Rows[i]["Song_Track"].Equals(DBNull.Value))
                {
                    SongTrack = Global.SongSrcDT.Rows[i]["Song_Track"].ToString();
                }
                else
                {
                    SongTrack = "1";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為空值,已自動將其數值改為1: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongVolume = "";
                if (!Global.SongSrcDT.Rows[i]["Song_Volume"].Equals(DBNull.Value))
                {
                    SongVolume = Global.SongSrcDT.Rows[i]["Song_Volume"].ToString();
                }
                else
                {
                    SongVolume = "100";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲音量數值為空值,已自動將其數值改為100: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongPlayCount = "";
                if (!Global.SongSrcDT.Rows[i]["Song_PlayCount"].Equals(DBNull.Value))
                {
                    SongPlayCount = Global.SongSrcDT.Rows[i]["Song_PlayCount"].ToString();
                }
                else
                {
                    SongPlayCount = "0";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲點播次數數值為空值,已自動將其數值改為0: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongCreatDate = "";
                if (!Global.SongSrcDT.Rows[i]["Song_CreatDate"].Equals(DBNull.Value))
                {
                    SongCreatDate = Global.SongSrcDT.Rows[i]["Song_CreatDate"].ToString();
                }
                else
                {
                    SongCreatDate = DateTime.Now.ToString();
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲加歌日期數值為空值,已自動將其數值改為現在日期: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongPlayState = "";
                if (!Global.SongSrcDT.Rows[i]["Song_PlayState"].Equals(DBNull.Value))
                {
                    SongPlayState = Global.SongSrcDT.Rows[i]["Song_PlayState"].ToString();
                }
                else
                {
                    SongPlayState = "0";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲播放狀態數值為空值,已自動將其數值改為0: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongWordCount = "";
                string SongSpell = "";
                string SongSpellNum = "";
                string SongSongStroke = "";
                string SongPenStyle = "";

                // 計算歌曲字數
                List<string> SongWordCountList = new List<string>();
                SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                SongWordCount = SongWordCountList[0];

                // 計算歌曲大小
                float SongMB = 0;
                if (Global.SongSrcDT.Rows[i]["Song_MB"].ToString() != "")
                {
                    SongMB = float.Parse(Global.SongSrcDT.Rows[i]["Song_MB"].ToString());
                }

                // 取得歌曲拼音
                List<string> SongSpellList = new List<string>();
                SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);

                SongSpell = SongSpellList[0];
                SongSpellNum = SongSpellList[1];
                if (SongSpellList[2] == "") SongSpellList[2] = "0";
                SongSongStroke = SongSpellList[2];
                SongPenStyle = SongSpellList[3];

                string SongDBConvValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState;
                Global.SongDBConvValueList.Add(SongDBConvValue);

                lock (LockThis)
                {
                    Global.TotalList[0]++;
                }
            }
        }

        public static void StartConvFromJetktvDB(int i)
        {
            bool ConvStatus = true;
            string SongId = Global.SongSrcDT.Rows[i]["Song_ID"].ToString();
            
            string SongLang = "";
            List<string> list = new List<string>();

            if (Global.SongSrcDT.Rows[i]["Song_Type"].ToString() != "")
            {
                if (Convert.ToInt32(Global.SongSrcDT.Rows[i]["Song_Type"]) > 9)
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲語系類別並未定義,已自動將其數值改為其它: " + SongId + "|" + SongLang;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    SongLang = "其它";
                }
                else
                {
                    SongLang = Global.SongDBConvJetktvLangList[Convert.ToInt32(Global.SongSrcDT.Rows[i]["Song_Type"])];
                }
            }
            
            string SongSongName = Global.SongSrcDT.Rows[i]["Song_Title"].ToString();
            string SongSinger = Global.SongSrcDT.Rows[i]["Song_SingerList"].ToString();
            
            string SongSingerType = "";
            if (SongSinger != "")
            {
                Regex r = new Regex("[&+、](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                if (r.IsMatch(SongSinger))
                {
                    SongSinger = Regex.Replace(SongSinger, "[&+、]", "&", RegexOptions.IgnoreCase);
                    SongSingerType = "3";
                }
                else
                {
                    var query = from row in Global.SingerSrcDT.AsEnumerable()
                                where row.Field<string>("Singer_Name").ToLower().Equals(SongSinger.ToLower())
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            list = new List<string>() { "1", "0", "2" };
                            if (row["Singer_Sex"].ToString() != "")
                            {
                                if (SongSingerType == "") SongSingerType = list[Convert.ToInt32(row["Singer_Sex"])];
                                break;
                            }
                        }
                    }
                }
            }

            string SongSongType = "";
            string SongFileName = Global.SongSrcDT.Rows[i]["Song_FileName"].ToString();
            string SongPath = Global.SongSrcDT.Rows[i]["Song_Path"].ToString();
            if (!Directory.Exists(SongPath)) SongPath = "";
            string file = Path.Combine(SongPath, SongFileName);

            list = new List<string>() { SongId, SongLang, SongSongName, SongSinger };

            foreach (string str in list)
            {
                if (str == "")
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】已忽略轉換此首歌曲,因為此首歌曲資料不全: " + SongId + "|" + SongLang + "|" + SongSongName + "|" + SongSinger;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    lock (LockThis) Global.TotalList[1]++;
                    ConvStatus = false;
                    break;
                }
            }

            if (ConvStatus == true)
            {
                if (SongSingerType == "")
                {
                    SongSingerType = "10";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌手類別數值為空值,已自動將其數值改為10: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongTrack = "";
                if (!Global.SongSrcDT.Rows[i]["Song_Channel"].Equals(DBNull.Value))
                {
                    SongTrack = Global.SongSrcDT.Rows[i]["Song_Channel"].ToString();
                    if (SongTrack == "3")
                    {
                        SongTrack = "1";
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為3立體聲,已自動將其數值改為1: " + SongId + "|" + SongSongName;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                }
                else
                {
                    SongTrack = "1";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為空值,已自動將其數值改為1: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongVolume = "";
                if (!Global.SongSrcDT.Rows[i]["Song_Volume"].Equals(DBNull.Value))
                {
                    SongVolume = Global.SongSrcDT.Rows[i]["Song_Volume"].ToString();
                }
                else
                {
                    SongVolume = "100";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲音量數值為空值,已自動將其數值改為100: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
                
                string SongPlayCount = "";
                if (!Global.SongSrcDT.Rows[i]["Song_Count"].Equals(DBNull.Value))
                {
                    SongPlayCount = Global.SongSrcDT.Rows[i]["Song_Count"].ToString();
                }
                else
                {
                    SongPlayCount = "0";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲點播次數數值為空值,已自動將其數值改為0: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongCreatDate = "";
                if (!Global.SongSrcDT.Rows[i]["Song_CreateDate"].Equals(DBNull.Value))
                {
                    SongCreatDate = Global.SongSrcDT.Rows[i]["Song_CreateDate"].ToString();
                }
                else
                {
                    SongCreatDate = DateTime.Now.ToString();
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲加歌日期數值為空值,已自動將其數值改為現在日期: " + SongId + "|" + SongSongName;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                string SongPlayState = "0";
                string SongWordCount = "";
                string SongSpell = "";
                string SongSpellNum = "";
                string SongSongStroke = "";
                string SongPenStyle = "";

                // 計算歌曲字數
                List<string> SongWordCountList = new List<string>();
                SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                SongWordCount = SongWordCountList[0];

                // 計算歌曲大小
                float SongMB = 0;
                if (File.Exists(file))
                {
                    FileInfo f = new FileInfo(file);
                    SongMB = float.Parse(((f.Length / 1024f) / 1024f).ToString("F2"));
                }

                // 取得歌曲拼音
                List<string> SongSpellList = new List<string>();
                SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);

                SongSpell = SongSpellList[0];
                SongSpellNum = SongSpellList[1];
                if (SongSpellList[2] == "") SongSpellList[2] = "0";
                SongSongStroke = SongSpellList[2];
                SongPenStyle = SongSpellList[3];

                string SongDBConvValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState;
                Global.SongDBConvValueList.Add(SongDBConvValue);

                lock (LockThis)
                {
                    Global.TotalList[0]++;
                }
            }
        }






    }
}
