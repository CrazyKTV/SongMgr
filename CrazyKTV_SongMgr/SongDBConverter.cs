using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void SongDBConverter_SrcDBFile_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.InitialDirectory = Application.StartupPath;
            opd.Filter = "資料庫檔案 (*.mdb)|*.mdb";
            opd.FilterIndex = 1;
            
            if (opd.ShowDialog() == DialogResult.OK && opd.FileName.Length > 0)
            {
                List<string> list = new List<string>();
                string SongQuerySqlStr = "";
                DataTable dt = new DataTable();
                
                switch (SongDBConverter_SrcDBType_ComboBox.SelectedValue.ToString())
                {
                    case "2":
                        SongDBConverter_SrcDBFile_TextBox.Text = "";
                        SongDBConverter_StartConv_Button.Enabled = false;
                        SongQuerySqlStr = "select Song_ID, Song_Type, Song_Singer, Song_SingerList, Song_Title from Tbl_Song";

                        list = CommonFunc.GetOleDbTableList(opd.FileName, "tmwcmgumbonqd");

                        if (list.IndexOf("Tbl_Song") >= 0)
                        {
                            if (SongDBConverter_Tooltip_Label.Text == "你選取的來源資料庫不是 JetKTV 資料庫!") SongDBConverter_Tooltip_Label.Text = "";
                        }
                        else
                        {
                            SongDBConverter_Tooltip_Label.Text = "你選取的來源資料庫不是 JetKTV 資料庫!";
                        }

                        if (SongDBConverter_Tooltip_Label.Text != "你選取的來源資料庫不是 JetKTV 資料庫!")
                        {
                            dt = CommonFunc.GetOleDbDataTable(opd.FileName, SongQuerySqlStr, "tmwcmgumbonqd");
                            if (dt.Rows.Count > 0)
                            {
                                if (SongDBConverter_Tooltip_Label.Text == "你選取的來源資料庫沒有歌曲資料!") SongDBConverter_Tooltip_Label.Text = "";
                                SongDBConverter_SrcDBFile_TextBox.Text = opd.FileName;
                                SongDBConverter_SwitchButton(1);
                            }
                            else
                            {
                                SongDBConverter_Tooltip_Label.Text = "你選取的來源資料庫沒有歌曲資料!";
                            }
                        }
                        dt.Dispose();
                        dt = null;
                        break;
                }
            }
        }

        private void SongDBConverter_DestDBFile_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.InitialDirectory = Application.StartupPath;
            opd.Filter = "資料庫檔案 (*.mdb)|*.mdb";
            opd.FilterIndex = 1;

            if (opd.ShowDialog() == DialogResult.OK && opd.FileName.Length > 0)
            {
                SongDBConverter_DestDBFile_TextBox.Text = "";
                SongDBConverter_StartConv_Button.Enabled = false;

                bool CrazyktvDatabaseError = false;
                List<string> TableList = new List<string>() { "ktv_Favorite", "ktv_Langauage", "ktv_Phonetics", "ktv_Remote", "ktv_Singer", "ktv_SingerName", "ktv_Song", "ktv_Swan", "ktv_User", "ktv_Words" };
                List<string> CrazyktvDBTableList = new List<string>(CommonFunc.GetOleDbTableList(opd.FileName, ""));

                foreach (string TableName in TableList)
                {
                    if (CrazyktvDBTableList.IndexOf(TableName) < 0) CrazyktvDatabaseError = true;
                }

                if (!CrazyktvDatabaseError)
                {
                    if (SongDBConverter_Tooltip_Label.Text == "你選取的目的資料庫不是 CrazyKTV 資料庫!") SongDBConverter_Tooltip_Label.Text = "";
                }
                else
                {
                    SongDBConverter_Tooltip_Label.Text = "你選取的目的資料庫不是 CrazyKTV 資料庫!";
                }
                    
                if (SongDBConverter_Tooltip_Label.Text != "你選取的目的資料庫不是 CrazyKTV 資料庫!")
                {
                    string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_Singer, Song_SongName, Song_SongType from ktv_Song";
                    using (DataTable dt = CommonFunc.GetOleDbDataTable(opd.FileName, SongQuerySqlStr, ""))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            SongDBConverter_Tooltip_Label.Text = "你選取的目的資料庫不是空白的 CrazyKTV 資料庫!";
                        }
                        else
                        {
                            if (SongDBConverter_Tooltip_Label.Text == "你選取的目的資料庫不是空白的 CrazyKTV 資料庫!") SongDBConverter_Tooltip_Label.Text = "";
                            SongDBConverter_DestDBFile_TextBox.Text = opd.FileName;
                            SongDBConverter_SwitchButton(1);
                        }
                    }
                }
            }
        }

        private void SongDBConverter_StartConv_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            switch (SongDBConverter_SrcDBType_ComboBox.SelectedValue.ToString())
            {
                case "2":
                    SongDBConverter_StartConv_Button.Enabled = false;
                    SongDBConverter_Converter_GroupBox.Enabled = false;
                    SongDBConverter_JetktvPathCfg_GroupBox.Enabled = false;
                    SongDBConverter_JetktvLangCfg_GroupBox.Enabled = false;
                    Common_SwitchSetUI(false);
                    Task.Factory.StartNew(SongDBConverter_ConvFromSrcDBTask, 2);
                    break;
            }
        }

        private void SongDBConverter_SetRtfText(string titlestr, string textstr)
        {
            SongDBConverter_ConvHelp_RichTextBox.SelectionColor = Color.Blue;
            SongDBConverter_ConvHelp_RichTextBox.AppendText(titlestr);

            SongDBConverter_ConvHelp_RichTextBox.SelectionColor = Color.Black;
            SongDBConverter_ConvHelp_RichTextBox.AppendText(textstr);
        }

        private void SongDBConverter_SwitchButton(int button)
        {
            switch (button)
            {
                case 1:
                    if (SongDBConverter_SrcDBFile_TextBox.Text.Length > 0 & SongDBConverter_DestDBFile_TextBox.Text.Length > 0)
                    {
                        SongDBConverter_StartConv_Button.Enabled = true;
                    }
                    else { SongDBConverter_StartConv_Button.Enabled = false; }
                    break;
            }
        }

        private void SongDBConverter_SrcDBType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SongDBConverter_Tooltip_Label.Text = "";
            switch (SongDBConverter_SrcDBType_ComboBox.SelectedValue.ToString())
            {
                case "2":
                    SongDBConverter_ConvHelp_GroupBox.Visible = false;
                    SongDBConverter_JetktvPathCfg_GroupBox.Visible = true;
                    SongDBConverter_JetktvLangCfg_GroupBox.Visible = true;
                    SongDBConverter_ConvHelp_GroupBox.Width = 660;
                    SongDBConverter_ConvHelp_RichTextBox.Width = 628;
                    SongDBConverter_SrcDBFile_Button.Enabled = true;

                    ComboBox[] SongDBConverter_JetktvLang_ComboBox = 
                    {
                        SongDBConverter_JetktvLang1_ComboBox,
                        SongDBConverter_JetktvLang2_ComboBox,
                        SongDBConverter_JetktvLang3_ComboBox,
                        SongDBConverter_JetktvLang4_ComboBox,
                        SongDBConverter_JetktvLang5_ComboBox,
                        SongDBConverter_JetktvLang6_ComboBox,
                        SongDBConverter_JetktvLang7_ComboBox,
                        SongDBConverter_JetktvLang8_ComboBox,
                        SongDBConverter_JetktvLang9_ComboBox
                    };

                    int cboxvalue = 0;
                    List<int> list = new List<int>();
                    list = new List<int>() { 1, 2, 3, 6, 5, 4, 10, 9, 10 };

                    foreach (ComboBox cbox in SongDBConverter_JetktvLang_ComboBox)
                    {
                        cbox.DataSource = SongDBConverter.GetSongLangList();
                        cbox.DisplayMember = "Display";
                        cbox.ValueMember = "Value";
                        cbox.SelectedValue = list[cboxvalue];
                        cboxvalue++;
                    }
                    break;
                default:
                    SongDBConverter_JetktvPathCfg_GroupBox.Visible = false;
                    SongDBConverter_JetktvLangCfg_GroupBox.Visible = false;
                    SongDBConverter_ConvHelp_GroupBox.Visible = true;
                    SongDBConverter_ConvHelp_GroupBox.Width = 952;
                    SongDBConverter_ConvHelp_RichTextBox.Width = 920;
                    SongDBConverter_SrcDBFile_Button.Enabled = false;
                    break;
            }
        }


        private void SongDBConverter_JetktvLang_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((ComboBox)sender).SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "10":
                    Global.SongDBConvJetktvLangList = new List<string>();
                    Global.SongDBConvJetktvLangList.Add("未使用");
                    ComboBox[] SongDBConverter_JetktvLang_ComboBox = 
                    {
                        SongDBConverter_JetktvLang1_ComboBox,
                        SongDBConverter_JetktvLang2_ComboBox,
                        SongDBConverter_JetktvLang3_ComboBox,
                        SongDBConverter_JetktvLang4_ComboBox,
                        SongDBConverter_JetktvLang5_ComboBox,
                        SongDBConverter_JetktvLang6_ComboBox,
                        SongDBConverter_JetktvLang7_ComboBox,
                        SongDBConverter_JetktvLang8_ComboBox,
                        SongDBConverter_JetktvLang9_ComboBox
                    };

                    foreach (ComboBox cbox in SongDBConverter_JetktvLang_ComboBox)
                    {
                        int value = Convert.ToInt32(cbox.SelectedValue) - 1;
                        string LangStr = CommonFunc.GetSongLangStr(value, 0, "null");
                        Global.SongDBConvJetktvLangList.Add(LangStr);
                    }
                    break;
            }

        }

        private void SongDBConverter_JetktvPathCfg_Button_Click(object sender, EventArgs e)
        {
            switch (SongDBConverter_JetktvPathCfg_Button.Text)
            {
                case "瀏覽":
                    FolderBrowserDialog opd = new FolderBrowserDialog();
                    if (SongDBConverter_JetktvPathCfg_TextBox.Text != "") opd.SelectedPath = SongDBConverter_JetktvPathCfg_TextBox.Text;

                    if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
                    {
                        SongDBConverter_JetktvPathCfg_TextBox.Text = opd.SelectedPath;
                        SongDBConverter_JetktvPathCfg_Button.Text = "加入";
                    }
                    break;
                case "加入":
                    DataTable dt = (DataTable)SongDBConverter_JetktvPathCfg_ListBox.DataSource;
                    if (dt == null)
                    {
                        dt = new DataTable();
                        dt.Columns.Add(new DataColumn("Display", typeof(string)));
                        dt.Columns.Add(new DataColumn("Value", typeof(int)));
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = SongDBConverter_JetktvPathCfg_TextBox.Text;
                        dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                    }
                    else
                    {
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = SongDBConverter_JetktvPathCfg_TextBox.Text;
                        dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                    }
                    SongDBConverter_JetktvPathCfg_TextBox.Text = "";
                    SongDBConverter_JetktvPathCfg_Button.Text = "瀏覽";

                    SongDBConverter_JetktvPathCfg_ListBox.DataSource = dt;
                    SongDBConverter_JetktvPathCfg_ListBox.DisplayMember = "Display";
                    SongDBConverter_JetktvPathCfg_ListBox.ValueMember = "Value";

                    Global.SongDBConvJetktvPathList = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (row[column] != null)
                            {
                                if (column.ColumnName == "Display")
                                {
                                    Global.SongDBConvJetktvPathList.Add(row[column].ToString());
                                }
                            }
                        }
                    }
                    break;
            }
        }

        // 轉換至 CrazyKTV 歌庫
        private void SongDBConverter_ConvFromSrcDBTask(object DBType)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Global.SongDBConvValueList = new List<string>();
            Global.TotalList = new List<int>() { 0, 0, 0, 0 };
            string SongSrcDBFile = SongDBConverter_SrcDBFile_TextBox.Text;
            string SongDestDBFile = SongDBConverter_DestDBFile_TextBox.Text;

            SongDBConverterSongDB.CreateSongDataTable((int)DBType, SongSrcDBFile, SongDestDBFile);

            int count = Global.SongSrcDT.Rows.Count;

            for (int i = 0; i < count; i++)
            //Parallel.For(0, count, (i, loopState) =>
            {
                switch ((int)DBType)
                {
                    case 2:
                        SongDBConverterSongDB.StartConvFromJetktvDB(i);
                        break;
                }
                
                this.BeginInvoke((Action)delegate()
                {
                    SongDBConverter_Tooltip_Label.Text = "已成功解析 " + Global.TotalList[0] + " 首歌曲...";
                });
            }//);

            // 加入歌曲
            OleDbConnection conn = CommonFunc.OleDbOpenConn(SongDestDBFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState";
            string sqlValuesStr = "@SongId, @SongLang, @SongSingerType, @SongSinger, @SongSongName, @SongTrack, @SongSongType, @SongVolume, @SongWordCount, @SongPlayCount, @SongMB, @SongCreatDate, @SongFileName, @SongPath, @SongSpell, @SongSpellNum, @SongSongStroke, @SongPenStyle, @SongPlayState";
            string SongDBConverterSqlStr = "insert into ktv_Song ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(SongDBConverterSqlStr, conn);
            List<string> valuelist = new List<string>();

            foreach (string str in Global.SongDBConvValueList)
            {
                valuelist = new List<string>(str.Split('|'));

                cmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                cmd.Parameters.AddWithValue("@SongLang", valuelist[1]);
                cmd.Parameters.AddWithValue("@SongSingerType", valuelist[2]);
                cmd.Parameters.AddWithValue("@SongSinger", valuelist[3]);
                cmd.Parameters.AddWithValue("@SongSongName", valuelist[4]);
                cmd.Parameters.AddWithValue("@SongTrack", valuelist[5]);
                cmd.Parameters.AddWithValue("@SongSongType", valuelist[6]);
                cmd.Parameters.AddWithValue("@SongVolume", valuelist[7]);
                cmd.Parameters.AddWithValue("@SongWordCount", valuelist[8]);
                cmd.Parameters.AddWithValue("@SongPlayCount", valuelist[9]);
                cmd.Parameters.AddWithValue("@SongMB", valuelist[10]);
                cmd.Parameters.AddWithValue("@SongCreatDate", valuelist[11]);
                cmd.Parameters.AddWithValue("@SongFileName", valuelist[12]);
                cmd.Parameters.AddWithValue("@SongPath", valuelist[13]);
                cmd.Parameters.AddWithValue("@SongSpell", valuelist[14]);
                cmd.Parameters.AddWithValue("@SongSpellNum", valuelist[15]);
                cmd.Parameters.AddWithValue("@SongSongStroke", valuelist[16]);
                cmd.Parameters.AddWithValue("@SongPenStyle", valuelist[17]);
                cmd.Parameters.AddWithValue("@SongPlayState", valuelist[18]);

                try
                {
                    cmd.ExecuteNonQuery();
                    Global.TotalList[3]++;
                }
                catch
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】寫入至資料庫時發生錯誤: " + str;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    Global.TotalList[0]--;
                    Global.TotalList[2]++;
                }
                cmd.Parameters.Clear();

                this.BeginInvoke((Action)delegate()
                {
                    SongDBConverter_Tooltip_Label.Text = "正在將第 " + Global.TotalList[3] + " 首歌曲寫入資料庫,請稍待...";
                });

            }
            Global.SongDBConvValueList.Clear();

            // 加入歌手
            OleDbCommand singercmd = new OleDbCommand();
            sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
            string SingerAddSqlStr = "insert into ktv_Singer ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            singercmd = new OleDbCommand(SingerAddSqlStr, conn);

            List<string> NotExistsSingerId = new List<string>();
            NotExistsSingerId = CommonFunc.GetNotExistsSingerId("ktv_Singer", SongDestDBFile);
            int MaxSingerId = CommonFunc.GetMaxSingerId("ktv_Singer", SongDestDBFile) + 1;
            string NextSingerId = "";
            List<string> spelllist = new List<string>();

            DataTable dt = new DataTable();
            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, First(Song_SingerType) AS Song_SingerType, Count(Song_Singer) AS Song_SingerCount FROM ktv_Song GROUP BY Song_Singer HAVING (((First(Song_SingerType))<>3) AND ((Count(Song_Singer))>0)) ORDER BY First(Song_SingerType), First(Song_Singer)";
            dt = CommonFunc.GetOleDbDataTable(SongDestDBFile, SingerQuerySqlStr, "");
            count = dt.Rows.Count;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (NotExistsSingerId.Count > 0)
                    {
                        NextSingerId = NotExistsSingerId[0];
                        NotExistsSingerId.RemoveAt(0);
                    }
                    else
                    {
                        NextSingerId = MaxSingerId.ToString();
                        MaxSingerId++;
                    }

                    valuelist = new List<string>();
                    valuelist.Add(dt.Rows[i].Field<string>("Song_Singer"));
                    valuelist.Add(dt.Rows[i].Field<Int16>("Song_SingerType").ToString());
                    spelllist = new List<string>();
                    spelllist = CommonFunc.GetSongNameSpell(valuelist[0]);

                    singercmd.Parameters.AddWithValue("@SingerId", NextSingerId);
                    singercmd.Parameters.AddWithValue("@SingerName", valuelist[0]);
                    singercmd.Parameters.AddWithValue("@SingerType", valuelist[1]);
                    singercmd.Parameters.AddWithValue("@SingerSpell", spelllist[0]);
                    singercmd.Parameters.AddWithValue("@SingerStrokes", spelllist[2]);
                    singercmd.Parameters.AddWithValue("@SingerSpellNum", spelllist[1]);
                    singercmd.Parameters.AddWithValue("@SingerPenStyle", spelllist[3]);

                    try
                    {
                        singercmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】加入歌手至 ktv_Singer 時發生錯誤: " + valuelist[0];
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                    singercmd.Parameters.Clear();
                    
                    this.BeginInvoke((Action)delegate()
                    {
                        SongDBConverter_Tooltip_Label.Text = "已成功將 " + (i + 1) + " 位新歌手加入至資料庫,請稍待...";
                    });
                }
            }
            dt.Dispose();
            conn.Close();

            this.BeginInvoke((Action)delegate()
            {
                Global.TimerEndTime = DateTime.Now;
                SongDBConverter_Tooltip_Label.Text = "總共轉換 " + Global.TotalList[0] + " 首歌曲,忽略 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成轉換。";

                SongDBConverter_SrcDBFile_TextBox.Text = "";
                SongDBConverter_DestDBFile_TextBox.Text = "";
                SongDBConverter_SrcDBFile_Button.Enabled = true;
                SongDBConverter_SrcDBType_ComboBox.Enabled = true;
                SongDBConverter_DestDBFile_Button.Enabled = true;
                SongDBConverter_JetktvPathCfg_GroupBox.Enabled = true;
                SongDBConverter_JetktvLangCfg_GroupBox.Enabled = true;
                SongDBConverter_Converter_GroupBox.Enabled = true;

                Common_SwitchSetUI(true);
            });
            SongDBConverterSongDB.DisposeSongDataTable();
        }
    }


    class SongDBConverter
    {
        public static DataTable GetSrcDBTypeList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "請選擇所要轉換的資料庫類型";
            list.Rows[0][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = "JetKTV 資料庫";
            list.Rows[1][1] = 2;
            return list;
        }

        public static DataTable GetSongLangList()
        {
            List<string> list = new List<string>();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Display", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(int)));

            foreach (string s in Global.CrazyktvSongLangList)
            {
                dt.Rows.Add(dt.NewRow());
                dt.Rows[dt.Rows.Count - 1][0] = s;
                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
            }
            return dt;
        }

    }
}
