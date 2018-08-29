using System;
namespace AepApp.Models
{
    public class GridEventFollowModel:BaseModel
    {
        public bool canEdit { get; set; }
        public Guid id { get; set; }
        public string title { get; set; }
        public string staffName { get; set; }
        public string staffTel { get; set; }
        public string rowState { get; set; }
        public Guid incident { get; set; }
        public Guid previous { get; set; }
        public Guid staff { get; set; }
        public DateTime date { get; set; }
        public int level { get; set; }
        public int state { get; set; }

        private string remarks { get; set; }
        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; NotifyPropertyChanged(); }
        }

    }
}
