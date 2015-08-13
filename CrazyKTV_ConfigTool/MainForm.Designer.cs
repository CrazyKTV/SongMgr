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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.CrazyktvTheme_TabPage = new System.Windows.Forms.TabPage();
            this.CrazyktvTheme_MainScreen_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_D3DButton_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_ScreenDpi_Label = new System.Windows.Forms.Label();
            this.CrazyktvTheme_AutoScreen_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_ScreenDpi_ComboBox = new System.Windows.Forms.ComboBox();
            this.CrazyktvTheme_WinState_CheckBox = new System.Windows.Forms.CheckBox();
            this.CrazyktvTheme_ListView = new System.Windows.Forms.ListView();
            this.CrazyktvTheme_ApplyTheme_Button = new System.Windows.Forms.Button();
            this.CrazyktvTheme_PictureBox = new System.Windows.Forms.PictureBox();
            this.MainTabControl.SuspendLayout();
            this.CrazyktvTheme_TabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CrazyktvTheme_PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.CrazyktvTheme_TabPage);
            resources.ApplyResources(this.MainTabControl, "MainTabControl");
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
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
            resources.ApplyResources(this.CrazyktvTheme_TabPage, "CrazyktvTheme_TabPage");
            this.CrazyktvTheme_TabPage.Name = "CrazyktvTheme_TabPage";
            this.CrazyktvTheme_TabPage.UseVisualStyleBackColor = true;
            // 
            // CrazyktvTheme_MainScreen_CheckBox
            // 
            resources.ApplyResources(this.CrazyktvTheme_MainScreen_CheckBox, "CrazyktvTheme_MainScreen_CheckBox");
            this.CrazyktvTheme_MainScreen_CheckBox.Checked = true;
            this.CrazyktvTheme_MainScreen_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CrazyktvTheme_MainScreen_CheckBox.Name = "CrazyktvTheme_MainScreen_CheckBox";
            this.CrazyktvTheme_MainScreen_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_MainScreen_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_MainScreen_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_D3DButton_CheckBox
            // 
            resources.ApplyResources(this.CrazyktvTheme_D3DButton_CheckBox, "CrazyktvTheme_D3DButton_CheckBox");
            this.CrazyktvTheme_D3DButton_CheckBox.Checked = true;
            this.CrazyktvTheme_D3DButton_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CrazyktvTheme_D3DButton_CheckBox.Name = "CrazyktvTheme_D3DButton_CheckBox";
            this.CrazyktvTheme_D3DButton_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_D3DButton_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_D3DButton_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_ScreenDpi_Label
            // 
            resources.ApplyResources(this.CrazyktvTheme_ScreenDpi_Label, "CrazyktvTheme_ScreenDpi_Label");
            this.CrazyktvTheme_ScreenDpi_Label.Name = "CrazyktvTheme_ScreenDpi_Label";
            // 
            // CrazyktvTheme_AutoScreen_CheckBox
            // 
            resources.ApplyResources(this.CrazyktvTheme_AutoScreen_CheckBox, "CrazyktvTheme_AutoScreen_CheckBox");
            this.CrazyktvTheme_AutoScreen_CheckBox.Name = "CrazyktvTheme_AutoScreen_CheckBox";
            this.CrazyktvTheme_AutoScreen_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_AutoScreen_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_AutoScreen_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_ScreenDpi_ComboBox
            // 
            resources.ApplyResources(this.CrazyktvTheme_ScreenDpi_ComboBox, "CrazyktvTheme_ScreenDpi_ComboBox");
            this.CrazyktvTheme_ScreenDpi_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CrazyktvTheme_ScreenDpi_ComboBox.FormattingEnabled = true;
            this.CrazyktvTheme_ScreenDpi_ComboBox.Items.AddRange(new object[] {
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items1"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items2"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items3"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items4"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items5"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items6"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items7"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items8"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items9"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items10"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items11"),
            resources.GetString("CrazyktvTheme_ScreenDpi_ComboBox.Items12")});
            this.CrazyktvTheme_ScreenDpi_ComboBox.Name = "CrazyktvTheme_ScreenDpi_ComboBox";
            this.CrazyktvTheme_ScreenDpi_ComboBox.SelectedIndexChanged += new System.EventHandler(this.CrazyktvTheme_ScreenDpi_ComboBox_SelectedIndexChanged);
            // 
            // CrazyktvTheme_WinState_CheckBox
            // 
            resources.ApplyResources(this.CrazyktvTheme_WinState_CheckBox, "CrazyktvTheme_WinState_CheckBox");
            this.CrazyktvTheme_WinState_CheckBox.Name = "CrazyktvTheme_WinState_CheckBox";
            this.CrazyktvTheme_WinState_CheckBox.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_WinState_CheckBox.CheckedChanged += new System.EventHandler(this.CrazyktvTheme_WinState_CheckBox_CheckedChanged);
            // 
            // CrazyktvTheme_ListView
            // 
            resources.ApplyResources(this.CrazyktvTheme_ListView, "CrazyktvTheme_ListView");
            this.CrazyktvTheme_ListView.FullRowSelect = true;
            this.CrazyktvTheme_ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.CrazyktvTheme_ListView.MultiSelect = false;
            this.CrazyktvTheme_ListView.Name = "CrazyktvTheme_ListView";
            this.CrazyktvTheme_ListView.Scrollable = false;
            this.CrazyktvTheme_ListView.UseCompatibleStateImageBehavior = false;
            this.CrazyktvTheme_ListView.View = System.Windows.Forms.View.Details;
            this.CrazyktvTheme_ListView.SelectedIndexChanged += new System.EventHandler(this.CrazyktvTheme_ListView_SelectedIndexChanged);
            // 
            // CrazyktvTheme_ApplyTheme_Button
            // 
            resources.ApplyResources(this.CrazyktvTheme_ApplyTheme_Button, "CrazyktvTheme_ApplyTheme_Button");
            this.CrazyktvTheme_ApplyTheme_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CrazyktvTheme_ApplyTheme_Button.Name = "CrazyktvTheme_ApplyTheme_Button";
            this.CrazyktvTheme_ApplyTheme_Button.UseVisualStyleBackColor = true;
            this.CrazyktvTheme_ApplyTheme_Button.Click += new System.EventHandler(this.CrazyktvTheme_ApplyTheme_Button_Click);
            // 
            // CrazyktvTheme_PictureBox
            // 
            resources.ApplyResources(this.CrazyktvTheme_PictureBox, "CrazyktvTheme_PictureBox");
            this.CrazyktvTheme_PictureBox.BackColor = System.Drawing.Color.DarkGray;
            this.CrazyktvTheme_PictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrazyktvTheme_PictureBox.Image = global::CrazyKTV_ConfigTool.Properties.Resources.tbasic;
            this.CrazyktvTheme_PictureBox.Name = "CrazyktvTheme_PictureBox";
            this.CrazyktvTheme_PictureBox.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.MainTabControl);
            this.Name = "MainForm";
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

