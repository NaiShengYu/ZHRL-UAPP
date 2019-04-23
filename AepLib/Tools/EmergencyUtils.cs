using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AepApp.Models;
using AepApp.Services;
using AepApp.View;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using SimpleAudioForms;
using Xamarin.Forms;

namespace AepApp.Tools
{
    public class EmergencyUtils
    {
        //地图标题
        public static string LocateOnMap(UploadEmergencyShowModel showModel)
        {
            string title = "";
            switch (showModel.category)
            {
                case "IncidentWindDataSendingEvent":
                    title = "风速风向发出位置";
                    break;
                case "IncidentFactorMeasurementEvent":
                    title = "文字信息发出位置";
                    break;
                case "IncidentLocationSendingEvent":
                    title = "事故中心点";
                    break;
                case "IncidentMessageSendingEvent":
                    title = "文字信息发出位置";
                    break;
                default:
                    break;
            }

            return title;
        }

        //创建一个存储模型
        public static UploadEmergencyModel creatingUploadEmergencyModel(string type,string emergencyId)
        {
            UploadEmergencyModel emergencyModel = new UploadEmergencyModel
            {
                uploadStatus = "notUploaded",
                Title = "",
                creationTime = System.DateTime.Now,
                emergencyid = emergencyId,
                category = type,
                creatorUserName = App.userInfo.userid,
            };
            try
            {
                emergencyModel.lat = App.currentLocation.Latitude;
                emergencyModel.lng = App.currentLocation.Longitude;
            }
            catch (Exception)
            {
                emergencyModel.lat = 0;
                emergencyModel.lng = 0;
            }
            return emergencyModel;
        }
        public static UploadEmergencyShowModel creatingUploadEmergencyShowModel(UploadEmergencyModel emergencyModel)
        {
            string data = JsonConvert.SerializeObject(emergencyModel);
            UploadEmergencyShowModel showModel = Tools.JsonUtils.DeserializeObject<UploadEmergencyShowModel>(data);
            if (showModel != null)
            {
                showModel.isEdit = true;
            }
            return showModel;
        }
        //请求参数字典
        public static Dictionary<string,object> RequestDictionary(UploadEmergencyShowModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("lat", Convert.ToDouble(model.lat));
            dic.Add("lng", Convert.ToDouble(model.lng));
            dic.Add("index", 0);
            dic.Add("loggingTime", model.creationTime.ToString());
            dic.Add("incidentId", model.emergencyid);

            switch (model.category)
            {
                case "IncidentLocationSendingEvent":
                    {
                        dic.Add("targetLat", model.TargetLat);
                        dic.Add("targetLng", model.TargetLng);
                        dic.Add("targetAddress", "");
                        dic.Remove("lat");
                        dic.Remove("lng");
                        dic.Add("lat", 0);
                        dic.Add("lng", 0);
                    }
                    break;
                //化学因子名称
                case "IncidentFactorIdentificationEvent":
                    {
                        dic.Add("factorId", model.factorId);
                        dic.Add("factorName", model.factorName);
                    }
                    break;
                case "IncidentMessageSendingEvent":
                    {
                        dic.Add("content", model.Content);
                        dic.Add("title", model.Title);
                        dic.Add("creatorUserName", model.creatorUserName);

                    }
                    break;
                case "IncidentWindDataSendingEvent":
                    {
                        dic.Add("direction", Convert.ToDecimal(model.direction));
                        dic.Add("speed", Convert.ToDecimal(model.speed));
                    }
                    break;
                //事故性质
                case "IncidentNatureIdentificationEvent":
                    {
                        dic.Add("natureString", model.natureString);
                    }
                    break;
                //添加化学因子检测值
                case "IncidentFactorMeasurementEvent":
                    {
                        dic.Add("factorId", model.factorId);
                        dic.Add("factorName", model.factorName);
                        dic.Add("testMethodId", "");
                        dic.Add("testMethodName", "");
                        dic.Add("unitId", model.unitId);
                        dic.Add("unitName", model.unitName);
                        dic.Add("equipmentId", "");
                        dic.Add("equipmentName", "");
                        dic.Add("factorValue", model.factorValue);
                        dic.Add("incidentNature", 4);
                    }
                    break;
                //图片
                case "IncidentPictureSendingEvent":
                    break;
                //视频
                case "IncidentVideoSendingEvent":
                    break;
                case "IncidentVoiceSendingEvent":
                    {
                        dic.Add("length", Convert.ToInt32(model.Length));//录音的时长
                    }
                    break;
            }
            return dic;
        }

