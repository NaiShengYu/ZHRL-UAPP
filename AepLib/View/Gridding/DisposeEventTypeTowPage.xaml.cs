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
        private GridEventFollowModel detail;
        private GridEventFollowModel _followMoel;
        string _eventId = "";
        private ObservableCollection<GridTaskInfoModel> taskInfoList = new ObservableCollection<GridTaskInfoModel>();

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            Followup task = e.SelectedItem as Followup;
            if (task == null) return;
            Navigation.PushAsync(new TaskInfoPage());
            listV.SelectedItem = null;

        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {

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
            towPage.AddATask += (object taskinfo, EventArgs args) =>
            {
                GridTaskInfoModel taskInfoModel = taskinfo as GridTaskInfoModel;
                if(taskInfoModel !=null){
                    taskInfoList.Add(taskInfoModel);
                    GridEventFollowTaskModel taskModel = new GridEventFollowTaskModel
                    {
                        name = taskInfoModel.title,
                        state = taskInfoModel.state,
                    };
                    _followMoel.Tasks.Add(taskModel);
                }

            };
                

            Navigation.PushAsync(towPage);
        }

        void addEventFollowUp (object sender,System.EventArgs eventArgs){
        

            upDateIncidentfollowup();
        }


        public DisposeEventTypeTowPage(GridEventModel eventModel)
        {
            InitializeComponent();
            _eventModel = eventModel;
            GetTaskDetail();

         

        }

        /// <summary>
        /// 事件详情
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
                    detail = JsonConvert.DeserializeObject<GridEventFollowModel>(res.Results);
                    detail.title = _eventModel.Title;
                    BindingContext = detail;
                    if (detail != null)
                    {
                        if (detail.staff != null)
                        {
                            GetStaffInfo(detail.staff.Value);
                        }
                    }
                    listV.ItemsSource = detail.Tasks;
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
                }

            }
        }


        private async void upDateIncidentfollowup (){

            string url = App.EP360Module.url + "/api/gbm/updateincidentfollowup";
            _followMoel.state = SW.IsToggled == true ? 4 : 1;
            Dictionary<string, object> par = new Dictionary<string, object>();
            par.Add("id", Guid.NewGuid());
            par.Add("rowState", "add");
            par.Add("incident", _eventModel.id);
            par.Add("staff", _followMoel.staff);
            par.Add("date", _followMoel.date);
            par.Add("remarks", _followMoel.Remarks);
            par.Add("level", _followMoel.level);
            par.Add("state", _followMoel.state);
            par.Add("task", taskInfoList);
            string param = JsonConvert.SerializeObject(par);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK){
                if(res.Results =="OK"){
                    await DisplayAlert("提示", "成功", "确定");
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
                detail.staffTel = auditor.tel;
                detail.staffName = auditor.userName;
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
