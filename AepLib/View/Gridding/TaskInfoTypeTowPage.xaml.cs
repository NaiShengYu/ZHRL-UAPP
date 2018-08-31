﻿using System;
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

        public delegate void AddTaskToEvent(object sender, EventArgs args);
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
            Navigation.PushAsync(new GridTreeViewPage());
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
                results = @"<p> 初始化的内容 </p><p> 初始化的内容 </p>",
            };
            if (_infoModel.staff != null)
                record.staff = _infoModel.staff.Value;
            Navigation.PushAsync(new TaskResultPage(_infoModel.id, record, mNeedExcute));
        }

        void editContent(object sender, System.EventArgs e)
        {
            EditContentsPage editContentsPage = new EditContentsPage(_infoModel, "EditContents", 2);
            editContentsPage.Title = "任务内容";
            Navigation.PushAsync(editContentsPage);
        }
        private void pickerNature_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            if (typeName == "日常任务") _infoModel.type = 0;
            if (typeName == "时间处理任务") _infoModel.type = 1;
        }

        private void pickerStatud_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            if (typeName == "上报中") _infoModel.type = 1;
            if (typeName == "乡级审核") _infoModel.type = 2;
            if (typeName == "县级审核") _infoModel.type = 3;
            if (typeName == "已处理") _infoModel.type = 4;
        }

        /// <summary>
        /// 选择任务模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseTemplate(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TaskTemplatePage());
        }


        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="needExcute">是否需要执行记录 true：可以添加执行结果 false：只能查看执行结果</param>
        ///  <param name="needUp">是否需要上传任务 true：可以上传 false：将任务提交到上一级</param>
        public TaskInfoTypeTowPage(string taskId, bool needExcute, string eventId, bool needUp)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _eventId = eventId;
            _taskId = taskId;
            mNeedExcute = needExcute;
            mNeedUp = needUp;
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
                    state = 2,
                    type = 1,
                    id = Guid.NewGuid(),
                    index = 2,
                    userName = App.userInfo.userName,
                    coords = new ObservableCollection<Coords>(),
                    enterprise = new ObservableCollection<Enterprise>(),
                    assignments = new ObservableCollection<Assignments>(),
                };
                BindingContext = _infoModel;
                pickerNature.Title = "日常任务";
                pickerStatus.Title = "上报中";
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
                     result = result.Replace("taskassignments", "assignments");
                     result = result.Replace("taskcoords", "coords");
                     result = result.Replace("taskenterprises", "enterprise");

                    _infoModel = JsonConvert.DeserializeObject<GridTaskInfoModel>(result);
                    try
                    {
                            foreach (var ass in _infoModel.assignments)
                        {
                            _infoModel.AssignName = _infoModel.AssignName + ass.gridName;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    _infoModel.canEdit = false;
                    creatPositionList();
                    creatEnterpriseList();
                    GR.IsVisible = true;
                    GH.Height = 0;
                    BindingContext = _infoModel;
                    ReqGridTaskList();
                }
                catch (Exception e)
                {
                    Navigation.PopAsync();
                }
            }

        }

        //任务执行记录
        private async void ReqGridTaskList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTaskHandleList";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("task", _infoModel.id);
            map.Add("staff", _infoModel.staff);
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
            dic.Add("enterprise", _infoModel.enterprise);
            dic.Add("coords", _infoModel.coords);
            dic.Add("assignments", _infoModel.assignments);


            //if(mNeedUp ==false){
            //    AddATask(_infoModel, new EventArgs());
            //    Navigation.PopAsync();
            //    return;
            //}

            string url = App.EP360Module.url + "/api/gbm/updatetask";
            string param = JsonConvert.SerializeObject(dic);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (hTTPResponse.Results == "OK") _infoModel.rowState = "upd";
            }

        }




        /// <summary>
        /// 相关企业列表
        /// </summary>
        void creatEnterpriseList()
        {
            foreach (var po in _infoModel.enterprise)
            {
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
                    Text = "1",
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
