﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalQuality
{
    public partial class PaiKouSiteListPage : ContentPage
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
            Navigation.PushAsync(new VOCDetailPage(item.id, 2) { Title = item.name });
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

        public PaiKouSiteListPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = App.moduleConfigENVQ.menuPaikouLabel;
            ToolbarItems.Add(new ToolbarItem("", "map.png", () =>
            {
                var mapVC = new VOCMapPage(dataList);
                mapVC.Title = App.moduleConfigENVQ.menuPaikouLabel;
                Navigation.PushAsync(mapVC);
            }));
            ReqVOCSite();
            listView.ItemsSource = dataList;
        }



        private async void ReqVOCSite()
        {

            if (_isEnd == true) return;//如果满了就不要请求了
            Dictionary<string, object> dic = new Dictionary<string, object>();

            string url = App.EP360Module.url + DetailUrl.GetChangJieSite;
            dic.Add("keyword", _searchKey);
            dic.Add("pageIndex", _pageIndex);
            dic.Add("pageSize", 20);
            dic.Add("subtype", 4);
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    _pageIndex += 1;
                    var result = Tools.JsonUtils.DeserializeObject<VOCSiteResult>(hTTPResponse.Results);
                    if (result == null || result.Items == null) return;
                    var total = result.count;
                    List<VOCSiteListModel> list = result.Items;
                    foreach (var item in list)
                    {
                        dataList.Add(item);
                    }
                    if (dataList.Count >= result.count) _isEnd = true;
                    else _isEnd = false;
                    if (total != 0) Title = App.moduleConfigENVQ.menuPaikouLabel + "（" + total + "）";
                    else Title = App.moduleConfigENVQ.menuPaikouLabel;
                }
                catch (Exception ex)
                {

                }

            }
        }
        

    }
}
