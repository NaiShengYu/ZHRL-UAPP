using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class SuccessCase : ContentPage
    {
        private int start = 0;
        private int totalNum = 0;
        private ObservableCollection<SuccessCaseModels.ItemsBean> dataList = new ObservableCollection<SuccessCaseModels.ItemsBean>();
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

        public SuccessCase()
        {
            InitializeComponent();

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {

            };

            ReqSuccessCase("", "", start, 10);



            //var item1 = new item
            //{
            //    imgSourse = "https://ss1.bdstatic.com/70cFvXSh_Q1YnxGkpoWK1HF6hhy/it/u=729412813,2297218092&fm=27&gp=0.jpg",
            //    info = "丙烯氰污染事故及处理案例",
            //};

            //dataList.Add(item1);

            //var item2 = new item
            //{
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=331890373,3824021971&fm=27&gp=0.jpg",
            //    info = "六百余桶危险废物偷弃菜州",
            //};

            //dataList.Add(item2);

            //var item3 = new item
            //{
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
            //    info = "厉害了，sdfa",
            //};

            //dataList.Add(item3);

            //listView.ItemsSource = dataList;

        }

        private async void ReqSuccessCase(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.BaseUrlForYINGJI + DetailUrl.SuccessCase + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                start += 10;
                SuccessCaseModels.SuccessCaseBean successCaseBean = new SuccessCaseModels.SuccessCaseBean();
                successCaseBean = JsonConvert.DeserializeObject<SuccessCaseModels.SuccessCaseBean>(hTTPResponse.Results);
                totalNum = successCaseBean.result.cases.totalCount;
                List<SuccessCaseModels.ItemsBean> list = successCaseBean.result.cases.items;
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
            public string imgSourse { get; set; }
            public string info { set; get; }

        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            SuccessCaseModels.ItemsBean item = e.Item as SuccessCaseModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (start <= totalNum)
                {
                    ReqSuccessCase("", "", start, 10); //网络请求敏感源，10条每次
                }
            }
        }
    }
}
