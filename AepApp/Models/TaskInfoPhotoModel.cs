using System;
using System.ComponentModel;

namespace AepApp.Models
{
    public class TaskInfoPhotoModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string imagePath { get; set; }

        private bool select;
        public bool isSelect
        {
            get { return select; }
            set { select = value; NotifyPropertyChanged("isSelect"); }
        }




    }
}
