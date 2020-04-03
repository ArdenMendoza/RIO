using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
//Speech
using System.Speech.Recognition;
using System.Speech.Synthesis;
//for volume control
using System.Runtime.InteropServices;
//for weather
using System.Web;
using System.Xml;
using System.Xml.Linq;
//using Windows.Input;
using System.Net;
//database
using System.Data;
using System.Data.SqlServerCe;

//Webservices
using System.ServiceModel;
using R_Lib.WebServiceConverterLength;
using R_Lib.WebServiceWeather;
using R_Lib.WebServiceSunsertRise;
using R_Lib.WebServicePeriodicTable;
using HtmlAgilityPack;
using System.Xml;


namespace R_Lib
{
    public class R_Speech
    {
        public R_Speech(RichTextBox v1)
        {
            rtb = v1;
        }
        public RichTextBox rtb { get; set; }
        
        //speech
        public SpeechRecognitionEngine R_recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
        SpeechSynthesizer RIO = new SpeechSynthesizer();

        //Database
        public string StartUpPath;

        //grammars
        public Grammar R_shellcommandgrammar;
        public Grammar R_systemcommandgrammar;
        public Grammar R_spoken;
        public Grammar R_VideoGrammar;
        public Grammar R_SongGrammar;

        //misc
        public string R_QEvent;
        public string R_AEvent;

        //User info
        public string R_userName = Environment.UserName;

        //rio Speak methods
        public string R_Speak(string phrase)
        {
            appendTextToDisplay(phrase);
            RIO.Speak(phrase);
            return phrase;
        }
        public string R_SpeakAsync(string phrase)
        {
            appendTextToDisplay(phrase);
            RIO.SpeakAsync(phrase);
            return phrase;
        }
        void appendTextToDisplay(string phrase)
        {
            if (rtb != null)
            {
                rtb.AppendText("RIO: " + phrase);
            }
        }

        public void R_SpeakCancelAll()
        {
            RIO.SpeakAsyncCancelAll();
        }

    }

    public class R_db_commands
    {
        private DataTable sys_cmds = new DataTable("SysCommands");
        private DataColumn sys_cmds_col;
        private DataRow sys_cmds_row;

        public string create_sysCommands()
        {
            sys_cmds_col = new DataColumn();
            sys_cmds_col.DataType = System.Type.GetType("System.Int32");
            sys_cmds_col.ColumnName = "id";
            sys_cmds_col.Unique = true;

            string cmd = "";

            return cmd;
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
            public string ws_get_weather()
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
