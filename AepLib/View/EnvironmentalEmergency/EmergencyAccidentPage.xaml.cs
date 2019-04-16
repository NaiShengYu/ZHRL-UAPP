using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Sample;
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
        bool _isWater = false;
        bool _isGas = false;
        bool _isSoil = false;
        void Handle_AddEmergencyAccident(object sender, System.EventArgs e)
        {
            editGrid.IsVisible = true;
            addGrid.IsVisible = false;
            var toolbar = new ToolbarItem("取消", "", () =>
            {
                editGrid.IsVisible = false;
                addGrid.IsVisible = true;
                ToolbarItems.RemoveAt(0);
            });
            ToolbarItems.Add(toolbar);
        }

        void Handle_SelectGas(object sender, System.EventArgs e)
        {
            _isGas = !_isGas;
            var but = sender as Button;
            but.BackgroundColor = _isGas == true ? Color.FromHex("#92A6B0") :Color.FromHex("#FFFFFF");

        }
        void Handle_SelectWater(object sender, System.EventArgs e)
        {
            _isWater = !_isWater;
            var but = sender as Button;
            but.BackgroundColor = _isWater == true ? Color.FromHex("#2772A5") : Color.FromHex("#FFFFFF");

        }
        void Handle_SelectSoil(object sender, System.EventArgs e)
        {
            _isSoil = !_isSoil;
            var but = sender as Button;
            but.BackgroundColor = _isSoil == true ? Color.FromHex("#A56827") : Color.FromHex("#FFFFFF");
        }
       async void Handle_EditEmergencyAccident(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleEntry.Text))
            {
                DependencyService.Get<IToast>().ShortAlert("请输入标题");
                return;
            }
            string url = App.EmergencyModule.url + DetailUrl.AddEmergencyAccident;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", titleEntry.Text);
            string aaa = "";
            aaa = _isGas == true ? "1" : "0";
            aaa = aaa + (_isWater == true ? "1" : "0");
            aaa = aaa+(_isSoil == true ? "1" : "0");
            dic.Add("natureString", aaa);
            string parma = JsonConvert.SerializeObject(dic);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, parma, "POST", App.EmergencyToken);
            if (hTTPResponse.StatusCode != HttpStatusCode.ExpectationFailed)
            {
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(hTTPResponse.Results);
                var itemdic = result["result"];
                string itemString = JsonConvert.SerializeObject(itemdic);
                EmergencyAccidentPageModels.ItemsBean item = JsonConvert.DeserializeObject<EmergencyAccidentPageModels.ItemsBean>(itemString);
                dataList.Insert(0, item);
                addGrid.IsVisible = true;
                editGrid.IsVisible = false;
                ToolbarItems.RemoveAt(0);
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert("添加失败");
            }
        }
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqEmergencyAccidentInfo(searchKey, "");      
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqEmergencyAccidentInfo(searchKey, ""); //网络请求专家库，10条每次       
        }
        string searchKey = "";

        private int totalNum = 0;
        private ObservableCollection<EmergencyAccidentPageModels.ItemsBean> dataList = new ObservableCollection<EmergencyAccidentPageModels.ItemsBean>();
      

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            EmergencyAccidentPageModels.ItemsBean item = e.SelectedItem as EmergencyAccidentPageModels.ItemsBean;
            if (item == null)
                return;

            List<SampleTypeModel> aaa = new List<SampleTypeModel>();
            if(item.hasAirNature =="true"){
                SampleTypeModel model = new SampleTypeModel
                {
                    name = "大气",
                    isSelect = false,
                };
            aaa.Add(model);
            }
           
            if (item.hasWaterNature == "true")
            {
                SampleTypeModel model = new SampleTypeModel
                {
                    name = "水质",
                    isSelect = false,
                };
                aaa.Add(model);
            }
            if (item.hasSoilNature == "true")
            {
                SampleTypeModel model = new SampleTypeModel
                {
                    name = "土壤",
                    isSelect = false,
                };
                aaa.Add(model);
            }
            App.sampleTypeList = aaa;
            App.EmergencyAccidentID = item.id;
            App.EmergencyAccidengtModel = item;
            Navigation.PushAsync(new EmergencyAccidentInfoPage(item));
            listView.SelectedItem = null;

        }
   
        public EmergencyAccidentPage()
        {
            InitializeComponent();
            ReqEmergencyAccidentInfo(searchKey, "");
            NavigationPage.SetBackButtonTitle(this, "");

        }

        private async void ReqEmergencyAccidentInfo(String Filter, String Sorting)
        {
            string url = App.EmergencyModule.url + DetailUrl.GetEmergencyAccidentList +
                    "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=10" + "&SkipCount=" + dataList.Count; ;
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
                    ReqEmergencyAccidentInfo(searchKey, ""); //网络请求救援地点，10条每次
                }
            }
        }
    }
}
