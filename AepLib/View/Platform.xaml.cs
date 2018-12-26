using AepApp.View.EnvironmentalQuality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Platform : ContentPage
    {
        public Platform()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            NavigationPage.SetHasBackButton(this, false);
            this.Title = "智慧环保预警平台";
            //grid.MinimumHeightRequest = grid.Width / 2.0 * 2.0;    
        }

        //private void VOC_CLK(object sender, EventArgs e)
        //{
           
        //}

        private void AIR_CLK(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AirPage());
        }

        //private void WATER_CLK(object sender, EventArgs e)
        //{
          
        //}

        //private void NOISE_CLK(object sender, EventArgs e)
        //{
           
        //}
        //private void SOIL_CLK(object sender, EventArgs e)
        //{
           
        //}
        //private void RADIATION_CLK(object sender, EventArgs e)
        //{
           
        //}
        //private void DUST_CLK(object sender, EventArgs e)
        //{
            
        //}
        //private void HAZARD_CLK(object sender, EventArgs e)
        //{
            
        //}
        private void EMERG_CLK(object sender, EventArgs e)
        {
            
        }
        private void POLL_CLK(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PollutionSourcePage());
        }

    }
}