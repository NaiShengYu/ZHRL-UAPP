﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AepApp.Droid.Renderers;
using AepApp.MaterialForms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebViewer), typeof(WebViewRender))]
namespace AepApp.Droid.Renderers
{
    public class WebViewRender : WebViewRenderer
    {

        public WebViewRender(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            var webView = e.NewElement as WebViewer;
            if (webView != null)
                webView.EvaluateJavascript = async (js) =>
                {
                    var reset = new ManualResetEvent(false);
                    var response = string.Empty;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Control?.EvaluateJavascript(js, new JavascriptCallback((r) => { response = r; reset.Set(); }));
                    });
                    await Task.Run(() => { reset.WaitOne(); });
                    return response;
                };
        }

        internal class JavascriptCallback : Java.Lang.Object, IValueCallback
        {
            public JavascriptCallback(Action<string> callback)
            {
                _callback = callback;
            }

            private Action<string> _callback;
            public void OnReceiveValue(Java.Lang.Object value)
            {
                string json = ((Java.Lang.String)value).ToString();
                string res = JsonConvert.DeserializeObject<string>(json);
                _callback?.Invoke(res);
            }
        }
    }
}