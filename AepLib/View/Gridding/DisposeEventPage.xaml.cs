
using AepApp.Models;
using AepApp.Tools;
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
            if(detail == null)
            {
                return;
            }
            Navigation.PushAsync(new SelectGridWorkerPage(detail.gridcell));
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
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    detail = JsonConvert.DeserializeObject<GridEventInfoModel>(res.Results);
                    BindingContext = detail;
                    if (detail != null && detail.staff != null)
                    {
                        GetStaffInfo(detail.staff);
                    }
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
                LabelAuditor.Text = auditor.userName;
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
