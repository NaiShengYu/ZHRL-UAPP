using System;
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
        //获取应急事故详情
        public static string GetEmergencyDetail = "/api/services/app/Incident/Get";
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
        //获取设备列表
        public static string EquipmentList = "/api/mod/GetEquipmentList";
        //获取设备详情
        public static string EquipmentInfo = "/api/mod/GetEquipmentByid";

        //获取关键污染物
        public static string GetIncidentFactors = "/api/services/app/IncidentFactor/GetIncidentFactorsByIncidentId";
    }
}