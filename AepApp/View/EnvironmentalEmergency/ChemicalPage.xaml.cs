using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalPage : ContentPage
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
            Navigation.PushAsync(new ChemicalInfoPage());
            listView.SelectedItem = null;
        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public ChemicalPage()
        {
            InitializeComponent();

            var item1 = new item
            {
                name = "铁",
                Yname = "Fe",
                type = "CAS编号",
                TypeNum = "2321",

            };

            dataList.Add(item1);

            var item2 = new item
            {
                name = "钙",
                Yname = "Ca",
                type = "CAS编号",
                TypeNum = "2321",

            };

            dataList.Add(item2);

            var item3 = new item
            {
                name = "钠",
                Yname = "Na",
                type = "CAS编号",
                TypeNum = "2321",

            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;
        }

        internal class item
        {
            public string name { get; set; }
            public string Yname { set; get; }
            public string type { set; get; }
            public string TypeNum { set; get; }
        }

    }
}
