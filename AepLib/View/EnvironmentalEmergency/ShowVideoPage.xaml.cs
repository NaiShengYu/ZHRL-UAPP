
using AepApp.Models;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using System;
using Xamarin.Forms;

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
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "视频播放";
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


        //上传数据页面播放视频
        public ShowVideoPage(UploadEmergencyShowModel model) : this()
        {
            this.BindingContext = model;
            //Console.WriteLine("时间长度" + MV.NaturalDuration);
        }
        //事故详情页播放视频
        public ShowVideoPage(EmergencyAccidentInfoDetail.IncidentLoggingEventsBean model) : this()
        {
            this.BindingContext = model;
        }

    }
}
