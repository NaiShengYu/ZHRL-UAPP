using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AepApp.View.Monitor;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Reflection;
using System.IO;
using System.Diagnostics;
#if __MOBILE__
using Xamarin.Forms.BaiduMaps;
#endif
namespace AepApp.View
{
    public partial class PollutionSourceMapPage : ContentPage
    {
        void Annotation_Clicked(object sender, EventArgs e)
        {
            Pin pin = sender as Pin;
            Console.WriteLine("点击了大头针：" + pin.Tag);
            Navigation.PushAsync(new PollutionSourceInfoPage(pin.Tag as EnterpriseModel));

        }

        ObservableCollection<EnterpriseModel> _enterList = null;
        public PollutionSourceMapPage(ObservableCollection<EnterpriseModel> enterList)
        {
            InitializeComponent();
            this.Title = "污染源在线";
            _enterList = enterList;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            IMapManager mapManager = DependencyService.Get<IMapManager>();
            Console.WriteLine(mapManager.CoordinateType);
            mapManager.CoordinateType = CoordType.GCJ02;
            Console.WriteLine(mapManager.CoordinateType);

            map.Loaded += MapLoaded;

        }
        public void MapLoaded(object sender, EventArgs x)
        {
            map.ShowScaleBar = true;
            InitLocationService();
            if (_enterList !=null){
                for (int i = 0; i < _enterList.Count;i ++){
                    EnterpriseModel enter = _enterList[i];
                    AddPin(enter);
                }

            }       
        }

        private bool moved = false;
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
        //添加大头针
        void AddPin(EnterpriseModel enter)
        {
            //Pin annotation = new Pin
            //{
            //    //Title = enter.value.ToString(),
            //    Coordinate = new Coordinate(double.Parse(enter.lat), double.Parse(enter.lng)),
            //    Animate = true,
            //    Draggable = false,
            //    Enabled3D = true,
            //};
            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    annotation.Image = XImage.FromStream(
            //        typeof(AQIMapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.iOS.pin.png")
            //       );
            //}
            //else
            //{
            //    annotation.Image = XImage.FromStream(
            //        typeof(AQIMapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.pin.png")
            //        );
            //}
            //annotation.Tag = enter;
            //annotation.Clicked += Annotation_Clicked;
            //map.Pins.Add(annotation);


            Pin pin = new Pin
            {
                Coordinate = new Coordinate(double.Parse(enter.lat), double.Parse(enter.lng)),
                Animate = false,
                Draggable = false,
                Enabled3D = false,
                Image = XImage.FromStream(typeof(MapPage2).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.label_back_blue.png")),
                AnchorY = 0.5f,
                AnchorX = 0.5f
            };
            pin.Tag = enter;
            pin.Clicked += Annotation_Clicked;
            map.Pins.Add(pin);

            string name = enter.name;
            if (enter.name.Length > 6) name = enter.name.Substring(0, 5) + "...";

            Xamarin.Forms.BaiduMaps.Label label = new Xamarin.Forms.BaiduMaps.Label
            {
                Title = name,
                Coordinate = new Coordinate(double.Parse(enter.lat), double.Parse(enter.lng)),
                BackgroundColor = Color.FromRgb(55, 113, 184),
                FontColor = Color.White,
                FontSize = 22
            };
            label.Tag = enter;
            label.Clicked += Annotation_Clicked;
            map.Labels.Add(label);

        }


    }
}
