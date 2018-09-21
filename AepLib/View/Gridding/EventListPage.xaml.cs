using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class EventListPage : ContentPage
    {
        private string mSearchKey;
        int pageIndex = 0;//请求页码
        bool haveMore = true;//返回是否还有
        private ObservableCollection<GridEventModel> dataList = new ObservableCollection<GridEventModel>();

        public EventListPage()
        {
            InitializeComponent();
            SearchData();
        }

        public void AddButtonClicked(Object sender, EventArgs e)
        {
            if (App.gridUser == null) return;
            Navigation.PushAsync(new RegistrationEventPage(""));
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridEventModel eventM = e.SelectedItem as GridEventModel;
            if (eventM == null)
            {
                return;
            }
            if (App.gridUser == null) return;
            if(eventM.direction == "received"){
                //乡级
                if (App.gridUser.gridLevel == App.GridMaxLevel - 1)
                {
                    var dsPage = new DisposeEventPage(eventM);
                    Navigation.PushAsync(dsPage);
                    listView.SelectedItem = null;

                }
                //县级
                if (App.gridUser.gridLevel == App.GridMaxLevel - 2)
                {
                    var dsPage = new DisposeEventTypeTowPage(eventM);
                    Navigation.PushAsync(dsPage);
                    listView.SelectedItem = null;
                }
            }
            if(eventM.direction == "issued") 
                Navigation.PushAsync(new RegistrationEventPage(eventM.id.ToString()));

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
            haveMore = true;
            if(App.gridUser == null)
            {
                App.gridUser = await (App.Current as App).getStaffInfo(App.userInfo.id);
                if (App.gridUser == null) {
                    addButGR.Height = 0;
                    return;
                }
            }
            ReqGridEventList();
        }


        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridEventModel item = e.Item as GridEventModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (haveMore && dataList.Count >= ConstantUtils.PAGE_SIZE)
                {
                    ReqGridEventList();
                }
            }
        }

        private async void ReqGridEventList(){

            string url = App.EP360Module.url+"/api/gbm/GetIncidentsByKey";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", ConstantUtils.PAGE_SIZE);
            map.Add("searchKey", mSearchKey);
            map.Add("grid", App.gridUser.grid);
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<GridEventModel> eventList = JsonConvert.DeserializeObject<List<GridEventModel>>(hTTPResponse.Results);
                    if(eventList != null && eventList.Count > 0)
                    {
                        int count = eventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            dataList.Add(eventList[i]);
                        }
                        pageIndex++;
                    }
                    else
                    {
                        haveMore = false;
                    }
                    
                    listView.ItemsSource = dataList;
                }
                catch (Exception ex)
                {

                }

               
            }

        }
        
    }
}
