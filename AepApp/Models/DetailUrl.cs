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
	}
}