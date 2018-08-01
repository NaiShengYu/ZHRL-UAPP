using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class ScanningPage : ContentPage
    {
        

        void Handle_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Console.Write("扫描结果：" + result.Text + "----");
                await Navigation.PopAsync();
                MessagingCenter.Send<ContentPage, string>(this, "ScanningResult", result.Text);
            });
           
        }

        public ScanningPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
           

        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ZXING.IsScanning = false;

        }

    }
}
