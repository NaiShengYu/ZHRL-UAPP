﻿using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Net;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AepApp.Models.VOCDetailModels;

namespace AepApp.View.EnvironmentalQuality
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VOCChartPage : ContentPage
    {
        private Factors factor;
        private string siteId;
        private DateTime startTime;
        private DateTime endTime;
        private string filterType = "D";
        private int _type = 0;
        private bool isFirst = true;

        public VOCChartPage(string siteId, Factors factorInfo, int type)
        {
            InitializeComponent();
            _type = type;
            NavigationPage.SetBackButtonTitle(this, "");
            factor = factorInfo;
            this.siteId = siteId;
            Title = factor.name;
            startTime = DateTime.Now.Date.AddDays(-1); ;
            endTime = DateTime.Now;
            PickerStart.Date = startTime;
            PickerEnd.Date = endTime;
            pickerType.Title = "请选择类型";
            pickerType.SelectedIndex = 0;
        }
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            Charts task = e.SelectedItem as Charts;
            if (task == null) return;
            listView.SelectedItem = null;
        }


        void Handle_ScrollToRequested(object sender, Xamarin.Forms.ScrollToRequestedEventArgs e)
        {
            Console.WriteLine(e.ScrollX);
        }
        private async void GetFactorData()
        {
            if (isFirst)
            {
                isFirst = false;
                return;
            }
            string url = "";
            if (_type == 1) url = App.environmentalQualityModel.url + DetailUrl.GetVOCFactorData;
            else url = App.EP360Module.url + DetailUrl.GetPaiKouAndChangJieFactorData;
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("refId", siteId);
            map.Add("fromType", 0);
            map.Add("vType", 0);
            map.Add("sDate", startTime);
            map.Add("eDate", endTime);
            map.Add("dType", filterType);//Y:年均值,M:月均值,D:日均值,H:小时均值,R:原始值
            map.Add("facId", factor.id);
            map.Add("dataType", 0);//0：单因子 1：因子组
            string param = JsonConvert.SerializeObject(map);

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<Charts> list = Tools.JsonUtils.DeserializeObject<List<Charts>>(res.Results);
                    if (list != null)
                    {
                        DrawOxy(list);
                        listView.ItemsSource = list;
                    }else
                        listView.ItemsSource = null;
                }
                catch (Exception e)
                {
                    listView.ItemsSource = null;
                }
            }
        }

        private void clearFocus()
        {
            pickerType.Unfocus();
            PickerStart.Unfocus();
            PickerEnd.Unfocus();
        }

        private void pickerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            filterType = picker2Type(typeName);
            clearFocus();
            GetFactorData();
        }

        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            startTime = e.NewDate;
            clearFocus();
            GetFactorData();
        }

        private void DatePickerEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            endTime = e.NewDate;
            clearFocus();
            GetFactorData();
        }


        /// <summary>
        /// 画图
        /// </summary>
        /// <param name="datas"></param>
        private void DrawOxy(List<Charts> datas)
        {
            if (datas == null)
            {
                return;
            }
            var plotModel = new PlotModel();
            var lineSeries = new LineSeries();
            lineSeries.Background = OxyColor.FromRgb(200, 200, 200);
            int num = datas.Count;

            string timeFormat = "yyyy-MM-dd";
            DateTimeIntervalType dit = DateTimeIntervalType.Days;
            if ("Y" == filterType)
            {
                timeFormat = "yyyy";
                //dit = DateTimeIntervalType.Years;
            }
            else if ("M" == filterType)
            {
                timeFormat = "yyyy-MM";
                //dit = DateTimeIntervalType.Months;
            }
            else if ("H" == filterType)
            {
                timeFormat = "yy-MM-dd HH";
                //dit = DateTimeIntervalType.Hours;
            }

            DateTimeAxis dtx = new DateTimeAxis()
            {
                StringFormat = timeFormat,
                Position = AxisPosition.Bottom,
                MinorIntervalType = dit,
                IntervalType = dit,
                MajorGridlineStyle = LineStyle.DashDot, //主网格样式
                MinorGridlineStyle = LineStyle.DashDashDot, //次网格样式

            };
            if (datas.Count == 0)
            {
                dtx.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddDays(-1));
                dtx.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
            }
            LinearAxis ylx = new LinearAxis()
            {
                IsPanEnabled = false,
                IsZoomEnabled = true,
                Position = AxisPosition.Left,
                ExtraGridlineStyle = LineStyle.DashDot,
            };
            for (int i = 0; i < num; i++)
            {
                Charts para = datas[i];
                string value = para.date.ToString().Replace("T", " ");
                DateTime dt = Convert.ToDateTime(value);
                double abc = DateTimeAxis.ToDouble(dt);
                lineSeries.Points.Add(new DataPoint(abc, para.val));
            }

            plotModel.Axes.Add(dtx);
            plotModel.Axes.Add(ylx);
            plotModel.Series.Add(lineSeries);
            oxyPlot.Model = plotModel;

        }

      

        private void BtnSearch_Clicked(object sender, EventArgs e)
        {
            GetFactorData();
        }

        private string picker2Type(string picker)
        {
            if (string.IsNullOrWhiteSpace(picker))
            {
                return "R";
            }
            if (picker.Contains("年"))
            {
                return "Y";
            }
            else if (picker.Contains("月"))
            {
                return "M";
            }
            else if (picker.Contains("日"))
            {
                return "D";
            }
            else if (picker.Contains("小时"))
            {
                return "H";
            }
            return "R";

        }
    }
}