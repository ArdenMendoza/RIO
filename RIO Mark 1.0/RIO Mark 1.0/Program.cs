using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RIO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
        static R_Lib.R_Speech R_Speech;

        public static void UISpeak(string msg)
        {
            R_Speech.R_SpeakAsync(msg);
            MessageBox.Show(msg);
        }
    }
}
