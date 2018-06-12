using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class DetectionValueUnitPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;
            MessagingCenter.Send<ContentPage, string>(this, "DetectionValueUnit", item.name);
            Navigation.PopAsync();
            listView.SelectedItem = null;

        }
         ObservableCollection<item> dataList1 = new ObservableCollection<item>();

        public DetectionValueUnitPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            var item1 = new item { name = "mg/m³"};
            dataList1.Add(item1);

            var item2 = new item { name = "mg/L" };
            dataList1.Add(item2);

            var item3 = new item { name = "μg/m³" };
            dataList1.Add(item3);

            var item4 = new item { name = "ppbV" };
            dataList1.Add(item4);

            var item5 = new item { name = "ppmV" };
            dataList1.Add(item5);

            var item6 = new item { name = "hPa" };
            dataList1.Add(item6);

            var item7 = new item { name = "kPa" };
            dataList1.Add(item7);

            var item8 = new item { name = "MPa" };
            dataList1.Add(item8);

            var item9 = new item { name = "cm" };
            dataList1.Add(item9);

            var item10 = new item { name = "m" };
            dataList1.Add(item10);

            var item11 = new item { name = "°C" };
            dataList1.Add(item11);

            var item12 = new item { name = "°F" };
            dataList1.Add(item12);

            var item13 = new item { name = "K" };
            dataList1.Add(item13);
            var item14 = new item { name = "%RH" };
            dataList1.Add(item14);
            var item15 = new item { name = "m³/h" };
            dataList1.Add(item15);
            var item16 = new item { name = "m³/s" };
            dataList1.Add(item16);
            var item17 = new item { name = "dB" };
            dataList1.Add(item17);
            var item18 = new item { name = "Pa" };
            dataList1.Add(item18);
            var item19 = new item { name = "%" };
            dataList1.Add(item19);
            var item20 = new item { name = "°" };
            dataList1.Add(item20);
            var item21 = new item { name = "1/mm" };
            dataList1.Add(item21);
            var item22 = new item { name = "m/s" };
            dataList1.Add(item22);
            var item23 = new item { name = "ms/m" };
            dataList1.Add(item23);
            var item24 = new item { name = "t(km2.30d)" };
            dataList1.Add(item24);
            var item25 = new item { name = "个/L" };
            dataList1.Add(item25);
            var item26 = new item { name = "无量纲" };
            dataList1.Add(item26);
            listView.ItemsSource = dataList1;

        }

        internal class item
        {
            public string name { get; set; }
            public string address { set; get; }

        }


    }
}
