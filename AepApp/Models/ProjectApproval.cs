using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace AepApp.Models
{
    public class ProjectApproval
    {
        public string id { get; set; }
        public string ENV_CREATE_TIME { get; set; }
        public string PROJECT_NAME { get; set; }
        public string ENV_PROCESSING_STAGE { get; set; }
        public string COM_PROCESSING_STAGE { get; set; }
        public string PROJECT_LIFE_CYCLE { get; set; }
        public string project_status { get; set; }
        public string rn { get; set; }
        //public List<ProjectFileData> FileData { get; set; }
    }
}
