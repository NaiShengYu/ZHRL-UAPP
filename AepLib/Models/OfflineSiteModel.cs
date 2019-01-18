using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    /// <summary>
    /// 离线站点
    /// </summary>
    public class OfflineSiteModel
    {
        public string locid { get; set; }
        public string locname { get; set; }
        public int dtype { get; set; }//数据种类 0：实时；1：分钟；2：小时；3：天；-1：其他原始值
        public int subtype { get; set; }//点位类型 0：VOC站；3：空气站
    }
}
