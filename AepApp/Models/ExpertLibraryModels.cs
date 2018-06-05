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

            public string name;
            public string mobilePhone;
            public string telephone;
            public string email;
            public string qq;
            public string address;
            public string organization;
            public string title;
            public string professionalField;
            public string creatorUserName;
            public string id;
        }
    }
}