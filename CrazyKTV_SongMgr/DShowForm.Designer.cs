namespace CrazyKTV_SongMgr
{
    partial class DShowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DShowForm));
            this.Player_SwithChannel_Button = new System.Windows.Forms.Button();
            this.Player_CurrentChannel_Label = new System.Windows.Forms.Label();
            this.Player_UpdateChannel_Button = new System.Windows.Forms.Button();
            this.Player_CurrentChannelValue_Label = new System.Windows.Forms.Label();
            this.Player_PlayControl_Button = new System.Windows.Forms.Button();
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this.Player_CurrentGainValue_Label = new System.Windows.Forms.Label();
            this.Player_CurrentGain_Label = new System.Windows.Forms.Label();
            this.Player_CurrentVolumeValue_Label = new System.Windows.Forms.Label();
            this.Player_CurrentVolume_Label = new System.Windows.Forms.Label();
            this.Player_VideoSizeValue_Label = new System.Windows.Forms.Label();
            this.Player_VideoSize_Label = new System.Windows.Forms.Label();
            this.Player_ProgressTrackBar = new CrazyKTV_SongMgr.ProgressTrackBar();
            this.SuspendLayout();
            // 
            // Player_SwithChannel_Button
            // 
            this.Player_SwithChannel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Player_SwithChannel_Button.AutoSize = true;
            this.Player_SwithChannel_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_SwithChannel_Button.Location = new System.Drawing.Point(155, 508);
            this.Player_SwithChannel_Button.Margin = new System.Windows.Forms.Padding(12, 1, 12, 3);
            this.Player_SwithChannel_Button.Name = "Player_SwithChannel_Button";
            this.Player_SwithChannel_Button.Size = new System.Drawing.Size(110, 40);
            this.Player_SwithChannel_Button.TabIndex = 2;
            this.Player_SwithChannel_Button.Text = "切換聲道";
            this.Player_SwithChannel_Button.UseVisualStyleBackColor = true;
            this.Player_SwithChannel_Button.Click += new System.EventHandler(this.Player_SwithChannel_Button_Click);
            // 
            // Player_CurrentChannel_Label
            // 
            this.Player_CurrentChannel_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_CurrentChannel_Label.AutoSize = true;
            this.Player_CurrentChannel_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentChannel_Label.Location = new System.Drawing.Point(303, 531);
            this.Player_CurrentChannel_Label.Margin = new System.Windows.Forms.Padding(12, 6, 0, 6);
            this.Player_CurrentChannel_Label.Name = "Player_CurrentChannel_Label";
            this.Player_CurrentChannel_Label.Size = new System.Drawing.Size(82, 21);
            this.Player_CurrentChannel_Label.TabIndex = 3;
            this.Player_CurrentChannel_Label.Text = "目前聲道: ";
            // 
            // Player_UpdateChannel_Button
            // 
            this.Player_UpdateChannel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_UpdateChannel_Button.AutoSize = true;
            this.Player_UpdateChannel_Button.Enabled = false;
            this.Player_UpdateChannel_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_UpdateChannel_Button.Location = new System.Drawing.Point(693, 508);
            this.Player_UpdateChannel_Button.Margin = new System.Windows.Forms.Padding(12, 1, 12, 3);
            this.Player_UpdateChannel_Button.Name = "Player_UpdateChannel_Button";
            this.Player_UpdateChannel_Button.Size = new System.Drawing.Size(110, 40);
            this.Player_UpdateChannel_Button.TabIndex = 4;
            this.Player_UpdateChannel_Button.Text = "設為伴唱";
            this.Player_UpdateChannel_Button.UseVisualStyleBackColor = true;
            this.Player_UpdateChannel_Button.Click += new System.EventHandler(this.Player_UpdateChannel_Button_Click);
            // 
            // Player_CurrentChannelValue_Label
            // 
            this.Player_CurrentChannelValue_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_CurrentChannelValue_Label.AutoSize = true;
            this.Player_CurrentChannelValue_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentChannelValue_Label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Player_CurrentChannelValue_Label.Location = new System.Drawing.Point(385, 531);
            this.Player_CurrentChannelValue_Label.Margin = new System.Windows.Forms.Padding(0, 6, 12, 6);
            this.Player_CurrentChannelValue_Label.Name = "Player_CurrentChannelValue_Label";
            this.Player_CurrentChannelValue_Label.Size = new System.Drawing.Size(74, 21);
            this.Player_CurrentChannelValue_Label.TabIndex = 5;
            this.Player_CurrentChannelValue_Label.Text = "尚無資料";
            // 
            // Player_PlayControl_Button
            // 
            this.Player_PlayControl_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Player_PlayControl_Button.AutoSize = true;
            this.Player_PlayControl_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_PlayControl_Button.Location = new System.Drawing.Point(21, 508);
            this.Player_PlayControl_Button.Margin = new System.Windows.Forms.Padding(12, 1, 12, 3);
            this.Player_PlayControl_Button.Name = "Player_PlayControl_Button";
            this.Player_PlayControl_Button.Size = new System.Drawing.Size(110, 37);
            this.Player_PlayControl_Button.TabIndex = 6;
            this.Player_PlayControl_Button.Text = "暫停播放";
            this.Player_PlayControl_Button.UseVisualStyleBackColor = true;
            this.Player_PlayControl_Button.Click += new System.EventHandler(this.Player_PlayControl_Button_Click);
            // 
            // elementHost
            // 
            this.elementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.elementHost.Location = new System.Drawing.Point(12, 12);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(800, 450);
            this.elementHost.TabIndex = 7;
            this.elementHost.Text = "elementHost";
            this.elementHost.Child = null;
            // 
            // Player_CurrentGainValue_Label
            // 
            this.Player_CurrentGainValue_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_CurrentGainValue_Label.AutoSize = true;
            this.Player_CurrentGainValue_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentGainValue_Label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Player_CurrentGainValue_Label.Location = new System.Drawing.Point(573, 531);
            this.Player_CurrentGainValue_Label.Margin = new System.Windows.Forms.Padding(0, 6, 12, 6);
            this.Player_CurrentGainValue_Label.Name = "Player_CurrentGainValue_Label";
            this.Player_CurrentGainValue_Label.Size = new System.Drawing.Size(74, 21);
            this.Player_CurrentGainValue_Label.TabIndex = 9;
            this.Player_CurrentGainValue_Label.Text = "尚無資料";
            // 
            // Player_CurrentGain_Label
            // 
            this.Player_CurrentGain_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_CurrentGain_Label.AutoSize = true;
            this.Player_CurrentGain_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentGain_Label.Location = new System.Drawing.Point(491, 531);
            this.Player_CurrentGain_Label.Margin = new System.Windows.Forms.Padding(12, 6, 0, 6);
            this.Player_CurrentGain_Label.Name = "Player_CurrentGain_Label";
            this.Player_CurrentGain_Label.Size = new System.Drawing.Size(82, 21);
            this.Player_CurrentGain_Label.TabIndex = 8;
            this.Player_CurrentGain_Label.Text = "目前增益: ";
            // 
            // Player_CurrentVolumeValue_Label
            // 
            this.Player_CurrentVolumeValue_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_CurrentVolumeValue_Label.AutoSize = true;
            this.Player_CurrentVolumeValue_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentVolumeValue_Label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Player_CurrentVolumeValue_Label.Location = new System.Drawing.Point(573, 506);
            this.Player_CurrentVolumeValue_Label.Margin = new System.Windows.Forms.Padding(0, 9, 12, 6);
            this.Player_CurrentVolumeValue_Label.Name = "Player_CurrentVolumeValue_Label";
            this.Player_CurrentVolumeValue_Label.Size = new System.Drawing.Size(74, 21);
            this.Player_CurrentVolumeValue_Label.TabIndex = 13;
            this.Player_CurrentVolumeValue_Label.Text = "尚無資料";
            // 
            // Player_CurrentVolume_Label
            // 
            this.Player_CurrentVolume_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_CurrentVolume_Label.AutoSize = true;
            this.Player_CurrentVolume_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentVolume_Label.Location = new System.Drawing.Point(491, 506);
            this.Player_CurrentVolume_Label.Margin = new System.Windows.Forms.Padding(12, 9, 0, 6);
            this.Player_CurrentVolume_Label.Name = "Player_CurrentVolume_Label";
            this.Player_CurrentVolume_Label.Size = new System.Drawing.Size(82, 21);
            this.Player_CurrentVolume_Label.TabIndex = 12;
            this.Player_CurrentVolume_Label.Text = "目前音量: ";
            // 
            // Player_VideoSizeValue_Label
            // 
            this.Player_VideoSizeValue_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_VideoSizeValue_Label.AutoSize = true;
            this.Player_VideoSizeValue_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_VideoSizeValue_Label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Player_VideoSizeValue_Label.Location = new System.Drawing.Point(385, 506);
            this.Player_VideoSizeValue_Label.Margin = new System.Windows.Forms.Padding(0, 9, 12, 6);
            this.Player_VideoSizeValue_Label.Name = "Player_VideoSizeValue_Label";
            this.Player_VideoSizeValue_Label.Size = new System.Drawing.Size(74, 21);
            this.Player_VideoSizeValue_Label.TabIndex = 11;
            this.Player_VideoSizeValue_Label.Text = "尚無資料";
            // 
            // Player_VideoSize_Label
            // 
            this.Player_VideoSize_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_VideoSize_Label.AutoSize = true;
            this.Player_VideoSize_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_VideoSize_Label.Location = new System.Drawing.Point(303, 506);
            this.Player_VideoSize_Label.Margin = new System.Windows.Forms.Padding(12, 9, 0, 6);
            this.Player_VideoSize_Label.Name = "Player_VideoSize_Label";
            this.Player_VideoSize_Label.Size = new System.Drawing.Size(82, 21);
            this.Player_VideoSize_Label.TabIndex = 10;
            this.Player_VideoSize_Label.Text = "畫面大小: ";
            // 
            // Player_ProgressTrackBar
            // 
            this.Player_ProgressTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Player_ProgressTrackBar.Location = new System.Drawing.Point(12, 462);
            this.Player_ProgressTrackBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Player_ProgressTrackBar.Maximum = 100;
            this.Player_ProgressTrackBar.Minimum = 0;
            this.Player_ProgressTrackBar.Name = "Player_ProgressTrackBar";
            this.Player_ProgressTrackBar.ProgressBarValue = 0;
            this.Player_ProgressTrackBar.Size = new System.Drawing.Size(800, 32);
            this.Player_ProgressTrackBar.TabIndex = 1;
            this.Player_ProgressTrackBar.TrackBarValue = 100;
            this.Player_ProgressTrackBar.Click += new System.EventHandler(this.Player_ProgressTrackBar_Click);
            // 
            // DShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(824, 561);
            this.Controls.Add(this.elementHost);
            this.Controls.Add(this.Player_CurrentVolumeValue_Label);
            this.Controls.Add(this.Player_CurrentVolume_Label);
            this.Controls.Add(this.Player_VideoSizeValue_Label);
            this.Controls.Add(this.Player_VideoSize_Label);
            this.Controls.Add(this.Player_CurrentGainValue_Label);
            this.Controls.Add(this.Player_CurrentGain_Label);
            this.Controls.Add(this.Player_PlayControl_Button);
            this.Controls.Add(this.Player_CurrentChannelValue_Label);
            this.Controls.Add(this.Player_UpdateChannel_Button);
            this.Controls.Add(this.Player_CurrentChannel_Label);
            this.Controls.Add(this.Player_SwithChannel_Button);
            this.Controls.Add(this.Player_ProgressTrackBar);
            this.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(840, 600);
            this.Name = "DShowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DShowForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DShowForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DShowForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Player_SwithChannel_Button;
        private System.Windows.Forms.Label Player_CurrentChannel_Label;
        private System.Windows.Forms.Button Player_UpdateChannel_Button;
        private System.Windows.Forms.Label Player_CurrentChannelValue_Label;
        private System.Windows.Forms.Button Player_PlayControl_Button;
        private System.Windows.Forms.Integration.ElementHost elementHost;
        private ProgressTrackBar Player_ProgressTrackBar;
        private System.Windows.Forms.Label Player_CurrentGainValue_Label;
        private System.Windows.Forms.Label Player_CurrentGain_Label;
        private System.Windows.Forms.Label Player_CurrentVolumeValue_Label;
        private System.Windows.Forms.Label Player_CurrentVolume_Label;
        private System.Windows.Forms.Label Player_VideoSizeValue_Label;
        private System.Windows.Forms.Label Player_VideoSize_Label;
    }
}