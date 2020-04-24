using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {

        #region --- Debug 建立測試歌庫 ---

        private void Debug_CreateTestFile_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("請先變更資料庫裡的歌曲路徑!", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在建立測試歌庫,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CreateTestFileTask()) };


                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共建立 " + Global.TotalList[0] + " 個檔案,忽略 " + Global.TotalList[1] + " 個,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_CreateTestFileTask()
        {
            DataTable dt = new DataTable();
            string SqlStr = "select Song_Lang, Song_FileName, Song_Path from ktv_Song order by Song_Id";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SqlStr, "");

            Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals(langstr)
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        lock (LockThis) { Global.TotalList[2]++; }
                        if (!Directory.Exists(row.Field<string>("Song_Path"))) Directory.CreateDirectory(row.Field<string>("Song_Path"));
                        string file = Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"));
                        if (!File.Exists(file))
                        {
                            FileStream fs = new FileStream(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")), FileMode.Create);
                            fs.Close();
                            lock (LockThis) { Global.TotalList[0]++; }
                        }
                        else
                        {
                            lock (LockThis) { Global.TotalList[1]++; }
                        }

                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "正在建立 " + Global.TotalList[2] + " 個檔案...";
                        });
                    }
                }
            });
            dt.Dispose();
            dt = null;
        }
        #endif

        #endregion

        #region --- Debug 建立錢櫃資料 ---

        private void Debug_CreateCashboxTable_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要建立錢櫃資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在建立錢櫃資料,請稍待...";
                    /*
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CreateCashboxTableTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共建立 " + Global.TotalList[0] + " / " + Global.TotalList[1] + " 筆資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                    */
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_CreateCashboxTableTask()
        {
        }
        #endif

        #endregion

        #region --- Debug 更新錢櫃線上資料 ---

        private void Debug_CashboxUpdateOnlineData_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要更新錢櫃線上資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string file = string.Empty;
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);
                    Debug_Tooltip_Label.Text = "正在更新錢櫃線上資料,請稍待...";

                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CashboxUpdateOnlineDataTask()) };
                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate ()
                        {
                            Common_InitializeSongData(false, false, true, false, false, false);
                            Common_SwitchSetUI(true);
                            Global.TimerEndTime = DateTime.Now;
                            Debug_Tooltip_Label.Text = "總共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成資料更新。";
                        });
                    });
                }
            }
            #endif
        }

