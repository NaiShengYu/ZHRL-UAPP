using AepApp.Models;
using CloudWTO.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAccidentInfoPage : ContentPage
    {

        bool isSelectText = false;
        bool isSelectImage = false;
        bool isSelectData = false;
        bool isSelectReport = false;
        //选中文字图标
        void selectText(object sender, System.EventArgs e){
            isSelectText = !isSelectText;
            var but = sender as Button;
            if (isSelectText == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }
        //选中图片图标
        void selectImage(object sender, System.EventArgs e)
        {
            isSelectImage = !isSelectImage;
            var but = sender as Button;
            if (isSelectImage == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }
        //选中数据图标
        void selectData(object sender, System.EventArgs e)
        {
            isSelectData = !isSelectData;
            var but = sender as Button;
            if (isSelectData == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }
        //选中报告图标
        void selectReport(object sender, System.EventArgs e)
        {
            isSelectReport = !isSelectReport;
            var but = sender as Button;
            if (isSelectReport == true)
                but.BackgroundColor = Color.FromRgba(0, 0, 0, 0.2);
            else
                but.BackgroundColor = Color.Transparent;
        }

        //添加事故
        void addSouce(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddEmergencyAccidentInfoPage());
        }





        void Handle_Clicked(object sender, System.EventArgs e)
        {

            var but = sender as Button;
            var item = but.BindingContext as item;

            appearList.Clear();

            listView.ScrollTo(item, ScrollToPosition.Start, true);
        }

        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {

            var item = e.Item as item;

            try
            {
                TimeSpan time1 = appearList[appearList.Count - 1].time.Subtract(item.time).Duration();
                var timelong = time1.Seconds;

                int a = dataList.IndexOf(item);
                int b = dataList.IndexOf(appearList[appearList.Count - 1]);

                if (a < b)
                {
                    appearList.Insert(0, item);

                }
                else
                {
                    appearList.Add(item);
                }
                showCurrentItems();

            }
            catch (Exception ex)
            {
                appearList.Add(item);
                showCurrentItems();

            }

        }

        void Handle_ItemDisappearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            var item = e.Item as item;
            appearList.Remove(item);
            showCurrentItems();
        }




        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            if (item.imgSourse.Length > 0 && item.imgSourse != null)
            {
                List<string> imgs = new List<string>();
                imgs.Add("https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg");
                imgs.Add("https://ss0.bdstatic.com/70cFvHSh_Q1YnxGkpoWK1HF6hhy/it/u=2738969446,4147851924&fm=27&gp=0.jpg");
                imgs.Add("https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg");

                Navigation.PushAsync(new BrowseImagesPage(imgs));
            }


            listView.SelectedItem = null;
        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();
        List<item> appearList = new List<item>();
        public EmergencyAccidentInfoPage(string name,string id)
        {
            InitializeComponent();
            this.Title = name;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            ReqEmergencyAccidentDetail(id);


            //var item1 = new item
            //{
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "",
            //    info = "先帝创业未半而中道崩殂，今天下三分，益州疲弊，此诚危急存亡之秋也。然侍卫之臣不懈于内，忠志之士忘身于外者，盖追先帝之殊遇，欲报之于陛下也。诚宜开张圣听，以光先帝遗德，恢弘志士之气，不宜妄自菲薄，引喻失义，以塞忠谏之路也。",
            //    address = "121.123455,29.222222",
            //    isShowAddress = true,

            //    time = Convert.ToDateTime("2018-03-16 16:51:46.310"),

            //};

            //dataList.Add(item1);

            //var item2 = new item
            //{
            //    address = "",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "",
            //    info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
            //    isShowAddress = false,
            //    time = Convert.ToDateTime("2018-03-19 16:51:46.310"),

            //};

            //dataList.Add(item2);

            //var item3 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
            //    info = "",
            //    isShowAddress = true,
            //    time = Convert.ToDateTime("2018-03-19 17:51:46.310"),
            //};

            //dataList.Add(item3);


            //var item4 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "",
            //    info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
            //    isShowAddress = false,
            //    time = Convert.ToDateTime("2018-03-19 18:06:46.310"),
            //};

            //dataList.Add(item4);

            //var item5 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
            //    info = "",
            //    isShowAddress = true,
            //    time = Convert.ToDateTime("2018-03-20 17:51:46.310"),
            //};

            //dataList.Add(item5);

            //var item7 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "",
            //    info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
            //    isShowAddress = false,
            //    time = Convert.ToDateTime("2018-04-9 17:51:46.310"),
            //};

            //dataList.Add(item7);

            //var item8 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
            //    info = "",
            //    isShowAddress = true,
            //    time = Convert.ToDateTime("2018-05-19 17:51:46.310"),
            //};

            //dataList.Add(item8);

            //var item9 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "",
            //    info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
            //    isShowAddress = false,
            //    time = Convert.ToDateTime("2018-05-19 22:51:46.310"),
            //};

            //dataList.Add(item9);

            //var item10 = new item
            //{
            //    address = "121.123455,29.222222",
            //    timeAndName = "2018/05/28 11:16/张三",
            //    imgSourse = "",
            //    info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
            //    isShowAddress = false,
            //    time = Convert.ToDateTime("2018-05-19 22:55:46.310"),
            //};

            //dataList.Add(item10);

            //listView.ItemsSource = dataList;

            //listView滑动到第七个item
            //listView.ScrollTo(item7,ScrollToPosition.Start,true);
            //scroll.scr

            creatScrollerView();
        }

        private async void ReqEmergencyAccidentDetail(string id)
        {
            string url = App.BaseUrlForYINGJI + DetailUrl.GetEmergencyDetail +
                   "?Id=" + id;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                //start += 10;
                //EmergencyAccidentPageModels.EmergencyAccidentBean accidentPageModels = new EmergencyAccidentPageModels.EmergencyAccidentBean();
                //accidentPageModels = JsonConvert.DeserializeObject<EmergencyAccidentPageModels.EmergencyAccidentBean>(hTTPResponse.Results);
                //totalNum = accidentPageModels.result.incidents.totalCount;
                //List<EmergencyAccidentPageModels.ItemsBean> list = accidentPageModels.result.incidents.items;
                //int count = list.Count;
                //for (int i = 0; i < count; i++)
                //{
                //    dataList.Add(list[i]);
                //}
                //listView.ItemsSource = dataList;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }


        void creatScrollerView(){

            DateTime lastTime = new DateTime();
            for (int i = 0; i < dataList.Count; i++)
            {
                var item = dataList[i];
                if (i == 0)
                {
                    lastTime = item.time;
                }

                TimeSpan time1 = item.time.Subtract(lastTime).Duration();
                double timeLong = time1.TotalHours;

                BoxView box = new BoxView
                {
                    HeightRequest = timeLong,
                };

                BoxView box1 = new BoxView
                {
                    BackgroundColor = Color.Gray,
                    Margin = new Thickness(5, 0, 5, 0),
                    HeightRequest = 8,
                    WidthRequest = 30,
                };
                box1.BindingContext = item;

                scrollLayout.Children.Add(box);
                scrollLayout.Children.Add(box1);
                lastTime = item.time;
            }

        }

        void showCurrentItems()
        {
            if (appearList.Count > 0)
            {
                var firstItem = appearList[0];
                var lastItem = appearList[appearList.Count - 1];
                TimeSpan time1 = firstItem.time.Subtract(lastItem.time).Duration();
                double timeLong = time1.TotalHours;
                positionView.HeightRequest = 10 * appearList.Count + timeLong - 2;

                //循环出当前第一个item在原数组排第几位
                var item = dataList[0];
                int a = dataList.IndexOf(firstItem);

                //postiongView 开始位置
                TimeSpan time2 = firstItem.time.Subtract(item.time).Duration();
                double timeLong1 = time2.TotalHours;
                positionView.Margin = new Thickness(0, 10 * a + timeLong1 + 1, 0, 0);

                scroll.ScrollToAsync(0, 10 * a + timeLong1 + 1, true);

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
    }
}
