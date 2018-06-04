using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class LHXZInfoPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            
            var item = e.SelectedItem as LHXZAddressMode;
            if (item == null)
                return;

            foreach(LHXZAddressMode aaa in dataList1){
                if(aaa.isCurrent ==true){
                    aaa.isCurrent = false;
                    break;
                }
            }

            item.isCurrent = true;


            listView.SelectedItem = null;
        
        
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();

        }

        ObservableCollection<LHXZAddressMode> dataList1 = new ObservableCollection<LHXZAddressMode>();

        public LHXZInfoPage()
        {
            InitializeComponent();

            var item1 = new LHXZAddressMode
            {
                Name = "当前位置",
                SiteAddr = "123.2345 E, 29.34322 N",
                isCurrent = false,
            };

            var item2 = new LHXZAddressMode
            {
                Name = "文化广场",
                SiteAddr = "123.2345 E, 29.34322 N",
                isCurrent = false,
            };

            var item3 = new LHXZAddressMode
            {
                Name = "国际金融中心",
                SiteAddr = "123.2345 E, 29.34322 N",
                isCurrent = true,
            };

            dataList1.Add(item1);
            dataList1.Add(item2);
            dataList1.Add(item3);
            listView.ItemsSource = dataList1;
        }

    }
}
