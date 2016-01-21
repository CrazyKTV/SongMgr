using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {

        #region --- SingerMgr 列表欄位格式 ---

        private void SingerMgr_DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int val;
            string valStr = "";
            switch (SingerMgr_DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "Singer_Type":
                    val = Convert.ToInt32(e.Value);
                    valStr = Global.CrazyktvSingerTypeList[val];
                    e.Value = valStr;
                    e.FormattingApplied = true;
                    break;
            }
        }

        #endregion

        #region --- SingerMgr 列表滑鼠點擊事件 ---

        private void SingerMgr_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0)) return;

            if (e.Button == MouseButtons.Right)
            {
                if (!SingerMgr_DataGridView.Rows[e.RowIndex].Selected) SingerMgr_DataGridView.CurrentCell = SingerMgr_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                List<string> ContextMenuItemList = new List<string>() { "查詢歌曲", "移除歌手" };

                if (SingerMgr_EditMode_CheckBox.Checked == true) // 編輯模式
                {
                    if (SingerMgr_DataGridView.SelectedRows.Count > 1)
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        GridView_ContextMenuItem = new ToolStripMenuItem[1];

                        for (int i = 0; i < 1; i++)
                        {
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(ContextMenuItemList[1]);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SingerMgr_DataGridView_ContextMenuItem_RightClick);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                    else
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        GridView_ContextMenuItem = new ToolStripMenuItem[ContextMenuItemList.Count];

                        for (int i = 0; i < ContextMenuItemList.Count; i++)
                        {
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(ContextMenuItemList[i]);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SingerMgr_DataGridView_ContextMenuItem_RightClick);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                }
                else // 查詢模式
                {
                    if (SingerMgr_DataGridView.SelectedRows.Count == 1)
                    {
                        GridView_ContextMenu = new ContextMenuStrip();
                        GridView_ContextMenuItem = new ToolStripMenuItem[1];

                        for (int i = 0; i < 1; i++)
                        {
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(ContextMenuItemList[0]);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SingerMgr_DataGridView_ContextMenuItem_RightClick);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                    }
                }
            }
        }

        #endregion

        #region --- SingerMgr 列表滑鼠右鍵功能表點擊事件 ---

        private void SingerMgr_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            switch (sender.ToString())
            {
                case "查詢歌曲":
                    string SingerName = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Name"].Value.ToString();
                    SongQuery_QueryType_ComboBox.SelectedValue = 2;
                    SongQuery_QueryValue_TextBox.Text = SingerName;
                    SongQuery_QueryFilter_ComboBox.SelectedValue = 1;
                    SongQuery_EditMode_CheckBox.Checked = false;
                    SongQuery_Query_Button_Click(new Button(), new EventArgs());
                    MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
                    break;
                case "移除歌手":
                    if (MessageBox.Show("你確定要移除歌手嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        List<string> SingerIdlist = new List<string>();

                        foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                        {
                            SingerIdlist.Add(row.Cells["Singer_Id"].Value.ToString());
                            SingerMgr_DataGridView.Rows.Remove(row);
                        }

                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        Common_SwitchSetUI(false);
                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerRemoveTask(SingerIdlist)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Common_SwitchSetUI(true);
                                Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                                SingerMgr_Tooltip_Label.Text = "已成功移除 " + Global.TotalList[0] + " 位歌手資料。";
                            });
                        });
                    }
                    break;
            }
        }

        #endregion

        #region --- SingerMgr 列表滑鼠雙擊事件 ---

        private void SingerMgr_DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string SingerName = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Name"].Value.ToString();
            SongQuery_QueryType_ComboBox.SelectedValue = 2;
            SongQuery_QueryValue_TextBox.Text = SingerName;
            SongQuery_QueryFilter_ComboBox.SelectedValue = 1;
            SongQuery_EditMode_CheckBox.Checked = false;
            SongQuery_Query_Button_Click(new Button(), new EventArgs());
            MainTabControl.SelectedIndex = MainTabControl.TabPages.IndexOf(SongQuery_TabPage);
        }

        #endregion

        #region --- SingerMgr 列表滑鼠點擊狀態事件 ---

        private void SingerMgr_DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                Global.SingerMgrDataGridViewRestoreCurrentRow = SingerMgr_DataGridView.CurrentRow.Cells["Singer_Id"].Value.ToString();
                Global.SingerMgrDataGridViewRestoreSelectList = new List<string>();
                foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                {
                    string SingerId = row.Cells["Singer_Id"].Value.ToString();
                    Global.SingerMgrDataGridViewRestoreSelectList.Add(SingerId);
                }
            }
        }

        private void SingerMgr_DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (SingerMgr_EditMode_CheckBox.Checked == true)
            {
                int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    Global.SingerMgrDataGridViewSelectList = new List<string>();

                    foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                    {
                        string SingerId = row.Cells["Singer_Id"].Value.ToString();
                        int SingerType = Convert.ToInt32(row.Cells["Singer_Type"].Value);
                        string SingerName = row.Cells["Singer_Name"].Value.ToString();
                        string SingerSpell = row.Cells["Singer_Spell"].Value.ToString();
                        string SingerStrokes = row.Cells["Singer_Strokes"].Value.ToString();
                        string SingerSpellNum = row.Cells["Singer_SpellNum"].Value.ToString();
                        string SingerPenStyle = row.Cells["Singer_PenStyle"].Value.ToString();

                        string SelectValue = SingerId + "|" + SingerType + "|" + SingerName + "|" + SingerSpell + "|" + SingerStrokes + "|" + SingerSpellNum + "|" + SingerPenStyle;
                        Global.SingerMgrDataGridViewSelectList.Add(SelectValue);
                    }
                }
            }
        }

        #endregion

        #region --- SingerMgr 列表鍵盤點擊狀態事件 ---

        private void SingerMgr_DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (SingerMgr_EditMode_CheckBox.Checked == true)
            {
                if (SingerMgr_DataGridView.SelectedRows.Count > 0)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Delete:
                            if (MessageBox.Show("你確定要移除歌手嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                List<string> SingerIdlist = new List<string>();

                                foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                                {
                                    SingerIdlist.Add(row.Cells["Singer_Id"].Value.ToString());
                                    SingerMgr_DataGridView.Rows.Remove(row);
                                }

                                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                                Common_SwitchSetUI(false);
                                var tasks = new List<Task>();
                                tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerRemoveTask(SingerIdlist)));

                                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                                {
                                    this.BeginInvoke((Action)delegate()
                                    {
                                        Common_SwitchSetUI(true);
                                        Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                                        SingerMgr_Tooltip_Label.Text = "已成功移除 " + Global.TotalList[0] + " 位歌手資料。";
                                    });
                                });
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region --- SingerMgr 列表選取項目變更事件 ---

        private void SingerMgr_DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (SingerMgr_Tooltip_Label.Text != "" && SingerMgr.ClearTooltipLabel)
            {
                SingerMgr_Tooltip_Label.Text = "";
            }
            else
            {
                SingerMgr.ClearTooltipLabel = true;
            }

            if (SingerMgr_EditMode_CheckBox.Checked == true)
            {
                int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;
                Global.SingerMgrDataGridViewSelectList.Clear();

                if (SelectedRowsCount > 1)
                {
                    Global.SingerMgrMultiEditUpdateList = new List<bool>() { false };

                    if (!Global.SingerMgrMultiEdit)
                    {
                        SingerMgr_Edit_GroupBox.Text = "批次編輯";
                        SingerMgr_GetSingerEditComboBoxList(true);

                        SingerMgr_EditSingerId_TextBox.Enabled = false;
                        SingerMgr_EditSingerType_ComboBox.Enabled = true;
                        SingerMgr_EditSingerName_TextBox.Enabled = false;
                        SingerMgr_EditSingerSpell_TextBox.Enabled = false;
                        SingerMgr_EditApplyChanges_Button.Enabled = true;

                        SingerMgr_EditSingerId_TextBox.Text = "";
                        SingerMgr_EditSingerType_ComboBox.SelectedValue = 1;
                        SingerMgr_EditSingerName_TextBox.Text = "";
                        SingerMgr_EditSingerSpell_TextBox.Text = "";

                        SingerMgr_EditSingerImg_Panel.Enabled = false;
                        SingerMgr_EditSingerImg_Panel.BackColor = Color.Transparent;
                        SingerMgr_EditSingerImg_Panel.BackgroundImage = null;
                    }
                }
                else if (SelectedRowsCount == 1)
                {
                    SingerMgr_Edit_GroupBox.Text = "歌手編輯";
                    SingerMgr_GetSingerEditComboBoxList(false);

                    SingerMgr_EditSingerId_TextBox.Enabled = false;
                    SingerMgr_EditSingerType_ComboBox.Enabled = true;
                    SingerMgr_EditSingerName_TextBox.Enabled = true;
                    SingerMgr_EditSingerSpell_TextBox.Enabled = false;
                    SingerMgr_EditApplyChanges_Button.Enabled = true;

                    string SingerId = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Id"].Value.ToString();
                    int SingerType = Convert.ToInt32(SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Type"].Value);
                    string SingerName = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Name"].Value.ToString();
                    string SingerSpell = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Spell"].Value.ToString();
                    string SingerStrokes = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_Strokes"].Value.ToString();
                    string SingerSpellNum = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_SpellNum"].Value.ToString();
                    string SingerPenStyle = SingerMgr_DataGridView.SelectedRows[0].Cells["Singer_PenStyle"].Value.ToString();

                    SingerMgr_EditSingerId_TextBox.Text = SingerId;
                    string SongSingerTypeStr = CommonFunc.GetSingerTypeStr(SingerType, 1, "null");
                    SingerMgr_EditSingerType_ComboBox.SelectedValue = Convert.ToInt32(CommonFunc.GetSingerTypeStr(0, 3, SongSingerTypeStr)) + 1; ;
                    SingerMgr_EditSingerName_TextBox.Text = SingerName;
                    SingerMgr_EditSingerSpell_TextBox.Text = SingerSpell;

                    SingerMgr_EditSingerImg_Panel.Enabled = true;
                    string ImageFile = "";
                    List<string> SupportFormat = new List<string>() { ".bmp", ".gif", ".jpg", ".png" };
                    foreach (string FileExt in SupportFormat)
                    {
                        if (File.Exists(Application.StartupPath + @"\Web\singerimg\" + SingerName + FileExt))
                        {
                            SingerMgr_EditSingerImg_Panel.BackColor = Color.Transparent;
                            ImageFile = Application.StartupPath + @"\Web\singerimg\" + SingerName + FileExt;
                        }
                    }

                    if (ImageFile == "")
                    {
                        if (File.Exists(Application.StartupPath + @"\Web\images\singertype_default.png"))
                        {
                            SingerMgr_EditSingerImg_Panel.BackColor = Color.ForestGreen;
                            ImageFile = Application.StartupPath + @"\Web\images\singertype_default.png";
                        }
                        else
                        {
                            SingerMgr_EditSingerImg_Panel.BackColor = Color.Transparent;
                            SingerMgr_EditSingerImg_Panel.BackgroundImage = null;
                        }
                    }

                    if (ImageFile !="")
                    {
                        Image img = Image.FromFile(ImageFile);
                        Bitmap bmp = new Bitmap(img);
                        img.Dispose();
                        SingerMgr_EditSingerImg_Panel.BackgroundImage = bmp;
                    }

                    Global.SingerMgrDataGridViewSelectList = new List<string>();
                    string SelectValue = SingerId + "|" + SingerType + "|" + SingerName + "|" + SingerSpell + "|" + SingerStrokes + "|" + SingerSpellNum + "|" + SingerPenStyle;
                    Global.SingerMgrDataGridViewSelectList.Add(SelectValue);
                }
            }
        }

        #endregion

        #region --- SingerMgr 列表排序事件 ---

        private void SingerMgr_DataGridView_Sorted(object sender, EventArgs e)
        {
            if (Global.SingerMgrDataGridViewRestoreSelectList.Count > 0)
            {
                if (Global.SingerMgrDataGridViewRestoreCurrentRow != "")
                {
                    var query = from row in SingerMgr_DataGridView.Rows.Cast<DataGridViewRow>()
                                where row.Cells["Singer_Id"].Value.Equals(Global.SingerMgrDataGridViewRestoreCurrentRow)
                                select row;

                    if (query.Count() > 0)
                    {
                        foreach (DataGridViewRow row in query)
                        {
                            SingerMgr_DataGridView.CurrentCell = row.Cells[0];
                        }
                    }
                }

                SingerMgr_DataGridView.ClearSelection();
                foreach (string str in Global.SingerMgrDataGridViewRestoreSelectList)
                {
                    var query = from row in SingerMgr_DataGridView.Rows.Cast<DataGridViewRow>()
                                where row.Cells["Singer_Id"].Value.ToString().Equals(str)
                                select row;

                    foreach (DataGridViewRow row in query)
                    {
                        row.Selected = true;
                    }
                }
                Global.SingerMgrDataGridViewRestoreSelectList.Clear();
            }
        }

        #endregion

    }
}
