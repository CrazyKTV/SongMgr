using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using Vlc.DotNet.Forms;
using System.IO;
using Vlc.DotNet.Core;
using System.Windows.Forms.Integration;
using CrazyKTV_MediaKit.DirectShow.Controls;
using System.Windows;
using System.Threading;

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
        private double mWidth;
        private double mHeight;
        private int eWidth;
        private int eHeight;
        private bool sliderInit;
        private bool sliderDrag;
        private bool sliderMediaChange;

        public DShowForm()
        {
            InitializeComponent();
        }

        public DShowForm(Form ParentForm, List<string> PlayerSongInfoList)
        {
            InitializeComponent();

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
            mediaUriElement.VideoRenderer = CrazyKTV_MediaKit.DirectShow.MediaPlayers.VideoRendererType.VideoMixingRenderer9;
            mediaUriElement.Stretch = System.Windows.Media.Stretch.Fill;


            mediaUriElement.MediaFailed += MediaUriElement_MediaFailed;
            mediaUriElement.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;
            mediaUriElement.MediaEnded += MediaUriElement_MediaEnded;
            mediaUriElement.MouseLeftButtonDown += mediaUriElement_MouseLeftButtonDown;

            mediaUriElement.Source = new Uri(SongFilePath);

            SpinWait.SpinUntil(() => mediaUriElement.GetAudioTrackList().Count > 0);

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
            mediaUriElement.AudioAmplify = 800;
        }

        private void MediaUriElement_MediaFailed(object sender, CrazyKTV_MediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        {
            this.BeginInvokeIfRequired(form => form.Text = e.Message);
        }

        private void MediaUriPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            if (sliderDrag)
                return;

            this.BeginInvoke((Action)delegate ()
            {
                if (!sliderInit)
                {
                    if (mediaUriElement.MediaDuration > 0)
                    {
                        Player_ProgressTrackBar.Maximum = (int)mediaUriElement.MediaDuration * -1;
                        sliderInit = true;
                    }
                }
                if (sliderInit)
                    ChangeSlideValue();
            });
        }

        private void ChangeSlideValue()
        {
            if (sliderDrag)
                return;

            if (sliderInit)
            {
                sliderMediaChange = true;
                double perc = (double)mediaUriElement.MediaPosition / mediaUriElement.MediaDuration;
                Player_ProgressTrackBar.TrackBarValue = (int)(Player_ProgressTrackBar.Maximum * perc);
                Player_ProgressTrackBar.ProgressBarValue = (int)(Player_ProgressTrackBar.Maximum * perc);
                sliderMediaChange = false;
            }
        }

        private void ChangeMediaPosition()
        {
            if (sliderMediaChange || !sliderInit)
                return;

            sliderDrag = true;
            double perc = (double)Player_ProgressTrackBar.TrackBarValue / Player_ProgressTrackBar.Maximum;
            mediaUriElement.MediaPosition = (long)(mediaUriElement.MediaDuration * perc);
            sliderDrag = false;
        }

        private void Player_ProgressTrackBar_Click(object sender, EventArgs e)
        {
            if (sliderMediaChange || !sliderInit)
                return;

            this.BeginInvoke((Action)delegate ()
            {
                ChangeMediaPosition();
            });
        }

        private void MediaUriElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaUriElement.Stop();
            mediaUriElement.MediaPosition = 0;
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
            }
        }

        private void mediaUriElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleFullscreen();
            }
        }

        private void ToggleFullscreen()
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
                this.Player_ProgressTrackBar.Visible = true;
                this.Player_PlayControl_Button.Visible = true;
                this.Player_SwithChannel_Button.Visible = true;
                this.Player_CurrentChannel_Label.Visible = true;
                this.Player_CurrentChannelValue_Label.Visible = true;
                this.Player_UpdateChannel_Button.Visible = true;


            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;
                this.WindowState = FormWindowState.Normal;
                this.WindowState = FormWindowState.Maximized;

                this.Player_ProgressTrackBar.Visible = false;
                this.Player_PlayControl_Button.Visible = false;
                this.Player_SwithChannel_Button.Visible = false;
                this.Player_CurrentChannel_Label.Visible = false;
                this.Player_CurrentChannelValue_Label.Visible = false;
                this.Player_UpdateChannel_Button.Visible = false;
                this.Controls.Remove(Player_ProgressTrackBar);
                this.Controls.Remove(Player_PlayControl_Button);
                this.Controls.Remove(Player_SwithChannel_Button);
                this.Controls.Remove(Player_CurrentChannel_Label);
                this.Controls.Remove(Player_CurrentChannelValue_Label);
                this.Controls.Remove(Player_UpdateChannel_Button);
                elementHost.Location = new System.Drawing.Point(0, 0);
                elementHost.Width = this.Width;
                elementHost.Height = this.Height;


            }
        }

        private void DShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mediaUriElement.Stop();
            mediaUriElement.Close();

            NativeMethods.SystemSleepManagement.ResotreSleep();
            this.Owner.Show();
        }
    }
}
