using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Xamarin.Forms;
using SimpleAudioForms;
using static AepApp.Models.EmergencyAccidentInfoDetail;
using AepApp.Tools;
using System.IO;
using AepApp.Services;

namespace AepApp.View.EnvironmentalEmergency
{
   
    public partial class EmergencyAccidentInfoPage : ContentPage
    {
        private ObservableCollection<IncidentLoggingEventsBean> dataList = new ObservableCollection<IncidentLoggingEventsBean>();
        private ObservableCollection<IncidentLoggingEventsBean> appearList = new ObservableCollection<IncidentLoggingEventsBean>();
        private EmergencyAccidentPageModels.ItemsBean _accident = null;
        bool isSelectText = false;
        bool isSelectImage = false;
        bool isSelectData = false;
        bool isSelectReport = false;
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

            App.LastNatureAccidentModel = null;
            foreach (IncidentLoggingEventsBean emergencyModel1 in dataList)
            {
                if (emergencyModel1.category == "IncidentNatureIdentificationEvent")
                {
                    UploadEmergencyShowModel emergencyModel = new UploadEmergencyShowModel
                    {
                        creationTime = System.DateTime.Now,
                        natureString = emergencyModel1.natureString,
                        emergencyid = _accident.id,
                        category = "IncidentNatureIdentificationEvent"
                    };
                    App.LastNatureAccidentModel = emergencyModel;
                    break;
                }
            }
            Navigation.PushAsync(new AddEmergencyAccidentInfoPage(_accident.id));
        }

        void selectAll(object sender, System.EventArgs e)
        {
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



        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as IncidentLoggingEventsBean;
            if (item == null)
                return;
            List<string> imgs = new List<string>();
            if(item.category =="IncidentPictureSendingEvent"){
                List<IncidentLoggingEventsBean> imgsSouce = new List<IncidentLoggingEventsBean>();
                imgsSouce.Add(item);
                Navigation.PushAsync(new BrowseImagesPage(imgsSouce));
            }
            //进入视频播放页
            if (item.category == "IncidentVideoSendingEvent"){
                Navigation.PushAsync(new ShowVideoPage(item));
            } 
            listView.SelectedItem = null;
        }

