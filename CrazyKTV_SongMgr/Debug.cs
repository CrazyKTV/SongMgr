using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
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
                            Debug_Tooltip_Label.Text = "總共建立 " + Global.TotalList[0] + " 筆檔案,忽略 " + Global.TotalList[1] + " 筆,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
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
                            Debug_Tooltip_Label.Text = "正在建立 " + Global.TotalList[2] + " 筆檔案...";
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
                        this.BeginInvoke((Action)delegate ()
                        {
                            Debug_Tooltip_Label.Text = "總共建立 " + Global.TotalList[0] + " 筆檔案,忽略 " + Global.TotalList[1] + " 筆,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                            Common_SwitchSetUI(true);
                        });
                    });
                }
            }

            /*
            OleDbConnection conn = new OleDbConnection();
            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand("create table ktv_Cashbox (Cashbox_Id TEXT(20) NOT NULL PRIMARY KEY, Song_Lang TEXT(60) WITH COMPRESSION, Song_SongName TEXT(80) WITH COMPRESSION, Song_Singer TEXT(60) WITH COMPRESSION)", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            */
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("id", typeof(string)));
            dt.Columns.Add(new DataColumn("lang", typeof(string)));
            dt.Columns.Add(new DataColumn("song", typeof(string)));
            dt.Columns.Add(new DataColumn("singer", typeof(string)));
            dt.Columns.Add(new DataColumn("data", typeof(string)));

            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = hw.Load("http://www.cashboxparty.com/mysong/mysong_search_r.asp?Page=" + 1);
            // string strResult = "";

            //HtmlNode main = doc.GetElementbyId("form1");
            //HtmlNode table = main.SelectSingleNode("//table[3]");
            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[3]");
            HtmlNodeCollection child = table.SelectNodes("tr");

            foreach (HtmlNode row in child)
            {
                HtmlNodeCollection td = row.SelectNodes("td");
                foreach (HtmlNode row1 in td)
                {
                    Console.Write(row1.InnerText);
                }


            }


        }

        private void Debug_CreateCashboxTableTask()
        {

        }

        #endregion








    }





    class Debug
    {
    }
}
