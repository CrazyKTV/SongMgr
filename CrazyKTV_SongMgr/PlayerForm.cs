using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using System.Threading;
using System.Data.OleDb;
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
        string SongFilePath;
        string dvRowIndex;
        string UpdateSongTrack;
        string UpdateDataGridView;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

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
            SongFilePath = PlayerSongInfoList[5];
            dvRowIndex = PlayerSongInfoList[6];
            UpdateDataGridView = PlayerSongInfoList[7];

            this.Text = "【" + SongLang + "】" + SongSinger + " - " + SongSongName;

            m_factory = new MediaPlayerFactory(true);
            m_player = m_factory.CreatePlayer<IDiskPlayer>();
            m_player.WindowHandle = Player_Panel.Handle;

            Player_ProgressTrackBar.ProgressBarValue = 0;
            Player_ProgressTrackBar.TrackBarValue = 0;

            m_media = m_factory.CreateMedia<IMediaFromFile>(SongFilePath);
            m_player.Open(m_media);
            m_media.Parse(true);
            m_player.Mute = true;
            m_player.Play();

            Thread.Sleep(1000);

            bool WaitTrackInfo = false;
            while (!WaitTrackInfo)
            {
                if (m_player.AudioTrackCount > 1)
                {
                    foreach (Declarations.TrackDescription TrackDesc in m_player.AudioTracksInfo)
                    {
                        if (TrackDesc.Id != -1)
                        {
                            WaitTrackInfo = true;
                        }
                    }
                }
            }

            List<int> TrackIdList = new List<int>();
            foreach (Declarations.TrackDescription TrackDesc in m_player.AudioTracksInfo)
            {
                TrackIdList.Add(TrackDesc.Id);
            }

            if (TrackIdList.Count > 2)
            {
                switch (SongTrack)
                {
                    case "1":
                        if (m_player.AudioTrack != TrackIdList[1]) m_player.AudioTrack = TrackIdList[1];
                        break;
                    case "2":
                        if (m_player.AudioTrack != TrackIdList[2]) m_player.AudioTrack = TrackIdList[2];
                        break;
                }
                Player_CurrentChannelValue_Label.Text = (TrackIdList.IndexOf(m_player.AudioTrack) == Convert.ToInt32(SongTrack)) ? "伴唱" : "人聲";
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
                Player_CurrentChannelValue_Label.Text = (ChannelValue == SongTrack) ? "伴唱" : "人聲";
            }

            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);

            m_player.Position = 0;
            m_player.Mute = false;

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 500;
            timer.Start();
        }

        // Player Events
        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            this.BeginInvoke((Action)delegate()
            {
                Player_ProgressTrackBar.ProgressBarValue = (int)(e.NewPosition * 100);
                Player_ProgressTrackBar.TrackBarValue = (int)(e.NewPosition * 100);
            });
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!m_player.IsPlaying && Player_PlayControl_Button.Text == "暫停播放")
            {
                timer.Stop();
                this.Close();
            }
        }

        private void Player_ProgressTrackBar_Click(object sender, EventArgs e)
        {
            if(Player_ProgressTrackBar.TrackBarValue >= 99.5)
            {
                m_player.Position = (float)99.5 / 100;
            }
            else
            {
                m_player.Position = (float)Player_ProgressTrackBar.TrackBarValue / 100;
            }
        }

        private void Player_SwithChannel_Button_Click(object sender, EventArgs e)
        {
            List<int> TrackIdList = new List<int>();
            foreach (Declarations.TrackDescription TrackDesc in m_player.AudioTracksInfo)
            {
                TrackIdList.Add(TrackDesc.Id);
            }

            if (TrackIdList.Count > 2)
            {
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

            if (Player_CurrentChannelValue_Label.Text == "人聲")
            {
                Player_UpdateChannel_Button.Enabled = true;
            }
            else
            {
                Player_UpdateChannel_Button.Enabled = false;
            }
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
            timer.Dispose();
            this.Owner.Show();
        }
    }
}
