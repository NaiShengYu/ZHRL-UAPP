using System;
using System.Diagnostics;
using System.IO;
using Foundation;
using SimpleAudioForms.iOS;
using Xamarin.Forms;
using AepApp.Interface;
using AepApp.iOS.Services;
using UIKit;
using System.Collections.Generic;

[assembly: Dependency(typeof(OpenApp))]

namespace AepApp.iOS.Services
{
    public class OpenApp : IOpenApp
    {

        public OpenApp()
        {

        }

        public List<string> JudgeCanOpenAPP(){
            List<string> aaa = new List<string>();
            try{
                if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("baidumap://map/")) == true) 

                    aaa.Insert(0, "百度地图");

            }catch(Exception ex){
                
            }
            try
            {
                if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("iosamap://")) == true) aaa.Insert(0, "高德地图");

            }
            catch (Exception ex)
            {

            }

            aaa.Add("苹果地图");

            return aaa;
        }
    }
}
