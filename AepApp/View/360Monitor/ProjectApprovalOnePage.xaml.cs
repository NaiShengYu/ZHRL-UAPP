using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif
namespace AepApp.View.Monitor
{
    public partial class ProjectApprovalOnePage : ContentPage
    {
        public ProjectApprovalOnePage()
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) => {
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) => {
                this.BindingContext = list;

            };
            wrk.RunWorkerAsync();

        }

        titleName list = null;
        void makeData()
        {
            try
            {
                string url = "https://192.168.2.97/api/AppEnterprise/GetApprovalList?id=67be6548-40fe-4b59-b5ca-655ec3f35095&pageindx=1&pageSize=10";
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);

                list = JsonConvert.DeserializeObject<titleName>(result);

            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
            }
        }

        internal class titleName
        {
            public string count { get; set; }
            //public ProjectApproval items { get; set; }
            public string ncount { get; set; }
        }


    }
}