#if DEBUG
        private void Debug_CashboxUpdateOnlineDataTask()
        {
            string CashboxQuerySqlStr = "select Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate from ktv_Cashbox where Song_CreatDate >= #2020/04/10# order by Cashbox_Id, Song_CreatDate";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, CashboxQuerySqlStr, ""))
            {
                if (!Directory.Exists(Application.StartupPath + @"\Cashbox")) Directory.CreateDirectory(Application.StartupPath + @"\Cashbox");
                CommonFunc.ObjtoJSONFile(dt, Application.StartupPath + @"\Cashbox\cashbox_newsong.json");
            }
            Debug_CashboxUpdateBillboard("https://www.cashboxparty.com/billboard/billboard_newbill.asp", 2, 4, "cashbox_newbill.json");
            Debug_CashboxUpdateBillboard("https://www.cashboxparty.com/billboard/billboard_totalbill.asp", 2, 4, "cashbox_totalbill.json");
            Debug_CashboxUpdateBillboard("https://www.cashboxparty.com/billboard/billboard_otherlangbill.asp?langcode=2", 1, 1, "cashbox_cantonesebill.json");
            Debug_CashboxUpdateBillboard("https://www.cashboxparty.com/billboard/billboard_otherlangbill.asp?langcode=1", 1, 1, "cashbox_englishbill.json");
            Debug_CashboxUpdateBillboard("https://www.cashboxparty.com/billboard/billboard_otherlangbill.asp?langcode=3", 1, 1, "cashbox_japanesebill.json");

            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add("Cashbox_Id", typeof(string));
                dt.Columns.Add("Song_Lang", typeof(string));
                dt.Columns.Add("Song_SongName", typeof(string));
                dt.Columns.Add("Song_Singer", typeof(string));
                dt.Columns.Add("Song_Type", typeof(string));
                Debug_CashboxUpdateGoldSong("1", dt, "cashbox_3456goldsong.json");
                Debug_CashboxUpdateGoldSong("2", dt, "cashbox_3456goldsong.json");
                Debug_CashboxUpdateGoldSong("3", dt, "cashbox_3456goldsong.json");
                Debug_CashboxUpdateGoldSong("4", dt, "cashbox_3456goldsong.json");
            }
        }
        
        private void Debug_CashboxUpdateBillboard(string url, int tablecount, int tdcount, string jsonfile)
        {
            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add("Cashbox_Id", typeof(string));
                dt.Columns.Add("Song_Lang", typeof(string));
                dt.Columns.Add("Song_SongName", typeof(string));
                dt.Columns.Add("Song_Singer", typeof(string));

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                HtmlNode table;
                HtmlNodeCollection child;

                using (MemoryStream ms = CommonFunc.Download(url))
                {
                    if (ms.Length > 0)
                    {
                        ms.Position = 0;
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                switch (jsonfile)
                                {
                                    case "cashbox_newsong.json":
                                        Debug_Tooltip_Label.Text = "正在更新錢櫃新歌快報,請稍待...";
                                        break;
                                    case "cashbox_newbill.json":
                                        Debug_Tooltip_Label.Text = "正在更新錢櫃新歌排行榜,請稍待...";
                                        break;
                                    case "cashbox_totalbill.json":
                                        Debug_Tooltip_Label.Text = "正在更新錢櫃點播總排行,請稍待...";
                                        break;
                                    case "cashbox_cantonesebill.json":
                                        Debug_Tooltip_Label.Text = "正在更新錢櫃粵語排行榜,請稍待...";
                                        break;
                                    case "cashbox_englishbill.json":
                                        Debug_Tooltip_Label.Text = "正在更新錢櫃英語排行榜,請稍待...";
                                        break;
                                    case "cashbox_japanesebill.json":
                                        Debug_Tooltip_Label.Text = "正在更新錢櫃日語排行榜,請稍待...";
                                        break;
                                }
                            });

                            doc.Load(sr);
                            table = doc.DocumentNode.SelectSingleNode("//form[@name='form1']//table[1]");
                            child = table.SelectNodes("tr");
                            foreach (HtmlNode childnode in child)
                            {
                                DataRow dtrow = dt.NewRow();
                                HtmlNodeCollection td = childnode.SelectNodes("td");
                                bool exitloop = false;
                                foreach (HtmlNode tdnode in td)
                                {
                                    string data = Regex.Replace(tdnode.InnerText, @"^\s*|\s*$", ""); //去除頭尾空白
                                    switch (td.IndexOf(tdnode))
                                    {
                                        case 1:
                                            if (tdcount == 1)
                                            {
                                                if (CommonFunc.IsSongId(data))
                                                {
                                                    dtrow["Cashbox_Id"] = data;
                                                }
                                                else
                                                {
                                                    exitloop = true;
                                                }
                                            }
                                            break;
                                        case 2:
                                            if (tdcount == 1) dtrow["Song_Lang"] = data;
                                            break;
                                        case 3:
                                            if (tdcount == 1) dtrow["Song_SongName"] = data;
                                            break;
                                        case 4:
                                            if (tdcount == 1)
                                            {
                                                data = Regex.Replace(data, "、", "&");
                                                dtrow["Song_Singer"] = data;
                                            }
                                            else if (tdcount == 4)
                                            {
                                                if (CommonFunc.IsSongId(data))
                                                {
                                                    dtrow["Cashbox_Id"] = data;
                                                }
                                                else
                                                {
                                                    exitloop = true;
                                                }
                                            }
                                            break;
                                        case 5:
                                            if (tdcount == 4) dtrow["Song_Lang"] = data;
                                            break;
                                        case 6:
                                            if (tdcount == 4) dtrow["Song_SongName"] = data;
                                            break;
                                        case 7:
                                            if (tdcount == 4)
                                            {
                                                data = Regex.Replace(data, "、", "&");
                                                dtrow["Song_Singer"] = data;
                                            }
                                            break;
                                    }
                                    if (exitloop) break;
                                }
                                if(!exitloop) dt.Rows.Add(dtrow);
                            }

                            if (tablecount >= 2)
                            {
                                table = doc.DocumentNode.SelectSingleNode("//form[@name='form1']//table[2]");
                                child = table.SelectNodes("tr");
                                foreach (HtmlNode childnode in child)
                                {
                                    DataRow dtrow = dt.NewRow();
                                    bool exitloop = false;
                                    HtmlNodeCollection td = childnode.SelectNodes("td");
                                    foreach (HtmlNode tdnode in td)
                                    {
                                        string data = Regex.Replace(tdnode.InnerText, @"^\s*|\s*$", ""); //去除頭尾空白
                                        switch (td.IndexOf(tdnode))
                                        {
                                            case 1:
                                                if (tdcount == 1)
                                                {
                                                    if (CommonFunc.IsSongId(data))
                                                    {
                                                        dtrow["Cashbox_Id"] = data;
                                                    }
                                                    else
                                                    {
                                                        exitloop = true;
                                                    }
                                                }
                                                break;
                                            case 2:
                                                if (tdcount == 1) dtrow["Song_Lang"] = data;
                                                break;
                                            case 3:
                                                if (tdcount == 1) dtrow["Song_SongName"] = data;
                                                break;
                                            case 4:
                                                if (tdcount == 1)
                                                {
                                                    data = Regex.Replace(data, "、", "&");
                                                    dtrow["Song_Singer"] = data;
                                                }
                                                else if (tdcount == 4)
                                                {
                                                    if (CommonFunc.IsSongId(data))
                                                    {
                                                        dtrow["Cashbox_Id"] = data;
                                                    }
                                                    else
                                                    {
                                                        exitloop = true;
                                                    }
                                                }
                                                break;
                                            case 5:
                                                if (tdcount == 4) dtrow["Song_Lang"] = data;
                                                break;
                                            case 6:
                                                if (tdcount == 4) dtrow["Song_SongName"] = data;
                                                break;
                                            case 7:
                                                if (tdcount == 4)
                                                {
                                                    data = Regex.Replace(data, "、", "&");
                                                    dtrow["Song_Singer"] = data;
                                                }
                                                break;
                                        }
                                        if (exitloop) break;
                                    }
                                    if (!exitloop) dt.Rows.Add(dtrow);
                                }
                            }
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    if (!Directory.Exists(Application.StartupPath + @"\Cashbox")) Directory.CreateDirectory(Application.StartupPath + @"\Cashbox");
                    CommonFunc.ObjtoJSONFile(dt, Application.StartupPath + @"\Cashbox\" + jsonfile);
                }
            }
        }

        private void Debug_CashboxUpdateGoldSong(string songtype, DataTable dt, string jsonfile)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlNode table;
            HtmlNodeCollection child;

            int pages = 0;
            string url = string.Format("https://www.cashboxparty.com/billboard/billboard_3456gold.asp?SongType={0}&page=1", songtype);
            using (MemoryStream ms = CommonFunc.Download(url))
            {
                if (ms.Length > 0)
                {
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        this.BeginInvoke((Action)delegate ()
                        {
                            switch (songtype)
                            {
                                case "1":
                                    Debug_Tooltip_Label.Text = "正在更新錢櫃三四年級台語金曲,請稍待...";
                                    break;
                                case "2":
                                    Debug_Tooltip_Label.Text = "正在更新錢櫃三四年級國語金曲,請稍待...";
                                    break;
                                case "3":
                                    Debug_Tooltip_Label.Text = "正在更新錢櫃五六年級台語金曲,請稍待...";
                                    break;
                                case "4":
                                    Debug_Tooltip_Label.Text = "正在更新錢櫃五六年級國語金曲,請稍待...";
                                    break;
                            }
                        });

                        doc.Load(sr);
                        table = doc.DocumentNode.SelectSingleNode("//form[@name='form1']//table[1]");
                        child = table.SelectNodes("tr");
                        foreach (HtmlNode childnode in child)
                        {
                            DataRow dtrow = dt.NewRow();
                            HtmlNodeCollection td = childnode.SelectNodes("td");
                            foreach (HtmlNode tdnode in td)
                            {
                                string data = Regex.Replace(tdnode.InnerText, @"^\s*|\s*$", ""); //去除頭尾空白
                                if (data.Contains("::::::"))
                                {
                                    MatchCollection matches = Regex.Matches(data, @"(\d+?)(\s{2}::::::$)");
                                    if (matches.Count > 0)
                                    {
                                        pages = Convert.ToInt32(matches[0].Groups[1].Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (pages > 0)
            {
                for (int page = 1; page <= pages; page++)
                {
                    url = string.Format("https://www.cashboxparty.com/billboard/billboard_3456gold.asp?SongType={0}&page={1}", songtype, page);

                    using (MemoryStream ms = CommonFunc.Download(url))
                    {
                        if (ms.Length > 0)
                        {
                            ms.Position = 0;
                            using (StreamReader sr = new StreamReader(ms))
                            {
                                doc.Load(sr);
                                table = doc.DocumentNode.SelectSingleNode("//form[@name='form1']//table[2]");
                                child = table.SelectNodes("tr");
                                foreach (HtmlNode childnode in child)
                                {
                                    DataRow dtrow = dt.NewRow();
                                    bool exitloop = false;
                                    HtmlNodeCollection td = childnode.SelectNodes("td");
                                    foreach (HtmlNode tdnode in td)
                                    {
                                        string data = Regex.Replace(tdnode.InnerText, @"^\s*|\s*$", ""); //去除頭尾空白
                                        switch (td.IndexOf(tdnode))
                                        {
                                            case 0:
                                                if (CommonFunc.IsSongId(data))
                                                {
                                                    dtrow["Cashbox_Id"] = data;
                                                }
                                                else
                                                {
                                                    exitloop = true;
                                                }
                                                break;
                                            case 1:
                                                dtrow["Song_Lang"] = data;
                                                break;
                                            case 2:
                                                dtrow["Song_SongName"] = data;
                                                break;
                                            case 3:
                                                dtrow["Song_Singer"] = data;
                                                break;
                                        }
                                        if (exitloop) break;
                                    }
                                    if (!exitloop)
                                    {
                                        dtrow["Song_Type"] = songtype;
                                        dt.Rows.Add(dtrow);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (songtype == "4" && dt.Rows.Count > 0)
            {
                if (!Directory.Exists(Application.StartupPath + @"\Cashbox")) Directory.CreateDirectory(Application.StartupPath + @"\Cashbox");
                CommonFunc.ObjtoJSONFile(dt, Application.StartupPath + @"\Cashbox\" + jsonfile);
            }
        }

#endif

        #endregion

        #region --- Debug 更新錢櫃遺漏歌曲 ---

        private void Debug_CashboxUpdateLostSong_Button_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要更新錢櫃遺漏歌曲嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在更新錢櫃遺漏歌曲,請稍待...";

                    List<string> datelist = new List<string>();
                    List<string> updatelist = new List<string>();
                    using (DataTable dt = ((DataTable)Cashbox_DateQuery_ComboBox.DataSource).Copy())
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (datelist.Count > 0)
                            {
                                DateTime dt1 = DateTime.Parse(datelist[dt.Rows.IndexOf(row) - 1]);
                                DateTime dt2 = DateTime.Parse(row["Display"].ToString());
                                TimeSpan ts = dt1 - dt2;
                                if (ts.Days > 8)
                                {
                                    for (int i = 1; i < ts.Days; i++)
                                    {
                                        if (updatelist.IndexOf(dt2.AddDays(i).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)) < 0)
                                        {
                                            updatelist.Add(dt2.AddDays(i).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
                                        }
                                    }
                                }
                            }
                            datelist.Add(row["Display"].ToString());
                        }
                    }
                    datelist.Clear();
                    /*
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CashboxUpdateLostSongTask(updatelist)) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate ()
                        {
                            updatelist.Clear();
                            Debug_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆資料,失敗 " + Global.TotalList[1] + " 筆,忽略 " + Global.TotalList[2] + " 筆,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_InitializeSongData(false, false, true, false, false, false);
                            Common_SwitchSetUI(true);
                        });
                    });
                    */
                }
            }
#endif
        }

#if DEBUG
        private void Debug_CashboxUpdateLostSongTask(List<string> sDateList)
        {
        }
#endif

        #endregion

        #region --- Debug 記錄錢櫃無資料歌手 ---

        private void Debug_CashboxNonSingerDataLog_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            Global.TimerStartTime = DateTime.Now;
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            SongMaintenance.CreateSongDataTable();
            Common_SwitchSetUI(false);

            Debug_Tooltip_Label.Text = "正在解析錢櫃歌庫歌手資料,請稍待...";

            var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CashboxNonSingerDataLogTask()) };

            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
            {
                Global.TimerEndTime = DateTime.Now;
                this.BeginInvoke((Action)delegate()
                {
                    Debug_Tooltip_Label.Text = "總共從錢櫃歌庫解析出 " + Global.TotalList[0] + " 筆歌手資料,查詢到 " + Global.TotalList[1] + " 筆歌手無資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    Common_SwitchSetUI(true);
                });
                SongMaintenance.DisposeSongDataTable();
            });
            #endif
        }

        #if DEBUG
        private void Debug_CashboxNonSingerDataLogTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> list = new List<string>();
            List<string> Singerlist = new List<string>();
            List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));

            DataTable dt = new DataTable();
            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, Count(Song_Singer) AS Song_SingerCount FROM ktv_Cashbox GROUP BY Song_Singer HAVING (Count(Song_Singer)>0) ORDER BY First(Song_Singer)";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SingerQuerySqlStr, "");

            if (dt.Rows.Count > 0)
            {
                string SingerName = "";

                foreach (DataRow row in dt.AsEnumerable())
                {
                    SingerName = row["Song_Singer"].ToString();

                    List<string> slist = CommonFunc.GetChorusSingerList(SingerName);
                    foreach (string str in slist)
                    {
                        string ChorusSingerName = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                        if (Singerlist.IndexOf(ChorusSingerName) < 0)
                        {
                            // 查找資料庫預設歌手資料表
                            if (Global.AllSingerLowCaseList.IndexOf(ChorusSingerName.ToLower()) < 0)
                            {
                                if (list.IndexOf(ChorusSingerName) < 0)
                                {
                                    list.Add(ChorusSingerName);
                                    lock (LockThis) { Global.TotalList[1]++; }
                                }
                            }
                            Singerlist.Add(ChorusSingerName);
                        }
                    }
                    slist.Clear();
                    slist = null;

                    lock (LockThis) { Global.TotalList[0]++; }
                    this.BeginInvoke((Action)delegate()
                    {
                        Debug_Tooltip_Label.Text = "已解析到 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                    });
                }
            }
            Singerlist.Clear();
            dt.Dispose();
            dt = null;

            if (list.Count > 0)
            {
                foreach (string str in list)
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【記錄無資料歌手】未在預設歌手資料表的歌手: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }
        }
        #endif

        #endregion

        #region --- Debug 匯入/匯出錢櫃資料 ---

        private void Debug_CashboxExport_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            List<string> list = new List<string>()
            {
                "ktv_Version|1",
                "ktv_UpdDate|" + DateTime.Now
            };

            string sqlColumnStr = "Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate";
            string CashboxQuerySqlStr = "select " + sqlColumnStr + " from ktv_Cashbox order by Cashbox_Id";

            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, CashboxQuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        list.Add("ktv_Cashbox|" + row["Cashbox_Id"].ToString() + "|" + row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString() + "|" + row["Song_SongName"].ToString() + "|" + row["Song_CreatDate"].ToString());
                    }
                }

                if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\UpdateCashboxDB.txt");
                foreach (string str in list)
                {
                    sw.WriteLine(str);
                }
                Debug_Tooltip_Label.Text = @"已將錢櫃資料匯出至【SongMgr\Backup\UpdateCashboxDB.txt】檔案。";
                sw.Close();
                list.Clear();
            }
            #endif
        }

        private void Debug_CashboxImport_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\UpdateCashboxDB.txt"))
                {
                    if (Debug_Tooltip_Label.Text == "錢櫃資料檔案不存在!") Debug_Tooltip_Label.Text = "";
                    if (MessageBox.Show("你確定要匯入錢櫃資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Global.TimerStartTime = DateTime.Now;
                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        Common_SwitchSetUI(false);

                        Debug_Tooltip_Label.Text = "正在匯入錢櫃資料,請稍待...";
                        var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CashboxImportTask()) };

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            Global.TimerEndTime = DateTime.Now;
                            this.BeginInvoke((Action)delegate()
                            {
                                Debug_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 筆錢櫃資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                                Common_SwitchSetUI(true);
                            });
                        });
                    }
                }
                else
                {
                    Debug_Tooltip_Label.Text = "錢櫃資料檔案不存在!";
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_CashboxImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> Addlist = new List<string>();

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
            {
                string TruncateSqlStr = "delete * from ktv_Cashbox";
                using (OleDbCommand cmd = new OleDbCommand(TruncateSqlStr, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\UpdateCashboxDB.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        Addlist.Add(sr.ReadLine());
                    }
                }

                string sqlColumnStr = "Cashbox_Id, Song_Lang, Song_SongName, Song_Singer, Song_CreatDate";
                string sqlValuesStr = "@CashboxId, @SongLang, @SongSongName, @SongSinger, @SongCreatDate";
                string InsertSqlStr = "insert into ktv_Cashbox ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";

                using (OleDbCommand cmd = new OleDbCommand(InsertSqlStr, conn))
                {
                    List<string> list = new List<string>();

                    foreach (string AddStr in Addlist)
                    {
                        list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));

                        switch (list[0])
                        {
                            case "ktv_Cashbox":
                                cmd.Parameters.AddWithValue("@CashboxId", list[1]);
                                cmd.Parameters.AddWithValue("@SongLang", list[2]);
                                cmd.Parameters.AddWithValue("@SongSongName", list[4]);
                                cmd.Parameters.AddWithValue("@SongSinger", list[3]);
                                cmd.Parameters.AddWithValue("@SongCreatDate", list[5]);

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                lock (LockThis) { Global.TotalList[0]++; }
                                break;
                        }
                        list.Clear();

                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 筆錢櫃資料,請稍待...";
                        });
                    }
                }
            }
            Addlist.Clear();
        }
        #endif

        #endregion

        #region --- Debug 建立表格資料 ---

        private void Debug_CreateDataTable_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要建立表格資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在建立表格資料,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CreateDataTableTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "新增表格資料完畢,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_CreateDataTableTask()
        {
            string DatabaseFile = Global.CrazyktvSongMgrDatabaseFile;
            //string TableName = "ktv_SongMgr";
            //string Columns = " (Config_Id INTEGER NOT NULL PRIMARY KEY, Config_Type TEXT(30) WITH COMPRESSION, Config_Value TEXT(60) WITH COMPRESSION)";
            string TableName = "ktv_Version";
            string Columns = " (Id INTEGER NOT NULL PRIMARY KEY, SongDB TEXT(10), CashboxUpdDate DATETIME)";

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(DatabaseFile, ""))
            {
                OleDbCommand[] cmds =
                {
                    new OleDbCommand("create table " + TableName + Columns, conn)
                };
                cmds[0].ExecuteNonQuery();
            }
        }
        #endif

        #endregion

        #region --- Debug 建立欄位資料 ---

        private void Debug_CreateDataColumn_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要建立資料欄位嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在建立資料欄位,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_CreateDataColumnTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆新欄位資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_CreateDataColumnTask()
        {
            string DatabaseFile = Global.CrazyktvSongMgrDatabaseFile;
            string TableName = "ktv_AllSinger";
            string ColumnName = "Singer_Group";
            string ColumnType = " TEXT(10)";
            string WhereColumn = "Singer_Id";

            List<string> WhereValueList = new List<string>();
            string QuerySqlStr = "select * from " + TableName;
            using (DataTable dt = CommonFunc.GetOleDbDataTable(DatabaseFile, QuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        WhereValueList.Add(row[WhereColumn].ToString());
                    }
                }
            }

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(DatabaseFile, ""))
            {
                OleDbCommand[] cmds =
                {
                    new OleDbCommand("alter table " + TableName +" add column " + ColumnName + ColumnType, conn),
                    new OleDbCommand("update " + TableName +" set " + ColumnName + " = 0 where " + WhereColumn +" = @WhereValue", conn)
                };
                cmds[0].ExecuteNonQuery();

                if (WhereValueList.Count > 0)
                {
                    foreach (string value in WhereValueList)
                    {
                        cmds[1].Parameters.AddWithValue("@WhereValue", value);

                        try
                        {
                            cmds[1].ExecuteNonQuery();
                            Global.TotalList[0]++;
                            this.BeginInvoke((Action)delegate()
                            {
                                Debug_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 筆新欄位資料,請稍待...";
                            });
                        }
                        catch
                        {
                            Global.TotalList[1]++;
                            Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "更新欄位資料時發生未知的錯誤: " + value;
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                        }
                        cmds[1].Parameters.Clear();
                    }
                }
            }
            WhereValueList.Clear();
        }
        #endif
        
        #endregion

        #region --- Debug 更新欄位資料 ---

        private void Debug_UpdateDataColumn_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要更新資料欄位嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在更新資料欄位,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_UpdateDataColumnTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆欄位資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            #endif
        }

        #if DEBUG
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:必須檢視 SQL 查詢中是否有安全性弱點")]
        private void Debug_UpdateDataColumnTask()
        {
            string DatabaseFile = Global.CrazyktvSongMgrDatabaseFile;
            string TableName = "ktv_Cashbox";
            string ColumnName = "Song_CreatDate";
            string ColumnValue = DateTime.Parse("2016/01/01").ToString();
            string WhereColumn = "Cashbox_Id";

            List<string> WhereValueList = new List<string>();
            string QuerySqlStr = "select * from " + TableName;
            using (DataTable dt = CommonFunc.GetOleDbDataTable(DatabaseFile, QuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        WhereValueList.Add(row[WhereColumn].ToString());
                    }
                }
            }

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(DatabaseFile, ""))
            {
                OleDbCommand[] cmds =
                {
                    new OleDbCommand("update " + TableName +" set " + ColumnName + " = '" + ColumnValue +"' where " + WhereColumn +" = @WhereValue", conn)
                };

                if (WhereValueList.Count > 0)
                {
                    foreach (string value in WhereValueList)
                    {
                        cmds[0].Parameters.AddWithValue("@WhereValue", value);

                        try
                        {
                            cmds[0].ExecuteNonQuery();
                            Global.TotalList[0]++;
                            this.BeginInvoke((Action)delegate()
                            {
                                Debug_Tooltip_Label.Text = "正在更新第 " + Global.TotalList[0] + " 筆欄位資料,請稍待...";
                            });
                        }
                        catch
                        {
                            Global.TotalList[1]++;
                            Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "更新欄位資料時發生未知的錯誤: " + value;
                            Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                        }
                        cmds[0].Parameters.Clear();
                    }
                }
            }
            WhereValueList.Clear();
        }
        #endif

        #endregion

        #region --- Debug 錢櫃資料編輯 ---

        private void Cashbox_EditMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            #if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                Cashbox_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(270 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                Cashbox_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                Cashbox_Edit_GroupBox.Visible = true;
                Cashbox_TabControl.Visible = false;
                Cashbox_QueryFilter_GroupBox.Visible = false;
                Cashbox_UpdDate_GroupBox.Visible = false;
                Cashbox_Maintenance_GroupBox.Visible = false;

                Global.CashboxMultiEdit = false;
                Cashbox_InitializeEditControl();

                int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;
                Cashbox_DataGridView_SelectionChanged(new object(), new EventArgs());
                if (SelectedRowsCount > 1) Cashbox_DataGridView_MouseUp(new object(), null);

                Cashbox_QueryStatus_Label.Text = "已進入編輯模式...";
            }
            else
            {
                Cashbox_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(296 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                Cashbox_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(365 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                Cashbox_EditMode_CheckBox.Enabled = (Cashbox_DataGridView.RowCount == 0) ? false : true;
                Cashbox_Edit_GroupBox.Visible = false;
                Cashbox_TabControl.Visible = true;
                Cashbox_QueryFilter_GroupBox.Visible = true;
                Cashbox_UpdDate_GroupBox.Visible = true;
                Cashbox_Maintenance_GroupBox.Visible = true;

                Cashbox_QueryStatus_Label.Text = "已進入檢視模式...";
            }
            Cashbox_DataGridView.Focus();
        #endif
        }

        private void Cashbox_EditSongLang_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            #if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                if (Cashbox_EditSongLang_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.CashboxDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.CashboxMultiEditUpdateList[0] = (Cashbox_EditSongLang_ComboBox.Text != "不變更") ? true : false;
                    }
                }
            }
            #endif
        }

        private void Cashbox_EditSongCreatDate_DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            #if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                if (Global.CashboxDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.CashboxMultiEditUpdateList[1] = true;
                }
            }
            #endif
        }

        private void Cashbox_EditSongSinger_TextBox_Validated(object sender, EventArgs e)
        {
            #if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                if (Global.CashboxDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;
                string SongSinger = Cashbox_EditSongSinger_TextBox.Text;

                if (SelectedRowsCount > 1)
                {
                    Global.CashboxMultiEditUpdateList[2] = (Cashbox_EditSongSinger_TextBox.Text != "") ? true : false;
                }
            }
            #endif
        }

        private void Cashbox_EditApplyChanges_Button_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                if (Global.CashboxDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;
                List<string> UpdateList = new List<string>();
                Common_SwitchSetUI(false);

                string SongId;
                string SongLang;
                string SongSongName;
                string SongSinger;
                string SongCreatDate;

                if (SelectedRowsCount > 1)
                {
                    foreach (DataGridViewRow row in Cashbox_DataGridView.SelectedRows)
                    {
                        SongId = row.Cells["Cashbox_Id"].Value.ToString();
                        SongLang = (Global.CashboxMultiEditUpdateList[0]) ? ((DataRowView)Cashbox_EditSongLang_ComboBox.SelectedItem)[0].ToString() : row.Cells["Song_Lang"].Value.ToString();
                        SongSongName = row.Cells["Song_SongName"].Value.ToString();
                        SongCreatDate = (Global.CashboxMultiEditUpdateList[1]) ? Cashbox_EditSongCreatDate_DateTimePicker.Value.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) : row.Cells["Song_CreatDate"].Value.ToString();
                        SongSinger = (Global.CashboxMultiEditUpdateList[2]) ? Cashbox_EditSongSinger_TextBox.Text : row.Cells["Song_Singer"].Value.ToString();

                        UpdateList.Add(SongId + "|" + SongLang + "|" + SongSongName + "|" + SongSinger + "|" + SongCreatDate);
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    foreach (DataGridViewRow row in Cashbox_DataGridView.SelectedRows)
                    {
                        SongId = row.Cells["Cashbox_Id"].Value.ToString();
                        SongLang = ((DataRowView)Cashbox_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        SongSongName = Cashbox_EditSongSongName_TextBox.Text;
                        SongCreatDate = Cashbox_EditSongCreatDate_DateTimePicker.Value.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                        SongSinger = Cashbox_EditSongSinger_TextBox.Text;

                        UpdateList.Add(SongId + "|" + SongLang + "|" + SongSongName + "|" + SongSinger + "|" + SongCreatDate);
                    }
                }

                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Global.CashboxDataGridViewRestoreSelectList = new List<string>();
                Global.CashboxDataGridViewRestoreCurrentRow = Cashbox_DataGridView.CurrentRow.Cells["Cashbox_Id"].Value.ToString();
                Cashbox_DataGridView.Sorted -= new EventHandler(Cashbox_DataGridView_Sorted);
                Cashbox_QueryStatus_Label.Text = "正在更新錢櫃資料,請稍待...";

                using (DataTable UpdateDT = (DataTable)Cashbox_DataGridView.DataSource)
                {
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Cashbox_SongUpdate(UpdateList, UpdateDT)) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Cashbox_DataGridView.Sorted += new EventHandler(Cashbox_DataGridView_Sorted);
                            Cashbox_DataGridView_Sorted(new object(), new EventArgs());

                            SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;

                            if (SelectedRowsCount > 1)
                            {
                                Global.CashboxDataGridViewSelectList = new List<string>();

                                foreach (DataGridViewRow row in Cashbox_DataGridView.SelectedRows)
                                {
                                    SongId = row.Cells["Cashbox_Id"].Value.ToString();
                                    SongLang = row.Cells["Song_Lang"].Value.ToString();
                                    SongSinger = row.Cells["Song_Singer"].Value.ToString();
                                    SongSongName = row.Cells["Song_SongName"].Value.ToString();
                                    SongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();

                                    string SelectValue = SongId + "|" + SongLang + "|" + SongSinger + "|" + SongSongName + "|" + SongCreatDate;
                                    Global.CashboxDataGridViewSelectList.Add(SelectValue);
                                }
                            }

                            Common_InitializeSongData(false, false, true, false, false, false);
                            Cashbox_QueryStatus_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆資料,失敗 " + Global.TotalList[1] + " 筆。";
                            Common_SwitchSetUI(true);
                        });
                        UpdateList.Clear();
                    });
                }
            }
