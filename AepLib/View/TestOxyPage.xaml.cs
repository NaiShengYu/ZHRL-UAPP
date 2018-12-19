using AepApp.Interface;
using AepApp.Models;
using AepApp.Tools;
using AepApp.ViewModels;
using CloudWTO.Services;
using Plugin.Media;
using SimpleAudioForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestOxyPage : ContentPage, INotifyPropertyChanged
    {

        async Task aaaaaa()
        {

            string url = "http://sx.azuratech.com:32017" + "/api/mod/custconfig";
            //string result = await EasyWebRequest.testPostNetWork(url);

            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url,"","POST","","json");


        }



        void Handle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        
        
        
        
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        void changeModel3(object sender, System.EventArgs e)
        {


            TestModel.secondModel.thirdModel = new thirdTestModel
            {
                thirdTitle = "一个新的第三层模型",
            };


        }
        void changeModel2(object sender, System.EventArgs e)
        {


            TestModel.secondModel = new secondTestModel
            {
                secondTitle = "一个新的第2层模型",
                thirdModel = new thirdTestModel
                {
                    thirdTitle = "xinde第三层",
                }
            };


        }
        void changeModel1(object sender, System.EventArgs e)
        {


            TestModel = new TestModel
            {
                tsetModelTitle = "xin第一层",
                secondModel = new secondTestModel
                {
                    secondTitle = "xin第二层",
                    thirdModel = new thirdTestModel
                    {
                        thirdTitle = "xin第三层",
                    }
                }

            };
            //BindingContext = _model;



        }


        async void scanZXing(object sender, System.EventArgs e)
        {

#if __ANDROID__
    // Initialize the scanner first so it can track the current context
         //MobileBarcodeScanner.Initialize (Application);
#endif

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.CancelButtonText = "取消";
            scanner.FlashButtonText = "闪光灯";
            scanner.CameraUnsupportedMessage = "CameraUnsupportedMessage";
            var result = await scanner.Scan();

            if (result != null)
                Console.WriteLine("Scanned Barcode: " + result.Text);
        }






        //string filename;
        //void StartRecordVoice(object sender, System.EventArgs e){
        //    //IRecordVoice
        //    //DependencyService.Get<IAudio>().PlayWavFile(
        //    //recorder.FilePath
        //    //);

        //    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //    var dir = path + "/Voice/";
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //    }
        //    //存储文件名
        //    string name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
        //    filename = Path.Combine(dir, name);

        //    Console.WriteLine(filename);
        //    DependencyService.Get<IRecordVoice>().startRecord(filename);

        //}

        // void StopRecordVoice(object sender, System.EventArgs e){


        //    var time = DependencyService.Get<IRecordVoice>().stopRecord(filename);

        //    var upfilebytes = File.ReadAllBytes(filename);
        //    Console.WriteLine("fileLength===" + upfilebytes.Length);
        //    Console.WriteLine("播放时长===" + time);


        //    //DependencyService.Get<IAudio>().PlayMp3File(filename);


        //}


        //async void Handle_Clicked(object sender, System.EventArgs e)
        //{
        //    await CrossMedia.Current.Initialize();
        //    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        //    {
        //        await DisplayAlert("No Camera", ":( No camera available.", "OK");
        //        return;
        //    }


        //    var file1 = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
        //    {
        //        DesiredLength = new TimeSpan(0, 0, 10),
        //        Name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4",
        //        Directory = "Video",
        //    });
        //    MV.Source = new Uri(file1.Path);



        //}


        public TestModel TestModel
        {
            get { return _model; }
            set { _model = value; NotifyPropertyChanged("TestModel"); }
        }

        TestModel _model = null;
        List<string> _dddd = new List<string>();
        List<string> _bbbb = new List<string>();

       //MonkeysViewModel _monkeyModel = new MonkeysViewModel();
        public TestOxyPage()
        {
            InitializeComponent();


            ////_dddd.Add("");
            //_dddd.Add("bbbb");
            //_dddd.Add("cccc");
            ////dddd.Add("dddd");
            ////BindingContext = _dddd;
            //ssss.ItemsSource = _dddd;

            //BindingContext = _monkeyModel;



        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            if (_bbbb.Count > 0)
                _bbbb.Remove(_bbbb[0]);
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {

            //_monkeyModel.Monkeys.Add(new Monkey
            //{
            //    Name = "Capuchin Monkey",
            //    Location = "Central & South America",
            //    Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
            //    ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg"
            //});



        }
    }
}