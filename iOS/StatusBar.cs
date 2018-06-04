using AepApp.Interface;
using AepApp.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StatusBar))]

namespace AepApp.iOS
{
    class StatusBar : IStatusBar
    {
        public int GetHeight()
        {
            return (int)UIApplication.SharedApplication.StatusBarFrame.Height;
        }
    }

}
