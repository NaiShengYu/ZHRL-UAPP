using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class EmergencyAccidentDetailModel 
	{
        public class EmergencyAccidentDetailModelBean {
            /// 纬度
            public double Lat { get; set; }
            /// 经度
            public double Lng { get; set; }
            /// 排序索引
            public int Index { get; set; }
            public Guid IncidentId { get; set; }
            /// 删除状态
            public bool IsDeleted { get; set; }
            /// 因子编号
            public Guid FactorId { get; set; }
            /// 因子名称
            public string FactorName { get; set; }
            /// 测试方法编号
            public Guid TestMethodId { get; set; }
            /// 测试方法名称
            public string TestMethodName { get; set; }
            /// 计量单位编号
            public Guid UnitId { get; set; }
            /// 计量单位名称
            public string UnitName { get; set; }
            /// 测量设备编号
            public Guid? EquipmentId { get; set; }
            /// 测量设备名称
            public string EquipmentName { get; set; }
            /// 因子的值
            public decimal FactorValue { get; set; }          
        }
        public abstract class IncidentLoggingEvent 
        {
            /// <summary>
            /// 纬度
            /// </summary>
            public double Lat { get; set; }

            /// <summary>
            /// 经度
            /// </summary>
            public double Lng { get; set; }

            /// <summary>
            /// 排序索引
            /// </summary>
            public int Index { get; set; }


            public Guid IncidentId { get; set; }



            /// <summary>
            /// 删除状态
            /// </summary>
            public bool IsDeleted { get; set; }


        }



        public class IncidentFactorIdentificationEvent : IncidentLoggingEvent
        {
            [Required]
            /// <summary>
            /// 因子编号
            /// </summary>
            public Guid FactorId { get; set; }
            /// <summary>
            /// 因子名称
            /// </summary>
            public string FactorName { get; set; }


            public IncidentFactorIdentificationEvent()
            {
            }
        }


        public class IncidentFactorMeasurementEvent : IncidentLoggingEvent
        {
            [Required]
            /// <summary>
            /// 因子编号
            /// </summary>
            public Guid FactorId { get; set; }
            /// <summary>
            /// 因子名称
            /// </summary>
            public string FactorName { get; set; }

            /// <summary>
            /// 测试方法编号
            /// </summary>
            public Guid? TestMethodId { get; set; }
            /// <summary>
            /// 测试方法名称
            /// </summary>
            public string TestMethodName { get; set; }

            [Required]
            /// <summary>
            /// 计量单位编号
            /// </summary>
            public Guid UnitId { get; set; }
            /// <summary>
            /// 计量单位名称
            /// </summary>
            public string UnitName { get; set; }

            /// <summary>
            /// 测量设备编号
            /// </summary>
            public Guid? EquipmentId { get; set; }

            /// <summary>
            /// 测量设备名称
            /// </summary>
            public string EquipmentName { get; set; }

            [Required]
            /// <summary>
            /// 因子的值
            /// </summary>
            public decimal FactorValue { get; set; }

            /// <summary>
            /// 对应的事故性质
            /// </summary>
            public Nature IncidentNature { get; set; }

            public IncidentFactorMeasurementEvent()
            {
            }
        }


        public class IncidentLocationSendingEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 目标点位纬度
            /// </summary>
            public double TargetLat { get; set; }

            /// <summary>
            /// 目标点位经度
            /// </summary>
            public double TargetLng { get; set; }

            /// <summary>
            /// 目标点位地址
            /// </summary>
            public string TargetAddress { get; set; }

            public IncidentLocationSendingEvent()
            {
            }
        }


        public class IncidentMessageSendingEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            public IncidentMessageSendingEvent()
            {
            }
        }


        public class IncidentNameModificationEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 原始名称
            /// </summary>
            public string Original { get; set; }
            /// <summary>
            /// 当前名称
            /// </summary>
            public string Current { get; set; }

            public IncidentNameModificationEvent()
            {
            }
        }


        public class IncidentNatureIdentificationEvent : IncidentLoggingEvent
        {
            [Required]
            /// <summary>
            /// 事故性质字符串(000分别对应 气 水 土)
            /// </summary>
            public string NatureString { get; set; }


            public IncidentNatureIdentificationEvent()
            {
            }
        }

        public class IncidentOccurredTimeRespecifyingEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 原始时间
            /// </summary>
            public DateTime Original { get; set; }
            /// <summary>
            /// 当前时间
            /// </summary>
            public DateTime Current { get; set; }

            public IncidentOccurredTimeRespecifyingEvent()
            {
            }
        }

        public class IncidentPictureSendingEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 存储路径(相对路径)
            /// </summary>
            public string StorePath { get; set; }
            /// <summary>
            /// 宽度(单位px)
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// 高度(单位px)
            /// </summary>
            public int Height { get; set; }

            public IncidentPictureSendingEvent()
            {
            }
        }

        public class IncidentPlanGenerationEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 方案存储地址
            /// </summary>
            public string StoreUrl { get; set; }

            public IncidentPlanGenerationEvent()
            {
            }
        }


        public class IncidentReportGenerationEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 报告编号
            /// </summary>
            public Guid ReportId { get; set; }

            /// <summary>
            /// 报告名称
            /// </summary>
            public string ReportName { get; set; }

            public IncidentReportGenerationEvent()
            {
            }
        }


        public class IncidentVoiceSendingEvent : IncidentLoggingEvent
        {
            /// <summary>
            /// 存储路径(相对路径)
            /// </summary>
            public string StorePath { get; set; }
            /// <summary>
            /// 内容长度(单位秒)
            /// </summary>
            public int Length { get; set; }

            public IncidentVoiceSendingEvent()
            {
            }
        }

        public class IncidentWindDataSendingEvent : IncidentLoggingEvent
        {
            //对应旧表Remark字段 风向,风速

            /// <summary>
            /// 风向(单位 度)
            /// </summary>
            public decimal Direction { get; set; }

            /// <summary>
            /// 风速(单位 米/秒)
            /// </summary>
            public decimal Speed { get; set; }

            public IncidentWindDataSendingEvent()
            {
            }
        }
    }
}