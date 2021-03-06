﻿using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace AepApp.Models
{
    /// <summary>
    /// 采样 - 样本信息
    /// </summary>
    public class SampleInfoModel : BaseModel
    {
        public string id { get; set; }
        private string status;// 0未上传 1已上传 2运输中 3已接收
        public string Status { get { return status; } set { status = value; NotifyPropertyChanged(); } }

        private string appearance;//水样外观
        public string Appearance { get { return appearance; } set { appearance = value; NotifyPropertyChanged(); } }
        private string depth;//水深 
        public string Depth { get { return depth; } set { depth = value; NotifyPropertyChanged(); } }
        private string direction;//流向 
        public string Direction { get { return direction; } set { direction = value; NotifyPropertyChanged(); } }
        private string DO;//溶解氧
        public string DoDyn { get { return DO; } set { DO = value; NotifyPropertyChanged(); } }
        private string number;//样品编号
        public string Number { get { return number; } set { number = value; NotifyPropertyChanged(); } }
        private string PH;
        public string PhDyn { get { return PH; } set { PH = value; NotifyPropertyChanged(); } }
        private DateTime sampletime;
        public DateTime Sampletime { get { return sampletime; } set { sampletime = value; NotifyPropertyChanged(); } }
        private string stationname;
        public string Stationname { get { return stationname; } set { stationname = value; NotifyPropertyChanged(); } }
        private Guid taskid { get; set; }
        private string tide;//潮水  
        public string Tide { get { return tide; } set { tide = value; NotifyPropertyChanged(); } }
        private string waterlevel;//水位  
        public string Waterlevel { get { return waterlevel; } set { waterlevel = value; NotifyPropertyChanged(); } }
        private string width;//断面宽度
        public string Width { get { return width; } set { width = value; NotifyPropertyChanged(); } }
        private string qrcode;
        public string Qrcode { get { return qrcode; } set { qrcode = value; NotifyPropertyChanged(); } }
        private string anatype;//检测项目
        public string Anatype { get { return anatype; } set { anatype = value; NotifyPropertyChanged(); } }
        private string fixative;//固定剂信息
        public string Fixative { get { return fixative; } set { fixative = value; NotifyPropertyChanged(); } }

        private bool filterTrans;
        public bool FilterTrans { get { return filterTrans; } set { filterTrans = value; NotifyPropertyChanged(); } }
        private bool filterAccept;
        public bool FilterAccept { get { return filterAccept; } set { filterAccept = value; NotifyPropertyChanged(); } }
        private int sampleCount;
        public int SampleCount { get { return sampleCount; } set { sampleCount = value; NotifyPropertyChanged(); } }
        public bool HasSample
        {
            get { return sampleCount > 0; }
        }
        private int photoCount;
        public int PhotoCount { get { return photoCount; } set { photoCount = value; NotifyPropertyChanged(); } }
        public bool HasPhoto
        {
            get { return photoCount > 0; }
        }

        private Color _backgroundColor;
        public Color BackgroundColor { get { return _backgroundColor; }set { _backgroundColor = value; NotifyPropertyChanged(); } }

        public void SetColors(bool isSelected)
        {
            if (isSelected)
            {
                BackgroundColor = Color.FromHex("#EDEDED");
            }
            else
            {
                BackgroundColor = Color.White;
            }
        }
    }
}
