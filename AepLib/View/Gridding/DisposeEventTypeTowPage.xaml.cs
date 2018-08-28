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
        private Guid mEventId;
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

        public DisposeEventTypeTowPage(Guid eventId)
        {
            InitializeComponent();
            mEventId = eventId;
            GetTaskDetail();
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
                    if (detail != null)
                    {
                        listV.ItemsSource = detail.Followup;
                        if (detail.staff != null)
                        {
                            GetStaffInfo(detail.staff);
                        }
                    }
                }
                catch (Exception x)
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
