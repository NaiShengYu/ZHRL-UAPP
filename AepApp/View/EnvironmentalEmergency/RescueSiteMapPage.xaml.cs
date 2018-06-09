using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using AepApp.Models;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueSiteMapPage : ContentPage
    {

        static int i = 0;

        async void HandleEventHandler()
        {

            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    map.SetCenter(13, new AzmCoord(location.Longitude, location.Latitude));
                    if (i > 0)
                    {
                        return;
                    } 
                    var img = ImageSource.FromFile("bluetarget.png");
                    var aaa = new AzmMarkerView(img, new Size(15, 15), new AzmCoord(location.Longitude, location.Latitude));
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

        public RescueSiteMapPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            HandleEventHandler();
        }

        public RescueSiteMapPage(ObservableCollection<RescueSiteModel.ItemsBean> dataList):this(){
            //// Marker usage sample

            foreach(RescueSiteModel.ItemsBean item in dataList){

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), new AzmCoord(item.lng, item.lat))
                {
                    Text = item.name
                };
                map.Overlays.Add(mv);

            }

        }

        public RescueSiteMapPage(ObservableCollection<SensitiveModels.ItemsBean> dataList) : this()
        {
            //// Marker usage sample

            foreach (SensitiveModels.ItemsBean item in dataList)
            {

                AzmMarkerView mv = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(24, 24), new AzmCoord(item.lng, item.lat))
                {
                    Text = item.name
                };
                map.Overlays.Add(mv);

            }

        }


    }
}
