using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    public class TaskTemplateModel
    {
        public Guid? id { get; set; }
        public string period;
        public string title { get; set; }
        public string replycontents { get; set; }
        public string contents { get; set; }

        public string Period { get { return period + "天"; } }

    }
}
