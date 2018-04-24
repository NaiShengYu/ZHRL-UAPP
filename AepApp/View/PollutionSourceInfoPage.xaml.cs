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
using System.Globalization;
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
                _24But.BackgroundColor = Color.FromRgb(34, 145, 224);
                _is30Select = false;
                _30But.BackgroundColor = Color.Silver;
                haveHistoryData();
            }

        }
        //进入360监控界面
        void Handle_360(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MonitorPage(_enterprise));

        }
        //30天数据
        void Handle_30day(object sender, System.EventArgs e)
        {
            if (_is30Select == true)
                return;
            else
            {
                _is30Select = true;
                _30But.BackgroundColor = Color.FromRgb(34, 145, 224);
                _is24Select = false;
                _24But.BackgroundColor = Color.Silver;
                haveHistoryData();
            }
        }

        //选中哪个因子
        void selectFactor(object sender, EventArgs e)
        {
            var but = sender as Button;
            var tup = _uidatadict[but];
            _currentPort=tup.Item1;
            _currentFactor = tup.Item2;
            _currentStack.BackgroundColor = Color.White;
            _currentStack = tup.Item3;
            _currentStack.BackgroundColor = Color.FromRgb(242, 242, 242);
            haveHistoryData();

        }

        EnterpriseModel _enterprise = null;
        //因子24小时、30天数据
        ObservableCollection<FactorForDateData> _chartData = null;
        ObservableCollection<ProjectApprovalInfoDischargePort> group = new ObservableCollection<ProjectApprovalInfoDischargePort>();
        bool _is24Select = true;
        bool _is30Select = false;
        //当前排口
        ProjectApprovalInfoDischargePort _currentPort = null;
        //当前因子
        ProjectApprovalInfoFactor _currentFactor = null;
        //当前的stakeLayout,用于改变颜色
        StackLayout _currentStack = null;

        //用于存放按钮和按钮对应的数据，通过他来修改按钮背景颜色，和获取站点以及站点因子
        Dictionary<Button, Tuple<ProjectApprovalInfoDischargePort, ProjectApprovalInfoFactor, StackLayout>> _uidatadict
      = new Dictionary<Button, Tuple<ProjectApprovalInfoDischargePort, ProjectApprovalInfoFactor, StackLayout>>();

        public PollutionSourceInfoPage(EnterpriseModel preiseModel)
        {
            InitializeComponent();
            _enterprise = preiseModel;
            Title = preiseModel.name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            factorData();
        }

        //获取站点所有排口及排口下的因子
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
                CreatView();
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
                string uri = App.BaseUrl + "/api/AppEnterprise/GetPortFactorList?fid=" + _currentFactor.id + "&type="+_is30Select  + "&id=" + _currentPort.id;
                var result = EasyWebRequest.sendGetHttpWebRequest(uri);

                try
                {
                    _chartData = JsonConvert.DeserializeObject<ObservableCollection<FactorForDateData>>(result);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("错误提示：" + ex);
                }

            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {

                ProcessingData();
                CrossHud.Current.Dismiss();
            };
            wrk.RunWorkerAsync();




        }

      
        //根据因子个数来增加
        void CreatView(){
            ScrollView scr = new ScrollView(){
                BackgroundColor = Color.FromRgb(234, 234, 234),
            };
            GridName.Children.Add(scr);

            var layout = new StackLayout(){
                Spacing = 1,
            };
            for (int i = 0; i < group.Count;i ++){

                ProjectApprovalInfoDischargePort port = group[i];

                Label header = new Label
                {
                    Text = port.nameAndType,
                    Font = Font.SystemFontOfSize(20.0),
                    Margin = new Thickness(15,0,0,0),
                    HeightRequest = 50,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                layout.Children.Add(header);

                for (int j = 0; j < port.items.Count; j++)
                {
                    var G = new Grid();
                    var stack = new StackLayout(){
                        Orientation = StackOrientation.Horizontal,
                        BackgroundColor = Color.White,
                    };
                    G.Children.Add(stack);
                    ProjectApprovalInfoFactor factor = port.items[j];
                   
                    if(i==0 && j ==0){
                        _currentPort = port;
                        _currentFactor = factor;
                        stack.BackgroundColor = Color.FromRgb(242, 242, 242);
                        _currentStack = stack;
                        haveHistoryData();
                    }



                    //参数名称按钮
                    Button infobut = new Button
                    {
                        Text = "   " + factor.name,
                        Font = Font.SystemFontOfSize(17.0),
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        Margin = new Thickness(0),
                        BorderRadius = 0,
                        TextColor = Color.Black,
                    };

                    //参数值lab
                    Label infoLab = new Label
                    {
                        Text = Math.Round(factor.value, 2).ToString() + " " + factor.unit,
                        Font = Font.SystemFontOfSize(17.0),
                        Margin = new Thickness(10, 0, 10, 0),
                        HeightRequest = 50,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.End,
                    };
                    stack.Children.Add(infobut);
                    stack.Children.Add(infoLab);

                    //整条按钮
                    Button backBut = new Button
                    {
                        Margin = new Thickness(0),
                        BorderRadius = 0,
                        BindingContext = factor,
                    };
                    backBut.Clicked += selectFactor;
                    _uidatadict.Add(backBut, new Tuple<ProjectApprovalInfoDischargePort, ProjectApprovalInfoFactor,StackLayout>(port,factor,stack));

                    G.Children.Add(backBut);
                    layout.Children.Add(G);
                }
            }
            scr.Content = layout; 
        }


        //处理数据
        void ProcessingData()
        {
            //防止为空的时候崩溃
            if (_chartData == null)
                return;
            
            var plotModel1 = new PlotModel();
            var lineSeries = new LineSeries();
            lineSeries.Background = OxyColor.FromRgb(200, 200, 200);
            FactorForDateData para;
            int num = _chartData.Count;
            // 设置X轴设置(时间轴)
            DateTimeAxis dtx = new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,//x轴的位置
                Title = "时间",               
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days, //间隔类型（天数）
                MajorGridlineStyle = LineStyle.Solid, //主网格样式
                MinorGridlineStyle = LineStyle.None, //次网格样式

            };
            if (_is30Select == false)
            {
                dtx.StringFormat = "HH";
            }
            else
            {
                dtx.StringFormat = "yyyy-MM-dd";
            }

            //设置y轴(线性轴)
            LinearAxis ylx = new LinearAxis()
            {
                IsPanEnabled = false,//是否可滑动
                IsZoomEnabled = false,//是否可变焦
                Position = AxisPosition.Left,//Y轴的位置
                ExtraGridlineStyle = LineStyle.Dash,//
            };

            //设置Y轴最大最小值
            float min = float.MaxValue;
            float max = float.MinValue;
            //获取所有点
            for (int i = 0; i < num; i++)
            {
                para = _chartData[i];
                string value = para.dt.Replace("T", " ");
                DateTime dt;
                if(_is30Select ==false){
                    dt = DateTime.ParseExact(value, "yyyy-MM-dd HH", CultureInfo.CurrentCulture);
                }else{
                    dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
                }
                double abc = DateTimeAxis.ToDouble(dt);
                    lineSeries.Points.Add(new DataPoint(abc, para.value.Value));

                    min = Math.Min(para.value.Value, min);//设置Y轴最小值
                    max = Math.Max(para.value.Value, max);//设置Y轴最大值

            }
            ylx.Minimum = min - (max - min) * 0.05;
            ylx.Maximum = max + (max - min) * 0.05;

            plotModel1.Axes.Add(dtx); //添加时间轴（X轴）         
            plotModel1.Axes.Add(ylx); //添加Y轴
            plotModel1.Series.Add(lineSeries);
            pView.Model = plotModel1;

        }

    }
}
