using AepApp.Models;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class SendInformationInfoPage : ContentPage
    {


        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {

        }

        public SendInformationInfoPage(string info)
        {
            InitializeComponent();
            GetInformationDetail(info);

            //for (int i = 0; i < 10; i++)
            //{
            //    informationFile file = new informationFile
            //    {
            //        name = "2017年国家环境保护局",
            //        lenth = "1.5M",
            //        type = i % 2,
            //    };
            //    dataList.Add(file);
            //}
            //BindingContext = info;
            //listV.ItemsSource = dataList;
        }

        /// <summary>
        /// 下发信息详情
        /// </summary>
        /// <param name="id"></param>
        private async void GetInformationDetail(string id)
        {
            if(id == null)
            {
                return;
            }
            string url = App.EP360Module.url + "/api/gbm/GetDisseminateDetail";
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "id=" + id, "POST");
            if(res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                GridSendInformationModel detail = JsonConvert.DeserializeObject<GridSendInformationModel>(res.Results);
                BindingContext = detail;
                if (detail != null)
                {
                    listV.ItemsSource = detail.attachments;
                }
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var g = sender as Grid;
            AttachmentInfo attach = g.BindingContext as AttachmentInfo;
            string fileName = attach.id + ".pdf";
            string url = App.EP360Module.url + attach.url;
            HTTPResponse res = await EasyWebRequest.HTTPRequestDownloadAsync(url, fileName, App.EmergencyToken);
            await Navigation.PushAsync(new ShowFilePage(fileName));
        }
    }
}
