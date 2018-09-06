using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class ExecutionRecordPage : ContentPage
    {
        private int pageIndex;
        private bool hasMore = true;
        private int totalNum;
        private string mSearchKey;
        private ObservableCollection<GridTaskHandleRecordModel> dataList = new ObservableCollection<GridTaskHandleRecordModel>();
        private string mTaskId;
        private string mStaff;
        public ExecutionRecordPage(string taskId,string staff)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            mTaskId = taskId;
            mStaff = staff;
            SearchData();
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridTaskHandleRecordModel record = e.SelectedItem as GridTaskHandleRecordModel;
            if (record == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskResultPage(record.id.Value, record, false));
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
            ReqGridTaskList();
        }

        private async void ReqGridTaskList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskHandleList";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("staff",mStaff);
            map.Add("task", mTaskId);

            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<GridTaskHandleRecordModel> list = JsonConvert.DeserializeObject<List<GridTaskHandleRecordModel>>(res.Results);
                    if (list != null && list.Count > 0)
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
                catch (Exception e)
                {

                }

            }

            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridTaskHandleRecordModel item = e.Item as GridTaskHandleRecordModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore && dataList.Count >= ConstantUtils.PAGE_SIZE)
                {
                    ReqGridTaskList();
                }
            }
        }
    }
}
