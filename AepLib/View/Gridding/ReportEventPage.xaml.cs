using AepApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class ReportEventPage : ContentPage
    {
        private int totalNum;
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
            Navigation.PushAsync(new DisposeEventTypeTowPage());
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
                _event.Address = "李家村";
                _event.taskList = new ObservableCollection<GridTaskModel>();
                for (int j = 0; j < 8; j++)
                {
                    GridTaskModel taskM = new GridTaskModel
                    {
                        name = j + "调度事件",
                        addTime = "2018-12-11 09:10",
                        taskStatus = (j % 3).ToString(),
                    };
                    _event.taskList.Add(taskM);
                }
                if (i % 3 == 0)
                {
                    _event.EventStatus = "0";
                }
                else if (i % 3 == 1)
                {
                    _event.EventStatus = "1";
                }
                else if (i % 3 == 2)
                {
                    _event.EventStatus = "2";
                }
                if (i % 2 == 0)
                {
                    _event.EventType = "0";
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
