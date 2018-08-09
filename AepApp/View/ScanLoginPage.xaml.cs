using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	public partial class ScanLoginPage : ContentPage
	{
        void Handle_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("扫描结果：", result.Text, "确定");
                //Console.Write("扫描结果：" + result.Text + "----");
                //await Navigation.PopAsync();
                //MessagingCenter.Send<ContentPage, string>(this, "ScanningResult", result.Text);
            });

        }

        public ScanLoginPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //LoginZxing.IsScanning = false;

        }

    }
}