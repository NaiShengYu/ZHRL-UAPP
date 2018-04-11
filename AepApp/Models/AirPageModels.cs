using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class AirPageModels 
	{
        internal  class AirInfo {
            public string StationId { get; set; }
            public string StationName { get; set; }
            public string StationLng { get; set; }
            public string StationLat { get; set; }
            public List<Pollutant> info { get; set; }
            //public string info { get; set; }
        }
        internal class Pollutant
        {
            public string facId { get; set; }
            public string facName { get; set; }
            public string AQI { get; set; }
            public string PM25 { get; set; }
            public List<AQI_INFO> AQIInfo { get; set; }
        }
        internal class AQI_INFO
        {
            public string AQI { get; set; }
            public string AQILevel { get; set; }
            public string AQIClass { get; set; }
            public string AQIColor { get; set; }
            public string Health { get; set; }
            public string Suggestion { get; set; }
            public string Idx { get; set; }
        }
    }
}