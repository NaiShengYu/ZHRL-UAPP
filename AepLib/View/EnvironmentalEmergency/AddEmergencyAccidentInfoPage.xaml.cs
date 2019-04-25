using AepApp.Interface;
using AepApp.Models;
using AepApp.Services;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Sample;
using SimpleAudioForms;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Xamarin.Essentials;
using Xamarin.Forms;
#if __IOS__
using Foundation;
using UIKit;
using CoreGraphics;
#endif
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddEmergencyAccidentInfoPage : ContentPage
    {

        //当前位置名称
        Xamarin.Essentials.Location _location = null;
        //获取当前位置
        async void HandleEventHandler()
        {
            try
            {
                //_location = await Geolocation.GetLastKnownLocationAsync();

                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                _location = await Geolocation.GetLocationAsync(request);


                if (_location != null)
                {
                    App.currentLocation = _location;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private ObservableCollection<UploadEmergencyShowModel> dataList = new ObservableCollection<UploadEmergencyShowModel>();
        private ObservableCollection<UploadEmergencyModel> saveList = new ObservableCollection<UploadEmergencyModel>();
        private bool isFirstAppear = true;
        private string emergencyId;
        void cellRightBut(object sender, System.EventArgs e)
        {
            if (isfunctionBarIsShow == true)
            {
                canceshiguxingzhi();
                return;
            }

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //如果在编辑事故性质
            if (isfunctionBarIsShow == true)
            {
                canceshiguxingzhi();
                listView.SelectedItem = null;
                return;
            }
            var item = e.SelectedItem as UploadEmergencyShowModel;

            if (item == null)
                return;
            if (item.category == "IncidentVideoSendingEvent")
            {
                Navigation.PushAsync(new ShowVideoPage(item));
            }

            listView.SelectedItem = null;

        }
        //左滑删除
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            var menu = sender as MenuItem;
            var item = menu.BindingContext as UploadEmergencyShowModel;
            var i = dataList.IndexOf(item);
            await App.Database.DeleteEmergencyAsync(saveList[i]);
            dataList.Remove(item);
        }
            //输入框点击了编辑按钮
        async void clickedReturnKey(object sender, System.EventArgs e)
        {
            var a = ENT.Text;
            if (string.IsNullOrWhiteSpace(a)) return;
            UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentMessageSendingEvent",emergencyId);
            emergencyModel.Content = a;
            UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
            AzmCoord center = new AzmCoord(Convert.ToDouble(emergencyModel.lng), Convert.ToDouble(emergencyModel.lat));
            LocateOnMap(showModel);

            saveUploadEmergencyModel(emergencyModel,showModel);
        }

        //点击了位置按钮
        async void AccidentPosition(object sender, System.EventArgs e)
        {
            AccidentPositionPage page = new AccidentPositionPage(App.EmergencyCenterCoord.lng.ToString(), App.EmergencyCenterCoord.lat.ToString());
            page.Title = "事故位置";
            page.SavePosition += async (object arg2, EventArgs arg1) =>
             {
                 var aaa = arg2 as string;
                 aaa = aaa.Replace("E", "");
                 aaa = aaa.Replace("N", "");
                 aaa = aaa.Replace("W", "");
                 aaa = aaa.Replace("S", "");
                 aaa = aaa.Replace(" ", "");
                 string[] bbb = aaa.Split(",".ToCharArray());
                 double lon = System.Convert.ToDouble(bbb[0]);
                 double lat = System.Convert.ToDouble(bbb[1]);
          
                 UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentLocationSendingEvent",emergencyId);
                 emergencyModel.TargetLat = lat;
                 emergencyModel.TargetLng = lon;
                 UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);

                 AzmCoord center = new AzmCoord(emergencyModel.TargetLng.Value, emergencyModel.TargetLat.Value);
                 LocateOnMap(showModel);

                 saveUploadEmergencyModel(emergencyModel, showModel);

             };

            await Navigation.PushAsync(page);

        }

        //点击了录音按钮
        string _filename;
        string voisePath;
        bool isRecording = false;
        void recordVoice(object sender, System.EventArgs e)
        {
            try
            {
                if (isRecording == false)
                {
                    isRecording = true;
                    var dir = DependencyService.Get<IFileService>().GetExtrnalStoragePath(Constants.STORAGE_TYPE_MUSIC)+"/";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    //存储文件名
                    voisePath = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
                    _filename = Path.Combine(dir, voisePath);
                    Console.WriteLine(_filename);
                    DependencyService.Get<IRecordVoice>().startRecord(_filename);
                }
                else
                {
                    isRecording = false;
                    var time = DependencyService.Get<IRecordVoice>().stopRecord(_filename);

                    UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentVoiceSendingEvent",emergencyId);
                    emergencyModel.VoicePath = voisePath;
                    emergencyModel.VoiceStorePath = voisePath;
                    emergencyModel.Length = time;

                    UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
                    showModel.VoicePath = _filename;
                    showModel.VoiceStorePath = _filename;

                    showModel.PlayVoiceCommand = new Command(() =>
                    {
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        var dir = showModel.VoicePath;
                        DependencyService.Get<IAudio>().PlayLocalFile(dir);
                    });

                    saveUploadEmergencyModel(emergencyModel, showModel);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("errer：" + ex.Message);
            }
        }


#pragma mark --点击事故性质按钮一系列操作开始
        //点击了事故性质按钮
        bool isfunctionBarIsShow = false;
        void showshiguxingzhi(object sender, System.EventArgs e)
        {
            //entryStack.TranslateTo(0, 260);
            b2.TranslateTo(0, 260);
            aaaa.Height = 0;
            bbbb.Height = 75;
            functionBar.TranslateTo(0, -130);
            isfunctionBarIsShow = true;
            //记录上一次的选择
            UploadEmergencyShowModel IdentificationModel = App.LastNatureAccidentModel;
            foreach (UploadEmergencyShowModel emergencyModel1 in dataList)
            {
                if (emergencyModel1.category == "IncidentNatureIdentificationEvent" && emergencyModel1.emergencyid == emergencyId)
                {
                    IdentificationModel = emergencyModel1;
                }
            }
            if (IdentificationModel != null)
            {
                var dq = IdentificationModel.natureString.Substring(0, 1);
                var sz = IdentificationModel.natureString.Substring(1, 1);
                var tr = IdentificationModel.natureString.Substring(2, 1);
                if (dq == "0")
                {
                    isSelectDQ = false;
                    dqBut.BackgroundColor = Color.Transparent;
                }
                else
                {
                    isSelectDQ = true;
                    dqBut.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
                }
                if (sz == "0")
                {
                    isSelectSZ = false;
                    szBut.BackgroundColor = Color.Transparent;
                }
                else
                {
                    isSelectSZ = true;
                    szBut.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
                }
                if (tr == "0")
                {
                    isSelectTR = false;
                    trBut.BackgroundColor = Color.Transparent;
                }
                else
                {
                    isSelectTR = true;
                    trBut.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
                }
            }

        }
        //退出事故性质编辑
        void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (isfunctionBarIsShow == true)
            {
                canceshiguxingzhi();
            }
        }
        //选中了大气
        bool isSelectDQ = false;
        bool isSelectSZ = false;
        bool isSelectTR = false;
        void selectDQ(object sender, System.EventArgs e)
        {
            isSelectDQ = !isSelectDQ;
            var but = sender as Button;
            but.BackgroundColor = isSelectDQ == true ? Color.FromRgba(0, 0, 0, 0.2) : Color.Transparent;
        }
        void selectSZ(object sender, System.EventArgs e)
        {
            isSelectSZ = !isSelectSZ;
            var but = sender as Button;
            but.BackgroundColor = isSelectSZ == true ? Color.FromRgba(0, 0, 0, 0.2) : Color.Transparent;
        }
        void selectTR(object sender, System.EventArgs e)
        {
            isSelectTR = !isSelectTR;
            var but = sender as Button;
            but.BackgroundColor = isSelectTR == true ? Color.FromRgba(0, 0, 0, 0.2) : Color.Transparent;

        }

        //完成选择事故性质
        async void finishishiguxingzhi(object sender, System.EventArgs e)
        {
            canceshiguxingzhi();
            string a = "";
            a += isSelectDQ == true ? "1" : "0";
            a += isSelectSZ == true ? "1" : "0";
            a += isSelectTR == true ? "1" : "0";
            //如果什么都不选
            if (a == "000") return;
            //如果和上次选的一样
            UploadEmergencyShowModel IdentificationModel = null;
            foreach (UploadEmergencyShowModel emergencyModel1 in dataList)
            {
                if (emergencyModel1.category == "IncidentNatureIdentificationEvent" && emergencyModel1.emergencyid == emergencyId)
                {
                    IdentificationModel = emergencyModel1;
                }
            }
            //如果原来有，并且和a新选的相同
            if (IdentificationModel != null && IdentificationModel.natureString == a) return;
            UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentNatureIdentificationEvent",emergencyId);
            emergencyModel.natureString = a;
            UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
            saveUploadEmergencyModel(emergencyModel, showModel);

        }

        void canceshiguxingzhi()
        {
            //entryStack.TranslateTo(0, 0);
            b2.TranslateTo(0, 0);
            aaaa.Height = 55;
            bbbb.Height = 150;
            functionBar.TranslateTo(0, 0);
            if (dataList.Count > 0) listView.ScrollTo(dataList[dataList.Count - 1], ScrollToPosition.End, true);

        }

#pragma 点击事故性质按钮一系列操作结束


        //点击了数据按钮
        void addShuju(object sender, System.EventArgs e)
        {
            Button but = sender as Button;
            if (App.contaminantsList == null && App.AppLHXZList == null) return;
            MessagingCenter.Unsubscribe<ContentPage, AddDataForChemicolOrLHXZModel.ItemsBean>(this, "AddLHXZ");

            MessagingCenter.Subscribe<ContentPage, AddDataForChemicolOrLHXZModel.ItemsBean>(this, "AddLHXZ", async (arg1, arg2) =>
            {
                AddDataForChemicolOrLHXZModel.ItemsBean item = arg2 as AddDataForChemicolOrLHXZModel.ItemsBean;
              
                UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentFactorMeasurementEvent",emergencyId);
                emergencyModel.factorId = item.factorId;
                emergencyModel.factorName = item.factorName;
                emergencyModel.unitName = item.unitName;
                emergencyModel.factorValue = item.jianCeZhi;
                emergencyModel.unitId = Guid.NewGuid().ToString();
                emergencyModel.lat = Convert.ToDouble(item.lat);
                emergencyModel.lng = Convert.ToDouble(item.lng);

                if (item.yangBenLeiXing == "大气") emergencyModel.incidentNature = "4";
                else if (item.yangBenLeiXing == "水质") emergencyModel.incidentNature = "2";
                else if (item.yangBenLeiXing == "土壤") emergencyModel.incidentNature = "1";
                else emergencyModel.incidentNature = "8";

                UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);

                LocateOnMap(showModel);
                saveUploadEmergencyModel(emergencyModel, showModel);

                MessagingCenter.Unsubscribe<ContentPage, AddDataForChemicolOrLHXZModel.ItemsBean>(this, "AddLHXZ");
            });
            MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");

            MessagingCenter.Subscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", async (arg1, arg2) =>
            {
                AddDataIncidentFactorModel.ItemsBean item = arg2 as AddDataIncidentFactorModel.ItemsBean;
               
                UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentFactorIdentificationEvent",emergencyId);
                emergencyModel.factorId = item.factorId;
                emergencyModel.factorName = item.factorName;
                emergencyModel.datatype = Convert.ToInt32(item.dataType);

                UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);

                saveUploadEmergencyModel(emergencyModel, showModel);

                MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");
            });
            Navigation.PushAsync(new addDataPage());

        }

        //点击了风速风向按钮
        async void fengSuFengXiang(object sender, System.EventArgs e)
        {

            await Navigation.PushAsync(new WindSpeedAndDirectionPage());
            MessagingCenter.Unsubscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection");
            MessagingCenter.Subscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection", async (arg1, arg2) =>
            {
                string speed = arg2[0];
                string direction = arg2[1];

                UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentWindDataSendingEvent",emergencyId);
                emergencyModel.direction = direction;
                emergencyModel.speed = speed;

                UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
                LocateOnMap(showModel);
                saveUploadEmergencyModel(emergencyModel, showModel);

            });
        }

        async void LocateOnMap(UploadEmergencyShowModel showModel) {
            string title = EmergencyUtils.LocateOnMap(showModel);
            AzmCoord center = new AzmCoord(Convert.ToDouble(showModel.lng), Convert.ToDouble(showModel.lat));
            showModel.LocateOnMapCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new RescueSiteMapPage(title, center));
            });
        }

        //点击了污染物按钮
        void wuRanWu(object sender, System.EventArgs e)
        {
            MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");
            MessagingCenter.Subscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", (arg1, arg2) =>
            {
                AddDataIncidentFactorModel.ItemsBean item = arg2 as AddDataIncidentFactorModel.ItemsBean;

                UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentFactorIdentificationEvent",emergencyId);
                emergencyModel.factorId = item.factorId;
                emergencyModel.factorName = item.factorName;
                emergencyModel.datatype = Convert.ToInt32(item.dataType);

                UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
                saveUploadEmergencyModel(emergencyModel, showModel);

                MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");
            });

            Navigation.PushAsync(new ChemicalPage(2));




        }
        //点击了拍照
        async void paiZhao(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                CompressionQuality = 50,
                Directory = "Sample",
                Name = imageName,
                SaveToAlbum = true,
            });
            if (file == null)
            {
                return;
            }
            else
            {
                var bm = SKBitmap.Decode(file.Path);
                var info = new FileInfo(file.Path);
                UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentPictureSendingEvent",emergencyId);
                emergencyModel.StorePath = "Sample/" + imageName;
                emergencyModel.imagePath = "Sample/" + imageName;

                UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
                showModel.StorePath = file.Path;
                showModel.imagePath = file.Path;
                saveUploadEmergencyModel(emergencyModel, showModel);
            }
        }
        //点击了录制视频
        async void recordVideo(object sender, System.EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                string videoName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
                string imgName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_thumb.jpg";
                var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
                {
                    DesiredLength = new TimeSpan(0, 0, 10),
                    Name = videoName,
                    Directory = "Video",
                    SaveToAlbum = true,
                    CompressionQuality = 92,
                    Quality = Plugin.Media.Abstractions.VideoQuality.High,

                });
                if (file == null || string.IsNullOrWhiteSpace(file.Path)) return;
                string thumbPath = FileUtils.SaveThumbImage(file.AlbumPath, imgName);
                string _videoPath = FileUtils.VidioTranscoding(file.AlbumPath, videoName);//完整路径
                RecordBackSuccess(_videoPath, videoName, imgName);
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IAudio>().TakeVideo();
                MessagingCenter.Unsubscribe<ContentPage, string>(this, "RecordVideo");
                MessagingCenter.Subscribe<ContentPage, string>(this, "RecordVideo", async (arg1, arg2) =>
                {
                    string _videoPartPath = arg2 as string;//相对路径
                    if (string.IsNullOrWhiteSpace(_videoPartPath)) return;
                    string imgName = FileUtils.GetFileName(_videoPartPath, false) + "_thumb.jpg";
                    string dirPath = DependencyService.Get<IFileService>().GetExtrnalStoragePath(Constants.STORAGE_TYPE_MOVIES) + "/";
                    MessagingCenter.Unsubscribe<ContentPage, string>(this, "RecordVideo");
                    RecordBackSuccess(dirPath + _videoPartPath, _videoPartPath, imgName);
                });
            }
        }


        /// <summary>
        /// 视频拍摄后，插入数据并更新UI
        /// </summary>
        /// <param name="videoTotalPath">视频完整路径</param>
        /// <param name="_videoPartPath">相对路径</param>
        /// <param name="imgName"></param>
        private async void RecordBackSuccess(string videoTotalPath, string _videoPartPath, string imgName)
        {
            if (string.IsNullOrWhiteSpace(videoTotalPath) || string.IsNullOrWhiteSpace(_videoPartPath) || string.IsNullOrWhiteSpace(imgName)) return;
            string thumbPath = FileUtils.SaveThumbImage(_videoPartPath, imgName);

            UploadEmergencyModel emergencyModel = EmergencyUtils.creatingUploadEmergencyModel("IncidentVideoSendingEvent",emergencyId);
            emergencyModel.VideoPath = _videoPartPath;
            emergencyModel.VideoStorePath = _videoPartPath;
            emergencyModel.CoverPath = imgName;

            UploadEmergencyShowModel showModel = EmergencyUtils.creatingUploadEmergencyShowModel(emergencyModel);
            showModel.VideoPath = videoTotalPath;
            showModel.VideoStorePath = videoTotalPath;
            showModel.CoverPath = thumbPath;
            saveUploadEmergencyModel(emergencyModel, showModel);
        }

        //添加布点
        async void AddPlacement(object sender, System.EventArgs e)
        {
            AccidentPositionPage page = new AccidentPositionPage(null, null, "布点位置");
            page.Title = "布点位置";
            page.SavePosition += (arg2, arg1) =>
            {
                var aaa = arg2 as string;
                aaa = aaa.Replace("E", "");
                aaa = aaa.Replace("N", "");
                aaa = aaa.Replace("W", "");
                aaa = aaa.Replace("S", "");
                aaa = aaa.Replace(" ", "");
                string[] bbb = aaa.Split(",".ToCharArray());
                double lon = Convert.ToDouble(bbb[0]);
                double lat = Convert.ToDouble(bbb[1]);
                Navigation.InsertPageBefore(new AddPlacementPage(emergencyId, bbb[1], bbb[0]), page);
            };
            await Navigation.PushAsync(page);
        }

        public AddEmergencyAccidentInfoPage(string id)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            emergencyId = id;

            //进入上传数据界面先清空contaminantsList
            App.contaminantsList = null;
            App.AppLHXZList = null;
            EmergencyUtils.ReqaddData();

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DependencyService.Get<IAudio>().stopPlay();
            MessagingCenter.Unsubscribe<ContentPage, UploadEmergencyShowModel>(this, "deleteUnUploadData");
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged");
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "RecordVideo");
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //键盘高度改变
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged");
            MessagingCenter.Subscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged", (ContentPage arg1, KeyboardSizeModel arg2) =>
            {
                Console.WriteLine(arg2.Height);
                Console.WriteLine(arg2.Wight);
                if (arg2.Y != App.ScreenHeight)
                    cccc.Height = 55 - (206 - arg2.Height);
                else
                    cccc.Height = 55;
            });

            //删除不要的错误的数据
            MessagingCenter.Subscribe<ContentPage, UploadEmergencyShowModel>(this, "deleteUnUploadData", (ContentPage arg1, UploadEmergencyShowModel arg2) =>
            {
                var i = dataList.IndexOf(arg2);
                try
                {
                    App.Database.DeleteEmergencyAsync(saveList[i]);
                    saveList.Remove(saveList[i]);
                }
                catch (Exception ex)
                {
                }
               
                dataList.Remove(arg2);

            });

            //请求数据库数据
            // App.Database.CreatEmergencyTable();
            if (isFirstAppear)
            {
                List<UploadEmergencyModel> dataList2 = await App.Database.GetEmergencyAsync();
                int count = dataList2.Count;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                foreach (UploadEmergencyModel model in dataList2)
                {
                    if (!string.IsNullOrWhiteSpace(model.category))
                    {
                        string data = JsonConvert.SerializeObject(model);
                        UploadEmergencyShowModel ShowModel = Tools.JsonUtils.DeserializeObject<UploadEmergencyShowModel>(data);
                        if(ShowModel == null)
                        {
                            continue;
                        }
                        ShowModel.isEdit = true;
                        dataList.Add(ShowModel);
                        saveList.Add(model);
                        string cagy = model.category;
                        LocateOnMap(ShowModel);
                        EmergencyUtils.GetLocalData(ShowModel);
                    }
                }
                listView.ItemsSource = dataList;
                isFirstAppear = false;
                if (dataList.Count > 0) listView.ScrollTo(dataList[dataList.Count - 1], ScrollToPosition.End, true);
                else EmergencyUtils.DeleteLocalImageAndVideo();
            }

        }


        private void uploadData(object sender, EventArgs e)
        {
            int count = dataList.Count;
            for (int i = 0; i < count; i++)
            {
                UploadEmergencyShowModel model = dataList[i];
                Dictionary<string, object> dic = EmergencyUtils.RequestDictionary(model);
                if (model.uploadStatus == "UploadedOver") continue;//如果已经上传的就跳过
                model.uploadStatus = "uploading";
                switch (model.category)
                {   //事故位置
                    case "IncidentLocationSendingEvent":
                        UpLocalData(model, dic, "AppendIncidentLocationSendingEvent");
                        break;
                    //化学因子名称
                    case "IncidentFactorIdentificationEvent":
                        PostFactorIdentificationSendingSecond(model,dic);
                        break;
                        //文字
                    case "IncidentMessageSendingEvent":
                         UpLocalData(model, dic, "AppendIncidentMessageSendingEvent");
                        break;
                        //风
                    case "IncidentWindDataSendingEvent":
                            UpLocalData(model, dic, "AppendIncidentWindDataSendingEvent");
                        break;
                    //事故性质
                    case "IncidentNatureIdentificationEvent":
                            UpLocalData(model,dic, "AppendIncidentNatureIdentificationEvent");
                        break;
                    //添加化学因子检测值
                    case "IncidentFactorMeasurementEvent":
                            UpLocalData(model,dic, "AppendIncidentFactorMeasurementEvent");
                        break;
                    //图片
                    case "IncidentPictureSendingEvent":
                        PostupLoadImageSending(model,dic);
                        break;
                    //视频
                    case "IncidentVideoSendingEvent":
                            PostupLoadVideoSending(model,dic);
                        break;
                    case "IncidentVoiceSendingEvent": 
                            PostupLoadVoiceSending(model,dic);
                        break;
                }
            }
        }
        private async void PostNatureIdentificationSecond(UploadEmergencyShowModel model)
        {
            EmergencyAccidentPageModels.ItemsBean parameter = new EmergencyAccidentPageModels.ItemsBean();
            parameter = App.EmergencyAccidengtModel;
            parameter.natureString = model.natureString;

            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/Incident/Update", param, "PUT", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                deleteDataFromDB(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        //上传新的化学因子
        private async void PostFactorIdentificationSendingSecond(UploadEmergencyShowModel model, Dictionary<string, object> dic1)
        {

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("factorId", model.factorId);
            dic.Add("factorName", model.factorName);
            dic.Add("dataType", model.datatype);
            dic.Add("incidentId", model.emergencyid);

            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentFactor/Append", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UpLocalData(model, dic1, "AppendIncidentFactorIdentificationEvent");
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }
        //上传图片
        private async void PostupLoadImageSending(UploadEmergencyShowModel model, Dictionary<string, object> dic)
        {
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.StorePath, ".png", App.EmergencyModule.url, ApiUtils.UPLOAD_EMERGENCY_API);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                uploadImageResurt resultData = Tools.JsonUtils.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if (resultData == null || resultData.result == null) return;
                if (resultData.result.Count > 0)
                {
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    dic.Add("width", (int)imageResurtData.width);
                    dic.Add("height", (int)imageResurtData.height);
                    dic.Add("storePath", imageResurtData.storeUrl);
                    UpLocalData(model, dic, "AppendIncidentPictureSendingEvent");
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }

        //上传视频封面
        private async void PostupLoadVideoCoverSending(UploadEmergencyShowModel model, Dictionary<string, object> dic)
        {
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.CoverPath, FileUtils.GetFileName(model.VideoStorePath, false) + ".jpg",
                App.EmergencyModule.url, ApiUtils.UPLOAD_COVER);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UpLocalData(model, dic, "AppendIncidentVideoSendingEvent");
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        //上传视频
        private async void PostupLoadVideoSending(UploadEmergencyShowModel model,Dictionary<string,object> dic)
        {
            if (model == null) return;
            string path = model.VideoStorePath;
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(path, ".mp4", App.EmergencyModule.url, ApiUtils.UPLOAD_EMERGENCY_API);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                uploadImageResurt resultData = Tools.JsonUtils.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if (resultData == null || resultData.result == null) return;
                if (resultData.result.Count > 0)
                {
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    if (imageResurtData != null)
                    {
                        model.VideoStorePath = imageResurtData.storeUrl;
                        dic.Add("storePath", model.VideoStorePath);
                        PostupLoadVideoCoverSending(model,dic);
                    }
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }

        //上传录音
        private async void PostupLoadVoiceSending(UploadEmergencyShowModel model, Dictionary<string, object> dic)
        {
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.VoiceStorePath, ".mp4", App.EmergencyModule.url, ApiUtils.UPLOAD_EMERGENCY_API);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                uploadImageResurt resultData = Tools.JsonUtils.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if (resultData == null || resultData.result == null) return;
                if (resultData.result.Count > 0)
                {
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    dic.Add("storePath", imageResurtData.storeUrl);
                    UpLocalData(model, dic, "AppendIncidentVoiceSendingEvent");
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
                model.uploadStatus = "notUploaded";
                DependencyService.Get<IToast>().ShortAlert("上传失败");
            }
        }

        //上传数据
        private async void UpLocalData(UploadEmergencyShowModel model, Object dic,string url)
        {
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/" + url, param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (url == "AppendIncidentNatureIdentificationEvent")
                    PostNatureIdentificationSecond(model);
                else deleteDataFromDB(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }
        //保存数据
        private async void saveUploadEmergencyModel(UploadEmergencyModel emergencyModel, UploadEmergencyShowModel showModel)
        {
            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(showModel);
            saveList.Add(emergencyModel);
            listView.ScrollTo(showModel, ScrollToPosition.End, true);
            await entryStack.TranslateTo(0, 0);
            ENT.Text = "";
        }
        //上传完成后删除数据库数据
        private async void deleteDataFromDB(UploadEmergencyShowModel model) {
            var i = dataList.IndexOf(model);
            await App.Database.DeleteEmergencyAsync(saveList[i]);
            model.uploadStatus = "UploadedOver";
        }
    }
    //上传图片返回结果
    internal class uploadImageResurtData
    {
        public string storeUrl { get; set; }
        public string format { get; set; }
        public double size { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }
    internal class uploadImageResurt
    {
        public List<uploadImageResurtData> result { get; set; }
    }
}
