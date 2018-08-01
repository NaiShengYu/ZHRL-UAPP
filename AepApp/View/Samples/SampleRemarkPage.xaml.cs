using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class SampleRemarkPage : ContentPage
    {
        public SampleRemarkPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "送样（接样）备注";
        }
    }
}
