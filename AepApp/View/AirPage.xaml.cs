using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
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
        private List<AirPageModels.AirInfo> airPages = new List<AirPageModels.AirInfo>();
        private List<AirPageModels.AirInfo> hasAirDate = new List<AirPageModels.AirInfo>();
        private List<AirPageModels.AirInfo> hasNoAirDate = new List<AirPageModels.AirInfo>();

        //airPages.Add


        public AirPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = "环境空气站";
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                Navigation.PushAsync(new MapPage());
            }));
            //请求网络数据
            CrossHud.Current.Show("加载中...");
            ReqAirSiteData();       
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
                airPages = JsonConvert.DeserializeObject<List<AirPageModels.AirInfo>>(result);
                //排序
                //testList = testList.OrderBy(u=>u.age).ToList();
                
                for (int i = 0; i < airPages.Count; i++)
                {
                    AirPageModels.AirInfo item = airPages[i];

                    if (item.info == null)//数据匹配
                    {
                        hasNoAirDate.Add(item);
                    }
                    else {
                        hasAirDate.Add(item);
                    }
                }
                airPages.RemoveRange(0,airPages.Count);
                airPages.AddRange(hasAirDate);
                airPages.AddRange(hasNoAirDate);
                listView.ItemsSource = airPages;
                CrossHud.Current.Dismiss();
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