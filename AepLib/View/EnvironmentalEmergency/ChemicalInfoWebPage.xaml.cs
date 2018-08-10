

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalInfoWebPage : ContentPage
    {
        public ChemicalInfoWebPage(string sss)
        {
            InitializeComponent();

            HtmlWebViewSource aaa = new HtmlWebViewSource
            {
                Html = sss,
            };
            web.Source = aaa;
        
        }
    }
}
