using AepApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

//添加应急任务
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAddTaskPage : ContentPage
    {
        private List<SampleExamineItem> examineItems = new List<SampleExamineItem>();
        private ObservableCollection<SampleExamineItem> datas = new ObservableCollection<SampleExamineItem>();
        public EmergencyAddTaskPage()
        {
            InitializeComponent();
            pickerType.SelectedIndex = 0;
        }

        //添加项目
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SampleItemsListPage());
            MessagingCenter.Unsubscribe<ContentPage, SampleExamineItem>(this, "selectItem");
            MessagingCenter.Subscribe<ContentPage, SampleExamineItem>(this, "selectItem", async (arg1, arg2) =>
            {
                var item = arg2 as SampleExamineItem;
                bool has = false;
                foreach (var s in examineItems)
                {
                    if (s.id == item.id)
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    examineItems.Add(item);
                }
                datas = new ObservableCollection<SampleExamineItem>(examineItems);
                LvItems.ItemsSource = datas;
                LbNum.Text = examineItems.Count + "";
            });
        }

        private void pickerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;

        }

        private void Del_Clicked(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            SampleExamineItem s = item.BindingContext as SampleExamineItem;
            examineItems.Remove(s);
            datas.Remove(s);
            LbNum.Text = examineItems.Count + "";
        }
    }
}