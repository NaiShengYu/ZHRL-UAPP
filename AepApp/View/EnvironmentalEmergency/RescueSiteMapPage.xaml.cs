using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using AepApp.Models;
using static AepApp.Models.EmergencyAccidentInfoDetail;
using CloudWTO.Services;
using Newtonsoft.Json;
using System.Windows.Input;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueSiteMapPage : ContentPage
    {


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

            try{
                currentMarker = new AzmMarkerView(ImageSource.FromFile("loc2.png"), new Size(30, 30), new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude));
                map.Overlays.Add(currentMarker);

            }catch(Exception ex){
                
            }
           
        }
        //从救援地点进入
        public RescueSiteMapPage(ObservableCollection<RescueSiteModel.ItemsBean> dataList):this(){
            //// Marker usage sample
            Title = "救援地点";
            foreach(RescueSiteModel.ItemsBean item in dataList){

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), new AzmCoord(item.lng, item.lat))
                {
                    Text = item.name,
                    AlwaysShowLabel = true
                };
                map.Overlays.Add(mv);

            }

        }
        //从敏感源进入
        public RescueSiteMapPage(ObservableCollection<SensitiveModels.ItemsBean> dataList) : this()
        {
            //// Marker usage sample
            ///
            Title = "敏感源";
            foreach (SensitiveModels.ItemsBean item in dataList)
            {
                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), new AzmCoord(item.lng, item.lat))
                {
                    Text = item.name,
                    AlwaysShowLabel = true
                };
                map.Overlays.Add(mv);
            }

            if(App.currentLocation !=null){
                map.SetCenter(12, new AzmCoord(App.currentLocation.Longitude,App.currentLocation.Latitude));
            }
             

        }
        //从应急事故详情进入
        public RescueSiteMapPage(ObservableCollection<IncidentLoggingEventsBean> dataList,string incidengtId) : this()
        {
            //// Marker usage sample

            AzmCoord coord = null; 
             foreach (IncidentLoggingEventsBean item in dataList)
            {
                if(item.TargetLat !=null){//筛选最新的一次事故中心位置
                    if(Convert.ToDouble(item.TargetLat)<=90.0)coord = new AzmCoord(Convert.ToDouble(item.TargetLng), Convert.ToDouble(item.TargetLat));
                }
                if (item.lat != null)
                {
                    if (item.category == "IncidentFactorMeasurementEvent")
                    {
                        AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("reddot"), new Size(25, 25), new AzmCoord(Convert.ToDouble(item.lng), Convert.ToDouble(item.lat)))
                        {

                        };
                        map.Overlays.Add(mv);
                    }
                }
            }

            Console.WriteLine("lat==="+coord.lat+"lng=="+ coord.lng);
            //设置target坐标//事故中心位置
            if (coord != null)
            {
                try
                {
                    ControlTemplate cvt = Resources["labelwithnavtemp"] as ControlTemplate;

                    NavLabelView cv = new NavLabelView("事发地点", coord)
                    {
                        BackgroundColor = Color.FromHex("#f0f0f0"),
                        Size = new Size(100, 25),
                        Anchor = new Point(50, 25),
                        ControlTemplate = cvt,
                    };

                    cv.BindingContext = cv;

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

        private async void ReqPlanLis(string incidentId)
        {
            //string url = App.BasicDataModule.url + DetailUrl.ChemicalList;
            string url = "http://gx.azuratech.com:30011/api/Sampleplan/GetPlanListByProid" + "?Proid=" + incidentId;
            Console.WriteLine(url);
          
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, null, "GET",null);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                List<BuDianItem> list = JsonConvert.DeserializeObject<List<BuDianItem>>(hTTPResponse.Results);
                foreach(BuDianItem item in list){
                    AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("bluetarget"), new Size(30, 30), new AzmCoord(Convert.ToDouble(item.lng), Convert.ToDouble(item.lat)))
                    {
                        Text = item.address,
                        };
                        map.Overlays.Add(mv);
                }
            }
        }

        internal class BuDianItem{
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

        public NavLabelView(string text, AzmCoord coord = null, double maxwidthrequest = 100.0)
        {
            if (text == null) text = "";
            BackgroundColor = Color.FromHex("#002060");
            Text = text.Trim();
            WidthRequest = maxwidthrequest;
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);

            this.BindingContext = new { name = Text };
            double height = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextHeightGivenMaxWidth(Text, WidthRequest - 6, 14));
            double width = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextWidthGivenExactHeight(Text, height, 14));
            width = Math.Min(width, WidthRequest - 6);
            Size = new Size(width + 6, height + 6);
            Anchor = new Point((width + 6) / 2, height + 6 + 7);

            NavCommand = new Command(() => {
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
