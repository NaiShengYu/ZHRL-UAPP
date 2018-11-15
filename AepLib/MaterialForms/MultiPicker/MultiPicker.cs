using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace XFMultiPicker
{
    public class MultiPickerView<T> : Button where T : class
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(ObservableCollection<T>), typeof(MultiPickerView<T>),
                null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemsProperty =
            BindableProperty.Create("SelectedItems", typeof(ObservableCollection<T>), typeof(MultiPickerView<T>),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedItemsChanged);

        public MultiPickerView()
        {
            BackgroundColor = Color.Transparent;
            Command = new Command(() => { PopupNavigation.Instance.PushAsync(PopupPage); }, () => SelectedItems != null);
            PopupPage = new MultiPickerPopupPage<T>();
        }

        public MultiPickerPopupPage<T> PopupPage { get; set; }

        public ObservableCollection<T> ItemsSource
        {
            get { return (ObservableCollection<T>) GetValue(ItemsSourceProperty); }
            set { if (value == null) return; SetValue(ItemsSourceProperty, value); }
        }

        public ObservableCollection<T> SelectedItems
        {
            get { return (ObservableCollection<T>) GetValue(SelectedItemsProperty); }
            set { if (value == null) return; SetValue(SelectedItemsProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as MultiPickerView<T>;
            var items = newvalue as ObservableCollection<T>;
            if (items != null)
                picker.PopupPage.Items = items;
        }

        private static void OnSelectedItemsChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as MultiPickerView<T>;
            ((Command)picker.Command).ChangeCanExecute();
            var items = newvalue as ObservableCollection<T>;
            if (items != null)
                picker.PopupPage.SelectedItems = items;
        }
    }
}