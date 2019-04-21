using System;
namespace AepApp.Models
{
    public class UserDepartmentsModel
    {
        public Guid? parentid { get; set; }
        public Guid id { get; set; }
        public string name { get; set; }
    }
}
