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

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private bool isFirstAppear = true;
        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");
           
        }

        private void Login(object sender, EventArgs e)
        {
            var acc = account.Text;
            var pwd = password.Text;
            var siteNmae = site_name.Text;
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
                if (remember_pwd.IsToggled)
                {
                    Account count = new Account
                    {
                        Username = acc
                    };
                    count.Properties.Add("pwd", pwd);
                    AccountStore.Create().Save(count, App.appName);
                }
                else
                {
                    //循环删除所存的数据
                    IEnumerable<Account> outs = AccountStore.Create().FindAccountsForService(App.appName);
                    for (int i = 0; i < outs.Count(); i++)
                    {
                        AccountStore.Create().Delete(outs.ElementAt(i), App.appName);
                    }
                }
                //请求登陆
                ReqLoginHttp();
            }
        }

        private void ReqLoginHttp()
        {
            Navigation.PushAsync(new Platform());
            //BackgroundWorker wrk = new BackgroundWorker();
            //wrk.DoWork += (sender1, e1) =>
            //{
            //  //http请求

            //};
            //wrk.RunWorkerCompleted += (sender1, e1) =>
            //{
            //    Navigation.PushAsync(new EnvironmentalPlatform());
            //};
            //wrk.RunWorkerAsync();
        }

        private void Select_site(object sender, EventArgs e)
        {
            //DependencyService.Get<Sample.IToast>().ShortAlert("账号不能为空");
            Navigation.PushAsync(new SelectSitePage());
        }

        protected override async void OnAppearing()
        {
            //获取数据库的数据
            ((App)App.Current).ResumeAtTodoId = -1;
            List<TodoItem> todoItems = await App.Database.GetItemsAsync();
            if (todoItems != null && todoItems.Count != 0 ) {
                TodoItem item = todoItems[App.itemNum];
                site_name.Text = item.Name;
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
            }          
        }
    }
}