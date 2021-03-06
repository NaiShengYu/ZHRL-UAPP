﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System.Text;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalPage : ContentPage
    {
       

        private int Index;
        private int total;
        private ObservableCollection<ReqChemicalPageModel.ItemsBean> dataList = new ObservableCollection<ReqChemicalPageModel.ItemsBean>();
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            ReqChemicalPageModel.ItemsBean ChemicalModel = e.SelectedItem as ReqChemicalPageModel.ItemsBean;
            if (ChemicalModel == null)
                return;
            if (_type == 1)
                Navigation.PushAsync(new ChemicalInfoPage(ChemicalModel.id, ChemicalModel.chinesename));

            if (_type == 2)
            {
                AddDataIncidentFactorModel.ItemsBean factor = new AddDataIncidentFactorModel.ItemsBean
                {
                    factorId = ChemicalModel.id,
                    factorName = ChemicalModel.chinesename,
                };
                App.contaminantsList.Add(factor);
                MessagingCenter.Send<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", factor);
                Navigation.PopAsync();
            }
            if (_type ==3){

                AddDataIncidentFactorModel.ItemsBean model = new AddDataIncidentFactorModel.ItemsBean
                {
                    factorId = ChemicalModel.id,
                    factorName = ChemicalModel.chinesename,

                };
                Navigation.PushAsync(new LHXZInfoPage(model,0));


                //MessagingCenter.Send<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddDataIncidentFactor", model);
                //Navigation.PopAsync();

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
                ReqChemicalList("", Index, 10);
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            Index = 0;
            ReqChemicalList("", Index, 10);         
        }

        string searchKey = "";
        int _type = 0;
        public ChemicalPage(int type)
        {
            InitializeComponent();
            Title = "化学品";
            _type = type;
            NavigationPage.SetBackButtonTitle(this,"");//去掉返回键文字
            ReqChemicalList("", Index, 10);
            listView.ItemsSource = dataList;
        }

        public ChemicalPage():this(1){
            
        }

        private async void ReqChemicalList(string keyword, int pageIndex, int pageSize)
        {
            string url = App.BasicDataModule.url + DetailUrl.ChemicalList;
            Console.WriteLine(url);
           
            ChemicalStruct parameter = new ChemicalStruct
            {
                keyword = searchKey,
                pageIndex = Index +"",
                pagesize = 10 + ""
            };
            string param = JsonConvert.SerializeObject(parameter);
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                Index += 1;
                ReqChemicalPageModel.ReqChemicalBean reqChemicalBean = new ReqChemicalPageModel.ReqChemicalBean();
                reqChemicalBean = Tools.JsonUtils.DeserializeObject<ReqChemicalPageModel.ReqChemicalBean>(hTTPResponse.Results);
                List<ReqChemicalPageModel.ItemsBean> list = new List<ReqChemicalPageModel.ItemsBean>();
                if (reqChemicalBean != null)
                {
                    total = reqChemicalBean.count;
                    list = reqChemicalBean.items;
                }
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        dataList.Add(list[i]);
                    }
                }
                if (total != 0) Title = "化学品（" + total + "）";
                else Title = "化学品";

            }
        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            ReqChemicalPageModel.ItemsBean item = e.Item as ReqChemicalPageModel.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < total)
                {
                    ReqChemicalList("", Index, 10);
                }
            }
           
        }
    }

    internal class ChemicalStruct
    {
        public string keyword { get; set; }
        public string pageIndex { get; set; }
        public string pagesize { get; set; }
    }
}
