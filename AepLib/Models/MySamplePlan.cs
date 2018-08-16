using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AepApp.Models
{

    public class MySamplePlanResult
    {

        public ObservableCollection<MySamplePlanItems> Items { get; set; }

    }


    public class MySamplePlanItems : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Key]
        public int PlanId { get; set; }
        //采样计划ID guid
        public string id { get; set; }
        //采样计划名称 string
        public string name { get; set; }
        //预计执行时间 datetime
        public DateTime plantime { get; set; }
        //计划创建时间 datetime 
        public DateTime createtime { get; set; }
        //地点经度 double
        public double lng { get; set; }
        //地点纬度 double 
        public double lat { get; set; }
        //地址 string
        public string address { get; set; }
        //预处理信息 string
        public string pretreatment { get; set; }
        //质控说明 string
        public string qctip { get; set; }
        //安全说明 string
        public string security { get; set; }
        //备注信息 string
        public string remarks { get; set; }
        public string ischeck { get; set; }
        public string gisid { get; set; }
        //状态（0未完成1已完成)
        public int status { get; set; }
        //public int status { get { return 1; } set{} }

        public ObservableCollection<taskslist> tasklist { get; set; }

        //未完成的数量
        public int unCompleteNum
        {
            get
            {
                int i = 0;
                if (tasklist == null)
                {
                    return i;
                }
                foreach (taskslist task in tasklist)
                {
                    if (task.taskstatus == 1) i += 1;
                }
                return i;
            }
            set { }
        }

        //计划的状态 0 表示未开始采样 1表示采样进行中 2表示采样完成，等待审批 3表示采样完成
        public int planStatus
        {
            get
            {
                if (status == 1)
                    return 3;
                else
                {
                    if (tasklist == null || tasklist.Count == 0) return 0;//如果任务个数为0 标记为未开始采样
                    else
                    {
                        if (unCompleteNum == 0) return 0;
                        else if (unCompleteNum == tasklist.Count) return 2;
                        else return 1;
                    }
                }
            }

            set { }
        }



        public string completeRatio
        {
            get
            {
                return unCompleteNum + "/" + (tasklist == null ? 0 : tasklist.Count);
            }
            set { }

        }

        public string completeRatioInfo
        {
            get
            {
                return "采样进行中，" + completeRatio;
            }
            set { }

        }



    }

    public class taskslist : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Key]
        public int Id { get; set; }
        //子任务ID guid,（Add模式不传）
        public string taskid { get; set; }
        //子任务名称 string,
        public string taskname { get; set; }
        //子任务排序值 int ,
        public int taskindex { get; set; }
        //子任务采样类型 int （0地表水1废水2饮用水3废气4环境空气5室内空气6农业用地7工业用地）,
        public int tasktype { get; set; }
        //子任务状态 int （0未完成1已完成）,
        public int taskstatus { get; set; }
        public string tasklabel { get; set; }
        //样本数量（数据上传模块未做暂时留参数值为0）
        public string samplecount { get; set; }
        public ObservableCollection<tasksAnas> taskAnas { get; set; }


        public string tasktypeName
        {
            get
            {
                if (tasktype == 0) return "地表水";
                else if (tasktype == 1) return "废水";
                else if (tasktype == 2) return "饮用水";
                else if (tasktype == 3) return "废气";
                else if (tasktype == 4) return "环境空气";
                else if (tasktype == 5) return "室内空气";
                else if (tasktype == 6) return "农业用地";
                else return "工业用地";
            }
            set { }
        }

    }

    public class tasksAnas : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Key]
        public int Id { get; set; }
        //检测项目ID guid
        public string atid { get; set; }
        //检测项目名称 string 
        public string atname { get; set; }
        //检测项目分类 int （0:检测分类 1: 化学品 2：其他因子）
        public string attype { get; set; }
    }

}
