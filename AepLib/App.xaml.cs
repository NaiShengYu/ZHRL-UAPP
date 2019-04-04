using AepApp.AuxiliaryExtension;
using AepApp.Interface;
using AepApp.Models;
using AepApp.Services;
//using AepApp.View.SecondaryFunction;
using AepApp.Tools;
using AepApp.View;
using AepApp.View.EnvironmentalEmergency;
using AepApp.ViewModel;
using AepApp.ViewModels;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Todo;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AepApp
{
    public partial class App : Application, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //最底层网格员
        public static int GridMaxLevel = 3;
        public static UploadEmergencyShowModel LastNatureAccidentModel = null;
        public static MasterDetailPage appHunbegerPage = null;
        public static Location currentLocation = null;
        public const string EmergencyModuleID = "99A2844E-DF79-41D1-8CC4-CE98074CF31A";
        public const string BasicDataModuleID = "D53E7751-26A7-4B6C-B8E1-E243621A84CF"; //基础数据模块id
        public const string EP360ModuleID = "C105368C-7AF6-49C8-AED3-6A0C7A9E3F7B";
        public const string SamplingModuleID = "65B3E603-4493-44CA-953A-685513B01298";
        public const string SimVisModuleID = "4C534464-AD7D-42FF-80AF-0049CDC6A9F6";
        public const string environmentalQualityID = "3DB80834-1FAA-4062-AAC9-3FAB6B390B4D";//环保监测预警平台基础数据

        public static string siteName = "";
        public static string FrameworkURL = "";
        //public static string FrameworkURL = "http://gx.azuratech.com:30000";
        //public static string FrameworkURL = "http://dev.azuratech.com:50000";
        public static string SampleURL = "http://192.168.1.128:30011";//采样模块暂时接口

        //public static int TYPE_GAOXIN = 1;//高新区
        //public static int TYPE_YISHUI = 2;//沂水
        //public static int APP_TYPE = TYPE_GAOXIN;

        public static UserInfoModel userInfo = null;
        public static GridUserInfoModel gridUser = null;
        public static ObservableCollection<UserDepartmentsModel> userDepartments = null;
        public static ObservableCollection<GridAllDepartmentsModel> allDepartments = null;

        public static ModuleConfigEP360 moduleConfigEP360 = null;
        public static ModuleConfigSampling moduleConfigSampling = null;
        public static ModuleConfigEmergency moduleConfigEmergency = null;//应急模块需要展示的内容
        public static ModuleConfigENVQ moduleConfigENVQ = null;//环境质量需要展示的内容
        public static ModuleConfigBasicData moduleConfigBasicData = null;

        public List<ModuleInfo> Modules = null;
        public static ModuleInfo EmergencyModule = null;//应急
        public static ModuleInfo BasicDataModule = null;
        public static ModuleInfo EP360Module = null;
        public static ModuleInfo SamplingModule = null;//采样
        public static ModuleInfo SimVisModule = null;//
        public static ModuleInfo environmentalQualityModel = null;

        public static TestPersonViewModel personViewModel = null;
        public static EmergencyAccidentPageModels.ItemsBean EmergencyAccidengtModel = null;

        public static MySamplePlanResult mySamplePlanResult = null;

        //样本类型数组
        public static List<SampleTypeModel> sampleTypeList = null;

        //"上传数据"关键污染物列表数组
        public static List<AddDataIncidentFactorModel.ItemsBean> contaminantsList = null;
        public static List<AddDataIncidentFactorModel.ItemsBean> AppLHXZList = null;

        public static string FrameworkToken = "";       // Returned by the framework server. To be used as the ONLY access token throughout the APP
        public static string EmergencyToken = "";        // used temporarily for the emergency module
        public static string EmergencyAccidentID = "";


        // needed in AccountStore for credential storing
        public static string AppName = "Aep";
        public static string SiteData = "site";

        ///////////////////////////
        public static string token = "";    // old app token
        public static bool isDeleteInfo = false;
        public static bool isAutoLogin = false;
        public static bool isSetToLogin = false;
        ///////////////////////////

        public static List<TodoItem> todoItemList = new List<TodoItem>();

        static TodoItemDatabase database;
        public static SplashPage splashPage;
        public static MasterAndDetailPage masterAndDetailPage;

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


        //private NetworkAccess _NetworkState = Connectivity.NetworkAccess;
        //public NetworkAccess NetworkState {
        //    get { return _NetworkState; }
        //    set { _NetworkState = value; NotifyPropertyChanged("NetworkState"); }
        //}


        private TodoItem item;
        private Account acc;

        public static VM vm { get; set; }

        public App()
        {
            InitializeComponent();
            vm = new VM();
            HandleEventHandler();
            //MainPage = new HomePagePage();
            //MainPage = new NavigationPage(new SamplePlanPage());

            splashPage = new SplashPage();
            MainPage = splashPage;
            personViewModel = new TestPersonViewModel();

            //MainPage = new TestOxyPage();

            //MainPage = new NavigationPage(new SendInformationPage());
            //aaaa();

            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

        }

        protected void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args == null || args.ExceptionObject == null)
            {
                return;
            }
            Exception e = (Exception)args.ExceptionObject;
            // log won't be available, because dalvik is destroying the process
            //Log.Debug (logTag, "MyHandler caught : " + e.Message);
            // instead, your err handling code shoudl be run:
            String content = args.ExceptionObject.ToString();
            Console.WriteLine(content);
            FileUtils.SaveLogFile(content);
        }



        //使用后删除错误数据
        async void aaaa()
        {
            List<UploadEmergencyModel> dataList2 = await App.Database.GetEmergencyAsync();
            foreach (UploadEmergencyModel model in dataList2)
            {
                await App.Database.DeleteEmergencyAsync(model);
            }
        }

        //获取当前位置
        async void HandleEventHandler()
        {

            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    currentLocation = await Geolocation.GetLastKnownLocationAsync();
                }
                else
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    currentLocation = await Geolocation.GetLocationAsync(request);
                }

                if (currentLocation == null) HandleEventHandler();
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
        protected async override void OnStart()
        {
            base.OnStart();

            //MainPage = new NavigationPage(new EditContentPage("检查大队", true, ""));

            //return;

            //if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            //{
            //    bool al = await LoginAsync("admin", "123456");
            //    if (al) MainPage = new NavigationPage(new MasterAndDetailPage());
            //    else MainPage = new NavigationPage(new LoginPage());

            //    return;
            //}

            //return;
            //获取站点URL
            TodoItem item = await AddSiteUtil.getCurrentSite();
            if(item == null)
            {
                MainPage = new NavigationPage(new SelectSitePage());
                return;
            }
            if (item != null)
            {
                App.FrameworkURL = item.SiteAddr; //获取baseUrl
                //App.BaseUrl = "https://" + item.SiteAddr; //获取baseUrl
                App.siteName = item.customerName;
            }



            //获取存储的账号密码
            acc = (await AccountStore.Create().FindAccountsForServiceAsync(App.AppName)).LastOrDefault();

            if (acc == null)
            {
                MainPage = new NavigationPage(new LoginPage());
                return;
            }

            await GetSqlDataAsync();

            string password = acc.Properties["pwd"];
            string username = acc.Username;

            // try auto login
            _autologgedin = await LoginAsync(username, password);
            if (_autologgedin && _canGo)
            {
                if (App.masterAndDetailPage == null)
                    App.masterAndDetailPage = new MasterAndDetailPage();
                Application.Current.MainPage = new NavigationPage(App.masterAndDetailPage);
            }
            else MainPage = new NavigationPage(new LoginPage());
        }

        public static bool _autologgedin = false;
        public static bool _isSampling = false;
        public static bool _isEmergency = false;
        public static bool _ISBasicData = false;
        public static bool _isEP360 = false;
        public static bool _isenvironmental = false;

        public bool _canGo = false;

        private void canGo()
        {

            if (_autologgedin == true &&
               _isSampling == true &&
               _isEmergency == true &&
               _ISBasicData == true &&
               _isEP360 == true &&
               _isenvironmental == true)
            {
                _canGo = true;
                ////if(masterAndDetailPage == null)
                //masterAndDetailPage = new MasterAndDetailPage();
                //MainPage = new NavigationPage(masterAndDetailPage);
                //_autologgedin = false;
                //_isSampling = false;
                //_isEmergency = false;
                //_ISBasicData = false;
                //_isEP360 = false;
                //_isenvironmental = false;
            }
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
            if (fwtoken == null) return false;
            App.userInfo = await getUserInfoAsync(fwtoken.access_token);//获取用户信息
            if (App.userInfo == null) return false;
            if(App.userInfo.id != null)//设置JPush别名
            {
                DependencyService.Get<IPushService>().SetAlias(App.userInfo.id.ToString());
            }
            _autologgedin = true;
            FrameworkToken = fwtoken.access_token;

            Modules = await GetModuleInfoAsync(fwtoken.access_token);

            // if there is no module defined for the site, disable auto login and goto login page
            if (Modules == null)
            {
                return false;
            }
            else
            {

                try
                {
                    foreach (ModuleInfo mi in Modules)
                    {
                        switch (mi.id.ToUpper())
                        {
                            case EmergencyModuleID: EmergencyModule = mi; break;
                            case BasicDataModuleID: BasicDataModule = mi; break;
                            case EP360ModuleID: EP360Module = mi; break;
                            case SamplingModuleID: SamplingModule = mi; break;
                            case SimVisModuleID: SimVisModule = mi; break;
                            case environmentalQualityID: environmentalQualityModel = mi; break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        FileUtils.SaveLogFile("mi.id.ToUpper错误：" + ex.ToString());
                    }
                    //DependencyService.Get<IToast>().ShortAlert("mi.id.ToUpper错误："+ex.Message);
                }
                try
                {
                    List<Task> tasks = new List<Task>();
               
                    if (EP360Module != null && EP360Module.status.Equals("0")) { tasks.Add(GetModuleConfigEP360()); } else _isEP360 = true;
                    if (SamplingModule != null && SamplingModule.status.Equals("0")) tasks.Add(GetModuleConfigSampling()); else _isSampling = true;
                    if (BasicDataModule != null && BasicDataModule.status.Equals("0")) tasks.Add(GetModuleConfigFramework()); else _ISBasicData = true;
                    if (EmergencyModule != null && EmergencyModule.status.Equals("0")) tasks.Add(postEmergencyReq()); else _isEmergency = true;
                    if (environmentalQualityModel != null && environmentalQualityModel.status.Equals("0")) { tasks.Add(postEnvironmentalReq()); } else _isenvironmental = true;
                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        FileUtils.SaveLogFile("Task.WhenAll错误：" + ex.ToString());
                    }
                   
                }




            }

            if (EmergencyModule != null)
            {
                // for emergency module temorarily
                //正式环境去掉下面部分

                //string url = EmergencyModule.url + "/api/TokenAuth/Authenticate"; //无法转换token 先用这个
                //ConvertedTokenReqStruct2 parameter2 = new ConvertedTokenReqStruct2
                //{
                //    userNameOrEmailAddress = "admin",
                //    password = "123qwe",
                //    rememberClient = "true"
                //};
                //string param2 = JsonConvert.SerializeObject(parameter2);
                //HTTPResponse res2 = await EasyWebRequest.SendHTTPRequestAsync(url, param2, "POST");
                //if (res2.StatusCode == HttpStatusCode.OK)
                //{
                //    var tokenstr = JsonConvert.DeserializeObject<ConvertedTokenResult>(res2.Results);
                //    EmergencyToken = tokenstr.result.accessToken;
                //}

                EmergencyToken = await GetConvertTokenAsync(fwtoken.access_token);

                //EmergencyToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6IjE4MmVjZTI2LTBjNGItYTg0Ny0wYmJiLTM5ZTY2ZjAzN2M3YiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiaHR0cDovL3d3dy5hc3BuZXRib2lsZXJwbGF0ZS5jb20vaWRlbnRpdHkvY2xhaW1zL3RlbmFudElkIjoiMSIsInN1YiI6IjciLCJqdGkiOiJhNmIyYmJlZi03ZTQ3LTQ1M2QtYWRlYi01ZmI5OTQ4OGNmMWMiLCJpYXQiOjE1Mjg0Mjg4MDgsIm5iZiI6MTUyODQyODgwOCwiZXhwIjoxNTI4NTE1MjA4LCJpc3MiOiJFbWVyZ2VuY3kiLCJhdWQiOiJFbWVyZ2VuY3kifQ.vPWJxjqy1YikbbcKlx_90nf7QoLZGf53PgNFY4NQn3Q";

                if (EmergencyToken != null)
                {
                    return true;
                }
            }
            else
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
                string param = "username=" + username + "&password=" + password + "&grant_type=password";

                //string url = "http://dev2.azuratech.com:30000/token";
                //Dictionary<string, object> map = new Dictionary<string, object>();
                //map.Add("userid", username);
                //map.Add("password", password);
                //map.Add("grant_type", "password");
                //string param = JsonConvert.SerializeObject(map);


                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", null);
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
        /// 获取登录人员信息
        /// </summary>
        /// <param name="frameToken"></param>
        /// <returns></returns>
        private async Task<UserInfoModel> getUserInfoAsync(string frameToken)
        {

            try
            {
                ///api/f w/GetUser
                //string url = FrameworkURL + "/api/fw/getUserinfo";
                string url = FrameworkURL + "/api/fw/GetUser";
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", frameToken);
                UserInfoModel userInfo = null;
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    userInfo = JsonConvert.DeserializeObject<UserInfoModel>(res.Results);
                }
                return userInfo;
            }
            catch
            {
                return null;
            }


        }

        /// <summary>
        /// 获取特定用户的信息
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public async Task<UserInfoModel> GetUserInfo(Guid staffId)
        {

            string url = App.FrameworkURL + "/api/fw/GetUserByid?id=" + staffId;
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UserInfoModel user = JsonConvert.DeserializeObject<UserInfoModel>(res.Results);
                return user;
            }
            return null;
        }



        /// <summary>
        /// 获取所有部门
        /// </summary>
        ///
        /// <returns></returns>
        public async Task<ObservableCollection<GridAllDepartmentsModel>> GetDepartmentTree()
        {
            try
            {
                ObservableCollection<GridAllDepartmentsModel> departs = new ObservableCollection<GridAllDepartmentsModel>();
                string url = App.BasicDataModule.url + "/api/Modmanage/GetDepartmentTree";
               
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", FrameworkToken);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    departs = JsonConvert.DeserializeObject<ObservableCollection<GridAllDepartmentsModel>>(res.Results);
                }
                return departs;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 获取用户所在部门
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<UserDepartmentsModel>> GetStaffDepartments(Guid staffId)
        {
            try
            {
                ObservableCollection<UserDepartmentsModel> departs = new ObservableCollection<UserDepartmentsModel>();
                string url = App.BasicDataModule.url + "/api/Modmanage/GetStaffDepartments";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", staffId);
                string param = JsonConvert.SerializeObject(dic);
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", FrameworkToken);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    departs = JsonConvert.DeserializeObject<ObservableCollection<UserDepartmentsModel>>(res.Results);
                }
                return departs;
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
                FileUtils.SaveLogFile(res.Results);
                return modules;
            }
            catch
            {
                return null;
            }
        }

        //侧滑删除
        void deleteUnUploadData(object sender, System.EventArgs e){

            MenuItem item = sender as MenuItem;
            UploadEmergencyShowModel showModel = item.CommandParameter as UploadEmergencyShowModel;
            if (showModel == null) return;
            MessagingCenter.Send<ContentPage, UploadEmergencyShowModel>(new ContentPage() , "deleteUnUploadData", showModel);
                    
        }

        /// <summary>
        /// 获取网格化登录人员信息
        /// </summary>
        /// <value>The get staff info.</value>
        public async Task<GridUserInfoModel> getStaffInfo(Guid staffid)
        {
            try
            {
                string url = App.EP360Module.url + "/api/gbm/GetStaffInfo";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", staffid);
                string param = JsonConvert.SerializeObject(dic);
                GridUserInfoModel gridUserInfo = null;
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", FrameworkToken);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    gridUserInfo = JsonConvert.DeserializeObject<GridUserInfoModel>(res.Results);
                }
                return gridUserInfo;
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
                // url = "http://192.168.1.128:5000/api/TokenAuth/ExternalAuthenticate";

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



        /// <summary>
        /// 获取360模块
        /// </summary>
        /// <returns></returns>
        private async Task GetModuleConfigEP360()
        {
            try
            {
                string url = App.EP360Module.url + "/api/mod/custconfig";
                ConvertedTokenReqStruct parameter = new ConvertedTokenReqStruct
                {
                    authProvider = "AzuraAuth",
                };
                string param = JsonConvert.SerializeObject(parameter);
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    moduleConfigEP360 = JsonConvert.DeserializeObject<ModuleConfigEP360>(res.Results);

                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                //GetModuleConfigSampling();
                _isEP360 = true;
                canGo();
            }
        }

        /// <summary>
        /// 获取采样模块
        /// </summary>
        /// <returns></returns>
        private async Task GetModuleConfigSampling()
        {
            try
            {
                string url = App.SamplingModule.url + "/api/mod/custconfig";
                ConvertedTokenReqStruct parameter = new ConvertedTokenReqStruct
                {
                    authProvider = "AzuraAuth",
                };
                string param = JsonConvert.SerializeObject(parameter);
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    moduleConfigSampling = JsonConvert.DeserializeObject<ModuleConfigSampling>(res.Results);

                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                //GetModuleConfigFramework();
                _isSampling = true;
                canGo();
            }
        }

        /// <summary>
        /// 获取基础数据模块
        /// </summary>
        /// <returns></returns>
        private async Task GetModuleConfigFramework()
        {
            try
            {
                string url = App.BasicDataModule.url + "/api/mod/custconfig";
                ConvertedTokenReqStruct parameter = new ConvertedTokenReqStruct
                {
                };
                string param = JsonConvert.SerializeObject(parameter);
                HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", App.FrameworkToken);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    moduleConfigBasicData = JsonConvert.DeserializeObject<ModuleConfigBasicData>(res.Results);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                //postEmergencyReq();
                _ISBasicData = true;
                canGo();
            }
        }
        /// <summary>
        /// 环境应急需要展示的窗口
        /// </summary>
        async Task postEmergencyReq()
        {
            try
            {
                string url = App.EmergencyModule.url + "/api/mod/custconfig";
                HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", "");

                if (hTTPResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine(hTTPResponse.Results);
                    var emergency = JsonConvert.DeserializeObject<ModuleConfigEmergency>(hTTPResponse.Results);
                    App.moduleConfigEmergency = emergency;
                }

            }
            catch (Exception e)
            {

            }
            finally
            {
                _isEmergency = true;
                canGo();
            }


        }

        /// <summary>
        /// 环境质量需要展示的窗口
        /// </summary>
        async Task postEnvironmentalReq()
        {
            try
            {
                string url = environmentalQualityModel.url + "/api/mod/custconfig";
                HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST", "");

                if (hTTPResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine(hTTPResponse.Results);
                    var eNVQ = JsonConvert.DeserializeObject<ModuleConfigENVQ>(hTTPResponse.Results);
                    //emergency.showEmeSummary = false;
                    //eNVQ.menuPastIncident = false;
                    //eNVQ.menuDutyRoster = false;
                    App.moduleConfigENVQ = eNVQ;
                }
            }
            catch (Exception e)
            {


            }
            finally
            {
                _isenvironmental = true;
                canGo();
            }

        }


        private async Task GetSqlDataAsync()
        {
            //获取数据库的数据
            //((App)App.Current).ResumeAtTodoId = -1;
            List<TodoItem> todoItems = await Database.GetItemsAsync();

            if (todoItems != null && todoItems.Count != 0)
            {

                for (int i = 0; i < todoItems.Count; i++)
                {
                    item = todoItems[i];
                    if (item.isCurrent == true)
                    {
                        //App.BaseUrl = "https://" + item.SiteAddr; //获取baseUrl    
                        break;
                    }
                }
            }
        }


        public static void OpenMenu(Page p)
        {
            if (p == null || App.masterAndDetailPage == null)
            {
                return;
            }
            App.masterAndDetailPage.Detail = new NavigationPage(p);
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
        public string status { get; set; }//0:模块启用 1:模块禁用
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
