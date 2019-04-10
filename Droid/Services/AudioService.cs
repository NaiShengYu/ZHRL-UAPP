using System;
using Xamarin.Forms;
using SimpleAudioForms.Droid;
using Android.Media;
using System.Collections.Generic;
using Android.Graphics;
using Java.IO;
using System.IO;
using Android.Content;
using System.Threading.Tasks;

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
                System.Console.WriteLine("there was an error trying to start the MediaPlayer! cause:" + ex);
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

        public ImageSource GenerateThumbImage(string savePath, string url, long usecond)
        {
            try
            {
                MediaMetadataRetriever retriever = new MediaMetadataRetriever();
                retriever.SetDataSource(url, new Dictionary<string, string>());
                Bitmap bitmap = retriever.GetFrameAtTime(usecond <= 0 ? 2 * 1000 * 1000 : usecond, Option.ClosestSync);
                retriever.Release();
                if (bitmap != null)
                {
                    MemoryStream stream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    byte[] bitmapData = stream.ToArray();
                    return ImageSource.FromStream(() => new MemoryStream(bitmapData));
                }
            }
            catch (Exception)
            {
                return null;
            }            
            return null;
        }

        public void SaveThumbImage(string savePath, string fileName, string url, long usecond)
        {
            if (string.IsNullOrWhiteSpace(savePath) || string.IsNullOrWhiteSpace(url)) return;
            
            try
            {
                if (savePath.StartsWith("http") || savePath.StartsWith("https"))
                {
                    Context c = Android.App.Application.Context;
                    Java.IO.File f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryMovies);
                    savePath = f.AbsolutePath;
                }
                Java.IO.FileInputStream input = new FileInputStream(url);
                MediaMetadataRetriever retriever = new MediaMetadataRetriever();
                //retriever.SetDataSource(url, new Dictionary<string, string>());
                retriever.SetDataSource(input.FD);
                Bitmap bitmap = retriever.GetFrameAtTime(usecond <= 0 ? 2 * 1000 * 1000 : usecond, Option.ClosestSync);
                if (bitmap != null)
                {
                    MemoryStream stream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    byte[] bitmapData = stream.ToArray();
                    System.IO.File.WriteAllBytes(savePath + fileName, bitmapData);
                }
                retriever.Release();
                bitmap.Recycle();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("===error:" + e);
            }
            
        }

        public Task<bool> CompressVideo(string inputPath, string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}