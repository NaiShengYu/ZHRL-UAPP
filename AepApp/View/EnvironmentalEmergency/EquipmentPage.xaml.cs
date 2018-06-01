using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EquipmentPage : ContentPage
    {
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;
            Navigation.PushAsync(new EquipmentInfoPage(item.name));

            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public EquipmentPage()
        {
            InitializeComponent();

            var item1 = new item
            {
                name = "VOC气体检测仪",
                message = "品牌/类型",
            };

            dataList.Add(item1);

            var item2 = new item
            {
                name = "光谱显示仪",
                message = "品牌/类型",
            };

            dataList.Add(item2);

            var item3 = new item
            {
                name = "磁共振设备",
                message = "品牌/类型",
            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;
        }

        internal class item
        {
            public string name { get; set; }
            public string message { set; get; }

        }
    }
}
