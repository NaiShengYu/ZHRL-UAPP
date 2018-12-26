using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalQuality
{
    public partial class VOCMapPage : ContentPage
    {
        private bool firsttime = true;




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
        public VOCMapPage(ObservableCollection<VOCSiteListModel> dataList)
        {
            InitializeComponent();
            Title = "VOC地点";
            NavigationPage.SetBackButtonTitle(this, "");

            //var site = dataList[0];
            //BackgroundWorker wrk = new BackgroundWorker();
            //wrk.DoWork += (sender, e) => {
            double x = 0;
            double y = 0;
            int cnt = 0;
            for (int i = 0 ; i< dataList.Count; i++)
                {
                var site = dataList[i];
                if (Convert.ToDouble(site.lng) < 90 || Convert.ToDouble(site.lat) <= 0) continue;

                AzmLabelView lv = new AzmLabelView(site.name, new AzmCoord(Convert.ToDouble(site.lng), Convert.ToDouble(site.lat)))
                    {
                        BackgroundColor = Color.FromHex("#4169E1"),
                    };
                lv.OnTapped += Lv_OnTapped;
                    map.Overlays.Add(lv);
                    map.SetCenter(13,new AzmCoord(Convert.ToDouble(site.lng), Convert.ToDouble(site.lat)));
                    if (firsttime)
                    {
                        x += Convert.ToDouble(site.lng);
                        y += Convert.ToDouble(site.lat);
                        cnt++;
                    }
                }
                if (firsttime)
                {
                    if (cnt > 0)
                    {
                        x /= cnt;
                        y /= cnt;
                        map.SetCenter(11, new AzmCoord(x, y));
                    }
                    firsttime = false;
                }

            //};
            //wrk.RunWorkerCompleted += (sender, e) => {


            //};
            //wrk.RunWorkerAsync();




        }

        void Lv_OnTapped(object sender, EventArgs e)
        {

        }

    }
}
