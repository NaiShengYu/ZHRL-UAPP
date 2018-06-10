using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class UploadEmergencyModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string factorId { get; set; }/// 因子编号
        public string factorName { get; set; } /// 因子名称
        public string testMethodId { get; set; }/// 测试方法编号
        public string testMethodName { get; set; } /// 测试方法名称
        public string unitId { get; set; }/// 计量单位编号
        public string unitName { get; set; } /// 计量单位名称
        public string equipmentId { get; set; }/// 测量设备编号
        public string equipmentName { get; set; }/// 测量设备名称
        public string factorValue { get; set; }  /// 因子的值
        public string incidentNature { get; set; } /// 对应的事故性质

        public string lat { get; set; }   /// 纬度
        public string lng { get; set; } /// 经度
        public string index { get; set; } /// 排序索引
        public double? TargetLat { get; set; }   /// 目标点位纬度
        public double? TargetLng { get; set; }  /// 目标点位经度
        public string TargetAddress { get; set; } /// 目标点位地址
        public string Content { get; set; } /// 内容
        public string Title { get; set; }   /// 标题
        public string Original { get; set; } /// 原始名称
        public string Current { get; set; } /// 当前名称
        public string NatureString { get; set; } /// 事故性质字符串(000分别对应 气 水 土)
        ////public DateTime Original { get; set; }/// 原始时间
        ////public DateTime Current { get; set; }/// 当前时间
        public string storepath { get; set; }/// 存储路径(相对路径)
        public string width { get; set; }  /// 宽度(单位px)
        public string height { get; set; } /// 高度(单位px)
        public string storeurl { get; set; } /// 方案存储地址
        public string reportid { get; set; } /// 报告编号
        public string reportname { get; set; } /// 报告名称  
        public string length { get; set; } /// 内容长度(单位秒)
        public string direction { get; set; } /// 风向(单位 度)
        public string speed { get; set; } /// 风速(单位 米/秒)
        public DateTime creationTime { get; set; }
        public string creatorusername { get; set; }
        public string category { get; set; }
        public string emergencyid { get; set; }


    }
}
