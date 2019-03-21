using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddPlacementPage : ContentPage
    {
        ObservableCollection<string> _person = new ObservableCollection<string>();
        ObservableCollection<string> _equipment = new ObservableCollection<string>();
        ObservableCollection<string> _task = new ObservableCollection<string>();



        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }
        void editContent(object sender, System.EventArgs e)
        {

        }
        //添加相关人员
        void Handle_AddPerson(object sender, System.EventArgs e)
        {
            _person.Add("张三");
        }
        void Handle_DeletPerson(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _person.Remove(item.BindingContext as string);
        }
        //添加相关设备
        void Handle_AddEquipment(object sender, System.EventArgs e)
        {
            _equipment.Add("瓶式深水采样器");
        }
        void Handle_DeletEquipment(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _equipment.Remove(item.BindingContext as string);
        }
        void Handle_AddTask(object sender, System.EventArgs e)
        {
            _task.Add("瓶式深水采样器");
        }
        void Handle_DeletTask(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _task.Remove(item.BindingContext as string);
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {

        }


        public AddPlacementPage()
        {
            InitializeComponent();

            personLV.ItemsSource = _person;
            PersonNumFrame.BindingContext = _person;
            equipmentLV.ItemsSource
             = _equipment;
            equipmentNumFrame.BindingContext = _equipment;
            taskLV.ItemsSource = _task;
            taskNumFrame.BindingContext = _task;
        }



     
    }
}
