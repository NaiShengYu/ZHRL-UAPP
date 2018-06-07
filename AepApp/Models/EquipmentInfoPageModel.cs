using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class EquipmentInfoPageModel 
	{
        public class EquipmentInfoBean {
            public String id { get; set; }
            public String code { get; set; }
            public String name { get; set; }
            public int adjust_period { get; set; }
            public String brand { get; set; }
            public int category { get; set; }
            public int charging_period { get; set; }
            public String model { get; set; }
            public string replace_period { get; set; }
            public int status { get; set; }
            public string type { get; set; }
            public String updatedate { get; set; }

        }
    }
}