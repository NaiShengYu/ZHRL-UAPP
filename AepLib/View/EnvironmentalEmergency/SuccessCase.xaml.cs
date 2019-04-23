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
        private int totalNum = 0;
        private ObservableCollection<SuccessCaseModels.ItemsBean> dataList = new ObservableCollection<SuccessCaseModels.ItemsBean>();
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqSuccessCase(searchKey, ""); //网络请求专家库，10条每次       
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqSuccessCase(searchKey, ""); //网络请求专家库，10条每次       
        }
        string searchKey = "";

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as SuccessCaseModels.ItemsBean;
            if (item == null)
            {
                return;
            }

            List<SuccessCaseModels.FilesBean> file = item.files;
            string fileName = file[0].id + "." + file[0].format;
            downloadPlan(item.files[0].storeUrl, fileName);
            listView.SelectedItem = null;
        }

        public SuccessCase()
        {
            InitializeComponent();

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
          
            };

            ReqSuccessCase(searchKey, "");

        }


        private async void downloadPlan(String filePath, string fileFormat)
        {
            string url = App.EmergencyModule.url + filePath;
            HTTPResponse hTTPResponse = await EasyWebRequest.HTTPRequestDownloadAsync(url, fileFormat, App.EmergencyToken);
            await Navigation.PushAsync(new ShowFilePage(fileFormat));
        }

        private async void ReqSuccessCase(String Filter, String Sorting)
        {
            string url = App.EmergencyModule.url + DetailUrl.SuccessCase + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=10" + "&SkipCount=" + dataList.Count;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                SuccessCaseModels.SuccessCaseBean successCaseBean = new SuccessCaseModels.SuccessCaseBean();
                successCaseBean = Tools.JsonUtils.DeserializeObject<SuccessCaseModels.SuccessCaseBean>(hTTPResponse.Results);
                if (successCaseBean == null || successCaseBean.result == null || successCaseBean.result.cases == null || successCaseBean.result.cases.items == null) return;
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


        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            SuccessCaseModels.ItemsBean item = e.Item as SuccessCaseModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < totalNum)
                {
                    ReqSuccessCase(searchKey, ""); //网络请求敏感源，10条每次
                }
            }
        }
    }
}
