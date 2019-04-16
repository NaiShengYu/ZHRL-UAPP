using System;
using System.Collections.Generic;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using AepApp.Tools;

namespace AepApp.View.Common
{
    public partial class EnterpriseInformationPage : ContentPage
    {

        EnterpriseModel _enterprise = null;

        public EnterpriseInformationPage(EnterpriseModel enterprise)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _enterprise = enterprise;
            GetEnterpriseInfomation();
        }


        /// <summary>
        /// 获取企业信息
        /// </summary>
        private async void GetEnterpriseInfomation()
        {
            string uri = App.BasicDataModule.url + "/api/Modmanage/GetEnterpriseByid";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _enterprise.id);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(uri, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
               
                try
                {
                    res.Results.Replace("null", "");
                    EnterpriseInformationModel information = JsonConvert.DeserializeObject<EnterpriseInformationModel>(res.Results);
                    BindingContext = information;
                }
                catch (Exception e)
                {
                }
            }

        }

        void Handle_CallPhone(object sender, System.EventArgs e)
        {
            Grid grid = sender as Grid;
            EnterpriseInformationModel model = grid.BindingContext as EnterpriseInformationModel;
            if(string.IsNullOrWhiteSpace(model.contactstelephone))
            DeviceUtils.phone(model.contactstelephone);
        }

        void Handle_CallMobile(object sender, System.EventArgs e)
        {
            Grid grid = sender as Grid;
            EnterpriseInformationModel model = grid.BindingContext as EnterpriseInformationModel;
            if (string.IsNullOrWhiteSpace(model.contactsmobile))
                DeviceUtils.phone(model.contactsmobile);
        }
    }
}
