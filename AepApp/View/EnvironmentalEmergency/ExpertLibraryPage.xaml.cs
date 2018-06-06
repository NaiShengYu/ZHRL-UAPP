using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ExpertLibraryPage : ContentPage
    {
        private int start = 0;
        private int totalNum = 0;
        private ObservableCollection<ExpertLibraryModels.ItemsBean> dataList = new ObservableCollection<ExpertLibraryModels.ItemsBean>();
        //打电话
        void Handle_Clicked_1(object sender, System.EventArgs e)
        {

            var but = sender as Button;
            var item = but.BindingContext as item;

            Device.OpenUri(new Uri("tel:" + item.tel));

        }
        //发信息
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Console.WriteLine("发信息");
            var but = sender as Button;
            var item = but.BindingContext as item;
            Device.OpenUri(new Uri("sms:" + item.tel));
        }
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;
            Console.WriteLine("点击了item");


        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {          
            ExpertLibraryModels.ItemsBean item = e.Item as ExpertLibraryModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (start <= totalNum)
                {
                    ReqExpertLibrary("", "", start, 10); //网络请求专家库，10条每次
                }
            }
        }

        private async void ReqExpertLibrary(String Filter, String Sorting, int SkipCount, int MaxResultCount)
        {
            string url = App.BaseUrlForYINGJI + DetailUrl.ExpertLibraryUrl + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=" + MaxResultCount + "&SkipCount=" + SkipCount;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                start += 10;
                ExpertLibraryModels.SpecialBean specialBean = new ExpertLibraryModels.SpecialBean();
                specialBean = JsonConvert.DeserializeObject<ExpertLibraryModels.SpecialBean>(hTTPResponse.Results);
                totalNum = specialBean.result.professionals.totalCount;
                List<ExpertLibraryModels.ItemsBean> list = specialBean.result.professionals.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
            }
        }

        Dictionary<Button, Tuple<item>> _butData = new Dictionary<Button, Tuple<item>>();


        public ExpertLibraryPage()
        {
            InitializeComponent();
            ReqExpertLibrary("", "", start, 10); //网络请求专家库，10条每次       
        }

        internal class item
        {
            public string name { get; set; }
            public string message { set; get; }
            public string tel { get; set; }
        }
    }
}
