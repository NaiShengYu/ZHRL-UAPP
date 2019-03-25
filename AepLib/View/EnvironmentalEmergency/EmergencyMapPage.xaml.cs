using AepApp.Interface;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using static AepApp.Models.EmergencyAccidentInfoDetail;

//应急事故地图
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyMapPage : ContentPage
    {
        AzmMarkerView currentMarker;
        string _incidengtId = "";
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

        public EmergencyMapPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
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

        public EmergencyMapPage(ObservableCollection<IncidentLoggingEventsBean> dataList, string incidengtId) : this()
        {
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
            _incidengtId = incidengtId;
            ReqPlanLis(incidengtId);

        }


        //应急布点位置/采样计划
        private async void ReqPlanLis(string incidentId)
        {
            //string url = App.BasicDataModule.url + DetailUrl.ChemicalList;
            string url = App.SamplingModule.url + "/api/Sampleplan/GetPlanListByProid" + "?Proid=" + incidentId;
            Console.WriteLine(url);

            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, null, "GET", null);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                List<BuDianItem> list = JsonConvert.DeserializeObject<List<BuDianItem>>(hTTPResponse.Results);
                if (list != null && list.Count > 0)
                {
                    lvPlanList.IsVisible = true;
                    ObservableCollection<BuDianItem> o = new ObservableCollection<BuDianItem>(list);
                    lvPlanList.ItemsSource = o;
                }
                foreach (BuDianItem item in list)
                {
                    //Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(item.lat), Convert.ToDouble(item.lng));
                    AzmCoord singlecoord = new AzmCoord(Convert.ToDouble(item.lng), Convert.ToDouble(item.lat));
                    ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;
                    NavLabelView cv = new NavLabelView(item.name, singlecoord)
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
                }
            }
        }


        internal class BuDianItem
        {
            public string address { set; get; }
            public string name { set; get; }
            public double? lng { set; get; }
            public double? lat { set; get; }
            public string id { get; set; }
        }

        //点击布点计划
        private async void lvPlanList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            BuDianItem bu = e.SelectedItem as BuDianItem;
            if (bu == null)
            {
                return;
            }
            //await DisplayActionSheet("点击" + bu.name, "取消", "确定");

            await Navigation.PushAsync(new AddPlacementPage(bu.id));



        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
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
                Navigation.InsertPageBefore(new AddPlacementPage(_incidengtId, bbb[1], bbb[0]), page);

                //todo
            };
            await Navigation.PushAsync(page);
        }

        private void BtnDelete_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EmergencyAddTaskPage());
        }

        private void DeletePlan()
        {

        }
    }
}