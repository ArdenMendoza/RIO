using System;
using System.Data;

namespace R_Lib
{
    public class R_db_commands
    {
        private DataColumn sys_cmds_col;

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
}
