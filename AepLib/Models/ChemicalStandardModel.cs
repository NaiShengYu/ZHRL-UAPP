using System;
namespace AepApp.Models
{
    public class ChemicalStandardModel
    {
        public string name { get; set; }
        public string source { get; set; }
        public string metric1 { get; set; }
        public string value1 { get; set; }
        public string metric2 { get; set; }
        public string value2 { get; set; }
        public string metric3 { get; set; }
        public string value3 { get; set; }
        public string metric4 { get; set; }
        public string value4 { get; set; }
        public string metric5 { get; set; }
        public string value5 { get; set; }
        public string metric6 { get; set; }
        public string value6 { get; set; }

        public string allValue
        {
            get
            {
                string aaa = "";
                if (!string.IsNullOrWhiteSpace(metric1)) aaa = metric1;
                if (!string.IsNullOrWhiteSpace(value1)) aaa = aaa +value1 + "\n";
                if (!string.IsNullOrWhiteSpace(metric2)) aaa = aaa + metric2;
                if (!string.IsNullOrWhiteSpace(value2)) aaa = aaa + value2 + "\n";
                if (!string.IsNullOrWhiteSpace(metric3)) aaa = aaa + metric3;
                if (!string.IsNullOrWhiteSpace(value3)) aaa = aaa + value3 + "\n";
                if (!string.IsNullOrWhiteSpace(metric4)) aaa = aaa + metric4;
                if (!string.IsNullOrWhiteSpace(value4)) aaa = aaa + value4 + "\n";
                if (!string.IsNullOrWhiteSpace(metric5)) aaa = aaa + metric5;
                if (!string.IsNullOrWhiteSpace(value5)) aaa = aaa + value5 + "\n";
                if (!string.IsNullOrWhiteSpace(metric6)) aaa = aaa + metric6;
                if (!string.IsNullOrWhiteSpace(value6)) aaa = aaa + value6;
                if(aaa.Length > 2)
                {
                    string bbb = aaa.Substring(aaa.Length-1);
                    if(bbb == "\n")
                    {
                        aaa = aaa.Substring(0, aaa.Length - 1);
                    }
                }
                return aaa;
            }
            set { }
        }
    }
}
