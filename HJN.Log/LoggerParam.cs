using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJN.Log
{
    public class LoggerParam
    {
        public const string ConnectionType_SqlServer = "System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string ConnectionType_MySql = "MySql.Data.MySqlClient.MySqlConnection, MySql.Data";

        public string LoggerName { get; set; }
        public AppenderParam FileAppenderParam { get; private set; }
        public DBAppenderParam DBAppenderParam { get; set; }
        public bool UseFile { get; set; }
        public bool UseDb { get; set; }

        public LoggerParam(string loggername)
        {
            this.UseFile = true;
            this.UseDb = false;
            this.LoggerName = loggername;
            this.FileAppenderParam = new AppenderParam() { AppenderName = loggername + "_FileApender", ModuleName = loggername };
        }

        public static LoggerParam Create(string loggername, bool usedb = false)
        {
            if (string.IsNullOrEmpty(loggername)) loggername = "default";
            LoggerParam lp = new LoggerParam(loggername);
            if (usedb) lp.UseDb = usedb;
            return lp;
        }

        public DBAppenderParam CreaterDbAppender(List<DBAppenderCommandTextColumn> ColList, string tableName, string connectionType, string connectionString)
        {

            for (int ii = 0; ii < ColList.Count; ii++)
            {
                if (ColList[ii].ParameterName == "Logger")
                {
                    ColList[ii] = new DBAppenderCommandTextColumn() { ParameterName = "Logger", DbType = "string", ConversionPattern = "%logger", Size = 100 };
                }
                if (ColList[ii].ParameterName == "LogLevel")
                {
                    ColList[ii] = new DBAppenderCommandTextColumn() { ParameterName = "LogLevel", DbType = "string", ConversionPattern = "%level", Size = 50, };
                }
                if (ColList[ii].ParameterName == "LogDate")
                {
                    ColList[ii] = new DBAppenderCommandTextColumn() { ParameterName = "LogDate", DbType = "DateTime", ConversionPattern = "%d", Size = 0 };
                }
            }

            var paramList = ColList.Select(x => x.ParameterName);
            if (!paramList.Contains("Logger"))
                ColList.Add(new DBAppenderCommandTextColumn() { ParameterName = "Logger", DbType = "string", ConversionPattern = "%logger", Size = 100 });
            if (!paramList.Contains("LogLevel"))
                ColList.Add(new DBAppenderCommandTextColumn() { ParameterName = "LogLevel", DbType = "string", ConversionPattern = "%level", Size = 50, });
            if (!paramList.Contains("LogDate"))
                ColList.Add(new DBAppenderCommandTextColumn() { ParameterName = "LogDate", DbType = "DateTime", ConversionPattern = "%d", Size = 0 });
            if (!paramList.Contains("Message"))
                ColList.Add(new DBAppenderCommandTextColumn() { ParameterName = "Message", DbType = "string", ConversionPattern = "%message", Size = 4000 });
            if (!paramList.Contains("MessageObj"))
                ColList.Add(new DBAppenderCommandTextColumn() { ParameterName = "MessageObj", DbType = "string", ConversionPattern = "", Size = 4000 });
            if (!paramList.Contains("Exception"))
                ColList.Add(new DBAppenderCommandTextColumn() { ParameterName = "Exception", DbType = "string", ConversionPattern = "%exception", Size = 4000 });

            HJN.Log.DBAppenderParam adpp = new DBAppenderParam()
            {
                AppenderName = LoggerName + "_DbAppender",
                ConnectionString = connectionString,
                ConnectionType = connectionType,
                DBAppenderCommandTextColumnList = ColList
            };

            StringBuilder sb = new StringBuilder();
            //INSERT INTO [ServiceLog]([AppId],[ServiceId],[Name],[IP],[ReferenceId],[Tag],[LogTime],[Result],[RequestParam],[RequestResult],
            //[LocalCost],[RemoteCost]) 
            //VALUES(@AppId,@ServiceId,@Name,@IP,@ReferenceId,@Tag,@LogTime,@Result,@RequestParam,@RequestResult,@LocalCost,@RemoteCost)
            string specialchar1 = "[", specialchar2 = "]"; ;
            if (connectionType.Contains("MySql.Data.MySqlClient.MySqlConnection")) { specialchar1 = specialchar2 = "`"; }

            sb.AppendFormat("INSERT INTO {1}{0}{2}(", tableName, specialchar1, specialchar2);
            int i = 0;
            foreach (var item in ColList)
            {
                i++;
                if (i != ColList.Count)
                {
                    sb.AppendFormat("{1}{0}{2},", item.ParameterName, specialchar1, specialchar2);
                }
                else
                {
                    sb.AppendFormat("{1}{0}{2}", item.ParameterName, specialchar1, specialchar2);
                }
            }
            sb.AppendFormat(")VALUES(");
            i = 0;
            foreach (var item in ColList)
            {
                i++;
                if (i != ColList.Count)
                {
                    sb.AppendFormat("@{0},", item.ParameterName);
                }
                else
                {
                    sb.AppendFormat("@{0}", item.ParameterName);
                }
            }
            sb.AppendFormat(")");
            adpp.CommandText = sb.ToString();
            this.UseDb = true;
            return adpp;
        }
    }

    public class AppenderParam
    {
        public string AppenderName { get; set; }
        public string ModuleName { get; set; }
    }

    public class DBAppenderParam
    {
        public string AppenderName { get; set; }
        public string ConnectionType { get; set; }
        public string ConnectionString { get; set; }
        public string CommandText { get; set; }

        public List<DBAppenderCommandTextColumn> DBAppenderCommandTextColumnList { get; set; }
    }

    public class DBAppenderCommandTextColumn
    {
        public string ParameterName { get; set; }
        public string DbType { get; set; }
        public string ConversionPattern { get; set; }
        public int Size { get; set; }
    }
}
