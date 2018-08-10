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

            public string field_testA
            {
                get
                {
                    return Shorttext(field_test);
                }
                set { }
            }

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

            public string elnameA {
                get{
                    return Shorttext(elname);
                }
                set{}
            }

            public string responseA
            {
                get
                {
                    return Shorttext(response);
                }
                set { }
            }

            public string env_impactA
            {
                get
                {
                    return Shorttext(env_impact);
                }
                set { }
            }

            public string applicationA
            {
                get
                {
                    return Shorttext(application);
                }
                set { }
            }

            private string Shorttext(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return "";
                string a = HtmlRemoval.StripTagsRegex(input);
                string[] strs = a.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 0) return null;
                if (strs[0].Length > 16 || strs.Length > 1)
                {
                    if (strs[0].Length > 16) return strs[0].Substring(0, 16) + "...";
                    return strs[0] + "...";
                }
                return strs[0];
            }

        }

    }

    
}