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
        List<BuDianItem> list = new List<BuDianItem>();
        ObservableCollection<BuDianItem> sources = new ObservableCollection<BuDianItem>();
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
                var singlecoord = new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude);

                currentMarker = new AzmMarkerView(ImageSource.FromFile("loc2.png"), new Size(30, 30), singlecoord);
                map.Overlays.Add(currentMarker);
                map.SetCenter(12, singlecoord, false);
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            map.InvalidateSurface();
        }

        public EmergencyMapPage(ObservableCollection<IncidentLoggingEventsBean> dataList, string incidengtId) : this()
        {
            foreach (IncidentLoggingEventsBean item in dataList)
            {
               
                if (item.lat != null && Convert.ToDouble(item.lat) != 0.0)
                {
                    //因子上传位置
                    if (item.category == "IncidentFactorMeasurementEvent")
                    {
                        //Gps gps = PositionUtil.gcj_To_Gps84(Convert.ToDouble(item.lat), Convert.ToDouble(item.lng));
                        var coord1 = new AzmCoord(Convert.ToDouble(item.lng), Convert.ToDouble(item.lat));
                        //红色原点
                        AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("reddot"), new Size(25, 25), coord1)
                        {
                        };
                        map.Overlays.Add(mv);
                    }
                }
            }

            //设置target坐标//事故中心位置
            if (App.EmergencyCenterCoord.lat != 0.0f && App.EmergencyCenterCoord.lng != 0.0)
            {
                try
                {
                    ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;
                    NavLabelView cv = new NavLabelView("事发地点", App.EmergencyCenterCoord)
                    {
                        BackgroundColor = Color.FromHex("#f0f0f0"),
                        Size = new Size(100, 25),
                        Anchor = new Point(50, 25),
                        ControlTemplate = cvt,
                    };

                    cv.BindingContext = cv;
                    cv.NavCommand = new Command(() => { openMapNav(App.EmergencyCenterCoord.lat, App.EmergencyCenterCoord.lng, "事故地点"); });

                    AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(35, 35), App.EmergencyCenterCoord)
                    {
                        BackgroundColor = Color.Transparent,
                        CustomView = cv
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
            _incidengtId = incidengtId;
            ReqPlanLis(incidengtId);

        }


        //应急布点位置/采样计划
        private async void ReqPlanLis(string incidentId)
        {
            string url = App.SamplingModule.url + "/api/Sampleplan/GetPlanListByProid" + "?Proid=" + incidentId;
            Console.WriteLine(url);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, null, "GET", null);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                list = JsonConvert.DeserializeObject<List<BuDianItem>>(hTTPResponse.Results);
                if (list != null && list.Count > 0)
                {
                    lvPlanList.IsVisible = true;
                    sources = new ObservableCollection<BuDianItem>(list);
                    lvPlanList.ItemsSource = sources;
                }
                else
                {
                    lvPlanList.IsVisible = false;
                }
                AddOverlays();
            }
        }

        //地图上添加点位图层
        private void AddOverlays()
        {
            if (list == null || list.Count == 0)
            {
                return;
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
                mv.BindingContext = item;
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += MarkerIcon_Tapped;
                mv.GestureRecognizers.Add(tap);
                map.Overlays.Add(mv);
            }
        }

        private AzmMarkerView mLastClickMv;
        private BuDianItem dianSelect = null;

        //点位点击
        private void MarkerIcon_Tapped(object sender, EventArgs e)
        {
            AzmMarkerView mv = sender as AzmMarkerView;
            if (mv == null) return;
            BuDianItem dian = mv.BindingContext as BuDianItem;
            if (mv == mLastClickMv)//取消选中
            {
                mv.Source = ImageSource.FromFile("bluetarget");
                selectItem(dian, true);
                mLastClickMv = null;
            }
            else//选中
            {
                mv.Source = ImageSource.FromFile("orangetarget");
                if (mLastClickMv != null)
                {
                    mLastClickMv.Source = ImageSource.FromFile("bluetarget");
                }
                selectItem(dian, false);
                mLastClickMv = mv;
            }
        }

        //选中/取消列表中的点位
        private void selectItem(BuDianItem dian, bool isCancel)
        {
            if (dian == null) return;
            if (list == null || list.Count == 0) return;
            if (dian.lat == null) return;
            foreach (var item in list)
            {
                if (item.lat != null && item.lat.Value == dian.lat.Value)
                {
                    item.selected = !isCancel;
                    if (!isCancel)
                    {
                        dianSelect = item;
                    }
                }
                else
                {
                    item.selected = false;
                }
            }
            if (isCancel)
            {
                dianSelect = null;
            }
            sources = new ObservableCollection<BuDianItem>(list);
            lvPlanList.ItemsSource = sources;
            if (dianSelect != null)
            {
                lvPlanList.ScrollTo(dianSelect, ScrollToPosition.MakeVisible, true);
            }
        }


        internal class BuDianItem
        {
            public string id { set; get; }
            public string address { set; get; }
            public string name { set; get; }
            public double? lng { set; get; }
            public double? lat { set; get; }
            public bool selected { set; get; }
        }

        //点击布点计划
        private async void lvPlanList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            BuDianItem bu = e.SelectedItem as BuDianItem;
            if (bu == null)
            {
                return;
            }
            lvPlanList.SelectedItem = null;
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
            };
            await Navigation.PushAsync(page);
        }

        //删除点位
        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            if (dianSelect == null)
            {
                await DisplayAlert("提示", "请先在地图上点击要删除的点位", "确定");
                return;
            }
            var sure = await DisplayAlert("提示", "确定删除该点位吗？", "确定", "取消");
            if (!sure)
            {
                return;
            }
            string url = App.SamplingModule.url + "/api/SamplePlan/UpdateAll";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("flag", "Del");
            dic.Add("id", dianSelect.id);
            List<Dictionary<string, object>> l = new List<Dictionary<string, object>>();
            l.Add(dic);
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PlanLists", l);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(param), "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                bool res = JsonConvert.DeserializeObject<Boolean>(hTTPResponse.Results);
                if (res)
                {
                    await DisplayAlert("提示", "删除成功", "确定");
                    if (list != null)
                    {
                        list.Remove(dianSelect);
                        sources = new ObservableCollection<BuDianItem>(list);
                        lvPlanList.ItemsSource = sources;
                    }
                    //if (mLastClickMv != null)
                    //{
                    //    mLastClickMv.Detached();
                    //}
                    if (map != null && map.Overlays != null && mLastClickMv != null)
                    {
                        map.Overlays.Remove(mLastClickMv);
                    }
                    dianSelect = null;
                    mLastClickMv = null;
                }
                else
                {
                    await DisplayAlert("提示", "删除失败", "确定");
                }
            }
        }

        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

    }
}