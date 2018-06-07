using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class ChemicalInfoPageModel
    {
        public class ChemicalInfoPageModelBean
        {

            public string alias { get; set; }
            public string application { get; set; }
            public string boiling_point { get; set; }
            public string cas { get; set; }
            public string category { get; set; }
            public string characteristics { get; set; }
            public string chinesename { get; set; }
            public string danger_mark { get; set; }
            public string density { get; set; }
            public string dissolvability { get; set; }
            public string englishname { get; set; }
            public string elname
            {
                get
                {
                    string[] strs = englishname.Replace("<BR>", "\n").Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length == 0) return null;
                    return strs[0];
                }
            }
            public string env_impact { get; set; }
            public string field_test { get; set; }
            public string flash_point { get; set; }
            public string gb { get; set; }
            public string chemid { get; set; }
            public string melting_point { get; set; }
            public string meltingPoint
            {
                get
                {
                    string[] strs = melting_point.Replace("<BR>", "\n").Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length == 0) return null;
                    return strs[0];
                }
            }

            public string molecular_mass { get; set; }
            public string molecular_str { get; set; }
            public string priority { get; set; }
            public string response { get; set; }
            public string source { get; set; }
            public string stability { get; set; }
            public string vapour_pressure { get; set; }

        }

    }
}