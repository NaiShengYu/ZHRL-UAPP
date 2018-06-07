
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueSitePage : ContentPage
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
            Navigation.PushAsync(new RescueMaterialsPage());

            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public RescueSitePage()
        {
            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                Navigation.PushAsync(new RescueSiteMapPage());

            }));


            var item1 = new item
            {
                name = "第一小学",
                address = "浙江省宁波市鄞州区中兴路360号",
            };

            dataList.Add(item1);

            var item2 = new item
            {
                name = "中环瑞蓝",
                address = "浙江省宁波市鄞州区江南路1958号宁波检测认证园分园5楼",
            };

            dataList.Add(item2);

            var item3 = new item
            {
                name = "宁波大学",
                address = "浙江省宁波市江北区风华路818号",
            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;
        }

        internal class item
        {
            public string name { get; set; }
            public string address { set; get; }

        }

    }
}
