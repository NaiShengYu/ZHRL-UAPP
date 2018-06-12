
using System.Collections.Generic;

using System;
namespace AepApp.Models
{
    public class AddDataIncidentFactorModel
    {
        
        public class Factor{
            public string targetUrl { get; set; }
            public string success { get; set; }
            public string error { get; set; }
            public string unAuthorizedRequest { get; set; }
            public string __abp { get; set; }
            public result result { get; set; }
        }

        public class result{

            public List<ItemsBean> items { get; set; }
       
        }


        public class ItemsBean
        {
            public string factorId { get; set; }
            public string factorName { get; set; }
            public string dataType { get; set; }
            public string creatorUserName { get; set; }
            public string id { get; set; }
            public string unit { get; set; }
    }

    }
}
