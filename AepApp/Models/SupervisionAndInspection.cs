using System;
namespace AepApp.Models
{
    public class SupervisionAndInspection
    {

        public string ID { get; set; }
        public string TEMPLATEID { get; set; }
        public string MONITORDATE { get; set; }
        public string INPUTUSER { get; set; }
        public string CHECKPERSON { get; set; }
        public string RESULT { get; set; }
        public string NAME { get; set; }
        public string FLAG { get; set; }
        public string RN { get; set; }

        public string NAMEName
        {
            get
            {
                return "排口名称：" + NAME;
            }
            set { }
        }
        public string INPUTUSERName
        {
           
            get
            {         return "录入人：" + INPUTUSER;         
            }
            set { }

        }
        public string MONITORDATEName
        {
            get
            {
                if (MONITORDATE.Length >= 10)
                    return "时间：" + MONITORDATE.Substring(0, 10);

                return "时间：";
            }
            set { }

        }
        public string CHECKPERSONName
        {
            get
            {
                return "检查人：" + CHECKPERSON;
            }
            set { }

        }



    }
}
