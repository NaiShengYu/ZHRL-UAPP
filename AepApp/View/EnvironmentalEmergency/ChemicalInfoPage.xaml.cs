using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalInfoPage : ContentPage
    {
        public ChemicalInfoPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

        }
    }
}
