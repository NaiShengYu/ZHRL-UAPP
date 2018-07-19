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

        /// <summary>
        /// 播放本地录音
        /// </summary>
        /// <param name="fileName"></param>
        public void PlayNetFile(string fileName)
        {
            Play(fileName);
        }

        /// <summary>
        /// 播放网络录音文件
        /// </summary>
        /// <param name="fileName"></param>
        public void PlayLocalFile(string fileName)
        {
            Play(fileName);
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void stopPlay()
        {
            Stop();
        }

        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="fileName"></param>
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

        /// <summary>
        /// 停止播放
        /// </summary>
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