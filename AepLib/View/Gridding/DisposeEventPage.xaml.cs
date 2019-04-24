
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
    //二级网格处理事件
    public partial class DisposeEventPage : ContentPage
    {
        private UserInfoModel auditor;//审核人
        private GridEventModel _eventModel;
        private GridEventFollowModel _followMoel;
        private ObservableCollection<GridTaskInfoModel> _taskInfoList = new ObservableCollection<GridTaskInfoModel>();
       //任务模型
        GridTaskInfoModel _taskInfoModel = null;
        Assignments _assignments = null;
        void SetSelectDown(object sender, System.EventArgs e)
        {
            _followMoel.Downing = true;
            _followMoel.Uping = false;

        }
        void SetSelectUp(object sender, System.EventArgs e)
        {
            _followMoel.Downing = false;
            _followMoel.Uping = true;
        }

        void RegistrationEvent(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new RegistrationEventPage(_eventModel.id.ToString()));
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (_followMoel != null)
            {
                EditContentsPage editContentsPage = new EditContentsPage(_followMoel, "EditContent", 3);
                editContentsPage.Title = "备注";
                Navigation.PushAsync(editContentsPage);
            }
        }


        void addEventFollowUp(object sender, System.EventArgs eventArgs)
        {
            UpdateIncidentState();
            upDateIncidentfollowup();
        }
        /// <summary>
        /// 选择时间
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            _taskInfoModel.deadline = e.NewDate;
        }

        public DisposeEventPage(GridEventModel eventModel)
        {
            InitializeComponent();
            _eventModel = eventModel;
            getEventInfo();
            _taskInfoModel = new GridTaskInfoModel
            {
                canEdit = true,
                id = Guid.NewGuid(),
                rowState = "add",
                title = eventModel.Title,
                type = 2,
                state = 4,
                index = 1,
                date = DateTime.Now,
                deadline = DateTime.Now,
                assignments = new ObservableCollection<Assignments>(),
                coords = new ObservableCollection<Coords>(),
                enterprise = new ObservableCollection<Enterprise>(),
            };
            _assignments = new Assignments
            {
                id = Guid.NewGuid(),
                rowState = "add",
                type = 1,
            };
            _taskInfoModel.assignments.Add(_assignments);
            _taskInfoList.Add(_taskInfoModel);
            gridWorker.BindingContext = _assignments;
            gridWorker.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnWorkersTapped()),
            });


            DatePickerStart.BindingContext = _taskInfoModel;

        }

        private void OnWorkersTapped()
        {
            if (_followMoel == null)
            {
                return;
            }
            Navigation.PushAsync(new SelectGridWorkerPage(_assignments, _eventModel.id));
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
                    var eventInfoModel = Tools.JsonUtils.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                    if (eventInfoModel == null) return;
                    if (eventInfoModel.state == 3)//事件处理完成了
                    {
                        _followMoel = new GridEventFollowModel
                        {
                            rowState = "add",
                            id = Guid.NewGuid(),
                            canEdit = false,
                            title = _eventModel.Title,
                            date = eventInfoModel.date,
                            staff = App.userInfo.id,
                            staffName = App.userInfo.userName,
                            staffTel = App.userInfo.tel,
                            incident = _eventModel.id,
                            level = App.gridUser.gridLevel,
                            gridName = App.gridUser.gridName,
                            grid = App.gridUser.id,
                        };
                        BindingContext = _followMoel;
                        GR.IsVisible = true;
                        sureH.Height = 0;

                    }
                    else
                    {
                        if (eventInfoModel.Followup.Count > 0)
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
        /// 处理事件记录
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
                    _followMoel.rowState = "otherwise";
                    _followMoel.gridName = _eventModel.GridName;
                    BindingContext = _followMoel;
                    if (_followMoel != null)
                    {
                        if (_followMoel.staff != null)
                        {
                            GetStaffInfo(_followMoel.staff.Value);
                        }
                    }
                    sureH.Height = 0;
                    if( _followMoel != null &&_followMoel.Tasks.Count >0){
                        GridEventFollowTaskModel followTaskModel = _followMoel.Tasks[0];
                        getTaskInfo(followTaskModel.id);
                    }else{
                        
                    }
                    GR.IsVisible = true;
                    if (_followMoel.state == 4) SW.IsToggled = false;

                }
                catch (Exception x)
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
                    BindingContext = _followMoel;
                    GR.IsVisible = true;
                }
            }
        }



        private async void upDateIncidentfollowup()
        {

            if(_followMoel.Uping==false && _followMoel.Downing == false && SW.IsToggled ==true){
                await DisplayAlert("提示", "请选择‘下发责令整改任务’或者‘上报上级处理’", "确定");
                return;
            }
            Dictionary<string, object> par = new Dictionary<string, object>();
            if(_followMoel.Downing == true){
                if(_assignments.staff ==null){
                    await DisplayAlert("提示", "请指派网格员", "确定");
                    return;
                }

                ObservableCollection<Dictionary<string, object>> assigmengtList = new ObservableCollection<Dictionary<string, object>>();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", _taskInfoModel.id);
                dic.Add("rowState", _taskInfoModel.rowState);
                if (_followMoel.incident != Guid.Empty)
                {
                    dic.Add("incident", _followMoel.incident);
                }
                dic.Add("staff", _followMoel.staff);
                if (_taskInfoModel.template != Guid.Empty) dic.Add("template", _taskInfoModel.template);
                dic.Add("title", _taskInfoModel.title);
                dic.Add("contents", _taskInfoModel.Contents);
                dic.Add("deadline", _taskInfoModel.deadline);
                dic.Add("period", 1);
                dic.Add("type", _taskInfoModel.type);
                dic.Add("state", _taskInfoModel.state);
                dic.Add("index", _taskInfoModel.index);
                dic.Add("date", _taskInfoModel.date);
                ObservableCollection<Dictionary<string, object>> taskList = new ObservableCollection<Dictionary<string, object>>();
                foreach (var item in _taskInfoModel.assignments)
                {
                    Dictionary<string, object> assigmengtdic = new Dictionary<string, object>();
                    assigmengtdic.Add("id", item.id);
                    assigmengtdic.Add("rowState", item.rowState);
                    assigmengtdic.Add("dept", item.dept);
                    assigmengtdic.Add("staff", item.staff);
                    assigmengtdic.Add("grid", item.grid);
                    assigmengtdic.Add("type", item.type);
                    assigmengtList.Add(assigmengtdic);
                }

                ObservableCollection<Dictionary<string, object>> coordsList = new ObservableCollection<Dictionary<string, object>>();
                foreach (var item in _taskInfoModel.coords)
                {
                    Dictionary<string, object> coordsdic = new Dictionary<string, object>();
                    coordsdic.Add("id", item.id);
                    coordsdic.Add("rowState", item.rowState);
                    coordsdic.Add("title", item.title);
                    coordsdic.Add("lng", item.lng);
                    coordsdic.Add("lat", item.lat);
                    coordsdic.Add("remarks", item.remarks);
                    coordsdic.Add("index", item.index);
                    coordsList.Add(coordsdic);
                }

                ObservableCollection<Dictionary<string, object>> enterprisesList = new ObservableCollection<Dictionary<string, object>>();
                foreach (var item in _taskInfoModel.enterprise)
                {
                    Dictionary<string, object> enterprisesdic = new Dictionary<string, object>();
                    enterprisesdic.Add("id", item.id);
                    enterprisesdic.Add("rowState", item.rowState);
                    enterprisesdic.Add("enterprise", item.enterprise);
                    enterprisesList.Add(enterprisesdic);
                }
                dic.Add("enterprises", enterprisesList);
                dic.Add("coords", coordsList);
                dic.Add("assignments", assigmengtList);
                taskList.Add(dic);
                par.Add("tasks", taskList);
            }

            string url = App.EP360Module.url + "/api/gbm/updateincidentfollowup";
            //2属实，4虚假
            _followMoel.state = SW.IsToggled == true ? 2 : 4;
            par.Add("id", _followMoel.id);
            par.Add("rowState", _followMoel.rowState);
            par.Add("incident", _eventModel.id);
            par.Add("staff", _followMoel.staff);
            par.Add("date", _followMoel.date);
            par.Add("remarks", _followMoel.Remarks);
            par.Add("level", _followMoel.level);
            par.Add("state", _followMoel.state);
            string param = JsonConvert.SerializeObject(par);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (res.Results == "\"OK\"")
                {
                    await DisplayAlert("提示", "成功", "确定");
                    _followMoel.rowState = "otherwise";
                               
                }
            }

        }

        private async void UpdateIncidentState()
        {
           int a =  SW.IsToggled == true ? 2 : 4;
            string url = App.EP360Module.url + "/api/gbm/UpdateIncidentState";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _eventModel.id);
            map.Add("state", a);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }
        }



        private async void getTaskInfo(Guid taskId)
        {

            string url = App.EP360Module.url + "/api/gbm/GetTaskDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", taskId);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    _taskInfoModel = Tools.JsonUtils.DeserializeObject<GridTaskInfoModel>(hTTPResponse.Results);
                    if(_taskInfoModel == null)return;
                    _taskInfoModel.canEdit = false;

                    if(_taskInfoModel.assignments !=null && _taskInfoModel.assignments.Count >0){
                        _assignments = _taskInfoModel.assignments[0];
                        gridWorker.BindingContext = _assignments;
                    }
                    DatePickerStart.BindingContext = _taskInfoModel;
                }
                catch (Exception e)
                {

                }
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
                BtnPhoneAuditor.IsVisible = !string.IsNullOrWhiteSpace(_followMoel.staffTel);
                BtnMsgAuditor.IsVisible = !string.IsNullOrWhiteSpace(_followMoel.staffTel);
            }
        }

        private void BtnPhone_Clicked(object sender, EventArgs e)
        {
            if (auditor == null)
            {
                return;
            }
            DeviceUtils.phone(auditor.telephone);
        }

        private void BtnMsg_Clicked(object sender, EventArgs e)
        {
            if (auditor == null)
            {
                return;
            }
            DeviceUtils.sms(auditor.telephone);
        }
    }
}
