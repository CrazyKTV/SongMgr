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

        #region --- SongQuery 控制項事件 ---

        private void SongQuery_QueryType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongQuery_QueryType_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = true;
                    SongQuery_Clear_Button.Enabled = true;
                    SongQuery_QueryValue_TextBox.Focus();
                    break;
                case "2":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = true;
                    SongQuery_Clear_Button.Enabled = true;
                    SongQuery_QueryValue_TextBox.Focus();
                    break;
                case "3":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_QueryValue_TextBox.Focus();
                    break;
                case "4":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "*";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button.PerformClick();
                    SongQuery_DataGridView.Focus();
                    break;
                case "5":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "100";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button.PerformClick();
                    SongQuery_DataGridView.Focus();
                    break;
                case "6":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "*";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button.PerformClick();
                    SongQuery_DataGridView.Focus();
                    break;
                case "7":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    
                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongType", false, false);
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "8":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;

                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SingerType", false, false);
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "9":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;

                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", false, false);
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 0;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "10":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "*";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button.PerformClick();
                    SongQuery_DataGridView.Focus();
                    break;
            }
        }

        private void SongQuery_QueryFilter_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongQuery_QueryFilter_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                Global.SongQueryFilter = SongQuery_QueryFilter_ComboBox.Text;
            }
        }

        private void SongQuery_QueryValue_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (SongQuery_QueryType_ComboBox.SelectedValue.ToString())
            {
                case "3":
                case "5":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13)
                    {
                        e.Handled = true;
                    }
                    break;
            }
            if ((int)e.KeyChar == 13)
            {
                SongQuery_Query_Button_Click(new Button(), new EventArgs());
            }
        }

        private void SongQuery_Paste_Button_Click(object sender, EventArgs e)
        {
            SongQuery_QueryValue_TextBox.Text = Clipboard.GetText();
        }

        private void SongQuery_Clear_Button_Click(object sender, EventArgs e)
        {
            SongQuery_QueryValue_TextBox.Text = "";
        }

        private void SongQuery_SynonymousQuery_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongQuerySynonymousQuery = SongQuery_SynonymousQuery_CheckBox.Checked;
        }

        private void SongQuery_FuzzyQuery_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SongQuery_FuzzyQuery_CheckBox.Checked == true)
            {
                Global.SongQueryFuzzyQuery = "True";
            }
            else
            {
                Global.SongQueryFuzzyQuery = "False";
            }
        }

        private void SongQuery_CommonFilter_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Focused)
            {
                if (((ComboBox)sender).Name == "SongQuery_LangFilter_ComboBox")
                {
                    List<int> WordCountFilterList = new List<int>();
                    ((DataTable)SongQuery_DataGridView.DataSource).DefaultView.RowFilter = "";

                    using (DataTable dt = (DataTable)SongQuery_DataGridView.DataSource)
                    {
                        using (DataTable WordCountDistinctDT = dt.DefaultView.ToTable(true, "Song_Lang", "Song_WordCount"))
                        {
                            if (WordCountDistinctDT.Rows.Count > 0)
                            {
                                foreach (DataRow row in WordCountDistinctDT.AsEnumerable())
                                {
                                    if (row["Song_Lang"].ToString() == SongQuery_LangFilter_ComboBox.Text)
                                    {
                                        WordCountFilterList.Add(Convert.ToInt32(row["Song_WordCount"].ToString()));
                                    }
                                    else if (SongQuery_LangFilter_ComboBox.Text == "全部")
                                    {
                                        if (WordCountFilterList.IndexOf(Convert.ToInt32(row["Song_WordCount"].ToString())) <= 0)
                                        {
                                            WordCountFilterList.Add(Convert.ToInt32(row["Song_WordCount"].ToString()));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (WordCountFilterList.Count > 0)
                    {
                        WordCountFilterList.Sort();
                        SongQuery_WordCountFilter_ComboBox.Enabled = true;
                        SongQuery_WordCountFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(WordCountFilterList.ConvertAll(i => i.ToString()));
                        SongQuery_WordCountFilter_ComboBox.DisplayMember = "Display";
                        SongQuery_WordCountFilter_ComboBox.ValueMember = "Value";
                        WordCountFilterList.Clear();
                    }
                }

                string FilterStr = "";
                if (SongQuery_LangFilter_ComboBox.Text != "全部" && SongQuery_LangFilter_ComboBox.Text != "") FilterStr = "Song_Lang = '" + SongQuery_LangFilter_ComboBox.Text + "'";
                if (SongQuery_WordCountFilter_ComboBox.Text != "全部" && SongQuery_WordCountFilter_ComboBox.Text != "") FilterStr += (FilterStr != "") ? " and Song_WordCount = '" + SongQuery_WordCountFilter_ComboBox.Text + "'" : "Song_WordCount = '" + SongQuery_WordCountFilter_ComboBox.Text + "'";
                ((DataTable)SongQuery_DataGridView.DataSource).DefaultView.RowFilter = FilterStr;

                string SongFullPath = string.Empty;
                for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                {
                    SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                    SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                }
            }
        }

        #endregion

        #region --- SongQuery 查詢歌曲 ---

        private void SongQuery_Query_Button_Click(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus)
            {
                SongQuery_QueryType_ComboBox.SelectedIndexChanged -= new EventHandler(SongQuery_QueryType_ComboBox_SelectedIndexChanged);

                SongQuery.QuerySameFileSong = false;
                Global.SongQueryQueryType = "SongQuery";

                SongQuery_Query_Button.Enabled = false;
                Common_SwitchSetUI(false);

                SongQuery_DataGridView.DataSource = null;
                if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                SongQuery_QueryStatus_Label.Text = "";
                GC.Collect();

                string SongQueryType = "None";
                string SongQueryValue = "";
                string SongQueryStatusText = "";

                var tasks = new List<Task>();

                switch (SongQuery_QueryType_ComboBox.SelectedValue.ToString())
                {
                    case "1":
                        SongQueryType = "SongName";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = SongQuery_QueryValue_TextBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢歌曲名稱為『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "2":
                        SongQueryType = "SingerName";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = SongQuery_QueryValue_TextBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢歌手名稱為『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "3":
                        SongQueryType = "SongID";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "歌曲編號中包含 " + SongQuery_QueryValue_TextBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "4":
                        SongQueryType = "AllSong";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "全部歌曲";
                        SongQuery_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                    case "5":
                        SongQueryType = "NewSong";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "新進歌曲";
                        SongQuery_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                    case "6":
                        SongQueryType = "ChorusSong";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "合唱歌曲";
                        SongQuery_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                    case "7":
                        SongQueryType = "SongType";
                        SongQueryValue = SongQuery_QueryValue_ComboBox.Text;
                        SongQueryStatusText = "歌曲類別為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "8":
                        SongQueryType = "SingerType";
                        SongQueryValue = Global.CrazyktvSingerTypeList.IndexOf(SongQuery_QueryValue_ComboBox.Text).ToString();
                        SongQueryStatusText = "歌手類別為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "9":
                        SongQueryType = "SongTrack";
                        SongQueryValue = SongQuery_QueryValue_ComboBox.SelectedValue.ToString();
                        SongQueryStatusText = "歌曲聲道為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "10":
                        SongQueryType = "CashboxId";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "使用錢櫃編號";
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                }
                tasks.Add(Task.Factory.StartNew(() => SongQuery_QueryTask(SongQueryType, SongQueryValue, SongQueryStatusText)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryType_ComboBox.SelectedIndexChanged += new EventHandler(SongQuery_QueryType_ComboBox_SelectedIndexChanged);
                        Common_SwitchSetUI(true);
                        SongQuery_Query_Button.Enabled = true;
                    });
                });
            }
        }


        private void SongQuery_QueryTask(string SongQueryType, string SongQueryValue, string SongQueryStatusText)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                if (SongQueryValue == "")
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_EditMode_CheckBox.Enabled = false;
                        SongQuery_LangFilter_ComboBox.Enabled = false;
                        SongQuery_WordCountFilter_ComboBox.Enabled = false;
                        SongQuery_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                    });
                }
                else
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        switch (SongQueryType)
                        {
                            case "SongName":
                                if (Global.SongQuerySynonymousQuery)
                                {
                                    List<string> SynonymousSongNameList = new List<string>();
                                    SynonymousSongNameList = CommonFunc.GetSynonymousSongNameList(SongQueryValue);

                                    if (SynonymousSongNameList.Count > 0)
                                    {
                                        dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, SynonymousSongNameList), "");
                                    }
                                    else
                                    {
                                        dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                                    }
                                    SynonymousSongNameList.Clear();
                                    SynonymousSongNameList = null;
                                }
                                else
                                {
                                    dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                                }
                                break;
                            case "SingerName":
                                if (Global.GroupSingerLowCaseList.IndexOf(SongQueryValue.ToLower()) >= 0)
                                {
                                    List<string> GroupSingerList = new List<string>();

                                    int i = Global.GroupSingerIdList[Global.GroupSingerLowCaseList.IndexOf(SongQueryValue.ToLower())];
                                    List<string> list = new List<string>(Global.SingerGroupList[i].Split(','));
                                    foreach (string GroupSingerName in list)
                                    {
                                        if (GroupSingerName.ToLower() != SongQueryValue.ToLower())
                                        {
                                            GroupSingerList.Add(GroupSingerName);
                                        }
                                    }
                                    list.Clear();
                                    list = null;

                                    dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, GroupSingerList), "");
                                    GroupSingerList.Clear();
                                    GroupSingerList = null;
                                }
                                else
                                {
                                    dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                                }
                                break;
                            case "CashboxId":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                List<string> CashboxIdList = new List<string>();

                                string CashboxSqlStr = "select Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate from ktv_Cashbox order by Cashbox_Id";
                                using (DataTable CashboxDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, CashboxSqlStr, ""))
                                {
                                    string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";

                                    Parallel.ForEach(Global.CashboxSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in CashboxDT.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            foreach (DataRow row in query)
                                            {
                                                string CashboxId = Convert.ToInt32(row["Cashbox_Id"].ToString()).ToString(MaxDigitCode);
                                                lock (LockThis) { CashboxIdList.Add(CashboxId); }
                                            }
                                        }
                                    });
                                }

                                CashboxIdList.Sort();

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in dt.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            foreach (DataRow row in query)
                                            {
                                                if (CashboxIdList.IndexOf(row["Song_Id"].ToString()) < 0)
                                                {
                                                    lock (LockThis) { RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row)); }
                                                }
                                            }
                                        }
                                    });

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                }
                                break;
                            default:
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                                break;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_EditMode_CheckBox.Enabled = false;
                                SongQuery_LangFilter_ComboBox.Enabled = false;
                                SongQuery_WordCountFilter_ComboBox.Enabled = false;
                                SongQuery_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                            });
                        }
                        else
                        {
                            if (SongQueryType == "SingerName" && Global.SongQueryFuzzyQuery == "False")
                            {
                                var query = from row in dt.AsEnumerable()
                                            where row.Field<string>("Song_Singer") != SongQueryValue
                                            select row;

                                if (query.Count<DataRow>() > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    foreach (DataRow row in query)
                                    {
                                        bool RemoveRow = true;
                                        List<string> list = CommonFunc.GetChorusSingerList(row["Song_Singer"].ToString());
                                        foreach (string str in list)
                                        {
                                            if (str.ToLower() == SongQueryValue.ToLower()) RemoveRow = false;
                                        }
                                        if (RemoveRow) RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        list.Clear();
                                        list = null;
                                    }

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                }
                            }

                            if (dt.Rows.Count == 0)
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    SongQuery_EditMode_CheckBox.Enabled = false;
                                    SongQuery_LangFilter_ComboBox.Enabled = false;
                                    SongQuery_WordCountFilter_ComboBox.Enabled = false;
                                    SongQuery_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                                });
                            }
                            else
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    SongQuery_EditMode_CheckBox.Enabled = (dt.Rows.Count > 0) ? true : false;

                                    if(!SongQuery.QueryStatusLabel)
                                    {
                                        SongQuery.QueryStatusLabel = true;
                                    }
                                    else
                                    {
                                        SongQuery_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的歌曲。";
                                    }

                                    SongQuery_DataGridView.DataSource = dt;

                                    List<string> LangFilterList = new List<string>();
                                    List<int> WordCountFilterList = new List<int>();

                                    using (DataTable LangDistinctDT = dt.DefaultView.ToTable(true, "Song_Lang"))
                                    {
                                        if (LangDistinctDT.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in LangDistinctDT.AsEnumerable())
                                            {
                                                LangFilterList.Add(row["Song_Lang"].ToString());
                                            }
                                        }
                                    }

                                    using (DataTable WordCountDistinctDT = dt.DefaultView.ToTable(true, "Song_WordCount"))
                                    {
                                        if (WordCountDistinctDT.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in WordCountDistinctDT.AsEnumerable())
                                            {
                                                WordCountFilterList.Add(Convert.ToInt32(row["Song_WordCount"].ToString()));
                                            }
                                        }
                                    }

                                    if (LangFilterList.Count > 0)
                                    {
                                        SongQuery_LangFilter_ComboBox.Enabled = true;
                                        SongQuery_LangFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(LangFilterList);
                                        SongQuery_LangFilter_ComboBox.DisplayMember = "Display";
                                        SongQuery_LangFilter_ComboBox.ValueMember = "Value";
                                        LangFilterList.Clear();
                                    }

                                    if (WordCountFilterList.Count > 0)
                                    {
                                        WordCountFilterList.Sort();
                                        SongQuery_WordCountFilter_ComboBox.Enabled = true;
                                        SongQuery_WordCountFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(WordCountFilterList.ConvertAll(i => i.ToString()));
                                        SongQuery_WordCountFilter_ComboBox.DisplayMember = "Display";
                                        SongQuery_WordCountFilter_ComboBox.ValueMember = "Value";
                                        WordCountFilterList.Clear();
                                    }

                                    for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
                                    {
                                        List<string> DataGridViewColumnName = SongQuery.GetDataGridViewColumnSet(SongQuery_DataGridView.Columns[i].Name);
                                        SongQuery_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                        if (DataGridViewColumnName[1].ToString() == "0")
                                        {
                                            SongQuery_DataGridView.Columns[i].Visible = false;
                                        }

                                        if (DataGridViewColumnName[2].ToString() != "none")
                                        {
                                            ((DataGridViewTextBoxColumn)SongQuery_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                        }

                                        SongQuery_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                    }

                                    string SongFullPath = "";
                                    int SongFullPathIndex = SongQuery_DataGridView.ColumnCount - 1;
                                    SongQuery_DataGridView.Columns.Add("Song_FullPath", "檔案路徑");

                                    int ColumnWidth_640 = Convert.ToInt32(640 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor);
                                    SongQuery_DataGridView.Columns["Song_FullPath"].Width = ColumnWidth_640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = ColumnWidth_640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                                    for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                    {
                                        SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                        SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                    }
                                    SongQuery_DataGridView.Focus();

                                    dt.Dispose();
                                    dt = null;
                                });
                            }
                        }
                    }
                    catch
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_EditMode_CheckBox.Enabled = false;
                            SongQuery_LangFilter_ComboBox.Enabled = false;
                            SongQuery_WordCountFilter_ComboBox.Enabled = false;
                            SongQuery_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        });
                    }
                }
            }
        }




        #endregion

        #region --- SongQuery 編輯模式切換 ---

        private void SongQuery_EditMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                SongQuery_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(270 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                SongQuery_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                SongQuery_Edit_GroupBox.Visible = true;
                SongQuery_TabControl.Visible = false;
                SongQuery_QueryFilter_GroupBox.Visible = false;
                SongQuery_Statistics_GroupBox.Visible = false;

                Global.SongQueryMultiEdit = false;
                SongQuery_InitializeEditControl();

                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
                SongQuery_DataGridView_SelectionChanged(new object(), new EventArgs());
                if (SelectedRowsCount > 1) SongQuery_DataGridView_MouseUp(new object(), null);

                SongQuery_QueryStatus_Label.Text = "已進入編輯模式...";
            }
            else
            {
                SongQuery_DataGridView.Size = new Size(Convert.ToInt32(952 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(296 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                SongQuery_DataGridView.Location = new Point(Convert.ToInt32(22 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor), Convert.ToInt32(365 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor));
                SongQuery_EditMode_CheckBox.Enabled = (SongQuery_DataGridView.RowCount == 0) ? false : true;
                SongQuery_Edit_GroupBox.Visible = false;
                SongQuery_TabControl.Visible = true;
                SongQuery_QueryFilter_GroupBox.Visible = true;
                SongQuery_Statistics_GroupBox.Visible = true;

                SongQuery_QueryStatus_Label.Text = "已進入檢視模式...";
            }
            SongQuery_DataGridView.Focus();
        }

        #endregion

        #region --- SongQuery 更新歌曲 ---

        private void SongQuery_SongUpdate(List<string> UpdateList, DataTable UpdateDT)
        {
            if (UpdateList.Count <= 0) return;

            List<string> SongUpdateValueList = new List<string>();
            List<string> updvaluelist;

            foreach (string UpdateStr in UpdateList)
            {
                updvaluelist = new List<string>(UpdateStr.Split('|'));

                string OldSongId = updvaluelist[19];
                string SongId = updvaluelist[0];
                string SongLang = updvaluelist[1];
                int SongSingerType = Convert.ToInt32(updvaluelist[2]);
                string SongSinger = updvaluelist[3];
                string SongSongName = updvaluelist[4];
                int SongTrack = Convert.ToInt32(updvaluelist[5]);
                string SongSongType = updvaluelist[6];
                string SongVolume = updvaluelist[7];
                string SongWordCount = updvaluelist[8];
                string SongPlayCount = updvaluelist[9];
                string SongMB = updvaluelist[10];
                string SongCreatDate = updvaluelist[11];
                string SongFileName = updvaluelist[12];
                string SongPath = updvaluelist[13];
                string SongSpell = updvaluelist[14];
                string SongSpellNum = updvaluelist[15];
                string SongSongStroke = updvaluelist[16];
                string SongPenStyle = updvaluelist[17];
                string SongPlayState = updvaluelist[18];

                updvaluelist.Clear();

                string DupSongData = "";
                bool DuplicateSongStatus = false;
                bool MoveError = false;

                // SingerName bug
                bool SingerNameisEmpty = false;
                if (SongSinger == "")
                {
                    SingerNameisEmpty = true;

                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】歌手名稱空白: 階段1 [" + UpdateStr + "]";
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }

                // 重複歌曲判斷
                if (SongQuery.SongDataLowCaseList.Count > 0)
                {
                    string SongData = SongLang + "|" + SongSinger.ToLower() + "|" + SongSongName.ToLower() + "|" + SongSongType.ToLower();

                    if (SongQuery.SongDataLowCaseList.IndexOf(SongData) >= 0)
                    {
                        if (SongQuery.SongDataIdList[SongQuery.SongDataLowCaseList.IndexOf(SongData)] != OldSongId)
                        {
                            DupSongData = SongQuery.SongDataIdList[SongQuery.SongDataLowCaseList.IndexOf(SongData)] + "|" + SongQuery.SongDataList[SongQuery.SongDataLowCaseList.IndexOf(SongData)];
                            DuplicateSongStatus = true;
                        }
                    }
                    else
                    {
                        if (Global.GroupSingerLowCaseList.IndexOf(SongSinger.ToLower()) >= 0)
                        {
                            int SingerGroupId = Global.GroupSingerIdList[Global.GroupSingerLowCaseList.IndexOf(SongSinger.ToLower())];
                            List<string> GroupSingerList = new List<string>(Global.SingerGroupList[SingerGroupId].Split(','));
                            if (GroupSingerList.Count > 0)
                            {
                                foreach (string GroupSingerName in GroupSingerList)
                                {
                                    if (GroupSingerName.ToLower() != SongSinger.ToLower())
                                    {
                                        SongData = SongLang + "|" + GroupSingerName.ToLower() + "|" + SongSongName.ToLower() + "|" + SongSongType.ToLower();

                                        if (SongQuery.SongDataLowCaseList.IndexOf(SongData) >= 0)
                                        {
                                            if (SongQuery.SongDataIdList[SongQuery.SongDataLowCaseList.IndexOf(SongData)] != OldSongId)
                                            {
                                                DupSongData = SongQuery.SongDataIdList[SongQuery.SongDataLowCaseList.IndexOf(SongData)] + "|" + SongQuery.SongDataList[SongQuery.SongDataLowCaseList.IndexOf(SongData)];
                                                DuplicateSongStatus = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                GroupSingerList.Clear();
                            }
                        }
                    }

                    if (!DuplicateSongStatus && SongSingerType == 3)
                    {
                        List<string> ChorusSongDatalist = new List<string>() { SongLang, SongSongName.ToLower() };
                        if (SongSongType != "") ChorusSongDatalist.Add(SongSongType.ToLower());

                        List<string> ChorusGroupSongDatalist = new List<string>() { SongLang, SongSongName.ToLower() };
                        if (SongSongType != "") ChorusGroupSongDatalist.Add(SongSongType.ToLower());

                        // 處理合唱歌曲中的特殊歌手名稱
                        string ChorusSongSingerName = SongSinger;
                        int ChorusGroupSongSingerCount = 0;
                        bool MatchChorusGroupSongSinger = false;
                        List<string> ChorusSingerList = new List<string>();
                        List<string> ChorusGroupSingerList = new List<string>();

                        List<string> slist = CommonFunc.GetChorusSingerList(ChorusSongSingerName);
                        foreach (string str in slist)
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
                        slist.Clear();
                        slist = null;

                        List<string> FindResultList = new List<string>();

                        if (!DuplicateSongStatus && ChorusSingerList.Count > 0)
                        {
                            FindResultList = SongQuery.SongDataLowCaseList.FindAll(SongInfo => SongInfo.ContainsAll(ChorusSongDatalist.ToArray()));
                            if (FindResultList.Count > 0)
                            {
                                foreach (string FindResult in FindResultList)
                                {
                                    List<string> list = new List<string>(FindResult.Split('|'));

                                    if (list[1].ContainsAll(ChorusSingerList.ToArray()) && list[2] == SongSongName.ToLower())
                                    {
                                        int ChorusSongDataIndex = SongQuery.SongDataLowCaseList.IndexOf(FindResult);
                                        if (SongQuery.SongDataIdList[ChorusSongDataIndex] != OldSongId)
                                        {
                                            DupSongData = SongQuery.SongDataIdList[ChorusSongDataIndex] + "|" + SongQuery.SongDataList[ChorusSongDataIndex];
                                            DuplicateSongStatus = true;
                                        }
                                        break;
                                    }
                                    list.Clear();
                                }
                            }
                            FindResultList.Clear();
                        }

                        if (!DuplicateSongStatus && MatchChorusGroupSongSinger)
                        {
                            FindResultList = SongQuery.SongDataLowCaseList.FindAll(SongInfo => SongInfo.ContainsAll(ChorusGroupSongDatalist.ToArray()));
                            if (FindResultList.Count > 0)
                            {
                                foreach (string FindResult in FindResultList)
                                {
                                    List<string> list = new List<string>(FindResult.Split('|'));

                                    if (list[1].ContainsCount(ChorusGroupSongSingerCount, ChorusGroupSingerList.ToArray()) && list[2] == SongSongName.ToLower())
                                    {
                                        int ChorusSongDataIndex = SongQuery.SongDataLowCaseList.IndexOf(FindResult);
                                        if (SongQuery.SongDataIdList[ChorusSongDataIndex] != OldSongId)
                                        {
                                            DupSongData = SongQuery.SongDataIdList[ChorusSongDataIndex] + "|" + SongQuery.SongDataList[ChorusSongDataIndex];
                                            DuplicateSongStatus = true;
                                        }
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
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】已有重複歌曲: " + DupSongData;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                    lock(LockThis) { Global.TotalList[1]++; }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                    });
                }

                // SingerName bug
                if (SongSinger == "")
                {
                    SingerNameisEmpty = true;

                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】歌手名稱空白: 階段2 [" + UpdateStr + "]";
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                    lock (LockThis) { Global.TotalList[2]++; }

                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                    });
                }

                if (!DuplicateSongStatus && !SingerNameisEmpty)
                {
                    if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
                    {
                        string SongSrcPath = Path.Combine(SongPath, SongFileName);
                        string SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongFileName, SongPath, false, "", true, false);
                        SongPath = Path.GetDirectoryName(SongDestPath) + @"\";
                        SongFileName = Path.GetFileName(SongDestPath);

                        if (File.Exists(SongSrcPath))
                        {
                            if (!Directory.Exists(SongPath)) Directory.CreateDirectory(SongPath);

                            if (File.Exists(SongDestPath))
                            {
                                if (SongSrcPath.ToLower() == SongDestPath.ToLower())
                                {
                                    try
                                    {
                                        FileAttributes attributes = File.GetAttributes(SongSrcPath);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(SongSrcPath, attributes);
                                        }

                                        File.Move(SongSrcPath, SongPath + "Temp_" + SongFileName);
                                        File.Move(SongPath + "Temp_" + SongFileName, SongDestPath);
                                    }
                                    catch
                                    {
                                        MoveError = true;
                                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (檔案唯讀或正在使用)";
                                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                                        lock (LockThis) { Global.TotalList[2]++; }

                                        this.BeginInvoke((Action)delegate()
                                        {
                                            SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                                        });
                                    }
                                }
                                else
                                {
                                    MoveError = true;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (歌庫裡已存在該首歌曲的檔案)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                                    lock (LockThis) { Global.TotalList[2]++; }

                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                                    });
                                }
                            }
                            else
                            {
                                try
                                {
                                    File.Move(SongSrcPath, SongDestPath);
                                }
                                catch
                                {
                                    MoveError = true;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (檔案唯讀或正在使用)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                                    lock (LockThis) { Global.TotalList[2]++; }

                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                                    });
                                }
                            }
                        }
                        else
                        {
                            MoveError = true;
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (來源檔案不存在)";
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                            lock (LockThis) { Global.TotalList[2]++; }

                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                            });
                        }
                    }
                }
                
                if (MoveError || DuplicateSongStatus || SingerNameisEmpty)
                {
                    if (Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)] == Convert.ToInt32(SongId))
                    {
                        Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]--;
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        foreach (string str in Global.SongQueryDataGridViewSelectList)
                        {
                            List<string> RestoreSongDataList = new List<string>(str.Split('|'));
                            if (RestoreSongDataList[0] == OldSongId)
                            {
                                List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));
                                Global.SongQueryDataGridViewRestoreSelectList.Add(RestoreSongDataList[0]);

                                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
                                if (SelectedRowsCount == 1)
                                {
                                    SongQuery_EditSongId_TextBox.Text = RestoreSongDataList[0];
                                    SongQuery_EditSongLang_ComboBox.SelectedValue = Global.CrazyktvSongLangList.IndexOf(RestoreSongDataList[1]) + 1;
                                    SongQuery_EditSongCreatDate_DateTimePicker.Value = DateTime.Parse(RestoreSongDataList[11]);
                                    SongQuery_EditSongSinger_TextBox.Text = RestoreSongDataList[3];
                                    string SongSingerTypeStr = CommonFunc.GetSingerTypeStr(Convert.ToInt32(RestoreSongDataList[2]), 1, "null");
                                    SongQuery_EditSongSingerType_ComboBox.SelectedValue = Convert.ToInt32(CommonFunc.GetSingerTypeStr(0, 3, SongSingerTypeStr)) + 1;
                                    SongQuery_EditSongSongName_TextBox.Text = RestoreSongDataList[4];
                                    SongQuery_EditSongSongType_ComboBox.SelectedValue = (SongSongType == "") ? 1 : SongSongTypeList.IndexOf(RestoreSongDataList[6]) + 2;
                                    SongQuery_EditSongSpell_TextBox.Text = RestoreSongDataList[14];
                                    SongQuery_EditSongWordCount_TextBox.Text = RestoreSongDataList[8];
                                    SongQuery_EditSongSrcPath_TextBox.Text = RestoreSongDataList[19];
                                    SongQuery_EditSongTrack_ComboBox.SelectedValue = RestoreSongDataList[5];
                                    SongQuery_EditSongVolume_TextBox.Text = RestoreSongDataList[7];
                                    SongQuery_EditSongPlayCount_TextBox.Text = RestoreSongDataList[9];
                                }
                                break;
                            }
                        }
                    });
                }
                else
                {
                    string SongUpdateValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + OldSongId;
                    SongUpdateValueList.Add(SongUpdateValue);
                }
            }

            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, ""))
            {
                string sqlColumnStr = "Song_Id = @SongId, Song_Lang = @SongLang, Song_SingerType = @SongSingerType, Song_Singer = @SongSinger, Song_SongName = @SongSongName, Song_Track = @SongTrack, Song_SongType = @SongSongType, Song_Volume = @SongVolume, Song_WordCount = @SongWordCount, Song_PlayCount = @SongPlayCount, Song_MB = @SongMB, Song_CreatDate = @SongCreatDate, Song_FileName = @SongFileName, Song_Path = @SongPath, Song_Spell = @SongSpell, Song_SpellNum = @SongSpellNum, Song_SongStroke = @SongSongStroke, Song_PenStyle = @SongPenStyle, Song_PlayState = @SongPlayState";
                string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @OldSongId";
                using (OleDbCommand cmd = new OleDbCommand(SongUpdateSqlStr, conn))
                {
                    List<string> valuelist;
                    List<string> dtvaluelist;

                    foreach (string str in SongUpdateValueList)
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
                        cmd.Parameters.AddWithValue("@OldSongId", valuelist[19]);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            Global.TotalList[0]++;

                            this.BeginInvoke((Action)delegate()
                            {
                                dtvaluelist = new List<string>(str.Split('|'));
                                Global.SongQueryDataGridViewRestoreSelectList.Add(dtvaluelist[0]);

                                var query = from row in UpdateDT.AsEnumerable()
                                            where row["Song_Id"].ToString() == dtvaluelist[19]
                                            select row;

                                foreach (DataRow row in query)
                                {
                                    row["Song_Id"] = dtvaluelist[0];
                                    row["Song_Lang"] = dtvaluelist[1];
                                    row["Song_SingerType"] = dtvaluelist[2];
                                    row["Song_Singer"] = dtvaluelist[3];
                                    row["Song_SongName"] = dtvaluelist[4];
                                    row["Song_Track"] = dtvaluelist[5];
                                    row["Song_SongType"] = dtvaluelist[6];
                                    row["Song_Volume"] = dtvaluelist[7];
                                    row["Song_WordCount"] = dtvaluelist[8];
                                    row["Song_PlayCount"] = dtvaluelist[9];
                                    row["Song_MB"] = dtvaluelist[10];
                                    row["Song_CreatDate"] = dtvaluelist[11];
                                    row["Song_FileName"] = dtvaluelist[12];
                                    row["Song_Path"] = dtvaluelist[13];
                                    row["Song_Spell"] = dtvaluelist[14];
                                    row["Song_SpellNum"] = dtvaluelist[15];
                                    row["Song_SongStroke"] = dtvaluelist[16];
                                    row["Song_PenStyle"] = dtvaluelist[17];
                                    row["Song_PlayState"] = dtvaluelist[18];
                                }

                                dtvaluelist.Clear();
                                SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                            });

                        }
                        catch
                        {
                            Global.TotalList[2]++;
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】更新資料庫時發生錯誤: " + str;
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首...";
                            });
                        }
                        cmd.Parameters.Clear();

                        if (SongQuery.FavoriteDataIdList.IndexOf(valuelist[19]) >= 0)
                        {
                            string FavoriteUpdSqlStr = "update ktv_Favorite set Song_Id = @SongId where Song_Id = @OldSongId";

                            using (OleDbCommand Fcmd = new OleDbCommand(FavoriteUpdSqlStr, conn))
                            {
                                Fcmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                                Fcmd.Parameters.AddWithValue("@OldSongId", valuelist[19]);
                                Fcmd.ExecuteNonQuery();
                                Fcmd.Parameters.Clear();
                            }
                        }
                        valuelist.Clear();
                    }
                }
            }

            int MaxDigitCode;
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
            var tasks = new List<Task>()
            {
                Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)),
                Task.Factory.StartNew(() => CommonFunc.GetUnusedSongId(MaxDigitCode)),
                Task.Factory.StartNew(() => Common_GetSongStatisticsTask())
            };

            this.BeginInvoke((Action)delegate()
            {
                if (Global.SongLogDT.Rows.Count > 0)
                {
                    SongLog_TabPage.Text = "操作記錄 (" + Global.SongLogDT.Rows.Count + ")";
                }

                if (SongQuery_QueryStatus_Label.Text == "") SongQuery_QueryStatus_Label.Text = (Global.SongMgrSongAddMode != "3") ? "已成功更新所選歌曲資料及檔案!" : "已成功更新所選歌曲資料!";
            });
        }

        #endregion

        #region --- SongQuery 移除歌曲 ---

        private List<string> SongQuery_SongRemove(List<string> SongIdlist, List<string> SongFilelist)
        {
            List<string> RemoveSongIdlist = new List<string>();
            using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, ""))
            {
                string SongRemoveSqlStr = "delete from ktv_Song where Song_Id = @SongId";
                using (OleDbCommand cmd = new OleDbCommand(SongRemoveSqlStr, conn))
                {
                    foreach (string SongId in SongIdlist)
                    {
                        int i = SongIdlist.IndexOf(SongId);
                        bool RemoveError = false;

                        string str = SongFilelist[i];

                        if (Global.SongMgrSongAddMode != "3" && !SongQuery.QuerySameFileSong)
                        {
                            if (File.Exists(str))
                            {
                                try
                                {
                                    FileAttributes attributes = File.GetAttributes(str);
                                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                    {
                                        attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                        File.SetAttributes(str, attributes);
                                    }

                                    if (Global.SongMgrBackupRemoveSong == "True")
                                    {
                                        string SongFileName = Path.GetFileName(str);
                                        if (!Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\RemoveSong");
                                        if (File.Exists(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName)) File.Delete(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                        File.Move(str, Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName);
                                        CommonFunc.SetFileTime(Application.StartupPath + @"\SongMgr\RemoveSong\" + SongFileName, DateTime.Now);
                                    }
                                    else
                                    {
                                        File.Delete(str);
                                    }
                                }
                                catch
                                {
                                    RemoveError = true;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + str + " (唯讀或使用中)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                                    });
                                }
                            }
                        }

                        if (!RemoveError)
                        {
                            cmd.Parameters.AddWithValue("@SongId", SongIdlist[i]);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            RemoveSongIdlist.Add(SongIdlist[i]);

                            if (SongQuery.FavoriteDataIdList.IndexOf(SongIdlist[i]) >= 0)
                            {
                                string FavoriteRemoveSqlStr = "delete from ktv_Favorite where Song_Id = @SongId";
                                using (OleDbCommand Fcmd = new OleDbCommand(FavoriteRemoveSqlStr, conn))
                                {
                                    Fcmd.Parameters.AddWithValue("@SongId", SongIdlist[i]);
                                    Fcmd.ExecuteNonQuery();
                                    Fcmd.Parameters.Clear();
                                }
                            }

                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_QueryStatus_Label.Text = (Global.SongMgrSongAddMode != "3") ? "已成功移除所選歌曲資料及檔案!" : "已成功移除所選歌曲資料!";
                            });
                        }
                    }
                }
            }

            int MaxDigitCode;
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
            var tasks = new List<Task>()
            {
                Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)),
                Task.Factory.StartNew(() => CommonFunc.GetUnusedSongId(MaxDigitCode)),
                Task.Factory.StartNew(() => Common_GetSongStatisticsTask()),
                Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6))
            };
            return RemoveSongIdlist;
        }

        #endregion

        #region --- SongQuery 異常查詢 --

        private void SongQuery_ExceptionalQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus && SongQuery_ExceptionalQuery_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                SongQuery_ExceptionalQuery_ComboBox.SelectedIndexChanged -= new EventHandler(SongQuery_ExceptionalQuery_ComboBox_SelectedIndexChanged);
                SongQuery.QuerySameFileSong = false;
                Global.SongQueryQueryType = "SongQuery";

                SongQuery_Query_Button.Enabled = false;
                Common_SwitchSetUI(false);

                SongQuery_DataGridView.DataSource = null;
                if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                SongQuery_QueryStatus_Label.Text = "";
                GC.Collect();

                string SongQueryType = "None";
                string SongQueryValue = "";
                string SongQueryStatusText = "";

                var tasks = new List<Task>();

                switch (SongQuery_ExceptionalQuery_ComboBox.SelectedValue.ToString())
                {
                    case "1":
                        SongQueryType = "FileNotExists";
                        SongQueryValue = "NA";
                        SongQueryStatusText = "檔案不存在";
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "2":
                        SongQueryType = "SameFileSong";
                        SongQueryValue = "NA";
                        SongQueryStatusText = "使用相同檔案";
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        SongQuery.QuerySameFileSong = true;
                        break;
                    case "3":
                        SongQueryType = "DuplicateSong";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "4":
                        SongQueryType = "DuplicateSongIgnoreSongType";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "5":
                        SongQueryType = "DuplicateSongIgnorebrackets";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "6":
                        SongQueryType = "DuplicateSongIgnoreSinger";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "7":
                        SongQueryType = "DuplicateSongOnlyChorusSinger";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "8":
                        SongQueryType = "MatchSongTrack";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "9":
                        SongQueryType = "MatchSingerData";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "10":
                        SongQueryType = "MatchSingerType";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "11":
                        SongQueryType = "MatchSongLang";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "12":
                        SongQueryType = "SameNameSongIgnorebrackets";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                }
                tasks.Add(Task.Factory.StartNew(() => SongQuery_ExceptionalQueryTask(SongQueryType, SongQueryValue, SongQueryStatusText)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_ExceptionalQuery_ComboBox.SelectedIndexChanged += new EventHandler(SongQuery_ExceptionalQuery_ComboBox_SelectedIndexChanged);
                        Common_SwitchSetUI(true);
                        SongQuery_Query_Button.Enabled = true;
                    });
                });
            }
        }

        private void SongQuery_ExceptionalQueryTask(string SongQueryType, string SongQueryValue, string SongQueryStatusText)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                if (SongQueryValue == "")
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_EditMode_CheckBox.Enabled = false;
                        SongQuery_LangFilter_ComboBox.Enabled = false;
                        SongQuery_WordCountFilter_ComboBox.Enabled = false;
                        SongQuery_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                    });
                }
                else
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        switch (SongQueryType)
                        {
                            case "FileNotExists":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in dt.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            foreach (DataRow row in query)
                                            {
                                                if (File.Exists(Path.Combine(row.Field<string>("Song_Path"), row.Field<string>("Song_FileName"))))
                                                {
                                                    lock (LockThis)
                                                    {
                                                        RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                                    }
                                                }
                                            }
                                        }
                                    });

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear(); 
                                }
                                break;
                            case "DuplicateSong":
                            case "DuplicateSongIgnoreSongType":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                                using (DataTable chorusdt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr("DuplicateSongOnlyChorusSinger", SongQueryValue, null), ""))
                                {
                                    if (chorusdt.Rows.Count > 0)
                                    {
                                        int rowid;
                                        string SongLang = string.Empty;
                                        string SongSinger = string.Empty;
                                        string SongSongName = string.Empty;
                                        string SongSongType = string.Empty;

                                        List<int> rowidlist = new List<int>();
                                        List<string> songdatalist = new List<string>();
                                        foreach (DataRow row in chorusdt.Rows)
                                        {
                                            rowid = chorusdt.Rows.IndexOf(row);
                                            List<string> list = CommonFunc.GetChorusSingerList(row["Song_Singer"].ToString());
                                            list.Sort();

                                            SongLang = row["Song_Lang"].ToString();
                                            SongSinger = string.Join("|", list).ToLower();
                                            SongSongName = row["Song_SongName"].ToString().ToLower();
                                            SongSongType = row["Song_SongType"].ToString();
                                            
                                            if (songdatalist.IndexOf(SongLang + "|" + SongSinger + "|" + SongSongName + ((SongQueryType == "DuplicateSong") ? "|" + SongSongType : "")) >= 0)
                                            {
                                                int duprowid = songdatalist.IndexOf(SongLang + "|" + SongSinger + "|" + SongSongName + ((SongQueryType == "DuplicateSong") ? "|" + SongSongType : ""));
                                                DataRow addrow = dt.NewRow();
                                                foreach (DataColumn col in dt.Columns)
                                                {
                                                    addrow[col.ColumnName] = chorusdt.Rows[rowidlist[duprowid]][col.ColumnName];
                                                }
                                                if (dt.Rows.IndexOf(addrow) < 0) dt.Rows.Add(addrow);

                                                addrow = dt.NewRow();
                                                foreach (DataColumn col in dt.Columns)
                                                {
                                                    addrow[col.ColumnName] = row[col.ColumnName];
                                                }
                                                if (dt.Rows.IndexOf(addrow) < 0) dt.Rows.Add(addrow);
                                            }
                                            rowidlist.Add(rowid);
                                            songdatalist.Add(SongLang + "|" + SongSinger + "|" + SongSongName + ((SongQueryType == "DuplicateSong") ? "|" + SongSongType : ""));
                                            
                                            list.Clear();
                                            list = null;
                                        }
                                        rowidlist.Clear();
                                        rowidlist = null;
                                        songdatalist.Clear();
                                        songdatalist = null;
                                    }
                                }
                                break;
                            case "DuplicateSongIgnorebrackets":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "正在比對重複歌曲 (忽略括號及歌曲類別),請稍待...";
                                    });

                                    Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in dt.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            List<string> SongDataList = new List<string>();
                                            List<int> RowIndexList = new List<int>();

                                            foreach (DataRow row in query)
                                            {
                                                string SongSingerType = row["Song_SingerType"].ToString();
                                                List<string> list = new List<string>();
                                                if (SongSingerType == "3")
                                                {
                                                    list = CommonFunc.GetChorusSingerList(row["Song_Singer"].ToString());
                                                    list.Sort();
                                                }
                                                else
                                                {
                                                    list.Add(row["Song_Singer"].ToString());
                                                }

                                                string SongLang = row["Song_Lang"].ToString();
                                                string SongSinger = string.Join("|", list).ToLower();
                                                string SongSongName = row["Song_SongName"].ToString();
                                                string SongSongType = row["Song_SongType"].ToString();
                                                SongSongName = Regex.Replace(SongSongName, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", "", RegexOptions.IgnoreCase);
                                                SongSongName = Regex.Replace(SongSongName, @"^\s*|\s*$", ""); //去除頭尾空白

                                                string SongData = SongLang + "|" + SongSinger.ToLower() + "|" + SongSongName.ToLower();
                                                SongDataList.Add(SongData);
                                                RowIndexList.Add(dt.Rows.IndexOf(row));
                                                list.Clear();
                                                list = null;
                                            }
                                            
                                            if (SongDataList.Count > 0)
                                            {
                                                List<string> FindResultList = new List<string>();
                                                foreach (string SongData in SongDataList)
                                                {
                                                    FindResultList = SongDataList.FindAll(SongInfo => SongInfo.Equals(SongData));
                                                    if (FindResultList.Count > 1)
                                                    {
                                                        lock (LockThis)
                                                        {
                                                            Global.TotalList[0]++;
                                                            this.BeginInvoke((Action)delegate()
                                                            {
                                                                SongQuery_QueryStatus_Label.Text = "已在歌庫比對到 " + Global.TotalList[0] + " 首重複歌曲...";
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lock (LockThis) { RemoveRowsIdxlist.Add(RowIndexList[SongDataList.IndexOf(SongData)]); }
                                                    }
                                                }
                                            }
                                            SongDataList.Clear();
                                            RowIndexList.Clear();
                                        }
                                    });

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                }
                                break;
                            case "MatchSongTrack":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    string SongInfoSeparate = (Global.SongMgrSongInfoSeparate == "1") ? "_" : "-";

                                    var MatchSongTrackQuery = from row in dt.AsEnumerable()
                                                              where row.Field<string>("Song_FileName").Contains(SongInfoSeparate + CommonFunc.GetSongTrackStr(row.Field<byte>("Song_Track"), 1, "null"))
                                                              select row;

                                    foreach (DataRow row in MatchSongTrackQuery)
                                    {
                                        RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                    }

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                }
                                break;
                            case "MatchSingerData":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();
                                    List<string> SingerLowCaseList = new List<string>();

                                    string SongSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_Singer";
                                    using (DataTable SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, ""))
                                    {
                                        foreach (DataRow SingerRow in SingerDT.AsEnumerable())
                                        {
                                            SingerLowCaseList.Add(SingerRow["Singer_Name"].ToString().ToLower());
                                        }
                                    }

                                    var MatchSingerDataQuery = from row in dt.AsEnumerable()
                                                               where SingerLowCaseList.IndexOf(row["Song_Singer"].ToString().ToLower()) >= 0 ||
                                                                     row["Song_SingerType"].ToString() == "3"
                                                               select row;

                                    foreach (DataRow row in MatchSingerDataQuery)
                                    {
                                        bool RomoveRow = true;
                                        if (row["Song_SingerType"].ToString() == "3")
                                        {
                                            List<string> list = CommonFunc.GetChorusSingerList(row["Song_Singer"].ToString());
                                            foreach (string singer in list)
                                            {
                                                if (SingerLowCaseList.IndexOf(singer.ToLower()) < 0)
                                                {
                                                    RomoveRow = false;
                                                    break;
                                                }
                                            }
                                            list.Clear();
                                            list = null;
                                        }
                                        if (RomoveRow) RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                    }

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                    RemoveRowsIdxlist = null;
                                    SingerLowCaseList.Clear();
                                    SingerLowCaseList = null;
                                }
                                break;
                            case "MatchSingerType":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();
                                    List<string> SingerLowCaseList = new List<string>();
                                    List<string> SingerTypeList = new List<string>();

                                    string SongSingerQuerySqlStr = "select Singer_Id, Singer_Name, Singer_Type from ktv_Singer";
                                    using (DataTable SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, ""))
                                    {
                                        foreach (DataRow SingerRow in SingerDT.AsEnumerable())
                                        {
                                            SingerLowCaseList.Add(SingerRow["Singer_Name"].ToString().ToLower());
                                            SingerTypeList.Add(SingerRow["Singer_Type"].ToString());
                                        }
                                    }

                                    var MatchSingerTypeQuery = from row in dt.AsEnumerable()
                                                               where row["Song_SingerType"].ToString() == ((SingerLowCaseList.IndexOf(row["Song_Singer"].ToString().ToLower()) > -1) ? SingerTypeList[SingerLowCaseList.IndexOf(row["Song_Singer"].ToString().ToLower())] : "11")
                                                               select row;

                                    foreach (DataRow row in MatchSingerTypeQuery)
                                    {
                                        RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                    }

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                    SingerLowCaseList.Clear();
                                    SingerTypeList.Clear();
                                }
                                break;
                            case "MatchSongLang":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();
                                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "正在比對歌曲語系,請稍待...";
                                    });

                                    Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in dt.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            foreach (DataRow row in query)
                                            {
                                                string SongLang = row["Song_Lang"].ToString();
                                                string SongSinger = row["Song_Singer"].ToString();
                                                string SongSongName = row["Song_SongName"].ToString();

                                                int SongDataIndex = CommonFunc.MatchCashboxSong("CashboxLang", null, SongLang, SongSinger, SongSongName, Global.CashboxSongDataLowCaseList, Global.CashboxSongDataFuzzyList);

                                                if (SongDataIndex >= 0)
                                                {
                                                    if (SongLang != Global.CashboxSongDataLangList[SongDataIndex])
                                                    {
                                                        string SongDataLowCase = SongLang + "|" + SongSinger.ToLower() + "|" + SongSongName.ToLower();
                                                        if (Global.CashboxSongDataFullList.IndexOf(SongDataLowCase) >= 0)
                                                        {
                                                            RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                                        }
                                                        else
                                                        {
                                                            Global.TotalList[0]++;
                                                            this.BeginInvoke((Action)delegate()
                                                            {
                                                                SongQuery_QueryStatus_Label.Text = "已在歌庫比對到 " + Global.TotalList[0] + " 首語系不符的歌曲...";
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                                    }

                                                }
                                                else
                                                {
                                                    RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                                }
                                            }
                                        }
                                    });

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                }
                                break;
                            case "SameNameSongIgnorebrackets":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                                    this.BeginInvoke((Action)delegate ()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "正在比對同名歌曲 (忽略括號及歌曲類別),請稍待...";
                                    });

                                    Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in dt.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            List<string> SongDataList = new List<string>();
                                            List<int> RowIndexList = new List<int>();

                                            foreach (DataRow row in query)
                                            {
                                                string SongLang = row["Song_Lang"].ToString();
                                                string SongSongName = row["Song_SongName"].ToString();
                                                SongSongName = Regex.Replace(SongSongName, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", "", RegexOptions.IgnoreCase);
                                                SongSongName = Regex.Replace(SongSongName, @"^\s*|\s*$", ""); //去除頭尾空白

                                                string SongData = SongLang + "|" + SongSongName.ToLower();
                                                SongDataList.Add(SongData);
                                                RowIndexList.Add(dt.Rows.IndexOf(row));
                                            }

                                            if (SongDataList.Count > 0)
                                            {
                                                List<string> FindResultList = new List<string>();
                                                foreach (string SongData in SongDataList)
                                                {
                                                    FindResultList = SongDataList.FindAll(SongInfo => SongInfo.Equals(SongData));
                                                    if (FindResultList.Count > 1)
                                                    {
                                                        lock (LockThis)
                                                        {
                                                            Global.TotalList[0]++;
                                                            this.BeginInvoke((Action)delegate ()
                                                            {
                                                                SongQuery_QueryStatus_Label.Text = "已在歌庫比對到 " + Global.TotalList[0] + " 首同名歌曲...";
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lock (LockThis) { RemoveRowsIdxlist.Add(RowIndexList[SongDataList.IndexOf(SongData)]); }
                                                    }
                                                }
                                            }
                                            SongDataList.Clear();
                                            RowIndexList.Clear();
                                        }
                                    });

                                    RemoveRowsIdxlist.Sort();
                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                    RemoveRowsIdxlist.Clear();
                                }
                                break;
                            default:
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                                break;
                        }
                        
                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_EditMode_CheckBox.Enabled = false;
                                SongQuery_LangFilter_ComboBox.Enabled = false;
                                SongQuery_WordCountFilter_ComboBox.Enabled = false;
                                SongQuery_QueryStatus_Label.Text = "查無異常歌曲,請重新查詢...";
                            });
                        }
                        else
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_EditMode_CheckBox.Enabled = (dt.Rows.Count > 0) ? true : false;
                                SongQuery_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的異常歌曲。";

                                SongQuery_DataGridView.DataSource = dt;

                                List<string> LangFilterList = new List<string>();
                                List<int> WordCountFilterList = new List<int>();

                                using (DataTable LangDistinctDT = dt.DefaultView.ToTable(true, "Song_Lang"))
                                {
                                    if (LangDistinctDT.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in LangDistinctDT.AsEnumerable())
                                        {
                                            LangFilterList.Add(row["Song_Lang"].ToString());
                                        }
                                    }
                                }

                                using (DataTable WordCountDistinctDT = dt.DefaultView.ToTable(true, "Song_WordCount"))
                                {
                                    if (WordCountDistinctDT.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in WordCountDistinctDT.AsEnumerable())
                                        {
                                            WordCountFilterList.Add(Convert.ToInt32(row["Song_WordCount"].ToString()));
                                        }
                                    }
                                }

                                if (LangFilterList.Count > 0)
                                {
                                    SongQuery_LangFilter_ComboBox.Enabled = true;
                                    SongQuery_LangFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(LangFilterList);
                                    SongQuery_LangFilter_ComboBox.DisplayMember = "Display";
                                    SongQuery_LangFilter_ComboBox.ValueMember = "Value";
                                    LangFilterList.Clear();
                                }

                                if (WordCountFilterList.Count > 0)
                                {
                                    WordCountFilterList.Sort();
                                    SongQuery_WordCountFilter_ComboBox.Enabled = true;
                                    SongQuery_WordCountFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(WordCountFilterList.ConvertAll(i => i.ToString()));
                                    SongQuery_WordCountFilter_ComboBox.DisplayMember = "Display";
                                    SongQuery_WordCountFilter_ComboBox.ValueMember = "Value";
                                    WordCountFilterList.Clear();
                                }

                                for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
                                {
                                    List<string> DataGridViewColumnName = SongQuery.GetDataGridViewColumnSet(SongQuery_DataGridView.Columns[i].Name);
                                    SongQuery_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                    if (DataGridViewColumnName[1].ToString() == "0")
                                    {
                                        SongQuery_DataGridView.Columns[i].Visible = false;
                                    }

                                    if (DataGridViewColumnName[2].ToString() != "none")
                                    {
                                        ((DataGridViewTextBoxColumn)SongQuery_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                    }

                                    SongQuery_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                }

                                string SongFullPath = "";
                                int SongFullPathIndex = SongQuery_DataGridView.ColumnCount - 1;
                                SongQuery_DataGridView.Columns.Add("Song_FullPath", "檔案路徑");

                                int ColumnWidth_640 = Convert.ToInt32(640 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor);
                                SongQuery_DataGridView.Columns["Song_FullPath"].Width = ColumnWidth_640;
                                SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = ColumnWidth_640;
                                SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                {
                                    SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                    SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                }
                                SongQuery_DataGridView.Focus();
                                dt.Dispose();
                                dt = null;
                            });
                        }
                    }
                    catch
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_EditMode_CheckBox.Enabled = false;
                            SongQuery_LangFilter_ComboBox.Enabled = false;
                            SongQuery_WordCountFilter_ComboBox.Enabled = false;
                            SongQuery_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        });
                    }
                }
            }
        }

        #endregion

        #region --- SongQuery 我的最愛 ---

        private void SongQuery_GetFavoriteUserList()
        {
            SongQuery_FavoriteQuery_ComboBox.DataSource = CommonFunc.GetFavoriteUserList(0);
            SongQuery_FavoriteQuery_ComboBox.DisplayMember = "Display";
            SongQuery_FavoriteQuery_ComboBox.ValueMember = "Value";
            SongQuery_FavoriteQuery_ComboBox.SelectedValue = 1;
        }

        private void SongQuery_FavoriteQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainTabControl.SelectedTab.Name == "SongQuery_TabPage")
            {
                if (SongQuery_FavoriteQuery_ComboBox.Focused && SongQuery_FavoriteQuery_ComboBox.Text != "System.Data.DataRowView" && SongQuery_FavoriteQuery_ComboBox.Text != "無最愛用戶")
                {
                    Global.SongQueryQueryType = "FavoriteQuery";
                    SongQuery_EditMode_CheckBox.Checked = false;
                    SongQuery_EditMode_CheckBox.Enabled = false;
                    SongQuery_Query_Button.Enabled = false;

                    string UserId = "";

                    var query = from row in Global.FavoriteUserDT.AsEnumerable()
                                where row.Field<string>("User_Name").Equals(SongQuery_FavoriteQuery_ComboBox.Text)
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            UserId = row["User_Id"].ToString();
                            break;
                        }
                    }

                    DataTable dt = new DataTable();
                    string SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
                    dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                    List<string> list = new List<string>();

                    var dtquery = from row in dt.AsEnumerable()
                                  where row.Field<string>("User_Id").Equals(UserId)
                                  select row;

                    foreach (DataRow row in dtquery)
                    {
                        list.Add(row["Song_Id"].ToString());
                    }

                    dt.Dispose();

                    Common_SwitchSetUI(false);
                    var tasks = new List<Task>()
                    {
                        Task.Factory.StartNew(() => SongQuery_FavoriteQueryTask(list))
                    };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Common_SwitchSetUI(true);
                            SongQuery_Query_Button.Enabled = true;
                        });
                    });
                }
            }
        }

        private void SongQuery_FavoriteQueryTask(List<string> SongIdList)
        {
            this.BeginInvoke((Action)delegate()
            {
                SongQuery_DataGridView.DataSource = null;
                if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
                SongQuery_QueryStatus_Label.Text = "";

                string SongQueryStatusText = "";
                string SongQueryValue = "";
                string SongQueryType = "None";

                if (File.Exists(Global.CrazyktvDatabaseFile))
                {
                    SongQueryType = "FavoriteSong";
                    SongQueryValue = "NA";
                    SongQueryStatusText = SongQuery_FavoriteQuery_ComboBox.Text;

                    if (SongQueryValue == "")
                    {
                        SongQuery_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                    }
                    else
                    {
                        SongQuery_QueryStatus_Label.Text = "查詢中,請稍待...";
                        try
                        {
                            DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue, null), "");
                            if (dt.Rows.Count == 0)
                            {
                                SongQuery_LangFilter_ComboBox.Enabled = false;
                                SongQuery_WordCountFilter_ComboBox.Enabled = false;
                                SongQuery_QueryStatus_Label.Text = "查無最愛歌曲,請重新查詢...";
                            }
                            else
                            {
                                List<int> RemoveRowsIdxlist = new List<int>();

                                var query = from row in dt.AsEnumerable()
                                            where !SongIdList.Contains(row.Field<string>("Song_Id"))
                                            select row;

                                foreach (DataRow row in query)
                                {
                                    RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                }

                                for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                {
                                    dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                }


                                SongQuery_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆屬於『" + SongQueryStatusText + "』的最愛歌曲。";

                                if (dt.Rows.Count > 0)
                                {
                                    SongQuery_DataGridView.DataSource = dt;

                                    List<string> LangFilterList = new List<string>();
                                    List<int> WordCountFilterList = new List<int>();

                                    using (DataTable LangDistinctDT = dt.DefaultView.ToTable(true, "Song_Lang"))
                                    {
                                        if (LangDistinctDT.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in LangDistinctDT.AsEnumerable())
                                            {
                                                LangFilterList.Add(row["Song_Lang"].ToString());
                                            }
                                        }
                                    }

                                    using (DataTable WordCountDistinctDT = dt.DefaultView.ToTable(true, "Song_WordCount"))
                                    {
                                        if (WordCountDistinctDT.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in WordCountDistinctDT.AsEnumerable())
                                            {
                                                WordCountFilterList.Add(Convert.ToInt32(row["Song_WordCount"].ToString()));
                                            }
                                        }
                                    }

                                    if (LangFilterList.Count > 0)
                                    {
                                        SongQuery_LangFilter_ComboBox.Enabled = true;
                                        SongQuery_LangFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(LangFilterList);
                                        SongQuery_LangFilter_ComboBox.DisplayMember = "Display";
                                        SongQuery_LangFilter_ComboBox.ValueMember = "Value";
                                        LangFilterList.Clear();
                                    }

                                    if (WordCountFilterList.Count > 0)
                                    {
                                        WordCountFilterList.Sort();
                                        SongQuery_WordCountFilter_ComboBox.Enabled = true;
                                        SongQuery_WordCountFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(WordCountFilterList.ConvertAll(i => i.ToString()));
                                        SongQuery_WordCountFilter_ComboBox.DisplayMember = "Display";
                                        SongQuery_WordCountFilter_ComboBox.ValueMember = "Value";
                                        WordCountFilterList.Clear();
                                    }

                                    for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
                                    {
                                        List<string> DataGridViewColumnName = SongQuery.GetDataGridViewColumnSet(SongQuery_DataGridView.Columns[i].Name);
                                        SongQuery_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                        if (DataGridViewColumnName[1].ToString() == "0")
                                        {
                                            SongQuery_DataGridView.Columns[i].Visible = false;
                                        }

                                        if (DataGridViewColumnName[2].ToString() != "none")
                                        {
                                            ((DataGridViewTextBoxColumn)SongQuery_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                        }

                                        SongQuery_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                    }

                                    string SongFullPath = "";
                                    int SongFullPathIndex = SongQuery_DataGridView.ColumnCount - 1;
                                    SongQuery_DataGridView.Columns.Add("Song_FullPath", "檔案路徑");

                                    int ColumnWidth_640 = Convert.ToInt32(640 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor);
                                    SongQuery_DataGridView.Columns["Song_FullPath"].Width = ColumnWidth_640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = ColumnWidth_640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                    for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                    {
                                        SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                        SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                    }
                                    SongQuery_DataGridView.Focus();
                                    dt.Dispose();
                                }
                            }
                        }
                        catch
                        {
                            SongQuery_LangFilter_ComboBox.Enabled = false;
                            SongQuery_WordCountFilter_ComboBox.Enabled = false;
                            SongQuery_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        }
                    }
                }
            });
        }

        private void SongQuery_FavoriteRemove(object SongIdlist, object UserId)
        {
            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string FavoriteRemoveSqlStr = "delete from ktv_Favorite where User_Id = @UserId and Song_Id = @SongId";
            cmd = new OleDbCommand(FavoriteRemoveSqlStr, conn);

            foreach (string str in (List<string>)SongIdlist)
            {
                cmd.Parameters.AddWithValue("@UserId", (string)UserId);
                cmd.Parameters.AddWithValue("@SongId", str);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            conn.Close();
        }

        private void SongQuery_FavoriteAdd(object SongIdlist, object UserId)
        {
            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            List<string> SongIdAddlist = (List<string>)SongIdlist;

            var query = from row in dt.AsEnumerable()
                        where row.Field<string>("User_Id").Equals((string)UserId)
                        select row;

            foreach (DataRow row in query)
            {
                if (SongIdAddlist.Contains(row["Song_Id"].ToString()))
                {
                    SongIdAddlist.Remove(row["Song_Id"].ToString());
                }
            }

            dt.Dispose();

            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "User_Id, Song_Id";
            string sqlValuesStr = "@UserId, @SongId";
            string FavoriteAddSqlStr = "insert into ktv_Favorite ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";
            cmd = new OleDbCommand(FavoriteAddSqlStr, conn);

            foreach (string str in SongIdAddlist)
            {
                cmd.Parameters.AddWithValue("@UserId", (string)UserId);
                cmd.Parameters.AddWithValue("@SongId", str);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            conn.Close();
        }

        #endregion

        #region --- SongQuery 歌曲編輯 ---

        private void SongQuery_EditSongLang_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (SongQuery_EditSongLang_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongQueryMultiEditUpdateList[0] = (SongQuery_EditSongLang_ComboBox.Text != "不變更") ? true : false;
                    }
                    else if (SelectedRowsCount == 1)
                    {
                        List<string> list = new List<string>(Global.SongQueryDataGridViewSelectList[0].Split('|'));
                        
                        string SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        SongQuery_EditSongId_TextBox.Text = (list[1] != SongLang) ? SongQuery.GetNextSongId(SongLang, false) : list[0];
                        if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                    }
                }
            }
        }

        private void SongQuery_EditSongCreatDate_DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongQueryMultiEditUpdateList[1] = true;
                }
            }
        }

        private void SongQuery_EditSongSinger_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
                string SongSinger = SongQuery_EditSongSinger_TextBox.Text;

                if (SelectedRowsCount > 1)
                {
                    Global.SongQueryMultiEditUpdateList[2] = (SongQuery_EditSongSinger_TextBox.Text != "") ? true : false;
                }
                else if (SelectedRowsCount == 1)
                {
                    if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                }
            }
        }

        private void SongQuery_EditSongSingerType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (SongQuery_EditSongSingerType_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongQueryMultiEditUpdateList[3] = (SongQuery_EditSongSingerType_ComboBox.Text != "不變更") ? true : false;
                    }
                    else if (SelectedRowsCount == 1)
                    {
                        if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                    }
                }
            }
        }

        private void SongQuery_EditSongSongName_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount == 1)
                {
                    string SongSongName = SongQuery_EditSongSongName_TextBox.Text;
                    // 計算歌曲字數
                    List<string> SongWordCountList = new List<string>();
                    SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                    SongQuery_EditSongWordCount_TextBox.Text = SongWordCountList[0];

                    // 取得歌曲拼音
                    List<string> SongSpellList = new List<string>();
                    SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                    SongQuery_EditSongSpell_TextBox.Text = SongSpellList[0];

                    if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                }
            }
        }

        private void SongQuery_EditSongSongType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (SongQuery_EditSongSongType_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongQueryMultiEditUpdateList[4] = (SongQuery_EditSongSongType_ComboBox.Text != "不變更") ? true : false;
                    }
                    else if (SelectedRowsCount == 1)
                    {
                        if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                    }
                }
            }
        }

        private void SongQuery_EditSongTrack_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (SongQuery_EditSongTrack_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                    int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                    if (SelectedRowsCount > 1)
                    {
                        Global.SongQueryMultiEditUpdateList[5] = (SongQuery_EditSongTrack_ComboBox.Text != "不變更") ? true : false;
                    }
                    else if (SelectedRowsCount == 1)
                    {
                        if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                    }
                }
            }
        }

        private void SongQuery_EditSongTrack_Button_Click(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount == 1)
                {
                    List<string> list = new List<string>(Global.SongQueryDataGridViewSelectList[0].Split('|'));

                    string SongFilePath = list[19];
                    if (!File.Exists(SongFilePath))
                    {
                        SongQuery_QueryStatus_Label.Text = "檔案不存在...";
                    }
                    else
                    {
                        string SongId = SongQuery_EditSongId_TextBox.Text;
                        string SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        string SongSinger = SongQuery_EditSongSinger_TextBox.Text;
                        string SongSongName = SongQuery_EditSongSongName_TextBox.Text;
                        string SongTrack = SongQuery_EditSongTrack_ComboBox.SelectedValue.ToString();
                        string SongVolume = SongQuery_EditSongVolume_TextBox.Text;
                        string SongReplayGain = list[20];
                        string SongMeanVolume = list[21];

                        List<string> PlayerSongInfoList = new List<string>() { SongId, SongLang, SongSinger, SongSongName, SongTrack, SongVolume, SongReplayGain, SongMeanVolume, SongFilePath, "0", "SongQueryEdit" };

                        Global.PlayerUpdateSongValueList = new List<string>();
                        if (Global.MainCfgPlayerCore == "1")
                        {
                            DShowForm newPlayerForm = new DShowForm(this, PlayerSongInfoList);
                            newPlayerForm.Show();
                        }
                        this.Hide();
                    }
                }
            }
        }

        private void SongQuery_EditSongVolume_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongQueryMultiEditUpdateList[6] = (SongQuery_EditSongVolume_TextBox.Text != "") ? true : false;
                }
            }
        }

        private void SongQuery_EditSongPlayCount_TextBox_Validated(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongQueryMultiEditUpdateList[7] = (SongQuery_EditSongPlayCount_TextBox.Text != "") ? true : false;
                }
            }
        }

        private void SongQuery_EditApplyChanges_Button_Click(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (Global.SongQueryDataGridViewSelectList.Count <= 0) return;
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
                List<string> UpdateList = new List<string>();

                SongQuery.CreateSongDataTable();
                SongQuery_QueryStatus_Label.Text = "正在更新歌曲資料,請稍待...";
                Common_SwitchSetUI(false);

                string OldSongId;
                string OldSongLang;
                string SongId;
                string SongLang;
                string SongSingerType;
                string SongSinger;
                string SongSongName;
                string SongTrack;
                string SongSongType;
                string SongVolume;
                string SongWordCount;
                string SongPlayCount;
                string SongMB;
                string SongCreatDate;
                string SongFileName;
                string SongPath;
                string SongSpell;
                string SongSpellNum;
                string SongSongStroke;
                string SongPenStyle;
                string SongPlayState;
                string SongSrcPath;
                string CurrentRowSongId = "";

                if (SelectedRowsCount > 1)
                {
                    foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                    {
                        OldSongId = row.Cells["Song_Id"].Value.ToString();
                        OldSongLang = row.Cells["Song_Lang"].Value.ToString();
                        SongId = OldSongId;
                        SongLang = OldSongLang;
                        SongSingerType = row.Cells["Song_SingerType"].Value.ToString();
                        SongSinger = row.Cells["Song_Singer"].Value.ToString();
                        SongSongName = row.Cells["Song_SongName"].Value.ToString();
                        SongTrack = row.Cells["Song_Track"].Value.ToString();
                        SongSongType = row.Cells["Song_SongType"].Value.ToString();
                        SongVolume = row.Cells["Song_Volume"].Value.ToString();
                        SongWordCount = row.Cells["Song_WordCount"].Value.ToString();
                        SongPlayCount = row.Cells["Song_PlayCount"].Value.ToString();
                        SongMB = row.Cells["Song_MB"].Value.ToString();
                        SongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();
                        SongFileName = row.Cells["Song_FileName"].Value.ToString();
                        SongPath = row.Cells["Song_Path"].Value.ToString();
                        SongSpell = row.Cells["Song_Spell"].Value.ToString();
                        SongSpellNum = row.Cells["Song_SpellNum"].Value.ToString();
                        SongSongStroke = row.Cells["Song_SongStroke"].Value.ToString();
                        SongPenStyle = row.Cells["Song_PenStyle"].Value.ToString();
                        SongPlayState = row.Cells["Song_PlayState"].Value.ToString();

                        if (Global.SongQueryMultiEditUpdateList[0])
                        {
                            SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                            if (OldSongLang != SongLang)
                            {
                                SongId = SongQuery.GetNextSongId(SongLang, true);
                            }
                        }

                        if (Global.SongQueryMultiEditUpdateList[1])
                        {
                            SongCreatDate = SongQuery_EditSongCreatDate_DateTimePicker.Value.ToString();
                        }

                        if (Global.SongQueryMultiEditUpdateList[2])
                        {
                            SongSinger = SongQuery_EditSongSinger_TextBox.Text;
                        }

                        if (Global.SongQueryMultiEditUpdateList[3])
                        {
                            string SongSingerTypeStr = ((DataRowView)SongQuery_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                            SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                        }

                        if (Global.SongQueryMultiEditUpdateList[4])
                        {
                            string SongSongTypeStr = ((DataRowView)SongQuery_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                            SongSongType = (SongSongTypeStr != "無類別") ? SongSongTypeStr : "";
                        }

                        if (Global.SongQueryMultiEditUpdateList[5])
                        {
                            SongTrack = ((int)SongQuery_EditSongTrack_ComboBox.SelectedValue - 1).ToString();
                            
                            // 自動偵測歌曲聲道
                            if (SongTrack == "6")
                            {
                                string FilePath = Path.Combine(SongPath, SongFileName);
                                var task = Task.Factory.StartNew(() => CommonFunc.AutoDetectSongTrack(FilePath));
                                SongTrack = task.Result;
                            }
                        }

                        if (Global.SongQueryMultiEditUpdateList[6])
                        {
                            SongVolume = SongQuery_EditSongVolume_TextBox.Text;
                        }

                        if (Global.SongQueryMultiEditUpdateList[7])
                        {
                            SongPlayCount = SongQuery_EditSongPlayCount_TextBox.Text;
                        }

                        if (SongQuery_DataGridView.Rows.IndexOf(row) == SongQuery_DataGridView.CurrentRow.Index)
                        {
                            CurrentRowSongId = SongId;
                        }

                        UpdateList.Add(SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + OldSongId);
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                    {
                        OldSongId = row.Cells["Song_Id"].Value.ToString();
                        SongId = SongQuery_EditSongId_TextBox.Text;
                        SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                        CurrentRowSongId = SongId;

                        string SongSingerTypeStr = ((DataRowView)SongQuery_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                        SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);

                        SongSinger = SongQuery_EditSongSinger_TextBox.Text;
                        SongSongName = SongQuery_EditSongSongName_TextBox.Text;

                        SongTrack = SongQuery_EditSongTrack_ComboBox.SelectedValue.ToString();

                        string SongSongTypeStr = ((DataRowView)SongQuery_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                        SongSongType = (SongSongTypeStr != "無類別") ? SongSongTypeStr : "";

                        SongVolume = SongQuery_EditSongVolume_TextBox.Text;

                        // 計算歌曲字數
                        List<string> SongWordCountList = new List<string>();
                        SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                        SongWordCount = SongWordCountList[0];

                        SongPlayCount = SongQuery_EditSongPlayCount_TextBox.Text;
                        SongMB = row.Cells["Song_MB"].Value.ToString();
                        SongCreatDate = SongQuery_EditSongCreatDate_DateTimePicker.Value.ToString();
                        SongFileName = row.Cells["Song_FileName"].Value.ToString();
                        SongPath = row.Cells["Song_Path"].Value.ToString();

                        // 自動偵測歌曲聲道
                        if (SongTrack == "6")
                        {
                            string FilePath = Path.Combine(SongPath, SongFileName);
                            var task = Task.Factory.StartNew(() => CommonFunc.AutoDetectSongTrack(FilePath));
                            SongTrack = task.Result;
                        }

                        // 取得歌曲拼音
                        List<string> SongSpellList = new List<string>();
                        SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                        if (SongSpellList[2] == "") SongSpellList[2] = "0";

                        SongSpell = SongSpellList[0];
                        SongSpellNum = SongSpellList[1];
                        SongSongStroke = SongSpellList[2];
                        SongPenStyle = SongSpellList[3];
                        SongPlayState = row.Cells["Song_PlayState"].Value.ToString();

                        UpdateList.Add(SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + OldSongId);
                    }
                }

                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Global.SongQueryDataGridViewRestoreSelectList = new List<string>();
                Global.SongQueryDataGridViewRestoreCurrentRow = CurrentRowSongId;
                SongQuery_DataGridView.Sorted -= new EventHandler(SongQuery_DataGridView_Sorted);

                using (DataTable UpdateDT = (DataTable)SongQuery_DataGridView.DataSource)
                {
                    var tasks = new List<Task>()
                    {
                        Task.Factory.StartNew(() => SongQuery_SongUpdate(UpdateList, UpdateDT))
                    };

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_DataGridView.Sorted += new EventHandler(SongQuery_DataGridView_Sorted);
                            SongQuery_DataGridView_Sorted(new object(), new EventArgs());

                            SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                            if (SelectedRowsCount > 1)
                            {
                                Global.SongQueryDataGridViewSelectList = new List<string>();

                                foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                                {
                                    SongId = row.Cells["Song_Id"].Value.ToString();
                                    SongLang = row.Cells["Song_Lang"].Value.ToString();
                                    SongSingerType = row.Cells["Song_SingerType"].Value.ToString();
                                    SongSinger = row.Cells["Song_Singer"].Value.ToString();
                                    SongSongName = row.Cells["Song_SongName"].Value.ToString();
                                    SongTrack = row.Cells["Song_Track"].Value.ToString();
                                    SongSongType = row.Cells["Song_SongType"].Value.ToString();
                                    SongVolume = row.Cells["Song_Volume"].Value.ToString();
                                    SongWordCount = row.Cells["Song_WordCount"].Value.ToString();
                                    SongPlayCount = row.Cells["Song_PlayCount"].Value.ToString();
                                    SongMB = row.Cells["Song_MB"].Value.ToString();
                                    SongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();
                                    SongFileName = row.Cells["Song_FileName"].Value.ToString();
                                    SongPath = row.Cells["Song_Path"].Value.ToString();
                                    SongSpell = row.Cells["Song_Spell"].Value.ToString();
                                    SongSpellNum = row.Cells["Song_SpellNum"].Value.ToString();
                                    SongSongStroke = row.Cells["Song_SongStroke"].Value.ToString();
                                    SongPenStyle = row.Cells["Song_PenStyle"].Value.ToString();
                                    SongPlayState = row.Cells["Song_PlayState"].Value.ToString();
                                    SongSrcPath = Path.Combine(SongPath, SongFileName);

                                    string SelectValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongSrcPath;
                                    Global.SongQueryDataGridViewSelectList.Add(SelectValue);
                                }
                            }
                            SongQuery_QueryStatus_Label.Text = "總共更新 " + Global.TotalList[0] + " 首歌曲,忽略重複歌曲 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首。";
                            Common_SwitchSetUI(true);
                        });
                        UpdateList.Clear();
                        SongQuery.DisposeSongDataTable();
                    });
                }
            }
        }

        private void SongQuery_InitializeEditControl()
        {
            SongQuery_EditSongId_TextBox.Text = "";
            SongQuery_EditSongLang_ComboBox.SelectedValue = 1;
            SongQuery_EditSongCreatDate_DateTimePicker.Value = DateTime.Now;
            SongQuery_EditSongSinger_TextBox.Text = "";
            SongQuery_EditSongSingerType_ComboBox.SelectedValue = 1;
            SongQuery_EditSongSongName_TextBox.Text = "";
            SongQuery_EditSongSongType_ComboBox.SelectedValue = 1;
            SongQuery_EditSongSpell_TextBox.Text = "";
            SongQuery_EditSongWordCount_TextBox.Text = "";
            SongQuery_EditSongSrcPath_TextBox.Text = "";
            SongQuery_EditSongTrack_ComboBox.SelectedValue = 1;
            SongQuery_EditSongVolume_TextBox.Text = "";
            SongQuery_EditSongPlayCount_TextBox.Text = "";

            SongQuery_EditSongId_TextBox.Enabled = false;
            SongQuery_EditSongLang_ComboBox.Enabled = false;
            SongQuery_EditSongCreatDate_DateTimePicker.Enabled = false;
            SongQuery_EditSongSinger_TextBox.Enabled = false;
            SongQuery_EditSongSingerType_ComboBox.Enabled = false;
            SongQuery_EditSongSongName_TextBox.Enabled = false;
            SongQuery_EditSongSongType_ComboBox.Enabled = false;
            SongQuery_EditSongSpell_TextBox.Enabled = false;
            SongQuery_EditSongWordCount_TextBox.Enabled = false;
            SongQuery_EditSongSrcPath_TextBox.Enabled = false;
            SongQuery_EditSongTrack_ComboBox.Enabled = false;
            SongQuery_EditSongTrack_Button.Enabled = false;
            SongQuery_EditSongVolume_TextBox.Enabled = false;
            SongQuery_EditSongPlayCount_TextBox.Enabled = false;
            SongQuery_EditApplyChanges_Button.Enabled = false;
        }

        private void SongQuery_GetSongEditComboBoxList(bool MultiEdit)
        {
            Global.SongQueryMultiEdit = MultiEdit;
            SongQuery_EditSongLang_ComboBox.DataSource = SongQuery.GetEditSongLangList(MultiEdit);
            SongQuery_EditSongLang_ComboBox.DisplayMember = "Display";
            SongQuery_EditSongLang_ComboBox.ValueMember = "Value";

            SongQuery_EditSongSingerType_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SingerType", MultiEdit, false);
            SongQuery_EditSongSingerType_ComboBox.DisplayMember = "Display";
            SongQuery_EditSongSingerType_ComboBox.ValueMember = "Value";

            SongQuery_EditSongSongType_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongType", MultiEdit, false);
            SongQuery_EditSongSongType_ComboBox.DisplayMember = "Display";
            SongQuery_EditSongSongType_ComboBox.ValueMember = "Value";

            SongQuery_EditSongTrack_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", MultiEdit, true);
            SongQuery_EditSongTrack_ComboBox.DisplayMember = "Display";
            SongQuery_EditSongTrack_ComboBox.ValueMember = "Value";
        }

        private void SongQuery_RefreshSongSrcPathTextBox()
        {
            string SongId = SongQuery_EditSongId_TextBox.Text;
            string SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
            string SongSingerTypeStr = CommonFunc.GetSingerTypeStr(Convert.ToInt32(SongQuery_EditSongSingerType_ComboBox.SelectedValue) - 1, 3, "null");
            int SongSingerType = Convert.ToInt32(CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr));
            string SongSinger = SongQuery_EditSongSinger_TextBox.Text;
            string SongSongName = SongQuery_EditSongSongName_TextBox.Text;
            int SongTrack = Convert.ToInt32(SongQuery_EditSongTrack_ComboBox.SelectedValue);
            string SongSongType = ((DataRowView)SongQuery_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
            string SongFileName = SongQuery_DataGridView.CurrentRow.Cells["Song_FileName"].Value.ToString();
            string SongPath = SongQuery_DataGridView.CurrentRow.Cells["Song_Path"].Value.ToString();

            SongQuery_EditSongSrcPath_TextBox.Text = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, (SongSongType != "無類別") ? SongSongType : "", SongFileName, SongPath, false, "", true, false);
        }





        #endregion

    }



    class SongQuery
    {
        public static bool QueryStatusLabel = true;
        public static bool QuerySameFileSong = false;

        #region --- SongQuery 建立資料表 ---

        public static List<string> SongDataIdList;
        public static List<string> SongDataList;
        public static List<string> SongDataLowCaseList;
        public static List<string> FavoriteDataIdList;

        public static void CreateSongDataTable()
        {
            SongDataIdList = new List<string>();
            SongDataList = new List<string>();
            SongDataLowCaseList = new List<string>();

            string SongDataSqlStr = "select Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState from ktv_Song order by Song_Id";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongDataSqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SongDataIdList.Add(row["Song_Id"].ToString());
                    SongDataList.Add(row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString() + "|" + row["Song_SongName"].ToString() + "|" + row["Song_SongType"].ToString());
                    SongDataLowCaseList.Add(row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString().ToLower() + "|" + row["Song_SongName"].ToString().ToLower() + "|" + row["Song_SongType"].ToString().ToLower());
                }
            }

            FavoriteDataIdList = new List<string>();
            string FavoriteSqlStr = "select User_Id, Song_Id from ktv_Favorite";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, FavoriteSqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    if (FavoriteDataIdList.IndexOf(row["Song_Id"].ToString()) < 0)
                    {
                        FavoriteDataIdList.Add(row["Song_Id"].ToString());
                    }
                }
            }
        }

        public static void DisposeSongDataTable()
        {
            SongDataIdList.Clear();
            SongDataList.Clear();
            SongDataLowCaseList.Clear();
            FavoriteDataIdList.Clear();
            GC.Collect();
        }

        #endregion

        #region --- SongQuery 歌曲查詢下拉清單 ---

        public static DataTable GetSongQueryTypeList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> ItemList = new List<string>() { "歌曲名稱", "歌手名稱", "歌曲編號", "全部歌曲", "新進歌曲", "合唱歌曲", "歌曲類別", "歌手類別", "歌曲聲道", "錢櫃編號" };

                foreach (string str in ItemList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                ItemList.Clear();
                return list;
            }
        }

        public static DataTable GetSongQueryValueList(string ValueType, bool MultiEdit, bool AutoDetectSongTrack)
        {
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

                switch (ValueType)
                {
                    case "SongType":
                        string str = "";
                        if (Global.SongMgrSongType != "") { str = "無類別," + Global.SongMgrSongType; } else { str = "無類別"; }

                        List<string> valuelist = new List<string>(str.Split(','));
                        foreach (string value in valuelist)
                        {
                            list.Rows.Add(list.NewRow());
                            list.Rows[list.Rows.Count - 1][0] = value;
                            list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                        }
                        valuelist.Clear();
                        break;
                    case "SingerType":
                        foreach (string SingerTypeStr in Global.CrazyktvSingerTypeList)
                        {
                            if (SingerTypeStr != "未使用")
                            {
                                list.Rows.Add(list.NewRow());
                                list.Rows[list.Rows.Count - 1][0] = SingerTypeStr;
                                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                            }
                        }
                        break;
                    case "SongTrack":
                        foreach (string value in Global.CrazyktvSongTrackWordList)
                        {
                            list.Rows.Add(list.NewRow());
                            list.Rows[list.Rows.Count - 1][0] = value;
                            list.Rows[list.Rows.Count - 1][1] = list.Rows.Count - 1;
                        }

                        if (AutoDetectSongTrack)
                        {
                            list.Rows.Add(list.NewRow());
                            list.Rows[list.Rows.Count - 1][0] = "自動偵測";
                            list.Rows[list.Rows.Count - 1][1] = list.Rows.Count - 1;
                        }
                        break;
                }
                return list;
            }
        }

        public static DataTable GetSongQueryFilterList(List<string> FilterList)
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                list.Rows.Add(list.NewRow());
                list.Rows[0][0] = "全部";
                list.Rows[0][1] = 1;

                foreach (string str in FilterList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
        }

        public static DataTable GetSongQueryExceptionalList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> ItemList = new List<string>() { "無檔案歌曲", "同檔案歌曲", "重複歌曲", "重複歌曲 (忽略歌曲類別)", "重複歌曲 (忽略括號及歌曲類別)", "重複歌曲 (忽略歌手及歌曲類別)", "重複歌曲 (合唱歌曲)", "檔名不符 (歌曲聲道)", "歌手未在資料庫", "歌手類別不符", "語系類別不符 (錢櫃)", "同名歌曲 (忽略括號及歌曲類別)" };

                foreach (string str in ItemList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                ItemList.Clear();
                return list;
            }
        }

        #endregion

        #region --- SongQuery 歌曲編輯下拉清單 ---

        public static DataTable GetEditSongLangList(bool MultiEdit)
        {
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

                foreach (string str in Global.CrazyktvSongLangList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
        }

        #endregion

        #region --- SongQuery 取得 SQL 查詢字串 ---

        public static string GetSongQuerySqlStr(string QueryType, string QueryValue, List<string> MultiQueryList)
        {
            string sqlCommonStr = " Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_SongType, Song_Track, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState, Song_ReplayGain, Song_MeanVolume ";
            string SongQuerySqlStr = string.Empty;
            string MultiQuerySqlStr = string.Empty;
            string SongQueryOrderStr = " order by Song_Id";
            string SongQueryFilterStr = (Global.SongQueryFilter == "全部") ? "" : " and Song_Lang = '" + Global.SongQueryFilter + "'";
            string SongQueryColumnName = string.Empty;

            List<string> QueryValueList = new List<string>();

            Regex HasWideChar = new Regex("[\x21-\x7E\xFF01-\xFF5E]");
            Regex HasSymbols = new Regex("[']");

            if (QueryType == "SongName" || QueryType == "SingerName")
            {
                switch (QueryType)
                {
                    case "SongName":
                        SongQueryColumnName = "Song_SongName";
                        break;
                    case "SingerName":
                        SongQueryColumnName = "Song_Singer";
                        break;
                }

                QueryValueList.Add(QueryValue);
                if (MultiQueryList != null) QueryValueList.AddRange(MultiQueryList);

                foreach (string value in QueryValueList)
                {
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        string QueryValueDefault = value;
                        if (HasWideChar.IsMatch(value))
                        {
                            string QueryValueNarrow = CommonFunc.ConvToNarrow(value);
                            string QueryValueWide = CommonFunc.ConvToWide(value);

                            if (HasSymbols.IsMatch(value))
                            {
                                QueryValueDefault = Regex.Replace(QueryValueDefault, "[']", delegate (Match match)
                                {
                                    string str = "'" + match.ToString();
                                    return str;
                                });
                            }

                            if (HasSymbols.IsMatch(QueryValueNarrow))
                            {
                                QueryValueNarrow = Regex.Replace(QueryValueNarrow, "[']", delegate (Match match)
                                {
                                    string str = "'" + match.ToString();
                                    return str;
                                });
                            }
                            if (value != QueryValue) MultiQuerySqlStr += " or InStr(1,LCase(" + SongQueryColumnName + "),LCase('" + QueryValueDefault + "'),0) <>0" + SongQueryFilterStr;
                            if (QueryValueDefault != QueryValueNarrow) MultiQuerySqlStr += " or InStr(1,LCase(" + SongQueryColumnName + "),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr;
                            MultiQuerySqlStr += " or InStr(1,LCase(" + SongQueryColumnName + "),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            if (HasSymbols.IsMatch(value))
                            {
                                QueryValueDefault = Regex.Replace(QueryValueDefault, "[']", delegate (Match match)
                                {
                                    string str = "'" + match.ToString();
                                    return str;
                                });
                            }
                            if (value != QueryValue) MultiQuerySqlStr += " or InStr(1,LCase(" + SongQueryColumnName + "),LCase('" + QueryValueDefault + "'),0) <>0" + SongQueryFilterStr;
                        }
                    }
                }
                QueryValueList.Clear();
                QueryValueList = null;

                if (HasSymbols.IsMatch(QueryValue))
                {
                    QueryValue = Regex.Replace(QueryValue, "[']", delegate (Match match)
                    {
                        string str = "'" + match.ToString();
                        return str;
                    });
                }
            }

            switch (QueryType)
            {
                case "SongName":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + MultiQuerySqlStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongName = '" + QueryValue + "'" + SongQueryFilterStr;
                    }
                    break;
                case "SingerName":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + MultiQuerySqlStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Singer = '" + QueryValue + "'" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('&" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "&'),0) <>0" + SongQueryFilterStr;
                    }
                    break;
                case "SongID":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Id like '%" + QueryValue + "%'" + SongQueryFilterStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Id = '" + QueryValue + "'";
                    }
                    break;
                case "AllSong":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Lang = '" + Global.SongQueryFilter + "'" + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song" + SongQueryOrderStr;
                    }
                    break;
                case "NewSong":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select top " + QueryValue + sqlCommonStr + "from ktv_Song where Song_Lang = '" + Global.SongQueryFilter + "' order by Song_CreatDate desc, Song_Id desc";
                    }
                    else
                    {
                        SongQuerySqlStr = "select top " + QueryValue + sqlCommonStr + "from ktv_Song order by Song_CreatDate desc, Song_Id desc";
                    }
                    break;
                case "ChorusSong":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = 3" + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = 3" + SongQueryOrderStr;
                    }
                    break;
                case "SongType":
                    if (QueryValue == "無類別") QueryValue = "";
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongType = '" + QueryValue + "'" + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongType = '" + QueryValue + "'" + SongQueryOrderStr;
                    }
                    break;
                case "SingerType":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = " + QueryValue + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType = " + QueryValue + SongQueryOrderStr;
                    }
                    break;
                case "SongTrack":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Track = " + QueryValue + SongQueryFilterStr + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Track = " + QueryValue + SongQueryOrderStr;
                    }
                    break;
                case "CashboxId":
                    if (Global.SongQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_Lang = '" + Global.SongQueryFilter + "'" + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song" + SongQueryOrderStr;
                    }
                    break;
                case "FileNotExists":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
                case "SameFileSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_FileName) In (select Song_FileName from ktv_Song As Tmp group by Song_FileName, Song_Path HAVING Count(*)>1 and Song_FileName = ktv_Song.Song_FileName and Song_Path = ktv_Song.Song_Path))) order by Song_FileName";
                    break;
                case "DuplicateSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_Singer, Song_SongType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_Singer = ktv_Song.Song_Singer and Song_SongType = ktv_Song.Song_SongType))) order by Song_Lang, Song_SongName";
                    break;
                case "DuplicateSongIgnorebrackets":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Lang, Song_SongName";
                    break;
                case "DuplicateSongIgnoreSinger":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang))) order by Song_Lang, Song_SongName";
                    break;
                case "DuplicateSongIgnoreSongType":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_Singer HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_Singer = ktv_Song.Song_Singer))) order by Song_Lang, Song_SongName";
                    break;
                case "DuplicateSongOnlyChorusSinger":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_SingerType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_SingerType = 3))) order by Song_Lang, Song_SongName";
                    break;
                case "MatchSongTrack":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
                case "MatchSingerData":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Singer";
                    break;
                case "MatchSingerType":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SingerType <> 3 order by Song_Singer";
                    break;
                case "MatchSongLang":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
                case "FavoriteSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
                case "SameNameSongIgnorebrackets":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Lang, Song_SongName";
                    break;
            }

            return SongQuerySqlStr;
        }

        #endregion

        #region --- SongQuery 歌曲列表欄位設定 ---

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            string ColumnWidth_120 = Convert.ToInt32((120 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_140 = Convert.ToInt32((140 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_160 = Convert.ToInt32((160 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_270 = Convert.ToInt32((270 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Song_Id":
                    list = new List<string>() { "歌曲編號", ColumnWidth_120, "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", ColumnWidth_120, "none" };
                    break;
                case "Song_SingerType":
                    list = new List<string>() { "歌手類別", ColumnWidth_120, "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", ColumnWidth_160, "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", ColumnWidth_270, "none" };
                    break;
                case "Song_Track":
                    list = new List<string>() { "歌曲聲道", ColumnWidth_140, "none" };
                    break;
                case "Song_SongType":
                    list = new List<string>() { "歌曲類別", ColumnWidth_120, "none" };
                    break;
                case "Song_Volume":
                    list = new List<string>() { "歌曲音量", ColumnWidth_120, "3" };
                    break;
                case "Song_WordCount":
                    list = new List<string>() { "歌曲字數", ColumnWidth_120, "2" };
                    break;
                case "Song_PlayCount":
                    list = new List<string>() { "點播次數", ColumnWidth_120, "9" };
                    break;
                case "Song_MB":
                    list = new List<string>() { "歌曲大小", ColumnWidth_120, "7" };
                    break;
                case "Song_CreatDate":
                    list = new List<string>() { "加歌日期", ColumnWidth_140, "none" };
                    break;
                case "Song_FileName":
                    list = new List<string>() { "檔案名稱", "0", "none" };
                    break;
                case "Song_Path":
                    list = new List<string>() { "歌曲路徑", "0", "none" };
                    break;
                case "Song_Spell":
                    list = new List<string>() { "歌曲拼音", "0", "none" };
                    break;
                case "Song_SpellNum":
                    list = new List<string>() { "手機輸入", "0", "none" };
                    break;
                case "Song_SongStroke":
                    list = new List<string>() { "歌曲筆劃", "0", "none" };
                    break;
                case "Song_PenStyle":
                    list = new List<string>() { "筆形順序", "0", "none" };
                    break;
                case "Song_PlayState":
                    list = new List<string>() { "播放狀態", "0", "none" };
                    break;
                case "Song_SrcPath":
                    list = new List<string>() { "來源檔案路徑", "0", "none" };
                    break;
                case "Song_SortIndex":
                    list = new List<string>() { "排序索引", "0", "none" };
                    break;
                case "Song_AddStatus":
                    list = new List<string>() { "加歌狀況", "0", "none" };
                    break;
                case "Song_ReplayGain":
                    list = new List<string>() { "播放增益", "0", "none" };
                    break;
                case "Song_MeanVolume":
                    list = new List<string>() { "平均音量", "0", "none" };
                    break;
            }
            return list;
        }

        #endregion

        #region --- SongQuery 取得歌曲類別字串 ---

        public static string GetSongTypeStr(int SongType)
        {
            List<string> list = new List<string>(Global.SongMgrSongType.Split(','));
            string Str = list[SongType];
            list.Clear();
            return Str;
        }

        #endregion

        #region --- SongQuery 取得右鍵功能表字串 ---

        public static string GetContextMenuStr(int ContextMenu, int ListType, bool ReturnCount)
        {
            List<string> list;

            switch (ListType)
            {
                case 1:
                    list = new List<string>() { "開啟資料夾", "播放檔案", "刪除資料列" };
                    break;
                case 2:
                    list = new List<string>() { "開啟資料夾", "播放檔案", "查詢此歌手所有歌曲" };
                    break;
                case 3:
                    list = new List<string>() { "開啟資料夾", "播放檔案", "查詢此歌手所有歌曲", "從我的最愛移除" };
                    break;
                default:
                    list = new List<string>() { "刪除資料列" };
                    break;
            }
            string Str = list[ContextMenu];
            if (ReturnCount) Str = list.Count.ToString();
            list.Clear();
            return Str;
        }

        #endregion

        #region --- SongQuery 取得下個歌曲編號 ---

        public static string GetNextSongId(string SongLang, bool UpdateIdList)
        {
            string NewSongID = "";

            // 查詢歌曲編號有無斷號
            if (Global.UnusedSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Count > 0)
            {
                NewSongID = Global.UnusedSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)][0];
                if (UpdateIdList) Global.UnusedSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Remove(NewSongID);
            }

            // 若無斷號查詢各語系下個歌曲編號
            if (NewSongID == "")
            {
                string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";
                Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]++;
                NewSongID = Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)].ToString(MaxDigitCode);
                if (!UpdateIdList) Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]--;
            }
            return NewSongID;
        }

        #endregion

    }
}
