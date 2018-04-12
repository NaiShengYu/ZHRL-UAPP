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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    //代码binding
    //Binding b = new Binding("AQIInfo.AQILevel");
    //b.Source = airInfo;
    //level.SetBinding(Label.TextProperty, b);
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AirDetailPage : ContentPage
	{
        private AirPageModels.AirInfo airInfo;
        private string result;
        //private List<AirDetailModels.Factors> factors = new List<AirDetailModels.Factors>();
        ObservableCollection<AirDetailModels.Factors> factors = new ObservableCollection<AirDetailModels.Factors>();
        public AirDetailPage (AirPageModels.AirInfo airInfo)
		{
            this.airInfo = airInfo;
            InitializeComponent();
            this.Title = airInfo.StationName;
            level.Text = airInfo.info.AQIInfo.AQILevel;
            health.Text = airInfo.info.AQIInfo.Health;
            //CrossHud.Current.Show("");
            //获取站点因子
            ReqSiteFactors();
            //https://192.168.1.251/api/location/GetFactors?id=6191a228-d4f8-4770-8d9e-200f9409e693



        }

        private void ReqSiteFactors()
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                string uri = App.BaseUrl + "/api/location/GetFactors?id="+ airInfo.StationId;
                result = EasyWebRequest.sendGetHttpWebRequest(uri);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                AirDetailModels.Factors factor = new AirDetailModels.Factors();
                factor.name = "AQI";
                factors.Add(factor);
                List<AirDetailModels.Factors> factor1 = JsonConvert.DeserializeObject<List<AirDetailModels.Factors>>(result);
                int count = factor1.Count;
                for (int i = 0;i<count;i++) {
                    factors.Add(factor1[i]);
                }           
                listView.ItemsSource = factors;
                //获取因子值      
                int num = factors.Count;
                for (int i = 1; i < num; i++) {
                    ReqLastRefFacVals(factors[i]);
                }                         
            };
            wrk.RunWorkerAsync();
        }

        private void ReqLastRefFacVals(AirDetailModels.Factors factor)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                string uri = App.BaseUrl + "/api/FactorData/GetLastRefFacVals";
                AirDetailModels.FacValsParam parameter = new AirDetailModels.FacValsParam();
                parameter.facId =factor.id ;
                parameter.fromType = 0;
                parameter.refIds = new string[] { airInfo.StationId };
                string param = JsonConvert.SerializeObject(parameter);               
                String result = EasyWebRequest.sendPOSTHttpWebWithTokenRequest(uri, param);
                Console.WriteLine(result);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {              
                //AirDetailModels.Factors factor = new AirDetailModels.Factors();
                //factor.name = "AQI";
                //factors.Add(factor);
                //List<AirDetailModels.Factors> factor1 = JsonConvert.DeserializeObject<List<AirDetailModels.Factors>>(result);
                //factors.AddRange(factor1);          
                //listView.ItemsSource = factors;
                //SortAQI();
                //listView.ItemsSource = dataList;
                //CrossHud.Current.Dismiss();
            };
            wrk.RunWorkerAsync();
        }
    }
}