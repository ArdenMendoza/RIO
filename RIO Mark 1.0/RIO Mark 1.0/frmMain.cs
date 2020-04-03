using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
//database
using System.Data;

//Speech
using System.Speech.Recognition;
//for volume control
using System.Runtime.InteropServices;
//for weather
using System.Xml;
//using Windows.Input;
//database
using System.Data.SqlServerCe;
//Media(Music)
//using TagLib;
using rioSystem_2._0;
using rio_Mark_1._0.Properties;

// Dynamic calling of method
using System.Reflection;

//WebServices
using System.ServiceModel;
using R_Lib.WebServiceConverterLength;
using R_Lib.WebServiceWeather;
using R_Lib.WebServiceSunsertRise;
using R_Lib.WebServicePeriodicTable;
using System.Xml.Linq;

namespace rio_Mark_1._0
{
    public partial class frmMain : Form
    {
        FormControls R_Forms = new FormControls();

        R_Lib.R_Speech R_Speech;
        R_Lib.R_db_conn db = new R_Lib.R_db_conn(Application.StartupPath);
        R_Lib.WebServices.Unit_Converters R_ws_UnitConverters = new R_Lib.WebServices.Unit_Converters();
        R_Lib.WebServices.Weather R_ws_weather = new R_Lib.WebServices.Weather();
        R_Lib.WebServices.PeriodicTable R_ws_PeriodicTable = new R_Lib.WebServices.PeriodicTable();

        bool Dictation_Switch;
        bool google_search_switch;

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
        }


        public void frmMain_Load(object sender, EventArgs e)
        {
            R_Speech = new R_Lib.R_Speech(R_Forms.getForm<frmMain>().rt_Convo);

            #region setup db connection

            //System.IO.File.Delete(Application.StartupPath + @"\rioDB.sdf"); //Testing script

            //Initialize database connection properties
            db.conn_init();

            #endregion setup db connection

            #region Load default grammars
            loadSystemVocabs(); // load rio default commands
            R_Speech.R_SpeakAsync("System commands loaded");
            loadShellGrammar(); // Load user custom commands
            R_Speech.R_SpeakAsync("User commands loaded");
            //loadDictationGrammar();
            //R_Speech.R_SpeakAsync("Dictation grammars loaded");
            #endregion Load default grammars

            #region setup speech recognizer properties
            try
            {
                R_Speech.R_recognizer.SetInputToDefaultAudioDevice(); //load default microphone
                R_Speech.R_recognizer.RecognizeAsync(RecognizeMode.Multiple);  //sets the program to interpret multiple commands
                R_Speech.R_SpeakAsync("Ready");
            }
            catch (Exception ex)
            {
                //R_Speech.R_SpeakAsync(ex.ToString());
                MessageBox.Show(ex.ToString());
                R_Speech.R_SpeakAsync("Loaded with errors.");
            }
            #endregion setup speech recognizer properties
        }

