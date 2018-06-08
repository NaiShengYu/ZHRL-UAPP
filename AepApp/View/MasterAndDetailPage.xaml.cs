using System;
using System.Collections.Generic;

using Xamarin.Forms;

using AepApp.View.EnvironmentalEmergency;
namespace AepApp.View
{
    public partial class MasterAndDetailPage : MasterDetailPage
    {
        public readonly static BindableProperty WidthRatioProperty =
            BindableProperty.Create("WidthRatio",
            typeof(float),
            typeof(MasterAndDetailPage),
            (float)0.6);

        public float WidthRatio
        {
            get
            {
                return (float)GetValue(WidthRatioProperty);
            }
            set
            {
                SetValue(WidthRatioProperty, value);
            }
        }



        public MasterAndDetailPage()
        {
            InitializeComponent();
            Master = new MasterPage(this);
            Master.WidthRequest = 100;

            Detail = new NavigationPage(new EmergencyAccidentPage());

        }
    }
}
