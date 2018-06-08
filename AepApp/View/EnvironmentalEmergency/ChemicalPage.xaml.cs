using System;
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
                MessagingCenter.Send<ContentPage, ReqChemicalPageModel.ItemsBean>(this, "Value", ChemicalModel);
                Navigation.PopAsync();
            }

            listView.SelectedItem = null; 
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            Console.WriteLine("====="+searchKey+"++++");
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
            _type = type;
            NavigationPage.SetBackButtonTitle(this,"");//去掉返回键文字
            ReqChemicalList("", Index, 10);         
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
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.frameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                Index += 1;
                ReqChemicalPageModel.ReqChemicalBean reqChemicalBean = new ReqChemicalPageModel.ReqChemicalBean();
                reqChemicalBean = JsonConvert.DeserializeObject<ReqChemicalPageModel.ReqChemicalBean>(hTTPResponse.Results);
                total = reqChemicalBean.count;
                List<ReqChemicalPageModel.ItemsBean> list = reqChemicalBean.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
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
