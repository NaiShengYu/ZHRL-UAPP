using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class SampleTypePage : ContentPage
    {

       

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var SampleTypeModel = e.SelectedItem as SampleTypeModel;
            if (SampleTypeModel == null)
                return;
            SampleTypeModel.isSelect = true;
            MessagingCenter.Send<ContentPage, string>(this, "SampleType", SampleTypeModel.name);
            Navigation.PopAsync();

            listView.SelectedItem = null;


        }

        ObservableCollection<SampleTypeModel> dataList1 = new ObservableCollection<SampleTypeModel>();

        public SampleTypePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            listView.ItemsSource = App.sampleTypeList;
        }
      
    }
}
