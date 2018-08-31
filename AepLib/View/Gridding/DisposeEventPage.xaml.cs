
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
    public partial class DisposeEventPage : ContentPage
    {
        private UserInfoModel auditor;//审核人
        private GridEventModel _eventModel;
        private GridEventFollowModel detail;
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
            Navigation.PushAsync(new RegistrationEventPage(null));
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
            GetTaskDetail();

            _taskInfoModel = new GridTaskInfoModel
            {
                id = Guid.NewGuid(),
                rowState = "add",
                title = eventModel.Title,
                type = 1,
                state = 1,
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
            Navigation.PushAsync(new SelectGridWorkerPage(_assignments));
        }

        /// <summary>
        /// 处理事件记录
        /// </summary>
        private async void GetTaskDetail()
        {
            string url = App.EP360Module.url + "/api/gbm/GetIncidentFollowupDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _eventModel.id);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    _followMoel = JsonConvert.DeserializeObject<GridEventFollowModel>(res.Results);
                    _followMoel.title = _eventModel.Title;
                    _followMoel.rowState = "otherwise";
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
                        //level = App.gridUser.gridLevel,
                        //gridName = App.gridUser.gridName,
                        //grid = App.gridUser.id,
                        Tasks = new ObservableCollection<GridEventFollowTaskModel>(),
                    };
                    BindingContext = _followMoel;

                }
            }
        }



        private async void upDateIncidentfollowup()
        {

            if(_followMoel.Uping==false && _followMoel.Downing == false){
                await DisplayAlert("提示", "请选择‘下发责令整改任务’或者‘上报上级处理’", "确定");
                return;
            }
            Dictionary<string, object> par = new Dictionary<string, object>();
            if(_followMoel.Downing == true){
                if(_assignments.staff ==null){
                    await DisplayAlert("提示", "请指派网格员", "确定");
                    return;
                }
                par.Add("task", _taskInfoList);
            }

            string url = App.EP360Module.url + "/api/gbm/updateincidentfollowup";
            _followMoel.state = SW.IsToggled == true ? 7 : 8;
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
                if (res.Results == "OK")
                {
                    await DisplayAlert("提示", "成功", "确定");
                }
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
                    _taskInfoModel = JsonConvert.DeserializeObject<GridTaskInfoModel>(hTTPResponse.Results);
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
    }
}
