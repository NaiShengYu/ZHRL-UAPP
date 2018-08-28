using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Tools
{
    public class ConstConvertUtils
    {
        public static string GridTaskStatus2String(int status)
        {
            string des = "";
            switch (status)
            {
                case 1:
                    des = "已上报";
                    break;
                case 2:
                    des = "处理中";
                    break;
                case 3:
                    des = "已处理";
                    break;
            }
            return des;
        }

        public static int GridTaskStatus2Int(string status)
        {
            if ("已上报".Equals(status))
            {
                return 1;
            }
            else if ("处理中".Equals(status))
            {
                return 2;
            }
            else if ("已处理".Equals(status))
            {
                return 3;
            }
            return 1;
        }
        public static string GridTaskType2String(int type)
        {
            string des = "";
            switch (type)
            {
                case 1:
                    des = "恶臭事件";
                    break;
                case 2:
                    des = "污水偷排事件";
                    break;
            }
            return des;
        }

        public static int GridTaskType2Int(string type)
        {
            if ("恶臭事件".Equals(type))
            {
                return 1;
            }
            else if ("污水偷排事件".Equals(type))
            {
                return 2;
            }
            return 1;
        }
    }
}
