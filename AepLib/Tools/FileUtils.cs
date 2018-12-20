using AepApp.Services;
using System;
using System.IO;
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
            if(Device.RuntimePlatform == Device.Android)
            {
                path = path + "/Android/data/com.zhrl.AepApp/logs";
            }
            Console.WriteLine("file path:" + path);
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
    }
}
