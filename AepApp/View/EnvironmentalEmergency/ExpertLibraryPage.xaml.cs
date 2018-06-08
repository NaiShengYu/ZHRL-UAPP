using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ExpertLibraryPage : ContentPage
    {
        private int totalNum = 0;
        private ObservableCollection<ExpertLibraryModels.ItemsBean> dataList = new ObservableCollection<ExpertLibraryModels.ItemsBean>();
        //打电话
        void phone_Tapped(object sender, System.EventArgs e)
        {
            var but = sender as Image;
            ExpertLibraryModels.ItemsBean item = but.BindingContext as ExpertLibraryModels.ItemsBean;

            Device.OpenUri(new Uri("tel:" + item.mobilePhone));

        }
        //发信息
        void sms_Tapped(object sender, System.EventArgs e)
        {
            var but = sender as Image;
            ExpertLibraryModels.ItemsBean item = but.BindingContext as ExpertLibraryModels.ItemsBean;
            Device.OpenUri(new Uri("sms:" + item.mobilePhone));
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            ExpertLibraryModels.ItemsBean item = e.SelectedItem as ExpertLibraryModels.ItemsBean;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }
        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {          
            ExpertLibraryModels.ItemsBean item = e.Item as ExpertLibraryModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count <= totalNum)
                {
                    ReqExpertLibrary(searchKey,"",  dataList.Count, 10); //网络请求专家库，10条每次
                }
            }
        }




        private async void ReqExpertLibrary(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.EmergencyModule.url + DetailUrl.ExpertLibraryUrl + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ExpertLibraryModels.SpecialBean specialBean = new ExpertLibraryModels.SpecialBean();
                specialBean = JsonConvert.DeserializeObject<ExpertLibraryModels.SpecialBean>(hTTPResponse.Results);
                totalNum = specialBean.result.professionals.totalCount;
                List<ExpertLibraryModels.ItemsBean> list = specialBean.result.professionals.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
            }
        }

        //Dictionary<Button, Tuple<item>> _butData = new Dictionary<Button, Tuple<item>>();

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqExpertLibrary(searchKey,"",  0, 10); //网络请求专家库，10条每次       
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqExpertLibrary(searchKey, "", 0, 10); //网络请求专家库，10条每次       
        }

        string searchKey = "";

        public ExpertLibraryPage()
        {
            InitializeComponent();
            ReqExpertLibrary(searchKey, "", 0, 10); //网络请求专家库，10条每次       
        }
    }
}
