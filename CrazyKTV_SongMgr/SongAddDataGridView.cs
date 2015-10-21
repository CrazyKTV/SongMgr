using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        #region --- SongAdd_DataGridView ---

        private void SongAdd_DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int val;
            string valStr = "";
            switch (SongAdd_DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "Song_AddStatus":
                    e.CellStyle.ForeColor = Color.Red;
                    break;
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
                case "Song_WordCount":
                case "Song_MB":
                case "Song_SrcPath":
                    e.CellStyle.ForeColor = Color.Blue;
                    break;
            }
        }

        private void SongAdd_DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            switch (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌曲音量":
                    if (e.FormattedValue.ToString() == "")
                    {
                        SongAdd_Tooltip_Label.Text = "此項目的值不可為空白!";
                        e.Cancel = true;
                    }
                    else if (Convert.ToInt32(e.FormattedValue) > 100)
                    {
                        SongAdd_Tooltip_Label.Text = "此項目只能輸入 0 ~ 100 的值!";
                        e.Cancel = true;
                    }
                    break;
                case "歌手名稱":
                case "歌曲名稱":
                    if (e.FormattedValue.ToString() == "")
                    {
                        SongAdd_Tooltip_Label.Text = "此項目的值不可為空白!";
                        e.Cancel = true;
                    }
                    else
                    {
                        Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
                        if (r.IsMatch(e.FormattedValue.ToString()))
                        {
                            SongAdd_Tooltip_Label.Text = "此項目的值含有非法字元!";
                            e.Cancel = true;
                        }
                    }
                    break;
            }
        }

        private void SongAdd_DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (SongAdd_DataGridView.CurrentCell.ColumnIndex < 0) return;
            Control parentCTL = e.Control.Parent;

            switch (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌曲編號":
                case "歌曲音量":
                case "歌曲字數":
                case "點播次數":
                case "歌曲大小":
                    e.Control.KeyPress += new KeyPressEventHandler(SongAdd_DataGridView_Keyin_Number);
                    break;
                case "加歌日期":
                    DateTimePicker dtPicker = new DateTimePicker();
                    dtPicker.Name = "dateTimePicker";
                    dtPicker.Size = SongAdd_DataGridView.CurrentCell.Size;
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

                    dtPicker.CloseUp += new EventHandler(SongAdd_DataGridView_DateTimePicker_CloseUp);
                    break;
            }

            if (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText != "加歌日期")
            {
                foreach (Control tmpCTL in parentCTL.Controls)
                {
                    if (tmpCTL.Name == "dateTimePicker") parentCTL.Controls.Remove(tmpCTL);
                }
            }
        }

        private void SongAdd_DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            switch (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌手名稱":
                    string SongSinger = SongAdd_DataGridView.CurrentCell.Value.ToString();
                    if (SongSinger.ContainsAny("&", "+")) SongAdd_DataGridView.CurrentRow.Cells["Song_SingerType"].Value = "3";
                    break;
                case "歌曲名稱":
                    string SongSongName = SongAdd_DataGridView.CurrentCell.Value.ToString();
                    // 計算歌曲字數
                    List<string> SongWordCountList = new List<string>();
                    SongWordCountList = CommonFunc.GetSongWordCount(SongSongName);
                    SongAdd_DataGridView.CurrentRow.Cells["Song_WordCount"].Value = SongWordCountList[0];

                    // 取得歌曲拼音
                    List<string> SongSpellList = new List<string>();
                    SongSpellList = CommonFunc.GetSongNameSpell(SongSongName);
                    SongAdd_DataGridView.CurrentRow.Cells["Song_Spell"].Value = SongSpellList[0];
                    SongAdd_DataGridView.CurrentRow.Cells["Song_SpellNum"].Value = SongSpellList[1];
                    if (SongSpellList[2] == "") SongSpellList[2] = "0";
                    SongAdd_DataGridView.CurrentRow.Cells["Song_SongStroke"].Value = SongSpellList[2];
                    SongAdd_DataGridView.CurrentRow.Cells["Song_PenStyle"].Value = SongSpellList[3];
                    break;
            }

            if (SongAdd_Tooltip_Label.Text == "此項目只能輸入數字!" | SongAdd_Tooltip_Label.Text == "此項目只能輸入 0 ~ 100 的值!" | SongAdd_Tooltip_Label.Text == "此項目只能輸入數字及小數點!" | SongAdd_Tooltip_Label.Text == "此項目的值不可為空白!" | SongAdd_Tooltip_Label.Text == "此項目的值含有非法字元!") SongAdd_Tooltip_Label.Text = "";
        }

        private void SongAdd_DataGridView_Keyin_Number(object sender, KeyPressEventArgs e)
        {
            switch (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌曲編號":
                case "歌曲音量":
                case "歌曲字數":
                case "點播次數":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13 & (int)e.KeyChar != 27)
                    {
                        e.Handled = true;
                        SongAdd_Tooltip_Label.Text = "此項目只能輸入數字!";
                    }
                    else
                    {
                        if (SongAdd_Tooltip_Label.Text == "此項目只能輸入數字!") SongAdd_Tooltip_Label.Text = "";
                    }
                    break;
                case "歌曲大小":
                    if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13 & (int)e.KeyChar != 27 & (int)e.KeyChar != 46)
                    {
                        e.Handled = true;
                        SongAdd_Tooltip_Label.Text = "此項目只能輸入數字及小數點!";
                    }
                    else
                    {
                        if (SongAdd_Tooltip_Label.Text == "此項目只能輸入數字及小數點!") SongAdd_Tooltip_Label.Text = "";
                    }
                    break;
            }

        }

        private void SongAdd_DataGridView_DateTimePicker_CloseUp(Object sender, EventArgs e)
        {
            string DateTimeValue = ((DateTimePicker)sender).Value.ToString("yyyy/M/d") + " " + DateTime.Now.ToString("tt hh:mm:ss");
            int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

            if (SongAdd_DataGridView.SelectedRows.Count > 1)
            {
                for (int i = 0; i < SelectedRowsCount; i++)
                {
                    SongAdd_DataGridView.SelectedRows[i].Cells["Song_CreatDate"].Value = DateTimeValue;
                }
            }
            else
            {
                SongAdd_DataGridView.CurrentCell.Value = DateTimeValue;
            }
            SongAdd_DataGridView.EndEdit();
        }

        private void SongAdd_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0))
            {
                SongAdd_DataGridView.EndEdit();
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                string EditCell = SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText;
                List<string> list = new List<string>();
                list = new List<string>() { "歌手名稱", "歌曲名稱", "歌曲音量", "加歌日期" };

                var query = from editlist in list
                            where editlist == EditCell
                            select editlist;
                foreach (var q in query)
                {
                    SongAdd_DataGridView.BeginEdit(true);
                }

                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                string valStr = "";

                switch (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText)
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
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_Click);
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
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_Click);
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
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_Click);
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
                        GridView_ContextMenuItem[0].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_Click);

                        for (int i = 0; i < count; i++)
                        {
                            valStr = SongQuery.GetSongTypeStr(i);
                            GridView_ContextMenuItem[i + 1] = new ToolStripMenuItem(valStr);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i + 1]);
                            GridView_ContextMenuItem[i + 1].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_Click);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                string valStr = "";

                if (!SongAdd_DataGridView.Rows[e.RowIndex].Selected)
                {
                    SongAdd_DataGridView.ClearSelection();
                    SongAdd_DataGridView.Rows[e.RowIndex].Selected = true;
                    SongAdd_DataGridView.CurrentCell = SongAdd_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }

                if (SongAdd_DataGridView.SelectedRows.Count > 1)
                {
                    GridView_ContextMenu = new ContextMenuStrip();
                    GridView_ContextMenuItem = new ToolStripMenuItem[1];

                    for (int i = 0; i < 1; i++)
                    {
                        valStr = SongQuery.GetContextMenuStr(i, 0);
                        GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                        GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                        GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_RightClick);
                    }
                    GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                }
                else
                {
                    SongAdd_DataGridView.ClearSelection();
                    SongAdd_DataGridView.Rows[e.RowIndex].Selected = true;
                    SongAdd_DataGridView.CurrentCell = SongAdd_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    GridView_ContextMenu = new ContextMenuStrip();
                    GridView_ContextMenuItem = new ToolStripMenuItem[3];

                    for (int i = 0; i < 3; i++)
                    {
                        valStr = SongQuery.GetContextMenuStr(i, 1);
                        GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                        GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                        GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_RightClick);
                    }
                    GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void SongAdd_DataGridView_ContextMenuItem_Click(object sender, EventArgs e)
        {
            int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;
            string CellName = "";
            string CellValue = "";

            switch (SongAdd_DataGridView.Columns[SongAdd_DataGridView.CurrentCell.ColumnIndex].HeaderText)
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

            if (SongAdd_DataGridView.SelectedRows.Count > 1)
            {
                for (int i = 0; i < SelectedRowsCount; i++)
                {
                    SongAdd_DataGridView.SelectedRows[i].Cells[CellName].Value = CellValue;
                    
                    switch (CellName)
                    {
                        case "Song_Lang":
                            if (SongAdd_DataGridView.SelectedRows[i].Cells["Song_AddStatus"].Value.ToString() == "語系類別必須有值才能加歌!")
                            {
                                if (SongAdd_DataGridView.SelectedRows[i].Cells["Song_SingerType"].Value.ToString() == "10")
                                {
                                    SongAdd_DataGridView.SelectedRows[i].Cells["Song_AddStatus"].Value = "此歌手尚未設定歌手資料!";
                                }
                                else
                                {
                                    SongAdd_DataGridView.SelectedRows[i].Cells["Song_AddStatus"].Value = "";
                                }
                            }
                            break;
                        case "Song_SingerType":
                            if (CellValue != "10" & SongAdd_DataGridView.SelectedRows[i].Cells["Song_AddStatus"].Value.ToString() == "此歌手尚未設定歌手資料!")
                            {
                                SongAdd_DataGridView.SelectedRows[i].Cells["Song_AddStatus"].Value = "";
                            }
                            break;
                    }
                }
                SongAdd_DataGridView.EndEdit();
            }
            else
            {
                SongAdd_DataGridView.CurrentCell.Value = CellValue;
                
                switch (CellName)
                {
                    case "Song_Lang":
                        if (SongAdd_DataGridView.CurrentRow.Cells["Song_AddStatus"].Value.ToString() == "語系類別必須有值才能加歌!")
                        {
                            if (SongAdd_DataGridView.CurrentRow.Cells["Song_SingerType"].Value.ToString() == "10")
                            {
                                SongAdd_DataGridView.CurrentRow.Cells["Song_AddStatus"].Value = "此歌手尚未設定歌手資料!";
                            }
                            else
                            {
                                SongAdd_DataGridView.CurrentRow.Cells["Song_AddStatus"].Value = "";
                            }
                        }
                        break;
                    case "Song_SingerType":
                        if (CellValue != "10" & SongAdd_DataGridView.CurrentRow.Cells["Song_AddStatus"].Value.ToString() == "此歌手尚未設定歌手資料!")
                        {
                            SongAdd_DataGridView.CurrentRow.Cells["Song_AddStatus"].Value = "";
                        }
                        break;
                }
                SongAdd_DataGridView.EndEdit();
            }
            if (CellName == "Song_Lang")
            {
                SongAdd_Add_Button.Enabled = SongAdd_CheckSongAddStatus();
            }
        }

        private void SongAdd_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            string SongPath = "";
            string SongFilePath = "";

            for (int i = 0; i < SongAdd_DataGridView.ColumnCount; i++)
            {
                switch (SongAdd_DataGridView.Columns[i].HeaderText)
                {
                    case "來源檔案路徑":
                        SongPath = Path.GetDirectoryName(SongAdd_DataGridView.Rows[SongAdd_DataGridView.CurrentCell.RowIndex].Cells[i].Value.ToString());
                        SongFilePath = SongAdd_DataGridView.Rows[SongAdd_DataGridView.CurrentCell.RowIndex].Cells[i].Value.ToString();
                        break;
                }
            }

            switch (sender.ToString())
            {
                case "開啟資料夾":
                    if (!Directory.Exists(SongPath) || !File.Exists(SongFilePath))
                    {
                        SongAdd_Tooltip_Label.Text = "選取歌曲的資料夾或檔案不存在...";
                    }
                    else
                    {
                        string arg = "/select, " + SongFilePath;
                        Process.Start("explorer", arg);
                    }
                    break;
                case "播放檔案":
                    if (!File.Exists(SongFilePath))
                    {
                        SongAdd_Tooltip_Label.Text = "【" + SongFilePath + "】檔案不存在...";
                    }
                    else
                    {
                        Process.Start(SongFilePath);
                    }
                    break;
                case "刪除資料列":
                    if (MessageBox.Show("你確定要刪除資料嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                        {
                            SongAdd_DataGridView.Rows.Remove(row);
                        }
                    }
                    break;
            }
        }

        private void SongAdd_DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 46)
            {
                if (MessageBox.Show("你確定要刪除資料嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        SongAdd_DataGridView.Rows.Remove(row);
                    }
                }
            }
        }


        #endregion

    }

    class SongAddDataGridView
    {

    }

}
