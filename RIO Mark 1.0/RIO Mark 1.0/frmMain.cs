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
using System.Speech.Recognition;
using System.ComponentModel;

namespace RIO
{
    public partial class frmMain : Form
    {

        R_Lib.R_Speech R_Speech;

        #region drag form via picturebox
        bool FormDragging = false;
        private void img_rio_MouseDown(object sender, MouseEventArgs e)
        {
            FormControls.mouseStartX= MousePosition.X;
            FormControls.mouseStartY = MousePosition.Y;
            FormControls.formStartX = this.Location.X;
            FormControls.formStartY = this.Location.Y;
            FormDragging = true;
        }
        private void img_rio_MouseMove(object sender, MouseEventArgs e)
        {
            if (FormDragging)
            {
                this.Location = new Point(
                FormControls.formStartX + MousePosition.X - FormControls.mouseStartX,
                FormControls.formStartY + MousePosition.Y - FormControls.mouseStartY
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
            //rt_Convo.DataBindings.Add("Text", R_Speech, "conversation", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        // CONVERSATION

        public void frmMain_Load(object sender, EventArgs e)
        {
            R_Speech = new R_Lib.R_Speech(FormControls.getForm<frmMain>().rt_Convo);

            #region setup db connection

            //System.IO.File.Delete(Application.StartupPath + @"\rioDB.sdf"); //Testing script
            #endregion setup db connection

            R_Speech.loadSystemVocabs(); // load rio default commands
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
            //loadDictationGrammar();
            //R_Speech.R_SpeakAsync("Dictation grammars loaded");

            // load default microphone
            R_Speech.R_recognizer.SetInputToDefaultAudioDevice();
            // sets the program to interpret multiple commands
            R_Speech.R_recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }


        private void rt_Convo_TextChanged(object sender, EventArgs e)
        {
            rt_Convo.AppendText("\r\n");
            rt_Convo.ScrollToCaret();
        }
    }
}
