using System;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class ProgressTrackBar : UserControl
    {
        public ProgressTrackBar()
        {
            InitializeComponent();
        }

        public EventHandler TrackBarValueChanged;
        public EventHandler ProgressBarValueChanged;

        private int _Maximum = 100;
        private int _Minimum = 0;
        private int _TrackBarValue = 100;
        private int _ProgressBarValue = 0;
        private bool _Moving = false;

        public int Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                _Maximum = value;
            }
        }

        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                _Minimum = value;
            }
        }

        public int TrackBarValue
        {
            get
            {
                return _TrackBarValue;
            }
            set
            {
                _TrackBarValue = value;
                MoveThumb();
                TrackBarValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int ProgressBarValue
        {
            get
            {
                return _ProgressBarValue;
            }
            set
            {
                _ProgressBarValue = value;
                ProgressBarValueChanged?.Invoke(this, EventArgs.Empty);
                float fractionMoved = (float)(_ProgressBarValue - _Minimum) /
                    (float)(_TrackBarValue - _Minimum); // 0 to 1
                pbProgress.Width = (int)(fractionMoved * (float)btnThumb.Left);

            }
        }

        private void MoveThumb()
        {
            int trackDistance = this.ClientSize.Width - btnThumb.Width;
            float fractionMoved = (float)(_TrackBarValue - _Minimum) /
                (float)(_Maximum - _Minimum); // 0 to 1
            btnThumb.Left = (int)(fractionMoved * (float)trackDistance);
        }

        private void ProgressTrackBar_Resize(object sender, EventArgs e)
        {
            pnlTrack.Width = this.ClientSize.Width;
        }

        private void ProgressTrackBar_MouseDown(object sender, MouseEventArgs e)
        {
            _Moving = true;
            // this line is necessary so that a single click will move the thumb:
            ProgressTrackBar_MouseMove(sender, e);
        }

        private void ProgressTrackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Moving)
            {
                int value = _Minimum + (int)(((float)e.X /
                    (float)(this.ClientSize.Width - btnThumb.Width)) *
                    (float)(_Maximum - _Minimum));
                if (value > _Maximum)
                {
                    value = _Maximum;
                }
                if (value < _Minimum)
                {
                    value = _Minimum;
                }
                this.TrackBarValue = value;
            }
        }

        private void ProgressTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            _Moving = false;
        }
    }
}
