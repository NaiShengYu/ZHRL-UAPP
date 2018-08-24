using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using OxyPlot;
using Xamarin.Forms;
using CoreGraphics;
using AepApp.Models;

namespace AepApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        void HandleAction(NSNotification obj)
        {
            var dic = obj.UserInfo as NSMutableDictionary;
            var rc = dic.ValueForKey((Foundation.NSString)"UIKeyboardFrameEndUserInfoKey");
            CGRect r = (rc as NSValue).CGRectValue;
            KeyboardSizeModel keyboardSizeModel = new KeyboardSizeModel
            {
                X = Convert.ToInt32(r.X),
                Y = Convert.ToInt32(r.Y),
                Wight = Convert.ToInt32(r.Size.Width),
                Height = Convert.ToInt32(r.Size.Height),
            };
            MessagingCenter.Send<ContentPage,KeyboardSizeModel>(new ContentPage(), "keyBoardFrameChanged",keyboardSizeModel);


        }


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            
            var not = NSNotificationCenter.DefaultCenter;
            not.AddObserver(UIKeyboard.WillChangeFrameNotification, HandleAction);

            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            //视频录制
            Rox.VideoIos.Init();
            //数据库初始化
            SQLitePCL.Batteries.Init();
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
