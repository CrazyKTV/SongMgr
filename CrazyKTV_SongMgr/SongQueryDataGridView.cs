using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{

    public partial class MainForm : Form
    {
        
        #region --- SongQuery 列表欄位格式 ---

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
                    valStr = CommonFunc.GetSongTrackStr(val, 0, "null");
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

        #endregion

        #region --- SongQuery 列表滑鼠點擊事件 ---

        private void SongQuery_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0)) return;

            if (e.Button == MouseButtons.Right) 
            {
                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                string valStr = "";

                if (SongQuery_EditMode_CheckBox.Checked == true) // 編輯模式
                {
                    if (!SongQuery_DataGridView.Rows[e.RowIndex].Selected) SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (SongQuery_DataGridView.SelectedRows.Count > 1)
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        int ContextMenuCount = Convert.ToInt32(SongQuery.GetContextMenuStr(0, 0, true));
                        if (Global.FavoriteUserDT.Rows.Count > 0) { GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount + 1]; } else { GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount]; }

                        for (int i = 0; i < ContextMenuCount; i++)
                        {
                            valStr = SongQuery.GetContextMenuStr(i, 0, false);
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
                        GridView_ContextMenu = new ContextMenuStrip();
                        int ContextMenuCount = Convert.ToInt32(SongQuery.GetContextMenuStr(0, 1, true));
                        if (Global.FavoriteUserDT.Rows.Count > 0) { GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount + 1]; } else { GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount]; }

                        for (int i = 0; i < ContextMenuCount; i++)
                        {
                            valStr = SongQuery.GetContextMenuStr(i, 1, false);
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
                else // 查詢模式
                {
                    if (!SongQuery_DataGridView.Rows[e.RowIndex].Selected) SongQuery_DataGridView.CurrentCell = SongQuery_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

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
                            GridView_ContextMenu = new ContextMenuStrip();
                            int ContextMenuCount = Convert.ToInt32(SongQuery.GetContextMenuStr(0, 3, true));
                            GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount];
                            
                            for (int i = 0; i < ContextMenuCount; i++)
                            {
                                valStr = SongQuery.GetContextMenuStr(i, 3, false);
                                GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                                GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                                GridView_ContextMenuItem[i].Click += new EventHandler(SongQuery_DataGridView_ContextMenuItem_RightClick);
                            }
                            GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        }
                        else
                        {
                            GridView_ContextMenu = new ContextMenuStrip();
                            int ContextMenuCount = Convert.ToInt32(SongQuery.GetContextMenuStr(0, 2, true));
                            if (Global.FavoriteUserDT.Rows.Count > 0) { GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount + 1]; } else { GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuCount]; }

                            for (int i = 0; i < ContextMenuCount; i++)
                            {
                                valStr = SongQuery.GetContextMenuStr(i, 2, false);
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

        #endregion

        #region --- SongQuery 列表滑鼠右鍵功能表點擊事件 ---

        private void SongQuery_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            string SongPath = SongQuery_DataGridView.Rows[SongQuery_DataGridView.CurrentCell.RowIndex].Cells["Song_Path"].Value.ToString();
            string SongFileName = SongQuery_DataGridView.Rows[SongQuery_DataGridView.CurrentCell.RowIndex].Cells["Song_FileName"].Value.ToString();
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
                case "查詢此歌手所有歌曲":
                    string SingerName = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Singer"].Value.ToString();
                    SongQuery_QueryType_ComboBox.SelectedValue = 2;
                    SongQuery_QueryValue_TextBox.Text = SingerName;
                    SongQuery_QueryFilter_ComboBox.SelectedValue = 1;
                    SongQuery_EditMode_CheckBox.Checked = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
                    break;
                case "刪除資料列":
                    string MessageText = (Global.SongMgrSongAddMode != "3") ? "你確定要刪除歌曲資料及歌曲檔案嗎?" : "你確定要刪除歌曲資料嗎?";
                    if (MessageBox.Show(MessageText, "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        List<string> SongIdlist = new List<string>();
                        List<string> SongFilelist = new List<string>();
                        List<string> RemoveSongIdlist = new List<string>();

                        foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                        {
                            SongIdlist.Add(row.Cells["Song_Id"].Value.ToString());
                            SongFilelist.Add(row.Cells["Song_FullPath"].Value.ToString());
                        }

                        SongQuery.CreateSongDataTable();
                        Common_SwitchSetUI(false);

                        var task = Task<List<string>>.Factory.StartNew(() => SongQuery_SongRemove(SongIdlist, SongFilelist));
                        RemoveSongIdlist = task.Result;

                        using (DataTable dt = (DataTable)SongQuery_DataGridView.DataSource)
                        {
                            List<DataRow> RemoveRomList = new List<DataRow>();
                            foreach (DataRow row in dt.Rows)
                            {
                                if (RemoveSongIdlist.IndexOf(row["Song_Id"].ToString()) > -1)
                                {
                                    RemoveRomList.Add(row);
                                }
                            }

                            foreach (DataRow row in RemoveRomList)
                            {
                                dt.Rows.Remove(row);
                            }
                            RemoveRomList.Clear();
                        }

                        if (SongQuery_DataGridView.RowCount == 0)
                        {
                            SongQuery_DataGridView.DataSource = null;
                            if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");

                            SongQuery_InitializeEditControl();
                            SongQuery_QueryStatus_Label.Text = "已無歌曲可編輯!";
                        }
                        SongQuery.DisposeSongDataTable();
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

        #endregion

        #region --- SongQuery 列表滑鼠雙擊事件 ---

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

        #endregion

        #region --- SongQuery 列表滑鼠點擊狀態事件 ---

        private void SongQuery_DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                Global.SongQueryDataGridViewRestoreCurrentRow = SongQuery_DataGridView.CurrentRow.Cells["Song_Id"].Value.ToString();
                Global.SongQueryDataGridViewRestoreSelectList = new List<string>();
                foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                {
                    string SongId = row.Cells["Song_Id"].Value.ToString();
                    Global.SongQueryDataGridViewRestoreSelectList.Add(SongId);
                }
            }
        }

        private void SongQuery_DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SongQueryDataGridViewSelectList = new List<string>();

                    foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
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
                        string SongSrcPath = Path.Combine(SongPath, SongFileName);

                        string SelectValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongSrcPath;
                        Global.SongQueryDataGridViewSelectList.Add(SelectValue);
                    }
                }
            }
        }

        #endregion

        #region --- SongQuery 列表鍵盤點擊狀態事件 ---

        private void SongQuery_DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                if (SongQuery_DataGridView.SelectedRows.Count > 0)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Delete:
                            string MessageText = (Global.SongMgrSongAddMode != "3") ? "你確定要刪除歌曲資料及歌曲檔案嗎?" : "你確定要刪除歌曲資料嗎?";
                            if (MessageBox.Show(MessageText, "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                List<string> SongIdlist = new List<string>();
                                List<string> SongFilelist = new List<string>();
                                List<string> RemoveSongIdlist = new List<string>();

                                foreach (DataGridViewRow row in SongQuery_DataGridView.SelectedRows)
                                {
                                    SongIdlist.Add(row.Cells["Song_Id"].Value.ToString());
                                    SongFilelist.Add(row.Cells["Song_FullPath"].Value.ToString());
                                }

                                SongQuery.CreateSongDataTable();
                                Common_SwitchSetUI(false);

                                var task = Task<List<string>>.Factory.StartNew(() => SongQuery_SongRemove(SongIdlist, SongFilelist));
                                RemoveSongIdlist = task.Result;

                                using (DataTable dt = (DataTable) SongQuery_DataGridView.DataSource)
                                {
                                    List<DataRow> RemoveRomList = new List<DataRow>();
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        if (RemoveSongIdlist.IndexOf(row["Song_Id"].ToString()) > -1)
                                        {
                                            RemoveRomList.Add(row);
                                        }
                                    }

                                    foreach (DataRow row in RemoveRomList)
                                    {
                                        dt.Rows.Remove(row);
                                    }
                                    RemoveRomList.Clear();
                                }

                                if (SongQuery_DataGridView.RowCount == 0)
                                {
                                    SongQuery_DataGridView.DataSource = null;
                                    if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");

                                    SongQuery_InitializeEditControl();
                                    SongQuery_QueryStatus_Label.Text = "已無歌曲可編輯!";
                                }
                                SongQuery.DisposeSongDataTable();
                                Common_SwitchSetUI(true);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region --- SongQuery 列表選取項目變更事件 ---

        private void SongQuery_DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (SongQuery_EditMode_CheckBox.Checked == true)
            {
                int SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;
                List<string> SongSongTypeList = new List<string>(Global.SongMgrSongType.Split(','));
                if (SongQuery_QueryStatus_Label.Text != "") SongQuery_QueryStatus_Label.Text = "";
                Global.SongQueryDataGridViewSelectList.Clear();

                if (SelectedRowsCount > 1)
                {
                    Global.SongQueryMultiEditUpdateList = new List<bool>() { false, false, false, false, false, false, false, false };

                    if (!Global.SongQueryMultiEdit)
                    {
                        SongQuery_Edit_GroupBox.Text = "批次編輯";
                        SongQuery_GetSongEditComboBoxList(true);

                        SongQuery_EditSongId_TextBox.Enabled = false;
                        SongQuery_EditSongLang_ComboBox.Enabled = true;
                        SongQuery_EditSongCreatDate_DateTimePicker.Enabled = true;
                        SongQuery_EditSongSinger_TextBox.Enabled = true;
                        SongQuery_EditSongSingerType_ComboBox.Enabled = true;
                        SongQuery_EditSongSongName_TextBox.Enabled = false;
                        SongQuery_EditSongSongType_ComboBox.Enabled = true;
                        SongQuery_EditSongSpell_TextBox.Enabled = false;
                        SongQuery_EditSongWordCount_TextBox.Enabled = false;
                        SongQuery_EditSongSrcPath_TextBox.Enabled = false;
                        SongQuery_EditSongTrack_ComboBox.Enabled = true;
                        SongQuery_EditSongTrack_Button.Enabled = false;
                        SongQuery_EditSongVolume_TextBox.Enabled = true;
                        SongQuery_EditSongPlayCount_TextBox.Enabled = true;
                        SongQuery_EditApplyChanges_Button.Enabled = true;

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
                }
                else if (SelectedRowsCount == 1)
                {
                    SongQuery_Edit_GroupBox.Text = "單曲編輯";
                    SongQuery_GetSongEditComboBoxList(false);

                    SongQuery_EditSongId_TextBox.Enabled = true;
                    SongQuery_EditSongLang_ComboBox.Enabled = true;
                    SongQuery_EditSongCreatDate_DateTimePicker.Enabled = true;
                    SongQuery_EditSongSinger_TextBox.Enabled = true;
                    SongQuery_EditSongSingerType_ComboBox.Enabled = true;
                    SongQuery_EditSongSongName_TextBox.Enabled = true;
                    SongQuery_EditSongSongType_ComboBox.Enabled = true;
                    SongQuery_EditSongSpell_TextBox.Enabled = true;
                    SongQuery_EditSongWordCount_TextBox.Enabled = true;
                    SongQuery_EditSongSrcPath_TextBox.Enabled = true;
                    SongQuery_EditSongTrack_ComboBox.Enabled = true;
                    SongQuery_EditSongTrack_Button.Enabled = true;
                    SongQuery_EditSongVolume_TextBox.Enabled = true;
                    SongQuery_EditSongPlayCount_TextBox.Enabled = true;
                    SongQuery_EditApplyChanges_Button.Enabled = true;

                    string SongId = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Id"].Value.ToString();
                    string SongLang = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Lang"].Value.ToString();
                    int SongSingerType = Convert.ToInt32(SongQuery_DataGridView.SelectedRows[0].Cells["Song_SingerType"].Value);
                    string SongSinger = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Singer"].Value.ToString();
                    string SongSongName = SongQuery_DataGridView.SelectedRows[0].Cells["Song_SongName"].Value.ToString();
                    int SongTrack = Convert.ToInt32(SongQuery_DataGridView.SelectedRows[0].Cells["Song_Track"].Value);
                    string SongSongType = SongQuery_DataGridView.SelectedRows[0].Cells["Song_SongType"].Value.ToString();
                    string SongVolume = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Volume"].Value.ToString();
                    string SongWordCount = SongQuery_DataGridView.SelectedRows[0].Cells["Song_WordCount"].Value.ToString();
                    string SongPlayCount = SongQuery_DataGridView.SelectedRows[0].Cells["Song_PlayCount"].Value.ToString();
                    string SongMB = SongQuery_DataGridView.SelectedRows[0].Cells["Song_MB"].Value.ToString();
                    string SongCreatDate = SongQuery_DataGridView.SelectedRows[0].Cells["Song_CreatDate"].Value.ToString();
                    string SongFileName = SongQuery_DataGridView.SelectedRows[0].Cells["Song_FileName"].Value.ToString();
                    string SongPath = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Path"].Value.ToString();
                    string SongSpell = SongQuery_DataGridView.SelectedRows[0].Cells["Song_Spell"].Value.ToString();
                    string SongSpellNum = SongQuery_DataGridView.SelectedRows[0].Cells["Song_SpellNum"].Value.ToString();
                    string SongSongStroke = SongQuery_DataGridView.SelectedRows[0].Cells["Song_SongStroke"].Value.ToString();
                    string SongPenStyle = SongQuery_DataGridView.SelectedRows[0].Cells["Song_PenStyle"].Value.ToString();
                    string SongPlayState = SongQuery_DataGridView.SelectedRows[0].Cells["Song_PlayState"].Value.ToString();
                    string SongSrcPath = Path.Combine(SongPath, SongFileName);

                    SongQuery_EditSongId_TextBox.Text = SongId;
                    SongQuery_EditSongLang_ComboBox.SelectedValue = Global.CrazyktvSongLangList.IndexOf(SongLang) + 1;
                    SongQuery_EditSongCreatDate_DateTimePicker.Value = DateTime.Parse(SongCreatDate);
                    SongQuery_EditSongSinger_TextBox.Text = SongSinger;
                    string SongSingerTypeStr = CommonFunc.GetSingerTypeStr(SongSingerType, 1, "null");
                    SongQuery_EditSongSingerType_ComboBox.SelectedValue = Convert.ToInt32(CommonFunc.GetSingerTypeStr(0, 3, SongSingerTypeStr)) + 1;
                    SongQuery_EditSongSongName_TextBox.Text = SongSongName;
                    SongQuery_EditSongSongType_ComboBox.SelectedValue = (SongSongType == "") ? 1 : SongSongTypeList.IndexOf(SongSongType) + 2;
                    SongQuery_EditSongSpell_TextBox.Text = SongSpell;
                    SongQuery_EditSongWordCount_TextBox.Text = SongWordCount;
                    SongQuery_EditSongSrcPath_TextBox.Text = SongSrcPath;
                    SongQuery_EditSongTrack_ComboBox.SelectedValue = SongTrack;
                    SongQuery_EditSongVolume_TextBox.Text = SongVolume;
                    SongQuery_EditSongPlayCount_TextBox.Text = SongPlayCount;

                    Global.SongQueryDataGridViewSelectList = new List<string>();
                    string SelectValue = SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + SongSrcPath;
                    Global.SongQueryDataGridViewSelectList.Add(SelectValue);
                }
            }
        }

        #endregion

        #region --- SongQuery 列表排序事件 ---

        private void SongQuery_DataGridView_Sorted(object sender, EventArgs e)
        {
            SongQuery_DataGridView.ClearSelection();
            if (Global.SongQueryDataGridViewRestoreCurrentRow != "")
            {
                var query = from row in SongQuery_DataGridView.Rows.Cast<DataGridViewRow>()
                            where row.Cells["Song_Id"].Value.Equals(Global.SongQueryDataGridViewRestoreCurrentRow)
                            select row;

                if (query.Count() > 0)
                {
                    foreach (DataGridViewRow row in query)
                    {
                        SongQuery_DataGridView.CurrentCell = row.Cells[0];
                    }
                }
            }

            foreach (string str in Global.SongQueryDataGridViewRestoreSelectList)
            {
                var query = from row in SongQuery_DataGridView.Rows.Cast<DataGridViewRow>()
                            where row.Cells["Song_Id"].Value.Equals(str)
                            select row;

                foreach (DataGridViewRow row in query)
                {
                    row.Selected = true;
                }
            }
            Global.SongQueryDataGridViewRestoreSelectList.Clear();

            foreach (DataGridViewRow row in SongQuery_DataGridView.Rows)
            {
                string SongFullPath = row.Cells["Song_Path"].Value.ToString() + row.Cells["Song_FileName"].Value.ToString();
                row.Cells["Song_FullPath"].Value = SongFullPath;
            }
        }

        #endregion

    }
}
