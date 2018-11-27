
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CN.Jpush.Android.Api;
using OxyPlot;
using Plugin.CurrentActivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: UsesPermission(Android.Manifest.Permission.Flashlight)]
namespace AepApp.Droid
{
    [Activity(Label = "环境保护", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            CrossCurrentActivity.Current.Init(this, bundle);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            //百度地图配置
            //Xamarin.FormsBaiduMaps.Init(null);
            InitJPush();
            StatusBar.Activity = this;//获取状态栏高度
            LoadApplication(new App());
            //监听网络变化
            Connectivity.ConnectivityChanged += (ConnectivityChangedEventArgs e) => {
                var access = e.NetworkAccess;
                MessagingCenter.Send<Grid, NetworkAccess>(new Grid(), "NetworkChanged", access);
            };
            //Initializer.Initialize();
        }
        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }


        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            var handler = new Handler(Looper.MainLooper);
            handler.PostDelayed(() =>
            {
                if (callback())
                    StartTimer(interval, callback);

                handler.Dispose();
                handler = null;
            }, (long)interval.TotalMilliseconds);
        }

        /// <summary>
        /// init JPush
        /// </summary>
        private void InitJPush()
        {
            JPushInterface.SetDebugMode(true);
            JPushInterface.Init(Application.Context);
            JPushInterface.SetAlias(Application.Context, 0, "alias_test");

            BasicPushNotificationBuilder builder = new BasicPushNotificationBuilder(this);
            builder.StatusBarDrawable = Resource.Drawable.jpush_notification_icon;
            JPushInterface.SetPushNotificationBuilder(new Java.Lang.Integer(1), builder);
        }


        protected override void OnResume()
        {
            base.OnResume();

            JPushInterface.OnResume(this);
        }

        protected override void OnPause()
        {
            base.OnPause();

            JPushInterface.OnPause(this);
        }
    }

}
