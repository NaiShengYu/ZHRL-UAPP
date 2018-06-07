using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class EmergencyAccidentDetailModel 
	{
        public class EmergencyAccidentDetailModelBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean {
            public String name { get; set; }
            public String startDate { get; set; }
            public bool natureDetermined { get; set; }
            public String id { get; set; }
            public List<IncidentLoggingEventsBean> incidentLoggingEvents { get; set; }
        }
        public class IncidentLoggingEventsBean {
           public string factorId;
           public string factorName;
           public string testMethodId;
           public string testMethodName;
           public string unitId;
           public string unitName;
           public string equipmentId;
           public string equipmentName;
           public int factorValue;
           public int incidentNature;
           public double lat;
           public double lng;
           public int index;
           public string creationTime;
           public string creatorUserName;
           public string category;
           public string id;
        }
    }
}