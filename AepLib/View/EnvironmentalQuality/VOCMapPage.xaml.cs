using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AepApp.Models;
using AepApp.Tools;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalQuality
{
    public partial class VOCMapPage : ContentPage
    {
        private bool firsttime = true;

        public VOCMapPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
        }

        public VOCMapPage(ObservableCollection<WaterQualityItem> dataList) : this()
        {

            //var site = dataList[0];
            //BackgroundWorker wrk = new BackgroundWorker();
            //wrk.DoWork += (sender, e) => {
            double x = 0;
            double y = 0;
            int cnt = 0;
            for (int i = 0; i < dataList.Count; i++)
            {
                var site = dataList[i];
                if (site == null || site.basic == null) continue;
                if (string.IsNullOrWhiteSpace(site.basic.lng) || string.IsNullOrWhiteSpace(site.basic.lat)) continue;
                if (Convert.ToDouble(site.basic.lng) < 90 || Convert.ToDouble(site.basic.lat) <= 0) continue;

                AzmLabelView lv = new AzmLabelView(site.basic.stname, new AzmCoord(Convert.ToDouble(site.basic.lng), Convert.ToDouble(site.basic.lat)))
                {
                    BackgroundColor = Color.FromHex("#4169E1"),
                    BindingContext = site.basic,
                };
                lv.OnTapped += Lv_OnTapped;
                map.Overlays.Add(lv);
                map.SetCenter(13, new AzmCoord(Convert.ToDouble(site.basic.lng), Convert.ToDouble(site.basic.lat)), false);
                if (firsttime)
                {
                    x += Convert.ToDouble(site.basic.lng);
                    y += Convert.ToDouble(site.basic.lat);
                    cnt++;
                }
            }
            if (firsttime)
            {
                if (cnt > 0)
                {
                    x /= cnt;
                    y /= cnt;
                    map.SetCenter(11, new AzmCoord(x, y), false);
                }
                firsttime = false;
            }
        }

        public VOCMapPage(ObservableCollection<VOCSiteListModel> dataList):this()
        {

            //var site = dataList[0];
            //BackgroundWorker wrk = new BackgroundWorker();
            //wrk.DoWork += (sender, e) => {
            double x = 0;
            double y = 0;
            int cnt = 0;
            for (int i = 0 ; i< dataList.Count; i++)
                {
                var site = dataList[i];
                if (site == null || string.IsNullOrWhiteSpace(site.lng) || string.IsNullOrWhiteSpace(site.lat)) continue;
                if (Convert.ToDouble(site.lng) < 90 || Convert.ToDouble(site.lat) <= 0) continue;

                AzmLabelView lv = new AzmLabelView(site.name, StringUtils.string2Coord(site.lng, site.lat))
                {
                    BackgroundColor = Color.FromHex("#4169E1"),
                    BindingContext = site,
                    };
                lv.OnTapped += Lv_OnTapped;
                    map.Overlays.Add(lv);
                    map.SetCenter(13, StringUtils.string2Coord(site.lng, site.lat), false);
                    if (firsttime)
                    {
                        x += StringUtils.string2Double(site.lng);
                        y += StringUtils.string2Double(site.lat);
                        cnt++;
                    }
                }
                if (firsttime)
                {
                    if (cnt > 0)
                    {
                        x /= cnt;
                        y /= cnt;
                        map.SetCenter(11, new AzmCoord(x, y), false);
                    }
                    firsttime = false;
                }   
        }

        void Lv_OnTapped(object sender, EventArgs e)
        {
            AzmLabelView lv = sender as AzmLabelView;
            VOCSiteListModel item = lv.BindingContext as VOCSiteListModel;

            int type = 0;
            if (Title == App.moduleConfigENVQ.menuChangjieLabel || Title == App.moduleConfigENVQ.menuPaikouLabel || Title == App.moduleConfigENVQ.menuQyStenchLabel) type = 2;
            else type = 1;
            if (item != null)
            {
                Navigation.PushAsync(new VOCDetailPage(item.id, type) {
                    Title = item.name
                });
            }
            else
            {
                WaterQualityBasic item1 = lv.BindingContext as WaterQualityBasic;
                Navigation.PushAsync(new VOCDetailPage(item1.id, type) { Title = item1.name});
            }
        }

    }
}
