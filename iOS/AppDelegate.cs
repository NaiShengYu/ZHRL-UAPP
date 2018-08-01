using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using OxyPlot;

namespace AepApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            //视频录制
            Rox.VideoIos.Init();
            global::Xamarin.Forms.Forms.Init();
            InTheHand.Forms.Platform.iOS.InTheHandForms.Init();
            //图表
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
            //扫描
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            LoadApplication(new App());
         
            return base.FinishedLaunching(app, options);
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            NSTimer timer = NSTimer.CreateRepeatingTimer(interval, t =>
            {
                if (!callback())
                    t.Invalidate();
            });
            NSRunLoop.Main.AddTimer(timer, NSRunLoopMode.Common);
        }

    }
}
