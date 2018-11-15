using AepApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AepApp.ViewModel
{
    public class MultiSelectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MultiSelectDataType> _selectedItems;
        public ObservableCollection<MultiSelectDataType> AvailableItems { get; set; }

        public ObservableCollection<MultiSelectDataType> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                if (Equals(value, _selectedItems)) return;
                if (_selectedItems != null)
                    _selectedItems.CollectionChanged -= SelectedItemsCollectionChanged;
                _selectedItems = value;
                if (value != null)
                    _selectedItems.CollectionChanged += SelectedItemsCollectionChanged;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedItems));
        }

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
