using System;
using System.Collections.Generic;

using Xamarin.Forms;
using static AepApp.Models.EmergencyAccidentInfoDetail;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class BrowseImagesPage : ContentPage
    {


        double stackLastLeftMargin = 0;
        bool isLeft = false;

        void Handle_swipeStop(object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            var img = sender as ZoomImage;

            double aaa = stack.Margin.Left % App.ScreenWidth;
            double bbb = stack.Margin.Left - aaa;
            double ccc = bbb / App.ScreenWidth;
            Console.WriteLine("图片scale=" + img.Scale + "---页面1/3宽度" + -App.ScreenWidth / 3 + "----页面偏移量==" + aaa + "页面2/3宽度" + -App.ScreenWidth / 3 * 2);

            if (isLeft == true)
            {//向左滑动
                if (aaa > -App.ScreenWidth / 3)
                {
                    stack.Margin = new Thickness(bbb + 10 * ccc, 0, 0, 0);
                }
                else
                {
                    stack.Margin = new Thickness(bbb - App.ScreenWidth + 10 * (ccc - 1), 0, 0, 0);
                }
            }
            if (isLeft == false)
            {//向右滑动
                if (aaa < -App.ScreenWidth / 3 * 2)//向右滑动不超过屏幕的三分之一返回当前页
                {
                    stack.Margin = new Thickness(bbb - App.ScreenWidth + 10 * (ccc - 1), 0, 0, 0);
                }
                else
                {
                    stack.Margin = new Thickness(bbb + 10 * ccc, 0, 0, 0);
                }
            }

            stackLastLeftMargin = stack.Margin.Left;

        }

        void Handle_swipeRunning(object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            //判断左滑还是右划
            if (e.TotalX < 0) isLeft = true;
            if (e.TotalX > 0) isLeft = false;
            //第一张图
            if (stack.Margin.Left >= 0 && e.TotalX > 0)
            {
                stack.Margin = new Thickness(0, 0, 0, 0);
                return;
            }

            //最后一张图
            if (stack.Margin.Left <= -stack.WidthRequest + App.ScreenWidth && e.TotalX < 0)
            {

                stack.Margin = new Thickness(stack.Margin.Left, 0, 0, 0);
                return;
            }
            stack.Margin = new Thickness(stackLastLeftMargin + e.TotalX, 0, 0, 0);
        }

        public BrowseImagesPage(List<IncidentLoggingEventsBean> imgsSouce):this()
        {
            InitializeComponent();
            titleLab.IsVisible = true;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            ToolbarItems.Add(new ToolbarItem("", "map", () =>
            {
                //Navigation.PushAsync(new PollutionSourceMapPage(dataList));

            }));

            stack.WidthRequest = imgsSouce.Count * App.ScreenWidth + 20;
            for (int i = 0; i < imgsSouce.Count; i++)
            {
                ZoomImage img = new ZoomImage
                {
                    Source = imgsSouce[i].imagePath,
                    WidthRequest = App.ScreenWidth,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                img.swipeRunning += Handle_swipeRunning;

                img.swipeStop += Handle_swipeStop;

                stack.Children.Add(img);


                string time = string.Format("{0:yyyy-MM-dd HH:mm:ss}", imgsSouce[i].creationTime);

                titleLab.Text = time + "/" + imgsSouce[i].creatorUserName;

            }
        }

        public BrowseImagesPage(){
        
        
        
        
        }


        public BrowseImagesPage(List<string> imgsSouce):this()
        {
            InitializeComponent();
            stack.WidthRequest = imgsSouce.Count * App.ScreenWidth;

            for (int i = 0; i < imgsSouce.Count; i++)
            {
                ZoomImage img = new ZoomImage
                {
                    Source = imgsSouce[i],
                    WidthRequest = App.ScreenWidth,
                };
                img.swipeRunning += Handle_swipeRunning;

                img.swipeStop += Handle_swipeStop;

                stack.Children.Add(img);

            }
        }



        //#pragma mark --取消masterDeftail的返回手势
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.appHunbegerPage.IsGestureEnabled = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.appHunbegerPage.IsGestureEnabled = true;
        }


    }
}
