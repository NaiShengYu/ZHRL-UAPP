using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class OxyDataPageModle 
	{
        public PlotModel AreaModel { get; set; }

        public OxyDataPageModle()
        {
            AreaModel = CreateAreaChart();         
        }
        public PlotModel CreateAreaChart()
        {
            //var plotModel1 = new PlotModel { Title = "Area Series with crossing lines" };
            var plotModel1 = new PlotModel();
            var areaSeries1 = new AreaSeries();
            areaSeries1.Points.Add(new DataPoint(0, 50));
            areaSeries1.Points.Add(new DataPoint(10, 140));
            areaSeries1.Points.Add(new DataPoint(20, 60));
            areaSeries1.Points2.Add(new DataPoint(0, 60));
            areaSeries1.Points2.Add(new DataPoint(5, 80));
            areaSeries1.Points2.Add(new DataPoint(20, 70));
            plotModel1.Series.Add(areaSeries1);
            return plotModel1;
        }
    }
}