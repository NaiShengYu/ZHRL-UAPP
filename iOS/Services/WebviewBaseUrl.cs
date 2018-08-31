using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.Interface;
using AepApp.iOS.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebviewService))]
namespace AepApp.iOS.Services
{
    public class WebviewService : IWebviewService
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }

        public string SetEditorContent(WebView web, string html)
        {
            return null;
        }
    }
}