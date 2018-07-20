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
    /// <summary>
    /// Android App属性工具包
    /// </summary>
    class AppUtil
    {
        /// <summary>
        /// 检查某个应用是否安装
        /// </summary>
        /// <param name="Appname"></param>
        /// <returns></returns>
        public static bool IsInstalled(string Appname)
        {
            bool IsInstallflag = false;
            var flag = PackageInfoFlags.Activities;
            var apps = Application.Context.PackageManager.GetInstalledApplications(flag);
            if (string.IsNullOrEmpty(Appname))
            {
                return IsInstallflag;
            }
            try
            {
                foreach (var app in apps)
                {
                    var appInfo = Application.Context.PackageManager.GetApplicationInfo(app.PackageName, 0);
                    var appLabel = Application.Context.PackageManager.GetApplicationLabel(appInfo);
                    //var versionNumber = Application.Context.PackageManager.GetPackageInfo(app.PackageName, 0).VersionName;
                    if (appLabel.ToLower().Contains(Appname.ToLower()))
                    {
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