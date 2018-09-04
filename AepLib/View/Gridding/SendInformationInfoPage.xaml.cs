using AepApp.Models;
using AepApp.Tools;
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

        public SendInformationInfoPage(GridSendInformationModel info)
        {
            InitializeComponent();
            GetStaffInfo(info.staff);
            GetInformationDetail(info.id);
        }

        /// <summary>
        /// 下发信息详情
        /// </summary>
        /// <param name="id"></param>
        private async void GetInformationDetail(string id)
        {
            if (id == null)
            {
                return;
            }
            string url = App.EP360Module.url + "/api/gbm/GetDisseminateDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", id);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    GridSendInformationModel detail = JsonConvert.DeserializeObject<GridSendInformationModel>(res.Results);
                    BindingContext = detail;
                    if (detail != null)
                    {
                        listV.ItemsSource = detail.attachments;
                        HtmlWebViewSource source = new HtmlWebViewSource();
                        source.Html = @detail.contents;
                        webviewContent.Source = source;
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// 下发人
        /// </summary>
        /// <param name="staffId"></param>
        private async void GetStaffInfo(Guid staffId)
        {
            UserInfoModel author = await (App.Current as App).GetUserInfo(staffId);
            if (author != null)
            {
                LabelAuthor.Text = author.userName;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var g = sender as Grid;
            AttachmentInfo attach = g.BindingContext as AttachmentInfo;
            string fileName = attach.filename;
            //string url = App.EP360Module.url + "/api/gbm/GetAttachment/" + attach.id;
            if (!string.IsNullOrWhiteSpace(attach.url))
            {
                attach.url = attach.url.Replace("~", "");
            }
            string url = App.EP360Module.url + attach.url;
            if (StringUtils.IsImg(fileName))
            {

                List<string> imgs = new List<string>();
                imgs.Add(url);
                await Navigation.PushAsync(new BrowseImagesPage(imgs));
                //await Navigation.PushAsync(new ShowFilePage(url, true));
            }
            else
            {

                Device.OpenUri(new Uri(url));
                //HTTPResponse res = await EasyWebRequest.HTTPRequestDownloadAsync(url, fileName, App.FrameworkToken);
                //await Navigation.PushAsync(new ShowFilePage(fileName));
            }
        }
    }
}
