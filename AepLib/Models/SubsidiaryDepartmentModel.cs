using System;
namespace AepApp.Models
{
    public class SubsidiaryDepartmentModel : BaseModel
    {

        private string Assignment { get; set; }
        public string assignment
        {
            get { return Assignment; }
            set { Assignment = value; NotifyPropertyChanged(); }
        }

        private string ToDept { get; set; }
        public string toDept
        {
            get { return ToDept; }
            set { ToDept = value; NotifyPropertyChanged(); }
        }

        private string DeptName { get; set; }
        public string deptName
        {
            get { return DeptName; }
            set { DeptName = value; NotifyPropertyChanged(); }
        }

    }
}
