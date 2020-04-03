namespace rio_Mark_1._0
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.img_rio = new System.Windows.Forms.PictureBox();
            this.notifIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rt_Convo = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.img_rio)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // img_rio
            // 
            this.img_rio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.img_rio.Image = ((System.Drawing.Image)(resources.GetObject("img_rio.Image")));
            this.img_rio.Location = new System.Drawing.Point(12, 12);
            this.img_rio.Name = "img_rio";
            this.img_rio.Size = new System.Drawing.Size(231, 235);
            this.img_rio.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.img_rio.TabIndex = 0;
            this.img_rio.TabStop = false;
            this.img_rio.MouseDown += new System.Windows.Forms.MouseEventHandler(this.img_rio_MouseDown);
            this.img_rio.MouseMove += new System.Windows.Forms.MouseEventHandler(this.img_rio_MouseMove);
            this.img_rio.MouseUp += new System.Windows.Forms.MouseEventHandler(this.img_rio_MouseUp);
            // 
            // notifIcon
            // 
            this.notifIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifIcon.Icon")));
            this.notifIcon.Text = "J.A.R.V.I.S. System";
            this.notifIcon.Visible = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.panel1.Controls.Add(this.rt_Convo);
            this.panel1.Location = new System.Drawing.Point(199, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(382, 235);
            this.panel1.TabIndex = 1;
            // 
            // rt_Convo
            // 
            this.rt_Convo.BackColor = System.Drawing.SystemColors.Highlight;
            this.rt_Convo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rt_Convo.BulletIndent = 1;
            this.rt_Convo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rt_Convo.ForeColor = System.Drawing.Color.White;
            this.rt_Convo.Location = new System.Drawing.Point(48, 17);
            this.rt_Convo.Margin = new System.Windows.Forms.Padding(0);
            this.rt_Convo.Name = "rt_Convo";
            this.rt_Convo.ReadOnly = true;
            this.rt_Convo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rt_Convo.Size = new System.Drawing.Size(326, 196);
            this.rt_Convo.TabIndex = 0;
            this.rt_Convo.TabStop = false;
            this.rt_Convo.Text = "";
            this.rt_Convo.TextChanged += new System.EventHandler(this.rt_Convo_TextChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(593, 260);
            this.Controls.Add(this.img_rio);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMain";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.SteelBlue;
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.img_rio)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox img_rio;
        private System.Windows.Forms.NotifyIcon notifIcon;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.RichTextBox rt_Convo;
    }
}

