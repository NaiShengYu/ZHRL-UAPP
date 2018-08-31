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
    public partial class TaskExaminePage : ContentPage
    {

        TaskExamineModel model;
        public TaskExaminePage(Guid grid)
        {
            InitializeComponent();
            GetAssessmentInfo(grid);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            TaskExamineModel taskExamine = e.SelectedItem as TaskExamineModel;
            if (taskExamine == null)
            {
                return;
            }
            if(taskExamine.gridLevel == App.GridMaxLevel)
            {
                Navigation.PushAsync(new TaskExamineStaffPage(taskExamine.grid));
            }
            else
            {
                Navigation.PushAsync(new TaskExaminePage(taskExamine.grid));
            }
            ListView.SelectedItem = null;
        }

        private async void GetAssessmentInfo(Guid grid)
        {
            string url = App.EP360Module.url + "/api/gbm/GetGridAssessment";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("grid", grid);
            map.Add("year", DateTime.Now.Year);
            map.Add("month", DateTime.Now.Month);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    model = JsonConvert.DeserializeObject<TaskExamineModel>(res.Results);
                    BindingContext = model;
                    if (model != null)
                    {
                        ListView.ItemsSource = model.children;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

    }
}