using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class DisposeEventTypeTowPage : ContentPage
    {
        private GridEventModel _eventModel;
        private UserInfoModel auditor;//审核人
        private GridEventFollowModel _followMoel;
        private ObservableCollection<Dictionary<string, object>> taskInfoList = new ObservableCollection<Dictionary<string, object>>();

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            GridEventFollowTaskModel task = e.SelectedItem as GridEventFollowTaskModel;
            if (task == null) return;
            TaskInfoTypeTowPage towPage = new TaskInfoTypeTowPage(task.id.ToString(), false, _followMoel.id.ToString(),false,"");
            Navigation.PushAsync(towPage);
            listV.SelectedItem = null;
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {

        }

        void showEvent(object sender,EventArgs eventArgs){
            Navigation.PushAsync(new RegistrationEventPage(_eventModel.id.ToString()));

        }

        void townDisposeEvent(object sender,System.EventArgs e){
            var dsPage = new DisposeEventPage(_eventModel);
            Navigation.PushAsync(dsPage);
        }


        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (_followMoel !=null)
            {
                EditContentsPage editContentsPage = new EditContentsPage(_followMoel,"EditContent",3);
                editContentsPage.Title = "备注";
                Navigation.PushAsync(editContentsPage);
            }
        }
        void AddEventTask(object sender, System.EventArgs e){

            TaskInfoTypeTowPage towPage = new TaskInfoTypeTowPage("", false, _eventModel.id.ToString(),false,"");
            towPage.AddATask += (object taskinfo,object taskModel1, EventArgs args) =>
            {
                Dictionary<string, object> task = taskinfo as Dictionary<string,object>;
                GridTaskInfoModel taskInfoModel = taskModel1 as GridTaskInfoModel;
                if(task !=null){
                    taskInfoList.Add(task);
                    GridEventFollowTaskModel taskModel = new GridEventFollowTaskModel
                    {
                        name = taskInfoModel.title,
                        state = taskInfoModel.state.Value,
                    };
                    _followMoel.Tasks.Add(taskModel);
                }
                listV.ItemsSource = _followMoel.Tasks;

            };
                

            Navigation.PushAsync(towPage);
        }

        void addEventFollowUp (object sender,System.EventArgs eventArgs){

            UpdateIncidentState();
            upDateIncidentfollowup();
        }


        public DisposeEventTypeTowPage(GridEventModel eventModel)
        {
            InitializeComponent();
            _eventModel = eventModel;

            getEventInfo();

         

        }


        //获取事件详情
        private async void getEventInfo()
        {
            string url = App.EP360Module.url + "/api/gbm/GetIncidentDetail";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("id", _eventModel.id);
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {

                    var eventInfoModel = JsonConvert.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                    if (eventInfoModel.state == 3)
                    {
                        _followMoel = new GridEventFollowModel
                        {
                            rowState = "add",
                            id = Guid.NewGuid(),
                            canEdit = false,
                            title = _eventModel.Title,
                            date = _eventModel.Date,
                            staff = App.userInfo.id,
                            staffName = App.userInfo.userName,
                            staffTel = App.userInfo.tel,
                            state = _eventModel.state,
                            incident = _eventModel.id,
                            level = App.gridUser.gridLevel,
                            gridName = App.gridUser.gridName,
                            grid = App.gridUser.id,
                            Tasks = new ObservableCollection<GridEventFollowTaskModel>(),
                        };
                        GR.IsVisible = true;
                        BindingContext = _followMoel;
                    }
                    else
                    {
                        bool isup = false;
                        if (eventInfoModel.Followup.Count > 0)
                        {
                            foreach (var item in eventInfoModel.Followup)
                            {
                                if (item.level == App.gridUser.gridLevel.ToString())
                                    isup = true;
                            }
                        }
                        if (isup == true)
                        {
                            Followup followup = eventInfoModel.Followup[0];
                            GetFollowupDetail(Guid.Parse(followup.id));
                        }
                        else
                        {
                            _followMoel = new GridEventFollowModel
                            {
                                rowState = "add",
                                id = Guid.NewGuid(),
                                canEdit = true,
                                title = _eventModel.Title,
                                date = DateTime.Now,
                                staff = App.userInfo.id,
                                staffName = App.userInfo.userName,
                                staffTel = App.userInfo.tel,
                                state = 4,
                                incident = _eventModel.id,
                                level = App.gridUser.gridLevel,
                                gridName = App.gridUser.gridName,
                                grid = App.gridUser.id,
                                Tasks = new ObservableCollection<GridEventFollowTaskModel>(),
                            };
                            GR.IsVisible = true;
                            BindingContext = _followMoel;
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

        }






        /// <summary>
        /// 事件跟进详情
        /// </summary>
        private async void GetFollowupDetail(Guid followId)
        {
            string url = App.EP360Module.url + "/api/gbm/GetIncidentFollowupDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", followId);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    _followMoel = JsonConvert.DeserializeObject<GridEventFollowModel>(res.Results);
                    _followMoel.title = _eventModel.Title;
                    BindingContext = _followMoel;
                    GR.IsVisible = true;
                    if (_followMoel != null)
                    {
                        if (_followMoel.staff != null)
                        {
                            GetStaffInfo(_followMoel.staff.Value);
                        }
                    }
                    listV.ItemsSource = _followMoel.Tasks;
                }
                catch (Exception x)
                {
                    _followMoel = new GridEventFollowModel
                    {
                        canEdit = true,
                        title = _eventModel.Title,
                        date = DateTime.Now,
                        staff = App.userInfo.id,
                        staffName = App.userInfo.userName,
                        staffTel = App.userInfo.tel,
                        state = 4,
                        incident = _eventModel.id,
                        level = App.gridUser.gridLevel,
                        Tasks = new ObservableCollection<GridEventFollowTaskModel>(),
                    };
                    BindingContext = _followMoel;
                    listV.ItemsSource = _followMoel.Tasks;
                    GR.IsVisible = true;

                }

            }
        }


        private async void upDateIncidentfollowup (){


            string url = App.EP360Module.url + "/api/gbm/updateincidentfollowup";
            _followMoel.state = SW.IsToggled == true ? 3 : 1;
            Dictionary<string, object> par = new Dictionary<string, object>();
            par.Add("id", Guid.NewGuid());
            par.Add("rowState", "add");
            par.Add("incident", _eventModel.id);
            par.Add("staff", _followMoel.staff);
            par.Add("date", _followMoel.date);
            par.Add("remarks", _followMoel.Remarks);
            par.Add("level", _followMoel.level);
            par.Add("state", _followMoel.state);
            par.Add("tasks", taskInfoList);
            string param = JsonConvert.SerializeObject(par);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK){
                if(res.Results =="\"OK\""){
                    await DisplayAlert("提示", "成功", "确定");
                    await Navigation.PopAsync();
                }


            }

        }

        private async void UpdateIncidentState()
        {

            if (SW.IsToggled ==false) return;

            string url = App.EP360Module.url + "/api/gbm/UpdateIncidentState";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _eventModel.id);
            map.Add("state", 3);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }
        }


        /// <summary>
        /// 审核人
        /// </summary>
        /// <param name="staffId"></param>
        private async void GetStaffInfo(Guid staffId)
        {
            auditor = await (App.Current as App).GetUserInfo(staffId);
            if (auditor != null)
            {
                _followMoel.staffTel = auditor.tel;
                _followMoel.staffName = auditor.userName;
                BtnPhone.IsVisible = !string.IsNullOrWhiteSpace(_followMoel.staffTel);
                BtnMsg.IsVisible = !string.IsNullOrWhiteSpace(_followMoel.staffTel);
            }
        }

        private void BtnPhone_Clicked(object sender, EventArgs e)
        {
            if (auditor == null)
            {
                return;
            }
            DeviceUtils.phone(auditor.tel);
        }

        private void BtnMsg_Clicked(object sender, EventArgs e)
        {
            if (auditor == null)
            {
                return;
            }
            DeviceUtils.sms(auditor.tel);
        }
        //private class Task
        //{
        //    public string name
        //    {
        //        get;
        //        set;
        //    }

        //    public string address
        //    {
        //        get;
        //        set;
        //    }
        //}

    }
}
