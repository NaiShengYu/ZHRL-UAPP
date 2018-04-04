using CloudWTO.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Hud;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddSite : ContentPage
	{
        private String result;
		public AddSite ()
		{
			InitializeComponent ();          
            this.Title = "添加站点";
            ToolbarItems.Add(new ToolbarItem("添加", "",  () =>
            {
                //请求该站点数据
                reqSiteData();
               // App.Database.SaveItemAsync();
               //await Navigation.PopAsync(false);

            }));
        }

        private void reqSiteData()
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                Console.WriteLine("添加站点");
                var aaaa = this.siteAddr.Text;
                if(aaaa.Length == 0 || this.siteAddr.Text ==null){
                    CrossHud.Current.ShowError(message: "请输入站点IP");
                    return;
                }
                string uri = "https://"+this.siteAddr.Text+"/api/login/getstationName?stationurl="+this.siteAddr.Text;               
                result = EasyWebRequest.sendGetHttpWebRequestWithNoToken(uri);          
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                Console.WriteLine("ex:" + result);
                //添加站点

            };
            wrk.RunWorkerAsync();
        }
    }
}