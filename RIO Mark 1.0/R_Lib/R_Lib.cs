using System;
using System.Collections.Generic;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
//Speech
using System.Speech.Recognition;
using System.Speech.Synthesis;
//for weather
using System.Xml;
//database
using System.Data;

//Webservices
using System.ServiceModel;
using R_Lib.WebServiceConverterLength;
using R_Lib.WebServiceWeather;
using R_Lib.WebServicePeriodicTable;
using System.Data.SqlServerCe;
using System.Reflection;
using System.Diagnostics;

namespace R_Lib
{
    public class ActionResult
    {
        public bool success { get; set; }
        public string msg { get; set; }
    }

    public partial class R_Speech
    {
        public R_Speech(RichTextBox convo)
        {
            //Initialize database connection properties
            db.conn_init();
            rtb_Convo = convo;
        }
        public RichTextBox rtb_Convo { get; set; }

        //speech
        public SpeechRecognitionEngine R_recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
        SpeechSynthesizer RIO = new SpeechSynthesizer();
        public bool Dictation_Switch;
        public bool google_search_switch;

        //Database
        public string StartUpPath;
        public R_Lib.R_db_conn db = new R_Lib.R_db_conn(Application.StartupPath);

        //grammars
        public Grammar R_shellcommandgrammar;
        public Grammar R_systemcommandgrammar;
        public Grammar R_spoken;
        public Grammar R_VideoGrammar;
        public Grammar R_SongGrammar;

        //misc
        public string QEvent;
        public string AEvent;

        //User info
        public string userName = Environment.UserName;

        //rio Speak methods
        public void Speak(string phrase)
        {
            rtb_Convo.AppendText("RIO: " + phrase);
            RIO.Speak(phrase);
        }
        public void SpeakAsync(string phrase)
        {
            rtb_Convo.AppendText("RIO: " + phrase);
            RIO.SpeakAsync(phrase);
        }

        public void SpeakCancelAll()
        {
            RIO.SpeakAsyncCancelAll();
        }

        //-------------------- Vocabularies ---------------------------//
        public bool speech_conn = new bool();
        List<string> sysGrammar = new List<string>();
        List<string> usrGrammar = new List<string>();
        public void loadSystemVocabs()
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

                R_systemcommandgrammar = new Grammar(new GrammarBuilder(new Choices(sysGrammar.ToArray())));
                R_recognizer.LoadGrammar(R_systemcommandgrammar);         // loads system's commands

