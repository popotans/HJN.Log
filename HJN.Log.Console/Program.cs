using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HJN.Log.ConsoleTest
{
    class Program
    {
        /// <summary>
        /// 使用说明：存DB已经内置了Logger，LogLevel，LogDate 三个字段，不需要在特别添加。
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<DBAppenderCommandTextColumn> collist = new List<DBAppenderCommandTextColumn>();
            collist.Add(new DBAppenderCommandTextColumn() { ParameterName = "IP", DbType = "string", ConversionPattern = "" });

            string conn1 = "server=10.98.72.31;database=CNblogs;User Id=sa;Password=niejunhua;";
            conn1 = "server=localhost;database=hjntest;User Id=root;Password=;Charset=utf8";
            
            LoggerParam lp3 = new LoggerParam("wfservices") { UseFile = true };
            lp3.DBAppenderParam = lp3.CreaterDbAppender(collist, "log4net", LoggerParam.ConnectionType_MySql, conn1);

            MyLog log = new MyLog();
            log.AddLogger(lp3);
            log.LoadConfig();

            ILog logger = log.GetLogger("wfservices");
            Console.WriteLine(sw.Elapsed);
            logger.Info("info is now ");
            Console.WriteLine(sw.Elapsed);

            CustomLogMsg clm = new CustomLogMsg().CreateMsg("这是消息" + Guid.NewGuid());
            logger.Debug("debug");
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine(i);
                // logger.Info("begin " + i);
                try
                {
                    int df = 5;
                    int j = 7;
                    var item = 7 / ((j + df * -1) - 2);
                }
                catch (Exception ee)
                {
                    // logger.Error(clm, ee);
                }
            }


            Console.ReadKey();
        }
    }
}
