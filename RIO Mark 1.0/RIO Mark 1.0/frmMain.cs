using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
//database
using System.Data;

//WebServices
using System.ServiceModel;
using R_Lib.WebServiceConverterLength;
using R_Lib.WebServiceWeather;
using R_Lib.WebServiceSunsertRise;
using R_Lib.WebServicePeriodicTable;
using System.Xml.Linq;
using R_Lib;

namespace RIO
{
    public partial class frmMain : Form
    {
        FormControls R_Forms = new FormControls();

        R_Lib.R_Speech R_Speech;

        #region drag form via picturebox
        bool FormDragging = false;
        private void img_rio_MouseDown(object sender, MouseEventArgs e)
        {
            R_Forms.mouseStartX= MousePosition.X;
            R_Forms.mouseStartY = MousePosition.Y;
            R_Forms.formStartX = this.Location.X;
            R_Forms.formStartY = this.Location.Y;
            FormDragging = true;
        }
        private void img_rio_MouseMove(object sender, MouseEventArgs e)
        {
            if (FormDragging)
            {
                this.Location = new Point(
                R_Forms.formStartX + MousePosition.X - R_Forms.mouseStartX,
                R_Forms.formStartY + MousePosition.Y - R_Forms.mouseStartY
                );
            }

        }
        private void img_rio_MouseUp(object sender, MouseEventArgs e)
        {
            FormDragging = false;
        }

        #endregion

        public frmMain()
        {
            InitializeComponent();
            R_Speech = new R_Lib.R_Speech();
        }

        public void frmMain_Load(object sender, EventArgs e)
        {
            #region setup db connection

            //System.IO.File.Delete(Application.StartupPath + @"\rioDB.sdf"); //Testing script
            #endregion setup db connection

            #region Load default grammars
            R_Speech.loadSystemVocabs(); // load rio default commands
            R_Speech.R_SpeakAsync("System commands loaded");
            // loadShellGrammar
            ActionResult shellGrammar = R_Speech.loadShellGrammar();
            if (shellGrammar.success)
            {
                if(shellGrammar.msg == "AddCommand")
                {
                    frmCustom cust = new frmCustom();
                    cust.isShown = true;
                    cust.ShowDialog();
                }
            } else
            {
                MessageBox.Show(shellGrammar.msg);
            }

            R_Speech.R_SpeakAsync("User commands loaded");
            //loadDictationGrammar();
            //R_Speech.R_SpeakAsync("Dictation grammars loaded");
            #endregion Load default grammars
        }


        private void rt_Convo_TextChanged(object sender, EventArgs e)
        {
            rt_Convo.AppendText("\r\n");
            rt_Convo.ScrollToCaret();
        }
    }
}
