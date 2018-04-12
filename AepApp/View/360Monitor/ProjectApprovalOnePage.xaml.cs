using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

using AepApp.Models;
namespace AepApp.View.Monitor
{
    public partial class ProjectApprovalOnePage : ContentPage
    {
        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
        
            ProjectApproval item = e.Item as ProjectApproval;
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

        EnterpriseModel _preiseModel = null;//企业模型
        int _page = 1;//当前页数
        bool _haveMore = true;//判断是否有更多的数据
        ObservableCollection<ProjectApproval> dataList = new ObservableCollection<ProjectApproval>();


        public ProjectApprovalOnePage(EnterpriseModel enterpriseModel)
        {
            _preiseModel = enterpriseModel;
            this.Title = "项目审批";

            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) => {
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) => {
                //listV.ItemsSource = dataList;
                Console.WriteLine("解析结果："+ list);
            };
            wrk.RunWorkerAsync();
        }

        titleName list = null;
        void makeData()
        {
            try
            {
                string url = App.BaseUrl+"/api/AppEnterprise/GetApprovalList?id="+_preiseModel.id+"&pageindx="+_page+"&pageSize=10";
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。

                list = JsonConvert.DeserializeObject<titleName>(result);

                if (_page == 1)
                    dataList.Clear();
                for (int i = 0; i < list.items.Count;i ++){
                    ProjectApproval item = list.items[i];
                    dataList.Add(item);
                }

                if (int.Parse(list.count) <= dataList.Count)
                    _haveMore = false;
                else
                    _haveMore = true;

            }
            catch (Exception ex)
            {
                
                DisplayAlert("Alert", ex.Message, "OK");
            }
        }

        internal class titleName
        {
            public string count { get; set; }
            public List<ProjectApproval> items { get; set; }
            public string ncount { get; set; }
        }
    }
}
