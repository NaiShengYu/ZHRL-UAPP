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
            Navigation.PushAsync(new MonitorPage(enterModel));
            listView.SelectedItem = null;
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var search = sender as SearchBar;
            Console.WriteLine(e.NewTextValue);
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
        }


        private void ReqPollutionSiteData()
        {
            //api/FactorData/GetLastAQIValsForPhone
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                CrossHud.Current.Show("请求中...");
                string uri = App.BaseUrl + "/api/AppEnterprise/GetEnterpriseList?keys=";
                result = EasyWebRequest.sendGetHttpWebRequest(uri);

                try{
                    res = JsonConvert.DeserializeObject<resutlDic>(result);

                }catch (Exception ex){
                    Console.WriteLine("解析错误：" +ex);
                }



                Console.WriteLine(res);
                res.Items.Sort(delegate (EnterpriseModel x, EnterpriseModel y)
                {
                    return y.count.CompareTo(x.count);
                });
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                CrossHud.Current.Dismiss();
                getCurrentData("");
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