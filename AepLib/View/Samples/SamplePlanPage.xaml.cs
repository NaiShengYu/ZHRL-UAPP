using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;
using Newtonsoft.Json;
using CloudWTO.Services;
using Xamarin.Essentials;
using AepApp.AuxiliaryExtension;

namespace AepApp.View.Samples
{
    public partial class SamplePlanPage : ContentPage
    {
        //加一天
        void addADay(object sender, System.EventArgs e)
        {
            //currentDay = currentDay.AddDays(1.0);           
            //timeLab.Text = currentDay.ToString("yyyy-MM-dd");

            //requestSamplePlanList();
            App.vm.CurrentDay = App.vm.CurrentDay.AddDays(1);
            //App.vm.requestSamplePlanList();


        }
        //减一天
        void reduceADay(object sender, System.EventArgs e)
        {
            //currentDay = currentDay.AddDays(-1.0);
            //timeLab.Text = currentDay.ToString("yyyy-MM-dd");

            App.vm.CurrentDay = App.vm.CurrentDay.AddDays(-1);
            //App.vm.requestSamplePlanList();
        }
          
        /// <summary>
        /// 选择日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectDay(object sender, DateChangedEventArgs e)
        {
            App.vm.CurrentDay = e.NewDate;
            App.vm.requestSamplePlanList();
        }

        /// <summary>
        /// 选中了item
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            MySamplePlanItems item = e.SelectedItem as MySamplePlanItems;
            item = App.vm.selectSamplePlanWithItem(item);
            Navigation.PushAsync(new SamplePlanInfoPage(item));
            listView.SelectedItem = null;
        }
        //private ObservableCollection<CollectionAndTransportSampleModel> dataList = new ObservableCollection<CollectionAndTransportSampleModel>();
        //private DateTime currentDay = DateTime.Now;
        public SamplePlanPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            //timeLab.Text = currentDay.ToString("yyyy-MM-dd");
            App.vm.CurrentDay = DateTime.Now;
            App.vm.requestSamplePlanList();
            BindingContext = App.vm;
        }

    

      


    }
}
