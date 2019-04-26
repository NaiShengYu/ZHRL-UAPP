using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.iOS.Services;
using AepApp.Services;
using Foundation;
using UIKit;
using JPush.Binding.iOS;
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

                if (!string.IsNullOrWhiteSpace(userid))
                {
                    string alias = userid.Replace("-", "");
                    string[] tags = App.FrameworkURL.Split(":");
                    string tag = "";
                    if (tags.Count() >1)
                    {
                        tag = tags[1];
                        tag = tag.Replace("//", "");
                        tag = tags[0] + tag;
                    }
                    Console.WriteLine(" ios SetAlias userid = " + userid);
                    NSSet<NSString> nSSet = new NSSet<NSString>(new NSString[] { (NSString)tag});
                    JPUSHService.SetTags(nSSet, (arg0, arg1, arg2) => { }, 1);
                    JPUSHService.SetAlias(alias, (arg0, arg1, arg2) => { }, 1);
                }
                //JPUSHService.DeleteAlias((arg0, arg1, arg2) => { }, 1);

                //todo
            }
        }
    }
}