using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class SensitiveSourcePage : ContentPage
    {
        private int totalNum = 0;
        private ObservableCollection<SensitiveModels.ItemsBean> dataList = new ObservableCollection<SensitiveModels.ItemsBean>();
     
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as SensitiveModels.ItemsBean;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqSensitiveSource(searchKey, "", 0, 10); //网络请求专家库，10条每次       
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqSensitiveSource(searchKey, "", 0, 10); //网络请求专家库，10条每次       
        }
        string searchKey = "";

        public SensitiveSourcePage()
        {
            InitializeComponent();
            ReqSensitiveSource(searchKey, "", 0, 10); //网络请求专家库，10条每次       

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                Navigation.PushAsync(new RescueSiteMapPage(dataList));
            }));
        }

        private async void ReqSensitiveSource(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.EmergencyModule.url + DetailUrl.Sensitive + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK) {
                Console.WriteLine(hTTPResponse.Results);
                SensitiveModels.SensitiveBean sensitiveBean = new SensitiveModels.SensitiveBean();
                sensitiveBean = JsonConvert.DeserializeObject<SensitiveModels.SensitiveBean>(hTTPResponse.Results);
                totalNum = sensitiveBean.result.sensitiveUnits.totalCount;
                List<SensitiveModels.ItemsBean> list = sensitiveBean.result.sensitiveUnits.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
            }
        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            SensitiveModels.ItemsBean item = e.Item as SensitiveModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count <= totalNum)
                {
                    ReqSensitiveSource(searchKey, "", dataList.Count, 10); //网络请求敏感源，10条每次
                }
            }
        }
    }
}
