using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
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
                    case "1":
                        SongDBConverter_SrcDBFile_TextBox.Text = "";
                        SongDBConverter_StartConv_Button.Enabled = false;
                        SongQuerySqlStr = "select Song_Id, Song_Lang, Song_Singer, Song_SongName, Song_SongType from ktv_Song";

                        list = CommonFunc.GetOleDbTableList(opd.FileName, "");

                        if (list.IndexOf("ktv_Song") >= 0)
                        {
                            if (SongDBConverter_Tooltip_Label.Text == "你選取的來源資料庫不是 CrazyKTV 資料庫!") SongDBConverter_Tooltip_Label.Text = "";
                        }
                        else
                        {
                            SongDBConverter_Tooltip_Label.Text = "你選取的來源資料庫不是 CrazyKTV 資料庫!";
                        }

                        if (SongDBConverter_Tooltip_Label.Text != "你選取的來源資料庫不是 CrazyKTV 資料庫!")
                        {
                            dt = CommonFunc.GetOleDbDataTable(opd.FileName, SongQuerySqlStr, "");
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

                string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_Singer, Song_SongName, Song_SongType from ktv_Song";
                List<string> list = new List<string>();
                list = CommonFunc.GetOleDbTableList(opd.FileName, "");

                if (list.IndexOf("ktv_AllSinger") >= 0)
                {
                    if (SongDBConverter_Tooltip_Label.Text == "你選取的目的資料庫不是新版的 CrazyKTV 資料庫!") SongDBConverter_Tooltip_Label.Text = "";
                }
                else
                {
                    SongDBConverter_Tooltip_Label.Text = "你選取的目的資料庫不是新版的 CrazyKTV 資料庫!";
                }
                    
                if (SongDBConverter_Tooltip_Label.Text != "你選取的目的資料庫不是新版的 CrazyKTV 資料庫!")
                {
                    DataTable dt = new DataTable();
                    dt = CommonFunc.GetOleDbDataTable(opd.FileName, SongQuerySqlStr, "");

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

                    dt.Dispose();
                    dt = null;
                }
            }
        }

        private void SongDBConverter_StartConv_Button_Click(object sender, EventArgs e)
        {
            Global.TimerStartTime = DateTime.Now;
            switch (SongDBConverter_SrcDBType_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    SongDBConverter_StartConv_Button.Enabled = false;
                    SongDBConverter_Converter_GroupBox.Enabled = false;
                    Common_SwitchSetUI(false);
                    Task.Factory.StartNew(SongDBConverter_ConvFromSrcDBTask, 1);
                    break;
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
                    case 1:
                        SongDBConverterSongDB.StartConvFromOldDB(i);
                        break;
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
            OleDbCommand allsingercmd = new OleDbCommand();
            sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
            sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
            string SingerAddSqlStr = "insert into ktv_Singer ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            string AllSingerAddSqlStr = "insert into ktv_AllSinger ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            singercmd = new OleDbCommand(SingerAddSqlStr, conn);
            allsingercmd = new OleDbCommand(AllSingerAddSqlStr, conn);

            List<string> NotExistsSingerId = new List<string>();
            NotExistsSingerId = CommonFunc.GetNotExistsSingerId("ktv_Singer", SongDestDBFile);
            List<string> NotExistsAllSingerId = new List<string>();
            NotExistsAllSingerId = CommonFunc.GetNotExistsSingerId("ktv_AllSinger", SongDestDBFile);
            int MaxSingerId = CommonFunc.GetMaxSingerId("ktv_Singer", SongDestDBFile) + 1;
            int MaxAllSingerId = CommonFunc.GetMaxSingerId("ktv_AllSinger", SongDestDBFile) + 1;
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
                    var query = from row in Global.AllSingerDT.AsEnumerable()
                                where row.Field<string>("Singer_Name").ToLower().Equals(dt.Rows[i].Field<string>("Song_Singer").ToLower())
                                select row;

                    if (query.Count<DataRow>() < 1)
                    {
                        if (NotExistsAllSingerId.Count > 0)
                        {
                            NextSingerId = NotExistsAllSingerId[0];
                            NotExistsAllSingerId.RemoveAt(0);
                        }
                        else
                        {
                            NextSingerId = MaxAllSingerId.ToString();
                            MaxAllSingerId++;
                        }

                        valuelist = new List<string>();
                        valuelist.Add(dt.Rows[i].Field<string>("Song_Singer"));
                        valuelist.Add(dt.Rows[i].Field<Int16>("Song_SingerType").ToString());
                        spelllist = new List<string>();
                        spelllist = CommonFunc.GetSongNameSpell(valuelist[0]);

                        allsingercmd.Parameters.AddWithValue("@SingerId", NextSingerId);
                        allsingercmd.Parameters.AddWithValue("@SingerName", valuelist[0]);
                        allsingercmd.Parameters.AddWithValue("@SingerType", valuelist[1]);
                        allsingercmd.Parameters.AddWithValue("@SingerSpell", spelllist[0]);
                        allsingercmd.Parameters.AddWithValue("@SingerStrokes", spelllist[2]);
                        allsingercmd.Parameters.AddWithValue("@SingerSpellNum", spelllist[1]);
                        allsingercmd.Parameters.AddWithValue("@SingerPenStyle", spelllist[3]);

                        try
                        {
                            allsingercmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】加入歌手至 ktv_AllSinger 時發生錯誤: " + valuelist[0];
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        }
                        allsingercmd.Parameters.Clear();
                    }

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

            
            if ((int)DBType == 1)
            {
                // 轉換我的最愛
                this.BeginInvoke((Action)delegate()
                {
                    SongDBConverter_Tooltip_Label.Text = "正在轉換我的最愛,請稍待...";
                });

                List<string> list = new List<string>();
                List<string> Addlist = new List<string>();

                OleDbCommand Ucmd = new OleDbCommand();
                OleDbCommand Fcmd = new OleDbCommand();
                OleDbCommand Rcmd = new OleDbCommand();

                string TruncateSqlStr = "";

                TruncateSqlStr = "delete * from ktv_User";
                Ucmd = new OleDbCommand(TruncateSqlStr, conn);
                Ucmd.ExecuteNonQuery();

                TruncateSqlStr = "delete * from ktv_Favorite";
                Fcmd = new OleDbCommand(TruncateSqlStr, conn);
                Fcmd.ExecuteNonQuery();
                
                string SongQuerySqlStr = "select User_Id, User_Name from ktv_User";
                dt = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        Addlist.Add("ktv_User," + row["User_Id"].ToString() + "," + row["User_Name"].ToString());
                    }
                }

                SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
                dt = CommonFunc.GetOleDbDataTable(SongSrcDBFile, SongQuerySqlStr, "");
                
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        Addlist.Add("ktv_Favorite," + row["User_Id"].ToString() + "," + row["Song_Id"].ToString());
                    }
                }

                string UserColumnStr = "User_Id, User_Name";
                string UserValuesStr = "@UserId, @UserName";
                string UserAddSqlStr = "insert into ktv_User ( " + UserColumnStr + " ) values ( " + UserValuesStr + " )";
                Ucmd = new OleDbCommand(UserAddSqlStr, conn);

                string FavoriteColumnStr = "User_Id, Song_Id";
                string FavoriteValuesStr = "@UserId, @SongId";
                string FavoriteAddSqlStr = "insert into ktv_Favorite ( " + FavoriteColumnStr + " ) values ( " + FavoriteValuesStr + " )";
                Fcmd = new OleDbCommand(FavoriteAddSqlStr, conn);

                foreach(string AddStr in Addlist)
                {
                    list = new List<string>(Regex.Split(AddStr, ",", RegexOptions.None));
                    switch (list[0])
                    {
                        case "ktv_User":
                            Ucmd.Parameters.AddWithValue("@UserId", list[1]);
                            Ucmd.Parameters.AddWithValue("@UserName", list[2]);
                            try
                            {
                                Ucmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】轉換最愛用戶 ktv_User 時發生錯誤: " + AddStr;
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                            Ucmd.Parameters.Clear();
                            break;
                        case "ktv_Favorite":
                            Fcmd.Parameters.AddWithValue("@UserId", list[1]);
                            Fcmd.Parameters.AddWithValue("@SongId", list[2]);
                            try
                            {
                                Fcmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】轉換最愛歌曲 ktv_Favorite 時發生錯誤: " + AddStr;
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                            Fcmd.Parameters.Clear();
                            break;
                    }
                    this.BeginInvoke((Action)delegate()
                    {
                        SongDBConverter_Tooltip_Label.Text = "正在轉換第 " + Addlist.IndexOf(AddStr) + " 筆我的最愛資料,請稍待...";
                    });
                }

                // 轉換遙控器設定
                this.BeginInvoke((Action)delegate()
                {
                    SongDBConverter_Tooltip_Label.Text = "正在轉換遙控器設定,請稍待...";
                });

                list = new List<string>();
                Addlist = new List<string>();

                string RemoteQuerySqlStr = "select Remote_Id, Remote_Subject, Remote_Controler, Remote_Controler2, Remote_Name from ktv_Remote";
                dt = CommonFunc.GetOleDbDataTable(SongSrcDBFile, RemoteQuerySqlStr, "");

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        Addlist.Add(row["Remote_Id"].ToString() + "," + row["Remote_Subject"].ToString() + "," + row["Remote_Controler"].ToString() + "," + row["Remote_Controler2"].ToString() + "," + row["Remote_Name"].ToString());
                    }
                }

                string RemoteColumnStr = "Remote_Id = @RemoteId, Remote_Subject = @RemoteSubject, Remote_Controler = @RemoteControler, Remote_Controler2 = @RemoteControler2, Remote_Name = @RemoteName";
                string RemoteUpdateSqlStr = "update ktv_Remote set " + RemoteColumnStr + " where Remote_Id = @OldRemoteId";
                Rcmd = new OleDbCommand(RemoteUpdateSqlStr, conn);

                foreach (string AddStr in Addlist)
                {
                    list = new List<string>(AddStr.Split(','));

                    Rcmd.Parameters.AddWithValue("@RemoteId", list[0]);
                    Rcmd.Parameters.AddWithValue("@RemoteSubject", list[1]);
                    Rcmd.Parameters.AddWithValue("@RemoteControler", list[2]);
                    Rcmd.Parameters.AddWithValue("@RemoteControler2", list[3]);
                    Rcmd.Parameters.AddWithValue("@RemoteName", list[4]);
                    Rcmd.Parameters.AddWithValue("@OldRemoteId", list[0]);

                    try
                    {
                        Rcmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫轉換】轉換遙控器設定 ktv_Remote 時發生錯誤: " + AddStr;
                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    }
                    Rcmd.Parameters.Clear();

                    this.BeginInvoke((Action)delegate()
                    {
                        SongDBConverter_Tooltip_Label.Text = "正在轉換第 " + Addlist.IndexOf(AddStr) + " 筆遙控器設定資料,請稍待...";
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
            list.Rows[0][0] = "舊版 CrazyKTV 資料庫";
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
