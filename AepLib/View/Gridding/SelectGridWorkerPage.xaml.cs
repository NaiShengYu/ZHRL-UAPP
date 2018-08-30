using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectGridWorkerPage : ContentPage
    {
        private Guid gridId;
        private int pageIndex;
        private string mSearchKey;
        private bool hasMore;
        private ObservableCollection<GridStaffModel> dataList = new ObservableCollection<GridStaffModel>();

        public SelectGridWorkerPage(Guid gridcell)
        {
            InitializeComponent();
            gridId = gridcell;
            SearchData();
        }

        public void OnMessageClicked(Object sender, EventArgs e)
        {
            var but = sender as Button;
            GridStaffModel item = but.BindingContext as GridStaffModel;
            DeviceUtils.sms(item.mobile);
        }
        public void OnPhoneClicked(Object sender, EventArgs e)
        {
            var but = sender as Button;
            GridStaffModel item = but.BindingContext as GridStaffModel;
            DeviceUtils.phone(item.mobile);
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

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {

        }
        private void SearchData()
        {
            pageIndex = 0;
            hasMore = true;
            dataList.Clear();
            ReqWorksList();
        }

        private async void ReqWorksList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetGridAndParentGridStaff";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", ConstantUtils.PAGE_SIZE);
            map.Add("searchKey", mSearchKey);
            //map.Add("grid", gridId);
            map.Add("grid", Guid.Parse("72a38f57-1939-40e6-8cca-2960e0d994ea"));
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<GridStaffModel> list = JsonConvert.DeserializeObject<List<GridStaffModel>>(res.Results);
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
            GridStaffModel item = e.Item as GridStaffModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore && dataList.Count >= ConstantUtils.PAGE_SIZE)
                {
                    ReqWorksList();
                }
            }
        }
    }
}