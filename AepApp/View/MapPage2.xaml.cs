using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.BaiduMaps;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage2 : ContentPage
    {
        private List<AirPageModels.AirInfo> sites = new List<AirPageModels.AirInfo>();
        private string result;
        List<AirDetailModels.Factors> factors = new List<AirDetailModels.Factors>();
        Dictionary<string, AirPageModels.AirInfo> idsitedict = new Dictionary<string, AirPageModels.AirInfo>();

        Dictionary<string, string> finishedloading = new Dictionary<string, string>();

        public MapPage2(List<AirPageModels.AirInfo> sendPages)
        {
            InitializeComponent();
            this.Title = "AQI分布情况";
            this.sites = sendPages;
            IMapManager mapManager = DependencyService.Get<IMapManager>();
            Console.WriteLine(mapManager.CoordinateType);
            mapManager.CoordinateType = CoordType.GCJ02;
            Console.WriteLine(mapManager.CoordinateType);

            map.Loaded += MapLoaded; //地图相关操作   

            //数据相关操作
            if (sites.Count != 0)
            {
                for (int i = 0; i < sites.Count; i++)
                {
                    //将AQI的值数据封装
                    AQIMapPageModel.ValueForSite value = new AQIMapPageModel.ValueForSite();
                    value.stationId = sites[i].StationId;
                    value.AQIValue = sites[i].info.AQI;
                    sites[i].values = value; //将站点数据封装
                    ReqSiteFactors(value); //请求该站点并封装该站点的因子Id

                    idsitedict.Add(sites[i].StationId, sites[i]);
                }
            }
        }

        public void MapLoaded(object sender, EventArgs x)
        {
            map.ShowScaleBar = true;
            AddLabels("AQI");
        }

        private bool firsttime = true;
        private void AddLabels(string valuename)
        {
            map.Pins.Clear();
            map.Labels.Clear();

            double x = 0;
            double y = 0;
            int cnt = 0;
            foreach (var s in sites)
            {
                double value = 0.0;
                string unit = " μg/m³";

                switch (valuename)
                {
                    case "AQI": value = s.values.AQIValue; unit = ""; break;
                    case "PM25": value = s.values.PM25Value; break;
                    case "PM10": value = s.values.PM10Value; break;
                    case "O3": value = s.values.O3Value; break;
                    case "CO": value = s.values.COValue; break;
                    default: break;
                }

                Pin pin = new Pin
                {
                    Coordinate = new Coordinate(s.StationLat, s.StationLng),
                    Animate = false,
                    Draggable = false,
                    Enabled3D = false,
                    Image = XImage.FromStream(typeof(MapPage2).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.label_back_blue.png")),
                    AnchorY = 0.5f,
                    AnchorX = 0.5f
                };
                map.Pins.Add(pin);

                Xamarin.Forms.BaiduMaps.Label label = new Xamarin.Forms.BaiduMaps.Label
                {
                    Title = value.ToString("0.0") + unit,
                    Coordinate = new Coordinate(s.StationLat, s.StationLng),
                    BackgroundColor = Color.FromRgb(55, 113, 184),
                    FontColor = Color.White,
                    FontSize = 22
                };

                map.Labels.Add(label);
                if (firsttime)
                {
                    x += s.StationLng;
                    y += s.StationLat;
                    cnt++;
                }
            }
            if (firsttime)
            {
                x /= cnt;
                y /= cnt;
                map.Center = new Coordinate(y, x);
                firsttime = false;
            }
        }


        private void ButtonClicked(object sender, EventArgs e)
        {
            Button b = sender as Button;
            string factorname = b.BindingContext as string;

            aqi.BackgroundColor = Color.FromHex("#4169E1");
            pm10.BackgroundColor = Color.FromHex("#4169E1");
            pm25.BackgroundColor = Color.FromHex("#4169E1");
            o3.BackgroundColor = Color.FromHex("#4169E1");
            co.BackgroundColor = Color.FromHex("#4169E1");
            (sender as Button).BackgroundColor = Color.Gray;

            if (factorname != "AQI")
            {
                if (!finishedloading.ContainsKey(factorname)) ReqLastRefFacVals(factorname);
            }
            AddLabels(factorname);
        }


        private void ReqSiteFactors(AQIMapPageModel.ValueForSite value)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                string uri = App.BaseUrl + "/api/location/GetFactors?id=" + value.stationId;
                result = EasyWebRequest.sendGetHttpWebRequest(uri);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                List<AirDetailModels.Factors> factor = JsonConvert.DeserializeObject<List<AirDetailModels.Factors>>(result);
                for (int i = 0; i < factor.Count; i++)
                {
                    AirDetailModels.Factors facValue = factor[i];
                    if (facValue.gasName.Equals("PM10"))
                    {
                        value.PM10Id = facValue.id;
                        continue;
                    }
                    else if (facValue.gasName.Equals("PM2.5"))
                    {
                        value.PM25Id = facValue.id;
                        continue;
                    }
                    else if (facValue.gasName.Equals("O₃"))
                    {
                        value.O3Id = facValue.id;
                        continue;
                    }
                    else if (facValue.gasName.Equals("CO"))
                    {
                        value.COId = facValue.id;
                        continue;
                    }
                }
            };
            wrk.RunWorkerAsync();
        }
        private void ReqLastRefFacVals(string valueName)
        {
            if (sites.Count == 0) return;

            List<AirDetailModels.FacValsDetails> details = null;
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                string uri = App.BaseUrl + "/api/FactorData/GetLastRefFacVals";
                AirDetailModels.FacValsParam parameter = new AirDetailModels.FacValsParam();
                switch (valueName)
                {
                    case "PM10":
                        parameter.facId = sites[0].values.PM10Id;
                        break;
                    case "PM25":
                        parameter.facId = sites[0].values.PM25Id;
                        break;
                    case "O3":
                        parameter.facId = sites[0].values.O3Id;
                        break;
                    case "CO":
                        parameter.facId = sites[0].values.COId;
                        break;
                }
                parameter.fromType = 0;
                //parameter.refIds = new string[] { factor.stationId };
                parameter.refIds = new string[sites.Count];
                for (int i = 0; i < sites.Count; i++) parameter.refIds[i] = sites[i].StationId;
                string param = JsonConvert.SerializeObject(parameter);
                String result = EasyWebRequest.sendPOSTHttpWebWithTokenRequest(uri, param);
                details = JsonConvert.DeserializeObject<List<AirDetailModels.FacValsDetails>>(result);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                foreach (var d in details)
                {
                    AirPageModels.AirInfo site = idsitedict[d.refId];
                    switch (valueName)
                    {
                        case "PM10":
                            site.values.PM10Value = d.val;
                            break;
                        case "PM25":
                            site.values.PM25Value = d.val;
                            break;
                        case "O3":
                            site.values.O3Value = d.val;
                            break;
                        case "CO":
                            site.values.COValue = d.val;
                            break;
                    }
                }
                AddLabels(valueName);
                finishedloading.Add(valueName, valueName);
            };
            wrk.RunWorkerAsync();
        }
    }
}