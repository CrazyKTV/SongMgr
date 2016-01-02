using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
        #region --- Cashbox 控制項事件 ---

        private void Cashbox_QueryType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Cashbox_QueryType_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    Cashbox_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    Cashbox_QueryValue_TextBox.Text = "";
                    Cashbox_QueryValue_TextBox.Enabled = true;
                    Cashbox_QueryValue_ComboBox.Visible = false;
                    Cashbox_QueryValue_TextBox.Visible = true;
                    Cashbox_Paste_Button.Enabled = true;
                    Cashbox_Clear_Button.Enabled = true;
                    Cashbox_QueryValue_TextBox.Focus();
                    break;
                case "2":
                    Cashbox_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    Cashbox_QueryValue_TextBox.Text = "";
                    Cashbox_QueryValue_TextBox.Enabled = true;
                    Cashbox_QueryValue_ComboBox.Visible = false;
                    Cashbox_QueryValue_TextBox.Visible = true;
                    Cashbox_Paste_Button.Enabled = true;
                    Cashbox_Clear_Button.Enabled = true;
                    Cashbox_QueryValue_TextBox.Focus();
                    break;
                case "3":
                    Cashbox_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    Cashbox_QueryValue_TextBox.Text = "";
                    Cashbox_QueryValue_TextBox.Enabled = true;
                    Cashbox_QueryValue_ComboBox.Visible = false;
                    Cashbox_QueryValue_TextBox.Visible = true;
                    Cashbox_Paste_Button.Enabled = false;
                    Cashbox_Clear_Button.Enabled = false;
                    Cashbox_QueryValue_TextBox.Focus();
                    break;
                case "4":
                    Cashbox_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    Cashbox_QueryValue_TextBox.Text = "100";
                    Cashbox_QueryValue_TextBox.Enabled = true;
                    Cashbox_QueryValue_ComboBox.Visible = false;
                    Cashbox_QueryValue_TextBox.Visible = true;
                    Cashbox_Paste_Button.Enabled = false;
                    Cashbox_Clear_Button.Enabled = false;
                    Cashbox_Query_Button_Click(new Button(), new EventArgs());
                    Cashbox_DataGridView.Focus();
                    break;
            }
        }

        private void Cashbox_QueryFilter_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Cashbox_QueryFilter_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                Global.CashboxQueryFilter = Cashbox_QueryFilter_ComboBox.Text;
            }
        }

        private void Cashbox_QueryValue_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Cashbox_QueryType_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                    break;
                case "3":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13)
                    {
                        e.Handled = true;
                    }
                    break;
                case "4":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13)
                    {
                        e.Handled = true;
                    }
                    break;
            }
            if ((int)e.KeyChar == 13)
            {
                Cashbox_Query_Button_Click(new Button(), new EventArgs());
            }
        }

        private void Cashbox_Paste_Button_Click(object sender, EventArgs e)
        {
            Cashbox_QueryValue_TextBox.Text = Clipboard.GetText();
        }

        private void Cashbox_Clear_Button_Click(object sender, EventArgs e)
        {
            Cashbox_QueryValue_TextBox.Text = "";
        }

        private void Cashbox_SynonymousQuery_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.CashboxSynonymousQuery = Cashbox_SynonymousQuery_CheckBox.Checked;
        }

        private void Cashbox_FuzzyQuery_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.CashboxFuzzyQuery = Cashbox_FuzzyQuery_CheckBox.Checked;
        }

        #endregion

        #region --- Cashbox 查詢歌曲 ---

        private void Cashbox_Query_Button_Click(object sender, EventArgs e)
        {
            Cashbox_DataGridView.DataSource = null;
            GC.Collect();

            Cashbox_QueryStatus_Label.Text = "";
            string SongQueryStatusText = "";
            string SongQueryValue = "";

            if (Global.CrazyktvDatabaseStatus)
            {
                string SongQueryType = "None";
                switch (Cashbox_QueryType_ComboBox.SelectedValue.ToString())
                {
                    case "1":
                        SongQueryType = "SongName";
                        SongQueryStatusText = Cashbox_QueryValue_TextBox.Text;
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        break;
                    case "2":
                        SongQueryType = "SingerName";
                        SongQueryStatusText = Cashbox_QueryValue_TextBox.Text;
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        break;
                    case "3":
                        SongQueryType = "SongID";
                        SongQueryStatusText = "歌曲編號中包含 " + Cashbox_QueryValue_TextBox.Text;
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        break;
                    case "4":
                        SongQueryType = "NewSong";
                        SongQueryStatusText = "新進歌曲";
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        break;
                }

                if (SongQueryValue == "")
                {
                    Cashbox_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                }
                else
                {
                    try
                    {
                        DataTable dt = new DataTable();
                        switch (SongQueryType)
                        {
                            case "SongName":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

                                if (Global.CashboxHasWideChar)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    var query = from row in dt.AsEnumerable()
                                                where !CommonFunc.ConvToNarrow(row.Field<string>("Song_SongName")).ToLower().Contains(CommonFunc.ConvToNarrow(SongQueryValue).ToLower())
                                                select row;

                                    if (query.Count<DataRow>() > 0)
                                    {
                                        foreach (DataRow row in query)
                                        {
                                            RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        }

                                        if (RemoveRowsIdxlist.Count > 0)
                                        {
                                            for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                            {
                                                dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                            }
                                        }
                                    }
                                }

                                if (Global.CashboxSynonymousQuery)
                                {
                                    List<string> SynonymousSongNameList = new List<string>();
                                    SynonymousSongNameList = CommonFunc.GetSynonymousSongNameList(SongQueryValue);

                                    if (SynonymousSongNameList.Count > 0)
                                    {
                                        DataTable SynonymousSongDT = new DataTable();
                                        foreach (string SynonymousSongName in SynonymousSongNameList)
                                        {
                                            SynonymousSongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SynonymousSongName), "");
                                            foreach (DataRow row in SynonymousSongDT.Rows)
                                            {
                                                dt.ImportRow(row);
                                            }
                                        }
                                        SynonymousSongDT.Dispose();
                                        SynonymousSongDT = null;
                                    }
                                }
                                break;
                            case "SingerName":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

                                if (Global.CashboxHasWideChar)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    var query = from row in dt.AsEnumerable()
                                                where !CommonFunc.ConvToNarrow(row.Field<string>("Song_Singer")).ToLower().Contains(CommonFunc.ConvToNarrow(SongQueryValue).ToLower())
                                                select row;

                                    if (query.Count<DataRow>() > 0)
                                    {
                                        foreach (DataRow row in query)
                                        {
                                            RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        }

                                        if (RemoveRowsIdxlist.Count > 0)
                                        {
                                            for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                            {
                                                dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                                break;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            Cashbox_EditMode_CheckBox.Enabled = false;
                            Cashbox_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                        }
                        else
                        {
                            if (SongQueryType == "SingerName" && !Global.CashboxFuzzyQuery)
                            {
                                var query = from row in dt.AsEnumerable()
                                            where row.Field<string>("Song_Singer") != Cashbox_QueryValue_TextBox.Text
                                            select row;
                                if (query.Count<DataRow>() > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");

                                    foreach (DataRow row in query)
                                    {
                                        if (r.IsMatch(row["Song_Singer"].ToString()))
                                        {
                                            string RemoveThisRow = "True";
                                            string[] singers = Regex.Split(row["Song_Singer"].ToString(), "&", RegexOptions.None);
                                            foreach (string str in singers)
                                            {
                                                if (str == Cashbox_QueryValue_TextBox.Text) { RemoveThisRow = "False"; }
                                            }
                                            if (RemoveThisRow == "True") RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        }
                                    }

                                    if (RemoveRowsIdxlist.Count > 0)
                                    {
                                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                                        {
                                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                                        }
                                    }
                                }
                            }

                            if (dt.Rows.Count == 0)
                            {
                                Cashbox_EditMode_CheckBox.Enabled = false;
                                Cashbox_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                            }
                            else
                            {
                                Cashbox_EditMode_CheckBox.Enabled = true;
                                Cashbox_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的歌曲。";

                                Cashbox_DataGridView.DataSource = dt;

                                for (int i = 0; i < Cashbox_DataGridView.ColumnCount; i++)
                                {
                                    List<string> DataGridViewColumnName = Cashbox.GetDataGridViewColumnSet(Cashbox_DataGridView.Columns[i].Name);
                                    Cashbox_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                    if (DataGridViewColumnName[1].ToString() == "0")
                                    {
                                        Cashbox_DataGridView.Columns[i].Visible = false;
                                    }

                                    if (DataGridViewColumnName[2].ToString() != "none")
                                    {
                                        ((DataGridViewTextBoxColumn)Cashbox_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                    }

                                    Cashbox_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                }

                                Cashbox_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                                Cashbox_DataGridView.Focus();
                            }
                        }
                        dt.Dispose();
                        dt = null;
                    }
                    catch
                    {
                        Cashbox_EditMode_CheckBox.Enabled = false;
                        Cashbox_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                    }
                }
            }
        }

        #endregion

        #region --- Cashbox 其它查詢 ---

        private void Cashbox_OtherQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus && Cashbox_OtherQuery_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                Cashbox_Query_Button.Enabled = false;
                Cashbox.CreateSongDataTable();
                Common_SwitchSetUI(false);

                Cashbox_DataGridView.DataSource = null;
                Cashbox_QueryStatus_Label.Text = "";
                GC.Collect();

                string SongQueryType = "None";
                string SongQueryValue = "";
                string SongQueryStatusText = "";

                switch (Cashbox_OtherQuery_ComboBox.SelectedValue.ToString())
                {
                    case "1":
                        SongQueryType = "NonSong";
                        SongQueryValue = "NA";
                        SongQueryStatusText = Cashbox_OtherQuery_ComboBox.Text;
                        Cashbox_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";

                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => Cashbox_OtherQueryTask(SongQueryType, SongQueryValue, SongQueryStatusText)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                Common_SwitchSetUI(true);
                                Cashbox_Query_Button.Enabled = true;
                            });
                            Cashbox.DisposeSongDataTable();
                        });
                        break;
                }
            }
        }

        private void Cashbox_OtherQueryTask(string SongQueryType, string SongQueryValue, string SongQueryStatusText)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                if (SongQueryValue == "")
                {
                    this.BeginInvoke((Action)delegate ()
                    {
                        Cashbox_QueryStatus_Label.Text = "必須輸入查詢條件才能查詢...";
                    });
                }
                else
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        switch (SongQueryType)
                        {
                            case "NonSong":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    Parallel.ForEach(Global.CashboxSongLangList, (langstr, loopState) =>
                                    {
                                        var query = from row in dt.AsEnumerable()
                                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                                    select row;

                                        if (query.Count<DataRow>() > 0)
                                        {
                                            foreach (DataRow row in query)
                                            {
                                                string SongData = row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString().ToLower() + "|" + row["Song_SongName"].ToString().ToLower();

                                                if (Cashbox.SongDataList.IndexOf(SongData) >= 0)
                                                {
                                                    lock (LockThis)
                                                    {
                                                        RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                                    }

                                                    this.BeginInvoke((Action)delegate ()
                                                    {
                                                        Cashbox_QueryStatus_Label.Text = "已在歌庫找到 " + RemoveRowsIdxlist.Count + " 首錢櫃歌曲...";
                                                    });
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
                        }

                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                Cashbox_EditMode_CheckBox.Enabled = false;
                                Cashbox_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                            });
                        }
                        else
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                Cashbox_EditMode_CheckBox.Enabled = true;
                                Cashbox_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的歌曲。";

                                Cashbox_DataGridView.DataSource = dt;

                                for (int i = 0; i < Cashbox_DataGridView.ColumnCount; i++)
                                {
                                    List<string> DataGridViewColumnName = Cashbox.GetDataGridViewColumnSet(Cashbox_DataGridView.Columns[i].Name);
                                    Cashbox_DataGridView.Columns[i].HeaderText = DataGridViewColumnName[0];

                                    if (DataGridViewColumnName[1].ToString() == "0")
                                    {
                                        Cashbox_DataGridView.Columns[i].Visible = false;
                                    }

                                    if (DataGridViewColumnName[2].ToString() != "none")
                                    {
                                        ((DataGridViewTextBoxColumn)Cashbox_DataGridView.Columns[i]).MaxInputLength = int.Parse(DataGridViewColumnName[2]);
                                    }
                                    Cashbox_DataGridView.Columns[i].Width = int.Parse(DataGridViewColumnName[1]);
                                }
                                Cashbox_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                                Cashbox_DataGridView.Focus();

                                dt.Dispose();
                                dt = null;
                            });
                        }
                    }
                    catch
                    {
                        this.BeginInvoke((Action)delegate ()
                        {
                            Cashbox_EditMode_CheckBox.Enabled = false;
                            Cashbox_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        });
                    }
                }
            }
        }

        #endregion

    }


    class Cashbox
    {
        /// <summary>
        /// [Song_Lang] [Song_Singer] [Song_SongName]
        /// </summary>
        public static List<string> SongDataList;

        #region --- Cashbox 建立資料表 ---

        public static void CreateSongDataTable()
        {
            SongDataList = new List<string>();

            string SongQuerySqlStr = "select Song_Lang, Song_Singer, Song_SongName from ktv_Song";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SongDataList.Add(row["Song_Lang"].ToString() + "|" + row["Song_Singer"].ToString().ToLower() + "|" + row["Song_SongName"].ToString().ToLower());
                }
            }
        }

        public static void DisposeSongDataTable()
        {
            SongDataList.Clear();
            GC.Collect();
        }

        #endregion

        #region --- Cashbox 歌曲查詢下拉清單 ---

        public static DataTable GetQueryTypeList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                List<string> ItemList = new List<string>() { "歌曲名稱", "歌手名稱", "歌曲編號", "新進歌曲" };

                foreach (string str in ItemList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
        }

        public static DataTable GetQueryFilterList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                list.Rows.Add(list.NewRow());
                list.Rows[0][0] = "全部";
                list.Rows[0][1] = 1;

                foreach (string str in Global.CashboxSongLangList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
        }

        public static DataTable GetOtherQueryList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                List<string> ItemList = new List<string>() { "錢櫃缺歌" };

                foreach (string str in ItemList)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = str;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
                return list;
            }
        }

        #endregion

        #region --- Cashbox 取得 SQL 查詢字串 ---

        public static string GetSongQuerySqlStr(string QueryType, string QueryValue)
        {
            string sqlCommonStr = " Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate ";
            string SongQuerySqlStr = "";
            string SongQueryOrderStr = " order by Cashbox_Id";
            string SongQueryFilterStr = "";
            string QueryValueNarrow = QueryValue;
            string QueryValueWide = QueryValue;
            string HasWideCharQueryValue = QueryValue;

            Global.CashboxHasWideChar = false;

            Regex HasWideChar = new Regex("[\x21-\x7E\xFF01-\xFF5E]");
            if (QueryType == "SongName" | QueryType == "SingerName")
            {
                if (Global.CashboxFuzzyQuery)
                {
                    if (HasWideChar.IsMatch(HasWideCharQueryValue))
                    {
                        Global.CashboxHasWideChar = true;
                        QueryValueNarrow = CommonFunc.ConvToNarrow(QueryValue);
                        QueryValueWide = CommonFunc.ConvToWide(QueryValue);
                        HasWideCharQueryValue = Regex.Replace(HasWideCharQueryValue, "[\x21-\x7E\xFF01-\xFF5E]", "", RegexOptions.IgnoreCase);
                        if (HasWideCharQueryValue == "") HasWideCharQueryValue = QueryValue;
                    }
                }

                Regex HasSymbols = new Regex("[']");
                if (HasSymbols.IsMatch(QueryValue))
                {
                    QueryValue = Regex.Replace(QueryValue, "[']", delegate (Match match)
                    {
                        string str = "' + \"" + match.ToString() + "\" + '";
                        return str;
                    });
                }

                if (HasSymbols.IsMatch(QueryValueNarrow))
                {
                    QueryValueNarrow = Regex.Replace(QueryValueNarrow, "[']", delegate (Match match)
                    {
                        string str = "' + \"" + match.ToString() + "\" + '";
                        return str;
                    });
                }
            }

            if (Global.CashboxQueryFilter != "全部")
            {
                SongQueryFilterStr = " and Song_Lang = '" + Global.CashboxQueryFilter + "'";
            }

            switch (QueryType)
            {
                case "SongName":
                    if (Global.CashboxFuzzyQuery)
                    {
                        if (Global.CashboxHasWideChar)
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + HasWideCharQueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where Song_SongName = '" + QueryValue + "'" + SongQueryFilterStr;
                    }
                    break;
                case "SingerName":
                    if (Global.CashboxFuzzyQuery)
                    {
                        if (Global.CashboxHasWideChar)
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + HasWideCharQueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where Song_Singer = '" + QueryValue + "'" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('&" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "&'),0) <>0" + SongQueryFilterStr;
                    }
                    break;
                case "SongID":
                    if (Global.CashboxFuzzyQuery)
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where Cashbox_Id like '%" + QueryValue + "%'" + SongQueryFilterStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where Cashbox_Id = '" + QueryValue + "'";
                    }
                    break;
                case "NewSong":
                    if (Global.CashboxQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select top " + QueryValue + sqlCommonStr + "from ktv_Cashbox where Song_Lang = '" + Global.CashboxQueryFilter + "' order by Song_CreatDate desc, Cashbox_Id desc";
                    }
                    else
                    {
                        SongQuerySqlStr = "select top " + QueryValue + sqlCommonStr + "from ktv_Cashbox order by Song_CreatDate desc, Cashbox_Id desc";
                    }
                    break;
                case "NonSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox" + SongQueryOrderStr;
                    break;
            }

            return SongQuerySqlStr;
        }

        #endregion

        #region --- Cashbox 歌曲列表欄位設定 ---

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Cashbox_Id":
                    list = new List<string>() { "歌曲編號", "120", "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", "120", "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", "190", "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", "320", "none" };
                    break;
                case "Song_CreatDate":
                    list = new List<string>() { "更新日期", "140", "none" };
                    break;
            }
            return list;
        }

        #endregion




    }
}
