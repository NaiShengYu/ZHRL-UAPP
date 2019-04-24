using System;

namespace AepApp.Services
{
    public interface IFileService
    {
        string GetDbPath();
        string GetExtrnalStoragePath();
        string GetExtrnalStoragePath(string type);
        void OpenFileDocument(string localPath, string suffix);//打开文档
    }
}
