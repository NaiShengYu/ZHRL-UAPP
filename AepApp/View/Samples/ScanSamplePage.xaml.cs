using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class ScanSamplePage : ContentPage
    {

        /// <summary>
        /// 送样备注
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SampleRemarkPage());

        
        }

        void Handle_Scrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
        {

            //Console.WriteLine(e.ScrollY);

            if (e.ScrollY <= App.ScreenHeight / 3) ZXING.IsAnalyzing = true;
            else ZXING.IsAnalyzing = false;

            //ZXING.IsAnalyzing

        }

        void Handle_OnScanResult(ZXing.Result result)
        {

            Console.Write("扫描结果：" + result);

        }


        private ObservableCollection<CollectionAndTransportSampleModel> dataList = new ObservableCollection<CollectionAndTransportSampleModel>();

        public ScanSamplePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "送样（接样）";
            ZXING.HeightRequest = App.ScreenHeight / 4;
            dataList.Add(new CollectionAndTransportSampleModel());
            dataList.Add(new CollectionAndTransportSampleModel());
            dataList.Add(new CollectionAndTransportSampleModel());
            dataList.Add(new CollectionAndTransportSampleModel());
            dataList.Add(new CollectionAndTransportSampleModel());
            dataList.Add(new CollectionAndTransportSampleModel());

            planView();



        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ZXING.IsScanning = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ZXING.IsScanning = true;
        }

        void planView(){

            foreach(CollectionAndTransportSampleModel model in dataList){

                Grid G1 = new Grid{
                    BackgroundColor = Color.White,
                };
                StackLayout SK1 = new StackLayout();
                G1.Children.Add(SK1);

                Label lab1 = new Label
                {
                    Text = "计划1",
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(10, 10, 100, 5),
                    FontSize = 17,
                };
                SK1.Children.Add(lab1);

                Label addressLab = new Label
                {
                    Text = "江南路1958号",
                    TextColor = Color.Gray,
                    Margin = new Thickness(10, 0, 100, 5),
                    FontSize = 15,
                };
                SK1.Children.Add(addressLab);

                Grid G2 = new Grid
                {
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 10, 10, 0),
                    WidthRequest = 50,
                    HeightRequest = 24,
                };
                G1.Children.Add(G2);

                Frame F1 = new Frame
                {
                    Padding = new Thickness(2, 2, 2, 2),
                    CornerRadius = 12,
                    IsClippedToBounds = true,
                    BackgroundColor = Color.Gray,
                };

                G2.Children.Add(F1);

                Label lab2 = new Label
                {
                    TextColor = Color.White,
                    FontSize = 15,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "1/3",
                };
                F1.Content = lab2;

                ScrollView scroll = new ScrollView
                {
                    HeightRequest = 85,
                    VerticalOptions = LayoutOptions.Fill,
                    //BackgroundColor = Color.Brown,
                    Orientation = ScrollOrientation.Horizontal,
                };

                StackLayout SK3 = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(10, 5, 10, 5),
                    //BackgroundColor = Color.Beige,
                };
                scroll.Content = SK3;
                SK1.Children.Add(scroll);

                foreach (CollectionAndTransportSampleModel model1 in dataList){

                    Button button = new Button
                    {
                        BackgroundColor = Color.Transparent,
                        VerticalOptions = LayoutOptions.Start,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        Margin = new Thickness(0, 0, 0, 0),
                        Image = ImageSource.FromFile("bottle") as FileImageSource,
                        HorizontalOptions = LayoutOptions.Center,
                    };

                    Label lab3 = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = 13,
                        Text = "24938291",
                        Margin = new Thickness(0, 0, 0, 0),
                        HorizontalTextAlignment = TextAlignment.Center,
                    };

                    Label lab4 = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = 12,
                        Text = "10.33",
                        Margin = new Thickness(0, 0, 0, 0),

                        HorizontalTextAlignment = TextAlignment.Center,
                    };

                    StackLayout layout = new StackLayout{
                        Spacing = 1,
                        //BackgroundColor = Color.Black
                    };
                    layout.Children.Add(button);
                    layout.Children.Add(lab3);
                    layout.Children.Add(lab4);

                    Grid grid = new Grid{
                        //BackgroundColor = Color.Blue,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                    };
                    grid.Children.Add(layout);

                    Button button1 = new Button
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        //BackgroundColor = Color.Orange,
                    };
                    grid.Children.Add(button1); 

                    SK3.Children.Add(grid);
                }


                planST.Children.Add(G1);



            }

           





        }







    }
}
