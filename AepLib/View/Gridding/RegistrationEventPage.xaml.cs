using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using AepApp.Tools;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Xamarin.Forms;
using AepApp.Tools;
using System.Collections.Specialized;
using Sample;

namespace AepApp.View.Gridding
{
    public partial class RegistrationEventPage : ContentPage
    {

        private int UploadSuccessCount = 0;
        private ObservableCollection<AttachmentInfo> photoList = new ObservableCollection<AttachmentInfo>();
        private ObservableCollection<GridAttachmentUploadModel> uploadModel = new ObservableCollection<GridAttachmentUploadModel>();
        GridEventInfoModel _infoModel = null;
        string _eventId = "";

        private void pickerNature_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            _infoModel.type = ConstConvertUtils.GridEventType2Int(typeName);
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _infoModel.title = e.NewTextValue;     

        }

        //打电话
        void phone_Tapped(object sender, System.EventArgs e)
        {

            if (string.IsNullOrEmpty(_infoModel.Tel)) return;
            DeviceUtils.phone(_infoModel.Tel);          

        }
        //发信息
        void sms_Tapped(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(_infoModel.Tel)) return;
            DeviceUtils.sms(_infoModel.Tel);
        }


        /// <summary>
        /// 经纬度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventPositon(object sender, System.EventArgs e)
        {
            AccidentPositionPage page;
            if (_infoModel.lat == 0.0 || _infoModel.lng == 0.0)
            {
                page = new AccidentPositionPage(null, null);
            }
            else
            {
                page = new AccidentPositionPage(_infoModel.lng.ToString(), _infoModel.lat.ToString());
            }
            page.Title = "事件位置";
            Navigation.PushAsync(page);
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");
            MessagingCenter.Subscribe<ContentPage, string>(this, "savePosition", (s, arg) =>
            {
                var pos = arg as string;
                if (pos == null)
                {
                    return;
                }
                string[] p = pos.Replace("E", "").Replace("N", "").Replace("W", "").Replace("S", "").Split(",".ToCharArray());

                _infoModel.lng = Convert.ToDouble(p[0]);
                _infoModel.lat = Convert.ToDouble(p[1]);
                setPosition();
                labelLngLat.Text = _infoModel.LnglatString;
                getAddressWihtLocation();
            });
        }

        //相关企业
        void RelatedEnterPrises(object sender, System.EventArgs e)
        {

            Navigation.PushAsync(new RelatedEnterprisesPage(_infoModel));
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

                photoList.Add(new AttachmentInfo
                {
                    url = file.Path,
                    isUploaded = false,
                });
                creatPhotoView(false);


            }


        }

        /// <summary>
        /// 根据拍照张数创建图片
        /// </summary>
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

        //事件类型
        void selectEventType(object sender, System.EventArgs e){


        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            string type = "";
            string title = "";
            Button button = sender as Button;
            if(button == EditResult){
                title = "处理结果";
                type = "EditResult";
            }
            if (button == EditContents)
            {
                title = "事件描述";
                type = "EditContent";
            }
            EditContentsPage editContentsPage = new EditContentsPage(_infoModel, type,1);
            editContentsPage.Title = title;
            Navigation.PushAsync(editContentsPage);
        }



        public RegistrationEventPage(string eventId)
        {
            InitializeComponent();

            _eventId = eventId;
            //
            if (!string.IsNullOrEmpty(_eventId)) {
                getEventInfo();
                Title = "事件内容";
            }
            else
            {
                _infoModel = new GridEventInfoModel
                {
                    canEdit = true,
                    date = DateTime.Now,
                    staff = App.userInfo.id,
                    UserName = App.userInfo.userName,
                    state = 4,
                    type = 2,
                    id = Guid.NewGuid(),
                    gridcell = App.gridUser.grid,
                    Tel = App.userInfo.tel,


                };
                try{
                    _infoModel.lat = App.currentLocation.Latitude;
                    _infoModel.lng = App.currentLocation.Longitude; 
                }catch(Exception ex){
                    
                }

                BindingContext = _infoModel;
                Title = "登记事件";
                getAddressWihtLocation();
                GR.IsVisible = true;
            }

            setPosition();
            NavigationPage.SetBackButtonTitle(this, "");
            //ToolbarItems.Add(new ToolbarItem("", "qrcode", HandleAction));
            ST.BindingContext = photoList;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        void HandleAction()
        {
            //Navigation.PushAsync(new EventHandleProcessPage(eventModel));
        }

        private void setPosition()
        {
            //labelLngLat.Text = eventModel == null ? "" : eventModel.lnglatString;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 添加事件记录
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
                nameValue.Add("id", _infoModel.canEdit ? Guid.NewGuid().ToString() : _infoModel.id.ToString());
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
                postAddEvent();
            }
            else
            {
                DependencyService.Get<IToast>().LongAlert("图片上传失败，请重试");
            }
        }

        private async void postAddEvent()
        {

            if(string.IsNullOrEmpty(_infoModel.title)){
               await DisplayAlert("提示", "请填写事件名称", "确定");
                return;
            }

            if (SW.IsToggled == true)
                _infoModel.state = 3;
            else _infoModel.state = 2;


            string url = App.EP360Module.url + "/api/gbm/updateincident";
            parameModel parame = new parameModel
            {
                id = _infoModel.id.Value,
                rowState = "add",
                grid = _infoModel.gridcell.Value,
                title = _infoModel.title,
                date = _infoModel.date,
                handleDate = DateTime.Now,
                contents = _infoModel.contents,
                results = _infoModel.Results,
                type = _infoModel.type.Value,
                state = _infoModel.state.Value,
                lat = _infoModel.lat.Value,
                lng = _infoModel.lng.Value,
                addr = _infoModel.Addr,
                staff = App.userInfo.id,
                enterprise = _infoModel.enterprise,
                attachments = uploadModel,
            };
            string param = JsonConvert.SerializeObject(parame);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {

                if (hTTPResponse.Results == "\"OK\"")await Navigation.PopAsync();



            }

        }


        //获取事件详情
        private async void getEventInfo()
        {

            string url = App.EP360Module.url + "/api/gbm/GetIncidentDetail";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", _eventId);
            string pa = JsonConvert.SerializeObject(param);


            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url,pa , "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    
                    _infoModel = JsonConvert.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                    _infoModel.canEdit = false;
                    getAddressWihtLocation();
                    getStaffInfo();
                    getEnterprise();
                    BindingContext = _infoModel;
                    bottom.Height = 0;
                    GR.IsVisible = true;
                }
                catch (Exception ex)
                {
                    Navigation.PopAsync();
                }
            }

        }

        //获取网格员信息
        private async void getStaffInfo()
        {

            var auditor = await (App.Current as App).GetUserInfo(_infoModel.staff.Value);
            if (auditor != null)
            {
                _infoModel.Tel = auditor.telephone;
                _infoModel.UserName = auditor.userName;
            }
        }

        //获取相关企业名称
        private async void getEnterprise()
        {

            string url = App.BasicDataModule.url + "/api/Modmanage/GetEnterpriseByid";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", _infoModel.enterprise);
            string par = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, par, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var enterpriseModel = JsonConvert.DeserializeObject<GridEnterpriseModel>(hTTPResponse.Results);
                if(enterpriseModel !=null){
                    _infoModel.EnterpriseName = enterpriseModel.name;
                }
            }

        }

        //反地理编码
        private async void getAddressWihtLocation ()
        {
            string param = "";
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync("https://apis.map.qq.com/ws/geocoder/v1/?location="+_infoModel.lat+","+_infoModel.lng+"&key=72NBZ-3YWK2-XV3U7-CM7OL-MKPMK-DRF2B", param, "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Dictionary<string,object> dic = JsonConvert.DeserializeObject<Dictionary<string,object>>(hTTPResponse.Results);
                Dictionary<string,object> resultDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(dic["result"].ToString());
                try
                {
                    _infoModel.Addr = resultDic["address"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }


        private class parameModel{
            public Guid id{ get;set;}

            public string rowState{get; set;}

            public Guid grid{get;set;}

            public string title{get;set;}

            public Guid staff{get;set;}

            public DateTime date{get;set;}

            public string addr{get;set;}

            public double lng{get;set;}

            public double lat{get;set;}

            public int type{get;set;}

            public int state{get;set;}

            public string contents{get;set;}

            public string results { get; set; }
            public string enterprise { get; set; }
                
            public DateTime handleDate{get;set;}

            public ObservableCollection<GridAttachmentUploadModel> attachments { set; get; }

        }


  


    }
}
