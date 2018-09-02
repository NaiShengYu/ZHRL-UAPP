using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        public DateTime? deadline { get; set; }
        public double? period { get; set; }
        public int? type { get; set; }
        public int? state { get; set; }
        public int? index { get; set; }
        public DateTime date { get; set; }


        private string IncidentTitle;//事件标题
        public string incidentTitle
        {
            get { return IncidentTitle; }
            set { IncidentTitle = value; NotifyPropertyChanged(); }
        }

        private string UridName;//发出人
        public string userName
        {
            get { return UridName; }
            set { UridName = value; NotifyPropertyChanged(); }
        }


        private string GridName;
        public string gridName
        {
            get { return GridName; }
            set { GridName = value; NotifyPropertyChanged(); }
        }

        private string NatureName;
        public string natureName
        {
            get { return NatureName; }
            set { NatureName = value; NotifyPropertyChanged(); }
        }

        private string StateName;
        public string stateName
        {
            get { return StateName; }
            set { StateName = value; NotifyPropertyChanged(); }
        }


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
        private string TemplateName;
        public string templateName
        {
            get { return TemplateName; }
            set { TemplateName = value; NotifyPropertyChanged(); }
        }

        private string Result;
        public string results
        {
            get { return Result; }
            set { Result = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Assignments> assignments { get; set; }
        public ObservableCollection<Coords> coords { get; set; }
        public ObservableCollection<Enterprise> enterprise { get; set; }
        public ObservableCollection<Guid> taskenterprises { get; set; }
        public ObservableCollection<taskassignment> taskassignments { get; set; }

        private Func<string, Task<string>> _evaluateJavascript;//webview调用js
        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return _evaluateJavascript; }
            set { _evaluateJavascript = value; }
        }

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
        public string remarks {
            get
            {
                if (lng != null && lat != null) return lat + "E, " + lng + "N";
                else return "";
            }
            set { }
        }
        public int? index { get; set; }

    }

    public class Enterprise : BaseModel
    {
        public Guid id { get; set; }
        public Guid enterprise { get; set; }
        public string rowState { get; set; }
        public string enterpriseName { get; set; }

    }

    public class taskassignment : BaseModel
    {

        public Guid? id { get; set; }
        public string gridcell { get; set; }
        public string dept { get; set; }
        public string gridName { get; set; }
        public Guid? staff { get; set; }
        public int? type { get; set; }
        public Guid? grid { get; set; }
        public taskassignment nextLevel { get; set; }

        //public ObservableCollection<taskassignment> nextLevel { get; set; }
    }

}
