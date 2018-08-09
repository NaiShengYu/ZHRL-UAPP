using System;

using System.IO;

using Xamarin.Forms;

using Todo.iOS;
using AepApp.Interface;

[assembly: Dependency(typeof(FileHelper))]

namespace Todo.iOS

{
    public class FileHelper : IFileHelper
    {

        public FileHelper(){



        }


        public string GetLocalFilePath(string filename)

        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))

            {

                Directory.CreateDirectory(libFolder);

            }

            return Path.Combine(libFolder, filename);

        }

    }

}

