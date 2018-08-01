using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;
namespace AepApp.View.Samples
{
    public partial class SamplePlanPage : ContentPage
    {


        /// <summary>
        /// 选中了item
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            CollectionAndTransportSampleModel item = e.SelectedItem as CollectionAndTransportSampleModel;
            Navigation.PushAsync(new SamplePlanInfoPage(item));
            listView.SelectedItem = null;
        }
        private ObservableCollection<CollectionAndTransportSampleModel> dataList = new ObservableCollection<CollectionAndTransportSampleModel>();

        public SamplePlanPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            var model1 = new CollectionAndTransportSampleModel
            {
                time = "计划1",
                num = "江南路1958号",
                type = "start",
                state = "B",
            };
            var model2 = new CollectionAndTransportSampleModel
            {
                time = "计划2",
                num = "梅墟路99号",
                type = "finish",
                state = "B",
            };
            var model3 = new CollectionAndTransportSampleModel
            {
                time = "计划3",
                num = "通途路44号",
                type = "finish",
                state = "A",
            };
       
            var model4 = new CollectionAndTransportSampleModel
            {
                time = "计划3",
                num = "通途路44号",
                type = "wait",
                state = "B",
            };
            dataList.Add(model1);
            dataList.Add(model2);
            dataList.Add(model3);
            dataList.Add(model4);
            listView.ItemsSource = dataList;

        }
    }
}
