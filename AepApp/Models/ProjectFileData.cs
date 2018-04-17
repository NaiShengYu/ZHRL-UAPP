using System;
namespace AepApp.Models
{
    public class ProjectFileData
    {

        public string ID { get; set; }
        public string FILENAME { get; set; }
        public string SUFFIX { get; set; }
        public string UPDATEDATE { get; set; }
        public float MODULECODE { get; set; }
        public string FILESAVENAME { get; set; }
        public string UPDATEUSER { get; set; }
        public string YEAR { get; set; }
        public string MONTH { get; set; }
        public string PROJECTID { get; set; }
        public string BUSINESSTYPE { get; set; }
        public string FULLNAME { get; set; }

        public string modulName
        {
            get
            {
                if (MODULECODE == 1)
                    return "环评文件";

                if (MODULECODE == 2)
                    return "拟审批意见";

                if (MODULECODE == 3)
                    return "环境批复";

                if (MODULECODE == 5)
                    return "检测报告";

                return null;

            }
            set
            {

            }

        }

        public string UPDATEDATEName{

            get{
                if (UPDATEDATE.Length > 0)
                    return UPDATEDATE.Substring(0, 10);
                else
                    return "";
            }
            set{}

        }


    }
}