#endif
        }

        private void Cashbox_SongUpdate(List<string> UpdateList, DataTable UpdateDT)
        {
            #if DEBUG
            if (UpdateList.Count <= 0) return;
            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
            {
                string sqlUpdStr = "Cashbox_Id = @CashboxId, Song_Lang = @SongLang, Song_SongName = @SongSongName, Song_Singer = @SongSinger, Song_CreatDate = @SongCreatDate";
                string UpdSqlStr = "update ktv_Cashbox set " + sqlUpdStr + " where Cashbox_Id = @OldCashboxId";

                OleDbCommand UpdCmd = new OleDbCommand(UpdSqlStr, conn);
                List<string> valuelist;
                List<string> dtvaluelist;

                foreach (string SongData in UpdateList)
                {
                    valuelist = new List<string>(SongData.Split('|'));

                    UpdCmd.Parameters.AddWithValue("@CashboxId", valuelist[0]);
                    UpdCmd.Parameters.AddWithValue("@SongLang", valuelist[1]);
                    UpdCmd.Parameters.AddWithValue("@SongSongName", valuelist[2]);
                    UpdCmd.Parameters.AddWithValue("@SongSinger", valuelist[3]);
                    UpdCmd.Parameters.AddWithValue("@SongCreatDate", valuelist[4]);
                    UpdCmd.Parameters.AddWithValue("@OldCashboxId", valuelist[0]);

                    try
                    {
                        UpdCmd.ExecuteNonQuery();
                        Global.TotalList[0]++;

                        this.BeginInvoke((Action)delegate()
                        {
                            dtvaluelist = new List<string>(SongData.Split('|'));
                            Global.CashboxDataGridViewRestoreSelectList.Add(dtvaluelist[0]);

                            var query = from row in UpdateDT.AsEnumerable()
                                        where row["Cashbox_Id"].ToString() == dtvaluelist[0]
                                        select row;

                            foreach (DataRow row in query)
                            {
                                row["Cashbox_Id"] = dtvaluelist[0];
                                row["Song_Lang"] = dtvaluelist[1];
                                row["Song_SongName"] = dtvaluelist[2];
                                row["Song_Singer"] = dtvaluelist[3];
                                row["Song_CreatDate"] = dtvaluelist[4];
                            }

                            dtvaluelist.Clear();
                            Cashbox_QueryStatus_Label.Text = "正在更新第 " + Global.TotalList[0] + " 首歌曲,請稍待...";
                        });
                    }
                    catch
                    {
                        Global.TotalList[1]++;
                        Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                        Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "更新資料時發生未知的錯誤: " + SongData;
                        Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                    }
                    UpdCmd.Parameters.Clear();
                    valuelist.Clear();
                }
            }
            #endif
        }

        private void Cashbox_InitializeEditControl()
        {
            #if DEBUG
            Cashbox_EditSongId_TextBox.Text = "";
            Cashbox_EditSongLang_ComboBox.SelectedValue = 1;
            Cashbox_EditSongCreatDate_DateTimePicker.Value = DateTime.Now;
            Cashbox_EditSongSinger_TextBox.Text = "";
            Cashbox_EditSongSongName_TextBox.Text = "";

            Cashbox_EditSongId_TextBox.Enabled = false;
            Cashbox_EditSongLang_ComboBox.Enabled = false;
            Cashbox_EditSongCreatDate_DateTimePicker.Enabled = false;
            Cashbox_EditSongSinger_TextBox.Enabled = false;
            Cashbox_EditSongSongName_TextBox.Enabled = false;
            Cashbox_EditApplyChanges_Button.Enabled = false;
            #endif
        }

        private void Cashbox_GetSongEditComboBoxList(bool MultiEdit)
        {
            #if DEBUG
            Global.CashboxMultiEdit = MultiEdit;
            Cashbox_EditSongLang_ComboBox.DataSource = Debug.GetEditSongLangList(MultiEdit);
            Cashbox_EditSongLang_ComboBox.DisplayMember = "Display";
            Cashbox_EditSongLang_ComboBox.ValueMember = "Value";
            #endif
        }

        #endregion

        #region --- Debug 匯入/匯出新增歌手 ---

        private void Debug_NewSingerExport_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要匯出新增歌手嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    SingerMgr.CreateSongDataTable();
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在匯出新增歌手,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_NewSingerExportTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共從歌庫歌曲解析出 " + Global.TotalList[0] + " 筆歌手資料,查詢到 " + Global.TotalList[1] + " 筆歌手無資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                        SingerMgr.DisposeSongDataTable();
                    });
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_NewSingerExportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, First(Song_SingerType) AS Song_SingerType, Count(Song_Singer) AS Song_SingerCount FROM ktv_Song GROUP BY Song_Singer HAVING First(Song_SingerType)<=10 AND First(Song_SingerType)<>8 AND First(Song_SingerType)<>9 AND Count(Song_Singer)>0 ORDER BY First(Song_SingerType), First(Song_Singer)";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SingerQuerySqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    List<string> list = new List<string>();
                    List<string> Singerlist = new List<string>();
                    List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, @"\|", RegexOptions.IgnoreCase));

                    string SingerName = "";
                    string SingerType = "";

                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        SingerName = row["Song_Singer"].ToString();
                        SingerType = row["Song_SingerType"].ToString();

                        if (SingerType == "3")
                        {
                            List<string> slist = CommonFunc.GetChorusSingerList(SingerName);
                            foreach (string str in slist)
                            {
                                string ChorusSingerName = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                if (Singerlist.IndexOf(ChorusSingerName) < 0)
                                {
                                    // 查找資料庫預設歌手資料表
                                    if (SingerMgr.AllSingerLowCaseList.IndexOf(ChorusSingerName.ToLower()) < 0)
                                    {
                                        if (list.IndexOf(ChorusSingerName) < 0)
                                        {
                                            list.Add(ChorusSingerName + "|" + SingerType);
                                            lock (LockThis) { Global.TotalList[1]++; }
                                        }
                                    }
                                    Singerlist.Add(ChorusSingerName);
                                }
                            }
                            slist.Clear();
                            slist = null;
                        }
                        else
                        {
                            if (Singerlist.IndexOf(SingerName) < 0)
                            {
                                if (SingerMgr.AllSingerLowCaseList.IndexOf(SingerName.ToLower()) < 0)
                                {
                                    if (list.IndexOf(SingerName) < 0)
                                    {
                                        list.Add(SingerName + "|" + SingerType);
                                        lock (LockThis) { Global.TotalList[1]++; }
                                    }
                                }
                                Singerlist.Add(SingerName);
                            }
                        }
                        lock (LockThis) { Global.TotalList[0]++; }
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "已解析到 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                        });
                    }
                    SpecialStrlist.Clear();
                    Singerlist.Clear();

                    if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                    using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\NewSinger.txt"))
                    {
                        foreach (string str in list)
                        {
                            sw.WriteLine(str);
                        }
                    }
                    list.Clear();
                }
            }
        }
        #endif

        private void Debug_NewSingerImport_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (File.Exists(Application.StartupPath + @"\SongMgr\Backup\NewSinger.txt"))
                {
                    if (Debug_Tooltip_Label.Text == "新增歌手資料檔案不存在!") Debug_Tooltip_Label.Text = "";
                    if (MessageBox.Show("你確定要匯入新增歌手嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Global.TimerStartTime = DateTime.Now;
                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        Common_SwitchSetUI(false);

                        Debug_Tooltip_Label.Text = "正在匯入新增歌手,請稍待...";
                        var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_NewSingerImportTask()) };

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            Global.TimerEndTime = DateTime.Now;
                            this.BeginInvoke((Action)delegate()
                            {
                                Debug_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 位歌手資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                                Common_SwitchSetUI(true);
                            });
                        });
                    }
                }
                else
                {
                    Debug_Tooltip_Label.Text = "新增歌手資料檔案不存在!";
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_NewSingerImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> Addlist = new List<string>();

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
            {
                using (StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\NewSinger.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        Addlist.Add(sr.ReadLine());
                    }
                }
                string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
                string sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
                string SingerAddSqlStr = "insert into ktv_AllSinger" + " ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";

                using (OleDbCommand singercmd = new OleDbCommand(SingerAddSqlStr, conn))
                {
                    int MaxSingerId = CommonFunc.GetMaxSingerId("ktv_AllSinger", Global.CrazyktvSongMgrDatabaseFile);
                    List<string> UnUsedSingerIdList = CommonFunc.GetUnusedSingerId("ktv_AllSinger", Global.CrazyktvSongMgrDatabaseFile);
                    string SingerId = "";
                    List <string> list = new List<string>();
                    List <string> SpellList = new List<string>();

                    foreach (string AddStr in Addlist)
                    {
                        if (UnUsedSingerIdList.Count > 0)
                        {
                            SingerId = UnUsedSingerIdList[0];
                            UnUsedSingerIdList.RemoveAt(0);
                        }
                        else
                        {
                            MaxSingerId++;
                            SingerId = MaxSingerId.ToString();
                        }

                        list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));
                        SpellList = CommonFunc.GetSongNameSpell(list[0]);

                        singercmd.Parameters.AddWithValue("@SingerId", SingerId);
                        singercmd.Parameters.AddWithValue("@SingerName", list[0]);
                        singercmd.Parameters.AddWithValue("@SingerType", list[1]);
                        singercmd.Parameters.AddWithValue("@SingerSpell", SpellList[0]);
                        singercmd.Parameters.AddWithValue("@SingerStrokes", SpellList[2]);
                        singercmd.Parameters.AddWithValue("@SingerSpellNum", SpellList[1]);
                        singercmd.Parameters.AddWithValue("@SingerPenStyle", SpellList[3]);

                        singercmd.ExecuteNonQuery();
                        singercmd.Parameters.Clear();

                        SpellList.Clear();
                        list.Clear();

                        lock (LockThis)
                        {
                            Global.TotalList[0]++;
                        }

                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                        });
                    }
                }
            }
            Addlist.Clear();
        }
        #endif

        #endregion

        #region --- Debug 新增設定資料 ---

        private void Debug_ConfigData_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            #if DEBUG
            switch (Debug_ConfigData_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    Debug.ConfigDataType = "SpecialStr";
                    break;
                case "2":
                    Debug.ConfigDataType = "SingerGroup";
                    break;

            }
            #endif
        }

        private void Debug_ConfigData_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Debug_ConfigData_TextBox.Text != "")
            {
                if (Global.CrazyktvDatabaseStatus)
                {
                    if (MessageBox.Show("你確定要加入設定資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Global.TimerStartTime = DateTime.Now;
                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        Common_SwitchSetUI(false);

                        Debug_Tooltip_Label.Text = "正在加入設定資料,請稍待...";

                        string ConfigType = Debug.ConfigDataType;
                        string ConfigValue = Debug_ConfigData_TextBox.Text;
                        int ConfigId;

                        string SqlStr = "select * from ktv_SongMgr";
                        using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SqlStr, ""))
                        {
                            ConfigId = dt.Rows.Count + 1;
                        }
                        
                        var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_AddConfigDataTask(ConfigType, ConfigValue, ConfigId)) };

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            Common_InitializeSongData(false, false, false, true, true, false);
                            Global.TimerEndTime = DateTime.Now;
                            this.BeginInvoke((Action)delegate()
                            {
                                Debug_Tooltip_Label.Text = "加入設定資料完畢,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                                Common_SwitchSetUI(true);
                            });
                        });
                    }
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_AddConfigDataTask(string ConfigType, string ConfigValue, int ConfigId)
        {
            string DatabaseFile = Global.CrazyktvSongMgrDatabaseFile;
            string sqlColumnStr = "Config_Id, Config_Type, Config_Value";
            string sqlValuesStr = "@ConfigId, @ConfigType, @ConfigValue";
            string AddSqlStr = "insert into ktv_SongMgr ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(DatabaseFile, ""))
            {
                OleDbCommand[] cmds =
                {
                    new OleDbCommand(AddSqlStr, conn)
                };

                cmds[0].Parameters.AddWithValue("@ConfigId", ConfigId);
                cmds[0].Parameters.AddWithValue("@ConfigType", ConfigType);
                cmds[0].Parameters.AddWithValue("@ConfigValue", ConfigValue);
                cmds[0].ExecuteNonQuery();
            }
        }
        #endif

        #endregion

        #region --- Debug 匯入/匯出設定資料 ---

        private void Debug_ConfigDataExport_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要匯出設定資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在匯出設定資料,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_ConfigDataExportTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Common_InitializeSongData(false, false, false, true, true, false);
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共匯出 " + Global.TotalList[0] + " 筆設定資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_ConfigDataExportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            string SqlStr = "select * from ktv_SongMgr order by Config_Type, Config_Value";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SqlStr, ""))
            {
                if (dt.Rows.Count > 0)
                {
                    List<string> list = new List<string>();

                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        list.Add(row["Config_Type"].ToString() + "|" + row["Config_Value"].ToString());
                    }

                    if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                    using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\ConfigData.txt"))
                    {
                        foreach (string str in list)
                        {
                            sw.WriteLine(str);
                            Global.TotalList[0]++;
                        }
                    }
                    list.Clear();
                }
            }
        }
        #endif

        private void Debug_ConfigDataImport_Button_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要匯入設定資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在匯入設定資料,請稍待...";
                    var tasks = new List<Task>() { Task.Factory.StartNew(() => Debug_ConfigDataImportTask()) };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Common_InitializeSongData(false, false, false, true, true, false);
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "總共匯入 " + Global.TotalList[0] + " 筆設定資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
            #endif
        }

        #if DEBUG
        private void Debug_ConfigDataImportTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            List<string> Addlist = new List<string>();

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
            {
                string TruncateSqlStr = "delete * from ktv_SongMgr";
                using (OleDbCommand cmd = new OleDbCommand(TruncateSqlStr, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\ConfigData.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        Addlist.Add(sr.ReadLine());
                    }
                }

                string sqlColumnStr = "Config_Id, Config_Type, Config_Value";
                string sqlValuesStr = "@ConfigId, @ConfigType, @ConfigValue";
                string InsertSqlStr = "insert into ktv_SongMgr ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";

                using (OleDbCommand cmd = new OleDbCommand(InsertSqlStr, conn))
                {
                    List<string> list = new List<string>();

                    foreach (string AddStr in Addlist)
                    {
                        list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));

                        cmd.Parameters.AddWithValue("@ConfigId", Addlist.IndexOf(AddStr)+1);
                        cmd.Parameters.AddWithValue("@ConfigType", list[0]);
                        cmd.Parameters.AddWithValue("@ConfigValue", list[1]);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        Global.TotalList[0]++;
                        list.Clear();

                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "正在匯入第 " + Global.TotalList[0] + " 筆設定資料,請稍待...";
                        });
                    }
                }
            }
            Addlist.Clear();
        }
        #endif

        #endregion

    }


    class Debug
    {
        public static string ConfigDataType = "SpecialStr";

        public static DataTable GetEditSongLangList(bool MultiEdit)
        {
            #if DEBUG
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                if (MultiEdit)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = "不變更";
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }

                foreach (string str in Global.CashboxSongLangList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
            #else
                return null;
            #endif
        }

        public static DataTable GetConfigDataList()
        {
            #if DEBUG
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> Itemlist = new List<string>() { "SpecialStr", "SingerGroup" };

                foreach (string str in Itemlist)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
            #else
                return null;
            #endif
        }


    }
}
