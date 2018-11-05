using System.ComponentModel;
using SQLite;

namespace Todo
{
    public class TodoItem: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string SiteId { get; set; }
        public string SiteAddr { get; set; }
        public string customerName { get; set; }
        public string appTitle { get; set; }
        public string majorVersion { get; set; }
        public string minorVersion { get; set; }
        public string revision { get; set; }
        public string date { get; set; }

        private bool _isCurrent = false;
        public bool isCurrent
        {
            get { return _isCurrent; }
            set { _isCurrent = value; NotifyPropertyChanged("isCurrent"); }
        }

    }
}