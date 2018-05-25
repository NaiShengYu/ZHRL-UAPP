using System;
using System.Collections.Generic;

using Xamarin.Forms;

using AepApp.View.EnvironmentalEmergency;
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
    }
}
