using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.Interface;
using AepApp.iOS.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebviewBaseUrl))]
namespace AepApp.iOS.Services
{
    public class WebviewBaseUrl : IWebviewBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}