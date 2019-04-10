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
        public int _type = 1;
        public EventHandler<EventArgs> SelectItem;
        private ObservableCollection<SampleExamineItem> dataList = new ObservableCollection<SampleExamineItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AepApp.View.EnvironmentalEmergency.SampleItemsListPage"/> class.
        /// </summary>
        /// <param name="type">1表示因子组，2表示单因子</param>
        public SampleItemsListPage(int type)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _type = type;
            SearchData();
        }


        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            SampleExamineItem s = e.SelectedItem as SampleExamineItem;
            if (s == null)
            {
                return;
            }
            SelectItem.Invoke(s, new EventArgs());
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

            string url = "";
            if (_type==1)
                url = App.SamplingModule.url + "/api/Analysistype/PagedList";
            else
                url = App.SamplingModule.url + "/api/Factor/PagedList";
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