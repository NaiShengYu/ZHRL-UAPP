﻿using AepApp.iOS.Services;
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
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        }
    }

}
