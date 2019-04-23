using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AepApp.Models
{
    public class UploadEmergencyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string factorId { get; set; }/// 因子编号
        public string factorName { get; set; } /// 因子名称
        public int datatype { get; set; }//因子类型
        public string factor
        {
            get
            {
                return "已添加关键化学品\"" + factorName + "\"";
            }
            set { }
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
            set { }
        }
        public double? lat { get; set; }   /// 纬度
        public double? lng { get; set; } /// 经度
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
                return Content;
            }
            set { }
        }
        public string Original { get; set; } /// 原始名称
        public string Current { get; set; } /// 当前名称
        public string natureString { get; set; } /// 事故性质字符串(000分别对应 气 水 土)
        ////public DateTime Original { get; set; }/// 原始时间
        ////public DateTime Current { get; set; }/// 当前时间
        public string StorePath { get; set; }/// 存储路径(相对路径)
        public string imagePath { get; set; }/// 图片显示路径(相对路径)
        public string VideoStorePath { get; set; }// 录像的存储位路径
        public string VideoPath { get; set; }// 录像的显示路径
        public string VoiceStorePath { get; set; }// 录音的显示路径
        public string VoicePath { get; set; }// 录音的显示路径
        public string CoverPath { get; set; }// 视频封面路径

        public string VoiceLenth
        {
            get { return "语音" + Length + "秒"; }
            set { }
        }// 录音的时长




        public string width { get; set; }  /// 宽度(单位px)
        public string height { get; set; } /// 高度(单位px)
        public string storeurl { get; set; } /// 方案存储地址
        public string reportid { get; set; } /// 报告编号
        public string reportname { get; set; } /// 报告名称  
        public string Length { get; set; } /// 内容长度(单位秒)
        public string direction { get; set; } /// 风向(单位 度)
        public string speed { get; set; } /// 风速(单位 米/秒)
        public string windDescribe
        {
            get
            {
                return "风速:" + speed + "m/s," + "风向" + direction + "度";
            }
            set { }
        }
        public DateTime creationTime { get; set; }
        public string creatorUserName { get; set; }
        public string category { get; set; }
        public string emergencyid { get; set; }


        private bool _isNew = true;

        public bool isNew
        {
            get { return _isNew; }
            set { _isNew = value; NotifyPropertyChanged("isNew"); }
        }

        private string _uploadStatus = "hasUploaded";

        public string uploadStatus
        {
            get { return _uploadStatus; }
            set { _uploadStatus = value; NotifyPropertyChanged("uploadStatus"); }
        }


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
                if (TargetLat != null && TargetLng != null)
                    return TargetLng.Value.ToString("0.0#####") + " E, " + TargetLat.Value.ToString("0.0#####") + " N";
                //else return "121.658237 E,29.897719 N";
                else return "";
            }
            set { }
        }

        public string centerLocString
        {
            get
            {
                return "事发地点定位" + locString + "。";
            }
            set { }
        }
        public string NatureShowString
        {
            get
            {
                return "事故已被定性为" + NatureName + "。";
            }
            set { }
        }
    
        //// command that launchs the map view and put an marker on the location
        //public ICommand LocateOnMapCommand { get; set; }

        //// command that downloads a document and launch a viewer to read
        //public ICommand DocumentDownloadCommand { get; set; }
      
    }
}
