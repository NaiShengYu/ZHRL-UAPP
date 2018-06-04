using AepApp.Droid;
using AepApp.Interface;
using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
[assembly: Dependency(typeof(StatusBar))]
namespace AepApp.Droid
{
    public class StatusBar : IStatusBar
    {
        public static Activity Activity { get; set; }
        public int GetHeight()
        {
            int statusBarHeight = -1;
            int resourceId = Activity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                statusBarHeight = Activity.Resources.GetDimensionPixelSize(resourceId);
            }
            return statusBarHeight;
        }

    }
}