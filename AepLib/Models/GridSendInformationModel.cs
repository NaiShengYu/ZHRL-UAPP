using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    public class GridSendInformationModel
    {
        public string Disseminate { get; set; }
        public string staff { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public DateTime date { get; set; }
        public string contents { get; set; }
        public string subTitle
        {
            get { return TimeUtils.DateTime2YMD(date) + ": "+contents; }
        }

    }
}
