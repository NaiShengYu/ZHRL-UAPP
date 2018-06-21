
using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Essentials;
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AccidentPositionPage : ContentPage
    {
        static int i = 0;

        async void HandleEventHandler()
        {

            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();
                App.currentLocation = location;
                if (location != null)
                {
                    if (i >0){
                        map.SetCenter(12, new AzmCoord(location.Longitude, location.Latitude));
                    }
                    var img = ImageSource.FromFile("markerred.png");
                    var aaa = new AzmMarkerView(img, new Size(30, 30),new AzmCoord(location.Longitude, location.Latitude));
                    map.Overlays.Add(aaa);
                    i += 1;
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }
            }
          
            catch (Exception ex)
            {
                // Unable to get location
            }
        }


        //保存此位置
        void savePosition(object sender, System.EventArgs e)
        {
            Console.WriteLine(centercoorLab.Text);

            MessagingCenter.Send<ContentPage, string>(this,"savePosition", centercoorLab.Text);

            Navigation.PopAsync();

        }

        //回到当前位置
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            HandleEventHandler();
        }

        void Handle_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            //虚线间隔
            float[] aa = new float[2];
            for (int i = 0; i < 2; i++)
            {
                aa[i] = 10;
            }
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Gray.ToSKColor(),
                StrokeWidth = 3,
                //StrokeCap = (SKStrokeCap)Enum.Parse(typeof(SKStrokeCap),@"10"),
                PathEffect = SKPathEffect.CreateDash(aa, 0),

            };


            SKPath path = new SKPath();
            path.MoveTo(0.5f * info.Width, 0);
            path.LineTo(0.5f * info.Width, info.Height);
            canvas.DrawPath(path, paint);

            SKPath path1 = new SKPath();
            path1.MoveTo(0, 0.5f * info.Height);
            path1.LineTo(info.Width, 0.5f * info.Height);
            canvas.DrawPath(path1, paint);
        }


        public AccidentPositionPage()
        {
            InitializeComponent();
            HandleEventHandler();
            if (App.currentLocation != null)
            {
                map.SetCenter(12, new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude));
            }
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
        }
    }
}
