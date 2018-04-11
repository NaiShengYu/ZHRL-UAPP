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

                airPages.Sort((a, b) => { if (a.info == null) return 1; else if (b.info == null) return -1; else return -a.info.AQI.CompareTo(b.info.AQI); });
                for (int i = 0; i < airPages.Count; i++) {
                    airPages[i].Rank = (i + 1)+ "";
                }

                //for (int i = 0; i < airPages.Count; i++)
                //{
                //    AirPageModels.AirInfo item = airPages[i];

                //    if (item.info == null)//数据匹配
                //    {
                //        hasNoAirDate.Add(item);
                //    }
                //    else {
                //        hasAirDate.Add(item);
                //    }
                //}
                //hasAirDate.Sort(delegate (AirPageModels.Pollutant x, AirPageModels.Pollutant y)
                //{
                //    return y.AQI.CompareTo(x.AQI);
                //});
                //var result1 = hasAirDate.OrderBy(a => a.info.AQI);               
                //airPages.RemoveRange(0,airPages.Count);
                //airPages.AddRange(hasAirDate);
                //airPages.AddRange(hasNoAirDate);
                listView.ItemsSource = airPages;
                CrossHud.Current.Dismiss();
            };
            wrk.RunWorkerAsync();
        }

        private void PMSort(object sender, EventArgs e)
        {
            UnitName.Text = "PM2.5(μg/m³)";
            //listView.ItemTemplate.SetBinding(Label.TextProperty, "info.PM25");
            listView.ItemTemplate = this.Resources["pm25"] as DataTemplate;
        }

        private void AQIsort(object sender, EventArgs e)
        {
            UnitName.Text = "AQI";
            //listView.ItemTemplate.SetBinding(Label.TextProperty, "info.AQI");
            listView.ItemTemplate = this.Resources["aqi"] as DataTemplate;
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AirPageModels.AirInfo airInfo =  e.SelectedItem as AirPageModels.AirInfo;
        }
    }

 

}