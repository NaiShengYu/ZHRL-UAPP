using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    public class GridTaskModel : BaseModel
    {
        public string name { get; set; }
        public string addTime { get; set; }
        public string taskStatus { get; set; }
        public string eventName { get; set; }

        public string task { get; set; }
        public string taskTitle { get; set; }
        public string incidentTitle { get; set; }
        public DateTime lastDate { get; set; }
        public string gridName { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public string addr { get; set; }
        public string pointName { get; set; }



    }
}
