using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using CrazyKTV_MediaKit.DirectShow.Controls;
using CrazyKTV_MediaKit.DirectShow.MediaPlayers;

namespace CrazyKTV_SongMgr
{
    public partial class DShowForm : Form
    {
        string SongId;
        string SongLang;
        string SongSinger;
        string SongSongName;
        string SongTrack;
        string SongVolume;
        string SongReplayGain;
        string SongMeanVolume;
        string SongFilePath;
        string dvRowIndex;
        string UpdateSongTrack;
        string UpdateDataGridView;

        private MediaUriElement mediaUriElement;
        private System.Timers.Timer mouseClickTimer;
        private bool sliderInit;
        private bool sliderDrag;

        public DShowForm()
        {
            InitializeComponent();
        }

        public DShowForm(Form ParentForm, List<string> PlayerSongInfoList)
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(DShowForm_MouseWheel);

            this.Owner = ParentForm;
            SongId = PlayerSongInfoList[0];
            SongLang = PlayerSongInfoList[1];
            SongSinger = PlayerSongInfoList[2];
            SongSongName = PlayerSongInfoList[3];
            SongTrack = PlayerSongInfoList[4];
            SongVolume = PlayerSongInfoList[5];
            SongReplayGain = PlayerSongInfoList[6];
            SongMeanVolume = PlayerSongInfoList[7];
            SongFilePath = PlayerSongInfoList[8];
            dvRowIndex = PlayerSongInfoList[9];
            UpdateDataGridView = PlayerSongInfoList[10];

            this.Text = "【" + SongLang + "】" + SongSinger + " - " + SongSongName;

            sliderInit = false;

            mediaUriElement = new MediaUriElement();
            mediaUriElement.BeginInit();
            elementHost.Child = mediaUriElement;
            mediaUriElement.EndInit();

            mediaUriElement.MediaUriPlayer.CodecsDirectory = System.Windows.Forms.Application.StartupPath + @"\Codec";
            mediaUriElement.VideoRenderer = (Global.MainCfgPlayerOutput == "1") ? CrazyKTV_MediaKit.DirectShow.MediaPlayers.VideoRendererType.VideoMixingRenderer9 : CrazyKTV_MediaKit.DirectShow.MediaPlayers.VideoRendererType.EnhancedVideoRenderer;
            mediaUriElement.DeeperColor = (Global.MainCfgPlayerOutput == "1") ? false : true;
            mediaUriElement.Stretch = System.Windows.Media.Stretch.Fill;
            mediaUriElement.EnableAudioCompressor = bool.Parse(Global.MainCfgPlayerEnableAudioCompressor);
            mediaUriElement.EnableAudioProcessor = bool.Parse(Global.MainCfgPlayerEnableAudioProcessor);

            mediaUriElement.MediaFailed += MediaUriElement_MediaFailed;
            mediaUriElement.MediaEnded += MediaUriElement_MediaEnded;
            mediaUriElement.MouseLeftButtonDown += mediaUriElement_MouseLeftButtonDown;
            mediaUriElement.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;

            // 隨選視訊
            if (Global.PlayerRandomVideoList.Count == 0)
            {
                string dir = System.Windows.Forms.Application.StartupPath + @"\Video";
                if (Directory.Exists(dir))
                {
                    Global.PlayerRandomVideoList.AddRange(Directory.GetFiles(dir));
                    if (Global.PlayerRandomVideoList.Count > 0)
                    {
                        Random rand = new Random(Guid.NewGuid().GetHashCode());
                        Global.PlayerRandomVideoList = Global.PlayerRandomVideoList.OrderBy(str => rand.Next()).ToList<string>();
                    }
                }
            }
            mediaUriElement.VideoSource = (Global.PlayerRandomVideoList.Count > 0) ? new Uri(Global.PlayerRandomVideoList[0]) : null;
            mediaUriElement.Source = new Uri(SongFilePath);

