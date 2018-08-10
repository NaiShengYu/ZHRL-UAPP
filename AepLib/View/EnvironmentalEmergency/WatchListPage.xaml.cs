using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class WatchListPage : ContentPage
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
            Navigation.PushAsync(new WatchListInfoPage());
            listView.SelectedItem = null;
        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public WatchListPage()
        {
            InitializeComponent();

            var item1 = new item
            {
                name = "组一",
                num = "3个成员",
                startTime = "2018-05-24 17:30",
                endTime = "2018-05-25 17:30",
                longTime = "24\n小时",

            };

            dataList.Add(item1);

            var item2 = new item
            {
                name = "组二",
                num = "3个成员",
                startTime = "2018-05-24 17:30",
                endTime = "2018-05-25 05:30",
                longTime = "12\n小时",
             };

            dataList.Add(item2);

            var item3 = new item
            {
                name = "组三",
                num = "3个成员",
                startTime = "2018-05-24 17:30",
                endTime = "2018-05-24 23:30",
                longTime = "8\n小时",
            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;
        }

        internal class item
        {
            public string name { get; set; }
            public string num { set; get; }
            public string startTime { set; get; }
            public string endTime { set; get; }
            public string longTime { set; get; }
        }

    }
}
