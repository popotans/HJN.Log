using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJN.Log
{
    public class LogExceptionHandler : IErrorHandler
    {
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            Console.WriteLine(e);
        }
        public void Error(string message, Exception e)
        {
            Console.WriteLine(e);
        }
        public void Error(string message)
        {
            Console.WriteLine(message);
        }
    }
}
