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
    }
}
