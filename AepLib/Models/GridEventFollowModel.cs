using System;
using System.Collections.ObjectModel;
namespace AepApp.Models
{
    public class GridEventFollowModel : BaseModel
    {
        public bool canEdit { get; set; }
        public Guid id { get; set; }
        public string title { get; set; }
        public string staffTel { get; set; }
        public string rowState { get; set; }
        public Guid? incident { get; set; }
        public Guid? previous { get; set; }
        public Guid? staff { get; set; }
        public Guid? grid { get; set; }
        public DateTime date { get; set; }
        public int level { get; set; }
        public int state { get; set; }

        ////选中上报事件
        private bool uping { get; set; }         public bool Uping         {             get { return uping; }             set { uping = value; NotifyPropertyChanged(); }         }
        ////选中下发事件
        private bool downing { get; set; }         public bool Downing         {             get { return downing; }             set { downing = value; NotifyPropertyChanged(); }         } 

        private string StaffName;
        public string staffName
        {
            get { return StaffName; }
            set { StaffName = value; NotifyPropertyChanged(); }
        }

        private string GridName;
        public string gridName
        {
            get { return GridName; }
            set { GridName = value; NotifyPropertyChanged(); }
        }
        private string GridId;
        public string gridId
        {
            get { return GridId; }
            set { GridId = value; NotifyPropertyChanged(); }
        }

        private string remarks;
        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<GridEventFollowTaskModel> tasks;
        public ObservableCollection<GridEventFollowTaskModel> Tasks
        {
            get { return tasks; }
            set { tasks = value; NotifyPropertyChanged(); }
        }


    }

    public class GridEventFollowTaskModel : BaseModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int state { get; set; }
        public int index { get; set; }
    }



}
