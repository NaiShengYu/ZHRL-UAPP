using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddDataForLHXZPage : ContentPage
    {
        void HandleEventHandler(object sender, EventArgs e)
        {
            var but = sender as Button;
            var aaa = but.BindingContext as AddDataIncidentFactorModel.ItemsBean;

            Navigation.PushAsync(new LHXZInfoPage(aaa, 1));

        }
        ObservableCollection<AddDataIncidentFactorModel.ItemsBean> dataList1 = new ObservableCollection<AddDataIncidentFactorModel.ItemsBean>();
        ObservableCollection<AddDataIncidentFactorModel.ItemsBean> dataList2 = new ObservableCollection<AddDataIncidentFactorModel.ItemsBean>();
        ObservableCollection<AddDataIncidentFactorModel.ItemsBean> dataList3 = new ObservableCollection<AddDataIncidentFactorModel.ItemsBean>();

        public AddDataForLHXZPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            //App.navgationPage = this;

            var itme1 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "PM2.5（大气）",
                factorId = "2692FAD2-589B-457B-BFF0-0B1723A52391",
                dataType = "1",
                unit = "μg/m³",
            };
            dataList1.Add(itme1);

            var itme2 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "PM10（大气）",
                factorId = "C6C1933C-468B-4517-B852-F0722C3F8CBC",
                dataType = "1",
                unit = "μg/m³",
            };
            dataList1.Add(itme2);

            var itme004 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "臭氧 O₃（大气）",
                factorId = "11047A01-D083-4DE1-B252-35FE62D845CE",
                dataType = "1",
                unit = "μg/m³",
            };
            dataList1.Add(itme004);
            var itme003 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "一氧化碳 CO（大气）",
                factorId = "ED83D8E0-C732-440A-BF78-BDDB4AFB9386",
                dataType = "1",
                unit = "μg/m³",
            };
            dataList1.Add(itme003);

            var itme002 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "二氧化氮 NO₂（大气）",
                factorId = "38693B8C-FEA5-440F-B675-20D52B22D13A",
                dataType ="1",
                unit = "μg/m³",
            };
            dataList1.Add(itme002);

            var itme001 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "二氧化硫 SO₂（大气）",
                factorId = "23D34C02-446A-4413-80CD-9EE0D7BE9308",
                dataType = "1",
                unit = "μg/m³",
            };
            dataList1.Add(itme001);



            var itme3 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "水温",
                factorId = "463D677E-4287-4B6E-B7B6-7A9399420AD8",
                dataType = "2",
                unit ="°C",
            };
            dataList2.Add(itme3);

            var itme4 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "pH（水质）",
                factorId = "7735D12D-F11E-4D74-9864-0A3275144C99",
                dataType = "2",
                unit = "无量纲",
            };
            dataList2.Add(itme4);

            var itme5 = new AddDataIncidentFactorModel.ItemsBean
            {
                factorName = "溶解氧（水质）",
                factorId = "399A6708-C6E1-4591-8C25-493AE05F7EED",
                dataType = "2",
                unit = "mg/L",
            };
            dataList2.Add(itme5);

            var itme6 = new AddDataIncidentFactorModel.ItemsBean
              {
                factorName = "高锰酸钾指数（水质）",
                factorId = "67F3B264-2E32-48D9-9F56-C4E554B0B9E3",
                dataType = "2",
                unit = "mg/L",
            };
            dataList2.Add(itme6);

            var itme50 = new AddDataIncidentFactorModel.ItemsBean
              {
                factorName = "化学需氧量 COD（水质）",
                factorId = "3362D199-8EEE-4DA2-80E3-C1334F016E24",
                dataType = "2",
                unit = "mg/L",
            };
            dataList2.Add(itme50);

            var itme7 = new AddDataIncidentFactorModel.ItemsBean
              {
                factorName = "五日生化需氧量 BOD₅（水质）",
                factorId = "018170F5-D189-4F8F-B0B5-56BBE371C6E4",
                dataType = "2",
                unit = "mg/L",
            };
            dataList2.Add(itme7);

            var itme51 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "氨氮 NH₃-N（水质）",
                factorId = "4A742D72-3ABB-45B0-A38D-CD0E00BC1B3E"
            };
            dataList2.Add(itme51);

            var itme8 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "总磷（水质）",
                factorId = "544BB0E2-4CE2-4824-B8C0-99D52BF70DEC"
            };
            dataList2.Add(itme8);

            var itme9 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "总氮（水质）",
                factorId = "8C4DE8C9-862D-4A80-BA1D-DFD619E93BBB"
            };
            dataList2.Add(itme9);

            var itme10 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "铜（水质）",
                factorId = "D3DA2844-6644-4A91-8AFF-93094B089041"
            };
            dataList2.Add(itme10);

            var itme11 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "锌（水质）",
                factorId = "9B2252F2-5402-4D17-AA6D-3010CFA816D9"
            };
            dataList2.Add(itme11);

            var itme12 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "氟化物（水质）",
                factorId = "69616E47-3728-4929-A938-45BE41286770"
            };
            dataList2.Add(itme12);

            var itme13 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "硒（水质）",
                factorId = "CBD265B7-6C9F-4609-AA0F-07BC86801C39"
            };
            dataList2.Add(itme13);

            var itme14 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "砷（水质）",
                factorId = "96609051-A9AB-4772-80A4-547544608F8F"
            };
            dataList2.Add(itme14);

            var itme15 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "汞（水质）",
                factorId = "492CDEBD-113F-43E1-9007-C02453B47B73"
            };
            dataList2.Add(itme15);

            var itme16 = new AddDataIncidentFactorModel.ItemsBean
              {
                unit = "mg/L",
                dataType = "2",
                factorName = "镉（水质）",
                factorId = "568E8B6C-36B5-4C28-B4F6-5FBCE79F864B"
            };
            dataList2.Add(itme16);

            var itme17 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "铬（水质）",
                factorId = "5C3B9285-8D97-4682-85B5-D5E34ACBBF20"
            };
            dataList2.Add(itme17);

            var itme18 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "铅（水质）",
                factorId = "1D7F8202-0464-4AA7-8067-2A3224E24CB1"
            };
            dataList2.Add(itme18);

            var itme19 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "氰化物（水质）",
                factorId = "B1D230B8-D196-455A-9BEF-E2B205D171C6"
            };
            dataList2.Add(itme19);

            var itme20 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "挥发酚（水质）",
                factorId = "25D7009A-CFE4-46F8-A01B-3C54AAFE6C43"
            };
            dataList2.Add(itme20);

            var itme21 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "石油类（水质）",
                factorId = "3F536B06-6087-4AB7-8BF7-069563B0BDD2"
            };
            dataList2.Add(itme21);

            var itme22 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "阴离子表面活性剂（水质）",
                factorId = "8F5FE1FD-D8F1-42E4-878C-02606808A7F7"
            };
            dataList2.Add(itme22);

            var itme23 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "mg/L",
                dataType = "2",
                factorName = "硫化物（水质）",
                factorId = "0EA13028-6796-43F1-806D-8801FA185799"
            };
            dataList2.Add(itme23);

            var itme24 = new AddDataIncidentFactorModel.ItemsBean
             {
                unit = "个/L",
                dataType = "2",
                factorName = "粪大肠菌群（水质）",
                factorId = "5B2DCFE2-8E89-4689-813C-A84B537DB9CE"
            };
            dataList2.Add(itme24);

            var itme25 = new AddDataIncidentFactorModel.ItemsBean
             {
                dataType = "3",
                factorName = "镉（土壤）",
                factorId = "5E39005A-6025-488E-9C19-6FB9801C8D9C"
            };
            dataList3.Add(itme25);

            var itme26 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "汞（土壤）",
                factorId = "C6ECA20D-036F-47DF-94C8-52597735ADB0"
            };
            dataList3.Add(itme26);

            var itme27 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "砷（土壤）",
                factorId = "1610BAC2-C2D6-4919-B207-F15C8E303F73"
            };
            dataList3.Add(itme27);

            var itme28 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                  factorName = "铜（土壤）",
                factorId = "EA5BEA27-FC08-48C3-AED6-BF87E85E3B78"
            };
            dataList3.Add(itme28);

            var itme29 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "铅（土壤）",
                factorId = "54C1AD9E-495E-487E-B4E5-CA02668E1F23"
            };
            dataList3.Add(itme29);
            var itme30 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "铬（土壤）",
                factorId = "14942C51-52EC-4827-B807-D069ED9A224E"
            };
            dataList3.Add(itme30);

            var itme31 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "锌（土壤）",
                factorId = "50078932-72A2-4DEA-87D4-6C434BB60520"
            };
            dataList3.Add(itme31);

            var itme32 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "镍（土壤）",
                factorId = "2CC5062F-E22A-4252-ABD4-802877F0C158"
            };
            dataList3.Add(itme32);

            var itme33 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "六六六（土壤）",
                factorId = "30F9C81D-63D8-4CCA-8F87-1DB1C719A2E6"
            };
            dataList3.Add(itme33);

            var itme34 = new AddDataIncidentFactorModel.ItemsBean
              {
                dataType = "3",
                factorName = "滴滴涕（土壤）",
                factorId = "44BABEAE-2EE2-4E61-9107-547811F8838A"
            };
            dataList3.Add(itme34);


            //根据事件类型来显示理化性质类型
            foreach(SampleTypeModel model in App.sampleTypeList)
            {
                if(model.name =="大气")creatDQ();
                if (model.name == "水质") creatSZ();
                if (model.name == "土壤") creatTR();
            }

        }


        void creatDQ()
        {
            for (int i = 0; i < dataList1.Count; i++)
            {
                var it = dataList1[i];
                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,
                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.factorName,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but = new Button
                {
                    BackgroundColor = Color.Transparent,
                };
                but.BindingContext = it;
                but.Clicked += HandleEventHandler;
                but.HorizontalOptions = LayoutOptions.FillAndExpand;
                but.VerticalOptions = LayoutOptions.FillAndExpand;
                sk.Children.Add(but);
                DQ.Children.Add(sk);
            }

        }

        void creatSZ()
        {
            for (int i = 0; i < dataList2.Count; i++)
            {
                var it = dataList2[i];
                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,

                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.factorName,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but = new Button
                {
                    BackgroundColor = Color.Transparent,
                };
                but.BindingContext = it;
                but.Clicked += HandleEventHandler;
                but.HorizontalOptions = LayoutOptions.FillAndExpand;
                but.VerticalOptions = LayoutOptions.FillAndExpand;
                sk.Children.Add(but);
                SZ.Children.Add(sk);
            }

        }

        void creatTR()
        {
            for (int i = 0; i < dataList3.Count; i++)
            {
                var it = dataList3[i];

                var sk = new Grid
                {
                    BackgroundColor = Color.White,
                    HeightRequest = 50,

                };

                var lab1 = new Label
                {
                    Margin = new Thickness(15, 5, 15, 5),
                    Text = it.factorName,
                    VerticalTextAlignment = TextAlignment.Center,

                };
                lab1.VerticalOptions = LayoutOptions.Center;
                lab1.HorizontalOptions = LayoutOptions.Start;
                sk.Children.Add(lab1);

                var but = new Button
                {
                    BackgroundColor = Color.Transparent,
                };
                but.BindingContext = it;
                but.Clicked += HandleEventHandler;
                but.HorizontalOptions = LayoutOptions.FillAndExpand;
                but.VerticalOptions = LayoutOptions.FillAndExpand;
                sk.Children.Add(but);
                TR.Children.Add(sk);
            }
        }
    }
}