        public EmergencyAccidentInfoPage(EmergencyAccidentPageModels.ItemsBean accident)
        {
            InitializeComponent();
            _accident = accident;
            this.Title = accident.name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            BindingContext = this;
            if (accident.isArchived != "false")
            {
                GridBottom.ColumnDefinitions[5].Width = 0;
            }
            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                    Navigation.PushAsync(new EmergencyMapPage(dataList,_accident.id));
            }));
         
        }

        private async void ReqEmergencyAccidentDetail(string id)
        {
            string url = "";
            if(dataList.Count ==0)url= App.EmergencyModule.url + DetailUrl.GetEmergencyDetail +
                   "?Id=" + id;
            else {
                EmergencyAccidentInfoDetail.IncidentLoggingEventsBean item = dataList[0];
                string time = string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.creationTime.AddSeconds(2));
                time = System.Net.WebUtility.UrlEncode(time); 
                url = App.EmergencyModule.url + DetailUrl.GetEmergencyDetail +
                         "?Id=" + id + "&After=" +time;
            }
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                EmergencyAccidentInfoDetail.EmergencyAccidentBean emergencyAccidentBean = new EmergencyAccidentInfoDetail.EmergencyAccidentBean();
                emergencyAccidentBean = Tools.JsonUtils.DeserializeObject<EmergencyAccidentInfoDetail.EmergencyAccidentBean>(hTTPResponse.Results);
                if(emergencyAccidentBean == null || emergencyAccidentBean.result == null || emergencyAccidentBean.result.items == null)
                {
                    return;
                }
                List<EmergencyAccidentInfoDetail.IncidentLoggingEventsBean> list = emergencyAccidentBean.result.items;
                int count = list.Count;
                rightListV.ItemsSource = null;
                for (int i = count-1; i >=0; i--)
                {
                    EmergencyAccidentInfoDetail.IncidentLoggingEventsBean bean = list[i];
                    string cagy = bean.category;
                    if (cagy != "IncidentNameModificationEvent" && cagy != "IncidentOccurredTimeRespecifyingEvent"
                        )
                    {

                        //dataList.Add(bean);
                        dataList.Insert(0, bean);
                    }

                    if (cagy == "IncidentLocationSendingEvent")
                    {
                        AzmCoord center =new AzmCoord(bean.targetLng == null ? 0 : bean.targetLng.Value, bean.targetLat == null ? 0 : bean.targetLat.Value);
                        bean.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("事故中心点", center)); });
                    }
                    else if (cagy == "IncidentFactorMeasurementEvent")
                    {
                        AzmCoord center = StringUtils.string2Coord(bean.lng, bean.lat);
                        string measment = bean.measurement;
                        bean.LocateOnMapCommand = new Command(async () => {await Navigation.PushAsync(new RescueSiteMapPage("数据位置", center,measment)); });
                    }
                    else if (cagy == "IncidentMessageSendingEvent")
                    {
                        try{
                            AzmCoord center = StringUtils.string2Coord(bean.lng, bean.lat);
                            bean.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("文字信息发出位置", center)); });
                        }catch(Exception ex){
                        }
                    }
                    else if (cagy == "IncidentWindDataSendingEvent")
                    {
                        try
                        {
                            AzmCoord center = StringUtils.string2Coord(bean.lng, bean.lat);
                            bean.LocateOnMapCommand = new Command(async () => { await Navigation.PushAsync(new RescueSiteMapPage("风速风向发出位置", center)); });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else if (cagy == "IncidentVoiceSendingEvent")
                    {
                        try
                        {
                            EmergencyAccidentInfoDetail.IncidentLoggingEventsBean item = bean;
                            bean.PlayVoiceCommand = new Command( () => {
                            DependencyService.Get<IAudio>().PlayNetFile(item.VoicePath); 
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else if(cagy == "IncidentVideoSendingEvent")
                    {
                        string videoPath = bean.VideoPath;
                        if (!string.IsNullOrWhiteSpace(videoPath) && i < list.Count && i >= 0)
                        {
                            bean.CoverPath = FileUtils.ReplaceFileSuffix(videoPath, ".jpg");
                            dataList[0].CoverPath = bean.CoverPath;
                        }
                    }

                    else if (cagy == "IncidentReportGenerationEvent" || cagy == "IncidentPlanGenerationEvent")
                    {
                        string fileurl = App.EmergencyModule.url + bean.StorePath;
                        string fileFormat = FileUtils.GetFileName(bean.StorePath, true);
                        bean.DocumentDownloadCommand = new Command(async () => 
                        {
                            string dirPath = DependencyService.Get<IFileService>().GetExtrnalStoragePath(Constants.STORAGE_TYPE_DOC);
                            string absPath = Path.Combine(dirPath, fileFormat);
                            if (!File.Exists(absPath))
                            {
                                HTTPResponse res = await EasyWebRequest.HTTPRequestDownloadAsync(fileurl, fileFormat, App.EmergencyToken);
                            }
                            if (Device.RuntimePlatform == Device.Android)
                            {
                                DependencyService.Get<IFileService>().OpenFileDocument(absPath, FileUtils.GetFileSuffix(absPath));
                            }
                            else
                            {
                                await Navigation.PushAsync(new ShowFilePage(absPath));
                            }
                        });
                    }
                }
                creatScrollerView();
                //清空listView数据为了显示最新数据
                //listView.ItemsSource = null;
                //rightListV.ItemsSource = null;
                rightListV.ItemsSource = dataList;
                listView.ItemsSource = dataList;
                GetEmergencyCenterCoord();
            }
        }

        void GetEmergencyCenterCoord()
        {
            App.EmergencyCenterCoord = null;
            foreach (IncidentLoggingEventsBean item in dataList)
            {
                if (item.targetLat != 0 && item.targetLng != 0 && item.targetLat != null && item.targetLng != null)
                {//筛选最新的一次事故中心位置
                    if (App.EmergencyCenterCoord == null)
                    {
                        if (Convert.ToDouble(item.targetLat) <= 90.0) App.EmergencyCenterCoord = new AzmCoord(Convert.ToDouble(item.targetLng), Convert.ToDouble(item.targetLat));
                    }
                }
            }

            if (App.EmergencyCenterCoord == null)
            {
                if (App.currentLocation != null)//初始给一个当前定位
                    App.EmergencyCenterCoord = new AzmCoord(App.currentLocation.Longitude, App.currentLocation.Latitude);
                //如果无法获取当前定位，就给一个默认值
                else App.EmergencyCenterCoord = new AzmCoord(121.630325, 29.889472);
            }
            //if (App.EmergencyCenterCoord.lat != 0.0f && App.EmergencyCenterCoord.lng != 0.0)
            //{
            //    Gps gps = PositionUtil.gcj_To_Gps84(App.EmergencyCenterCoord.lat, App.EmergencyCenterCoord.lng);
            //    App.EmergencyCenterCoord = new AzmCoord(gps.getWgLon(), gps.getWgLat());
            //}

        }



        bool isStart = true;
        protected override void OnAppearing()
        {
            base.OnAppearing();

            ReqEmergencyAccidentDetail(_accident.id);

            isStart = true;
            //为了进入界面item在顶部
            if (dataList.Count > 0) listView.ScrollTo(dataList[0], ScrollToPosition.Start, true);
            if (dataList.Count > 0) rightListV.ScrollTo(dataList[0], ScrollToPosition.Start, true);

            //定时器
            if (_accident.isArchived == "false")
            {
            rowOne.Height = 55;
            timeBut.IsVisible = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (isStart == false) return false;

                var nowTime = DateTime.Now;
                var lastTime = _accident.startDate;
                TimeSpan time1 = lastTime.Subtract(nowTime).Duration();
                //注意：需要用小写字母来显示时时间
                timeBut.Text = time1.ToString("d'天 'h'小时 'm'分 's'秒'");
                return true;
            });
            }else{
                rowOne.Height = 0;
                timeBut.IsVisible = false;
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isStart = false;
            DependencyService.Get<IAudio>().stopPlay();
        }

        void creatScrollerView(){
            for (int i = 0; i < dataList.Count; i++)
            {
                var item = dataList[i];
                if (i == 0)
                {
                    item.marge = new Thickness(5, 0, 5, 0);
                }else{
                    var item1 = dataList[i - 1];
                    TimeSpan time1 = item.creationTime.Subtract(item1.creationTime).Duration();
                    double timeLong = time1.TotalHours;

                    //定义最高高度
                    double maxH = 100;
                    //定义每五分钟一个像素
                    double minPixel = 5.0 / 60;
                    double K = -(1 / minPixel) * Math.Log(1.0 - 1.0 / maxH);
                    timeLong =  maxH* (1 - Math.Exp(-timeLong * K));
                    item.marge = new Thickness(5, timeLong, 5, 0);
                }
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
        DataTemplate WindDataDT = null;
        DataTemplate VideoDT = null;
        DataTemplate VoiceDT = null;
        DataTemplate PlanGenerationDT = null;

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
            WindDataDT = a.Resources["WindDataDT"] as DataTemplate;
            PlanGenerationDT = a.Resources["PlanGenerationDT"] as DataTemplate;
            VideoDT = a.Resources["VideoDT"] as DataTemplate;
            VoiceDT = a.Resources["VoiceDT"] as DataTemplate;

        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            IncidentLoggingEventsBean i = item as IncidentLoggingEventsBean;
            if (i == null)
            {
                UploadEmergencyShowModel j = item as UploadEmergencyShowModel;
                switch (j.category)
                {
                    case "IncidentNatureIdentificationEvent": return natureDT;
                    case "IncidentLocationSendingEvent": return centerLocDT;
                    case "IncidentFactorIdentificationEvent": return FactorIdentificationDT;
                    case "IncidentFactorMeasurementEvent": return FactorMeasurementDT;
                    case "IncidentMessageSendingEvent": return MessageSendingDT;
                    case "IncidentPictureSendingEvent": return PictureSendingDT;
                    case "IncidentReportGenerationEvent": return ReportGenerationDT;
                    case "IncidentWindDataSendingEvent": return WindDataDT;
                    case "IncidentPlanGenerationEvent": return PlanGenerationDT;
                    case "IncidentVideoSendingEvent": return VideoDT;
                    case "IncidentVoiceSendingEvent": return VoiceDT;
                }
                return natureDT;
            }
            else {
                switch (i.category)
                {
                    case "IncidentNatureIdentificationEvent": return natureDT;
                    case "IncidentLocationSendingEvent": return centerLocDT;
                    case "IncidentFactorIdentificationEvent": return FactorIdentificationDT;
                    case "IncidentFactorMeasurementEvent": return FactorMeasurementDT;
                    case "IncidentMessageSendingEvent": return MessageSendingDT;
                    case "IncidentPictureSendingEvent": return PictureSendingDT;
                    case "IncidentReportGenerationEvent": return ReportGenerationDT;
                    case "IncidentWindDataSendingEvent": return WindDataDT;
                    case "IncidentPlanGenerationEvent": return PlanGenerationDT;
                    case "IncidentVideoSendingEvent": return VideoDT;
                    case "IncidentVoiceSendingEvent": return VoiceDT;

                }
                return natureDT;
            }                    
        }
    }

}
