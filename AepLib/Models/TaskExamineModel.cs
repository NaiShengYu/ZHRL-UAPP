using AepApp.Tools;
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
        public Guid grid { get; set; }
        public int gridLevel { get; set; }
        public string gridName { get; set; }
        public DateTime date { get { return DateTime.Now; } }
        public int taskCount { get; set; }// 当月任务数目
        public int completedCount { get; set; }// 当月完成任务数目
        public string completedPerM1 { get; set; }// 上月任务完成百分比
        public string completedPerM2 { get; set; }// 当月月任务完成百分比(children列表里的)

        public List<TaskExamineModel> children { get; set; }

        public string gridLevelDes { get { return ConstConvertUtils.GridLevel2String(gridLevel); } }

        public string ratio
        {
            get
            {
                double r = 0;
                if (taskCount != 0)
                {
                    r = (completedCount / Convert.ToDouble(taskCount));
                }
                return string.Format("{0:P}", r);
            }
        }//完成率

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
                    Text = taskCount + "",
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
                    Text = completedCount + "",
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
                    Text = (taskCount - completedCount < 0 ? 0 : (taskCount - completedCount)) + "",
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
                    Text = completedPerM1,
                    FontSize = 18,
                });

                return fs;
            }
        }


        public FormattedString childrenRatioDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "完成率  ",
                    FontSize = 12,
                    ForegroundColor = Color.Black,
                });
                fs.Spans.Add(new Span
                {
                    Text = string.IsNullOrWhiteSpace(completedPerM2) ? "0.00%" : completedPerM2,
                    FontSize = 14,
                    ForegroundColor = Color.Black,
                });
                return fs;
            }
        }

        public FormattedString childrenLastMonthDes
        {
            get
            {
                FormattedString fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = "上月完成率 ",
                    FontSize = 12,
                    ForegroundColor = Color.Gray,
                });
                fs.Spans.Add(new Span
                {
                    Text = string.IsNullOrWhiteSpace(completedPerM1) ? "0.00%" : completedPerM1,
                    FontSize = 14,
                    ForegroundColor = Color.Gray,
                });

                return fs;
            }
        }
    }
}
