using System;
using System.Collections.Generic;
using System.IO;
using AepApp.Interface;
using AepApp.View;
using Todo;
using Xamarin.Forms;
using Newtonsoft.Json;
namespace AepApp
{
    public partial class App : Application
    {
        public static bool UseMockDataStore = true;
        public static string BackendUrl = "https://localhost:5000";      
        //public static double pid = 3.14;
        public static string BaseUrl = "";
        public static string token = "";
        public static string appName = "Aep";
        public static string SiteData = "site";

        public static int itemNum = 0;
        public static List<TodoItem> todoItemList = new List<TodoItem> ();
        static TodoItemDatabase database;

        public App()
        {
            InitializeComponent();
           

            //if (UseMockDataStore)
            //    DependencyService.Register<MockDataStore>();
            //else
            //    DependencyService.Register<CloudDataStore>();

            //if (Device.RuntimePlatform == Device.iOS)
            //    MainPage = new MainPage();
            //else
            //    MainPage = new NavigationPage(new MainPage());

            MainPage = new NavigationPage(new LoginPage());
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
