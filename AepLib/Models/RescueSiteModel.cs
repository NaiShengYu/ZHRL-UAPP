using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class RescueSiteModel
    {
        public class RescueSiteModelBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
        {
            public RescuePointsBean rescuePoints { get; set; }
        }
        public class RescuePointsBean
        {
            public int totalCount { get; set; }
            public List<ItemsBean> items { get; set; }
        }
        public class ItemsBean
        {
            public String code { get; set; }
            public String name { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public String province { get; set; }
            public String city { get; set; }
            public String district { get; set; }
            public String street { get; set; }
            public String address { get; set; }
            public Object creatorUserName { get; set; }
            public String notes { get; set; }
            public String id { get; set; }
            public List<ReliefSuppliesBean> reliefSupplies { get; set; }
        }
        public class ReliefSuppliesBean {
            public String name { get; set; }
            public string qty { get; set; }
            public String unit { get; set; }
            public Object creatorUserName { get; set; }
            public String id { get; set; }
        }
    }
}