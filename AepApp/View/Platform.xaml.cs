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
		public Platform ()
		{
			InitializeComponent ();
            NavigationPage.SetBackButtonTitle(this, "");
            NavigationPage.SetHasBackButton(this, false);         
            this.Title = "智慧环保预警平台";
            //grid.MinimumHeightRequest = grid.Width / 2.0 * 2.0;    
        }
    }
}