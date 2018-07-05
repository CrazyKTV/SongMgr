using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
                    Cashbox_QueryValue_TextBox.Text = "*";
                    Cashbox_QueryValue_TextBox.Enabled = false;
                    Cashbox_QueryValue_ComboBox.Visible = false;
                    Cashbox_QueryValue_TextBox.Visible = true;
                    Cashbox_Paste_Button.Enabled = false;
                    Cashbox_Clear_Button.Enabled = false;
                    Cashbox_Query_Button_Click(new Button(), new EventArgs());
                    Cashbox_DataGridView.Focus();
                    break;
                case "5":
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

        private void Cashbox_CommonFilter_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Focused)
            {
                if (((ComboBox)sender).Name == "Cashbox_LangFilter_ComboBox")
                {
                    List<string> SongSingerFilterList = new List<string>();
                    ((DataTable)Cashbox_DataGridView.DataSource).DefaultView.RowFilter = "";

                    using (DataTable dt = (DataTable)Cashbox_DataGridView.DataSource)
                    {
                        using (DataTable SongSingerDistinctDT = dt.DefaultView.ToTable(true, "Song_Lang", "Song_Singer"))
                        {
                            if (SongSingerDistinctDT.Rows.Count > 0)
                            {
                                string SongSinger = string.Empty;
                                foreach (DataRow row in SongSingerDistinctDT.AsEnumerable())
                                {
                                    SongSinger = row["Song_Singer"].ToString();
                                    if (SongSinger.Contains("&"))
                                    {
                                        string ChorusSongSingerName = SongSinger;
                                        List<string> list = CommonFunc.GetChorusSingerList(ChorusSongSingerName);
                                        foreach (string str in list)
                                        {
                                            string SingerStr = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                            if (SongSingerFilterList.IndexOf(SingerStr) < 0)
                                            {
                                                if (row["Song_Lang"].ToString() == Cashbox_LangFilter_ComboBox.Text || Cashbox_LangFilter_ComboBox.Text == "全部")
                                                {
                                                    SongSingerFilterList.Add(SingerStr);
                                                }
                                            }
                                        }
                                        list.Clear();
                                        list = null;
                                    }
                                    else
                                    {
                                        if (SongSingerFilterList.IndexOf(SongSinger) < 0)
                                        {
                                            if (row["Song_Lang"].ToString() == Cashbox_LangFilter_ComboBox.Text || Cashbox_LangFilter_ComboBox.Text == "全部")
                                            {
                                                SongSingerFilterList.Add(SongSinger);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (SongSingerFilterList.Count > 0)
                    {
                        SongSingerFilterList.Sort();
                        Cashbox_SongSingerFilter_ComboBox.Enabled = true;
                        Cashbox_SongSingerFilter_ComboBox.DataSource = Cashbox.GetQueryFilterList(SongSingerFilterList);
                        Cashbox_SongSingerFilter_ComboBox.DisplayMember = "Display";
                        Cashbox_SongSingerFilter_ComboBox.ValueMember = "Value";
                        SongSingerFilterList.Clear();
                    }
                }

                string FilterStr = "";
                if (Cashbox_LangFilter_ComboBox.Text != "全部" && Cashbox_LangFilter_ComboBox.Text != "") FilterStr = "Song_Lang = '" + Cashbox_LangFilter_ComboBox.Text + "'";
                if (Cashbox_SongSingerFilter_ComboBox.Text != "全部" && Cashbox_SongSingerFilter_ComboBox.Text != "")
                {
                    string SongSinger = Cashbox_SongSingerFilter_ComboBox.Text;
                    SongSinger = Regex.Replace(SongSinger, @"(%)|(_)|(\[)|(])|(\^)", "[$1$2$3$4$5]");

                    FilterStr += (FilterStr != "") ? " and (" : "";
                    FilterStr += "Song_Singer = '" + SongSinger + "' or Song_Singer like '%&" + SongSinger + "%' or Song_Singer like '%" + SongSinger + "&%'" + ((FilterStr != "") ? ")" : "");
                }
                ((DataTable)Cashbox_DataGridView.DataSource).DefaultView.RowFilter = FilterStr;
            }
        }

        #endregion

        #region --- Cashbox 查詢歌曲 ---

        private void Cashbox_Query_Button_Click(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus)
            {
                Cashbox_Query_Button.Enabled = false;
                Common_SwitchSetUI(false);

                Cashbox_DataGridView.DataSource = null;
                Cashbox_QueryStatus_Label.Text = "";
                GC.Collect();

                string SongQueryType = "None";
                string SongQueryValue = "";
                string SongQueryStatusText = "";

                var tasks = new List<Task>();

                switch (Cashbox_QueryType_ComboBox.SelectedValue.ToString())
                {
                    case "1":
                        SongQueryType = "SongName";
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        SongQueryStatusText = Cashbox_QueryValue_TextBox.Text;
                        Cashbox_QueryStatus_Label.Text = "正在查詢歌曲名稱為『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "2":
                        SongQueryType = "SingerName";
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        SongQueryStatusText = Cashbox_QueryValue_TextBox.Text;
                        Cashbox_QueryStatus_Label.Text = "正在查詢歌手名稱為『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "3":
                        SongQueryType = "SongID";
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        SongQueryStatusText = "歌曲編號中包含 " + Cashbox_QueryValue_TextBox.Text;
                        Cashbox_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "4":
                        SongQueryType = "AllSong";
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        SongQueryStatusText = "全部歌曲";
                        Cashbox_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                    case "5":
                        SongQueryType = "NewSong";
                        SongQueryValue = Cashbox_QueryValue_TextBox.Text;
                        SongQueryStatusText = "新進歌曲";
                        Cashbox_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                }
                tasks.Add(Task.Factory.StartNew(() => Cashbox_QueryTask(SongQueryType, SongQueryValue, SongQueryStatusText)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        Cashbox_Query_Button.Enabled = true;
                    });
                });
            }
        }

        private void Cashbox_QueryTask(string SongQueryType, string SongQueryValue, string SongQueryStatusText)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                if (SongQueryValue == "")
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Cashbox_EditMode_CheckBox.Enabled = false;
                        Cashbox_LangFilter_ComboBox.Enabled = false;
                        Cashbox_SongSingerFilter_ComboBox.Enabled = false;
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

                                if (Global.GroupSingerLowCaseList.IndexOf(SongQueryValue.ToLower()) >= 0)
                                {
                                    List<string> dtSongIdList = new List<string>();
                                    foreach (DataRow row in dt.AsEnumerable())
                                    {
                                        dtSongIdList.Add(row["Cashbox_Id"].ToString());
                                    }

                                    int i = Global.GroupSingerIdList[Global.GroupSingerLowCaseList.IndexOf(SongQueryValue.ToLower())];
                                    List<string> list = new List<string>(Global.SingerGroupList[i].Split(','));
                                    if (list.Count > 0)
                                    {
                                        foreach (string GroupSingerName in list)
                                        {
                                            if (GroupSingerName.ToLower() != SongQueryValue.ToLower())
                                            {
                                                using (DataTable SingerGroupDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, GroupSingerName), ""))
                                                {
                                                    foreach (DataRow row in SingerGroupDT.Rows)
                                                    {
                                                        if (dtSongIdList.IndexOf(row["Cashbox_Id"].ToString()) < 0)
                                                        {
                                                            dtSongIdList.Add(row["Cashbox_Id"].ToString());
                                                            dt.ImportRow(row);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    dtSongIdList.Clear();
                                }
                                break;
                            default:
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                                break;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Cashbox_EditMode_CheckBox.Enabled = false;
                                Cashbox_LangFilter_ComboBox.Enabled = false;
                                Cashbox_SongSingerFilter_ComboBox.Enabled = false;
                                Cashbox_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                            });
                        }
                        else
                        {
                            if (SongQueryType == "SingerName" && !Global.CashboxFuzzyQuery)
                            {
                                var query = from row in dt.AsEnumerable()
                                            where row.Field<string>("Song_Singer") != SongQueryValue
                                            select row;

                                if (query.Count<DataRow>() > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    foreach (DataRow row in query)
                                    {
                                        string RemoveThisRow = "True";

                                        List<string> list = CommonFunc.GetChorusSingerList(row["Song_Singer"].ToString());
                                        foreach (string str in list)
                                        {
                                            if (str.ToLower() == SongQueryValue.ToLower()) { RemoveThisRow = "False"; }
                                        }
                                        list.Clear();
                                        list = null;
                                        if (RemoveThisRow == "True") RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
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
                                    Cashbox_EditMode_CheckBox.Enabled = false;
                                    Cashbox_LangFilter_ComboBox.Enabled = false;
                                    Cashbox_SongSingerFilter_ComboBox.Enabled = false;
                                    Cashbox_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                                });
                            }
                            else
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    Cashbox_EditMode_CheckBox.Enabled = (dt.Rows.Count > 0) ? true : false;
                                    Cashbox_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的歌曲。";

                                    Cashbox_DataGridView.DataSource = dt;

                                    List<string> LangFilterList = new List<string>();
                                    List<string> SongSingerFilterList = new List<string>();

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

                                    using (DataTable SongSingerDistinctDT = dt.DefaultView.ToTable(true, "Song_Singer"))
                                    {
                                        if (SongSingerDistinctDT.Rows.Count > 0)
                                        {
                                            string SongSinger = string.Empty;
                                            foreach (DataRow row in SongSingerDistinctDT.AsEnumerable())
                                            {
                                                SongSinger = row["Song_Singer"].ToString();
                                                
                                                if (SongSinger.Contains("&"))
                                                {
                                                    string ChorusSongSingerName = SongSinger;

                                                    List<string> list = CommonFunc.GetChorusSingerList(ChorusSongSingerName);
                                                    foreach (string str in list)
                                                    {
                                                        string SingerStr = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                                        if (SongSingerFilterList.IndexOf(SingerStr) < 0)
                                                        {
                                                            SongSingerFilterList.Add(SingerStr);
                                                        }
                                                    }
                                                    list.Clear();
                                                    list = null;
                                                }
                                                else
                                                {
                                                    if (SongSingerFilterList.IndexOf(SongSinger) < 0)
                                                    {
                                                        SongSingerFilterList.Add(SongSinger);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (LangFilterList.Count > 0)
                                    {
                                        Cashbox_LangFilter_ComboBox.Enabled = true;
                                        Cashbox_LangFilter_ComboBox.DataSource = Cashbox.GetQueryFilterList(LangFilterList);
                                        Cashbox_LangFilter_ComboBox.DisplayMember = "Display";
                                        Cashbox_LangFilter_ComboBox.ValueMember = "Value";
                                        LangFilterList.Clear();
                                    }

                                    if (SongSingerFilterList.Count > 0)
                                    {
                                        SongSingerFilterList.Sort();
                                        Cashbox_SongSingerFilter_ComboBox.Enabled = true;
                                        Cashbox_SongSingerFilter_ComboBox.DataSource = Cashbox.GetQueryFilterList(SongSingerFilterList);
                                        Cashbox_SongSingerFilter_ComboBox.DisplayMember = "Display";
                                        Cashbox_SongSingerFilter_ComboBox.ValueMember = "Value";
                                        SongSingerFilterList.Clear();
                                    }

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

                                    int ColumnWidth_140 = Convert.ToInt32(140 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor);
                                    Cashbox_DataGridView.Columns["Song_CreatDate"].MinimumWidth = ColumnWidth_140;
                                    Cashbox_DataGridView.Columns["Song_CreatDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                    Cashbox_DataGridView.Focus();

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
                            Cashbox_EditMode_CheckBox.Enabled = false;
                            Cashbox_LangFilter_ComboBox.Enabled = false;
                            Cashbox_SongSingerFilter_ComboBox.Enabled = false;
                            Cashbox_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        });
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
                        break;
                    case "2":
                        SongQueryType = "DuplicateSong";
                        SongQueryValue = "NA";
                        SongQueryStatusText = Cashbox_OtherQuery_ComboBox.Text;
                        Cashbox_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                }

                Cashbox_Query_Button.Enabled = false;
                Cashbox.CreateSongDataTable();
                Common_SwitchSetUI(false);

                Cashbox_DataGridView.DataSource = null;
                Cashbox_QueryStatus_Label.Text = "";
                GC.Collect();

                var tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => Cashbox_OtherQueryTask(SongQueryType, SongQueryValue, SongQueryStatusText))
                };

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        Cashbox_Query_Button.Enabled = true;
                    });
                    Cashbox.DisposeSongDataTable();
                });
            }
        }

        private void Cashbox_DateQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus && Cashbox_DateQuery_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                string SongQueryType = "SongDate";
                string SongQueryValue = DateTime.Parse(Cashbox_DateQuery_ComboBox.Text).ToShortDateString();
                string SongQueryStatusText = Cashbox_DateQuery_ComboBox.Text;

                Cashbox_QueryStatus_Label.Text = "正在查詢更新日期為『" + SongQueryStatusText + "』的歌曲,請稍待...";

                Cashbox_Query_Button.Enabled = false;
                Cashbox.CreateSongDataTable();
                Common_SwitchSetUI(false);

                Cashbox_DataGridView.DataSource = null;
                Cashbox_QueryStatus_Label.Text = "";
                GC.Collect();

                var tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => Cashbox_OtherQueryTask(SongQueryType, SongQueryValue, SongQueryStatusText))
                };

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        Cashbox_Query_Button.Enabled = true;
                    });
                    Cashbox.DisposeSongDataTable();
                });
            }
        }

        private void Cashbox_OtherQueryTask(string SongQueryType, string SongQueryValue, string SongQueryStatusText)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                if (SongQueryValue == "")
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Cashbox_EditMode_CheckBox.Enabled = false;
                        Cashbox_LangFilter_ComboBox.Enabled = false;
                        Cashbox_SongSingerFilter_ComboBox.Enabled = false;
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
                                                bool RemoveRowStatus = (CommonFunc.MatchCashboxSong("CashboxSong", row, "", "", "", Cashbox.SongDataLowCaseList, Cashbox.SongDataFuzzyList) >= 0) ? true : false;

                                                if (RemoveRowStatus)
                                                {
                                                    lock (LockThis) RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                                    this.BeginInvoke((Action)delegate()
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
                            case "SongDate":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

                                string DateStr = DateTime.Parse(SongQueryValue).ToString("yyyy/MM/dd");

                                if (dt.Rows.Count > 0)
                                {
                                    List<int> RemoveRowsIdxlist = new List<int>();

                                    var query = from row in dt.AsEnumerable()
                                                where !row.Field<DateTime>("Song_CreatDate").ToString("yyyy/MM/dd").Equals(DateStr)
                                                select row;

                                    if (query.Count<DataRow>() > 0)
                                    {
                                        foreach (DataRow row in query)
                                        {
                                            RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                                        }
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
                            default:
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, Cashbox.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                                break;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Cashbox_EditMode_CheckBox.Enabled = false;
                                Cashbox_LangFilter_ComboBox.Enabled = false;
                                Cashbox_SongSingerFilter_ComboBox.Enabled = false;
                                Cashbox_QueryStatus_Label.Text = "查無『" + SongQueryStatusText + "』的相關歌曲,請重新查詢...";
                            });
                        }
                        else
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Cashbox_EditMode_CheckBox.Enabled = (dt.Rows.Count > 0 ) ? true : false;

                                switch (SongQueryType)
                                {
                                    case "NonSong":
                                        Cashbox_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆歌庫所沒有的錢櫃歌曲。";
                                        break;
                                    case "SongDate":
                                        Cashbox_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆更新日期為『" + SongQueryStatusText + "』的歌曲。";
                                        break;
                                    default:
                                        Cashbox_QueryStatus_Label.Text = "總共查詢到 " + dt.Rows.Count + " 筆有關『" + SongQueryStatusText + "』的歌曲。";
                                        break;
                                }
                                
                                Cashbox_DataGridView.DataSource = dt;

                                List<string> LangFilterList = new List<string>();
                                List<string> SongSingerFilterList = new List<string>();

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

                                using (DataTable SongSingerDistinctDT = dt.DefaultView.ToTable(true, "Song_Singer"))
                                {
                                    if (SongSingerDistinctDT.Rows.Count > 0)
                                    {
                                        string SongSinger = string.Empty;
                                        foreach (DataRow row in SongSingerDistinctDT.AsEnumerable())
                                        {
                                            SongSinger = row["Song_Singer"].ToString();

                                            if (SongSinger.Contains("&"))
                                            {
                                                string ChorusSongSingerName = SongSinger;

                                                List<string> list = CommonFunc.GetChorusSingerList(ChorusSongSingerName);
                                                foreach (string str in list)
                                                {
                                                    string SingerStr = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                                    if (SongSingerFilterList.IndexOf(SingerStr) < 0)
                                                    {
                                                        SongSingerFilterList.Add(SingerStr);
                                                    }
                                                }
                                                list.Clear();
                                                list = null;
                                            }
                                            else
                                            {
                                                if (SongSingerFilterList.IndexOf(SongSinger) < 0)
                                                {
                                                    SongSingerFilterList.Add(SongSinger);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (LangFilterList.Count > 0)
                                {
                                    Cashbox_LangFilter_ComboBox.Enabled = true;
                                    Cashbox_LangFilter_ComboBox.DataSource = Cashbox.GetQueryFilterList(LangFilterList);
                                    Cashbox_LangFilter_ComboBox.DisplayMember = "Display";
                                    Cashbox_LangFilter_ComboBox.ValueMember = "Value";
                                    LangFilterList.Clear();
                                }

                                if (SongSingerFilterList.Count > 0)
                                {
                                    SongSingerFilterList.Sort();
                                    Cashbox_SongSingerFilter_ComboBox.Enabled = true;
                                    Cashbox_SongSingerFilter_ComboBox.DataSource = Cashbox.GetQueryFilterList(SongSingerFilterList);
                                    Cashbox_SongSingerFilter_ComboBox.DisplayMember = "Display";
                                    Cashbox_SongSingerFilter_ComboBox.ValueMember = "Value";
                                    SongSingerFilterList.Clear();
                                }

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

                                int ColumnWidth_140 = Convert.ToInt32(140 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor);
                                Cashbox_DataGridView.Columns["Song_CreatDate"].MinimumWidth = ColumnWidth_140;
                                Cashbox_DataGridView.Columns["Song_CreatDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                Cashbox_DataGridView.Focus();

                                dt.Dispose();
                                dt = null;
                            });
                        }
                    }
                    catch
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Cashbox_EditMode_CheckBox.Enabled = false;
                            Cashbox_LangFilter_ComboBox.Enabled = false;
                            Cashbox_SongSingerFilter_ComboBox.Enabled = false;
                            Cashbox_QueryStatus_Label.Text = "查詢條件輸入錯誤,請重新輸入...";
                        });
                    }
                }
            }
        }

        #endregion

        #region --- Cashbox 更新歌曲資料 ---

        private void Cashbox_UpdDate_Button_Click(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Common_SwitchSetUI(false);

                Cashbox_QueryStatus_Label.Text = "正在更新錢櫃資料,請稍待...";

                var tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => Cashbox_UpdDateTask())
                };

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        if (Cashbox_QueryStatus_Label.Text != "錢櫃資料已是最新!")
                            Common_InitializeSongData(false, false, true, false, false, false);
                        Common_SwitchSetUI(true);
                        Global.TimerEndTime = DateTime.Now;
                        if (Cashbox_QueryStatus_Label.Text != "錢櫃資料已是最新!")
                            Cashbox_QueryStatus_Label.Text = "總共更新 " + Global.TotalList[0] + " 筆資料,失敗 " + Global.TotalList[1] + " 筆,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                    });
                });
            }
        }

        private void Cashbox_UpdDateTask()
        {
            string CashboxUpdDate = string.Empty;
            string SongDate = string.Empty;
            List<string> SongDataList = new List<string>();
            List<string> SongIdList = new List<string>();
            List<string> SongDateList = new List<string>();

            string CashboxQuerySqlStr = "select Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate from ktv_Cashbox order by Cashbox_Id";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, CashboxQuerySqlStr, ""))
            {
                Parallel.ForEach(dt.AsEnumerable(), (row, loopState) =>
                {
                    string SongCreatDate = DateTime.Parse(row["Song_CreatDate"].ToString()).ToString("yyyy/MM/dd");
                    lock (LockThis)
                    {
                        SongIdList.Add(row["Cashbox_Id"].ToString());
                        if (SongDateList.IndexOf(SongCreatDate) < 0) SongDateList.Add(SongCreatDate);
                    }
                });
            }

            string url = "https://raw.githubusercontent.com/CrazyKTV/WebUpdater/master/CrazyKTV_WebUpdater/Cashbox/cashbox_newsong.md";
            using (MemoryStream ms = CommonFunc.Download(url))
            {
                if (ms.Length > 0)
                {
                    ms.Position = 0;
                    string line = string.Empty;
                    Regex dateline = new Regex(@"^\d{4}\/\d{2}\/\d{2}");
                    Regex dataline = new Regex(@"\d{5}\s\t.+?\s\t.+?\s\t");

                    StreamReader sr = new StreamReader(ms);
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (dateline.IsMatch(line))
                        {
                            if (CashboxUpdDate == "") CashboxUpdDate = line;
                            SongDate = line;
                            if (SongDateList.IndexOf(SongDate) >= 0)
                            {
                                this.BeginInvoke((Action)delegate ()
                                {
                                    Cashbox_QueryStatus_Label.Text = "錢櫃資料已是最新!";
                                });
                                return;
                            }
                        }
                        else if (dataline.IsMatch(line))
                        {
                            if (SongDate == "") return;
                            line = Regex.Replace(line, @"\s\s\t$", "");
                            line = Regex.Replace(line, @"\s\t", "|");
                            List<string> list = new List<string>(line.Split('|'));
                            if (CommonFunc.IsSongId(list[0]) && list[1] != "" && list[2] != "" && list[3] != "")
                            {
                                list.Add(SongDate);
                                if (list[1] == "") list[1] = "其它";
                                list[3] = Regex.Replace(list[3], "、", "&");

                                if (SongIdList.IndexOf(list[0]) < 0)
                                {
                                    SongIdList.Add(list[0]);
                                    list.Add("AddSong");
                                }
                                else
                                {
                                    list.Add("UpdSong");
                                }
                                SongDataList.Add(string.Join("|", list));
                            }
                            list.Clear();
                            list = null;
                        }
                    }
                }
            }
            SongIdList.Clear();
            SongIdList = null;
            SongDateList.Clear();
            SongDateList = null;

            if (SongDataList.Count >= 0)
            {
                using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
                {
                    string sqlAddStr = "Cashbox_Id, Song_Lang, Song_SongName, Song_Singer, Song_CreatDate";
                    string sqlValuesStr = "@CashboxId, @SongLang, @SongSongName, @SongSinger, @SongCreatDate";
                    string AddSqlStr = "insert into ktv_Cashbox ( " + sqlAddStr + " ) values ( " + sqlValuesStr + " )";
                    string sqlUpdStr = "Cashbox_Id = @CashboxId, Song_Lang = @SongLang, Song_SongName = @SongSongName, Song_Singer = @SongSinger, Song_CreatDate = @SongCreatDate";
                    string UpdSqlStr = "update ktv_Cashbox set " + sqlUpdStr + " where Cashbox_Id = @OldCashboxId";

                    OleDbCommand AddCmd = new OleDbCommand(AddSqlStr, conn);
                    OleDbCommand UpdCmd = new OleDbCommand(UpdSqlStr, conn);

                    foreach (string SongData in SongDataList)
                    {
                        List<string> valuelist = new List<string>(SongData.Split('|'));

                        switch (valuelist[5])
                        {
                            case "AddSong":
                                AddCmd.Parameters.AddWithValue("@CashboxId", valuelist[0]);
                                AddCmd.Parameters.AddWithValue("@SongLang", valuelist[1]);
                                AddCmd.Parameters.AddWithValue("@SongSongName", valuelist[2]);
                                AddCmd.Parameters.AddWithValue("@SongSinger", valuelist[3]);
                                AddCmd.Parameters.AddWithValue("@SongCreatDate", valuelist[4]);

                                try
                                {
                                    AddCmd.ExecuteNonQuery();
                                    Global.TotalList[0]++;
                                    this.BeginInvoke((Action)delegate ()
                                    {
                                        Cashbox_QueryStatus_Label.Text = "正在將第 " + Global.TotalList[0] + " 首歌曲寫入資料庫,請稍待...";
                                    });
                                }
                                catch
                                {
                                    Global.TotalList[1]++;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "加入錢櫃資料時發生未知的錯誤: " + SongData;
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                }
                                AddCmd.Parameters.Clear();
                                break;
                            case "UpdSong":
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
                                    this.BeginInvoke((Action)delegate ()
                                    {
                                        Cashbox_QueryStatus_Label.Text = "正在將第 " + Global.TotalList[0] + " 首歌曲寫入資料庫,請稍待...";
                                    });
                                }
                                catch
                                {
                                    Global.TotalList[1]++;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "更新錢櫃資料時發生未知的錯誤: " + SongData;
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                }
                                UpdCmd.Parameters.Clear();
                                break;
                        }
                        valuelist.Clear();
                        valuelist = null;
                    }
                }
                SongDataList.Clear();
                SongDataList = null;

                using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvSongMgrDatabaseFile, ""))
                {
                    Global.CashboxUpdDate = DateTime.Parse(CashboxUpdDate);
                    string CashboxUpdDateSqlStr = "CashboxUpdDate = @CashboxUpdDate";
                    string CashboxUpdDateUpdateSqlStr = "update ktv_Version set " + CashboxUpdDateSqlStr + " where Id = @Id";
                    OleDbCommand Versioncmd = new OleDbCommand(CashboxUpdDateUpdateSqlStr, conn);

                    Versioncmd.Parameters.AddWithValue("@CashboxUpdDate", Global.CashboxUpdDate.ToString());
                    Versioncmd.Parameters.AddWithValue("@Id", "1");
                    Versioncmd.ExecuteNonQuery();
                    Versioncmd.Parameters.Clear();
                }

                this.BeginInvoke((Action)delegate ()
                {
                    Cashbox_UpdDateValue_Label.Text = (CultureInfo.CurrentCulture.Name == "zh-TW") ? Global.CashboxUpdDate.ToLongDateString() : Global.CashboxUpdDate.ToShortDateString();

                    Cashbox_DateQuery_ComboBox.SelectedIndexChanged -= new EventHandler(Cashbox_DateQuery_ComboBox_SelectedIndexChanged);
                    Cashbox_DateQuery_ComboBox.DataSource = Cashbox.GetDateQueryList();
                    Cashbox_DateQuery_ComboBox.DisplayMember = "Display";
                    Cashbox_DateQuery_ComboBox.ValueMember = "Value";
                    Cashbox_DateQuery_ComboBox.SelectedValue = 1;
                    Cashbox_DateQuery_ComboBox.SelectedIndexChanged += new EventHandler(Cashbox_DateQuery_ComboBox_SelectedIndexChanged);

                    SongMaintenance_Favorite_UpdateNewsong_ComboBox.DataSource = SongMaintenance.GetNewsongDateList();
                    SongMaintenance_Favorite_UpdateNewsong_ComboBox.DisplayMember = "Display";
                    SongMaintenance_Favorite_UpdateNewsong_ComboBox.ValueMember = "Value";
                    SongMaintenance_Favorite_UpdateNewsong_ComboBox.SelectedValue = 1;
                });
            }
        }

        #endregion

        #region --- Cashbox 套用錢櫃編號 ---

        private void Cashbox_ApplyCashboxId_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要套用錢櫃編號嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Cashbox_QueryStatus_Label.Text = "正在解析錢櫃歌曲編號,請稍待...";

                Common_SwitchSetUI(false);
                Cashbox.CreateSongDataTable();

                var tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => Cashbox_ApplyCashboxIdTask())
                };

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        Cashbox_QueryStatus_Label.Text = "總共變更 " + Global.TotalList[0] + " 首歌曲為錢櫃編號,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_SwitchSetUI(true);
                    });
                    Cashbox.DisposeSongDataTable();
                });
            }
        }

        private void Cashbox_ApplyCashboxIdTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                List<string> FavoriteList = new List<string>();

                string SongQuerySqlStr = "select User_Id, User_Name from ktv_User";
                using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.AsEnumerable())
                        {
                            FavoriteList.Add("ktv_User|" + row["User_Id"].ToString() + "|" + row["User_Name"].ToString());
                        }
                    }
                }

                SongQuerySqlStr = "select User_Id, Song_Id from ktv_Favorite";
                using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.AsEnumerable())
                        {
                            if (Cashbox.SongIdList.IndexOf(row["Song_Id"].ToString()) >= 0)
                            {
                                int i = Cashbox.SongIdList.IndexOf(row["Song_Id"].ToString());
                                List<string> list = new List<string>(Regex.Split(Cashbox.SongDataList[i], @"\|", RegexOptions.None));

                                FavoriteList.Add("ktv_Favorite|" + row["User_Id"].ToString() + "|" + list[0] + "|" + list[1] + "|" + list[2]);
                            }
                        }
                    }
                }

                if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Backup")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\Backup\Favorite.txt");
                foreach (string str in FavoriteList)
                {
                    sw.WriteLine(str);
                }
                sw.Close();
                FavoriteList.Clear();

                List<string> ReNewList = new List<string>();
                List<string> ReOldList = new List<string>();

                string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";
                CommonFunc.GetMaxSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6);
                CommonFunc.GetUnusedSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6);

                string SqlStr = "select Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate from ktv_Cashbox";
                using (DataTable CashboxDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SqlStr, ""))
                {
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
                                int SongDataIndex = CommonFunc.MatchCashboxSong("CashboxSong", row, "", "", "", Cashbox.SongDataLowCaseList, Cashbox.SongDataFuzzyList);

                                if (SongDataIndex >= 0)
                                {
                                    lock (LockThis) { Global.TotalList[2]++; }

                                    if (CashboxId != Cashbox.SongIdList[SongDataIndex])
                                    {
                                        List<string> list = new List<string>(Cashbox.SongDataList[SongDataIndex].Split('|'));
                                        if (Cashbox.SongIdList.IndexOf(CashboxId) >= 0)
                                        {
                                            lock (LockThis) { ReOldList.Add(CashboxId + "|" + Cashbox.SongIdList[SongDataIndex] + "|" + list[0] + "|" + list[1] + "|" + list[2]); }
                                        }
                                        else
                                        {
                                            lock (LockThis) { ReNewList.Add(CashboxId + "|" + Cashbox.SongIdList[SongDataIndex] + "|" + list[0] + "|" + list[1] + "|" + list[2]); }
                                        }
                                        list.Clear();
                                    }

                                    this.BeginInvoke((Action)delegate()
                                    {
                                        Cashbox_QueryStatus_Label.Text = "已在歌庫找到 " + Global.TotalList[2] + " 首錢櫃歌曲...";
                                    });
                                }
                            }
                        }
                    });
                }
                ReOldList.Sort();
                ReNewList.Sort();

                using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, ""))
                {
                    OleDbCommand cmd = new OleDbCommand();
                    string UpdateSqlStr = "update ktv_Song set Song_Id = @SongId where Song_Id = @OldSongId";
                    cmd = new OleDbCommand(UpdateSqlStr, conn);

                    List<string> valuelist;
                    foreach (string str in ReOldList)
                    {
                        valuelist = new List<string>(str.Split('|'));
                        string NextSongId = Cashbox.GetNextSongId(valuelist[2]);
                        try
                        {
                            cmd.Parameters.AddWithValue("@SongId", NextSongId);
                            cmd.Parameters.AddWithValue("@OldSongId", valuelist[0]);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                            cmd.Parameters.AddWithValue("@OldSongId", valuelist[1]);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            Global.TotalList[0]++;
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "已成功套用編號: " + valuelist[0] + " => " + NextSongId + " 及 " + valuelist[1] + " => " + valuelist[0] + " (" + valuelist[2] + "-" + valuelist[3] + "-" + valuelist[4] + ")";
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        }
                        catch
                        {
                            Global.TotalList[1]++;
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "分配編號時發生未知的錯誤: " + valuelist[0] + " => " + NextSongId + " 及 " + valuelist[1] + " => " + valuelist[0] + " (" + valuelist[2] + "-" + valuelist[3] + "-" + valuelist[4] + ")";
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        }
                        finally
                        {
                            cmd.Parameters.Clear();
                        }

                        this.BeginInvoke((Action)delegate()
                        {
                            Cashbox_QueryStatus_Label.Text = "已成功將 " + Global.TotalList[0] + " 首歌曲變更為錢櫃編號,失敗 " + Global.TotalList[1] + " 首...";
                        });
                        valuelist.Clear();
                    }
                    ReOldList.Clear();

                    foreach (string str in ReNewList)
                    {
                        valuelist = new List<string>(str.Split('|'));

                        cmd.Parameters.AddWithValue("@SongId", valuelist[0]);
                        cmd.Parameters.AddWithValue("@OldSongId", valuelist[1]);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            Global.TotalList[0]++;
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "已成功套用編號: " + valuelist[1] + " => " + valuelist[0] + " (" + valuelist[2] + "-" + valuelist[3] + "-" + valuelist[4] + ")";
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        }
                        catch
                        {
                            Global.TotalList[1]++;
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "分配編號時發生未知的錯誤: " + valuelist[1] + " => " + valuelist[0] + " (" + valuelist[2] + "-" + valuelist[3] + "-" + valuelist[4] + ")";
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        }
                        cmd.Parameters.Clear();

                        this.BeginInvoke((Action)delegate()
                        {
                            Cashbox_QueryStatus_Label.Text = "已成功將 " + Global.TotalList[0] + " 首歌曲變更錢櫃編號,失敗 " + Global.TotalList[1] + " 首...";
                        });
                        valuelist.Clear();
                    }
                    ReNewList.Clear();
                }

                using (OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, ""))
                {
                    OleDbCommand Ucmd = new OleDbCommand();
                    OleDbCommand Fcmd = new OleDbCommand();

                    string TruncateSqlStr = "";

                    TruncateSqlStr = "delete * from ktv_User";
                    Ucmd = new OleDbCommand(TruncateSqlStr, conn);
                    Ucmd.ExecuteNonQuery();

                    TruncateSqlStr = "delete * from ktv_Favorite";
                    Fcmd = new OleDbCommand(TruncateSqlStr, conn);
                    Fcmd.ExecuteNonQuery();

                    List<string> Addlist = new List<string>();
                    StreamReader sr = new StreamReader(Application.StartupPath + @"\SongMgr\Backup\Favorite.txt", Encoding.UTF8);
                    while (!sr.EndOfStream)
                    {
                        Addlist.Add(sr.ReadLine());
                    }
                    sr.Close();

                    string UserColumnStr = "User_Id, User_Name";
                    string UserValuesStr = "@UserId, @UserName";
                    string UserAddSqlStr = "insert into ktv_User ( " + UserColumnStr + " ) values ( " + UserValuesStr + " )";
                    Ucmd = new OleDbCommand(UserAddSqlStr, conn);

                    string FavoriteColumnStr = "User_Id, Song_Id";
                    string FavoriteValuesStr = "@UserId, @SongId";
                    string FavoriteAddSqlStr = "insert into ktv_Favorite ( " + FavoriteColumnStr + " ) values ( " + FavoriteValuesStr + " )";
                    Fcmd = new OleDbCommand(FavoriteAddSqlStr, conn);

                    List<string> list = new List<string>();
                    foreach (string AddStr in Addlist)
                    {
                        list = new List<string>(Regex.Split(AddStr, @"\|", RegexOptions.None));
                        switch (list[0])
                        {
                            case "ktv_User":
                                Ucmd.Parameters.AddWithValue("@UserId", list[1]);
                                Ucmd.Parameters.AddWithValue("@UserName", list[2]);
                                Ucmd.ExecuteNonQuery();
                                Ucmd.Parameters.Clear();
                                break;
                            case "ktv_Favorite":
                                string SongData = list[2] + "|" + list[3].ToLower() + "|" + list[4].ToLower();

                                if (Cashbox.SongDataLowCaseList.IndexOf(SongData) > 0)
                                {
                                    string SongId = Cashbox.SongIdList[Cashbox.SongDataLowCaseList.IndexOf(SongData)];
                                    Fcmd.Parameters.AddWithValue("@UserId", list[1]);
                                    Fcmd.Parameters.AddWithValue("@SongId", SongId);
                                    Fcmd.ExecuteNonQuery();
                                    Fcmd.Parameters.Clear();
                                }
                                break;
                        }
                        list.Clear();
                    }
                    Addlist.Clear();
                }
            }
        }

        #endregion

        #region --- Cashbox 記錄已有歌曲 ---

        private void Cashbox_WriteHaveSongData_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要記錄已有歌曲嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Cashbox_QueryStatus_Label.Text = "正在解析已有歌曲,請稍待...";

                Common_SwitchSetUI(false);
                Cashbox.CreateSongDataTable();

                var tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => Cashbox_WriteHaveSongDataTask())
                };

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        Cashbox_QueryStatus_Label.Text = "總共解析到 " + Global.TotalList[0] + " 首已有歌曲,更新 " + Global.TotalList[1] + " 首,失敗 " + Global.TotalList[2] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        Common_InitializeSongData(false, false, false, false, false, true);
                        Common_SwitchSetUI(true);
                    });
                    Cashbox.DisposeSongDataTable();
                });
            }
        }

        private void Cashbox_WriteHaveSongDataTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            if (Global.CrazyktvDatabaseStatus)
            {
                List<string> UpdateList = new List<string>();
                string SqlStr = "select Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate, Song_HaveSong from ktv_Cashbox";
                using (DataTable CashboxDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SqlStr, ""))
                {
                    Parallel.ForEach(Global.CashboxSongLangList, (langstr, loopState) =>
                    {
                        var query = from row in CashboxDT.AsEnumerable()
                                    where row.Field<string>("Song_Lang").Equals(langstr)
                                    select row;

                        if (query.Count<DataRow>() > 0)
                        {
                            foreach (DataRow row in query)
                            {
                                int SongDataIndex = CommonFunc.MatchCashboxSong("CashboxSong", row, "", "", "", Cashbox.SongDataLowCaseList, Cashbox.SongDataFuzzyList);

                                string CashboxId = row["Cashbox_Id"].ToString();

                                if (SongDataIndex >= 0)
                                {
                                    lock (LockThis) { Global.TotalList[0]++; }
                                    UpdateList.Add(CashboxId);

                                    this.BeginInvoke((Action)delegate()
                                    {
                                        Cashbox_QueryStatus_Label.Text = "已在歌庫找到 " + Global.TotalList[0] + " 首錢櫃歌曲...";
                                    });
                                }
                            }
                        }
                    });
                }

                if (!Directory.Exists(Application.StartupPath + @"\SongMgr")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Backup");
                using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\SongMgr\CashboxLog.txt"))
                {
                    foreach (string str in UpdateList)
                    {
                        sw.WriteLine(str);
                        Global.TotalList[1]++;

                        this.BeginInvoke((Action)delegate()
                        {
                            Cashbox_QueryStatus_Label.Text = "正在將第 " + Global.TotalList[1] + " 首已有歌曲寫入記錄,請稍待...";
                        });
                    }
                }
                UpdateList.Clear();
            }
        }

        #endregion

    }


    class Cashbox
    {
        
        #region --- Cashbox 建立資料表 ---

        public static List<string> SongIdList;
        public static List<string> SongDataList;
        public static List<string> SongDataLowCaseList;
        public static List<string> SongDataFuzzyList;

        public static void CreateSongDataTable()
        {
            SongIdList = new List<string>();
            SongDataList = new List<string>();
            SongDataLowCaseList = new List<string>();
            SongDataFuzzyList = new List<string>();

            string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_Singer, Song_SongName from ktv_Song order by Song_Id";
            using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
            {
                string SongId = string.Empty;
                string SongLang = string.Empty;
                string SongSinger = string.Empty;
                string SongSongName = string.Empty;
                string FuzzyStr = string.Empty;
                foreach (DataRow row in dt.AsEnumerable())
                {
                    SongId = row["Song_Id"].ToString();
                    SongLang = row["Song_Lang"].ToString();
                    SongSinger = row["Song_Singer"].ToString();
                    SongSongName = row["Song_SongName"].ToString();

                    SongIdList.Add(SongId);
                    SongDataList.Add(SongLang + "|" + SongSinger + "|" + SongSongName);
                    SongDataLowCaseList.Add(SongLang + "|" + SongSinger.ToLower() + "|" + SongSongName.ToLower());

                    FuzzyStr = SongLang + "|" + Regex.Replace(SongSinger.ToLower(), @"\s?[\{\(\[｛（［【].+?[】］）｝\]\)\}]\s?|\s|" + Global.CashboxNonSymbolList, "") + "|" + Regex.Replace(SongSongName.ToLower(), @"\s?[\{\(\[｛（［【].+?[】］）｝\]\)\}]\s?|\s|" + Global.CashboxNonSymbolList, "");
                    if (Global.CashboxFullMatchSongList.IndexOf(FuzzyStr) > 0 || Global.GroupSingerLowCaseList.IndexOf(SongSinger.ToLower()) >= 0) FuzzyStr = CommonFunc.GetFuzzyMatchStr("UserSong", SongLang, SongSinger.ToLower(), SongSongName.ToLower(), false, "");
                    SongDataFuzzyList.Add(FuzzyStr);
                }
            }
        }

        public static void DisposeSongDataTable()
        {
            SongIdList.Clear();
            SongDataList.Clear();
            SongDataLowCaseList.Clear();
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
                List<string> ItemList = new List<string>() { "歌曲名稱", "歌手名稱", "歌曲編號", "全部歌曲", "新進歌曲" };

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

        public static DataTable GetQueryFilterList(List<string> FilterList)
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

        public static DataTable GetOtherQueryList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                List<string> ItemList = new List<string>() { "錢櫃缺歌", "重複歌曲" };

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

        public static DataTable GetDateQueryList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                List<string> ItemList = new List<string>();

                string QuerySqlStr = "SELECT First(Song_CreatDate) AS Song_CreatDate, Count(Song_CreatDate) AS Song_CreatDateCount FROM ktv_Cashbox GROUP BY Song_CreatDate HAVING (Count(Song_CreatDate)>0) ORDER BY First(Song_CreatDate) desc";
                using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, QuerySqlStr, ""))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.AsEnumerable())
                        {
                            string DateItem = DateTime.Parse(row["Song_CreatDate"].ToString()).ToString("yyyy/MM/dd");
                            if (ItemList.IndexOf(DateItem) < 0)
                            {
                                ItemList.Add(DateItem);
                            }
                        }
                    }
                }

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

        #region --- Cashbox 取得 SQL 查詢字串 ---

        public static string GetSongQuerySqlStr(string QueryType, string QueryValue)
        {
            string sqlCommonStr = " Cashbox_Id, Song_Lang, Song_Singer, Song_SongName, Song_CreatDate, Song_HaveSong ";
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
                        if (HasWideCharQueryValue == "" || HasWideCharQueryValue == " ") HasWideCharQueryValue = QueryValue;
                    }
                }

                Regex HasSymbols = new Regex("[']");
                if (HasSymbols.IsMatch(QueryValue))
                {
                    QueryValue = Regex.Replace(QueryValue, "[']", delegate (Match match)
                    {
                        string str = "'" + match.ToString();
                        return str;
                    });
                }

                if (HasSymbols.IsMatch(HasWideCharQueryValue))
                {
                    HasWideCharQueryValue = Regex.Replace(HasWideCharQueryValue, "[']", delegate (Match match)
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
                case "AllSong":
                    if (Global.CashboxQueryFilter != "全部")
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where Song_Lang = '" + Global.CashboxQueryFilter + "'" + SongQueryOrderStr;
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox" + SongQueryOrderStr;
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
                case "DuplicateSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where (((Song_SongName) In (select Song_SongName from ktv_Cashbox As Tmp group by Song_SongName, Song_Lang, Song_Singer HAVING Count(*)>1 and Song_SongName = ktv_Cashbox.Song_SongName and Song_Lang = ktv_Cashbox.Song_Lang and Song_Singer = ktv_Cashbox.Song_Singer))) order by Song_SongName";
                    break;
                case "SongDate":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Cashbox where Song_CreatDate like '%" + QueryValue + "%' order by Cashbox_Id";
                    break;
            }

            return SongQuerySqlStr;
        }

        #endregion

        #region --- Cashbox 歌曲列表欄位設定 ---

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            string ColumnWidth_120 = Convert.ToInt32((120 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_140 = Convert.ToInt32((140 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_190 = Convert.ToInt32((190 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();
            string ColumnWidth_320 = Convert.ToInt32((320 * Global.DPIScalingFactor * MainUIScale.UIScalingFactor)).ToString();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Cashbox_Id":
                    list = new List<string>() { "歌曲編號", ColumnWidth_120, "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", ColumnWidth_120, "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", ColumnWidth_190, "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", ColumnWidth_320, "none" };
                    break;
                case "Song_CreatDate":
                    list = new List<string>() { "更新日期", ColumnWidth_140, "none" };
                    break;
                case "Song_HaveSong":
                    list = new List<string>() { "已有歌曲", "0", "none" };
                    break;
            }
            return list;
        }

        #endregion

        #region --- Cashbox 取得下個歌曲編號 ---

        public static string GetNextSongId(string SongLang)
        {
            string NewSongID = "";

            // 查詢歌曲編號有無斷號
            if (Global.UnusedSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Count > 0)
            {
                NewSongID = Global.UnusedSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)][0];
                Global.UnusedSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Remove(NewSongID);
            }

            // 若無斷號查詢各語系下個歌曲編號
            if (NewSongID == "")
            {
                string MaxDigitCode = (Global.SongMgrMaxDigitCode == "1") ? "D5" : "D6";
                Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)]++;
                NewSongID = Global.MaxIDList[Global.CrazyktvSongLangList.IndexOf(SongLang)].ToString(MaxDigitCode);
            }
            return NewSongID;
        }

        #endregion

    }
}
