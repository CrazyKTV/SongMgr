using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CrazyKTV_SongMgr
{
    public static class ControlExtentions
    {
        public static void MakeDoubleBuffered(this Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }
    }

    public partial class MainForm : Form
    {
        private static object LockThis = new object();

        private void Common_NumericOnly_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 13 & (int)e.KeyChar != 27)
            {
                e.Handled = true;
                switch (((TextBox)sender).Name)
                {
                    case "SongAdd_DefaultSongVolume_TextBox":
                        SongAdd_Tooltip_Label.Text = "此項目只能輸入數字!";
                        break;
                    case "SongMaintenance_VolumeChange_TextBox":
                        SongMaintenance_Tooltip_Label.Text = "此項目只能輸入數字!";
                        break;
                    default:
                        SongMgrCfg_Tooltip_Label.Text = "此項目只能輸入數字!";
                        break;
                }
            }
            else
            {
                switch (((TextBox)sender).Name)
                {
                    case "SongAdd_DefaultSongVolume_TextBox":
                        if (SongAdd_Tooltip_Label.Text == "此項目只能輸入數字!") SongAdd_Tooltip_Label.Text = "";
                        break;
                    case "SongMaintenance_VolumeChange_TextBox":
                        if (SongMaintenance_Tooltip_Label.Text == "此項目只能輸入數字!") SongMaintenance_Tooltip_Label.Text = "";
                        break;
                    default:
                        if (SongMgrCfg_Tooltip_Label.Text == "此項目只能輸入數字!") SongMgrCfg_Tooltip_Label.Text = "";
                        break;
                }
            }
            if ((int)e.KeyChar == 13)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void Common_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void Common_IsNullOrEmpty_TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                SongMaintenance_Tooltip_Label.Text = "此項目的值不能為空白!";
                e.Cancel = true;
            }
            else
            {
                if (SongMaintenance_Tooltip_Label.Text == "此項目的值不能為空白!") SongMaintenance_Tooltip_Label.Text = "";
                Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
                if (r.IsMatch(((TextBox)sender).Text))
                {
                    e.Cancel = true;
                    SongMaintenance_Tooltip_Label.Text = "此項目的值含有非法字元!";
                }
                else
                {
                    if (SongMaintenance_Tooltip_Label.Text == "此項目的值含有非法字元!") SongMaintenance_Tooltip_Label.Text = "";
                }
            }
        }

        private void Common_HasInvalidChar_TextBox_Validating(object sender, CancelEventArgs e)
        {
            Regex r = new Regex(@"[\\/:*?<>|" + '"' + "]");
            if (r.IsMatch(((TextBox)sender).Text))
            {
                e.Cancel = true;
                switch (((TextBox)sender).Name)
                {
                    case "SongAdd_SpecialStr_TextBox":
                        SongAdd_Tooltip_Label.Text = "此項目的值含有非法字元!";
                        break;
                    case "SingerMgr_SingerAddName_TextBox":
                        SingerMgr_Tooltip_Label.Text = "此項目的值含有非法字元!";
                        break;
                    case "SongMgrCfg_SongType_TextBox":
                        SongMgrCfg_Tooltip_Label.Text = "此項目的值含有非法字元!";
                        break;
                }
            }
            else
            {
                switch (((TextBox)sender).Name)
                {
                    case "SongAdd_SpecialStr_TextBox":
                        if (SongAdd_Tooltip_Label.Text == "此項目的值含有非法字元!") SongAdd_Tooltip_Label.Text = "";
                        break;
                    case "SingerMgr_SingerAddName_TextBox":
                        if (SingerMgr_Tooltip_Label.Text == "此項目的值含有非法字元!") SingerMgr_Tooltip_Label.Text = "";
                        break;
                    case "SongMgrCfg_SongType_TextBox":
                        if (SongMgrCfg_Tooltip_Label.Text == "此項目的值含有非法字元!") SongMgrCfg_Tooltip_Label.Text = "";
                        break;
                }
            }
        }
        
        private void Common_ListBox_DoubleClick(object sender, EventArgs e)
        {
            string str = "";
            ListBox lbox = ((ListBox)sender);

            if (lbox.Items.Count > 0)
            {
                DataTable dt = (DataTable)lbox.DataSource;
                foreach (DataRow row in dt.Rows)
                {
                    str += row["Display"] + Environment.NewLine;
                }
                
                try
                {
                    Clipboard.SetData(DataFormats.UnicodeText, str);
                }
                catch
                {
                    // 剪貼簿被別的程式占用
                    Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【複製到剪貼簿】無法完成複製到剪貼簿,因剪貼簿已被占用。";
                    Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                }
            }
        }

        private void Common_SwitchDBVerErrorUI(bool status)
        {
            SongQuery_Query_GroupBox.Enabled = status;
            SongQuery_OtherQuery_GroupBox.Enabled = status;
            SongQuery_Statistics_GroupBox.Enabled = status;
            SongQuery_EditMode_CheckBox.Enabled = status;
            SongAdd_DefaultSongInfo_GroupBox.Enabled = status;
            SongAdd_SpecialStr_GroupBox.Enabled = status;
            SongAdd_SongAddCfg_GroupBox.Enabled = status;
            SongAdd_Save_Button.Enabled = status;
            SongAdd_DataGridView.Enabled = status;
            SongAdd_DragDrop_Label.Enabled = status;
            SongMaintenance_SpellCorrect_GroupBox.Enabled = status;
            SongMaintenance_CodeConv_GroupBox.Enabled = status;
            SongMaintenance_TrackExchange_GroupBox.Enabled = status;
            SongMaintenance_VolumeChange_GroupBox.Enabled = status;
            SongMaintenance_TabControl.Enabled = status;
            SongMaintenance_PlayCount_GroupBox.Enabled = status;
            SongMaintenance_SongPathChange_GroupBox.Enabled = status;
            SongMaintenance_Save_Button.Enabled = status;
            SingerMgr_Query_GroupBox.Enabled = status;
            SingerMgr_Statistics_GroupBox.Enabled = status;
            SingerMgr_SingerAdd_GroupBox.Enabled = status;
            SingerMgr_Manager_GroupBox.Enabled = status;
            SingerMgr_DataGridView.Enabled = status;
        }

        private void Common_SwitchSetUI(bool status)
        {
            if (!status)
            {
                SongLog_ListBox.DataSource = null;
                SongLog_ListBox.Items.Clear();

                SongAddResult_DuplicateSong_ListBox.DataSource = null;
                SongAddResult_DuplicateSong_ListBox.Items.Clear();

                SongAddResult_FailureSong_ListBox.DataSource = null;
                SongAddResult_FailureSong_ListBox.Items.Clear();
            }

            SongAdd_DefaultSongInfo_GroupBox.Enabled = status;
            SongAdd_SpecialStr_GroupBox.Enabled = status;
            SongAdd_SongAddCfg_GroupBox.Enabled = status;
            SongAdd_Save_Button.Enabled = status;
            SongMgrCfg_General_GroupBox.Enabled = status;
            SongMgrCfg_SongID_GroupBox.Enabled = status;
            SongMgrCfg_TabControl.Enabled = status;
            SongMgrCfg_Save_Button.Enabled = status;
            SongMaintenance_SpellCorrect_GroupBox.Enabled = status;
            SongMaintenance_CodeConv_GroupBox.Enabled = status;
            SongMaintenance_TrackExchange_GroupBox.Enabled = status;
            SongMaintenance_VolumeChange_GroupBox.Enabled = status;
            SongMaintenance_TabControl.Enabled = status;
            SongMaintenance_PlayCount_GroupBox.Enabled = status;
            SongMaintenance_SongPathChange_GroupBox.Enabled = status;
            SongMaintenance_Save_Button.Enabled = status;
            SingerMgr_Query_GroupBox.Enabled = status;
            SingerMgr_Statistics_GroupBox.Enabled = status;
            SingerMgr_SingerAdd_GroupBox.Enabled = status;
            SingerMgr_Manager_GroupBox.Enabled = status;
            SingerMgr_DataGridView.Enabled = status;

            if (Global.SongLogDT.Rows.Count > 0)
            {
                if (status)
                {
                    SongLog_ListBox.DataSource = Global.SongLogDT;
                    SongLog_ListBox.DisplayMember = "Display";
                    SongLog_ListBox.ValueMember = "Value";

                    if (MainTabControl.TabPages.IndexOf(SongLog_TabPage) < 0)
                    {
                        MainCfg_HideSongLogTab_CheckBox.Checked = false;
                    }
                }
                SongLog_TabPage.Text = "操作記錄 (" + Global.SongLogDT.Rows.Count + ")";
            }
            else
            {
                SongLog_TabPage.Text = "操作記錄";
            }

            if (Global.DuplicateSongDT.Rows.Count > 0)
            {
                if (status)
                {
                    SongAddResult_DuplicateSong_ListBox.DataSource = Global.DuplicateSongDT;
                    SongAddResult_DuplicateSong_ListBox.DisplayMember = "Display";
                    SongAddResult_DuplicateSong_ListBox.ValueMember = "Value";
                }
            }

            if (Global.FailureSongDT.Rows.Count > 0)
            {
                if (status)
                {
                    SongAddResult_FailureSong_ListBox.DataSource = Global.FailureSongDT;
                    SongAddResult_FailureSong_ListBox.DisplayMember = "Display";
                    SongAddResult_FailureSong_ListBox.ValueMember = "Value";
                }
            }

            if (Global.DuplicateSongDT.Rows.Count > 0 | Global.FailureSongDT.Rows.Count > 0)
            {
                if (status)
                {
                    if (MainTabControl.TabPages.IndexOf(SongAddResult_TabPage) < 0)
                    {
                        MainCfg_HideSongAddResultTab_CheckBox.Checked = false;
                    }
                }
            }
        }


        private void Common_InitializeSongData()
        {
            if (Global.CrazyktvDatabaseStatus)
            {
                Global.PhoneticsWordList = new List<string>();
                Global.PhoneticsSpellList = new List<string>();
                Global.PhoneticsStrokesList = new List<string>();
                Global.PhoneticsPenStyleList = new List<string>();

                Global.PhoneticsDT = new DataTable();
                string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
                Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongPhoneticsQuerySqlStr, "");

                var query = from row in Global.PhoneticsDT.AsEnumerable()
                            where row.Field<Int16>("SortIdx") < 2
                            select row;

                foreach (DataRow row in query)
                {
                    Global.PhoneticsWordList.Add(row["Word"].ToString());
                    Global.PhoneticsSpellList.Add((row["Spell"].ToString()).Substring(0, 1));
                    Global.PhoneticsStrokesList.Add(row["Strokes"].ToString());
                    Global.PhoneticsPenStyleList.Add((row["PenStyle"].ToString()).Substring(0, 1));
                }
                Global.PhoneticsDT.Dispose();
                Global.PhoneticsDT = null;
            }
        }


        private void Common_GetSongStatisticsTask()
        {
            if (File.Exists(Global.CrazyktvDatabaseFile) & Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
            {
                Global.SongStatisticsDT = new DataTable();
                string SongQuerySqlStr = "select Song_Id, Song_Lang, Song_Path, Song_FileName from ktv_Song";
                Global.SongStatisticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                List<int> SongLangCount = new List<int>();
                List<int> SongFileCount = new List<int>();

                Label[] SongQuery_Statistics_Label =
                {
                    SongQuery_Statistics2_Label,
                    SongQuery_Statistics3_Label,
                    SongQuery_Statistics4_Label,
                    SongQuery_Statistics5_Label,
                    SongQuery_Statistics6_Label,
                    SongQuery_Statistics7_Label,
                    SongQuery_Statistics8_Label,
                    SongQuery_Statistics9_Label,
                    SongQuery_Statistics10_Label,
                    SongQuery_Statistics11_Label
                };


                this.BeginInvoke((Action)delegate()
                {
                    for (int i = 0; i < SongQuery_Statistics_Label.Count<Label>(); i++)
                    {
                        SongQuery_Statistics_Label[i].Text = Global.CrazyktvSongLangList[i] + ":";
                    }
                });

                Label[] SongQuery_StatisticsValue_Label =
                {
                    SongQuery_Statistics2Value_Label,
                    SongQuery_Statistics3Value_Label,
                    SongQuery_Statistics4Value_Label,
                    SongQuery_Statistics5Value_Label,
                    SongQuery_Statistics6Value_Label,
                    SongQuery_Statistics7Value_Label,
                    SongQuery_Statistics8Value_Label,
                    SongQuery_Statistics9Value_Label,
                    SongQuery_Statistics10Value_Label,
                    SongQuery_Statistics11Value_Label,
                    SongQuery_Statistics1Value_Label,
                    SongQuery_Statistics12Value_Label
                };

                TextBox[] SongMaintenance_Lang_TextBox =
                {
                    SongMaintenance_Lang1_TextBox,
                    SongMaintenance_Lang2_TextBox,
                    SongMaintenance_Lang3_TextBox,
                    SongMaintenance_Lang4_TextBox,
                    SongMaintenance_Lang5_TextBox,
                    SongMaintenance_Lang6_TextBox,
                    SongMaintenance_Lang7_TextBox,
                    SongMaintenance_Lang8_TextBox,
                    SongMaintenance_Lang9_TextBox,
                    SongMaintenance_Lang10_TextBox
                };

                this.BeginInvoke((Action)delegate()
                {
                    SongQuery_StatisticsValue_Label[11].Text = "統計中...";
                });

                var tasks = new List<Task<List<int>>>();
                tasks.Add(Task<List<int>>.Factory.StartNew(CommonFunc.GetSongLangCount));
                tasks.Add(Task<List<int>>.Factory.StartNew(CommonFunc.GetSongFileCount));

                SongLangCount = tasks[0].Result;
                this.BeginInvoke((Action)delegate()
                {
                    for (int i = 0; i < SongLangCount.Count; i++)
                    {
                        SongQuery_StatisticsValue_Label[i].Text = SongLangCount[i].ToString() + " 首";
                        if (i < 10)
                        {
                            if (SongLangCount[i] > 0) SongMaintenance_Lang_TextBox[i].Enabled = false;
                        }
                    }
                });

                SongFileCount = tasks[1].Result;
                this.BeginInvoke((Action)delegate()
                {
                    SongQuery_StatisticsValue_Label[11].Text = SongFileCount[10].ToString() + " 個";
                });

                Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                {
                    Global.SongStatisticsDT.Dispose();
                    Global.SongStatisticsDT = null;
                });
            }
        }

        private void Common_CheckSongLang()
        {
            if (File.Exists(Global.CrazyktvDatabaseFile) & Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
            {
                bool UpdateLang = false;

                List<string> LangNamelist = new List<string>();
                List<string> LangKeyWordlist = new List<string>();

                DataTable dt = new DataTable();
                string SongQuerySqlStr = "select Langauage_Id, Langauage_Name, Langauage_KeyWord from ktv_Langauage";
                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        int i = Convert.ToInt32(row["Langauage_Id"]);
                        if (i < 10)
                        {
                            if (row["Langauage_Name"].ToString() != Global.CrazyktvSongLangList[i]) UpdateLang = true;
                            LangNamelist.Add(row["Langauage_Name"].ToString());

                            if (row["Langauage_KeyWord"].ToString() == "") 
                            {
                                UpdateLang = true;
                                LangKeyWordlist.Add(Global.CrazyktvSongLangKeyWordList[i]);
                            }
                            else
                            {
                                if (row["Langauage_KeyWord"].ToString() != Global.CrazyktvSongLangKeyWordList[i]) UpdateLang = true;
                                LangKeyWordlist.Add(row["Langauage_KeyWord"].ToString());
                            }
                        }
                    }

                    if (UpdateLang)
                    {
                        Global.CrazyktvSongLangList = LangNamelist;
                        Global.CrazyktvSongLangKeyWordList = LangKeyWordlist;
                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };

                        CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangStr", string.Join(",", Global.CrazyktvSongLangList));
                        CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord", string.Join("|", Global.CrazyktvSongLangKeyWordList));

                        var tasks = new List<Task>();
                        tasks.Add(Task.Factory.StartNew(() => SongMaintenance_SongLangUpdateTask()));

                        Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                        {
                            this.BeginInvoke((Action)delegate()
                            {
                                Common_RefreshSongLang();
                            });
                        });
                    }
                }
                dt.Dispose();
                dt = null;
            }
        }

        private void Common_RefreshSongLang()
        {
            SongQuery_QueryFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList();
            SongQuery_QueryFilter_ComboBox.DisplayMember = "Display";
            SongQuery_QueryFilter_ComboBox.ValueMember = "Value";
            SongQuery_QueryFilter_ComboBox.SelectedValue = 1;

            SongAdd_DefaultSongLang_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongLang");
            SongAdd_DefaultSongLang_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongLang_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongLang_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongLang);

            SongMgrCfg_SetLangLB();
            SongMaintenance_SetCustomLangControl();
        }

        private void Common_GetSingerStatisticsTask()
        {
            if (File.Exists(Global.CrazyktvDatabaseFile) & Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
            {
                SingerMgr.CreateSongDataTable();
                List<int> SingerTypeCount = new List<int>();
                List<string> SingerTypeList = new List<string>();

                foreach (string str in Global.CrazyktvSingerTypeList)
                {
                    if (str != "未使用")
                    {
                        SingerTypeList.Add(str);
                    }
                }

                Label[] SingerMgr_Statistics_Label =
                {
                    SingerMgr_Statistics2_Label,
                    SingerMgr_Statistics3_Label,
                    SingerMgr_Statistics4_Label,
                    SingerMgr_Statistics5_Label,
                    SingerMgr_Statistics6_Label,
                    SingerMgr_Statistics7_Label,
                    SingerMgr_Statistics8_Label,
                    SingerMgr_Statistics9_Label,
                    SingerMgr_Statistics10_Label
                };

                this.BeginInvoke((Action)delegate()
                {
                    for (int i = 0; i < SingerMgr_Statistics_Label.Count<Label>(); i++)
                    {
                        SingerMgr_Statistics_Label[i].Text = SingerTypeList[i] + ":";
                    }
                });

                Label[] SingerMgr_StatisticsValue_Label =
                {
                    SingerMgr_Statistics2Value_Label,
                    SingerMgr_Statistics3Value_Label,
                    SingerMgr_Statistics4Value_Label,
                    SingerMgr_Statistics5Value_Label,
                    SingerMgr_Statistics6Value_Label,
                    SingerMgr_Statistics7Value_Label,
                    SingerMgr_Statistics8Value_Label,
                    SingerMgr_Statistics9Value_Label,
                    SingerMgr_Statistics10Value_Label,
                    SingerMgr_Statistics1Value_Label
                };

                var task = Task<List<int>>.Factory.StartNew(CommonFunc.GetSingerTypeCount);

                SingerTypeCount = task.Result;
                this.BeginInvoke((Action)delegate()
                {
                    for (int i = 0; i < SingerTypeCount.Count; i++)
                    {
                        SingerMgr_StatisticsValue_Label[i].Text = SingerTypeCount[i].ToString() + " 位";
                    }
                });
                SingerMgr.DisposeSongDataTable();
            }
        }

        private void Common_RebuildSingerDataTask(string TooltipName)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            OleDbConnection conn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();

            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            string TruncateSqlStr = "delete * from ktv_Singer";
            cmd = new OleDbCommand(TruncateSqlStr, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            int MaxSingerId = CommonFunc.GetMaxSingerId("ktv_Singer", Global.CrazyktvDatabaseFile) + 1;
            List<string> NotExistsSingerId = new List<string>();
            NotExistsSingerId = CommonFunc.GetNotExistsSingerId("ktv_Singer", Global.CrazyktvDatabaseFile);

            DataTable dt = new DataTable();
            string SingerQuerySqlStr = "SELECT First(Song_Singer) AS Song_Singer, First(Song_SingerType) AS Song_SingerType, Count(Song_Singer) AS Song_SingerCount FROM ktv_Song GROUP BY Song_Singer HAVING (((First(Song_SingerType))<>10) AND ((Count(Song_Singer))>0)) ORDER BY First(Song_SingerType), First(Song_Singer)";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SingerQuerySqlStr, "");

            if (dt.Rows.Count > 0)
            {
                string SingerId = "";
                string SingerName = "";
                string SingerType = "";

                List<string> list = new List<string>();
                List<string> Addlist = new List<string>();
                List<string> spelllist = new List<string>();
                List<string> ChorusSingerList = new List<string>();
                List<string> SpecialStrlist = new List<string>(Regex.Split(Global.SongAddSpecialStr, ",", RegexOptions.IgnoreCase));

                foreach (DataRow row in dt.AsEnumerable())
                {
                    SingerName = row["Song_Singer"].ToString();
                    SingerType = row["Song_SingerType"].ToString();

                    if (SingerType == "3")
                    {
                        // 處理合唱歌曲中的特殊歌手名稱
                        foreach (string SpecialSingerName in SpecialStrlist)
                        {
                            Regex SpecialStrRegex = new Regex(SpecialSingerName, RegexOptions.IgnoreCase);
                            if (SpecialStrRegex.IsMatch(SingerName))
                            {
                                if (ChorusSingerList.IndexOf(SpecialSingerName) < 0)
                                {
                                    ChorusSingerList.Add(SpecialSingerName);
                                }
                                SingerName = Regex.Replace(SingerName, "&" + SpecialSingerName + "|" + SpecialSingerName + "&", "");
                            }
                        }

                        Regex r = new Regex("[&+](?=(?:[^%]*%%[^%]*%%)*(?![^%]*%%))");
                        if (r.IsMatch(SingerName))
                        {
                            string[] singers = Regex.Split(SingerName, "&", RegexOptions.None);
                            foreach (string str in singers)
                            {
                                if (ChorusSingerList.IndexOf(str) < 0)
                                {
                                    ChorusSingerList.Add(str);
                                }
                            }
                        }
                        else
                        {
                            if (ChorusSingerList.IndexOf(SingerName) < 0)
                            {
                                ChorusSingerList.Add(SingerName);
                            }
                        }
                    }
                    else
                    {
                        if (Global.AllSingerLowCaseList.IndexOf(SingerName.ToLower()) >= 0)
                        {
                            SingerName = Global.AllSingerList[Global.AllSingerLowCaseList.IndexOf(SingerName.ToLower())];
                            SingerType = Global.AllSingerTypeList[Global.AllSingerLowCaseList.IndexOf(SingerName.ToLower())];
                        }

                        if (NotExistsSingerId.Count > 0)
                        {
                            SingerId = NotExistsSingerId[0];
                            NotExistsSingerId.RemoveAt(0);
                        }
                        else
                        {
                            SingerId = MaxSingerId.ToString();
                            MaxSingerId++;
                        }

                        spelllist = new List<string>();
                        spelllist = CommonFunc.GetSongNameSpell(SingerName);
                        Addlist.Add("ktv_Singer" + "|" + SingerId + "|" + SingerName + "|" + SingerType + "|" + spelllist[0] + "|" + spelllist[2] + "|" + spelllist[1] + "|" + spelllist[3]);
                    }

                    this.BeginInvoke((Action)delegate()
                    {
                        switch (TooltipName)
                        {
                            case "SongMaintenance":
                                SongMaintenance_Tooltip_Label.Text = "正在解析第 " + SingerId + " 位歌手資料,請稍待...";
                                break;
                            case "SingerMgr":
                                SingerMgr_Tooltip_Label.Text = "正在解析第 " + SingerId + " 位歌手資料,請稍待...";
                                break;
                        }
                    });
                }
                dt.Dispose();
                dt = null;

                string sqlColumnStr = "Singer_Id, Singer_Name, Singer_Type, Singer_Spell, Singer_Strokes, Singer_SpellNum, Singer_PenStyle";
                string sqlValuesStr = "@SingerId, @SingerName, @SingerType, @SingerSpell, @SingerStrokes, @SingerSpellNum, @SingerPenStyle";
                string SingerAddSqlStr = "insert into ktv_Singer ( " + sqlColumnStr + " ) values ( " + sqlValuesStr + " )";

                conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
                cmd = new OleDbCommand(SingerAddSqlStr, conn);

                foreach (string AddStr in Addlist)
                {
                    list = new List<string>(AddStr.Split('|'));

                    switch (list[0])
                    {
                        case "ktv_Singer":
                            cmd.Parameters.AddWithValue("@SingerId", list[1]);
                            cmd.Parameters.AddWithValue("@SingerName", list[2]);
                            cmd.Parameters.AddWithValue("@SingerType", list[3]);
                            cmd.Parameters.AddWithValue("@SingerSpell", list[4]);
                            cmd.Parameters.AddWithValue("@SingerStrokes", list[5]);
                            cmd.Parameters.AddWithValue("@SingerSpellNum", list[6]);
                            cmd.Parameters.AddWithValue("@SingerPenStyle", list[7]);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            lock (LockThis)
                            {
                                Global.TotalList[0]++;
                            }
                            break;
                    }
                    this.BeginInvoke((Action)delegate()
                    {
                        switch (TooltipName)
                        {
                            case "SongMaintenance":
                                SongMaintenance_Tooltip_Label.Text = "正在重建第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                                break;
                            case "SingerMgr":
                                SingerMgr_Tooltip_Label.Text = "正在重建第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                                break;
                        }
                    });
                }
                Addlist.Clear();

                Global.SingerList = new List<string>();
                Global.SingerLowCaseList = new List<string>();
                Global.SingerTypeList = new List<string>();

                Global.SingerDT = new DataTable();
                string SongSingerQuerySqlStr = "select Singer_Name, Singer_Type from ktv_Singer";
                Global.SingerDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongSingerQuerySqlStr, "");

                foreach (DataRow row in Global.SingerDT.AsEnumerable())
                {
                    Global.SingerList.Add(row["Singer_Name"].ToString());
                    Global.SingerLowCaseList.Add(row["Singer_Name"].ToString().ToLower());
                    Global.SingerTypeList.Add(row["Singer_Type"].ToString());
                }

                Addlist = new List<string>();
                // 判斷是否要加入合唱歌手資料至歌庫歌手資料庫
                foreach (string ChorusSinger in ChorusSingerList)
                {
                    string ChorusSingerName = Regex.Replace(ChorusSinger, @"^\s*|\s*$", ""); //去除頭尾空白
                    // 查找資料庫歌庫歌手資料表
                    if (Global.SingerLowCaseList.IndexOf(ChorusSingerName.ToLower()) < 0)
                    {
                        // 查找資料庫預設歌手資料表
                        if(Global.AllSingerLowCaseList.IndexOf(ChorusSingerName.ToLower()) >= 0)
                        {
                            SingerName = Global.AllSingerList[Global.AllSingerLowCaseList.IndexOf(ChorusSingerName.ToLower())];
                            SingerType = Global.AllSingerTypeList[Global.AllSingerLowCaseList.IndexOf(ChorusSingerName.ToLower())];

                            if (NotExistsSingerId.Count > 0)
                            {
                                SingerId = NotExistsSingerId[0];
                                NotExistsSingerId.RemoveAt(0);
                            }
                            else
                            {
                                SingerId = MaxSingerId.ToString();
                                MaxSingerId++;
                            }

                            spelllist = new List<string>();
                            spelllist = CommonFunc.GetSongNameSpell(SingerName);
                            Addlist.Add("ktv_Singer" + "|" + SingerId + "|" + SingerName + "|" + SingerType + "|" + spelllist[0] + "|" + spelllist[2] + "|" + spelllist[1] + "|" + spelllist[3]);
                        }
                    }
                    
                    this.BeginInvoke((Action)delegate()
                    {
                        switch (TooltipName)
                        {
                            case "SongMaintenance":
                                SongMaintenance_Tooltip_Label.Text = "正在從第 " + ChorusSingerList.IndexOf(ChorusSinger) + " 組合唱歌手解析歌手資料,請稍待...";
                                break;
                            case "SingerMgr":
                                SingerMgr_Tooltip_Label.Text = "正在從第 " + ChorusSingerList.IndexOf(ChorusSinger) + " 組合唱歌手解析歌手資料,請稍待...";
                                break;
                        }
                    });
                }

                foreach (string AddStr in Addlist)
                {
                    list = new List<string>(AddStr.Split('|'));

                    switch (list[0])
                    {
                        case "ktv_Singer":
                            cmd.Parameters.AddWithValue("@SingerId", list[1]);
                            cmd.Parameters.AddWithValue("@SingerName", list[2]);
                            cmd.Parameters.AddWithValue("@SingerType", list[3]);
                            cmd.Parameters.AddWithValue("@SingerSpell", list[4]);
                            cmd.Parameters.AddWithValue("@SingerStrokes", list[5]);
                            cmd.Parameters.AddWithValue("@SingerSpellNum", list[6]);
                            cmd.Parameters.AddWithValue("@SingerPenStyle", list[7]);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            lock (LockThis)
                            {
                                Global.TotalList[0]++;
                            }
                            break;
                    }
                    this.BeginInvoke((Action)delegate()
                    {
                        switch (TooltipName)
                        {
                            case "SongMaintenance":
                                SongMaintenance_Tooltip_Label.Text = "正在從合唱歌手重建第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                                break;
                            case "SingerMgr":
                                SingerMgr_Tooltip_Label.Text = "正在從合唱歌手重建第 " + Global.TotalList[0] + " 位歌手資料,請稍待...";
                                break;
                        }
                    });
                }
                Addlist.Clear();
                ChorusSingerList.Clear();
                conn.Close();
            }
        }

        private void Common_CheckBackupRemoveSongTask()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            if (Global.SongMgrBackupRemoveSong == "True")
            {
                if (Directory.Exists(Application.StartupPath + @"\SongMgr\RemoveSong"))
                {
                    List<string> RemoveFileList = new List<string>();
                    List<string> SupportFormat = new List<string>();
                    SupportFormat = new List<string>(Global.SongMgrSupportFormat.Split(';'));
                    

                    DirectoryInfo dir = new DirectoryInfo(Application.StartupPath + @"\SongMgr\RemoveSong");
                    FileInfo[] Files = dir.GetFiles("*", SearchOption.AllDirectories).Where(p => SupportFormat.Contains(p.Extension.ToLower())).ToArray();

                    foreach (FileInfo fi in Files)
                    {
                        if ((int)(DateTime.Now - fi.CreationTime).TotalDays > Convert.ToInt32(Global.MainCfgBackupRemoveSongDays))
                        {
                            if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                fi.Attributes = fi.Attributes & ~FileAttributes.ReadOnly;
                            }
                            RemoveFileList.Add(fi.FullName);
                        }
                    }

                    foreach (string FilePath in RemoveFileList)
                    {
                        try
                        {
                            File.Delete(FilePath);
                        }
                        catch
                        {
                            Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【刪除備份移除歌曲】無法完成刪除,因檔案已被占用。";
                            Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
                        }
                    }
                }
            }
        }

        private void Common_ClearDataGridView()
        {
            Global.SongQueryQueryType = "SongQuery";
            SongQuery_EditMode_CheckBox.Checked = false;
            SongQuery_EditMode_CheckBox.Enabled = true;
            SongQuery_DataGridView.DataSource = null;
            if (SongQuery_DataGridView.Columns.Count > 0) SongQuery_DataGridView.Columns.Remove("Song_FullPath");
            SongQuery_QueryStatus_Label.Text = "";

            SongAdd_DataGridView.DataSource = null;
            if (Global.SongMgrSongAddMode != "4") SongAdd_DragDrop_Label.Visible = true;
            SongAdd_DataGridView.AllowDrop = true;
            SongAdd_Add_Button.Text = "加入歌庫";
            SongAdd_Add_Button.Enabled = false;
            SongAdd_Save_Button.Text = "儲存設定";
            SongAdd_Tooltip_Label.Text = "";

            SingerMgr_DataGridView.DataSource = null;
            SingerMgr_Tooltip_Label.Text = "";
            GC.Collect();
        }

        private void Common_CheckSongMgrVer()
        {
            string WebUpdaterFile = Application.StartupPath + @"\CrazyKTV_WebUpdater.exe";
            if (Global.MainCfgEnableAutoUpdate == "True" && File.Exists(WebUpdaterFile))
            {
                string WebUpdaterTempFile = Application.StartupPath + @"\CrazyKTV_WebUpdater.tmp";
                string WebUpdaterUrl = "https://raw.githubusercontent.com/KenLuoTW/CrazyKTVSongMgr/master/CrazyKTV_WebUpdater/UpdateFile/CrazyKTV_WebUpdater.ver";

                if (CommonFunc.DownloadFile(WebUpdaterTempFile, WebUpdaterUrl))
                {
                    if (File.Exists(WebUpdaterTempFile))
                    {
                        List<string> RemoteSongMgrVerList = CommonFunc.LoadVersionXmlFile(WebUpdaterTempFile, "CrazyKTV_SongMgr.exe");
                        File.Delete(WebUpdaterTempFile);
                        if (RemoteSongMgrVerList.Count > 0)
                        {
                            if (Convert.ToInt32(RemoteSongMgrVerList[1]) > Convert.ToInt32(Global.SongMgrVer))
                            {
                                if (MessageBox.Show("你確定要更新 CrazyKTV 加歌程式嗎?", "偵測到 CrazyKTV 加歌程式更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    Process p = Process.Start(WebUpdaterFile);
                                    Environment.Exit(0);
                                }
                            }
                        }
                    }
                }
                File.Delete(WebUpdaterTempFile);
            }
        }

        private void Common_RefreshSongMgr()
        {
            Global.CrazyktvDatabaseStatus = false;
            Global.CrazyktvDatabaseIsOld = false;
            Global.CrazyktvDatabaseMaxDigitCode = true;

            // 清除歌曲查詢、歌手查詢、加歌頁面的相關歌曲、歌手列表
            Common_ClearDataGridView();

            // 檢查資料庫檔案是否為舊版資料庫
            SongDBUpdate_CheckDatabaseFile();

            // 初始化所需資料
            Common_InitializeSongData();

            // 歌庫監視
            SongMonitor_CheckCurSong();

            MainTabControl_SelectedIndexChanged(new TabControl(), new EventArgs());
        }

    }


    class CommonFunc
    {
        private static object LockThis = new object();
        public static void CreateConfigXmlFile(string ConfigFile)
        {
            XDocument xmldoc = new XDocument
                (
                    new XDeclaration("1.0", "utf-16", null),
                    new XElement("Configeruation")
                );
            xmldoc.Save(ConfigFile);
        }

        public static string LoadConfigXmlFile(string ConfigFile, string ConfigName)
        {
            string Value = "";
            try
            {
                XElement rootElement = XElement.Load(ConfigFile);
                var Query = from childNode in rootElement.Elements("setting")
                            where (string)childNode.Attribute("Name") == ConfigName
                            select childNode;

                foreach (XElement childNode in Query)
                {
                    Value = childNode.Value;
                }
            }
            catch
            {
                Path.GetFileName(ConfigFile);
                MessageBox.Show("【" + Path.GetFileName(ConfigFile) + "】設定檔內容有錯誤,請刪除後再執行。");
            }
            return Value;
        }

        public static void SaveConfigXmlFile(string ConfigFile, string ConfigName, string ConfigValue)
        {
            XDocument xmldoc = XDocument.Load(ConfigFile);
            XElement rootElement = xmldoc.XPathSelectElement("Configeruation");
            
            var Query =from childNode in rootElement.Elements("setting")
                       where (string)childNode.Attribute("Name") == ConfigName
                       select childNode;

            if (Query.ToList().Count > 0)
            {
                foreach (XElement childNode in Query)
                {
                    childNode.Element("Value").Value = ConfigValue;
                }
            }
            else
            {
                XElement AddNode = new XElement("setting", new XAttribute("Name", ConfigName), new XElement("Value",  ConfigValue));
                rootElement.Add(AddNode);
            }
            xmldoc.Save(ConfigFile);
        }

        public static void RemoveConfigXmlFile(string ConfigFile, string ConfigName)
        {
            XDocument xmldoc = XDocument.Load(ConfigFile);
            XElement rootElement = xmldoc.XPathSelectElement("Configeruation");

            var Query = from childNode in rootElement.Elements("setting")
                        where (string)childNode.Attribute("Name") == ConfigName
                        select childNode;

            if (Query.ToList().Count > 0)
            {
                foreach (XElement childNode in Query)
                {
                    childNode.Remove();
                }
            }
            xmldoc.Save(ConfigFile);
        }

        public static List<string> LoadVersionXmlFile(string VersionFile, string FileName)
        {
            List<string> Value = new List<string>();
            Value.Add(FileName);
            try
            {
                XElement rootElement = XElement.Load(VersionFile);
                var Query = from childNode in rootElement.Elements("File")
                            where (string)childNode.Attribute("Name") == FileName
                            select childNode;

                foreach (XElement childNode in Query)
                {
                    Value.Add(childNode.Element("Ver").Value);
                    Value.Add(childNode.Element("Url").Value);
                    Value.Add(childNode.Element("Desc").Value);
                }
            }
            catch
            {
                MessageBox.Show("【" + Path.GetFileName(VersionFile) + "】設定檔內容有錯誤,請刪除後再執行。");
            }
            return Value;
        }

        public static OleDbConnection OleDbOpenConn(string Database, string Password)
        {
            string cnstr = "";
            if (Password != "")
            {
                cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Database + ";Jet OLEDB:Database Password=" + Password + ";");
            }
            else
            {
                cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Database + ";");
            }
            OleDbConnection icn = new OleDbConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            try
            {
                icn.Open();
            }
            catch
            {
                
            }
            return icn;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:必須檢視 SQL 查詢中是否有安全性弱點")]
        public static DataTable GetOleDbDataTable(string Database, string OleDbString, string Password)
        {
            DataTable myDataTable = new DataTable();
            OleDbConnection icn = OleDbOpenConn(Database, Password);
            OleDbDataAdapter da = new OleDbDataAdapter(OleDbString, icn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            myDataTable = ds.Tables[0];
            if (icn.State == ConnectionState.Open) icn.Close();
            return myDataTable;
        }

        public static List<string> GetOleDbTableList(string Database, string Password)
        {
            List<string> list = new List<string>();

            if (File.Exists(Database))
            {
                OleDbConnection conn = new OleDbConnection();
                conn = CommonFunc.OleDbOpenConn(Database, Password);
                DataTable dt = conn.GetSchema("Tables");

                if (dt.Rows.Count > 0)
                {
                    Global.CrazyktvDBTableList = new List<string>();
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        if (row["TABLE_TYPE"].ToString() == "TABLE")
                        {
                            list.Add(row["TABLE_NAME"].ToString());
                        }
                    }
                }
                conn.Close();
                dt.Dispose();
                dt = null;
            }
            return list;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:必須檢視 SQL 查詢中是否有安全性弱點")]
        public static List<string> GetOleDbColumnList(string Database, string Password, string TableName)
        {
            List<string> list = new List<string>();

            OleDbConnection conn = new OleDbConnection();
            conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            DataTable dt = new DataTable();

            OleDbCommand cmd = new OleDbCommand("select * from " + TableName, conn);
            OleDbDataReader Reader = cmd.ExecuteReader();
            dt = Reader.GetSchemaTable();
            Reader.Close();

            foreach (DataRow row in dt.AsEnumerable())
            {
                list.Add(row["ColumnName"].ToString());
            }
            dt.Dispose();
            dt = null;
            return list;
        }

        public static void CompactAccessDB(string connectionString, string mdwfilename)
        {
            object[] oParams;
            object objJRO = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));

            oParams = new object[]
            {
                connectionString,
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + @"\tempdb.mdb;Jet OLEDB:Engine Type=5"
            };

            objJRO.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, objJRO, oParams);

            try
            {
                File.Copy(Application.StartupPath + @"\tempdb.mdb", mdwfilename, true);
            }
            catch
            {
                Global.SongLogDT.Rows.Add(Global.SongLogDT.NewRow());
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][0] = "【壓縮並修復資料庫】無法完成資料庫壓縮並修復,因資料庫已由其它使用者開啟。";
                Global.SongLogDT.Rows[Global.SongLogDT.Rows.Count - 1][1] = Global.SongLogDT.Rows.Count;
            }

            File.Delete(Application.StartupPath + @"\tempdb.mdb");
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objJRO);
            objJRO = null;
        }

        public static bool IsSongId(String str)
        {
            Regex r = new Regex(@"^(?:\d{5})?$|^(?:\d{6})?$");
            return r.IsMatch(str);
        }

        public static string GetSongLangStr(int SongLang, int ListType, string IndexOfList)
        {
            string Str;
            List<string> list = new List<string>();
            if (SongLang < 0) SongLang = 0;

            foreach (string langstr in Global.CrazyktvSongLangList)
            {
                list.Add(langstr);
            }
            list.Add("未知");

            if (IndexOfList == "null")
            {
                Str = list[SongLang];
            }
            else
            {
                int Value = list.IndexOf(IndexOfList);
                Str = Value.ToString();
            }
            return Str;
        }

        public static string GetSingerTypeStr(int SingerType, int ListType, string IndexOfList)
        {
            string Str;
            List<string> list = new List<string>();
            if (SingerType < 0) SingerType = 0;

            switch (ListType)
            {
                case 1:
                    list = new List<string>() { "男歌星", "女歌星", "樂團", "合唱歌曲", "外國男", "外國女", "外國樂團", "其它", "歌星姓氏", "全部歌星", "新進歌星" };
                    break;
                case 2:
                    list = Global.SongMgrCustomSingerTypeStructureList;
                    break;
                default:
                    list = new List<string>() { "男歌星", "女歌星", "樂團", "外國男", "外國女", "外國樂團", "其它", "新進歌星" };
                    break;
            }
            
            if (IndexOfList == "null")
            {
                Str = list[SingerType];
            }
            else
            {
                int Value = list.IndexOf(IndexOfList);
                Str = Value.ToString();

            }
            return Str;
        }

        public static string GetSongTrackStr(int SongTrack, int ListType, string IndexOfList)
        {
            string Str;
            List<string> list = new List<string>();
            if (SongTrack < 0) SongTrack = 0;

            switch (ListType)
            {
                case 1:
                    if (Global.SongMgrSongTrackMode == "True")
                    {
                        list = new List<string>() { "VR", "VL", "V3", "V4", "V5" };
                    }
                    else
                    {
                        list = new List<string>() { "VL", "VR", "V3", "V4", "V5" };
                    }
                    break;
                default:
                    if (Global.SongMgrSongTrackMode == "True")
                    {
                        list = new List<string>() { "右聲道 / 音軌2", "左聲道 / 音軌1", "音軌3", "音軌4", "音軌5" };
                    }
                    else
                    {
                        list = new List<string>() { "左聲道 / 音軌1", "右聲道 / 音軌2", "音軌3", "音軌4", "音軌5" };
                    }
                    break;
            }

            if (IndexOfList == "null")
            {
                Str = list[SongTrack];
            }
            else
            {
                int Value = list.IndexOf(IndexOfList);
                Str = Value.ToString();
            }
            return Str;
        }

        public static void GetRemainingSongId(int DigitCode)
        {
            if (File.Exists(Global.CrazyktvDatabaseFile) & Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
            {
                List<string> StartIdlist = new List<string>();
                StartIdlist = new List<string>(Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));
                StartIdlist.Add((DigitCode == 5) ? "100000" : "1000000");
                int RemainingSongId;

                DataTable dt = new DataTable();
                string SongQuerySqlStr = "select Song_Id, Song_Lang from ktv_Song order by Song_Id";
                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                if (dt.Rows.Count != 0)
                {
                    int i;
                    foreach (string str in Global.CrazyktvSongLangList)
                    {
                        var query = from row in dt.AsEnumerable()
                                    where row.Field<string>("Song_Lang").Equals(str) &&
                                          row.Field<string>("Song_Id").Length == DigitCode
                                    orderby row.Field<string>("Song_Id") descending
                                    select row;

                        foreach (DataRow row in query)
                        {
                            i = Global.CrazyktvSongLangList.IndexOf(str);
                            RemainingSongId = Convert.ToInt32(StartIdlist[i + 1]) - Convert.ToInt32(row["Song_Id"]) - 1;
                            if (RemainingSongId < Global.RemainingSongID) Global.RemainingSongID = RemainingSongId;
                            break;
                        }
                    }
                }
                dt.Dispose();
            }
        }

        public static void GetMaxSongId(int DigitCode)
        {
            Global.MaxIDList = new List<int>();
            List<string> StartIdlist = new List<string>();
            StartIdlist = new List<string> (Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));

            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select Song_Id, Song_Lang from ktv_Song order by Song_Id";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            Global.MaxIDList = new List<int>() { Convert.ToInt32(StartIdlist[0]) - 1, Convert.ToInt32(StartIdlist[1]) - 1,
                Convert.ToInt32(StartIdlist[2]) - 1, Convert.ToInt32(StartIdlist[3]) - 1, Convert.ToInt32(StartIdlist[4]) - 1,
                Convert.ToInt32(StartIdlist[5]) - 1, Convert.ToInt32(StartIdlist[6]) - 1, Convert.ToInt32(StartIdlist[7]) - 1,
                Convert.ToInt32(StartIdlist[8]) - 1, Convert.ToInt32(StartIdlist[9]) - 1 };

            if(dt.Rows.Count != 0)
            {
                int i;
                Parallel.ForEach(Global.CrazyktvSongLangList, (str, loopState) =>
                {
                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("Song_Lang").Equals(str) &&
                                      row.Field<string>("Song_Id").Length == DigitCode
                                orderby row.Field<string>("Song_Id") descending
                                select row;
                    
                    foreach (DataRow row in query)
                    {
                        i = Global.CrazyktvSongLangList.IndexOf(str);
                        Global.MaxIDList[i] = Convert.ToInt32(row["Song_Id"]);
                        break;
                    }
                });
            }
            dt.Dispose();
            dt = null;
        }

        public static int GetMaxSingerId(string TableName, string DatabaseFile)
        {
            int i = 0;
            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select Singer_Id from " + TableName + " order by Singer_Id";
            dt = CommonFunc.GetOleDbDataTable(DatabaseFile, SongQuerySqlStr, "");

            if (dt.Rows.Count != 0)
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<Int32>("Singer_Id") > 0
                            orderby row.Field<Int32>("Singer_Id") descending
                            select row;

                foreach (DataRow row in query)
                {
                    i = Convert.ToInt32(row["Singer_Id"]);
                    break;
                }
            }

            dt.Dispose();
            return i;
        }

        public static void GetNotExistsSongId(int DigitCode)
        {
            string MaxDigitCode = "";
            if (Global.SongMgrMaxDigitCode == "1") { MaxDigitCode = "D5"; } else { MaxDigitCode = "D6"; }
            
            List<string> StartIdlist = new List<string>();
            StartIdlist = new List<string> (Regex.Split(Global.SongMgrLangCode, ",", RegexOptions.None));

            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select Song_Id, Song_Lang from ktv_Song order by Song_Id";
            dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

            Global.LostSongIdList = new List<List<string>>();
            for (int i = 0; i < Global.CrazyktvSongLangList.Count; i++)
            {
                List<string> list = new List<string>();
                Global.LostSongIdList.Add(list);
            }

            if (dt.Rows.Count != 0)
            {
                Parallel.ForEach(Global.CrazyktvSongLangList, (str, loopState) =>
                {
                    int iMin = Convert.ToInt32(StartIdlist[Global.CrazyktvSongLangList.IndexOf(str)]);
                    List<string> ExistsIdlist = new List<string>();

                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("Song_Lang").Equals(str) &&
                                      row.Field<string>("Song_Id").Length == DigitCode
                                orderby row.Field<string>("Song_Id")
                                select row;

                    if (query.Count<DataRow>() > 0)
                    {
                        foreach (DataRow row in query)
                        {
                            ExistsIdlist.Add(row["Song_Id"].ToString());
                        }

                        if (ExistsIdlist.Count > 0)
                        {
                            int iMax = Convert.ToInt32(ExistsIdlist[ExistsIdlist.Count - 1]);
                            for (int i = iMin; i < iMax; i++)
                            {
                                if (ExistsIdlist.IndexOf(i.ToString(MaxDigitCode)) < 0)
                                {
                                    Global.LostSongIdList[Global.CrazyktvSongLangList.IndexOf(str)].Add(i.ToString(MaxDigitCode));
                                }
                            }
                        }
                    }
                    ExistsIdlist.Clear();
                });
            }
            dt.Dispose();
        }

        public static List<string> GetNotExistsSingerId(string TableName, string DatabaseFile)
        {
            List<string> list = new List<string>();
            List<int> ExistsIdlist = new List<int>();
            DataTable dt = new DataTable();
            string SongQuerySqlStr = "select Singer_Id from " + TableName + " order by Singer_Id";
            dt = CommonFunc.GetOleDbDataTable(DatabaseFile, SongQuerySqlStr, "");
            
            if (dt.Rows.Count != 0)
            {
                var query = from row in dt.AsEnumerable()
                            where row.Field<Int32>("Singer_Id") > 0
                            orderby row.Field<Int32>("Singer_Id")
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        ExistsIdlist.Add(Convert.ToInt32(row["Singer_Id"]));
                    }

                    if (ExistsIdlist.Count > 0)
                    {
                        int iMin = 1;
                        int iMax = Convert.ToInt32(ExistsIdlist[ExistsIdlist.Count - 1]);
                        Parallel.For(iMin, iMax, (i, ForloopState) =>
                        {
                            if (ExistsIdlist.IndexOf(i) < 0)
                            {
                                list.Add(i.ToString());
                            }
                        });
                    }
                }
            }
            dt.Dispose();
            return list;
        }

        public static List<string> GetSongWordCount(string SongStr)
        {
            List<string> WordCountList = new List<string>() { "0", "False" };
            if (string.IsNullOrEmpty(SongStr)) return WordCountList;

            SongStr = Regex.Replace(SongStr, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", ""); // 排除計算括號字數

            MatchCollection CJKCharMatches = Regex.Matches(SongStr, @"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d]|[\uac00-\ud7ff])");
            int CharCount = Regex.Matches(SongStr, @"[0-9][0-9'\-.]*").Count + Regex.Matches(SongStr, @"[A-Za-z][A-Za-z'\-.]*").Count + CJKCharMatches.Count;

            WordCountList[0] = CharCount.ToString();
            if (CJKCharMatches.Count == 0) { WordCountList[1] = "True"; }
            return WordCountList;
        }


        public static List<string> GetSongNameSpell(string SongStr)
        {
            List<string> SpellList = new List<string>() { "", "", "0", "" };
            if (string.IsNullOrEmpty(SongStr)) return SpellList;

            if (Global.PhoneticsWordList.Count == 0)
            {
                Global.PhoneticsWordList = new List<string>();
                Global.PhoneticsSpellList = new List<string>();
                Global.PhoneticsStrokesList = new List<string>();
                Global.PhoneticsPenStyleList = new List<string>();

                string SongPhoneticsQuerySqlStr = "select * from ktv_Phonetics";
                Global.PhoneticsDT = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongPhoneticsQuerySqlStr, "");

                var query = from row in Global.PhoneticsDT.AsEnumerable()
                            where row.Field<Int16>("SortIdx") < 2
                            select row;

                foreach (DataRow row in query)
                {
                    Global.PhoneticsWordList.Add(row["Word"].ToString());
                    Global.PhoneticsSpellList.Add((row["Spell"].ToString()).Substring(0, 1));
                    Global.PhoneticsStrokesList.Add(row["Strokes"].ToString());
                    Global.PhoneticsPenStyleList.Add((row["PenStyle"].ToString()).Substring(0, 1));
                }
                Global.PhoneticsDT.Dispose();
                Global.PhoneticsDT = null;
            }

            List<string> list = new List<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("SortIndex", typeof(string));
            dt.Columns.Add("Spell", typeof(string));
            dt.Columns.Add("SpellNum", typeof(string));
            dt.Columns.Add("Stroke", typeof(string));
            dt.Columns.Add("PenStyle", typeof(string));

            SongStr = Regex.Replace(SongStr, @"[\{\(\[｛（［【].+?[】］）｝\]\)\}]", ""); // 排除解析括號字串
            SongStr = Regex.Replace(SongStr, @"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d])", @" $1 "); // 以空格隔開中英字串
            SongStr = Regex.Replace(SongStr, @"^\s*|\s*$", ""); //去除頭尾空白

            if (SongStr != "")
            {
                list = new List<string>(Regex.Split(SongStr, @"[\s]+", RegexOptions.None));
                foreach (string str in list)
                {
                    DataRow dtrow = dt.NewRow();
                    dtrow["SortIndex"] = list.IndexOf(str);

                    Regex r = new Regex("^[A-Za-z0-9]");
                    if (r.IsMatch(str))
                    {
                        r = new Regex("^[A-Za-z]");
                        if (r.IsMatch(str))
                        {
                            dtrow["Spell"] = str.Substring(0, 1).ToUpper(); // 拼音
                            switch (str.Substring(0, 1).ToUpper())
                            {
                                case "A":
                                case "B":
                                case "C":
                                    dtrow["SpellNum"] = "2"; // 手機輸入
                                    break;
                                case "D":
                                case "E":
                                case "F":
                                    dtrow["SpellNum"] = "3";
                                    break;
                                case "G":
                                case "H":
                                case "I":
                                    dtrow["SpellNum"] = "4";
                                    break;
                                case "J":
                                case "K":
                                case "L":
                                    dtrow["SpellNum"] = "5";
                                    break;
                                case "M":
                                case "N":
                                case "O":
                                    dtrow["SpellNum"] = "6";
                                    break;
                                case "P":
                                case "Q":
                                case "R":
                                case "S":
                                    dtrow["SpellNum"] = "7";
                                    break;
                                case "T":
                                case "U":
                                case "V":
                                    dtrow["SpellNum"] = "8";
                                    break;
                                case "W":
                                case "X":
                                case "Y":
                                case "Z":
                                    dtrow["SpellNum"] = "9";
                                    break;
                            }
                        }
                        else
                        {
                            r = new Regex(@"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d])");
                            if (r.IsMatch(SongStr))
                            {
                                for (int i = 0; i < str.Length; i++)
                                {
                                    switch (str.Substring(i, 1))
                                    {
                                        case "0":
                                        case "6":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄌ"; // 拼音
                                            break;
                                        case "1":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄧ";
                                            break;
                                        case "2":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄦ";
                                            break;
                                        case "3":
                                        case "4":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄙ";
                                            break;
                                        case "5":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄨ";
                                            break;
                                        case "7":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄑ";
                                            break;
                                        case "8":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄅ";
                                            break;
                                        case "9":
                                            dtrow["Spell"] = dtrow["Spell"] + "ㄐ";
                                            break;
                                    }
                                    dtrow["SpellNum"] = dtrow["SpellNum"] + str.Substring(i, 1); // 手機輸入
                                }
                            }
                            else
                            {
                                for (int i = 0; i < str.Length; i++)
                                {
                                    dtrow["Spell"] = dtrow["Spell"] + str.Substring(i, 1); // 拼音
                                    dtrow["SpellNum"] = dtrow["SpellNum"] + str.Substring(i, 1); // 手機輸入
                                }
                            }
                        }
                        
                        if (list.IndexOf(str) == 0) dtrow["Stroke"] = "1"; // 筆劃
                        dtrow["PenStyle"] = ""; // 筆形順序
                    }
                    else
                    {
                        if (Global.PhoneticsWordList.IndexOf(str) >= 0)
                        {
                            dtrow["Spell"] = Global.PhoneticsSpellList[Global.PhoneticsWordList.IndexOf(str)]; // 拼音

                            switch (dtrow["Spell"].ToString())
                            {
                                case "ㄅ":
                                case "ㄆ":
                                case "ㄇ":
                                case "ㄈ":
                                    dtrow["SpellNum"] = "1"; // 手機輸入
                                    break;
                                case "ㄉ":
                                case "ㄊ":
                                case "ㄋ":
                                case "ㄌ":
                                    dtrow["SpellNum"] = "2";
                                    break;
                                case "ㄍ":
                                case "ㄎ":
                                case "ㄏ":
                                    dtrow["SpellNum"] = "3";
                                    break;
                                case "ㄐ":
                                case "ㄑ":
                                case "ㄒ":
                                    dtrow["SpellNum"] = "4";
                                    break;
                                case "ㄓ":
                                case "ㄔ":
                                case "ㄕ":
                                case "ㄖ":
                                    dtrow["SpellNum"] = "5";
                                    break;
                                case "ㄗ":
                                case "ㄘ":
                                case "ㄙ":
                                    dtrow["SpellNum"] = "6";
                                    break;
                                case "ㄚ":
                                case "ㄛ":
                                case "ㄜ":
                                case "ㄝ":
                                    dtrow["SpellNum"] = "7";
                                    break;
                                case "ㄞ":
                                case "ㄟ":
                                case "ㄠ":
                                case "ㄡ":
                                    dtrow["SpellNum"] = "8";
                                    break;
                                case "ㄢ":
                                case "ㄣ":
                                case "ㄤ":
                                case "ㄥ":
                                case "ㄦ":
                                    dtrow["SpellNum"] = "9";
                                    break;
                                case "ㄧ":
                                case "ㄨ":
                                case "ㄩ":
                                    dtrow["SpellNum"] = "0";
                                    break;
                            }

                            if (list.IndexOf(str) == 0) dtrow["Stroke"] = Global.PhoneticsStrokesList[Global.PhoneticsWordList.IndexOf(str)]; // 筆劃
                            dtrow["PenStyle"] = Global.PhoneticsPenStyleList[Global.PhoneticsWordList.IndexOf(str)]; // 筆形順序
                        }
                    }
                    dt.Rows.Add(dtrow);
                }
            }
            
            dt.DefaultView.Sort = "SortIndex DESC";
            foreach (DataRow row in dt.AsEnumerable())
            {
                SpellList[0] = SpellList[0] + row["Spell"].ToString();
                SpellList[1] = SpellList[1] + row["SpellNum"].ToString();
                if (row["Stroke"].ToString() != "") SpellList[2] = row["Stroke"].ToString();
                SpellList[3] = SpellList[3] + row["PenStyle"].ToString();
            }

            if (SpellList[2] == "") SpellList[2] = "0";
            return SpellList;
        }

        public static DataTable GetFavoriteUserList(int ListTpye)
        {
            Global.FavoriteUserDT = new DataTable();
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Display", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(int)));

            if (File.Exists(Global.CrazyktvDatabaseFile) & Global.CrazyktvDBTableList.IndexOf("ktv_AllSinger") >= 0)
            {
                DataTable dt = new DataTable();
                string SongQuerySqlStr = "select User_Id, User_Name from ktv_User";
                dt = CommonFunc.GetOleDbDataTable(Global.CrazyktvDatabaseFile, SongQuerySqlStr, "");

                List<string> removelist = new List<string>() { "####", "****", "9999", "^NT" };
                List<int> RemoveRowsIdxlist = new List<int>();

                var query = from row in dt.AsEnumerable()
                            where removelist.Contains(row.Field<string>("User_Id")) ||
                                  removelist.Contains(row.Field<string>("User_Id").Substring(0, row.Field<string>("User_Id").Length - 1)) ||
                                  row.Field<string>("User_Name").Contains("錢櫃新歌")
                            select row;

                if (query.Count<DataRow>() > 0)
                {
                    foreach (DataRow row in query)
                    {
                        RemoveRowsIdxlist.Add(dt.Rows.IndexOf(row));
                    }

                    if (RemoveRowsIdxlist.Count > 0)
                    {
                        for (int i = RemoveRowsIdxlist.Count - 1; i >= 0; i--)
                        {
                            dt.Rows.RemoveAt(RemoveRowsIdxlist[i]);
                        }
                    }
                }


                if (dt.Rows.Count > 0)
                {
                    Global.FavoriteUserDT = dt;
                    foreach (DataRow row in dt.AsEnumerable())
                    {
                        list.Rows.Add(list.NewRow());
                        list.Rows[list.Rows.Count - 1][0] = row["User_Name"].ToString();
                        list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                    }
                }
                else
                {
                    if (ListTpye == 0)
                    {
                        list.Rows.Add(list.NewRow());
                        list.Rows[list.Rows.Count - 1][0] = "無最愛用戶";
                        list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                    }
                }
                dt.Dispose();
            }
            else
            {
                if (ListTpye == 0)
                {
                    list.Rows.Add(list.NewRow());
                    list.Rows[list.Rows.Count - 1][0] = "無最愛用戶";
                    list.Rows[list.Rows.Count - 1][1] = list.Rows.Count;
                }
            }
            return list;
        }

        public static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        public static void SetFileTime(string Path, DateTime Time)
        {
            File.SetCreationTime(Path, Time);
            File.SetLastWriteTime(Path, Time);
            File.SetLastAccessTime(Path, Time);
        }

        public static List<int> GetSongLangCount()
        {
            List<int> SongLangCount = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
            {
                var query = from row in Global.SongStatisticsDT.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals(langstr)
                            select row;
                if (query.Count<DataRow>() > 0)
                {
                    SongLangCount[Global.CrazyktvSongLangList.IndexOf(langstr)] = query.Count<DataRow>();
                }
                else
                {
                    SongLangCount[Global.CrazyktvSongLangList.IndexOf(langstr)] = 0;
                }
            });

            for (int i = 0; i < 10; i++)
            {
                SongLangCount[10] += SongLangCount[i];
            }
            return SongLangCount;
        }


        public static List<int> GetSongFileCount()
        {
            List<int> SongFileCount = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            string SongFilePath = "";

            Parallel.ForEach(Global.CrazyktvSongLangList, (langstr, loopState) =>
            {
                var query = from row in Global.SongStatisticsDT.AsEnumerable()
                            where row.Field<string>("Song_Lang").Equals(langstr)
                            select row;
                if (query.Count<DataRow>() > 0)
                {
                    foreach(DataRow row in query)
                    {
                        SongFilePath = Path.Combine(row["Song_Path"].ToString(), row["Song_FileName"].ToString());
                        if (File.Exists(SongFilePath)) SongFileCount[Global.CrazyktvSongLangList.IndexOf(langstr)]++;
                    }
                }
                else
                {
                    SongFileCount[Global.CrazyktvSongLangList.IndexOf(langstr)] = 0;
                }
            });

            for (int i = 0; i < 10; i++)
            {
                SongFileCount[10] += SongFileCount[i];
            }
            return SongFileCount;
        }

        public static List<int> GetSingerTypeCount()
        {
            List<int> SingerTypeCount = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            List<int> SingerTypeList = new List<int>();

            foreach (string str in Global.CrazyktvSingerTypeList)
            {
                if (str != "未使用")
                {
                    SingerTypeList.Add(Global.CrazyktvSingerTypeList.IndexOf(str));
                }
            }

            foreach (int SingerTypeValue in SingerTypeList)
            {
                var query = from row in (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? Global.SingerDT.AsEnumerable() : Global.AllSingerDT.AsEnumerable()
                            where row.Field<string>("Singer_Type").Equals(SingerTypeValue.ToString())
                            select row;
                if (query.Count<DataRow>() > 0)
                {
                    SingerTypeCount[SingerTypeList.IndexOf(SingerTypeValue)] = query.Count<DataRow>();
                    SingerTypeCount[9] += query.Count<DataRow>();
                }
                else
                {
                    SingerTypeCount[SingerTypeList.IndexOf(SingerTypeValue)] = 0;
                }
            }
            return SingerTypeCount;
        }

        public static string GetWordUnicode(string word)
        {
            string Unicode = "";
            byte[] UnicodeByte = Encoding.UTF32.GetBytes(word);
            if (UnicodeByte[2] != 00)
            {
                Unicode = String.Format("{0:X}", UnicodeByte[2]) + String.Format("{0:X2}", UnicodeByte[1]) + String.Format("{0:X2}", UnicodeByte[0]);
            }
            else
            {
                Unicode = String.Format("{0:X2}", UnicodeByte[1]) + String.Format("{0:X2}", UnicodeByte[0]);
            }
            return Unicode;
        }

        public static string ConvToWide(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] < 127) { c[i] = (char)(c[i] + 65248); }
            }
            return new string(c);
        }

        public static string ConvToNarrow(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] > 65280 && c[i] < 65375) { c[i] = (char)(c[i] - 65248); }
            }
            return new string(c);
        }

        public static List<string> GetSynonymousSongNameList(string SongQueryValue)
        {
            List<string> SynonymousSongNameList = new List<string>();

            string SynonymousWord = string.Join("|", Global.SynonymousWordList);
            List<string> SynonymousWordList = new List<string>(SynonymousWord.Split('|'));

            MatchCollection matches = Regex.Matches(SongQueryValue, @"([\u2E80-\u33FF]|[\u4E00-\u9FCC\u3400-\u4DB5\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\ud840-\ud868][\udc00-\udfff]|\ud869[\udc00-\uded6\udf00-\udfff]|[\ud86a-\ud86c][\udc00-\udfff]|\ud86d[\udc00-\udf34\udf40-\udfff]|\ud86e[\udc00-\udc1d]|[\uac00-\ud7ff])");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (SynonymousWordList.IndexOf((match.Value)) >= 0)
                    {
                        foreach (string str in Global.SynonymousWordList)
                        {
                            if (str.Contains(match.Value))
                            {
                                List<string> list = new List<string>(str.Split('|'));
                                foreach (string wordstr in list)
                                {
                                    string SongName = Regex.Replace(SongQueryValue, match.Value, wordstr, RegexOptions.IgnoreCase);
                                    if (SynonymousSongNameList.IndexOf(SongName) < 0 && SongName != SongQueryValue)
                                    {
                                        SynonymousSongNameList.Add(SongName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return SynonymousSongNameList;
        }

        public static bool DownloadFile(string File, string Url)
        {
            bool DownloadStatus = false;
            FileStream FStream = new FileStream(File, FileMode.Create);

            try
            {
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(Url);
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();

                Stream DataStream = Response.GetResponseStream();
                byte[] Databuffer = new byte[1024];
                int CompletedLength = 0;

                while ((CompletedLength = DataStream.Read(Databuffer, 0, 1024)) > 0)
                {
                    FStream.Write(Databuffer, 0, CompletedLength);
                }

                FStream.Close();
                DataStream.Close();
                Response.Close();
                DownloadStatus = true;
            }
            catch
            {
                FStream.Close();
                DownloadStatus = false;
            }
            return DownloadStatus;
        }

        public static string ConvToTraditionalChinese(string strSource)
        {
            int LocaleSystemDefault = 0x0800;
            int LcmapTraditionalChinese = 0x04000000;

            byte[] strSourceByte = Encoding.Default.GetBytes(strSource);
            int Result = ChEncAutoDetector.Analyze(strSourceByte);
            if (Result == -1)
            {
                byte[] strDestByte = Encoding.Convert(Encoding.GetEncoding(936), Encoding.UTF8, strSourceByte);
                strSource = Encoding.UTF8.GetString(strDestByte);
            }
            
            var strDest = new String(' ', strSource.Length);
            NativeMethods.LCMapString(LocaleSystemDefault, LcmapTraditionalChinese, strSource, strSource.Length, strDest, strSource.Length);
            return strDest;
        }


        // BIG5 GB2312繁簡編碼快篩 https://github.com/darkthread/CEAD 
        public class ChEncAutoDetector
        {
            /// <summary>
            /// 分析報告
            /// </summary>
            class Report
            {
                //統計解讀為ASCII、符號、常用字、次常用字及亂碼(非有效字元)字數
                public int Ascii, Symbol, Common, Rare, Unknow;
                /// <summary>
                /// 亂碼指標(數值愈大，不是該編碼的機率愈高)
                /// </summary>
                public float BadSmell
                {
                    get
                    {
                        int total = Ascii + Symbol + Common + Rare + Unknow;
                        if (total == 0) return 0;
                        return (float)(Rare + Unknow * 3) / total;
                    }
                }
            }

            //將00-01-02-03格式轉為byte[]
            public static byte[] ParseHexStr(string hex)
            {
                if (string.IsNullOrEmpty(hex)) return new byte[] { };
                var l = (hex.Length + 1) / 3;
                byte[] b = new byte[l];
                int i = 0;
                foreach (string h in hex.Split('-'))
                    b[i++] = Convert.ToByte(h, 16);
                return b;
            }

            /// <summary>
            /// 分析二進位資料為繁體或是簡體
            /// </summary>
            /// <param name="data">二進位內容</param>
            /// <returns>1表示較可能為繁體, -1表示較可能為簡體, 0表無法識別</returns>
            public static int Analyze(byte[] data)
            {
                var resBig5 = AnalyzeBig5(data);
                var resGB = AnalyzeGB2312(data);
                if (resBig5.BadSmell < resGB.BadSmell)
                    return 1;
                else if (resBig5.BadSmell > resGB.BadSmell)
                    return -1;
                else
                    return 0;
            }
            //試著解析成BIG5編碼，取得分析報告
            private static Report AnalyzeBig5(byte[] data)
            {
                Report res = new Report();
                bool isDblBytes = false;
                byte dblByteHi = 0;
                foreach (byte b in data)
                {
                    if (isDblBytes)
                    {
                        if (b >= 0x40 && b <= 0x7e || b >= 0xa1 && b <= 0xfe)
                        {
                            int c = dblByteHi * 0x100 + b;
                            if (c >= 0xa140 && c <= 0xa3bf)
                                res.Symbol++; //符號
                            else if (c >= 0xa440 && c <= 0xc67e)
                                res.Common++; //常用字
                            else if (c >= 0xc940 && c <= 0xf9d5)
                                res.Rare++; //次常用字
                            else
                                res.Unknow++; //無效字元
                        }
                        else
                            res.Unknow++;
                        isDblBytes = false;
                    }
                    else if (b >= 0x80 && b <= 0xfe)
                    {
                        isDblBytes = true;
                        dblByteHi = b;
                    }
                    else if (b < 0x80)
                        res.Ascii++;
                }
                return res;
            }
            //試著解析成GB2312，取得分析報告
            private static Report AnalyzeGB2312(byte[] data)
            {
                Report res = new Report();
                bool isDblBytes = false;
                byte dblByteHi = 0;
                foreach (byte b in data)
                {
                    if (isDblBytes)
                    {
                        if (b >= 0xa1 && b <= 0xfe)
                        {
                            if (dblByteHi >= 0xa1 && dblByteHi <= 0xa9)
                                res.Symbol++; //符號
                            else if (dblByteHi >= 0xb0 && dblByteHi <= 0xd7)
                                res.Common++; //一級漢字(常用字)
                            else if (dblByteHi >= 0xd8 && dblByteHi <= 0xf7)
                                res.Rare++; //二級漢字(次常用字)
                            else
                                res.Unknow++; //無效字元
                        }
                        else
                            res.Unknow++; //無效字元
                        isDblBytes = false;
                    }
                    else if (b >= 0xa1 && b <= 0xf7)
                    {
                        isDblBytes = true;
                        dblByteHi = b;
                    }
                    else if (b < 0x80)
                        res.Ascii++;
                }
                return res;
            }
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


    }
}
