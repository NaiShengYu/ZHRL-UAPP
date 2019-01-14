using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    /// <summary>
    /// 环境质量返回的数据模型
    /// </summary>
    public class VOCSiteResult
    {
        public int count { get; set; }//厂界/排口接口返回的总数
        public int Totals { get; set; }
        public List<VOCSiteListModel> Items = new List<VOCSiteListModel>();
    }
}
