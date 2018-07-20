using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Android.Content.PM
{
    class AppUtil
    {
        public static bool IsInstalled(string Appname)
        {
            bool IsInstallflag = false;
            var flag = PackageInfoFlags.Activities;
            var apps = Application.Context.PackageManager.GetInstalledApplications(flag);
            try
            {
                foreach (var app in apps)
                {
                    var appInfo = Application.Context.PackageManager.GetApplicationInfo(app.PackageName, 0);
                    var appLabel = Application.Context.PackageManager.GetApplicationLabel(appInfo);
                    var versionNumber = Application.Context.PackageManager.GetPackageInfo(app.PackageName, 0).VersionName;
                    if (appLabel.ToLower().Contains(Appname.ToLower()))
                    {
                        //var builder = new alertdialog.builder(this);
                        //builder.settitle("found it!");
                        //builder.setmessage(applabel + " installed at: " + app.sourcedir);
                        //builder.show();
                        IsInstallflag = true;
                        break;
                    }
                }
                return IsInstallflag;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                return IsInstallflag;
            }
        }
    }
}