using AepApp.Interface;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using static AepApp.Models.EmergencyAccidentInfoDetail;

#if __IOS__
using MapKit;//苹果地图用的
using CoreFoundation;
#endif
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueSiteMapPage : ContentPage
    {

        async void openMapNav(double lat, double lng, string destination)
        {

            List<string> aaa = DependencyService.Get<IOpenApp>().JudgeCanOpenAPP();
            string[] bbb = aaa.ToArray();
            var action = await DisplayActionSheet("选择地图", "取消", null, bbb);
            Console.Write(action);
            switch (action)
            {
                case "高德地图":
                    //Gps gps = PositionUtil.gcj_To_Gps84(lat, lng);
                    //Device.OpenUri(new Uri("iosamap://navi?sourceApplication=" + "AEPAPP" + "&backScheme=applicationScheme&poiname=fangheng&poiid=BGVIS&lat=" + gps.getWgLat() + "&" + "lon=" + gps.getWgLon() + "&dev=0&style=2"));
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Device.OpenUri(new Uri("androidamap://navi?sourceApplication=" + "AEPAPP" + "&backScheme=applicationScheme&poiname=fangheng&poiid=BGVIS&lat=" + lat + "&" + "lon=" + lng + "&dev=0&style=2"));
                    }
                    else if (Device.RuntimePlatform == Device.iOS)
                    {
                        Device.OpenUri(new Uri("iosamap://navi?sourceApplication=" + "AEPAPP" + "&backScheme=applicationScheme&poiname=fangheng&poiid=BGVIS&lat=" + lat + "&" + "lon=" + lng + "&dev=0&style=2"));
                    }
                    break;
                case "百度地图":
                    Device.OpenUri(new Uri("baidumap://map/direction?origin=latlng:" + App.currentLocation.Latitude + "," + App.currentLocation.Longitude + "|name:我的位置&destination=latlng:" + lat + "," + lng + "|name:" + destination + "&mode=transit"));
                    break;
                case "腾讯地图":
                    Device.OpenUri(new Uri("qqmap://map/routeplan?from=我的位置&type=drive&tocoord=" + lat + "," + lng + "&to=" + destination + "&coord_type=1&policy=0"));
                    break;
                case "苹果地图":

#if __IOS__
                    //Gps gps1 = PositionUtil.gcj_To_Gps84(lat, lng);
                    MKMapItem currentlocation = new MKMapItem(new MKPlacemark(new CoreLocation.CLLocationCoordinate2D(App.currentLocation.Latitude, App.currentLocation.Longitude)));
                    currentlocation.Name = "当前位置";
                    MKPlacemark placemark = new MKPlacemark(new CoreLocation.CLLocationCoordinate2D(lat, lng));
                    MKMapItem tolocation = new MKMapItem(placemark);
                    tolocation.Name = destination;
                    MKMapItem[] items = { currentlocation, tolocation };
                    MKLaunchOptions mKLaunchOptions = new MKLaunchOptions
                    {
                        DirectionsMode = MKDirectionsMode.Driving,
                        ShowTraffic = true,
                    };
                    MKMapItem.OpenMaps(items, mKLaunchOptions);
#endif
                    break;
                default: break;
            }
        }

        //地图放大
        void zoomout(object sender, System.EventArgs e)
        {
            map.ZoomOut();


        }
        //地图缩小
        void zoomin(object sender, System.EventArgs e)
        {
            map.ZoomIn();
        }


        AzmMarkerView currentMarker;
        public RescueSiteMapPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            try
            {
                //Gps gps = PositionUtil.gcj_To_Gps84(App.currentLocation.Latitude, App.currentLocation.Longitude);
                var singlecoord = new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude);

                currentMarker = new AzmMarkerView(ImageSource.FromFile("loc2.png"), new Size(30, 30), singlecoord);
                map.Overlays.Add(currentMarker);
                map.SetCenter(12, singlecoord);

            }
            catch (Exception ex)
            {

            }

        }
        //从救援地点进入
        public RescueSiteMapPage(ObservableCollection<RescueSiteModel.ItemsBean> dataList) : this()
        {
            //// Marker usage sample
            Title = "救援地点";

            foreach (RescueSiteModel.ItemsBean item in dataList)
            {
                Gps gps = PositionUtil.gcj_To_Gps84(item.lat, item.lng);
                var coord = new AzmCoord(gps.getWgLon(), gps.getWgLat());

                ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;
                NavLabelView cv = new NavLabelView(item.name, coord)
                {
                    BackgroundColor = Color.FromHex("#f0f0f0"),
                    Size = new Size(100, 25),
                    Anchor = new Point(50, 25),
                    ControlTemplate = cvt,
                };

                cv.BindingContext = cv;
                cv.NavCommand = new Command(() => { openMapNav(coord.lat, coord.lng, item.name); });

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), coord)
                {
                    BackgroundColor = Color.Transparent,
                    CustomView = cv
                };
                map.Overlays.Add(mv);

            }

        }


        /// <summary>
        /// 添加一个位置信息
        /// </summary>
        /// <param name="title">页面标题</param>
        /// <param name="locdes">位置文字描述</param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public RescueSiteMapPage(string title, string locdes, double lat, double lng) : this()
        {
            Title = string.IsNullOrWhiteSpace(title) ? "位置信息" : title;
            Gps gps = PositionUtil.gcj_To_Gps84(lat, lng);
            var coord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
            ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

            NavLabelView cv = new NavLabelView(locdes, coord)
            {
                BackgroundColor = Color.FromHex("#f0f0f0"),
                Size = new Size(100, 25),
                Anchor = new Point(50, 25),
                ControlTemplate = cvt,
            };

            cv.BindingContext = cv;
            cv.NavCommand = new Command(() => { openMapNav(coord.lat, coord.lng, locdes); });

            AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), coord)
            {
                BackgroundColor = Color.Transparent,
                CustomView = cv
            };
            map.Overlays.Add(mv);
            map.SetCenter(15, new AzmCoord(lng, lat));
        }



        //从敏感源进入
        public RescueSiteMapPage(ObservableCollection<SensitiveModels.ItemsBean> dataList) : this()
        {
            //// Marker usage sample
            ///
            Title = "敏感源";
            foreach (SensitiveModels.ItemsBean item in dataList)
            {
                Gps gps = PositionUtil.gcj_To_Gps84(item.lat, item.lng);
                var coord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
                ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

                NavLabelView cv = new NavLabelView(item.name, coord)
                {
                    BackgroundColor = Color.FromHex("#f0f0f0"),
                    Size = new Size(100, 25),
                    Anchor = new Point(50, 25),
                    ControlTemplate = cvt,
                };

                cv.BindingContext = cv;
                cv.NavCommand = new Command(() => { openMapNav(coord.lat, coord.lng, item.name); });

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), coord)
                {
                    BackgroundColor = Color.Transparent,
                    CustomView = cv
                };
                map.Overlays.Add(mv);

            }

            if (App.currentLocation != null)
            {
                map.SetCenter(12, new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude));
            }


        }

        //从网格化任务详情进入
        public RescueSiteMapPage(Coords coords) : this()
        {
            //// Marker usage sample
            ///
            Title = "相关位置";

            Gps gps = PositionUtil.gcj_To_Gps84(coords.lat.Value, coords.lng.Value);
            var coord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
            ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

            NavLabelView cv = new NavLabelView(coords.title, coord)
            {
                BackgroundColor = Color.FromHex("#f0f0f0"),
                Size = new Size(100, 25),
                Anchor = new Point(50, 25),
                ControlTemplate = cvt,
            };

            cv.BindingContext = cv;
            cv.NavCommand = new Command(() => { openMapNav(coord.lat, coord.lng, coords.title); });

            AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), coord)
            {
                BackgroundColor = Color.Transparent,
                CustomView = cv
            };
            map.Overlays.Add(mv);

            map.SetCenter(12, new AzmCoord(coords.lng.Value, coords.lat.Value));


        }


        //从应急事故详情进入
        public RescueSiteMapPage(ObservableCollection<IncidentLoggingEventsBean> dataList, string incidengtId) : this()
        {
            //// Marker usage sample

            AzmCoord coord = null;
            foreach (IncidentLoggingEventsBean item in dataList)
            {
                if (item.TargetLat != 0 && item.TargetLng != 0)
                {//筛选最新的一次事故中心位置
                    if (coord == null)
                    {
                        if (Convert.ToDouble(item.TargetLat) <= 90.0) coord = new AzmCoord(Convert.ToDouble(item.TargetLng), Convert.ToDouble(item.TargetLat));
                    }
                }
                if (item.lat != null && Convert.ToDouble(item.lat) != 0.0)
                {
                    //因子上传位置
                    if (item.category == "IncidentFactorMeasurementEvent")
                    {
                        //Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(item.lat), Convert.ToDouble(item.lng));
                        var coord1 = new AzmCoord(Convert.ToDouble(item.lng), Convert.ToDouble(item.lat));

                        AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("reddot"), new Size(25, 25), coord1)
                        {
                        };
                        map.Overlays.Add(mv);
                    }
                }
            }

            Console.WriteLine("lat===" + coord.lat + "lng==" + coord.lng);
            //设置target坐标//事故中心位置
            if (coord.lat != 0.0f && coord.lng != 0.0)
            {
                try
                {
                    Gps gps = PositionUtil.gcj_To_Gps84(coord.lat, coord.lng);
                    coord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
                    ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

                    NavLabelView cv = new NavLabelView("事发地点", coord)
                    {
                        BackgroundColor = Color.FromHex("#f0f0f0"),
                        Size = new Size(100, 25),
                        Anchor = new Point(50, 25),
                        ControlTemplate = cvt,
                    };

                    cv.BindingContext = cv;
                    cv.NavCommand = new Command(() => { openMapNav(coord.lat, coord.lng, "事故地点"); });

                    AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(35, 35), coord)
                    {
                        BackgroundColor = Color.Transparent,
                        CustomView = cv
                    };
                    map.Overlays.Add(mv);

                    var fivekmcir = new AzmEllipseView(coord, 5000.0);
                    fivekmcir.StrokeThickness = 6;
                    fivekmcir.DashArray = new float[2] { 10.0f, 10.0f };
                    map.ShapeOverlays.Add(fivekmcir);

                    map.SetCenter(13, coord);
                }
                catch (Exception ex)
                {

                }

            }

            ReqPlanLis(incidengtId);

        }
        //从采样计划进入
        public RescueSiteMapPage(MySamplePlanItems sampleModel) : this()
        {

            foreach (MySamplePlanItems item in App.mySamplePlanResult.Items)
            {
                Gps gps = PositionUtil.gcj_To_Gps84(item.lat, item.lng);
                var coord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
                ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

                NavLabelView cv = new NavLabelView(item.name, coord)
                {
                    BackgroundColor = Color.FromHex("#f0f0f0"),
                    Size = new Size(100, 25),
                    Anchor = new Point(50, 25),
                    ControlTemplate = cvt,
                };

                cv.BindingContext = cv;
                cv.NavCommand = new Command(() => { openMapNav(coord.lat, coord.lng, item.name); });

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("bluetarget"), new Size(24, 24), coord)
                {
                    BackgroundColor = Color.Transparent,
                    CustomView = cv
                };
                if (item == sampleModel)
                {
                    mv.Source = ImageSource.FromFile("orangetarget");
                }
                map.Overlays.Add(mv);

            }
        }

        //事故详情列表进入
        public RescueSiteMapPage(string title, AzmCoord singlecoord) : this()
        {
            Title = title;
            //Gps gps = PositionUtil.gcj_To_Gps84(singlecoord.lat, singlecoord.lng);
            //singlecoord = new AzmCoord(gps.getWgLon(), gps.getWgLat());

            ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

            NavLabelView cv = new NavLabelView(title, singlecoord)
            {
                BackgroundColor = Color.FromHex("#f0f0f0"),
                Size = new Size(100, 25),
                Anchor = new Point(50, 25),
                ControlTemplate = cvt,
            };

            cv.BindingContext = cv;
            cv.NavCommand = new Command(() => { openMapNav(singlecoord.lat, singlecoord.lng, title); });

            AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(30, 30), singlecoord)
            {
                BackgroundColor = Color.Transparent,
                CustomView = cv
            };
            map.Overlays.Add(mv);
            map.SetCenter(13, singlecoord);
        }



        //布点位置
        private async void ReqPlanLis(string incidentId)
        {
            //string url = App.BasicDataModule.url + DetailUrl.ChemicalList;
            string url = "http://gx.azuratech.com:30011/api/Sampleplan/GetPlanListByProid" + "?Proid=" + incidentId;
            Console.WriteLine(url);

            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, null, "GET", null);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                List<BuDianItem> list = JsonConvert.DeserializeObject<List<BuDianItem>>(hTTPResponse.Results);
                foreach (BuDianItem item in list)
                {
                    Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(item.lat), Convert.ToDouble(item.lng));

                    AzmCoord singlecoord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
                    ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;
                    NavLabelView cv = new NavLabelView(item.address, singlecoord)
                    {
                        BackgroundColor = Color.FromHex("#f0f0f0"),

                        ControlTemplate = cvt,
                    };




                    cv.BindingContext = cv;
                    var s = cv.Measure(100.0, 1000.0);


                    cv.NavCommand = new Command(() => { openMapNav(singlecoord.lat, singlecoord.lng, item.address); });
                    AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("bluetarget"), new Size(30, 30), singlecoord)
                    {
                        BackgroundColor = Color.Transparent,
                        CustomView = cv
                    };
                    map.Overlays.Add(mv);

                    //AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("bluetarget"), new Size(30, 30), new AzmCoord(Convert.ToDouble(item.lng), Convert.ToDouble(item.lat)))
                    //{
                    //Text = item.address,
                    //};
                    //map.Overlays.Add(mv);
                }
            }
        }



        internal class BuDianItem
        {
            public string address { set; get; }
            public string name { set; get; }
            public double? lng { set; get; }
            public double? lat { set; get; }
        }


    }







    public class NavLabelView : AzmOverlayView
    {
        public delegate void OnTappedEventHandler(object sender, EventArgs e);
        public event OnTappedEventHandler OnTapped;

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(AzmLabelView),
            defaultValue: default(string)
        );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(AzmLabelView),
            defaultValue: Color.White
        );


        private ICommand _navcommand = null;

        public ICommand NavCommand
        {
            get { return _navcommand; }
            set { _navcommand = value; }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public NavLabelView(string text, AzmCoord coord = null, double maxwidthrequest = 120.0)
        {
            if (text == null) text = "";
            BackgroundColor = Color.FromHex("#002060");
            Text = text.Trim();
            WidthRequest = maxwidthrequest;
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);

            this.BindingContext = new { name = Text };
            double height = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextHeightGivenMaxWidth(Text, WidthRequest - 40, 14));
            double width = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextWidthGivenExactHeight(Text, height, 14));
            width = Math.Min(width, WidthRequest - 40);
            Size = new Size(width + 40, height + 6);
            Anchor = new Point((width + 40) / 2, height + 6 + 7);

            NavCommand = new Command(() =>
            {
                // navigation here
            });
        }

        public override void Attached(AzMapView mapview)
        {
            base.Attached(mapview);

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Tap_Tapped;
            this.GestureRecognizers.Add(tap);

        }

        public override void Detached()
        {
            base.Detached();
            this.GestureRecognizers.Clear();
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            if (OnTapped != null) OnTapped(this, e);
        }
    }

}
