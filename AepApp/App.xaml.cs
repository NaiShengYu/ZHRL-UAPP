using System;
using System.Collections.Generic;
using System.IO;
using AepApp.Interface;
using AepApp.View;
using Todo;
using Xamarin.Forms;
using Newtonsoft.Json;

using AepApp.View.Monitor;
using System.Collections.ObjectModel;
using Xamarin.Auth;
using System.Linq;
using System.ComponentModel;
using AepApp.Models;
using CloudWTO.Services;
using System.Threading.Tasks;

namespace AepApp
{
    public partial class App : Application, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static bool UseMockDataStore = true;
        public static string BackendUrl = "https://localhost:5000";
        public static string NEWTOKENURL = "http://gx.azuratech.com:30000/token";    
        //public static double pid = 3.14;
        public static string BaseUrl = "";
        //新应急接口baseURL
        public static string BaseUrlForYINGJI = "http://192.168.1.128:5000/";
        public static string token = "";
        public static string convertToken = "";
        public static string appName = "Aep";
        public static string appAccident = "Accident";
        public static string SiteData = "site";
        public static bool isDeleteInfo = false;
        public static bool isAutoLogin = false;
        public static bool isSetToLogin = false;

        public static List<TodoItem> todoItemList = new List<TodoItem>();

        static TodoItemDatabase database;

        private bool _isMasterDetailPageGestureEnabled = true;
        public bool IsMasterDetailPageGestureEnabled
        {
            get
            {
                return _isMasterDetailPageGestureEnabled;
            }
            set
            {
                _isMasterDetailPageGestureEnabled = value;

                NotifyPropertyChanged("IsMasterDetailPageGestureEnabled");
            }
        }


        private string result;
        private TodoItem item;
        private Account acc;

        public App()
        {
            InitializeComponent();
            //页面启动必须要有一个mainPage
            MainPage = new NavigationPage(new LoginPage());
        }


        protected async override void OnStart()
        {
            base.OnStart();
            //获取存储的账号密码
            acc = AccountStore.Create().FindAccountsForService(App.appName).LastOrDefault();
            if (acc != null)
            {
                await getSqlDataAsync();
                //AutoLogin(acc);//自动登陆
                //新接口
                GetNewToken(NEWTOKENURL, acc);
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void GetNewToken(string url, Account account)
        {
            string password = account.Properties["pwd"];
            string userName = account.Username;
            SendNewHttpLogin(url, "username=" + userName + "&password=" + password + "&grant_type=password");
        }

        private void SendNewHttpLogin(string url, string param)
        {
            result = EasyWebRequest.sendPOSTHttpWebRequest(url, param, true);
            if (result.Contains("error"))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {

            }
            Console.WriteLine(result);
        }

        private async System.Threading.Tasks.Task getSqlDataAsync()
        {
            //获取数据库的数据
            ((App)App.Current).ResumeAtTodoId = -1;
            List<TodoItem> todoItems = await Database.GetItemsAsync();

            if (todoItems != null && todoItems.Count != 0)
            {

                for (int i = 0; i < todoItems.Count; i++)
                {
                    item = todoItems[i];
                    if (item.isCurrent == true)
                    {
                        App.BaseUrl = "https://" + item.SiteAddr; //获取baseUrl    
                        break;
                    }
                }
            }
        }

        private void AutoLogin(Account account)
        {
            string uri = App.BaseUrl + "/api/login/Login";
            LoginPageModels.loginParameter parameter = new LoginPageModels.loginParameter
            {
                Password = account.Properties["pwd"],
                UserName = account.Username,
                rememberStatus = true,
                sid = item.SiteId,
                sname = item.Name,
                userdel = 1
            };

            string p = JsonConvert.SerializeObject(parameter);
            SendHttpLogin(uri, p);

            //BackgroundWorker wrk = new BackgroundWorker();
            //wrk.DoWork +=  (sender1, e1) =>
            //{
            //};
            //wrk.RunWorkerCompleted += (sender1, e1) =>
            //{
            //    //LoginPageModels.haveToken haveToken = new LoginPageModels.haveToken();
            //    //haveToken = JsonConvert.DeserializeObject<LoginPageModels.haveToken>(result);
            //    //if (haveToken.success.Equals("false"))
            //    //{
            //        MainPage = new NavigationPage(new LoginPage());
            //    //}
            //    //else
            //    //{
            //    //    App.token = haveToken.token;
            //    //    isAutoLogin = true;
            //    //    MainPage = new NavigationPage(new MasterAndDetailPage());
            //    //}

            //};
            //wrk.RunWorkerAsync();
        }

        private void SendHttpLogin(string uri, string p)
        {
            // BackgroundWorker wrk = new BackgroundWorker();
            // wrk.DoWork += (sender1, e1) =>
            //{
            result = EasyWebRequest.sendPOSTHttpWebRequest(uri, p, false);
            //};
            // wrk.RunWorkerCompleted += (sender1, e1) =>
            // {
            LoginPageModels.haveToken haveToken = new LoginPageModels.haveToken();
            haveToken = JsonConvert.DeserializeObject<LoginPageModels.haveToken>(result);
            if (haveToken.success.Equals("false"))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                App.token = haveToken.token;
                isAutoLogin = true;
                MainPage = new NavigationPage(new MasterAndDetailPage());
            }
            //};
            //wrk.RunWorkerAsync();
        }

        public static TodoItemDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new TodoItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("TodoSQLite.db3"));
                }
                return database;
            }
        }
        public int ResumeAtTodoId { get; set; }


        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
    }
}
