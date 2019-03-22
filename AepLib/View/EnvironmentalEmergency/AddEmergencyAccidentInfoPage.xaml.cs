using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Plugin.Media;
using CloudWTO.Services;
using static AepApp.Models.EmergencyAccidentInfoDetail;
using AepApp.Models;
using System.Threading.Tasks;
using Xamarin.Essentials;

using Todo;
using System.Net;
using Newtonsoft.Json;
using Plugin.AudioRecorder;
using System.IO;

using SkiaSharp;
using SimpleAudioForms;
using InTheHand.Forms;
using AepApp.Interface;
using AepApp.Tools;
using AepApp.AuxiliaryExtension;
using Sample;
#if __IOS__
using Foundation;
using UIKit;
using CoreGraphics;
#endif
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddEmergencyAccidentInfoPage : ContentPage
    {
       


        //void Handle_PanUpdated(object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        //{

        //    switch(e.StatusType)
        //    {
        //        case GestureStatus.Started:
                   
        //            break;
        //        case GestureStatus.Running:
                    
        //            break;
        //        case GestureStatus.Completed:
                    
        //            break;

        //    }
        
        
        
        //}

       

        //当前位置名称
        Location _location = null;
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
        private ObservableCollection<UploadEmergencyShowModel> dataListDelete = new ObservableCollection<UploadEmergencyShowModel>();
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
            if (item.category == "IncidentVideoSendingEvent"){
                Navigation.PushAsync(new ShowVideoPage(item));
            } 
         
            listView.SelectedItem = null;

        }


      

      //输入框点击了编辑按钮
        async void clickedReturnKey(object sender, System.EventArgs e)
        {
            var a = ENT.Text;
            if (string.IsNullOrWhiteSpace(a)) return;
            UploadEmergencyModel emergencyModel = new UploadEmergencyModel
            {
                uploadStatus = "notUploaded",
                Title = "",
                Content = a,
                creationTime = System.DateTime.Now,
                emergencyid = emergencyId,
                category = "IncidentMessageSendingEvent",
                creatorusername = App.userInfo.userName,
            };
            try
            {
                emergencyModel.lat = App.currentLocation.Latitude;
                emergencyModel.lng = App.currentLocation.Longitude;
            }
            catch (Exception)
            {
                emergencyModel.lat = 0;
                emergencyModel.lng = 0;
            }

            UploadEmergencyShowModel showModel = new UploadEmergencyShowModel
            {
                uploadStatus = "notUploaded",
                Title = "",
                Content = a,
                creationTime = System.DateTime.Now,
                emergencyid = emergencyId,
                category = "IncidentMessageSendingEvent",
                lat = emergencyModel.lat,
                lng = emergencyModel.lng,
                isEdit = true,
                creatorusername = App.userInfo.userName,
            };
            AzmCoord center = new AzmCoord(Convert.ToDouble(emergencyModel.lng), Convert.ToDouble(emergencyModel.lat));
            showModel.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("文字信息发出位置", center)); });

            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(showModel);
            dataListDelete.Add(showModel);
            saveList.Add(emergencyModel);
            await entryStack.TranslateTo(0, 0);
            ENT.Text = "";
            listView.ScrollTo(showModel, ScrollToPosition.End, true);
        }
      

        //点击了位置按钮
        async void AccidentPosition(object sender, System.EventArgs e)
        {
            AccidentPositionPage page = new AccidentPositionPage(null, null);
            page.Title = "事故位置";
            await Navigation.PushAsync(page);
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");

            MessagingCenter.Subscribe<ContentPage, string>(this, "savePosition", async (arg1, arg2) =>
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

                MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    TargetLat = lat,
                    TargetLng = lon,
                    emergencyid = emergencyId,
                    category = "IncidentLocationSendingEvent",
                    creatorusername = App.userInfo.userName,
                };
                try
                {
                    emergencyModel.lat = App.currentLocation.Latitude;
                    emergencyModel.lng = App.currentLocation.Longitude;
                }
                catch (Exception)
                {
                    emergencyModel.lat = 0;
                    emergencyModel.lng = 0;
                }

                UploadEmergencyShowModel showModel = new UploadEmergencyShowModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = emergencyModel.creationTime,
                    TargetLat = lat,
                    TargetLng = lon,
                    emergencyid = emergencyId,
                    category = "IncidentLocationSendingEvent",
                    lat = emergencyModel.lat,
                    lng = emergencyModel.lng,
                    isEdit = true,
                    creatorusername = App.userInfo.userName,
                };
                AzmCoord center = new AzmCoord(emergencyModel.TargetLng.Value, emergencyModel.TargetLat.Value);
                showModel.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("事故中心点", center)); });
              
                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(showModel);
                dataListDelete.Add(showModel);
                saveList.Add(emergencyModel);
                listView.ScrollTo(showModel, ScrollToPosition.End, true);
            });
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


        //点击了录音按钮
        string _filename;
        string voisePath;
        bool isRecording = false;
         void recordVoice(object sender, System.EventArgs e)
        {
            try{
                if(isRecording ==false){
                    isRecording = true;
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var dir = path + "/Voice/";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    //存储文件名
                    string name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
                    voisePath= "/Voice/"+name;
                    _filename = Path.Combine(dir, name);
                Console.WriteLine(_filename);
                DependencyService.Get<IRecordVoice>().startRecord(_filename);
                } 
                else {

                    isRecording = false;

                    var time = DependencyService.Get<IRecordVoice>().stopRecord(_filename);

                    UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                    {
                        uploadStatus = "notUploaded",
                        creationTime = System.DateTime.Now,
                        VoicePath = voisePath,
                        VoiceStorePath = voisePath,
                        emergencyid = emergencyId,
                        Length = time,
                        category = "IncidentVoiceSendingEvent",
                        creatorusername = App.userInfo.userName,

                    };
                    try
                    {
                        emergencyModel.lat = App.currentLocation.Latitude;
                        emergencyModel.lng = App.currentLocation.Longitude;
                    }
                    catch (Exception)
                    {
                        emergencyModel.lat = 0;
                        emergencyModel.lng = 0;
                    }
                    UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                    {
                        uploadStatus = "notUploaded",
                        creationTime = emergencyModel.creationTime,
                        VoicePath = _filename,
                        VoiceStorePath = _filename,
                        emergencyid = emergencyId,
                        category = "IncidentVoiceSendingEvent",
                        lat = emergencyModel.lat,
                        lng = emergencyModel.lng,
                        Length = time,
                        isEdit = true,
                        creatorusername = App.userInfo.userName,

                    };
                    ShowModel.PlayVoiceCommand = new Command( () => { 
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        var dir = path + ShowModel.VoicePath;
                        DependencyService.Get<IAudio>().PlayLocalFile(dir);
                    });

                    App.Database.SaveEmergencyAsync(emergencyModel);
                    dataList.Add(ShowModel);
                    dataListDelete.Add(ShowModel);
                    saveList.Add(emergencyModel);
                    listView.ScrollTo(ShowModel, ScrollToPosition.End, true);
                }
            }catch (Exception ex){
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
            if(IdentificationModel !=null){
                var dq = IdentificationModel.natureString.Substring(0,1);
                var sz = IdentificationModel.natureString.Substring(1,1);
                var tr = IdentificationModel.natureString.Substring(2,1);
                if(dq =="0"){
                    isSelectDQ = false;
                    dqBut.BackgroundColor = Color.Transparent;
                }
                else {
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
            if (isSelectDQ == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }
        void selectSZ(object sender, System.EventArgs e)
        {
            isSelectSZ = !isSelectSZ;
            var but = sender as Button;
            if (isSelectSZ == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }
        void selectTR(object sender, System.EventArgs e)
        {
            isSelectTR = !isSelectTR;
            var but = sender as Button;
            if (isSelectTR == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }

        //完成选择事故性质
        async void finishishiguxingzhi(object sender, System.EventArgs e)
        {
            canceshiguxingzhi();
            string a = "";
            if (isSelectDQ)
            {
                a += "1";
            }
            else
            {
                a += "0";
            }
            if (isSelectSZ)
            {
                a += "1";
            }
            else
            {
                a += "0";
            }
            if (isSelectTR)
            {
                a += "1";
            }
            else
            {
                a += "0";
            }
          
            //如果什么都不选
            if (a == "000") return;
            //如果和上次选的一样
            UploadEmergencyShowModel IdentificationModel = null;
            foreach(UploadEmergencyShowModel emergencyModel1 in dataList){
                if(emergencyModel1.category == "IncidentNatureIdentificationEvent" && emergencyModel1.emergencyid ==emergencyId){
                    IdentificationModel = emergencyModel1;
                }
            }
            //如果原来有，并且和a新选的相同
            if (IdentificationModel != null && IdentificationModel.natureString == a) return;

            UploadEmergencyModel emergencyModel = new UploadEmergencyModel
            {
                uploadStatus = "notUploaded",
                creationTime = System.DateTime.Now,
                natureString = a,
                emergencyid = emergencyId,
                category = "IncidentNatureIdentificationEvent",
                creatorusername = App.userInfo.userName,
            };
            try
            {
                emergencyModel.lat = App.currentLocation.Latitude;
                emergencyModel.lng = App.currentLocation.Longitude;
            }
            catch (Exception)
            {
                emergencyModel.lat = 0;
                emergencyModel.lng = 0;
            }

            UploadEmergencyShowModel showModel = new UploadEmergencyShowModel
            {
                uploadStatus = "notUploaded",
                creationTime = System.DateTime.Now,
                natureString = a,
                emergencyid = emergencyId,
                category = "IncidentNatureIdentificationEvent",
                lat = emergencyModel.lat,
                lng = emergencyModel.lng,
                   isEdit = true,
                creatorusername = App.userInfo.userName,
            };

            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(showModel);
            dataListDelete.Add(showModel);
            saveList.Add(emergencyModel);
            listView.ScrollTo(showModel, ScrollToPosition.End, true);

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
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    emergencyid = emergencyId,
                    category = "IncidentFactorMeasurementEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                    unitName = item.unitName,
                    factorValue = item.jianCeZhi,
                    //暂定0
                    unitId = Guid.NewGuid().ToString(),
                    creatorusername = App.userInfo.userName,

                };
                try
                {
                    emergencyModel.lat = Convert.ToDouble(item.lat);
                    emergencyModel.lng = Convert.ToDouble(item.lng);
                }
                catch (Exception)
                {
                    emergencyModel.lat = 0;
                    emergencyModel.lng = 0;
                }
                await App.Database.SaveEmergencyAsync(emergencyModel);
                if (item.yangBenLeiXing == "大气") emergencyModel.incidentNature = "4";
                else if (item.yangBenLeiXing == "水质") emergencyModel.incidentNature = "2";
                else if (item.yangBenLeiXing == "土壤") emergencyModel.incidentNature = "1";
                else emergencyModel.incidentNature = "8";

                UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = emergencyModel.creationTime,
                    emergencyid = emergencyId,
                    category = "IncidentFactorMeasurementEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                    unitName = item.unitName,
                    factorValue = item.jianCeZhi,
                    //暂定0
                    unitId = Guid.NewGuid().ToString(),
                    incidentNature = emergencyModel.incidentNature,
                    lat = emergencyModel.lat,
                    lng = emergencyModel.lng,
                    isEdit = true,
                    creatorusername = App.userInfo.userName,

                };

                AzmCoord center = new AzmCoord(Convert.ToDouble(emergencyModel.lng), Convert.ToDouble(emergencyModel.lat));
                ShowModel.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("文字信息发出位置", center)); });
                         
                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(ShowModel);
                dataListDelete.Add(ShowModel);
                saveList.Add(emergencyModel);
                listView.ScrollTo(ShowModel, ScrollToPosition.End, true);

                MessagingCenter.Unsubscribe<ContentPage, AddDataForChemicolOrLHXZModel.ItemsBean>(this, "AddLHXZ");
            });
            MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");

            MessagingCenter.Subscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", async (arg1, arg2) =>
            {
                AddDataIncidentFactorModel.ItemsBean item = arg2 as AddDataIncidentFactorModel.ItemsBean;
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    emergencyid = emergencyId,
                    category = "IncidentFactorIdentificationEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                    datatype = Convert.ToInt32(item.dataType),
                    creatorusername = App.userInfo.userName,

                };
                try
                {
                    emergencyModel.lat = App.currentLocation.Latitude;
                    emergencyModel.lng = App.currentLocation.Longitude;
                }
                catch (Exception)
                {
                    emergencyModel.lat = 0;
                    emergencyModel.lng = 0;
                }

                UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = emergencyModel.creationTime,
                    emergencyid = emergencyId,
                    category = "IncidentFactorIdentificationEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                    lat = emergencyModel.lat,
                    lng = emergencyModel.lng,
                    datatype = emergencyModel.datatype,
                    isEdit = true,
                    creatorusername = App.userInfo.userName,

                };

                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(ShowModel);
                dataListDelete.Add(ShowModel);
                saveList.Add(emergencyModel);
                listView.ScrollTo(ShowModel, ScrollToPosition.End, true);
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

                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    direction = direction,
                    speed = speed,
                    emergencyid = emergencyId,
                    category = "IncidentWindDataSendingEvent",
                    creatorusername = App.userInfo.userName,

                };
                try
                {
                    emergencyModel.lat = App.currentLocation.Latitude;
                    emergencyModel.lng = App.currentLocation.Longitude;
                }
                catch (Exception)
                {
                    emergencyModel.lat = 0;
                    emergencyModel.lng = 0;
                }

                UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = emergencyModel.creationTime,
                    direction = direction,
                    speed = speed,
                    emergencyid = emergencyId,
                    category = "IncidentWindDataSendingEvent",
                    lat = emergencyModel.lat,
                    lng = emergencyModel.lng,
                    isEdit = true,
                    creatorusername = App.userInfo.userName,
                };

                AzmCoord center = new AzmCoord(Convert.ToDouble(emergencyModel.lng), Convert.ToDouble(emergencyModel.lat));
                ShowModel.LocateOnMapCommand = new Command(async () => {
                    
                    await Navigation.PushAsync(new RescueSiteMapPage("风速风向发出位置", center)); 
                });
                       

                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(ShowModel);
                dataListDelete.Add(ShowModel);
                saveList.Add(emergencyModel);
                listView.ScrollTo(ShowModel, ScrollToPosition.End, true);
                MessagingCenter.Unsubscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection");

            });


        }
        //点击了污染物按钮
        void wuRanWu(object sender, System.EventArgs e)
        {
            MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");
            MessagingCenter.Subscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", (arg1, arg2) =>
            {
                AddDataIncidentFactorModel.ItemsBean item = arg2 as AddDataIncidentFactorModel.ItemsBean;
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    emergencyid = emergencyId,
                    category = "IncidentFactorIdentificationEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                    datatype = Convert.ToInt32(item.dataType),
                    creatorusername = App.userInfo.userName,

                };
                try
                {
                    emergencyModel.lat = App.currentLocation.Latitude;
                    emergencyModel.lng = App.currentLocation.Longitude;
                }
                catch (Exception)
                {
                    emergencyModel.lat = 0;
                    emergencyModel.lng = 0;
                }

                UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = emergencyModel.creationTime,
                    emergencyid = emergencyId,
                    category = "IncidentFactorIdentificationEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                    lat = emergencyModel.lat,
                    lng = emergencyModel.lng,
                    datatype = Convert.ToInt32(item.dataType),
                    isEdit = true,
                    creatorusername = App.userInfo.userName,

                };


                App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(ShowModel);
                dataListDelete.Add(ShowModel);
                saveList.Add(emergencyModel);
                listView.ScrollTo(ShowModel, ScrollToPosition.End, true);
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
                DisplayAlert("No Camera", ":( No camera available.", "OK");
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
                SaveToAlbum = true ,
            });

            if (file == null)
            {
                return;
            }

            else
            {

                var bm = SKBitmap.Decode(file.Path);
                var info = new FileInfo(file.Path); 

                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    StorePath = "/Sample/"+imageName,
                    imagePath = "/Sample/" + imageName,
                    emergencyid = emergencyId,
                    category = "IncidentPictureSendingEvent",
                    creatorusername = App.userInfo.userName,

                };
                try
                {
                    emergencyModel.lat = App.currentLocation.Latitude;
                    emergencyModel.lng = App.currentLocation.Longitude;
                }
                catch (Exception)
                {
                    emergencyModel.lat = 0;
                    emergencyModel.lng = 0;
                }
                UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = emergencyModel.creationTime,
                    StorePath = file.Path,
                    imagePath = file.Path,
                    emergencyid = emergencyId,
                    category = "IncidentPictureSendingEvent",
                    lat = emergencyModel.lat,
                    lng = emergencyModel.lng,
                    isEdit = true,
                    creatorusername = App.userInfo.userName,

                };

                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(ShowModel);
                dataListDelete.Add(ShowModel);
                saveList.Add(emergencyModel);
                listView.ScrollTo(ShowModel, ScrollToPosition.End, true);

            }
           
        }
        //点击了录制视频
        async void recordVideo(object sender, System.EventArgs e){
           
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

            var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
            {
                DesiredLength = new TimeSpan(0, 0, 10),
                Name = imageName,
                Directory = "Video",
                Quality = Plugin.Media.Abstractions.VideoQuality.High,
            });
            if (file == null) return;

            UploadEmergencyModel emergencyModel = new UploadEmergencyModel
            {
                uploadStatus = "notUploaded",
                creationTime = System.DateTime.Now,
                VideoPath = "/Video/" + imageName,
                VideoStorePath = "/Video/" + imageName,
                emergencyid = emergencyId,
                category = "IncidentVideoSendingEvent",
                creatorusername = App.userInfo.userName,

            };
            try
            {
                emergencyModel.lat = App.currentLocation.Latitude;
                emergencyModel.lng = App.currentLocation.Longitude;
            }
            catch (Exception)
            {
                emergencyModel.lat = 0;
                emergencyModel.lng = 0;
            }
            UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
            {
                uploadStatus = "notUploaded",
                creationTime = emergencyModel.creationTime,
                VideoPath = file.Path,
                VideoStorePath = file.Path,
                emergencyid = emergencyId,
                category = "IncidentVideoSendingEvent",
                lat = emergencyModel.lat,
                lng = emergencyModel.lng,
                creatorusername = App.userInfo.userName,
                isEdit = true,
            };

            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(ShowModel);
            dataListDelete.Add(ShowModel);
            saveList.Add(emergencyModel);
            listView.ScrollTo(ShowModel, ScrollToPosition.End, true);
        }
        async void AddPlacement(object sender, System.EventArgs e)
        {
           await Navigation.PushAsync(new AddPlacementPage());

        }

        public AddEmergencyAccidentInfoPage(string id)
        {
            InitializeComponent();

          
            emergencyId = id;

            //进入上传数据界面先清空contaminantsList
            App.contaminantsList = null;
            App.AppLHXZList = null;
            ReqaddData();

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DependencyService.Get<IAudio>().stopPlay();
            MessagingCenter.Unsubscribe<ContentPage, UploadEmergencyShowModel>(this, "deleteUnUploadData");
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged");
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //键盘高度改变
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged");
            MessagingCenter.Subscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged", (ContentPage arg1, KeyboardSizeModel arg2) => {

                Console.WriteLine(arg2.Height);
                Console.WriteLine(arg2.Wight);
                if (arg2.Y != App.ScreenHeight)
                {
                    cccc.Height = 55 - (206 - arg2.Height);
                }
                else
                {
                    cccc.Height = 55;
                }
            });


            //删除不要的错误的数据
            MessagingCenter.Subscribe<ContentPage, UploadEmergencyShowModel>(this, "deleteUnUploadData", (ContentPage arg1, UploadEmergencyShowModel arg2) => {
                var i = dataListDelete.IndexOf(arg2);
                try
                {
                    App.Database.DeleteEmergencyAsync(saveList[i]);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    dataListDelete.Remove(arg2);
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
                        UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel
                        {
                            uploadStatus = model.uploadStatus,
                            ID = model.ID,
                            factorId = model.factorId,
                            factorName = model.factorName,
                            testMethodId = model.testMethodId,
                            testMethodName = model.testMethodName,
                            unitId = model.unitId,
                            unitName = model.unitName,
                            equipmentId = model.equipmentId,
                            emergencyid = model.emergencyid,
                            equipmentName = model.equipmentName,
                            factorValue = model.factorValue,
                            incidentNature= model.incidentNature,
                            lat = model.lat,
                            lng = model.lng,
                            index = model.index,
                            TargetLat = model.TargetLat,
                            TargetLng = model.TargetLng,
                            TargetAddress = model.TargetAddress,
                            Content = model.Content,
                            Title = model.Title,
                            Original = model.Original,
                            Current = model.Current,
                            natureString = model.natureString,
                            StorePath = model.StorePath,
                            imagePath = model.imagePath,
                            VideoStorePath = model.VideoStorePath,
                            VideoPath = model.VideoPath,
                            VoicePath = model.VoicePath,
                            VoiceStorePath = model.VoiceStorePath,
                            width = model.width,
                            height = model.height,
                            storeurl = model.storeurl,
                            reportid = model.reportid,
                            reportname = model.reportname,
                            Length = model.Length,
                            direction = model.direction,
                            speed = model.speed,
                            creationTime = model.creationTime,
                            creatorusername = model.creatorusername,
                            category = model.category,
                            datatype = model.datatype,

                        };
                        //UploadEmergencyShowModel ShowModel = new UploadEmergencyShowModel();
                        //ShowModel = ElementMapping.Mapper(ShowModel, model);
                         ShowModel.isEdit = true;

                        dataList.Add(ShowModel);
                        dataListDelete.Add(ShowModel);
                        saveList.Add(model);
                        string cagy = model.category;
                        if (cagy == "IncidentLocationSendingEvent")
                        {
                            AzmCoord center = new AzmCoord(model.TargetLng.Value, model.TargetLat.Value);
                            ShowModel.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("事故中心点", center)); });
                        }
                        else if (cagy == "IncidentPictureSendingEvent")
                        {
                            ShowModel.StorePath = path + ShowModel.StorePath;
                            ShowModel.imagePath = path + ShowModel.imagePath;
                        }
                        else if (cagy == "IncidentVoiceSendingEvent")
                        {
                            ShowModel.VoicePath = path + ShowModel.VoicePath;
                            ShowModel.VoiceStorePath = path + ShowModel.VoiceStorePath;
                            try
                            {
                                ShowModel.PlayVoiceCommand = new Command(() =>
                                {
                                    var dir = path + model.VoicePath;
                                    DependencyService.Get<IAudio>().PlayLocalFile(dir);
                                });
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else if (cagy == "IncidentVideoSendingEvent")
                        {
                            ShowModel.VideoPath = path + ShowModel.StorePath;
                            ShowModel.VideoStorePath = path + ShowModel.imagePath;
                        }

                        else if (cagy == "IncidentFactorMeasurementEvent")
                        {
                            AzmCoord center = new AzmCoord(Convert.ToDouble(model.lng), Convert.ToDouble(model.lat));
                            ShowModel.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("数据位置", center)); });
                        }
                        else if (cagy == "IncidentMessageSendingEvent")
                        {
                            try
                            {
                                AzmCoord center = new AzmCoord(Convert.ToDouble(model.lng), Convert.ToDouble(model.lat));
                                ShowModel.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("文字信息发出位置", center)); });
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                }
                listView.ItemsSource = dataList;
                //dataListDelete =  dataList;
                isFirstAppear = false;
                if (dataList.Count > 0) listView.ScrollTo(dataList[dataList.Count - 1], ScrollToPosition.End, true);


            }




        }
        private void GetLocalDataFromDB()
        {
            //((App)App.Current).ResumeAtTodoId = -1;

            //Console.WriteLine(incidentLoggingEvents);
        }

        void addbar()
        {
            var G = new Grid();
            G.ColumnDefinitions.Add(new ColumnDefinition
            {

            });
        }

        private void uploadData(object sender, EventArgs e)
        {
            int count = dataList.Count;
            for (int i = 0; i < count; i++)
            {
                UploadEmergencyShowModel model = dataList[i];

                if (model.uploadStatus == "UploadedOver") continue;//如果已经上传的就跳过
                model.uploadStatus = "uploading";
                switch (model.category)
                {
                    case "IncidentLocationSendingEvent":                       
                        PostLocationSending(model);
                        break;
                    //化学因子名称
                    case "IncidentFactorIdentificationEvent":
                        PostFactorIdentificationSendingSecond(model);
                        break;
                    case "IncidentMessageSendingEvent":                     
                        PostMessageSending(model);
                        break;
                    case "IncidentWindDataSendingEvent":
                        PostWindDataSending(model);
                        break;
                        //事故性质
                    case "IncidentNatureIdentificationEvent":
                        PostNatureIdentification(model);
                        break;
                    //添加化学因子检测值
                    case "IncidentFactorMeasurementEvent":
                        PostFactorMeasurmentSending(model);
                        break;
                        //图片
                    case "IncidentPictureSendingEvent":
                        PostupLoadImageSending(model);
                        break;
                        //视频
                    case "IncidentVideoSendingEvent":
                        PostupLoadVideoSending(model);
                        break;
                    case "IncidentVoiceSendingEvent":
                        PostupLoadVoiceSending(model);
                        break;
                }
            }
        }

        private async void PostNatureIdentification(UploadEmergencyShowModel model)
        {
            NatureIdentification parameter = new NatureIdentification
            {
                natureString = model.natureString,
                lat = model.lat,
                lng = model.lng,
                index = 0,
                incidentId = model.emergencyid,
                loggingTime = model.creationTime.ToString(),

            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentNatureIdentificationEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {

                 PostNatureIdentificationSecond(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
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
                var i = dataListDelete.IndexOf(model);
                await App.Database.DeleteEmergencyAsync(saveList[i]);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }


        private async void PostWindDataSending(UploadEmergencyShowModel model)
        {
            WindData parameter = new WindData
            {
                direction = Convert.ToDecimal(model.direction),
                speed = Convert.ToDecimal(model.speed),
                index = 0,
                incidentId = model.emergencyid,
                lat = model.lat,
                loggingTime = model.creationTime.ToString(),

                lng = model.lng
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentWindDataSendingEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var i = dataListDelete.IndexOf(model);
                await App.Database.DeleteEmergencyAsync(saveList[i]);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        private async void PostMessageSending(UploadEmergencyShowModel model)
        {
            MessageSending parameter = new MessageSending
            {
                content = model.Content,
                title = model.Title,
                lat = model.lat,
                lng = model.lng,
                index = 0,
                loggingTime = model.creationTime.ToString(),

                incidentId = emergencyId
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentMessageSendingEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var i = dataListDelete.IndexOf(model);
                await App.Database.DeleteEmergencyAsync(saveList[i]);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        //上传事故位置
        private async void PostLocationSending(UploadEmergencyShowModel model)
        {
            LocationSending parameter = new LocationSending
            {
                targetLat = model.TargetLat,
                targetLng = model.TargetLng,
                targetAddress = "",
                lat = 0,
                lng = 0,
                index = 0,
                incidentId = emergencyId,
                    loggingTime = model.creationTime.ToString(),

            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentLocationSendingEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var i = dataListDelete.IndexOf(model);
                await App.Database.DeleteEmergencyAsync(saveList[i]);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }
        //上传新的化学因子
        private async void PostFactorIdentificationSending(UploadEmergencyShowModel model)
        {
            FactorIdentificationSending parameter = new FactorIdentificationSending
            {
                index = 0,
                factorId = model.factorId,
                factorName = model.factorName,
                incidentId = model.emergencyid,
                lat = Convert.ToDouble(model.lat),
                lng = Convert.ToDouble(model.lng),
                loggingTime = model.creationTime.ToString(),
            };

            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentFactorIdentificationEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var i = dataListDelete.IndexOf(model);
                await App.Database.DeleteEmergencyAsync(saveList[i]);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }
        //上传新的化学因子
        private async void PostFactorIdentificationSendingSecond(UploadEmergencyShowModel model)
        {
           
            AppendFactor parameter = new AppendFactor
            {
                factorId = model.factorId,
                factorName = model.factorName,
                dataType = model.datatype,
                incidentId = model.emergencyid,
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentFactor/Append", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                PostFactorIdentificationSending(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        //上传图片
        private async void PostupLoadImageSending(UploadEmergencyShowModel model)
        {

            //HTTPResponse hTTPResponse = await EasyWebRequest.SendImageAsync(model.StorePath);
           
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.StorePath,".png", App.EmergencyModule.url, ConstantUtils.UPLOAD_EMERGENCY_API);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                uploadImageResurt resultData = JsonConvert.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if (resultData ==null || resultData.result == null) return;
                if(resultData.result.Count>0){
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    uploadImageModel parama = new uploadImageModel
                    {
                        index = 0,
                        incidentId = emergencyId,
                        width = (int)imageResurtData.width,
                        height = (int)imageResurtData.height,
                        storePath = imageResurtData.storeUrl,
                        lat = Convert.ToDouble(model.lat),
                        lng = Convert.ToDouble(model.lng),
                        loggingTime = model.creationTime.ToString(),

                    };

                    string param = JsonConvert.SerializeObject(parama);

                    HTTPResponse hTTPResponse1 = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentPictureSendingEvent", param, "POST", App.EmergencyToken);
                    Console.WriteLine(hTTPResponse1);
                    if (hTTPResponse1.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var i = dataListDelete.IndexOf(model);
                        await App.Database.DeleteEmergencyAsync(saveList[i]);
                        model.uploadStatus = "UploadedOver";
                        dataListDelete.Remove(model);
                    }
                    else
                    {
                        Console.WriteLine(hTTPResponse);
                    }

                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }

        //上传视频
        private async void PostupLoadVideoSending(UploadEmergencyShowModel model)
        {

            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.VideoStorePath,".mp4", App.EmergencyModule.url, ConstantUtils.UPLOAD_EMERGENCY_API);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                uploadImageResurt resultData = JsonConvert.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if (resultData.result == null) return;
                if (resultData.result.Count > 0)
                {
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    uploadVidoeModel parama = new uploadVidoeModel
                    {
                        index = 0,
                        incidentId = model.emergencyid,
                        storePath = imageResurtData.storeUrl,
                        lat = Convert.ToDouble(model.lat),
                        lng = Convert.ToDouble(model.lng),
                        loggingTime = model.creationTime,
                    };

                    string param = JsonConvert.SerializeObject(parama);

                    HTTPResponse hTTPResponse1 = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentVideoSendingEvent", param, "POST", App.EmergencyToken);
                    Console.WriteLine(hTTPResponse1);
                    if (hTTPResponse1.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var i = dataListDelete.IndexOf(model);
                        await App.Database.DeleteEmergencyAsync(saveList[i]);
                        model.uploadStatus = "UploadedOver";
                        dataListDelete.Remove(model);
                    }
                    else
                    {
                        Console.WriteLine(hTTPResponse);
                    }

                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }

        //上传录音
        private async void PostupLoadVoiceSending(UploadEmergencyShowModel model)
        {
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.VoiceStorePath, ".mp4", App.EmergencyModule.url, ConstantUtils.UPLOAD_EMERGENCY_API);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                uploadImageResurt resultData = JsonConvert.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if (resultData.result == null) return;
                if (resultData.result.Count > 0)
                {
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    uploadVoiceModel parama = new uploadVoiceModel
                    {
                        index = 0,
                        incidentId = model.emergencyid,
                        storePath = imageResurtData.storeUrl,
                        lat = Convert.ToDouble(model.lat),
                        lng = Convert.ToDouble(model.lng),
                        loggingTime = model.creationTime.ToString(),
                        length = Convert.ToInt32(model.Length),//录音的时长
                    };
                    string param = JsonConvert.SerializeObject(parama);
                    HTTPResponse hTTPResponse1 = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentVoiceSendingEvent", param, "POST", App.EmergencyToken);
                    Console.WriteLine(hTTPResponse1);
                    if (hTTPResponse1.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var i = dataListDelete.IndexOf(model);
                        await App.Database.DeleteEmergencyAsync(saveList[i]);
                        model.uploadStatus = "UploadedOver";
                        dataListDelete.Remove(model);
                    }
                    else
                    {
                        Console.WriteLine(hTTPResponse);
                    }

                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
                model.uploadStatus = "notUploaded";
                DependencyService.Get<IToast>().ShortAlert("上传失败");


            }

        }


        //上传化学因子检测值
        private async void PostFactorMeasurmentSending(UploadEmergencyShowModel model)
        {
            FactorMeasurmentSending parameter = new FactorMeasurmentSending
            {
                index = 0,
                incidentId = emergencyId,
                factorId = model.factorId,
                factorName = model.factorName,
                testMethodId = "",
                testMethodName = "",
                unitId = model.unitId,
                unitName = model.unitName,
                equipmentId = "",
                equipmentName = "",
                factorValue = Convert.ToDouble(model.factorValue),
                incidentNature = 4,
                loggingTime = model.creationTime.ToString(),
                lat = Convert.ToDouble(model.lat),
                lng = Convert.ToDouble(model.lng),
            };
 

            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentFactorMeasurementEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var i = dataListDelete.IndexOf(model);
                await App.Database.DeleteEmergencyAsync(saveList[i]);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }



        //获取"数据"页面的"关键污染物"和"关键理化性质"
        private async void ReqaddData()
        {
            List<AddDataIncidentFactorModel.ItemsBean> WuRanWus = new List<AddDataIncidentFactorModel.ItemsBean>();
            List<AddDataIncidentFactorModel.ItemsBean> LHXZs = new List<AddDataIncidentFactorModel.ItemsBean>();
            string url = App.EmergencyModule.url + DetailUrl.GetIncidentFactors +
                        "?Id=" + App.EmergencyAccidentID;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode != HttpStatusCode.ExpectationFailed)
            {
                Console.WriteLine(hTTPResponse.Results);
                AddDataIncidentFactorModel.Factor fa = JsonConvert.DeserializeObject<AddDataIncidentFactorModel.Factor>(hTTPResponse.Results);

                for (int i = 0; i < fa.result.items.Count; i++)
                {

                    AddDataIncidentFactorModel.ItemsBean model = fa.result.items[i];
                    if (model.dataType == "0") WuRanWus.Add(model);
                    if (model.dataType == "1" || model.dataType == "2" || model.dataType == "3") LHXZs.Add(model);

                }
                App.contaminantsList = WuRanWus;
                App.AppLHXZList = LHXZs;
            }
        }


    }



    //追加因子
    internal class AppendFactor{
        public string factorId { get; set; }
        public string factorName { get; set; }
        public int dataType { get; set; }
        public string incidentId { get; set; }
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

    internal class uploadImageModel
    {
        public string storePath { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string loggingTime { get; set; }

    }

    internal class uploadVidoeModel
    {
        public string storePath { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public DateTime loggingTime { get; set; }
    }

    internal class uploadVoiceModel
    {
        public string storePath { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public string loggingTime { get; set; }
        public int length { get; set; }
    }

    internal class NatureIdentification
    {
        public string natureString { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public string loggingTime { get; set; }

    }

    internal class WindData
    {
        public decimal direction { get; set; }
        public decimal speed { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string loggingTime { get; set; }

    }

    internal class MessageSending
    {
        public string content { get; set; }
        public string title { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public string loggingTime { get; set; }

    }

    //internal class imageFile{

    //    public 
    //}



    internal class LocationSending
    {
        public double? targetLat { get; set; }
        public double? targetLng { get; set; }
        public string targetAddress { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public string loggingTime { get; set; }


    }

    internal class FactorIdentificationSending
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public string factorId { get; set; }
        public string factorName { get; set; }
        public string loggingTime { get; set; }

    }

    internal class FactorMeasurmentSending
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public double factorValue { get; set; }
        public int incidentNature { get; set; }
        public string incidentId { get; set; }
        public string factorId { get; set; }
        public string factorName { get; set; }
        public string testMethodId { get; set; }
        public string testMethodName { get; set; }
        public string unitId { get; set; }
        public string unitName { get; set; }
        public string equipmentId { get; set; }
        public string equipmentName { get; set; }
        public string loggingTime { get; set; }

    }


}
