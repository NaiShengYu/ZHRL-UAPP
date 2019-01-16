using System;
namespace AepApp.Models
{
    public class VOCSiteListModel
    {
        public string id { get; set; }
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
        public string subtype { get; set; }//0： 固定代表VOC点位  3：表示常规空气  9：表示地表水
        public string areacode { get; set; }
        public string isauto { get; set; }
        public string timesplit { get; set; }
        public string level { get; set; }
    }
}
