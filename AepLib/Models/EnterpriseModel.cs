using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class EnterpriseModel
    {
        public Guid id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string name { get; set; }

        //下面四个值从另一个接口获取后，与上面的字段组合起来
        public int? count { get; set; }
        public string stvalue { get; set; }
        public string time { get; set; }
        public decimal? value { get; set; }

        public bool countIsVisible
        {
            get { return count != null && count.Value > 0; }
        }
    }
}