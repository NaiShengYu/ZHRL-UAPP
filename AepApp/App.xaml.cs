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
using System.Net;
using AepApp.View.EnvironmentalEmergency;
namespace AepApp
{
    public partial class App : Application, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private const string EmergencyModuleID = "D53E7751-26A7-4B6C-B8E1-E243621A84CF";
        private const string BasicDataModuleID = "D53E7751-26A7-4B6C-B8E1-E243621A84CF"; //基础数据模块id

        public string FrameworkURL = "http://gx.azuratech.com:30000";
        public List<ModuleInfo> Modules = null;
        public ModuleInfo EmergencyModule = null;
        public static ModuleInfo BasicDataModule = null;



        public static bool UseMockDataStore = true;
        public static string BackendUrl = "https://localhost:5000";
        public static string NEWTOKENURL = "http://gx.azuratech.com:30000/token";
        //public static string BasicDataModule = "";//基础数据模块url
        //public static double pid = 3.14;
        public static string BaseUrl = "";
        //新应急接口baseURL
        public static string BaseUrlForYINGJI = "http://192.168.1.128:5000/";
        public static string token = "";
        public static string EmergencyToken = "";
        public static string frameworkToken = "";
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
            MainPage = new NavigationPage(new LoginPage());

            //MainPage = new WindSpeedAndDirectionPage();
        }


        protected async override void OnStart()
        {
            base.OnStart();
            //return;
            //获取存储的账号密码
            acc = (await AccountStore.Create().FindAccountsForServiceAsync(App.appName)).LastOrDefault();

            if (acc == null)
            {
                MainPage = new NavigationPage(new LoginPage());
                return;
            }

            await GetSqlDataAsync();

            string password = acc.Properties["pwd"];
            string username = acc.Username;

            // try auto login
            bool autologgedin = await LoginAsync(username, password);
            if (autologgedin) MainPage = new NavigationPage(new MasterAndDetailPage());
            else MainPage = new NavigationPage(new LoginPage());
        }

        /// <summary>
        /// Login server with provided username and password
        /// </summary>
        /// <param name="username">user name</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            FrameworkToken fwtoken = await GetFrameworkTokenAsync(username, password);

            if (fwtoken == null)
            {

                string url = "http://192.168.1.128:5000//api/TokenAuth/Authenticate"; //无法转换token 先用这个
                ConvertedTokenReqStruct2 parameter2 = new ConvertedTokenReqStruct2
                {
                    userNameOrEmailAddress = "admin",
                    password = "123qwe",
                    rememberClient = "true"
                };
                string param2 = JsonConvert.SerializeObject(parameter2);
                HTTPResponse res2 = await EasyWebRequest.SendHTTPRequestAsync(url, param2, "POST");
                if (res2.StatusCode == HttpStatusCode.OK)
                {
                    var tokenstr = JsonConvert.DeserializeObject<ConvertedTokenResult>(res2.Results);
                    EmergencyToken = tokenstr.result.accessToken;
                }


                return false;
            }
            else {
                string url = "http://192.168.1.128:5000//api/TokenAuth/Authenticate"; //无法转换token 先用这个
                ConvertedTokenReqStruct2 parameter2 = new ConvertedTokenReqStruct2
                {
                    userNameOrEmailAddress = "admin",
                    password = "123qwe",
                    rememberClient = "true"
                };
                string param2 = JsonConvert.SerializeObject(parameter2);
                HTTPResponse res2 = await EasyWebRequest.SendHTTPRequestAsync(url, param2, "POST");
                if (res2.StatusCode == HttpStatusCode.OK)
                {
                    var tokenstr = JsonConvert.DeserializeObject<ConvertedTokenResult>(res2.Results);
                    EmergencyToken = tokenstr.result.accessToken;
                }
                frameworkToken = fwtoken.access_token;  
                return true;

            }      

            Modules = await GetModuleInfoAsync(fwtoken.access_token);
            //D53E7751-26A7-4B6C-B8E1-E243621A84CF

            // if there is no module defined for the site, disable auto login and goto login page
            if (Modules == null)
            {
                return false;
            }
            else
            {
                foreach (ModuleInfo mi in Modules)
                {
                    //if (mi.id == EmergencyModuleID)
                    //{
                    //    EmergencyModule = mi;
                    //    continue;
                    //}
                    if (mi.id == BasicDataModuleID) {
                        BasicDataModule = mi;
                        continue;
                    }
                }
                return false;
            }

            //if (EmergencyModule != null)
            //{
                EmergencyToken = await GetConvertTokenAsync(fwtoken.access_token);
            //}

            if (EmergencyToken != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get an access token from the framework server with the provided credential
        /// </summary>
        /// <param name="username">User Name</param>
        /// <param name="password">Password</param>
        /// <returns>A FrameworkToken structure that contains the access token for all subsequence requests</returns>
        private async Task<FrameworkToken> GetFrameworkTokenAsync(string username, string password)
        {
            try
            {
                string url = FrameworkURL + "/token";
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "username=" + username + "&password=" + password + "&grant_type=password", "POST", null);
                FrameworkToken ft = null;
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    ft = JsonConvert.DeserializeObject<FrameworkToken>(res.Results);
                }
                return ft;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get a list of module names and URLs from the framework server
        /// </summary>
        /// <param name="accesstoken">Access token retrieved from the framework server</param>
        /// <returns>A list of ModuleInfo structures, each of which contains some details about the installed modules</returns>
        private async Task<List<ModuleInfo>> GetModuleInfoAsync(string accesstoken)
        {
            try
            {
                string url = FrameworkURL + "/api/fw/getmodsList";

                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, null, "GET", accesstoken);
                List<ModuleInfo> modules = null;
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    modules = new List<ModuleInfo>();
                    modules = JsonConvert.DeserializeObject<List<ModuleInfo>>(res.Results);
                }
                return modules;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get a converted token to be used for all subsequent requests to the Emergency module server
        /// </summary>
        /// <param name="accesstoken">Access token retrieved from the framework server</param>
        /// <returns>A converted access token for all subsequent requests to the Emergency module server</returns>
        private async Task<string> GetConvertTokenAsync(string accesstoken)
        {
            try
            {
                string url = EmergencyModule.url + "/api/TokenAuth/ExternalAuthenticate";
                url = "http://192.168.1.128:5000//api/TokenAuth/ExternalAuthenticate";

                ConvertedTokenReqStruct parameter = new ConvertedTokenReqStruct
                {
                    authProvider = "AzuraAuth",
                    providerAccessCode = accesstoken
                };
                string param = JsonConvert.SerializeObject(parameter);
                string token = null;

                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST");
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    var tokenstr = JsonConvert.DeserializeObject<ConvertedTokenResult>(res.Results);
                    token = tokenstr.result.accessToken;
                }               
                return token;
            }
            catch
            {
                return null;
            }
        }

        private async Task GetSqlDataAsync()
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

    public class FrameworkToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string userName { get; set; }
    }

    public class ModuleInfo
    {
        public string id { get; set; }
        public string index { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public string initjsurl { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    internal class ConvertedTokenReqStruct
    {
        public string authProvider { get; set; }
        public string providerAccessCode { get; set; }

    }
    internal class ConvertedTokenReqStruct2
    {
        public string userNameOrEmailAddress { get; set; }
        public string password { get; set; }
        public string rememberClient { get; set; }

    }

    internal class ConvertedToken
    {
        public string accessToken { get; set; }
        public string encryptedAccessToken { get; set; }
        public string expireInSeconds { get; set; }
        public string waitingForActivation { get; set; }
    }

    internal class ConvertedTokenResult
    {
        public ConvertedToken result;
    }


}
