using CloudWTO.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddSite : ContentPage
	{
        private String result;
        private String site;
		public AddSite ()
		{
			InitializeComponent ();          
            this.Title = "添加站点";
            ToolbarItems.Add(new ToolbarItem("添加", "",  () =>
            {
                //请求该站点数据
                ReqSiteData();
               // App.Database.SaveItemAsync();
               //await Navigation.PopAsync(false);

            }));
        }

        private void ReqSiteData()
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                site = this.siteAddr.Text;
                string uri = "https://192.168.2.97/api/login/getstationName";               
                result = EasyWebRequest.sendGetHttpWebRequestWithNoToken(uri);
                //string parama = "Password=" + pwd + "&" + "UserName=" + account + "&" + "grant_type=password";
                //try
                //{
                //    result = EasyWebRequest.sendPOSTHttpWebRequest(uri, parama);

                //}
                //catch (Exception ex)
                //{
                //    DisplayAlert("错误提示", ex.Message, "OK");
                //}

            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                Console.WriteLine("ex:" + result);

            };
            wrk.RunWorkerAsync();
        }
    }
}