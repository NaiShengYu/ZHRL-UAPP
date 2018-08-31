using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    //网格化模块附件上传返回值
    public class GridAttachmentResultModel
    {
        public Guid id { get; set; }
        public string fname { get; set; }
        public string url { get; set; }
        public string extension { get; set; }//eg: .png
    }
}
