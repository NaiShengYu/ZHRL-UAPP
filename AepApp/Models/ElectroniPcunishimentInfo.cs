
using System;
namespace AepApp.Models
{
    public class ElectroniPcunishimentInfo
    {

        public string id { get; set; }
        public string content { get; set; }
        public string date { get; set; }
        public string processing { get; set; }
        public string state { get; set; }
        public string type { get; set; }

        public string dateName
        {
            get
            {
                if (date.Length >= 10)
                    return "处理时间：" + date.Substring(0, 10);
                else
                    return "处理时间:";
            }
            set
            {

            }
        }
                  
        public string processingName
        {
            get
            {
                    return "处理人员:" + processing;
            }
            set
            {

            }



        }

    }
}