            mediaUriElement.Volume = Math.Round(Convert.ToDouble(Global.MainCfgPlayerDefaultVolume) / 100, 2);
            // 音量平衡
            int GainVolume = Convert.ToInt32(SongVolume);
            if (SongReplayGain != "" && SongMeanVolume != "")
            {
                int basevolume = 100;
                GainVolume = basevolume;

                List<int> maxvolumelist = new List<int>() { -18, -17, -16, -15, -14, -13, -12, -11, -10, -9 };
                int maxvolume = maxvolumelist[Convert.ToInt32(Global.SongMaintenanceMaxVolume) - 1];
                maxvolumelist.Clear();
                maxvolumelist = null;

                double GainDB = Convert.ToDouble(SongReplayGain);
                double MeanDB = Convert.ToDouble(SongMeanVolume);
                if (GainDB * -1 > 0)
                {
                    GainVolume = Convert.ToInt32(basevolume * Math.Pow(10, (GainDB * -1) / 20));
                }
                else
                {
                    if (MeanDB > maxvolume)
                    {
                        GainVolume = Convert.ToInt32(basevolume * Math.Pow(10, (maxvolume - MeanDB) / 20));
                    }
                }
            }
            mediaUriElement.AudioAmplify = GainVolume;
            Player_CurrentGainValue_Label.BeginInvokeIfRequired(lbl => lbl.Text = GainVolume + " %");

            SpinWait.SpinUntil(() => mediaUriElement.MediaUriPlayer.PlayerState == PlayerState.Opened);

            mediaUriElement.AudioTrackList = mediaUriElement.GetAudioTrackList();
            string ChannelValue = string.Empty;
            if (mediaUriElement.AudioTrackList.Count == 1)
            {
                switch (SongTrack)
                {
                    case "1":
                        if (mediaUriElement.AudioChannel != 1) mediaUriElement.AudioChannel = 1;
                        ChannelValue = "1";
                        break;
                    case "2":
                        if (mediaUriElement.AudioChannel != 2) mediaUriElement.AudioChannel = 2;
                        ChannelValue = "2";
                        break;
                }
            }
            else if (mediaUriElement.AudioTrackList.Count > 1)
            {
                switch (SongTrack)
                {
                    case "1":
                        if (mediaUriElement.AudioTrackList.IndexOf(mediaUriElement.AudioTrack) != mediaUriElement.AudioTrackList[0]) mediaUriElement.AudioTrack = mediaUriElement.AudioTrackList[0];
                        ChannelValue = "1";
                        break;
                    case "2":
                        if (mediaUriElement.AudioTrackList.IndexOf(mediaUriElement.AudioTrack) != mediaUriElement.AudioTrackList[1]) mediaUriElement.AudioTrack = mediaUriElement.AudioTrackList[1];
                        ChannelValue = "2";
                        break;
                }
            }
            Player_CurrentChannelValue_Label.BeginInvokeIfRequired(lbl => lbl.Text = (ChannelValue == SongTrack) ? "伴唱" : "人聲");
            Player_CurrentVolumeValue_Label.BeginInvokeIfRequired(lbl => lbl.Text = Convert.ToInt32(mediaUriElement.Volume * 100).ToString());

            if (mediaUriElement.MediaUriPlayer.IsAudioOnly && Global.PlayerRandomVideoList.Count > 0)
                Global.PlayerRandomVideoList.RemoveAt(0);

            NativeMethods.SystemSleepManagement.PreventSleep(true);
        }

