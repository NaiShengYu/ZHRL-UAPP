using AepApp.iOS.Services;
using AepApp.Services;
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
    }

}
