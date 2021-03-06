﻿using AepApp.Models;
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
        private bool _isSample = false;
        public event EventHandler<EventArgs> SampleEquipment;
        private ObservableCollection<EquipmentPageModel.ItemsBean> dataList = new ObservableCollection<EquipmentPageModel.ItemsBean>();
      

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            EquipmentPageModel.ItemsBean item = e.SelectedItem as EquipmentPageModel.ItemsBean;
            if (item == null)
                return;
            if (_isSample ==false)
            {
                Navigation.PushAsync(new EquipmentInfoPage(item.name, item.id));
            }
            else
            {//布点选择设备
                AddPlacement_Equipment equipment = new AddPlacement_Equipment {
                    equipid = item.id,
                    equipname = item.name,
                  };
                SampleEquipment.Invoke(equipment, new EventArgs());
                Navigation.PopAsync();
            }

            listView.SelectedItem = null;
        }
       
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                Index = 0;
                ReqEquipmentList("", Index, 10);
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            Index = 0;
            ReqEquipmentList("", Index, 10);
        }

        string searchKey = "";


        public EquipmentPage()
        {
            InitializeComponent();
            Title = "设备";
            ReqEquipmentList("", Index, 10);         
        }

        public EquipmentPage(bool IsSameple):this()
        {
            _isSample = IsSameple;

        }



        private async void ReqEquipmentList(string keyword, int pagrIndex, int pageSize)
        {
            string url = App.BasicDataModule.url + DetailUrl.EquipmentList;
            ChemicalStruct parameter = new ChemicalStruct
            {
                keyword = searchKey,
                pageIndex = Index + "",
                pagesize = 10 + ""
            };
            string param = JsonConvert.SerializeObject(parameter);
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                Index += 1;
                EquipmentPageModel.EquipmentPageModelBean equipmentPageModel = new EquipmentPageModel.EquipmentPageModelBean();
                equipmentPageModel = Tools.JsonUtils.DeserializeObject<EquipmentPageModel.EquipmentPageModelBean>(hTTPResponse.Results);
                List<EquipmentPageModel.ItemsBean> list = new List<EquipmentPageModel.ItemsBean>();
                if (equipmentPageModel != null)
                {
                    total = equipmentPageModel.count;
                    list = equipmentPageModel.items;
                }
                if (list == null) return;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                if (total != 0) Title = "设备（" + total + "）";
                else Title = "设备";
                listView.ItemsSource = dataList;
            }
        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            EquipmentPageModel.ItemsBean item = e.Item as EquipmentPageModel.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < total)
                {
                    ReqEquipmentList("", Index, 10);
                }
            }
        }
    }
}
