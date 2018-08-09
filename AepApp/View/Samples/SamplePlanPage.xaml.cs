using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;
using Newtonsoft.Json;
using CloudWTO.Services;

namespace AepApp.View.Samples
{
    public partial class SamplePlanPage : ContentPage
    {
        //加一天
        void addADay(object sender, System.EventArgs e)
        {
            currentDay = currentDay.AddDays(1.0);           
            timeLab.Text = currentDay.ToString("yyyy-MM-dd");

            requestSamplePlanList();

        
        }
        //减一天
        void reduceADay(object sender, System.EventArgs e)
        {
            currentDay = currentDay.AddDays(-1.0);
            timeLab.Text = currentDay.ToString("yyyy-MM-dd");

            requestSamplePlanList();
        }

        /// <summary>
        /// 选中了item
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            MySamplePlanItems item = e.SelectedItem as MySamplePlanItems;
            Navigation.PushAsync(new SamplePlanInfoPage(item));
            listView.SelectedItem = null;
        }
        private ObservableCollection<CollectionAndTransportSampleModel> dataList = new ObservableCollection<CollectionAndTransportSampleModel>();
        private DateTime currentDay = DateTime.Now;
        public SamplePlanPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            timeLab.Text = currentDay.ToString("yyyy-MM-dd");
            requestSamplePlanList();

        }


        private void AddPlans()
        {
            //DatabaseContext dbContext = new Da
        }

        private async void requestSamplePlanList(){

            samplePlanRequestDic parameter = new samplePlanRequestDic
            {
                pageIndex = -1,
                searchKey = "",
                planTime = currentDay.ToString("yyyy-MM-dd"),
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SampleURL + "/Api/SamplePlan/PagedListForPhone", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                App.mySamplePlanResult = JsonConvert.DeserializeObject<MySamplePlanResult>(hTTPResponse.Results);
                Console.WriteLine("结果是：" + App.mySamplePlanResult);
                listView.ItemsSource = App.mySamplePlanResult.Items;

            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }


        }


        internal class samplePlanRequestDic
        {
            public string searchKey { get; set; }
            public string planTime { get; set; }
            public int pageIndex { get; set; }
        }


    }
}
