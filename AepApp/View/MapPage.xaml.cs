using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using System.IO;


#if __MOBILE__
using Xamarin.Forms.BaiduMaps;
#endif


namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPage : ContentPage
	{
		public MapPage ()
		{
			InitializeComponent ();
            this.Title = "AQI分布情况";

            IMapManager mapManager = DependencyService.Get<IMapManager>();
            Console.WriteLine(mapManager.CoordinateType);
            mapManager.CoordinateType = CoordType.GCJ02;
            Console.WriteLine(mapManager.CoordinateType);

            map.Loaded += MapLoaded;

            IOfflineMap offlineMap = DependencyService.Get<IOfflineMap>();
            offlineMap.HasUpdate += (_, e) =>
            {
                Console.WriteLine("OfflineMap has update: " + e.CityID);
            };
            offlineMap.Downloading += (_, e) =>
            {
                Console.WriteLine("OfflineMap downloading: " + e.CityID);
            };

            var list = offlineMap.HotList;
            list = offlineMap.AllList;
            //offlineMap.Remove(131);
            var curr = offlineMap.Current;
            //offlineMap.Start(27);
            //offlineMap.Start(75);
            curr = offlineMap.Current;

            // 计算
            ICalculateUtils calc = DependencyService.Get<ICalculateUtils>();
            Console.WriteLine(calc.CalculateDistance(
                new Coordinate(40, 116),
                new Coordinate(41, 117)
            ));//139599.429229778 in iOS, 139689.085961837 in Android

        }

        public void MapLoaded(object sender, EventArgs x)
        {
            map.ShowScaleBar = true;
            InitLocationService();
            InitEvents();

            //Coordinate[] coords = {
            //    new Coordinate(40.044, 116.391),
            //    new Coordinate(39.861, 116.284),
            //    new Coordinate(39.861, 116.468)
            //};

            //map.Polygons.Add(new Polygon
            //{
            //    Points = new ObservableCollection<Coordinate>(coords),
            //    Color = Color.Blue,
            //    FillColor = Color.Red.MultiplyAlpha(0.7),
            //    Width = 2,
            //    Title = "多边形",               
            //});
        
            //map.Circles.Add(new Circle
            //{
            //    Coordinate = map.Center,
            //    Color = Color.Green,
            //    FillColor = Color.Yellow.MultiplyAlpha(0.2),
            //    Radius = 200,
            //    Width = 2,
            //    Title = "圆",
            //});

            //Task.Run(() =>
            //{
            //    for (; ; )
            //    {
            //        Task.Delay(1000).Wait();

            //        var p = map.Polygons[0].Points[0];
            //        p = new Coordinate(p.Latitude + 0.002, p.Longitude);
            //        map.Polygons[0].Points[0] = p;

            //        map.Circles[0].Radius += 100;
            //    }
            //});

            // 坐标转换
            IProjection proj = map.Projection;
            var coord = proj.ToCoordinate(new Point(100, 100));
            Console.WriteLine(proj.ToScreen(coord));
        }

        private static bool moved = false;
        public void InitLocationService()
        {
            map.LocationService.LocationUpdated += (_, e) =>
            {
                //Debug.WriteLine("LocationUpdated: " + ex.Coordinate);
                if (!moved)
                {
                    map.Center = e.Coordinate;
                    moved = true;
                }
            };

            map.LocationService.Failed += (_, e) =>
            {
                Console.WriteLine("Location failed: " + e.Message);
            };

            map.LocationService.Start();
        }

        public void InitEvents()
        {
            //btnTrack.Clicked += (_, e) => {
            //    if (map.ShowUserLocation)
            //    {
            //        map.UserTrackingMode = UserTrackingMode.None;
            //        map.ShowUserLocation = false;
            //    }
            //    else
            //    {
            //        map.UserTrackingMode = UserTrackingMode.Follow;
            //        map.ShowUserLocation = true;
            //    }
            //};

            map.LongClicked += (_, e) =>
            {
                AddPin(e.Coordinate);
            };

            map.StatusChanged += (_, e) =>
            {
                //Debug.WriteLine(map.Center + " @" + map.ZoomLevel);
            };
        }

        void AddPin(Coordinate coord)
        {
            //var img1 = XImage.FromResource("AepApp.Droid.voc.png");
            //XImage img2 = XImage.FromFile("voc.png");         
            //var img3 = XImage.FromBundle("voc");
            //var img4= XImage.FromResource("AepApp.Droid.voc.png");
            //Stream aaaaa = typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream(this.GetType(),"AepApp.Images.pin_purple.png");
            //Stream bbbb = typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.pin_purple.png");
            //Stream bbbb = typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.voc.png");
            // FileStream ccc = typeof(MapPage).GetTypeInfo().Assembly.GetFile("voc.png");
            Pin annotation = new Pin
            {
                Title = coord,
                Coordinate = coord,
                Animate = true,
                Draggable = false,
                Enabled3D = true,
                //Image = img2
                Image = XImage.FromStream(
                    //typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("App1.Images.pin_purple.png")
                   // bbbb
                    typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.voc.png")
                )
                //Image = XImage.FromResource(
                //    "AepApp.Droid.voc.png"
                //)
            };
            map.Pins.Add(annotation);
            
            annotation.Drag += (o, e) =>
            {
                Pin self = o as Pin;
                self.Title = null;//self.Coordinate;
                int i = map.Pins.IndexOf(self);

                if (map.Polylines.Count > 0 && i > -1)
                {
                    map.Polylines[0].Points[i] = self.Coordinate;
                }
            };            
            annotation.Clicked += (_, e) =>
            {
                Console.WriteLine("clicked");
                Pin self = _ as Pin;
                DependencyService.Get<Sample.IToast>().ShortAlert("......");
                //self.Title = "sfcdef";
                //((Pin)_).Image = XImage.FromStream(
                //    typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("Sample.Images.10660.png")
                //);
            };

            if (0 == map.Polylines.Count && map.Pins.Count > 1)
            {
                Polyline polyline = new Polyline
                {
                    Points = new ObservableCollection<Coordinate> {
                        map.Pins[0].Coordinate, map.Pins[1].Coordinate
                    },
                    Width = 4,
                    Color = Color.Purple
                };

                map.Polylines.Add(polyline);
            }
            else if (map.Polylines.Count > 0)
            {
                map.Polylines[0].Points.Add(annotation.Coordinate);
            }
        }
    }
}