using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class DisposeEventTypeTowPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            Task task = e.SelectedItem as Task;
            if (task == null) return;
            Navigation.PushAsync(new TaskInfoPage());
            listV.SelectedItem = null;
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {

        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }

        private ObservableCollection<Task> taskList = new ObservableCollection<Task>();

        public DisposeEventTypeTowPage()
        {
            InitializeComponent();

            taskList.Add(new Task
            {
                name = "在厂房周围监测水质",
                address = "村级网络，四官旺村",
            });
            taskList.Add(new Task
            {
                name = "在厂房周围监测水质",
                address = "村级网络，四官旺村",
            });
            taskList.Add(new Task
            {
                name = "在厂房周围监测水质",
                address = "村级网络，四官旺村",
            });
            listV.ItemsSource = taskList;

        }




        private class Task{
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
        }

    }
}
