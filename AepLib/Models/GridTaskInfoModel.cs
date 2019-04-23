using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AepApp.Tools;

namespace AepApp.Models
{
    public class GridTaskInfoModel : BaseModel
    {

        public Guid id { get; set; }
        public string rowState { get; set; }
        public Guid? incident { get; set; }
        public Guid? staff { get; set; }//发出人id
        public Guid? template { get; set; }
        public Guid? approvedBy { get; set; }
        public string title { get; set; }
        public double? period { get; set; }
        public int? type { get; set; }//1日常任务 2事件任务
        public int? state { get; set; }
        public int? index { get; set; }
        public DateTime date { get; set; }
        public DateTime? approveTime { get; set; }

        private DateTime? Deadline;//任务处理期限
        public DateTime? deadline
        {
            get { return Deadline; }
            set { Deadline = value; NotifyPropertyChanged(); }
        }

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

        private string ApprovedName;//审批人
        public string approvedName
        {
            get { return ApprovedName ; }
            set { ApprovedName = value; NotifyPropertyChanged(); }
        }

        private string ApprovedNameAndTime;//审批人
        public string approvedNameAndTime
        {
            get { try
                {
                    return approvedName + "\n" + approveTime.Value.ToString("yyyy-MM-dd HH:mm");
                }
                catch (Exception ex)
                {
                    return "";
                } }
            set { ApprovedNameAndTime = value; NotifyPropertyChanged(); }
        }


        private string ShowContens;//显示描述
        public string showContens
        {
            get { return StringUtils.ReplaceHtmlTag(contents); }
            set { ShowContens = value; NotifyPropertyChanged(); }
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

        public bool IsToDepartment { get; set; }

        public ObservableCollection<Assignments> assignments { get; set; }
        public ObservableCollection<Coords> coords { get; set; }
        public ObservableCollection<Enterprise> enterprise { get; set; }
        public ObservableCollection<Guid> taskenterprises { get; set; }
        public ObservableCollection<taskassignment> taskassignments { get; set; }
        public ObservableCollection<taskassignment2> taskassignments2 { get; set; }//用来查找assignmentid

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
        public Guid? grid { get; set; }//所属网格的id
        public int type { get; set; }//0指派给网格员(staff、grid必传) 1指派给网格(grid必传) 2指派给部门人员(staff、dept必传) 3指派给部门(dept必传)
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
        public int? type { get; set; }//0给网格员 1给网格 2给部门人员 3给部门
        public Guid? grid { get; set; }
        public taskassignment nextLevel { get; set; }
    }

    public class taskassignment2 : BaseModel
    {
        public Guid? id { get; set; }
        public Guid? staff { get; set; }
        public Guid? grid { get; set; }
        public string dept { get; set; }
        public string type { get; set; }
    }
}
