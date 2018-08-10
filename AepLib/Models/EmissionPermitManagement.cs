using System;
namespace AepApp.Models
{
    public class EmissionPermitManagement
    {

        public class EmissionPermitManagementList
        {
            public DateTime ISSUEDATE { get; set; }
            public string KIND { get; set; }
            public string LICENCEID { get; set; }
            public string PKID { get; set; }
            public float VALIDITY { get; set; }
            public string ID { get; set; }
        }



        public class EmissionPermitManagementInfo
        {

            public string id { get; set; }
            public string name { get; set; }
            public string credit_no { get; set; }
            public string code { get; set; }
            public string registerAdd { get; set; }
            public string address { get; set; }
            public DateTime enddate { get; set; }
            public DateTime startdate { get; set; }
            public string legal { get; set; }
            public string issuing { get; set; }
            public DateTime issuedate { get; set; }
            public string industry { get; set; }
            public string enterid { get; set; }
            public string state { get; set; }

        }



    }
}
