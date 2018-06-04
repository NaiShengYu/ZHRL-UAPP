using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
#if __IOS__
using Foundation;
using UIKit;
using CoreGraphics;
#endif
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddEmergencyAccidentInfoPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;
            listView.SelectedItem = null;

        }

#if __IOS__
        void HandleAction(NSNotification obj)
        {

            var dic = obj.UserInfo as NSMutableDictionary;
            var rc = dic.ValueForKey((Foundation.NSString)"UIKeyboardFrameEndUserInfoKey");
            CGRect r = (rc as NSValue).CGRectValue;
            entryStack.TranslateTo(0, 206 - r.Size.Height);

        }
#endif
        //编辑结束
        void Handle_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            entryStack.TranslateTo(0, 0);

        }

        //编辑开始
        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {

            //ENT.TranslateTo(0, 100);

            var entr = sender as Entry;
        }

        void AccidentPosition(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AccidentPositionPage());

        }
        //左滑删除
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var menu = sender as MenuItem;
            var item = menu.BindingContext as item;

            dataList.Remove(item);

        }

        //点击了数据按钮

        void addShuju(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new addDataPage());

        }



        ObservableCollection<item> dataList = new ObservableCollection<item>();
        public AddEmergencyAccidentInfoPage()
        {
            InitializeComponent();

#if __IOS__
            var not = NSNotificationCenter.DefaultCenter;
            not.AddObserver(UIKeyboard.WillChangeFrameNotification, HandleAction);

#endif
            var item1 = new item
            {
                timeAndName = "2018/05/28 11:16/张三",
                imgSourse = "",
                info = "先帝创业未半而中道崩殂，今天下三分，益州疲弊，此诚危急存亡之秋也。然侍卫之臣不懈于内，忠志之士忘身于外者，盖追先帝之殊遇，欲报之于陛下也。诚宜开张圣听，以光先帝遗德，恢弘志士之气，不宜妄自菲薄，引喻失义，以塞忠谏之路也。",
                address = "121.123455,29.222222",
                isShowAddress = true,

                time = Convert.ToDateTime("2018-03-16 16:51:46.310"),

            };

            dataList.Add(item1);

            var item2 = new item
            {
                address = "",
                timeAndName = "2018/05/28 11:16/张三",
                imgSourse = "",
                info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
                isShowAddress = false,
                time = Convert.ToDateTime("2018-03-19 16:51:46.310"),

            };

            dataList.Add(item2);

            var item3 = new item
            {
                address = "121.123455,29.222222",
                timeAndName = "2018/05/28 11:16/张三",
                imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
                info = "",
                isShowAddress = true,
                time = Convert.ToDateTime("2018-03-19 17:51:46.310"),
            };

            dataList.Add(item3);


            var item4 = new item
            {
                address = "121.123455,29.222222",
                timeAndName = "2018/05/28 11:16/张三",
                imgSourse = "",
                info = "宫中府中，俱为一体，陟罚臧否，不宜异同。若有作奸犯科及为忠善者，宜付有司论其刑赏，以昭陛下平明之理，不宜偏私，使内外异法也。",
                isShowAddress = false,
                time = Convert.ToDateTime("2018-03-19 18:06:46.310"),
            };

            dataList.Add(item4);


            listView.ItemsSource = dataList;




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
