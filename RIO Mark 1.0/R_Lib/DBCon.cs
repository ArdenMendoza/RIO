using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

//database
using System.Data.SqlServerCe;


namespace R_Lib
{
    public class R_db_conn
    {
        public SqlCeCommand cmd { get; set; }
        public SqlCeDataReader rdr { get; set; }
        public SqlCeConnection conn { get; set; }
        public db_scripts db_scripts = new db_scripts();
        public string R_conn_string { get; set; }

        public string R_db_path { get; set; }
        public R_db_conn(string v1)
        {
            R_db_path = v1;
        }

        public void conn_init()
        {
            conn = new SqlCeConnection();

            //set ConnectionString
            if (R_db_path.Substring(R_db_path.Length - 4, 4) != ".sdf")
            {
                R_db_path = R_db_path + @"\RIODB.sdf";
            }

            R_conn_string = @"Data Source=" + R_db_path + "; Password=RIODB01";
            conn.ConnectionString = R_conn_string;

            //set cmd command's connection
            cmd = conn.CreateCommand();

            //set db engine for database.
            SqlCeEngine en = new SqlCeEngine();
            en.LocalConnectionString = R_conn_string;

            //If db is not existing, create it.    
            if (!File.Exists(R_db_path))
            {
                try
                {
                    en.CreateDatabase();
                    //speech.R_Speak("Database created.");
                    Execute_Scripts();
                    Insert_default_data();
                }
                catch (Exception ex)
                {
                    //speech.R_Speak("Error while setting up database('conn_innit()'): \r\n" + ex.Message);
                }
            }

            #region upgrade engine if old version of sqlce is used.
            try { en.Upgrade(R_conn_string); } catch { }
            #endregion upgrade engine

        }
        private void Execute_Scripts()
        {
            db_scripts.add_list_items(); //Populate lists of insert commands for default data

            List<string> cmd_Array = new List<string>();
            foreach (string i in db_scripts.cmd_createTable_Commands)
            {
                cmd_Array.Add(i);
            }

            try
            {
                conn.Open();
                foreach (string i in cmd_Array)
                {
                    cmd.CommandText = i;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Table Created.\r\n" + i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating table @ create_Tables() function. \r\n" + ex.Message);
            }
            finally { conn.Close(); }

        }
        private void Insert_default_data()
        {
            try
            {
                conn.Open();

                foreach (string i in db_scripts.cmd_TblCmdTypes_Inserts)
                {
                    cmd.CommandText = i;
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Default data for tblCmdTypes inserted! Count: " + db_scripts.cmd_TblCmdTypes_Inserts.Count().ToString());

                foreach (string i in db_scripts.cmd_TblSysCommands_Inserts)
                {
                    cmd.CommandText = i;
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Default data for tblCmdTypes inserted! Count: " + db_scripts.cmd_TblSysCommands_Inserts.Count().ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while inserting dataset @ Insert_default_data() function. \r\n" + ex.Message);
            }
            finally { conn.Close(); }
        }

        public void exec_method(string methodName)
        {
            //Type calledType = Type.GetType();

            //// Invoke the method itself. The string returned by the method winds up in s
            ////String s = (String)calledType.InvokeMember(
            //calledType.InvokeMember(
            //                methodName,
            //                BindingFlags.InvokeMethod | BindingFlags.Public |
            //                    BindingFlags.Static,
            //                null,
            //                null,
            //                null);

            //// Return the string that was returned by the called method.
            //return s;
        }
    }
}
