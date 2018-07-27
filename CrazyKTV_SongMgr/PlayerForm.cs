using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using Vlc.DotNet.Forms;
using System.IO;
using Vlc.DotNet.Core;

namespace CrazyKTV_SongMgr
{
    public partial class PlayerForm : Form
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
        bool CloseForm = false;
        List<TrackDescription> AudioTracks = new List<TrackDescription>();

        public PlayerForm()
        {
            InitializeComponent();
        }

        public PlayerForm(Form ParentForm, List<string> PlayerSongInfoList)
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

            Player_VlcControl.BeginInit();

            var options = new string[]
            {
                "-I",
                "dumy",
                "--ignore-config",
                "--no-osd",
                "--disable-screensaver",
                "--deinterlace=1",
                "--deinterlace-mode=yadif",
                "--codec=h264,avcodec,all",
                "--demux=avformat,any"
            };

            Player_VlcControl.VlcLibDirectoryNeeded += new System.EventHandler<VlcLibDirectoryNeededEventArgs>(Events_OnVlcControlNeedLibDirectory);
            Player_VlcControl.PositionChanged += new EventHandler<VlcMediaPlayerPositionChangedEventArgs>(Events_PlayerPositionChanged);
            Player_VlcControl.EncounteredError += new EventHandler<VlcMediaPlayerEncounteredErrorEventArgs>(Events_PlayerEncounteredError);
            Player_VlcControl.EndReached += new EventHandler<VlcMediaPlayerEndReachedEventArgs>(Events_PlayerEndReached);
            Player_VlcControl.Stopped += new EventHandler<VlcMediaPlayerStoppedEventArgs>(Events_PlayerStopped);
            Player_VlcControl.VlcMediaplayerOptions = options;
            Player_VlcControl.EndInit();

            Player_ProgressTrackBar.ProgressBarValue = 0;
            Player_ProgressTrackBar.TrackBarValue = 0;

            int GainVolume = 0;
            if (SongReplayGain != "" && SongMeanVolume != "")
            {
                int basevolume = Convert.ToInt32(Global.SongMaintenanceReplayGainVolume);
                GainVolume = 100 - basevolume;
                List<int> maxvolumelist = new List<int>() { -18, -17, -16, -15, -14, -13, -12, -11, -10, -9 };
                int maxvolume = maxvolumelist[Convert.ToInt32(Global.SongMaintenanceMaxVolume) - 1];
                maxvolumelist.Clear();
                maxvolumelist = null;

                double GainDB = Convert.ToDouble(SongReplayGain);
                double MeanDB = Convert.ToDouble(SongMeanVolume);
                if (GainDB * -1 > 0)
                {
                    SongVolume = Convert.ToInt32(basevolume * Math.Pow(10, (GainDB * -1) / 20)).ToString();
                }
                else
                {
                    if (MeanDB > maxvolume)
                    {
                        SongVolume = Convert.ToInt32(basevolume * Math.Pow(10, (maxvolume - MeanDB) / 20)).ToString();
                    }
                }
            }

