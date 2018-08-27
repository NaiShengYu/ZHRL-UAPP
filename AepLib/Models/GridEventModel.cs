using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AepApp.Models
{
    public class GridEventModel : BaseModel
    {
        private string name;
        private string time;
        private string address;
        public string lng { get; set; }
        public string lat { get; set; }
        private string eventType;//事件类型
        private string eventStatus;//事件状态
        public string addTime { get; set; }
        public string townHandleTime { get; set; }
        public string countryHandleTime { get; set; }
        public ObservableCollection<GridTaskModel> taskList { get; set; }
        public string finishTime { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged(); }
        }
        public string Time
        {
            get { return time; }
            set { time = value; NotifyPropertyChanged(); }
        }
        public string Address
        {
            get { return address; }
            set { address = value; NotifyPropertyChanged(); }
        }

        public string EventType
        {
            get { return eventType; }
            set { eventType = value; NotifyPropertyChanged(); }
        }

        public string EventStatus
        {
            get { return eventStatus; }
            set { eventStatus = value; NotifyPropertyChanged(); }
        }

        public string lnglatString
        {
            get { return lng + " E, " + lat + " N"; }
        }

        private string Incident;
        public string incident             
        {
            get { return Incident; }
            set { Incident = value; NotifyPropertyChanged(); }
        }


        private string Gridcell;
        public string gridcell             
        {
            get { return Gridcell; }
            set { Gridcell = value; NotifyPropertyChanged(); }
        }


        private string Title;
        public string title                
        {
            get { return Title; }
            set { Title = value; NotifyPropertyChanged(); }
        }

        private int Type;
        public int type                 
        {
            get { return Type; }
            set { Type = value; NotifyPropertyChanged(); }
        }

        private DateTime Date;
        public DateTime date                 
        {
            get { return Date; }
            set { Date = value; NotifyPropertyChanged(); }
        }

        private string GridName;
        public string gridName             
        {
            get { return GridName; }
            set { GridName = value; NotifyPropertyChanged(); }
        }


        private int Level;
        public int level                
        {
            get { return Level; }
            set { Level = value; NotifyPropertyChanged(); }
        }

    }
}
