using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class EmergencyPlanModels
    {
        public class EmergencyPlanBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
        {
            public PreplansBean preplans { get; set; }
        }
        public class PreplansBean
        {
            public int totalCount { get; set; }
            public List<ItemsBean> items { get; set; }
        }

        public class ItemsBean
        {
            public String name { get; set; }
            public Object creatorUserName { get; set; }
            public Object notes { get; set; }
            public String id { get; set; }
            public List<FilesBean> files { get; set; }
        }
        public class FilesBean
        {
            public string storeUrl { get; set; }
            public string name { get; set; }
            public string format { get; set; }
            public string size { get; set; }
            public int sort { get; set; }
            public Object creatorUserName { get; set; }
            public string id { get; set; }
        }

    }
}