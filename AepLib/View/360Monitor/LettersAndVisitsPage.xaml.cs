using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
using Newtonsoft.Json;
using Sample;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif
namespace AepApp.View.Monitor
{
    public partial class LettersAndVisitsPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }


        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var selectItem = e.SelectedItem as LettersAndVisits;
            if (selectItem == null)
                return;
            Navigation.PushAsync(new LettersAndVisitsInfoPage(selectItem, _ent));
            listV.SelectedItem = null;

        }


        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {

            LettersAndVisits item = e.Item as LettersAndVisits;
            if (item == dataList[dataList.Count - 1] && _haveMore == true && item != null)
            {
                _page += 1;
                BackgroundWorker wrk = new BackgroundWorker();
                wrk.DoWork += (a, ee) => {
                    makeData();
                };
                wrk.RunWorkerAsync();
            }

        }

        int _page = 0;//当前页数
        bool _haveMore = true;//判断是否有更多的数据
        ObservableCollection<LettersAndVisits> dataList = new ObservableCollection<LettersAndVisits>();


        public LettersAndVisitsPage(EnterpriseModel ent)
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
                if (dataList == null || dataList.Count == 0) DependencyService.Get<IToast>().ShortAlert("无数据");

            };
            wrk.RunWorkerAsync();
        }

        aaaa list = null;
        void makeData()
        {
            try
            {
                string url = App.EP360Module.url + "/api/AppEnterprise/GetPetitionList?id=" + _ent.id + "&pageindx=" + _page + "&pageSize=10";
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                list = JsonConvert.DeserializeObject<aaaa>(result);
                CrossHud.Current.Dismiss();

                if (_page == 1)
                    dataList.Clear();
                for (int i = 0; i < list.items.Count; i++)
                {
                    LettersAndVisits item = list.items[i];
                    dataList.Add(item);
                }

                if (list.count <= (float)dataList.Count)
                    _haveMore = false;
                else
                    _haveMore = true;

            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 3));
            }

        }

        public class aaaa
        {
            public float count { get; set; }
            public List<LettersAndVisits> items { get; set; }
            public float ncount { get; set; }
        }

    }
}
