using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AepApp.Models
{
    public class ElectroniPcunishMentList
    {
        public string id { get; set; }
        public string name { get; set; }
        public string entercode { get; set; }
        public string pid { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public string sum { get; set; }
        public string askcode { get; set; }
        public string checkdate { get; set; }
        public string content { get; set; }
        public string createdate { get; set; }
        public string processing { get; set; }
        public string source { get; set; }
        public string state { get; set; }
        public string remark { get; set; }
        public List<ElectroniPcunishimentInfo> items { get; set; }

    }
}
