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
    }
}
