using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;
using AepApp.View.EnvironmentalEmergency;
namespace AepApp.View.Samples
{
    public partial class SamplePlanInfoPage : ContentPage
    {



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

        private ObservableCollection<TaskModel> dataList = new ObservableCollection<TaskModel>();
        MySamplePlanItems _samplePlanItems = null;
        public SamplePlanInfoPage(MySamplePlanItems sampleModel)
        {
            InitializeComponent();
            _samplePlanItems = sampleModel;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = sampleModel.name;
            this.BindingContext = sampleModel;


            creatTask();
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

    }
}
