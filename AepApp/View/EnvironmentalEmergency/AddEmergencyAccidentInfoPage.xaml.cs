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
                }
            }
            catch (Exception ex)
            {
            }
        }


        private ObservableCollection<UploadEmergencyModel> dataList = new ObservableCollection<UploadEmergencyModel>();
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

            if(isfunctionBarIsShow ==true){
                canceshiguxingzhi();
                listView.SelectedItem = null;
                return;
            }

            var item = e.SelectedItem as item;
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
            entryStack.TranslateTo(0, 206 - r.Size.Height);
        }
#endif
        //编辑结束
        void Handle_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            entryStack.TranslateTo(0, 0);

        }

        //编辑开始
        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            //ENT.TranslateTo(0, 100);

            var entr = sender as Entry;
        }

        //点击了位置按钮
        void AccidentPosition(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AccidentPositionPage());
            MessagingCenter.Subscribe<ContentPage, string>(this, "savePosition", (arg1, arg2) =>
            {
                var aaa = arg2 as string;
                aaa = aaa.Replace("E", "");
                aaa = aaa.Replace("N", "");
                aaa = aaa.Replace("W", "");
                aaa = aaa.Replace("S", "");
                aaa = aaa.Replace(" ", "");
                string[] bbb = aaa.Split(",".ToCharArray());
                double lat = System.Convert.ToDouble(bbb[0]);
                double lon = System.Convert.ToDouble(bbb[1]);

                MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");
                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {
                    //TargetLat = 0.5514516,
                    //TargetLng = 1.051516,
                    //category = "IncidentLocationSendingEvent"
                };
                App.Database.SaveEmergencyAsync(emergencyModel);
            });

        }
        //左滑删除
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var menu = sender as MenuItem;
            var item = menu.BindingContext as UploadEmergencyModel;

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
        }

        //选中了大气
        bool isSelectDQ = false;
        bool isSelectSZ = false;
        bool isSelectTR = false;
        void selectDQ(object sender, System.EventArgs e){
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

        //点击了数据按钮
        void addShuju(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new addDataPage());

        }
        //完成选择事故性质
        void finishishiguxingzhi(object sender, System.EventArgs e)
        {
            //entryStack.TranslateTo(0, 0);
            b2.TranslateTo(0, 0);
            aaaa.Height = 55;
            bbbb.Height = 150;
            functionBar.TranslateTo(0, 0);
            isfunctionBarIsShow = false;
        }

        void canceshiguxingzhi(){
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

#pragma 点击事故性质按钮一系列操作结束

        //点击了风速风向按钮
        void fengSuFengXiang(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new WindSpeedAndDirectionPage());
            MessagingCenter.Subscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection", (arg1, arg2) =>
            {
                MessagingCenter.Unsubscribe<ContentPage, string[]>(this, "saveWindSpeedAndDirection");

                string speed = arg2[0];
                string direction = arg2[1];

                decimal aa = Convert.ToDecimal(speed);
                decimal bb = Convert.ToDecimal(direction);


                UploadEmergencyModel emergencyModel = new UploadEmergencyModel
                {


                };
                App.Database.SaveEmergencyAsync(emergencyModel);
            });


        }
        //点击了污染物按钮
        void wuRanWu(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ChemicalPage(1));
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
                  Name = "test.jpg"
              });

            if (file == null)
               return;

            //var item3 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/俞乃胜",
            //    imgSourse = file.Path,
            //    info = "",
            //    isShowAddress = true,
            //    time = Convert.ToDateTime("2018-03-19 17:51:46.310"),
            //};
            //dataList.Add(item3);

            EasyWebRequest.UploadImage(file.Path);
        }



        
        public AddEmergencyAccidentInfoPage(ObservableCollection<IncidentLoggingEventsBean> dataList)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this,"");//去掉返回键文字

            HandleEventHandler();

#if __IOS__//监听键盘的高度
            var not = NSNotificationCenter.DefaultCenter;
            not.AddObserver(UIKeyboard.WillChangeFrameNotification, HandleAction);
#endif
           
            //dataList.Add(item4);

            //dataList.RemoveAt(1);
            //dataList.RemoveAt(3);
            //dataList.RemoveAt(5);
            //listView.ItemsSource = dataList;

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //TodoItemDatabase todoItemDatabase =   App.Database2;
            //请求数据库数据
            // App.Database.CreatEmergencyTable();
            List<UploadEmergencyModel>  dataList2 = await App.Database.GetEmergencyAsync();
            int count = dataList2.Count;

            foreach(UploadEmergencyModel model in dataList2){
                if(!string.IsNullOrWhiteSpace(model.category)) dataList.Add(model);
           
            }

            listView.ItemsSource = dataList;
            //Console.WriteLine(item);
            //List<UploadEmergencyModel>  item = await App.Database.GetEmergencyAsync();
            //
            // GetLocalDataFromDB();
        }
        private  void GetLocalDataFromDB()
        {
            //((App)App.Current).ResumeAtTodoId = -1;
            
           //Console.WriteLine(incidentLoggingEvents);
        }

        internal class item
        {
            public string imgSourse { get; set; }
            public string timeAndName { set; get; }
            public string info { set; get; }
            public string address { set; get; }
            public bool isShowAddress { set; get; }

            public DateTime time { get; set; }
        }



        void addbar(){
            var G = new Grid();
            G.ColumnDefinitions.Add(new ColumnDefinition
            {
               
            });








        }


    }
}
