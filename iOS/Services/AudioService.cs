using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using MediaPlayer;
using SimpleAudioForms.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]

namespace SimpleAudioForms.iOS
{
    public class AudioService : IAudio
    {
        private AVAudioPlayer audioPlayer;
        private string lastFileName;
        public AudioService() {
            UIImagePickerController controller = new UIImagePickerController();
            controller.ShowViewController(new UIViewController(),new NSObject());
            controller.FinishedPickingMedia += (object sender, UIImagePickerMediaPickedEventArgs e) => {
                UIImagePickerController pickerController = sender as UIImagePickerController;
               
 pickerController.DismissViewController(true,
                     () => { });
            
            };

        }
        /// <summary>
        /// 播放录音
        /// </summary>
        /// <param name="fileName">录音的位置</param>
        /// <param name="type">录音的方式，网络或者本地</param>
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
                    if (lastFileName == fileName) return;
                }
                if (type == "Net") 
                    audioPlayer = new AVAudioPlayer(data, AVFileType.Mpeg4, out error);
                if (type == "local") 
                    audioPlayer = new AVAudioPlayer(new NSUrl(fileName), AVFileType.Wave, out error);
                Debug.WriteLine("时间长度：" + audioPlayer.Duration);

                audioPlayer.Play();
                Debug.WriteLine("数据长度：" + audioPlayer.Data.Length / 1024);
   
            }catch(Exception ex){
            
            }
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


        public ImageSource GenerateThumbImage(string savePath, string url, long usecond)
        {
            AVAssetImageGenerator imageGenerator = new AVAssetImageGenerator(AVAsset.FromUrl((new Foundation.NSUrl(url))));
            imageGenerator.AppliesPreferredTrackTransform = true;
            CMTime actualTime;
            NSError error;
            CGImage cgImage = imageGenerator.CopyCGImageAtTime(new CMTime(usecond, 1000000), out actualTime, out error);
            //File.WriteAllBytes(savePath, cgImage);
            return ImageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream());
        }

        public void SaveThumbImage(string savePath, string fileName, string url, long usecond)
        {

            var asset = AVAsset.FromUrl(NSUrl.FromString(url));
            var imageGenerator = AVAssetImageGenerator.FromAsset(asset);
            imageGenerator.AppliesPreferredTrackTransform = true;

            var actualTime = asset.Duration;
            CoreMedia.CMTime cmTime = new CoreMedia.CMTime(1, 60);

            NSError error;
            var imageRef = imageGenerator.CopyCGImageAtTime(cmTime, out actualTime, out error);

            if (imageRef == null)
                return ;
            var image = UIImage.FromImage(imageRef);
            NSData data = image.AsPNG();
            string aa = Path.Combine(savePath, fileName);
            data.Save(aa, false, out error);

        }

        public Task<bool> CompressVideo(string inputPath, string outputPath)
        {
            throw new NotImplementedException();
        }

    }
}