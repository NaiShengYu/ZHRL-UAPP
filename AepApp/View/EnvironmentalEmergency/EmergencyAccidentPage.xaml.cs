using AepApp.Models;
using CloudWTO.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAccidentPage : ContentPage
    {
        private int start = 0;
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;
            Navigation.PushAsync(new EmergencyAccidentInfoPage(item.title));
            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();
        private object result;

        public EmergencyAccidentPage()
        {
            InitializeComponent();
            reqEmergencyAccidentInfo(start);
            var item1 = new item
            {
                title = "长江路事故",
                time = "2018-05-25 11:13",
                type = "1",
                state = "1",

            };

            dataList.Add(item1);

            var item2 = new item
            {
                title = "氨气污染事故",
                time = "2018-05-25 11:13",
                type = "2",
                state = "1",
            };

            dataList.Add(item2);

            var item3 = new item
            {
                title = "郊区土地污染事故",
                time = "2018-05-25 11:13",
                type = "3",
                state = "1",
            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;

        }

        private void reqEmergencyAccidentInfo(int start)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                result = EasyWebRequest.sendGetHttpWebRequestWithToken(App.BaseUrlForYINGJI + DetailUrl.GetEmergencyAccidentList
                    + "?MaxResultCount=" + (start + 10) + "&SkipCount=" + start + "&Filter=" + "" + "&Sorting=" + "", App.convertToken );
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                start = start + 10;
                Console.WriteLine(result);
            };
            wrk.RunWorkerAsync();
        }

        internal class item
        {
            public string title { get; set; }
            public string time { set; get; }
            public string type { set; get; }
            public string state { set; get; }
        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            reqEmergencyAccidentInfo(start);
        }
    }
}
