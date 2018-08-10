using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class AirPageModels 
	{
        public class AirInfo {
            public string StationId { get; set; }
            public string StationName { get; set; }
            public double StationLng { get; set; }
            public double StationLat { get; set; }
            public Pollutant info { get; set; }
            public string Rank { get; set; }
            //public string info { get; set; }
            public AQIMapPageModel.ValueForSite values;
        }

        public class Pollutant
        {
            public string facId { get; set; }
            public string facName { get; set; }
            public double AQI { get; set; }
            public double PM25 { get; set; }
            public AQI_INFO AQIInfo { get; set; }
        }
        public class AQI_INFO
        {
            public double AQI { get; set; }
            public string AQILevel { get; set; }
            public string AQIClass { get; set; }
            public string AQIColor { get; set; }
            public string Health { get; set; }
            public string Suggestion { get; set; }
            public string Idx { get; set; }
        }
    }
}