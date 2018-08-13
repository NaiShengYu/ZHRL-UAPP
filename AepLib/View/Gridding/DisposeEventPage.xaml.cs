
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class DisposeEventPage : ContentPage
    {

        void RegistrationEvent(object sender,System.EventArgs e){
            Navigation.PushAsync(new RegistrationEventPage());

        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }

        public DisposeEventPage()
        {
            InitializeComponent();
        }
    }
}
