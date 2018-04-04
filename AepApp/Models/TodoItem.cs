using SQLite;

namespace Todo
{
    public class TodoItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string SiteId { get; set; }
        public string SiteAddr { get; set; }
    }
}