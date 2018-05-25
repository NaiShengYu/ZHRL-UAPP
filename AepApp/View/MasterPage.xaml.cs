using System;
using System.Collections.Generic;

using Xamarin.Forms;
using AepApp.View.EnvironmentalEmergency;
namespace AepApp.View
{
    public partial class MasterPage : ContentPage
    {
        public MasterPage(MasterAndDetailPage aaaa)
        {
            InitializeComponent();

            //应急事故
            var nav6 = new NavigationPage((Page)Activator.CreateInstance(typeof(EmergencyAccidentPage)));
            //var nav2 = new NavigationPage((Page)Activator.CreateInstance(typeof(detailPage2)));
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                if (s == lab1)
                {
                    Console.WriteLine("采样任务");
                    //aaaa.Detail = nav1;

                }
                if (s == lab2)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }
                aaaa.IsPresented = false;

                if (s == lab3)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }
                aaaa.IsPresented = false;

                if (s == lab4)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }
                aaaa.IsPresented = false;

                if (s == lab5)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }
                aaaa.IsPresented = false;

                if (s == lab6)
                {
                    Console.WriteLine("应急事故");
                    aaaa.Detail = nav6;
                }
                aaaa.IsPresented = false;


            };
            lab1.GestureRecognizers.Add(tapGestureRecognizer);
            lab2.GestureRecognizers.Add(tapGestureRecognizer);
            lab3.GestureRecognizers.Add(tapGestureRecognizer);
            lab4.GestureRecognizers.Add(tapGestureRecognizer);
            lab5.GestureRecognizers.Add(tapGestureRecognizer);
            lab6.GestureRecognizers.Add(tapGestureRecognizer);
            lab7.GestureRecognizers.Add(tapGestureRecognizer);
            lab8.GestureRecognizers.Add(tapGestureRecognizer);
            lab9.GestureRecognizers.Add(tapGestureRecognizer);


        }
    }
}
