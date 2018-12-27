using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AepApp.Models.VOCDetailModels;

namespace AepApp.View.EnvironmentalQuality
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VOCDetailPage : ContentPage
    {
        private VOCSiteListModel siteInfo;
        private string siteId; //站点ID
        ObservableCollection<Factors> factors = new ObservableCollection<Factors>();

        int _type = 0;//1环境VOCs，2VOCs厂界，3VOCs排口
        public VOCDetailPage(VOCSiteListModel siteInfo,int type)
        {
            InitializeComponent();
            _type = type;
            this.Title = siteInfo.name;
            this.siteInfo = siteInfo;
            siteId = siteInfo.id;
            if(_type ==1) ReqSiteFactors();
            else ReqSiteFactors1();
        }

        private async void ReqSiteFactors()
        {
            string url = App.environmentalQualityModel.url + DetailUrl.GetVOCSiteFactor;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", siteId);
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<Factors> list = JsonConvert.DeserializeObject<List<Factors>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        factors.Clear();
                        foreach (var item in list)
                        {
                            factors.Add(item);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            listView.ItemsSource = factors;
        }


        private async void ReqSiteFactors1()
        {
            string url = App.EP360Module.url + DetailUrl.GetPaiKouAndChangJieSiteFactor;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", siteId);
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST",App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<Factors> list = JsonConvert.DeserializeObject<List<Factors>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        factors.Clear();
                        foreach (var item in list)
                        {
                            factors.Add(item);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            listView.ItemsSource = factors;
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null) return;
            Factors factorInfo = e.SelectedItem as Factors;
            if (factorInfo == null) return;
            Navigation.PushAsync(new VOCChartPage(siteId, factorInfo,_type));
            listView.SelectedItem = null;
        }
    }
}