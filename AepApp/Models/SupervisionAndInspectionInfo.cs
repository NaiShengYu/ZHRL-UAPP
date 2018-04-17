using System;
namespace AepApp.Models
{
    public class SupervisionAndInspectionInfo
    {

        public string ID { get; set; }
        public string TEMPLATEID { get; set; }
        public string VALUE { get; set; }
        public string INAME { get; set; }
        public string UNIT { get; set; }
        public string TNAME { get; set; }

        public string ValueUnit
        {
            get
            {
                return VALUE + "(" + UNIT + ")";
            }

            set { }

        }

    }
}
