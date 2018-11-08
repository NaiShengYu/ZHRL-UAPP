using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;

namespace AepApp.View.Samples
{
    public partial class SamplePlanInfoPage : ContentPage
    {

        private ObservableCollection<TaskModel> dataList = new ObservableCollection<TaskModel>();
        MySamplePlanItems _samplePlanItems = null;
        SampleBasicDataModel _basicDataModel = new SampleBasicDataModel();

        /// <summary>
        /// 进入地址
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void GoPlanMap(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RescueSiteMapPage(_samplePlanItems));

        }

        /// <summary>
        /// 选中某一个任务
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void selectTask(object sender, EventArgs e)
        {
            Button button = sender as Button;
            taskslist model = button.BindingContext as taskslist;
            Navigation.PushAsync(new TastInfoPage(model.taskname, model.taskid));
        }


        /// <summary>
        /// Handles the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Button button = sender as Button;
            string message = "";
            string pagetitle = "";
            if (button.StyleId =="dispose") {
                Console.WriteLine("预处理信息");
                message = _samplePlanItems.pretreatment;
                pagetitle = "预处理信息";
            }
            if (button.StyleId =="security") {
                Console.WriteLine("安全说明");
                message = _samplePlanItems.security;
                pagetitle = "安全说明";
            }
            if (button.StyleId == "remake") {
                Console.WriteLine("备注信息");
                message = _samplePlanItems.remarks;
                pagetitle = "备注信息";
            }
            if (button.StyleId == "quality") {
                Console.WriteLine("质控说明");
                message = _samplePlanItems.qctip;
                pagetitle = "质控说明";
            }
            var messagepage = new ShowMessagePage(message);
            messagepage.Title = pagetitle;
            Navigation.PushAsync(messagepage);

        }
     
        public SamplePlanInfoPage(MySamplePlanItems sampleModel)
        {
            InitializeComponent();
            _samplePlanItems = sampleModel;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = sampleModel.name;
            this.BindingContext = sampleModel;
            if (sampleModel.tasklist == null || sampleModel.tasklist.Count == 0)
                TaskNumFrame.BackgroundColor = Color.Transparent;
            requestSamplePublicData();
            creatTask();

            ObservableCollection<SamplePhotoModel> samplePhotoModels = new ObservableCollection<SamplePhotoModel>();
            for (int i = 0; i < 10; i++)
            {
                SamplePhotoModel photoModel = new SamplePhotoModel
                {
                    photoPath = i.ToString(),
                    isSelect = true,
                };
                if (i == 1 || i == 2 || i == 5 || i == 7 || i == 0 || i == 9)
                    photoModel.isSelect = false;
                samplePhotoModels.Add(photoModel);
            }

            for (int i = samplePhotoModels.Count-1; i >0; i--)
            {
                var photoModel = samplePhotoModels[i];
                if (photoModel.isSelect == false)
                    samplePhotoModels.Remove(photoModel);
            }



        }


        private async void requestSamplePublicData(){           
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SampleURL + "/Api/WaterRecord/GetDetailByPid?planid=" +_samplePlanItems.id,"" , "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SampleBasicDataModel basicDataModel = JsonConvert.DeserializeObject<SampleBasicDataModel>(hTTPResponse.Results);
                Console.WriteLine("结果是：" + App.mySamplePlanResult);
                _basicDataModel = basicDataModel;
            }
            else
            {
                Console.WriteLine(hTTPResponse);
                _basicDataModel.sampledate = DateTime.Now;
            }
            basicDataSK.BindingContext = _basicDataModel;
        }

        void Handle_purposeChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.purpose = e.NewTextValue;
        }
        void Handle_areanameChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.areaname = e.NewTextValue;
        }
        void Handle_areafunctypeChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.areafunctype = e.NewTextValue;
        }
        void Handle_carshipChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.carship = e.NewTextValue;
        }
        void Handle_toolsChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.tools = e.NewTextValue;
        }
        void Handle_positionChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.position = e.NewTextValue;
        }

        void Handle_sampledateChanged(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            _basicDataModel.sampledate = e.NewDate;   
        
        }
        void Handle_weatherChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.weather = e.NewTextValue;
        }
        void Handle_temperatureChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _basicDataModel.temperature = e.NewTextValue;
        }

        async void SaveBaiscData(object sender ,EventArgs e){
                Dictionary<string, object> dic = new Dictionary<string, object>();
            string url = "/Api/WaterRecord/Add";
            dic.Add("areafunctype", _basicDataModel.areafunctype);
            dic.Add("areaname", _basicDataModel.areaname);
            dic.Add("carship", _basicDataModel.carship);
            dic.Add("planid", _samplePlanItems.id);
            dic.Add("position", _basicDataModel.position);
            dic.Add("purpose", _basicDataModel.purpose);
            dic.Add("sampledate", _basicDataModel.sampledate);
            dic.Add("temperature", _basicDataModel.temperature);
            dic.Add("tools", _basicDataModel.tools);
            dic.Add("weather", _basicDataModel.weather);
            //假如有信息id就更新id
            if(!string.IsNullOrWhiteSpace(_basicDataModel.id)){
                dic.Add("id", _basicDataModel.id);
                url = "/Api/WaterRecord/Update";
            }
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SampleURL + url, param, "POST", "");
                Console.WriteLine(hTTPResponse);
                if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                DependencyService.Get<Sample.IToast>().ShortAlert("修改成功");
                if (!string.IsNullOrWhiteSpace(_basicDataModel.id))
                    _basicDataModel.id = hTTPResponse.Results;
                }
                else
                {
                    Console.WriteLine(hTTPResponse);
                }
        }



        void creatTask()
        {
            if(_samplePlanItems == null || _samplePlanItems.tasklist == null)
            {
                return;
            }
            foreach (taskslist model in _samplePlanItems.tasklist)
            {
                foreach (tasksAnas anas in model.taskAnas){

                    Grid G = new Grid
                    {
                        HeightRequest = 60,
                        BackgroundColor = Color.White,
                    };
                    StackLayout sk1 = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(10, 8, 100, 0),
                        Orientation = StackOrientation.Horizontal,
                    };
                    G.Children.Add(sk1);
                    Label taskLab = new Label
                    {
                        Text = model.taskname,
                        Font = Font.SystemFontOfSize(17),
                    };

                    Grid typeGird = new Grid
                    {
                        BackgroundColor = Color.FromRgb(59, 125, 169),
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 24,
                    };

                    Label typeLab = new Label
                    {
                        Margin = new Thickness(2, 2, 2, 2),
                        Text = model.tasktypeName,
                        TextColor = Color.White,
                        Font = Font.SystemFontOfSize(14),
                    };
                    typeGird.Children.Add(typeLab);
                    sk1.Children.Add(taskLab);
                    sk1.Children.Add(typeGird);

                    StackLayout sk2 = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(10, 2, 100, 8),
                        Orientation = StackOrientation.Horizontal,
                    };
                    G.Children.Add(sk2);
                    Label factorLab = new Label
                    {
                        Text = anas.atname,
                        Font = Font.SystemFontOfSize(14),
                        TextColor = Color.Gray,
                    };
                    sk2.Children.Add(factorLab);

                    //Frame frame = new Frame
                    //{
                    //    HorizontalOptions = LayoutOptions.End,
                    //    VerticalOptions = LayoutOptions.Center,
                    //    WidthRequest = 25,
                    //    HeightRequest = 25,
                    //    CornerRadius = (float)12.5,
                    //    BackgroundColor = Color.FromRgb(190, 190, 190),
                    //    Margin = new Thickness(0, 0, 15, 0),
                    //    Padding = new Thickness(0, 0, 0, 0),
                    //    IsClippedToBounds = true,
                    //};
                    //Label numLab = new Label
                    //{
                    //    Text = model.taskAnas.Count.ToString(),
                    //    VerticalOptions = LayoutOptions.Center,
                    //    HorizontalOptions = LayoutOptions.Center,
                    //    TextColor = Color.White,

                    //};
                    //if (model.taskAnas.Count.ToString() == "0") frame.IsVisible = false;
                    //frame.Content = numLab;
                    //G.Children.Add(frame);
                    SD.Children.Add(G);

                    Button button = new Button
                    {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                    };
                    button.Clicked += selectTask;
                    button.BindingContext = model;
                    G.Children.Add(button);

                }
                
            }
        }


        private class TaskModel{
            public string name { get; set; }
            public string num { get; set; }
            public string type { get; set; }
            public string factor { get; set; }
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            //测试用
            Navigation.PushAsync(new TastInfoPage("饮用水采样2", "328e5ea3-624d-40e2-a4b1-2300b51a7114"));
        }
    }
}
