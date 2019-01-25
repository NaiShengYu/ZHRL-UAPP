using System;
using System.Collections.Generic;

namespace AepApp.Models
{
    public class WaterQualitySiteModel
    {
        public int Totals { get; set; }
        public List<WaterQualityItem> Items = new List<WaterQualityItem>();
    }


    public class WaterQualityItem
    {
        public WaterQualityBasic basic { get; set; }
        public WaterQualityCurrentlevel currentlevel { get; set; }

    }
    public class WaterQualityBasic
    {
        public string id { get; set; }
        public string stcode { get; set; }
        public string stname { get; set; }
        public string riversystem { get; set; }
        public string rivername { get; set; }
        public string river { get; set; }
        public string number { get; set; }
        public string level { get; set; }
        public string prop1 { get; set; }
        public string prop2 { get; set; }
        public string prop3 { get; set; }
        public string prop4 { get; set; }
        public string prop5 { get; set; }
        public string prop6 { get; set; }
        public string funczonetype { get; set; }
        public string qualitytarget { get; set; }
        public string monitoringstation { get; set; }
        public string exstandard { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string radius { get; set; }
        public string type { get; set; }
        public string refid { get; set; }
        public string subtype { get; set; }
        public string areacode { get; set; }
        public string isauto { get; set; }
        public string timesplit { get; set; }
        public string updatedate { get; set; }
        public string riverindex { get; set; }

    }

    public class WaterQualityCurrentlevel
    {
        //public WaterQualityGrade grade { get {
        //        if (grade == null) return new WaterQualityGrade();
        //        else return grade;
        //} 
        //set { } }
        public string showColor
        { get {
                if (color == null) return "Transparent";
                else return color;
        } 
        set { } }
        public string color { get; set; }
        public WaterQualityGrade grade { get; set; }
        public List<WaterQualityTimesDatas> timesDatas { get; set; }
    }
    public class WaterQualityGrade
    {
        public string idx { get; set; }
        public string name { get; set; }
    }
    public class WaterQualityTimesDatas
    {
        public string facId { get; set; }
        public string val { get; set; }
        public string times { get; set; }
    }

}
