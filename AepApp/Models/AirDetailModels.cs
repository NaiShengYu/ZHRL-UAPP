using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class AirDetailModels 
	{
        public class Factors {
            public string id { get; set; }
            public string refid { get; set; }
            public string gasName {
                get {
                    if (name.Equals("臭氧")){
                        return "O₃";
                    }
                    if (name.Equals("一氧化碳")) {
                        return "CO";
                    }
                    if (name.Equals("二氧化氮")) {
                        return "NO₂";
                    }
                    if (name.Equals("二氧化硫")) {
                        return "SO₂";
                    }
                    return name;
                }
                set {
                }
            }
            public string catId { get; set; }
            public string facttype { get; set; }
            public string datatype { get; set; }
            public string catIdx { get; set; }
            public string name { get; set; }
        }
        public class FacValsParam {
            public string facId { get; set; }
            public int fromType { get; set; }
            public string[] refIds { get; set; }
        }

    }
}