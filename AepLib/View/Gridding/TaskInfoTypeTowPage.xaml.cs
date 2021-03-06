﻿using AepApp.Models;
using AepApp.Tools;
using AepApp.View.EnvironmentalEmergency;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

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
        string _followupId = "";
        public delegate void AddTaskToEvent(object sender, object model, EventArgs args);
        public event AddTaskToEvent AddATask;
        ObservableCollection<UserDepartmentsModel> _departMentList = new ObservableCollection<UserDepartmentsModel>();
        string _deptId;
        Dictionary<string, object> _assignMentDic = new Dictionary<string, object>();

        void updata(object sender, System.EventArgs eventArgs)
        {

            addTask();
        }

        //分派给子部门
        void assignmentTasks(object sender, EventArgs eventArgs)
        {

            Navigation.PushAsync(new AssignPersonInfoPage(_departMentList, 3, subDepartLab, _assignMentDic));
        }
        //上传要把任务分派给哪个子部门
        private async void TransferTaskAssignment(object sender, EventArgs eventArgs)
        {
            try
            {
                var aaa = _assignMentDic["toDept"];
            }
            catch (Exception ex)
            {
                await DisplayAlert("提示", "请添加子部门", "确定");
                return;
            }
            upTaskToSubDepartMent();
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
            Navigation.PushAsync(new ExecutionRecordPage(_infoModel.id.ToString(), _infoModel.staff.ToString()));
        }

        //选择事件
        void chooseEvent(object sender, System.EventArgs e)
        {

        }
        //指派网格员/部门
        void choiseUser(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AssignPersonPage(_infoModel));
        }

        //添加相关企业
        void AddEnterprise(object sender, System.EventArgs e)
        {
            var selectEnt = new RelatedEnterprisesPage(_infoModel.enterprise);
            selectEnt.addEnter += () =>
            {
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
            page.SavePosition += (arg, arg1) =>
            {
                var pos = arg as string;
                if (string.IsNullOrWhiteSpace(pos))
                {
                    return;
                }
                string[] p = pos.Replace("E", "").Replace("N", "").Replace("W", "").Replace("S", "").Split(",".ToCharArray());


                Coords coords = new Coords
                {
                    lng = Convert.ToDouble(p[0]),
                    lat = Convert.ToDouble(p[1]),
                    remarks = pos,
                    id = Guid.NewGuid(),
                    rowState = "add",
                    index = _infoModel.coords.Count + 1,

                };


                getAddressWihtLocation(coords);
            };
            Navigation.PushAsync(page);

        }

        //获取用户执行当前任务的assignmentid
        private string getAssignmentId()
        {
            string assignmentId = "";
            Guid currentUserId = App.userInfo == null ? Guid.Empty : App.userInfo.id;
            Guid currentGuid = App.gridUser == null ? Guid.Empty : App.gridUser.grid;
            if (_infoModel == null || _infoModel.taskassignments2 == null)
            {
                return assignmentId;
            }
            foreach (var item in _infoModel.taskassignments2)
            {
                if (!string.IsNullOrWhiteSpace(item.dept))//指派给部门
                {
                    assignmentId = item.id != null ? item.id.ToString() : "";
                }
                else//指派给网格/网格员
                {
                    if(item.staff != null && item.staff == currentUserId)
                    {
                        assignmentId = item.id != null ? item.id.ToString() : "";
                    }else if(item.grid != null && item.grid == currentGuid)
                    {
                        assignmentId = item.id != null ? item.id.ToString() : "";
                    }
                }
            }
            return assignmentId;
        }

        /// <summary>
        /// 添加执行结果
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void taskResult(object sender, System.EventArgs e)
        {
            if (_infoModel == null)
            {
                return;
            }
            GridTaskHandleRecordModel record = new GridTaskHandleRecordModel
            {
                date = DateTime.Now,
                assignment = getAssignmentId(),
                results = _infoModel.results,
                editName = App.userInfo.userName,
            };
            if (App.gridUser == null)
            {
                App.gridUser = await (App.Current as App).getStaffInfo(App.userInfo.id);
            }
            if (App.gridUser != null)
            {
                record.gridName = App.gridUser.gridName;
            }
            if (App.userDepartments == null)
            {
                App.userDepartments = await (App.Current as App).GetStaffDepartments(App.userInfo.id);
            }
            if (App.userDepartments != null && App.userDepartments.Count > 0)
                record.gridName = App.userDepartments[0].name;

            if (_infoModel.staff != null)
                record.staff = _infoModel.staff.Value;
            await Navigation.PushAsync(new TaskResultPage(_infoModel.id, record, mNeedExcute, _infoModel.enterprise));
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
            _infoModel.type = ConstConvertUtils.TaskNatureString2Int(typeName);
            _infoModel.natureName = typeName;
        }

        private void pickerStatud_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            _infoModel.state = ConstConvertUtils.TaskStateString2Int(typeName);
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
            templatePage.selectTemplateResult += (object s, EventArgs args) =>
            {
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
        public TaskInfoTypeTowPage(string taskId, bool needExcute, string eventId, bool needUp, string assignmentId, string followupid)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _eventId = eventId;
            _taskId = taskId;
            mNeedExcute = needExcute;
            mNeedUp = needUp;
            _assignmentId = assignmentId;
            _followupId = followupid;
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
                    type = 2,//默认事件任务
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
                _infoModel.stateName = ConstConvertUtils.TaskState2String(_infoModel.state.Value);
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
                    _infoModel.stateName = ConstConvertUtils.TaskState2String(_infoModel.state.Value);
                    _infoModel.canEdit = false;
                    creatPositionList();
                    if (_infoModel.taskassignments != null && _infoModel.taskassignments.Count > 0)
                    {
                        for (int i = 0; i < _infoModel.taskassignments.Count; i++)
                        {
                            if (i == 0)
                            {
                                _infoModel.AssignName = await getAssignName(_infoModel.taskassignments[i], "");
                            }
                            else
                            {
                                _infoModel.AssignName = _infoModel.AssignName + "\n" + await getAssignName(_infoModel.taskassignments[i], "");
                            }

                        }
                    }

                    DatePickerStart.Date = _infoModel.deadline.Value;
                    GR.IsVisible = true;
                    GH.Height = 0;
                    BindingContext = _infoModel;
                    ReqGridTaskList();
                    if (_infoModel.state == 6 || _infoModel.state == 5 || _infoModel.state == 3) addTaskResulGR.IsVisible = false;
                    _infoModel.enterprise = new ObservableCollection<Enterprise>();
                    GetStaffInfo();
                    GetSendUserInfo();
                    if (_infoModel.template != null) GetTemplateDetail();
                    if (_infoModel.taskenterprises != null && _infoModel.taskenterprises.Count > 0) ReqEnters();
                    //如果选择的是部门
                    foreach (var item in _infoModel.taskassignments)
                    {
                        if (item.dept != null && string.IsNullOrEmpty(_deptId))
                        {
                            _deptId = item.dept.ToString();
                            _assignMentDic.Add("assignment", item.id);
                            ReqDepartMentList();
                        }
                    }

                }
                catch (Exception e)
                {
                    await Navigation.PopAsync();
                }
            }
        }
        /// <summary>
        /// 获取任务模板
        /// </summary>
        private async void GetTemplateDetail()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTemplateDetail";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _infoModel.template);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    TaskTemplateModel data = Tools.JsonUtils.DeserializeObject<TaskTemplateModel>(res.Results);
                    if (data == null) return;
                    _infoModel.templateName = data.title;

                }
                catch (Exception e)
                {

                }
            }
        }

        //获取部门某人员的所有上级部门名称
        private async Task<string> getParentDepartment(string parentId, string currentName)
        {
            if (string.IsNullOrWhiteSpace(parentId)) return null;
            UserDepartmentsModel parentDepart = await (App.Current as App).GetDepartmentInfo(parentId);
            if (parentDepart != null)
            {
                if (string.IsNullOrWhiteSpace(currentName))
                {
                    currentName = parentDepart.name;
                }
                else
                {
                    currentName = parentDepart.name + "-" + currentName;
                }
                if (parentDepart.parentid == null)
                {
                    return currentName;
                }
                else
                {
                    return await getParentDepartment(parentDepart.parentid.ToString(), currentName);
                }
            }
            else
            {
                return currentName;
            }

        }

        //获取被指派人员名称
        private async Task<string> getAssignName(taskassignment currentItem, string currentName)
        {
            UserInfoModel staff = null;
            if (currentItem == null) return "";
            if (currentItem.staff != null)
            {
                staff = await (App.Current as App).GetUserInfo(currentItem.staff.Value);
            }
            if (currentItem.grid != null)//网格/网格员
            {
                currentName = (string.IsNullOrWhiteSpace(currentName) ? "" : (currentName + "-")) + currentItem.gridName;
                if (currentItem.nextLevel != null)
                {
                    return await getAssignName(currentItem.nextLevel, currentName);
                }
                else
                {
                    if (currentItem.staff != null && staff != null)
                    {
                        currentName += "-" + staff.userName;
                    }
                    return currentName;
                }
            }
            else //部门/部门人员
            {
                UserDepartmentsModel depart = await (App.Current as App).GetDepartmentInfo(currentItem.dept);
                if (currentItem.staff != null && staff != null)//给部门人员
                {
                    currentName = staff.userName;
                    if (depart != null)
                    {
                        if (depart.parentid != null)
                        {
                            currentName = await getParentDepartment(depart.parentid.ToString(), currentName);
                        }
                        else
                        {
                            currentName = depart.name + "-" + currentName;
                        }
                    }
                }
                else//给部门
                {
                    currentName = depart != null ? depart.name : "";
                    if (depart != null && depart.parentid != null)
                    {
                        currentName = await getParentDepartment(depart.parentid.ToString(), currentName);
                    }
                }
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
                    var eventInfoModel = Tools.JsonUtils.DeserializeObject<GridEventInfoModel>(hTTPResponse.Results);
                    if (eventInfoModel == null) return;
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
        private async Task<UserInfoModel> GetSendUserInfo()
        {
            UserInfoModel auditor = await (App.Current as App).GetUserInfo(_infoModel.staff.Value);
            if (auditor != null)
            {
                _infoModel.userName = auditor.userName;
            }

            return auditor;
        }


        private async void getApprovedUserInfo()
        {
            UserInfoModel auditor = await (App.Current as App).GetUserInfo(_infoModel.staff.Value);
            if (auditor != null)
            {
                _infoModel.approvedName = auditor.userName;
            }
        }

        //根据id获取企业
        private async void ReqEnters()
        {

            string url = App.BasicDataModule.url + "/api/mod/GetAllEnterpriseByarrid";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("items", _infoModel.taskenterprises);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                string result = res.Results.Replace("name", "enterpriseName");
                _infoModel.enterprise = Tools.JsonUtils.DeserializeObject<ObservableCollection<Enterprise>>(result);
                creatEnterpriseList();
            }
        }



        //任务执行记录
        private async void ReqGridTaskList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskHandleList";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("task", _infoModel.id);

            if (_infoModel.staff != null) map.Add("staff", _infoModel.staff);
            else map.Add("staff", "");
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);

            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    List<GridTaskHandleRecordModel> list = Tools.JsonUtils.DeserializeObject<List<GridTaskHandleRecordModel>>(res.Results);
                    //DependencyService.Get<IToast>().ShortAlert( "请求结果：" + res.Results);

                    if (list != null && list.Count > 0)
                    {
                        var recorModel = list[0];
                        _infoModel.LastRecordTime = recorModel.date;
                        _infoModel.RecordCount = list.Count;
                        _infoModel.results = recorModel.results;
                    }
                    else
                    {
                        //SK.IsVisible = false;
                    }

                }
                catch (Exception e)
                {
                    DependencyService.Get<IToast>().ShortAlert("任务执行记录失败：" + e + "\n\n" + res.Results);

                }
            }
        }
        //获取部门下面的子部门

        private async void ReqDepartMentList()
        {
            string url = App.BasicDataModule.url + "/api/Modmanage/GetDepartmentsUnder";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("id", _deptId);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    _departMentList = Tools.JsonUtils.DeserializeObject<ObservableCollection<UserDepartmentsModel>>(res.Results);
                    if (_departMentList != null && _departMentList.Count > 0)
                    {
                        if(_infoModel == null || _infoModel.state == null || (_infoModel.state.Value != 3 && _infoModel.state.Value != 6))
                        {
                            SKT.IsVisible = true;
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        //把任务分派给子部门
        private async void upTaskToSubDepartMent()
        {

            upDepartBut.IsEnabled = false;
            string url = App.EP360Module.url + "/api/gbm/TransferTaskAssignment";
            string param = JsonConvert.SerializeObject(_assignMentDic);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    if (res.Results == "\"OK\"")
                    {
                        upDepartBut.IsVisible = false;
                        //DependencyService.Get<IToast>().ShortAlert("");
                        CrossHud.Current.ShowSuccess(message: "任务分派成功!", timeout: new TimeSpan(0, 0, 2), cancelCallback: cancel);
                        await Navigation.PopAsync();
                    }
                }
                catch (Exception e)
                {
                    upDepartBut.IsEnabled = true;
                }
            }
        }

        private void cancel()
        {
            //CrossHud.Current.Dismiss();
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

            if (_infoModel.assignments.Count == 0)
            {
                await DisplayAlert("提示", "请选择任务执行人", "确定");
                return;
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", _infoModel.id);
            dic.Add("rowState", _infoModel.rowState);
            if (_infoModel.type == 2 && !string.IsNullOrWhiteSpace(_followupId))//事件任务才传incident字段
            {
                dic.Add("incident", _followupId);
            }
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
            for (int i = _infoModel.assignments.Count - 1; i >= 0; i--)
            {
                var assign = _infoModel.assignments[i];
                Dictionary<string, object> assigmengtdic = new Dictionary<string, object>();
                assigmengtdic.Add("id", assign.id);
                assigmengtdic.Add("rowState", assign.rowState);
                assigmengtdic.Add("dept", assign.dept);
                assigmengtdic.Add("staff", assign.staff);
                assigmengtdic.Add("grid", assign.grid);
                assigmengtdic.Add("type", assign.type);
                assigmengtList.Add(assigmengtdic);
                if (_infoModel.IsToDepartment)
                {
                    if(i == _infoModel.assignments.Count - 1)
                    {
                        break;
                    }
                }
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

            if (mNeedUp == false)
            {
                AddATask(dic, _infoModel, new EventArgs());
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
            if (_infoModel == null || _infoModel.enterprise == null) return;
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
                //Image image = new Image
                //{
                //    Source = ImageSource.FromFile("right"),
                //    Margin = new Thickness(10),
                //    HeightRequest = 20,
                //    WidthRequest = 10,
                //    VerticalOptions = LayoutOptions.Center,
                //    HorizontalOptions = LayoutOptions.End,
                //};

                BoxView box = new BoxView
                {
                    BackgroundColor = Color.Silver,
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                };
                G1.Children.Add(frame);
                G1.Children.Add(label);
                //G1.Children.Add(image);
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
                G1.Children.Add(image);
                G1.Children.Add(box);

                TapGestureRecognizer tap = new TapGestureRecognizer((arg1, arg2) =>
                {
                    Navigation.PushAsync(new RescueSiteMapPage(po));
                });
                tap.BindingContext = po;
                G1.GestureRecognizers.Add(tap);

            }



        }


    }
}
