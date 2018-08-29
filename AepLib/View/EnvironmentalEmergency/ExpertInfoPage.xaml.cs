using System;
using System.Collections.Generic;
using AepApp.Models;
using AepApp.Tools;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ExpertInfoPage : ContentPage
    {
        //打电话
        void phone_Tapped(object sender, System.EventArgs e)
        {
            var but = sender as Button;
            ExpertLibraryModels.ItemsBean item = but.BindingContext as ExpertLibraryModels.ItemsBean;
            DeviceUtils.phone(item.mobilePhone);
        }
        //发信息
        void sms_Tapped(object sender, System.EventArgs e)
        {
            var but = sender as Button;
            ExpertLibraryModels.ItemsBean item = but.BindingContext as ExpertLibraryModels.ItemsBean;
            DeviceUtils.sms(item.mobilePhone);
        }

        public ExpertInfoPage(ExpertLibraryModels.ItemsBean item)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = item.name + "信息";
            BindingContext = item;
        }
    }
}
