using System;
namespace AepApp.Models
{
    public class SampleBasicDataModel
    {
        //公共信息ID guid
        public string id { get; set; }
        //公共信息ID guid
        public string basicDataModelID { get; set; }
        //水域功能类别 string 
        public string areafunctype { get; set; }
        //水域名称 string
        public string areaname { get; set; }
        //调查车/船信息 string
        public string carship { get; set; }
        //采样计划ID GUID
        public string planid { get; set; }
        // 位置及层次 string
        public string position { get; set; }
        //监测目的 string
        public string purpose { get; set; }
        //采样日期 datetime
        public DateTime sampledate { get; set; }
        //气温 string
        public string temperature { get; set; }
        //采样工具 string
        public string tools { get; set; }
        //天气情况 string
        public string weather { get; set; }



    }
}
