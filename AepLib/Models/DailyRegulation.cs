using System;
namespace AepApp.Models
{
    public class DailyRegulation
    {
        public DateTime starttime { get; set; }
        public string code { get; set; }
        public string rummager { get; set; }
        public string remark { get; set; }
        public string type { get; set; }
        public string id { get; set; }

        //public string SUPERVISEDATEInfo
        //{
        //    get
        //    {
        //        if (SUPERVISEDATE.Length >= 10)
        //        {
        //            return SUPERVISEDATE.Substring(0, 10);
        //        }
        //        return "";

        //    }
        //    set { }
        //}

        //public string SUPERVISEDATEName
        //{
        //    get
        //    {
        //        if (SUPERVISEDATE.Length >= 10)
        //        {
        //            return "执法时间：" + SUPERVISEDATE.Substring(0, 10);
        //        }
        //        return "执法时间：";

        //    }
        //    set { }

        //}

        //public string SUPERVISORName
        //{

        //    get
        //    {
        //        return "执法人员：" + SUPERVISOR;
        //    }
        //    set { }

        //}

        //public string CONTEXTName
        //{
        //    get
        //    {
        //        return "执法内容：" + CONTEXT;
        //    }
        //    set { }
        //}
        //public string IMPROVECONTEXTName
        //{
        //    get
        //    {
        //        return "执法要求：" + IMPROVECONTEXT;
        //    }
        //    set { }
        //}

    }
}
