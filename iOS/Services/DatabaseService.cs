using System;
using System.IO;
using AepApp.iOS.Services;
using AepApp.Services;
[assembly: Xamarin.Forms.Dependency(typeof(DatabaseService))]
namespace AepApp.iOS.Services
{
        class DatabaseService : IDatabaseService
        {
            public string GetDbPath()
            {
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "UappDB.db");
            }
        }

}
