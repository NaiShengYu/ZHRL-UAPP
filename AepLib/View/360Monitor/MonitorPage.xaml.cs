using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
using System.Reflection;
using Newtonsoft.Json;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif
namespace AepApp.View.Monitor
{
    public partial class MonitorPage : ContentPage
    {
        //项目审批
        void Handle_Clicked1(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProjectApprovalOnePage(_ent));        
        }
        //排污许可证管理
        void Handle_Clicked2(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new EmissionPermitManagementPage(_ent));        
        }
        //信访管理
        void Handle_Clicked3(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new LettersAndVisitsPage(_ent));        
        }
        //日常监管
        void Handle_Clicked4(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new DailyRegulationPage(_ent));        
        }
        //电子处罚
        void Handle_Clicked5(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ElectroniPcunishmentPage(_ent));        
        }
        //监督检查
        void Handle_Clicked6(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SupervisionAndInspectionPage(_ent));        
        }

        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }

        List<Type> types = new List<Type> { typeof(ProjectApprovalOnePage), typeof(EmissionPermitManagementPage), typeof(LettersAndVisitsPage), typeof(DailyRegulationPage), typeof(ElectroniPcunishmentPage), typeof(SupervisionAndInspectionPage) };

        //void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        //{
        //    int i = labels.IndexOf(e.SelectedItem as string);
        //    if (i < 0) return;
        //    ConstructorInfo ctor=types[i].GetConstructor(new[] { typeof(EnterpriseModel) });
        //    object inst = ctor.Invoke(new object[] { _ent });
        //    Navigation.PushAsync(inst as ContentPage);
        //}
        public MonitorPage(EnterpriseModel ent)
        {
            InitializeComponent();
            _ent = ent;
            this.BindingContext = Enterprise;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字



            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) => {
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) => {
                //this.BindingContext = model;
                stac.BindingContext = model;
            };
            wrk.RunWorkerAsync();

        }

        MonitorModel model = null;
        void makeData()
        {
            try
            {
                string url = App.environmentalQualityModel.url + "/api/AppEnterprise/GetUndoneProject?id=" + _ent.id ;
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                model = JsonConvert.DeserializeObject<MonitorModel>(result);


            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 3));
            }
        }



    }
}
