
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalQuality
{
    public partial class WaterQualitySiteListPage : ContentPage
    {

        string _searchKey = "";
        int _pageIndex = 0;
        private ObservableCollection<WaterQualityItem> dataList = new ObservableCollection<WaterQualityItem>();
        bool _isEnd = false;
        void SearchBar_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(_searchKey))
            {
                _isEnd = false;
                _pageIndex = 0;
                ReqVOCSite();
            }
        }

        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            _isEnd = false;
            _pageIndex = 0;
            dataList.Clear();
            ReqVOCSite();
        }



        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null) return;
            WaterQualityItem item = e.SelectedItem as WaterQualityItem;
            if (item == null)
                return;
            Navigation.PushAsync(new VOCDetailPage(item.basic, 1));
            listView.SelectedItem = null;

        }

        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            WaterQualityItem item = e.Item as WaterQualityItem;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (_isEnd == false) ReqVOCSite();
            }

        }
        //水质量
        public WaterQualitySiteListPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = App.moduleConfigENVQ.menuWaterLabel;
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                var mapVC = new VOCMapPage(dataList);
                mapVC.Title = App.moduleConfigENVQ.menuWaterLabel;
                Navigation.PushAsync(mapVC);
            }));
            ReqVOCSite();
            listView.ItemsSource = dataList;
        }



        private async void ReqVOCSite()
        {

            if (_isEnd == true) return;//如果满了就不要请求了

            string url = App.environmentalQualityModel.url + DetailUrl.GetWaterSite;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("searchKey", _searchKey);
            dic.Add("pageIndex", _pageIndex);
            dic.Add("pageSize", 20);
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    _pageIndex += 1;
                    var result = Tools.JsonUtils.DeserializeObject<WaterQualitySiteModel>(hTTPResponse.Results);
                    if (result == null || result.Items == null) return;
                    var total = result.Totals;
                    List<WaterQualityItem> list = result.Items;
                    foreach (var item in list)
                    {
                        dataList.Add(item);
                    }
                    if (dataList.Count >= result.Totals) _isEnd = true;
                    else _isEnd = false;
                    if (total != 0) Title = App.moduleConfigENVQ.menuWaterLabel + "（" + total + "）";
                    else Title = App.moduleConfigENVQ.menuWaterLabel;
                }
                catch (Exception ex)
                {

                }

            }
        }
    }
}
