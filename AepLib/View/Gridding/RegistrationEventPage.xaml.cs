using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class RegistrationEventPage : ContentPage
    {
        void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Switch SW = sender as Switch;
            SW.IsToggled = !SW.IsToggled;
            if (SW.IsToggled == true)
                _infoModel.state = 4;
            else _infoModel.state = 0;

        }

        private ObservableCollection<string> photoList = new ObservableCollection<string>();


        void UpData(object sender, System.EventArgs e){
            postAddEvent();
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
                labelLngLat.Text = _infoModel.lnglatString;
                getAddressWihtLocation();
            });
        }

        //相关企业
        void RelatedEnterPrises(object sender, System.EventArgs e)
        {

            Navigation.PushAsync(new RelatedEnterprisesPage());
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

                creatPhotoView();


            }


        }

        /// <summary>
        /// 根据拍照张数创建图片
        /// </summary>
        void creatPhotoView()
        {

            PickSK.Children.Clear();

            foreach (string path in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source = ImageSource.FromFile(path) as FileImageSource,
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




                //Image = new Image
                //{
                //    VerticalOptions = LayoutOptions.Center,
                //    HorizontalOptions = LayoutOptions.Start,
                //    Aspect =Aspect.Fill,


                //};


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
            EditContentsPage editContentsPage = new EditContentsPage(_infoModel, type);
            editContentsPage.Title = title;
            Navigation.PushAsync(editContentsPage);


        }


        GridEventInfoModel _infoModel = null;
        string _eventId = "";
        public RegistrationEventPage(string eventId)
        {
            InitializeComponent();

            _eventId = eventId;
            //
            if (!string.IsNullOrEmpty(_eventId)) getEventInfo();
            else
            {
                _infoModel = new GridEventInfoModel
                {
                    canEdit = true,
                    date = DateTime.Now,
                    staff = App.userInfo.id,
                    userName = App.userInfo.userName,
                  
                    state = 4,
                    type = 2,
                    addr = "梅墟",
                    title = "biaoti",
                    id = Guid.NewGuid(),
                    gridcell = App.gridUser.gridcell,

                };
                try{
                    _infoModel.lat = App.currentLocation.Latitude;
                    _infoModel.lng = App.currentLocation.Longitude; 
                }catch(Exception ex){
                    
                }



                BindingContext = _infoModel;
            }
            Title = "登记事件";
            setPosition();
            NavigationPage.SetBackButtonTitle(this, "");
            ToolbarItems.Add(new ToolbarItem("", "qrcode", HandleAction));
            ST.BindingContext = photoList;
            getAddressWihtLocation();

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



        private async void postAddEvent()
        {
            string url = App.EP360Module.url + "/api/gbm/updateincident";
            parameModel parame = new parameModel
            {
                id = _infoModel.id,
                rowState = "add",
                grid = _infoModel.gridcell,
                title = _infoModel.title,
                date = _infoModel.date,
                handleDate = DateTime.Now,
                contents = _infoModel.content,
                results = _infoModel.results,
                type = _infoModel.type,
                state = _infoModel.state,
                lat = _infoModel.lat,
                lng = _infoModel.lng,
                addr = _infoModel.addr,
                staff = App.userInfo.id,


            };
            string param = JsonConvert.SerializeObject(parame);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
               




            }

        }


        //获取事件详情
        private async void getEventInfo()
        {

            string url = App.EmergencyModule.url + "/api/gbm/GetIncidentDetail";

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "id="+_eventId, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _infoModel = JsonConvert.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                _infoModel.canEdit = false;
                BindingContext = _infoModel;
            }

        }

        //获取网格员信息
        private async void getStaffInfo()
        {

            string url = App.FrameworkURL + "/api/fw/GetUserByid?id="+_infoModel.staff;

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _infoModel = JsonConvert.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                BindingContext = _infoModel;
            }

        }

        //获取相关企业名称
        private async void getEnterprise()
        {

            string url = App.FrameworkURL + "/api/Modmanage/GetEnterpriseByid";

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "id="+_infoModel.enterprise, "GET", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _infoModel = JsonConvert.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                BindingContext = _infoModel;
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
                    _infoModel.addr = resultDic["address"].ToString();
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

            public string results{get;set;}

            public DateTime handleDate{get;set;}

            ObservableCollection<Attachments> attachments { set; get; }

        }


        public class Attachments{
            public string id { get; set; }
            public string rowState { get; set; }
        }


    }
}
