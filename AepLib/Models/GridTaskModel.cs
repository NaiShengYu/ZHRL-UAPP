using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    public class GridTaskModel : BaseModel
    {
        public string task { get; set; }
        public string title { get; set; }
        public string incidentTitle { get; set; }
        public DateTime? lastDate { get; set; }
        public string gridName { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public string addr { get; set; }
        public string pointName { get; set; }
        public string id { get; set; }
        public string level { get; set; }
        public string date { get; set; }
        public string assignment { get; set; }
        public string direction { get; set; }



    }
}
