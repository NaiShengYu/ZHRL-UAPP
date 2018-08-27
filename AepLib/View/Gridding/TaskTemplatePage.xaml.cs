using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskTemplatePage : ContentPage
    {
        private bool hasMore = true;
        private int pageIndex;
        private string mSearchKey;
        private ObservableCollection<TaskTemplateModel> dataList = new ObservableCollection<TaskTemplateModel>();

        public TaskTemplatePage()
        {
            InitializeComponent();
            SearchData();
        }


        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            TaskTemplateModel v = e.SelectedItem as TaskTemplateModel;
            if (v == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskTemplateInfoPage(v.tasktemplate));
            listView.SelectedItem = null;
        }

        public void Handle_TextChanged(Object sender, TextChangedEventArgs e)
        {
            mSearchKey = e.NewTextValue;
            SearchData();
        }

        public void Handle_Search(Object sender, EventArgs e)
        {
            SearchData();
        }

        private void SearchData()
        {
            pageIndex = 0;
            hasMore = true;
            dataList.Clear();
            ReqTaskTemplateList();
        }

        private async void ReqTaskTemplateList()
        {
            //List<string> list = new List<string>();
            //list.Add("现场检查任务");
            //list.Add("日常检查任务");
            //list.Add("现场检查任务（2018修订）");

            string url = App.EP360Module.url + "/api/gbm/GetTemplateByKey";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", "10");
            map.Add("searchKey", mSearchKey);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST");
            if(res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<TaskTemplateModel> list = JsonConvert.DeserializeObject<List<TaskTemplateModel>>(res.Results);
                if(list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        dataList.Add(item);
                    }
                    pageIndex++;
                }
                else
                {
                    hasMore = false;
                }
            }
            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            TaskTemplateModel item = e.Item as TaskTemplateModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore)
                {
                    ReqTaskTemplateList();
                }
            }
        }

    }
}