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
using Android.Widget;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebviewBaseUrl))]
namespace AepApp.Droid.Services
{
    public class WebviewBaseUrl : IWebviewBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}