using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using AepApp.Models;
using static AepApp.Models.EmergencyAccidentInfoDetail;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueSiteMapPage : ContentPage
    {


        async void HandleEventHandler()
        {
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    map.SetCenter(13, new AzmCoord(location.Longitude, location.Latitude));

                    currentMarker.Coord = new AzmCoord(location.Longitude,location.Latitude);
                }
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
        AzmMarkerView currentMarker;
        public RescueSiteMapPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            currentMarker = new AzmMarkerView(ImageSource.FromFile("loc2.png"),new Size(30, 30) ,new AzmCoord(0.0,0.0));
            map.Overlays.Add(currentMarker);
            HandleEventHandler();
        }
        //从救援地点进入
        public RescueSiteMapPage(ObservableCollection<RescueSiteModel.ItemsBean> dataList):this(){
            //// Marker usage sample
            Title = "救援地点";
            foreach(RescueSiteModel.ItemsBean item in dataList){

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), new AzmCoord(item.lng, item.lat))
                {
                    Text = item.name
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
                    Text = item.name
                };
                map.Overlays.Add(mv);

            }

        }
        //从应急事故详情进入
        public RescueSiteMapPage(ObservableCollection<IncidentLoggingEventsBean> dataList) : this()
        {
            //// Marker usage sample

            AzmCoord coord = null; 
             foreach (IncidentLoggingEventsBean item in dataList)
            {

                if(item.TargetLat !=null){
                    coord = new AzmCoord(item.TargetLng.Value, item.TargetLat.Value);


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

            //设置target坐标
            if (coord != null)
            {
                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(35, 35), coord)
                {

                };
                map.Overlays.Add(mv);
            }



        }

    }
}
