using System;
using AepApp.Interface;
using AepApp.iOS.Services;
using Xamarin.Forms;
using AVFoundation;
using Foundation;
using System.Diagnostics;
using System.IO;

[assembly: Dependency(typeof(RecordVoice))]

namespace AepApp.iOS.Services
{
    public class RecordVoice : IRecordVoice
    {
        AVAudioRecorder recorder;
        NSDictionary settings;
        public RecordVoice(){
          
        }

        public void  startRecord(string filePath){

            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);//必须先把声道改成录音模式
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
            }
            err = audioSession.SetActive(true);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
            }

            NSObject[] values = new NSObject[]{
                NSNumber.FromFloat(22050.0f),
                NSNumber.FromInt32((int)AudioToolbox.AudioFormatType.MPEG4AAC),//
                NSNumber.FromInt32(1),
                NSNumber.FromInt32((int)AVAudioQuality.Medium),
            };
            NSObject[] keys = new NSObject[]{
                AVAudioSettings.AVSampleRateKey,//声音采样率（8000，44100）
                AVAudioSettings.AVFormatIDKey,//(编码格式)
                AVAudioSettings.AVNumberOfChannelsKey, //声道数 1为单声道， 2为双声道（立体声）
                AVAudioSettings.AVEncoderAudioQualityKey,//录音质量，在`AVAudioQuality`枚举中，值有`min low medium  high  max`四个
            };
            settings = NSDictionary.FromObjectsAndKeys(values, keys);
            recorder = AVAudioRecorder.Create(new Uri(filePath), new AudioSettings(settings), out NSError error);

            if ((recorder == null) || (error != null))
            {
                Console.WriteLine(error);
                return;
            }
            Debug.WriteLine("开始录制");
            recorder.PrepareToRecord();
            if (recorder.PrepareToRecord() == true)
            {
                recorder.MeteringEnabled = true;
                recorder.Record();
            }
            else Debug.WriteLine("录制没有准备好");

        }

        public string stopRecord (string filePath)
        {
            if(recorder !=null)
                recorder.Stop();
            try{
                NSError error;
                var audioPlayer = new AVAudioPlayer(new NSUrl(filePath), AVFileType.Mpeg4, out error);
                var aaa = audioPlayer.Duration+1;
                return aaa.ToString("f0");
            }catch(Exception ex){
                return "";
            }


        }



    }
}
