using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class EmergencyAccidentPageModels
    {
        public class EmergencyAccidentBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
        {
            public IncidentsBean incidents { get; set; }
        }
        public class IncidentsBean
        {
            public int totalCount { get; set; }
            public List<ItemsBean> items { get; set; }
        }
        public class ItemsBean
        {
           public String name { get; set; }
           public String startDate { get; set; }
           public string endDate { get; set; }
           public string isArchived { get; set; }
           public int incidentGrade { get; set; }
           public String natureString { get; set; }
           public string address { get; set; }
           public string code { get; set; }
           public string overview { get; set; }
           public string creatorUserName { get; set; }
           public string hasAirNature { get; set; }
           public string hasWaterNature { get; set; }
           public string hasSoilNature { get; set; }
           public bool natureDetermined { get; set; }
           public String id { get; set; }
        }
    }

}