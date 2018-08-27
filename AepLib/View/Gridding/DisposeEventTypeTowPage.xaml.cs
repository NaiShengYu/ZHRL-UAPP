using AepApp.Models;
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
        private string mEventId;
        private UserInfoModel auditor;//审核人
        private GridEventInfoModel detail;

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
        }

        public DisposeEventTypeTowPage(string eventId)
        {
            InitializeComponent();
            mEventId = eventId;
            GetTaskDetail();

            //taskList.Add(new Task
            //{
            //    name = "在厂房周围监测水质",
            //    address = "村级网络，四官旺村",
            //});
            //taskList.Add(new Task
            //{
            //    name = "在厂房周围监测水质",
            //    address = "村级网络，四官旺村",
            //});
            //taskList.Add(new Task
            //{
            //    name = "在厂房周围监测水质",
            //    address = "村级网络，四官旺村",
            //});
            //listV.ItemsSource = taskList;

        }

        /// <summary>
        /// 事件详情
        /// </summary>
        private async void GetTaskDetail()
        {
            string url = App.EP360Module.url + "/api/gbm/GetIncidentDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", mEventId);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST");
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                detail = JsonConvert.DeserializeObject<GridEventInfoModel>(res.Results);
                BindingContext = detail;
                if (detail != null)
                {
                    listV.ItemsSource = detail.Followup;
                    if (detail.staff != null)
                    {
                        GetStaffInfo(detail.staff.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 审核人
        /// </summary>
        /// <param name="staffId"></param>
        private async void GetStaffInfo(string staffId)
        {
            auditor = await (App.Current as App).GetUserInfo(staffId);
            if (auditor != null)
            {
                LabelAuditor.Text = auditor.userName;
            }
        }

        private void BtnPhone_Clicked(object sender, EventArgs e)
        {
            if (auditor == null)
            {
                return;
            }
            Device.OpenUri(new Uri("tel:" + auditor.tel));
        }

        private void BtnMsg_Clicked(object sender, EventArgs e)
        {
            if (auditor == null)
            {
                return;
            }
            Device.OpenUri(new Uri("sms:" + auditor.tel));
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
