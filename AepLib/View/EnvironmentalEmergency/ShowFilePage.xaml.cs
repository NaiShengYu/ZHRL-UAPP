using System;
using System.IO;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ShowFilePage : ContentPage
    {
        public ShowFilePage(string info) : this()
        {
            web.Source = info;
        }

        public ShowFilePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "附件";
        }
    }
}
