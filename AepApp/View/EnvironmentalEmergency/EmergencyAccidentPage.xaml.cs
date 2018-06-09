using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAccidentPage : ContentPage
    {
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqEmergencyAccidentInfo(searchKey, "", 0, 10); //网络请求专家库，10条每次       
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqEmergencyAccidentInfo(searchKey, "", 0, 10); //网络请求专家库，10条每次       
        }
        string searchKey = "";

        private int totalNum = 0;
        private ObservableCollection<EmergencyAccidentPageModels.ItemsBean> dataList = new ObservableCollection<EmergencyAccidentPageModels.ItemsBean>();
      

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            EmergencyAccidentPageModels.ItemsBean item = e.SelectedItem as EmergencyAccidentPageModels.ItemsBean;
            if (item == null)
                return;
            Navigation.PushAsync(new EmergencyAccidentInfoPage(item.name,item.id));
            listView.SelectedItem = null;
        }
   
        public EmergencyAccidentPage()
        {
            InitializeComponent();
            ReqEmergencyAccidentInfo(searchKey, "", 0, 10);
        }

        private async void ReqEmergencyAccidentInfo(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.EmergencyModule.url + DetailUrl.GetEmergencyAccidentList +
                    "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount; ;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode != HttpStatusCode.ExpectationFailed)
            {
                Console.WriteLine(hTTPResponse.Results);
                EmergencyAccidentPageModels.EmergencyAccidentBean accidentPageModels = new EmergencyAccidentPageModels.EmergencyAccidentBean();
                accidentPageModels = JsonConvert.DeserializeObject<EmergencyAccidentPageModels.EmergencyAccidentBean>(hTTPResponse.Results);
                totalNum = accidentPageModels.result.incidents.totalCount;
                List<EmergencyAccidentPageModels.ItemsBean> list = accidentPageModels.result.incidents.items;
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
            public string title { get; set; }
            public string time { set; get; }
            public string type { set; get; }
            public string state { set; get; }
        }

        private  void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            EmergencyAccidentPageModels.ItemsBean item = e.Item as EmergencyAccidentPageModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < totalNum)
                {
                    ReqEmergencyAccidentInfo(searchKey, "", 0, 10); //网络请求救援地点，10条每次
                }
            }
        }
    }
}
