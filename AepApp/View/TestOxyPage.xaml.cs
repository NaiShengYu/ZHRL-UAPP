using AepApp.Interface;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Media;
using SimpleAudioForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestOxyPage : ContentPage
    {

        string filename;
        void StartRecordVoice(object sender, System.EventArgs e){
            //IRecordVoice
            //DependencyService.Get<IAudio>().PlayWavFile(
            //recorder.FilePath
            //);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dir = path + "/Voice/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            //存储文件名
            string name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
            filename = Path.Combine(dir, name);

            Console.WriteLine(filename);
            DependencyService.Get<IRecordVoice>().startRecord(filename);

        }

         void StopRecordVoice(object sender, System.EventArgs e){


            var time = DependencyService.Get<IRecordVoice>().stopRecord(filename);

            var upfilebytes = File.ReadAllBytes(filename);
            Console.WriteLine("fileLength===" + upfilebytes.Length);
            Console.WriteLine("播放时长===" + time);


            //DependencyService.Get<IAudio>().PlayMp3File(filename);


        }


        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }


            var file1 = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
            {
                DesiredLength = new TimeSpan(0, 0, 10),
                Name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4",
                Directory = "Video",
            });
            MV.Source = new Uri(file1.Path);



        }



		public TestOxyPage ()
		{
			InitializeComponent ();


            //// Declare string for application temp path and tack on the file extension
         

            ////var source = new file("ppp.mp4");
            //var path = Path.Combine(Environment.CurrentDirectory, "app_icon_ios-2.doc");
            ////var path = Path.Combine(Environment.CurrentDirectory, "app_logo11111.png");

            //MV.Source = new Uri(path);
            //PostupLoadVideoSending(path);


            //var data = new OxyDataPageModle().AreaModel;
            //abc.Model = data;
        }
        private async void PostupLoadVideoSending(string path)
        {
            HTTPResponse hTTPResponse = await EasyWebRequest.upload(path, ".mp4");

        }
	}
}