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

namespace AepApp.View
{
    public partial class PollutionSourceMapPage : ContentPage
    {
        ObservableCollection<EnterpriseModel> _enterList = null;
        public PollutionSourceMapPage(ObservableCollection<EnterpriseModel> enterList)
        {
            InitializeComponent();
            _enterList = enterList;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            InitLocationService();
            if (_enterList != null)
            {
                double x = 0;
                double y = 0;
                int cnt = 0;
                for (int i = 0; i < _enterList.Count; i++)
                {
                    EnterpriseModel enter = _enterList[i];
                    double lng = double.Parse(enter.lng);
                    double lat = double.Parse(enter.lat);

                    AzmLabelView lv = new AzmLabelView(enter.name, new AzmCoord(lng, lat))
                    {
                        BackgroundColor = Color.FromHex("#4169E1")
                    };
                    lv.BindingContext = enter;
                    lv.OnTapped += Enterprise_OnTapped;
                    map.Overlays.Add(lv);

                    x += lng;
                    y += lat;
                    cnt++;
                }

                x /= cnt;
                y /= cnt;
                map.SetCenter(11, new AzmCoord(x, y));
            }
        }

        public void InitLocationService()
        {
            //map.LocationService.LocationUpdated += (_, e) =>
            //{
            //    //Debug.WriteLine("LocationUpdated: " + ex.Coordinate);
            //    if (!moved)
            //    {
            //        map.Center = e.Coordinate;
            //        moved = true;
            //    }
            //};

            //map.LocationService.Failed += (_, e) =>
            //{
            //    Console.WriteLine("Location failed: " + e.Message);
            //};

            //map.LocationService.Start();
        }

        private void Enterprise_OnTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PollutionSourceInfoPage((sender as AzmLabelView).BindingContext as EnterpriseModel));
        }
    }
}
