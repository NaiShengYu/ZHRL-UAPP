using AepApp.iOS.Services;
using AepApp.Services;
using AepApp.Tools;
using System.IO;
[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace AepApp.iOS.Services
{
    class FileService : IFileService
    {
        public string GetDbPath()
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "UappDB.db");
        }

        public string GetExtrnalStoragePath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        }

        public string GetExtrnalStoragePath(string type)
        {
            string mainPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            if (!Directory.Exists(Path.Combine(mainPath, "RVideo")))
                Directory.CreateDirectory(Path.Combine(mainPath, "RVideo"));
            if (!Directory.Exists(Path.Combine(mainPath, "Sample")))
                Directory.CreateDirectory(Path.Combine(mainPath, "Sample"));
            if (!Directory.Exists(Path.Combine(mainPath, "Video")))
                Directory.CreateDirectory(Path.Combine(mainPath, "Video"));
            if (!Directory.Exists(Path.Combine(mainPath, "Document")))
                Directory.CreateDirectory(Path.Combine(mainPath, "Document"));
            if (!Directory.Exists(Path.Combine(mainPath, "DownLoad")))
                Directory.CreateDirectory(Path.Combine(mainPath, "DownLoad"));
            if (Constants.STORAGE_TYPE_DOC.Equals(type))
            {
                mainPath = Path.Combine(mainPath, "Document");
            }
            else if (Constants.STORAGE_TYPE_DOWNLOAD.Equals(type))
            {
                mainPath = Path.Combine(mainPath, "DownLoad");
            }
            else if (Constants.STORAGE_TYPE_PICTURES.Equals(type))
            {
                //mainPath = Path.Combine(mainPath, "Sample");
            }
            else if (Constants.STORAGE_TYPE_MOVIES.Equals(type))
            {
                mainPath = Path.Combine(mainPath, "RVideo");
            }
            else if (Constants.STORAGE_TYPE_MUSIC.Equals(type))
            {
                mainPath = Path.Combine(mainPath, "Video");
            }
            return mainPath;
        }
    }

}
