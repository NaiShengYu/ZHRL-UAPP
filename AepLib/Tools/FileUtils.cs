using AepApp.Services;
using SimpleAudioForms;
using System;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AepApp.Tools
{
    public class FileUtils
    {

        /// <summary>
        /// 存储log文件
        /// </summary>
        /// <param name="content"></param>
        public static void SaveLogFile(String content)
        {
            String fileName = "_uapp_logging_" + TimeUtils.DateTime2YMD(DateTime.Now) + "_";
            String path = DependencyService.Get<IFileService>().GetExtrnalStoragePath();
            if (Device.RuntimePlatform == Device.Android)
            {
                path = path + "/logs";
            }
            Console.WriteLine("===file path:" + path);
            SaveFile(content, path, fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param> 存储路径，包含文件名
        /// <param name="fileName"></param> 文件名（带后缀）
        public static void SaveFile(string content, string path, string fileName)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            String fullName = Path.Combine(path, fileName);
            if (!File.Exists(fullName))
            {
                File.Create(fullName).Dispose();
            }
            File.AppendAllText(fullName, "\r\n" + DateTime.Now.ToString());
            File.AppendAllText(fullName, content);
            File.AppendAllText(fullName, "\r\n");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoPath">video的完整路径</param>
        /// <param name="thumbName"></param>
        public static string SaveThumbImage(string videoPath, string thumbName)
        {
            if (string.IsNullOrWhiteSpace(videoPath) || string.IsNullOrWhiteSpace(thumbName)) return "";
            string dirPath = "";

            if (Device.RuntimePlatform == Device.Android)
            {
                int index = videoPath.LastIndexOf("/");
                if (index >= 0)
                {
                    dirPath = videoPath.Substring(0, index + 1);
                }
            }
            if (Device.RuntimePlatform == Device.iOS) { 
                dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)+"/";
            }

            DependencyService.Get<IAudio>().stopPlay(); 
            DependencyService.Get<IAudio>().SaveThumbImage(dirPath, thumbName, videoPath, 1);
            return dirPath + thumbName;

        }

        public static string VidioTranscoding(string videoPath, string videoName)
        {
            if (string.IsNullOrWhiteSpace(videoPath) || string.IsNullOrWhiteSpace(videoName)) return "";
            string dirPath = "";
            dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
            DependencyService.Get<IAudio>().VideoTranscoding(dirPath + videoName,videoPath);
            return dirPath + videoName;
        }

        /// <summary>
        /// 获取本地或网络文件的文件名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>带/不带文件后缀的文件名</returns>
        public static string GetFileName(string filePath, bool containsSuffix)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return "";
            string name = filePath;
            int index = filePath.LastIndexOf("/");
            int formatIndex = filePath.LastIndexOf(".");
            if (index >= 0 && formatIndex >= 0)
            {
                if (containsSuffix)
                {
                    name = filePath.Substring(index + 1, filePath.Length - index - 1);
                }
                else
                {
                    int suffixLength = filePath.Length - formatIndex;
                    name = filePath.Substring(index + 1, filePath.Length - index - suffixLength - 1);
                }
            }
            return name;
        }

        public static string ReplaceFileSuffix(string filePath, string suffix)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(suffix)) return "";
            int index = filePath.LastIndexOf(".");
            if (index <= 0) return filePath;
            StringBuilder sb = new StringBuilder();
            string path = sb.Append(filePath.Substring(0, index)).Append(suffix).ToString();
            return path;
        }
    }
}
