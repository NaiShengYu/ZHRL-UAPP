using System;
using System.Collections.Generic;
using TouchTracking;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class WindSpeedAndDirectionPage : ContentPage
    {
        void Handle_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(directionLab.Text)) directionLab.Text = "0";

            var windAngle = Convert.ToDouble(directionLab.Text) % 360;
            directionLab.Text = windAngle.ToString("f2");
            zhizhen.RotateTo(windAngle, 0);

        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            

            if (string.IsNullOrWhiteSpace(speedLab.Text)) return;
            if (string.IsNullOrWhiteSpace(directionLab.Text)) return;
            string[] aa = new string[2]{
                speedLab.Text,
                directionLab.Text,
            };
            MessagingCenter.Send<ContentPage, string[]>(this, "saveWindSpeedAndDirection",aa);
            Navigation.PopAsync();        
        }

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
            winLab.Text = WindDirectionWithfloat(Math.Round(tand, 2));
            zhizhen.RotateTo(tand, 0);
        }
        string WindDirectionWithfloat(double tand)
        {
            if (tand <= 22.5) return " 北风";
            else if (tand <= 67.5)  return " 东北风";
            else if (tand <= 112.5) return " 东风";
            else if (tand <= 157.5) return " 东南风";
            else if (tand <= 202.5) return " 南风";
            else if (tand <= 247.5) return " 西南风";
            else if (tand <= 292.5) return " 西风";
            else if (tand <= 337.5) return " 西北风";
            else return " 北风";
        }

        //#pragma mark --取消masterDeftail的返回手势
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.appHunbegerPage.IsGestureEnabled = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.appHunbegerPage.IsGestureEnabled = true;
        }

    }
}
