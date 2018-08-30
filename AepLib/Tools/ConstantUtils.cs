using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Tools
{
    /// <summary>
    /// 常量类
    /// </summary>
    public class ConstantUtils
    {
        public const int PAGE_SIZE = 20;
        //文件上传--应急模块
        public const string UPLOAD_EMERGENCY_BASEURL = "http://gx.azuratech.com:5000";
        public const string UPLOAD_EMERGENCY_API = "/api/File/Upload";
        //文件上传--网格化模块
        //public const string UPLOAD_GRID_BASEURL = "http://dev.azuratech.com:50015";
        public const string UPLOAD_GRID_BASEURL = "http://192.168.2.97:50015";
        public const string UPLOAD_GRID_API = "/api/gbm/UploadAppAttachment";
    }
}
