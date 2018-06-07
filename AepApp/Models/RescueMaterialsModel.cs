using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class RescueMaterialsModel
    {
        public class RescueMaterialsModelBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
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
            public string creatorUserName { get; set; }
            public String notes { get; set; }
            public String id { get; set; }
            public List<ReliefSuppliesBean> reliefSupplies { get; set; }
        }
        public class ReliefSuppliesBean
        {
            public string name { get; set; }
            public string qty { get; set; }
            public string unit { get; set; }
            public string creatorUserName { get; set; }
            public String id { get; set; }
            public string qtyunit {
                get {

                    return qty + unit;
                }
                set { }
            }
        }
    }
}