        private void DShowForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta > 0)
                {
                    if (mediaUriElement.Volume <= 0.95)
                    {
                        mediaUriElement.Volume += 0.05;
                    }
                    else
                    {
                        mediaUriElement.Volume = 1.00;
                    }
                }
                else
                {
                    if (mediaUriElement.Volume >= 0.05)
                    {
                        mediaUriElement.Volume -= 0.05;
                    }
                    else
                    {
                        mediaUriElement.Volume = 0;
                    }
                }
                mediaUriElement.Volume = Math.Round(mediaUriElement.Volume, 2);
                Player_CurrentVolumeValue_Label.BeginInvokeIfRequired(lbl => lbl.Text = Convert.ToInt32(mediaUriElement.Volume * 100).ToString());
            }
        }

        private void MediaUriElement_MediaFailed(object sender, CrazyKTV_MediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        {
            this.BeginInvokeIfRequired(form => form.Text = e.Message);
        }

        private void MediaUriPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            if (Player_VideoSizeValue_Label.Text == "尚無資料")
            {
                this.Invoke((Action)delegate ()
                {
                    if (mediaUriElement.NaturalVideoWidth != 0 && mediaUriElement.NaturalVideoHeight != 0)
                    {
                        Player_VideoSizeValue_Label.Text = mediaUriElement.NaturalVideoWidth + "x" + mediaUriElement.NaturalVideoHeight;
                    }
                });
            }

            if (sliderDrag)
                return;

            if (!sliderInit)
            {
                this.Invoke((Action)delegate ()
                {
                    if (mediaUriElement.MediaDuration > 0)
                    {
                        Player_ProgressTrackBar.Maximum = ((int)mediaUriElement.MediaDuration < 0) ? (int)mediaUriElement.MediaDuration * -1 : (int)mediaUriElement.MediaDuration;
                        sliderInit = true;
                    }
                });
            }
            else
            {
                this.BeginInvoke(new Action(ChangeSlideValue), null);
            }
        }

        private void ChangeSlideValue()
        {
            if (sliderDrag)
                return;

            if (sliderInit)
            {
                double perc = (double)mediaUriElement.MediaPosition / mediaUriElement.MediaDuration;
                int newValue = (int)(Player_ProgressTrackBar.Maximum * perc);
                if (newValue - Player_ProgressTrackBar.ProgressBarValue < 500000) return;
                Player_ProgressTrackBar.TrackBarValue = newValue;
                Player_ProgressTrackBar.ProgressBarValue = newValue;
            }
        }

        private void Player_ProgressTrackBar_Click(object sender, EventArgs e)
        {
            if (!sliderInit)
                return;

            this.BeginInvoke(new Action(ChangeMediaPosition), null);
        }

        private void ChangeMediaPosition()
        {
            sliderDrag = true;
            double perc = (double)Player_ProgressTrackBar.TrackBarValue / Player_ProgressTrackBar.Maximum;
            mediaUriElement.MediaPosition = (long)(mediaUriElement.MediaDuration * perc);
            Player_ProgressTrackBar.ProgressBarValue = Player_ProgressTrackBar.TrackBarValue;
            sliderDrag = false;
        }

        private void MediaUriElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaUriElement.Stop();
            mediaUriElement.MediaPosition = 0;
            Player_ProgressTrackBar.TrackBarValue = 0;
            Player_ProgressTrackBar.ProgressBarValue = 0;
            Player_PlayControl_Button.Text = "播放";
        }

        private void Player_SwithChannel_Button_Click(object sender, EventArgs e)
        {
            string ChannelValue = string.Empty;

            if (mediaUriElement.AudioTrackList.Count > 1)
            {
                if (mediaUriElement.AudioTrackList.IndexOf(mediaUriElement.AudioTrack) == 0)
                {
                    mediaUriElement.AudioTrack = mediaUriElement.AudioTrackList[1];
                    ChannelValue = "2";
                    UpdateSongTrack = "2";
                }
                else
                {
                    mediaUriElement.AudioTrack = mediaUriElement.AudioTrackList[0];
                    ChannelValue = "1";
                    UpdateSongTrack = "1";
                }
            }
            else
            {
                if (mediaUriElement.AudioChannel == 1)
                {
                    mediaUriElement.AudioChannel = 2;
                    ChannelValue = "2";
                    UpdateSongTrack = "2";
                }
                else
                {
                    mediaUriElement.AudioChannel = 1;
                    ChannelValue = "1";
                    UpdateSongTrack = "1";
                }
            }
            Player_CurrentChannelValue_Label.Text = (ChannelValue == SongTrack) ? "伴唱" : "人聲";
            Player_UpdateChannel_Button.Enabled = (Player_CurrentChannelValue_Label.Text == "人聲") ? true : false;
        }

        private void Player_UpdateChannel_Button_Click(object sender, EventArgs e)
        {
            SongTrack = UpdateSongTrack;
            Player_UpdateChannel_Button.Enabled = false;
            Player_CurrentChannelValue_Label.Text = "伴唱";
            Global.PlayerUpdateSongValueList = new List<string>() { UpdateDataGridView, dvRowIndex, SongTrack };
        }

        private void Player_PlayControl_Button_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Text)
            {
                case "暫停播放":
                    mediaUriElement.Pause();
                    ((Button)sender).Text = "繼續播放";
                    break;
                case "繼續播放":
                    mediaUriElement.Play();
                    ((Button)sender).Text = "暫停播放";
                    break;
                case "播放":
                    mediaUriElement.Play();
                    ((Button)sender).Text = "暫停播放";
                    break;
            }
        }

        private void mediaUriElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mouseClickTimer == null)
            {
                mouseClickTimer = new System.Timers.Timer();
                mouseClickTimer.Interval = SystemInformation.DoubleClickTime;
                mouseClickTimer.Elapsed += new System.Timers.ElapsedEventHandler(mouseClickTimer_Tick);
            }

            if (!mouseClickTimer.Enabled)
            {
                mouseClickTimer.Start();
            }
            else
            {
                mouseClickTimer.Stop();
                ToggleFullscreen();
            }
        }

        private void mouseClickTimer_Tick(object sender, EventArgs e)
        {
            mouseClickTimer.Stop();

            switch (Player_PlayControl_Button.Text)
            {
                case "暫停播放":
                    mediaUriElement.Dispatcher.Invoke(new Action(() => mediaUriElement.Pause()));
                    Player_PlayControl_Button.InvokeIfRequired<Button>(btn => btn.Text = "繼續播放");
                    break;
                case "繼續播放":
                    mediaUriElement.Dispatcher.Invoke(new Action(() => mediaUriElement.Play()));
                    Player_PlayControl_Button.InvokeIfRequired<Button>(btn => btn.Text = "暫停播放");
                    break;
            }
        }

        private FormWindowState winState;
        private System.Drawing.Point winLoc;
        private int winWidth;
        private int winHeight;
        private int eHostWidth;
        private int eHostHeight;
        
        private void ToggleFullscreen()
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.WindowState = winState;

                this.Hide();
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.Location = winLoc;
                this.Width = winWidth;
                this.Height = winHeight;

                elementHost.Dock = DockStyle.None;
                elementHost.Location = new System.Drawing.Point(12, 12);
                elementHost.Width = eHostWidth;
                elementHost.Height = eHostHeight;
                elementHost.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
                this.Show();
            }
            else
            {
                winState = this.WindowState;
                winLoc = this.Location;
                winWidth = this.Width;
                winHeight = this.Height;
                eHostWidth = elementHost.Width;
                eHostHeight = elementHost.Height;

                this.Hide();
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;
                this.WindowState = FormWindowState.Normal;
                this.WindowState = FormWindowState.Maximized;
                elementHost.Dock = DockStyle.Fill;
                this.Show();
            }
        }

        private void DShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mediaUriElement.MediaUriPlayer.MediaPositionChanged -= MediaUriPlayer_MediaPositionChanged;
            mediaUriElement.Stop();
            mediaUriElement.Close();
            mediaUriElement.Source = null;
            mediaUriElement.VideoSource = null;

            if (mouseClickTimer != null)
                mouseClickTimer.Dispose();

            NativeMethods.SystemSleepManagement.ResotreSleep();
            this.Owner.Show();
            this.Owner.TopMost = (Global.MainCfgAlwaysOnTop == "True") ? true : false;
        }

        private void DShowForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mediaUriElement = null;
            this.Dispose();
            GC.Collect();
        }
    }
}
