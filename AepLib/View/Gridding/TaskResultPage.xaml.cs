using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
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
        private ObservableCollection<string> photoList = new ObservableCollection<string>();

        /// <summary>
        /// 添加/更新任务处理记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void ExecutionRecord(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Editor.Text))
            {
                DependencyService.Get<IToast>().ShortAlert("请输入执行结果");
                return;
            }
            string url = App.EP360Module.url + "/api/gbm/updatetaskhandle";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", "");
            map.Add("rowState", mIsEdit ? "add" : "");
            map.Add("task", mTaskId);
            map.Add("date", DatePicker.Date);
            map.Add("staff", mRecord== null ? new Guid() : mRecord.staff);
            map.Add("results", Editor.Text);
            map.Add("forassignment", "");
            map.Add("attachments", "");
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string result = res.Results;
                    await DisplayAlert("result", result, "ok");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error", ex.Message, "ok");
                }
            }

        }


        public TaskResultPage(Guid taskId, GridTaskHandleRecordModel record, bool isEdit)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            mIsEdit = isEdit;
            mTaskId = taskId;
            mRecord = record;
            GetStaffInfo(record);
            InitResultInfo(record, isEdit);
        }

        private void InitResultInfo(GridTaskHandleRecordModel record, bool isEdit)
        {
            if (record == null)
            {
                return;
            }
            GridOperate.IsVisible = isEdit ? true : false;
            Editor.IsEnabled = isEdit;
            BindingContext = record;
            photoList.Clear();
            if (record.attachments != null && record.attachments.Count > 0)
            {
                foreach (var item in record.attachments)
                {
                    photoList.Add(item.attach_url);
                }
                creatPhotoView(true);
            }
            ST.BindingContext = photoList;
        }

        /// <summary>
        /// 获取执行人信息
        /// </summary>
        /// <param name="record"></param>
        private async void GetStaffInfo(GridTaskHandleRecordModel record)
        {
            if (record == null)
            {
                return;
            }
            UserInfoModel user = await (App.Current as App).GetUserInfo(record.staff);
            if (user != null)
            {
                LabelStaff.Text = user.userName;
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
                Name = System.DateTime.Now + ".jpg"
            });

            if (file == null)
            {
                return;
            }

            else
            {

                photoList.Add(file.Path);

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

            foreach (string path in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source = isFromNetwork ? ImageSource.FromUri(new Uri(path)) : ImageSource.FromFile(path) as FileImageSource,
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

    }
}
