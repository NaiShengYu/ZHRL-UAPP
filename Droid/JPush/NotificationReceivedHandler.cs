﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CN.Jpush.Android.Api;

namespace AepApp.Droid.JPush
{
    /// <summary>
    /// Receive Notification
    /// </summary>
    public class NotificationReceivedHandler : BaseHandler
    {
        public override string Action
        {
            get
            {
                return "cn.jpush.android.intent.NOTIFICATION_RECEIVED";
            }
        }

        public override void Handle(Intent bundle)
        {
            //保存服务器推送下来的通知的标题。
            //对应 API 通知内容的 title 字段。
            //对应 Portal 推送通知界面上的“通知标题”字段。
            var title = bundle.GetStringExtra(JPushInterface.ExtraNotificationTitle);

            //保存服务器推送下来的通知内容。
            //对应 API 通知内容的 alert 字段。
            //对应 Portal 推送通知界面上的“通知内容”字段。
            var ctx = bundle.GetStringExtra(JPushInterface.ExtraAlert);

            //SDK 1.2.9 以上版本支持。
            //保存服务器推送下来的附加字段。这是个 JSON 字符串。
            //对应 API 通知内容的 extras 字段。
            //对应 Portal 推送消息界面上的“可选设置”里的附加字段。
            var extra = bundle.GetStringExtra(JPushInterface.ExtraExtra);

            //SDK 1.3.5 以上版本支持。
            //通知栏的Notification ID，可以用于清除Notification
            var id = bundle.GetIntExtra(JPushInterface.ExtraNotificationId, 0);

            //SDK 1.6.1 以上版本支持。
            //唯一标识通知消息的 ID, 可用于上报统计等。
            var msgID = bundle.GetStringExtra(JPushInterface.ExtraMsgId);

            //保存服务器推送下来的内容类型。
            //对应 API 消息内容的 content_type 字段。
            //Portal 上暂时未提供输入字段。
            var contentType = bundle.GetStringExtra(JPushInterface.ExtraContentType);

            //SDK 1.4.0 以上版本支持。
            //富媒体通知推送下载的HTML的文件路径,用于展现WebView。            
            var richHtmlPath = bundle.GetStringExtra(JPushInterface.ExtraRichpushHtmlPath);

            //SDK 1.4.0 以上版本支持。
            //富媒体通知推送下载的图片资源的文件名,多个文件名用 “，” 分开。 与 “JPushInterface.EXTRA_RICHPUSH_HTML_PATH” 位于同一个路径。
            var richHtmlRes = bundle.GetStringExtra(JPushInterface.ExtraRichpushHtmlRes);
        }
    }
}