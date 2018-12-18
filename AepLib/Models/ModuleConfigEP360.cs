using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Models
{
    /// <summary>
    /// 主菜单配置 -- EP360
    /// </summary>
    public class ModuleConfigEP360
    {
        public string menu360Label { get; set; }//“企业/污染源
        public bool menuPollutionSrc { get; set; }
        public string menuPollutionSrcLabel { get; set; }//污染源
        public bool show360Monitor { get; set; }//
        public bool show360Summary { get; set; }//
        public bool showGridSummary { get; set; }//
        public string menuGridLabel { get; set; }
        public bool menuGridTask { get; set; }//任务列表
        public string menuGridTaskLabel { get; set; }
        public bool menuGridEvent { get; set; }//事件列表
        public string menuGridEventLabel { get; set; }
        public bool menuGridEventReceived { get; set; }//下级上报事件
        public string menuGridEventReceivedLabel { get; set; }
        public bool menuGridInfoReceived { get; set; }//上级下发信息
        public string menuGridInfoReceivedLabel { get; set; }
        public bool showAddIncident { get; set; }//增加事件
        public string menuAddIncidentLabel { get; set; }
        public bool menuGridInfoAssessment { get; set; }//增加事件
        public string menuGridInfoAssessmentLabel { get; set; }

    }
}