        public static void GetLocalData(UploadEmergencyShowModel ShowModel) {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            string cagy = ShowModel.category;
            if (cagy == "IncidentLocationSendingEvent")
            {
                LocateOnMap(ShowModel);
            }
            else if (cagy == "IncidentPictureSendingEvent")
            {

                if (Device.RuntimePlatform == Device.Android)
                {
                    path = DependencyService.Get<IFileService>().GetExtrnalStoragePath(Constants.STORAGE_TYPE_PICTURES);
                }
                ShowModel.StorePath = path + "/" + ShowModel.StorePath;
                ShowModel.imagePath = path + "/" + ShowModel.imagePath;
            }
            else if (cagy == "IncidentVoiceSendingEvent")
            {

                if (Device.RuntimePlatform == Device.Android)
                {
                    path = DependencyService.Get<IFileService>().GetExtrnalStoragePath(null);
                }
                ShowModel.VoicePath = path + "/" + ShowModel.VoicePath;
                ShowModel.VoiceStorePath = path + "/" + ShowModel.VoiceStorePath;
                try
                {
                    ShowModel.PlayVoiceCommand = new Command(() =>
                    {
                        var dir = path + "/" + ShowModel.VoicePath;
                        DependencyService.Get<IAudio>().PlayLocalFile(dir);
                    });
                }
                catch (Exception ex)
                {

                }
            }
            else if (cagy == "IncidentVideoSendingEvent")
            {

                if (Device.RuntimePlatform == Device.Android)
                {
                    path = DependencyService.Get<IFileService>().GetExtrnalStoragePath(Constants.STORAGE_TYPE_MOVIES);
                }
                ShowModel.VideoPath = path + "/" + ShowModel.VideoPath;
                ShowModel.VideoStorePath = path + "/" + ShowModel.VideoStorePath;
                ShowModel.CoverPath = path + "/" + ShowModel.CoverPath;
            }

            else if (cagy == "IncidentFactorMeasurementEvent")
            {
                LocateOnMap(ShowModel);
            }
            else if (cagy == "IncidentMessageSendingEvent")
            {
                try
                {
                    LocateOnMap(ShowModel);
                }
                catch (Exception ex)
                {

                }
            }
        }
        //删除老的图片，视频
        public static void DeleteLocalImageAndVideo() {
            if (Device.RuntimePlatform == Device.iOS)
            {
                string mainPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                if (Directory.Exists(Path.Combine(mainPath, "ThuImage")))
                    Directory.Delete(Path.Combine(mainPath, "ThuImage"), true);
                if (Directory.Exists(Path.Combine(mainPath, "RVideo")))
                    Directory.Delete(Path.Combine(mainPath, "RVideo"), true);
                if (Directory.Exists(Path.Combine(mainPath, "Sample")))
                    Directory.Delete(Path.Combine(mainPath, "Sample"), true);
                if (Directory.Exists(Path.Combine(mainPath, "Video")))
                    Directory.Delete(Path.Combine(mainPath, "Video"), true);
            }
        }
         //获取"数据"页面的"关键污染物"和"关键理化性质"
       public static async void ReqaddData()
        {
            List<AddDataIncidentFactorModel.ItemsBean> WuRanWus = new List<AddDataIncidentFactorModel.ItemsBean>();
            List<AddDataIncidentFactorModel.ItemsBean> LHXZs = new List<AddDataIncidentFactorModel.ItemsBean>();
            string url = App.EmergencyModule.url + DetailUrl.GetIncidentFactors +
                        "?Id=" + App.EmergencyAccidentID;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode != HttpStatusCode.ExpectationFailed)
            {
                Console.WriteLine(hTTPResponse.Results);
                AddDataIncidentFactorModel.Factor fa = Tools.JsonUtils.DeserializeObject<AddDataIncidentFactorModel.Factor>(hTTPResponse.Results);
                if(fa == null || fa.result == null || fa.result.items == null)
                {
                    return;
                }
                for (int i = 0; i < fa.result.items.Count; i++)
                {
                    AddDataIncidentFactorModel.ItemsBean model = fa.result.items[i];
                    if (model.dataType == "0") WuRanWus.Add(model);
                    if (model.dataType == "1" || model.dataType == "2" || model.dataType == "3") LHXZs.Add(model);

                }
                App.contaminantsList = WuRanWus;
                App.AppLHXZList = LHXZs;
            }
        }

    }
}
