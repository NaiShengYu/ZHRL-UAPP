using System;
using System.Diagnostics;
using System.IO;
using AVFoundation;
using Foundation;
using SimpleAudioForms.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]

namespace SimpleAudioForms.iOS
{
    public class AudioService : IAudio
    {
        private AVAudioPlayer audioPlayer;

        public AudioService() {
       
        }

        private void Play(string fileName, string type)
        {
   

            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.Playback);//使播放器用免提播放声音
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
                return;
            }
            err = audioSession.SetActive(true);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
                return ;
            }


            try{
                
                NSData data = NSData.FromUrl(new NSUrl(fileName));
                NSError error;
                if(audioPlayer !=null){
                    if (audioPlayer.Playing == true)audioPlayer.Stop();
                }
                if (type == "Net") 
                    audioPlayer = new AVAudioPlayer(data, AVFileType.Mpeg4, out error);
                if (type == "local") 
                    audioPlayer = new AVAudioPlayer(new NSUrl(fileName), AVFileType.Wave, out error);
                Debug.WriteLine("时间长度：" + audioPlayer.Duration);

                audioPlayer.Play();
                Debug.WriteLine("数据长度：" + audioPlayer.Data.Length / 1024);
   
            }catch(Exception ex){}
        }

        #region IAudio
        public void PlayNetFile(string fileName)
        {
            Debug.WriteLine($"PlayNetFile(string {fileName})");
             Play(fileName, "Net");
        }

        public void PlayLocalFile(string fileName)
        {
            Debug.WriteLine($"PlayWavFile(string {fileName})");
             Play(fileName, "local");
        }

        public void stopPlay(){
            try
            {
                audioPlayer.Stop();
            }
            catch (Exception ex) { }

        }
        #endregion IAudio
    }
}