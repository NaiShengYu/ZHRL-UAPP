using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalInfoPage : ContentPage
    {
        public ChemicalInfoPage(string id,string name)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            this.Title = name;
            ReqChemicalDetail(id);
        }

        private async void ReqChemicalDetail(string id)
        {
            string url = App.BasicDataModule.url + DetailUrl.ChemicalInfo;
            ChemicalInfoStruct parameter = new ChemicalInfoStruct
            {
                chemid = id
            };
            string param = JsonConvert.SerializeObject(parameter);
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.frameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                ChemicalInfoPageModel.ChemicalInfoPageModelBean chemicalInfo = new ChemicalInfoPageModel.ChemicalInfoPageModelBean();
                chemicalInfo = JsonConvert.DeserializeObject<ChemicalInfoPageModel.ChemicalInfoPageModelBean>(hTTPResponse.Results);
                BindingContext = chemicalInfo;
            }
        }

        internal class ChemicalInfoStruct
        {
            public string chemid { get; set; }
        }
    }
}
