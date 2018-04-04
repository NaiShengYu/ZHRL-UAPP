
using System;

using System.IO;

using Xamarin.Forms;

using Todo.Droid;
using AepApp.Interface;

[assembly: Dependency(typeof(FileHelper))]

namespace Todo.Droid

{

    public class FileHelper : IFileHelper

    {

        public string GetLocalFilePath(string filename)

        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            return Path.Combine(path, filename);

        }

    }

}