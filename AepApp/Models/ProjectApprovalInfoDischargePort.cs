using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AepApp.Models
{
    public class ProjectApprovalInfoDischargePort : ObservableCollection<ProjectApprovalInfoFactor>
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
        //private ObservableCollection<ProjectApprovalInfoFactor> _factors = new ObservableCollection<ProjectApprovalInfoFactor>();

        public ObservableCollection<ProjectApprovalInfoFactor> items => this;

        public string nameAndType
        {
            get
            {
                return name + "(" + typename + ")";
            }
        }
    }
}
