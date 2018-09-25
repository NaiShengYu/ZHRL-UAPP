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
        private int type { get; set; }
        public string Type { get { return ConstConvertUtils.GridInformationType2String(type); } }
        public DateTime? date { get; set; }
        public string contents { get; set; }
        public string subTitle
        {
            get { return TimeUtils.DateTime2YMD(date) + ": "+ StringUtils.ReplaceHtmlTag(contents, 40); }
        }
        public ObservableCollection<AttachmentInfo> attachments { get; set; }
        public int Count
        {
            get { return attachments.Count; }
        }
    }

    public class AttachmentInfo
    {
        public Guid id { get; set; }
        public string url { get; set; }//本地/url地址
        public string title { get; set; }
        public string filename { get; set; }
        public bool isUploaded { get; set; }//是否已上传

        public string fileLog {
            get { return StringUtils.fileLogWithFileName(filename); }
            set {}
        }
    }
}
