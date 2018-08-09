using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class RescueMaterialsPage : ContentPage
    {
        private int start = 0;
        private ObservableCollection<RescueMaterialsModel.ReliefSuppliesBean> dataList = new ObservableCollection<RescueMaterialsModel.ReliefSuppliesBean>();

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            RescueMaterialsModel.ReliefSuppliesBean item = e.SelectedItem as RescueMaterialsModel.ReliefSuppliesBean;
            if (item == null)
                return;

            listView.SelectedItem = null;
        }

        public RescueMaterialsPage(RescueSiteModel.ItemsBean itemsBean)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = "救援物资" + "\r\n" + itemsBean.name;
            ReqRescueMaterials(itemsBean.id); //网络请求获取救援物资      
        }

        private async void ReqRescueMaterials(string id)
        {
            string url = App.EmergencyModule.url + DetailUrl.RescueMaterials + "?Id=" + id;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                start += 10;
                RescueMaterialsModel.RescueMaterialsModelBean rescueMaterials = new RescueMaterialsModel.RescueMaterialsModelBean();
                rescueMaterials = JsonConvert.DeserializeObject<RescueMaterialsModel.RescueMaterialsModelBean>(hTTPResponse.Results);            
                List<RescueMaterialsModel.ReliefSuppliesBean> list = rescueMaterials.result.reliefSupplies;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]); 
                }
                listView.ItemsSource = dataList;
            }
        }
    }
}
