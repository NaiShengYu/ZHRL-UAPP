using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueMaterialsPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public RescueMaterialsPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = "救援物资" + "\r\n" + "中环瑞蓝";

            var item1 = new item
            {
                name = "帐篷",
                num = "110个",
            };

            dataList.Add(item1);

            var item2 = new item
            {
                name = "10公升食用水",
                num = "180桶",
            };

            dataList.Add(item2);

            var item3 = new item
            {
                name = "被袋",
                num = "80袋",
            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;
        }

        internal class item
        {
            public string name { get; set; }
            public string num { set; get; }

        }
    }
}
