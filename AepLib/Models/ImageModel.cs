namespace AepApp.Models
{
    public class ImageModel : BaseModel
    {
        public string Id { get; set; }
        public string Url { get; set; }
        private int status;
        public int Status { get { return status; } set { status = value; NotifyPropertyChanged(); } }//0 正常 1选中 2选中删除
    }

}
