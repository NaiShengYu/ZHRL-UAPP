using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    /// <summary>
    /// 主菜单配置 -- 采样
    /// </summary>
    public class ModuleConfigSampling
    {
        public string menuSampleLabel { get; set; }
        public bool menuSampleTask { get; set; }// 采样系统 -> 采样计划
        public string menuSampleTaskLabel { get; set; }
        public bool menuSampleCarry { get; set; }// 采样系统 -> 送样
        public string menuSampleCarryLabel { get; set; }
        public bool menuSampleReceive { get; set; }// 采样系统 -> 送样
        public string menuSampleReceiveLabel { get; set; }

    }
}
