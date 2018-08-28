using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class ReportEventPage : ContentPage
    {
        private int pageIndex;
        bool haveMore = true;
        private string mSearchKey;
        private ObservableCollection<GridEventModel> dataList = new ObservableCollection<GridEventModel>();

        public ReportEventPage()
        {
            InitializeComponent();

            SearchData();
        }


        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridEventModel eventM = e.SelectedItem as GridEventModel;
            if (eventM == null)
            {
                return;
            }

            Navigation.PushAsync(new DisposeEventTypeTowPage(eventM.incident));
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
            haveMore = true;
            dataList.Clear();
            if (App.gridUser == null)
            {
                App.gridUser = await(App.Current as App).getStaffInfo();
                if (App.gridUser == null) return;
            }
            ReqGridEventList();
        }

        private async void ReqGridEventList()
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    GridEventModel _event = new GridEventModel();
            //    _event.Name = i + "化工偷排事件";
            //    _event.Time = "2018-8-13";
            //    _event.Address = "李家村";
            //    _event.taskList = new ObservableCollection<GridTaskModel>();
            //    for (int j = 0; j < 8; j++)
            //    {
            //        GridTaskModel taskM = new GridTaskModel
            //        {
            //            name = j + "调度事件",
            //            addTime = "2018-12-11 09:10",
            //            taskStatus = (j % 3).ToString(),
            //        };
            //        _event.taskList.Add(taskM);
            //    }
            //    if (i % 3 == 0)
            //    {
            //        _event.EventStatus = "0";
            //    }
            //    else if (i % 3 == 1)
            //    {
            //        _event.EventStatus = "1";
            //    }
            //    else if (i % 3 == 2)
            //    {
            //        _event.EventStatus = "2";
            //    }
            //    if (i % 2 == 0)
            //    {
            //        _event.EventType = "0";
            //    }
            //    else
            //    {
            //        _event.EventType = "1";
            //    }
            //    dataList.Add(_event);
            //}

            string url = App.EP360Module.url + "/api/gbm/GetIncidentsByKey";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", "20");
            map.Add("searchKey", mSearchKey);
            map.Add("grid", App.gridUser.gridcell);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST");
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<GridEventModel> list = JsonConvert.DeserializeObject<List<GridEventModel>>(res.Results);
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
                    haveMore = false;
                }
            }

            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridEventModel item = e.Item as GridEventModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (haveMore)
                {
                    ReqGridEventList();
                }
            }
        }
    }
}
