using System;
namespace AepApp.Models
{
    public class LettersAndVisits
    {
        //internal class LettersAndVisitsList
        //{
        public string id { get; set; }
        public string TITLE { get; set; }
        public string ORDERDATE { get; set; }
        public string CLOSECASESTATE { get; set; }
        public string CLOSECASEDATE { get; set; }
        public string ENDLINEDATE { get; set; }
        public string STATEDESC { get; set; }
        //public string FileData { get; set; }

        public string TITLEName
        {
            get
            {
                return "信访标题：" + TITLE;
            }
            set { }
        }
        public string ORDERDATEName
        {
            get
            {
                if (ORDERDATE.Length >= 10)
                    return "投诉时间：" + ORDERDATE.Substring(0, 10);

                return "投诉时间：";
            }
            set { }

        }
        public string CLOSECASEDATEName
        {
            get
            {
                if (CLOSECASEDATE.Length >= 10)
                    return "结束时间：" + ORDERDATE.Substring(0, 10);

                return "结束时间：";
            }
            set { }

        }
        public string STATEDESCName
        {
            get
            {
                return "状态：" + STATEDESC;
            }
            set { }

        }
    }



    //}
}
