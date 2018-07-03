using AepApp.AuxiliaryExtension;
using Plugin.Hud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public partial class SelectSitePage : ContentPage
    {
        private AddSiteUtil addSiteUtil;
        private string siteName;
        private string siteUrl;
        public SelectSitePage()
        {
            InitializeComponent();
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
        public void HideCrossHud() {
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
            bool answer = await  DisplayAlert("提示", "是否删除个人账号信息", "确定", "取消");
            if (answer)
            {
                //循环删除所存的数据
                IEnumerable<Account> outs = AccountStore.Create().FindAccountsForService(App.AppName);
                for (int i = 0; i < outs.Count(); i++)
                {
                    AccountStore.Create().Delete(outs.ElementAt(i), App.AppName);
                }
                App.isDeleteInfo = true;
            }
        }
        private void Logout(object sender, EventArgs e)
        {
            //if (App.isAutoLogin)
            //{
                App.isSetToLogin = true;  //从设置按钮进入登入按钮
            Application.Current.MainPage = new NavigationPage(new LoginPage());
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
                addSiteUtil.reqSiteInfo(siteName, siteUrl,dataList);
            }
            //网络请求站点
            sl_add_site.IsVisible = false;
        }
    }
}