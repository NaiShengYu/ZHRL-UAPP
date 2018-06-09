using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
using System.Text.RegularExpressions;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif
namespace AepApp.View.Monitor
{
    public partial class SupervisionAndInspectionInfoPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }

        List<SupervisionAndInspectionInfo> list = null;
        SupervisionAndInspection _spection = null;//企业模型
        public SupervisionAndInspectionInfoPage(SupervisionAndInspection spection, EnterpriseModel ent)
        {
            InitializeComponent();

            _ent = ent;
            this.BindingContext = Enterprise;

            NavigationPage.SetBackButtonTitle(this, "");
            _spection = spection;
            this.Title = spection.NAME;

            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender, e) => {
                CrossHud.Current.Show("请求中...");
                makeData();
            };
            wrk.RunWorkerCompleted += (sender, e) => {
                try
                {
                    listV.ItemsSource = list;
                }
                catch (Exception ex)
                {
                    CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 5));
                }

            };
            wrk.RunWorkerAsync();

        }

        public static string ReplaceSuperscript(Match match)
        {
            string input = match.Value;
            string ret = "";
            char[] sups = "⁰¹²³⁴⁵⁶⁷⁸⁹".ToCharArray();
            foreach (char c in input.ToCharArray())
            {
                int d = c - '0';
                if (d < 0 || d > 9) continue;
                ret += sups[c - '0'];
            }
            return ret;
        }

        private string ReplacePower(string input)
        {
            if (input == null) return "";
            MatchEvaluator evaluator = new MatchEvaluator(ReplaceSuperscript);
            string ret = Regex.Replace(input, @"<sup>\d*</sup>", evaluator);
            return ret;
        }

        public static string ReplaceSubscript(Match match)
        {
            string input = match.Value;
            string ret = "";
            char[] sups = "₀₁₂₃₄₅₆₇₈₉".ToCharArray();
            foreach (char c in input.ToCharArray())
            {
                int d = c - '0';
                if (d < 0 || d > 9) continue;
                ret += sups[c - '0'];
            }
            return ret;
        }

        private string ReplaceSubs(string input)
        {
            if (input == null) return "";
            MatchEvaluator evaluator = new MatchEvaluator(ReplaceSubscript);
            string ret = Regex.Replace(input, @"<sub>\d*</sub>", evaluator);
            return ret;
        }

        void makeData()
        {
            try
            {
                var id = float.Parse(_spection.ID);
                string url = App.BaseUrl + "/api/AppEnterprise/GetMonidataPollutionDetail?id=" + (int)id;
                Console.WriteLine("请求接口：" + url);
                string result = EasyWebRequest.sendGetHttpWebRequest(url);
                Console.WriteLine("请求结果：" + result);
                //var jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//这个设置，反序列化的时候，不处理为空的值。
                //result = "{'items':[],'count':'5.0','ncount':'2.0'}";
                list = JsonConvert.DeserializeObject<List<SupervisionAndInspectionInfo>>(result);
                foreach (var a in list)
                {
                    a.UNIT = ReplacePower(a.UNIT);
                    a.VALUE = ReplacePower(a.VALUE);
                }
                CrossHud.Current.Dismiss();
            }
            catch (Exception ex)
            {
                CrossHud.Current.ShowError(message: ex.Message, timeout: new TimeSpan(0, 0, 5));
            }
        }

    }
}
