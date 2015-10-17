namespace CrazyKTV_SongMgr
{
    partial class PlayerForm
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

            if (disposing && (m_factory != null))
            {
                m_factory.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerForm));
            this.Player_Panel = new System.Windows.Forms.Panel();
            this.Player_SwithChannel_Button = new System.Windows.Forms.Button();
            this.Player_CurrentChannel_Label = new System.Windows.Forms.Label();
            this.Player_UpdateChannel_Button = new System.Windows.Forms.Button();
            this.Player_CurrentChannelValue_Label = new System.Windows.Forms.Label();
            this.Player_PlayControl_Button = new System.Windows.Forms.Button();
            this.Player_ProgressTrackBar = new CrazyKTV_SongMgr.ProgressTrackBar();
            this.SuspendLayout();
            // 
            // Player_Panel
            // 
            this.Player_Panel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.Player_Panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Player_Panel.Location = new System.Drawing.Point(21, 19);
            this.Player_Panel.Margin = new System.Windows.Forms.Padding(12, 10, 12, 0);
            this.Player_Panel.Name = "Player_Panel";
            this.Player_Panel.Size = new System.Drawing.Size(640, 480);
            this.Player_Panel.TabIndex = 0;
            // 
            // Player_SwithChannel_Button
            // 
            this.Player_SwithChannel_Button.AutoSize = true;
            this.Player_SwithChannel_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_SwithChannel_Button.Location = new System.Drawing.Point(155, 542);
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
            this.Player_CurrentChannel_Label.AutoSize = true;
            this.Player_CurrentChannel_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentChannel_Label.Location = new System.Drawing.Point(289, 550);
            this.Player_CurrentChannel_Label.Margin = new System.Windows.Forms.Padding(12, 9, 6, 10);
            this.Player_CurrentChannel_Label.Name = "Player_CurrentChannel_Label";
            this.Player_CurrentChannel_Label.Size = new System.Drawing.Size(102, 25);
            this.Player_CurrentChannel_Label.TabIndex = 3;
            this.Player_CurrentChannel_Label.Text = "目前聲道: ";
            // 
            // Player_UpdateChannel_Button
            // 
            this.Player_UpdateChannel_Button.AutoSize = true;
            this.Player_UpdateChannel_Button.Enabled = false;
            this.Player_UpdateChannel_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_UpdateChannel_Button.Location = new System.Drawing.Point(551, 542);
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
            this.Player_CurrentChannelValue_Label.AutoSize = true;
            this.Player_CurrentChannelValue_Label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_CurrentChannelValue_Label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Player_CurrentChannelValue_Label.Location = new System.Drawing.Point(397, 550);
            this.Player_CurrentChannelValue_Label.Margin = new System.Windows.Forms.Padding(0, 9, 12, 10);
            this.Player_CurrentChannelValue_Label.Name = "Player_CurrentChannelValue_Label";
            this.Player_CurrentChannelValue_Label.Size = new System.Drawing.Size(92, 25);
            this.Player_CurrentChannelValue_Label.TabIndex = 5;
            this.Player_CurrentChannelValue_Label.Text = "尚無資料";
            // 
            // Player_PlayControl_Button
            // 
            this.Player_PlayControl_Button.AutoSize = true;
            this.Player_PlayControl_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Player_PlayControl_Button.Location = new System.Drawing.Point(21, 542);
            this.Player_PlayControl_Button.Margin = new System.Windows.Forms.Padding(12, 1, 12, 3);
            this.Player_PlayControl_Button.Name = "Player_PlayControl_Button";
            this.Player_PlayControl_Button.Size = new System.Drawing.Size(110, 40);
            this.Player_PlayControl_Button.TabIndex = 6;
            this.Player_PlayControl_Button.Text = "暫停播放";
            this.Player_PlayControl_Button.UseVisualStyleBackColor = true;
            this.Player_PlayControl_Button.Click += new System.EventHandler(this.Player_PlayControl_Button_Click);
            // 
            // Player_ProgressTrackBar
            // 
            this.Player_ProgressTrackBar.Location = new System.Drawing.Point(21, 499);
            this.Player_ProgressTrackBar.Margin = new System.Windows.Forms.Padding(12, 0, 12, 10);
            this.Player_ProgressTrackBar.Maximum = 100;
            this.Player_ProgressTrackBar.Minimum = 0;
            this.Player_ProgressTrackBar.Name = "Player_ProgressTrackBar";
            this.Player_ProgressTrackBar.ProgressBarValue = 0;
            this.Player_ProgressTrackBar.Size = new System.Drawing.Size(640, 32);
            this.Player_ProgressTrackBar.TabIndex = 1;
            this.Player_ProgressTrackBar.TrackBarValue = 100;
            this.Player_ProgressTrackBar.Click += new System.EventHandler(this.Player_ProgressTrackBar_Click);
            // 
            // PlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(682, 594);
            this.Controls.Add(this.Player_PlayControl_Button);
            this.Controls.Add(this.Player_CurrentChannelValue_Label);
            this.Controls.Add(this.Player_UpdateChannel_Button);
            this.Controls.Add(this.Player_CurrentChannel_Label);
            this.Controls.Add(this.Player_SwithChannel_Button);
            this.Controls.Add(this.Player_ProgressTrackBar);
            this.Controls.Add(this.Player_Panel);
            this.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PlayerForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PlayerForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Player_Panel;
        private ProgressTrackBar Player_ProgressTrackBar;
        private System.Windows.Forms.Button Player_SwithChannel_Button;
        private System.Windows.Forms.Label Player_CurrentChannel_Label;
        private System.Windows.Forms.Button Player_UpdateChannel_Button;
        private System.Windows.Forms.Label Player_CurrentChannelValue_Label;
        private System.Windows.Forms.Button Player_PlayControl_Button;
    }
}