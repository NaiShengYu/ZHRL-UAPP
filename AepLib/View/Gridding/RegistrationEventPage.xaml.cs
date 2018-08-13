﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.View.EnvironmentalEmergency;
using Plugin.Media;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class RegistrationEventPage : ContentPage
    {

        private ObservableCollection<string> photoList = new ObservableCollection<string>();

        void EventPositon(object sender, System.EventArgs e){
            AccidentPositionPage page = new AccidentPositionPage();
            Navigation.PushAsync(page);
        }

        //相关企业
        void RelatedEnterPrises(object sender, System.EventArgs e){

            Navigation.PushAsync(new RelatedEnterprisesPage());
        }


        //拍照
        async void takePhoto(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                CompressionQuality = 50,
                Directory = "Gridding",
                Name = System.DateTime.Now + ".jpg"
            });

            if (file == null)
            {
                return;
            }

            else
            {

                photoList.Add(file.Path);

                creatPhotoView();


            }


        }

        /// <summary>
        /// 根据拍照张数创建图片
        /// </summary>
        void creatPhotoView()
        {

            PickSK.Children.Clear();

            foreach (string path in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source = ImageSource.FromFile(path) as FileImageSource,
                    HeightRequest = 80,
                    WidthRequest = 80,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.Fill,
                };
                grid.Children.Add(button);

                if (100.0 * photoList.Count > App.ScreenWidth)
                    pickSCR.ScrollToAsync(100 * photoList.Count - (App.ScreenWidth), 0, true);
                

                

                //Image = new Image
                //{
                //    VerticalOptions = LayoutOptions.Center,
                //    HorizontalOptions = LayoutOptions.Start,
                //    Aspect =Aspect.Fill,


                //};


            }



        }


        void Handle_Clicked(object sender, System.EventArgs e)
        {

        
        }

        public RegistrationEventPage()
        {
            InitializeComponent();

            Title = "登记事件";
            NavigationPage.SetBackButtonTitle(this,"");
            ToolbarItems.Add(new ToolbarItem("", "qrcode", HandleAction));
            ST.BindingContext = photoList;
        }

        void HandleAction()
        {
            Console.WriteLine("导航栏右按钮");
        }

       

    }
}
