using System;
using System.Collections.ObjectModel;

namespace AepApp.Models
{
    public class GridAllDepartmentsModel
    {
        public string title { get; set; }
        public Guid id { get; set; }
        public string name { get; set; }
        public string parentName { get; set; }
        public string parentId { get; set; }
        public string type { get; set; }//0表示本部门，1表示外部机构
        public ObservableCollection<GridAllDepartmentsModel> subDepts { get; set; }
    }
}
