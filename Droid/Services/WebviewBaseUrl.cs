using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.Droid.Services;
using AepApp.Interface;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebviewService))]
namespace AepApp.Droid.Services
{
    public class WebviewService : IWebviewService
    {
        public string Get()
        {
            return "file:///android_asset/";
        }

        public string SetEditorContent(Xamarin.Forms.WebView web, string html)
        {
            return null;
        }
    }
}