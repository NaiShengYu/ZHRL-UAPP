using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View
{
    public partial class HomePagePage : ContentPage
    {
        private InformationStaticsModel info;

        public HomePagePage()
        {
            InitializeComponent();
            Title = App.gridUser == null ? App.userInfo.userName : App.gridUser.gridName;
            GetModuleGridWorkingTaskStatics();
            GetModuleGridRegularTaskStatics();
            GetModuleGridReportTaskStatics();
            GetModuleGridSendInformationStatics();
            GetModuleBasicCompanyStatics();
            GetModule360AlarmStatics();
        }

        /// <summary>
        /// 网格化模块 - 未完成任务数据统计
        /// </summary>
        private async void GetModuleGridWorkingTaskStatics()
        {
            string url = App.EP360Module.url + "/api/gbm/GetIncompleteTaskCount";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string info = JsonConvert.DeserializeObject<string>(res.Results);
                    if (info != null)
                    {
                        BtnWorkingTaskNum.Text = info;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }


        /// <summary>
        /// 网格化模块 - 日常任务数据统计
        /// </summary>
        private async void GetModuleGridRegularTaskStatics()
        {
            string url = App.EP360Module.url + "/api/gbm/GetDailyTaskCount";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string info = JsonConvert.DeserializeObject<string>(res.Results);
                    if (info != null)
                    {
                        BtnRegularTaskNum.Text = info;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }


        /// <summary>
        /// 网格化模块 - 下级上报事件数据统计
        /// </summary>
        private async void GetModuleGridReportTaskStatics()
        {
            string url = App.EP360Module.url + "/api/gbm/GetSubordinateIncidentCount";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string info = JsonConvert.DeserializeObject<string>(res.Results);
                    if (info != null)
                    {
                        BtnReportEventNum.Text = info;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }



        /// <summary>
        /// 网格化模块 - 下发信息数据统计
        /// </summary>
        private async void GetModuleGridSendInformationStatics()
        {
            string url = App.EP360Module.url + "/api/gbm/GetLastDisseminate";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    info = JsonConvert.DeserializeObject<InformationStaticsModel>(res.Results);
                    if (info != null)
                    {
                        //Btn360AlarmNum.Text = info;
                        LabelInformationTime.Text = "";
                    }
                }
                catch (Exception e)
                {

                }
            }
        }


        /// <summary>
        /// 基础模块 - 企业数据统计
        /// </summary>
        private async void GetModuleBasicCompanyStatics()
        {
            string url = App.BasicDataModule.url + "/api/mod/GetEnterpriseCount";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string info = JsonConvert.DeserializeObject<string>(res.Results);
                    if (info != null)
                    {
                        Btn360CompanyNum.Text = info;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }


        /// <summary>
        /// 360模块 - 报警数据统计
        /// </summary>
        private async void GetModule360AlarmStatics()
        {
            string url = App.EP360Module.url + "/api/AppEnterprise/GetAlarmCount";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string info = JsonConvert.DeserializeObject<string>(res.Results);
                    if (info != null)
                    {
                        Btn360AlarmNum.Text = info;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private void LayoutSendInformation_Tapped(object sender, EventArgs e)
        {
            if (info == null)
            {
                return;
            }

        }

        class InformationStaticsModel
        {

        }

    }
}
