namespace CrazyKTV_SongMgr
{
    partial class ProgressTrackBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnThumb = new System.Windows.Forms.Button();
            this.pnlTrack = new System.Windows.Forms.Panel();
            this.pbProgress = new System.Windows.Forms.PictureBox();
            this.pnlTrack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // btnThumb
            // 
            this.btnThumb.Enabled = false;
            this.btnThumb.Location = new System.Drawing.Point(0, 5);
            this.btnThumb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnThumb.Name = "btnThumb";
            this.btnThumb.Size = new System.Drawing.Size(19, 20);
            this.btnThumb.TabIndex = 1;
            this.btnThumb.UseVisualStyleBackColor = true;
            // 
            // pnlTrack
            // 
            this.pnlTrack.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlTrack.Controls.Add(this.pbProgress);
            this.pnlTrack.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlTrack.Enabled = false;
            this.pnlTrack.Location = new System.Drawing.Point(0, 10);
            this.pnlTrack.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlTrack.Name = "pnlTrack";
            this.pnlTrack.Size = new System.Drawing.Size(259, 11);
            this.pnlTrack.TabIndex = 2;
            // 
            // pbProgress
            // 
            this.pbProgress.BackColor = System.Drawing.SystemColors.Highlight;
            this.pbProgress.Location = new System.Drawing.Point(0, 0);
            this.pbProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(0, 20);
            this.pbProgress.TabIndex = 0;
            this.pbProgress.TabStop = false;
            // 
            // ProgressTrackBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnThumb);
            this.Controls.Add(this.pnlTrack);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ProgressTrackBar";
            this.Size = new System.Drawing.Size(264, 32);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProgressTrackBar_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ProgressTrackBar_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ProgressTrackBar_MouseUp);
            this.Resize += new System.EventHandler(this.ProgressTrackBar_Resize);
            this.pnlTrack.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnThumb;
        private System.Windows.Forms.Panel pnlTrack;
        private System.Windows.Forms.PictureBox pbProgress;
    }
}
