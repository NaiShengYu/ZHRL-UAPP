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
        private int start = 0;
        private int totalNum = 0;
        private ObservableCollection<SensitiveModels.ItemsBean> dataList = new ObservableCollection<SensitiveModels.ItemsBean>();
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }

       

        public SensitiveSourcePage()
        {
            InitializeComponent();
            ReqSensitiveSource("", "", start, 10);
            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                //Navigation.PushAsync(new PollutionSourceMapPage(dataList));
            }));
        }

        private async void ReqSensitiveSource(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.EmergencyModule.url + DetailUrl.Sensitive + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK) {
                Console.WriteLine(hTTPResponse.Results);
                start += 10;
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

        internal class item
        {
            public string name { get; set; }
            public string address { set; get; }
            public string type { get; set; }

        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            SensitiveModels.ItemsBean item = e.Item as SensitiveModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (start <= totalNum)
                {
                    ReqSensitiveSource("", "", start, 10); //网络请求敏感源，10条每次
                }
            }
        }
    }
}
