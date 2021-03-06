﻿using AepApp.AuxiliaryExtension;
using AepApp.Services;
using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            DependencyService.Get<IPushService>().SetAlias("");
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

                tree.HeightRequest = 90;
                tree.WidthRequest = 90;
                titleSK.Margin = new Thickness(40, 40, 90, 40);
                lab2.Margin = new Thickness(0, 0, 20, 0);
            }
            else if (App.ScreenWidth < 400)
            {
                lab1.FontSize = 22;
                lab2.FontSize = 19;
                //lab3.FontSize = 19;
                tree.HeightRequest = 130;
                tree.WidthRequest = 130;
                titleSK.Margin = new Thickness(40, 40, 150, 40);

            }


            //获取站点URL
            TodoItem item = await AddSiteUtil.getCurrentSite();
            if (item != null)
            {
                site_name.Text = item.Name;
                App.FrameworkURL = item.SiteAddr; //获取baseUrl
                //App.environmentalQualityModel.url = "https://" + item.SiteAddr; //获取baseUrl
                App.siteName = item.customerName;
                if (!string.IsNullOrWhiteSpace(item.appCNname))
                    item.appEnname = string.IsNullOrWhiteSpace(item.appEnname) ? "" : item.appEnname;
                else
                    item.appEnname = string.IsNullOrWhiteSpace(item.appEnname) ? "Azura Environmental Protection Platform" : item.appEnname;
              
                  item.appCNname = string.IsNullOrWhiteSpace(item.appCNname) ? "瑞蓝智慧环保系统" : item.appCNname;
                item.applogo = string.IsNullOrWhiteSpace(item.applogo) ? "tree" : item.applogo;
                tree.Source = ImageSource.FromFile(item.applogo);
                lab1.Text = item.appCNname;
                lab2.Text = item.appEnname;
            }
            else
            {
                site_name.Text = "";
                App.FrameworkURL = ""; //获取baseUrl
            }

            //获取存储文件下的内容
            LoginModel userModel = null;
            List<LoginModel> userModels = await App.Database.GetUserModelAsync();
            if (userModels != null && userModels.Count > 0)
                userModel = userModels[0];
            if (isFirstAppear)
            {
                if (userModel != null)
                {
                    account.Text = userModel.userName;
                    password.Text = userModel.password;
                    remember_pwd.IsToggled = true;
                }
                isFirstAppear = false;
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
            if (string.IsNullOrWhiteSpace(acc))
            {
                await DisplayAlert("提示", "账号不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("账号不能为空");
            }
            else if (string.IsNullOrWhiteSpace(pwd))
            {
                await DisplayAlert("提示", "密码不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("密码不能为空");
            }
            else if (string.IsNullOrWhiteSpace(siteNmae))
            {
                await DisplayAlert("提示", "请先添加站点", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("请先添加站点");
            }
            else
            {
                deleteData();
                bool autologin = await (App.Current as App).LoginAsync(acc, pwd);

                if (autologin && (App.Current as App)._canGo)
                {
                    try
                    {
                            //await Navigation.PushAsync(new MasterAndDetailPage());
                            //为了切换不同的账户，也显示不同的列表
                            if (App.masterAndDetailPage != null)
                                App.masterAndDetailPage = null;
                            App.masterAndDetailPage = new MasterAndDetailPage();
                            Application.Current.MainPage = new NavigationPage(App.masterAndDetailPage);
                            App.OpenMenu(new HomePagePage());
                            if (Navigation.NavigationStack.Count > 1)
                            {
                                Navigation.RemovePage(Navigation.NavigationStack[0]);
                            }
                        
                    }
                    catch (Exception ex)
                    {
                        if (ex != null)
                        {
                            FileUtils.SaveLogFile("登录失败错误：" + ex.ToString());
                        }
                        //DependencyService.Get<IToast>().ShortAlert("登录失败错误："+ex.Message);
                    }
                }
                else
                {
                    await DisplayAlert("提示", "登入失败", "确定");
                }
            }
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Login();

        }

        private async void deleteData()
        {

            ////循环删除所存的数据
             List<LoginModel> userModels = await App.Database.GetUserModelAsync();
            if (userModels != null && userModels.Count > 0)
                foreach (var item in userModels)
                {
                    await App.Database.DeleteUserModelAsync(item);
                }

            if(remember_pwd.IsToggled == true) {
                LoginModel loginModel = new LoginModel
                {
                    ID = 0,
                    userName = account.Text,
                    password = password.Text
                };
                await App.Database.SaveUserModelAsync(loginModel);
            }

        }

    }
}