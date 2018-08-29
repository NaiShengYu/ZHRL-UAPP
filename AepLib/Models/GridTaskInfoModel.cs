using System;
using System.Collections.ObjectModel;

namespace AepApp.Models
{
    public class GridTaskInfoModel :BaseModel
    {

        public bool canEdit { get; set; }
        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid incident { get; set; }
        public Guid staff { get; set; }
        public Guid template { get; set; }
        public string title { get; set; }
        public string userName { get; set; }//任务发出人
        public string gridName { get; set; }
        public DateTime deadline { get; set; }
        public double period { get; set; }
        public int type { get; set; }
        public int state { get; set; }
        public int index { get; set; }
        public DateTime date { get; set; }

       
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

    public class Assignments :BaseModel{
        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid dept { get; set; }
        public Guid staff { get; set; }
        public Guid grid { get; set; }
        public int type { get; set; }
    }

    public class Coords :BaseModel{
        public Guid id { get; set; }
        public string rowState { get; set; }
        public string title { get; set; }
        public double lng { get; set; }
        public double lat { get; set; }
        public string remarks { get; set; }
        public int index { get; set; }

    }

    public class Enterprise : BaseModel
    {
        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid enterprise { get; set; }
        public string enterpriseName { get; set; }

    }

}
