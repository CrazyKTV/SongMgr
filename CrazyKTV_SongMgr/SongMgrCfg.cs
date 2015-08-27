using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
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

                // 清除歌曲查詢、歌手查詢、加歌頁面的相關歌曲、歌手列表
                Common_ClearDataGridView();

                // 檢查資料庫檔案
                SongDBUpdate_CheckDatabaseFile();

                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile", Global.CrazyktvDatabaseFile);
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

                if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0 | !File.Exists(Global.CrazyktvDatabaseFile) | !Directory.Exists(Global.SongMgrDestFolder))
                {
                    if (!File.Exists(Global.CrazyktvDatabaseFile)) SongMgrCfg_Tooltip_Label.Text = "資料庫檔案不存在!";
                    else if (Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") < 0) SongMgrCfg_Tooltip_Label.Text = "資料庫檔案為舊版本!";
                    else if (Global.CrazyktvDatabaseMaxDigitCode == "Error") SongMgrCfg_Tooltip_Label.Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                    else if (!Directory.Exists(Global.SongMgrDestFolder)) SongMgrCfg_Tooltip_Label.Text = "歌庫資料夾不存在!";
                    Common_SwitchDBVerErrorUI(false);
                }
                else
                {
                    // 統計歌曲數量
                    Task.Factory.StartNew(() => Common_GetSongStatisticsTask());
                    if (SongMgrCfg_Tooltip_Label.Text == "資料庫檔案為舊版本!") SongMgrCfg_Tooltip_Label.Text = "";
                    else if (SongMgrCfg_Tooltip_Label.Text == "歌庫編碼混雜 5 及 6 位數編碼!") SongMgrCfg_Tooltip_Label.Text = "";
                    else if (SongMgrCfg_Tooltip_Label.Text == "資料庫檔案不存在!") SongMgrCfg_Tooltip_Label.Text = "";
                    else if (SongMgrCfg_Tooltip_Label.Text == "歌庫資料夾不存在!") SongMgrCfg_Tooltip_Label.Text = "";
                    Common_SwitchDBVerErrorUI(true);
                }

                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder", Global.SongMgrDestFolder);
            }
        }

        private void SongMgrCfg_SongAddMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString())
            {
                case "1":
                case "2":
                case "3":
                    Global.SongMgrSongAddMode = SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString();
                    if (SongMgrCfg_SongAddMode_ComboBox.SelectedValue.ToString() == "3") { SongMgrCfg_DestFolder_Button.Enabled = false; } else { SongMgrCfg_DestFolder_Button.Enabled = true; }
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
            int SelectedValue = int.Parse(Global.SongAddDefaultSongTrack);
            SongAdd_DefaultSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack");
            SongAdd_DefaultSongTrack_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongTrack_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongTrack_ComboBox.SelectedValue = SelectedValue;
            
            if (SongQuery_QueryType_ComboBox.SelectedValue != null)
            {
                if (SongQuery_QueryType_ComboBox.SelectedValue.ToString() == "8")
                {
                    SongQuery_QueryValue_ComboBox.DataSource = SongQuery.GetSongQueryValueList("SongTrack");
                    SongQuery_QueryValue_ComboBox.DisplayMember = "Display";
                    SongQuery_QueryValue_ComboBox.ValueMember = "Value";
                    SongQuery_QueryValue_ComboBox.SelectedValue = 1;
                }
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
            DataTable dt = new DataTable();
            switch (SongMgrCfg_SongType_Button.Text)
            {
                case "加入":
                    if (SongMgrCfg_SongType_TextBox.Text != "")
                    {
                        if (SongMgrCfg_Tooltip_Label.Text == "尚未輸入要加入的歌曲類別名稱!") SongMgrCfg_Tooltip_Label.Text = "";
                        dt = (DataTable)SongMgrCfg_SongType_ListBox.DataSource;
                        dt.Rows.Add(dt.NewRow());
                        dt.Rows[dt.Rows.Count - 1][0] = SongMgrCfg_SongType_TextBox.Text;
                        dt.Rows[dt.Rows.Count - 1][1] = dt.Rows.Count;
                        SongMgrCfg_SongType_TextBox.Text = "";
                        
                        List<string> list = new List<string>();

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
                        SongQuery_RefreshSongType();
                        SongAdd_RefreshDefaultSongType();
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
                            int index = int.Parse(SongMgrCfg_SongType_ListBox.SelectedIndex.ToString());
                            dt = (DataTable)SongMgrCfg_SongType_ListBox.DataSource;
                            dt.Rows.RemoveAt(index);

                            List<string> list = new List<string>();

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
                            SongQuery_RefreshSongType();
                            SongAdd_RefreshDefaultSongType();
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
                    ((TextBox)sender).ImeMode = ImeMode.Off;
                    break;
                case "2":
                    ((TextBox)sender).MaxLength = 6;
                    ((TextBox)sender).ImeMode = ImeMode.Off;
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
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = @"\語系\歌手類別\歌手";
            list.Rows[0][1] = 1;
            list.Rows.Add(list.NewRow());
            list.Rows[1][0] = @"\語系\歌手類別";
            list.Rows[1][1] = 2;
            return list;
        }

        public static DataTable GetFileStructureList()
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));
            
            switch(Global.SongMgrFolderStructure)
            {
                case "1":
                case "2":
                    list.Rows.Add(list.NewRow());
                    list.Rows[0][0] = "歌手_歌名";
                    list.Rows[0][1] = 1;
                    list.Rows.Add(list.NewRow());
                    list.Rows[1][0] = "歌名_歌手";
                    list.Rows[1][1] = 2;
                    list.Rows.Add(list.NewRow());
                    list.Rows[2][0] = "歌曲編號_歌手_歌名";
                    list.Rows[2][1] = 3;
                    break;
            }
            return list;
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


    }
}
