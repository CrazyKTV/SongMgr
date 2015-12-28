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
                    valStr = CommonFunc.GetSongTrackStr(val, 0, "null");
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

        private void SongAdd_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0)) return;

            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                string valStr = "";

                if (SongAdd_Save_Button.Text == "取消加入")
                {
                    if (!SongAdd_DataGridView.Rows[e.RowIndex].Selected) SongAdd_DataGridView.CurrentCell = SongAdd_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (SongAdd_DataGridView.SelectedRows.Count > 1)
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        int ContextMenuCount = Convert.ToInt32(SongQuery.GetContextMenuStr(0, 0, true));
                        GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount];

                        for (int i = 0; i < ContextMenuCount; i++)
                        {
                            valStr = SongQuery.GetContextMenuStr(i, 0, false);
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_RightClick);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                    else
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        int ContextMenuCount = Convert.ToInt32(SongQuery.GetContextMenuStr(0, 1, true));
                        GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount];

                        for (int i = 0; i < ContextMenuCount; i++)
                        {
                            valStr = SongQuery.GetContextMenuStr(i, 1, false);
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SongAdd_DataGridView_ContextMenuItem_RightClick);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                }
            }
        }

        private void SongAdd_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            string file = SongAdd_DataGridView.Rows[SongAdd_DataGridView.CurrentCell.RowIndex].Cells["Song_SrcPath"].Value.ToString();
            string SongPath = Path.GetDirectoryName(file);
            string SongFileName = Path.GetFileName(file);

            switch (sender.ToString())
            {
                case "開啟資料夾":
                    if (!Directory.Exists(SongPath) || !File.Exists(file))
                    {
                        SongAdd_Tooltip_Label.Text = "選取歌曲的資料夾或檔案不存在...";
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
                        SongAdd_Tooltip_Label.Text = "【" + SongFileName + "】檔案不存在...";
                    }
                    else
                    {
                        int i = SongAdd_DataGridView.CurrentCell.RowIndex;

                        string SongId = SongAdd_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                        string SongLang = SongAdd_DataGridView.Rows[i].Cells["Song_Lang"].Value.ToString();
                        string SongSinger = SongAdd_DataGridView.Rows[i].Cells["Song_Singer"].Value.ToString();
                        string SongSongName = SongAdd_DataGridView.Rows[i].Cells["Song_SongName"].Value.ToString();
                        string SongTrack = SongAdd_DataGridView.Rows[i].Cells["Song_Track"].Value.ToString();
                        string SongFilePath = file;

                        List<string> PlayerSongInfoList = new List<string>() { SongId, SongLang, SongSinger, SongSongName, SongTrack, SongFilePath, i.ToString(), "SongAdd" };

                        Global.PlayerUpdateSongValueList = new List<string>();
                        PlayerForm newPlayerForm = new PlayerForm(this, PlayerSongInfoList);
                        newPlayerForm.Show();
                        this.Hide();
                    }
                    break;
                case "刪除資料列":
                    if (MessageBox.Show("你確定要刪除資料嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                        {
                            SongAdd_DataGridView.Rows.Remove(row);
                        }

                        if (SongAdd_DataGridView.RowCount == 0)
                        {
                            SongAdd_DataGridView.DataSource = null;
                            SongAdd_Add_Button.Enabled = false;
                            SongAdd_InitializeEditControl();
                            SongAdd_Tooltip_Label.Text = "已無歌曲可加入!";
                        }
                    }
                    break;
            }
        }

        private void SongAdd_DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                Global.SongAddDataGridViewRestoreSelectList = new List<string>();
                foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                {
                    string SongPath = row.Cells["Song_SrcPath"].Value.ToString();
                    Global.SongAddDataGridViewRestoreSelectList.Add(SongPath);
                }
            }
        }

        private void SongAdd_DataGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddDataGridViewSelectList = new List<string>();

                    foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                    {
                        string SongId = row.Cells["Song_Id"].Value.ToString();
                        string SongLang = row.Cells["Song_Lang"].Value.ToString();
                        int SongSingerType = Convert.ToInt32(row.Cells["Song_SingerType"].Value);
                        string SongSinger = row.Cells["Song_Singer"].Value.ToString();
                        string SongSongName = row.Cells["Song_SongName"].Value.ToString();
                        int SongTrack = Convert.ToInt32(row.Cells["Song_Track"].Value);
                        string SongSongType = row.Cells["Song_SongType"].Value.ToString();
                        string SongVolume = row.Cells["Song_Volume"].Value.ToString();
                        string SongWordCount = row.Cells["Song_WordCount"].Value.ToString();
                        string SongPlayCount = row.Cells["Song_PlayCount"].Value.ToString();
                        string SongMB = row.Cells["Song_MB"].Value.ToString();
                        string SongCreatDate = row.Cells["Song_CreatDate"].Value.ToString();
                        string SongFileName = row.Cells["Song_FileName"].Value.ToString();
                        string SongPath = row.Cells["Song_Path"].Value.ToString();
                        string SongSpell = row.Cells["Song_Spell"].Value.ToString();
                        string SongSpellNum = row.Cells["Song_SpellNum"].Value.ToString();
                        string SongSongStroke = row.Cells["Song_SongStroke"].Value.ToString();
                        string SongPenStyle = row.Cells["Song_PenStyle"].Value.ToString();
                        string SongPlayState = row.Cells["Song_PlayState"].Value.ToString();
                        string SongSrcPath = row.Cells["Song_SrcPath"].Value.ToString();

                        string SelectValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongSrcPath;
                        Global.SongAddDataGridViewSelectList.Add(SelectValue);
                    }
                }
            }
        }

        private void SongAdd_DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (SongAdd_Save_Button.Text == "取消加入")
            {
                if (SongAdd_DataGridView.SelectedRows.Count > 0)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Delete:
                            if (MessageBox.Show("你確定要刪除資料嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                foreach (DataGridViewRow row in SongAdd_DataGridView.SelectedRows)
                                {
                                    SongAdd_DataGridView.Rows.Remove(row);
                                }

                                if (SongAdd_DataGridView.RowCount == 0)
                                {
                                    SongAdd_DataGridView.DataSource = null;
                                    SongAdd_Add_Button.Enabled = false;
                                    SongAdd_InitializeEditControl();
                                    SongAdd_Tooltip_Label.Text = "已無歌曲可加入!";
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void SongAdd_DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (SongAdd_DataGridView.SelectedRows.Count > 0)
            {
                int SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;
                List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));
                if (SongAdd_Tooltip_Label.Text != "") SongAdd_Tooltip_Label.Text = "";
                Global.SongAddDataGridViewSelectList.Clear();

                if (SelectedRowsCount > 1)
                {
                    Global.SongAddMultiEditUpdateList = new List<bool>() { false, false, false, false, false, false, false, false };

                    if (!Global.SongAddMultiEdit)
                    {
                        SongAdd_Edit_GroupBox.Text = "批次編輯";
                        SongAdd_GetSongEditComboBoxList(true);

                        SongAdd_EditSongId_TextBox.Enabled = false;
                        SongAdd_EditSongLang_ComboBox.Enabled = true;
                        SongAdd_EditSongCreatDate_DateTimePicker.Enabled = true;
                        SongAdd_EditSongSinger_TextBox.Enabled = true;
                        SongAdd_EditSongSingerType_ComboBox.Enabled = true;
                        SongAdd_EditSongSongName_TextBox.Enabled = false;
                        SongAdd_EditSongSongType_ComboBox.Enabled = true;
                        SongAdd_EditSongSpell_TextBox.Enabled = false;
                        SongAdd_EditSongWordCount_TextBox.Enabled = false;
                        SongAdd_EditSongSrcPath_TextBox.Enabled = false;
                        SongAdd_EditSongTrack_ComboBox.Enabled = true;
                        SongAdd_EditSongTrack_Button.Enabled = false;
                        SongAdd_EditSongVolume_TextBox.Enabled = true;
                        SongAdd_EditSongPlayCount_TextBox.Enabled = false;
                        SongAdd_EditApplyChanges_Button.Enabled = true;

                        SongAdd_EditSongId_TextBox.Text = "";
                        SongAdd_EditSongLang_ComboBox.SelectedValue = 1;
                        SongAdd_EditSongCreatDate_DateTimePicker.Value = DateTime.Now;
                        SongAdd_EditSongSinger_TextBox.Text = "";
                        SongAdd_EditSongSingerType_ComboBox.SelectedValue = 1;
                        SongAdd_EditSongSongName_TextBox.Text = "";
                        SongAdd_EditSongSongType_ComboBox.SelectedValue = 1;
                        SongAdd_EditSongSpell_TextBox.Text = "";
                        SongAdd_EditSongWordCount_TextBox.Text = "";
                        SongAdd_EditSongSrcPath_TextBox.Text = "";
                        SongAdd_EditSongTrack_ComboBox.SelectedValue = 1;
                        SongAdd_EditSongVolume_TextBox.Text = "";
                        SongAdd_EditSongPlayCount_TextBox.Text = "";
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    SongAdd_Edit_GroupBox.Text = "單曲編輯";
                    SongAdd_GetSongEditComboBoxList(false);

                    SongAdd_EditSongId_TextBox.Enabled = false;
                    SongAdd_EditSongLang_ComboBox.Enabled = true;
                    SongAdd_EditSongCreatDate_DateTimePicker.Enabled = true;
                    SongAdd_EditSongSinger_TextBox.Enabled = true;
                    SongAdd_EditSongSingerType_ComboBox.Enabled = true;
                    SongAdd_EditSongSongName_TextBox.Enabled = true;
                    SongAdd_EditSongSongType_ComboBox.Enabled = true;
                    SongAdd_EditSongSpell_TextBox.Enabled = true;
                    SongAdd_EditSongWordCount_TextBox.Enabled = true;
                    SongAdd_EditSongSrcPath_TextBox.Enabled = true;
                    SongAdd_EditSongTrack_ComboBox.Enabled = true;
                    SongAdd_EditSongTrack_Button.Enabled = true;
                    SongAdd_EditSongVolume_TextBox.Enabled = true;
                    SongAdd_EditSongPlayCount_TextBox.Enabled = false;
                    SongAdd_EditApplyChanges_Button.Enabled = true;

                    string SongId = SongAdd_DataGridView.SelectedRows[0].Cells["Song_Id"].Value.ToString();
                    string SongLang = SongAdd_DataGridView.SelectedRows[0].Cells["Song_Lang"].Value.ToString();
                    int SongSingerType = Convert.ToInt32(SongAdd_DataGridView.SelectedRows[0].Cells["Song_SingerType"].Value);
                    string SongSinger = SongAdd_DataGridView.SelectedRows[0].Cells["Song_Singer"].Value.ToString();
                    string SongSongName = SongAdd_DataGridView.SelectedRows[0].Cells["Song_SongName"].Value.ToString();
                    int SongTrack = Convert.ToInt32(SongAdd_DataGridView.SelectedRows[0].Cells["Song_Track"].Value);
                    string SongSongType = SongAdd_DataGridView.SelectedRows[0].Cells["Song_SongType"].Value.ToString();
                    string SongVolume = SongAdd_DataGridView.SelectedRows[0].Cells["Song_Volume"].Value.ToString();
                    string SongWordCount = SongAdd_DataGridView.SelectedRows[0].Cells["Song_WordCount"].Value.ToString();
                    string SongPlayCount = SongAdd_DataGridView.SelectedRows[0].Cells["Song_PlayCount"].Value.ToString();
                    string SongMB = SongAdd_DataGridView.SelectedRows[0].Cells["Song_MB"].Value.ToString();
                    string SongCreatDate = SongAdd_DataGridView.SelectedRows[0].Cells["Song_CreatDate"].Value.ToString();
                    string SongFileName = SongAdd_DataGridView.SelectedRows[0].Cells["Song_FileName"].Value.ToString();
                    string SongPath = SongAdd_DataGridView.SelectedRows[0].Cells["Song_Path"].Value.ToString();
                    string SongSpell = SongAdd_DataGridView.SelectedRows[0].Cells["Song_Spell"].Value.ToString();
                    string SongSpellNum = SongAdd_DataGridView.SelectedRows[0].Cells["Song_SpellNum"].Value.ToString();
                    string SongSongStroke = SongAdd_DataGridView.SelectedRows[0].Cells["Song_SongStroke"].Value.ToString();
                    string SongPenStyle = SongAdd_DataGridView.SelectedRows[0].Cells["Song_PenStyle"].Value.ToString();
                    string SongPlayState = SongAdd_DataGridView.SelectedRows[0].Cells["Song_PlayState"].Value.ToString();
                    string SongSrcPath = SongAdd_DataGridView.SelectedRows[0].Cells["Song_SrcPath"].Value.ToString();

                    SongAdd_EditSongId_TextBox.Text = SongId;
                    SongAdd_EditSongLang_ComboBox.SelectedValue = (SongLang == "未知") ? 11 : Global.CrazyktvSongLangList.IndexOf(SongLang) + 1;
                    SongAdd_EditSongCreatDate_DateTimePicker.Value = DateTime.Parse(SongCreatDate);
                    SongAdd_EditSongSinger_TextBox.Text = SongSinger;
                    string SongSingerTypeStr = CommonFunc.GetSingerTypeStr(SongSingerType, 1, "null");
                    SongAdd_EditSongSingerType_ComboBox.SelectedValue = Convert.ToInt32(CommonFunc.GetSingerTypeStr(0, 3, SongSingerTypeStr)) + 1;
                    SongAdd_EditSongSongName_TextBox.Text = SongSongName;
                    SongAdd_EditSongSongType_ComboBox.SelectedValue = (SongSongType == "") ? 1 : SongSongTypeList.IndexOf(SongSongType) + 2;
                    SongAdd_EditSongSpell_TextBox.Text = SongSpell;
                    SongAdd_EditSongWordCount_TextBox.Text = SongWordCount;
                    SongAdd_EditSongSrcPath_TextBox.Text = SongSrcPath;
                    SongAdd_EditSongTrack_ComboBox.SelectedValue = SongTrack;
                    SongAdd_EditSongVolume_TextBox.Text = SongVolume;
                    SongAdd_EditSongPlayCount_TextBox.Text = "";

                    Global.SongAddDataGridViewSelectList = new List<string>();
                    string SelectValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongSrcPath;
                    Global.SongAddDataGridViewSelectList.Add(SelectValue);
                }
            }
        }

        private void SongAdd_DataGridView_Sorted(object sender, EventArgs e)
        {
            if (Global.SongAddDataGridViewRestoreSelectList.Count > 0)
            {
                SongAdd_DataGridView.ClearSelection();
                foreach (string str in Global.SongAddDataGridViewRestoreSelectList)
                {
                    var query = from row in SongAdd_DataGridView.Rows.Cast<DataGridViewRow>()
                                where row.Cells["Song_SrcPath"].Value.Equals(str)
                                select row;

                    foreach (DataGridViewRow row in query)
                    {
                        row.Selected = true;
                    }
                    SongAdd_DataGridView.CurrentCell = SongAdd_DataGridView.SelectedRows[SongAdd_DataGridView.SelectedRows.Count - 1].Cells[0];
                }
                Global.SongAddDataGridViewRestoreSelectList.Clear();
            }
        }

        #endregion

    }
}
