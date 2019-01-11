using AepApp.Interface;
using AepApp.Models;
using AepApp.Tools;
using AepApp.View.Gridding;
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
            NavigationPage.SetBackButtonTitle(this, "");
            SetUserDepartment();
            Title = App.siteName;

            if(App.EP360Module != null)
            {
                GetModule360Statics();
                GetModuleGridStatics();
            }
            if (App.moduleConfigEP360 !=null)
            {
                Layout360Statics.IsVisible = App.moduleConfigEP360.menuPollutionSrc;
                LayoutGridStatics.IsVisible = App.moduleConfigEP360.menuGridTask;
            }



        }


        async void VersionComparison()
        {
              string versions = DependencyService.Get<IOpenApp>().GetVersion();
            var alert =await DisplayAlert("Alert", "You have been alerted", "OK","Cancel");
            if(alert == true)
            {
                if(Device.RuntimePlatform == Device.iOS)
                    Device.OpenUri(new Uri(""));


            }


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //VersionComparison();
        }

        private void GetModuleGridStatics()
        {
            GetModuleGridWorkingTaskStatics();
            GetModuleGridRegularTaskStatics();
            GetModuleGridReportTaskStatics();
            GetModuleGridSendInformationStatics();
        }

        private void GetModule360Statics()
        {
            GetModuleBasicCompanyStatics();
            GetModule360AlarmStatics();
        }

        private async void SetUserDepartment()
        {
            string department = "";
            if (App.userDepartments == null)
                App.userDepartments = await (App.Current as App).GetStaffDepartments(App.userInfo.id);
            if (App.userDepartments == null) return;
            foreach (var item in App.userDepartments)
            {
                if (!string.IsNullOrWhiteSpace(item.name))
                    if (!string.IsNullOrWhiteSpace(item.name))
                        department += item.name;
            }
            //Title = department;
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
                    int info = JsonConvert.DeserializeObject<int>(res.Results);
                    BtnWorkingTaskNum.Text = info + "";
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
                    int info = JsonConvert.DeserializeObject<int>(res.Results);
                    BtnRegularTaskNum.Text = info + "";
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
                    int info = JsonConvert.DeserializeObject<int>(res.Results);

                    BtnReportEventNum.Text = info + "";
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
                    if (info != null && info != null && info.count > 0)
                    {
                        LayoutSendInformation.IsVisible = true;
                        ImgAlarm.IsVisible = true;
                        LabelInformationTime.Text = TimeUtils.DateTime2YMDHM(info.date);
                    }
                    else
                    {
                        LayoutSendInformation.IsVisible = false;
                        ImgAlarm.IsVisible = false;
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
                    int info = JsonConvert.DeserializeObject<int>(res.Results);
                    Btn360CompanyNum.Text = info + "";
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
                    int info = JsonConvert.DeserializeObject<int>(res.Results);
                    Btn360AlarmNum.Text = info + "";
                }
                catch (Exception e)
                {

                }
            }
        }

        private void LayoutSendInformation_Tapped(object sender, EventArgs e)
        {
            if (info == null && info.count != null && info.count > 0)
            {
                return;
            }
            if (info.count > 1)
            {
                Navigation.PushAsync(new SendInformationPage());
            }
            else
            {
                GridSendInformationModel model = new GridSendInformationModel
                {
                    staff = info.staff,
                    id = info.id.ToString(),
                };
                Navigation.PushAsync(new SendInformationInfoPage(model));
            }
        }

        private void LayoutWorkingTask_Tapped(object sender, EventArgs e)
        {
            App.OpenMenu(new TaskListPage());
        }


        private void LayoutRegularTask_Tapped(object sender, EventArgs e)
        {
            App.OpenMenu(new TaskListPage());
        }


        private void LayoutReportEvent_Tapped(object sender, EventArgs e)
        {
            App.OpenMenu(new EventListPage());
        }


        private void LayoutCompanyNum_Tapped(object sender, EventArgs e)
        {
            App.OpenMenu(new PollutionSourcePage());
        }


        private void LayoutCompanyAlarmNum_Tapped(object sender, EventArgs e)
        {
            App.OpenMenu(new PollutionSourcePage());
        }

        class InformationStaticsModel
        {
            public Guid id;
            public DateTime date;
            public int? count;
            public Guid staff;
        }

    }
}
