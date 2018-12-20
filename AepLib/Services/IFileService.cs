using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Services
{
    public interface IFileService
    {
        string GetDbPath();
        String GetExtrnalStoragePath();
    }
}
