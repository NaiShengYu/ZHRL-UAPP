
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class DisposeEventPage : ContentPage
    {

        void RegistrationEvent(object sender,System.EventArgs e){
            Navigation.PushAsync(new RegistrationEventPage(null));
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }

        public DisposeEventPage()
        {
            InitializeComponent();
            gridWorker.GestureRecognizers.Add(new TapGestureRecognizer {
                Command  = new Command(()=>OnWorkersTapped()),
            });
        }

        private void OnWorkersTapped()
        {
            Navigation.PushAsync(new SelectGridWorkerPage());
        }

        public async void OnWorkerCall(Object sender, EventArgs e)
        {
            
        }
    }
}
