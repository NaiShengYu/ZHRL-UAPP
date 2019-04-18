using AepApp.Models;
using AepApp.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AepApp.Tools
{
    public class ConstConvertUtils
    {
        public static string GridLevel2String(int level)
        {
            string des = "";
            if (HomePagePage.gridCells != null)
            {
                foreach (var item in HomePagePage.gridCells)
                {
                    if (item.id == level)
                    {
                        return item.name;
                    }
                }
            }
            //switch (level)
            //{
            //    case 1:
            //        des = "调度中心";
            //        break;
            //    case 2:
            //        des = "乡级网格";
            //        break;
            //    case 3:
            //        des = "村级网格";
            //        break;
            //}
            return des;
        }

        /// <summary>
        /// 所有的事件状态
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllEventStatus()
        {
            List<string> list = new List<string>();
            list.Add("已上报");
            list.Add("处理中");
            list.Add("已处理");
            list.Add("虚假事件");
            return list;
        }

        /// <summary>
        /// 事件状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GridEventStatus2String(int status)
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
                case 4:
                    des = "虚假事件";
                    break;
            }
            return des;
        }

        public static int GridEventStatus2Int(string status)
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

        /// <summary>
        /// 所有的事件类型
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllEventType()
        {
            List<string> list = new List<string>();
            list.Add("恶臭事件");
            list.Add("污水偷排事件");
            return list;
        }


        public static string GridEventType2String(int type)
        {
            string des = "恶臭事件";
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
        /// 所有的任务类型
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllTaskType()
        {
            List<string> list = new List<string>();
            list.Add("事件任务");
            list.Add("日常任务");
            return list;
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
        public static int TaskNatureString2Int(string nature)
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
        /// 所有的任务状态
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllTaskState()
        {
            List<string> list = new List<string>();
            list.Add("未下发");
            list.Add("未审核");
            list.Add("已审核");
            list.Add("已撤销");
            list.Add("执行中");
            list.Add("退回");
            list.Add("任务完结");
            return list;
        }

        /// <summary>
        /// 任务状态int To string 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TaskState2String(int type)
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
        /// 任务状态 string To int 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int TaskStateString2Int(string nature)
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

        /// <summary>
        /// 获取固定剂（采样模块）
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<MultiSelectDataType> GetFixer()
        {
            return new ObservableCollection<MultiSelectDataType>(new[]
            {
                new MultiSelectDataType
                {
                    Id = "1",
                    Name = "硫酸"
                },
                new MultiSelectDataType
                {
                    Id = "2",
                    Name = "硝酸"
                },
                new MultiSelectDataType
                {
                    Id = "3",
                    Name = "甲苯"
                },
                new MultiSelectDataType
                {
                    Id = "4",
                    Name = "甲醛"
                },
                new MultiSelectDataType
                {
                    Id = "5",
                    Name = "硫酸铜"
                },
                new MultiSelectDataType
                {
                    Id = "6",
                    Name = "三氯甲烷"
                },
                new MultiSelectDataType
                {
                    Id = "7",
                    Name = "氢氧化钠"
                }
            });
        }

        public static int getTaskTypeCode(string typeDes)
        {
            if ("地表水".Equals(typeDes))
            {
                return 0;
            }
            else if ("废水".Equals(typeDes))
            {
                return 1;
            }
            else if ("饮用水".Equals(typeDes))
            {
                return 2;
            }
            else if ("废气".Equals(typeDes))
            {
                return 3;
            }
            else if ("环境空气".Equals(typeDes))
            {
                return 4;
            }
            else if ("室内空气".Equals(typeDes))
            {
                return 5;
            }
            else if ("农业用地".Equals(typeDes))
            {
                return 6;
            }
            else if ("工业用地".Equals(typeDes))
            {
                return 7;
            }
            return 0;
        }

    }
}
