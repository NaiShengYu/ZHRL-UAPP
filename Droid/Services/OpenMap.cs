using System;
using System.Collections.Generic;
using AepApp.Droid.Services;
using AepApp.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenMap))]

namespace AepApp.Droid.Services
{
    public class OpenMap : IOpenApp
    {


        public List<string> JudgeCanOpenAPP()
        {
            List<string> maps = new List<string>();
            string[] _map_name = new string[] { "百度地图", "高德地图", "腾讯地图" };
            foreach (string name in _map_name)
            {
                if (Android.Content.PM.AppUtil.IsInstalled(name))
                {
                    maps.Add(name);
                }
            }

            return maps;
        }
    }
}
