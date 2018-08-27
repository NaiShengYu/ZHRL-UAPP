using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class TaskListPage : ContentPage
    {
        private int pageIndex;
        private bool hasMore = true;
        private string mSearchKey;
        private ObservableCollection<GridTaskModel> dataList = new ObservableCollection<GridTaskModel>();
        private TaskFilterCondition filterCondition;

        private const string SEARCH_MULTIPLE = "《复杂条件搜索》";
        private bool isSearchMultiple = false;

        public TaskListPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            filterCondition = new TaskFilterCondition();
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridTaskModel taskM = e.SelectedItem as GridTaskModel;
            if (taskM == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskInfoTypeTowPage());
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

        private async void SearchData()
        {
            pageIndex = 0;
            dataList.Clear();
            hasMore = true;
            if (App.gridUser == null)
                App.gridUser = await (App.Current as App).getStaffInfo();
            ReqGridTaskList();
        }

        private async void ReqGridTaskList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTasksByKey";
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("pageIndex", pageIndex + "");
            map.Add("pageSize", "10");
            if (isSearchMultiple)
            {
                map.Add("taskName", filterCondition.isKeyOn ? filterCondition.searchName : "");
                map.Add("state", filterCondition.isStatusOn ? filterCondition.status : "");
                map.Add("type", filterCondition.isTypeOn ? filterCondition.type : "");
                map.Add("gridName", filterCondition.isGriderOn ? filterCondition.griders : "");
                map.Add("strDate", filterCondition.isTimeOn ? TimeUtils.DateTime2YMD(filterCondition.dayStart) : "");
                map.Add("endDate", filterCondition.isTimeOn ? TimeUtils.DateTime2YMD(filterCondition.dayEnd) : "");
                map.Add("addr", filterCondition.isAddressOn ? filterCondition.address : "");
                map.Add("pointName", filterCondition.isWatcherOn ? filterCondition.watcher : "");
            }
            else
            {
                map.Add("taskName", mSearchKey);
            }
            string param = JsonConvert.SerializeObject(map);
            await DisplayAlert("param", param, "ok");
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST");
            if (res.StatusCode == HttpStatusCode.OK)
            {
                List<GridTaskModel> list = JsonConvert.DeserializeObject<List<GridTaskModel>>(res.Results);
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

            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridTaskModel item = e.Item as GridTaskModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore)
                {
                    ReqGridTaskList();
                }
            }
        }

        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarFilter_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TaskFilterPage(filterCondition));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(filterCondition.isKeyOn || filterCondition.isStatusOn || filterCondition.isTypeOn || filterCondition.isGriderOn
                || filterCondition.isTimeOn || filterCondition.isAddressOn || filterCondition.isWatcherOn)
            {
                isSearchMultiple = true;
                search.Text = SEARCH_MULTIPLE;
            }
            else
            {
                isSearchMultiple = false;
                search.Text = "";
            }
            SearchData();
            //DisplayAlert("condition", "key: " + filterCondition.SearchName + "  status:" + filterCondition.Status, "ok");
        }

        //任务筛选条件
        public class TaskFilterCondition : BaseModel
        {
            public string searchName;
            public string status;
            public string type { get; set; }
            public string griders { get; set; }
            public DateTime dayStart { get; set; }
            public TimeSpan timeStart { get; set; }
            public DateTime dayEnd { get; set; }
            public TimeSpan timeEnd { get; set; }
            public string address { get; set; }
            public string watcher { get; set; }

            public bool isKeyOn { get; set; }
            public bool isStatusOn { get; set; }
            public bool isTypeOn { get; set; }
            public bool isGriderOn { get; set; }
            public bool isTimeOn { get; set; }
            public bool isAddressOn { get; set; }
            public bool isWatcherOn { get; set; }
        }
    }
}
