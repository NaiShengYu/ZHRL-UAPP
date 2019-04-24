using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddPlacementPage : ContentPage
    {
        AddPlacementModel _placementModel;
        string _projectId = "";
        async void Handle_updata(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_placementModel.name))
            {
                await DisplayAlert("提示", "布点名称不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("布点名称不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(_placementModel.address))
            {
                await DisplayAlert("提示", "布点位置不能为空", "确定");
                //DependencyService.Get<Sample.IToast>().ShortAlert("布点名称不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(_placementModel.id)) addPlacement();
            else updatePlacement();
        }

        //采样计划名称
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _placementModel.name = e.NewTextValue;
        }
        //编辑地址
        async void Handle_editAddress(object sender, System.EventArgs e)
        {
            if (_placementModel.canEdit ==false)
            {
                return;
            }
            AccidentPositionPage page;
            if (string.IsNullOrWhiteSpace(_placementModel.lat) || string.IsNullOrWhiteSpace(_placementModel.lng))
            {
                page = new AccidentPositionPage(null, null);
            }
            else
            {
                page = new AccidentPositionPage(_placementModel.lng, _placementModel.lat);
            }
            page.Title = "布点位置";
            await Navigation.PushAsync(page);
            page.SavePosition += (arg, arg1) =>

            {
                var pos = arg as string;
                if (string.IsNullOrWhiteSpace(pos))
                {
                    return;
                }
                string[] p = pos.Replace("E", "").Replace("N", "").Replace("W", "").Replace("S", "").Split(",".ToCharArray());

                _placementModel.lng = p[0];
                _placementModel.lat = p[1];
                getAddressWihtLocation();
            };
        }
        //选择时间
        void Handle_DateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            DateTime dateTime = e.NewDate;
            _placementModel.plantime = dateTime;
        }
        //样品预处理
        void Handle_editSample(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("样品预处理", _placementModel.canEdit, _placementModel.pretreatment);
            page.result += (object result, EventArgs even) =>
            {
                _placementModel.pretreatment = result as string;
            };
            Navigation.PushAsync(page);
        }

        //质控说明
        void Handle_editQuality(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("质控说明", _placementModel.canEdit, _placementModel.qctip);
            page.result += (object result, EventArgs even) =>
            {
                _placementModel.qctip = result as string;
            };
            Navigation.PushAsync(page);
        }
        //备注信息
        void Handle_editRemarks(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("备注信息", _placementModel.canEdit, _placementModel.remarks);
            page.result += (object result, EventArgs even) =>
            {
                _placementModel.remarks = result as string;
            };
            Navigation.PushAsync(page);
        }
        //安全说明
        void Handle_editSafety(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("安全说明", _placementModel.canEdit, _placementModel.security);
            page.result += (object result, EventArgs even) =>
            {
                _placementModel.security = result as string;
            };
            Navigation.PushAsync(page);
        }
        //添加相关人员
        void Handle_AddPerson(object sender, System.EventArgs e)
        {
            PersonListPage page = new PersonListPage();
            bool isSame = false;
            page.SamplePerson += (object equipment, EventArgs even) =>
            {
                AddPlacement_Staff result = equipment as AddPlacement_Staff;
                foreach (AddPlacement_Staff item in _placementModel.staffs)
                {
                    if (item.staffid == result.staffid)
                    {
                        isSame = true;
                        break;
                    }
                }
                if (isSame == false)
                    _placementModel.staffs.Add(result);
            };
            Navigation.PushAsync(page);
        }
        void PersonBindingChanged(object sender, System.EventArgs e)
        {
            ViewCell cell = sender as ViewCell;
            if (_placementModel.canEdit == true)
            {
                if (cell.ContextActions.Count == 0)
                {
                    var deletPersonAction = new MenuItem { Text = "删除", IsDestructive = true };
                    deletPersonAction.Clicked += Handle_DeletPerson;
                    cell.ContextActions.Add(deletPersonAction);
                }
            }
        }
        void Handle_DeletPerson(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _placementModel.staffs.Remove(item.BindingContext as AddPlacement_Staff);
        }
        //添加相关设备
        void Handle_AddEquipment(object sender, System.EventArgs e)
        {
            EquipmentPage page = new EquipmentPage(true);
            bool isSame = false;
            page.SampleEquipment += (object equipment, EventArgs even) =>
            {
                AddPlacement_Equipment result = equipment as AddPlacement_Equipment;
                foreach (AddPlacement_Equipment item in _placementModel.equips)
                {
                    if (item.equipid == result.equipid)
                    {
                        isSame = true;
                        break;
                    }
                }
                if (isSame == false)
                    _placementModel.equips.Add(result);
            };
            Navigation.PushAsync(page);
        }
      
        void EquipmentBindingChanged(object sender, System.EventArgs e)
        {
            ViewCell cell = sender as ViewCell;
            if (_placementModel.canEdit == true)
            {
                if (cell.ContextActions.Count == 0)
                {
                    var deletEquipmentAction = new MenuItem { Text = "删除", IsDestructive = true };
                    deletEquipmentAction.Clicked += Handle_DeletEquipment;
                    cell.ContextActions.Add(deletEquipmentAction);
                }
            }
        }
        void Handle_DeletEquipment(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _placementModel.equips.Remove(item.BindingContext as AddPlacement_Equipment);
        }

        //添加任务
        void Handle_AddTask(object sender, System.EventArgs e)
        {
            EmergencyAddTaskPage taskPage = new EmergencyAddTaskPage();
            taskPage.SaveTask += (arg, args) =>
            {
                AddPlacement_Task t = arg as AddPlacement_Task;
                if (t == null) return;
                _placementModel.tasklist.Add(t);
            };
            Navigation.PushAsync(taskPage);
        }

        void Handle_TaskSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            AddPlacement_Task item = e.SelectedItem as AddPlacement_Task;
            if (item == null)
                return;

            EmergencyAddTaskPage taskPage = new EmergencyAddTaskPage(item);
            Navigation.PushAsync(taskPage);
            taskLV.SelectedItem = null;


        }
        void taskBindingChanged(object sender, System.EventArgs e)
        {
            ViewCell cell = sender as ViewCell;
            if (_placementModel.canEdit == true)
            {
                if(cell.ContextActions.Count == 0) {
                    var deletTaskAction = new MenuItem { Text = "删除", IsDestructive = true };
                    deletTaskAction.Clicked += Handle_DeletTask;
                    cell.ContextActions.Add(deletTaskAction);
                }
            }
        }

        void Handle_DeletTask(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _placementModel.tasklist.Remove(item.BindingContext as AddPlacement_Task);
        }
        //反地理编码
        private async void getAddressWihtLocation()
        {
            string param = "";
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync("https://apis.map.qq.com/ws/geocoder/v1/?location=" + Convert.ToDouble(_placementModel.lat) + "," + Convert.ToDouble(_placementModel.lng) + "&key=72NBZ-3YWK2-XV3U7-CM7OL-MKPMK-DRF2B", param, "GET", "");
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Dictionary<string, object> dic = Tools.JsonUtils.DeserializeObject<Dictionary<string, object>>(hTTPResponse.Results);
                if (dic == null) return;
                Dictionary<string, object> resultDic = Tools.JsonUtils.DeserializeObject<Dictionary<string, object>>(dic["result"].ToString());
                try
                {
                    if (resultDic == null || resultDic["address"] == null) return;
                    _placementModel.address = resultDic["address"].ToString();
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
        public AddPlacementPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            Title = "布点详情";
        }

        public AddPlacementPage(string planId) : this()
        {
            getPlacement(planId);

        }


        public AddPlacementPage(string projectId, string lat, string lng) : this()
        {

            _placementModel = new AddPlacementModel
            {
                canEdit = true,
                createtime = DateTime.Now,
                plantime = DateTime.Now,
                flag = "",
                staffs = new ObservableCollection<AddPlacement_Staff>(),
                equips = new ObservableCollection<AddPlacement_Equipment>(),
                tasklist = new ObservableCollection<AddPlacement_Task>(),
            };
            if (!string.IsNullOrWhiteSpace(lat) && !string.IsNullOrWhiteSpace(lng))
            {
                _placementModel.lat = lat;
                _placementModel.lng = lng;
                getAddressWihtLocation();
            }
            bottomH.Height = 50;
            BindingContext = _placementModel;
        

            _projectId = projectId;
        }

        async void getPlacement(string planid)
        {
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SamplingModule.url + "/Api/SamplePlan/GetDetail?id=" + planid, "", "GET", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _placementModel = Tools.JsonUtils.DeserializeObject<AddPlacementModel>(hTTPResponse.Results);
                if (_placementModel != null)
                {
                    _placementModel.canEdit = false;
                    BindingContext = _placementModel;
                }
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }


        async void addPlacement()
        {
            Dictionary<object, object> param = new Dictionary<object, object>();
            param.Add("flag", _placementModel.flag);
            param.Add("name", _placementModel.name);
            param.Add("projectid", _projectId);
            param.Add("plantime", _placementModel.plantime);
            param.Add("createtime", _placementModel.createtime);
            param.Add("lng", _placementModel.lng);
            param.Add("lat", _placementModel.lat);
            param.Add("address", _placementModel.address);
            param.Add("pretreatment", _placementModel.pretreatment);
            param.Add("security", _placementModel.security);
            param.Add("remarks", _placementModel.remarks);
            param.Add("qctip", _placementModel.qctip);
            param.Add("type", 1);
            string parameter = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SamplingModule.url + "/Api/SamplePlan/Add", parameter, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string planId = Tools.JsonUtils.DeserializeObject<string>(hTTPResponse.Results);
                _placementModel.id = planId;
                updatePlacement();
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }


        async void updatePlacement()
        {
            Dictionary<object, object> param = new Dictionary<object, object>();
            param.Add("flag", "Edit");
            param.Add("id", _placementModel.id);
            param.Add("name", _placementModel.name);
            param.Add("plantime", _placementModel.plantime);
            param.Add("createtime", _placementModel.createtime);
            param.Add("lng", _placementModel.lng);
            param.Add("lat", _placementModel.lat);
            param.Add("address", _placementModel.address);
            param.Add("pretreatment", _placementModel.pretreatment);
            param.Add("security", _placementModel.security);
            param.Add("remarks", _placementModel.remarks);
            param.Add("qctip", _placementModel.qctip);
            List<Dictionary<object, object>> staffs = new List<Dictionary<object, object>>();
            foreach (AddPlacement_Staff item in _placementModel.staffs)
            {
                Dictionary<object, object> staff = new Dictionary<object, object>();
                staff.Add("staffid", item.staffid);
                staff.Add("staffname", item.staffname);
                staffs.Add(staff);
            }
            param.Add("staffs", staffs);
            List<Dictionary<object, object>> equips = new List<Dictionary<object, object>>();
            foreach (AddPlacement_Equipment item in _placementModel.equips)
            {
                Dictionary<object, object> equip = new Dictionary<object, object>();
                equip.Add("equipid", item.equipid);
                equip.Add("equipname", item.equipname);
                equips.Add(equip);
            }
            param.Add("equips", equips);
            List<Dictionary<object, object>> tasklist = new List<Dictionary<object, object>>();
            foreach (AddPlacement_Task item in _placementModel.tasklist)
            {
                Dictionary<object, object> task = new Dictionary<object, object>();
                task.Add("flag", item.flag);
                task.Add("taskid", item.taskid);
                task.Add("taskname", item.taskname);
                task.Add("tasktype", item.tasktype);
                task.Add("taskstatus", item.taskstatus);
                task.Add("taskindex", item.taskindex);
                List<Dictionary<object, object>> Analysistypes = new List<Dictionary<object, object>>();
                foreach (AddPlacement_Analysist item1 in item.taskAnas)
                {
                    Dictionary<object, object> Analysistype = new Dictionary<object, object>();
                    Analysistype.Add("atid", item1.atid);
                    Analysistype.Add("attype", item1.attype);
                    Analysistypes.Add(Analysistype);
                }
                task.Add("taskAnas", Analysistypes);
                tasklist.Add(task);
            }
            param.Add("tasklist", tasklist);
            List<Dictionary<object, object>> PlanLists = new List<Dictionary<object, object>>();
            Dictionary<object, object> plan = new Dictionary<object, object>();
            PlanLists.Add(param);
            plan.Add("PlanLists", PlanLists);
            string parameter = JsonConvert.SerializeObject(plan);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SamplingModule.url + "/Api/SamplePlan/UpdateAll", parameter, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                bool success = Tools.JsonUtils.DeserializeObject<bool>(hTTPResponse.Results);
                if (success == true)
                    await Navigation.PopAsync();
                else
                    await DisplayAlert("提示", "添加失败", "确定");
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }

        }


    }
}
