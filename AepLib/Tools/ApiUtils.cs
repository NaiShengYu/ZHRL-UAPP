﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Tools
{
    /// <summary>
    /// 常量类
    /// </summary>
    public class ApiUtils
    {
        public const int PAGE_SIZE = 20;
        //文件上传--应急模块
        //public const string UPLOAD_EMERGENCY_BASEURL = "http://gx.azuratech.com:5000";
        public const string UPLOAD_EMERGENCY_API = "/api/File/Upload";
        //上传视频封面
        public const string UPLOAD_COVER = "/api/File/UploadCover";
        //文件上传--网格化模块
        //public const string UPLOAD_GRID_BASEURL = "http://dev.azuratech.com:50015";
        //public const string UPLOAD_GRID_BASEURL = "http://192.168.2.97:50015";
        public const string UPLOAD_GRID_API = "/api/gbm/UploadAppAttachment";

        public const string SAMPLE_TEST_URL = "http://192.168.1.128:30011/";
    }
}
