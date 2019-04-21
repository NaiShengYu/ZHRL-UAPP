using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class ExecutionRecordPage : ContentPage
    {
        private int pageIndex;
        private bool hasMore = true;
        private int totalNum;
        private string mSearchKey;
        private ObservableCollection<GridTaskHandleRecordModel> dataList = new ObservableCollection<GridTaskHandleRecordModel>();
        private string mTaskId;
        private string mStaff;
        public ExecutionRecordPage(string taskId, string staff)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            mTaskId = taskId;
            mStaff = staff;
            SearchData();
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            GridTaskHandleRecordModel record = e.SelectedItem as GridTaskHandleRecordModel;
            if (record == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskResultPage(record.id.Value, record, false, new ObservableCollection<Enterprise>()));
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
            pageIndex = 0;
            hasMore = true;
            dataList.Clear();
            ReqGridTaskList();
        }

        private async void ReqGridTaskList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskHandleList";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("staff", mStaff);
            map.Add("task", mTaskId);

            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<GridTaskHandleRecordModel> list = JsonConvert.DeserializeObject<List<GridTaskHandleRecordModel>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.SubTitle = item.parentName + " - " + item.gridName;
                            dataList.Add(item);
                            if (item.enterprise != null)
                            {
                                ReqEnters(item);
                            }
                            else
                            {
                                listView.ItemsSource = null;
                                listView.ItemsSource = dataList;
                                GetSubTitle();
                            }
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

        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            GridTaskHandleRecordModel item = e.Item as GridTaskHandleRecordModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore && dataList.Count >= ApiUtils.PAGE_SIZE)
                {
                    ReqGridTaskList();
                }
            }
        }

        private async void GetSubTitle()
        {
            if(dataList == null)
            {
                return;
            }
            if (App.gridUser == null)
            {
                foreach (var item in dataList)
                {
                    try
                    {
                        string sub = await GetStaffGridInfo(item.staff.Value) + " - " + await GetStaffInfo(item.staff.Value);
                        item.SubTitle = sub;
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                }
                BindingContext = dataList;
            }
        }

        /// <summary>
        /// 获取执行人信息
        /// </summary>
        /// <param name="record"></param>
        private async Task<string> GetStaffInfo(Guid s)
        {        
            UserInfoModel user = await (App.Current as App).GetUserInfo(s);
            if (user != null)
            {
                return user.userName;
            }
            return "";
        }

        /// <summary>
        /// 获取执行人部门信息
        /// </summary>
        private async Task<string> GetStaffGridInfo(Guid s)
        {
            ObservableCollection<UserDepartmentsModel> departs = await (App.Current as App).GetStaffDepartments(s);

            if (departs != null && departs.Count > 0)
            {
                return departs[0].name;
            }
            return "";
        }

        //根据id获取企业
        private async void ReqEnters(GridTaskHandleRecordModel model)
        {

            string url = App.BasicDataModule.url + "/api/Modmanage/GetEnterpriseByid";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", model.enterprise);
            string par = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, par, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var enterpriseModel = JsonConvert.DeserializeObject<GridEnterpriseModel>(hTTPResponse.Results);
                    if (enterpriseModel != null)
                    {
                        model.enterpriseName = enterpriseModel.name;
                    }
                    listView.ItemsSource = null;
                    listView.ItemsSource = dataList;
                    GetSubTitle();
                }
                catch (Exception)
                {

                }                
            }
        }


    }
}
