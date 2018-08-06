using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseLibrary.ViewModels
{
    public class CollectionAndTransportSampleModel : BaseViewModel
    {

        public string time { get; set; }
        public string num { get; set; }

        private string Type;

        public string type
        {
            get { return Type; }
            set { Type = value; OnPropertyChanged("type"); }
        }

        public string state { get; set; }

    }
}
