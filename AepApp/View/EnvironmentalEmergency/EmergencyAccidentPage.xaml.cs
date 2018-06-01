using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAccidentPage : ContentPage
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
           
            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public EmergencyAccidentPage()
        {
            InitializeComponent();
           
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

        internal class item{
            public string title { get; set; }
            public string time { set; get; }
            public string type { set; get; }
            public string state { set; get; }




        }


    }
}
