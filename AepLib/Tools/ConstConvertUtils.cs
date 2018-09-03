using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Tools
{
    public class ConstConvertUtils
    {
        public static string GridLevel2String(int level)
        {
            string des = "";
            switch (level)
            {
                case 1:
                    des = "调度中心";
                    break;
                case 2:
                    des = "乡级网格";
                    break;
                case 3:
                    des = "村级网格";
                    break;
            }
            return des;
        }

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
            else if ("虚假事件".Equals(status))
            {
                return 4;
            }
            return 1;
        }

        public static string GridEventType2String(int type)
        {
            string des = "未知类型 ";
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

        /// <summary>
        /// 事件类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GridEventType2Int(string type)
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

        /// <summary>
        /// 网格化-下发信息的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GridInformationType2String(int type)
        {
            string des = "政策法规";
            switch (type)
            {
                case 1:
                    des = "政策法规";
                    break;
            }
            return des;
        }

        /// <summary>
        /// 任务性质int To string 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TaskNatureType2String(int type)
        {
            string des = "日常任务";
            switch (type)
            {
                case 1:
                    des = "日常任务";
                    break;

                case 2:
                    des = "事件任务";
                    break;
            }
            return des;
        }

        /// <summary>
        /// 任务性质 string To int 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int TaskNatureString2Type(string nature)
        {
            if ("日常任务".Equals(nature))
            {
                return 1;
            }
            else if ("事件任务".Equals(nature))
            {
                return 2;
            }
            return 1;
        }


        /// <summary>
        /// 任务状态int To string 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TaskStateType2String(int type)
        {
            string des = "未下发";
            switch (type)
            {
                case 0:
                    des = "未下发";
                    break;

                case 1:
                    des = "未审核";
                    break;

                case 2:
                    des = "已审核";
                    break;
                case 3:
                    des = "已撤销";
                    break;
                case 4:
                    des = "执行中";
                    break;
                case 5:
                    des = "退回";
                    break;
                case 6:
                    des = "任务完结";
                    break;
            }
            return des;
        }

        /// <summary>
        /// 任务性质 string To int 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int TaskStateString2Type(string nature)
        {
            if ("未下发".Equals(nature))
            {
                return 0;
            }
            else if ("未审核".Equals(nature))
            {
                return 1;
            }
            else if ("已审核".Equals(nature))
            {
                return 2;
            }
            else if ("已撤销".Equals(nature))
            {
                return 3;
            }
            else if ("执行中".Equals(nature))
            {
                return 4;
            }
            else if ("退回".Equals(nature))
            {
                return 5;
            }
            else if ("任务完结".Equals(nature))
            {
                return 6;
            }
            return 1;
        }


    }
}
