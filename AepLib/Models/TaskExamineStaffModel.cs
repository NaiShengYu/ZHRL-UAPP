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

        public int gridLevel { get; set; }
        public string gridName { get; set; }
        public DateTime date { get { return DateTime.Now; } }
        public int regularTaskCount { get; set; }// 当月固定任务数目
        public int regularcompletedCount { get; set; }// 当月固定任务完成数目
        public string regularcompletedPerM1 { get; set; }// 上月固定任务完成完成百分比

        public string regularRatio//当月完成率
        {
            get
            {
                double r = 0;
                if (regularTaskCount != 0)
                {
                    r = (regularcompletedCount / Convert.ToDouble(regularTaskCount));
                }
                return string.Format("{0:P}", r);
            }
        }

        public int assignedTaskCount { get; set; }//上级下发任务总数
        public int assignedCompletedCount { get; set; }// 上级下发任务完成数目
        public string assignedCompletedPerM1 { get; set; }// 上月上级下发任务完成百分比
        public string dispatchRatio//完成率
        {
            get
            {
                double r = 0;
                if (regularTaskCount != 0)
                {
                    r = (regularcompletedCount / Convert.ToDouble(regularTaskCount));
                }
                return string.Format("{0:P}", r);
            }
        }



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
                    Text = regularTaskCount + "",
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
                    Text = regularcompletedCount + "",
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
                    Text = (regularTaskCount - regularcompletedCount < 0 ? 0 : (regularTaskCount - regularcompletedCount)) + "",
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
                    Text = regularRatio,
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
                    Text = regularcompletedPerM1,
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
                    Text = assignedTaskCount + "",
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
                    Text = assignedCompletedCount + "",
                    FontSize = 22,
                    ForegroundColor = colorGreen,
                });
                return fs;
            }
        }

        public FormattedString dispatchWorkingDes
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
                    Text = (assignedTaskCount - assignedCompletedCount < 0 ? 0 : (assignedTaskCount - assignedCompletedCount)) + "",
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
                    Text = assignedCompletedPerM1,
                    FontSize = 18,
                });

                return fs;
            }
        }
    }
}
