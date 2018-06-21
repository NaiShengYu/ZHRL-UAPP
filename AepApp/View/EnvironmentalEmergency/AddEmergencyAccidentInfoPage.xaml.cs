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
        Location _location = null;
        //获取当前位置
        async void HandleEventHandler()
        {
            try
            {
                _location = await Geolocation.GetLastKnownLocationAsync();
                if (_location != null)
                {
                    App.currentLocation = _location;
                }
            }
            catch (Exception ex)
            {
            }
        }


        private ObservableCollection<UploadEmergencyModel> dataList = new ObservableCollection<UploadEmergencyModel>();
        private ObservableCollection<UploadEmergencyModel> dataListDelete = new ObservableCollection<UploadEmergencyModel>();
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

            if (isfunctionBarIsShow == true)
            {
                canceshiguxingzhi();
                listView.SelectedItem = null;
                return;
            }

            var item = e.SelectedItem as UploadEmergencyModel;
            if (item == null)
                return;
            listView.SelectedItem = null;

        }

#if __IOS__
        void HandleAction(NSNotification obj)
        {
            var dic = obj.UserInfo as NSMutableDictionary;
            var rc = dic.ValueForKey((Foundation.NSString)"UIKeyboardFrameEndUserInfoKey");
            CGRect r = (rc as NSValue).CGRectValue;

            if(Convert.ToInt32(r.Y) != App.ScreenHeight){
                //entryStack.TranslateTo(0, 206 - r.Size.Height);
                cccc.Height = 55-Convert.ToDouble((206 - r.Size.Height));
            }
            else {
                //entryStack.TranslateTo(0, 0);
                cccc.Height = 55;
            }
        }
#endif
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
                category = "IncidentMessageSendingEvent"
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
            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(emergencyModel);
            dataListDelete.Add(emergencyModel);
            await entryStack.TranslateTo(0, 0);
            ENT.Text = "";
            listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);
        }
      

        //点击了位置按钮
        async void AccidentPosition(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AccidentPositionPage());
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

                //MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    TargetLat = lat,
                    TargetLng = lon,
                    emergencyid = emergencyId,
                    category = "IncidentLocationSendingEvent"
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
                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(emergencyModel);
                dataListDelete.Add(emergencyModel);
                listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

            });

        }
        //左滑删除
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            var menu = sender as MenuItem;
            var item = menu.BindingContext as UploadEmergencyModel;
            await App.Database.DeleteEmergencyAsync(item);
            dataList.Remove(item);
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
            UploadEmergencyModel IdentificationModel = App.LastNatureAccidentModel;
            foreach (UploadEmergencyModel emergencyModel1 in dataList)
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
            UploadEmergencyModel IdentificationModel = null;
            foreach(UploadEmergencyModel emergencyModel1 in dataList){
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
                category = "IncidentNatureIdentificationEvent"
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
            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(emergencyModel);
            dataListDelete.Add(emergencyModel);
            listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

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
                await App.Database.SaveEmergencyAsync(emergencyModel);
                if (item.yangBenLeiXing == "大气") emergencyModel.incidentNature = "4";
                else if (item.yangBenLeiXing == "水质") emergencyModel.incidentNature = "2";
                else if (item.yangBenLeiXing == "土壤") emergencyModel.incidentNature = "1";
                else emergencyModel.incidentNature = "8";

                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(emergencyModel);
                dataListDelete.Add(emergencyModel);
                listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

                MessagingCenter.Unsubscribe<ContentPage, AddDataForChemicolOrLHXZModel.ItemsBean>(this, "AddLHXZ");
            });

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
                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(emergencyModel);
                dataListDelete.Add(emergencyModel);
                listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

                MessagingCenter.Unsubscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew");
            });
            Navigation.PushAsync(new addDataPage());

        }
        //完成选择事故性质
        async void finishishiguxingzhi(object sender, System.EventArgs e)
        {
            //entryStack.TranslateTo(0, 0);
            await b2.TranslateTo(0, 0);
            aaaa.Height = 55;
            bbbb.Height = 150;
            await functionBar.TranslateTo(0, 0);
            isfunctionBarIsShow = false;
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
            isSelectDQ = false;
            isSelectSZ = false;
            isSelectTR = false;
            UploadEmergencyModel emergencyModel = new UploadEmergencyModel
            {
                uploadStatus = "notUploaded",
                creationTime = System.DateTime.Now,
                natureString = a,
                emergencyid = emergencyId,
                category = "IncidentNatureIdentificationEvent"
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
            await App.Database.SaveEmergencyAsync(emergencyModel);
            dataList.Add(emergencyModel);
            dataListDelete.Add(emergencyModel);
            listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

        }

        void canceshiguxingzhi()
        {
            //entryStack.TranslateTo(0, 0);
            b2.TranslateTo(0, 0);
            aaaa.Height = 55;
            bbbb.Height = 150;
            functionBar.TranslateTo(0, 0);
            isSelectDQ = false;
            dqBut.BackgroundColor = Color.Transparent;
            isSelectSZ = false;
            szBut.BackgroundColor = Color.Transparent;
            isSelectTR = false;
            trBut.BackgroundColor = Color.Transparent;
            isfunctionBarIsShow = false;
        }
      

        //点击了风速风向按钮
        async void fengSuFengXiang(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new WindSpeedAndDirectionPage());
            MessagingCenter.Subscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection", async (arg1, arg2) =>
            {
                MessagingCenter.Unsubscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection");

                string speed = arg2[0];
                string direction = arg2[1];




                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    direction = direction,
                    speed = speed,
                    emergencyid = emergencyId,
                    category = "IncidentWindDataSendingEvent"
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
                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(emergencyModel);
                dataListDelete.Add(emergencyModel);
                listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

            });


        }
        //点击了污染物按钮
        void wuRanWu(object sender, System.EventArgs e)
        {

            MessagingCenter.Subscribe<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", (arg1, arg2) =>
            {
                AddDataIncidentFactorModel.ItemsBean item = arg2 as AddDataIncidentFactorModel.ItemsBean;
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    lat = App.currentLocation.Latitude,
                    lng = App.currentLocation.Longitude,
                    emergencyid = emergencyId,
                    category = "IncidentFactorIdentificationEvent",
                    factorId = item.factorId,
                    factorName = item.factorName,
                };
                App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(emergencyModel);
                dataListDelete.Add(emergencyModel);
                listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);

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
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = System.DateTime.Now + ".jpg"
            });

            if (file == null)
            {
                return;
            }
            else
            {
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    uploadStatus = "notUploaded",
                    creationTime = System.DateTime.Now,
                    StorePath = file.Path,
                    imagePath = file.Path,
                    emergencyid = emergencyId,
                    category = "IncidentPictureSendingEvent"
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
                await App.Database.SaveEmergencyAsync(emergencyModel);
                dataList.Add(emergencyModel);
                dataListDelete.Add(emergencyModel);                       
                listView.ScrollTo(emergencyModel, ScrollToPosition.End, true);
            }
           
        }




        public AddEmergencyAccidentInfoPage(string id)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            HandleEventHandler();

         
