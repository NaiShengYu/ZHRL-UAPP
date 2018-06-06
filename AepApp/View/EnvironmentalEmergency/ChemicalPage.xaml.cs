using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var ChemicalModel = e.SelectedItem as ChemicalModel;
            if (ChemicalModel == null)
                return;
            if (_type == 1)
                Navigation.PushAsync(new ChemicalInfoPage());

            if (_type == 2)
            {
                MessagingCenter.Send<ContentPage, ChemicalModel>(this, "Value", ChemicalModel);
                Navigation.PopAsync();

            }

            listView.SelectedItem = null; 
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }


        ObservableCollection<ChemicalModel> dataList = new ObservableCollection<ChemicalModel>();
        int _type = 0;
        public ChemicalPage(int type)
        {
            InitializeComponent();
            _type = type;
            NavigationPage.SetBackButtonTitle(this,"");//去掉返回键文字


            var ChemicalModel1 = new ChemicalModel
            {
                name = "铁",
                Yname = "Fe",
                type = "CAS编号",
                TypeNum = "2321",

            };

            dataList.Add(ChemicalModel1);

            var ChemicalModel2 = new ChemicalModel
            {
                name = "钙",
                Yname = "Ca",
                type = "CAS编号",
                TypeNum = "2321",

            };

            dataList.Add(ChemicalModel2);

            var ChemicalModel3 = new ChemicalModel
            {
                name = "钠",
                Yname = "Na",
                type = "CAS编号",
                TypeNum = "2321",

            };

            dataList.Add(ChemicalModel3);

            listView.ItemsSource = dataList;
        }

        public ChemicalPage() : this(1)
        {

        }
    }
}
