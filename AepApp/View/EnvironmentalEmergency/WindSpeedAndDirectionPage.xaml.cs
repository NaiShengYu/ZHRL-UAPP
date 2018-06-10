using System;
using System.Collections.Generic;
using TouchTracking;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class WindSpeedAndDirectionPage : ContentPage
    {
        public WindSpeedAndDirectionPage()
        {
            InitializeComponent();
            imgG.HeightRequest = App.ScreenWidth;
            imgG.WidthRequest = App.ScreenWidth;

        
        }

        void Handle_TouchAction(object sender, TouchTracking.TouchActionEventArgs args)
        {

            var x1 = args.Location.X -App.ScreenWidth / 2 ;
            var y1 =App.ScreenWidth / 2- args.Location.Y ;

            var x2 = 0;
            var y2 = 1;

            var x1y1 = ((x1 * x2) + (y1 * y2)) / (Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2)) * Math.Sqrt(Math.Pow(x2, 2) + Math.Pow(y2, 2)));

            var tanc = Math.Acos(x1y1);
            var tand = tanc / Math.PI * 180;

            if (x1 < 0) tand = 360 - tand;
            directionLab.Text = Math.Round(tand,2).ToString();
            zhizhen.RotateTo(tand, 0);
        }


        //#pragma mark --取消masterDeftail的返回手势
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (App.Current as App).IsMasterDetailPageGestureEnabled = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (App.Current as App).IsMasterDetailPageGestureEnabled = true;
        }

    }
}
