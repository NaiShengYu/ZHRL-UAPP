using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//采样检测项目列表
namespace AepApp.View.EnvironmentalEmergency
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SampleItemsListPage : ContentPage
    {
        private string mSearchKey;
        private ObservableCollection<SampleExamineItem> dataList = new ObservableCollection<SampleExamineItem>();

        public SampleItemsListPage()
        {
            InitializeComponent();
            SearchData();
        }


        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            SampleExamineItem s = e.SelectedItem as SampleExamineItem;
            if (s == null)
            {
                return;
            }
            MessagingCenter.Send<ContentPage, SampleExamineItem>(this, "selectItem", s);
            Navigation.PopAsync();
        }


        public void Handle_TextChanged(Object sender, TextChangedEventArgs e)
        {
            mSearchKey = e.NewTextValue;
            SearchData();
        }


        public void Handle_Search(Object sender, EventArgs e)
        {
            SearchData();
        }

        private void SearchData()
        {
            dataList.Clear();
            ReqList();
        }

        private async void ReqList()
        {
            string url = App.SamplingModule.url + "/api/Analysistype/PagedList";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", -1);
            map.Add("searchKey", mSearchKey);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    SampleExamineModel examineModel = JsonConvert.DeserializeObject<SampleExamineModel>(res.Results);
                    if (examineModel != null && examineModel.items != null && examineModel.items.Count > 0)
                    {
                        List<SampleExamineItem> list = examineModel.items;
                        dataList = new ObservableCollection<SampleExamineItem>(list);
                    }
                    listView.ItemsSource = dataList;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}