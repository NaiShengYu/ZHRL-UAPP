
using CloudWTO.Services;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AccidentPositionPage : ContentPage
    {

        public event EventHandler<EventArgs> SavePosition;

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
            map.SetCenter(12, new AzmCoord(Convert.ToDouble(item.lon), Convert.ToDouble(item.lat)), true);
            //map.CenterCoord = new AzmCoord(Convert.ToDouble(item.lon), Convert.ToDouble(item.lat));
            listView.SelectedItem = null;
            searchBar.TranslationY = 0;

        }
        //点击背景页关闭搜索框
        void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            searchBar.TranslationY = 0;
        }
        //点击搜索按钮搜索页面出现
        void SearchLoactionClick(object sender, System.EventArgs e)
        {
            searchBar.TranslationY = -App.ScreenHeight;
        }

        public AzmMarkerView aaa = null;
        async void HandleEventHandler()
        {

            try
            {
                Location location;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    location = await Geolocation.GetLastKnownLocationAsync();
                }
                else
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                App.currentLocation = location;
                if (location != null)
                {
                    map.SetCenter(12, new AzmCoord(location.Longitude, location.Latitude), true);
                    if (aaa != null)
                    {
                        return;
                    }

                    var img = ImageSource.FromFile("markerred.png");
                    aaa = new AzmMarkerView(img, new Size(30, 30), new AzmCoord(location.Longitude, location.Latitude));
                    map.Overlays.Add(aaa);
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }
            }

            catch (Exception ex)
            {
                // Unable to get location
            }
        }



        //保存此位置
        void savePosition(object sender, System.EventArgs e)
        {
            Console.WriteLine(centercoorLab.Text);
            SavePosition.Invoke(centercoorLab.Text, new EventArgs());
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

        public AccidentPositionPage() {
            InitializeComponent();
            searchBar.Margin = new Thickness(0, 0, 0, -App.ScreenHeight);
            searchBar.HeightRequest = App.ScreenHeight - 150;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            listView.ItemsSource = dataList;

        }
        

        public AccidentPositionPage(string lng, string lat):this()
        {

            if (string.IsNullOrWhiteSpace(lng) || string.IsNullOrWhiteSpace(lat))
            {
                if (App.currentLocation != null)
                {
                    map.SetCenter(12, new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude), true);
                }
            }
            else
            {
                map.SetCenter(12, new AzmCoord(Convert.ToDouble(lng), Convert.ToDouble(lat)), true);
            }

            HandleEventHandler();
        }
        

        public AccidentPositionPage(string lng, string lat, string title) : this()
        {
            Title = title;

            //设置target坐标//事故中心位置
            if (App.EmergencyCenterCoord.lat != 0.0f && App.EmergencyCenterCoord.lng != 0.0)
            {
                try
                {
                    AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(35, 35), App.EmergencyCenterCoord)
                    {
                        BackgroundColor = Color.Transparent,
                    };
                    map.Overlays.Add(mv);

                    var fivekmcir = new AzmEllipseView(App.EmergencyCenterCoord, 5000.0);
                    fivekmcir.StrokeThickness = 6;
                    fivekmcir.DashArray = new float[2] { 10.0f, 10.0f };
                    map.ShapeOverlays.Add(fivekmcir);

                    map.SetCenter(12, App.EmergencyCenterCoord, true);
                }
                catch (Exception ex)
                {

                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (MapGrid.Children.Contains(map) ==true)
            {
                MapGrid.Children.Remove(map);
            }

            MapGrid.Children.Insert(0,map);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MapGrid.Children.Remove(map);
        }


        private async void getSearchAddress()
        {
            string param = "";
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync("http://restapi.amap.com/v3/place/text?key=8325164e247e15eea68b59e89200988b&page=1&offset=10&city=330200&language=zh_cn&keywords=" + seach.Text, param, "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                searchAddressResultModel resultModel = Tools.JsonUtils.DeserializeObject<searchAddressResultModel>(hTTPResponse.Results);
                dataList.Clear();
                if (resultModel != null && resultModel.pois != null)
                {
                    for (int i = 0; i < resultModel.pois.Count; i++)
                    {
                        dataList.Add(resultModel.pois[i]);
                    }
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }


        internal class searchAddressResultModel
        {
            public List<poisModel> pois { get; set; }
        }

        internal class poisModel
        {
            public string name { get; set; }
            public string location { get; set; }
            public string lon
            {
                get
                {

                    string[] bbb = location.Split(",".ToCharArray());
                    Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(bbb[1]), Convert.ToDouble(bbb[0]));
                    return gps.getWgLon().ToString();
                }
            }
            public string lat
            {
                get
                {
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
