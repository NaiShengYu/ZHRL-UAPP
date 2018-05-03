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
            global::Xamarin.Forms.Forms.Init();
           
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
            Xamarin.FormsBaiduMaps.Init("20Nb1nC8Zsj0achB9mAZ4PBG8YkYbpXU");
            LoadApplication(new App());


            return base.FinishedLaunching(app, options);
        }
    }
}
