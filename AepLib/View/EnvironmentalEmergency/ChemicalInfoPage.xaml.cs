using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ChemicalInfoPage : ContentPage
    {
        //查看详情
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Button but = sender as Button;
            Navigation.PushAsync(new ChemicalInfoWebPage(but.BindingContext as string));
        }

        public ChemicalInfoPage(string id,string name)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            this.Title = name;
            ReqChemicalDetail(id);
            ToolbarItems.Add(new ToolbarItem("环境标准", "", () =>
            {
                Navigation.PushAsync(new ChemicalStandardPage(id));
            }));

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

        private async void ReqChemicalDetail(string id)
        {
            string url = App.BasicDataModule.url + DetailUrl.ChemicalInfo;

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("chemid", id);
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                hTTPResponse.Results = hTTPResponse.Results.Replace("null","\"\"");
                ChemicalInfoPageModel.ChemicalInfoPageModelBean chemicalInfo = new ChemicalInfoPageModel.ChemicalInfoPageModelBean();
                chemicalInfo = JsonConvert.DeserializeObject<ChemicalInfoPageModel.ChemicalInfoPageModelBean>(hTTPResponse.Results);

                chemicalInfo.molecular_str = ReplaceSubs(chemicalInfo.molecular_str);
                //chemicalInfo.env_impact = "“英文名称”：“2-甲基苯硼酸< Br> 2-甲基苯基硼酸< Br> 2-甲苯基硼酸< Br> AKO-BRN-017BR>邻甲基苯基硼酸< Br>邻甲苯基硼酸< Br> ReaChanm AH Pb 0228 Br＞甲苯-2-硼酸< Br> 2-甲苯基硼酸（2-甲基苯硼酸）< BR> 2-甲基苯基硼酸< BR>邻甲苯硼酸<Br> 2-甲基苯基硼酸<Br>邻甲苯基硼酸，M.97.%Br> 2-甲基苯基硼酸，邻甲苯硼酸< Br> 2-甲基磺酰基苯硼酸< BR> 2-甲基苯基硼酸（含不同量酸酐）< Br> 2-甲基苯基硼酸（邻甲苯基硼酸）< Br> 2-甲苯硼酸盐NIC酸＞Br＞O-硼酸甲苯＞Br> 2-甲苯基硼酸，98%，";
                BindingContext = chemicalInfo;
            }
        }

    }
}
