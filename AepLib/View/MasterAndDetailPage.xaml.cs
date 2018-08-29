using System;
using System.Collections.Generic;

using Xamarin.Forms;

using AepApp.View.EnvironmentalEmergency;
using AepApp.Models;
using CloudWTO.Services;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CloudWTO.Services;
using System.Net;
using AepApp.Models;
using Newtonsoft.Json;

using AepApp.View.Gridding;
namespace AepApp.View
{
    public partial class MasterAndDetailPage : MasterDetailPage
    {
        public readonly static BindableProperty WidthRatioProperty =
            BindableProperty.Create("WidthRatio",
            typeof(float),
            typeof(MasterAndDetailPage),
            (float)0.6);

        bool isfinishEmergency = false;
        bool isfinishENVQ = false;
        bool isfinishEP360 = false;
        bool isfinishSampling = false;


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


            App.appHunbegerPage = this;
            Master = new MasterPage(this);
            Master.WidthRequest = 100;

            Detail = new NavigationPage(new TaskInfoTypeTowPage("",false,""));

        }

    }
}
