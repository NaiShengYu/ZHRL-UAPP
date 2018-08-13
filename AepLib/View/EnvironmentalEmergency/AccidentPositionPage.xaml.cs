
using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Essentials;
using CloudWTO.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AccidentPositionPage : ContentPage
    {

        /// <summary>
        /// 点击了键盘的搜索按钮
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            //getSearchAddress();

            //string text = seach.Text;
            //string pat = @"^[\-\+]?(0?\d{1,2}\.\d{1,5}|1[0-7]?\d{1}\.\d{1,5}|180\.0{1,5})$/0{1,5})$";
            //// Compile the regular expression.
            //Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            //// Match the regular expression pattern against a text string.
            //Match m = r.Match(text);
            //int matchCount = 0;
            //while (m.Success)
            //{
            //}

        }

        //搜索框内容变化就调用搜索
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            getSearchAddress();
        }
        //选中某一个地址
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as poisModel;
            if (item == null)
                return;
            map.SetCenter(12, new AzmCoord(Convert.ToDouble(item.lon),Convert.ToDouble(item.lat)));
            //map.CenterCoord = new AzmCoord(Convert.ToDouble(item.lon), Convert.ToDouble(item.lat));
            listView.SelectedItem = null;
            searchBar.TranslationY = 0;

        }
        //点击背景页关闭搜索框
        void TapGestureRecognizer_Tapped(object sender, System.EventArgs e){
            searchBar.TranslationY = 0;
        }
        //点击搜索按钮搜索页面出现
        void SearchLoactionClick(object sender, System.EventArgs e)
        {
            searchBar.TranslationY = -App.ScreenHeight ;
        }

        static int i = 0;

        async void HandleEventHandler()
        {

            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();
                App.currentLocation = location;
                if (location != null)
                {
                    if (i >0){
                        map.SetCenter(12, new AzmCoord(location.Longitude, location.Latitude));
                    }
                    var img = ImageSource.FromFile("markerred.png");
                    var aaa = new AzmMarkerView(img, new Size(30, 30),new AzmCoord(location.Longitude, location.Latitude));
                    map.Overlays.Add(aaa);
                    i += 1;
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }
            }
          
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
        //地图放大
        void zoomout (object sender,System.EventArgs e){
            map.ZoomOut();


        }
        //地图缩小
        void zoomin (object sender ,System.EventArgs e){
            map.ZoomIn();
        }

        //保存此位置
        void savePosition(object sender, System.EventArgs e)
        {
            Console.WriteLine(centercoorLab.Text);

            MessagingCenter.Send<ContentPage, string>(this,"savePosition", centercoorLab.Text);

            Navigation.PopAsync();
                
        }

        //回到当前位置
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            HandleEventHandler();
        }

        void Handle_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            //虚线间隔
            float[] aa = new float[2];
            for (int i = 0; i < 2; i++)
            {
                aa[i] = 10;
            }
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Gray.ToSKColor(),
                StrokeWidth = 3,
                //StrokeCap = (SKStrokeCap)Enum.Parse(typeof(SKStrokeCap),@"10"),
                PathEffect = SKPathEffect.CreateDash(aa, 0),
            };

            SKPath path = new SKPath();
            path.MoveTo(0.5f * info.Width, 0);
            path.LineTo(0.5f * info.Width, info.Height);
            canvas.DrawPath(path, paint);

            SKPath path1 = new SKPath();
            path1.MoveTo(0, 0.5f * info.Height);
            path1.LineTo(info.Width, 0.5f * info.Height);
            canvas.DrawPath(path1, paint);
        }

        ObservableCollection<poisModel> dataList = new ObservableCollection<poisModel>();

        public AccidentPositionPage()
        {
            InitializeComponent();
            HandleEventHandler();
            searchBar.Margin = new Thickness(0, 0, 0, -App.ScreenHeight);
            searchBar.HeightRequest = App.ScreenHeight -150;
            if (App.currentLocation != null)
            {
                map.SetCenter(12, new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude));
            }
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            listView.ItemsSource = dataList;
        }


        public AccidentPositionPage(string position) : this(){

            Title = "事件位置";




        }




        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");
        }


        private async void getSearchAddress()
        {
            string param = "";
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync("http://restapi.amap.com/v3/place/text?key=8325164e247e15eea68b59e89200988b&page=1&offset=10&city=330200&language=zh_cn&keywords="+seach.Text, param, "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                searchAddressResultModel resultModel  = JsonConvert.DeserializeObject<searchAddressResultModel>(hTTPResponse.Results);
                dataList.Clear();
                for (int i = 0; i < resultModel.pois.Count; i++)
                {
                    dataList.Add(resultModel.pois[i]);
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }


        internal class searchAddressResultModel{
            public List<poisModel> pois { get; set; }
        }

        internal class poisModel
        {
            public string name { get; set; }
            public string location { get; set; }
            public string lon {
                get{
                    
                    string[] bbb = location.Split(",".ToCharArray());
                    Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(bbb[1]), Convert.ToDouble(bbb[0]));
                    return gps.getWgLon().ToString();
                }
            }
            public string lat  {
                get{
                    string[] bbb = location.Split(",".ToCharArray());
                    Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(bbb[1]), Convert.ToDouble(bbb[0]));
                    return gps.getWgLat().ToString();
                }
            }

            public string address
            {
                get; set;
            }


            //public string address
            //{
            //    get
            //    {
            //        return lon + " E ," + lat + " N";
            //    }
            //    set { }
            //}   
        }




    }
}
