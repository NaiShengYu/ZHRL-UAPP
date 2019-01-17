using System;
namespace AepApp.Models
{
    public class EnterpriseInformationModel 
    {
            public string id { get; set; }
            public string name { get; set; }
            public string creditcode { get; set; }
            public string orgcode { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string province { get; set; }
            public string city { get; set; }
            public string district { get; set; }
            public string town { get; set; }
            public string address { get; set; }
            public string zipcode { get; set; }
            public string legal { get; set; }
            public string legalmobile { get; set; }
            public string telephone { get; set; }
            public string contacts { get; set; }
            public string contactstelephone { get; set; }
            public string contactsmobile { get; set; }
            public string contactsemail { get; set; }
            public string fullAddress {
                  get{
                return province + " " + city + " " + district + " " + town + " " + address;
            }
            set { }
        }
            

    }
}
