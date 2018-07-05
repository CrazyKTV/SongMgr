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
                case 3:
                    SongQuerySqlStr = "select Song_ID, Song_Title, Song_Singer, Song_Channel, Song_Type, Song_FileName, Song_Count, Song_Path, Song_CreateDate, Song_Chorus, Song_Volume from Tbl_Song";
                    Global.SongSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");

                    SongQuerySqlStr = "select SongPath_Path from Tbl_SongPath";
                    Global.SongPathSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");
                    Global.SongDBConvHK2SongPathList = new List<string>();
                    foreach (DataRow row in Global.SongPathSrcDT.Rows)
                    {
                        Global.SongDBConvHK2SongPathList.Add(row["SongPath_Path"].ToString());
                    }

                    SongQuerySqlStr = "select Extension_Name from Tbl_Extension";
                    Global.ExtensionSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");

                    SongQuerySqlStr = "select Singer_ID, Singer_Name, Singer_Sex from Tbl_Singer";
                    Global.SingerSrcDT = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");
                    break;
            }
        }

        public static void DisposeSongDataTable()
        {
            Global.SongSrcDT.Dispose();
            Global.SongSrcDT = null;
            Global.SingerSrcDT.Dispose();
            Global.SingerSrcDT = null;
            Global.SongPathSrcDT.Dispose();
            Global.SongPathSrcDT = null;
            Global.ExtensionSrcDT.Dispose();
            Global.ExtensionSrcDT = null;
            Global.SongDBConvHK2SongPathList.Clear();
        }

        #region --- SongDBConverterSongDB 轉換 JetKTV 資料庫 ---

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
                Regex r = new Regex(Global.RegexChorusSeparate);
                if (r.IsMatch(SongSinger))
                {
                    SongSinger = Regex.Replace(SongSinger, Global.RegexChorusSeparate, "&", RegexOptions.IgnoreCase);
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
                                if (SongSingerType == "")
                                {
                                    try
                                    {
                                        SongSingerType = list[Convert.ToInt32(row["Singer_Sex"])];
                                    }
                                    catch
                                    {
                                        SongSingerType = "10";
                                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌手類別數值錯誤,已自動將其數值改為10: " + SongId + "|" + SongSongName;
                                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            string SongSongType = "";
            string SongFileName = Global.SongSrcDT.Rows[i]["Song_FileName"].ToString();
            string SongPath = Global.SongSrcDT.Rows[i]["Song_Path"].ToString() + @"\";

            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));

            if (!File.Exists(Path.Combine(SongPath, SongFileName)) || SongPath == @"\" || Path.GetExtension(SongFileName) == "")
            {
                foreach (string JetktvPath in Global.SongDBConvJetktvPathList)
                {
                    if (Path.GetExtension(SongFileName) == "")
                    {
                        foreach (string format in SupportFormat)
                        {
                            if (File.Exists(Path.Combine(JetktvPath, SongFileName + format)))
                            {
                                SongFileName = SongFileName + format;
                                SongPath = JetktvPath + @"\";
                                break;
                            }
                        }

                        if (File.Exists(Path.Combine(SongPath, SongFileName)))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(JetktvPath, SongFileName)))
                        {
                            SongPath = JetktvPath + @"\";
                            break;
                        }
                    }
                }
            }

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
                        SongTrack = "0";
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為3立體聲,已自動將其數值改為0: " + SongId + "|" + SongSongName;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                }
                else
                {
                    SongTrack = "0";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為空值,已自動將其數值改為0: " + SongId + "|" + SongSongName;
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

        #endregion

        #region --- SongDBConverterSongDB 轉換 HomeKara2 資料庫 ---

        public static void StartConvFromHomeKara2DB(int i)
        {
            bool ConvStatus = true;
            string SongId = Global.SongSrcDT.Rows[i]["Song_ID"].ToString();

            string SongLang = "";
            List<string> list = new List<string>();

            if (Global.SongSrcDT.Rows[i]["Song_Type"].ToString() != "")
            {
                SongLang = GetSongInfo("SongLang", Global.SongSrcDT.Rows[i]["Song_Type"].ToString());
                if (SongLang == "")
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲語系類別並未定義,已自動將其數值改為其它: " + SongId + "|" + SongLang;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    SongLang = "其它";
                }
            }
            
            string SongSongName = Global.SongSrcDT.Rows[i]["Song_Title"].ToString();
            string SongSinger = Regex.Replace(Global.SongSrcDT.Rows[i]["Song_Singer"].ToString(), @"^\+|\+$", "", RegexOptions.IgnoreCase);
            string SongSingerType = (Global.SongSrcDT.Rows[i]["Song_Chorus"].ToString() == "True") ? "3" : "";

            if (SongSinger != "")
            {
                Regex r = new Regex(Global.RegexChorusSeparate);
                if (r.IsMatch(SongSinger))
                {
                    SongSinger = Regex.Replace(SongSinger, Global.RegexChorusSeparate, "&", RegexOptions.IgnoreCase);
                }
            }

            if (SongSinger !="" && SongSingerType == "")
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
                            if (SongSingerType == "")
                            {
                                try
                                {
                                    SongSingerType = list[Convert.ToInt32(row["Singer_Sex"])];
                                }
                                catch
                                {
                                    SongSingerType = "10";
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌手類別數值錯誤,已自動將其數值改為10: " + SongId + "|" + SongSongName;
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            string SongSongType = "";
            string SongFileName = Global.SongSrcDT.Rows[i]["Song_FileName"].ToString();
            string SongPath = Global.SongSrcDT.Rows[i]["Song_Path"].ToString() + @"\";

            List<string> SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));

            if (!File.Exists(Path.Combine(SongPath, SongFileName)) || SongPath == @"\" || Path.GetExtension(SongFileName) == "")
            {
                foreach (string HK2SongPath in Global.SongDBConvHK2SongPathList)
                {
                    if (Path.GetExtension(SongFileName) == "")
                    {
                        foreach (string format in SupportFormat)
                        {
                            if (File.Exists(Path.Combine(HK2SongPath, SongFileName + format)))
                            {
                                SongFileName = SongFileName + format;
                                SongPath = HK2SongPath + @"\";
                                break;
                            }
                        }

                        if (File.Exists(Path.Combine(SongPath, SongFileName)))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(HK2SongPath, SongFileName)))
                        {
                            SongPath = HK2SongPath + @"\";
                            break;
                        }
                    }
                }
            }

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
                        SongTrack = "0";
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為3立體聲,已自動將其數值改為0: " + SongId + "|" + SongSongName;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                }
                else
                {
                    SongTrack = "0";
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】此首歌曲歌曲聲道數值為空值,已自動將其數值改為0: " + SongId + "|" + SongSongName;
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

        #endregion

        #region --- SongDBConverterSongDB 取得歌曲資訊 ---

        public static string GetSongInfo(string SongInfoType, string SongInfoValue)
        {
            string infovalue = "";
            List<string> list = new List<string>();
            List<string> CrazyktvSongLangKeyWordList = new List<string>() { "國語,國", "台語,台,閩南,閩,臺語,臺", "粵語,粵,廣東", "日語,日文,日", "英語,英文,英", "客語,客", "原住民語,民謠", "韓語,韓", "兒歌,兒,童謠", "其它" };

            switch (SongInfoType)
            {
                case "SongLang":
                    foreach (string str in CrazyktvSongLangKeyWordList)
                    {
                        list = new List<string>(str.Split(','));
                        foreach (string liststr in list)
                        {
                            if (SongInfoValue == liststr)
                            {
                                infovalue = Global.CrazyktvSongLangList[CrazyktvSongLangKeyWordList.IndexOf(str)];
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
