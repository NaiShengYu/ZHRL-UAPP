using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Sample;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalStandardPage : ContentPage
    {
        private ObservableCollection<ChemicalStandardModel> dataList = new ObservableCollection<ChemicalStandardModel>();

        public ChemicalStandardPage(string id)
        {
            InitializeComponent();
            ReqChemicalStandard(id);
        }


        private async void ReqChemicalStandard(string id)
        {
            string url = App.BasicDataModule.url + DetailUrl.ChemicalStandards;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", id);
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                dataList = JsonConvert.DeserializeObject<ObservableCollection<ChemicalStandardModel>>(hTTPResponse.Results);
               if(dataList ==null ||dataList.Count ==0) DependencyService.Get<IToast>().ShortAlert("无数据");
               else listView.ItemsSource = dataList;
            }
        }


    }
}
