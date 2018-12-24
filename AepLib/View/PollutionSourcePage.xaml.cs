using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AepApp.View.Monitor;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PollutionSourcePage : ContentPage
    {
        private int pageIndex = 0;
        private int totalCount;
        private string mSearchKey;
        ObservableCollection<EnterpriseModel> idsList = new ObservableCollection<EnterpriseModel>();
        ObservableCollection<EnterpriseModel> dataList = new ObservableCollection<EnterpriseModel>();


        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var enterModel = e.SelectedItem as EnterpriseModel;
            if (enterModel == null)
                return;
            Navigation.PushAsync(new PollutionSourceInfoPage(enterModel));
            listView.SelectedItem = null;
        }

        //搜索框内容变化监控
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            mSearchKey = e.NewTextValue;
            //getCurrentData(e.NewTextValue);
            GetData();
        }

        private string result;
        resutlDic pollutions = null;

        public PollutionSourcePage()
        {
            InitializeComponent();
            this.Title = "污染源在线";
            //ReqPollutionSiteData();
            GetData();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                Navigation.PushAsync(new PollutionSourceMapPage(dataList));

            }));
        }

        private void GetData()
        {
            pageIndex = 0;
            GetPollutionSiteData();
        }

        /// <summary>
        /// 获取企业主要信息
        /// </summary>
        private async void GetPollutionSiteData()
        {
            string uri = App.BasicDataModule.url + "/api/mod/GetAllEnterprise";
            //string uri = "http://dev.azuratech.com:50001/api/mod/GetAllEnterprise";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("keys", mSearchKey);
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", 10);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(uri, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (pageIndex == 0)
                {
                    dataList.Clear();
                }
                idsList.Clear();
                try
                {
                    pollutions = JsonConvert.DeserializeObject<resutlDic>(res.Results);
                    if (pollutions != null)
                    {
                        totalCount = pollutions.count;
                        List<EnterpriseModel> list = pollutions.Items;
                        if (list != null)
                        {

                            foreach (var item in list)
                            {
                                dataList.Add(item);
                                idsList.Add(item);
                            }
                            pageIndex++;
                        }
                        GetPollutionSiteExtraData();
                    }

                }
                catch (Exception e)
                {
                }
            }

        }

        /// <summary>
        /// 获取企业的额外信息，如：count、stvalue等
        /// </summary>
        private async void GetPollutionSiteExtraData()
        {
            if (idsList.Count == 0)
            {
                return;
            }
            string uri = App.EP360Module.url + "/api/AppEnterprise/GetEnterpriseByArrid";
            Dictionary<string, object> map = new Dictionary<string, object>();
            List<Guid> ids = new List<Guid>();
            foreach (var item in idsList)
            {
                if (item.id != null && item.id != Guid.Empty)
                    ids.Add(item.id);
            }
            map.Add("items", ids);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(uri, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<EnterpriseModel> extraList = JsonConvert.DeserializeObject<List<EnterpriseModel>>(res.Results);
                    if (extraList != null && extraList.Count > 0)
                    {
                        foreach (var extra in extraList)
                        {
                            foreach (var d in dataList)
                            {
                                if (d != null && extra != null && (d.id == extra.id))
                                {
                                    d.count = extra.count;
                                    d.stvalue = extra.stvalue;
                                    d.time = extra.time;
                                    d.value = extra.value;
                                    continue;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
                dataList.OrderByDescending(item => item.count);
                listView.ItemsSource = dataList;
            }
        }


        //旧的数据请求
        private void ReqPollutionSiteData()
        {
            //api/FactorData/GetLastAQIValsForPhone
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                //CrossHud.Current.Show("请求中...");
                string uri = App.environmentalQualityModel.url + "/api/AppEnterprise/GetEnterpriseList?keys=";
                result = EasyWebRequest.sendGetHttpWebRequest(uri);
                pollutions = JsonConvert.DeserializeObject<resutlDic>(result);
                //pollutions.Items.Sort(delegate (EnterpriseModel x, EnterpriseModel y)
                //{
                //    return y.count.CompareTo(x.count);
                //});
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                CrossHud.Current.Dismiss();
                try
                {
                    getCurrentData("");
                }
                catch
                {

                }
                listView.ItemsSource = dataList;

            };
            wrk.RunWorkerAsync();
        }

        private void getCurrentData(String value)
        {
            dataList.Clear();
            for (int i = 0; i < pollutions.Items.Count; i++)
            {
                EnterpriseModel item = pollutions.Items[i];

                if (item.name.Contains(value))//数据匹配
                {
                    dataList.Add(item);
                }
            }
        }

        internal class resutlDic
        {
            public int count { get; set; }
            public List<EnterpriseModel> Items { get; set; }
        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            EnterpriseModel item = e.Item as EnterpriseModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (totalCount > dataList.Count)
                {
                    GetPollutionSiteData();
                }
            }
        }
    }
}