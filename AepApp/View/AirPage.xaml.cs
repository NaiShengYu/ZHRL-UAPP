using AepApp.Models;
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
	public partial class AirPage : ContentPage
	{
        AirPageModels pageModels = new AirPageModels();
        private List<AirPageModels> airPages = new List<AirPageModels>();
       
        //airPages.Add
               

        public AirPage ()
		{
			InitializeComponent ();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = "环境空气站";
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                DependencyService.Get<Sample.IToast>().ShortAlert("(╬▔皿▔)凸");
            }));
            pageModels.Rank = "第1";
            pageModels.SiteName = "南浔区站";
            pageModels.MajorPollutants = "臭氧";
            pageModels.UnitCount = "15616.4515";
            airPages.Add(pageModels);
            listView.ItemsSource = airPages;
        }


    }
}