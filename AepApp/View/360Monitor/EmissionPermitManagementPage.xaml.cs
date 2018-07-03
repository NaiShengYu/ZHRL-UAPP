using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace AepApp.View.Monitor
{
    public partial class EmissionPermitManagementPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }


        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var selectItem = e.SelectedItem as EmissionPermitManagement.EmissionPermitManagementList;
            if (selectItem == null)
                return;
            Navigation.PushAsync(new EmissionPermitManagementInfoPage(_ent));
            listV.SelectedItem = null;
        }

        int _page = 1;//当前页数
        bool _haveMore = true;//判断是否有更多的数据
        ObservableCollection<EmissionPermitManagement.EmissionPermitManagementList> dataList = new ObservableCollection<EmissionPermitManagement.EmissionPermitManagementList>();


        public EmissionPermitManagementPage(EnterpriseModel ent)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
            _ent = ent;
            this.BindingContext = Enterprise;

            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) => {
                //CrossHud.Current.Show("请求中...");
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) => {
                listV.ItemsSource = dataList;
            };
            wrk.RunWorkerAsync();

        }

        void makeData()
        {
            try
            {
                string url = App.BaseUrl + "/api/AppEnterprise/GetPolluteMessageList?id=" + _ent.id + "&pageindx=1&pageSize=10";
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                dataList = JsonConvert.DeserializeObject<ObservableCollection<EmissionPermitManagement.EmissionPermitManagementList>>(result);
                CrossHud.Current.Dismiss();
            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 5));
            }
        }

    }
}
