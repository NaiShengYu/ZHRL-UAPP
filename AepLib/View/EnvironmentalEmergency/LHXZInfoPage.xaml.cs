using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class LHXZInfoPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as LHXZAddressMode;
            if (item == null)
                return;

            foreach(LHXZAddressMode aaa in dataList1){
                if(aaa.isCurrent ==true){
                    aaa.isCurrent = false;
                    break;
                }
            }

            item.isCurrent = true;
            lastItem = item;
            listView.SelectedItem = null;
        
        }

        //进入选择样本类型页面
        void SampleType(object sender, System.EventArgs e)
        {
            if (App.sampleTypeList.Count ==0)return;
            if (_factorModel.dataType == "1") return;
            if (_factorModel.dataType == "2") return;
            if (_factorModel.dataType == "3") return;

            Button but = sender as Button;
            Navigation.PushAsync(new SampleTypePage());
            MessagingCenter.Subscribe<ContentPage, string>(this, "SampleType", (arg1, arg2) =>
            {
                MessagingCenter.Unsubscribe<ContentPage, string>(this, "SampleType");
                but.Text = arg2;
            });
        }
        //进入单位选择界面
        void getUnit(object sender, System.EventArgs e){

            Button but = sender as Button;
            Navigation.PushAsync(new DetectionValueUnitPage());
            MessagingCenter.Subscribe<ContentPage, string>(this, "DetectionValueUnit", (arg1, arg2) =>
            {
                MessagingCenter.Unsubscribe<ContentPage, string>(this, "DetectionValueUnit");
                but.Text = arg2;
            });


        }

        //保存数据
        void Handle_Clicked(object sender, System.EventArgs e)
        {

            if (_type == 0)
            {
                bool ishad = false;
                foreach (AddDataIncidentFactorModel.ItemsBean model in App.contaminantsList)
                {
                    if (model.factorName == Title)
                    {
                        ishad = true;
                        break;
                    }
                }
                if (ishad == false)
                {
                    App.contaminantsList.Add(_factorModel);
                    MessagingCenter.Send<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", _factorModel);

                }

            }
            else if (_type == 1)
            {
                bool ishad = false;
                foreach (AddDataIncidentFactorModel.ItemsBean model in App.AppLHXZList)
                {
                    if (model.factorName == Title)
                    {
                        ishad = true;
                        break;
                    }
                }
                if (ishad == false)
                {
                    App.AppLHXZList.Add(_factorModel);
                    MessagingCenter.Send<ContentPage, AddDataIncidentFactorModel.ItemsBean>(this, "AddFactorNew", _factorModel);
                }

            }

            AddDataForChemicolOrLHXZModel.ItemsBean item = new AddDataForChemicolOrLHXZModel.ItemsBean
            {
                factorName = Title,
                lng = lastItem.lng,
                lat = lastItem.lat,
                yangBenLeiXing = sampleBut.Text,
                unitName = jianCeUnit.Text,
                factorId = _factorModel.factorId,
                datatype = _factorModel.dataType,
            };
            if (string.IsNullOrWhiteSpace(numLab.Text)) item.jianCeZhi = numLab.Placeholder;
            else item.jianCeZhi = numLab.Text;


            MessagingCenter.Send<ContentPage, AddDataForChemicolOrLHXZModel.ItemsBean>(this, "AddLHXZ", item);

            int aa = Navigation.NavigationStack.Count;
            for (int i = Navigation.NavigationStack.Count; i >= 0;i--){
                if (Navigation.NavigationStack[i - 2] is AddEmergencyAccidentInfoPage) break;
                else Navigation.RemovePage(Navigation.NavigationStack[i-2]);
            }

            Navigation.PopAsync();

        }

        ObservableCollection<LHXZAddressMode> dataList1 = new ObservableCollection<LHXZAddressMode>();
        LHXZAddressMode lastItem =null;
        public LHXZInfoPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            var item1 = new LHXZAddressMode
            {
                name = "当前位置",
                isCurrent = true,
            };           
            if (App.currentLocation != null)
            {
                item1.lng = Convert.ToString(Math.Round(App.currentLocation.Longitude, 6));
                item1.lat = Convert.ToString(Math.Round(App.currentLocation.Latitude, 6));
            }
            dataList1.Add(item1);
            lastItem = item1;
            listView.ItemsSource = dataList1;

            //默认显示第一个
            if(App.sampleTypeList.Count >0){
                sampleBut.Text = App.sampleTypeList[0].name;
            }
            getAllAddress();
        }
        int _type = 10;

        AddDataIncidentFactorModel.ItemsBean _factorModel;
        public LHXZInfoPage( AddDataIncidentFactorModel.ItemsBean model ,int type):this (){
            _type = type;
            Title = model.factorName;
            _factorModel = model;

            //设置默认单位
            if (!string.IsNullOrWhiteSpace(model.unit)) jianCeUnit.Text = model.unit;
            //设置默认样本类型
            if (model.dataType == "1") sampleBut.Text = "大气";
            if (model.dataType == "2") sampleBut.Text = "水质";
            if (model.dataType == "3") sampleBut.Text = "土壤";
        }



        private async void getAllAddress()
        {
            string param = "";
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SamplingModule.url + "/api/Sampleplan/GetPlanListByProid" + "?Proid=" + App.EmergencyAccidentID, param, "GET", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<LHXZAddressMode> dataList = Tools.JsonUtils.DeserializeObject<List<LHXZAddressMode>>(hTTPResponse.Results);
                if (dataList == null) return;
                for (int i = 0; i < dataList.Count;i ++){
                    dataList1.Add(dataList[i]);
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }


      
       
    }
}
