
using SQLite;



namespace Todo

{

    public class TodoItem

    {

        [PrimaryKey, AutoIncrement]

        public int ID { get; set; }

        public string siteName { get; set; }

        public string siteAddr { get; set; }

        public string id { get; set; }

       // public string Notes { get; set; }

       // public bool Done { get; set; }

    }

}