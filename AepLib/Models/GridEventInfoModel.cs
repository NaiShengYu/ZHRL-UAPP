using System;
using System.Collections.ObjectModel;

namespace AepApp.Models
{
    public class GridEventInfoModel:BaseModel
    {

        public bool canEdit { get; set; }
        public Guid id { get; set; }
        public string title { get; set; }
        public Guid staff { get; set; }
        public DateTime date { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string enterprise  { get; set; }
        public int state { get; set; }
        public int type  { get; set; }
        public Guid gridcell { get; set; }
        public string gridName { get; set; }
        public ObservableCollection<Followup> Followup { get; set; }
       
        //public bool isEnd{
        //    get {
        //        if (state == 4) return true;
        //        else return false;
        //    }
        //    set{}
        //}

        private string Addr { get; set; }
        public string addr
        {
            get { return Addr; }
            set { Addr = value; NotifyPropertyChanged(); }
        }

        private string Content;
        public string content
        {
            get { return Content; }
            set { Content = value; NotifyPropertyChanged(); }
        }
        private string Results;
        public string results
        {
            get { return Results; }
            set { Results = value; NotifyPropertyChanged(); }
        }


        private string UserName;
        public string userName
        {
            get { return UserName; }
            set { UserName = value; NotifyPropertyChanged(); }
        }

        private string Tel;
        public string tel
        {
            get { return Tel; }
            set { Tel = value; NotifyPropertyChanged(); }
        }

        private string EnterpriseName;
        public string enterpriseName
        {
            get { return EnterpriseName; }
            set { EnterpriseName = value; NotifyPropertyChanged(); }
        }


        private string LnglatString;
        public string lnglatString
        {
            get { return lng + " E, " + lat + " N"; }
            set { LnglatString = value; NotifyPropertyChanged(); }
        }


    }

    public class Followup {
        public string id { get; set; }
        public string level { get; set; }
        public string staff { get; set; }
        public string date { get; set; }
        public string remarks { get; set; }

    }


}
