using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AepApp.Models
{
    /// <summary>
    /// 考核网格员
    /// </summary>
    public class TaskExamineStaffModel
    {
        private Color colorBlue = Color.FromRgb(39, 114, 165);
        private Color colorGreen = Color.FromRgb(39, 165, 73);
        private Color colorYellow = Color.FromRgb(165, 121, 39);

        public string gridName { get; set; }
        public DateTime date { get; set; }
        public int total { get; set; }
        public int finished { get; set; }
        public string ratio//完成率
        {
            get
            {
                double r = (finished / Convert.ToDouble(total));
                return string.Format("{0:P}", r);
            }
        }
        public string lastRatio { get; set; }//上月完成率

        public int dispatchTotal { get; set; }
        public int dispatchFinished { get; set; }
        public string dispatchRatio//完成率
        {
            get
            {
                double r = (finished / Convert.ToDouble(total));
                return string.Format("{0:P}", r);
            }
        }
        public string dispatchLastRatio { get; set; }



        public FormattedString totalDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "固定任务数目 ",
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = total + "",
                    FontSize = 22,
                    ForegroundColor = colorBlue,
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
                    ForegroundColor = colorGreen,
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
                    ForegroundColor = colorYellow,
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

        //上级下发任务情况
        public FormattedString dispatchTotalDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "上级下发任务数目 ",
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = dispatchTotal + "",
                    FontSize = 22,
                    ForegroundColor = colorBlue,
                });

                return fs;
            }
        }


        public FormattedString dispatchFinishDes
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
                    Text = dispatchFinished + "",
                    FontSize = 22,
                    ForegroundColor = colorGreen,
                });
                return fs;
            }
        }

        public FormattedString dispatchworkingDes
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
                    Text = (dispatchTotal - dispatchFinished < 0 ? 0 : (dispatchTotal - dispatchFinished)) + "",
                    FontSize = 22,
                    ForegroundColor = colorYellow,
                });
                return fs;
            }
        }


        public FormattedString dispatchRatioDes
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
                    Text = dispatchRatio,
                    FontSize = 22,
                    ForegroundColor = Color.Black,
                });
                return fs;
            }
        }

        public FormattedString dispatchLastMonthDes
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
                    Text = dispatchLastRatio,
                    FontSize = 18,
                });

                return fs;
            }
        }
    }
}
