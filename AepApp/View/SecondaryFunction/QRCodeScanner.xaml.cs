
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace AepApp.View.SecondaryFunction
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRCodeScanner : ContentPage
    {
        ZXing.Net.Mobile.Forms.ZXingScannerView zxing;
        ZXingDefaultOverlay defaultOverlay;
        ZXingOverLayout customOverLayout;


        public QRCodeScanner() : base()
        {
            //InitializeComponent();
            int height = App.ScreenHeight / 2;
            this.Title = "二维码扫描";
            InitZxing(height);
            //initCustomOverLayout();
            DefaultSet();

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(zxing);
            //grid.Children.Add(customOverLayout);
            grid.Children.Add(defaultOverlay);
            Content = grid;

        }

        private void initCustomOverLayout()
        {
            customOverLayout = new ZXingOverLayout();
        }

        private void InitZxing(int height)
        {
            zxing = new ZXing.Net.Mobile.Forms.ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingScannerView",
                IsScanning = true,
                IsAnalyzing = true,
                Options = new MobileBarcodeScanningOptions()
                {
                    TryHarder = true,//识别大的二维码
                },
            };
            //zxing.Margin = new Thickness(0, -height + 10, 0, height - 10);
            zxing.OnScanResult += (result) =>
               Device.BeginInvokeOnMainThread(async () =>
               {
                   // Stop analysis until we navigate away so we don't keep reading barcodes
                   zxing.IsAnalyzing = false;
                   zxing.IsScanning = false;

                   DependencyService.Get<Sample.IToast>().ShortAlert(result.Text);
                   //customOverLayout.setMessage(result.Text);
                   await Navigation.PopAsync();
               });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;

            base.OnDisappearing();
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            ZXingScannerPage scanPage = new ZXingScannerPage();
            //scanPage.Title = "扫描条形码";           

            //add options and customize page
            //scanPage = new ZXingScannerPage(options)
            //{
            //    DefaultOverlayTopText = "Align the barcode within the frame",
            //    DefaultOverlayBottomText = string.Empty,
            //    DefaultOverlayShowFlashButton = true
            //};

            scanPage.OnScanResult += (result) =>
            {

                scanPage.IsScanning = false;


                Device.BeginInvokeOnMainThread(() =>
                {

                    Navigation.PopAsync();
                    txtBarcode.Text = result.Text;
                    DisplayAlert("Scanned Barcode", result.Text, "OK");

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
            defaultOverlay = new ZXingDefaultOverlay
            {
                TopText = "",
                BottomText = "将条码/二维码放入框内即可自动扫描",
                ShowFlashButton = false,
                AutomationId = "zxingDefaultOverlay",
            };
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
                //zxing.IsTorchOn = !zxing.IsTorchOn;
                zxing.IsAnalyzing = true;
            };
            //BoxView boxViewBottom = new BoxView
            //{
            //    BackgroundColor = Color.White,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    VerticalOptions = LayoutOptions.FillAndExpand
            //};   
            //overlay.Children.Add(boxViewBottom);          
            //defaultOverlay.Children.Add(bt);
            // Grid.SetRow(boxViewBottom, 2);          
            //Grid.SetRow(bt, 2);
            //overlay.FlashButtonClicked += (sender, e) =>
            //{
            //    zxing.IsTorchOn = !zxing.IsTorchOn;
            //    zxing.IsAnalyzing = true;
            //};
        }
    }
}