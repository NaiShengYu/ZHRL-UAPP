﻿using System;
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
            Rox.VideoIos.Init();
            global::Xamarin.Forms.Forms.Init();
           
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
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
