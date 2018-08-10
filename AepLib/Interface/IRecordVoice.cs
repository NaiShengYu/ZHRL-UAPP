using System;
namespace AepApp.Interface
{
    public interface IRecordVoice
    {
       
        void startRecord(string filePath);
        string stopRecord(string filePath);
    }
}
