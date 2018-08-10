using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class ReqChemicalPageModel 
	{
        public class ReqChemicalBean {
            public int count { get; set; }
            public List<ItemsBean> items { get; set; }
        }
        public class ItemsBean {          
            public String id { get; set; }
            public String chinesename { get; set; }
            public String englishname { get; set; }
            public string elname {
                get {
                    string[] strs = englishname.Replace("<BR>", "\n").Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length == 0) return null;
                    return strs[0];
                }
            }
            public String cas { get; set; }

        }
    }
}