﻿using AepApp.AuxiliaryExtension;
using AepApp.Models;
using AepApp.Services;
using Plugin.Hud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Todo;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectSitePage : ContentPage
    {
        private AddSiteUtil addSiteUtil;
        private string siteName;
        private string siteUrl;


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged");

        }

        public SelectSitePage()
        {
            InitializeComponent();

            //键盘高度改变
            MessagingCenter.Unsubscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged");
            MessagingCenter.Subscribe<ContentPage, KeyboardSizeModel>(this, "keyBoardFrameChanged", (ContentPage arg1, KeyboardSizeModel arg2) =>
            {

                Console.WriteLine(arg2.Height);
                Console.WriteLine(arg2.Wight);
                if (arg2.Y != App.ScreenHeight)
                {
                    sl_add_site.TranslationY = -arg2.Height + 60;
                }
                else
                {
                    sl_add_site.TranslationY = 0;
                }
            });

            this.Title = "选择站点";

            HeaderList.ItemSelected += (sender, e) =>
            {
                TodoItem item = (TodoItem)e.SelectedItem;

                for (int i = 0; i < dataList.Count; i++)
                {
                    TodoItem todoitem = dataList[i];
                    if (todoitem.isCurrent == true)//找到上一个选中的站点
                    {
                        if (todoitem.SiteAddr == item.SiteAddr)//如果选中的站点和原来的相同退出循环
                            return;
                        todoitem.isCurrent = false;
                        App.Database.SaveItemAsync(todoitem);

                    }
                }

                item.isCurrent = true;
                App.Database.SaveItemAsync(item);

                //HeaderList.ItemsSource = dataList;
                //DependencyService.Get<Sample.IToast>().ShortAlert(a + "");
                //HeaderList.SelectedItem = a;
            };
            HeaderList.ItemTapped += HeaderList_ItemTapped;
        }

        private void HeaderList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //var a = e.SelectedItem;
            //DependencyService.Get<Sample.IToast>().ShortAlert(a + "");
            //HeaderList.SelectedItem = a;
            //throw new NotImplementedException();
        }

        private void Select_site(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new AddSite());
            //DependencyService.Get<Sample.IToast>().ShortAlert("选择站点");
            //Navigation.PushAsync(new SelectSitePage());
        }

        private void deleteSite(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            var model = item.CommandParameter as TodoItem;
            dataList.Remove(model);
            App.Database.DeleteItemAsync(model);
        }


        ObservableCollection<TodoItem> dataList = new ObservableCollection<TodoItem>();

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            dataList.Clear();
            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtTodoId = -1;
            //TodoItemDatabase database =  App.Database;          
            List<TodoItem> todoItems = await App.Database.GetItemsAsync();
            for (int i = 0; i < todoItems.Count; i++)
            {
                TodoItem todoitem = todoItems[i];
                dataList.Add(todoitem);

            }
            HeaderList.ItemsSource = dataList;

            //Console.WriteLine("数据库内容:" + todoItems);
        }

        internal void SaveData(TodoItem todoItem)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                TodoItem todoitem = dataList[i];
                if (todoitem.isCurrent == true)//找到原来选中的那个点
                {
                    todoitem.isCurrent = false;
                    App.Database.SaveItemAsync(todoitem);

                }
            }
            dataList.Add(todoItem);
            App.Database.SaveItemAsync(todoItem);
            //收起列表
            CloseAddSitePage(false);
        }

        public void CloseAddSitePage(bool isAlert)
        {
            if (isAlert)
            {
                DependencyService.Get<Sample.IToast>().ShortAlert("存在该站点");
            }
            HideCrossHud();
            sl_add_site.IsVisible = false;
        }
        public void HideCrossHud()
        {
            CrossHud.Current.Dismiss();
        }
        private void Show_Add_Site(object sender, EventArgs e)
        {
            Console.WriteLine("显示站点");
            sl_add_site.IsVisible = true;
        }
        private void Hide_Add_Site(object sender, EventArgs e)
        {
            Console.WriteLine("隐藏站点");
            sl_add_site.IsVisible = false;
        }
        private async void Delete_Info(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("提示", "是否删除个人账号信息", "确定", "取消");
            if (answer)
            {
                ////循环删除所存的数据
                //IEnumerable<Account> outs = AccountStore.Create().FindAccountsForService(App.AppName);
                //for (int i = 0; i < outs.Count(); i++)
                //{
                //    AccountStore.Create().Delete(outs.ElementAt(i), App.AppName);
                //}
                List<LoginModel> userModels = await App.Database.GetUserModelAsync();
                if (userModels != null && userModels.Count > 0)
                    foreach (var item in userModels)
                    {
                        await App.Database.DeleteUserModelAsync(item);
                    }

                App.isDeleteInfo = true;
            }
        }
        private void Logout(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count == 1)
            {
                return;
            }
            //if (App.isAutoLogin)
            //{
            App.isSetToLogin = true;  //从设置按钮进入登入按钮
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            App.FrameworkToken = "";
            App.gridUser = null;//清空数据
            App.EP360Module = null;
            App.SamplingModule = null;
            App.BasicDataModule = null;
            App.EmergencyModule = null;
            App.environmentalQualityModel = null;
            App.moduleConfigEmergency = null;
            App.moduleConfigSampling = null;
            App.moduleConfigEP360 = null;
            App.moduleConfigBasicData = null;
            App.moduleConfigEmergency = null;
            App.moduleConfigENVQ = null;
            App.moduleConfigEmergency = null;
            App.moduleConfigEmergency = null;
            App.userDepartments = null;

            App.userInfo = null;
            App._autologgedin = false;
            App._isSampling = false;
            App._isEmergency = false;
            App._ISBasicData = false;
            App._isEP360 = false;
            App._isenvironmental = false;
            //Navigation.PushAsync(new LoginPage());  //自动登陆界面进去
            //}
            //else {
            //    Navigation.PopToRootAsync(); // 从登陆界面进去
            //}                     
        }
        private void Add_Site(object sender, EventArgs e)
        {
            Console.WriteLine("添加站点");
            if (addSiteUtil == null)
            {
                addSiteUtil = new AddSiteUtil(this);
            }
            //判断输入框中是否为空
            siteName = site_name.Text;
            siteUrl = site_url.Text;
            if (siteName == null || siteName.Length == 0)
            {
                DisplayAlert("提示", "账号不能为空", "确定");
            }
            else if (siteUrl == null || siteUrl.Length == 0)
            {
                DisplayAlert("提示", "网址不能为空", "确定");
            }
            else
            {
                //CrossHud.Current.Show("加载中...");
                addSiteUtil.reqSiteInfo(siteName, siteUrl, dataList);
            }
            //网络请求站点
            sl_add_site.IsVisible = false;
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //第一次进入
            if (Navigation.NavigationStack.Count == 1)
            {
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                if (App.userInfo != null)
                    Logout(new object(), new EventArgs());
                else
                    Navigation.PopAsync();
            }
        }
    }
}