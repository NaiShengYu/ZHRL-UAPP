using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class SendInformationPage : ContentPage
    {
        public SendInformationPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            SearchData();
        }

        private int totalNum;
        private string mSearchKey;
        private ObservableCollection<GridTaskModel> dataList = new ObservableCollection<GridTaskModel>();

     

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridTaskModel taskM = e.SelectedItem as GridTaskModel;
            if (taskM == null)
            {
                return;
            }
            Navigation.PushAsync(new SendInformationInfoPage());
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
            ReqGridTaskList();
        }

        private void ReqGridTaskList()
        {
            for (int i = 0; i < 20; i++)
            {
                GridTaskModel _event = new GridTaskModel();
                _event.name = i + "在工厂周围检测水质";
                _event.eventName = "化工偷排事件";
                _event.addTime = "高桥镇，韩佳差家偶尔";

                dataList.Add(_event);
            }
            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridTaskModel item = e.Item as GridTaskModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < totalNum)
                {
                    ReqGridTaskList();
                }
            }
        }
    }
}
