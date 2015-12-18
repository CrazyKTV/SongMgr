using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{

    public partial class MainForm : Form
    {
        #region --- SongQuery_DataGridView ---

        private void SongQuery_DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int val;
            string valStr = "";
            switch (SongQuery_DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "Song_SingerType":
                    val = Convert.ToInt32(e.Value);
                    valStr = CommonFunc.GetSingerTypeStr(val, 1, "null");
                    e.Value = valStr;
                    e.FormattingApplied = true;
                    break;
                case "Song_Track":
                    val = Convert.ToInt32(e.Value);
                    valStr = CommonFunc.GetSongTrackStr(val - 1, 0, "null");
                    e.Value = valStr;
                    e.FormattingApplied = true;
                    break;
                case "Song_CreatDate":
                    e.Value = DateTime.Parse(e.Value.ToString()).ToString("yyyy/MM/dd");
                    e.FormattingApplied = true;
                    break;
                case "Song_Id":
                case "Song_WordCount":
                case "Song_MB":
                case "Song_FullPath":
                    e.CellStyle.ForeColor = Color.Blue;
                    break;
            }
        }

        private void SongQuery_DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌曲音量":
                    if (e.FormattedValue.ToString() == "")
                    {
                        SongQuery_QueryStatus_Label.Text = "此項目的值不可為空白!";
                        e.Cancel = true;
                    }
                    else if (Convert.ToInt32(e.FormattedValue) > 100)
                    {
                        SongQuery_QueryStatus_Label.Text = "此項目只能輸入 0 ~ 100 的值!";
                        e.Cancel = true;
                    }
                    break;
                case "歌手名稱":
                case "歌曲名稱":
                    if (e.FormattedValue.ToString() == "")
                    {
                        SongQuery_QueryStatus_Label.Text = "此項目的值不可為空白!";
                        e.Cancel = true;
                    }
                    else
                    {
                        Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
                        if (r.IsMatch(e.FormattedValue.ToString()))
                        {
                            SongQuery_QueryStatus_Label.Text = "此項目的值含有非法字元!";
                            e.Cancel = true;
                        }
                    }
                    break;
                case "點播次數":
                    if (e.FormattedValue.ToString() == "")
                    {
                        SongQuery_QueryStatus_Label.Text = "此項目的值不可為空白!";
                        e.Cancel = true;
                    }
                    break;
            }
        }

        private void SongQuery_DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (SongQuery_DataGridView.CurrentCell.ColumnIndex < 0) return;
            Control parentCTL = e.Control.Parent;

            switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌曲編號":
                case "歌曲音量":
                case "歌曲字數":
                case "點播次數":
                case "歌曲大小":
                    e.Control.KeyPress += new KeyPressEventHandler(SongQuery_DataGridView_Keyin_Number);
                    break;
                case "加歌日期":
                    DateTimePicker dtPicker = new DateTimePicker();
                    dtPicker.Name = "dateTimePicker";
                    dtPicker.Size = SongQuery_DataGridView.CurrentCell.Size;
                    dtPicker.CustomFormat = "yyyy/MM/dd";
                    dtPicker.Format = DateTimePickerFormat.Custom;
                    dtPicker.Location = new Point(e.Control.Location.X - e.Control.Margin.Left < 0 ? 0 : e.Control.Location.X - e.Control.Margin.Left, e.Control.Location.Y - e.Control.Margin.Top < 0 ? 0 : e.Control.Location.Y - e.Control.Margin.Top);

                    if (e.Control.Text != "")
                    {
                        string DateParse = DateTime.Parse(e.Control.Text).ToString("yyyy/MM/dd");
                        dtPicker.Value = DateTime.ParseExact(DateParse, dtPicker.CustomFormat, null);
                    }
                    e.Control.Visible = false;

                    foreach (Control tmpCTL in parentCTL.Controls)
                    {
                        if (tmpCTL.Name == dtPicker.Name) parentCTL.Controls.Remove(tmpCTL);
                    }
                    parentCTL.Controls.Add(dtPicker);
                    dtPicker.CloseUp += new EventHandler(SongQuery_DataGridView_DateTimePicker_CloseUp);
                    break;
            }

            if (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText != "加歌日期")
            {
                foreach (Control tmpCTL in parentCTL.Controls)
                {
                    if (tmpCTL.Name == "dateTimePicker") parentCTL.Controls.Remove(tmpCTL);
                }
            }
        }

        private void SongQuery_DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Global.SongQueryDataGridViewValue = SongQuery_DataGridView.CurrentCell.Value.ToString();
        }

        private void SongQuery_DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (SongQuery_DataGridView.CurrentCell.Value.ToString() == "") return;
            switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌手名稱":
                    string SongSinger = SongQuery_DataGridView.CurrentCell.Value.ToString();
                    if (SongSinger.ContainsAny("&", "+")) SongQuery_DataGridView.CurrentRow.Cells["Song_SingerType"].Value = "3";
                    break;
                case "歌曲名稱":
                    string SongSongName = SongQuery_DataGridView.CurrentCell.Value.ToString();
                    // 計算歌曲字數
                    List<string> SongWordCountList = new List<string>();
                    SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                    SongQuery_DataGridView.CurrentRow.Cells["Song_WordCount"].Value = SongWordCountList[0];

                    // 取得歌曲拼音
                    List<string> SongSpellList = new List<string>();
                    SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                    SongQuery_DataGridView.CurrentRow.Cells["Song_Spell"].Value = SongSpellList[0];
                    SongQuery_DataGridView.CurrentRow.Cells["Song_SpellNum"].Value = SongSpellList[1];
                    if (SongSpellList[2] == "") SongSpellList[2] = "0";
                    SongQuery_DataGridView.CurrentRow.Cells["Song_SongStroke"].Value = SongSpellList[2];
                    SongQuery_DataGridView.CurrentRow.Cells["Song_PenStyle"].Value = SongSpellList[3];
                    break;
            }

            if(Global.SongQueryDataGridViewValue != SongQuery_DataGridView.CurrentCell.Value.ToString())
            {
                switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
                {
                    case "歌手名稱":
                    case "歌曲名稱":
                    case "歌曲音量":
                    case "點播次數":
                        DataTable dt = new DataTable();
                        dt.Columns.Add("RowIndex", typeof(int));
                        dt.Columns.Add("SongId", typeof(string));
                        dt.Columns.Add("SongLang", typeof(string));

                        DataRow row = dt.NewRow();
                        row["RowIndex"] = SongQuery_DataGridView.CurrentRow.Index;
                        row["SongId"] = SongQuery_DataGridView.CurrentRow.Cells["Song_Id"].Value.ToString();
                        row["SongLang"] = SongQuery_DataGridView.CurrentRow.Cells["Song_Lang"].Value.ToString();
                        dt.Rows.Add(row);

                        Common_SwitchSetUI(false);

                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Common_SwitchSetUI(true);
                            });
                            dt.Dispose();
                        });
                        break;
                }
            }
            if (SongQuery_QueryStatus_Label.Text == "此項目只能輸入數字!" | SongQuery_QueryStatus_Label.Text == "此項目只能輸入 0 ~ 100 的值!" | SongQuery_QueryStatus_Label.Text == "此項目只能輸入數字及小數點!" | SongQuery_QueryStatus_Label.Text == "此項目的值不可為空白!" | SongQuery_QueryStatus_Label.Text == "此項目的值含有非法字元!") SongQuery_QueryStatus_Label.Text = "";
        }

        private void SongQuery_DataGridView_Keyin_Number(object sender, KeyPressEventArgs e)
        {
            switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌曲編號":
                case "歌曲音量":
                case "歌曲字數":
                case "點播次數":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13 & (int)e.KeyChar != 27)
                    {
                        e.Handled = true;
                        SongQuery_QueryStatus_Label.Text = "此項目只能輸入數字!";
                    }
                    else
                    {
                        if (SongQuery_QueryStatus_Label.Text == "此項目只能輸入數字!") SongQuery_QueryStatus_Label.Text = "";
                    }
                    break;
                case "歌曲大小":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13 & (int)e.KeyChar != 27 & (int)e.KeyChar != 46)
                    {
                        e.Handled = true;
                        SongQuery_QueryStatus_Label.Text = "此項目只能輸入數字及小數點!";
                    }
                    else
                    {
                        if (SongQuery_QueryStatus_Label.Text == "此項目只能輸入數字及小數點!") SongQuery_QueryStatus_Label.Text = "";
                    }
                    break;
            }
        }

        private void SongQuery_DataGridView_DateTimePicker_CloseUp(Object sender, EventArgs e)
        {
            string DateTimeValue = ((DateTimePicker)sender).Value.ToString("yyyy/M/d") + " " + DateTime.Now.ToString("tt hh:mm:ss");
            int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

            if (SongQuery_DataGridView.SelectedRows.Count > 1)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("RowIndex", typeof(int));
                dt.Columns.Add("SongId", typeof(string));
                dt.Columns.Add("SongLang", typeof(string));

                for (int i = 0; i < SelectedRowsCount; i++)
                {
                    if (SongQuery_DataGridView.SelectedRows[i].Cells["Song_CreatDate"].Value.ToString() != DateTimeValue)
                    {
                        SongQuery_DataGridView.SelectedRows[i].Cells["Song_CreatDate"].Value = DateTimeValue;

                        DataRow row = dt.NewRow();
                        row["RowIndex"] = SongQuery_DataGridView.SelectedRows[i].Index;
                        row["SongId"] = SongQuery_DataGridView.SelectedRows[i].Cells["Song_Id"].Value.ToString();
                        row["SongLang"] = SongQuery_DataGridView.SelectedRows[i].Cells["Song_Lang"].Value.ToString();
                        dt.Rows.Add(row);
                    }
                }
                
                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                    });
                    dt.Dispose();
                });
            }
            else
            {
                if (SongQuery_DataGridView.CurrentCell.Value.ToString() != DateTimeValue)
                {
                    SongQuery_DataGridView.CurrentCell.Value = DateTimeValue;

                    DataTable dt = new DataTable();
                    dt.Columns.Add("RowIndex", typeof(int));
                    dt.Columns.Add("SongId", typeof(string));
                    dt.Columns.Add("SongLang", typeof(string));

                    DataRow row = dt.NewRow();
                    row["RowIndex"] = SongQuery_DataGridView.CurrentRow.Index;
                    row["SongId"] = SongQuery_DataGridView.CurrentRow.Cells["Song_Id"].Value.ToString();
                    row["SongLang"] = SongQuery_DataGridView.CurrentRow.Cells["Song_Lang"].Value.ToString();
                    dt.Rows.Add(row);

                    Common_SwitchSetUI(false);
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Common_SwitchSetUI(true);
                        });
                        dt.Dispose();
                    });
                }
            }

            SongQuery_DataGridView.EndEdit();
        }

        private void SongQuery_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0))
            {
                SongQuery_DataGridView.EndEdit();
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (SongQuery_EditMode_CheckBox.Checked == true)
                {
                    string EditCell = SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText;
                    List<string> list = new List<string>();
                    list = new List<string>() { "歌手名稱", "歌曲名稱", "歌曲音量", "點播次數", "加歌日期" };

                    var query = from editlist in list
                                where editlist == EditCell
                                select editlist;
                    foreach (var q in query)
                    {
                        SongQuery_DataGridView.BeginEdit(true);
                    }

                    ContextMenuStrip GridView_ContextMenu;
                    ToolStripMenuItem[] GridView_ContextMenuItem;
                    string valStr = "";

                    switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
                    {
                        case "語系類別":
                            GridView_ContextMenu = new ContextMenuStrip();
                            if (GridView_ContextMenu != null) GridView_ContextMenu.Dispose();
                            GridView_ContextMenuItem = new ToolStripMenuItem[10];
                            GridView_ContextMenu = new ContextMenuStrip();
                            for (int i = 0; i < 10; i++)
                            {
                                valStr = CommonFunc.GetSongLangStr(i, 0, "null");
                                GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                                GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_Click);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                            break;
                        case "歌手類別":
                            GridView_ContextMenu = new ContextMenuStrip();
                            if (GridView_ContextMenu != null) GridView_ContextMenu.Dispose();
                            GridView_ContextMenuItem = new ToolStripMenuItem[8];
                            GridView_ContextMenu = new ContextMenuStrip();
                            for (int i = 0; i < 8; i++)
                            {
                                valStr = CommonFunc.GetSingerTypeStr(i, 0, "null");
                                GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                                GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_Click);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                            break;
                        case "歌曲聲道":
                            GridView_ContextMenu = new ContextMenuStrip();
                            if (GridView_ContextMenu != null) GridView_ContextMenu.Dispose();
                            GridView_ContextMenuItem = new ToolStripMenuItem[5];
                            GridView_ContextMenu = new ContextMenuStrip();
                            for (int i = 0; i < 5; i++)
                            {
                                valStr = CommonFunc.GetSongTrackStr(i, 0, "null");
                                GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                                GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_Click);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                            break;
                        case "歌曲類別":
                            GridView_ContextMenu = new ContextMenuStrip();
                            if (GridView_ContextMenu != null) GridView_ContextMenu.Dispose();
                            int count = SongMgrCfg_SongType_ListBox.Items.Count;
                            GridView_ContextMenuItem = new ToolStripMenuItem[count + 1];
                            GridView_ContextMenu = new ContextMenuStrip();

                            GridView_ContextMenuItem[0] = new ToolStripMenuItem("無類別");
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[0]);
                            GridView_ContextMenuItem[0].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_Click);

                            for (int i = 0; i < count; i++)
                            {
                                valStr = SongQuery.GetSongTypeStr(i);
                                GridView_ContextMenuItem[i + 1] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i + 1]);
                                GridView_ContextMenuItem[i + 1].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_Click);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                            break;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                string valStr = "";

                if (SongQuery_EditMode_CheckBox.Checked == true)
                {
                    if (!SongQuery_DataGridView.Rows[e.RowIndex].Selected)
                    {
                        SongQuery_DataGridView.ClearSelection();
                        SongQuery_DataGridView.Rows[e.RowIndex].Selected = true;
                        SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }

                    if (SongQuery_DataGridView.SelectedRows.Count > 1)
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        if (Global.FavoriteUserDT.Rows.Count > 0) { GridView_ContextMenuItem = new ToolStripMenuItem[2]; } else { GridView_ContextMenuItem = new ToolStripMenuItem[1]; }
                        
                        for (int i = 0; i < 1; i++)
                        {
                            valStr = SongQuery.GetContextMenuStr(i, 0);
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_RightClick);
                        }

                        if (Global.FavoriteUserDT.Rows.Count > 0)
                        {
                            GridView_ContextMenuItem[1] = new ToolStripMenuItem("加入我的最愛");
                            ToolStripMenuItem[] GridView_ContextMenuSubItem = new ToolStripMenuItem[Global.FavoriteUserDT.Rows.Count];

                            foreach (DataRow row in Global.FavoriteUserDT.AsEnumerable())
                            {
                                GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)] = new ToolStripMenuItem(row["User_Name"].ToString());
                                GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)].Click += new EventHandler(SongQuery_DataGridView_FavoriteUserContextMenuItem_RightClick);
                                GridView_ContextMenuItem[1].DropDown.Items.Add(GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)]);
                            }

                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[1]);
                        }

                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                    else
                    {
                        SongQuery_DataGridView.ClearSelection();
                        SongQuery_DataGridView.Rows[e.RowIndex].Selected = true;
                        SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                        GridView_ContextMenu = new ContextMenuStrip();
                        if (Global.FavoriteUserDT.Rows.Count > 0) { GridView_ContextMenuItem = new ToolStripMenuItem[4]; } else { GridView_ContextMenuItem = new ToolStripMenuItem[3]; }

                        for (int i = 0; i < 3; i++)
                        {
                            valStr = SongQuery.GetContextMenuStr(i, 1);
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_RightClick);
                        }

                        if (Global.FavoriteUserDT.Rows.Count > 0)
                        {
                            GridView_ContextMenuItem[3] = new ToolStripMenuItem("加入我的最愛");
                            ToolStripMenuItem[] GridView_ContextMenuSubItem = new ToolStripMenuItem[Global.FavoriteUserDT.Rows.Count];

                            foreach (DataRow row in Global.FavoriteUserDT.AsEnumerable())
                            {
                                GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)] = new ToolStripMenuItem(row["User_Name"].ToString());
                                GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)].Click += new EventHandler(SongQuery_DataGridView_FavoriteUserContextMenuItem_RightClick);
                                GridView_ContextMenuItem[3].DropDown.Items.Add(GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)]);
                            }

                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[3]);
                        }

                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                }
                else
                {
                    if (!SongQuery_DataGridView.Rows[e.RowIndex].Selected)
                    {
                        SongQuery_DataGridView.ClearSelection();
                        SongQuery_DataGridView.Rows[e.RowIndex].Selected = true;
                        SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }

                    if (SongQuery_DataGridView.SelectedRows.Count > 1)
                    {
                        if (Global.SongQueryQueryType == "FavoriteQuery")
                        {
                            GridView_ContextMenu = new ContextMenuStrip();
                            GridView_ContextMenuItem = new ToolStripMenuItem[1];

                            if (Global.FavoriteUserDT.Rows.Count > 0)
                            {
                                GridView_ContextMenuItem[0] = new ToolStripMenuItem("從我的最愛移除");
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[0]);
                                GridView_ContextMenuItem[0].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_RightClick);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        }
                        else
                        {
                            GridView_ContextMenu = new ContextMenuStrip();
                            GridView_ContextMenuItem = new ToolStripMenuItem[1];

                            if (Global.FavoriteUserDT.Rows.Count > 0)
                            {
                                GridView_ContextMenuItem[0] = new ToolStripMenuItem("加入我的最愛");
                                ToolStripMenuItem[] GridView_ContextMenuSubItem = new ToolStripMenuItem[Global.FavoriteUserDT.Rows.Count];

                                foreach (DataRow row in Global.FavoriteUserDT.AsEnumerable())
                                {
                                    GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)] = new ToolStripMenuItem(row["User_Name"].ToString());
                                    GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)].Click += new EventHandler(SongQuery_DataGridView_FavoriteUserContextMenuItem_RightClick);
                                    GridView_ContextMenuItem[0].DropDown.Items.Add(GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)]);
                                }

                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[0]);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        }
                    }
                    else
                    {
                        if (Global.SongQueryQueryType == "FavoriteQuery")
                        {
                            SongQuery_DataGridView.ClearSelection();
                            SongQuery_DataGridView.Rows[e.RowIndex].Selected = true;
                            SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                            GridView_ContextMenu = new ContextMenuStrip();
                            GridView_ContextMenuItem = new ToolStripMenuItem[3];
                            
                            for (int i = 0; i < 3; i++)
                            {
                                valStr = SongQuery.GetContextMenuStr(i, 3);
                                GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                                GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_RightClick);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        }
                        else
                        {
                            SongQuery_DataGridView.ClearSelection();
                            SongQuery_DataGridView.Rows[e.RowIndex].Selected = true;
                            SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                            GridView_ContextMenu = new ContextMenuStrip();
                            if (Global.FavoriteUserDT.Rows.Count > 0) { GridView_ContextMenuItem = new ToolStripMenuItem[3]; } else { GridView_ContextMenuItem = new ToolStripMenuItem[2]; }

                            for (int i = 0; i < 2; i++)
                            {
                                valStr = SongQuery.GetContextMenuStr(i, 2);
                                GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                                GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_RightClick);
                            }

                            if (Global.FavoriteUserDT.Rows.Count > 0)
                            {
                                GridView_ContextMenuItem[2] = new ToolStripMenuItem("加入我的最愛");
                                ToolStripMenuItem[] GridView_ContextMenuSubItem = new ToolStripMenuItem[Global.FavoriteUserDT.Rows.Count];

                                foreach (DataRow row in Global.FavoriteUserDT.AsEnumerable())
                                {
                                    GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)] = new ToolStripMenuItem(row["User_Name"].ToString());
                                    GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)].Click += new EventHandler(SongQuery_DataGridView_FavoriteUserContextMenuItem_RightClick);
                                    GridView_ContextMenuItem[2].DropDown.Items.Add(GridView_ContextMenuSubItem[Global.FavoriteUserDT.Rows.IndexOf(row)]);
                                }

                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[2]);
                            }

                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        }
                    }
                }
            }
        }

        private void SongQuery_DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == false)
            {
                if (e.RowIndex < 0) return;

                int i = e.RowIndex;

                string SongId = SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                string SongLang = SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value.ToString();
                string SongSinger = SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value.ToString();
                string SongSongName = SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value.ToString();
                string SongTrack = SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Value.ToString();
                string SongFileName = SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                string SongPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString();
                string SongFilePath = Path.Combine(SongPath, SongFileName);

                List<string> PlayerSongInfoList = new List<string>() { SongId, SongLang, SongSinger, SongSongName, SongTrack, SongFilePath, i.ToString(), "SongQuery" };

                Global.PlayerUpdateSongValueList = new List<string>();
                PlayerForm newPlayerForm = new PlayerForm(this, PlayerSongInfoList);
                newPlayerForm.Show();
                this.Hide();
            }
        }

        private void SongQuery_DataGridView_ContextMenuItem_Click(object sender, EventArgs e)
        {
            int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
            string CellName = "";
            string CellValue = "";

            switch (SongQuery_DataGridView.Columns[SongQuery_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "語系類別":
                    CellName = "Song_Lang";
                    CellValue = sender.ToString();
                    break;
                case "歌手類別":
                    CellName = "Song_SingerType";
                    CellValue = CommonFunc.GetSingerTypeStr(0, 1, sender.ToString());
                    break;
                case "歌曲聲道":
                    CellName = "Song_Track";
                    string Value = CommonFunc.GetSongTrackStr(0, 0, sender.ToString());
                    CellValue = Convert.ToString(int.Parse(Value) + 1);
                    break;
                case "歌曲類別":
                    CellName = "Song_SongType";
                    if (sender.ToString() == "無類別") { CellValue = ""; } else { CellValue = sender.ToString(); }
                    break;
            }

            if (SongQuery_DataGridView.SelectedRows.Count > 1)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("RowIndex", typeof(int));
                dt.Columns.Add("SongId", typeof(string));
                dt.Columns.Add("SongLang", typeof(string));

                for (int i = 0; i < SelectedRowsCount; i++)
                {
                    if (CellName == "Song_Lang")
                    {
                        if (SongQuery_DataGridView.SelectedRows[i].Cells[CellName].Value.ToString() != CellValue)
                        {
                            string NewSongID = "";
                            string OldSongID = SongQuery_DataGridView.SelectedRows[i].Cells["Song_Id"].Value.ToString();
                            string OldSongLang = SongQuery_DataGridView.SelectedRows[i].Cells[CellName].Value.ToString();

                            string SongSinger = SongQuery_DataGridView.SelectedRows[i].Cells["Song_Singer"].Value.ToString();
                            string SongSongName = SongQuery_DataGridView.SelectedRows[i].Cells["Song_SongName"].Value.ToString();
                            string SongSongType = SongQuery_DataGridView.SelectedRows[i].Cells["Song_SongType"].Value.ToString();

                            SongQuery_DataGridView.SelectedRows[i].Cells["Song_Id"].Value = NewSongID;
                            SongQuery_DataGridView.SelectedRows[i].Cells[CellName].Value = CellValue;

                            DataRow row1 = dt.NewRow();
                            row1["RowIndex"] = SongQuery_DataGridView.SelectedRows[i].Index;
                            row1["SongId"] = OldSongID;
                            row1["SongLang"] = OldSongLang;
                            dt.Rows.Add(row1);
                        }
                    }
                    else
                    {
                        if (SongQuery_DataGridView.SelectedRows[i].Cells[CellName].Value.ToString() != CellValue)
                        {
                            SongQuery_DataGridView.SelectedRows[i].Cells[CellName].Value = CellValue;
                            
                            DataRow row = dt.NewRow();
                            row["RowIndex"] = SongQuery_DataGridView.SelectedRows[i].Index;
                            row["SongId"] = SongQuery_DataGridView.SelectedRows[i].Cells["Song_Id"].Value.ToString();
                            row["SongLang"] = SongQuery_DataGridView.SelectedRows[i].Cells["Song_Lang"].Value.ToString();
                            dt.Rows.Add(row);
                        }
                    }
                }

                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        SongQuery_DataGridView.EndEdit();
                    });
                    dt.Dispose();
                });
            }
            else
            {
                if (CellName == "Song_Lang")
                {
                    if (SongQuery_DataGridView.CurrentCell.Value.ToString() != CellValue)
                    {
                        string NewSongID = "";
                        string OldSongID = SongQuery_DataGridView.CurrentRow.Cells["Song_Id"].Value.ToString();
                        string OldSongLang = SongQuery_DataGridView.CurrentRow.Cells[CellName].Value.ToString();

                        SongQuery_DataGridView.CurrentRow.Cells["Song_Id"].Value = NewSongID;
                        SongQuery_DataGridView.CurrentCell.Value = CellValue;

                        DataTable dt = new DataTable();
                        dt.Columns.Add("RowIndex", typeof(int));
                        dt.Columns.Add("SongId", typeof(string));
                        dt.Columns.Add("SongLang", typeof(string));

                        DataRow row1 = dt.NewRow();
                        row1["RowIndex"] = SongQuery_DataGridView.CurrentRow.Index;
                        row1["SongId"] = OldSongID;
                        row1["SongLang"] = OldSongLang;
                        dt.Rows.Add(row1);

                        Common_SwitchSetUI(false);
                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Common_SwitchSetUI(true);
                            });
                            dt.Dispose();
                        });
                    }
                }
                else
                {
                    if (SongQuery_DataGridView.CurrentCell.Value.ToString() != CellValue)
                    {
                        SongQuery_DataGridView.CurrentCell.Value = CellValue;

                        DataTable dt = new DataTable();
                        dt.Columns.Add("RowIndex", typeof(int));
                        dt.Columns.Add("SongId", typeof(string));
                        dt.Columns.Add("SongLang", typeof(string));

                        DataRow row = dt.NewRow();
                        row["RowIndex"] = SongQuery_DataGridView.CurrentRow.Index;
                        row["SongId"] = SongQuery_DataGridView.CurrentRow.Cells["Song_Id"].Value.ToString();
                        row["SongLang"] = SongQuery_DataGridView.CurrentRow.Cells["Song_Lang"].Value.ToString();
                        dt.Rows.Add(row);

                        Common_SwitchSetUI(false);
                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(dt)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Common_SwitchSetUI(true);
                            });
                            dt.Dispose();
                        });
                    }
                }
                SongQuery_DataGridView.EndEdit();
            }
        }

        private void SongQuery_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            string SongPath = "";
            string SongFileName = "";

            for (int i = 0; i < SongQuery_DataGridView.ColumnCount; i++)
            {
                switch (SongQuery_DataGridView.Columns[i].HeaderText)
                {
                    case "歌曲路徑":
                        SongPath = SongQuery_DataGridView.Rows[SongQuery_DataGridView.CurrentCell.RowIndex].Cells[i].Value.ToString();
                        break;
                    case "檔案名稱":
                        SongFileName = SongQuery_DataGridView.Rows[SongQuery_DataGridView.CurrentCell.RowIndex].Cells[i].Value.ToString();
                        break;
                }
            }

            string file = Path.Combine(SongPath, SongFileName);

            switch (sender.ToString())
            {
                case "開啟資料夾":
                    if (!Directory.Exists(SongPath) || !File.Exists(file))
                    {
                        SongQuery_QueryStatus_Label.Text = "選取歌曲的資料夾或檔案不存在...";
                    }
                    else
                    {
                        string arg = "/select, " + file;
                        Process.Start("explorer", arg);
                    }
                    break;
                case "播放檔案":
                    if (!File.Exists(file))
                    {
                        SongQuery_QueryStatus_Label.Text = "【" + SongFileName + "】檔案不存在...";
                    }
                    else
                    {
                        int i = SongQuery_DataGridView.CurrentCell.RowIndex;

                        string SongId = SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                        string SongLang = SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value.ToString();
                        string SongSinger = SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value.ToString();
                        string SongSongName = SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value.ToString();
                        string SongTrack = SongQuery_DataGridView.Rows[i].Cells["Song_Track"].Value.ToString();
                        string SongFilePath = Path.Combine(SongPath, SongFileName);

                        List<string> PlayerSongInfoList = new List<string>() { SongId, SongLang, SongSinger, SongSongName, SongTrack, SongFilePath, i.ToString(), "SongQuery" };

                        Global.PlayerUpdateSongValueList = new List<string>();
                        PlayerForm newPlayerForm = new PlayerForm(this, PlayerSongInfoList);
                        newPlayerForm.Show();
                        this.Hide();
                    }
                    break;
                case "刪除資料列":
                    if (MessageBox.Show("你確定要刪除歌庫資料及歌曲檔案嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        List<string> SongIdlist = new List<string>();
                        List<string> SongFilelist = new List<string>();
                        List<string> RemoveSongIdlist = new List<string>();

                        foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                        {
                            SongIdlist.Add(row.Cells["Song_Id"].Value.ToString());
                            SongFilelist.Add(row.Cells["Song_FullPath"].Value.ToString());
                        }

                        Common_SwitchSetUI(false);

                        var task = Task<List<string>>.Factory.StartNew(() => SongQuery_SongRemove(SongIdlist, SongFilelist));
                        RemoveSongIdlist = task.Result;

                        foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                        {
                            if (RemoveSongIdlist.IndexOf(row.Cells["Song_Id"].Value.ToString()) > -1)
                            {
                                SongQuery_DataGridView.Rows.Remove(row);
                            }
                        }

                        Common_SwitchSetUI(true);
                    }
                    break;
                case "從我的最愛移除":
                    if (MessageBox.Show("你確定要從【" + SongQuery_FavoriteQuery_ComboBox.Text + "】移除我的最愛歌曲嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
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

                        List<string> SongIdlist = new List<string>();
                        foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                        {
                            SongIdlist.Add(row.Cells["Song_Id"].Value.ToString());
                            SongQuery_DataGridView.Rows.Remove(row);
                        }
                        Task.Factory.StartNew(() => SongQuery_FavoriteRemove(SongIdlist, UserId));
                    }
                    break;
            }
        }

        private void SongQuery_DataGridView_FavoriteUserContextMenuItem_RightClick(object sender, EventArgs e)
        {
            string UserId = "";
            string UserName = sender.ToString();
            var query = from row in Global.FavoriteUserDT.AsEnumerable()
                        where row.Field<string>("User_Name").Equals(UserName)
                        select row;

            if (query.Count<DataRow>() > 0)
            {
                foreach (DataRow row in query)
                {
                    UserId = row["User_Id"].ToString();
                    break;
                }
            }

            List<string> SongIdlist = new List<string>();
            foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
            {
                SongIdlist.Add(row.Cells["Song_Id"].Value.ToString());
            }
            Task.Factory.StartNew(() => SongQuery_FavoriteAdd(SongIdlist, UserId));
        }

        private void SongQuery_DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if ((int)e.KeyCode == 46)
                {
                    if (MessageBox.Show("你確定要刪除歌庫資料及歌曲檔案嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        List<string> SongIdlist = new List<string>();
                        List<string> SongFilelist = new List<string>();
                        List<string> RemoveSongIdlist = new List<string>();

                        foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                        {
                            SongIdlist.Add(row.Cells["Song_Id"].Value.ToString());
                            SongFilelist.Add(row.Cells["Song_FullPath"].Value.ToString());
                        }
                        
                        Common_SwitchSetUI(false);

                        var task = Task<List<string>>.Factory.StartNew(() => SongQuery_SongRemove(SongIdlist, SongFilelist));
                        RemoveSongIdlist = task.Result;

                        foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                        {
                            if (RemoveSongIdlist.IndexOf(row.Cells["Song_Id"].Value.ToString()) > -1)
                            {
                                SongQuery_DataGridView.Rows.Remove(row);
                            }
                        }

                        Common_SwitchSetUI(true);
                    }
                }
            }
        }


        #endregion

    }

    class SongQueryDataGridView
    {
    }


}
