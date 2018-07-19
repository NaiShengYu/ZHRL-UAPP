using System;
using Xamarin.Forms;
using SimpleAudioForms.Droid;
using Android.Media;

[assembly: Dependency(typeof(AudioService))]

namespace SimpleAudioForms.Droid
{
    public class AudioService : IAudio
    {

        MediaPlayer mediaPlayer;

        public AudioService()
        {
        }


        public void PlayNetFile(string fileName)
        {
            Play(fileName);
        }

        public void PlayLocalFile(string fileName)
        {
            Play(fileName);
        }

        public void stopPlay()
        {
            Stop();
        }


        void Play(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                DependencyService.Get<Sample.IToast>().ShortAlert("录音文件不存在");
                return;
            }
            if (mediaPlayer == null)
            {
                mediaPlayer = new MediaPlayer();
            }
            try
            {
                mediaPlayer.Reset();
                mediaPlayer.SetDataSource(fileName);
                mediaPlayer.Prepare();
                mediaPlayer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("there was an error trying to start the MediaPlayer! cause:" + ex);
            }
        }

        void Stop()
        {
            if (mediaPlayer == null)
            {
                return;
            }
            mediaPlayer.Release();
            mediaPlayer = null;
        }
    }
}