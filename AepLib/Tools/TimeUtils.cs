using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Tools
{
    public class TimeUtils
    {
        public static string DateTime2YMD(DateTime time)
        {
            if (time == null)
            {
                return "";
            }
            return time.ToString("yyyy-MM-dd");
        }

        public static string Time2HHmm(TimeSpan time)
        {
            if(time == null)
            {
                return "";
            }
            return time.ToString("HH:mm");
        }
    }
}
