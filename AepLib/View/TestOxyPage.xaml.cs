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


        public TestOxyPage()
        {
            InitializeComponent();




        }

       async void Handle_Clicked(object sender, System.EventArgs e)
        {
            //await CrossMedia.Current.Initialize();

            //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            //{
            //    await DisplayAlert("No Camera", ":( No camera available.", "OK");
            //    return;
            //}
            string videoName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
            string imgName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_thumb.jpg";

            //var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
            //{
            //    DesiredLength = new TimeSpan(0, 0, 10),
            //    Name = videoName,
            //    Directory = "Video",
            //    SaveToAlbum = true,
            //    CompressionQuality = 0,
            //    Quality = Plugin.Media.Abstractions.VideoQuality.Low,
            //});
            //if (file == null || string.IsNullOrWhiteSpace(file.Path)) return;

            //string thumbPath = FileUtils.SaveThumbImage(file.AlbumPath, imgName);

            //but.Image = ImageSource.FromFile(thumbPath) as FileImageSource;

           var videoPath = FileUtils.VidioTranscoding("", videoName);

        }

        void Handle_CurrentStateChanged(object sender, System.EventArgs e)
        {

            //Console.WriteLine("播放器状态：" +MV.CurrentState);


        }
    }

}