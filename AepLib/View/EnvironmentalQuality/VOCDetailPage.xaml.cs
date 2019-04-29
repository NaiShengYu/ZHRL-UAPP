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
        private string _siteId; //站点ID
        ObservableCollection<Factors> factors = new ObservableCollection<Factors>();

        int _type = 0;//1环境VOCs，邵峰倪黎腾负责  2。排口厂界张东明负责
        public VOCDetailPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");


        }

        public VOCDetailPage(string siteId, int type) : this()
        {
            _type = type;
            _siteId = siteId;
            ReqSiteFactors();
        }

        private async void ReqSiteFactors()
        {
            string url = "";
            if (_type == 1) url = App.environmentalQualityModel.url + DetailUrl.GetVOCSiteFactor;
            else url = App.EP360Module.url + DetailUrl.GetPaiKouAndChangJieSiteFactor;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _siteId);
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<Factors> list = Tools.JsonUtils.DeserializeObject<List<Factors>>(res.Results);
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
            ReqSiteFactorsLatestValue();
        }

        private async void ReqSiteFactorsLatestValue()
        {
            string url = "";
            if (_type == 1) url = App.environmentalQualityModel.url + DetailUrl.GetVOCSiteFactorLatestValue;
            else url = App.EP360Module.url + DetailUrl.GetPaiKouAndChangJieSiteFactorLatestValue;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("refId", _siteId);
            map.Add("fromType", "0");
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<FactorLatestValue> list = Tools.JsonUtils.DeserializeObject<List<FactorLatestValue>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            foreach (var factor in factors)
                            {
                                if (item.id == factor.id)
                                    factor.val = item.val;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }



        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null) return;
            Factors factorInfo = e.SelectedItem as Factors;
            if (factorInfo == null) return;
            Navigation.PushAsync(new VOCChartPage(_siteId, factorInfo, _type));
            listView.SelectedItem = null;
        }


        private class FactorLatestValue
        {
            public string id { get; set; }//因子ID
            public string val { get; set; }//因子ID


        }
    }
}