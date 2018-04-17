using System;
namespace AepApp.Models
{
    public class EmissionPermitManagement
    {

        internal class EmissionPermitManagementList
        {
            public string ISSUEDATE { get; set; }
            public string KIND { get; set; }
            public string LICENCEID { get; set; }
            public string PKID { get; set; }
            public string VALIDITY { get; set; }
            public string ID { get; set; }

            public string LICENCEIDName
            {
                get
                {
                    return "许可证编号:" + LICENCEID;
                }
                set { }
            }

            public string KINDName
            {
                get
                {
                    return "许可证类型:" + KIND;
                }
                set { }
            }

            public string ISSUEDATEName
            {
                get
                {
                    return "发证时间:" + ISSUEDATE;
                }
                set { }

            }

            public string VALIDITYName
            {
                get
                {
                    return "有效期:" + VALIDITY;
                }
                set { }
            }

        }



        internal class EmissionPermitManagementInfo
        {

            public string id { get; set; }
            public string name { get; set; }
            public string credit_no { get; set; }
            public string code { get; set; }
            public string registerAdd { get; set; }
            public string address { get; set; }
            public string enddate { get; set; }
            public string startdate { get; set; }
            public string legal { get; set; }
            public string issuing { get; set; }
            public string issuedate { get; set; }
            public string industry { get; set; }
            public string enterid { get; set; }
            public string state { get; set; }

        }



    }
}