            NativeMethods.SystemSleepManagement.PreventSleep(true);
            Player_VlcControl.SetMedia(new FileInfo(SongFilePath));
            Player_VlcControl.Audio.Volume = Convert.ToInt32(SongVolume) + GainVolume;
            Player_VlcControl.Play();
        }

        // Player OnVlcControlNeedLibDirectory Events
        private void Events_OnVlcControlNeedLibDirectory(object sender, VlcLibDirectoryNeededEventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SongMgr\Libs\libvlc.dll") && File.Exists(Application.StartupPath + @"\SongMgr\Libs\libvlccore.dll"))
            {
                e.VlcLibDirectory = new DirectoryInfo(Application.StartupPath + @"\SongMgr\Libs");
            }
            else
            {
                e.VlcLibDirectory = new DirectoryInfo(Path.Combine(Application.StartupPath, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            }
        }

        // Player PositionChanged Events
        void Events_PlayerPositionChanged(object sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (e.NewPosition < 100 && Convert.ToInt32(e.NewPosition * 100) - Player_ProgressTrackBar.ProgressBarValue > 1)
            {
                Player_ProgressTrackBar.BeginInvokeIfRequired(pbar => pbar.ProgressBarValue = (int)(e.NewPosition * 100));
                Player_ProgressTrackBar.BeginInvokeIfRequired(pbar => pbar.TrackBarValue = (int)(e.NewPosition * 100));
                NativeMethods.SystemSleepManagement.ResetSleepTimer(true);
            }

            if (AudioTracks != null && AudioTracks.Count == 0)
            {
                if (Player_VlcControl.Audio.Tracks.Count > 1)
                {
                    string ChannelValue = string.Empty;
                    float pos = Player_VlcControl.Position;

                    foreach (TrackDescription tdesc in Player_VlcControl.Audio.Tracks.All)
                    {
                        AudioTracks.Add(tdesc);
                    }
                    
                    if (AudioTracks.Count > 2)
                    {
                        switch (SongTrack)
                        {
                            case "1":
                                if (Player_VlcControl.Audio.Tracks.Current.ID != AudioTracks[1].ID) Player_VlcControl.Audio.Tracks.Current = AudioTracks[1];
                                ChannelValue = "1";
                                break;
                            case "2":
                                if (Player_VlcControl.Audio.Tracks.Current.ID != AudioTracks[2].ID) Player_VlcControl.Audio.Tracks.Current = AudioTracks[2];
                                ChannelValue = "2";
                                break;
                        }
                    }
                    else
                    {
                        switch (SongTrack)
                        {
                            case "1":
                                if (Player_VlcControl.Audio.Channel != 3) Player_VlcControl.Audio.Channel = 3;
                                ChannelValue = "1";
                                break;
                            case "2":
                                if (Player_VlcControl.Audio.Channel != 4) Player_VlcControl.Audio.Channel = 4;
                                ChannelValue = "2";
                                break;
                        }
                    }
                    Player_VlcControl.Position = pos;
                    Player_CurrentChannelValue_Label.BeginInvokeIfRequired(lbl => lbl.Text = (ChannelValue == SongTrack) ? "伴唱" : "人聲");
                }
            }
        }

        // Player PlayerEncounteredError Events
        private void Events_PlayerEncounteredError(object sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            Player_VlcControl.PositionChanged -= Events_PlayerPositionChanged;
            Task.Factory.StartNew(() => Player_VlcControl.Stop());
        }

        // Player PlayerEndReached Events
        private void Events_PlayerEndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            Player_VlcControl.PositionChanged -= Events_PlayerPositionChanged;
            Task.Factory.StartNew(() => Player_VlcControl.Stop());
        }

        // Player PlayerStopped Events
        private void Events_PlayerStopped(object sender, VlcMediaPlayerStoppedEventArgs e)
        {
            Player_VlcControl.PositionChanged -= Events_PlayerPositionChanged;
            if (!CloseForm) Task.Factory.StartNew(() => ControlExtensions.BeginInvokeIfRequired(this, f => f.Close()));
        }

        private void Player_ProgressTrackBar_Click(object sender, EventArgs e)
        {
            Player_VlcControl.PositionChanged -= Events_PlayerPositionChanged;
            Player_VlcControl.Position = (float)Player_ProgressTrackBar.TrackBarValue / 100;
            Player_ProgressTrackBar.ProgressBarValue = Player_ProgressTrackBar.TrackBarValue;
            Player_VlcControl.PositionChanged += new EventHandler<VlcMediaPlayerPositionChangedEventArgs>(Events_PlayerPositionChanged);
        }

        private void Player_SwithChannel_Button_Click(object sender, EventArgs e)
        {
            string ChannelValue = string.Empty;
            float pos = Player_VlcControl.Position;

            if (AudioTracks.Count > 2)
            {
                if (Player_VlcControl.Audio.Tracks.Current.ID == AudioTracks[1].ID)
                {
                    Player_VlcControl.Audio.Tracks.Current = AudioTracks[2];
                    ChannelValue = "2";
                    UpdateSongTrack = "2";
                }
                else
                {
                    Player_VlcControl.Audio.Tracks.Current = AudioTracks[1];
                    ChannelValue = "1";
                    UpdateSongTrack = "1";
                }
            }
            else
            {
                if (Player_VlcControl.Audio.Channel == 3)
                {
                    Player_VlcControl.Audio.Channel = 4;
                    ChannelValue = "2";
                    UpdateSongTrack = "2";
                }
                else
                {
                    Player_VlcControl.Audio.Channel = 3;
                    ChannelValue = "1";
                    UpdateSongTrack = "1";
                }
            }
            Player_VlcControl.Position = pos;
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
                    Player_VlcControl.Pause();
                    ((Button)sender).Text = "繼續播放";
                    break;
                case "繼續播放":
                    Player_VlcControl.Play();
                    ((Button)sender).Text = "暫停播放";
                    break;
            }
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player_VlcControl.PositionChanged -= Events_PlayerPositionChanged;
            if (Player_VlcControl.State != Vlc.DotNet.Core.Interops.Signatures.MediaStates.Stopped)
            {
                CloseForm = true;
                Task.Factory.StartNew(() => Player_VlcControl.Stop());
            }
            AudioTracks.Clear();
            AudioTracks = null;
            NativeMethods.SystemSleepManagement.ResotreSleep();
            this.Owner.Show();
        }
    }
}
