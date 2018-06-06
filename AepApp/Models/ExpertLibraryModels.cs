using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class ExpertLibraryModels
    {
        public class SpecialBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
        {
            public ProfessionalsBean professionals { get; set; }
        }
        public class ProfessionalsBean
        {
            public int totalCount { get; set; }
            public List<ItemsBean> items { get; set; }
        }
        public class ItemsBean {

            public string name { get; set; }
            public string mobilePhone { get; set; }
            public string telephone { get; set; }
            public string email { get; set; }
            public string qq { get; set; }
            public string address { get; set; }
            public string organization { get; set; }
            public string title { get; set; }
            public string professionalField { get; set; }
            public string creatorUserName { get; set; }
            public string id { get; set; }
        }
    }
}