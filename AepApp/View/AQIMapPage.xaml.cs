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
using Xamarin.Forms.BaiduMaps;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AQIMapPage : ContentPage
	{
        
        private List<AirPageModels.AirInfo> sendPage = new List<AirPageModels.AirInfo>();
        private List<AQIMapPageModel.ValueForSite> valueForSites = new List<AQIMapPageModel.ValueForSite>(); //封装了地图上站点id和站点因子数据
        private string result;
        List<AirDetailModels.Factors> factors = new List<AirDetailModels.Factors>();
        public AQIMapPage (List<AirPageModels.AirInfo> sendPages)
		{
			InitializeComponent ();
            InitializeComponent();
            this.Title = "AQI分布情况";
            this.sendPage = sendPages;
            IMapManager mapManager = DependencyService.Get<IMapManager>();
            Console.WriteLine(mapManager.CoordinateType);
            mapManager.CoordinateType = CoordType.GCJ02;
            Console.WriteLine(mapManager.CoordinateType);

            map.Loaded += MapLoaded; //地图相关操作   
            
            //数据相关操作
            if (sendPage.Count != 0) {
                for (int i = 0; i < sendPage.Count; i++) { 
                    //将AQI的值数据封装
                    AQIMapPageModel.ValueForSite value = new AQIMapPageModel.ValueForSite();
                    value.stationId = sendPage[i].StationId;
                    value.AQIValue = sendPage[i].info.AQI;
                    //value.StationLat = sendPage[i].StationLat;
                    //value.StationLng = sendPage[i].StationLng;
                    valueForSites.Add(value); //将站点数据封装
                    ReqSiteFactors(valueForSites[i]); //请求该站点并封装该站点的因子Id
                }
            }
        }

        public void MapLoaded(object sender, EventArgs x)
        {
            map.ShowScaleBar = true;
            //InitLocationService();
            //InitEvents();
            if (sendPage.Count != 0) {
                AddPin(sendPage);
            }
            
        }

        private void AddPin(List<AirPageModels.AirInfo> pinInfo)
        {
            //XImage im =  XImage.FromResource("AepApp.Droid.Resources.dot.png");
            //XImage im1 =  XImage.FromBundle("AepApp.Droid.Resources.dot.png");
            //XImage im2=  XImage.FromFile("AepApp.Droid.Resources.dot.png");        
            //XImage im6 = XImage.FromStream(
            //typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.dot.png")
            //);
            int count = pinInfo.Count;
            for (int i = 0; i < count; i++) {
                Pin annotation = new Pin
                {
                    Title = pinInfo[i].info.AQI + "",
                    Coordinate = new Coordinate(pinInfo[i].StationLat, pinInfo[i].StationLng),
                    Animate = false,
                    Draggable = false,
                    Enabled3D = false,
                    Tag = pinInfo[i].StationId,
                    //Image = XImage.FromStream(
                    //typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.voc.png")
                    //)
                    Image = XImage.FromStream(
                    typeof(MapPage).GetTypeInfo().Assembly.GetManifestResourceStream("AepApp.Droid.pin.png")
                    )
                };
                map.Pins.Add(annotation);
                
            }
            map.Center = new Coordinate(pinInfo[0].StationLat, pinInfo[0].StationLng);
        }

        private void AQI(object sender, EventArgs e)
        {
            ChangeButtonBG(1);          
        }
        private void PM10(object sender, EventArgs e)
        {
            ChangeButtonBG(2);
        }

        private void PM2_5(object sender, EventArgs e)
        {
            ChangeButtonBG(3);
        }

        private void O3(object sender, EventArgs e)
        {
            ChangeButtonBG(4);
        }

        private void CO(object sender, EventArgs e)
        {
            ChangeButtonBG(5);
        }
        private void ChangeButtonBG(int num)
        {
            int count = valueForSites.Count; 
            aqi.BackgroundColor = Color.FromHex("#4169E1");
            pm10.BackgroundColor = Color.FromHex("#4169E1");
            pm25.BackgroundColor = Color.FromHex("#4169E1");
            o3.BackgroundColor = Color.FromHex("#4169E1");
            co.BackgroundColor = Color.FromHex("#4169E1");
            switch (num) {
                case 1:
                    aqi.BackgroundColor = Color.Gray;
                    AddPinTitleValueForAQI(count);                   
                    break;
                case 2:
                    pm10.BackgroundColor = Color.Gray;
                    AddPinTitleValueForPM10(count);                                                             
                    break;
                case 3:
                    pm25.BackgroundColor = Color.Gray;
                    AddPinTitleValueForPM25(count);
                    break;
                case 4:
                    o3.BackgroundColor = Color.Gray;
                    AddPinTitleValueForO3(count);
                    break;
                case 5:
                    co.BackgroundColor = Color.Gray;
                    AddPinTitleValueForCO(count);
                    break;
            }
        }

        private void AddPinTitleValueForCO(int count)
        {
            if (valueForSites[0].COValue == 0.00) //证明没有请求过数据
            {
                for (int i = 0; i < count; i++)
                {
                    ReqLastRefFacVals(valueForSites[i], 5);
                }
            }
            else
            {
                //循环赋值每个对应站点的值                      
                int mapCount2 = map.Pins.Count;
                for (int i = 0; i < count; i++)
                {
                    //获取站点中的数值和站点
                    string stationId = valueForSites[i].stationId;
                    double co = valueForSites[i].COValue;
                    //循环比较对应站点
                    for (int j = 0; j < mapCount2; j++)
                    {
                        if (stationId.Equals(map.Pins[j].Tag))
                        {
                            map.Pins[j].Title = co + "";
                        }
                    }
                }
            }
        }

        private void AddPinTitleValueForO3(int count)
        {
            if (valueForSites[0].O3Value == 0.00) //证明没有请求过数据
            {
                for (int i = 0; i < count; i++)
                {
                    ReqLastRefFacVals(valueForSites[i], 4);
                }
            }
            else
            {
                //循环赋值每个对应站点的值                      
                int mapCount2 = map.Pins.Count;
                for (int i = 0; i < count; i++)
                {
                    //获取站点中的数值和站点
                    string stationId = valueForSites[i].stationId;
                    double o3 = valueForSites[i].O3Value;
                    //循环比较对应站点
                    for (int j = 0; j < mapCount2; j++)
                    {
                        if (stationId.Equals(map.Pins[j].Tag))
                        {
                            map.Pins[j].Title = o3 + "";
                        }
                    }
                }
            }
        }

        private void AddPinTitleValueForAQI(int count)
        {
            //循环赋值每个对应站点的值                      
            int mapCount2 = map.Pins.Count;
            for (int i = 0; i < count; i++)
            {
                //获取站点中的数值和站点
                string stationId = valueForSites[i].stationId;
                double aqi = valueForSites[i].AQIValue;
                //循环比较对应站点
                for (int j = 0; j < mapCount2; j++)
                {
                    if (stationId.Equals(map.Pins[j].Tag))
                    {
                        map.Pins[j].Title = aqi + "";
                    }
                }
            }
        }

        private void AddPinTitleValueForPM25(int count)
        {
            if (valueForSites[0].PM25Value == 0.00) //证明没有请求过数据
            {
                for (int i = 0; i < count; i++)
                {
                    ReqLastRefFacVals(valueForSites[i], 3);
                }
            }
            else
            {
                //循环赋值每个对应站点的值                      
                int mapCount2 = map.Pins.Count;
                for (int i = 0; i < count; i++)
                {
                    //获取站点中的数值和站点
                    string stationId = valueForSites[i].stationId;
                    double p25 = valueForSites[i].PM25Value;
                    //循环比较对应站点
                    for (int j = 0; j < mapCount2; j++)
                    {
                        if (stationId.Equals(map.Pins[j].Tag))
                        {
                            map.Pins[j].Title = p25 + "";
                        }
                    }
                }
            }
        }
        private void AddPinTitleValueForPM10(int count)
        {
            if (valueForSites[0].PM10Value == 0.00) //证明没有请求过数据
            {
                for (int i = 0; i < count; i++)
                {
                    ReqLastRefFacVals(valueForSites[i], 2);
                }
            }
            else
            {
                //循环赋值每个对应站点的值                      
                int mapCount2 = map.Pins.Count;
                for (int i = 0; i < count; i++)
                {
                    //获取站点中的数值和站点
                    string stationId = valueForSites[i].stationId;
                    double p10 = valueForSites[i].PM10Value;
                    //循环比较对应站点
                    for (int j = 0; j < mapCount2; j++)
                    {
                        if (stationId.Equals(map.Pins[j].Tag))
                        {
                            map.Pins[j].Title = p10 + "";
                        }
                    }
                }
            }
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
                for (int i = 0;i < factor.Count; i++) {
                    AirDetailModels.Factors facValue = factor[i];
                    if (facValue.gasName.Equals("PM10")) {
                        value.PM10Id = facValue.id;
                        continue;
                    } else if (facValue.gasName.Equals("PM2.5")) {
                        value.PM25Id = facValue.id;
                        continue;
                    }else if (facValue.gasName.Equals("O₃")) {
                        value.O3Id = facValue.id;
                        continue;
                    }else if (facValue.gasName.Equals("CO")) {
                        value.COId = facValue.id;
                        continue;
                    }
                }
            };
            wrk.RunWorkerAsync();
        }
        private void ReqLastRefFacVals(AQIMapPageModel.ValueForSite factor,int valueName)
        {
            List<AirDetailModels.FacValsDetails> details = null;
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                string uri = App.BaseUrl + "/api/FactorData/GetLastRefFacVals";
                AirDetailModels.FacValsParam parameter = new AirDetailModels.FacValsParam();
                switch (valueName) {
                    case 2:
                        parameter.facId = factor.PM10Id;
                        break;
                    case 3:
                        parameter.facId = factor.PM25Id;
                        break;
                    case 4:
                        parameter.facId = factor.O3Id;
                        break;
                    case 5:
                        parameter.facId = factor.COId;
                        break;
                }               
                parameter.fromType = 0;
                parameter.refIds = new string[] { factor.stationId };
                string param = JsonConvert.SerializeObject(parameter);
                String result = EasyWebRequest.sendPOSTHttpWebWithTokenRequest(uri, param);
                details = JsonConvert.DeserializeObject<List<AirDetailModels.FacValsDetails>>(result);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                switch (valueName) {
                    case 2:
                        factor.PM10Value = details[0].val;
                        break;
                    case 3:
                        factor.PM25Value = details[0].val;
                        break;
                    case 4:
                        factor.O3Value = details[0].val;
                        break;
                    case 5:
                        factor.COValue = details[0].val;
                        break;
                }
                //利用TAG来绑定站点id，根据站点id封装站点数据
                int count = map.Pins.Count;
                for (int i = 0; i <count; i++) {
                    if (map.Pins[i].Tag.Equals(factor.stationId)) {
                        map.Pins[i].Title = details[0].val + "";                       
                    }
                }
            };
            wrk.RunWorkerAsync();
        }
    }
}
