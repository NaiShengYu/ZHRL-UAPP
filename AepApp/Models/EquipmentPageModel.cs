using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class EquipmentPageModel
    {
        public class EquipmentPageModelBean
        {
            public int count;
            public List<ItemsBean> items;
        }
        public class ItemsBean
        {
            public String id { get; set; }
            public String name { get; set; }
            public int adjust_period { get; set; }
            public Object brand { get; set; }
            public int category { get; set; }
            public int charging_period { get; set; }
            public String model { get; set; }
            public string replace_period{ get; set; }
            public int status { get; set; }
            public int type { get; set; }
            public string updatedate { get; set; }

            public string BrandAndModel
            {
                get
                {
                    string ret = (brand + " " + model).Trim();
                    if (string.IsNullOrWhiteSpace(ret)) return null;
                    return ret;
                }
            }

        }
    }
}