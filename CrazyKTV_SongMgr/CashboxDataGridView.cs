using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {

        #region --- Cashbox 列表欄位格式 ---

        private void Cashbox_DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            switch (Cashbox_DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "Song_CreatDate":
                    e.Value = DateTime.Parse(e.Value.ToString()).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                    e.FormattingApplied = true;
                    break;
            }
        }

        private void Cashbox_DataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridViewRow row = Cashbox_DataGridView.Rows[e.RowIndex];
            if (Global.CashboxHaveSongList.IndexOf(row.Cells["Cashbox_Id"].Value.ToString()) > 0)
            {
                row.DefaultCellStyle.ForeColor = Color.Blue;
            }
        }

        #endregion

        #region --- Cashbox 列表滑鼠點擊事件 ---

        private void Cashbox_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0)) return;

            if (e.Button == MouseButtons.Right)
            {
                if (!Cashbox_DataGridView.Rows[e.RowIndex].Selected) Cashbox_DataGridView.CurrentCell = Cashbox_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                List<string> ContextMenuItemList = new List<string>() { "複製歌手名稱", "複製歌曲名稱" };

                if (Cashbox_DataGridView.SelectedRows.Count == 1)
                {
                    GridView_ContextMenu = new ContextMenuStrip();
                    GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuItemList.Count];

                    for (int i = 0; i < ContextMenuItemList.Count; i++)
                    {
                        GridView_ContextMenuItem[i] = new ToolStripMenuItem(ContextMenuItemList[i]);
                        GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                        GridView_ContextMenuItem[i].Click += new EventHandler(Cashbox_DataGridView_ContextMenuItem_RightClick);
                    }
                    GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        #endregion

        #region --- Cashbox 列表滑鼠右鍵功能表點擊事件 ---

        private void Cashbox_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            string ClipStr = string.Empty;
            switch (sender.ToString())
            {
                case "複製歌手名稱":
                    ClipStr = Cashbox_DataGridView.SelectedRows[0].Cells["Song_Singer"].Value.ToString();
                    break;
                case "複製歌曲名稱":
                    ClipStr = Cashbox_DataGridView.SelectedRows[0].Cells["Song_SongName"].Value.ToString();
                    break;
            }
            try
            {
                Clipboard.SetData(DataFormats.UnicodeText, ClipStr);
            }
            catch
            {
                // 剪貼簿被別的程式占用
                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【複製到剪貼簿】無法完成複製到剪貼簿,因剪貼簿已被占用。";
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
            }
        }

        #endregion

        #region --- Cashbox 列表滑鼠點擊狀態事件 ---

        private void Cashbox_DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                Global.CashboxDataGridViewRestoreCurrentRow = Cashbox_DataGridView.CurrentRow.Cells["Cashbox_Id"].Value.ToString();
                Global.CashboxDataGridViewRestoreSelectList = new List<string>();
                foreach (DataGridViewRow row in Cashbox_DataGridView.SelectedRows)
                {
                    string SongId = row.Cells["Cashbox_Id"].Value.ToString();
                    Global.CashboxDataGridViewRestoreSelectList.Add(SongId);
                }
            }
        }

        #endregion

        #region --- Cashbox 列表選取項目變更事件 ---

        private void Cashbox_DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            #if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;
                if (Cashbox_QueryStatus_Label.Text != "") Cashbox_QueryStatus_Label.Text = "";
                Global.CashboxDataGridViewSelectList.Clear();

                if (SelectedRowsCount > 1)
                {
                    Global.CashboxMultiEditUpdateList = new List<bool>() { false, false, false };

                    if (!Global.CashboxMultiEdit)
                    {
                        Cashbox_Edit_GroupBox.Text = "批次編輯";
                        Cashbox_GetSongEditComboBoxList(true);

                        Cashbox_EditSongId_TextBox.Enabled = false;
                        Cashbox_EditSongLang_ComboBox.Enabled = true;
                        Cashbox_EditSongCreatDate_DateTimePicker.Enabled = true;
                        Cashbox_EditSongSinger_TextBox.Enabled = true;
                        Cashbox_EditSongSongName_TextBox.Enabled = false;
                        Cashbox_EditApplyChanges_Button.Enabled = true;

                        Cashbox_EditSongId_TextBox.Text = "";
                        Cashbox_EditSongLang_ComboBox.SelectedValue = 1;
                        Cashbox_EditSongCreatDate_DateTimePicker.Value = DateTime.Now;
                        Cashbox_EditSongSinger_TextBox.Text = "";
                        Cashbox_EditSongSongName_TextBox.Text = "";
                    }

                    Global.CashboxDataGridViewSelectList = new List<string>();

                    foreach (DataGridViewRow row in Cashbox_DataGridView.SelectedRows)
                    {
                        string SongId = row.Cells["Cashbox_Id"].Value.ToString();
                        string SongLang = row.Cells["Song_Lang"].Value.ToString();
                        string SongSinger = row.Cells["Song_Singer"].Value.ToString();
                        string SongSongName = row.Cells["Song_SongName"].Value.ToString();
                        string SongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();

                        string SelectValue = SongId + "|" + SongLang + "|" + SongSinger + "|" + SongSongName + "|" + SongCreatDate;
                        Global.CashboxDataGridViewSelectList.Add(SelectValue);
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    Cashbox_Edit_GroupBox.Text = "單曲編輯";
                    Cashbox_GetSongEditComboBoxList(false);

                    Cashbox_EditSongId_TextBox.Enabled = true;
                    Cashbox_EditSongLang_ComboBox.Enabled = true;
                    Cashbox_EditSongCreatDate_DateTimePicker.Enabled = true;
                    Cashbox_EditSongSinger_TextBox.Enabled = true;
                    Cashbox_EditSongSongName_TextBox.Enabled = true;
                    Cashbox_EditApplyChanges_Button.Enabled = true;

                    string SongId = Cashbox_DataGridView.SelectedRows[0].Cells["Cashbox_Id"].Value.ToString();
                    string SongLang = Cashbox_DataGridView.SelectedRows[0].Cells["Song_Lang"].Value.ToString();
                    string SongSinger = Cashbox_DataGridView.SelectedRows[0].Cells["Song_Singer"].Value.ToString();
                    string SongSongName = Cashbox_DataGridView.SelectedRows[0].Cells["Song_SongName"].Value.ToString();
                    string SongCreatDate = Cashbox_DataGridView.SelectedRows[0].Cells["Song_CreatDate"].Value.ToString();

                    Cashbox_EditSongId_TextBox.Text = SongId;
                    Cashbox_EditSongLang_ComboBox.SelectedValue = Global.CashboxSongLangList.IndexOf(SongLang) + 1;
                    Cashbox_EditSongCreatDate_DateTimePicker.Value = DateTime.Parse(SongCreatDate);
                    Cashbox_EditSongSinger_TextBox.Text = SongSinger;
                    Cashbox_EditSongSongName_TextBox.Text = SongSongName;

                    Global.CashboxDataGridViewSelectList = new List<string>();
                    string SelectValue = SongId + "|" + SongLang + "|" + SongSinger + "|" + SongSongName + "|" + SongCreatDate;
                    Global.CashboxDataGridViewSelectList.Add(SelectValue);
                }
            }
            #endif
        }

        #endregion

        #region --- Cashbox 列表排序事件 ---

        private void Cashbox_DataGridView_Sorted(object sender, EventArgs e)
        {
            Cashbox_DataGridView.ClearSelection();

            if (Global.CashboxDataGridViewRestoreCurrentRow != "")
            {
                var query = from row in Cashbox_DataGridView.Rows.Cast<DataGridViewRow>()
                            where row.Cells["Cashbox_Id"].Value.Equals(Global.CashboxDataGridViewRestoreCurrentRow)
                            select row;

                if (query.Count() > 0)
                {
                    foreach (DataGridViewRow row in query)
                    {
                        Cashbox_DataGridView.CurrentCell = row.Cells[0];
                    }
                }
            }

            foreach (string str in Global.CashboxDataGridViewRestoreSelectList)
            {
                var query = from row in Cashbox_DataGridView.Rows.Cast<DataGridViewRow>()
                            where row.Cells["Cashbox_Id"].Value.Equals(str)
                            select row;

                foreach (DataGridViewRow row in query)
                {
                    row.Selected = true;
                }
            }
            Global.CashboxDataGridViewRestoreSelectList.Clear();
        }

        #endregion

    }
}
