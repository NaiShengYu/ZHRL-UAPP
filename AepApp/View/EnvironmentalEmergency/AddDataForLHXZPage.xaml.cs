using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddDataForLHXZPage : ContentPage
    {
        void HandleEventHandler(object sender, EventArgs e)
        {
            var but = sender as Button;
            var aaa = but.BindingContext as LHXZModel;
            MessagingCenter.Send<ContentPage, LHXZModel>(this, "addLHXZ", aaa);
            Navigation.PushAsync(new LHXZInfoPage());
            Navigation.RemovePage(this);

        }


        ObservableCollection<LHXZModel> dataList1 = new ObservableCollection<LHXZModel>();
        ObservableCollection<LHXZModel> dataList2 = new ObservableCollection<LHXZModel>();
        ObservableCollection<LHXZModel> dataList3 = new ObservableCollection<LHXZModel>();

        public AddDataForLHXZPage()
        {
            InitializeComponent();

            var itme1 = new LHXZModel
            {
                name = "PM2.5",
            };
            var itme2 = new LHXZModel
            {
                name = "PM10",
            };
            dataList1.Add(itme1);
            dataList1.Add(itme2);

            var itme3 = new LHXZModel
            {
                name = "化学需氧量",
            };
            var itme4 = new LHXZModel
            {
                name = "氨氮",
            };
            var itme5 = new LHXZModel
            {
                name = "酸碱度",
            };
            dataList2.Add(itme3);
            dataList2.Add(itme4);
            dataList2.Add(itme5);


            var itme6 = new LHXZModel
            {
                name = "氨氮",
            };
            var itme7 = new LHXZModel
            {
                name = "酸碱度",
            };
            dataList3.Add(itme6);
            dataList3.Add(itme7);


            creatDQ();
            creatSZ();
            creatTR();
        }



        void creatDQ()
        {
            for (int i = 0; i < dataList1.Count; i++)
            {
                var it = dataList1[i];
                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,
                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.name,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but = new Button
                {
                    BackgroundColor = Color.Transparent,
                };
                but.BindingContext = it;
                but.Clicked += HandleEventHandler;
                but.HorizontalOptions = LayoutOptions.FillAndExpand;
                but.VerticalOptions = LayoutOptions.FillAndExpand;
                sk.Children.Add(but);
                DQ.Children.Add(sk);
            }

        }

        void creatSZ()
        {
            for (int i = 0; i < dataList2.Count; i++)
            {
                var it = dataList2[i];
                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,

                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.name,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but = new Button
                {
                    BackgroundColor = Color.Transparent,
                };
                but.BindingContext = it;
                but.Clicked += HandleEventHandler;
                but.HorizontalOptions = LayoutOptions.FillAndExpand;
                but.VerticalOptions = LayoutOptions.FillAndExpand;
                sk.Children.Add(but);
                SZ.Children.Add(sk);
            }

        }

        void creatTR()
        {
            for (int i = 0; i < dataList3.Count; i++)
            {
                var it = dataList3[i];

                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,

                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.name,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but = new Button
                {
                    BackgroundColor = Color.Transparent,
                };
                but.BindingContext = it;
                but.Clicked += HandleEventHandler;
                but.HorizontalOptions = LayoutOptions.FillAndExpand;
                but.VerticalOptions = LayoutOptions.FillAndExpand;
                sk.Children.Add(but);
                TR.Children.Add(sk);
            }
        }
    }
}
