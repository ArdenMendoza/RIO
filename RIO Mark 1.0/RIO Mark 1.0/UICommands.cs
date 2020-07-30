using R_Lib;
using R_Lib.WebServiceConverterLength;
using rio.Properties;
using RIO;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RIO
{
    public class UICommands
    {
        R_Lib.R_Speech R_Speech;
        Random rnd = new Random();

        frmMain main = FormControls.getForm<frmMain>();

        public void R_Greet()
        {
            R_Speech.SpeakCancelAll();
            stopReadingEmails();
            int num = rnd.Next(1, 31);

            if (num <= 10) { R_Speech.SpeakAsync("Yes sir"); }
            else if (num <= 20) { R_Speech.SpeakAsync("Yes"); }
            else if (num <= 30) { R_Speech.SpeakAsync("How may i be of assistance?"); }
        }
        //Creator Info
        public void creatorInfo()
        {
            R_Speech.SpeakAsync("I'd like to say Tony Stark, but actually, Arden created me. here, check out his google plus profile.");
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
        public void UI_onSystemTray()
        {
            main.notifIcon.Visible = true;
            main.notifIcon.BalloonTipText = "I'll be right here if you need anything else sir";
            main.notifIcon.BalloonTipTitle = Application.ProductName;
            main.notifIcon.BalloonTipIcon = ToolTipIcon.Info;
            main.notifIcon.Text = "RIO hidden from view";
            main.notifIcon.ShowBalloonTip(3000);

            main.ShowInTaskbar = false;
            main.Hide();
            R_Speech.SpeakAsync(main.notifIcon.BalloonTipText);
        }
        public void UI_rioOnTop()
        {
            main.TopMost = true;
            main.TopMost = false;
            R_Speech.SpeakAsync("right here sir");
            main.Visible = true;

            UI_outOfSystemTrayrio(); UI_centerAlignIcon();
        }
        public void UI_outOfSystemTrayrio()
        {
            main.notifIcon.Visible = false;
            main.notifIcon.BalloonTipIcon = ToolTipIcon.Info;
            main.ShowInTaskbar = true;
            main.Visible = true;
        }
        public void UI_centerAlignIcon()
        {
            //R_Speech.R_SpeakAsync("in the middle of your screen");
            int scrnHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int scrnWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int myHorCenter = main.img_rio.Width / 2;
            int myVerCenter = main.img_rio.Height / 2;

            main.Left = (scrnWidth / 2) - myHorCenter;
            main.Top = (scrnHeight / 2) - myVerCenter;

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
            R_Speech.SpeakAsync(time);
        }
        public void dayCheck()
        {
            R_Speech.SpeakAsync(DateTime.Today.ToString("dddd"));
        }
        public void dateCheck()
        {
            R_Speech.SpeakAsync(System.DateTime.Today.ToString("dd-MM-yyyy"));
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
                custom.isShown = true;
            }
        }
        public void closeCommandWindow()
        {
            frmCustom cust = FormControls.getForm<frmCustom>();
            cust.isShown = false;
            cust.tmrSlide.Enabled = true;
        }
        public void UpdateCommands()
        {
            R_Speech.SpeakAsync("This may take a few seconds");
            custom = new frmCustom();
            custom.CustomSave();
            try
            {
                if (R_Speech.R_shellcommandgrammar != null)
                {
                    R_Speech.R_recognizer.UnloadGrammar(R_Speech.R_shellcommandgrammar);
                }
                R_Speech.loadShellGrammar();
            }
            catch (Exception ex) { R_Speech.SpeakAsync("Error while updating commands(UpdateCommands())\r\n" + ex.Message); }

            R_Speech.SpeakAsync("All commands updated");
        }
        public void UI_Closerio()
        {
            R_Speech.SpeakCancelAll();
            R_Speech.Speak("Till next time");
            main.Close();
        }
        public void ShowSystemCommands()
        {
            // TODO: Create function in R_Lib that will return list here
            // //listBox1.Items.Clear();
            // SqlCeCommand cmd = R_Lib.db.conn.CreateCommand();
            // cmd.CommandText = "SELECT Cmd FROM tblSysCommands";
            // if (db.conn.State != ConnectionState.Open) { if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }; }

            // SqlCeDataReader rdr = cmd.ExecuteReader();
            // while (rdr.Read())
            // {
            //     //listBox1.Items.Add(rdr[0].ToString());
            // }
            // db.conn.Close();
        }
        #endregion

        #region weather update
        string WOEID = Settings.Default.WOEID;
        public void getWeatherUpdate()
        {
            #region old  getWeatherUpdate function
            R_Speech.SpeakAsync("Retrieving latest weather data");
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
                    R_Speech.SpeakAsync(weather);
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
                R_Speech.SpeakAsync("I am unable to retrieve the latest weather data right now.");
                string weather = "The last time I checked was " + lastBuildDate + ". " +
                                 "The conditions outside was " + Condition + " " +
                                 "with temperature at " + Temp + " degrees" + Unit + " " +
                                 ". There will be a wind speed of " + WindSpeed + " miles per hour";
                R_Speech.SpeakAsync(weather);
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
            R_Speech.SpeakAsync(forecast);
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
                R_Speech.SpeakAsync("An error occured while trying to retrieve weather details. \r\n 'getWeatherDetails()' \r\n" + ex.Message);
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
                SendMessageW(main.Handle, WM_APPCOMMAND, main.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
            }
            //R_Speech.R_SpeakAsync("Increased volume by" + vol + " times twice");
        }
        public void volumeDown()
        {
            int vol = Settings.Default.VolIncDec;
            for (int i = 0; i < vol; i++)
            {
                SendMessageW(main.Handle, WM_APPCOMMAND, main.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
            }

            //R_Speech.R_SpeakAsync("Decreased volume by" + vol + " times twice");
        }
        public void mute()
        {
            SendMessageW(main.Handle, WM_APPCOMMAND, main.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
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
            R_Speech.SpeakCancelAll();
            R_Speech.SpeakAsync("Checking");
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
                    { R_Speech.SpeakAsync("You have no new emails"); }
                    else
                    {
                        R_Speech.QEvent = "read email";
                        R_Speech.AEvent = "";
                        R_Speech.Speak("You have " + nr + " new emails. I can read the latest 20 items. Would you like me to read them for you sir?");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\n" + ex.ToString());
                    if (ex.Message == "The remote name could not be resolved: 'mail.google.com'")
                    {
                        R_Speech.SpeakAsync("I'm sorry sir, it appears we cannot connect to the email server.");
                    }
                    else
                    {
                        R_Speech.SpeakAsync("invalid login information");
                        custom = new frmCustom();
                        custom.Show();
                        custom.isShown = true;
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
            R_Speech.QEvent = "";
            R_Speech.AEvent = "";
            ArraySenderEmail = null;
            ArraySenderName = null;
            ArrayTitle = null;
            ArraySummary = null;
            ArrayHref = null;
        }
        public void readEmail()
        {
            R_Speech.SpeakCancelAll();
            MessageBox.Show("[readEmail] R_Speech.R_QEvent: " + R_Speech.QEvent + " | R_Speech.R_AEvent: " + R_Speech.AEvent);
            if (R_Speech.QEvent == "read email" && R_Speech.AEvent == "yes")
            {
                nextEmail();
            }
            else { R_Speech.SpeakAsync("I'm not going to read your emails"); }
        }
        public void nextEmail()
        {
            R_Speech.SpeakCancelAll();
            if (R_Speech.QEvent == "read email" && R_Speech.AEvent == "yes")
            {
                if (emailID < TempCounter)
                {
                    R_Speech.SpeakAsync("E-mail number " + (emailID + 1) + " from " + ArraySenderName[emailID] + ". The Summary reads: " + ArraySummary[emailID]);
                    ShowEmailForm(ArrayTitle[emailID], ArraySummary[emailID], ArraySenderName[emailID], ArrayHref[emailID]);
                    emailID++;
                }
                else
                { R_Speech.SpeakAsync("no more emails to read"); }
            }
        }
        public void previousEmail()
        {
            R_Speech.SpeakCancelAll();
            emailID--;
            if (R_Speech.QEvent == "read email")
            {
                if (R_Speech.AEvent == "yes")
                {
                    if (emailID >= 1)
                    {
                        R_Speech.SpeakAsync("Email from: " + ArraySenderName[emailID] + ". The Summary reads: " + ArraySummary[emailID]);
                        ShowEmailForm(ArrayTitle[emailID], ArraySummary[emailID], ArraySenderName[emailID], ArrayHref[emailID]);
                    }

                    else
                    {
                        R_Speech.SpeakAsync("No more newer email.");
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
            if (R_Speech.QEvent == "read email")
            {
                R_Speech.SpeakCancelAll();
                R_Speech.AEvent = "yes";
                readEmail();
                R_Speech.AEvent = "";
            }
        }
        public void QEvent_Email_No()
        {
            R_Speech.AEvent = "no";
        }

        #endregion

        #region WebService Unit Converter
        public void convert_length()
        {
            WebServices.Unit_Converters converter = new WebServices.Unit_Converters();
            string result = converter.ConvertLength(13, Lengths.Inches, Lengths.Kilometers);
            R_Speech.SpeakAsync(result);
        }
        #endregion

        #region WebService Weather
        public void ws_get_weather()
        {
            WebServices.Weather weather = new WebServices.Weather();
            main.rt_Convo.AppendText(weather.getWeather());
            R_Speech.SpeakAsync("Got it.");
        }
        #endregion WebService weather

        #region WebService Periodic Table
        WebServices.PeriodicTable PeriodicTable = new WebServices.PeriodicTable();
        public void get_AtomicNumber() { main.rt_Convo.AppendText(PeriodicTable.get_AtomicNumber("Tungsten")); }
        public void get_AtomicWeight() { MessageBox.Show(PeriodicTable.get_AtomicWeight("Tungsten")); }
        public void get_Atoms() { MessageBox.Show(PeriodicTable.get_Atoms()); }
        public void get_ElementSymbol() { MessageBox.Show(PeriodicTable.get_ElementSymbol("Tungsten")); }

        #endregion WebService Periodic Table

        #region Google Search
        public void google_search()
        {
            //rt_Convo.AppendText("Unloading Default grammars");
            //R_Speech.R_recognizer.UnloadAllGrammars();
            R_Speech.SpeakAsync("Search about?");

            //rt_Convo.AppendText("Loading dictation grammar");
            R_Speech.loadDictationGrammar();

            R_Speech.Dictation_Switch = true;
            R_Speech.google_search_switch = true;

        }
        #endregion Google Search

    }
}
