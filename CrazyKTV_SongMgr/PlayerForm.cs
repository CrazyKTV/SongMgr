using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using System.Threading.Tasks;

namespace CrazyKTV_SongMgr
{
    public partial class PlayerForm : Form
    {
        IMediaPlayerFactory m_factory;
        IDiskPlayer m_player;
        IMediaFromFile m_media;
        string SongId;
        string SongLang;
        string SongSinger;
        string SongSongName;
        string SongTrack;
        string SongVolume;
        string SongFilePath;
        string dvRowIndex;
        string UpdateSongTrack;
        string UpdateDataGridView;
        List<int> TrackIdList = new List<int>();

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
            SongFilePath = PlayerSongInfoList[6];
            dvRowIndex = PlayerSongInfoList[7];
            UpdateDataGridView = PlayerSongInfoList[8];

            this.Text = "【" + SongLang + "】" + SongSinger + " - " + SongSongName;

            var args = new string[]
            {
                "-I",
                "dumy",
                "--ignore-config",
                "--no-osd",
                "--disable-screensaver",
                "--deinterlace-mode=yadif",
                "--codec=x264,avcodec,all",
                "--demux=avcodec,all"
            };

            m_factory = new MediaPlayerFactory(args, true);
            m_player = m_factory.CreatePlayer<IDiskPlayer>();
            m_player.WindowHandle = Player_Panel.Handle;
            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);

            Player_ProgressTrackBar.ProgressBarValue = 0;
            Player_ProgressTrackBar.TrackBarValue = 0;

            m_media = m_factory.CreateMedia<IMediaFromFile>(SongFilePath);
            m_player.Open(m_media);
            m_media.Parse(true);
            m_player.Volume = Convert.ToInt32(SongVolume);
            m_player.Play();
        }

        // Player PositionChanged Events
        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            if (TrackIdList.Count == 0)
            {
                if (m_player.AudioTrackCount > 1)
                {
                    foreach (Declarations.TrackDescription TrackDesc in m_player.AudioTracksInfo)
                    {
                        TrackIdList.Add(TrackDesc.Id);
                    }

                    if (TrackIdList.Count > 2)
                    {
                        float pos = m_player.Position;
                        switch (SongTrack)
                        {
                            case "1":
                                if (m_player.AudioTrack != TrackIdList[1]) m_player.AudioTrack = TrackIdList[1];
                                break;
                            case "2":
                                if (m_player.AudioTrack != TrackIdList[2]) m_player.AudioTrack = TrackIdList[2];
                                break;
                        }
                        Player_CurrentChannelValue_Label.InvokeIfRequired(lbl => lbl.Text = (TrackIdList.IndexOf(m_player.AudioTrack) == Convert.ToInt32(SongTrack)) ? "伴唱" : "人聲");
                        m_player.Position = pos;
                    }
                    else
                    {
                        string ChannelValue = "";
                        switch (SongTrack)
                        {
                            case "1":
                                if (m_player.Channel != AudioChannelType.Left) m_player.Channel = AudioChannelType.Left;
                                ChannelValue = "1";
                                break;
                            case "2":
                                if (m_player.Channel != AudioChannelType.Right) m_player.Channel = AudioChannelType.Right;
                                ChannelValue = "2";
                                break;
                        }
                        Player_CurrentChannelValue_Label.InvokeIfRequired(lbl => lbl.Text = (ChannelValue == SongTrack) ? "伴唱" : "人聲");
                    }
                }
            }
            this.BeginInvoke((Action)delegate()
            {
                if (e.NewPosition > 0.999) this.Close();
                Player_ProgressTrackBar.InvokeIfRequired(pbar => pbar.ProgressBarValue = (int)(e.NewPosition * 100));
                Player_ProgressTrackBar.InvokeIfRequired(pbar => pbar.TrackBarValue = (int)(e.NewPosition * 100));
            });
        }

        private void Player_ProgressTrackBar_Click(object sender, EventArgs e)
        {
            if (Player_ProgressTrackBar.TrackBarValue >= 99.9)
            {
                m_player.Position = (float)99.9 / 100;
            }
            else
            {
                m_player.Position = (float)Player_ProgressTrackBar.TrackBarValue / 100;
            }
        }

        private void Player_SwithChannel_Button_Click(object sender, EventArgs e)
        {
            if (TrackIdList.Count > 2)
            {
                float pos = m_player.Position;
                if (m_player.AudioTrack == TrackIdList[1])
                {
                    m_player.AudioTrack = TrackIdList[2];
                    UpdateSongTrack = "2";
                }
                else
                {
                    m_player.AudioTrack = TrackIdList[1];
                    UpdateSongTrack = "1";
                }
                Player_CurrentChannelValue_Label.Text = (TrackIdList.IndexOf(m_player.AudioTrack) == Convert.ToInt32(SongTrack)) ? "伴唱" : "人聲";
                m_player.Position = pos;
            }
            else
            {
                string ChannelValue = "";
                if (m_player.Channel == AudioChannelType.Left) 
                {
                    m_player.Channel = AudioChannelType.Right;
                    ChannelValue = "2";
                    UpdateSongTrack = "2";
                }
                else
                {
                    m_player.Channel = AudioChannelType.Left;
                    ChannelValue = "1";
                    UpdateSongTrack = "1";
                }
                Player_CurrentChannelValue_Label.Text = (ChannelValue == SongTrack) ? "伴唱" : "人聲";
            }

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
                    m_player.Pause();
                    ((Button)sender).Text = "繼續播放";
                    break;
                case "繼續播放":
                    m_player.Play();
                    ((Button)sender).Text = "暫停播放";
                    break;
            }
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Task.Factory.StartNew(() => m_player.Stop());
            TrackIdList.Clear();
            this.Owner.Show();
        }
    }
}
