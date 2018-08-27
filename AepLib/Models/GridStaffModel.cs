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
        public string id { get; set; }
        public string uSername { get; set; }
        public string mobil { get; set; }
        public int grade { get; set; }
        public string grid { get; set; }
        public string state { get; set; }
        public string remarks { get; set; }
        public DateTime registerdate { get; set; }

    }
}
