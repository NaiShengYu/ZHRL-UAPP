using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
//using Xamarin.Forms.BaiduMaps;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AQIMapPage : ContentPage
    {
        private List<AirPageModels.AirInfo> sites = new List<AirPageModels.AirInfo>();
        private string result;
        List<AirDetailModels.Factors> factors = new List<AirDetailModels.Factors>();
        Dictionary<string, AirPageModels.AirInfo> idsitedict = new Dictionary<string, AirPageModels.AirInfo>();

        Dictionary<string, string> finishedloading = new Dictionary<string, string>();

        List<AzmLabelView> labels = new List<AzmLabelView>();

        public AQIMapPage(List<AirPageModels.AirInfo> sites)
        {
            InitializeComponent();
            this.sites = sites;

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
            AddLabels("AQI");
        }

        private bool firsttime = true;
        private void AddLabels(string valuename)
        {
            foreach (var lv in labels)
            {
                map.Overlays.Remove(lv);
            }
            labels.Clear();

            double x = 0;
            double y = 0;
            int cnt = 0;
            foreach (var s in sites)
            {
                double value = 0.0;
                double finalValue = 0.0;
                string unit = " μg/m³";

                switch (valuename)
                {
                    case "AQI": value = s.values.AQIValue; unit = ""; finalValue = value; break;
                    case "PM25": value = s.values.PM25Value; finalValue = CalculatePM25(s.values.AQIValue, value); break;
                    case "PM10": value = s.values.PM10Value; finalValue = CalculatePM10(s.values.AQIValue, value); break;
                    case "O3": value = s.values.O3Value; finalValue = CalculateO3(s.values.AQIValue, value); break;
                    case "CO": value = s.values.COValue; finalValue = CalculateCO(s.values.AQIValue, value); break;
                    default: break;
                }

                AzmLabelView lv = new AzmLabelView(s.StationName + "\n" + value.ToString("0.0") + unit, new AzmCoord(s.StationLng, s.StationLat))
                {
                    BackgroundColor = Color.FromHex("#4169E1")
                };
                if (finalValue <= 50)
                {
                    lv.BackgroundColor = Color.FromHex("#37b83b");
                }
                else if (finalValue <= 100)
                {
                    lv.BackgroundColor = Color.FromHex("#d8d646");
                }
                else if (finalValue <= 150)
                {
                    lv.BackgroundColor = Color.FromHex("#d4831b");
                }
                else if (finalValue <= 200)
                {
                    lv.BackgroundColor = Color.FromHex("#b8373a");
                }
                else if (finalValue <= 300)
                {
                    lv.BackgroundColor = Color.FromHex("#b8377f");
                }
                else
                {
                    lv.BackgroundColor = Color.FromHex("#79191d");
                }
                map.Overlays.Add(lv);
                labels.Add(lv);

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
                map.SetCenter(11, new AzmCoord(x, y));
                firsttime = false;
            }
        }
        private double CalculatePM25(double aqiValue, double value)
        {
            return new CalculateData().CalculatePM25(aqiValue, value);
        }
        private double CalculatePM10(double aqiValue, double value)
        {
            return new CalculateData().CalculatePM10(aqiValue, value);
        }
        private double CalculateO3(double aqiValue, double value)
        {
            return new CalculateData().CalculateO3(aqiValue, value);
        }
        private double CalculateCO(double aqiValue, double value)
        {
            return new CalculateData().CalculateCO(aqiValue, value);
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