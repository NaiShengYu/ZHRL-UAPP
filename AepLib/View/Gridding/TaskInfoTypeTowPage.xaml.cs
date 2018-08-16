using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class TaskInfoTypeTowPage : ContentPage
    {


        /// <summary>
        /// 执行记录
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void ExecutionRecord(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ExecutionRecordPage());
        }

        /// <summary>
        /// 编辑任务结果
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void taskResult(object sender,System.EventArgs e){
            Navigation.PushAsync(new TaskResultPage());
        }

        //选择任务性质
        void choiseNature(object sender, System.EventArgs e)
        {

        }
        private ObservableCollection<position> addressList = new ObservableCollection<position>();
        private ObservableCollection<position> enterpriseList = new ObservableCollection<position>();

        public TaskInfoTypeTowPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            enterpriseList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count + 1,
            });

            enterpriseList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count + 1,
            });
            enterpriseList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count + 1,
            });

            addressList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count + 1,
            });

            addressList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count + 1,
            });
            addressList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count + 1,
            });

            creatPositionList();
            creatEnterpriseList();

        }


        /// <summary>
        /// 相关企业列表
        /// </summary>
        void creatEnterpriseList(){
            foreach (var po in enterpriseList)
            {
                Grid G1 = new Grid
                {
                    //BackgroundColor = Color.Blue,
                };
                enterpriseSK.Children.Add(G1);

                Label label = new Label
                {
                    Margin = new Thickness(50, 10, 30, 10),
                    Text = po.name,
                    FontSize = 18,
                    VerticalOptions = LayoutOptions.Center,
                };

                Frame frame = new Frame
                {
                    CornerRadius = 15,
                    HeightRequest = 30,
                    WidthRequest = 30,
                    BackgroundColor = Color.Red,
                    Padding = new Thickness(0),
                    Margin = new Thickness(10,15,10,15),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    HasShadow = false,
                };
                Label numLab = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.White,
                    Text = po.num.ToString(),
                };
                frame.Content =numLab;
                Image image = new Image
                {
                    Source = ImageSource.FromFile("right"),
                    Margin = new Thickness(10),
                    HeightRequest = 20,
                    WidthRequest = 10,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.End,
                };

                BoxView box = new BoxView
                {
                    BackgroundColor = Color.Silver,
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                };
                G1.Children.Add(frame);
                G1.Children.Add(label);
                G1.Children.Add(image);
                G1.Children.Add(box);



            }




        }

        // 相关位置列表
        void creatPositionList(){

            foreach (var po in addressList)
            {
                Grid G1 = new Grid{
                    //BackgroundColor = Color.Blue,
                };
                positionSK.Children.Add(G1);


                StackLayout SK = new StackLayout
                {
                    Margin = new Thickness(50, 10, 30, 10),
                    Spacing = 2,
                };
                G1.Children.Add(SK);


                Label label = new Label
                {
                    Margin = new Thickness(0),
                    Text = po.name,
                    FontSize = 18,
                };
                Label label1 = new Label
                {
                    Margin = new Thickness(0),
                    Text = po.address,
                    FontSize = 16,
                    TextColor = Color.Gray,
                };
                SK.Children.Add(label);
                SK.Children.Add(label1);



                Frame frame = new Frame
                {
                    CornerRadius = 15,
                    HeightRequest = 30,
                    WidthRequest = 30,
                    BackgroundColor = Color.Red,
                    Padding = new Thickness(0),
                    Margin = new Thickness(10, 5, 10, 5),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    HasShadow = false,

                };
                Label numLab = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.White,
                    Text = po.num.ToString(),
                };
                frame.Content = numLab;

                BoxView box = new BoxView
                {
                    BackgroundColor = Color.Silver,
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                };
                G1.Children.Add(frame);
                G1.Children.Add(box);


            }



        }


        private class position
        {
            public string name
            {
                get;
                set;
            }
            public string address
            {
                get;
                set;
            }
            public int num
            {
                get;
                set;


            }
        }



    }
}
