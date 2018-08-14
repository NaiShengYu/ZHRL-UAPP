using AepApp.Models;
using Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class EventListPage : ContentPage
    {
        private int totalNum;
        private string mSearchKey;
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

        private void SearchData()
        {
            dataList.Clear();
            ReqGridEventList();
        }

        private void ReqGridEventList()
        {
            for (int i = 0; i < 20; i++)
            {
                GridEventModel _event = new GridEventModel();
                _event.Name = i + "化工偷排事件";
                _event.Time = "2018-8-13";
                _event.addTime = "2017-12-13 09:10";
                _event.taskList = new ObservableCollection<GridTaskModel>();
                if (i % 3 == 0)
                {
                    _event.EventStatus = "0";
                    _event.lng = "121.659705";
                    _event.lat = "29.884929";
                    _event.townHandleTime = "2018-02-21 12:00";
                }
                else if (i % 3 == 1)//已完成
                {
                    _event.EventStatus = "1";
                    _event.lng = "121.564464";
                    _event.lat = "29.799935";
                    _event.townHandleTime = "2018-02-21 12:00";
                    _event.countryHandleTime = "2018-06-30 15:00";
                    _event.finishTime = "2018-08-01 15:30";
                }
                else if (i % 3 == 2)//处理中
                {
                    _event.EventStatus = "2";
                    _event.lng = "121.351159";
                    _event.lat = "29.384102";
                    _event.townHandleTime = "2018-02-21 12:00";
                    _event.countryHandleTime = "2018-06-30 15:00";
                }
                if (i % 2 == 0)
                {
                    _event.EventType = "0";
                    for (int j = 0; j < 8; j++)
                    {
                        GridTaskModel taskM = new GridTaskModel
                        {
                            name = j + "调度事件",
                            addTime = "2017-12-11 09:10",
                            taskStatus = (j % 3).ToString(),
                        };
                        _event.taskList.Add(taskM);
                    }
                }
                else
                {
                    _event.EventType = "1";
                }
                dataList.Add(_event);
            }
            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridEventModel item = e.Item as GridEventModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < totalNum)
                {
                    ReqGridEventList();
                }
            }
        }
    }
}
