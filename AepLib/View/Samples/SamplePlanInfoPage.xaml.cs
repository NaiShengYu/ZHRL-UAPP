using AepApp.Models;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class SamplePlanInfoPage : ContentPage
    {

        private ObservableCollection<TaskModel> dataList = new ObservableCollection<TaskModel>();
        MySamplePlanItems _samplePlanItems = null;

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
            TasksList model = button.BindingContext as TasksList;
            Navigation.PushAsync(new TastInfoPage(_samplePlanItems, model));
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
            if (button.StyleId == "dispose")
            {
                Console.WriteLine("预处理信息");
                message = _samplePlanItems.pretreatment;
                pagetitle = "预处理信息";
            }
            if (button.StyleId == "security")
            {
                Console.WriteLine("安全说明");
                message = _samplePlanItems.security;
                pagetitle = "安全说明";
            }
            if (button.StyleId == "remake")
            {
                Console.WriteLine("备注信息");
                message = _samplePlanItems.remarks;
                pagetitle = "备注信息";
            }
            if (button.StyleId == "quality")
            {
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
        }


        private async void requestSamplePublicData()
        {

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SampleURL + "/Api/WaterRecord/GetDetailByPid?planid=" + _samplePlanItems.id, "", "GET", "");
            //HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SamplingModule.url + "/Api/WaterRecord/GetDetailByPid?planid=" + _samplePlanItems.id, "", "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                hTTPResponse.Results = hTTPResponse.Results.Replace("id", "basicDataModelID");
                SampleBasicDataModel basicDataModel = JsonConvert.DeserializeObject<SampleBasicDataModel>(hTTPResponse.Results);
                Console.WriteLine("结果是：" + App.mySamplePlanResult);
                if(basicDataModel !=null)
                    _samplePlanItems.basicDataModel = basicDataModel;
            }
           
            basicDataSK.BindingContext = _samplePlanItems.basicDataModel;
        }

        void Handle_purposeChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.purpose = e.NewTextValue;
        }
        void Handle_areanameChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.areaname = e.NewTextValue;
        }
        void Handle_areafunctypeChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.areafunctype = e.NewTextValue;
        }
        void Handle_carshipChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.carship = e.NewTextValue;
        }
        void Handle_toolsChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.tools = e.NewTextValue;
        }
        void Handle_positionChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.position = e.NewTextValue;
        }

        void Handle_sampledateChanged(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.sampledate = e.NewDate;

        }
        void Handle_weatherChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.weather = e.NewTextValue;
        }
        void Handle_temperatureChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _samplePlanItems.basicDataModel.temperature = e.NewTextValue;
        }

        async void SaveBaiscData(object sender, EventArgs e)
        {

            if (netGrid.IsVisible == false)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string url = "/Api/WaterRecord/Add";
                dic.Add("areafunctype", _samplePlanItems.basicDataModel.areafunctype);
                dic.Add("areaname", _samplePlanItems.basicDataModel.areaname);
                dic.Add("carship", _samplePlanItems.basicDataModel.carship);
                dic.Add("planid", _samplePlanItems.id);
                dic.Add("position", _samplePlanItems.basicDataModel.position);
                dic.Add("purpose", _samplePlanItems.basicDataModel.purpose);
                dic.Add("sampledate", _samplePlanItems.basicDataModel.sampledate);
                dic.Add("temperature", _samplePlanItems.basicDataModel.temperature);
                dic.Add("tools", _samplePlanItems.basicDataModel.tools);
                dic.Add("weather", _samplePlanItems.basicDataModel.weather);
                //假如有信息id就更新id
                if (!string.IsNullOrWhiteSpace(_samplePlanItems.basicDataModel.basicDataModelID))
                {
                    dic.Add("id", _samplePlanItems.basicDataModel.basicDataModelID);
                    url = "/Api/WaterRecord/Update";
                }
                string param = JsonConvert.SerializeObject(dic);

                HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SampleURL + url, param, "POST", "");
                //HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SamplingModule.url + url, param, "POST", "");
                Console.WriteLine(hTTPResponse);
                if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DependencyService.Get<Sample.IToast>().ShortAlert("修改成功");
                    if (string.IsNullOrWhiteSpace(_samplePlanItems.basicDataModel.basicDataModelID)){
                        string result = JsonConvert.DeserializeObject<string>(hTTPResponse.Results);
                        _samplePlanItems.basicDataModel.basicDataModelID = result;

                    }
                }
                else
                {
                    Console.WriteLine(hTTPResponse);
                }
            }
                //如果没有网就存起来
                App.vm.saveDataWithModel(_samplePlanItems);

        }



        void creatTask()
        {
            if (_samplePlanItems == null || _samplePlanItems.tasklist == null)
            {
                return;
            }
            foreach (TasksList model in _samplePlanItems.tasklist)
            {

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
                    Font = Font.SystemFontOfSize(14),
                    TextColor = Color.Gray,
                };
                sk2.Children.Add(factorLab);
                foreach (TasksAnas anas in model.taskAnas)
                {
                    factorLab.Text += anas.atname + " ";
                }
                Frame frame = new Frame
                {
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 25,
                    HeightRequest = 25,
                    CornerRadius = (float)12.5,
                    BackgroundColor = Color.FromRgb(190, 190, 190),
                    Margin = new Thickness(0, 0, 15, 0),
                    Padding = new Thickness(0, 0, 0, 0),
                    IsClippedToBounds = true,
                };
                Label numLab = new Label
                {
                    Text = model.samplecount,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.White,

                };
                if (model.samplecount == "0") frame.IsVisible = false;
                frame.Content = numLab;
                G.Children.Add(frame);
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


        private class TaskModel
        {
            public string name { get; set; }
            public string num { get; set; }
            public string type { get; set; }
            public string factor { get; set; }
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {

        }
    }
}
