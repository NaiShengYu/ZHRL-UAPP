using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class RelatedEnterprisesPage : ContentPage
    {
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
           
        }

        private ObservableCollection<enterPrises> dataList = new ObservableCollection<enterPrises>();
        public RelatedEnterprisesPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            enterPrises enter1 = new enterPrises
            {
                name = "太医石油化工有限公司",
                address = "邻水县高桥镇李家大山",
            };
            dataList.Add(enter1);

            dataList.Add(new enterPrises
            {
                name = "中龙塑料制品有限公司",
                address = "高新区研究园",
            });

            listView.ItemsSource = dataList;

        }

        private class enterPrises{
            public string name { get; set; }
            public string address { get; set; }
        }

    }
}
