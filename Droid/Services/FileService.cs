using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AepApp.Droid.Services;
using AepApp.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
            return Android.OS.Environment.ExternalStorageDirectory.Path;
        }
    }
}