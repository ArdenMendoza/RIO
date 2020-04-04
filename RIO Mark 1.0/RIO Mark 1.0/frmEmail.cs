using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIO
{
    public partial class frmEmail : Form
    {
        public frmEmail()
        {
            InitializeComponent();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }


        int desLeftPos;
        int desTopPos;

        int visibilityCounter;
        private void frmEmail_Load(object sender, EventArgs e)
        {
            //this.BackColor = Color.FromArgb(0,25,30);
            desLeftPos = SystemInformation.WorkingArea.Width - this.Width + 20;
            desTopPos  = SystemInformation.WorkingArea.Height - this.Height;
            this.Left = SystemInformation.WorkingArea.Width + 5;
            this.Top = desTopPos;

        }

        private void tmrSlide_Tick(object sender, EventArgs e)
        {
            this.Left = this.Left - 20;
            if (this.Left < desLeftPos) { tmrSlide.Enabled = false; tmrFade.Enabled = true; }
        }

        private void tmrFade_Tick(object sender, EventArgs e)
        {
            int MouseX = System.Windows.Forms.Cursor.Position.X;
            int MouseY = System.Windows.Forms.Cursor.Position.Y;
            int MouseXOut = this.Left + this.Width;
            int MouseYOut = this.Top + this.Height;
            //lblMsg.Text = "MouseX: " + MouseX + "MouseY: " + MouseY;
            if (MouseX > this.Left && MouseY > this.Top && MouseX<MouseXOut && MouseY<MouseYOut)
            {
                this.Opacity = 0.9;
                visibilityCounter = 0;
            }
            else { formFade(); }

        }

        void formFade()
        {
            visibilityCounter++;
            if (visibilityCounter >= 20)
            {
                this.Opacity = this.Opacity - 0.02;
                if (this.Opacity < 0.1)
                {
                    this.Close();
                }
            }
        }

        private void frmEmail_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
        
    }
}
