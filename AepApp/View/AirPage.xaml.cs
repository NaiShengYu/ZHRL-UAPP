using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string result;

        //AirPageModels pageModels = new AirPageModels();
        // private List<AirPageModels> airPages = new List<AirPageModels>();

        //airPages.Add


        public AirPage ()
		{
			InitializeComponent ();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = "环境空气站";
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                Navigation.PushAsync(new MapPage());               
            }));
            //请求网络数据
            ReqAirSiteData();
            //pageModels.Rank = "第1";
            //pageModels.SiteName = "南浔区站";
            //pageModels.MajorPollutants = "臭氧";
            //pageModels.UnitCount = "15616.4515";
            //airPages.Add(pageModels);
            //listView.ItemsSource = airPages;
        }

        private void ReqAirSiteData()
        {
            //api/FactorData/GetLastAQIValsForPhone
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                string uri = App.BaseUrl + "/api/FactorData/GetLastAQIValsForPhone";
                result = EasyWebRequest.sendGetHttpWebRequest(uri);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                string test = "[{                    'StationId': 'f695c6da-880e-4db6-bf57-16239d477aba',        'StationName': '安吉城东',        'StationLng': 119.69527435,        'StationLat': 30.59805489,        'info': []    }                ]";
                List<AirPageModels.AirInfo>  info = JsonConvert.DeserializeObject<List<AirPageModels.AirInfo>>(test);
                //listView.ItemsSource = JsonConvert.DeserializeObject<List<AirPageModels.AirInfo>>(result);
            };
            wrk.RunWorkerAsync();
        }

        private void PMSort(object sender, EventArgs e)
        {
            UnitName.Text = "PM2.5(ug/m3)";            
        }

        private void AQIsort(object sender, EventArgs e)
        {
            UnitName.Text = "AQI(ug/m3)";           
        }
    }
}