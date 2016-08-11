using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJN.Log
{
    /// <summary>
    /// 自定义PatternLayout
    /// </summary>
    public class ReflectionLayout : PatternLayout
    {
        public ReflectionLayout()
        {
            this.AddConverter("property", typeof(ReflectionPatternConverter));
        }
    }
}
