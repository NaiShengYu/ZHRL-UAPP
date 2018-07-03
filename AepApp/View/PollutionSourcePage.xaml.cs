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
            getCurrentData(e.NewTextValue);
        }

        private string result;
        resutlDic res = null;
        ObservableCollection<EnterpriseModel> dataList = new ObservableCollection<EnterpriseModel>();
       
        public PollutionSourcePage()
        {
            InitializeComponent();
            this.Title = "污染源在线";
            ReqPollutionSiteData();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                Navigation.PushAsync(new PollutionSourceMapPage(dataList));

            }));
        }

        private void ReqPollutionSiteData()
        {
            //api/FactorData/GetLastAQIValsForPhone
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                //CrossHud.Current.Show("请求中...");
                string uri = App.BaseUrl + "/api/AppEnterprise/GetEnterpriseList?keys=";
                result = EasyWebRequest.sendGetHttpWebRequest(uri);
                res = JsonConvert.DeserializeObject<resutlDic>(result);
                res.Items.Sort(delegate (EnterpriseModel x, EnterpriseModel y)
                {
                    return y.count.CompareTo(x.count);
                });
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                CrossHud.Current.Dismiss();
                try{
                    getCurrentData("");
                }
                catch{
                    
                }
                listView.ItemsSource = dataList;

            };
            wrk.RunWorkerAsync();
        }

        private void getCurrentData(String value)
        {
            dataList.Clear();
            for (int i = 0; i < res.Items.Count; i++)
            {
                EnterpriseModel item = res.Items[i];

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