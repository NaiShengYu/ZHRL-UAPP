using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class TaskInfoPage : ContentPage
    {
        void Handle_Tapped(object sender, System.EventArgs e)
        {

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            position position = e.SelectedItem as position;
            if (position == null) return;



            listV.SelectedItem = null;
        }

        void AssignPerson(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AssignPersonTypeTowPage());
        }

        void RegistrationEvent(object sender, System.EventArgs e){
            Navigation.PushAsync(new RegistrationEventPage(new GridEventModel()));
        }
        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }

        private ObservableCollection<position> addressList = new ObservableCollection<position>();

        public TaskInfoPage()
        {
            InitializeComponent();

            addressList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count +1,
            });

            addressList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count +1,
            });
            addressList.Add(new position
            {
                name = "江南路",
                address = "121.98768 E,29.49247N",
                num = addressList.Count +1,
            });

            BindingContext = addressList;
            listV.ItemsSource = addressList;
        }


        private class position{
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
            public int num
            {
                get;
                set;
   
            
            }
        }
    }
}
