
using AepApp.Models;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using System;
using InTheHand.Forms;
using Xamarin.Forms;
using SimpleAudioForms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ShowVideoPage : ContentPage
    {
        string _videoPath;
        private IPlaybackController PlaybackController => CrossMediaManager.Current.PlaybackController;
        void Handle_CurrentStateChanged(object sender, System.EventArgs e)
        {

            //Console.WriteLine("播放器状态：" +MV.CurrentState);


        }

        public ShowVideoPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = "视频播放";
            //SetVideoView();
        }

        private void SetVideoView()
        {
            CrossMediaManager.Current.VolumeManager.Mute = false;
            CrossMediaManager.Current.MediaFileChanged += Current_MediaFileChanged;
            CrossMediaManager.Current.StatusChanged += Current_StatusChanged;
        }

        private void Current_MediaFileChanged(object sender, Plugin.MediaManager.Abstractions.EventArguments.MediaFileChangedEventArgs e)
        {
            string album = e.File.Metadata.AlbumArtUri;
            Console.WriteLine("=== album:" + album);
        }

        private void Current_StatusChanged(object sender, Plugin.MediaManager.Abstractions.EventArguments.StatusChangedEventArgs e)
        {
            //e.Status == Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing
        }

        private void play()
        {
            if (string.IsNullOrWhiteSpace(_videoPath)) return;
            //MV.Source = _videoPath;
            //PlaybackController.Play();
        }

        public ShowVideoPage(UploadEmergencyShowModel model) : this()
        {

            this.BindingContext = model;
            //Console.WriteLine("时间长度" + MV.NaturalDuration);
            if (model != null)
            {
                _videoPath = model.VideoPath;
                DependencyService.Get<IAudio>().SaveThumbImage("", "", model.VideoPath, 1);

                play();
            }
        }
        public ShowVideoPage(EmergencyAccidentInfoDetail.IncidentLoggingEventsBean model) : this()
        {
            this.BindingContext = model;
            //Console.WriteLine("时间长度" + MV.NaturalDuration);
            if (model != null)
            {
                _videoPath = model.VideoPath;
                play();
            }
        }

    }
}
