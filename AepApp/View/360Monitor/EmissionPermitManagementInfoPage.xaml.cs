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
    public partial class EmissionPermitManagementInfoPage : ContentPage
    {
        EmissionPermitManagement.EmissionPermitManagementInfo info =null;
        EnterpriseModel _preiseModel = null;//企业模型
        public EmissionPermitManagementInfoPage(EnterpriseModel enterpriseModel)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
            _preiseModel = enterpriseModel;
            this.Title = "排污许可证";

            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) => {
                CrossHud.Current.Show("请求中...");
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) => {
                try
                {
                    first.Text = "单位名称：" + info.name + '\n' + '\n' + "注册地址：" + info.registerAdd + '\n' + '\n' + "法人代表：" + info.legal + '\n' + '\n' + "生产经营场所地址：" + info.address + '\n' + '\n' + "行业类别：" + info.industry + '\n' + '\n' + "统一信用代码：" + info.code + '\n' + '\n' + "有效日期：" + info.startdate.Substring(0, 10) + "至" + info.enddate.Substring(0, 10);

                    second.Text = "发证机关：" + info.issuing + '\n' + '\n' + "发证日期：" + info.issuedate.Substring(0,10);
                }catch (Exception ex){
                    
                }

            };
            wrk.RunWorkerAsync();

        }

        void makeData()
        {
            try
            {
                string url = App.BaseUrl + "/api/AppEnterprise/GetPollutePermit?id=" + _preiseModel.id + "&pageindx=1&pageSize=10";
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                info = JsonConvert.DeserializeObject<EmissionPermitManagement.EmissionPermitManagementInfo>(result);
                CrossHud.Current.Dismiss();
            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 5));
            }
        }

    }
}
