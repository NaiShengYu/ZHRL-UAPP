
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
        private int start = 0;
        private int totalNum = 0;
        private ObservableCollection<RescueSiteModel.ItemsBean> dataList = new ObservableCollection<RescueSiteModel.ItemsBean>();
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            RescueSiteModel.ItemsBean item = e.SelectedItem as RescueSiteModel.ItemsBean;
            if (item == null)
                return;
            Navigation.PushAsync(new RescueMaterialsPage(item));

            listView.SelectedItem = null;

        }

        public RescueSitePage()
        {
            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                Navigation.PushAsync(new RescueSiteMapPage());

            }));
            ReqRescueSite("", "", start, 10);
        }
        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            RescueSiteModel.ItemsBean item = e.Item as RescueSiteModel.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (start <= totalNum)
                {
                    ReqRescueSite("", "", start, 10); //网络请求救援地点，10条每次
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
                start += 10;
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
