using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
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
        private void SongMgrCfg_Save_Button_Click(object sender, EventArgs e)
        {
            if (Global.SongMgrSongType == "") Global.SongMgrSongType = "null";
            SongMgrCfg_GetSongMgrLangCode();
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile", Global.CrazyktvDatabaseFile);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSupportFormat", Global.SongMgrSupportFormat);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder", Global.SongMgrDestFolder);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongAddMode", Global.SongMgrSongAddMode);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusMerge", Global.SongMgrChorusMerge);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode", Global.SongMgrMaxDigitCode);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode", Global.SongMgrLangCode);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType", Global.SongMgrSongType);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongInfoSeparate", Global.SongMgrSongInfoSeparate);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusSeparate", Global.SongMgrChorusSeparate);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFolderStructure", Global.SongMgrFolderStructure);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFileStructure", Global.SongMgrFileStructure);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongTrackMode", Global.SongMgrSongTrackMode);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrBackupRemoveSong", Global.SongMgrBackupRemoveSong);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrCustomSingerTypeStructure", Global.SongMgrCustomSingerTypeStructure);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrEnableMonitorFolders", Global.SongMgrEnableMonitorFolders);
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMonitorFolders", string.Join(",", Global.SongMgrMonitorFoldersList));
            CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSingerGroup", Global.SongMgrSingerGroup);
        }


        private void SongMgrCfg_DBFile_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            if (SongMgrCfg_DBFile_TextBox.Text != "") opd.InitialDirectory = Path.GetDirectoryName(SongMgrCfg_DBFile_TextBox.Text);
            opd.Filter = "資料庫檔案 (*.mdb)|*.mdb";
            opd.FilterIndex = 1;

            if (opd.ShowDialog() == DialogResult.OK && opd.FileName.Length > 0)
            {
                Global.CrazyktvDatabaseFile = opd.FileName;
                SongMgrCfg_DBFile_TextBox.Text = opd.FileName;
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile", Global.CrazyktvDatabaseFile);
                Common_RefreshSongMgr(true);
                if (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString() == "4")
                {
                    SongMgrCfg_MonitorFolders_CheckBox.Checked = false;
                    SongMgrCfg_MonitorFolders_CheckBox_CheckedChanged(new object(), new EventArgs());
                }
            }
        }


        private void SongMgrCfg_SupportFormat_TextBox_TextChanged(object sender, EventArgs e)
        {
            Global.SongMgrSupportFormat = SongMgrCfg_SupportFormat_TextBox.Text.ToLower();
        }


        private void SongMgrCfg_DestFolder_Button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog opd = new FolderBrowserDialog();
            if (SongMgrCfg_DestFolder_TextBox.Text != "") opd.SelectedPath = SongMgrCfg_DestFolder_TextBox.Text;

            if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
            {
                Global.SongMgrDestFolder = opd.SelectedPath;
                SongMgrCfg_DestFolder_TextBox.Text = opd.SelectedPath;
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder", Global.SongMgrDestFolder);
                Common_RefreshSongMgr(true);
            }
        }


        private void SongMgrCfg_SongAddMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                    Global.SongMgrSongAddMode = SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString();

                    SongMaintenance_RebuildSongStructure_GroupBox.Enabled = true;
                    SongMgrCfg_DestFolder_TextBox.Enabled = true;
                    SongMgrCfg_DestFolder_Button.Enabled = true;
                    SongAdd_DupSongMode_ComboBox.Enabled = true;
                    SongAdd_DataGridView.Visible = true;
                    SongAdd_DragDrop_Label.Visible = true;

                    if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SongStructure_TabPage) < 0)
                    {
                        SongMgrCfg_TabControl.TabPages.Insert(SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SingerGroup_TabPage) + 1, SongMgrCfg_SongStructure_TabPage);
                        SongMgrCfg_SongStructure_TabPage.Show();
                    }

                    if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_CustomStructure_TabPage) < 0)
                    {
                        SongMgrCfg_TabControl.TabPages.Insert(SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SingerGroup_TabPage) + 2, SongMgrCfg_CustomStructure_TabPage);
                        SongMgrCfg_CustomStructure_TabPage.Show();
                    }

                    if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_MonitorFolders_TabPage) >= 0)
                    {
                        SongMgrCfg_MonitorFolders_TabPage.Hide();
                        SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_MonitorFolders_TabPage);
                    }

                    if (Global.SongMgrInitializeStatus)
                    {
                        SongMgrCfg_MonitorFolders_CheckBox.Checked = false;
                        SongMgrCfg_MonitorFolders_CheckBox_CheckedChanged(new object(), new EventArgs());
                        Common_RefreshSongMgr(false);
                    }
                    break;
                case "3":
                case "4":
                    Global.SongMgrSongAddMode = SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString();

                    SongMaintenance_RebuildSongStructure_GroupBox.Enabled = false;
                    SongMgrCfg_DestFolder_TextBox.Enabled = false;
                    SongMgrCfg_DestFolder_Button.Enabled = false;

                    if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SongStructure_TabPage) >= 0)
                    {
                        SongMgrCfg_SongStructure_TabPage.Hide();
                        SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_SongStructure_TabPage);
                    }

                    if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_CustomStructure_TabPage) >= 0)
                    {
                        SongMgrCfg_CustomStructure_TabPage.Hide();
                        SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_CustomStructure_TabPage);
                    }

                    if (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString() == "4" && SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_MonitorFolders_TabPage) < 0)
                    {
                        SongMgrCfg_TabControl.TabPages.Insert(SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SingerGroup_TabPage) + 1, SongMgrCfg_MonitorFolders_TabPage);
                        SongMgrCfg_MonitorFolders_TabPage.Show();
                    }
                    else if (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString() == "3" && SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_MonitorFolders_TabPage) >= 0)
                    {
                        SongMgrCfg_MonitorFolders_TabPage.Hide();
                        SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_MonitorFolders_TabPage);
                    }

                    if (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString() == "4")
                    {
                        SongAdd_DupSongMode_ComboBox.SelectedValue = 1;
                        SongAdd_DupSongMode_ComboBox.Enabled = false;
                        SongAdd_DataGridView.Visible = false;
                        SongAdd_DragDrop_Label.Visible = false;
                        SongMgrCfg_MonitorFolders_SetUI();
                    }
                    else
                    {
                        SongAdd_DupSongMode_ComboBox.Enabled = true;
                        SongAdd_DataGridView.Visible = true;
                        SongAdd_DragDrop_Label.Visible = true;
                    }
                    if (SongMgrCfg_Tooltip_Label.Text == "請先設定歌庫資料夾!") SongMgrCfg_Tooltip_Label.Text = "";

                    if (Global.SongMgrInitializeStatus)
                    {
                        SongMgrCfg_MonitorFolders_CheckBox.Checked = false;
                        SongMgrCfg_MonitorFolders_CheckBox_CheckedChanged(new object(), new EventArgs());

                        if (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString() == "4")
                        {
                            SongMgrCfg_MonitorFolders_CheckBox.Enabled = false;
                            foreach (string MonitorFolder in Global.SongMgrMonitorFoldersList)
                            {
                                if (MonitorFolder != "") SongMgrCfg_MonitorFolders_CheckBox.Enabled = true;
                            }
                        }
                        Common_RefreshSongMgr(false);
                    }
                    break;
            }
        }


        private void SongMgrCfg_CrtchorusMerge_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongMgrChorusMerge = SongMgrCfg_CrtchorusMerge_CheckBox.Checked.ToString();
        }

        private void SongMgrCfg_SongTrackMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongMgrSongTrackMode = SongMgrCfg_SongTrackMode_CheckBox.Checked.ToString();

            if (Global.SongMgrSongTrackMode == "True")
            {
                Global.CrazyktvSongTrackWordList = new List<string>() { "立體聲", "右聲道 / 音軌2", "左聲道 / 音軌1", "音軌3", "音軌4", "音軌5" };
                Global.CrazyktvSongTrackList = new List<string>() { "V0", "VR", "VL", "V3", "V4", "V5" };
                Global.CrazyktvSongTrackKeyWordList = new List<string>() { "v0,立體", "vr,r,右", "vl,l,左", "v3", "v4", "v5" };
            }
            else
            {
                Global.CrazyktvSongTrackWordList = new List<string>() { "立體聲", "右聲道 / 音軌1", "左聲道 / 音軌2", "音軌3", "音軌4", "音軌5" };
                Global.CrazyktvSongTrackList = new List<string>() { "V0", "VR", "VL", "V3", "V4", "V5" };
                Global.CrazyktvSongTrackKeyWordList = new List<string>() { "v0,立體", "vr,r,右", "vl,l,左", "v3", "v4", "v5" };
            }

            string SongTrackStr = "";
            int SelectedValue = 0;
            int SelectedRowsCount;

            if (SongQuery_QueryType_ComboBox.SelectedValue != null)
            {
                if (SongQuery_QueryType_ComboBox.SelectedValue.ToString() == "9")
                {
                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", false, false);
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = SelectedValue;
                }
            }

            SongTrackStr = ((DataRowView)SongAdd_DefaultSongTrack_ComboBox.SelectedItem)[0].ToString();
            SelectedValue = 0;

            SongAdd_DefaultSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack", false, false);
            SongAdd_DefaultSongTrack_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongTrack_ComboBox.ValueMember = "Value";

            var trackquery = from row in ((DataTable)SongAdd_DefaultSongTrack_ComboBox.DataSource).AsEnumerable()
                            where row.Field<string>("Display").Equals(SongTrackStr)
                            select row;

            if (trackquery.Count<DataRow>() > 0)
            {
                foreach (DataRow row in trackquery)
                {
                    SelectedValue = Convert.ToInt32(row["Value"]);
                    break;
                }
            }
            SongAdd_DefaultSongTrack_ComboBox.SelectedValue = SelectedValue;

            if (SongQuery_EditMode_CheckBox.Checked)
            {
                SongTrackStr = ((DataRowView)SongQuery_EditSongTrack_ComboBox.SelectedItem)[0].ToString();
                SelectedValue = 0;
                SelectedRowsCount = SongQuery_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    SongQuery_EditSongTrack_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", true, true);
                    SongQuery_EditSongTrack_ComboBox.DisplayMember = "Display";
                    SongQuery_EditSongTrack_ComboBox.ValueMember = "Value";
                }
                else if (SelectedRowsCount == 1)
                {
                    SongQuery_EditSongTrack_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack", false, true);
                    SongQuery_EditSongTrack_ComboBox.DisplayMember = "Display";
                    SongQuery_EditSongTrack_ComboBox.ValueMember = "Value";
                }

                var query = from row in ((DataTable)SongQuery_EditSongTrack_ComboBox.DataSource).AsEnumerable()
                            where row.Field<string>("Display").Equals(SongTrackStr)
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        SelectedValue = Convert.ToInt32(row["Value"]);
                        break;
                    }
                }
                SongQuery_EditSongTrack_ComboBox.SelectedValue = SelectedValue;
            }

            if (SongAdd_Save_Button.Text == "取消加入")
            {
                SongTrackStr = ((DataRowView)SongAdd_EditSongTrack_ComboBox.SelectedItem)[0].ToString();
                SelectedValue = 0;
                SelectedRowsCount = SongAdd_DataGridView.SelectedRows.Count;

                if (SelectedRowsCount > 1)
                {
                    SongAdd_EditSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack", true, true);
                    SongAdd_EditSongTrack_ComboBox.DisplayMember = "Display";
                    SongAdd_EditSongTrack_ComboBox.ValueMember = "Value";
                }
                else if (SelectedRowsCount == 1)
                {
                    SongAdd_EditSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack", false, true);
                    SongAdd_EditSongTrack_ComboBox.DisplayMember = "Display";
                    SongAdd_EditSongTrack_ComboBox.ValueMember = "Value";
                }

                var query = from row in ((DataTable)SongAdd_EditSongTrack_ComboBox.DataSource).AsEnumerable()
                            where row.Field<string>("Display").Equals(SongTrackStr)
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        SelectedValue = Convert.ToInt32(row["Value"]);
                        break;
                    }
                }
                SongAdd_EditSongTrack_ComboBox.SelectedValue = SelectedValue;
            }
        }

        private void SongMgrCfg_MaxDigitCode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                    Global.SongMgrMaxDigitCode = SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue.ToString();
                    SongMgrCfg_RefreshSongMgrLangCode();
                    break;
            }
        }

        private void SongMgrCfg_RefreshSongMgrLangCode()
        {
            switch (SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    Global.SongMgrLangCode = "10001,20001,30001,40001,50001,60001,70001,80001,90001,95001";
                    break;
                case "2":
                    Global.SongMgrLangCode = "100001,200001,300001,400001,500001,600001,700001,800001,900001,950001";
                    break;
            }
            string[] str = Global.SongMgrLangCode.Split(',');
            SongMgrCfg_Lang1Code_TextBox.Text = str[0];
            SongMgrCfg_Lang2Code_TextBox.Text = str[1];
            SongMgrCfg_Lang3Code_TextBox.Text = str[2];
            SongMgrCfg_Lang4Code_TextBox.Text = str[3];
            SongMgrCfg_Lang5Code_TextBox.Text = str[4];
            SongMgrCfg_Lang6Code_TextBox.Text = str[5];
            SongMgrCfg_Lang7Code_TextBox.Text = str[6];
            SongMgrCfg_Lang8Code_TextBox.Text = str[7];
            SongMgrCfg_Lang9Code_TextBox.Text = str[8];
            SongMgrCfg_Lang10Code_TextBox.Text = str[9];
        }

        private void SongMgrCfg_LoadSongMgrLangCode()
        {
            string[] str = Global.SongMgrLangCode.Split(',');
            SongMgrCfg_Lang1Code_TextBox.Text = str[0];
            SongMgrCfg_Lang2Code_TextBox.Text = str[1];
            SongMgrCfg_Lang3Code_TextBox.Text = str[2];
            SongMgrCfg_Lang4Code_TextBox.Text = str[3];
            SongMgrCfg_Lang5Code_TextBox.Text = str[4];
            SongMgrCfg_Lang6Code_TextBox.Text = str[5];
            SongMgrCfg_Lang7Code_TextBox.Text = str[6];
            SongMgrCfg_Lang8Code_TextBox.Text = str[7];
            SongMgrCfg_Lang9Code_TextBox.Text = str[8];
            SongMgrCfg_Lang10Code_TextBox.Text = str[9];
        }

        private void SongMgrCfg_GetSongMgrLangCode()
        {
            string[] strlist = new[]
            {
                SongMgrCfg_Lang1Code_TextBox.Text,
                SongMgrCfg_Lang2Code_TextBox.Text,
                SongMgrCfg_Lang3Code_TextBox.Text,
                SongMgrCfg_Lang4Code_TextBox.Text,
                SongMgrCfg_Lang5Code_TextBox.Text,
                SongMgrCfg_Lang6Code_TextBox.Text,
                SongMgrCfg_Lang7Code_TextBox.Text,
                SongMgrCfg_Lang8Code_TextBox.Text,
                SongMgrCfg_Lang9Code_TextBox.Text,
                SongMgrCfg_Lang10Code_TextBox.Text
            };
            Global.SongMgrLangCode = string.Join(",", strlist);
        }

        private void SongMgrCfg_SongType_ListBox_Enter(object sender, EventArgs e)
        {
            SongMgrCfg_Tooltip_Label.Text = "";
            SongMgrCfg_SongType_Button.Text = "移除";
        }

        private void SongMgrCfg_SongType_TextBox_Enter(object sender, EventArgs e)
        {
            if (SongMgrCfg_Tooltip_Label.Text != "此項目的值含有非法字元!") SongMgrCfg_Tooltip_Label.Text = "";
            SongMgrCfg_SongType_Button.Text = "加入";
        }

        private void SongMgrCfg_SongType_Button_Click(object sender, EventArgs e)
        {
            switch (SongMgrCfg_SongType_Button.Text)
            {
                case "加入":
                    if (SongMgrCfg_SongType_TextBox.Text != "")
                    {
                        SongMgrCfg_Tooltip_Label.Text = "";

                        if (SongMgrCfg_SongType_TextBox.Text == "無類別")
                        {
                            SongMgrCfg_Tooltip_Label.Text = "此為加歌程式內部所使用的歌曲類別名稱!";
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            List<string> list = new List<string>();

                            if (SongMgrCfg_SongType_ListBox.SelectedItems.Count > 0)
                            {
                                dt = (DataTable)SongMgrCfg_SongType_ListBox.DataSource;
                                dt.Rows.Add(dt.NewRow());
                                dt.Rows[dt.Rows.Count - 1][0] = SongMgrCfg_SongType_TextBox.Text;
                                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                            }
                            else
                            {
                                using (DataTable NewDT = new DataTable())
                                {
                                    NewDT.Columns.Add(new DataColumn("Display", typeof(string)));
                                    NewDT.Columns.Add(new DataColumn("Value", typeof(int)));

                                    NewDT.Rows.Add(NewDT.NewRow());
                                    NewDT.Rows[NewDT.Rows.Count - 1][0] = SongMgrCfg_SongType_TextBox.Text;
                                    NewDT.Rows[NewDT.Rows.Count - 1][1] = NewDT.Rows.Count;
                                    dt = NewDT.Copy();
                                }
                                SongMgrCfg_SongType_ListBox.DataSource = dt;
                                SongMgrCfg_SongType_ListBox.DisplayMember = "Display";
                                SongMgrCfg_SongType_ListBox.ValueMember = "Value";
                            }

                            SongMgrCfg_SongType_TextBox.Text = "";

                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (row[column] != null)
                                    {
                                        if (column.ColumnName == "Display")
                                        {
                                            list.Add(row[column].ToString());
                                        }
                                    }
                                }
                            }

                            Global.SongMgrSongType = string.Join(",", list);
                            list.Clear();
                            Common_RefreshSongType();
                            SongMgrCfg_Tooltip_Label.Text = "已成功加入新的歌曲類別名稱!";
                        }
                    }
                    else
                    {
                        SongMgrCfg_Tooltip_Label.Text = "尚未輸入要加入的歌曲類別名稱!";
                    }
                    break;
                case "移除":
                    if (SongMgrCfg_SongType_ListBox.SelectedItem != null)
                    {
                        if (SongAdd_DefaultSongType_ComboBox.Text != SongMgrCfg_SongType_ListBox.Text)
                        {
                            List<string> list = new List<string>();
                            int index = int.Parse(SongMgrCfg_SongType_ListBox.SelectedIndex.ToString());

                            DataTable dt = (DataTable)SongMgrCfg_SongType_ListBox.DataSource;
                            dt.Rows.RemoveAt(index);

                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    if (row[column] != null)
                                    {
                                        if (column.ColumnName == "Display")
                                        {
                                            list.Add(row[column].ToString());
                                        }
                                    }
                                }
                            }

                            Global.SongMgrSongType = string.Join(",", list);
                            list.Clear();
                            Common_RefreshSongType();
                            SongMgrCfg_Tooltip_Label.Text = "已成功移除所選取的歌曲類別名稱!";
                        }
                        else
                        {
                            SongMgrCfg_Tooltip_Label.Text = "此為預設的歌曲類別名稱!";
                        }
                    }
                    else
                    {
                        SongMgrCfg_Tooltip_Label.Text = "已無可以刪除的歌曲類別名稱!";
                    }
                    break;
            }
        }

        private void SongMgrCfg_LangCode_TextBox_Enter(object sender, EventArgs e)
        {
            switch (SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    ((TextBox)sender).MaxLength = 5;
                    ((TextBox)sender).ImeMode = ImeMode.Close;
                    break;
                case "2":
                    ((TextBox)sender).MaxLength = 6;
                    ((TextBox)sender).ImeMode = ImeMode.Close;
                    break;
            }
        }

        private void SongMgrCfg_LangCode_TextBox_Validating(object sender, CancelEventArgs e)
        {
            switch (Global.SongMgrMaxDigitCode)
            {
                case "1":
                    if (((TextBox)sender).Text.Length != 5)
                    {
                        e.Cancel = true;
                        SongMgrCfg_Tooltip_Label.Text = "此項目必須輸入5位數的歌曲編號";
                    }
                    else
                    {
                        if (SongMgrCfg_Tooltip_Label.Text == "此項目必須輸入5位數的歌曲編號") SongMgrCfg_Tooltip_Label.Text = "";
                    }
                    break;
                case "2":
                    if (((TextBox)sender).Text.Length != 6)
                    {
                        e.Cancel = true;
                        SongMgrCfg_Tooltip_Label.Text = "此項目必須輸入6位數的歌曲編號";
                    }
                    else
                    {
                        if (SongMgrCfg_Tooltip_Label.Text == "此項目必須輸入6位數的歌曲編號") SongMgrCfg_Tooltip_Label.Text = "";
                    }
                    break;
            }

            if (SongMgrCfg_Tooltip_Label.Text != "此項目必須輸入5位數的歌曲編號" & SongMgrCfg_Tooltip_Label.Text != "此項目必須輸入6位數的歌曲編號")
            {
                TextBox[] SongMgrCfg_LangCode_TextBox =
                {
                    SongMgrCfg_Lang1Code_TextBox,
                    SongMgrCfg_Lang2Code_TextBox,
                    SongMgrCfg_Lang3Code_TextBox,
                    SongMgrCfg_Lang4Code_TextBox,
                    SongMgrCfg_Lang5Code_TextBox,
                    SongMgrCfg_Lang6Code_TextBox,
                    SongMgrCfg_Lang7Code_TextBox,
                    SongMgrCfg_Lang8Code_TextBox,
                    SongMgrCfg_Lang9Code_TextBox,
                    SongMgrCfg_Lang10Code_TextBox
                };

                List<string> SongMgrCfg_LangCode_TextBoxName = new List<string>()
                {
                    SongMgrCfg_Lang1Code_TextBox.Name,
                    SongMgrCfg_Lang2Code_TextBox.Name,
                    SongMgrCfg_Lang3Code_TextBox.Name,
                    SongMgrCfg_Lang4Code_TextBox.Name,
                    SongMgrCfg_Lang5Code_TextBox.Name,
                    SongMgrCfg_Lang6Code_TextBox.Name,
                    SongMgrCfg_Lang7Code_TextBox.Name,
                    SongMgrCfg_Lang8Code_TextBox.Name,
                    SongMgrCfg_Lang9Code_TextBox.Name,
                    SongMgrCfg_Lang10Code_TextBox.Name
                };

                bool ValueError = false;
                int i = SongMgrCfg_LangCode_TextBoxName.IndexOf(((TextBox)sender).Name);

                if (i != 0)
                {
                    if (Convert.ToInt32(((TextBox)sender).Text) <= Convert.ToInt32(SongMgrCfg_LangCode_TextBox[i - 1].Text))
                    {
                        e.Cancel = true;
                        SongMgrCfg_Tooltip_Label.Text = "數值不能小於或等於前面語系的歌曲編號";
                    }
                    else
                    {
                        if (SongMgrCfg_Tooltip_Label.Text == "數值不能小於或等於前面語系的歌曲編號") SongMgrCfg_Tooltip_Label.Text = "";

                        foreach (TextBox tb in SongMgrCfg_LangCode_TextBox)
                        {
                            if (tb.Name != "SongMgrCfg_Lang1Code_TextBox" & SongMgrCfg_LangCode_TextBoxName.IndexOf(tb.Name) > i)
                            {
                                if (Convert.ToInt32(((TextBox)sender).Text) >= Convert.ToInt32(tb.Text))
                                {
                                    ValueError = true;
                                    break;
                                }
                            }
                        }

                        if (ValueError)
                        {
                            e.Cancel = true;
                            SongMgrCfg_Tooltip_Label.Text = "數值不能大於或等於後面語系的歌曲編號";
                        }
                        else
                        {
                            if (SongMgrCfg_Tooltip_Label.Text == "數值不能大於或等於後面語系的歌曲編號") SongMgrCfg_Tooltip_Label.Text = "";
                        }
                    }
                }
                else
                {
                    foreach (TextBox tb in SongMgrCfg_LangCode_TextBox)
                    {
                        if (tb.Name != "SongMgrCfg_Lang1Code_TextBox")
                        {
                            if (Convert.ToInt32(((TextBox)sender).Text) >= Convert.ToInt32(tb.Text))
                            {
                                ValueError = true;
                                break;
                            }
                        }
                    }

                    if (ValueError)
                    {
                        e.Cancel = true;
                        SongMgrCfg_Tooltip_Label.Text = "數值不能大於或等於後面語系的歌曲編號";
                    }
                    else
                    {
                        if (SongMgrCfg_Tooltip_Label.Text == "數值不能大於或等於後面語系的歌曲編號") SongMgrCfg_Tooltip_Label.Text = "";
                    }
                }
            }
        }

        private void SongMgrCfg_LangCode_TextBox_Validated(object sender, EventArgs e)
        {
            SongMgrCfg_GetSongMgrLangCode();
            Task.Factory.StartNew(() => CommonFunc.GetRemainingSongIdCount((Global.SongMgrMaxDigitCode == "1") ? 5 : 6));
        }

        private void SongMgrCfg_SongInfoSeparate_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMgrCfg_SongInfoSeparate_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                    Global.SongMgrSongInfoSeparate = SongMgrCfg_SongInfoSeparate_ComboBox.SelectedValue.ToString();
                    break;
            }
        }

        private void SongMgrCfg_CrtchorusSeparate_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMgrCfg_CrtchorusSeparate_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                    Global.SongMgrChorusSeparate = SongMgrCfg_CrtchorusSeparate_ComboBox.SelectedValue.ToString();
                    break;
            }
        }

        private void SongMgrCfg_FolderStructure_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMgrCfg_FolderStructure_ComboBox.SelectedValue.ToString())
            {
                case "1":
                    SongMgrCfg_CrtchorusMerge_CheckBox.Enabled = true;
                    Global.SongMgrFolderStructure = SongMgrCfg_FolderStructure_ComboBox.SelectedValue.ToString();
                    break;
                case "2":
                    SongMgrCfg_CrtchorusMerge_CheckBox.Enabled = false;
                    Global.SongMgrFolderStructure = SongMgrCfg_FolderStructure_ComboBox.SelectedValue.ToString();
                    break;
                case "3":
                    SongMgrCfg_CrtchorusMerge_CheckBox.Enabled = false;
                    Global.SongMgrFolderStructure = SongMgrCfg_FolderStructure_ComboBox.SelectedValue.ToString();
                    break;
            }

            if (SongMgrCfg_FolderStructure_ComboBox.Focused && SongMgrCfg_FolderStructure_ComboBox.Text != "System.Data.DataRowView")
            {
                SongMgrCfg_FileStructure_ComboBox.DataSource = SongMgrCfg.GetFileStructureList();
                SongMgrCfg_FileStructure_ComboBox.DisplayMember = "Display";
                SongMgrCfg_FileStructure_ComboBox.ValueMember = "Value";
                SongMgrCfg_FileStructure_ComboBox.SelectedValue = 1;
            }
        }

        private void SongMgrCfg_FileStructure_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(SongMgrCfg_FileStructure_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                    Global.SongMgrFileStructure = SongMgrCfg_FileStructure_ComboBox.SelectedValue.ToString();
                    break;
            }
        }

        private void SongMgrCfg_SetLangLB()
        {
            Label[] SongMgrCfg_LangCode_Label =
            {
                SongMgrCfg_Lang1Code_Label,
                SongMgrCfg_Lang2Code_Label,
                SongMgrCfg_Lang3Code_Label,
                SongMgrCfg_Lang4Code_Label,
                SongMgrCfg_Lang5Code_Label,
                SongMgrCfg_Lang6Code_Label,
                SongMgrCfg_Lang7Code_Label,
                SongMgrCfg_Lang8Code_Label,
                SongMgrCfg_Lang9Code_Label,
                SongMgrCfg_Lang10Code_Label
            };

            for (int i = 0; i < SongMgrCfg_LangCode_Label.Count<Label>(); i++)
            {
                if (Global.CrazyktvSongLangList[i].Length > 2 )
                {
                    SongMgrCfg_LangCode_Label[i].Text = Global.CrazyktvSongLangList[i].Substring(0, 1) + "語:";
                }
                else
                {
                    SongMgrCfg_LangCode_Label[i].Text = Global.CrazyktvSongLangList[i] + ":";
                }
            }
        }

        private void SongMgrCfg_BackupRemoveSong_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongMgrBackupRemoveSong = SongMgrCfg_BackupRemoveSong_CheckBox.Checked.ToString();
        }

        private void SongMgrCfg_SetCustomSingerTypeStructureCbox()
        {
            ComboBox[] SongMgrCfg_CustomSingerTypeStructure_ComboBox =
            {
                SongMgrCfg_CustomSingerTypeStructure1_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure2_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure3_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure4_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure5_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure6_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure7_ComboBox,
                SongMgrCfg_CustomSingerTypeStructure8_ComboBox
            };

            List<string> cboxvalue = new List<string>(Global.SongMgrCustomSingerTypeStructure.Split(','));

            for (int i = 0; i < SongMgrCfg_CustomSingerTypeStructure_ComboBox.Count<ComboBox>(); i++)
            {
                SongMgrCfg_CustomSingerTypeStructure_ComboBox[i].DataSource = SongMgrCfg.GetCustomSingerTypeStructureList(i);
                SongMgrCfg_CustomSingerTypeStructure_ComboBox[i].DisplayMember = "Display";
                SongMgrCfg_CustomSingerTypeStructure_ComboBox[i].ValueMember = "Value";
                SongMgrCfg_CustomSingerTypeStructure_ComboBox[i].SelectedValue = cboxvalue[i];
            }
            SongMgrCfg.SetCustomSingerTypeStructureList();
        }

        private void SongMgrCfg_CustomSingerTypeStructure_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;

            if (cbox.Focused && cbox.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                ComboBox[] SongMgrCfg_CustomSingerTypeStructure_ComboBox =
                {
                    SongMgrCfg_CustomSingerTypeStructure1_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure2_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure3_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure4_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure5_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure6_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure7_ComboBox,
                    SongMgrCfg_CustomSingerTypeStructure8_ComboBox
                };

                List<string> cboxvalue = new List<string>();
                for (int i = 0; i < SongMgrCfg_CustomSingerTypeStructure_ComboBox.Count<ComboBox>(); i++)
                {
                    cboxvalue.Add(SongMgrCfg_CustomSingerTypeStructure_ComboBox[i].SelectedValue.ToString());
                }

                Global.SongMgrCustomSingerTypeStructure = string.Join(",", cboxvalue);
                SongMgrCfg.SetCustomSingerTypeStructureList();
            }
        }

        private void SongMgrCfg_SingerGroup_ListBox_Enter(object sender, EventArgs e)
        {
            SongMgrCfg_Tooltip_Label.Text = "";
            SongMgrCfg_SingerGroup_Button.Text = "移除";
        }

        private void SongMgrCfg_SingerGroup_TextBox_Enter(object sender, EventArgs e)
        {
            if (SongMgrCfg_Tooltip_Label.Text != "此項目的值含有非法字元!") SongMgrCfg_Tooltip_Label.Text = "";
            SongMgrCfg_SingerGroup_Button.Text = "加入";
        }

        private void SongMgrCfg_SingerGroup_Button_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            switch (SongMgrCfg_SingerGroup_Button.Text)
            {
                case "加入":
                    Regex r = new Regex("^,|,$|,,");
                    if (SongMgrCfg_SingerGroup_TextBox.Text != "" && SongMgrCfg_SingerGroup_TextBox.Text != "," && SongMgrCfg_SingerGroup_TextBox.Text.IndexOf(",") >= 0 && !r.IsMatch(SongMgrCfg_SingerGroup_TextBox.Text))
                    {
                        List<string> SingerGroupLowCaselist = new List<string>();

                        string SongQuerySqlStr = "select * from ktv_SongMgr where Config_Type = 'SingerGroup' order by Config_Value";
                        using (DataTable SingerGroupDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvSongMgrDatabaseFile, SongQuerySqlStr, ""))
                        {
                            foreach (DataRow row in SingerGroupDT.AsEnumerable())
                            {
                                SingerGroupLowCaselist.Add(row["Config_Value"].ToString().ToLower());
                            }
                        }

                        if (SingerGroupLowCaselist.IndexOf(SongMgrCfg_SingerGroup_TextBox.Text.ToLower()) >= 0)
                        {
                            SongMgrCfg_Tooltip_Label.Text = "要加入的歌手群組已內建!";
                        }
                        else
                        {
                            if (SongMgrCfg_SingerGroup_ListBox.SelectedItems.Count > 0)
                            {
                                dt = (DataTable)SongMgrCfg_SingerGroup_ListBox.DataSource;

                                foreach (DataRow row in dt.AsEnumerable())
                                {
                                    if (row["Display"].ToString().ToLower() == SongMgrCfg_SingerGroup_TextBox.Text.ToLower())
                                    {
                                        SongMgrCfg_Tooltip_Label.Text = "要加入的歌手群組已存在!";
                                        return;
                                    }
                                }

                                dt.Rows.Add(dt.NewRow());
                                dt.Rows[dt.Rows.Count - 1][0] = SongMgrCfg_SingerGroup_TextBox.Text;
                                dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                            }
                            else
                            {
                                using (DataTable NewDT = new DataTable())
                                {
                                    NewDT.Columns.Add(new DataColumn("Display", typeof(string)));
                                    NewDT.Columns.Add(new DataColumn("Value", typeof(int)));

                                    NewDT.Rows.Add(NewDT.NewRow());
                                    NewDT.Rows[NewDT.Rows.Count - 1][0] = SongMgrCfg_SingerGroup_TextBox.Text;
                                    NewDT.Rows[NewDT.Rows.Count - 1][1] = NewDT.Rows.Count;
                                    dt = NewDT.Copy();
                                }
                                SongMgrCfg_SingerGroup_ListBox.DataSource = dt;
                                SongMgrCfg_SingerGroup_ListBox.DisplayMember = "Display";
                                SongMgrCfg_SingerGroup_ListBox.ValueMember = "Value";
                            }
                            Global.SongMgrSingerGroup += (Global.SongMgrSingerGroup == "") ? SongMgrCfg_SingerGroup_TextBox.Text : "|" + SongMgrCfg_SingerGroup_TextBox.Text;
                            SongMgrCfg_SingerGroup_TextBox.Text = "";
                            SongMgrCfg_Tooltip_Label.Text = "已成功加入歌手群組!";
                            Common_InitializeSongData(false, false, false, false, true, false);
                        }
                        SingerGroupLowCaselist.Clear();
                    }
                    else
                    {
                        if (SongMgrCfg_SingerGroup_TextBox.Text == "")
                        {
                            SongMgrCfg_Tooltip_Label.Text = "尚未輸入要加入的歌手群組!";
                        }
                        else
                        {
                            SongMgrCfg_Tooltip_Label.Text = "輸入錯誤,請以逗號區隔歌手名稱。";
                        }
                    }
                    break;
                case "移除":
                    if (SongMgrCfg_SingerGroup_ListBox.SelectedItem != null)
                    {
                        string RemoveStr = ((DataRowView)SongMgrCfg_SingerGroup_ListBox.SelectedItem)[0].ToString();
                        int index = int.Parse(SongMgrCfg_SingerGroup_ListBox.SelectedIndex.ToString());
                        dt = (DataTable)SongMgrCfg_SingerGroup_ListBox.DataSource;
                        dt.Rows.RemoveAt(index);

                        RemoveStr = Regex.Replace(RemoveStr, @"[\{\(\[｛（［【]|[】］）｝\]\)\}]", delegate (Match match)
                        {
                            string str = @"\" + match.ToString();
                            return str;
                        });
                        Global.SongMgrSingerGroup = Regex.Replace(Global.SongMgrSingerGroup, "^" + RemoveStr + @"\|?" + @"|\|?" + RemoveStr, "", RegexOptions.IgnoreCase);
                        SongMgrCfg_Tooltip_Label.Text = "已成功移除歌手群組!";
                        Common_InitializeSongData(false, false, false, false, true, false);
                    }
                    else
                    {
                        SongMgrCfg_Tooltip_Label.Text = "已無可以刪除的歌手群組!";
                    }
                    break;
            }
        }


        private void SongMgrCfg_UpdateStructure_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你確定要更新目前歌庫裡的歌手結構嗎?", "確認提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.TimerStartTime = DateTime.Now;
                Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                Common_SwitchSetUI(false);
                SongMgrCfg_Tooltip_Label.Text = "正在解析須要更新歌手結構的歌曲資料,請稍待...";

                var tasks = new List<Task>()
                {
                    Task.Factory.StartNew(() => SongMgrCfg_UpdateStructureTask()),
                };
                
                Task.Factory.ContinueWhenAll(tasks.ToArray(), SpellCorrectEndTask =>
                {
                    Global.TimerEndTime = DateTime.Now;
                    this.BeginInvoke((Action)delegate()
                    {
                        if (SongMgrCfg.UpdateStructureError)
                        {
                            SongMgrCfg_Tooltip_Label.Text = "偵測到您的歌庫結構並非由加歌程式所建立,請改用歌庫維護頁面裡的重建歌庫結構功能。";
                        }
                        else
                        {
                            SongMgrCfg_Tooltip_Label.Text = "總共成功更新 " + Global.TotalList[0] + " 首歌曲檔案,失敗 " + Global.TotalList[1] + " 首,共花費 " + (long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds + " 秒完成。";
                        }
                        Common_SwitchSetUI(true);
                    });
                });
            }
        }

        private void SongMgrCfg_UpdateStructureTask()
        {
            if (File.Exists(Global.CrazyktvDatabaseFile))
            {
                string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_SingerType, Song_Singer, Song_SongName, Song_Track, Song_SongType, Song_Volume, Song_PlayCount, Song_FileName, Song_Path from ktv_Song order by Song_Id";

                using (DataTable dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, ""))
                {
                    if (dt.Rows.Count > 0)
                    {
                        List<string> UpdateList = new List<string>();
                        List<string> HasInvalidCharList = new List<string>();
                        SongMgrCfg.UpdateStructureError = false;

                        Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
                        {
                            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                            var query = from row in dt.AsEnumerable()
                                        where row.Field<string>("Song_Lang").Equals(langstr)
                                        select row;

                            if (query.Count<DataRow>() > 0)
                            {
                                foreach (DataRow row in query)
                                {
                                    string SongId = row["Song_Id"].ToString();
                                    string SongLang = row["Song_Lang"].ToString();
                                    int SongSingerType = Convert.ToInt32(row["Song_SingerType"]);
                                    string SongSinger = row["Song_Singer"].ToString();
                                    string SongSongName = row["Song_SongName"].ToString();
                                    int SongTrack = Convert.ToInt32(row["Song_Track"]);
                                    string SongSongType = row["Song_SongType"].ToString();
                                    string SongFileName = row["Song_FileName"].ToString();
                                    string SongPath = row["Song_Path"].ToString();

                                    if (SongPath.IndexOf(SongLang) < 0)
                                    {
                                        SongMgrCfg.UpdateStructureError = true;
                                        return;
                                    }

                                    string SongSrcPath = Path.Combine(SongPath, SongFileName);
                                    string SongDestPath = CommonFunc.GetFileStructure(SongId, SongLang, SongSingerType, SongSinger, SongSongName, SongTrack, SongSongType, SongFileName, SongPath, false, "", true, true);
                                    if (SongSrcPath != SongDestPath)
                                    {
                                        bool HasInvalidChar = false;
                                        Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
                                        if (r.IsMatch(SongLang)) HasInvalidChar = true;
                                        if (r.IsMatch(SongSinger)) HasInvalidChar = true;
                                        if (r.IsMatch(SongSongName)) HasInvalidChar = true;
                                        if (r.IsMatch(SongSongType)) HasInvalidChar = true;

                                        if (HasInvalidChar)
                                        {
                                            lock (LockThis) { HasInvalidCharList.Add(SongDestPath); }
                                        }
                                        else
                                        {
                                            lock (LockThis) { UpdateList.Add(SongId + "|" + SongSrcPath + "|" + SongDestPath); }
                                        }
                                    }
                                }
                            }
                        });
                        if (SongMgrCfg.UpdateStructureError) return;

                        if (HasInvalidCharList.Count > 0)
                        {
                            foreach (string SongDestPath in HasInvalidCharList)
                            {
                                Global.TotalList[1]++;
                                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構更新】新的結構含有無效字元: " + SongDestPath + " (已忽略更新)";
                                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                            }
                        }
                        HasInvalidCharList.Clear();

                        List<string> UpdateDBList = new List<string>();
                        if (UpdateList.Count > 0)
                        {
                            foreach (string UpdateStr in UpdateList)
                            {
                                List<string> list = new List<string>(UpdateStr.Split('|'));
                                string SongPath = Path.GetDirectoryName(list[2]) + @"\";
                                string SongFileName = Path.GetFileName(list[2]);

                                if (File.Exists(list[1]))
                                {
                                    if (!Directory.Exists(SongPath)) Directory.CreateDirectory(SongPath);
                                    try
                                    {
                                        FileAttributes attributes = File.GetAttributes(list[1]);
                                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                        {
                                            attributes = CommonFunc.RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                            File.SetAttributes(list[1], attributes);
                                        }

                                        if (File.Exists(list[2])) File.Delete(list[2]);
                                        File.Move(list[1], list[2]);
                                        UpdateDBList.Add(list[0] + "|" + SongFileName + "|" + SongPath);
                                        lock (LockThis) { Global.TotalList[2]++; }
                                    }
                                    catch
                                    {
                                        lock (LockThis) { Global.TotalList[1]++; }
                                        Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構更新】檔案移動失敗: " + list[1] + " (已忽略更新)";
                                        Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                    }
                                }
                                else
                                {
                                    lock (LockThis) { Global.TotalList[1]++; }
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構更新】原始檔案不存在: " + list[1] + " (已忽略更新)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                }

                                this.BeginInvoke((Action)delegate()
                                {
                                    SongMgrCfg_Tooltip_Label.Text = "已成功將 " + Global.TotalList[2] + " 首歌曲的檔案更新,請稍待...";
                                });
                            }
                        }
                        UpdateList.Clear();

                        if (UpdateDBList.Count > 0)
                        {
                            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                            OleDbCommand cmd = new OleDbCommand();
                            string sqlColumnStr = "Song_FileName = @SongFileName, Song_Path = @SongPath";
                            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id = @SongId";
                            cmd = new OleDbCommand(SongUpdateSqlStr, conn);

                            List<string> valuelist = new List<string>();

                            foreach (string str in UpdateDBList)
                            {
                                valuelist = new List<string>(str.Split('|'));
                                cmd.Parameters.AddWithValue("@SongFileName", valuelist[1]);
                                cmd.Parameters.AddWithValue("@SongPath", valuelist[2]);
                                cmd.Parameters.AddWithValue("@SongId", valuelist[0]);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    lock (LockThis) { Global.TotalList[0]++; Global.TotalList[3]++; }
                                }
                                catch
                                {
                                    lock (LockThis) { Global.TotalList[1]++; }
                                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【歌庫結構更新】資料更新失敗: " + valuelist[0] + " (檔案已經更動)";
                                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                                }
                                cmd.Parameters.Clear();

                                this.BeginInvoke((Action)delegate()
                                {
                                    SongMgrCfg_Tooltip_Label.Text = "已成功將 " + Global.TotalList[3] + " 首歌曲的資料更新,請稍待...";
                                });
                            }
                        }
                        UpdateDBList.Clear();
                    }
                }
            }
        }


        #region --- 監視目錄 ---


        private void SongMgrCfg_MonitorFolders_SetUI()
        {
            Button[] SongMgrCfg_MonitorFolders_Button =
            {
                SongMgrCfg_MonitorFolders1_Button,
                SongMgrCfg_MonitorFolders2_Button,
                SongMgrCfg_MonitorFolders3_Button,
                SongMgrCfg_MonitorFolders4_Button,
                SongMgrCfg_MonitorFolders5_Button
            };

            TextBox[] SongMgrCfg_MonitorFolders_TextBox =
            {
                SongMgrCfg_MonitorFolders1_TextBox,
                SongMgrCfg_MonitorFolders2_TextBox,
                SongMgrCfg_MonitorFolders3_TextBox,
                SongMgrCfg_MonitorFolders4_TextBox,
                SongMgrCfg_MonitorFolders5_TextBox
            };

            for (int i = 0; i < SongMgrCfg_MonitorFolders_TextBox.Count<TextBox>(); i++)
            {
                SongMgrCfg_MonitorFolders_TextBox[i].Text = Global.SongMgrMonitorFoldersList[i];
                if (SongMgrCfg_MonitorFolders_TextBox[i].Text == "")
                {
                    SongMgrCfg_MonitorFolders_Button[i].Text = "瀏覽";
                }
                else
                {
                    SongMgrCfg_MonitorFolders_Button[i].Text = "移除";
                }
            }
        }


        private void SongMgrCfg_MonitorFolders_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Global.SongMgrEnableMonitorFolders = SongMgrCfg_MonitorFolders_CheckBox.Checked.ToString();
            if (Global.SongMgrInitializeStatus)
            {
                if (!SongMgrCfg_MonitorFolders_CheckBox.Checked)
                {
                    SongMonitor_SwitchSongMonitorWatcher();
                }
                else
                {
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrEnableMonitorFolders", Global.SongMgrEnableMonitorFolders);
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMonitorFolders", string.Join(",", Global.SongMgrMonitorFoldersList));
                    SongMonitor_CheckCurSong();
                }
            }
        }


        private void SongMgrCfg_MonitorFolders_Button_Click(object sender, EventArgs e)
        {
            TextBox[] SongMgrCfg_MonitorFolders_TextBox =
            {
                SongMgrCfg_MonitorFolders1_TextBox,
                SongMgrCfg_MonitorFolders2_TextBox,
                SongMgrCfg_MonitorFolders3_TextBox,
                SongMgrCfg_MonitorFolders4_TextBox,
                SongMgrCfg_MonitorFolders5_TextBox
            };

            int i = 0;
            switch (((Button)sender).Name)
            {
                case "SongMgrCfg_MonitorFolders1_Button":
                    i = 0;
                    break;
                case "SongMgrCfg_MonitorFolders2_Button":
                    i = 1;
                    break;
                case "SongMgrCfg_MonitorFolders3_Button":
                    i = 2;
                    break;
                case "SongMgrCfg_MonitorFolders4_Button":
                    i = 3;
                    break;
                case "SongMgrCfg_MonitorFolders5_Button":
                    i = 4;
                    break;
            }

            switch (((Button)sender).Text)
            {
                case "瀏覽":
                    FolderBrowserDialog opd = new FolderBrowserDialog();
                    if (SongMgrCfg_MonitorFolders_TextBox[i].Text != "") opd.SelectedPath = SongMgrCfg_MonitorFolders_TextBox[i].Text;

                    if (opd.ShowDialog() == DialogResult.OK && opd.SelectedPath.Length > 0)
                    {
                        bool AddMonitor = true;
                        foreach (TextBox tb in SongMgrCfg_MonitorFolders_TextBox)
                        {
                            if (tb.Text != "" && opd.SelectedPath.Contains(tb.Text))
                            {
                                AddMonitor = false;
                                SongMgrCfg_Tooltip_Label.Text = "所選擇的資料夾已在監視中!";
                                break;
                            }
                        }

                        if (AddMonitor)
                        {
                            if (SongMgrCfg_Tooltip_Label.Text == "所選擇的資料夾已在監視中!") SongMgrCfg_Tooltip_Label.Text = "";
                            SongMgrCfg_MonitorFolders_TextBox[i].Text = opd.SelectedPath;
                            ((Button)sender).Text = "移除";

                            Global.SongMgrMonitorFoldersList = new List<string>();
                            foreach (TextBox tb in SongMgrCfg_MonitorFolders_TextBox)
                            {
                                Global.SongMgrMonitorFoldersList.Add(tb.Text);
                            }
                        }
                    }
                    break;
                case "移除":
                    Global.SongMgrMonitorFoldersList[Global.SongMgrMonitorFoldersList.IndexOf(SongMgrCfg_MonitorFolders_TextBox[i].Text)] = "";
                    SongMgrCfg_MonitorFolders_TextBox[i].Text = "";
                    ((Button)sender).Text = "瀏覽";
                    break;
            }

            SongMgrCfg_MonitorFolders_CheckBox.Enabled = false;
            foreach (string MonitorFolder in Global.SongMgrMonitorFoldersList)
            {
                if (MonitorFolder != "") SongMgrCfg_MonitorFolders_CheckBox.Enabled = true;
            }
            if (!SongMgrCfg_MonitorFolders_CheckBox.Enabled) SongMgrCfg_MonitorFolders_CheckBox.Checked = false;
        }





        #endregion


    }




    class SongMgrCfg
    {
        public static DataTable GetSongAddModeList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "自動搬移來源 KTV 檔案至歌庫資料夾";
            list.Rows[0][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = "自動複製來源 KTV 檔案至歌庫資料夾";
            list.Rows[1][1] = 2;
            list.Rows.Add(list.NewRow());
            list.Rows[2][0] = "不搬移及複製來源 KTV 檔案";
            list.Rows[2][1] = 3;
            list.Rows.Add(list.NewRow());
            list.Rows[3][0] = "自動監視歌庫資料夾";
            list.Rows[3][1] = 4;
            return list;
        }

        public static DataTable GetMaxDigitCodeList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "5位數編碼";
            list.Rows[0][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = "6位數編碼";
            list.Rows[1][1] = 2;
            return list;
        }

        public static DataTable GetSongTypeList()
        {
            switch (Global.SongMgrSongType)
            {
                case "":
                    Global.SongMgrSongType = "原聲原影,演唱會,原聲,原影,翻唱,自製,模糊,無人聲,無伴唱,消音不全";
                    break;
                case "null":
                    Global.SongMgrSongType = "";
                    break;
            }

            DataTable list = new DataTable(); 
            list.Columns.Add(new DataColumn("Display", typeof(string))); 
            list.Columns.Add(new DataColumn("Value", typeof(int))); 
            if (Global.SongMgrSongType != "")
            {
                string[] str = Global.SongMgrSongType.Split(',');
                foreach (string s in str)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = s;
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
            }
            return list;
        }

        public static DataTable GetSongInfoSeparateList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "_";
            list.Rows[0][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = "-";
            list.Rows[1][1] = 2;
            return list;
        }

        public static DataTable GetCrtchorusSeparateList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "&";
            list.Rows[0][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = "+";
            list.Rows[1][1] = 2;
            return list;
        }

        public static DataTable GetFolderStructureList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> ItemList = new List<string>() { @"\語系\歌手類別\歌手", @"\語系\歌手類別", @"\語系" };

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

        public static DataTable GetFileStructureList()
        {
            using (DataTable list = new DataTable())
            {
                list.Columns.Add(new DataColumn("Display", typeof(string)));
                list.Columns.Add(new DataColumn("Value", typeof(int)));

                List<string> ItemList = null;
                switch (Global.SongMgrFolderStructure)
                {
                    case "1":
                    case "2":
                        ItemList = new List<string>() { "歌手_歌名", "歌名_歌手", "歌曲編號_歌手_歌名" };
                        break;
                    case "3":
                        ItemList = new List<string>() { "歌手類別_歌手_歌名", "歌曲編號_歌手類別_歌手_歌名" };
                        break;
                }

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

        public static DataTable GetCustomSingerTypeStructureList(int ComboBoxIndex)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            string[] str = Global.SingerTypeStructureList[ComboBoxIndex].Split(',');
            foreach (string s in str)
            {
                list.Rows.Add(list.NewRow());
                list.Rows[list.Rows.Count - 1][0] = s;
                list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
            }
            return list;
        }

        public static void SetCustomSingerTypeStructureList()
        {
            Global.SongMgrCustomSingerTypeStructureList = new List<string>();
            List<string> valuelist = new List<string>(Global.SongMgrCustomSingerTypeStructure.Split(','));

            for (int i = 0; i < Global.SingerTypeStructureList.Count; i++)
            {
                List<string> strlist = new List<string>(Global.SingerTypeStructureList[i].Split(','));
                Global.SongMgrCustomSingerTypeStructureList.Add(strlist[Convert.ToInt32(valuelist[i])-1]);
            }

            Global.SongMgrCustomSingerTypeStructureList.Add("歌星姓氏");
            Global.SongMgrCustomSingerTypeStructureList.Add("全部歌星");
            Global.SongMgrCustomSingerTypeStructureList.Add("新進");
        }

        public static DataTable GetSingerGroupList()
        {
            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add(new DataColumn("Display", typeof(string)));
                dt.Columns.Add(new DataColumn("Value", typeof(int)));

                string SongMgrSingerGroup = string.Empty;
                List<string> list = new List<string>();

                if (Global.SongMgrSingerGroup != "")
                {
                    list = new List<string>(Global.SongMgrSingerGroup.Split('|'));
                }
                
                if (list.Count > 0)
                {
                    foreach (string s in list)
                    {
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = s;
                        dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                    }
                }
                list.Clear();
                return dt;
            }
        }

        public static bool UpdateStructureError = false;
    }
}
