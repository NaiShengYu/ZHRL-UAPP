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

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
namespace AepApp.View
{
    public partial class PollutionSourceInfoPage : ContentPage
    {
        void Handle_24Hour(object sender, System.EventArgs e)
        {
            if (_is24Select == true)
                return;
            else
            {
                _is24Select = true;
                _is30Select = false;
                haveHistoryData();
            }

        }
        //进入360监控界面
        void Handle_360(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MonitorPage(_enterprise));

        }

        void Handle_30day(object sender, System.EventArgs e)
        {
            if (_is30Select == true)
                return;
            else
            {
                _is30Select = true;
                _is24Select = false;
                haveHistoryData();
            }
        }

        EnterpriseModel _enterprise = null;
        ObservableCollection<ProjectApprovalInfoDischargePort> group = new ObservableCollection<ProjectApprovalInfoDischargePort>();
        bool _is24Select = true;
        bool _is30Select = false;
        ProjectApprovalInfoDischargePort _currentPort = null;//当前排口
        ProjectApprovalInfoFactor _currentFactor = null;//当前因子
        public PollutionSourceInfoPage(EnterpriseModel preiseModel)
        {
            InitializeComponent();
            _enterprise = preiseModel;
            Title = preiseModel.name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            factorData();
        }



        private void factorData()
        {

            BackgroundWorker wrk = new BackgroundWorker();

            wrk.DoWork += (sender1, e1) =>
            {
                CrossHud.Current.Show("请求中...");
                string uri = App.BaseUrl + "/api/AppEnterprise/GetPortOnlinelast?id=" + _enterprise.id;
                var result = EasyWebRequest.sendGetHttpWebRequest(uri);
                try
                {
                    group = JsonConvert.DeserializeObject<ObservableCollection<ProjectApprovalInfoDischargePort>>(result);
                }
                catch
                {

                }

            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {

                listV.ItemsSource = group;

                CrossHud.Current.Dismiss();

            };
            wrk.RunWorkerAsync();
        }

        //获取24小时或者30天数据
        void haveHistoryData()
        {

            BackgroundWorker wrk = new BackgroundWorker();

            wrk.DoWork += (sender1, e1) =>
            {
                CrossHud.Current.Show("请求中...");
                string uri = App.BaseUrl + "api/AppEnterprise/GetPortFactorList?fid=" + _currentFactor + "&type=" + _is30Select + "&id=" + _currentPort;
                var result = EasyWebRequest.sendGetHttpWebRequest(uri);
                try
                {
                    //factor = JsonConvert.DeserializeObject<ObservableCollection<ProjectApprovalInfoFactor>>(result);

                }
                catch
                {

                }

            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {

                //ProcessingData();
                CrossHud.Current.Dismiss();
            };
            wrk.RunWorkerAsync();




        }





        ////处理数据
        //void ProcessingData()
        //{
        //    var plotModel1 = new PlotModel();
        //    var lineSeries = new LineSeries();
        //    lineSeries.Background = OxyColor.FromRgb(200, 200, 200);
        //    AirDetailModels.AirData para;
        //    int num = datas.Count;
        //    // 设置X轴设置(时间轴)
        //    DateTimeAxis dtx = new DateTimeAxis()
        //    {
        //        StringFormat = "yyyy-MM-dd HH:00:00",
        //        Position = AxisPosition.Bottom,
        //        //Title = "Year",               
        //        MinorIntervalType = DateTimeIntervalType.Days,
        //        IntervalType = DateTimeIntervalType.Days, //间隔类型（天数）
        //        MajorGridlineStyle = LineStyle.Solid, //主网格样式
        //        MinorGridlineStyle = LineStyle.None, //次网格样式

        //    };
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
        //    pView.Model = plotModel1;

        //}

    }
}
