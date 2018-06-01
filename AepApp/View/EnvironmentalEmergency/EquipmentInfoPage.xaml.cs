using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EquipmentInfoPage : ContentPage
    {
        public EquipmentInfoPage(string name)
        {
            InitializeComponent();
            Title = name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

        }
    }
}
