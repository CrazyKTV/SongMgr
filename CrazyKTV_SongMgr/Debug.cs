using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void Debug_CreateTestFile_Button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Global.CrazyktvDatabaseFile))
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
                        Debug_Tooltip_Label.Text = "總共建立 " + Global.TotalList[0] + " 筆檔案,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                });
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
                        if (!Directory.Exists(row.Field<string>("Song_Path"))) Directory.CreateDirectory(row.Field<string>("Song_Path"));
                        FileStream fs = new FileStream(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName")), FileMode.Create);
                        fs.Close();
                        lock (LockThis)
                        {
                            Global.TotalList[0]++;
                        }
                    }
                }
            });
            dt.Dispose();
            dt = null;


        }















    }





    class Debug
    {
    }
}
