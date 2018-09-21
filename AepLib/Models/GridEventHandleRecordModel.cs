using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace AepApp.Models
{
    //任务执行记录
    public class GridTaskHandleRecordModel :BaseModel
    {
        public Guid? id { get; set; }
        public DateTime date { get; set; }
        public Guid? staff { get; set; }
        public Guid? enterprise { get; set; }
        public Guid? gridcell { get; set; }
        public string gridName { get; set; }
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

        public string SubTitle { get { return parentName + " - " + gridName; } }

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
