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
                    SongQuery_QueryValue_TextBox.Text = "100";
                    SongQuery_QueryValue_TextBox.Enabled = true;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
                    SongQuery_DataGridView.Focus();
                    break;
                case "5":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "*";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
                    SongQuery_DataGridView.Focus();
                    break;
                case "6":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    
                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongType", false);
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "7":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_TextBox.Visible = false;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;

                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SingerType", false);
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

                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", false);
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 0;

                    SongQuery_QueryValue_ComboBox.Visible = true;
                    SongQuery_QueryValue_ComboBox.Focus();
                    break;
                case "9":
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.Off;
                    SongQuery_QueryValue_TextBox.Text = "*";
                    SongQuery_QueryValue_TextBox.Enabled = false;
                    SongQuery_QueryValue_ComboBox.Visible = false;
                    SongQuery_QueryValue_TextBox.Visible = true;
                    SongQuery_Paste_Button.Enabled = false;
                    SongQuery_Clear_Button.Enabled = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
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
                case "1":

                    break;
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

        #endregion

        #region --- SongQuery 查詢歌曲 ---

        private void SongQuery_Query_Button_Click(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus)
            {
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
                        SongQueryType = "NewSong";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "新進歌曲";
                        SongQuery_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                    case "5":
                        SongQueryType = "ChorusSong";
                        SongQueryValue = SongQuery_QueryValue_TextBox.Text;
                        SongQueryStatusText = "合唱歌曲";
                        SongQuery_QueryStatus_Label.Text = "正在查詢" + SongQueryStatusText + ",請稍待...";
                        break;
                    case "6":
                        SongQueryType = "SongType";
                        SongQueryValue = SongQuery_QueryValue_ComboBox.Text;
                        SongQueryStatusText = "歌曲類別為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "7":
                        SongQueryType = "SingerType";
                        SongQueryValue = Global.CrazyktvSingerTypeList.IndexOf(SongQuery_QueryValue_ComboBox.Text).ToString();
                        SongQueryStatusText = "歌手類別為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "8":
                        SongQueryType = "SongTrack";
                        SongQueryValue = SongQuery_QueryValue_ComboBox.SelectedValue.ToString();
                        SongQueryStatusText = "歌曲聲道為" + SongQuery_QueryValue_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢『" + SongQueryStatusText + "』的相關歌曲,請稍待...";
                        break;
                    case "9":
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
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

                                if (Global.SongQueryHasWideChar)
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

                                if (Global.SongQuerySynonymousQuery)
                                {
                                    List<string> SynonymousSongNameList = new List<string>();
                                    SynonymousSongNameList = CommonFunc.GetSynonymousSongNameList(SongQueryValue);

                                    if (SynonymousSongNameList.Count > 0)
                                    {
                                        DataTable SynonymousSongDT = new DataTable();
                                        foreach (string SynonymousSongName in SynonymousSongNameList)
                                        {
                                            SynonymousSongDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SynonymousSongName), "");
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
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

                                if (Global.SongQueryHasWideChar)
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
                            case "CashboxId":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

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
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                                break;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_EditMode_CheckBox.Enabled = false;
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
                                    Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");

                                    foreach (DataRow row in query)
                                    {
                                        if (r.IsMatch(row["Song_Singer"].ToString()))
                                        {
                                            string RemoveThisRow = "True";
                                            string[] singers = Regex.Split(row["Song_Singer"].ToString(), "&", RegexOptions.None);
                                            foreach (string str in singers)
                                            {
                                                if (str == SongQuery_QueryValue_TextBox.Text) { RemoveThisRow = "False"; }
                                            }
                                            if (RemoveThisRow == "True") RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
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
                            }

                            if (dt.Rows.Count == 0)
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    SongQuery_EditMode_CheckBox.Enabled = false;
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

                                    SongQuery_DataGridView.Columns["Song_FullPath"].Width = 640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = 640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                                    for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                    {
                                        SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                        SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                    }
                                    SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
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
                SongQuery_DataGridView.Size = new Size(Convert.ToInt32(762 * Global.DPIScalingFactor), Convert.ToInt32(216 * Global.DPIScalingFactor));
                SongQuery_DataGridView.Location = new Point(Convert.ToInt32(18 * Global.DPIScalingFactor), Convert.ToInt32(18 * Global.DPIScalingFactor));
                SongQuery_Edit_GroupBox.Visible = true;
                SongQuery_Query_GroupBox.Visible = false;
                SongQuery_OtherQuery_GroupBox.Visible = false;
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
                SongQuery_DataGridView.Size = new Size(Convert.ToInt32(762 * Global.DPIScalingFactor), Convert.ToInt32(237 * Global.DPIScalingFactor));
                SongQuery_DataGridView.Location = new Point(Convert.ToInt32(18 * Global.DPIScalingFactor), Convert.ToInt32(292 * Global.DPIScalingFactor));
                SongQuery_EditMode_CheckBox.Enabled = (SongQuery_DataGridView.RowCount == 0) ? false : true;
                SongQuery_Edit_GroupBox.Visible = false;
                SongQuery_Query_GroupBox.Visible = true;
                SongQuery_OtherQuery_GroupBox.Visible = true;
                SongQuery_Statistics_GroupBox.Visible = true;

                SongQuery_QueryStatus_Label.Text = "已進入檢視模式...";
            }
            SongQuery_DataGridView.Focus();
        }

        #endregion

        #region --- SongQuery 更新歌曲 ---

        private void SongQuery_SongUpdate(DataTable SongUpdateDT)
        {
            List<string> SongUpdateValueList = new List<string>();

            this.BeginInvoke((Action)delegate()
            {
                SongQuery_QueryStatus_Label.Text = "";
            });

            foreach (DataRow row in SongUpdateDT.Rows)
            {
                int i = Convert.ToInt32(row["RowIndex"]);
                string OldSongId = row["SongId"].ToString();
                string OldSongLang = row["SongLang"].ToString();

                string SongId = SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                string SongLang = SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value.ToString();
                int SongSingerType = Convert.ToInt32(SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Value);
                string SongSinger = SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value.ToString();
                string SongSongName = SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value.ToString();
                int SongTrack = Convert.ToInt32(SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Value);
                string SongSongType = SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Value.ToString();
                string SongVolume = SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Value.ToString();
                string SongWordCount = SongQuery_DataGridView.Rows[i].Cells["Song_WordCount"].Value.ToString();
                string SongPlayCount = SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Value.ToString();
                string SongMB = SongQuery_DataGridView.Rows[i].Cells["Song_MB"].Value.ToString();
                string SongCreatDate = SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Value.ToString();
                string SongFileName = SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                string SongPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString();
                string SongSpell = SongQuery_DataGridView.Rows[i].Cells["Song_Spell"].Value.ToString();
                string SongSpellNum = SongQuery_DataGridView.Rows[i].Cells["Song_SpellNum"].Value.ToString();
                string SongSongStroke = SongQuery_DataGridView.Rows[i].Cells["Song_SongStroke"].Value.ToString();
                string SongPenStyle = SongQuery_DataGridView.Rows[i].Cells["Song_PenStyle"].Value.ToString();
                string SongPlayState = SongQuery_DataGridView.Rows[i].Cells["Song_PlayState"].Value.ToString();

                string DupSongData = "";
                bool DuplicateSongStatus = false;
                bool MoveError = false;

                // 重複歌曲判斷
                if (SongQuery.SongDataLowCaseList.Count > 0)
                {
                    // 同義字
                    List<string> SynonymousSongNameList = new List<string>();
                    List<string> SynonymousSongNameLowCaseList = new List<string>();
                    if (Global.SongQuerySynonymousQuery)
                    {
                        SynonymousSongNameList = CommonFunc.GetSynonymousSongNameList(SongSongName);
                        SynonymousSongNameLowCaseList = SynonymousSongNameList.ConvertAll(str => str.ToLower());
                    }

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
                        if (Global.SongQuerySynonymousQuery)
                        {
                            if (SynonymousSongNameList.Count > 0)
                            {
                                foreach (string SynonymousSongName in SynonymousSongNameLowCaseList)
                                {
                                    SongData = SongLang + "|" + SongSinger.ToLower() + "|" + SynonymousSongName + "|" + SongSongType.ToLower();
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
                        }
                    }

                    if (!DuplicateSongStatus && SongSingerType == 3)
                    {
                        List<string> ChorusSongDatalist = new List<string>() { SongLang, SongSongName.ToLower() };
                        if (SongSongType != "") ChorusSongDatalist.Add(SongSongType.ToLower());

                        // 處理合唱歌曲中的特殊歌手名稱
                        string ChorusSongSingerName = SongSinger;
                        List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, ",", RegexOptions.IgnoreCase));

                        foreach (string SpecialSingerName in SpecialStrlist)
                        {
                            Regex SpecialStrRegex = new Regex(SpecialSingerName, RegexOptions.IgnoreCase);
                            if (SpecialStrRegex.IsMatch(ChorusSongSingerName))
                            {
                                if (ChorusSongDatalist.IndexOf(SpecialSingerName.ToLower()) < 0) ChorusSongDatalist.Add(SpecialSingerName.ToLower());
                                ChorusSongSingerName = Regex.Replace(ChorusSongSingerName, "&" + SpecialSingerName + "|" + SpecialSingerName + "&", "");
                            }
                        }
                        SpecialStrlist.Clear();

                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                        if (r.IsMatch(ChorusSongSingerName))
                        {
                            string[] singers = Regex.Split(ChorusSongSingerName, "&", RegexOptions.None);
                            foreach (string str in singers)
                            {
                                string SingerStr = Regex.Replace(str, @"^\s*|\s*$", ""); //去除頭尾空白
                                if (ChorusSongDatalist.IndexOf(SingerStr.ToLower()) < 0) ChorusSongDatalist.Add(SingerStr.ToLower());
                            }
                        }
                        else
                        {
                            if (ChorusSongDatalist.IndexOf(ChorusSongSingerName.ToLower()) < 0) ChorusSongDatalist.Add(ChorusSongSingerName.ToLower());
                        }

                        if (SongQuery.SongDataLowCaseList.Find(SongInfo => SongInfo.ContainsAll(ChorusSongDatalist.ToArray())) != null)
                        {
                            int ChorusSongDataIndex = SongQuery.SongDataLowCaseList.IndexOf(SongQuery.SongDataLowCaseList.Find(ChorusSongData => ChorusSongData.ContainsAll(ChorusSongDatalist.ToArray())));
                            if (SongQuery.SongDataIdList[ChorusSongDataIndex] != OldSongId)
                            {
                                DupSongData = SongQuery.SongDataIdList[ChorusSongDataIndex] + "|" + SongQuery.SongDataList[ChorusSongDataIndex];
                                DuplicateSongStatus = true;
                            }
                        }
                        else
                        {
                            if (Global.SongQuerySynonymousQuery)
                            {
                                if (SynonymousSongNameList.Count > 0)
                                {
                                    foreach (string SynonymousSongName in SynonymousSongNameLowCaseList)
                                    {
                                        ChorusSongDatalist[1] = SynonymousSongName;
                                        if (SongQuery.SongDataLowCaseList.Find(ChorusSongData => ChorusSongData.ContainsAll(ChorusSongDatalist.ToArray())) != null)
                                        {
                                            int ChorusSongDataIndex = SongQuery.SongDataLowCaseList.IndexOf(SongQuery.SongDataLowCaseList.Find(ChorusSongData => ChorusSongData.ContainsAll(ChorusSongDatalist.ToArray())));
                                            if (SongQuery.SongDataIdList[ChorusSongDataIndex] != OldSongId)
                                            {
                                                DupSongData = SongQuery.SongDataIdList[ChorusSongDataIndex] + "|" + SongQuery.SongDataList[ChorusSongDataIndex];
                                                DuplicateSongStatus = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ChorusSongDatalist.Clear();
                    }
                    SynonymousSongNameList.Clear();
                    SynonymousSongNameLowCaseList.Clear(); 
                }

                if (DuplicateSongStatus)
                {
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】已有重複歌曲: " + DupSongData;
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                    this.BeginInvoke((Action)delegate()
                    {
                        SongQuery_QueryStatus_Label.Text = "部份歌曲已復原變更,因有重複歌曲,請參考操作記錄裡的內容!";
                    });
                }

                if (!DuplicateSongStatus)
                {
                    if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4")
                    {
                        string SongSrcPath = Path.Combine(SongPath, SongFileName);
                        string SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongFileName, SongPath);
                        SongPath = Path.GetDirectoryName(SongDestPath) + @"\";
                        SongFileName = Path.GetFileName(SongDestPath);

                        this.BeginInvoke((Action)delegate()
                        {
                            SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value = SongPath;
                            SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value = SongFileName;
                            SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongPath + SongFileName;
                        });

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
                                        this.BeginInvoke((Action)delegate()
                                        {
                                            SongQuery_QueryStatus_Label.Text = "部份歌曲已復原變更,因異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                                        });
                                    }
                                }
                                else
                                {
                                    MoveError = true;
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】異動檔案時發生錯誤: " + SongSrcPath + " (歌庫裡已存在該首歌曲的檔案)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "部份歌曲已復原變更,因異動檔案時發生錯誤,請參考操作記錄裡的內容!";
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
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        SongQuery_QueryStatus_Label.Text = "部份歌曲已復原變更,因異動檔案時發生錯誤,請參考操作記錄裡的內容!";
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
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_QueryStatus_Label.Text = "部份歌曲已復原變更,因異動檔案時發生錯誤,請參考操作記錄裡的內容!";
                            });
                        }
                    }
                }
                
                if (MoveError | DuplicateSongStatus)
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
                                SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value = RestoreSongDataList[0];
                                SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value = RestoreSongDataList[1];
                                SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Value = RestoreSongDataList[2];
                                SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value = RestoreSongDataList[3];
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value = RestoreSongDataList[4];
                                SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Value = RestoreSongDataList[5];
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Value = RestoreSongDataList[6];
                                SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Value = RestoreSongDataList[7];
                                SongQuery_DataGridView.Rows[i].Cells["Song_WordCount"].Value = RestoreSongDataList[8];
                                SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Value = RestoreSongDataList[9];
                                SongQuery_DataGridView.Rows[i].Cells["Song_MB"].Value = RestoreSongDataList[10];
                                SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Value = RestoreSongDataList[11];
                                SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value = RestoreSongDataList[12];
                                SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value = RestoreSongDataList[13];
                                SongQuery_DataGridView.Rows[i].Cells["Song_Spell"].Value = RestoreSongDataList[14];
                                SongQuery_DataGridView.Rows[i].Cells["Song_SpellNum"].Value = RestoreSongDataList[15];
                                SongQuery_DataGridView.Rows[i].Cells["Song_SongStroke"].Value = RestoreSongDataList[16];
                                SongQuery_DataGridView.Rows[i].Cells["Song_PenStyle"].Value = RestoreSongDataList[17];
                                SongQuery_DataGridView.Rows[i].Cells["Song_PlayState"].Value = RestoreSongDataList[18];

                                List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));

                                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
                                if (SelectedRowsCount > 1)
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
                                }
                                else if (SelectedRowsCount == 1)
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
                        }
                        catch
                        {
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫查詢】更新資料庫時發生錯誤: " + str;
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;

                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_QueryStatus_Label.Text = "修改歌曲資料有誤,請回報操作記錄裡的內容!";
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
            SongUpdateDT.Dispose();
            SongUpdateDT = null;

            int MaxDigitCode;
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = 5; } else { MaxDigitCode = 6; }
            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)));
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode)));
            tasks.Add(Task.Factory.StartNew(() => Common_GetSongStatisticsTask()));

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
                    foreach (string str in SongFilelist)
                    {
                        int i = SongFilelist.IndexOf(str);
                        bool RemoveError = false;

                        if (Global.SongMgrSongAddMode != "3")
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
            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetMaxSongId(MaxDigitCode)));
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetNotExistsSongId(MaxDigitCode)));
            tasks.Add(Task.Factory.StartNew(() => Common_GetSongStatisticsTask()));
            tasks.Add(Task.Factory.StartNew(() => CommonFunc.GetRemainingSongId((Global.SongMgrMaxDigitCode == "1") ? 5 : 6)));
            return RemoveSongIdlist;
        }

        #endregion

        #region --- SongQuery 異常查詢 --

        private void SongQuery_ExceptionalQuery_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Global.CrazyktvDatabaseStatus && SongQuery_ExceptionalQuery_ComboBox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
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
                        break;
                    case "3":
                        SongQueryType = "DuplicateSong";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "4":
                        SongQueryType = "DuplicateSongIgnoreSinger";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "5":
                        SongQueryType = "DuplicateSongIgnoreSongType";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "6":
                        SongQueryType = "DuplicateSongOnlyChorusSinger";
                        SongQueryValue = "NA";
                        SongQueryStatusText = SongQuery_ExceptionalQuery_ComboBox.Text;
                        SongQuery_QueryStatus_Label.Text = "正在查詢有關『" + SongQueryStatusText + "』的異常歌曲,請稍待...";
                        break;
                    case "7":
                        SongQueryType = "MatchSongTrack";
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
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

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
                            case "MatchSongTrack":
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");

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
                            default:
                                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                                break;
                        }
                        
                        if (dt.Rows.Count == 0)
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                SongQuery_EditMode_CheckBox.Enabled = false;
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

                                SongQuery_DataGridView.Columns["Song_FullPath"].Width = 640;
                                SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = 640;
                                SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                {
                                    SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                    SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                }

                                SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
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
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongQuery_FavoriteQueryTask(list)));

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
                            DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuery.GetSongQuerySqlStr(SongQueryType, SongQueryValue), "");
                            if (dt.Rows.Count == 0)
                            {
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

                                    SongQuery_DataGridView.Columns["Song_FullPath"].Width = 640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].MinimumWidth = 640;
                                    SongQuery_DataGridView.Columns["Song_FullPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                    for (int i = 0; i < SongQuery_DataGridView.Rows.Count; i++)
                                    {
                                        SongFullPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString() + SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                                        SongQuery_DataGridView.Rows[i].Cells["Song_FullPath"].Value = SongFullPath;
                                    }

                                    SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
                                    SongQuery_DataGridView.Focus();
                                    dt.Dispose();
                                }
                            }
                        }
                        catch
                        {
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
                    if (SongSinger.ContainsAny(Global.CrtchorusSeparateList.ToArray()))
                    {
                        List<string> list = new List<string>(Global.SongAddSpecialStr.Split(',')).ConvertAll(str => str.ToLower());
                        if (list.IndexOf(SongSinger.ToLower()) < 0) SongQuery_EditSongSingerType_ComboBox.SelectedValue = 5;
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    if (SongSinger.ContainsAny(Global.CrtchorusSeparateList.ToArray()))
                    {
                        List<string> list = new List<string>(Global.SongAddSpecialStr.Split(',')).ConvertAll(str => str.ToLower());
                        if (list.IndexOf(SongSinger.ToLower()) < 0)
                        {
                            SongQuery_EditSongSingerType_ComboBox.SelectedValue = 4;
                        }
                        else
                        {
                            if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                        }
                    }
                    else
                    {
                        if (Global.SongMgrSongAddMode != "3" && Global.SongMgrSongAddMode != "4") SongQuery_RefreshSongSrcPathTextBox();
                    }
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

                    string SongId = SongQuery_EditSongId_TextBox.Text;
                    string SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                    string SongSinger = SongQuery_EditSongSinger_TextBox.Text;
                    string SongSongName = SongQuery_EditSongSongName_TextBox.Text;
                    string SongTrack = SongQuery_EditSongTrack_ComboBox.SelectedValue.ToString();
                    string SongFilePath = list[19];

                    List<string> PlayerSongInfoList = new List<string>() { SongId, SongLang, SongSinger, SongSongName, SongTrack, SongFilePath, "0", "SongQueryEdit" };

                    Global.PlayerUpdateSongValueList = new List<string>();
                    PlayerForm newPlayerForm = new PlayerForm(this, PlayerSongInfoList);
                    newPlayerForm.Show();
                    this.Hide();
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

                DataTable dt = new DataTable();
                dt.Columns.Add("RowIndex", typeof(int));
                dt.Columns.Add("SongId", typeof(string));
                dt.Columns.Add("SongLang", typeof(string));

                SongQuery.CreateSongDataTable();
                Common_SwitchSetUI(false);

                if (SelectedRowsCount > 1)
                {
                    foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                    {
                        DataRow dtrow = dt.NewRow();
                        dtrow["RowIndex"] = row.Index;
                        dtrow["SongId"] = row.Cells["Song_Id"].Value.ToString();
                        dtrow["SongLang"] = row.Cells["Song_Lang"].Value.ToString();
                        dt.Rows.Add(dtrow);

                        if (Global.SongQueryMultiEditUpdateList[0])
                        {
                            string SongLang = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();
                            if (row.Cells["Song_Lang"].Value.ToString() != SongLang)
                            {
                                string SongId = SongQuery.GetNextSongId(SongLang, true);
                                row.Cells["Song_Id"].Value = SongId;
                                row.Cells["Song_Lang"].Value = SongLang;
                            }
                        }

                        if (Global.SongQueryMultiEditUpdateList[1])
                        {
                            string SongCreatDate = SongQuery_EditSongCreatDate_DateTimePicker.Value.ToString();
                            row.Cells["Song_CreatDate"].Value = SongCreatDate;
                        }

                        if (Global.SongQueryMultiEditUpdateList[2])
                        {
                            string SongSinger = SongQuery_EditSongSinger_TextBox.Text;
                            row.Cells["Song_Singer"].Value = SongSinger;
                        }

                        if (Global.SongQueryMultiEditUpdateList[3])
                        {
                            string SongSingerTypeStr = ((DataRowView)SongQuery_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                            string SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                            row.Cells["Song_SingerType"].Value = SongSingerType;
                        }

                        if (Global.SongQueryMultiEditUpdateList[4])
                        {
                            string SongSongType = ((DataRowView)SongQuery_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                            row.Cells["Song_SongType"].Value = (SongSongType != "無類別") ? SongSongType : "";
                        }

                        if (Global.SongQueryMultiEditUpdateList[5])
                        {
                            int SongTrack = (int)SongQuery_EditSongTrack_ComboBox.SelectedValue - 1;
                            row.Cells["Song_Track"].Value = SongTrack;
                        }

                        if (Global.SongQueryMultiEditUpdateList[6])
                        {
                            string SongVolume = SongQuery_EditSongVolume_TextBox.Text;
                            row.Cells["Song_Volume"].Value = SongVolume;
                        }

                        if (Global.SongQueryMultiEditUpdateList[7])
                        {
                            string SongPlayCount = SongQuery_EditSongPlayCount_TextBox.Text;
                            row.Cells["Song_PlayCount"].Value = SongPlayCount;
                        }
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                    {
                        DataRow dtrow = dt.NewRow();
                        dtrow["RowIndex"] = row.Index;
                        dtrow["SongId"] = row.Cells["Song_Id"].Value.ToString();
                        dtrow["SongLang"] = row.Cells["Song_Lang"].Value.ToString();
                        dt.Rows.Add(dtrow);

                        row.Cells["Song_Id"].Value = SongQuery_EditSongId_TextBox.Text;
                        row.Cells["Song_Lang"].Value = ((DataRowView)SongQuery_EditSongLang_ComboBox.SelectedItem)[0].ToString();

                        string SongCreatDate = SongQuery_EditSongCreatDate_DateTimePicker.Value.ToString();
                        row.Cells["Song_CreatDate"].Value = SongCreatDate;

                        row.Cells["Song_Singer"].Value = SongQuery_EditSongSinger_TextBox.Text;

                        string SongSingerTypeStr = ((DataRowView)SongQuery_EditSongSingerType_ComboBox.SelectedItem)[0].ToString();
                        string SongSingerType = CommonFunc.GetSingerTypeStr(0, 1, SongSingerTypeStr);
                        row.Cells["Song_SingerType"].Value = SongSingerType;

                        string SongSongName = SongQuery_EditSongSongName_TextBox.Text;
                        row.Cells["Song_SongName"].Value = SongSongName;

                        string SongSongType = ((DataRowView)SongQuery_EditSongSongType_ComboBox.SelectedItem)[0].ToString();
                        row.Cells["Song_SongType"].Value = (SongSongType != "無類別") ? SongSongType : "";

                        // 計算歌曲字數
                        List<string> SongWordCountList = new List<string>();
                        SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);

                        // 取得歌曲拼音
                        List<string> SongSpellList = new List<string>();
                        SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                        if (SongSpellList[2] == "") SongSpellList[2] = "0";

                        row.Cells["Song_Spell"].Value = SongSpellList[0];
                        row.Cells["Song_SpellNum"].Value = SongSpellList[1];
                        row.Cells["Song_SongStroke"].Value = SongSpellList[2];
                        row.Cells["Song_PenStyle"].Value = SongSpellList[3];
                        row.Cells["Song_WordCount"].Value = SongWordCountList[0];
                        row.Cells["Song_Track"].Value = SongQuery_EditSongTrack_ComboBox.SelectedValue;
                        row.Cells["Song_Volume"].Value = SongQuery_EditSongVolume_TextBox.Text;
                        row.Cells["Song_PlayCount"].Value = SongQuery_EditSongPlayCount_TextBox.Text;
                    }
                }

                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                    });
                    SongQuery.DisposeSongDataTable();
                    dt.Dispose();
                    dt = null;
                });
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

            SongQuery_EditSongSingerType_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SingerType", MultiEdit);
            SongQuery_EditSongSingerType_ComboBox.DisplayMember = "Display";
            SongQuery_EditSongSingerType_ComboBox.ValueMember = "Value";

            SongQuery_EditSongSongType_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongType", MultiEdit);
            SongQuery_EditSongSongType_ComboBox.DisplayMember = "Display";
            SongQuery_EditSongSongType_ComboBox.ValueMember = "Value";

            SongQuery_EditSongTrack_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", MultiEdit);
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

            SongQuery_EditSongSrcPath_TextBox.Text = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, (SongSongType != "無類別") ? SongSongType : "", SongFileName, SongPath);
        }





        #endregion

    }



    class SongQuery
    {
        public static bool QueryStatusLabel = true;

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

                List<string> ItemList = new List<string>() { "歌曲名稱", "歌手名稱", "歌曲編號", "新進歌曲", "合唱歌曲", "歌曲類別", "歌手類別", "歌曲聲道", "錢櫃編號" };

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

        public static DataTable GetSongQueryValueList(string ValueType, bool MultiEdit)
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
                        break;
                }
                return list;
            }
        }

        public static DataTable GetSongQueryFilterList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));
                list.Rows.Add(list.NewRow());
                list.Rows[0][0] = "全部";
                list.Rows[0][1] = 1;

                foreach (string str in Global.CrazyktvSongLangList)
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

                List<string> ItemList = new List<string>() { "無檔案歌曲", "同檔案歌曲", "重複歌曲", "重複歌曲 (忽略歌手)", "重複歌曲 (忽略類別)", "重複歌曲 (合唱歌曲)", "檔名不符 (歌曲聲道)" };

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

        public static string GetSongQuerySqlStr(string QueryType, string QueryValue)
        {
            string sqlCommonStr = " Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_SongType, Song_Track, Song_Volume, Song_WordCount, Song_PlayCount, Song_MB, Song_CreatDate, Song_FileName, Song_Path, Song_Spell, Song_SpellNum, Song_SongStroke, Song_PenStyle, Song_PlayState ";
            string SongQuerySqlStr = "";
            string SongQueryOrderStr = " order by Song_Id";
            string SongQueryFilterStr = "";
            string QueryValueNarrow = QueryValue;
            string QueryValueWide = QueryValue;
            string HasWideCharQueryValue = QueryValue;

            Global.SongQueryHasWideChar = false;

            Regex HasWideChar = new Regex("[\x21-\x7E\xFF01-\xFF5E]");
            if (QueryType == "SongName" | QueryType == "SingerName")
            {
                if (Global.SongQueryFuzzyQuery == "True")
                {
                    if (HasWideChar.IsMatch(HasWideCharQueryValue))
                    {
                        Global.SongQueryHasWideChar = true;
                        QueryValueNarrow = CommonFunc.ConvToNarrow(QueryValue);
                        QueryValueWide = CommonFunc.ConvToWide(QueryValue);
                        HasWideCharQueryValue = Regex.Replace(HasWideCharQueryValue, "[\x21-\x7E\xFF01-\xFF5E]", "", RegexOptions.IgnoreCase);
                        if (HasWideCharQueryValue == "") HasWideCharQueryValue = QueryValue;
                    }
                }

                Regex HasSymbols = new Regex("[']");
                if (HasSymbols.IsMatch(QueryValue))
                {
                    QueryValue = Regex.Replace(QueryValue, "[']", delegate(Match match)
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

            if (Global.SongQueryFilter != "全部")
            {
                SongQueryFilterStr = " and Song_Lang = '" + Global.SongQueryFilter + "'";
            }

            switch (QueryType)
            {
                case "SongName":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        if (Global.SongQueryHasWideChar)
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_SongName),LCase('" + HasWideCharQueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_SongName),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                    }
                    else
                    {
                        SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where Song_SongName = '" + QueryValue + "'" + SongQueryFilterStr;
                    }
                    break;
                case "SingerName":
                    if (Global.SongQueryFuzzyQuery == "True")
                    {
                        if (Global.SongQueryHasWideChar)
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValueNarrow + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + QueryValueWide + "'),0) <>0" + SongQueryFilterStr + " or InStr(1,LCase(Song_Singer),LCase('" + HasWideCharQueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
                        else
                        {
                            SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where InStr(1,LCase(Song_Singer),LCase('" + QueryValue + "'),0) <>0" + SongQueryFilterStr;
                        }
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
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_Singer, Song_SongType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_Singer = ktv_Song.Song_Singer and Song_SongType = ktv_Song.Song_SongType))) order by Song_SongName";
                    break;
                case "DuplicateSongIgnoreSinger":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_SongType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_SongType = ktv_Song.Song_SongType))) order by Song_SongName";
                    break;
                case "DuplicateSongIgnoreSongType":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_Singer HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_Singer = ktv_Song.Song_Singer))) order by Song_SongName";
                    break;
                case "DuplicateSongOnlyChorusSinger":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song where (((Song_SongName) In (select Song_SongName from ktv_Song As Tmp group by Song_SongName, Song_Lang, Song_SongType, Song_SingerType HAVING Count(*)>1 and Song_SongName = ktv_Song.Song_SongName and Song_Lang = ktv_Song.Song_Lang and Song_SongType = ktv_Song.Song_SongType and Song_SingerType = 3))) order by Song_SongName";
                    break;
                case "MatchSongTrack":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
                case "FavoriteSong":
                    SongQuerySqlStr = "select" + sqlCommonStr + "from ktv_Song order by Song_Id";
                    break;
            }

            return SongQuerySqlStr;
        }

        #endregion

        #region --- SongQuery 歌曲列表欄位設定 ---

        public static List<string> GetDataGridViewColumnSet(string ColumnName)
        {
            List<string> list = new List<string>();

            // List<string>() { "欄位名稱", "欄位寬度", "欄位字數" };
            switch (ColumnName)
            {
                case "Song_Id":
                    list = new List<string>() { "歌曲編號", "120", "6" };
                    break;
                case "Song_Lang":
                    list = new List<string>() { "語系類別", "120", "none" };
                    break;
                case "Song_SingerType":
                    list = new List<string>() { "歌手類別", "120", "none" };
                    break;
                case "Song_Singer":
                    list = new List<string>() { "歌手名稱", "160", "none" };
                    break;
                case "Song_SongName":
                    list = new List<string>() { "歌曲名稱", "270", "none" };
                    break;
                case "Song_Track":
                    list = new List<string>() { "歌曲聲道", "140", "none" };
                    break;
                case "Song_SongType":
                    list = new List<string>() { "歌曲類別", "120", "none" };
                    break;
                case "Song_Volume":
                    list = new List<string>() { "歌曲音量", "120", "3" };
                    break;
                case "Song_WordCount":
                    list = new List<string>() { "歌曲字數", "120", "2" };
                    break;
                case "Song_PlayCount":
                    list = new List<string>() { "點播次數", "120", "9" };
                    break;
                case "Song_MB":
                    list = new List<string>() { "歌曲大小", "120", "7" };
                    break;
                case "Song_CreatDate":
                    list = new List<string>() { "加歌日期", "140", "none" };
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
                    list = new List<string>() { "開啟資料夾", "播放檔案" };
                    break;
                case 3:
                    list = new List<string>() { "開啟資料夾", "播放檔案", "從我的最愛移除" };
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
            if (Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Count > 0)
            {
                NewSongID = Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)][0];
                if (UpdateIdList) Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(SongLang)].Remove(NewSongID);
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
