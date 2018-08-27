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
            Navigation.PushAsync(new RegistrationEventPage(null));
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridEventModel eventM = e.SelectedItem as GridEventModel;
            if (eventM == null)
            {
                return;
            }
            Navigation.PushAsync(new RegistrationEventPage(eventM));
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
                ReqGridEventList();


            }else{
                ReqGridEventList();
            }

        }

        //private void ReqGridEventList()
        //{
        //    for (int i = 0; i < 20; i++)
        //    {
        //        GridEventModel _event = new GridEventModel();
        //        _event.Name = i + "化工偷排事件";
        //        _event.Time = "2018-8-13";
        //        _event.addTime = "2017-12-13 09:10";
        //        _event.taskList = new ObservableCollection<GridTaskModel>();
        //        if (i % 3 == 0)
        //        {
        //            _event.EventStatus = "0";
        //            _event.lng = "121.659705";
        //            _event.lat = "29.884929";
        //            _event.townHandleTime = "2018-02-21 12:00";
        //        }
        //        else if (i % 3 == 1)//已完成
        //        {
        //            _event.EventStatus = "1";
        //            _event.lng = "121.564464";
        //            _event.lat = "29.799935";
        //            _event.townHandleTime = "2018-02-21 12:00";
        //            _event.countryHandleTime = "2018-06-30 15:00";
        //            _event.finishTime = "2018-08-01 15:30";
        //        }
        //        else if (i % 3 == 2)//处理中
        //        {
        //            _event.EventStatus = "2";
        //            _event.lng = "121.351159";
        //            _event.lat = "29.384102";
        //            _event.townHandleTime = "2018-02-21 12:00";
        //            _event.countryHandleTime = "2018-06-30 15:00";
        //        }
        //        if (i % 2 == 0)
        //        {
        //            _event.EventType = "0";
        //            for (int j = 0; j < 8; j++)
        //            {
        //                GridTaskModel taskM = new GridTaskModel
        //                {
        //                    name = j + "调度事件",
        //                    addTime = "2017-12-11 09:10",
        //                    taskStatus = (j % 3).ToString(),
        //                };
        //                _event.taskList.Add(taskM);
        //            }
        //        }
        //        else
        //        {
        //            _event.EventType = "1";
        //        }
        //        dataList.Add(_event);
        //    }
        //    listView.ItemsSource = dataList;
        //}

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

            string url = App.EmergencyModule.url+"/api/gbm/GetIncidentsByKey";
            ChemicalStruct parameter = new ChemicalStruct
            {
                searchKey = mSearchKey,
                pageSize = 20,
                pageIndex = 0,
                gridId ="0",
                
            };
            string param = JsonConvert.SerializeObject(parameter);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //ExpertLibraryModels.SpecialBean specialBean = new ExpertLibraryModels.SpecialBean();
                //specialBean = JsonConvert.DeserializeObject<ExpertLibraryModels.SpecialBean>(hTTPResponse.Results);
                //totalNum = specialBean.result.professionals.totalCount;
                //List<ExpertLibraryModels.ItemsBean> list = specialBean.result.professionals.items;
                //int count = list.Count;
                //for (int i = 0; i < count; i++)
                //{
                //    dataList.Add(list[i]);
                //}
                //listView.ItemsSource = dataList;
            }

        }

        internal class ChemicalStruct
        {
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
            public string searchKey { get; set; }
            public string gridId { get; set; }

        }



    }
}
