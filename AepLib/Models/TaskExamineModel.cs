using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AepApp.Models
{
    /// <summary>
    /// 任务考核
    /// </summary>
    public class TaskExamineModel
    {
        public string gridName { get; set; }
        public DateTime date { get; set; }
        public int total { get; set; }
        public int finished { get; set; }
        public string ratio
        {
            get
            {
                double r = (finished / Convert.ToDouble(total));
                return string.Format("{0:P}", r);
            }
        }//完成率
        public string lastRatio { get; set; }//上月完成率

        public FormattedString totalDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = date.Year + "年" + date.Month + "月总任务数量 ",
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = total + "",
                    FontSize = 22,
                    ForegroundColor = Color.FromRgb(39, 114, 165),
                });

                return fs;
            }
        }


        public FormattedString finishDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "已完成 ",
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = finished + "",
                    FontSize = 22,
                    ForegroundColor = Color.FromRgb(39, 165, 73),
                });
                return fs;
            }
        }

        public FormattedString workingDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "未完成 ",
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = (total - finished < 0 ? 0 : (total - finished)) + "",
                    FontSize = 22,
                    ForegroundColor = Color.FromRgb(165, 121, 39),
                });
                return fs;
            }
        }


        public FormattedString ratioDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "完成率  ",
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = ratio,
                    FontSize = 22,
                    ForegroundColor = Color.Black,
                });
                return fs;
            }
        }

        public FormattedString lastMonthDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "上月完成率 ",
                    FontSize = 14,
                });
                fs.Spans.Add(new Span
                {
                    Text = lastRatio,
                    FontSize = 18,
                });

                return fs;
            }
        }
    }
}
