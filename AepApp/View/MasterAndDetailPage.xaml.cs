using System;
using System.Collections.Generic;

using Xamarin.Forms;

using AepApp.View.EnvironmentalEmergency;
using Xamarin.Forms.PlatformConfiguration;

namespace AepApp.View
{
    public partial class MasterAndDetailPage : MasterDetailPage
    {
        public MasterAndDetailPage()
        {
            InitializeComponent();
            Master = new MasterPage(this);
            //Detail = new NavigationPage(new AirPage());
            Detail = new NavigationPage(new EmergencyAccidentPage());

        }
        protected override bool OnBackButtonPressed()
        {
            //return base.OnBackButtonPressed();
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            return true;
        }
    }
}
