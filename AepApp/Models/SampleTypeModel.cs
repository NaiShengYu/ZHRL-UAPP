using System;
using System.ComponentModel;

namespace AepApp.Models
{
    public class SampleTypeModel : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string name { get; set; }
      
        private bool _isSelect = false;
        public bool isSelect
        {
            get { return _isSelect; }
            set { _isSelect = value; NotifyPropertyChanged("isSelect"); }
        }

    }
}
