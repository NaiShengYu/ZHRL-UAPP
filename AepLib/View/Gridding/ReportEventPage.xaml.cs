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
            string url = App.EP360Module.url + "/api/gbm/GetIncidentsByKey";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", 20);
            map.Add("searchKey", mSearchKey);
            map.Add("grid", App.gridUser.gridcell);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
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
                catch (Exception e)
                {

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
