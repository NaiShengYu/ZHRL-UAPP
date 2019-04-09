
using System;
using System.Collections.Generic;
using AepApp.Models;
using InTheHand.Forms;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ShowVideoPage : ContentPage
    {
        void Handle_CurrentStateChanged(object sender, System.EventArgs e)
        {

            Console.WriteLine("播放器状态：" +MV.CurrentState);
        
        
        }

        public ShowVideoPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = "视频播放";
        }

        public ShowVideoPage(UploadEmergencyShowModel model):this(){

            this.BindingContext = model;
            Console.WriteLine("时间长度" + MV.NaturalDuration);

        }
        public ShowVideoPage(EmergencyAccidentInfoDetail.IncidentLoggingEventsBean model) : this()
        {

            this.BindingContext = model;

            Console.WriteLine("时间长度" + MV.NaturalDuration);

        }

    }
}
