﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class WatchListInfoPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;
            //Navigation.PushAsync(new RescueMaterialsPage());

            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public WatchListInfoPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            var item1 = new item
            {
                name = "袁晓东",
                message = "10年Android攻城狮\n10年Android攻城狮",
            };

            dataList.Add(item1);

            var item2 = new item
            {
                name = "陈老师",
                message = "香港大学博士",
            };

            dataList.Add(item2);

            var item3 = new item
            {
                name = "俞乃胜",
                message = "IT界的小学生",
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
