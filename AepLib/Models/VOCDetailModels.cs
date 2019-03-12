using System;

namespace AepApp.Models
{
    public class VOCDetailModels
    {
        public class Factors : BaseModel
        {

            public string id { get; set; }//因子ID
            public string catId { get; set; }//VOC类型ID
            public string catName { get; set; }//VOC类型名称            
            public string code { get; set; }//因子编码
            public string facttype { get; set; }//因子类型：0、化学品 1、气象参数 9、其他因子
            public string datatype { get; set; }//因子排序值
            public string digit { get; set; }//默认小数位数
            public string index { get; set; }//因子排序值
            public string name { get; set; }//因子名称
            public string refid { get; set; }//所关联化学品ID（如有）
            public string stype { get; set; }//测试类型: 0:其他,1:必测,2:选测
            public string unitId { get; set; }
            public string unitName { get; set; }

            private string Val = "";
            public string val
            {
                get { return Val; }
                set { Val = value; NotifyPropertyChanged(); }
            }

        }

        public class Charts : BaseModel
        {
            public DateTime? date { get; set; }
            public string name { get; set; }
            public double val { get; set; }
        }
    }
}