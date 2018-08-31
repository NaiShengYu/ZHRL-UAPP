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
    public partial class TaskExamineStaffPage : ContentPage
    {

        TaskExamineStaffModel model;
        public TaskExamineStaffPage(Guid grid)
        {
            InitializeComponent();
            GetAssessmentInfo(grid);
        }

        private async void GetAssessmentInfo(Guid grid)
        {
            string url = App.EP360Module.url + "/api/gbm/GetGridAssessmentDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("grid", grid);
            map.Add("year", DateTime.Now.Year);
            map.Add("month", DateTime.Now.Month);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    model = JsonConvert.DeserializeObject<TaskExamineStaffModel>(res.Results);
                    BindingContext = model;
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}