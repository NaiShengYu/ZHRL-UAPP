using Rg.Plugins.Popup.Pages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace XFMultiPicker
{
    public class MultiPickerPopupPage<T> : PopupPage
    {
        private ObservableCollection<T> _items;
        private ObservableCollection<T> _selectedItems;

        private ObservableCollection<WrappedSelection<T>> _wrappedItems;
        private readonly ListView _listView;

        public MultiPickerPopupPage()
        {
            HasSystemPadding = true;
            Padding = new Thickness(50, 150);

            var header = new Label
            {
                Text = "请选择",
                TextColor = Color.Black,
                FontSize = 16,
                Margin = new Thickness(0, 15),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            _listView = new ListView
            {
                BackgroundColor = Color.FromRgb(240, 240, 240),
                ItemTemplate = new DataTemplate(typeof(WrappedItemSelectionTemplate)),
                Header = header,
            };
            _listView.ItemTapped += (obj, item) =>
            {
                WrappedSelection<T> i = item.Item as WrappedSelection<T>;
                i.IsSelected = !i.IsSelected;
            };

            Content = new Frame
            {
                Padding = 10,
                BackgroundColor = Color.Transparent,
                Content = _listView
            };
        }

        public ObservableCollection<T> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                List<WrappedSelection<T>> l = _items.Select(item => new WrappedSelection<T> { Item = item, IsSelected = false }).ToList();
                WrappedItems = new ObservableCollection<WrappedSelection<T>>(l);
            }
        }

        public ObservableCollection<T> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                SetSelection(value);
            }
        }

        public ObservableCollection<WrappedSelection<T>> WrappedItems
        {
            get { return _wrappedItems; }
            set
            {
                _wrappedItems = value;
                _listView.ItemsSource = value;
            }
        }

        protected override void OnDisappearing()
        {
            _selectedItems.Clear();
            foreach (T item in GetSelection())
            {
                _selectedItems.Add(item);
            }
            base.OnDisappearing();
            MessagingCenter.Send<ContentPage, ObservableCollection<T>>(this, "SelectData", _selectedItems);

        }

        private ObservableCollection<T> GetSelection()
        {
            List<T> l = WrappedItems.Where(item => item.IsSelected).Select(wrappedItem => wrappedItem.Item).ToList();
            return new ObservableCollection<T>(l);
        }

        private void SetSelection(ObservableCollection<T> selectedItems)
        {
            foreach (WrappedSelection<T> wrappedItem in WrappedItems)
                wrappedItem.IsSelected = selectedItems.Contains(wrappedItem.Item);
        }

        public class WrappedSelection<T> : INotifyPropertyChanged
        {
            private bool _isSelected;
            public T Item { get; set; }

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }

        public class WrappedItemSelectionTemplate : ViewCell
        {
            public WrappedItemSelectionTemplate()
            {
                var name = new Label();
                name.TextColor = Color.Black;
                name.VerticalOptions = LayoutOptions.Center;
                name.Margin = new Thickness(20, 0);
                name.SetBinding(Label.TextProperty, new Binding("Item.Name"));
                
                var rightImg = new Image
                {
                    Source = "greentick",
                };
                rightImg.SetBinding(Image.IsVisibleProperty, new Binding("IsSelected"));

                var layout = new RelativeLayout();

                layout.Children.Add(name,
                    Constraint.Constant(5),
                    Constraint.Constant(5),
                    Constraint.RelativeToParent(p => p.Width - 60),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );

                layout.Children.Add(rightImg,
                    Constraint.RelativeToParent(p => p.Width - 55),
                    Constraint.Constant(5),
                    Constraint.Constant(50),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );


                View = layout;
            }
        }
    }
}