
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueSitePage : ContentPage
    {
        private int totalNum = 0;
        private ObservableCollection<RescueSiteModel.ItemsBean> dataList = new ObservableCollection<RescueSiteModel.ItemsBean>();
       

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            RescueSiteModel.ItemsBean item = e.SelectedItem as RescueSiteModel.ItemsBean;
            if (item == null)
                return;
            Navigation.PushAsync(new RescueMaterialsPage(item));

            listView.SelectedItem = null;

        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqRescueSite(searchKey, "", 0, 10); //网络请求专家库，10条每次       
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqRescueSite(searchKey, "", 0, 10); //网络请求专家库，10条每次       
        }

        string searchKey = "";

        public RescueSitePage()
        {
            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                if(dataList.Count !=0)
                    Navigation.PushAsync(new RescueSiteMapPage(dataList));
            }));
            ReqRescueSite(searchKey, "", 0, 10); //网络请求专家库，10条每次       
        }
        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            RescueSiteModel.ItemsBean item = e.Item as RescueSiteModel.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count <= totalNum)
                {
                    ReqRescueSite("", "", dataList.Count, 10); //网络请求救援地点，10条每次
                }
            }
        }
        private async void ReqRescueSite(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.EmergencyModule.url + DetailUrl.RescueSite + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                RescueSiteModel.RescueSiteModelBean rescueSiteModel = new RescueSiteModel.RescueSiteModelBean();
                rescueSiteModel = JsonConvert.DeserializeObject<RescueSiteModel.RescueSiteModelBean>(hTTPResponse.Results);
                totalNum = rescueSiteModel.result.rescuePoints.totalCount;
                List<RescueSiteModel.ItemsBean> list = rescueSiteModel.result.rescuePoints.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
            }

        }

        internal class item
        {
            public string name { get; set; }
            public string address { set; get; }

        }
    }
}
