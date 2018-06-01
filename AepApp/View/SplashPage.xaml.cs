using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashPage : ContentPage
	{
        private TodoItem item;
        private Account acc;
        private string result;

        public SplashPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");
           
        }
        protected override async void OnAppearing()
        {      
            //获取数据库的数据
            ((App)App.Current).ResumeAtTodoId = -1;
            List<TodoItem> todoItems = await App.Database.GetItemsAsync();

            if (todoItems != null && todoItems.Count != 0)
            {
                for (int i = 0; i < todoItems.Count; i++)
                {
                    if (todoItems[i].isCurrent == true)
                    {
                        item = todoItems[i];                    
                        App.BaseUrl = "https://" + item.SiteAddr; //获取baseUrl
                        break;
                    }
                }
                //获取存储文件下的内容
                acc = AccountStore.Create().FindAccountsForService(App.appName).LastOrDefault();
                if (acc != null)
                {
                    AutoLogin(acc);
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());
                    //NavigationPage MainPage = new NavigationPage(new LoginPage());
                }
            }     
        }
        private void AutoLogin(Account account)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                //await getSqlDataAsync();
                string uri = App.BaseUrl + "/api/login/Login";
                LoginPageModels.loginParameter parameter = new LoginPageModels.loginParameter();
                parameter.Password = acc.Properties["pwd"];
                parameter.UserName = acc.Username;
                parameter.rememberStatus = true;
                parameter.sid = item.SiteId;
                parameter.sname = item.Name;
                parameter.userdel = 1;
                string param = JsonConvert.SerializeObject(parameter);
                result = EasyWebRequest.sendPOSTHttpWebRequest(uri, param,false);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                LoginPageModels.haveToken haveToken = new LoginPageModels.haveToken();
                haveToken = JsonConvert.DeserializeObject<LoginPageModels.haveToken>(result);
                if (haveToken.success.Equals("false"))
                {
                    Navigation.PushAsync(new LoginPage());
                    //NavigationPage MainPage = new NavigationPage(new LoginPage());
                }
                else
                {
                    App.token = haveToken.token;
                    App.isAutoLogin = true;
                    Navigation.PushAsync(new MasterAndDetailPage());
                    // NavigationPage MainPage = new NavigationPage(new MasterAndDetailPage());
                }

            };
            wrk.RunWorkerAsync();
        }
    }
}