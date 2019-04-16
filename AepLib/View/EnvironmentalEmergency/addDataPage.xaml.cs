using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using AepApp.Models;
using CloudWTO.Services;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class addDataPage : ContentPage
    {
      
        //点击某一个污染源或理化性质，添加数值
        void HandleEventHandler(object sender, EventArgs e)
        {
            var but = sender as Button;
            AddDataIncidentFactorModel.ItemsBean item = but.BindingContext as AddDataIncidentFactorModel.ItemsBean;
            ////如果id为空，无法请求到数据，就直接return，不用返回到上一级
            if (string.IsNullOrWhiteSpace(item.factorId)) return;
            Navigation.PushAsync(new LHXZInfoPage(item, 0));

        }

        void addLHXZ(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddDataForLHXZPage());
           

        }

        //添加关键污染物
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ChemicalPage(3));
           

        }

        public addDataPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字


            creatView(wrw, App.contaminantsList);
            creatView(lhxz, App.AppLHXZList);

        }



        void creatView(StackLayout la,List<AddDataIncidentFactorModel.ItemsBean> list){
            la.Children.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var it = list[i];

                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.factorName,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                lab1.VerticalOptions = LayoutOptions.CenterAndExpand;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but1 = new Button
                {
                    BindingContext = it,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                };
                but1.Clicked += HandleEventHandler;
                sk.Children.Add(but1);
                la.Children.Add(sk);
            }

        }


     
       




    }
}
