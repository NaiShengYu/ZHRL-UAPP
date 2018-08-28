using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AepApp.Models
{
    public class GridEventModel : BaseModel
    {
        public string addTime { get; set; }
        public string townHandleTime { get; set; }
        public string countryHandleTime { get; set; }
        public string finishTime { get; set; }
        public ObservableCollection<GridTaskModel> taskList { get; set; }



        public Guid id { get; set; }
        public Guid grid { get; set; }

        public int state { get; set; }
        public string EventStatus
        {
            get { return ConstConvertUtils.GridTaskStatus2String(state); }
        }
        
        private string title;
        public string Title                
        {
            get { return title; }
            set { title = value; NotifyPropertyChanged(); }
        }

        private int type;
        public int Type                 
        {
            get { return type; }
            set { type = value; NotifyPropertyChanged(); }
        }

        private DateTime date;
        public DateTime Date                 
        {
            get { return date; }
            set { date = value; NotifyPropertyChanged(); }
        }

        private string gridName;
        public string GridName             
        {
            get { return gridName; }
            set { gridName = value; NotifyPropertyChanged(); }
        }


        private string level;
        public string Level                
        {
            get { return level; }
            set { level = value; NotifyPropertyChanged(); }
        }

    }
}
