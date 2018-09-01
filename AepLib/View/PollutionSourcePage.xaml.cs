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
        private string mSearchKey;

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
        ObservableCollection<EnterpriseModel> dataList = new ObservableCollection<EnterpriseModel>();

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
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(uri, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dataList.Clear();
                try
                {
                    List<EnterpriseModel> list = JsonConvert.DeserializeObject<List<EnterpriseModel>>(res.Results);
                    if (list != null)
                    {

                        foreach (var item in list)
                        {
                            dataList.Add(item);
                        }
                    }
                    GetPollutionSiteExtraData();
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
            if (dataList.Count == 0)
            {
                return;
            }
            ObservableCollection<EnterpriseModel> enterprises = new ObservableCollection<EnterpriseModel>();
            string uri = App.EP360Module.url + "/api/AppEnterprise/GetEnterpriseByArrid";
            Dictionary<string, object> map = new Dictionary<string, object>();
            List<Guid> ids = new List<Guid>();
            foreach (var item in dataList)
            {
                if (item.id != null)
                    ids.Add(item.id);
            }
            map.Add("items", ids);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(uri, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<EnterpriseModel> extraList = JsonConvert.DeserializeObject<List<EnterpriseModel>>(res.Results);
                    if (extraList != null)
                    {
                        extraList.Sort(delegate (EnterpriseModel x, EnterpriseModel y)
                        {
                            return y.count.CompareTo(x.count);
                        });
                        foreach (var extra in extraList)
                        {
                            foreach (var d in dataList)
                            {
                                if (d != null && extra != null && (d.id == extra.id))
                                {
                                    extra.name = d.name;
                                    extra.lat = d.lat;
                                    extra.lng = d.lng;
                                    continue;
                                }
                            }
                            enterprises.Add(extra);
                        }
                    }
                }
                catch (Exception e)
                {
                }
                listView.ItemsSource = enterprises;
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
                string uri = App.BaseUrl + "/api/AppEnterprise/GetEnterpriseList?keys=";
                result = EasyWebRequest.sendGetHttpWebRequest(uri);
                pollutions = JsonConvert.DeserializeObject<resutlDic>(result);
                pollutions.Items.Sort(delegate (EnterpriseModel x, EnterpriseModel y)
                {
                    return y.count.CompareTo(x.count);
                });
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
    }
}