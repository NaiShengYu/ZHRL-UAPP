﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    //网格化模块图片上传返回值
    public class GridImageModel
    {
        public Guid id { get; set; }
        public string fname { get; set; }
        public string url { get; set; }
        public string extension { get; set; }//eg: .png
    }
}
