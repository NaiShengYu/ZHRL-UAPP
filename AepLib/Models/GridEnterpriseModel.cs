﻿

using System;
namespace AepApp.Models
{
    public class GridEnterpriseModel
    {

        public string id { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }

        public string address { get{
                return lat.ToString("f6") +", "+ lng.ToString("f6");
            } set{} }

    }
}