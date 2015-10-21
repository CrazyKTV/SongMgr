using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
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

        private void SingerMgr_DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            switch (SingerMgr_DataGridView.Columns[SingerMgr_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌手名稱":
                    if (e.FormattedValue.ToString() == "")
                    {
                        SingerMgr_Tooltip_Label.Text = "此項目的值不可為空白!";
                        e.Cancel = true;
                    }
                    else
                    {
                        Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
                        if (r.IsMatch(e.FormattedValue.ToString()))
                        {
                            SingerMgr_Tooltip_Label.Text = "此項目的值含有非法字元!";
                            e.Cancel = true;
                        }
                    }
                    break;
            }
        }

        private void SingerMgr_DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Global.SingerQueryDataGridViewValue = SingerMgr_DataGridView.CurrentCell.Value.ToString();
        }

        private void SingerMgr_DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0))
            {
                SingerMgr_DataGridView.EndEdit();
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                string EditCell = SingerMgr_DataGridView.Columns[SingerMgr_DataGridView.CurrentCell.ColumnIndex].HeaderText;
                List<string> list = new List<string>();
                list = new List<string>() { "歌手名稱" };

                var query = from editlist in list
                            where editlist == EditCell
                            select editlist;
                foreach (var q in query)
                {
                    SingerMgr_DataGridView.BeginEdit(true);
                }

                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;
                string valStr = "";

                switch (SingerMgr_DataGridView.Columns[SingerMgr_DataGridView.CurrentCell.ColumnIndex].HeaderText)
                {
                    case "歌手類別":
                        GridView_ContextMenu = new ContextMenuStrip();
                        if (GridView_ContextMenu != null) GridView_ContextMenu.Dispose();
                        GridView_ContextMenuItem = new ToolStripMenuItem[9];
                        GridView_ContextMenu = new ContextMenuStrip();
                        
                        List<string> SingerTypeList = new List<string>();

                        foreach (string str in Global.CrazyktvSingerTypeList)
                        {
                            if (str != "未使用")
                            {
                                SingerTypeList.Add(str);
                            }
                        }

                        for (int i = 0; i < 9; i++)
                        {
                            valStr = SingerTypeList[i];
                            GridView_ContextMenuItem[i] = new ToolStripMenuItem(valStr);
                            GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                            GridView_ContextMenuItem[i].Click += new EventHandler(SingerMgr_DataGridView_ContextMenuItem_Click);
                        }
                        GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip GridView_ContextMenu;
                ToolStripMenuItem[] GridView_ContextMenuItem;

                SingerMgr_DataGridView.ClearSelection();
                SingerMgr_DataGridView.Rows[e.RowIndex].Selected = true;
                SingerMgr_DataGridView.CurrentCell = SingerMgr_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                GridView_ContextMenu = new ContextMenuStrip();
                GridView_ContextMenuItem = new ToolStripMenuItem[1];

                for (int i = 0; i < 1; i++)
                {
                    GridView_ContextMenuItem[i] = new ToolStripMenuItem("移除歌手");
                    GridView_ContextMenu.Items.Add(GridView_ContextMenuItem[i]);
                    GridView_ContextMenuItem[i].Click += new EventHandler(SingerMgr_DataGridView_ContextMenuItem_RightClick);
                }
                GridView_ContextMenu.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void SingerMgr_DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (SingerMgr_DataGridView.CurrentCell.Value.ToString() == "") return;
            if (Global.SingerQueryDataGridViewValue != SingerMgr_DataGridView.CurrentCell.Value.ToString())
            {
                SingerMgr.CreateSongDataTable();
                switch (SingerMgr_DataGridView.Columns[SingerMgr_DataGridView.CurrentCell.ColumnIndex].HeaderText)
                {
                    case "歌手名稱":
                        // 取得歌手拼音
                        string SingerName = SingerMgr_DataGridView.CurrentCell.Value.ToString();
                        List<string> SpellList = new List<string>();
                        SpellList = CommonFunc.GetSongNameSpell(SingerName);
                        SingerMgr_DataGridView.CurrentRow.Cells["Singer_Spell"].Value = SpellList[0];
                        SingerMgr_DataGridView.CurrentRow.Cells["Singer_SpellNum"].Value = SpellList[1];
                        if (SpellList[2] == "") SpellList[2] = "0";
                        SingerMgr_DataGridView.CurrentRow.Cells["Singer_Strokes"].Value = SpellList[2];
                        SingerMgr_DataGridView.CurrentRow.Cells["Singer_PenStyle"].Value = SpellList[3];

                        DataTable dt = new DataTable();
                        dt.Columns.Add("RowIndex", typeof(int));
                        dt.Columns.Add("SingerName", typeof(string));
                        DataRow row = dt.NewRow();
                        row["RowIndex"] = SingerMgr_DataGridView.CurrentRow.Index;
                        row["SingerName"] = Global.SingerQueryDataGridViewValue;
                        dt.Rows.Add(row);

                        Common_SwitchSetUI(false);
                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerUpdateTask(dt)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                Common_SwitchSetUI(true);
                                Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                            });
                            SingerMgr.DisposeSongDataTable();
                            dt.Dispose();
                        });
                        break;
                }
            }
            if (SingerMgr_Tooltip_Label.Text == "此項目只能輸入數字!" | SingerMgr_Tooltip_Label.Text == "此項目只能輸入 0 ~ 100 的值!" | SingerMgr_Tooltip_Label.Text == "此項目只能輸入數字及小數點!" | SingerMgr_Tooltip_Label.Text == "此項目的值不可為空白!" | SingerMgr_Tooltip_Label.Text == "此項目的值含有非法字元!") SingerMgr_Tooltip_Label.Text = "";
        }

        private void SingerMgr_DataGridView_ContextMenuItem_Click(object sender, EventArgs e)
        {
            SingerMgr.CreateSongDataTable();
            int SelectedRowsCount = SingerMgr_DataGridView.SelectedRows.Count;
            string CellValue = "";

            switch (SingerMgr_DataGridView.Columns[SingerMgr_DataGridView.CurrentCell.ColumnIndex].HeaderText)
            {
                case "歌手類別":

                    CellValue = Global.CrazyktvSingerTypeList.IndexOf(sender.ToString()).ToString();
                    break;
            }

            if (SingerMgr_DataGridView.CurrentCell.Value.ToString() != CellValue)
            {
                SingerMgr_DataGridView.CurrentCell.Value = CellValue;

                DataTable dt = new DataTable();
                dt.Columns.Add("RowIndex", typeof(int));
                dt.Columns.Add("SingerName", typeof(string));
                DataRow row = dt.NewRow();
                row["RowIndex"] = SingerMgr_DataGridView.CurrentRow.Index;
                row["SingerName"] = SingerMgr_DataGridView.CurrentRow.Cells["Singer_Name"].Value;
                dt.Rows.Add(row);

                Common_SwitchSetUI(false);
                var tasks = new List<Task>();
                tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerUpdateTask(dt)));

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    this.BeginInvoke((Action)delegate()
                    {
                        Common_SwitchSetUI(true);
                        Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                    });
                    SingerMgr.DisposeSongDataTable();
                    dt.Dispose();
                });
            }
            SingerMgr_DataGridView.EndEdit();
        }

        private void SingerMgr_DataGridView_ContextMenuItem_RightClick(object sender, EventArgs e)
        {
            switch (sender.ToString())
            {
                case "移除歌手":
                    if (MessageBox.Show("你確定要移除歌手嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        List<string> SingerIdlist = new List<string>();

                        foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                        {
                            SingerIdlist.Add(row.Cells["Singer_Id"].Value.ToString());
                            SingerMgr_DataGridView.Rows.Remove(row);
                        }

                        Common_SwitchSetUI(false);
                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerRemoveTask(SingerIdlist)));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Common_SwitchSetUI(true);
                                Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                            });
                        });
                    }
                    break;
            }
        }

        private void SingerMgr_DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 46)
            {
                if (MessageBox.Show("你確定要移除歌手嗎?", "刪除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<string> SingerIdlist = new List<string>();

                    foreach (DataGridViewRow row in SingerMgr_DataGridView.SelectedRows)
                    {
                        SingerIdlist.Add(row.Cells["Singer_Id"].Value.ToString());
                        SingerMgr_DataGridView.Rows.Remove(row);
                    }

                    Common_SwitchSetUI(false);
                    var tasks = new List<Task>();
                    tasks.Add(Task.Factory.StartNew(() => SingerMgr_SingerRemoveTask(SingerIdlist)));

                    Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                    {
                        this.BeginInvoke((Action)delegate()
                        {
                            Common_SwitchSetUI(true);
                            Task.Factory.StartNew(() => Common_GetSingerStatisticsTask());
                        });
                    });
                }
            }
        }


    }



    class SingerMgrDataGridView
    {
    }
}
