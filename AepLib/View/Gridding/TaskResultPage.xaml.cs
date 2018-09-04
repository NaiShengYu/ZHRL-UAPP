﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using AepApp.Interface;
using AepApp.Models;
using AepApp.Tools;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Sample;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    /// <summary>
    /// 任务执行结果
    /// </summary>
    public partial class TaskResultPage : ContentPage
    {
        private Guid mTaskId;
        private bool mIsEdit = false;
        private GridTaskHandleRecordModel mRecord;
        private ObservableCollection<AttachmentInfo> photoList = new ObservableCollection<AttachmentInfo>();
        private ObservableCollection<GridAttachmentUploadModel> uploadModel = new ObservableCollection<GridAttachmentUploadModel>();
        private int UploadSuccessCount = 0;

        public TaskResultPage(Guid taskId, GridTaskHandleRecordModel record, bool isEdit)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            mIsEdit = isEdit;
            mTaskId = taskId;
            mRecord = record;
            GridOperate.IsVisible = mIsEdit;
            GetStaffInfo();
            if (!isEdit) GetRecordDetail();
            else SetRecordInfo(record);
        }

        private void SetRecordInfo(GridTaskHandleRecordModel record)
        {
            if (record == null)
            {
                return;
            }
            if (!mIsEdit)
            {
                var source = new HtmlWebViewSource();
                source.Html = @record.results;
                Webview.Source = source;
            }
            else
            {
                var source = new UrlWebViewSource();
                var rootPath = DependencyService.Get<IWebviewService>().Get();
                source.Url = System.IO.Path.Combine(rootPath, "EditorHome.html");
                Webview.Source = source;
            }
            photoList.Clear();
            if (record.attachments != null && record.attachments.Count > 0)
            {
                foreach (var item in record.attachments)
                {
                    photoList.Add(item);
                }
                creatPhotoView(true);
            }
            BindingContext = record;
            ST.BindingContext = photoList;
        }

        /// <summary>
        /// 获取执行人信息
        /// </summary>
        /// <param name="record"></param>
        private async void GetStaffInfo()
        {
            if (mRecord == null)
            {
                return;
            }
            UserInfoModel user = await (App.Current as App).GetUserInfo(mRecord.staff.Value);
            if (user != null)
            {
                LabelStaff.Text = user.userName;
            }
        }

        private async void GetRecordDetail()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskHandleDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", mRecord.id);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    mRecord = JsonConvert.DeserializeObject<GridTaskHandleRecordModel>(res.Results);
                    SetRecordInfo(mRecord);
                }
                catch (Exception e)
                {

                }
            }
        }


        /// <summary>
        /// 添加任务处理记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExecutionRecord(object sender, System.EventArgs e)
        {
            uploadImg();
        }

        private async void uploadImg()
        {
            foreach (var item in photoList)
            {
                if (item.isUploaded)
                {
                    continue;
                }
                NameValueCollection nameValue = new NameValueCollection();
                nameValue.Add("id", mIsEdit ? Guid.NewGuid().ToString() : mRecord.id.ToString());
                HTTPResponse res = await EasyWebRequest.upload(item.url, ".png", ConstantUtils.UPLOAD_GRID_BASEURL, ConstantUtils.UPLOAD_GRID_API, nameValue);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        List<GridAttachmentResultModel> result = JsonConvert.DeserializeObject<List<GridAttachmentResultModel>>(res.Results);
                        if (result != null && result.Count > 0)
                        {
                            item.isUploaded = true;
                            GridAttachmentUploadModel m = new GridAttachmentUploadModel
                            {
                                id = result[0].id,
                                rowState = "add",
                            };
                            uploadModel.Add(m);
                            UploadSuccessCount++;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            if (UploadSuccessCount == photoList.Count)
            {
                addResult();
            }
            else
            {
                DependencyService.Get<IToast>().LongAlert("图片上传失败，请重试");
            }
        }

        //添加记录
        private async void addResult()
        {
            string result = await mRecord.EvaluateJavascript("javascript:getEditorValue();");
            string content = Regex.Unescape(result);
            if (string.IsNullOrWhiteSpace(content))
            {
                DependencyService.Get<IToast>().ShortAlert("请输入执行结果");
                return;
            }
            string url = App.EP360Module.url + "/api/gbm/updatetaskhandle";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", mIsEdit ? Guid.NewGuid() : mRecord.id);
            map.Add("rowState", mIsEdit ? "add" : "");
            map.Add("task", mTaskId);
            map.Add("date", DateTime.Now);
            map.Add("staff", App.userInfo.id);
            map.Add("results", content.ToString());
            map.Add("forassignment", mRecord.assignment);
            map.Add("attachments", uploadModel);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string status = JsonConvert.DeserializeObject<string>(res.Results);
                    if ("OK".Equals(status))
                    {
                        DependencyService.Get<IToast>().LongAlert("添加成功！");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        DependencyService.Get<IToast>().LongAlert("添加失败，请重试22！");

                    }
                }
                catch (Exception ex)
                {
                    DependencyService.Get<IToast>().LongAlert("添加失败，请重试！");
                }
            }
        }

        //拍照
        async void takePhoto(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                CompressionQuality = 50,
                Directory = "Gridding",
                Name = System.DateTime.Now + ".png"
            });

            if (file == null)
            {
                return;
            }

            else
            {

                photoList.Add(new AttachmentInfo
                {
                    url = file.Path,
                    isUploaded = false,
                });

                creatPhotoView(false);
            }


        }

        /// <summary>
        /// 根据照片数创建图片列表
        /// </summary>
        /// <param name="isFromNetwork"></param>
        void creatPhotoView(bool isFromNetwork)
        {

            PickSK.Children.Clear();

            foreach (AttachmentInfo attach in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);

                if (isFromNetwork)
                {
                    attach.url = "/grid/GetImage/";
                }
                Image img = new Image
                {
                    Source = isFromNetwork ? ImageSource.FromUri(new Uri(App.EP360Module.url + attach.url + attach.id)) : ImageSource.FromFile(attach.url) as FileImageSource,
                    HeightRequest = 80,
                    WidthRequest = 80,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.Fill,
                };
                grid.Children.Add(img);

                if (100.0 * photoList.Count > App.ScreenWidth)
                    pickSCR.ScrollToAsync(100 * photoList.Count - (App.ScreenWidth), 0, true);

                TapGestureRecognizer tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) =>
                {
                    List<string> imgs = new List<string>();
                    foreach (var item in photoList)
                    {
                        imgs.Add(item.url);
                    }
                    Navigation.PushAsync(new BrowseImagesPage(imgs));
                };
                img.GestureRecognizers.Add(tapGesture);

            }

        }

        private async void Webview_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (mIsEdit && mRecord != null && !string.IsNullOrWhiteSpace(mRecord.results))
            {
                await mRecord.EvaluateJavascript("javascript:setEditorValue('" + mRecord.results + "');");
            }

        }
    }
}