                R_recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
                R_recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
                SpeakAsync("System commands loaded");
            }
            catch (Exception ex)
            {
                SpeakAsync("Connection Unsuccessful: " + ex.Message);
                MessageBox.Show("Connection Unsuccessful: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.conn.Close();
            }
        }
        public ActionResult loadShellGrammar()
        {
            R_shellcommandgrammar = null;
            db.cmd = db.conn.CreateCommand();
            db.cmd.CommandText = "SELECT usrCmds from tblUserCommands";
            try
            {
                int cntr = 1;
                if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }

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
                    R_shellcommandgrammar = new Grammar(new GrammarBuilder(new Choices(usrGrammar.ToArray())));
                    R_recognizer.LoadGrammar(R_shellcommandgrammar); // loads user's custom commands
                    R_recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Shell_SpeechRecognized); //Unsubscribe to previous event. This part is to prevent double execution of Recognized speech event.
                    R_recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Shell_SpeechRecognized); //Subscribe to previous event again. 

                    SpeakAsync("User commands loaded");
                }
                else
                {
                    string msg = "No user commands found. Would you like to add a command now?";
                    SpeakAsync(msg);
                    DialogResult res = MessageBox.Show(msg, "No user commands loaded", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        return new ActionResult() { success = true, msg = "AddCommand" };
                    }
                }
                return new ActionResult() { success = true, msg = "ShellCommandsLoaded" };
            }
            catch (SqlCeException ex)
            {
                return new ActionResult() { success = false, msg = ex.Message };
            }
            finally { if (!speech_conn) { db.conn.Close(); } }
        }

        public void loadDictationGrammar()
        {
            Grammar dictate = new DictationGrammar();
            R_recognizer.LoadGrammar(dictate);
            R_recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Dictation_SpeechRecognized);
        }
        void reloadDefaultGrammars()
        {
            R_recognizer.UnloadAllGrammars();
            loadSystemVocabs();
            loadShellGrammar();
            // R_Speech.R_SpeakAsync("Default grammars reloaded");
        }

        void unloadDictationGrammar()
        {

            //R_Speech.R_recognizer;
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
                    SpeakAsync(rdr[2].ToString());
                    _ = rdr[3].ToString() != "" ? Process.Start(rdr[3].ToString()) : null;
                }
            }
            catch { }
            finally { db.conn.Close(); }
        }
        void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            rtb_Convo.AppendText("ME: " + speech);

            string speech_filtered = speech.Replace("'", "''");
            try
            {
                // userSpeech = speech;
                SqlCeCommand cmd = db.conn.CreateCommand();
                cmd.CommandText = "SELECT Method_resp FROM tblSysCommands where (Q_Event='" + QEvent + "' and Cmd='" + speech_filtered + "') OR (Q_Event='Default' and Cmd='" + speech_filtered + "')";
                //MessageBox.Show(cmd.CommandText);
                if (db.conn.State != ConnectionState.Open) { db.conn.Open(); }
                SqlCeDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    speech_conn = true;
                    exec_method(rdr[0].ToString());
                    SpeakAsync("I heard you say " + speech_filtered);
                }
                //db.conn.Close();
            }
            catch (Exception ex) { SpeakAsync(ex.Message); }
            finally { db.conn.Close(); }

        }
        void Dictation_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (Dictation_Switch)
            {
                if (google_search_switch)
                {
                    SpeakAsync("Querying google for " + e.Result.Text);
                    Process.Start("https://www.google.com/search?q=" + e.Result.Text);

                    Dictation_Switch = false;
                    google_search_switch = false;

                    reloadDefaultGrammars();
                }
            }
        }
        public void exec_method(string methodName)
        {
            if (methodName == "")
            {
                SpeakAsync("No action specified for this command.");
                return;
            }

            try
            {
                // TODO: Cannot invoke method from project referencing this. Maybe need to move commands here in R_Lib
                // Suggestion: Maybe pass whole instance of frmMain?
                //Type thisType = this.GetType();
                //MethodInfo theMethod = thisType.GetMethod(methodName);
                //theMethod.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Speak("Specified method: " + methodName + " is not existing in my current schema. Perhaps a patch update is in order?");
                // commandReply = ex.Message;
            }
        }
    }
    public class WebServices
    {
        public static BasicHttpBinding binding = new BasicHttpBinding();
        public static EndpointAddress address;

        public WebServices()
        {
            binding.MaxReceivedMessageSize = 200000000;

        }

        public partial class Unit_Converters
        {
            public string ConvertLength(double value, Lengths from, Lengths to)
            {
                try
                {

                    address = new EndpointAddress("http://www.webservicex.net/length.asmx?WSDL");

                    lengthUnitSoapClient lusc = new lengthUnitSoapClient(binding, address);

                    double converted_value = lusc.ChangeLengthUnit(value, from, to);
                    return converted_value.ToString();
                }
                catch (Exception ex)
                {
                    return "Convertion Error: " + ex.Message;
                }
            }
        }

        public partial class Weather
        {
            public string getWeather()
            {
                try
                {

                    address = new EndpointAddress("http://www.webservicex.net/globalweather.asmx?WSDL");

                    GlobalWeatherSoapClient gwsc = new GlobalWeatherSoapClient(binding, address);

                    string ret_cities = gwsc.GetCitiesByCountry("Philippines");
                    List<string> city_list = new List<string>();

                    //TODO: Read xml data into string[] of countries
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(ret_cities);
                    XmlNodeList parentNode = xmlDoc.GetElementsByTagName("NewDataSet");
                    foreach (XmlNode childrenNode in parentNode)
                    {
                        //HttpContext.Current.Response.Write(childrenNode.SelectSingleNode("//City").Value);
                        city_list.Add(childrenNode.SelectSingleNode("//City").Value);
                    }
                    // ------------------------------------------------------

                    string return_weather = string.Join(";", city_list.ToArray());
                    return return_weather;
                }
                catch (Exception ex)
                {
                    return "Convertion Error: " + ex.Message;
                }

            }
        }

        public partial class PeriodicTable
        {
            periodictableSoapClient PSC;

            public PeriodicTable()
            {
                address = new EndpointAddress("http://www.webservicex.net/periodictable.asmx?WSDL");
                PSC = new periodictableSoapClient(binding, address);
            }

            public string get_AtomicNumber(string element_name)
            {
                string result = PSC.GetAtomicNumber(element_name);
                string ret_string = "wiw";

                //XMLReader.XMLStringReader xml = new XMLReader.XMLStringReader(result);
                //List<string> xml_elements = xml.Elements();
                //foreach(string i in xml_elements)
                //{
                //    ret_string = xml.GetElement("AtomicNumber");
                //}

                return ret_string;
            }

            public string get_AtomicWeight(string element_name)
            {
                string ret_string = PSC.GetAtomicWeight(element_name);
                return ret_string;
            }

            public string get_Atoms()
            {
                string ret_string = PSC.GetAtoms();
                return ret_string;
            }

            public string get_ElementSymbol(string element_name)
            {
                string ret_string = PSC.GetElementSymbol(element_name);
                return ret_string;
            }
        }
    }

}
