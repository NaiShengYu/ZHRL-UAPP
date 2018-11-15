using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AepApp.Models
{
    public class MultiSelectDataType : NameType
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        // some more properties:
        private string attype; //检测项目分类
        public string SampleAttype
        {
            get { return attype; }
            set
            {
                if (value == attype) return;
                attype = value;
                OnPropertyChanged();
            }
        }
    }

    // some base class
    public class NameType : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
