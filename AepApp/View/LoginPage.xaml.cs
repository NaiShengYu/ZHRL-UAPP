using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Plugin.Hud;
using Xamarin.Forms.Xaml;
using Todo;
using System.ComponentModel;
using CloudWTO.Services;
using AepApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private bool isFirstAppear = true;
        private string result = "";
        private string acc;
        private string pwd;
        private string siteNmae;
        private TodoItem item;

        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        private void Login(object sender, EventArgs e)
        {
            Login();
        }

        private void ReqLoginHttp()
        {
            if (App.BaseUrl.Equals(""))
            {
                DisplayAlert("提示", "请先添加站点", "确定");
            }
            else
            {
                CrossHud.Current.Show("登陆中...");
                BackgroundWorker wrk = new BackgroundWorker();
                wrk.DoWork += (sender1, e1) =>
                {
                    string uri = App.BaseUrl + "/api/login/Login";
                    LoginPageModels.loginParameter parameter = new LoginPageModels.loginParameter();
                    parameter.Password = pwd;
                    parameter.UserName = acc;
                    parameter.rememberStatus = true;
                    parameter.sid = item.SiteId;
                    parameter.sname = item.Name;
                    parameter.userdel = 1;
                    string param = JsonConvert.SerializeObject(parameter);
                    result = EasyWebRequest.sendPOSTHttpWebRequest(uri, param, false);
                };
                wrk.RunWorkerCompleted += (sender1, e1) =>
                {
                    LoginPageModels.haveToken haveToken = new LoginPageModels.haveToken();
                    haveToken = JsonConvert.DeserializeObject<LoginPageModels.haveToken>(result);
                    if (haveToken.success.Equals("false"))
                    {
                        CrossHud.Current.Dismiss();
                        DisplayAlert("提示", "登录失败", "确定");
                    }
                    else
                    {
                        App.token = haveToken.token;
                        CrossHud.Current.Dismiss();
                        App.isAutoLogin = false;

                        //MasterAndDetailPage MainPage = new MasterAndDetailPage();
                        //GoAAAAAA();
                        //Navigation.PopAsync();                                            

                        Navigation.PushModalAsync(new MasterAndDetailPage());

                        //var nav6 = new NavigationPage((Page)Activator.CreateInstance(typeof(MasterAndDetailPage)));                    
                        //Navigation.PushAsync(new AirPage());
                        //Navigation.PushAsync(new TestOxyPage());
                    }

                };
                wrk.RunWorkerAsync();
            }
        }
        private void Select_site(object sender, EventArgs e)
        {
            //DependencyService.Get<Sample.IToast>().ShortAlert("账号不能为空");
            Navigation.PushAsync(new SelectSitePage());
        }

        protected override async void OnAppearing()
        {
            if (App.ScreenWidth < 374)
            {
                lab1.FontSize = 20;
                lab2.FontSize = 17;
                lab3.FontSize = 17;
                tree.HeightRequest = 100;
                tree.WidthRequest = 100;
            }
            else if (App.ScreenWidth < 400)
            {
                lab1.FontSize = 22;
                lab2.FontSize = 19;
                lab3.FontSize = 19;
                tree.HeightRequest = 130;
                tree.WidthRequest = 130;
            }

            //获取数据库的数据
            ((App)App.Current).ResumeAtTodoId = -1;
            List<TodoItem> todoItems = await App.Database.GetItemsAsync();

            if (todoItems != null && todoItems.Count != 0)
            {

                for (int i = 0; i < todoItems.Count; i++)
                {
                    item = todoItems[i];
                    if (item.isCurrent == true)
                    {
                        site_name.Text = item.Name;
                        App.BaseUrl = "https://" + item.SiteAddr; //获取baseUrl
                    }
                }
            }
            //获取存储文件下的内容
            var acc = AccountStore.Create().FindAccountsForService(App.appName).LastOrDefault();

            var siteData = AccountStore.Create().FindAccountsForService(App.SiteData).LastOrDefault();
            if (isFirstAppear)
            {
                if (acc != null)
                {
                    account.Text = acc.Username;
                    password.Text = acc.Properties["pwd"];
                    remember_pwd.IsToggled = true;
                }
                isFirstAppear = false;
                //Login();
            }
            else if (App.isDeleteInfo)
            {
                account.Text = "";
                password.Text = "";
                App.isDeleteInfo = false;
            }
        }

        private void Login()
        {
            //Navigation.PushAsync(new MapPage());
            acc = account.Text;
            pwd = password.Text;
            siteNmae = site_name.Text;
            if (acc == null || acc.Length == 0)
            {

                DisplayAlert("提示", "账号不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("账号不能为空");
            }
            else if (pwd == null || pwd.Length == 0)
            {
                DisplayAlert("提示", "密码不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("密码不能为空");
            }
            else if (siteNmae == null || siteNmae.Length == 0)
            {
                DisplayAlert("提示", "请先添加站点", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("请先添加站点");
            }
            else
            {
                //循环删除所存的数据
                IEnumerable<Account> outs = AccountStore.Create().FindAccountsForService(App.appName);
                for (int i = 0; i < outs.Count(); i++)
                {
                    AccountStore.Create().Delete(outs.ElementAt(i), App.appName);
                }
                if (remember_pwd.IsToggled)
                {
                    Account count = new Account
                    {
                        Username = acc
                    };
                    count.Properties.Add("pwd", pwd);
                    AccountStore.Create().Save(count, App.appName);
                }
                //请求登陆
                //ReqLoginHttp();
                //新接口
                ReqNewLoginToken(acc, pwd);
            }
        }

        private void ReqNewLoginToken(string userName, string password)
        {
            CrossHud.Current.Show("登陆中...");
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                result = EasyWebRequest.sendPOSTHttpWebRequest(App.NEWTOKENURL, "username=" + userName + "&password=" + password + "&grant_type=password", true);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                try{
                    LoginPageModels.newToken newToken = new LoginPageModels.newToken();
                    newToken = JsonConvert.DeserializeObject<LoginPageModels.newToken>(result);
                    GetDiffPlatformUrl(newToken.access_token);   
                }
                catch (Exception ex)
                {
                    CrossHud.Current.ShowError(ex.Message, timeout: new TimeSpan(0, 0, 3));
                }
               
            };
            wrk.RunWorkerAsync();
        }

        private void GetDiffPlatformUrl(string newToken)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                result = EasyWebRequest.sendGetHttpWebRequestWithToken("http://192.168.1.128:30000/api/fw/getmodsList", newToken);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                //获取各个模块的url，具体怎么用还不清楚~~~
                List<DifferentPlantFormUrl.DiffPlantFormUrlModle> plantFormUrlModle = new List<DifferentPlantFormUrl.DiffPlantFormUrlModle>();
                plantFormUrlModle = JsonConvert.DeserializeObject<List<DifferentPlantFormUrl.DiffPlantFormUrlModle>>(result);
                GetConvertToken(newToken);
            };
            wrk.RunWorkerAsync();
        }

        private void GetConvertToken(string newToken)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                LoginPageModels.newConvertTokenParameter parameter = new LoginPageModels.newConvertTokenParameter
                {
                    authProvider = "AzuraAuth",
                    providerAccessCode = newToken
                };
                string param = JsonConvert.SerializeObject(parameter);
                result = EasyWebRequest.sendPOSTHttpWebRequest(App.BaseUrlForYINGJI + DetailUrl.ConvertToken, param, false);
            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                LoginPageModels.convertToken convertToken = new LoginPageModels.convertToken();
                convertToken = JsonConvert.DeserializeObject<LoginPageModels.convertToken>(result);
                App.convertToken = convertToken.accessToken;
                CrossHud.Current.Dismiss();
               Navigation.PushModalAsync(new MasterAndDetailPage());
            };
            wrk.RunWorkerAsync();
        }
    }
}