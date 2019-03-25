using System.Collections.Generic;
using System.Text;

//采样检测项目
namespace AepApp.Models
{
    public class SampleExamineModel : BaseModel
    {
        public string Totals { get; set; }
        public List<SampleExamineItem> items { get; set; }
    }

    public class SampleExamineItem : BaseModel
    {
        public string id { get; set; }
        public string name { get; set; }//检测项目名称
        public List<SampleFactor> factors { get; set; }
        public string allFactor
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (factors == null || factors.Count == 0)
                {
                    return "";
                }
                for (int i = 0; i < factors.Count; i++)
                {
                    if (i > 5)
                    {
                        break;
                    }
                    SampleFactor f = factors[i];
                    s.Append(",").Append(f.factorname);
                }
                if (s.Length > 0)
                {
                    s = s.Remove(0, 1);
                }
                return s.ToString();
            }
            set
            {
            }
        }
    }

    //采样因子
    public class SampleFactor : BaseModel
    {
        public string factorid { get; set; }
        public string factorname { get; set; }//所含因子名称
    }
}
