using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
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

        private bool hasMore = true;
        private int pageIndex;
        private string mSearchKey;
        private ObservableCollection<GridSendInformationModel> dataList = new ObservableCollection<GridSendInformationModel>();

     

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridSendInformationModel info = e.SelectedItem as GridSendInformationModel;
            if (info == null)
            {
                return;
            }
            Navigation.PushAsync(new SendInformationInfoPage(info));
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
            hasMore = true;
            dataList.Clear();
            if (App.gridUser == null)
                App.gridUser = await(App.Current as App).getStaffInfo();
            ReqGridInformationList();
        }

        private async void ReqGridInformationList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetDisseminateByKey";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", 20);
            map.Add("searchKey", mSearchKey);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if(res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<GridSendInformationModel> list = JsonConvert.DeserializeObject<List<GridSendInformationModel>>(res.Results);
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
            GridSendInformationModel item = e.Item as GridSendInformationModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore)
                {
                    ReqGridInformationList();
                }
            }
        }
    }
}
