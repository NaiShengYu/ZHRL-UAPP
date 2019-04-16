﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.iOS.Services;
using AepApp.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PushService))]
namespace AepApp.iOS.Services
{
    public class PushService : IPushService
    {
        public static string TAG = "JIGUANG-ios";
        public void SetAlias(string userid)
        {
            if (!string.IsNullOrWhiteSpace(userid))
            {
                Console.WriteLine(" ios SetAlias userid = " + userid);
                //todo
            }
        }
    }
}