﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace AepApp.View.SecondaryFunction
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRCodeScanner : ContentPage
    {
        void Handle_OnScanResult(ZXing.Result result)
        {
            Console.Write("扫描结果：" + result);

            ZXING.IsAnalyzing = false;


        }

        ZXingScannerView zxing;
        ZXingDefaultOverlay overlay;
        ZXingOverLayout overLayout;

        public QRCodeScanner() 
        {
            InitializeComponent();
            //int height = App.ScreenHeight / 2;
            //this.Title = "二维码扫描";
            //initZxing(height);
            //initOverLayout();
            //DefaultSet();

            //var grid = new Grid
            //{
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //};
            //grid.Children.Add(zxing);
            //grid.Children.Add(overLayout);
            //grid.Children.Add(overlay);      
            //// The root page of your application
            //Content = grid;

        }



        private void initOverLayout()
        {
            overLayout = new ZXingOverLayout();
        }

        private void initZxing(int height)
        {
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingScannerView",
            };
            zxing.Margin = new Thickness(0, -height + 10, 0, height - 10);
            zxing.OnScanResult += (result) =>
               Device.BeginInvokeOnMainThread(async () =>
               {

                   // Stop analysis until we navigate away so we don't keep reading barcodes
                   zxing.IsAnalyzing = false; //若为true则为一直扫面
                   
                   // Show an alert
                   //await DisplayAlert("Scanned Barcode", result.Text, "OK");
                   overlay.TopText = result.Text;
                   //DependencyService.Get<Sample.IToast>().ShortAlert(result.Text);
                   overLayout.setMessage(result.Text);
                   // Navigate away
                   await Navigation.PopAsync();
               });
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();

            //zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            //zxing.IsScanning = false;

            //base.OnDisappearing();
        }
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            ZXingScannerPage scanPage = new ZXingScannerPage();
            //scanPage.Title = "扫描条形码";

            scanPage.OnScanResult += (result) =>
            {

                scanPage.IsScanning = false;



                Device.BeginInvokeOnMainThread(() =>
                {

                    Navigation.PopAsync();
                    //txtBarcode.Text = result.Text;
                    //DisplayAlert("Scanned Barcode", result.Text, "OK");

                });

            };
            await Navigation.PushAsync(scanPage);
            //try
            //{
            //    var scanner = DependencyService.Get<IQrScanningService>();
            //    var result = await scanner.ScanAsync();
            //    if (result != null)
            //    {
            //        txtBarcode.Text = result;
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
        }
        private void DefaultSet()
        {
            overlay = new ZXingDefaultOverlay
            {
                TopText = "Hold your phone up to the barcode",
                //BottomText = "Scanning will happen automatically",
                BottomText = "",
                //ShowFlashButton = zxing.HasTorch,
                ShowFlashButton = false,
                AutomationId = "zxingDefaultOverlay",
            };
            overLayout = new ZXingOverLayout();
            //添加刷新按钮
            Button bt = new Button
            {
                Text = "刷新",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 0, 50),
                HeightRequest = 50
            };
            bt.Clicked += (sender, e) =>
            {
                zxing.IsTorchOn = !zxing.IsTorchOn;
                zxing.IsAnalyzing = true;
            };
            //BoxView boxViewBottom = new BoxView
            //{
            //    BackgroundColor = Color.White,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    VerticalOptions = LayoutOptions.FillAndExpand
            //};   
            //overlay.Children.Add(boxViewBottom);          
            overlay.Children.Add(bt);
            // Grid.SetRow(boxViewBottom, 2);          
            Grid.SetRow(bt, 2);
            //overlay.FlashButtonClicked += (sender, e) =>
            //{
            //    zxing.IsTorchOn = !zxing.IsTorchOn;
            //    zxing.IsAnalyzing = true;
            //};
        }
    }
}