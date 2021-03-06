﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
    public class DetailUrl
    {       
        //应急token转换
        public static string ConvertToken = "/api/TokenAuth/ExternalAuthenticate";
        //获取应急事故
        public static string GetEmergencyAccidentList = "/api/services/app/Incident/GetPagedIncidents";
        //添加应急事故
        public static string AddEmergencyAccident = "/api/services/app/Incident/Create";
        //获取应急事故详情
        //public static string GetEmergencyDetail = "/api/services/app/Incident/Get";
        public static string GetEmergencyDetail = "/api/services/app/IncidentLoggingEvent/GetIncidentLoggingEventsByIncidentId";
        //专家库
        public static string ExpertLibraryUrl = "/api/services/app/professional/GetPagedprofessionals";
        //敏感源
        public static string Sensitive = "/Api/Services/App/SensitiveUnit/GetPagedSensitiveUnits";
        //成功案例
        public static string SuccessCase = "/api/services/app/Case/GetPagedCases";
        //应急预案
        public static string EmergencyPlan = "/api/services/app/preplan/getpagedpreplans";
        //救援地点
        public static string RescueSite = "/api/services/app/RescuePoint/GetPagedRescuePoints";
        //救援物资
        public static string RescueMaterials = "/api/services/app/RescuePoint/Get";
        //获取化学品列表
        public static string ChemicalList = "/api/mod/GetChemicalBykey";
        //获取化学品详细信息
        public static string ChemicalInfo = "/api/mod/GetChemical";
        //获取化学品环境标准
        public static string ChemicalStandards = "/api/mod/GetStandards";
        //获取设备列表
        public static string EquipmentList = "/api/mod/GetEquipmentList";
        //获取设备详情
        public static string EquipmentInfo = "/api/mod/GetEquipmentByid";

        //获取关键污染物
        public static string GetIncidentFactors = "/api/services/app/IncidentFactor/GetIncidentFactorsByIncidentId";
        //获取人员列表
        public static string PersonList = "/api/fw/GetUserBykey";


        //环境质量
        //获取地表水站点列表
        public static string GetWaterSite = "/Api/Section/PagedListWithLastGrade";
        //获取VOC站点列表
        public static string GetVOCSite = "/Api/location/PagedList";
        //获取VOC站点因子
        public static string GetVOCSiteFactor = "/Api/location/GetFactorListSortedPost";
        //获取VOC站点因子最新值
        public static string GetVOCSiteFactorLatestValue = "/Api/FactorData/GetLastFacValsPost";
        //获取VOC因子数据
        public static string GetVOCFactorData = "/Api/FactorData/GetFactValPost";
        //获取厂界、排口VOC站点列表
        public static string GetChangJieSite = "/api/pdo/GetLocationList";
        //获取厂界、排口VOC站点因子
        public static string GetPaiKouAndChangJieSiteFactor = "/api/pdo/GetLocationFactors";
        //获取厂界、排口VOC站点因子
        public static string GetPaiKouAndChangJieSiteFactorLatestValue = "/api/pdo/GetLocationFactorLatestValues";
        //获取厂界、排口VOC因子数据
        public static string GetPaiKouAndChangJieFactorData = "/api/pdo/GetData";
        //获取离线站点列表
        public static string GetOfflineSite = "/Api/FactorData/CheckAllStationConn";

    }
}