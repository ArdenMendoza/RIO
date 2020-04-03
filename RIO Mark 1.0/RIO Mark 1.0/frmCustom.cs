using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using rio_Mark_1._0.Properties;
using System.Drawing;
using System.Diagnostics;
using System.Data.SqlServerCe;
using rio_Mark_1._0;

namespace rioSystem_2._0
{
    public partial class frmCustom : Form
    {
        FormControls R_Forms = new FormControls();

        R_Lib.R_Speech R_Speech;
        R_Lib.R_db_conn db = new R_Lib.R_db_conn(Application.StartupPath);

        string StartUpPath=Application.StartupPath;
        
        public frmCustom()
        {
            InitializeComponent();
            R_Speech = new R_Lib.R_Speech(R_Forms.getForm<frmMain>().rt_Convo);
            string StartUpPath = Application.StartupPath;
            db.conn = new SqlCeConnection(db.R_conn_string);
            db.conn.ConnectionString = db.R_conn_string;
            db.R_db_path = StartUpPath;
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
            try
            {
                db.conn_init();
                db.cmd = db.conn.CreateCommand();
                db.cmd.CommandText = "SELECT usrCmds, jrvsRspns, shellLoc FROM tblUserCommands";

                if (db.conn.State != System.Data.ConnectionState.Open) { db.conn.Open(); }
                SqlCeDataReader rdr = db.cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string[] list = { rdr[1].ToString(), rdr[2].ToString() };
                    lvUsrCmds.Items.Add(rdr[0].ToString()).SubItems.AddRange(list);
                    lvUsrCmds.Columns[0].Width = -2;
                    lvUsrCmds.Columns[1].Width = -2;
                    lvUsrCmds.Columns[2].Width = -2;
                }
            }
            catch (Exception ex) { MessageBox.Show(R_Speech.R_SpeakAsync("An error occured while trying to load user commands('loadUserCommands()'). \r\n" + ex.Message)); }
            finally { db.conn.Close(); }
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
                    //main.getWeatherDetails();
                }
                catch 
                {
                    R_Speech.R_Speak("Please provide a valid W.O.E.I.D.");
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
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtShellLoc.Text = fbd.SelectedPath;

            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCeCommand cmd = db.conn.CreateCommand();
            db.cmd.Parameters.AddWithValue("@usrCmds", txtCommand.Text);
            db.cmd.Parameters.AddWithValue("@jrvsRspns", txtResponse.Text);
            db.cmd.Parameters.AddWithValue("@shellLoc", txtShellLoc.Text);
            db.cmd.CommandText = "INSERT INTO tblUserCommands(usrCmds,jrvsRspns,shellLoc) VALUES(@usrCmds, @jrvsRspns, @shellLoc)";
            try
            {
                if (db.conn.State != System.Data.ConnectionState.Open) { db.conn.Open(); }

                db.cmd.ExecuteNonQuery();
                R_Speech.R_SpeakAsync("new commands added");
            }
            catch(Exception ex) { MessageBox.Show(R_Speech.R_SpeakAsync("An error occured while trying to add custom commands.\r\n" + ex.Message)); }
            finally { db.conn.Close(); }
            loadUserCommands();
        }

        private void cmsDel_Click(object sender, EventArgs e)
        {
            frmMain main = new frmMain();
            SqlCeCommand cmd = db.conn.CreateCommand();
            string cmdForDeleting = lvUsrCmds.SelectedItems[0].Text;
            db.cmd.Parameters.AddWithValue("@userCommand", cmdForDeleting);
            txtCommand.Text = cmdForDeleting;
            db.cmd.CommandText = "DELETE FROM tblUserCommands WHERE usrCmds=@userCommand";
            try
            {
                if (db.conn.State != System.Data.ConnectionState.Open) { db.conn.Open(); }
                db.cmd.ExecuteNonQuery();
                R_Speech.R_SpeakAsync(cmdForDeleting + " command deleted");
                db.conn.Close();
                loadUserCommands();
            }
            catch 
            { 
                R_Speech.R_SpeakAsync(cmdForDeleting + " command not deleted.");
            }
        }

    }
}
