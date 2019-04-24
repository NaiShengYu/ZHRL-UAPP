using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
using System.Text;
using Newtonsoft.Json;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif
namespace AepApp.View.Monitor
{
    public partial class EmissionPermitManagementInfoPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }

        EmissionPermitManagement.EmissionPermitManagementInfo info = null;
        public EmissionPermitManagementInfoPage(EnterpriseModel ent)
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
                try
                {
                    sv.BindingContext = info;
                }
                catch (Exception ex)
                {

                }

            };
            wrk.RunWorkerAsync();

        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        void makeData()
        {
            try
            {
                string url = App.EP360Module.url + "/api/AppEnterprise/GetPollutePermit?id=" + _ent.id + "&pageindx=0&pageSize=20";
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                info = Tools.JsonUtils.DeserializeObject<EmissionPermitManagement.EmissionPermitManagementInfo>(result);
                if (info != null)
                {
                    info.credit_no = RemoveSpecialCharacters(info.credit_no);
                    info.code = RemoveSpecialCharacters(info.code);
                }
                CrossHud.Current.Dismiss();
            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 5));
            }
        }

    }
}
