using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using OxyPlot;
using Xamarin.Forms;
using CoreGraphics;
using AepApp.Models;
//using AepApp.iOS.Notification.JPush;
using UserNotifications;
using Xamarin.Essentials;
//using JPush.Binding.iOS;

namespace AepApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //JPushInterface jPushRegister { get; set; }

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


        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);
            uiApplication.ApplicationIconBadgeNumber = new nint(0);
         }

        public override void ProtectedDataDidBecomeAvailable(UIApplication application)
        {
            base.ProtectedDataDidBecomeAvailable(application);

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
            //Popup初始化
            Rg.Plugins.Popup.Popup.Init();
            Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();

            InTheHand.Forms.Platform.iOS.InTheHandForms.Init();
            //图表
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
            //扫描
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            //collectionView横向
            AiForms.Renderers.iOS.CollectionViewInit.Init(); 

            LoadApplication(new App());

            Connectivity.ConnectivityChanged += (ConnectivityChangedEventArgs e) => {
                var access = e.NetworkAccess;
                MessagingCenter.Send<Grid, NetworkAccess>(new Grid(), "NetworkChanged", access);
            };

            ////注册apns远程推送
            //if (options == null) options = new NSDictionary();
            //jPushRegister = new JPushInterface();
            //jPushRegister.Register(this, options);
            //this.RegistLogin(options);

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

        /// <summary>
        /// 注册apns远程推送
        /// </summary>
        /// <param name="launchOptions"></param>
        protected void RegistLogin(NSDictionary launchOptions)
        {
            string systemVersion = UIDevice.CurrentDevice.SystemVersion.Split('.')[0];
            Console.WriteLine("System Version: " + UIDevice.CurrentDevice.SystemVersion);

            //iOS10以上的注册方式
            if (float.Parse(systemVersion) >= 10.0)
            {
                UNUserNotificationCenter center = UNUserNotificationCenter.Current;
                center.RequestAuthorization((UNAuthorizationOptions.CarPlay | UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge), (bool arg1, NSError arg2) =>
                {
                    if (arg1)
                        Console.WriteLine("ios 10 request notification success");
                    else
                        Console.WriteLine("IOS 10 request notification failed");
                });
            }
            //iOS8以上的注册方式
            else if (float.Parse(systemVersion) >= 8.0)
            {
                UIUserNotificationSettings notiSettings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Badge | UIUserNotificationType.Sound | UIUserNotificationType.Alert, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(notiSettings);
            }
            //iOS8以下的注册方式，这里我们最低版本是7.0以上
            else
            {
                UIRemoteNotificationType myTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Sound | UIRemoteNotificationType.Badge;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(myTypes);
            }
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            if (launchOptions != null)
            {
                NSDictionary remoteNotification = (NSDictionary)(launchOptions.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey));
                if (remoteNotification != null)
                {
                    Console.WriteLine(remoteNotification);
                    //这里是跳转页面用的
                    //this.goToMessageViewControllerWith(remoteNotification);
                }
            }
        }


        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            application.RegisterForRemoteNotifications();
        }

        /// <summary>
        /// 注册成功获得token
        /// </summary>
        /// <param name="application"></param>
        /// <param name="deviceToken"></param>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //JPUSHService.RegisterDeviceToken(deviceToken);

            // Get current device token
            var DeviceToken = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(DeviceToken))
            {
                DeviceToken = DeviceToken.Trim('<').Trim('>');
            }

            // Get previous device token
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
            {
                //TODO: Put your own logic here to notify your server that the device token has changed/been created!
            }

            // Save new device token 
            NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");

            System.Console.WriteLine(DeviceToken);

        }

        /// <summary>
        /// 注册token失败
        /// </summary>
        /// <param name="application"></param>
        /// <param name="error"></param>
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            new UIAlertView("注册通知失败", error.LocalizedDescription, null, "OK", null).Show();
        }

        public void GetRegistrationID(int resCode, NSString registrationID)
        {
            if (resCode == 0)
            {
                Console.WriteLine("RegistrationID Successed: {0}", registrationID);

                //App1.ViewModel.UserCenterPageViewModel.Instance.RegistId = registrationID;
            }
            else
                Console.WriteLine("RegistrationID Failed. ResultCode:{0}", resCode);
        }



    }
}
