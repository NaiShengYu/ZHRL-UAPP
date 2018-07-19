using System;
namespace AepApp.Models
{
    public class AddDataForChemicolOrLHXZModel
    {
        public class ItemsBean
        {
            //位置
            public string lat { get; set; }
            public string lng { get; set; }
            //样本类型
            public string yangBenLeiXing { get; set; }
            //检测值
            public string jianCeZhi { get; set; }
            //监测值单位
            public string unitName { get; set; }
            public string unitId { get; set; }

            //样本名称
            public string factorName { get; set; }

            public string factorId { get; set; }
            public string datatype { get; set; }

        }
    }
}
