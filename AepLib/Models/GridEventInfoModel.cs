using System;
using System.Collections.ObjectModel;
using AepApp.Tools;

namespace AepApp.Models
{
    public class GridEventInfoModel:BaseModel
    {

        public bool canEdit { get; set; }
        public Guid? id { get; set; }
        public string title { get; set; }
        public Guid? staff { get; set; }
        public DateTime date { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string enterprise  { get; set; }
        public int? state { get; set; }
        public int? type  { get; set; }
        public Guid? gridcell { get; set; }
        public string gridName { get; set; }
        public ObservableCollection<Followup> Followup { get; set; }
        public ObservableCollection<AttachmentInfo> attaches { get; set; }

        private string TypeName { get; set; }
        public string typeName
        {
            get { return TypeName; }
            set { TypeName = value; NotifyPropertyChanged(); }
        }


        public bool stateStatus{
            get {
                if (state == 3) return true;
                else return false;
            }
            set{}
        }

        private string  addr{ get; set; }
        public string Addr
        {
            get { return addr; }
            set { addr = value; NotifyPropertyChanged(); }
        }

        private string Contents;
        public string contents
        {
            get { return Contents; }
            set {  Contents= value; NotifyPropertyChanged(); }
        }

        private string ShowContent;
        public string showContent
        {
            get { return StringUtils.ReplaceHtmlTag(contents); }
            set { ShowContent = value; NotifyPropertyChanged(); }
        }


        private string results;
        public string Results
        {
            get { return results; }
            set { results = value; NotifyPropertyChanged(); }
        }


        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; NotifyPropertyChanged(); }
        }

        private string tel;
        public string Tel
        {
            get { return tel; }
            set { tel = value; NotifyPropertyChanged(); }
        }

        private string enterpriseName;
        public string EnterpriseName
        {
            get { return enterpriseName; }
            set { enterpriseName = value; NotifyPropertyChanged(); }
        }


        private string lnglatString;
        public string LnglatString
        {
            get { return lng.Value.ToString("f6") + " E, " + lng.Value.ToString("f6") + " N"; }
            set { lnglatString = value; NotifyPropertyChanged(); }
        }
        public bool hasPhone
        {
            get { return !string.IsNullOrWhiteSpace(tel); }
        }

    }

    public class Followup {
        public string id { get; set; }
        public string level { get; set; }
        public string staff { get; set; }
        public DateTime date { get; set; }
        public string remarks { get; set; }

    }


}
