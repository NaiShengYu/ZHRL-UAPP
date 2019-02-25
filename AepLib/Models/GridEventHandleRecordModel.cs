using AepApp.Tools;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AepApp.Models
{
    //任务执行记录
    public class GridTaskHandleRecordModel : BaseModel
    {
        public Guid? id { get; set; }
        public DateTime date { get; set; }
        public Guid? staff { get; set; }
        public Guid? enterprise { get; set; }
        public Guid? gridcell { get; set; }
        private string gName;
        public string gridName { get { return gName; } set { gName = value; NotifyPropertyChanged(); } }
        public string editName { get; set; }
        public string parentName { get; set; }
        public string results { get; set; }
        public string assignment { get; set; }
        public ObservableCollection<AttachmentInfo> attachments { get; set; }

        private string EnterpriseName;
        public string enterpriseName
        {
            get { return EnterpriseName; }
            set { EnterpriseName = value; NotifyPropertyChanged(); }
        }


        public string staffName { get; set; }//通过staff获取后赋值

        //public string SubTitle { get { return parentName + " - " + gridName; }set { } }
        private string subTitle;
        public string SubTitle { get { return subTitle; } set { subTitle = value; NotifyPropertyChanged(); } }

        public string dateStr
        {
            get { return TimeUtils.DateTime2YMDHM(date); }
        }

        private Func<string, Task<string>> _evaluateJavascript;//webview调用js
        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return _evaluateJavascript; }
            set { _evaluateJavascript = value; }
        }
    }
}
