using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalQuality
{
    public partial class NoiseSiteListPage : ContentPage
    {

        string _searchKey = "";
        int _pageIndex = 0;
        private ObservableCollection<VOCSiteListModel> dataList = new ObservableCollection<VOCSiteListModel>();
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
            VOCSiteListModel item = e.SelectedItem as VOCSiteListModel;
            if (item == null)
                return;
            Navigation.PushAsync(new VOCDetailPage(item, 1));
            listView.SelectedItem = null;

        }

        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            VOCSiteListModel item = e.Item as VOCSiteListModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (_isEnd == false) ReqVOCSite();
            }

        }
        //噪音
        public NoiseSiteListPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = App.moduleConfigENVQ.menuNoiseLabel;
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                var mapVC = new VOCMapPage(dataList);
                mapVC.Title = App.moduleConfigENVQ.menuNoiseLabel;
                Navigation.PushAsync(mapVC);
            }));

            ReqVOCSite();
            listView.ItemsSource = dataList;
        }



        private async void ReqVOCSite()
        {

            if (_isEnd == true) return;//如果满了就不要请求了

            string url = App.environmentalQualityModel.url + DetailUrl.GetVOCSite;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("searchKey", _searchKey);
            dic.Add("pageIndex", _pageIndex);
            dic.Add("pageSize", 20);
            dic.Add("type", 3);
            dic.Add("subType",10);
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    _pageIndex += 1;
                    var result = Tools.JsonUtils.DeserializeObject<VOCSiteResult>(hTTPResponse.Results);
                    if (result == null || result.Items == null) return;
                    var total = result.Totals;
                    List<VOCSiteListModel> list = result.Items;
                    foreach (var item in list)
                    {
                        dataList.Add(item);
                    }
                    if (dataList.Count >= result.Totals) _isEnd = true;
                    else _isEnd = false;
                    if (total != 0) Title = App.moduleConfigENVQ.menuNoiseLabel + "（" + total + "）";
                    else Title = App.moduleConfigENVQ.menuNoiseLabel;
                }
                catch (Exception ex)
                {

                }

            }
        }
        
    }
}
