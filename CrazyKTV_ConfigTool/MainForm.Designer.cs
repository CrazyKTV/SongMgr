namespace CrazyKTV_ConfigTool
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.CrazyktvTheme_TabPage = new System.Windows.Forms.TabPage();
            this.CrazyktvTheme_MainScreen_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_D3DButton_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_ScreenDpi_Label = new System.Windows.Forms.Label();
            this.CrazyktvTheme_AutoScreen_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_ScreenDpi_ComboBox = new System.Windows.Forms.ComboBox();
            this.CrazyktvTheme_WinState_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_PictureBox = new System.Windows.Forms.PictureBox();
            this.CrazyktvTheme_ListView = new System.Windows.Forms.ListView();
            this.CrazyktvTheme_ApplyTheme_Button = new System.Windows.Forms.Button();
            this.MainTabControl.SuspendLayout();
            this.CrazyktvTheme_TabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CrazyktvTheme_PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.CrazyktvTheme_TabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(1006, 722);
            this.MainTabControl.TabIndex = 1;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            // 
            // CrazyktvTheme_TabPage
            // 
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_MainScreen_CheckBox);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_D3DButton_CheckBox);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_ScreenDpi_Label);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_AutoScreen_CheckBox);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_ScreenDpi_ComboBox);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_WinState_CheckBox);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_PictureBox);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_ListView);
            this.CrazyktvTheme_TabPage.Controls.Add(this.CrazyktvTheme_ApplyTheme_Button);
            this.CrazyktvTheme_TabPage.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CrazyktvTheme_TabPage.Location = new System.Drawing.Point(4, 34);
            this.CrazyktvTheme_TabPage.Margin = new System.Windows.Forms.Padding(0);
            this.CrazyktvTheme_TabPage.Name = "CrazyktvTheme_TabPage";
            this.CrazyktvTheme_TabPage.Padding = new System.Windows.Forms.Padding(20);
            this.CrazyktvTheme_TabPage.Size = new System.Drawing.Size(998, 684);
            this.CrazyktvTheme_TabPage.TabIndex = 0;
            this.CrazyktvTheme_TabPage.Text = "佈景主題";
            this.CrazyktvTheme_TabPage.UseVisualStyleBackColor = true;
            // 
            // CrazyktvTheme_MainScreen_CheckBox
            // 
            this.CrazyktvTheme_MainScreen_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_MainScreen_CheckBox.AutoSize = true;
            this.CrazyktvTheme_MainScreen_CheckBox.Checked = true;
            this.CrazyktvTheme_MainScreen_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CrazyktvTheme_MainScreen_CheckBox.Location = new System.Drawing.Point(600, 560);
            this.CrazyktvTheme_MainScreen_CheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_MainScreen_CheckBox.Name = "CrazyktvTheme_MainScreen_CheckBox";
            this.CrazyktvTheme_MainScreen_CheckBox.Size = new System.Drawing.Size(185, 26);
            this.CrazyktvTheme_MainScreen_CheckBox.TabIndex = 7;
            this.CrazyktvTheme_MainScreen_CheckBox.Text = "顯示點歌台在主螢幕";
            this.CrazyktvTheme_MainScreen_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_MainScreen_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_MainScreen_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_D3DButton_CheckBox
            // 
            this.CrazyktvTheme_D3DButton_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_D3DButton_CheckBox.AutoSize = true;
            this.CrazyktvTheme_D3DButton_CheckBox.Checked = true;
            this.CrazyktvTheme_D3DButton_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CrazyktvTheme_D3DButton_CheckBox.Location = new System.Drawing.Point(600, 523);
            this.CrazyktvTheme_D3DButton_CheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_D3DButton_CheckBox.Name = "CrazyktvTheme_D3DButton_CheckBox";
            this.CrazyktvTheme_D3DButton_CheckBox.Size = new System.Drawing.Size(165, 26);
            this.CrazyktvTheme_D3DButton_CheckBox.TabIndex = 6;
            this.CrazyktvTheme_D3DButton_CheckBox.Text = "使用 3D 圖檔按鈕";
            this.CrazyktvTheme_D3DButton_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_D3DButton_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_D3DButton_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_ScreenDpi_Label
            // 
            this.CrazyktvTheme_ScreenDpi_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_ScreenDpi_Label.AutoSize = true;
            this.CrazyktvTheme_ScreenDpi_Label.Location = new System.Drawing.Point(335, 561);
            this.CrazyktvTheme_ScreenDpi_Label.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_ScreenDpi_Label.Name = "CrazyktvTheme_ScreenDpi_Label";
            this.CrazyktvTheme_ScreenDpi_Label.Size = new System.Drawing.Size(163, 22);
            this.CrazyktvTheme_ScreenDpi_Label.TabIndex = 3;
            this.CrazyktvTheme_ScreenDpi_Label.Text = "自訂點歌台螢幕大小";
            // 
            // CrazyktvTheme_AutoScreen_CheckBox
            // 
            this.CrazyktvTheme_AutoScreen_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_AutoScreen_CheckBox.AutoSize = true;
            this.CrazyktvTheme_AutoScreen_CheckBox.Location = new System.Drawing.Point(339, 523);
            this.CrazyktvTheme_AutoScreen_CheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_AutoScreen_CheckBox.Name = "CrazyktvTheme_AutoScreen_CheckBox";
            this.CrazyktvTheme_AutoScreen_CheckBox.Size = new System.Drawing.Size(219, 26);
            this.CrazyktvTheme_AutoScreen_CheckBox.TabIndex = 2;
            this.CrazyktvTheme_AutoScreen_CheckBox.Text = "自動偵測點歌台螢幕大小";
            this.CrazyktvTheme_AutoScreen_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_AutoScreen_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_AutoScreen_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_ScreenDpi_ComboBox
            // 
            this.CrazyktvTheme_ScreenDpi_ComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_ScreenDpi_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CrazyktvTheme_ScreenDpi_ComboBox.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CrazyktvTheme_ScreenDpi_ComboBox.FormattingEnabled = true;
            this.CrazyktvTheme_ScreenDpi_ComboBox.ItemHeight = 22;
            this.CrazyktvTheme_ScreenDpi_ComboBox.Items.AddRange(new object[] {
            "1024 x 768 (4:3)",
            "1920 x 1080 (16:9)",
            "640 x 480 (4:3)",
            "800 x 480 (5:3)",
            "800 x 600 (4:3)",
            "1280 x 720 (16:9)",
            "1280 x 800 (16:10)",
            "1280 x 1024 (5:4)",
            "1366 x 768 (16:9)",
            "1440 x 900 (16:10)",
            "1600 x 1024 (16:10)",
            "1600 x 1200 (4:3)",
            "1680 x 1050 (16:10)"});
            this.CrazyktvTheme_ScreenDpi_ComboBox.Location = new System.Drawing.Point(339, 595);
            this.CrazyktvTheme_ScreenDpi_ComboBox.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_ScreenDpi_ComboBox.Name = "CrazyktvTheme_ScreenDpi_ComboBox";
            this.CrazyktvTheme_ScreenDpi_ComboBox.Size = new System.Drawing.Size(208, 30);
            this.CrazyktvTheme_ScreenDpi_ComboBox.TabIndex = 4;
            this.CrazyktvTheme_ScreenDpi_ComboBox.SelectedIndexChanged += new System.EventHandler(this.CrazyktvTheme_ScreenDpi_ComboBox_SelectedIndexChanged);
            // 
            // CrazyktvTheme_WinState_CheckBox
            // 
            this.CrazyktvTheme_WinState_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_WinState_CheckBox.AutoSize = true;
            this.CrazyktvTheme_WinState_CheckBox.Location = new System.Drawing.Point(339, 637);
            this.CrazyktvTheme_WinState_CheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_WinState_CheckBox.Name = "CrazyktvTheme_WinState_CheckBox";
            this.CrazyktvTheme_WinState_CheckBox.Size = new System.Drawing.Size(219, 26);
            this.CrazyktvTheme_WinState_CheckBox.TabIndex = 5;
            this.CrazyktvTheme_WinState_CheckBox.Text = "使用視窗模式啟動點歌台";
            this.CrazyktvTheme_WinState_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_WinState_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_WinState_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_PictureBox
            // 
            this.CrazyktvTheme_PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrazyktvTheme_PictureBox.BackColor = System.Drawing.Color.DarkGray;
            this.CrazyktvTheme_PictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CrazyktvTheme_PictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrazyktvTheme_PictureBox.Image = global::CrazyKTV_ConfigTool.Properties.Resources.tbasic;
            this.CrazyktvTheme_PictureBox.Location = new System.Drawing.Point(339, 23);
            this.CrazyktvTheme_PictureBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.CrazyktvTheme_PictureBox.MaximumSize = new System.Drawing.Size(1024, 768);
            this.CrazyktvTheme_PictureBox.Name = "CrazyktvTheme_PictureBox";
            this.CrazyktvTheme_PictureBox.Size = new System.Drawing.Size(636, 477);
            this.CrazyktvTheme_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CrazyktvTheme_PictureBox.TabIndex = 3;
            this.CrazyktvTheme_PictureBox.TabStop = false;
            // 
            // CrazyktvTheme_ListView
            // 
            this.CrazyktvTheme_ListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_ListView.FullRowSelect = true;
            this.CrazyktvTheme_ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.CrazyktvTheme_ListView.Location = new System.Drawing.Point(23, 23);
            this.CrazyktvTheme_ListView.MultiSelect = false;
            this.CrazyktvTheme_ListView.Name = "CrazyktvTheme_ListView";
            this.CrazyktvTheme_ListView.Scrollable = false;
            this.CrazyktvTheme_ListView.Size = new System.Drawing.Size(293, 588);
            this.CrazyktvTheme_ListView.TabIndex = 0;
            this.CrazyktvTheme_ListView.UseCompatibleStateImageBehavior = false;
            this.CrazyktvTheme_ListView.View = System.Windows.Forms.View.Details;
            this.CrazyktvTheme_ListView.SelectedIndexChanged += new System.EventHandler(this.CrazyktvTheme_ListView_SelectedIndexChanged);
            // 
            // CrazyktvTheme_ApplyTheme_Button
            // 
            this.CrazyktvTheme_ApplyTheme_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CrazyktvTheme_ApplyTheme_Button.AutoSize = true;
            this.CrazyktvTheme_ApplyTheme_Button.Enabled = false;
            this.CrazyktvTheme_ApplyTheme_Button.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CrazyktvTheme_ApplyTheme_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CrazyktvTheme_ApplyTheme_Button.Location = new System.Drawing.Point(23, 611);
            this.CrazyktvTheme_ApplyTheme_Button.Margin = new System.Windows.Forms.Padding(6);
            this.CrazyktvTheme_ApplyTheme_Button.Name = "CrazyktvTheme_ApplyTheme_Button";
            this.CrazyktvTheme_ApplyTheme_Button.Size = new System.Drawing.Size(294, 48);
            this.CrazyktvTheme_ApplyTheme_Button.TabIndex = 1;
            this.CrazyktvTheme_ApplyTheme_Button.Text = "套用背景";
            this.CrazyktvTheme_ApplyTheme_Button.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_ApplyTheme_Button.Click += new System.EventHandler(this.CrazyktvTheme_ApplyTheme_Button_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1006, 722);
            this.Controls.Add(this.MainTabControl);
            this.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "MainForm";
            this.Text = "CrazyKTV 設定工具 v1.0.0";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.MainTabControl.ResumeLayout(false);
            this.CrazyktvTheme_TabPage.ResumeLayout(false);
            this.CrazyktvTheme_TabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CrazyktvTheme_PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage CrazyktvTheme_TabPage;
        private System.Windows.Forms.CheckBox CrazyktvTheme_MainScreen_CheckBox;
        private System.Windows.Forms.CheckBox CrazyktvTheme_D3DButton_CheckBox;
        private System.Windows.Forms.Label CrazyktvTheme_ScreenDpi_Label;
        private System.Windows.Forms.CheckBox CrazyktvTheme_AutoScreen_CheckBox;
        private System.Windows.Forms.ComboBox CrazyktvTheme_ScreenDpi_ComboBox;
        private System.Windows.Forms.CheckBox CrazyktvTheme_WinState_CheckBox;
        private System.Windows.Forms.PictureBox CrazyktvTheme_PictureBox;
        private System.Windows.Forms.ListView CrazyktvTheme_ListView;
        private System.Windows.Forms.Button CrazyktvTheme_ApplyTheme_Button;
    }
}

