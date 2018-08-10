using System;
using System.ComponentModel;

namespace AepApp.Models
{
    public class TestModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string title;
        public string tsetModelTitle { get { return title; } set { title = value; NotifyPropertyChanged("tsetModelTitle"); }}

        private secondTestModel second;
        public secondTestModel secondModel { get { return second; } set { second = value; NotifyPropertyChanged("secondModel"); } }
       



    }


    public class secondTestModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string title;
        public string secondTitle { get { return title; } set { title = value; NotifyPropertyChanged("secondTitle"); }}

        private thirdTestModel third;
        public thirdTestModel thirdModel { get { return third; } set { third = value; NotifyPropertyChanged("thirdModel"); } }
       


    }


    public class thirdTestModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string title;
        public string thirdTitle { get { return title; } set { title = value; NotifyPropertyChanged("thirdTitle"); } }   
    }


}
