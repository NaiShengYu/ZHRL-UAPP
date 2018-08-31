using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using AepApp.Interface;
using AepApp.Models;
using AepApp.Tools;
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
            GridOperate.IsVisible = mIsEdit ? true : false;
            GetStaffInfo();
            GetRecordDetail();
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
            UserInfoModel user = await (App.Current as App).GetUserInfo(mRecord.staff);
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
            if(res.StatusCode == System.Net.HttpStatusCode.OK)
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
                        //await DisplayAlert("上传结果", res.Results, "确定");
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
            var result = await mRecord.EvaluateJavascript("javascript:getEditorValue();");
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
            map.Add("date", DatePicker.Date);
            map.Add("staff", App.userInfo.id);
            map.Add("results", content);
            map.Add("forassignment", "");
            map.Add("attachments", JsonConvert.SerializeObject(uploadModel));
            await DisplayAlert("imgs", JsonConvert.SerializeObject(uploadModel), "ok");
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string result1 = res.Results;
                    await DisplayAlert("result", result1, "ok");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error", ex.Message, "ok");
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

            foreach (AttachmentInfo img in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source = isFromNetwork ? ImageSource.FromUri(new Uri(img.url)) : ImageSource.FromFile(img.url) as FileImageSource,
                    HeightRequest = 80,
                    WidthRequest = 80,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.Fill,
                };
                grid.Children.Add(button);

                if (100.0 * photoList.Count > App.ScreenWidth)
                    pickSCR.ScrollToAsync(100 * photoList.Count - (App.ScreenWidth), 0, true);

            }

        }

        private async void Webview_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (mRecord != null && !string.IsNullOrWhiteSpace(mRecord.results))
            {
                await mRecord.EvaluateJavascript("javascript:setEditorValue('" + mRecord.results + "');");
            }

        }
    }
}
