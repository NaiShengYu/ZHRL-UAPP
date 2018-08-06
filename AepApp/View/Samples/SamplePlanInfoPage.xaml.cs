using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

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
            //Navigation.PushAsync(new SamplePlanMapPage());

        }

        /// <summary>
        /// 选中某一个任务
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void selectTask(object sender, EventArgs e)
        {
            Button button = sender as Button;
            TaskModel model = button.BindingContext as TaskModel;
            Navigation.PushAsync(new TastInfoPage
            {
                Title = model.name,
            });

        }


        /// <summary>
        /// Handles the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_Clicked(object sender, System.EventArgs e)
        {

        }

        private ObservableCollection<TaskModel> dataList = new ObservableCollection<TaskModel>();

        public SamplePlanInfoPage(CollectionAndTransportSampleModel sampleModel)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = sampleModel.time;
            this.BindingContext = sampleModel;

            TaskModel model1 = new TaskModel
            {
                name = "任务一",
                num = "2",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            TaskModel model2 = new TaskModel
            {
                name = "任务二",
                num = "1",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            TaskModel model3 = new TaskModel
            {
                name = "任务三",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            TaskModel model4 = new TaskModel
            {
                name = "任务四",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            TaskModel model5 = new TaskModel
            {
                name = "任务五",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            dataList.Add(model1);

            TaskModel model6 = new TaskModel
            {
                name = "任务五",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            dataList.Add(model6);
            TaskModel model8 = new TaskModel
            {
                name = "任务五",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            dataList.Add(model8);
            TaskModel model7 = new TaskModel
            {
                name = "任务五",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            dataList.Add(model7);
            TaskModel model9 = new TaskModel
            {
                name = "任务五",
                num = "0",
                type = "地表水",
                factor = "重金属，苯系物",
            };
            dataList.Add(model9);

            dataList.Add(model1);
            dataList.Add(model2);
            dataList.Add(model3);
            dataList.Add(model4);
            dataList.Add(model5);

            creatTask();
        }



        void creatTask()
        {
            
            foreach(TaskModel model in dataList){

                Grid G = new Grid{
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
                    Text = model.name,
                    Font = Font.SystemFontOfSize(17),
                };

                Grid typeGird = new Grid
                {
                    BackgroundColor = Color.FromRgb(59, 125, 169),
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions =LayoutOptions.Start,
                    HeightRequest = 24,
                };

                Label typeLab = new Label
                {
                    Margin = new Thickness(2,2,2,2),
                    Text = model.type,
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
                    Text = model.factor,
                    Font = Font.SystemFontOfSize(14),
                    TextColor =Color.Gray,
                };
                sk2.Children.Add(factorLab);

                Frame frame = new Frame
                {
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 25,
                    HeightRequest = 25,
                    CornerRadius = (float)12.5 ,
                    BackgroundColor = Color.FromRgb(190, 190, 190),
                    Margin = new Thickness(0,0,15,0),
                    Padding = new Thickness(0,0,0,0),
                    IsClippedToBounds = true,
                };
                Label numLab = new Label
                {
                    Text = model.num,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.White,

                };
                if (model.num == "0") frame.IsVisible = false;
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


        private class TaskModel{
            public string name { get; set; }
            public string num { get; set; }
            public string type { get; set; }
            public string factor { get; set; }
        }

    }
}
