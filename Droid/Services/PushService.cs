﻿using AepApp.Droid.Services;
using AepApp.Services;
using Android.App;
using CN.Jpush.Android.Api;

[assembly: Xamarin.Forms.Dependency(typeof(PushService))]
namespace AepApp.Droid.Services
{
    public class PushService : IPushService
    {
        public static string TAG = "JIGUANG-android";
        public void SetAlias(string userid)
        {
            if (!string.IsNullOrWhiteSpace(userid))
            {
                userid = userid.Replace("-", "");
                ThirdLogger.I(TAG, "SetAlias userid = " + userid);
                JPushInterface.SetAlias(Application.Context, 0, userid);
            }
            else
            {
                ThirdLogger.I(TAG, "SetAlias userid = " + userid);
                JPushInterface.SetAlias(Application.Context, 0, "");
                JPushInterface.DeleteAlias(Application.Context, 0);
            }
        }
    }
}