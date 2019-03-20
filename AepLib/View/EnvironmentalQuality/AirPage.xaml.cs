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



namespace AepApp.View.EnvironmentalQuality
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AirPage : ContentPage
    {
        private string result;
        string serchKey = "";
        ObservableCollection<AirPageModels.AirInfo> dataList = new ObservableCollection<AirPageModels.AirInfo>();
        private List<AirPageModels.AirInfo> airPages = new List<AirPageModels.AirInfo>();
        private List<AirPageModels.AirInfo> sendPages = new List<AirPageModels.AirInfo>(); //将有数据的信息传入地图界面使用

        public AirPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = App.moduleConfigENVQ.menuAirLabel;
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                Navigation.PushAsync(new AQIMapPage(sendPages));
            }));
             
                //请求网络数据
            //CrossHud.Current.Show("加载中...");
            ReqAirSiteData();       
        }

        async void ReqAirSiteData()
        {
                string uri = App.environmentalQualityModel.url + "/api/FactorData/GetLastAQIValsForPhone";
                HTTPResponse hTTPResponse =await EasyWebRequest.SendHTTPRequestAsync(uri, "", "GET", "", "json");
                try{
                    airPages = JsonConvert.DeserializeObject<List<AirPageModels.AirInfo>>(hTTPResponse.Results);
                    SortAQI();
                listView.ItemsSource = dataList;
                if (dataList.Count != 0)
                    Title =App.moduleConfigENVQ.menuAirLabel + "（" + dataList.Count + "）";
                else
                    Title = App.moduleConfigENVQ.menuAirLabel;
            }
            catch(Exception e)
            {
              
                }
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
                    info.showLab = info.info.AQI;
                    info.facName = info.info.facName;
                    sendPages.Add(info);
                }
            }
            getCurrentData(serchKey);
        }
        private void SortPM()
        {
            airPages.Sort((a, b) => { if (a.info == null || a.info.PM25 =="null") return 1; else if (b.info == null || b.info.PM25 == "null") return -1; else  return -a.info.PM25.CompareTo(b.info.PM25);});
            for (int i = 0; i < airPages.Count; i++)
            {
                airPages[i].Rank = (i + 1) + "";
                if (airPages[i].info != null)
                {
                    AirPageModels.AirInfo info = airPages[i];
                    if (info.info.PM25 !="null"){
                        info.showLab = Convert.ToDouble(info.info.PM25);
                        info.facName = info.info.facName;
                    }
                    else
                    {
                        info.showLab = 0;
                        info.facName = "";
                    }
                }

            }
            int num = airPages.Count();
            dataList.Clear();
            for (int i = 0; i < num; i++)
            {
                if (airPages[i].StationName.Contains(serchKey))//数据匹配
                {
                    if (airPages[i].info.PM25 != "null")
                    {
                        dataList.Add(airPages[i]);
                        airPages[i].Rank = dataList.Count.ToString();

                    }
                }
            }


        }

        private void PMSort(object sender, EventArgs e)
        {
            UnitName.Text = "PM2.5(μg/m³)";
            SortPM();
        }

        private void AQIsort(object sender, EventArgs e)
        {
            UnitName.Text = "AQI";
            SortAQI();
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null) return;
            AirPageModels.AirInfo airInfo =  e.SelectedItem as AirPageModels.AirInfo;
            if (airInfo.info != null)
            {
                Navigation.PushAsync(new AirDetailPage(airInfo));
                //DependencyService.Get<Sample.IToast>().ShortAlert("(　o=^•ェ•)o　┏━┓");
            }
            listView.SelectedItem = null;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            serchKey = e.NewTextValue;
            getCurrentData(serchKey);
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