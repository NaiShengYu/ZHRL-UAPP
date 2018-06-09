using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Xamarin.Forms;
using static AepApp.Models.EmergencyAccidentInfoDetail;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAccidentInfoPage : ContentPage
    {

        //bool isSelectText = false;
        //bool isSelectImage = false;
        //bool isSelectData = false;
        //bool isSelectReport = false;
        private ObservableCollection<IncidentLoggingEventsBean> dataList = new ObservableCollection<IncidentLoggingEventsBean>();
        private List<IncidentLoggingEventsBean> appearList = new List<IncidentLoggingEventsBean>();
        //选中文字图标
        void selectText(object sender, System.EventArgs e)
        {
            //ChangeBtBackgroundColor(true, false, false, false, false);
            //isSelectText = !isSelectText;
            //var but = sender as Button;
            //if (isSelectText == true)
            //    but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            //else
            //    but.BackgroundColor = Color.Transparent;
            appearList.Clear();
            int count = dataList.Count;
            for (int i = 0; i < count; i++)
            {
                string ca = dataList[i].category;
                switch (ca)
                {
                    case "IncidentFactorIdentificationEvent":
                    case "IncidentLocationSendingEvent":
                    case "IncidentNatureIdentificationEvent":
                    case "IncidentWindDataSendingEvent":
                    case "IncidentMessageSendingEvent":
                        appearList.Add(dataList[i]);
                        break;
                }

            }
            listView.ItemsSource = null;
            listView.ItemsSource = appearList;
        }

        //选中图片图标
        void selectImage(object sender, System.EventArgs e)
        {
            //ChangeBtBackgroundColor(false, true, false, false, false);
            //isSelectImage = !isSelectImage;
            //var but = sender as Button;
            //if (isSelectImage == true)
            //    but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            //else
            //    but.BackgroundColor = Color.Transparent;
            appearList.Clear();
            int count = dataList.Count;
            for (int i = 0; i < count; i++)
            {
                string ca = dataList[i].category;
                switch (ca)
                {
                    case "IncidentPictureSendingEvent":
                        appearList.Add(dataList[i]);
                        break;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = appearList;
        }
        //选中数据图标
        void selectData(object sender, System.EventArgs e)
        {
            //ChangeBtBackgroundColor(false, false, true, false, false);
            //isSelectData = !isSelectData;
            //var but = sender as Button;
            //if (isSelectData == true)
            //    but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            //else
            //    but.BackgroundColor = Color.Transparent;
            appearList.Clear();
            int count = dataList.Count;
            for (int i = 0; i < count; i++)
            {
                string ca = dataList[i].category;
                switch (ca)
                {
                    case "IncidentFactorMeasurementEvent":
                        appearList.Add(dataList[i]);
                        break;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = appearList;
        }
        //选中报告图标
        void selectReport(object sender, System.EventArgs e)
        {
            //ChangeBtBackgroundColor(false, false, false, true, false);
            //isSelectReport = !isSelectReport;
            //var but = sender as Button;
            //if (isSelectReport == true)
            //    but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            //else
            //    but.BackgroundColor = Color.Transparent;
            appearList.Clear();
            int count = dataList.Count;
            for (int i = 0; i < count; i++)
            {
                string ca = dataList[i].category;
                switch (ca)
                {
                    case "IncidentPlanGenerationEvent":
                    case "IncidentReportGenerationEvent":
                        appearList.Add(dataList[i]);
                        break;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = appearList;
        }

        //添加事故
        void addSouce(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddEmergencyAccidentInfoPage(dataList));
        }

        void selectAll(object sender, System.EventArgs e)
        {
            //ChangeBtBackgroundColor(false, false, false, false, true);
            //isSelectText = !isSelectText;
            //var but = sender as Button;
            //if (isSelectText == true)
            //    but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            //else
            //    but.BackgroundColor = Color.Transparent;
            appearList.Clear();
            listView.ItemsSource = dataList;
        }



        void Handle_Clicked(object sender, System.EventArgs e)
        {

            var but = sender as Button;
            var item = but.BindingContext as item;

            appearList.Clear();

            listView.ScrollTo(item, ScrollToPosition.Start, true);
        }

        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {

            var item = e.Item as IncidentLoggingEventsBean;

            try
            {
                TimeSpan time1 = appearList[appearList.Count - 1].creationTime.Subtract(item.creationTime).Duration();
                var timelong = time1.Seconds;

                int a = dataList.IndexOf(item);
                int b = dataList.IndexOf(appearList[appearList.Count - 1]);

                if (a < b)
                {
                    appearList.Insert(0, item);

                }
                else
                {
                    appearList.Add(item);
                }
                showCurrentItems();

            }
            catch (Exception ex)
            {
                appearList.Add(item);
                showCurrentItems();

            }

        }

        void Handle_ItemDisappearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            var item = e.Item as IncidentLoggingEventsBean;
            appearList.Remove(item);
            showCurrentItems();
        }




        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            if (item.imgSourse.Length > 0 && item.imgSourse != null)
            {
                List<string> imgs = new List<string>();
                imgs.Add("https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg");
                imgs.Add("https://ss0.bdstatic.com/70cFvHSh_Q1YnxGkpoWK1HF6hhy/it/u=2738969446,4147851924&fm=27&gp=0.jpg");
                imgs.Add("https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg");

                Navigation.PushAsync(new BrowseImagesPage(imgs));
            }


            listView.SelectedItem = null;
        }



        public EmergencyAccidentInfoPage(string name, string id)
        {
            InitializeComponent();
            //DTS = new EventDataTemplateSelector(this);

            this.Title = name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            ReqEmergencyAccidentDetail(id);

            BindingContext = this;
            //select_All.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            creatScrollerView();
        }

        private async void ReqEmergencyAccidentDetail(string id)
        {
            string url = App.BaseUrlForYINGJI + DetailUrl.GetEmergencyDetail +
                   "?Id=" + id;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                //start += 10;
                EmergencyAccidentInfoDetail.EmergencyAccidentBean emergencyAccidentBean = new EmergencyAccidentInfoDetail.EmergencyAccidentBean();
                emergencyAccidentBean = JsonConvert.DeserializeObject<EmergencyAccidentInfoDetail.EmergencyAccidentBean>(hTTPResponse.Results);
                //totalNum = accidentPageModels.result.incidents.totalCount;
                List<EmergencyAccidentInfoDetail.IncidentLoggingEventsBean> list = emergencyAccidentBean.result.incidentLoggingEvents;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    string cagy = list[i].category;
                    if (cagy != "IncidentNameModificationEvent" && cagy != "IncidentOccurredTimeRespecifyingEvent"
                        && cagy != "IncidentPlanGenerationEvent")
                    {
                        dataList.Add(list[i]);
                    }
                }
                listView.ItemsSource = dataList;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }


        void creatScrollerView()
        {

            DateTime lastTime = new DateTime();
            for (int i = 0; i < dataList.Count; i++)
            {
                var item = dataList[i];
                if (i == 0)
                {
                    lastTime = item.creationTime;
                }

                TimeSpan time1 = item.creationTime.Subtract(lastTime).Duration();
                double timeLong = time1.TotalHours;

                BoxView box = new BoxView
                {
                    HeightRequest = timeLong,
                };

                BoxView box1 = new BoxView
                {
                    BackgroundColor = Color.Gray,
                    Margin = new Thickness(5, 0, 5, 0),
                    HeightRequest = 8,
                    WidthRequest = 30,
                };
                box1.BindingContext = item;

                scrollLayout.Children.Add(box);
                scrollLayout.Children.Add(box1);
                lastTime = item.creationTime;
            }

        }

        void showCurrentItems()
        {
            if (appearList.Count > 0)
            {
                var firstItem = appearList[0];
                var lastItem = appearList[appearList.Count - 1];
                TimeSpan time1 = firstItem.creationTime.Subtract(lastItem.creationTime).Duration();
                double timeLong = time1.TotalHours;
                positionView.HeightRequest = 10 * appearList.Count + timeLong - 2;

                //循环出当前第一个item在原数组排第几位
                var item = dataList[0];
                int a = dataList.IndexOf(firstItem);

                //postiongView 开始位置
                TimeSpan time2 = firstItem.creationTime.Subtract(item.creationTime).Duration();
                double timeLong1 = time2.TotalHours;
                positionView.Margin = new Thickness(0, 10 * a + timeLong1 + 1, 0, 0);

                scroll.ScrollToAsync(0, 10 * a + timeLong1 + 1, true);

            }
        }



        internal class item
        {
            public string imgSourse { get; set; }
            public string timeAndName { set; get; }
            public string info { set; get; }
            public string address { set; get; }
            public bool isShowAddress { set; get; }

            public DateTime time { get; set; }
        }
        private void ChangeBtBackgroundColor(bool isSelectText, bool isSelectImage, bool isSelectData, bool isSelectReport, bool selectAll)
        {
            select_Text.BackgroundColor = Color.Transparent;
            select_Image.BackgroundColor = Color.Transparent;
            select_Data.BackgroundColor = Color.Transparent;
            select_Report.BackgroundColor = Color.Transparent;
            select_All.BackgroundColor = Color.Transparent;
            if (isSelectText) select_Text.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            if (isSelectImage) select_Image.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            if (isSelectData) select_Data.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            if (isSelectReport) select_Report.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            if (selectAll) select_All.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
        }


    }

    public class EventDataTemplateSelector : DataTemplateSelector
    {
        private static EventDataTemplateSelector _inst = new EventDataTemplateSelector();

        public static EventDataTemplateSelector Instance
        {
            get { return _inst; }
        }

        DataTemplate natureDT = null;
        DataTemplate centerLocDT = null;
        DataTemplate FactorIdentificationDT = null;
        DataTemplate FactorMeasurementDT = null;
        DataTemplate MessageSendingDT = null;
        DataTemplate PictureSendingDT = null;
        DataTemplate ReportGenerationDT = null;

        public EventDataTemplateSelector()
        {
            App a = App.Current as App;
            natureDT = a.Resources["natureDT"] as DataTemplate;
            centerLocDT = a.Resources["centerLocDT"] as DataTemplate;
            FactorIdentificationDT = a.Resources["FactorIdentificationDT"] as DataTemplate;
            FactorMeasurementDT = a.Resources["FactorMeasurementDT"] as DataTemplate;
            MessageSendingDT = a.Resources["MessageSendingDT"] as DataTemplate;
            PictureSendingDT = a.Resources["PictureSendingDT"] as DataTemplate;
            ReportGenerationDT = a.Resources["ReportGenerationDT"] as DataTemplate;
            //PlanGenerationDT = page.Resources["PlanGenerationDT"] as DataTemplate;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            IncidentLoggingEventsBean i = item as IncidentLoggingEventsBean;

            switch (i.category)
            {
                case "IncidentNatureIdentificationEvent": return natureDT;
                case "IncidentLocationSendingEvent": return centerLocDT;
                case "IncidentFactorIdentificationEvent": return FactorIdentificationDT;
                case "IncidentFactorMeasurementEvent": return FactorMeasurementDT;
                case "IncidentMessageSendingEvent": return MessageSendingDT;
                case "IncidentPictureSendingEvent": return PictureSendingDT;
                case "IncidentReportGenerationEvent": return ReportGenerationDT;
                    //case "IncidentPlanGenerationEvent": return PlanGenerationDT;
            }
            return natureDT;
        }
    }

}
