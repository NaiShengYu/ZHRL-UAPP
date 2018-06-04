using System;
using System.Collections.Generic;

using Xamarin.Forms;
using AepApp.View.EnvironmentalEmergency;
using AepApp.View.SecondaryFunction;
using Xamarin.Forms.PlatformConfiguration;

namespace AepApp.View
{
    public partial class MasterPage : ContentPage
    {
        public MasterPage(MasterAndDetailPage aaaa)
        {
            InitializeComponent();
            Label lastLab = lab6;
            lastLab.BackgroundColor = Color.White;
            //应急事故
            var nav6 = new NavigationPage((Page)Activator.CreateInstance(typeof(EmergencyAccidentPage)));
            //成功案例
            var nav7 = new NavigationPage((Page)Activator.CreateInstance(typeof(SuccessCase)));
            //应急预案
            var nav8 = new NavigationPage((Page)Activator.CreateInstance(typeof(EmergencyPlan)));
            //敏感源
            var nav9 = new NavigationPage((Page)Activator.CreateInstance(typeof(SensitiveSourcePage)));
            //救援地点
            var nav10 = new NavigationPage((Page)Activator.CreateInstance(typeof(RescueSitePage)));
            //专家车
            var nav11 = new NavigationPage((Page)Activator.CreateInstance(typeof(ExpertLibraryPage)));
            //设备
            var nav12 = new NavigationPage((Page)Activator.CreateInstance(typeof(EquipmentPage)));
            //值班表
            var nav13 = new NavigationPage((Page)Activator.CreateInstance(typeof(WatchListPage)));
            //化学品
            var nav14 = new NavigationPage(new ChemicalPage(1));
            //二维码
            var nav17 = new NavigationPage((Page)Activator.CreateInstance(typeof(QRCodeScanner)));
            //设置
            var nav18 = new NavigationPage((Page)Activator.CreateInstance(typeof(SelectSitePage)));
          
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {

                lastLab.BackgroundColor = Color.White;

                var sender = s as Label;
                sender.BackgroundColor = Color.Gray;
                lastLab = sender;
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

                if (s == lab3)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }

                if (s == lab4)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }

                if (s == lab5)
                {
                    Console.WriteLine("空气质量");
                    //aaaa.Detail = nav2;
                }

                if (s == lab6)
                {
                    Console.WriteLine("应急事故");
                    aaaa.Detail = nav6;
                }

                if (s == lab7)
                {
                    Console.WriteLine("成功案例");
                    aaaa.Detail = nav7;
                }
                if (s == lab8)
                {
                    Console.WriteLine("应急预案");
                    aaaa.Detail = nav8;
                }
                if (s == lab9)
                {
                    Console.WriteLine("敏感源");
                    aaaa.Detail = nav9;
                }
                if (s == lab10)
                {
                    Console.WriteLine("救援地点");
                    aaaa.Detail = nav10;
                }

                if (s == lab11)
                {
                    Console.WriteLine("专家库");
                    aaaa.Detail = nav11;
                }

                if (s == lab12)
                {
                    Console.WriteLine("设备");
                    aaaa.Detail = nav12;
                }

                if (s == lab13)
                {
                    Console.WriteLine("值班表");
                    aaaa.Detail = nav13;
                }

                if (s == lab14)
                {
                    Console.WriteLine("化学品");
                    aaaa.Detail = nav14;
                }
                if (s == lab17)
                {
                    Console.WriteLine("二维码");                
                    aaaa.Detail = nav17;
                }
                if (s == lab18)
                {
                    Console.WriteLine("设置");                
                    aaaa.Detail = nav18;
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
            lab10.GestureRecognizers.Add(tapGestureRecognizer);
            lab11.GestureRecognizers.Add(tapGestureRecognizer);
            lab12.GestureRecognizers.Add(tapGestureRecognizer);
            lab13.GestureRecognizers.Add(tapGestureRecognizer);
            lab14.GestureRecognizers.Add(tapGestureRecognizer);
            lab17.GestureRecognizers.Add(tapGestureRecognizer);        
            lab18.GestureRecognizers.Add(tapGestureRecognizer);        
        }
    }
}
