using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class TransportSamplesPage : ContentPage
    {
        void Handle_Clicked(object sender, System.EventArgs e)
        {

            //var item = 
            //if (item == null)
            //return;
            MenuItem item = sender as MenuItem;
            CollectionAndTransportSampleModel model = item.BindingContext as CollectionAndTransportSampleModel;
            dataList.Remove(model);
        
        }

        //选中了item
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            CollectionAndTransportSampleModel model = e.SelectedItem as CollectionAndTransportSampleModel;
            if (model == null)return;
            Navigation.PushAsync(new ScanSamplePage());
            listView.SelectedItem = null;
        }

        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            
        }
        //搜索
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            
        }

        private ObservableCollection<CollectionAndTransportSampleModel> dataList = new ObservableCollection<CollectionAndTransportSampleModel>();

        public TransportSamplesPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            var model1 = new CollectionAndTransportSampleModel
            {
                time = "2018-10-24",
                num = "5 采样瓶个",
                type = "start",
            };
            var model2 = new CollectionAndTransportSampleModel
            {
                time = "2018-10-26",
                num = "6 采样瓶个",
                type = "finish",
            };
            var model3 = new CollectionAndTransportSampleModel
            {
                time = "2018-10-27",
                num = "10 采样瓶个",
                type = "finish",
            };

            dataList.Add(model1);
            dataList.Add(model2);
            dataList.Add(model3);
            listView.ItemsSource = dataList;





        }
    }
}
