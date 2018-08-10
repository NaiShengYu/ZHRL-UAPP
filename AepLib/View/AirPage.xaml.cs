using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<AirPageModels.AirInfo> dataList = new ObservableCollection<AirPageModels.AirInfo>();
        private List<AirPageModels.AirInfo> airPages = new List<AirPageModels.AirInfo>();
        private List<AirPageModels.AirInfo> sendPages = new List<AirPageModels.AirInfo>(); //将有数据的信息传入地图界面使用

        public AirPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = "环境空气站";
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                //Navigation.PushAsync(new MapPage());
                Navigation.PushAsync(new AQIMapPage(sendPages));
                //Navigation.PushAsync(new MapPage2(sendPages));
            }));

                //请求网络数据
            //CrossHud.Current.Show("加载中...");
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

                try{
                    airPages = JsonConvert.DeserializeObject<List<AirPageModels.AirInfo>>(result);
                    SortAQI();
                    listView.ItemsSource = dataList;
                }catch{
                    
                }
              
                CrossHud.Current.Dismiss();
            };
            wrk.RunWorkerAsync();
        }

        private void SortAQI()
        {
            airPages.Sort((a, b) => { if (a.info == null) return 1; else if (b.info == null) return -1; else return -a.info.AQI.CompareTo(b.info.AQI); });
            sendPages.Clear();
            for (int i = 0; i < airPages.Count; i++)
            {
                airPages[i].Rank = (i + 1) + "";               
                if (airPages[i].info != null) {
                    AirPageModels.AirInfo info = airPages[i];
                    sendPages.Add(info);
                }
            }
            getCurrentData("");
        }
        private void SortPM()
        {
            airPages.Sort((a, b) => { if (a.info == null) return 1; else if (b.info == null) return -1; else return -a.info.PM25.CompareTo(b.info.PM25); });
            for (int i = 0; i < airPages.Count; i++)
            {
                airPages[i].Rank = (i + 1) + "";

            }
            getCurrentData("");
        }

        private void PMSort(object sender, EventArgs e)
        {
            UnitName.Text = "PM2.5(μg/m³)";
            SortPM();
            listView.ItemTemplate = this.Resources["pm25"] as DataTemplate;
        }

        private void AQIsort(object sender, EventArgs e)
        {
            UnitName.Text = "AQI";
            SortAQI();
            listView.ItemTemplate = this.Resources["aqi"] as DataTemplate;
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AirPageModels.AirInfo airInfo =  e.SelectedItem as AirPageModels.AirInfo;
            if (airInfo.info != null)
            {
                Navigation.PushAsync(new AirDetailPage(airInfo));
                //DependencyService.Get<Sample.IToast>().ShortAlert("(　o=^•ェ•)o　┏━┓");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = sender as SearchBar;            
            getCurrentData(e.NewTextValue);
        }
        private void getCurrentData(String value)
        {
            int num = airPages.Count();
            dataList.Clear();
            for (int i = 0; i < num; i++)
            {             
                if (airPages[i].StationName.Contains(value))//数据匹配
                {
                    dataList.Add(airPages[i]);
                }
            }            
        }
    }

 

}