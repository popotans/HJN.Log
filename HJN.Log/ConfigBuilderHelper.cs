using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJN.Log
{
    public class ConfigBuilder
    {
        public string BuildFull(List<LoggerParam> loggerlist)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.Append("<configuration><configSections>");
            sb.Append("<section name=\"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler\" requirePermission=\"false\"/></configSections>");
            sb.Append("<appender>");
            sb.Append("<errorHandler type=\"HJN.Log.LogExceptionHandler,HJN.Log\" /></appender>");
            sb.Append("<log4net>");
            //BUILDLOGGERS
            string loggerStr = "";
            foreach (LoggerParam loggerParam in loggerlist)
            {
                loggerStr = BuildLogger(loggerParam);
                sb.Append(loggerStr);
            }

            //BUILDAPPENDERS
            loggerStr = "";
            string level = "";
            foreach (var item in loggerlist)
            {
                if (item.UseFile)
                {
                    level = "info";
                    loggerStr = BuildFileAppender(item.FileAppenderParam.AppenderName + "_" + level, item.FileAppenderParam.ModuleName, level);
                    sb.Append(loggerStr);
                    level = "error";
                    loggerStr = BuildFileAppender(item.FileAppenderParam.AppenderName + "_" + level, item.FileAppenderParam.ModuleName, level);
                    sb.Append(loggerStr);
                    level = "debug";
                    loggerStr = BuildFileAppender(item.FileAppenderParam.AppenderName + "_" + level, item.FileAppenderParam.ModuleName, level);
                    sb.Append(loggerStr);
                }

                if (item.UseDb)
                {
                    loggerStr = BuildDbAppender(item.DBAppenderParam.AppenderName + "_db", item.DBAppenderParam);
                    sb.Append(loggerStr);
                }
            }

            sb.Append("</log4net>");
            sb.Append("</configuration>");
            return sb.ToString();
        }

        private string BuildLogger(LoggerParam logpam)
        {
            //string loggerName, string replateAppenderName, 
            string loggerName = logpam.LoggerName;
            string replateAppenderName = "";
            bool useFile = logpam.UseFile;
            bool useDb = logpam.UseDb;
            StringBuilder sb = new StringBuilder();
            /* <logger name="ServiceLog">
      <level value="ALL" />
      <appender-ref ref="RollingLogFileAppenderInfo" />
    </logger>*/
            sb.AppendFormat("<logger name=\"{0}\">", loggerName);
            sb.AppendFormat("<level value=\"ALL\" />");
            if (useFile)
            {
                replateAppenderName = logpam.FileAppenderParam.AppenderName;
                sb.AppendFormat("<appender-ref ref=\"{0}_info\" />", replateAppenderName);
                sb.AppendFormat("<appender-ref ref=\"{0}_error\" />", replateAppenderName);
                sb.AppendFormat("<appender-ref ref=\"{0}_debug\" />", replateAppenderName);
            }

            if (useDb)
            {
                replateAppenderName = logpam.DBAppenderParam.AppenderName;
                sb.AppendFormat("<appender-ref ref=\"{0}_db\" />", replateAppenderName);
            }
            sb.AppendFormat("</logger>");
            return sb.ToString();
        }

        private string BuildFileAppender(string appendername, string moduleName, string level)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<appender name=\"{0}\" type=\"log4net.Appender.RollingFileAppender\">", appendername);
            if (string.IsNullOrEmpty(moduleName)) moduleName = "default";
            sb.AppendFormat("<param name=\"File\" value=\"_logs\\{0}\\{1}\\\"/>", moduleName, level);
            sb.Append("<filter type=\"log4net.Filter.LevelRangeFilter\">");
            if (string.Compare("info", level, true) == 0)
            {
                sb.Append("<param name=\"LevelMin\" value=\"INFO\"/>");
                sb.Append("<param name=\"LevelMax\" value=\"WARN\"/>");
            }
            else if (string.Compare("error", level, true) == 0)
            {
                sb.Append("<param name=\"LevelMin\" value=\"ERROR\"/>");
                sb.Append("<param name=\"LevelMax\" value=\"ERROR\"/>");
            }
            else if (string.Compare("debug", level, true) == 0)
            {
                sb.Append("<param name=\"LevelMin\" value=\"DEBUG\"/>");
                sb.Append("<param name=\"LevelMax\" value=\"DEBUG\"/>");
            }
            sb.Append("</filter>");
            sb.Append(" <param name=\"AppendToFile\" value=\"true\"/>");
            sb.Append(" <param name=\"RollingStyle\" value=\"Date\"/>");
            sb.Append("<param name=\"MaxSizeRollBackups\" value=\"100\"/>");
            sb.Append("<param name=\"MaximumFileSize\" value=\"200MB\"/>");
            sb.Append("<param name=\"StaticLogFileName\" value=\"false\"/>");
            sb.Append("<param name=\"DatePattern\" value=\"yyyyMMdd&quot;.log&quot;\"/>");
            sb.Append("<layout type=\"log4net.Layout.PatternLayout\">");
            sb.Append("<param name=\"ConversionPattern\" value=\"%d [%t]: %-5p %m%n\"/>");
            sb.Append("</layout>");
            sb.Append("</appender>");
            /*
                <appender name="RollingLogFileAppenderInfo" type="log4net.Appender.RollingFileAppender">
      <!--运行日志存放路径-->
      <param name="File" value="logs\\Info\\"/>
      <filter type="log4net.Filter.LevelRangeFilter">
        <param name="LevelMin" value="INFO"/>
        <param name="LevelMax" value="WARN"/>
      </filter>
      <param name="AppendToFile" value="true"/>
      <param name="RollingStyle" value="Date"/>
      <param name="MaxSizeRollBackups" value="100"/>
      <param name="MaximumFileSize" value="200MB"/>
      <param name="StaticLogFileName" value="false"/>
      <param name="DatePattern" value="yyyyMMdd&quot;.log&quot;"/>
      <layout type="log4net.Layout.PatternLayout">
        <param name="ConversionPattern" value="%d [%t] %-5p %m%n"/>
      </layout>
    </appender>
             */

            return sb.ToString();
        }

        private string BuildDbAppender(string appendername, DBAppenderParam DBAppenderParam)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<appender name=\"{0}\" type=\"log4net.Appender.AdoNetAppender\">", appendername);
            sb.Append("<bufferSize value=\"0\" />");
            sb.AppendFormat("<connectionType value=\"{0}\" />", DBAppenderParam.ConnectionType);
            sb.AppendFormat("<connectionString value=\"{0}\" />", DBAppenderParam.ConnectionString);
            sb.AppendFormat("<commandText value=\"{0}\" />", DBAppenderParam.CommandText);

            foreach (DBAppenderCommandTextColumn dctc in DBAppenderParam.DBAppenderCommandTextColumnList)
            {
                sb.Append("<parameter>");
                sb.AppendFormat("<parameterName value=\"@{0}\" />", dctc.ParameterName);
                sb.AppendFormat("<dbType value=\"{0}\" />", dctc.DbType);
                if (dctc.DbType == "String" || dctc.DbType == "string")
                {
                    if (dctc.Size <= 0) dctc.Size = 255;
                }
                if (dctc.Size > 0)
                {
                    sb.AppendFormat(" <size value=\"{0}\" />", dctc.Size);
                }
                if (string.IsNullOrEmpty(dctc.ConversionPattern))
                {
                    sb.Append("<layout type=\"HJN.Log.ReflectionLayout,HJN.Log\">");
                    sb.Append("<conversionPattern value=\"%property{" + dctc.ParameterName + "}\"/>");
                    sb.Append("</layout>");
                }
                else
                {
                    if (dctc.ConversionPattern == "%message" || dctc.ConversionPattern == "%m")
                    {
                        sb.Append("<layout type=\"log4net.Layout.PatternLayout\">");
                        sb.Append("<conversionPattern value=\"" + dctc.ConversionPattern + "\"/>");
                        sb.Append("</layout>");
                    }
                    else if (dctc.ConversionPattern == "%d")
                    {
                        sb.Append("<layout type=\"log4net.Layout.RawTimeStampLayout\"/>");
                    }
                    else if (dctc.ConversionPattern == "%exception")
                    {
                        sb.Append("<layout type=\"log4net.Layout.ExceptionLayout\"/>");
                    }
                    else
                    {
                        sb.Append("<layout type=\"log4net.Layout.PatternLayout\">");
                        sb.Append("<conversionPattern value=\"" + dctc.ConversionPattern + "\"/>");
                        sb.Append("</layout>");
                    }
                }

                sb.Append("</parameter>");
            }
            sb.Append("</appender>");

            return sb.ToString();
        }
    }
}