        //-------------------- Vocabularies ---------------------------//
        List<string> sysGrammar = new List<string>();
        void loadSystemVocabs()
        {
            db.cmd = db.conn.CreateCommand();
            db.cmd.CommandText = "SELECT Cmd from tblSysCommands";
            try
            {
                if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }
                db.rdr = db.cmd.ExecuteReader();

                while (db.rdr.Read())
                {
                    sysGrammar.Add(db.rdr["Cmd"].ToString());
                }
                db.rdr.Dispose();

                R_Speech.R_systemcommandgrammar = new Grammar(new GrammarBuilder(new Choices(sysGrammar.ToArray())));
                R_Speech.R_recognizer.LoadGrammar(R_Speech.R_systemcommandgrammar);         // loads system's commands

                R_Speech.R_recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(System_SpeechRecognized);
                R_Speech.R_recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(System_SpeechRecognized);
                //R_Speech.R_SpeakAsync("System commands loaded");
            }
            catch (SqlCeException ex)
            {
                R_Speech.R_SpeakAsync("Connection Unsuccessful: " + ex.Message);
                MessageBox.Show("Connection Unsuccessful: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.conn.Close();
            }


            
        }
        List<string> usrGrammar = new List<string>();
        void loadShellGrammar()
        {
            R_Speech.R_shellcommandgrammar = null;
            db.cmd = db.conn.CreateCommand();
            db.cmd.CommandText = "SELECT usrCmds from tblUserCommands";
            try
            {
                int cntr = 1;
                if(db.conn.State != ConnectionState.Open) { db.conn.Open(); }

                db.rdr = db.cmd.ExecuteReader();
                usrGrammar.Clear();
                while (db.rdr.Read())
                {
                    usrGrammar.Add(db.rdr["usrCmds"].ToString());
                    //R_Speech.R_Speak(rt_Convo, cntr.ToString());
                    cntr++;
                }
                db.rdr.Dispose();
                //MessageBox.Show("User command count after: " + usrGrammar.Count.ToString());
                if (usrGrammar.Count > 0)
                {
                    R_Speech.R_shellcommandgrammar = new Grammar(new GrammarBuilder(new Choices(usrGrammar.ToArray())));
                    R_Speech.R_recognizer.LoadGrammar(R_Speech.R_shellcommandgrammar); // loads user's custom commands
                    R_Speech.R_recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Shell_SpeechRecognized); //Unsubscribe to previous event. This part is to prevent double execution of Recognized speech event.
                    R_Speech.R_recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Shell_SpeechRecognized); //Subscribe to previous event again. 
                    
                    //R_Speech.R_SpeakAsync("User commands loaded");
                }
                else
                {
                    DialogResult res = MessageBox.Show(R_Speech.R_Speak("No user commands found. Would you like to add a command now? "), "No user commands loaded", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        frmCustom c = new frmCustom();
                        c.slideDir = "in";
                        c.ShowDialog();
                    }
                }
            }
            catch (SqlCeException ex)
            { MessageBox.Show("Connection Unsuccessful: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { if (!speech_conn) { db.conn.Close(); } }
        }
        void loadDictationGrammar()
        {
            Grammar dictate = new DictationGrammar();
            R_Speech.R_recognizer.LoadGrammar(dictate);
            R_Speech.R_recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Dictation_SpeechRecognized);
        }
        void unloadDictationGrammar()
        {

            //R_Speech.R_recognizer;
        }
        void reloadDefaultGrammars()
        {
            R_Speech.R_recognizer.UnloadAllGrammars();
            loadSystemVocabs();
            loadShellGrammar();
            // R_Speech.R_SpeakAsync("Default grammars reloaded");
        }


        //--------------------------------- Speech Recognized---------------------------//
        public void exec_method(string methodName)
        {
            if (methodName == "")
            {
                R_Speech.R_SpeakAsync("No action specified for this command.");
                return;
            }

            try
            {
                Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod(methodName);
                theMethod.Invoke(this, null);
            }
            catch (Exception ex)
            {
                R_Speech.R_Speak("Specified method: " + methodName + " is not existing in my current schema. Perhaps a patch update is in order?");
                rt_Convo.AppendText(ex.Message);
            }
        }
        Random rnd = new Random();
        public bool speech_conn = new bool();
        void System_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            string speech_filtered = speech.Replace("'", "''");
            try
            {
                
                SqlCeCommand cmd = db.conn.CreateCommand();
                //MessageBox.Show("Q_Event: " + R_Speech.R_QEvent + "; Speech: " + speech);
                rt_Convo.AppendText("Me: " +  speech);
                cmd.CommandText = "SELECT Method_resp FROM tblSysCommands where (Q_Event='" + R_Speech.R_QEvent + "' and Cmd='" + speech_filtered + "') OR (Q_Event='Default' and Cmd='" + speech_filtered + "')";
                //MessageBox.Show(cmd.CommandText);
                if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }
                SqlCeDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    speech_conn = true;
                    exec_method(rdr[0].ToString());
                }
                //db.conn.Close();
            }
            catch (Exception ex) { MessageBox.Show(R_Speech.R_SpeakAsync(ex.Message)); }
            finally { db.conn.Close(); }

        }
        
        void Shell_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            string speech = e.Result.Text;
            try
            {
                SqlCeCommand cmd = db.conn.CreateCommand();
                cmd.CommandText = "SELECT id,usrCmds,jrvsRspns,shellLoc FROM tblUserCommands where usrCmds='" + speech + "'";
                if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }
                SqlCeDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    R_Speech.R_SpeakAsync(rdr[2].ToString());
                    Process.Start(rdr[3].ToString());
                }
            }
            catch { }
            finally { db.conn.Close(); }
        }

        void Dictation_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(Dictation_Switch)
            {
                if (google_search_switch)
                {
                    R_Speech.R_SpeakAsync("Querying google for " + e.Result.Text);
                    Process.Start("https://www.google.com/search?q=" + e.Result.Text);

                    Dictation_Switch = false;
                    google_search_switch = false;

                    reloadDefaultGrammars();
                }
            }
        }


        //--------------------------------- COMMANDS ----------------------------------//
        #region COMMANDS

        public void R_Greet()
        {
            R_Speech.R_SpeakCancelAll();
            stopReadingEmails();
            int num = rnd.Next(1, 31);

            if (num <= 10) { R_Speech.R_SpeakAsync("Yes sir"); }
            else if (num <= 20) { R_Speech.R_SpeakAsync("Yes"); }
            else if (num <= 30) { R_Speech.R_SpeakAsync("How may i be of assistance?"); }
        }
        //Creator Info
        public void creatorInfo()
        {
            R_Speech.R_SpeakAsync("I'd like to say Tony Stark, but actually, Arden created me. here, check out his google plus profile.");
            Process.Start("https://plus.google.com/+ArdenCristopherMendoza/posts/p/pub");
        }
        //LockScreen
        public void lockScreen()
        {
            //bool result = LockWorkStation();

            //if (result == false)
            //{
            //    // An error occured
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
            //}
        }
        //Close Window
        public void closeWindow()
        {
            System.Windows.Forms.SendKeys.SendWait("%{F4}");
        }

        #region rio Show/hide
        public void UI_onSystemTrayrio()
        {
            notifIcon.Visible = true;
            notifIcon.BalloonTipText = "I'll be right here if you need anything else sir";
            notifIcon.BalloonTipTitle = Application.ProductName;
            notifIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifIcon.Text = "rio hidden from view";
            notifIcon.ShowBalloonTip(3000);

            this.ShowInTaskbar = false;
            this.Hide();
            R_Speech.R_SpeakAsync(notifIcon.BalloonTipText);
        }
        public void UI_rioOnTop()
        {
            this.TopMost = true;
            this.TopMost = false;
            R_Speech.R_SpeakAsync("right here sir");
            this.Visible = true;

            UI_outOfSystemTrayrio(); UI_centerAlignIcon();
        }
        public void UI_outOfSystemTrayrio()
        {
            notifIcon.Visible = false;
            notifIcon.BalloonTipIcon = ToolTipIcon.Info;
            this.ShowInTaskbar = true;
            this.Visible = true;
        }
        public void UI_centerAlignIcon()
        {
            //R_Speech.R_SpeakAsync("in the middle of your screen");
            int scrnHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int scrnWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int myHorCenter = this.img_rio.Width / 2;
            int myVerCenter = this.img_rio.Height / 2;

            this.Left = (scrnWidth / 2) - myHorCenter;
            this.Top = (scrnHeight / 2) - myVerCenter;

            //this.Left = (scrnWidth / 2);
            //this.Top = (scrnHeight / 2);

            //this.AllowTransparency = false;
        }
        #endregion

        #region Date/Time voids
        public void timecheck()
        {
            DateTime now = DateTime.Now;
            string time = now.GetDateTimeFormats('t')[0];
            R_Speech.R_SpeakAsync(time);
        }
        public void dayCheck()
        {
            R_Speech.R_SpeakAsync(DateTime.Today.ToString("dddd"));
        }
        public void dateCheck()
        {
            R_Speech.R_SpeakAsync(System.DateTime.Today.ToString("dd-MM-yyyy"));
        }
        #endregion

        #region User Custom Command Voids
        public bool customIsOpened()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f == custom)
                {
                    return true;
                }
            }
            return false;
        }
        frmCustom custom;
        public void addCommand()
        {
            if (!customIsOpened())
            {
                custom = new frmCustom();
                custom.Show();
                custom.slideDir = "in";
            }
        }
        public void closeCommandWindow()
        {
            frmCustom cust = R_Forms.getForm<frmCustom>();
            cust.slideDir = "out";
            cust.tmrSlide.Enabled = true;
        }
        public void UpdateCommands()
        {
            R_Speech.R_SpeakAsync("This may take a few seconds");
            custom = new frmCustom();
            custom.CustomSave();
            try
            {
                R_Speech.R_recognizer.UnloadGrammar(R_Speech.R_shellcommandgrammar);
                loadShellGrammar();
            }
            catch(Exception ex) { R_Speech.R_SpeakAsync("Error while updating commands(UpdateCommands())\r\n" + ex.Message); }

            R_Speech.R_SpeakAsync("All commands updated");
        }
        public void UI_Closerio()
        {
            R_Speech.R_SpeakCancelAll();
            R_Speech.R_Speak("Till next time");
            this.Close();
        }
        public void ShowSystemCommands()
        {
            //listBox1.Items.Clear();
            SqlCeCommand cmd = db.conn.CreateCommand();
            cmd.CommandText = "SELECT Cmd FROM tblSysCommands";
            if (db.conn.State != ConnectionState.Open) { if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }; }

            SqlCeDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                //listBox1.Items.Add(rdr[0].ToString());
            }

            db.conn.Close();

        }
        #endregion
        
        #region weather update
        string WOEID = Settings.Default.WOEID;
        public void getWeatherUpdate()
        {
            #region old  getWeatherUpdate function
            R_Speech.R_SpeakAsync("Retrieving latest weather data");
            try
            {
                if (getWeatherDetails())
                {
                    string lastBuildDate = Settings.Default.LastBuildDate;
                    string Temp = Settings.Default.Temperature;
                    string Unit = Settings.Default.Unit;
                    string Condition = Settings.Default.Condition;
                    string Humidity = Settings.Default.Humidity;
                    string WindSpeed = Settings.Default.WindSpeed;
                    string Town = Settings.Default.Town;
                    string TFCond = Settings.Default.TFCond;
                    string TFHigh = Settings.Default.TFHigh;
                    string TFLow = Settings.Default.TFLow;

                    string weather = "The weather details for " + Town + ": as of " + lastBuildDate + ", is as follows: " +
                                     "Conditions outside is " + Condition + " " +
                                     "with temperature at " + Temp + " degrees" + Unit + " " +
                                     ". There will be a wind speed of " + WindSpeed + " miles per hour";
                    R_Speech.R_SpeakAsync(weather);
                }
            }
            catch
            {
                string lastBuildDate = Settings.Default.LastBuildDate;
                string Temp = Settings.Default.Temperature;
                string Unit = Settings.Default.Unit;
                string Condition = Settings.Default.Condition;
                string Humidity = Settings.Default.Humidity;
                string WindSpeed = Settings.Default.WindSpeed;
                string Town = Settings.Default.Town;
                string TFCond = Settings.Default.TFCond;
                string TFHigh = Settings.Default.TFHigh;
                string TFLow = Settings.Default.TFLow;
                R_Speech.R_SpeakAsync("I am unable to retrieve the latest weather data right now.");
                string weather = "The last time I checked was " + lastBuildDate + ". " +
                                 "The conditions outside was " + Condition + " " +
                                 "with temperature at " + Temp + " degrees" + Unit + " " +
                                 ". There will be a wind speed of " + WindSpeed + " miles per hour";
                R_Speech.R_SpeakAsync(weather);
            }
            #endregion
        }
        public void getWeatherForecast()
        {
            string lastBuildDate = Settings.Default.LastBuildDate;
            string Temp = Settings.Default.Temperature;
            string Unit = Settings.Default.Unit;
            string Condition = Settings.Default.Condition;
            string Humidity = Settings.Default.Humidity;
            string WindSpeed = Settings.Default.WindSpeed;
            string Town = Settings.Default.Town;
            string TFCond = Settings.Default.TFCond;
            string TFHigh = Settings.Default.TFHigh;
            string TFLow = Settings.Default.TFLow;

            string forecast;
            try
            {
                getWeatherDetails();
                forecast = "Tomorrow, there will be " + TFCond + " in " + Town + " with a high of " + TFHigh + " and a low of " + TFLow;
            }
            catch
            {
                forecast = "We're currently not connected to the Internet. The latest weather data I acquired was last" + lastBuildDate + " it says. There will be " + TFCond + " in " + Town + " with a high of " + TFHigh + " and a low of " + TFLow;
            }
            R_Speech.R_SpeakAsync(forecast);
        }
        public bool getWeatherDetails()
        {

            try
            {
                string query = String.Format("http://weather.yahooapis.com/forecastrss?w=" + Settings.Default.WOEID);
                XmlDocument wData = new XmlDocument();
                wData.Load(query);

                XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
                manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
                XmlNode channel = wData.SelectSingleNode("rss").SelectSingleNode("channel");
                XmlNodeList nodes = wData.SelectNodes("/rss/channel/item/yweather:forecast", manager);

                string weatherDay = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["day"].Value;
                switch (weatherDay) { case "Sun": weatherDay = "Sunday"; break; case "Mon": weatherDay = "Monday"; break; case "Tue": weatherDay = "Tuesday"; break; case "Wed": weatherDay = "Wednesday"; break; case "Thu": weatherDay = "Thursday"; break; case "Fri": weatherDay = "Friday"; break; case "Sat": weatherDay = "Saturday"; break; }
                string weatherDate = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["date"].Value;
                string Temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
                string Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                string Humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;
                string WindSpeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;
                string Town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;
                string TFCond = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["text"].Value;
                string TFHigh = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
                string TFLow = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value;
                string unit = channel.SelectSingleNode("yweather:units", manager).Attributes["temperature"].Value;
                if (unit == "F") { unit = "Fahrenheit"; }
                else { unit = "Celsius"; }

                //Send latest weather details to system settings
                DateTime now = DateTime.Now;
                string time = now.GetDateTimeFormats('t')[0];

                Settings.Default.LastBuildDate = weatherDay + ", " + weatherDate + " at " + time; Settings.Default.Save();
                Settings.Default.Temperature = Temperature;
                Settings.Default.Condition = Condition;
                Settings.Default.Humidity = Humidity;
                Settings.Default.WindSpeed = WindSpeed;
                Settings.Default.Town = Town;
                Settings.Default.TFCond = TFCond;
                Settings.Default.TFHigh = TFHigh;
                Settings.Default.TFLow = TFLow;
                Settings.Default.Unit = unit;
                Settings.Default.Save();
                return true;
            }
            catch (Exception ex)
            {
                R_Speech.R_SpeakAsync("An error occured while trying to retrieve weather details. \r\n 'getWeatherDetails()' \r\n" + ex.Message);
                return false;
            }
        }
        #endregion weather updates

        #region Volume Void
        public const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        public const int APPCOMMAND_VOLUME_UP = 0xA0000; //default value is 0xA0000
        public const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        public const int WM_APPCOMMAND = 0x319; //default is 0x319

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);
        public void volumeUp()
        {
            int vol = Settings.Default.VolIncDec;
            for (int i = 0; i < vol; i++)
            {
                SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
            }
            //R_Speech.R_SpeakAsync("Increased volume by" + vol + " times twice");
        }
        public void volumeDown()
        {
            int vol = Settings.Default.VolIncDec;
            for (int i = 0; i < vol; i++)
            {
                SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
            }

            //R_Speech.R_SpeakAsync("Decreased volume by" + vol + " times twice");
        }
        public void mute()
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }
        #endregion 

        #region Email Voids
        #region email variables

        int TempCounter;
        string[] ArrayTitle = new string[1000];
        string[] ArraySummary = new string[1000];
        string[] ArraySenderName = new string[1000];
        string[] ArraySenderEmail = new string[1000];
        string[] ArrayHref = new string[1000];
        int emailID = 0;
        #endregion
        public void getUnreadEmails()
        {
            R_Speech.R_SpeakCancelAll();
            R_Speech.R_SpeakAsync("Checking");
            TempCounter = 0;
            emailID = 0;
            try
            {
                System.Net.WebClient objClient = new System.Net.WebClient();
                string response;
                //string fullcount;
                string Title;
                string Summary;
                string SenderName;
                string SenderEmail;
                string href;

                //Creating a new xml document
                XmlDocument doc = new XmlDocument();

                //Logging in Gmail server to get data
                string email = Settings.Default.Email;
                string pass = Settings.Default.Password;
                objClient.Credentials = new System.Net.NetworkCredential(email, pass);
                //reading data and converting to string
                try
                {
                    response = Encoding.UTF8.GetString(objClient.DownloadData(@"https://mail.google.com/mail/feed/atom/"));
                    response = response.Replace(@"<feed version=""0.3"" xmlns=""http://purl.org/atom/ns#"">", @"<feed>");
                    //loading into an XML so we can get information easily
                    doc.LoadXml(response);
                    //nr of emails
                    string nr = doc.SelectSingleNode(@"/feed/fullcount").InnerText;
                    //Reading the title and the summary for every email
                    foreach (XmlNode node in doc.SelectNodes(@"/feed/entry"))
                    {
                        Title = node.SelectSingleNode("title").InnerText;
                        Summary = node.SelectSingleNode("summary").InnerText + "...";
                        SenderName = node.SelectSingleNode("author").FirstChild.InnerText;
                        SenderEmail = node.SelectSingleNode("author").LastChild.InnerText;
                        href = node.SelectSingleNode("link").Attributes["href"].Value;

                        ArraySenderEmail[TempCounter] = SenderEmail;
                        ArraySenderName[TempCounter] = SenderName;
                        ArrayTitle[TempCounter] = Title;
                        ArraySummary[TempCounter] = Summary;
                        ArrayHref[TempCounter] = href;
                        TempCounter++;
                    }
                    if (Convert.ToInt32(nr) == 0)
                    { R_Speech.R_SpeakAsync("You have no new emails"); }
                    else
                    {
                        R_Speech.R_QEvent = "read email";
                        R_Speech.R_AEvent = "";
                        R_Speech.R_Speak("You have " + nr + " new emails. I can read the latest 20 items. Would you like me to read them for you sir?");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\n" + ex.ToString());
                    if (ex.Message == "The remote name could not be resolved: 'mail.google.com'")
                    {
                        R_Speech.R_SpeakAsync("I'm sorry sir, it appears we cannot connect to the email server.");
                    }
                    else
                    {
                        R_Speech.R_SpeakAsync("invalid login information");
                        custom = new frmCustom();
                        custom.Show();
                        custom.slideDir = "in";
                        custom.tabControl1.SelectedIndex = 1;
                        custom.txtEmail.Focus();
                    }
                }
            }
            catch (Exception exe)
            {
                MessageBox.Show("check your network connection: " + exe.Message);
            }
        }
        public void stopReadingEmails()
        {
            R_Speech.R_QEvent = "";
            R_Speech.R_AEvent = "";
            ArraySenderEmail = null;
            ArraySenderName = null;
            ArrayTitle = null;
            ArraySummary = null;
            ArrayHref = null;
        }
        public void readEmail()
        {
            R_Speech.R_SpeakCancelAll();
            MessageBox.Show("[readEmail] R_Speech.R_QEvent: " + R_Speech.R_QEvent + " | R_Speech.R_AEvent: " + R_Speech.R_AEvent);
            if (R_Speech.R_QEvent == "read email" && R_Speech.R_AEvent == "yes")
            {
                nextEmail();
            }
            else { R_Speech.R_SpeakAsync("I'm not going to read your emails"); }
        }
        public void nextEmail()
        {
            R_Speech.R_SpeakCancelAll();
            if (R_Speech.R_QEvent == "read email" && R_Speech.R_AEvent == "yes")
            {
                if (emailID < TempCounter)
                {
                    R_Speech.R_SpeakAsync("E-mail number " + (emailID + 1) + " from " + ArraySenderName[emailID] + ". The Summary reads: " + ArraySummary[emailID]);
                    ShowEmailForm(ArrayTitle[emailID], ArraySummary[emailID], ArraySenderName[emailID], ArrayHref[emailID]);
                    emailID++;
                }
            else
                { R_Speech.R_SpeakAsync("no more emails to read"); }
            }
        }
        public void previousEmail()
        {
            R_Speech.R_SpeakCancelAll();
            emailID--;
            if (R_Speech.R_QEvent == "read email")
            {
                if (R_Speech.R_AEvent == "yes")
                {
                    if (emailID >= 1)
                    {
                        R_Speech.R_SpeakAsync("Email from: " + ArraySenderName[emailID] + ". The Summary reads: " + ArraySummary[emailID]);
                        ShowEmailForm(ArrayTitle[emailID], ArraySummary[emailID], ArraySenderName[emailID], ArrayHref[emailID]);
                    }

                    else
                    {
                        R_Speech.R_SpeakAsync("No more newer email.");
                        emailID = 0;
                    }
                }
            }
        }
        frmEmail email;
        public void ShowEmailForm(string title, string msg, string sender, string href)
        {
            email = new frmEmail();
            email.Left = SystemInformation.WorkingArea.Width + 5;
            email.lblTitle.Text = title;
            email.lblMsg.Text = msg;
            email.lblMsg.Click += new System.EventHandler(frmEmailMsg_Click);
            email.tmrSlide.Enabled = true;
            email.Show();

        }
        public void frmEmailMsg_Click(object sender, EventArgs e)
        {
            Process.Start(ArrayHref[emailID - 1]);
        }

        public void QEvent_Email_Yes()
        {
            if (R_Speech.R_QEvent == "read email")
            {
                R_Speech.R_SpeakCancelAll();
                R_Speech.R_AEvent = "yes";
                readEmail();
                R_Speech.R_AEvent = "";
            }
        }
        public void QEvent_Email_No()
        {
            R_Speech.R_AEvent = "no";
        }

        #endregion

        #region WebService Unit Converter
        public void convert_length()
        {
            string result = R_ws_UnitConverters.ConvertLength(13, Lengths.Inches, Lengths.Kilometers);
            R_Speech.R_SpeakAsync(result);
        }
        #endregion

        #region WebService Weather
        public void ws_get_weather()
        {
            rt_Convo.AppendText(R_ws_weather.ws_get_weather());
            R_Speech.R_SpeakAsync("Got it.");
        }
        #endregion WebService weather

        #region WebService Periodic Table
        public void get_AtomicNumber() { rt_Convo.AppendText(R_ws_PeriodicTable.get_AtomicNumber("Tungsten")); }
        public void get_AtomicWeight() { MessageBox.Show(R_ws_PeriodicTable.get_AtomicWeight("Tungsten")); }
        public void get_Atoms() { MessageBox.Show(R_ws_PeriodicTable.get_Atoms()); }
        public void get_ElementSymbol() { MessageBox.Show(R_ws_PeriodicTable.get_ElementSymbol("Tungsten")); }

        #endregion WebService Periodic Table

        #region Google Search
        public void google_search()
        {
            //rt_Convo.AppendText("Unloading Default grammars");
            //R_Speech.R_recognizer.UnloadAllGrammars();
            R_Speech.R_SpeakAsync("Search about?");

            //rt_Convo.AppendText("Loading dictation grammar");
            loadDictationGrammar();

            Dictation_Switch = true;
            google_search_switch = true;

        }
        #endregion Google Search


        #endregion

        private void rt_Convo_TextChanged(object sender, EventArgs e)
        {
            rt_Convo.AppendText("\r\n");
            rt_Convo.ScrollToCaret();
        }
    }
}
