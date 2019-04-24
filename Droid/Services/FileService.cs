using AepApp.Droid.Services;
using AepApp.Services;
using AepApp.Tools;
using Android.Content;
using Android.Net;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace AepApp.Droid.Services
{
    class FileService : IFileService
    {
        public string GetDbPath()
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "UappDB.db");
        }

        public string GetExtrnalStoragePath()
        {

            Context c = Android.App.Application.Context;
            Java.IO.File f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
            return f.AbsolutePath;
        }

        public string GetExtrnalStoragePath(string type)
        {
            Context c = Android.App.Application.Context;
            Java.IO.File f = c.GetExternalFilesDir(null);
            if (Constants.STORAGE_TYPE_DOC.Equals(type))
            {
                f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
            }
            else if (Constants.STORAGE_TYPE_DOWNLOAD.Equals(type))
            {
                f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads);
            }
            else if (Constants.STORAGE_TYPE_PICTURES.Equals(type))
            {
                f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);
            }
            else if (Constants.STORAGE_TYPE_MOVIES.Equals(type))
            {
                f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryMovies);
            }
            else if (Constants.STORAGE_TYPE_MUSIC.Equals(type))
            {
                f = c.GetExternalFilesDir(Android.OS.Environment.DirectoryMusic);
            }
            return f.AbsolutePath;
        }

        /// <summary>
        /// 打开手机本地文档
        /// </summary>
        /// <param name="localPath"></param>
        public void OpenFileDocument(string localPath, string suffix)
        {
            if (string.IsNullOrWhiteSpace(localPath)) return;
            try
            {
                string dataType = "application/msword";
                if (".pdf".Equals(suffix))
                {
                    dataType = "application/pdf";
                }
                Java.IO.File f = new Java.IO.File(localPath);
                Context c = Android.App.Application.Context;
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(Uri.FromFile(f), dataType);
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                c.StartActivity(intent);
            }
            catch (System.Exception)
            {

            }
        }

    }
}