using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class AQIMapPageModel
    {
        public class ValueForSite : INotifyPropertyChanged  //里面的字段暂时没有具体实现
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void NotifyPropertyChanged(string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public string stationId { get; set; }
            //public double StationLng { get; set; }
            //public double StationLat { get; set; }
            public double AQIValue { get; set; }
            public double PM25Value { get; set; }
            public double PM10Value { get; set; }
            public double O3Value { get; set; }
            public double COValue { get; set; }

            public string AQIId { get; set; }
            public string PM25Id { get; set; }
            public string PM10Id { get; set; }
            public string O3Id { get; set; }
            public string COId { get; set; }
        }

    }
}