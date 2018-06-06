using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyPlan : ContentPage
    {
        private int start = 0;
        private int totalNum = 0;
        private ObservableCollection<EmergencyPlanModels.ItemsBean> dataList = new ObservableCollection<EmergencyPlanModels.ItemsBean>();
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }

       

        public EmergencyPlan()
        {
            InitializeComponent();
            ReqEmergencyPlan("", "", start, 10);
            //var item1 = new item
            //{
            //    imgSourse = "https://ss1.bdstatic.com/70cFvXSh_Q1YnxGkpoWK1HF6hhy/it/u=729412813,2297218092&fm=27&gp=0.jpg",
            //    info = "先帝创业未半而中道崩殂，今天下三分，益州疲弊，此诚危急存亡之秋也。然侍卫之臣不懈于内，忠志之士忘身于外者，盖追先帝之殊遇，欲报之于陛下也。诚宜开张圣听，以光先帝遗德，恢弘志士之气，不宜妄自菲薄，引喻失义，以塞忠谏之路也。",
            //};

            //dataList.Add(item1);

            //var item2 = new item
            //{
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=331890373,3824021971&fm=27&gp=0.jpg",
            //    info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
            //};

            //dataList.Add(item2);

            //var item3 = new item
            //{
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
            //    info = "侍中、侍郎郭攸之、费祎、董允等，此皆良实，志虑忠纯，是以先帝简拔以遗陛下。愚以为宫中之事，事无大小，悉以咨之，然后施行，必能裨补阙漏，有所广益。",
            //};

            //dataList.Add(item3);

            //listView.ItemsSource = dataList;
    
        }
        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            EmergencyPlanModels.ItemsBean item = e.Item as EmergencyPlanModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (start <= totalNum)
                {
                    ReqEmergencyPlan("", "", start, 10); //网络请求敏感源，10条每次
                }
            }
        }

        private async void ReqEmergencyPlan(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.BaseUrlForYINGJI + DetailUrl.EmergencyPlan + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                start += 10;
                EmergencyPlanModels.EmergencyPlanBean emergencyPlanBean = new EmergencyPlanModels.EmergencyPlanBean();
                emergencyPlanBean = JsonConvert.DeserializeObject<EmergencyPlanModels.EmergencyPlanBean>(hTTPResponse.Results);
                totalNum = emergencyPlanBean.result.preplans.totalCount;
                List<EmergencyPlanModels.ItemsBean> list = emergencyPlanBean.result.preplans.items;
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
     
              public string imgSourse { get; set; }
            public string info { set; get; }




        }

        
    }
}
