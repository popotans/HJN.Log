using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HJN.Log
{
    public class CustomLogMsg
    {
        public string LogLevel { get; set; }
        public DateTime LogDate { get; set; }
        public string Logger { get; set; }
        public string MessageObj { get; set; }
        public string Message { get; set; }

        public string IP { get; set; }

        public CustomLogMsg CreateMsg(string msg)
        {
            this.MessageObj = msg;
            return this;
        }

        public CustomLogMsg CreateMsg(Exception exception)
        {
            this.MessageObj = exception.ToString();
            return this;
        }

        public CustomLogMsg CreateMsg(string msg, Exception exception)
        {
            this.MessageObj = msg + "," + exception.ToString();
            return this;
        }
    }
}
