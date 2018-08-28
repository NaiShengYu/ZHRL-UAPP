
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class DisposeEventPage : ContentPage
    {
        private string mEventId;
        private UserInfoModel auditor;//审核人
        private GridEventInfoModel detail;


        void RegistrationEvent(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new RegistrationEventPage(null));
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {

        }

        public DisposeEventPage(string id)
        {
            InitializeComponent();
            mEventId = id;
            GetTaskDetail();

            gridWorker.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnWorkersTapped()),
            });
        }

        private void OnWorkersTapped()
        {
            Navigation.PushAsync(new SelectGridWorkerPage());
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
                if (detail != null && detail.staff != null)
                {
                    GetStaffInfo(detail.staff.ToString());
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
    }
}
