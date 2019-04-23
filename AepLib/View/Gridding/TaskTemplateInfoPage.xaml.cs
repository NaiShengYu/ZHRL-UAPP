using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskTemplateInfoPage : ContentPage
    {
        private string templateId;
        public TaskTemplateInfoPage(string id)
        {
            InitializeComponent();
            templateId = id;
            GetTemplateDetail();
        }

        private async void GetTemplateDetail()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTemplateDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", templateId);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    TaskTemplateModel data = Tools.JsonUtils.DeserializeObject<TaskTemplateModel>(res.Results);
                    BindingContext = data;
                    SetWebviewInfo(data);
                }
                catch (Exception e)
                {

                }
            }
        }

        private void SetWebviewInfo(TaskTemplateModel data)
        {
            if (data == null)
            {
                return;
            }
            var htmSource = new HtmlWebViewSource();
            var htmSourceReply = new HtmlWebViewSource();
            htmSource.Html = @data.contents;
            htmSourceReply.Html = @data.replycontents;
            contentWebview.Source = htmSource;
            reportWebview.Source = htmSourceReply;
        }

    }
}