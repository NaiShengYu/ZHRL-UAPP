using System;
namespace AepApp.Models
{
    public class GridUserInfoModel
    {

        public Guid id
        {
            get;
            set;
        }

        public Guid gridcell
        {
            get;
            set;
        }

        public string gridName
        {
            get;
            set;
        }

        public int gridLevel
        {
            get;
            set;
        }
    }
}
