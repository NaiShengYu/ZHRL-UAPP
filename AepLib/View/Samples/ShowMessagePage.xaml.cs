
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class ShowMessagePage : ContentPage
    {
        public ShowMessagePage(string info)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            BindingContext = info;
        }
    }
}
