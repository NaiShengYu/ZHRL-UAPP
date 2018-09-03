using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AepApp.Models
{
    public class EmergencyAccidentInfoDetail {

        public class EmergencyAccidentBean
        {
            public ResultBean result { get; set; }
        }
        public class ResultBean
        {

            //
            public List<IncidentLoggingEventsBean> items { get; set; }

            //public String name { get; set; }
            //public String startDate { get; set; }
            //public bool natureDetermined { get; set; }
            //public String id { get; set; }
            //public List<IncidentLoggingEventsBean> incidentLoggingEvents { get; set; }
        }
        public class IncidentLoggingEventsBean : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void NotifyPropertyChanged(string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public string factorId { get; set; }/// 因子编号
            public string factorName { get; set; } /// 因子名称
            public int datatype { get; set; }//因子类型
            public string factor
            {
                get
                {
                    return "已添加关键化学品\"" + factorName + "\"";
                }
            }
            public string testMethodId { get; set; }/// 测试方法编号
            public string testMethodName { get; set; } /// 测试方法名称
            public string unitId { get; set; }/// 计量单位编号
            public string unitName { get; set; } /// 计量单位名称
            public string equipmentId { get; set; }/// 测量设备编号
            public string equipmentName { get; set; }/// 测量设备名称
            public string factorValue { get; set; }  /// 因子的值
            public string incidentNature { get; set; } /// 对应的事故性质
            public string measurement
            {
                get
                {
                    return factorName + testMethodName + "的化验结果为" + factorValue + unitName;
                }
            }
            public string lat { get; set; }   /// 纬度
            public string lng { get; set; } /// 经度
            public string index { get; set; } /// 排序索引
            public double? TargetLat { get; set; }   /// 目标点位纬度
            public double? TargetLng { get; set; }  /// 目标点位经度
            public string TargetAddress { get; set; } /// 目标点位地址
            public string Content { get; set; } /// 内容
            public string Title { get; set; }   /// 标题
            public string messageSending
            {
                get
                {
                    return Title + ": " + Content;
                }
            }

            private bool _isNew = false;

            public bool isNew
            {
                get { return _isNew; }
                set { _isNew = value; NotifyPropertyChanged("isNew"); }
            }

            private string _uploadStatus = "notUploaded";

            public string uploadStatus
            {
                get { return _uploadStatus; }
                set { _uploadStatus = value; NotifyPropertyChanged("uploadStatus"); }
            }
            public string Original { get; set; } /// 原始名称
            public string Current { get; set; } /// 当前名称
            public string natureString { get; set; } /// 事故性质字符串(000分别对应 气 水 土)
            //public DateTime Original { get; set; }/// 原始时间
            //public DateTime Current { get; set; }/// 当前时间
            public string StorePath { get; set; }/// 存储路径(相对路径)
            public string imagePath {
                get { return App.EmergencyModule.url + StorePath; }
            }
            public string VideoPath {
                get { return App.EmergencyModule.url + StorePath; }
            }// 录像的显示路径
            public string VoicePath {get
                {
                    return App.EmergencyModule.url + StorePath;
                }
            }// 录音的显示路径
            public string VoiceLenth { 
                get { return "语音"+Length+"秒"; }
            }// 录音的时长

            public string Width { get; set; }  /// 宽度(单位px)
            public string Height { get; set; } /// 高度(单位px)
            public string StoreUrl { get; set; } /// 方案存储地址
            public Guid? ReportId { get; set; } /// 报告编号
            public string ReportName { get; set; } /// 报告名称  
            public string Length { get; set; } /// 内容长度(单位秒)
            public decimal? Direction { get; set; } /// 风向(单位 度)
            public decimal? Speed { get; set; } /// 风速(单位 米/秒)
            public string windDescribe
            {
                get
                {
                    return "风速:" + Speed + "m/s," + "风向" + Direction + "度";
                }
            }
            public DateTime creationTime { get; set; }
            public string creatorUserName { get; set; }
            public string category { get; set; }
            public string id { get; set; }
            public Thickness marge { get; set; }
            public string NatureName
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(natureString)) return null;
                    if (natureString.Length != 3) return null;
                    string ret = "";
                    if (natureString[0] == '1') ret += "大气";
                    if (natureString[1] == '1')
                    {
                        if (natureString[0] == '1') ret += "及";
                        ret += "水";
                    }
                    if (natureString[2] == '1')
                    {
                        if (natureString[0] == '1' || natureString[1] == '1') ret += "及";
                        ret += "土壤";
                    }
                    return ret;
                }
            }

            public string locString
            {
                get
                {
                    return TargetLng.Value.ToString("0.0#####") + " E, " + TargetLat.Value.ToString("0.0#####") + " N";
                }
            }

            public string centerLocString
            {
                get
                {
                    return "事发地点定位" + locString + "。";
                }
            }
            public string NatureString
            {
                get
                {
                    return "事故已被定性为" + NatureName + "。";
                }
            }

            // command that launchs the map view and put an marker on the location
            public ICommand LocateOnMapCommand { get; set; }

            public ICommand PlayVoiceCommand { get; set; }

            // command that downloads a document and launch a viewer to read
            public ICommand DocumentDownloadCommand { get; set; }

        }
    }
}