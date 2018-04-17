using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace AepApp.Models
{
    public class ProjectApproval
    {
        public int id { get; set; }
        public string ENV_CREATE_TIME { get; set; }
        public string PROJECT_NAME { get; set; }
        public int ENV_PROCESSING_STAGE { get; set; }
        public int COM_PROCESSING_STAGE { get; set; }
        public int PROJECT_LIFE_CYCLE { get; set; }
        public string project_status { get; set; }
        public string rn { get; set; }
        public List<ProjectFileData> FileData { get; set; }

        public string titileNum
        {
            get
            {
                return PROJECT_NAME + '\n' + ENV_CREATE_TIME;
            }
            set { }
        }

        public string stateName {
            get {
                if( PROJECT_LIFE_CYCLE==1){
                    if (ENV_PROCESSING_STAGE == 0)
                        return "添加";
                    
                    if (ENV_PROCESSING_STAGE == 1)
                        return "科长审核";
                    
                    if (ENV_PROCESSING_STAGE == 2)
                        return "科员受理";
                    
                    if (ENV_PROCESSING_STAGE == 3)
                        return "拟审批邮件";
                    
                    if (ENV_PROCESSING_STAGE == 5)
                        return "通过";

                    if (ENV_PROCESSING_STAGE == 6)
                        return "不通过";


                    return null;
                    
                }else{

                    if (COM_PROCESSING_STAGE == 0)
                        return "添加";

                    if (COM_PROCESSING_STAGE == 1)
                        return "科长审核";

                    if (COM_PROCESSING_STAGE == 2)
                        return "科员受理";

                    if (COM_PROCESSING_STAGE == 3)
                        return "拟审批邮件";

                    if (COM_PROCESSING_STAGE == 5)
                        return "环评批复";


                    return null;
                }

            }
            set{}

        }

    }
}
