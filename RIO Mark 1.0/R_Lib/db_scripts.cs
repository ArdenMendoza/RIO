using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R_Lib
{
    public class db_scripts
    {
        public List<string> cmd_TblCmdTypes_Inserts = new List<string>();
        public List<string> cmd_TblSysCommands_Inserts = new List<string>();
        public List<string> cmd_createTable_Commands = new List<string>();

        public class sysCommand
        {
            public sysCommand(int v1, string v2, string v3, string v4)
            {
                cmdType = v1;
                cmd = v2;
                QEvent = v3;
                resp = v4;
            }

            public int cmdType { get; set; }
            public string cmd { get; set; }
            public string QEvent { get; set; }
            public string resp { get; set; }
        }

        public void add_list_items()
        {
            createTable_Commands();
            TblCmdTypes_Inserts();
            TblSysCommands_Inserts();
        }

        public void TblCmdTypes_Inserts()
        {
            string[] cmdTypes = {
            "creator info",
            "greetings",
            "RIO interface",
            "custom command",
            "date and time",
            "listbox",
            "volume",
            "directory",
            "weather",
            "media",
            "windows interface",
            "email",
            "confirmation",
            "google search",
            "facebook"
            };
            foreach (string cmdType in cmdTypes)
            {
                cmd_TblCmdTypes_Inserts.Add("Insert into TblCmdTypes(CmdName) VALUES('" + cmdType + "');");
            }

        }

        public void TblSysCommands_Inserts()
        {
            sysCommand[] sysCommands = {
                new sysCommand(1, "who created you", "Default", "creatorInfo"),
                new sysCommand(1, "who is your creator", "Default", "creatorInfo"),
                new sysCommand(2, "rio", "Default", "R_Greet"),
                new sysCommand(2, "hello rio", "Default", "R_Greet"),
                new sysCommand(2, "hey rio", "Default", "R_Greet"),
                new sysCommand(2, "hey buddy", "Default", "R_Greet"),
                new sysCommand(3, "rio hide", "Default", "UI_onSystemTrayrio"),
                new sysCommand(3, "hide rio", "Default", "UI_onSystemTrayrio"),
                new sysCommand(3, "hide", "Default", "UI_onSystemTrayrio"),
                new sysCommand(3, "to the system tray", "Default", "UI_onSystemTrayrio"),
                new sysCommand(3, "rio up top", "Default", "UI_rioOnTop"),
                new sysCommand(3, "rio come here", "Default", "UI_rioOnTop"),
                new sysCommand(3, "come here", "Default", "UI_rioOnTop"),
                new sysCommand(3, "show", "Default", "UI_rioOnTop"),
                new sysCommand(3, "come back", "Default", "UI_rioOnTop"),
                new sysCommand(3, "come out rio", "Default", "UI_rioOnTop"),
                new sysCommand(3, "come out", "Default", "UI_rioOnTop"),
                new sysCommand(3, "rio restore", "Default", ""),
                new sysCommand(3, "restore", "Default", ""),
                new sysCommand(3, "to the center", "Default", ""),
                new sysCommand(3, "default size", "Default", ""),
                new sysCommand(3, "restore size", "Default", ""),
                new sysCommand(3, "maximize rio", "Default", ""),
                new sysCommand(3, "expand rio", "Default", ""),
                new sysCommand(3, "expand", "Default", ""),
                new sysCommand(3, "rio expand", "Default", ""),
                new sysCommand(3, "size up", "Default", ""),
                new sysCommand(3, "size down", "Default", ""),
                new sysCommand(3, "minimize rio", "Default", ""),
                new sysCommand(3, "show desktop", "Default", ""),
                new sysCommand(3, "minimize everything", "Default", ""),
                new sysCommand(3, "minimize all", "Default", ""),
                new sysCommand(3, "scroll up", "Default", ""),
                new sysCommand(3, "scroll down", "Default", ""),
                new sysCommand(3, "stop", "Default", ""),
                new sysCommand(4, "add command", "Default", "addCommand"),
                new sysCommand(4, "add commands", "Default", "addCommand"),
                new sysCommand(4, "close command window", "Default", "closeCommandWindow"),
                new sysCommand(4, "update commands", "Default", "UpdateCommands"),
                new sysCommand(4, "show system commands", "Default", "ShowSystemCommands"),
                new sysCommand(4, "system commands", "Default", "ShowSystemCommands"),
                new sysCommand(4, "off you go", "Default", "UI_Closerio"),
                new sysCommand(4, "bye rio", "Default", "UI_Closerio"),
                new sysCommand(5, "what''s the time", "Default", "timecheck"),
                new sysCommand(5, "time check", "Default", "timecheck"),
                new sysCommand(5, "what time is it", "Default", "timecheck"),
                new sysCommand(5, "time please", "Default", "timecheck"),
                new sysCommand(5, "what day is it", "Default", "dayCheck"),
                new sysCommand(5, "what date is it", "Default", "dateCheck"),
                new sysCommand(5, "date please", "Default", "dateCheck"),
                new sysCommand(5, "what''s the date", "Default", "dateCheck"),
                new sysCommand(5, "what''s the time", "Default", "timecheck"),
                new sysCommand(6, "show listbox", "Default", ""),
                new sysCommand(6, "hide listbox", "Default", ""),
                new sysCommand(6, "expand listbox", "Default", ""),
                new sysCommand(6, "expand list", "Default", ""),
                new sysCommand(6, "restore listbox", "Default", ""),
                new sysCommand(6, "restore list", "Default", ""),
                new sysCommand(7, "increase volume", "Default", "volumeUp"),
                new sysCommand(7, "volume up", "Default", "volumeUp"),
                new sysCommand(7, "decrease volume", "Default", "volumeDown"),
                new sysCommand(7, "volume down", "Default", "volumeDown"),
                new sysCommand(7, "mute", "Default", "mute"),
                new sysCommand(7, "unmute", "Default", ""),
                new sysCommand(8, "load music directory", "Default", ""),
                new sysCommand(8, "load video directory", "Default", ""),
                new sysCommand(8, "browse for directory", "Default", ""),
                new sysCommand(8, "browse  music directory", "Default", ""),
                new sysCommand(8, "change video directory", "Default", ""),
                new sysCommand(8, "change picture directory", "Default", ""),
                new sysCommand(9, "how''s the weather", "Default", "getWeatherUpdate"),
                new sysCommand(9, "weather update rio", "Default", "getWeatherUpdate"),
                new sysCommand(9, "weather update", "Default", "getWeatherUpdate"),
                new sysCommand(9, "what''s tomorrow''s forecast", "Default", "getWeatherForecast"),
                new sysCommand(9, "is it going to rain tomorrow", "Default", "getWeatherForecast"),
                new sysCommand(9, "weather forecast", "Default", "getWeatherForecast"),
                new sysCommand(10, "create media grammar", "Default", ""),
                new sysCommand(10, "play music", "Default", ""),
                new sysCommand(10, "play song", "Default", ""),
                new sysCommand(10, "play video", "Default", ""),
                new sysCommand(10, "play movie", "Default", ""),
                new sysCommand(10, "watch video", "Default", ""),
                new sysCommand(10, "watch movie", "Default", ""),
                new sysCommand(11, "switch window", "Default", ""),
                new sysCommand(11, "close window", "Default", ""),
                new sysCommand(11, "close app", "Default", ""),
                new sysCommand(11, "close application", "Default", ""),
                new sysCommand(11, "switch tab", "Default", ""),
                new sysCommand(11, "next tab", "Default", ""),
                new sysCommand(11, "previous tab", "Default", ""),
                new sysCommand(12, "check for new emails", "Default", "getUnreadEmails"),
                new sysCommand(12, "check email", "Default", "getUnreadEmails"),
                new sysCommand(12, "read email", "ScanEmail", "nextEmail"),
                new sysCommand(12, "next email", "ScanEmail", "previousEmail"),
                new sysCommand(13, "previous email", "ScanEmail", "QEvent_Email_Yes"),
                new sysCommand(13, "yes", "ReadEmail", "QEvent_Email_Yes"),
                new sysCommand(13, "yep", "ReadEmail", "QEvent_Email_Yes"),
                new sysCommand(13, "please", "ReadEmail", "QEvent_Email_Yes"),
                new sysCommand(13, "yes please", "ReadEmail", "QEvent_Email_No"),
                new sysCommand(13, "no", "ReadEmail", "QEvent_Email_No"),
                new sysCommand(13, "maybe later", "ReadEmail", "QEvent_Email_No"),
                new sysCommand(13, "nope", "ReadEmail", ""),
                new sysCommand(14, "search google", "Default", "google_search"),
                new sysCommand(14, "google search", "Default", "google_search"),
                new sysCommand(11, "facebook feed", "Default", "")
            };

            foreach (sysCommand syscmd in sysCommands)
            {
                cmd_TblSysCommands_Inserts.Add("Insert into TblSysCommands(CmdType, Cmd, Q_Event, Method_resp) VALUES(" +
                    syscmd.cmdType.ToString() + "," +
                    "'" + syscmd.cmd + "'," +
                    "'" + syscmd.QEvent + "'," +
                    "'" + syscmd.resp + "');");
            }
        }

        public void createTable_Commands()
        {
            cmd_createTable_Commands.Add("Create table TblSysCommands(id int IDENTITY(1,1) , CmdType int,Cmd nvarchar(200), Q_Event nvarchar(200), Method_resp nvarchar(200), PRIMARY KEY (id));");
            cmd_createTable_Commands.Add("Create table TblAudioDetails(id int IDENTITY(1,1) , SongTitle nvarchar(200), SongArtist nvarchar(200), SongPath nvarchar(200), SongGrammar nvarchar(200),PRIMARY KEY (id));");
            cmd_createTable_Commands.Add("Create table TblCmdTypes(id int IDENTITY(1,1) , CmdName nvarchar(200),PRIMARY KEY (id));");
            cmd_createTable_Commands.Add("Create table TblUserCommands(id int IDENTITY(1,1) , UsrCmds nvarchar(200), JrvsRspns nvarchar(200), ShellLoc nvarchar(200), PRIMARY KEY (id));");
            cmd_createTable_Commands.Add("Create table TblVidDetails(id int IDENTITY(1,1) , VidPath nvarchar(200), VidName nvarchar(200), PRIMARY KEY (id));");
        }
    }
}
