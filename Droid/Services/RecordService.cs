using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AepApp.Interface;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SimpleAudioForms.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(RecordService))]

namespace SimpleAudioForms.Droid
{


    class RecordService : IRecordVoice
    {
        MediaRecorder recorder;
        IRecordEventListener listener;
        string recordPath;//音频完整路径名
        long startRecordTime = 0L;//单位： 10000纳秒= 1毫秒
        long intervalTime = 0L;//音频时长(ms)

        void IRecordVoice.startRecord(string filePath)
        {
            Start(filePath);
        }

        string IRecordVoice.stopRecord(string filePath)
        {
            Stop();
            return (intervalTime / 1000).ToString();
        }


        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="filePath"></param>
        void Start(string filePath)
        {
            recordPath = filePath;
            try
            {
                if (recorder == null)
                {
                    recorder = new MediaRecorder();
                    recorder.SetAudioSource(AudioSource.Mic);
                    recorder.SetOutputFormat(OutputFormat.Mpeg4);
                    recorder.SetAudioEncoder(AudioEncoder.Aac);
                    recorder.SetOutputFile(recordPath);
                    recorder.Prepare();
                }
                else
                {
                    recorder.Reset();
                    recorder.SetOutputFile(recordPath);
                }
                recorder.Start();
                startRecordTime = DateTime.Now.Ticks;
                if (this.listener != null)
                {
                    listener.OnStartRecord();
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("failed to start MediaRecorder. cause : " + ex.StackTrace);
            }
        }
        /// <summary>
        /// 停止录音
        /// </summary>
        void Stop()
        {
            if (recorder != null)
            {
                try
                {
                    recorder.Stop();
                    intervalTime = (DateTime.Now.Ticks - startRecordTime) / 10000;
                    if (listener != null)
                    {
                        if (intervalTime < 1000)
                        {
                            RemoveRecordFile();
                            listener.OnFinishedRecord(0L, "time is too short(less than 1 send)");
                            DependencyService.Get<Sample.IToast>().ShortAlert("录音时间太短");
                        }
                        else
                        {
                            listener.OnFinishedRecord(intervalTime, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to stop MediaRecorder. cause: " + ex);
                }
                finally
                {
                    recorder.Release();
                    recorder = null;
                }
            }
        }

        /// <summary>
        /// 录音取消
        /// </summary>
        void Cancel()
        {
            Stop();
            RemoveRecordFile();
        }

        /// <summary>
        /// 删除录音文件
        /// </summary>
        void RemoveRecordFile()
        {
            if (Directory.Exists(recordPath))
            {
                Directory.Delete(recordPath);
                Console.WriteLine("delete file:" + recordPath);
            }
        }

        void SetRecordEventListener(IRecordEventListener listener)
        {
            this.listener = listener;
        }
    }


    public interface IRecordEventListener
    {
        void OnFinishedRecord(long var1, String var2);

        void OnStartRecord();
    }
}