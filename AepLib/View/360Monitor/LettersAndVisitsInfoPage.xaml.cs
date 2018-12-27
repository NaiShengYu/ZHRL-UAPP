using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
using Newtonsoft.Json;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace AepApp.View.Monitor
{
    public partial class LettersAndVisitsInfoPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }

        LettersAndVisits _lettersAndVisits = null;
        public LettersAndVisitsInfoPage(LettersAndVisits LAndV, EnterpriseModel ent)
        {
            InitializeComponent();
            _ent = ent;
            this.BindingContext = Enterprise;
            NavigationPage.SetBackButtonTitle(this, "");
            _lettersAndVisits = LAndV;
            this.Title = _lettersAndVisits.TITLE;

            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) =>
            {
                //CrossHud.Current.Show("请求中...");
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) =>
            {
                try
                {
                    sv.BindingContext = currentModel;

                }
                catch (Exception ex)
                {

                }

            };
            wrk.RunWorkerAsync();

        }

        resultModel currentModel = null;
        void makeData()
        {
            try
            {
                string url = App.EP360Module.url + "/api/AppEnterprise/GetPetitionByid?id=" + _lettersAndVisits.id;
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                currentModel = JsonConvert.DeserializeObject<resultModel>(result);
                if (currentModel.SPETITIONCONTENT != null)
                {
                    currentModel.SPETITIONCONTENT = HtmlRemoval.StripTagsRegex(currentModel.SPETITIONCONTENT);
                    currentModel.SPETITIONCONTENT = currentModel.SPETITIONCONTENT.Replace("&nbsp;", " ");
                }
                if (currentModel.RESULTCONTENT != null)
                {
                    currentModel.RESULTCONTENT = HtmlRemoval.StripTagsRegex(currentModel.RESULTCONTENT);
                    currentModel.RESULTCONTENT = currentModel.RESULTCONTENT.Replace("&nbsp;", " ");
                }
                CrossHud.Current.Dismiss();
            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 3));
            }
        }


        public class resultModel
        {
            public string ID { get; set; }
            public string SERIALNO { get; set; }
            public string TITLE { get; set; }
            public DateTime ORDERDATE { get; set; }
            public string SOURCEID { get; set; }
            public string REASONID { get; set; }
            public string IMPORTANCEID { get; set; }
            public string PETITIONERNAME { get; set; }
            public string TELEPHONE { get; set; }
            public string EMAIL { get; set; }
            public string ADDR { get; set; }
            public string PETITIONEREPID { get; set; }
            public string SIGNCONTENT { get; set; }
            public string CLOSECASEDATE { get; set; }
            public string CLOSECASESTATE { get; set; }
            public string EDITORUSERID { get; set; }
            public string CREATEDATETIME { get; set; }
            public DateTime UPDATEDATETIME { get; set; }
            public DateTime ENDLINEDATE { get; set; }
            public string PROCESSUSERID { get; set; }
            public string PROCESSDATETIME { get; set; }
            public string DEPARTMENTID { get; set; }
            public string VERIFYUSERID { get; set; }
            public string VERIFYDATETIME { get; set; }
            public string VERIFYCONTENT { get; set; }
            public string SIGNUSERID { get; set; }
            public string SIGNDATETIME { get; set; }
            public string CURRENTSTATE { get; set; }
            public string PREVSTATE { get; set; }
            public string STATEDESC { get; set; }
            public string HIDE { get; set; }
            public string VERSION { get; set; }
            public string LEDARIDEA { get; set; }
            public string GUANLIANID { get; set; }
            public string PUBLISHED { get; set; }
            public string ISUPLOAD { get; set; }
            public string UPLOADTIME { get; set; }
            public string SPETITIONCONTENT { get; set; }
            public string RESULTCONTENT { get; set; }
            public string name { get; set; }
            public string ReasonName { get; set; }
            public string SourceName { get; set; }
            public string DepartmenName { get; set; }
            public string ProcessorName { get; set; }
            public string CONTACTNAME { get; set; }
            public string CONTACTTEL { get; set; }
            public string editname { get; set; }








        }
    }
}
