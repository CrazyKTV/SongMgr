using System;
using System.Collections.Generic;
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
                    e.Value = DateTime.Parse(e.Value.ToString()).ToString("yyyy/MM/dd");
                    e.FormattingApplied = true;
                    break;
            }
        }

        #endregion

        #region --- Cashbox 列表滑鼠點擊狀態事件 ---

        private void Cashbox_DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            #if DEBUG
            if (Cashbox_EditMode_CheckBox.Checked == true)
            {
                int SelectedRowsCount = Cashbox_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
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
            }
            #endif
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


    }
}
