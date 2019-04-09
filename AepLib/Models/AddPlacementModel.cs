using System;
using System.Collections.ObjectModel;

namespace AepApp.Models
{
    public class AddPlacementModel : BaseModel
    {
        public string flag { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public DateTime plantime { get; set; }
        public DateTime createtime { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }

        public bool canEdit { get; set; }


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
        public ObservableCollection<AddPlacement_Task> tasklist { get; set; }

    }

    public class AddPlacement_Equipment : BaseModel
    {
        public string equipid { get; set; }
        public string equipname { get; set; }
    }

    public class AddPlacement_Staff : BaseModel
    {
        public string staffid { get; set; }
        public string staffname { get; set; }
    }

    public class AddPlacement_Task : BaseModel
    {
        public string flag { get; set; }
        public string taskid { get; set; }
        public string taskname { get; set; }
        public string tasktype { get; set; }
        public string taskTypeDes
        {
            get
            {
                if ("0" == tasktype)
                {
                    return "地表水";
                }
                else if ("1" == tasktype)
                {
                    return "废水";
                }
                else if ("2" == tasktype)
                {
                    return "饮用水";
                }
                else if ("3" == tasktype)
                {
                    return "废气";
                }
                else if ("4" == tasktype)
                {
                    return "环境空气";
                }
                else if ("5" == tasktype)
                {
                    return "室内空气";
                }
                else if ("6" == tasktype)
                {
                    return "农业用地";
                }
                else if ("7" == tasktype)
                {
                    return "工业用地";
                }
                return "";
            }
            set { }
        }

        public string taskstatus { get; set; }
        public string taskindex { get; set; }
        public ObservableCollection<AddPlacement_Analysist> taskAnas { get; set; }
        public bool canEdit { get; set; }
        public string factorName
        {
            get
            {
                string aa = "";
                if(taskAnas == null || taskAnas.Count == 0)
                {
                    return aa;
                }
                foreach (AddPlacement_Analysist item in taskAnas)
                {
                    if (string.IsNullOrWhiteSpace(aa))
                        aa = item.atname;
                    else
                        aa = aa + "," + item.atname;
                }
                return aa;
            }
            set { }
        }
    }
    public class AddPlacement_Analysist : BaseModel
    {
        public string atid { get; set; }
        public string attype { get; set; }
        public string atname { get; set; }
    }
}
