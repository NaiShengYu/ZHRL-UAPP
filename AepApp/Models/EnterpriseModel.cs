using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class EnterpriseModel
    {
        public int count { get; set; }
        public string id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string name { get; set; }
        public string stvalue { get; set; }
        public string time { get; set; }
        public decimal value { get; set; }
    }
}