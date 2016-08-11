using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJN.Log
{
    public class MyLog
    {
        private List<LoggerParam> LoggerList = new List<LoggerParam>();
        public Encoding Encoding = Encoding.GetEncoding("utf-8");

        public MyLog(string encoding = "utf-8")
        {
            if (string.Compare("utf-8", encoding, true) != 0)
                Encoding = Encoding.GetEncoding("utf-8");
        }

        private MyLog(List<LoggerParam> LoggerList)
        {
            this.Encoding = Encoding.GetEncoding("utf-8"); ;
            this.LoggerList = LoggerList;
            LoadConfig();
        }

        private MyLog(List<LoggerParam> LoggerList, Encoding encoding)
        {
            if (encoding == null) encoding = Encoding.GetEncoding("utf-8");
            this.Encoding = encoding;
            this.LoggerList = LoggerList;
            LoadConfig();
        }

        public void AddLogger(LoggerParam loggerParam)
        {
            if (LoggerList == null) LoggerList = new List<LoggerParam>();
            LoggerList.Add(loggerParam);
        }

        public void LoadConfig()
        {
            StringBuilder sb = new StringBuilder();
            //  StreamReader sr = new StreamReader(@"J:\njhdisk\ProjectStudy\MyLog4NetWrapper\MyLog4NetWrapper\log4net.config", Encoding.GetEncoding("utf-8"));
            //sb.Append(sr.ReadToEnd());
            //  sr.Dispose();

            string configContent = new ConfigBuilder().BuildFull(LoggerList);

            byte[] array = Encoding.GetBytes(configContent);
            using (MemoryStream stream = new MemoryStream(array))
            {
                XmlConfigurator.Configure(stream);
            }
        }

        public ILog GetLogger(string loggerName)
        {
            foreach (var item in this.LoggerList)
            {
                if (string.Compare(loggerName, item.LoggerName, true) == 0) return LogManager.GetLogger(loggerName);
            }
            throw new ArgumentException("logger " + loggerName + " is not exists");
        }
    }
}
