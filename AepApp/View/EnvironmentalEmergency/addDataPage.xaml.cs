using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class addDataPage : ContentPage
    {
        //添加关键污染物
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ChemicalPage(2));
            MessagingCenter.Subscribe<ContentPage, ChemicalModel>(this, "Value", (arg1, arg2) =>
            {
                dataList1.Add(arg2);
                creatWRWView();
            });

        }
        ObservableCollection<ChemicalModel> dataList1 = new ObservableCollection<ChemicalModel>();
        ObservableCollection<ChemicalModel> dataList2 = new ObservableCollection<ChemicalModel>();

        public addDataPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            var ChemicalModel1 = new ChemicalModel
            {
                name = "苯",
                Yname = "BenZene",
            };

            var ChemicalModel2 = new ChemicalModel
            {
                name = "二甲苯",
                Yname = "BenZene",
            };
            dataList1.Add(ChemicalModel1);
            dataList1.Add(ChemicalModel2);
          
            var ChemicalModel12 = new ChemicalModel
            {
                name = "化学需氧量",
               
            };

            var ChemicalModel22 = new ChemicalModel
            {
                name = "氨氮",
            };
            dataList2.Add(ChemicalModel12);
            dataList2.Add(ChemicalModel22);


            creatWRWView();
            creatLHXZView();
           


        }

        void creatWRWView(){
            wrw.Children.Clear();
            for (int i = 0; i < dataList1.Count; i++)
            {

                var it = dataList1[i];
                var sk = new StackLayout
                {
                    Spacing = 1,
                    BackgroundColor = Color.White,
                };
                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 0),
                    Text = it.name,

                };
                var lab2 = new Label
                {
                    Margin = new Thickness(15, 0, 15, 5),
                    Text = it.Yname,
                    TextColor = Color.Gray,
                    FontSize = 15,

                };
                sk.Children.Add(lab1);
                sk.Children.Add(lab2);
                wrw.Children.Add(sk);
            }

        }

        void creatLHXZView(){

            lhxz.Children.Clear();
            for (int i = 0; i < dataList2.Count; i++)
            {
                var it = dataList2[i];
                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.name,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                lhxz.Children.Add(lab1);
            }

        }
      

    }
}
