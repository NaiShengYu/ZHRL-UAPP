using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class SuccessCaseModels
    {
        public class SuccessCaseBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
        {
            public CasesBean cases { get; set; }
        }
        public class CasesBean
        {
            public int totalCount { get; set; }
            public List<ItemsBean> items { get; set; }
        }
        public class ItemsBean
        {
            public string name { get; set; }
            public Object creatorUserName { get; set; }
            public string notes { get; set; }
            public string id { get; set; }
            public List<FilesBean> files;
        }
        public class FilesBean
        {
            public string storeUrl { get; set; }
            public string name { get; set; }
            public string format { get; set; }
            public string size { get; set; }
            public int sort { get; set; }
            public Object creatorUserName { get; set; }
            public string id { get; set; }
            public string imgSourse
            {
                get
                {
                    if (format.Equals("doc"))
                    {
                        return "doc.png";
                    }
                    else if (format.Equals("pdf"))
                    {
                        return "pdf.png";
                    }
                    else {
                        return "";
                    }
                }
            }
        }
    }

    public class FileFormatStringToIconImageConverter : IValueConverter
    {
        private static FileFormatStringToIconImageConverter instance;
        public static FileFormatStringToIconImageConverter Instance { get { return instance; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = value as string;

            string resstring = "";
            switch (format)
            {
                case "pdf": resstring = ""; break;
                case "doc": resstring = ""; break;
            }
            ImageSource img = ImageSource.FromResource("");

            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
