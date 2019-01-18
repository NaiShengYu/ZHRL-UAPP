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
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AepApp.Models.VOCDetailModels;

namespace AepApp.View.EnvironmentalQuality
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

        private string _siteId; //站点ID
        private bool isHours = true; //当前选择查看的数据是否24小时(默认初始化为显示24小时)
        //private List<AirDetailModels.Factors> factors = new List<AirDetailModels.Factors>();
        ObservableCollection<Factors> factors = new ObservableCollection<Factors>();
        public AirDetailPage (AirPageModels.AirInfo airInfo)
		{
            this.airInfo = airInfo;
            InitializeComponent();
            this.Title = airInfo.StationName;
            level.Text = airInfo.info.AQIInfo.AQILevel;
            health.Text ="当前空气AQI为"+airInfo.info.AQI+"环境质量为"+airInfo.info.AQIInfo.AQIClass+"。" + airInfo.info.AQIInfo.Health+"。建议："+airInfo.info.AQIInfo.Suggestion;
            _siteId = airInfo.StationId;
            //CrossHud.Current.Show("");
            //initOxyPlotView();
            //获取站点因子
            ReqSiteFactors();
        }


        private async void ReqSiteFactors()
        {
            string url =  App.environmentalQualityModel.url + DetailUrl.GetVOCSiteFactor;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _siteId);
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<Factors> list = JsonConvert.DeserializeObject<List<Factors>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        factors.Clear();
                        foreach (var item in list)
                        {
                            factors.Add(item);
                        }
                    }

                    Factors f = new Factors
                    {
                        name = "AQI",
                        val = airInfo.info.AQI.ToString(),
                    };
                    factors.Insert(0, f);
                }
                catch (Exception e)
                {
                }
            }
            listView.ItemsSource = factors;
            ReqLastRefFacVals();
        }

        private async void ReqLastRefFacVals()
        {
            string url = App.environmentalQualityModel.url + DetailUrl.GetVOCSiteFactorLatestValue;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("refId", _siteId);
            map.Add("fromType", "0");
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<FactorLatestValue> list = JsonConvert.DeserializeObject<List<FactorLatestValue>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            foreach (var factor in factors)
                            {
                                if (item.id == factor.id)
                                    factor.val = item.val;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }



        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listView.SelectedItem == null) return;
            Factors factorInfo = e.SelectedItem as Factors;
            if (factorInfo == null) return;
            if (factorInfo.name == "AQI") return;
            Navigation.PushAsync(new VOCChartPage(_siteId, factorInfo, 1));
            listView.SelectedItem = null;
        }

        //private void ReqDefHour()
        //{
        //    InitHoursHttp();         
        //}

        //private void Hours(object sender, EventArgs e)
        //{
        //    isHours = true;
        //    hours.BackgroundColor = Color.Gray;
        //    days.BackgroundColor = Color.White;
        //    InitHoursHttp();
        //}

        //private void Days(object sender, EventArgs e)
        //{
        //    isHours = false;
        //    hours.BackgroundColor = Color.White;
        //    days.BackgroundColor = Color.Gray;
        //    InitDaysHttp();
        //}
        //private void InitHoursHttp()
        //{
        //    string eDay = DateTime.Now.ToString("yyyy-MM-dd");
        //    string eHours = DateTime.Now.ToString("hh:00:00");
        //    string eDH = eDay + " " + eHours;
        //    string lsDay = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        //    string sDH = lsDay + " " + eHours;
        //    string url = App.environmentalQualityModel.url + "/api/FactorData/GetFactVal?refId=" + curRefId + "&fromType=0&vType=0&dataType=0&facId="
        //        + curFacId + "&dType=H" + "&eDate=" + eDH + "&sDate=" + sDH;
        //    //请求图表数据
        //    ReqFactVal(url, eDH, sDH);
        //}
        //private void InitDaysHttp()
        //{
        //    string eDH = DateTime.Now.ToString("yyyy-MM-dd");
        //    string sDH = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
        //    string url = App.environmentalQualityModel.url + "/api/FactorData/GetFactVal?refId=" + curRefId + "&fromType=0&vType=0&dataType=0&facId="
        //        + curFacId + "&dType=D" + "&eDate=" + eDH + "&sDate=" + sDH;
        //    //请求图表数据
        //    ReqFactVal(url, eDH + " 00:00", sDH + " 00:00");
        //}
        //private void ReqFactVal(string url ,string eDh, string sDh)
        //{
        //    BackgroundWorker wrk = new BackgroundWorker();
        //    List<AirDetailModels.AirData> datas = new List<AirDetailModels.AirData>();
        //    wrk.DoWork += (sender1, e1) =>
        //    {                            
        //        String result = EasyWebRequest.sendGetHttpWebRequest(url);
        //        //Console.WriteLine("有结果了");
        //        datas = JsonConvert.DeserializeObject<List<AirDetailModels.AirData>>(result);              

        //    };
        //    wrk.RunWorkerCompleted += (sender1, e1) =>
        //    {
        //        CrossHud.Current.Dismiss();
        //        if (isHours) {
        //            DrawOxyForHours(datas);
        //        }
        //        else {
        //            DrawOxyForDay(datas);
        //        }              
        //    };
        //    wrk.RunWorkerAsync();
        //}

        //private void DrawOxyForHours(List<AirDetailModels.AirData> datas)
        //{
        //    var plotModel1 = new PlotModel();
        //    var lineSeries = new LineSeries();
        //    lineSeries.Background = OxyColor.FromRgb(200, 200, 200);
        //    AirDetailModels.AirData para;
        //    int num = datas.Count;
        //    // 设置X轴设置(时间轴)
        //    DateTimeAxis dtx = new DateTimeAxis()
        //    {
        //        StringFormat = "yyyy-MM-dd HH",
        //        Position = AxisPosition.Bottom,
        //        //Title = "Year",               
        //        MinorIntervalType = DateTimeIntervalType.Days,
        //        IntervalType = DateTimeIntervalType.Days, //间隔类型（天数）
        //        MajorGridlineStyle = LineStyle.DashDot, //主网格样式
        //        MinorGridlineStyle = LineStyle.None, //次网格样式

        //    };
        //    if (datas.Count == 0)
        //    {
        //        dtx.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddDays(-24));
        //        dtx.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
        //    }
        //    //设置y轴(线性轴)
        //    LinearAxis ylx = new LinearAxis()
        //    {
        //        IsPanEnabled = false,
        //        IsZoomEnabled = false,
        //        Position = AxisPosition.Left,
        //        ExtraGridlineStyle = LineStyle.Dash,
        //    };
        //    //获取所有点
        //    for (int i = 0; i < num; i++)
        //    {
        //        para = datas[i];
        //        string value = para.date.Replace("T", " ");
        //        DateTime dt = Convert.ToDateTime(value);
        //        double abc = DateTimeAxis.ToDouble(dt);
        //        lineSeries.Points.Add(new DataPoint(abc, para.val));
        //    }

        //    plotModel1.Axes.Add(dtx); //添加时间轴（X轴）         
        //    plotModel1.Axes.Add(ylx); //添加Y轴
        //    plotModel1.Series.Add(lineSeries);
        //    oxyPlot.Model = plotModel1;

        //}

        //private void DrawOxyForDay(List<AirDetailModels.AirData> datas)
        //{
        //    var plotModel1 = new PlotModel();
        //    var lineSeries = new LineSeries();
        //    lineSeries.Background = OxyColor.FromRgb(200, 200, 200);

        //    AirDetailModels.AirData para;
        //    int num = datas.Count;
        //    // 设置X轴设置(时间轴)
        //    DateTimeAxis dtx = new DateTimeAxis() {
        //        StringFormat = "yyyy-MM-dd",
        //        Position = AxisPosition.Bottom,
        //        //Title = "Year",               
        //        MinorIntervalType = DateTimeIntervalType.Days,
        //        IntervalType = DateTimeIntervalType.Days, //间隔类型（天数）
        //        MajorGridlineStyle = LineStyle.DashDot, //主网格样式
        //        MinorGridlineStyle = LineStyle.None, //次网格样式

        //    };
        //    if(datas.Count == 0)
        //    {
        //        dtx.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddDays(-30));
        //        dtx.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
        //    }
        //    //设置y轴(线性轴)
        //    LinearAxis ylx = new LinearAxis() {
        //        IsPanEnabled = false,
        //        IsZoomEnabled = false,
        //        Position = AxisPosition.Left,
        //        ExtraGridlineStyle = LineStyle.Dash,
        //    };
        //    //获取所有点
        //    for (int i = 0; i < num; i++)
        //    {                
        //        para = datas[i];
        //        string value = para.date.Replace("T", " ");
        //        DateTime dt = Convert.ToDateTime(value);              
        //        double abc = DateTimeAxis.ToDouble(dt);
        //        lineSeries.Points.Add(new DataPoint(abc, para.val));
        //    }

        //    plotModel1.Axes.Add(dtx); //添加时间轴（X轴）         
        //    plotModel1.Axes.Add(ylx); //添加Y轴
        //    plotModel1.Series.Add(lineSeries);
        //    oxyPlot.Model = plotModel1;
        //}




        private class FactorLatestValue
        {
            public string id { get; set; }//因子ID
            public string val { get; set; }//因子ID
        }

    }
   
}