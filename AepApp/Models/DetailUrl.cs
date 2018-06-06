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
        public static string ConvertToken = "api/TokenAuth/ExternalAuthenticate";
        //获取应急事故
        public static string GetEmergencyAccidentList = "api/services/app/Incident/GetPagedIncidents";
        //专家库
        public static string ExpertLibraryUrl = "api/services/app/professional/GetPagedprofessionals";
        //敏感源
        public static string Sensitive = "Api/Services/App/SensitiveUnit/GetPagedSensitiveUnits";
        //成功案例
        public static string SuccessCase = "api/services/app/Case/GetPagedCases";
        //应急预案
        public static string EmergencyPlan = "api/services/app/preplan/getpagedpreplans";
    }
}