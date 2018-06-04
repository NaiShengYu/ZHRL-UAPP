
using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AccidentPositionPage : ContentPage
    {


        //保存此位置
        void savePosition(object sender, System.EventArgs e)
        {

        }

        //回到当前位置
        void Handle_Clicked(object sender, System.EventArgs e)
        {

            var aaa = sender as Entry;

            map.SetCenter(16, new AzmCoord(110, 100));
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
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字




        }
    }
}
