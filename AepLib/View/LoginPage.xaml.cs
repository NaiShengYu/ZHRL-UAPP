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
                        //App.BaseUrl = "https://" + item.SiteAddr; //获取baseUrl
                    }
                }
            }
            //获取存储文件下的内容
            var acc = AccountStore.Create().FindAccountsForService(App.AppName).LastOrDefault();

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

        private async void Login()
        {
            //Navigation.PushAsync(new MapPage());
            acc = account.Text;
            pwd = password.Text;
            siteNmae = site_name.Text;
            if (acc == null || acc.Length == 0)
            {
                await DisplayAlert("提示", "账号不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("账号不能为空");
            }
            else if (pwd == null || pwd.Length == 0)
            {
                await DisplayAlert("提示", "密码不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("密码不能为空");
            }
            else if (siteNmae == null || siteNmae.Length == 0)
            {
                await DisplayAlert("提示", "请先添加站点", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("请先添加站点");
            }
            else
            {
                bool autologin = await (App.Current as App).LoginAsync(acc, pwd);

                if (autologin)
                {
                    await Navigation.PushAsync(new MasterAndDetailPage());
                    // save the credential only after successful login
                    deleteData();
                    if(Navigation.NavigationStack.Count > 1)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[0]);
                    }
                }
                else
                {
                    await DisplayAlert("提示", "登入失败", "确定");
                }
            }
        }

        private void deleteData()
        {

#if !(DEBUG && __IOS__)

            //循环删除所存的数据
            IEnumerable<Account> outs = AccountStore.Create().FindAccountsForService(App.AppName);
            for (int i = 0; i < outs.Count(); i++)
            {
                AccountStore.Create().Delete(outs.ElementAt(i), App.AppName);
            }
            if (remember_pwd.IsToggled)
            {
                Account count = new Account
                {
                    Username = acc
                };
                count.Properties.Add("pwd", pwd);
                AccountStore.Create().Save(count, App.AppName);
            }
#endif
        }

    }
}