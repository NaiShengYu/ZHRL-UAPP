using System;
using System.ComponentModel;

namespace AepApp.Models
{
    public class CollectionAndTransportSampleModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string time { get; set; }
        public string num { get; set; }

        private string Type;

        public string type
        {
            get { return Type; }
            set { Type = value; NotifyPropertyChanged("type"); }
        }

        public string state{ get; set; }

    }
}
