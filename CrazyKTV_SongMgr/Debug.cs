using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {


        #region --- Debug 建立測試歌庫 ---

        private void Debug_CreateTestFile_Button_Click(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("請先變更資料庫裡的歌曲路徑!", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在建立測試歌庫,請稍待...";
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => Debug_CreateTestFileTask()));

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
        }

        private void Debug_CreateTestFileTask()
        {
            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select Song_Lang, Song_FileName, Song_Path from ktv_Song order by Song_Id";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

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

        #endregion

        #region --- Debug 建立錢櫃資料 ---

        private void Debug_CreateCashboxTable_Button_Click(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus)
            {
                if (MessageBox.Show("你確定要建立錢櫃資料嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Global.TimerStartTime = DateTime.Now;
                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                    Common_SwitchSetUI(false);

                    Debug_Tooltip_Label.Text = "正在建立錢櫃資料,請稍待...";
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => Debug_CreateCashboxTableTask()));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        Global.TimerEndTime = DateTime.Now;
                        this.BeginInvoke((Action)delegate ()
                        {
                            Debug_Tooltip_Label.Text = "總共建立 " + Global.TotalList[0] + " / " + Global.TotalList[1] + " 筆資料,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }
        }

        private void Debug_CreateCashboxTableTask()
        {
            OleDbConnection clearconn = new OleDbConnection();
            clearconn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, "");
            //OleDbCommand clearcmd = new OleDbCommand("create table ktv_Cashbox (Cashbox_Id TEXT(20) NOT NULL PRIMARY KEY, Song_Lang TEXT(60) WITH COMPRESSION, Song_SongName TEXT(80) WITH COMPRESSION, Song_Singer TEXT(60) WITH COMPRESSION)", clearconn);
            OleDbCommand clearcmd = new OleDbCommand("delete * from ktv_Cashbox", clearconn);
            clearcmd.ExecuteNonQuery();
            clearconn.Close();

            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = hw.Load("http://www.cashboxparty.com/mysong/mysong_search_r.asp?Page=1");

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[2]");
            HtmlNodeCollection child = table.SelectNodes("tr[2]");

            foreach (var RemoveSelect in child.Descendants("select").ToArray())
            {
                RemoveSelect.Remove();
            }

            string PageCountStr = "";
            foreach (HtmlNode childnode in child)
            {
                PageCountStr = childnode.InnerText;
            }

            int ItemCount = 0;
            int PageCount = 0;

            MatchCollection PageCountMatches = Regex.Matches(PageCountStr, @"\d+");
            if (PageCountMatches.Count == 2)
            {
                ItemCount = Convert.ToInt32(PageCountMatches[0].Value);
                PageCount = Convert.ToInt32(PageCountMatches[1].Value);
                Global.TotalList[1] = ItemCount;
            }

            List<string> SongIdList = new List<string>();
            List<string> SongDataList = new List<string>();

            for (int i=0; i < PageCount; i++)
            {
                doc = hw.Load("http://www.cashboxparty.com/mysong/mysong_search_r.asp?Page=" + (i +1));
                table = doc.DocumentNode.SelectSingleNode("//table[3]");
                child = table.SelectNodes("tr");

                this.BeginInvoke((Action)delegate()
                {
                    Debug_Tooltip_Label.Text = "正在分析第 " + (i + 1) + " / " + PageCount + " 頁資料,請稍待...";
                });

                foreach (HtmlNode childnode in child)
                {
                    List<string> list = new List<string>();
                    HtmlNodeCollection td = childnode.SelectNodes("td");
                    foreach (HtmlNode tdnode in td)
                    {
                        string data = Regex.Replace(tdnode.InnerText, @"^\s*|\s*$", ""); //去除頭尾空白
                        if (list.Count < 4)
                        {
                            list.Add(data);
                        }
                    }

                    if (CommonFunc.IsSongId(list[0]))
                    {
                        if (SongIdList.IndexOf(list[0]) < 0)
                        {
                            SongIdList.Add(list[0]);
                            if (list[1] == "") list[1] = "其它";
                            list[3] = Regex.Replace(list[3], "、", "&");
                            SongDataList.Add(string.Join("|", list));
                        }
                    }
                    list.Clear();
                }
            }

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
            {
                OleDbCommand cmd = new OleDbCommand();
                string sqlColumnStr = "Cashbox_Id, Song_Lang, Song_SongName, Song_Singer";
                string sqlValuesStr = "@CashboxId, @SongLang, @SongSongName, @SongSinger";
                string AddSqlStr = "insert into ktv_Cashbox ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
                cmd = new OleDbCommand(AddSqlStr, conn);

                foreach (string SongData in SongDataList)
                {
                    List<string> valuelist = new List<string>(SongData.Split('|'));

                    cmd.Parameters.AddWithValue("@CashboxId", valuelist[0]);
                    cmd.Parameters.AddWithValue("@SongLang", valuelist[1]);
                    cmd.Parameters.AddWithValue("@SongSongName", valuelist[2]);
                    cmd.Parameters.AddWithValue("@SongSinger", valuelist[3]);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        Global.TotalList[0]++;
                        this.BeginInvoke((Action)delegate()
                        {
                            Debug_Tooltip_Label.Text = "正在將第 " + Global.TotalList[0] + " 首歌曲寫入資料庫,請稍待...";
                        });
                    }
                    catch
                    {
                        Global.TotalList[2]++;
                        Global.FailureSongDT.Rows.Add(Global.FailureSongDT.NewRow());
                        Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][0] = "加入歌曲時發生未知的錯誤: " + SongData;
                        Global.FailureSongDT.Rows[Global.FailureSongDT.Rows.Count - 1][1] = Global.FailureSongDT.Rows.Count;
                    }
                    cmd.Parameters.Clear();
                }
            }
            CommonFunc.CompactAccessDB("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Global.CrazyktvSongMgrDatabaseFile + ";", Global.CrazyktvSongMgrDatabaseFile);
        }

        #endregion








    }





    class Debug
    {
    }
}