#if __IOS__//监听键盘的高度
            var not = NSNotificationCenter.DefaultCenter;
            not.AddObserver(UIKeyboard.WillChangeFrameNotification, HandleAction);
#endif
            emergencyId = id;

            //进入上传数据界面先清空contaminantsList
            App.contaminantsList = null;
            App.AppLHXZList = null;
            ReqaddData();

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //请求数据库数据
            // App.Database.CreatEmergencyTable();
            if (isFirstAppear)
            {
                List<UploadEmergencyModel> dataList2 = await App.Database.GetEmergencyAsync();
                int count = dataList2.Count;
                foreach (UploadEmergencyModel model in dataList2)
                {
                    if (!string.IsNullOrWhiteSpace(model.category))
                    {
                        dataList.Add(model);
                        dataListDelete.Add(model);
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
                UploadEmergencyModel model = dataList[i];
                model.uploadStatus = "uploading";
                switch (model.category)
                {
                    case "IncidentLocationSendingEvent":                       
                        PostLocationSending(model);
                        break;
                    //化学因子名称
                    case "IncidentFactorIdentificationEvent":
                        PostFactorIdentificationSending(model);
                        break;
                    case "IncidentMessageSendingEvent":                     
                        PostMessageSending(model);
                        break;
                    case "IncidentWindDataSendingEvent":
                        PostWindDataSending(model);
                        break;
                    case "IncidentNatureIdentificationEvent":
                        PostNatureIdentification(model);
                        break;
                    //添加化学因子检测值
                    case "IncidentFactorMeasurementEvent":
                        PostFactorMeasurmentSending(model);
                        break;
                    case "IncidentPictureSendingEvent":
                        PostupLoadImageSending(model);
                        break;
                }
            }
        }

        private async void PostNatureIdentification(UploadEmergencyModel model)
        {
            NatureIdentification parameter = new NatureIdentification
            {
                natureString = model.natureString,
                lat = model.lat,
                lng = model.lng,
                index = 0,
                incidentId = model.emergencyid
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentNatureIdentificationEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await App.Database.DeleteEmergencyAsync(model);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        private async void PostWindDataSending(UploadEmergencyModel model)
        {
            WindData parameter = new WindData
            {
                direction = Convert.ToDecimal(model.direction),
                speed = Convert.ToDecimal(model.speed),
                index = 0,
                incidentId = model.emergencyid,
                lat = model.lat,
                lng = model.lng
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentWindDataSendingEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await App.Database.DeleteEmergencyAsync(model);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        private async void PostMessageSending(UploadEmergencyModel model)
        {
            MessageSending parameter = new MessageSending
            {
                content = model.Content,
                title = model.Title,
                lat = model.lat,
                lng = model.lng,
                index = 0,
                incidentId = emergencyId
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentMessageSendingEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await App.Database.DeleteEmergencyAsync(model);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        //上传事故位置
        private async void PostLocationSending(UploadEmergencyModel model)
        {
            LocationSending parameter = new LocationSending
            {
                targetLat = model.TargetLat,
                targetLng = model.TargetLng,
                targetAddress = "",
                lat = 0,
                lng = 0,
                index = 0,
                incidentId = emergencyId
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentLocationSendingEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await App.Database.DeleteEmergencyAsync(model);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }
        //上传新的化学因子
        private async void PostFactorIdentificationSending(UploadEmergencyModel model)
        {
            FactorIdentificationSending parameter = new FactorIdentificationSending
            {
                index = 0,
                factorId = model.factorId,
                factorName = model.factorName,
                incidentId = emergencyId
            };
            try
            {
                parameter.lat = Convert.ToDouble(model.lat);
                parameter.lng = Convert.ToDouble(model.lng);
            }
            catch
            {
                parameter.lat = 0;
                parameter.lng = 0;
            }

            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentFactorIdentificationEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await App.Database.DeleteEmergencyAsync(model);
                model.uploadStatus = "UploadedOver";
                dataListDelete.Remove(model);
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }

        //上传图片
        private async void PostupLoadImageSending(UploadEmergencyModel model)
        {

            //HTTPResponse hTTPResponse = await EasyWebRequest.SendImageAsync(model.StorePath);

            HTTPResponse hTTPResponse = await EasyWebRequest.upload(model.StorePath);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {

                uploadImageResurt resultData = JsonConvert.DeserializeObject<uploadImageResurt>(hTTPResponse.Results);
                if(resultData.result.Count>0){
                    uploadImageResurtData imageResurtData = resultData.result[0];
                    uploadImageModel parama = new uploadImageModel
                    {
                        index = 0,
                        incidentId = emergencyId,
                        width = (int)imageResurtData.width,
                        height = (int)imageResurtData.height,
                        storePath = imageResurtData.storeUrl,
                    };
                    try
                    {
                        parama.lat = Convert.ToDouble(model.lat);
                        parama.lng = Convert.ToDouble(model.lng);
                    }
                    catch
                    {
                        parama.lat = 0;
                        parama.lng = 0;
                    }
                    string param = JsonConvert.SerializeObject(parama);

                    HTTPResponse hTTPResponse1 = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentPictureSendingEvent", param, "POST", App.EmergencyToken);
                    Console.WriteLine(hTTPResponse1);
                    if (hTTPResponse1.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await App.Database.DeleteEmergencyAsync(model);
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

        //上传化学因子检测值
        private async void PostFactorMeasurmentSending(UploadEmergencyModel model)
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

            };
            try
            {
                parameter.lat = Convert.ToDouble(model.lat);
                parameter.lng = Convert.ToDouble(model.lng);
            }
            catch
            {
                parameter.lat = 0;
                parameter.lng = 0;
            }

            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.EmergencyModule.url + "/api/services/app/IncidentLoggingEvent/AppendIncidentFactorMeasurementEvent", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await App.Database.DeleteEmergencyAsync(model);
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
    }

    internal class NatureIdentification
    {
        public string natureString { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
    }

    internal class WindData
    {
        public decimal direction { get; set; }
        public decimal speed { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
    }

    internal class MessageSending
    {
        public string content { get; set; }
        public string title { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
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

    }

    internal class FactorIdentificationSending
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int index { get; set; }
        public string incidentId { get; set; }
        public string factorId { get; set; }
        public string factorName { get; set; }
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
    }


}
