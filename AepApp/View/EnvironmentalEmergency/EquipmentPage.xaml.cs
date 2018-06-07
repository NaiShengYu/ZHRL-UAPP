using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EquipmentPage : ContentPage
    {    
        private int Index;
        private int total;
        private int sum;
        private ObservableCollection<EquipmentPageModel.ItemsBean> dataList = new ObservableCollection<EquipmentPageModel.ItemsBean>();
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            EquipmentPageModel.ItemsBean item = e.SelectedItem as EquipmentPageModel.ItemsBean;
            if (item == null)
                return;
            Navigation.PushAsync(new EquipmentInfoPage(item.name,item.id));

            listView.SelectedItem = null;

        }

       

        public EquipmentPage()
        {
            InitializeComponent();
            ReqEquipmentList("", Index, 10);         
        }

        private async void ReqEquipmentList(string keyword, int pagrIndex, int pageSize)
        {
            string url = App.BasicDataModule + DetailUrl.EquipmentList;
            ChemicalStruct parameter = new ChemicalStruct
            {
                keyword = "",
                pageIndex = Index + "",
                pagesize = 10 + ""
            };
            string param = JsonConvert.SerializeObject(parameter);
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.frameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                Index += 1;
                sum += 10;
                EquipmentPageModel.EquipmentPageModelBean equipmentPageModel = new EquipmentPageModel.EquipmentPageModelBean();
                equipmentPageModel = JsonConvert.DeserializeObject<EquipmentPageModel.EquipmentPageModelBean>(hTTPResponse.Results);
                total = equipmentPageModel.count;
                List<EquipmentPageModel.ItemsBean> list = equipmentPageModel.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
            }
        }

        internal class item
        {
            public string name { get; set; }
            public string message { set; get; }

        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            EquipmentPageModel.ItemsBean item = e.Item as EquipmentPageModel.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (sum < total)
                {
                    ReqEquipmentList("", Index, 10);
                }
            }
        }
    }
}
