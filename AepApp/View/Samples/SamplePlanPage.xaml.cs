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

        }
        private ObservableCollection<CollectionAndTransportSampleModel> dataList = new ObservableCollection<CollectionAndTransportSampleModel>();

        public SamplePlanPage()
        {
            InitializeComponent();


            var model1 = new CollectionAndTransportSampleModel
            {
                time = "2018-10-24",
                num = "5 采样瓶个",
                type = "start",
                state = "B",
            };
            var model2 = new CollectionAndTransportSampleModel
            {
                time = "2018-10-26",
                num = "6 采样瓶个",
                type = "finish",
                state = "B",
            };
            var model3 = new CollectionAndTransportSampleModel
            {
                time = "2018-10-27",
                num = "10 采样瓶个",
                type = "finish",
                state = "A",
            };

            dataList.Add(model1);
            dataList.Add(model2);
            dataList.Add(model3);
            listView.ItemsSource = dataList;

        }
    }
}
