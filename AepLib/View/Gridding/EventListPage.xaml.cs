using AepApp.Models;
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
        int page = 0;//请求页码
        bool haveMore = true;//返回是否还有
        private ObservableCollection<GridEventModel> dataList = new ObservableCollection<GridEventModel>();

        public EventListPage()
        {
            InitializeComponent();
            SearchData();
        }

        public void AddButtonClicked(Object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegistrationEventPage(""));
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridEventModel eventM = e.SelectedItem as GridEventModel;
            if (eventM == null)
            {
                return;
            }
            Navigation.PushAsync(new RegistrationEventPage(""));
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
            page = 0;
            dataList.Clear();
            haveMore = true;
            if(App.gridUser ==null){
                App.gridUser = await (App.Current as App).getStaffInfo();
                if (App.gridUser == null) return;

                ReqGridEventList();
            }else{
                ReqGridEventList();
            }

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

        private async void ReqGridEventList(){

            string url = App.EP360Module.url+"/api/gbm/GetIncidentsByKey";
            ChemicalStruct parameter = new ChemicalStruct
            {
                searchKey = mSearchKey,
                pageSize = 20,
                pageIndex = 0,
                //gridId = App.gridUser.gridcell,
                gridId = Guid.Parse("08429856-a5fe-4861-87ae-1a0b247c94bc"),
            };
            string param = JsonConvert.SerializeObject(parameter);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<GridEventModel> eventList = JsonConvert.DeserializeObject<List<GridEventModel>>(hTTPResponse.Results);
                    //totalNum = specialBean.result.professionals.totalCount;
                    //List<ExpertLibraryModels.ItemsBean> list = specialBean.result.professionals.items;
                    int count = eventList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        dataList.Add(eventList[i]);
                    }
                    listView.ItemsSource = dataList;
                }
                catch (Exception ex)
                {

                }

               
            }

        }

        internal class ChemicalStruct
        {
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
            public string searchKey { get; set; }
            public Guid gridId { get; set; }
        }

    }
}
