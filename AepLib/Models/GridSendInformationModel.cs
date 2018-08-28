using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AepApp.Models
{
    //下发信息
    public class GridSendInformationModel
    {
        public string id { get; set; }
        public Guid staff { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public DateTime date { get; set; }
        public string contents { get; set; }
        public string subTitle
        {
            get { return TimeUtils.DateTime2YMD(date) + ": "+contents; }
        }
        public ObservableCollection<AttachmentInfo> attachments { get; set; }
        public int Count
        {
            get { return attachments.Count; }
        }
    }

    public class AttachmentInfo
    {
        public Guid attach_id { get; set; }
        public string attach_url { get; set; }
        public string attach_title { get; set; }
        public string attach_filename { get; set; }
    }
}
