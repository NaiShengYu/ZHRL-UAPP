using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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

        private string curRefId; //站点ID
        private string curFacId;
        private bool isHours = true; //当前选择查看的数据是否24小时(默认初始化为显示24小时)
        //private List<AirDetailModels.Factors> factors = new List<AirDetailModels.Factors>();
        ObservableCollection<AirDetailModels.Factors> factors = new ObservableCollection<AirDetailModels.Factors>();
        public AirDetailPage (AirPageModels.AirInfo airInfo)
		{
            this.airInfo = airInfo;
            InitializeComponent();
            this.Title = airInfo.StationName;
            level.Text = airInfo.info.AQIInfo.AQILevel;
            health.Text = airInfo.info.AQIInfo.Health;
            curRefId = airInfo.StationId;
            curFacId = airInfo.info.facId;
            CrossHud.Current.Show("");
            //initOxyPlotView();
            Appearing += AirDetailPage_Appearing;
        }

        private void AirDetailPage_Appearing(object sender, EventArgs e)
        {
            //获取站点因子
            ReqSiteFactors();
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
                factor.FacValueDetails = new AirDetailModels.FacValsDetails() { val = airInfo.info.AQI, refId =airInfo.info.facId};
                factors.Add(factor);
                List<AirDetailModels.Factors> factor1 = JsonConvert.DeserializeObject<List<AirDetailModels.Factors>>(result);
                int count = factor1.Count;
                for (int i = 0;i<count;i++) {
                    factors.Add(factor1[i]);
                    if (i >= 0) factor1[i].unit = "μg/m³";
                }


                listView.ItemsSource = factors;
                listView.HeightRequest = 45 * factors.Count;

                //获取因子值      
                int num = factors.Count;
                foreach (var f in factors) {
                    if (!f.gasName.Equals("AQI"))
                    {
                        ReqLastRefFacVals(f);
                    } 
                }               
            };
            wrk.RunWorkerAsync();
        }

        private void ReqLastRefFacVals(AirDetailModels.Factors factor)
        {
            List<AirDetailModels.FacValsDetails> details = null;
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
                details = JsonConvert.DeserializeObject<List<AirDetailModels.FacValsDetails>>(result);                
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                //factor.FacValueDetails = details[0];
                if (details != null)
                {
                    factor.FacValueDetails = details[0];
                }
                ReqDefHour();
            };
            wrk.RunWorkerAsync();
        }

        private void ReqDefHour()
        {
            InitHoursHttp();         
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AirDetailModels.Factors factorInfo = e.SelectedItem as AirDetailModels.Factors;
            //DependencyService.Get<Sample.IToast>().ShortAlert("" + factorInfo.FacValueDetails.val);
            curFacId = factorInfo.FacValueDetails.refId;
            //获取图表数据
            if (isHours)
            {
                InitHoursHttp();               
            }
            else {
                InitDaysHttp();              
            }
        }
      
        private void Hours(object sender, EventArgs e)
        {
            isHours = true;
            hours.BackgroundColor = Color.Gray;
            days.BackgroundColor = Color.White;
            InitHoursHttp();
        }

        private void Days(object sender, EventArgs e)
        {
            isHours = false;
            hours.BackgroundColor = Color.White;
            days.BackgroundColor = Color.Gray;
            InitDaysHttp();
        }
        private void InitHoursHttp()
        {
            string eDay = DateTime.Now.ToString("yyyy-MM-dd");
            string eHours = DateTime.Now.ToString("hh:00:00");
            string eDH = eDay + " " + eHours;
            string lsDay = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string sDH = lsDay + " " + eHours;
            string url = App.BaseUrl + "/api/FactorData/GetFactVal?refId=" + curRefId + "&fromType=0&vType=0&dataType=0&facId="
                + curFacId + "&dType=H" + "&eDate=" + eDH + "&sDate=" + sDH;
            //请求图表数据
            ReqFactVal(url, eDH, sDH);
        }
        private void InitDaysHttp()
        {
            string eDH = DateTime.Now.ToString("yyyy-MM-dd");
            string sDH = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string url = App.BaseUrl + "/api/FactorData/GetFactVal?refId=" + curRefId + "&fromType=0&vType=0&dataType=0&facId="
                + curFacId + "&dType=D" + "&eDate=" + eDH + "&sDate=" + sDH;
            //请求图表数据
            ReqFactVal(url, eDH + " 00:00", sDH + " 00:00");
        }
        private void ReqFactVal(string url ,string eDh, string sDh)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            List<AirDetailModels.AirData> datas = new List<AirDetailModels.AirData>();
            wrk.DoWork += (sender1, e1) =>
            {                            
                String result = EasyWebRequest.sendGetHttpWebRequest(url);
                //Console.WriteLine("有结果了");
                datas = JsonConvert.DeserializeObject<List<AirDetailModels.AirData>>(result);              
                           
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                CrossHud.Current.Dismiss();
                if (isHours) {
                    DrawOxyForHours(datas);
                }
                else {
                    DrawOxyForDay(datas);
                }              
            };
            wrk.RunWorkerAsync();
        }

        private void DrawOxyForHours(List<AirDetailModels.AirData> datas)
        {
            var plotModel1 = new PlotModel();
            var lineSeries = new LineSeries();
            lineSeries.Background = OxyColor.FromRgb(200, 200, 200);
            AirDetailModels.AirData para;
            int num = datas.Count;
            // 设置X轴设置(时间轴)
            DateTimeAxis dtx = new DateTimeAxis()
            {
                StringFormat = "yyyy-MM-dd HH:00:00",
                Position = AxisPosition.Bottom,
                //Title = "Year",               
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days, //间隔类型（天数）
                MajorGridlineStyle = LineStyle.Solid, //主网格样式
                MinorGridlineStyle = LineStyle.None, //次网格样式
               
            };
            //设置y轴(线性轴)
            LinearAxis ylx = new LinearAxis()
            {
                IsPanEnabled = false,
                IsZoomEnabled = false,
                Position = AxisPosition.Left,
                ExtraGridlineStyle = LineStyle.Dash,
            };
            //获取所有点
            for (int i = 0; i < num; i++)
            {
                para = datas[i];
                string value = para.date.Replace("T", " ");
                DateTime dt = Convert.ToDateTime(value);
                double abc = DateTimeAxis.ToDouble(dt);
                lineSeries.Points.Add(new DataPoint(abc, para.val));
            }

            plotModel1.Axes.Add(dtx); //添加时间轴（X轴）         
            plotModel1.Axes.Add(ylx); //添加Y轴
            plotModel1.Series.Add(lineSeries);
            oxyPlot.Model = plotModel1;

        }

        private void DrawOxyForDay(List<AirDetailModels.AirData> datas)
        {
            var plotModel1 = new PlotModel();
            var lineSeries = new LineSeries();
            lineSeries.Background = OxyColor.FromRgb(200, 200, 200);
        
            AirDetailModels.AirData para;
            int num = datas.Count;
            // 设置X轴设置(时间轴)
            DateTimeAxis dtx = new DateTimeAxis() {
                StringFormat = "yyyy-MM-dd",               
                Position = AxisPosition.Bottom,             
                //Title = "Year",               
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days, //间隔类型（天数）
                MajorGridlineStyle = LineStyle.Solid, //主网格样式
                MinorGridlineStyle = LineStyle.None, //次网格样式
            };
            //设置y轴(线性轴)
            LinearAxis ylx = new LinearAxis() {
                IsPanEnabled = false,
                IsZoomEnabled = false,
                Position = AxisPosition.Left,
                ExtraGridlineStyle = LineStyle.Dash,
            };
            //获取所有点
            for (int i = 0; i < num; i++)
            {                
                para = datas[i];
                string value = para.date.Replace("T", " ");
                DateTime dt = Convert.ToDateTime(value);              
                double abc = DateTimeAxis.ToDouble(dt);
                lineSeries.Points.Add(new DataPoint(abc, para.val));
            }

            plotModel1.Axes.Add(dtx); //添加时间轴（X轴）         
            plotModel1.Axes.Add(ylx); //添加Y轴
            plotModel1.Series.Add(lineSeries);
            oxyPlot.Model = plotModel1;
        }
              
    }
   
}