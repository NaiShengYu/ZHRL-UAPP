using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using AepApp.View.EnvironmentalEmergency;
using AepApp.Tools;
using System.Net;

namespace AepApp.View.Gridding
{
    public partial class TaskInfoTypeTowPage : ContentPage
    {
        GridTaskInfoModel _infoModel = null;
        string _taskId = "";
        bool mNeedExcute = false;
        string _eventId = "";
        bool mNeedUp = true;
        string _assignmentId = "";
        public delegate void AddTaskToEvent(object sender,object model, EventArgs args);
        public event AddTaskToEvent AddATask;

        void updata(object sender, System.EventArgs eventArgs)
        {

            addTask();
        }

        void period_change(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                _infoModel.period = Convert.ToDouble(e.NewTextValue);
            }
            catch (Exception ex)
            {

            }

        }
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _infoModel.title = e.NewTextValue;
        }
        //任务执行期限
        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            _infoModel.deadline = e.NewDate;
        }
        /// <summary>
        /// 执行记录
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void ExecutionRecord(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ExecutionRecordPage(_infoModel.id.ToString(),_infoModel.staff.ToString()));
        }

        //选择事件
        void chooseEvent(object sender, System.EventArgs e)
        {

        }
        //指派网格员
        void choiseUser(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new GridTreeWithUserPage(_infoModel));
        }

        //添加相关企业
        void AddEnterprise(object sender, System.EventArgs e)
        {
            var selectEnt = new RelatedEnterprisesPage(_infoModel.enterprise);
            selectEnt.addEnter +=() =>{
                creatEnterpriseList();  
            };
            Navigation.PushAsync(selectEnt);
        }
        //添加相关位置
        void AddPosition(object sender, System.EventArgs e)
        {
            AccidentPositionPage page;
            //if (_infoModel.lat == 0.0 || _infoModel.lng == 0.0)
            //{
            page = new AccidentPositionPage(null, null);
            //}
            //else
            //{
            //    page = new AccidentPositionPage(_infoModel.lng.ToString(), _infoModel.lat.ToString());
            //}
            page.Title = "任务位置";
            Navigation.PushAsync(page);
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "savePosition");
            MessagingCenter.Subscribe<ContentPage, string>(this, "savePosition", (s, arg) =>
            {
                var pos = arg as string;
                if (pos == null)
                {
                    return;
                }
                string[] p = pos.Replace("E", "").Replace("N", "").Replace("W", "").Replace("S", "").Split(",".ToCharArray());


                Coords coords = new Coords
                {
                    lng = Convert.ToDouble(p[0]),
                    lat = Convert.ToDouble(p[1]),
                    remarks = arg,
                    id = Guid.NewGuid(),
                    rowState = "add",
                    index = _infoModel.coords.Count + 1,

                };


                getAddressWihtLocation(coords);
            });

        }


        /// <summary>
        /// 编辑任务结果
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void taskResult(object sender, System.EventArgs e)
        {
            if (_infoModel == null)
            {
                return;
            }
            GridTaskHandleRecordModel record = new GridTaskHandleRecordModel
            {
                date = _infoModel.date,
                gridName = _infoModel.gridName,
                assignment = _assignmentId,
                results = _infoModel.results,
            };
            if (_infoModel.staff != null)
                record.staff = _infoModel.staff.Value;
            Navigation.PushAsync(new TaskResultPage(_infoModel.id, record, mNeedExcute));
        }

        void editContent(object sender, System.EventArgs e)
        {
            GridTaskContentsPage editContentsPage = new GridTaskContentsPage(_infoModel);
            editContentsPage.Title = "任务内容";
            Navigation.PushAsync(editContentsPage);
        }
        private void pickerNature_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            _infoModel.type = ConstConvertUtils.TaskNatureString2Type(typeName);
            _infoModel.natureName = typeName;
        }

        private void pickerStatud_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            _infoModel.state = ConstConvertUtils.TaskStateString2Type(typeName);
            _infoModel.stateName = typeName;

        }

        /// <summary>
        /// 选择任务模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseTemplate(object sender, EventArgs e)
        {
            TaskTemplatePage templatePage = new TaskTemplatePage();
            templatePage.selectTemplateResult += (object s, EventArgs args) =>{
                TaskTemplateModel model = s as TaskTemplateModel;
                _infoModel.template = model.id;
                _infoModel.Contents = model.contents;
                _infoModel.templateName = model.title;
            };
            Navigation.PushAsync(templatePage);
        }


        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="needExcute">是否需要执行记录 true：可以添加执行结果 false：只能查看执行结果</param>
        ///  <param name="needUp">是否需要上传任务 true：可以上传 false：将任务提交到上一级</param>
        public TaskInfoTypeTowPage(string taskId, bool needExcute, string eventId, bool needUp,string assignmentId)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _eventId = eventId;
            _taskId = taskId;
            mNeedExcute = needExcute;
            mNeedUp = needUp;
            _assignmentId = assignmentId;
            //
            SK.IsVisible = mNeedExcute;

            if (!string.IsNullOrEmpty(_taskId)) getTaskInfo();
            else
            {

                _infoModel = new GridTaskInfoModel
                {
                    canEdit = true,
                    rowState = "add",
                    date = DateTime.Now,
                    deadline = DateTime.Now,
                    staff = App.userInfo.id,
                    state = 1,
                    type = 1,
                    id = Guid.NewGuid(),
                    index = 2,
                    userName = App.userInfo.userName,
                    gridName = App.gridUser.gridName,
                    coords = new ObservableCollection<Coords>(),
                    enterprise = new ObservableCollection<Enterprise>(),
                    assignments = new ObservableCollection<Assignments>(),
                };
                if (!string.IsNullOrEmpty(_eventId)) getEventInfo();


                _infoModel.natureName = ConstConvertUtils.TaskNatureType2String(_infoModel.type.Value);
                _infoModel.stateName = ConstConvertUtils.TaskStateType2String(_infoModel.state.Value);
                BindingContext = _infoModel;
                try
                {
                    _infoModel.incident = new Guid(_eventId);
                }
                catch (Exception ex)
                {

                }
                GR.IsVisible = true;
            }
        }

        //获取任务详情
        private async void getTaskInfo()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _taskId);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string result = hTTPResponse.Results.Replace("[null]", "[]");
                     result = result.Replace("taskcoords", "coords");
                    _infoModel = JsonConvert.DeserializeObject<GridTaskInfoModel>(result);
                   
                    _infoModel.natureName = ConstConvertUtils.TaskNatureType2String(_infoModel.type.Value);
                    _infoModel.stateName = ConstConvertUtils.TaskStateType2String(_infoModel.state.Value);
                    _infoModel.canEdit = false;
                    creatPositionList();

                    if (_infoModel.taskassignments != null && _infoModel.taskassignments.Count > 0)
                        _infoModel.AssignName = getAssignName(_infoModel.taskassignments[0], "");

                    GR.IsVisible = true;
                    GH.Height = 0;
                    BindingContext = _infoModel;
                    ReqGridTaskList();
                    _infoModel.enterprise = new ObservableCollection<Enterprise>();
                    GetStaffInfo();
                    GetSendUserInfo();
                    if (_infoModel.taskenterprises !=null && _infoModel.taskenterprises.Count > 0) ReqEnters();
                }
                catch (Exception e)
                {
                    await Navigation.PopAsync();
                }
            }

        }

        private string getAssignName(taskassignment currentItem,string currentName){
               
            if (string.IsNullOrEmpty(currentName)) currentName = currentItem.gridName;
                else currentName = currentName +"-"+currentItem.gridName;

            if(currentItem.nextLevel != null){
                return getAssignName(currentItem.nextLevel, currentName);
                }
            return currentName;
        }

        //获取事件详情
        private async void getEventInfo()
        {

            string url = App.EP360Module.url + "/api/gbm/GetIncidentDetail";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", _eventId);
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var eventInfoModel = JsonConvert.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                    _infoModel.incidentTitle = eventInfoModel.title;
                }
                catch (Exception ex)
                {
                    Navigation.PopAsync();
                }
            }

        }

        /// <summary>
        /// 网格部门
        /// </summary>
        /// <param name="staffId"></param>
        private async void GetStaffInfo()
        {
            GridUserInfoModel auditor = await (App.Current as App).getStaffInfo(_infoModel.staff.Value);
            if (auditor != null)
            {
                _infoModel.gridName = auditor.gridName;
            }
        }

        /// <summary>
        /// 发出人名称
        /// </summary>
        /// <param name="staffId"></param>
        private async void GetSendUserInfo()
        {
            UserInfoModel auditor = await (App.Current as App).GetUserInfo(_infoModel.staff.Value);
            if (auditor != null)
            {
                _infoModel.userName = auditor.userName;
            }
        }
        //根据id获取企业
        private async void ReqEnters(){

            string url = App.BasicDataModule.url + "/api/mod/GetAllEnterpriseByarrid";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("items",_infoModel.taskenterprises);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                string result = res.Results.Replace("name", "enterpriseName");

                _infoModel.enterprise = JsonConvert.DeserializeObject<ObservableCollection<Enterprise>>(result);

                creatEnterpriseList();

            }


        }



        //任务执行记录
        private async void ReqGridTaskList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskHandleList";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("task", _infoModel.id);

            if(_infoModel.staff !=null)map.Add("staff", _infoModel.staff);
            else map.Add("staff", "");
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<GridTaskHandleRecordModel> list = JsonConvert.DeserializeObject<List<GridTaskHandleRecordModel>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        var recorModel = list[0];
                        _infoModel.LastRecordTime = recorModel.date;
                        _infoModel.RecordCount = list.Count;
                        _infoModel.results = recorModel.results;
                        resultTime.Text = _infoModel.LastRecordTime.ToString("yyyy-MM-dd");
                    }else{
                        //SK.IsVisible = false;
                    }

                }
                catch (Exception e)
                {

                }

            }
        }


        //反地理编码
        private async void getAddressWihtLocation(Coords coords)
        {
            string param = "";
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync("https://apis.map.qq.com/ws/geocoder/v1/?location=" + coords.lat + "," + coords.lng + "&key=72NBZ-3YWK2-XV3U7-CM7OL-MKPMK-DRF2B", param, "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(hTTPResponse.Results);
                Dictionary<string, object> resultDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(dic["result"].ToString());
                try
                {
                    coords.title = resultDic["address"].ToString();
                    _infoModel.coords.Add(coords);
                    creatPositionList();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
        }


        //添加任务
        private async void addTask()
        {
            if (string.IsNullOrEmpty(_infoModel.title))
            {
                await DisplayAlert("提示", "请添加标题", "确定");
                return;
            }




            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", _infoModel.id);
            dic.Add("rowState", _infoModel.rowState);
            if (_infoModel.incident != Guid.Empty) dic.Add("incident", _infoModel.incident);
            dic.Add("staff", _infoModel.staff);
            if (_infoModel.template != Guid.Empty) dic.Add("template", _infoModel.template);
            dic.Add("title", _infoModel.title);
            dic.Add("contents", _infoModel.Contents);
            dic.Add("deadline", _infoModel.deadline);
            dic.Add("period", _infoModel.period);
            dic.Add("type", _infoModel.type);
            dic.Add("state", _infoModel.state);
            dic.Add("index", _infoModel.index);
            dic.Add("date", _infoModel.date);
            ObservableCollection<Dictionary<string, object>> assigmengtList = new ObservableCollection<Dictionary<string, object>>();
            foreach (var item in _infoModel.assignments)
            {
                Dictionary<string, object> assigmengtdic = new Dictionary<string, object>();
                assigmengtdic.Add("id", item.id);
                assigmengtdic.Add("rowState", item.rowState);
                assigmengtdic.Add("dept", item.dept);
                assigmengtdic.Add("staff", item.staff);
                assigmengtdic.Add("grid", item.grid);
                assigmengtdic.Add("type", item.type);
                assigmengtList.Add(assigmengtdic);
            }

            ObservableCollection<Dictionary<string, object>> coordsList = new ObservableCollection<Dictionary<string, object>>();
            foreach (var item in _infoModel.coords)
            {
                Dictionary<string, object> coordsdic = new Dictionary<string, object>();
                coordsdic.Add("id", item.id);
                coordsdic.Add("rowState", item.rowState);
                coordsdic.Add("title", item.title);
                coordsdic.Add("lng", item.lng);
                coordsdic.Add("lat", item.lat);
                coordsdic.Add("remarks", item.remarks);
                coordsdic.Add("index", item.index);
                coordsList.Add(coordsdic);
            }

            ObservableCollection<Dictionary<string, object>> enterprisesList = new ObservableCollection<Dictionary<string, object>>();
            foreach (var item in _infoModel.enterprise)
            {
                Dictionary<string, object> enterprisesdic = new Dictionary<string, object>();
                enterprisesdic.Add("id", item.id);
                enterprisesdic.Add("rowState", item.rowState);
                enterprisesdic.Add("enterprise", item.enterprise);
                enterprisesList.Add(enterprisesdic);
            }
            dic.Add("enterprises", enterprisesList);
            dic.Add("coords", coordsList);
            dic.Add("assignments", assigmengtList);

            if(mNeedUp ==false){
                AddATask(dic, _infoModel,new EventArgs());
                await Navigation.PopAsync();
                return;
            }

            string url = App.EP360Module.url + "/api/gbm/updatetask";
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (hTTPResponse.Results == "\"OK\"") _infoModel.rowState = "upd";
            }

        }




        /// <summary>
        /// 相关企业列表
        /// </summary>
        void creatEnterpriseList()
        {
            enterpriseSK.Children.Clear();
            for (int i = 0; i < _infoModel.enterprise.Count; i++)
            {
                var po = _infoModel.enterprise[i];
            
                Grid G1 = new Grid
                {
                    //BackgroundColor = Color.Blue,
                };
                enterpriseSK.Children.Add(G1);

                Label label = new Label
                {
                    Margin = new Thickness(50, 10, 30, 10),
                    Text = po.enterpriseName,
                    FontSize = 18,
                    VerticalOptions = LayoutOptions.Center,
                };

                Frame frame = new Frame
                {
                    CornerRadius = 15,
                    HeightRequest = 30,
                    WidthRequest = 30,
                    BackgroundColor = Color.Red,
                    Padding = new Thickness(0),
                    Margin = new Thickness(10, 15, 10, 15),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    HasShadow = false,
                };
                Label numLab = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.White,
                    Text = (i + 1).ToString(),
                };
                frame.Content = numLab;
                Image image = new Image
                {
                    Source = ImageSource.FromFile("right"),
                    Margin = new Thickness(10),
                    HeightRequest = 20,
                    WidthRequest = 10,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.End,
                };

                BoxView box = new BoxView
                {
                    BackgroundColor = Color.Silver,
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                };
                G1.Children.Add(frame);
                G1.Children.Add(label);
                G1.Children.Add(image);
                G1.Children.Add(box);
            }




        }

        // 相关位置列表
        void creatPositionList()
        {

            positionNum.Text = _infoModel.coords.Count.ToString();
            positionSK.Children.Clear();
            for (int i = 0; i < _infoModel.coords.Count; i++)
            {
                var po = _infoModel.coords[i];

                Grid G1 = new Grid
                {
                    //BackgroundColor = Color.Blue,
                };
                positionSK.Children.Add(G1);


                StackLayout SK = new StackLayout
                {
                    Margin = new Thickness(50, 10, 30, 10),
                    Spacing = 2,
                };
                G1.Children.Add(SK);


                Label label = new Label
                {
                    Margin = new Thickness(0),
                    Text = po.title,
                    FontSize = 18,
                };
                Label label1 = new Label
                {
                    Margin = new Thickness(0),
                    Text = po.remarks,
                    FontSize = 16,
                    TextColor = Color.Gray,
                };
                SK.Children.Add(label);
                SK.Children.Add(label1);



                Frame frame = new Frame
                {
                    CornerRadius = 15,
                    HeightRequest = 30,
                    WidthRequest = 30,
                    BackgroundColor = Color.Red,
                    Padding = new Thickness(0),
                    Margin = new Thickness(10, 5, 10, 5),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    HasShadow = false,

                };
                Label numLab = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.White,
                    Text = po.index.ToString(),
                };
                frame.Content = numLab;

                BoxView box = new BoxView
                {
                    BackgroundColor = Color.Silver,
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                };
                G1.Children.Add(frame);
                G1.Children.Add(box);


            }



        }


    }
}
