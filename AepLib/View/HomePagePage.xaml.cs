using AepApp.Interface;
using AepApp.Models;
using AepApp.Tools;
using AepApp.View.EnvironmentalQuality;
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
        private int totalAirSites = 0;//空气站总数
        private int offlineAirSites = 0;//离线空气站数
        private int totalVocSites = 0;//voc站点总数
        private int offlineVocSites = 0;//离线voc站点数
        private int totalWaterSites = 0;

        public HomePagePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            SetUserDepartment();
            Title = App.siteName;
            //360,网格化
            if (App.EP360Module != null)
            {
                GetModule360Statics();
                GetModuleGridStatics();
            }
            if (App.moduleConfigEP360 != null)
            {
                Layout360Statics.IsVisible = App.moduleConfigEP360.menuPollutionSrc;
                LayoutGridStatics.IsVisible = App.moduleConfigEP360.menuGridTask;
            }

            //环境质量
            if (App.environmentalQualityModel != null)
            {
                GetEnvironmentQualityStatics();
            }
            if (App.moduleConfigENVQ != null)
            {
                LayoutEnvironmentStatics.IsVisible = App.moduleConfigENVQ.showEnvSummary;
                LayoutEnvironmentAir.IsVisible = App.moduleConfigENVQ.menuAir;
                LayoutEnvironmentVOC.IsVisible = App.moduleConfigENVQ.menuVOC;
                LayoutEnvironmentWater.IsVisible = App.moduleConfigENVQ.menuWater;
            }


        }


        async void VersionComparison()
        {
            string versions = DependencyService.Get<IOpenApp>().GetVersion();
            var alert = await DisplayAlert("Alert", "You have been alerted", "OK", "Cancel");
            if (alert == true)
            {
                if (Device.RuntimePlatform == Device.iOS)
                    Device.OpenUri(new Uri("https://itunes.apple.com/cn/app/%E7%8E%AF%E4%BF%9D%E7%9B%91%E7%AE%A1-%E5%85%A8%E6%96%B0%E7%89%88%E6%9C%AC/id1445804624?mt=8"));
                if (Device.RuntimePlatform == Device.Android)
                    Device.OpenUri(new Uri("https://www.pgyer.com/ai2Y"));
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //VersionComparison();
        }

        /// <summary>
        /// 网格化模块统计数据
        /// </summary>
        private void GetModuleGridStatics()
        {
            GetModuleGridWorkingTaskStatics();
            GetModuleGridRegularTaskStatics();
            GetModuleGridReportTaskStatics();
            GetModuleGridSendInformationStatics();
        }

        /// <summary>
        /// 360模块统计数据
        /// </summary>
        private void GetModule360Statics()
        {
            GetModuleBasicCompanyStatics();
            GetModule360AlarmStatics();
        }

        /// <summary>
        /// 环境质量模块统计数据
        /// </summary>
        private void GetEnvironmentQualityStatics()
        {
            GetModelEnvironmentQualityStatics();
            GetModelEnvironmentOfflineSite();
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

        /// <summary>
        /// 环境质量模块 - 报警数据统计
        /// </summary>
        private async void GetModelEnvironmentQualityStatics()
        {
            string url = App.environmentalQualityModel.url + DetailUrl.GetVOCSite;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("pageIndex", -1);
            dic.Add("type", 3);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(dic), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<VOCSiteResult>(res.Results);
                    List<VOCSiteListModel> list = result.Items;
                    int countAir = 0;
                    int countVocs = 0;
                    int countWater = 0;
                    foreach (var item in list)
                    {
                        string subtype = item.subtype;
                        if ("0".Equals(subtype))
                        {
                            countVocs++;
                        }
                        else if ("3".Equals(subtype))
                        {
                            countAir++;
                        }
                        else if ("9".Equals(subtype))
                        {
                            countWater++;
                        }
                    }
                    totalAirSites = countAir;
                    totalVocSites = countVocs;
                    totalWaterSites = countWater;
                    setSiteCount();
                }
                catch (Exception e)
                {

                }
            }
        }
        /// <summary>
        /// 环境质量模块 - 离线站点数据统计
        /// </summary>
        private async void GetModelEnvironmentOfflineSite()
        {
            string url = App.environmentalQualityModel.url + DetailUrl.GetOfflineSite;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(dic), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<OfflineSiteModel> list = JsonConvert.DeserializeObject<List<OfflineSiteModel>>(res.Results);
                    int countAir = 0;
                    int countVocs = 0;
                    foreach (var item in list)
                    {
                        int subtype = item.subtype;
                        if (subtype == 0)
                        {
                            countVocs++;
                        }
                        else if (subtype == 3)
                        {
                            countAir++;
                        }
                    }
                    offlineAirSites = countAir;
                    offlineVocSites = countVocs;
                    setSiteCount();
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// 设置站点在线数/总数
        /// </summary>
        private void setSiteCount()
        {
            string airCount = totalAirSites + "";
            string vocCount = totalVocSites + "";
            if (totalAirSites > 0)
            {
                if (offlineAirSites > 0 && offlineAirSites <= totalAirSites)
                {
                    airCount = (totalAirSites - offlineAirSites) + "/" + totalAirSites;
                }
                BtnEnvironmentAirNum.Text = airCount;
            }
            if (totalVocSites > 0)
            {
                if (offlineVocSites > 0 && offlineVocSites <= totalVocSites)
                {
                    vocCount = (totalVocSites - offlineVocSites) + "/" + totalVocSites;
                }
                BtnEnvironmentVOCNum.Text = vocCount;
            }
            BtnEnvironmentWaterNum.Text = totalWaterSites + "";
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
            Navigation.PushAsync(new TaskListPage());
        }


        private void LayoutRegularTask_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TaskListPage());
        }


        private void LayoutReportEvent_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EventListPage());
        }


        private void LayoutCompanyNum_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PollutionSourcePage());
        }


        private void LayoutCompanyAlarmNum_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PollutionSourcePage());
        }


        private void LayoutEnvironmentAir_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AirPage());
        }


        private void LayoutEnvironmentVOC_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VOCSiteListPage());
        }


        private void LayoutEnvironmentWater_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WaterQualitySiteListPage());
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
