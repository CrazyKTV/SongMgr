using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        private void MainUIScale_UIScale(float ScalingFactor, string FontName)
        {
            this.Size = new System.Drawing.Size(Convert.ToInt32(1024 * Global.DPIScalingFactor * ScalingFactor), Convert.ToInt32(768 * Global.DPIScalingFactor * ScalingFactor));
            this.MaximumSize = new System.Drawing.Size(Convert.ToInt32(1024 * Global.DPIScalingFactor * ScalingFactor), Convert.ToInt32(768 * Global.DPIScalingFactor * ScalingFactor));
            this.MinimumSize = new System.Drawing.Size(Convert.ToInt32(1024 * Global.DPIScalingFactor * ScalingFactor), Convert.ToInt32(768 * Global.DPIScalingFactor * ScalingFactor));
            this.ClientSize = new System.Drawing.Size(Convert.ToInt32(1008 * Global.DPIScalingFactor * ScalingFactor), Convert.ToInt32(729 * Global.DPIScalingFactor * ScalingFactor));
            this.Font = new System.Drawing.Font(FontName, 12F * Global.DPIScalingFactor * ScalingFactor, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

            this.SongQuery_DataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SongQuery_DataGridView.RowsDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SongQuery_DataGridView.RowTemplate.Height = this.SongQuery_DataGridView.RowsDefaultCellStyle.Font.Height;

            this.SongAdd_DataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SongAdd_DataGridView.RowsDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SongAdd_DataGridView.RowTemplate.Height = this.SongAdd_DataGridView.RowsDefaultCellStyle.Font.Height;

            this.SingerMgr_DataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SingerMgr_DataGridView.RowsDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SingerMgr_DataGridView.RowTemplate.Height = this.SingerMgr_DataGridView.RowsDefaultCellStyle.Font.Height;

            this.Cashbox_DataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cashbox_DataGridView.RowsDefaultCellStyle.Font = new System.Drawing.Font(FontName, 12F * ScalingFactor, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cashbox_DataGridView.RowTemplate.Height = this.Cashbox_DataGridView.RowsDefaultCellStyle.Font.Height;

            MainTabControl.Visible = false;
            MainTabControl.ItemSize = new System.Drawing.Size(0, 0);
            MainTabControl.SizeMode = TabSizeMode.Normal;
            SongQuery_TabControl.ItemSize = new System.Drawing.Size(0, 0);
            SongQuery_TabControl.SizeMode = TabSizeMode.Normal;
            SongMgrCfg_TabControl.ItemSize = new System.Drawing.Size(0, 0);
            SongMgrCfg_TabControl.SizeMode = TabSizeMode.Normal;
            SongMaintenance_TabControl.ItemSize = new System.Drawing.Size(0, 0);
            SongMaintenance_TabControl.SizeMode = TabSizeMode.Normal;
            Cashbox_TabControl.ItemSize = new System.Drawing.Size(0, 0);
            Cashbox_TabControl.SizeMode = TabSizeMode.Normal;

            if (Global.MainCfgHideSongDBConverterTabPage == "True")
            {
                if (MainTabControl.TabPages.IndexOf(SongDBConverter_TabPage) < 0)
                {
                    MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(MainCfg_TabPage) + 1, SongDBConverter_TabPage);
                    SongDBConverter_TabPage.Show();
                }
            }

            if (Global.MainCfgHideSongAddResultTabPage == "True")
            {
                if (MainTabControl.TabPages.IndexOf(SongAddResult_TabPage) < 0)
                {
                    MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(MainCfg_TabPage) + 1, SongAddResult_TabPage);
                    SongAddResult_TabPage.Show();
                }
            }

            if (Global.MainCfgHideSongLogTabPage == "True")
            {
                if (MainTabControl.TabPages.IndexOf(SongLog_TabPage) < 0)
                {
                    MainTabControl.TabPages.Insert(MainTabControl.TabPages.IndexOf(MainCfg_TabPage) + 1, SongLog_TabPage);
                    SongLog_TabPage.Show();
                }
            }

            if (Global.SongMgrSongAddMode != "4")
            {
                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_MonitorFolders_TabPage) < 0)
                {
                    SongMgrCfg_TabControl.TabPages.Insert(SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SingerGroup_TabPage) + 1, SongMgrCfg_MonitorFolders_TabPage);
                }
            }

            if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
            {
                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_CustomStructure_TabPage) < 0)
                {
                    SongMgrCfg_TabControl.TabPages.Insert(SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SingerGroup_TabPage) + 1, SongMgrCfg_CustomStructure_TabPage);
                }

                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SongStructure_TabPage) < 0)
                {
                    SongMgrCfg_TabControl.TabPages.Insert(SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SingerGroup_TabPage) + 1, SongMgrCfg_SongStructure_TabPage);
                }
            }

            foreach (Control MainControl in this.Controls)
            {
                MainControl.Location = new System.Drawing.Point(Convert.ToInt32(MainControl.Location.X / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(MainControl.Location.Y / MainUIScale.UIScalingFactor * ScalingFactor));
                MainControl.Size = new System.Drawing.Size(Convert.ToInt32(MainControl.Size.Width / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(MainControl.Size.Height / MainUIScale.UIScalingFactor * ScalingFactor));
                MainControl.Font = new System.Drawing.Font(FontName, MainControl.Font.Size / MainUIScale.UIScalingFactor * ScalingFactor, MainControl.Font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

                if (MainControl.Controls.Count > 0)
                {
                    foreach (Control SubControl in MainControl.Controls)
                    {
                        SubControl.Location = new System.Drawing.Point(Convert.ToInt32(SubControl.Location.X / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(SubControl.Location.Y / MainUIScale.UIScalingFactor * ScalingFactor));
                        SubControl.Size = new System.Drawing.Size(Convert.ToInt32(SubControl.Size.Width / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(SubControl.Size.Height / MainUIScale.UIScalingFactor * ScalingFactor));
                        SubControl.Font = new System.Drawing.Font(FontName, SubControl.Font.Size / MainUIScale.UIScalingFactor * ScalingFactor, SubControl.Font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

                        if (SubControl.Controls.Count > 0)
                        {
                            foreach (Control Sub1Control in SubControl.Controls)
                            {
                                Sub1Control.Location = new System.Drawing.Point(Convert.ToInt32(Sub1Control.Location.X / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub1Control.Location.Y / MainUIScale.UIScalingFactor * ScalingFactor));
                                Sub1Control.Size = new System.Drawing.Size(Convert.ToInt32(Sub1Control.Size.Width / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub1Control.Size.Height / MainUIScale.UIScalingFactor * ScalingFactor));
                                Sub1Control.Font = new System.Drawing.Font(FontName, Sub1Control.Font.Size / MainUIScale.UIScalingFactor * ScalingFactor, Sub1Control.Font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

                                if (Sub1Control.Controls.Count > 0)
                                {
                                    foreach (Control Sub2Control in Sub1Control.Controls)
                                    {
                                        Sub2Control.Location = new System.Drawing.Point(Convert.ToInt32(Sub2Control.Location.X / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub2Control.Location.Y / MainUIScale.UIScalingFactor * ScalingFactor));
                                        Sub2Control.Size = new System.Drawing.Size(Convert.ToInt32(Sub2Control.Size.Width / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub2Control.Size.Height / MainUIScale.UIScalingFactor * ScalingFactor));
                                        Sub2Control.Font = new System.Drawing.Font(FontName, Sub2Control.Font.Size / MainUIScale.UIScalingFactor * ScalingFactor, Sub2Control.Font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

                                        if (Sub2Control.Controls.Count > 0)
                                        {
                                            foreach (Control Sub3Control in Sub2Control.Controls)
                                            {
                                                Sub3Control.Location = new System.Drawing.Point(Convert.ToInt32(Sub3Control.Location.X / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub3Control.Location.Y / MainUIScale.UIScalingFactor * ScalingFactor));
                                                Sub3Control.Size = new System.Drawing.Size(Convert.ToInt32(Sub3Control.Size.Width / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub3Control.Size.Height / MainUIScale.UIScalingFactor * ScalingFactor));
                                                Sub3Control.Font = new System.Drawing.Font(FontName, Sub3Control.Font.Size / MainUIScale.UIScalingFactor * ScalingFactor, Sub3Control.Font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

                                                if (Sub3Control.Controls.Count > 0)
                                                {
                                                    foreach (Control Sub4Control in Sub3Control.Controls)
                                                    {
                                                        Sub4Control.Location = new System.Drawing.Point(Convert.ToInt32(Sub4Control.Location.X / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub4Control.Location.Y / MainUIScale.UIScalingFactor * ScalingFactor));
                                                        Sub4Control.Size = new System.Drawing.Size(Convert.ToInt32(Sub4Control.Size.Width / MainUIScale.UIScalingFactor * ScalingFactor), Convert.ToInt32(Sub4Control.Size.Height / MainUIScale.UIScalingFactor * ScalingFactor));
                                                        Sub4Control.Font = new System.Drawing.Font(FontName, Sub4Control.Font.Size / MainUIScale.UIScalingFactor * ScalingFactor, Sub4Control.Font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ((MainTabControl.ItemSize.Width - MainCfg_MainTabWidth_Button.Width) > 16)
            {
                MainTabControl.SizeMode = TabSizeMode.Fixed;
                MainTabControl.ItemSize = new System.Drawing.Size(MainCfg_MainTabWidth_Button.Width, MainTabControl.ItemSize.Height);
                SongQuery_TabControl.SizeMode = TabSizeMode.Fixed;
                SongQuery_TabControl.ItemSize = new System.Drawing.Size(MainCfg_SubTabWidth_Button.Width, SongQuery_TabControl.ItemSize.Height);
                SongMgrCfg_TabControl.SizeMode = TabSizeMode.Fixed;
                SongMgrCfg_TabControl.ItemSize = new System.Drawing.Size(MainCfg_SubTabWidth_Button.Width, SongMgrCfg_TabControl.ItemSize.Height);
                SongMaintenance_TabControl.SizeMode = TabSizeMode.Fixed;
                SongMaintenance_TabControl.ItemSize = new System.Drawing.Size(MainCfg_SubTabWidth_Button.Width, SongMaintenance_TabControl.ItemSize.Height);
                Cashbox_TabControl.SizeMode = TabSizeMode.Fixed;
                Cashbox_TabControl.ItemSize = new System.Drawing.Size(MainCfg_SubTabWidth_Button.Width, Cashbox_TabControl.ItemSize.Height);
            }

            if (Global.MainCfgHideSongDBConverterTabPage == "True")
            {
                if (MainTabControl.TabPages.IndexOf(SongDBConverter_TabPage) >= 0)
                {
                    SongDBConverter_TabPage.Hide();
                    MainTabControl.TabPages.Remove(SongDBConverter_TabPage);
                }
            }

            if (Global.MainCfgHideSongAddResultTabPage == "True")
            {
                if (MainTabControl.TabPages.IndexOf(SongAddResult_TabPage) >= 0)
                {
                    SongAddResult_TabPage.Hide();
                    MainTabControl.TabPages.Remove(SongAddResult_TabPage);
                }
            }

            if (Global.MainCfgHideSongLogTabPage == "True")
            {
                if (MainTabControl.TabPages.IndexOf(SongLog_TabPage) >= 0)
                {
                    SongLog_TabPage.Hide();
                    MainTabControl.TabPages.Remove(SongLog_TabPage);
                }
            }

            if (Global.SongMgrSongAddMode != "4")
            {
                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_MonitorFolders_TabPage) >= 0)
                {
                    SongMgrCfg_MonitorFolders_TabPage.Hide();
                    SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_MonitorFolders_TabPage);
                }
            }

            if (Global.SongMgrSongAddMode == "3" || Global.SongMgrSongAddMode == "4")
            {
                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_CustomStructure_TabPage) >= 0)
                {
                    SongMgrCfg_CustomStructure_TabPage.Hide();
                    SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_CustomStructure_TabPage);
                }

                if (SongMgrCfg_TabControl.TabPages.IndexOf(SongMgrCfg_SongStructure_TabPage) >= 0)
                {
                    SongMgrCfg_SongStructure_TabPage.Hide();
                    SongMgrCfg_TabControl.TabPages.Remove(SongMgrCfg_SongStructure_TabPage);
                }
            }

            MainCfg_PlayerRegAudioProcessor_Button.Image = MainUIScale.GetDPIScaleImage(Properties.Resources.UAC, ScalingFactor);

            MainTabControl.Visible = true;
            MainUIScale.UIScalingFactor = ScalingFactor;
        }
    }

    class MainUIScale
    {
        public static float UIScalingFactor = 1;

        public static Image GetDPIScaleImage(Image image, float ScalingFactor)
        {
            int width = Convert.ToInt32(image.Width * Global.DPIScalingFactor * ScalingFactor);
            int height = Convert.ToInt32(image.Height * Global.DPIScalingFactor * ScalingFactor);
            Bitmap bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(image, new Rectangle(0, 0, width, height));
            }
            image.Dispose();
            return bitmap;
        }
    }
}
