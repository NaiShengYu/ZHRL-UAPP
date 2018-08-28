using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AepApp.Models
{
    //事件执行记录
    public class GridEventHandleRecordModel
    {
        public Guid id { get; set; }
        public DateTime date { get; set; }
        public Guid staff { get; set; }
        public Guid gridcell { get; set; }
        public string gridName { get; set; }
        public string parentName { get; set; }
        public string results { get; set; }
        public ObservableCollection<AttachmentInfo> attachments { get; set; }

        public string SubTitle { get { return parentName + " - " + gridName; } }
    }
}
