using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AepApp.Models
{
    public class ProjectApprovalInfoDischargePort 
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void NotifyPropertyChanged(string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


        public string id { get; set; }
        public string name { get; set; }
        public string ptype { get; set; }
        public string typename { get; set; }
        public ObservableCollection<ProjectApprovalInfoFactor> items = new ObservableCollection<ProjectApprovalInfoFactor>();


        public string nameAndType
        {
            get
            {
                return name + "(" + typename + ")";
            }
        }
    }
}
