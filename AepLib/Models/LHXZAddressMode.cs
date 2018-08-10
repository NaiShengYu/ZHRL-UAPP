using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class LHXZAddressMode : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string name { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }
        public string address { 
            get
            {
                return lng + " E ," + lat + " N";
            }
            set { }
        }

        private bool _isCurrent = false;
        public bool isCurrent
        {
            get { return _isCurrent; }
            set { _isCurrent = value; NotifyPropertyChanged("isCurrent"); }
        }

    }
}
