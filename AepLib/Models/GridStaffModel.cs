using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    /// <summary>
    /// 网格员
    /// </summary>
    public class GridStaffModel
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public string mobile { get; set; }
        public int? grade { get; set; }
        public Guid? grid { get; set; }
        public string state { get; set; }
        public string gridName { get; set; }
        public int? gridLevel { get; set; }
        public DateTime? registerdate { get; set; }

        public Guid? user { get; set; }


    }
}
