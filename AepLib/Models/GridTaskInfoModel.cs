using System;
using System.Collections.ObjectModel;

namespace AepApp.Models
{
    public class GridTaskInfoModel : BaseModel
    {

        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid? incident { get; set; }
        public Guid? staff { get; set; }
        public Guid? template { get; set; }
        public string title { get; set; }
        public string userName { get; set; }//任务发出人
        public string gridName { get; set; }
        public string incidentTitle { get; set; }
        public DateTime? deadline { get; set; }
        public double? period { get; set; }
        public int type { get; set; }
        public int state { get; set; }
        public int index { get; set; }
        public DateTime date { get; set; }

        private bool CanEdit;
        public bool canEdit
        {
            get { return CanEdit; }
            set { CanEdit = value; NotifyPropertyChanged(); }
        }

        private string assignName;
        public string AssignName
        {
            get { return assignName; }
            set { assignName = value; NotifyPropertyChanged(); }
        }

        private DateTime lastRecordTime;
        public DateTime LastRecordTime
        {
            get { return lastRecordTime; }
            set { lastRecordTime = value; NotifyPropertyChanged(); }
        }

        private int recordCount;
        public int RecordCount
        {
            get { return recordCount; }
            set { recordCount = value; NotifyPropertyChanged(); }
        }

        private string contents;
        public string Contents
        {
            get { return contents; }
            set { contents = value; NotifyPropertyChanged(); }
        }



        public ObservableCollection<Assignments> assignments { get; set; }
        public ObservableCollection<Coords> coords { get; set; }
        public ObservableCollection<Enterprise> enterprise { get; set; }
    }

    public class Assignments : BaseModel
    {
        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid? dept { get; set; }
        public Guid? staff { get; set; }
        public Guid? grid { get; set; }
        public int type { get; set; }
        public string gridName { get; set; }


        private string staffName;
        public string StaffName
        {
            get { return staffName; }
            set { staffName = value; NotifyPropertyChanged(); }
        }

        private string tel;
        public string Tel
        {
            get { return tel; }
            set { tel = value; NotifyPropertyChanged(); }
        }

        //public ObservableCollection<Assignments> nextLevel { get; set; }




    }

    public class Coords : BaseModel
    {
        public Guid id { get; set; }
        public string rowState { get; set; }
        public string title { get; set; }
        public double? lng { get; set; }
        public double? lat { get; set; }
        public string remarks { get; set; }
        public int? index { get; set; }

    }

    public class Enterprise : BaseModel
    {
        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid enterprise { get; set; }
        public string enterpriseName { get; set; }

    }

}
