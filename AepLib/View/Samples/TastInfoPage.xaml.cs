using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plugin.Media;
using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class TastInfoPage : ContentPage
    {

        private ObservableCollection<string> samplingBottleList = new ObservableCollection<string>();
        private ObservableCollection<string> photoList = new ObservableCollection<string>();

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
                Directory = "Sample",
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
        /// 进入扫描界面
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
         void scanning(object sender, System.EventArgs e)
        {
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "ScanningResult");

             MessagingCenter.Subscribe<ContentPage, string>(this, "ScanningResult", async (arg1, arg2) =>
            {
                Console.WriteLine("采样瓶二维码结果：" + arg2);
                samplingBottleList.Add(arg2);
                creatSamplingBottle();
            });

            Navigation.PushAsync(new ScanningPage
            {
                Title = "扫描采样瓶",
            });

        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }

        public TastInfoPage()
        {
            InitializeComponent();


        }

        /// <summary>
        /// 扫码创建采样瓶
        /// </summary>
        void creatSamplingBottle(){

            sampleBottleSK.Children.Clear();
            foreach (string barCode in samplingBottleList)

            {
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
                        Text = barCode,
                    Margin = new Thickness(0, 0, 0, 0),
                    WidthRequest=65,
                    HorizontalTextAlignment = TextAlignment.Center,
                };


                StackLayout layout = new StackLayout
                {
                    Spacing = 1,
                    //BackgroundColor = Color.Black
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                layout.Children.Add(button);
                layout.Children.Add(lab3);

                Grid grid = new Grid
                {
                    //BackgroundColor = Color.Blue,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,

                };
                grid.Children.Add(layout);

                Button button1 = new Button
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    //BackgroundColor = Color.Orange,
                };
                grid.Children.Add(button1);
                sampleBottleSK.Children.Add(grid);
            }


        }

        /// <summary>
        /// 根据拍照张数创建图片
        /// </summary>
        void creatPhotoView(){

            PickSK.Children.Clear();

            foreach(string path in photoList){
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source =ImageSource.FromFile(path) as FileImageSource,
                    HeightRequest =80,
                    WidthRequest = 80,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.Fill,
                };
                grid.Children.Add(button);


                //Image = new Image
                //{
                //    VerticalOptions = LayoutOptions.Center,
                //    HorizontalOptions = LayoutOptions.Start,
                //    Aspect =Aspect.Fill,


                //};


            }
          


        }




    }
}
