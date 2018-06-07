using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EquipmentInfoPage : ContentPage
    {
        public EquipmentInfoPage(string name,string id)
        {
            InitializeComponent();
            Title = name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            ReqEquipmentInfo(id);
        }

        private async void ReqEquipmentInfo(string id)
        {
            string url = App.BasicDataModule + DetailUrl.EquipmentInfo;
            EquipmenInfoStruct parameter = new EquipmenInfoStruct
            {
                equipmentid = id
            };
            string param = JsonConvert.SerializeObject(parameter);
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.frameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                EquipmentInfoPageModel.EquipmentInfoBean equipmentInfo = new EquipmentInfoPageModel.EquipmentInfoBean();
                equipmentInfo = JsonConvert.DeserializeObject<EquipmentInfoPageModel.EquipmentInfoBean>(hTTPResponse.Results);
                BindingContext = equipmentInfo;
            }
        }
    }

    internal class EquipmenInfoStruct
    {
        public string equipmentid { get; set; }
    }
}
