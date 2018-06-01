using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class SuccessCase : ContentPage
    {
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //seach.Text = e.NewTextValue;

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as item;
            if (item == null)
                return;

            listView.SelectedItem = null;

        }

        ObservableCollection<item> dataList = new ObservableCollection<item>();

        public SuccessCase()
        {
            InitializeComponent();

            var tap = new TapGestureRecognizer();
            tap.Tapped +=(s,e) =>{



            };




            var item1 = new item
            {
                imgSourse = "https://ss1.bdstatic.com/70cFvXSh_Q1YnxGkpoWK1HF6hhy/it/u=729412813,2297218092&fm=27&gp=0.jpg",
                info = "丙烯氰污染事故及处理案例",
            };

            dataList.Add(item1);

            var item2 = new item
            {
                imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=331890373,3824021971&fm=27&gp=0.jpg",
                info = "六百余桶危险废物偷弃菜州",
            };

            dataList.Add(item2);

            var item3 = new item
            {
                imgSourse = "https://ss2.bdstatic.com/70cFvnSh_Q1YnxGkpoWK1HF6hhy/it/u=1851366601,1588844299&fm=27&gp=0.jpg",
                info = "厉害了，sdfa",
            };

            dataList.Add(item3);

            listView.ItemsSource = dataList;
        }

        internal class item
        {
            public string imgSourse { get; set; }
            public string info { set; get; }

        }

    }
}
