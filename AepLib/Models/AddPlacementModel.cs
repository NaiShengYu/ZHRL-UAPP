using System;
using System.Collections.ObjectModel;

namespace AepApp.Models
{
    public class AddPlacementModel:BaseModel
    {
        public string flag { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public DateTime plantime { get; set; }
        public DateTime createtime { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }

        private string Type;


        private string Address;
        public string address
        {
            get { return Address; }
            set { Address = value; NotifyPropertyChanged("address"); }
        }
        private string Pretreatment;
        public string pretreatment 
        {   
        get { return Pretreatment; }
            set { Pretreatment = value; NotifyPropertyChanged("pretreatment"); }
        }
        private string Security;
        public string security
        {
            get { return Security; }
            set { Security = value; NotifyPropertyChanged("security"); }
        }
        private string Remarks;
        public string remarks
        {
            get { return Remarks; }
            set { Remarks = value; NotifyPropertyChanged("remarks"); }
        }
        private string Qctip;
        public string qctip
        {
            get { return Qctip; }
            set { Qctip = value; NotifyPropertyChanged("qctip"); }
        }
        public ObservableCollection<AddPlacement_Staff> staffs { get; set; }
        public ObservableCollection<AddPlacement_Equipment> equips { get; set; }
        public ObservableCollection<AddPlacement_Task> tasks { get; set; }

    }

    public class AddPlacement_Equipment:BaseModel
    {
        public string equipid { get; set; }
        public string equipname { get; set; }
    }

    public class AddPlacement_Staff:BaseModel
    {
        public string staffid { get; set; }
        public string staffname { get; set; }
    }

    public class AddPlacement_Task : BaseModel
    {
        public string flag { get; set; }
        public string taskid { get; set; }
        public string taskname { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string index { get; set; }
        public ObservableCollection<AddPlacement_Analysist> Analysistypes { get; set; }
    }
    public class AddPlacement_Analysist : BaseModel
    {
        public string atid { get; set; }
        public string attype { get; set; }
    }
}
