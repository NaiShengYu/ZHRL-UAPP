﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UserNotifications;
using AepApp.iOS;
using JPush.Binding.iOS;
using UIKit;
using Xamarin.Forms;
using AepApp.Models;

namespace AepApp.iOS.Notification.JPush
{
    public class JPushInterface : JPUSHRegisterDelegate
    {//ab4f6d9522395eb71a74c0cc
        internal static string JPushAppKey = "47ddd95d443bebfbce7859a4";
        internal static string Channel = "";
        JPushRegisterEntity entity { get; set; }
        public void Register(AppDelegate app, NSDictionary options)
        {
            //注册apns远程推送
            string advertisingId = AdSupport.ASIdentifierManager.SharedManager.AdvertisingIdentifier.AsString();
            this.entity = new JPushRegisterEntity();
            this.entity.Types = 1 | 2 | 3;//entity.Types = (nint)(JPAuthorizationOptions.Alert) | JPAuthorizationOptions.Badge | JPAuthorizationOptions.Sound;
            JPUSHService.RegisterForRemoteNotificationConfig(entity, this);
            JPUSHService.SetupWithOption(options, JPushAppKey, Channel, true, advertisingId);
            JPUSHService.RegistrationIDCompletionHandler(app.GetRegistrationID);
           
            //JPUSHService.SetAlias("", (arg0, arg1, arg2) => { }, 1);

        }

        /// <summary>
        /// 前台收到通知,IOS10 support
        /// </summary>
        /// <param name="center"></param>
        /// <param name="notification"></param>
        /// <param name="completionHandler"></param>
        public override void WillPresentNotification(UserNotifications.UNUserNotificationCenter center, UserNotifications.UNNotification notification, Action<nint> completionHandler)
        {
            Console.WriteLine("WillPresentNotification:");
            var content = notification.Request.Content;
            var userInfo = notification.Request.Content.UserInfo;
            if (typeof(UserNotifications.UNPushNotificationTrigger) == notification.Request.Trigger.GetType())
            {//远程通知
                System.Console.WriteLine(" 前台收到远程通知,Title:{0} -SubTitle:{1}, -Body:{2}", content.Title, content.Subtitle, content.Body);
                this.AddNotificationToView(content);
                //UIApplication.SharedApplication.ApplicationIconBadgeNumber =new nint(0);
                JPUSHService.HandleRemoteNotification(userInfo);
            }
            else
            {//本地通知

            }

            if (completionHandler != null)
            {
                completionHandler(4|2);//UNNotificationPresentationOptions： None = 0,Badge = 1,Sound = 2,Alert = 4,
            }
        }

        /// <summary>
        /// 后台收到通知
        /// </summary>
        /// <param name="center"></param>
        /// <param name="response"></param>
        /// <param name="completionHandler"></param>
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            Console.WriteLine("DidReceiveNotificationResponse:");
            var content = response.Notification.Request.Content;
            var userInfo = response.Notification.Request.Content.UserInfo;
            if (typeof(UserNotifications.UNPushNotificationTrigger) == response.Notification.Request.Trigger.GetType())
            {//远程通知
                System.Console.WriteLine("后台收到远程通知,Title:{0} -SubTitle:{1}, -Body:{2}", content.Title, content.Subtitle, content.Body);
                this.AddNotificationToView(content);
                NSString from = (NSString)"from";
                PushResultModel pushResultModel = new PushResultModel();
                if (userInfo.ObjectForKey(from) !=null && userInfo.ObjectForKey(from).ToString() == "1")
                {
                    pushResultModel.type = userInfo.ObjectForKey((NSString)"type").ToString();
                    pushResultModel.from = userInfo.ObjectForKey((NSString)"from").ToString();
                    pushResultModel.factorid = userInfo.ObjectForKey((NSString)"factorid").ToString();
                    pushResultModel.factorname = userInfo.ObjectForKey((NSString)"factorname").ToString();
                    pushResultModel.locid = userInfo.ObjectForKey((NSString)"locid").ToString();
                    pushResultModel.locname = userInfo.ObjectForKey((NSString)"locname").ToString();
                  
                    MessagingCenter.Send<ContentPage, PushResultModel>(new ContentPage(), "PushResult", pushResultModel);

                }

                UIApplication.SharedApplication.ApplicationIconBadgeNumber = new nint(UIApplication.SharedApplication.ApplicationIconBadgeNumber-1);
                JPUSHService.HandleRemoteNotification(userInfo);
            }
            else
            {//本地通知

            }

            if (completionHandler != null)
            {
                completionHandler();
            }
        }

        /// <summary>
        /// 通知添加到视图
        /// </summary>
        /// <param name="content"></param>
        public void AddNotificationToView(UNNotificationContent content)
        {
            //AepApp.ViewModel.PushsPageViewModel.Item item = new ViewModel.PushsPageViewModel.Item()
            //{
            //    Id = content.CategoryIdentifier,
            //    Text = content.Title,
            //    Detail = content.Body,
            //};
            //AepApp.ViewModel.PushsPageViewModel.Instance.AddItem(item);
        }
    }
}
