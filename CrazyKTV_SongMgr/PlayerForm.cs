using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using System.IO;
using System.Threading;
using System.Data.OleDb;

namespace CrazyKTV_SongMgr
{
    public partial class PlayerForm : Form
    {
        IMediaPlayerFactory m_factory;
        IDiskPlayer m_player;
        IMediaFromFile m_media;
        string SongId;
        string SongSinger;
        string SongSongName;
        string SongTrack;
        string SongFilePath;
        string dvRowIndex;
        string UpdateSongTrack;

        public PlayerForm()
        {
            InitializeComponent();
        }

        public PlayerForm(List<string> PlayerSongInfoList)
        {
            InitializeComponent();
            SongId = PlayerSongInfoList[0];
            SongSinger = PlayerSongInfoList[1];
            SongSongName = PlayerSongInfoList[2];
            SongTrack = PlayerSongInfoList[3];
            SongFilePath = PlayerSongInfoList[4];
            dvRowIndex = PlayerSongInfoList[5];

            this.Text = SongSinger + " - " + SongSongName;

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

            Global.TimerStartTime = DateTime.Now;
            Global.TimerEndTime = DateTime.Now;
            while ((long)(Global.TimerEndTime - Global.TimerStartTime).TotalSeconds < 1)
            {
                Global.TimerEndTime = DateTime.Now;
            }

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

            m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);
            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);

            m_player.Position = 0;
            m_player.Mute = false;
            m_player.Play();
        }

        // Player Events
        void Events_PlayerStopped(object sender, EventArgs e)
        {
            this.BeginInvoke((Action)delegate ()
            {
                Player_ProgressTrackBar.ProgressBarValue = 0;
                Player_ProgressTrackBar.TrackBarValue = 0;
            });
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            this.BeginInvoke((Action)delegate ()
            {
                if ((e.NewPosition * 100) > 99.9 || !m_player.IsPlaying)
                {
                    this.Close();
                }
                else
                {
                    Player_ProgressTrackBar.ProgressBarValue = (int)(e.NewPosition * 100);
                    Player_ProgressTrackBar.TrackBarValue = (int)(e.NewPosition * 100);
                }
            });
        }

        private void PlayerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_player.IsPlaying) m_player.Stop();
            this.Owner.Show();
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
            OleDbConnection conn = CommonFunc.OleDbOpenConn(Global.CrazyktvDatabaseFile, "");
            OleDbCommand cmd = new OleDbCommand();
            string sqlColumnStr = "Song_Track = @SongTrack";
            string SongUpdateSqlStr = "update ktv_Song set " + sqlColumnStr + " where Song_Id=@SongId";

            cmd = new OleDbCommand(SongUpdateSqlStr, conn);
            cmd.Parameters.AddWithValue("@SongTrack", UpdateSongTrack);
            cmd.Parameters.AddWithValue("@SongId", SongId);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            conn.Close();
            
            SongTrack = UpdateSongTrack;
            Player_UpdateChannel_Button.Enabled = false;
            Player_CurrentChannelValue_Label.Text = "伴唱";
            Global.PlayerUpdateSongValueList = new List<string>() { dvRowIndex, SongTrack };
        }
    }
}
