using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Jarvis_Mark_1._0.Properties;
using System.Drawing;
using System.Diagnostics;
using System.Data.SqlServerCe;
using Jarvis_Mark_1._0;

namespace JarvisSystem_2._0
{
    public partial class frmCustom : Form
    {
        string StartUpPath=Application.StartupPath;
        
        //SqlCeCommand cmd;
        public SqlCeConnection conn;
        public frmCustom()
        {
            InitializeComponent();
            conn = new SqlCeConnection(@"Data Source="+ StartUpPath + @"\DB\JarvisDb.sdf; Password=JarvisDB01");
        }
        
        private void frmCustom_Load(object sender, EventArgs e)
        {
            this.Left = (SystemInformation.WorkingArea.Width/2) - (this.Width / 2);
            this.Top = 0 - this.Height;
            tmrSlide.Enabled = true;

            this.BackColor = Color.FromArgb(0, 80, 100);
            tabPage1.BackColor = Color.FromArgb(0,90,100);
            tabPage3.BackColor = Color.FromArgb(0, 85, 100);

            loadUserCommands();

            //volume increase Decrease Value
            VolVal.Value = Settings.Default.VolIncDec;
            //load WOEID
            txtWOEID.Text = Settings.Default.WOEID;
            lblTown.Text = Settings.Default.Town;
            //load EMAIL and PASSWORD from system settings
            txtEmail.Text = Settings.Default.Email;
            txtPass.Text = Settings.Default.Password;


        }

        internal string slideDir;
        void loadUserCommands()
        {
            lvUsrCmds.Items.Clear();
            lvUsrCmds.View = View.Details;
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT usrCmds, jrvsRspns, shellLoc FROM tblUserCommands";

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string[] list = { rdr[1].ToString(), rdr[2].ToString() };
                lvUsrCmds.Items.Add(rdr[0].ToString()).SubItems.AddRange(list);
                lvUsrCmds.Columns[0].Width = -2;
                lvUsrCmds.Columns[1].Width = -2;
                lvUsrCmds.Columns[2].Width = -2;

            }
            conn.Close();
        }
        private void tmrSlide_Tick(object sender, EventArgs e)
        {
            if (slideDir == "in")
            {
                if (this.Top > -20)
                {
                    tmrSlide.Enabled = false;
                }
                this.Top = this.Top + 10;
            }
            else if (slideDir == "out")
            {
                if (this.Top <= -1 * this.Height)
                {
                    tmrSlide.Enabled = false;
                    this.Close();
                }
                this.Top = this.Top - 10;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CustomSave();
        }
        internal void CustomSave()
        {
            try
            {
                Settings.Default.VolIncDec = Convert.ToInt32(VolVal.Value);
                Settings.Default.WOEID = txtWOEID.Text;
                Settings.Default.Email = txtEmail.Text;
                Settings.Default.Password = txtPass.Text;
                frmMain main = new frmMain();
                try 
                { 
                    main.getWeatherDetails();
                }
                catch 
                {
                    main.JARVIS.Speak("Please provide a valid W.O.E.I.D.");
                    Process.Start("http://isithackday.com/geoplanet-explorer/index.php");
                    tabControl1.SelectedIndex = 1;
                    txtWOEID.Focus();
                    this.TopMost = true;
                    this.TopMost = false;
                }

                lblTown.Text = Settings.Default.Town;
                Settings.Default.Save();

            }
            catch
            {

            }
        }

        private void btnFilePath_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK) // Test result.
            {
                txtShellLoc.Text = openFileDialog1.FileName;
            }

        }

        private void btnURL_Click(object sender, EventArgs e)
        {
            txtShellLoc.Text = "http://www.";
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            frmMain main = new frmMain();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a directory to browse";
            //main.JARVIS.SpeakAsync(fbd.Description);
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtShellLoc.Text = fbd.SelectedPath;

            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmMain main= new frmMain();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@usrCmds", txtCommand.Text);
            cmd.Parameters.AddWithValue("@jrvsRspns", txtResponse.Text);
            cmd.Parameters.AddWithValue("@shellLoc", txtShellLoc.Text);
            cmd.CommandText = "INSERT INTO tblUserCommands(usrCmds,jrvsRspns,shellLoc) VALUES(@usrCmds, @jrvsRspns, @shellLoc)";
            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
                main.JARVIS.SpeakAsync("new commands added");
                conn.Close();
            }
            catch { }
            loadUserCommands();
        }

        private void cmsDel_Click(object sender, EventArgs e)
        {
            frmMain main = new frmMain();
            SqlCeCommand cmd = conn.CreateCommand();
            string cmdForDeleting = lvUsrCmds.SelectedItems[0].Text;
            cmd.Parameters.AddWithValue("@userCommand", cmdForDeleting);
            txtCommand.Text = cmdForDeleting;
            cmd.CommandText = "DELETE FROM tblUserCommands WHERE usrCmds=@userCommand";
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                main.JARVIS.SpeakAsync(cmdForDeleting + " command deleted");
                conn.Close();
                loadUserCommands();
            }
            catch 
            { 
                main.JARVIS.SpeakAsync(cmdForDeleting + " command not deleted.");
            }
        }

    }
}
