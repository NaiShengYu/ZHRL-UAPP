using AepApp.Droid.Services;
using AepApp.Services;
using Android.App;
using CN.Jpush.Android.Api;
using System.Collections.Generic;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(PushService))]
namespace AepApp.Droid.Services
{
    public class PushService : IPushService
    {
        public static string TAG = "JIGUANG-android";

        public void SetAlias(string userid)
        {
            //设置tag
            string siteUrl = string.IsNullOrWhiteSpace(App.FrameworkURL) ? "" : App.FrameworkURL;
            if (!string.IsNullOrWhiteSpace(siteUrl))
            {
                string[] urls = siteUrl.Split(':');
                string tag = "";
                if (urls != null && urls.Count() > 1)
                {
                    tag = urls[1];
                    tag = tag.Replace("//", "");
                    tag = urls[0] + tag;
                }
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    List<string> _tags = new List<string>();
                    _tags.Add(tag.ToLower());
                    JPushInterface.SetTags(Application.Context, 0, _tags);
                }
            }


            if (!string.IsNullOrWhiteSpace(userid))
            {
                userid = userid.Replace("-", "").ToLower();
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