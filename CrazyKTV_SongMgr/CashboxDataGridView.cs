using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
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





    }
}
