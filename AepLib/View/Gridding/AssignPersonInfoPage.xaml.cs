using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    //指派部门/部门人员
    public partial class AssignPersonInfoPage : ContentPage
    {
      
      
        ObservableCollection<UserDepartmentsModel> gridList = new ObservableCollection<UserDepartmentsModel>();
        GridTaskInfoModel _infoModel = null;
        int _type = 1;
        Label _titleLab = null;
        Dictionary<string, object> _subDic = null;
        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {

        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            UserDepartmentsModel model = e.SelectedItem as UserDepartmentsModel;
            if (model == null) return;

            if(_type ==1){//部门/网格员类
                _infoModel.assignments.Clear();
                _infoModel.AssignName = model.name;
                if(model.name != "网格员")
                {
                    Assignments s1 = new Assignments
                    {
                        id = Guid.NewGuid(),
                        rowState = "add",
                        type = 3,
                        dept = model.id,
                    };
                    _infoModel.assignments.Add(s1);
                }
            }
            if (_type ==2)//部门人员
            {
                Assignments s1 = new Assignments
                {
                    id = Guid.NewGuid(),
                    rowState = "add",
                    type = 2,
                    staff = model.id,
                    dept = _infoModel.assignments[0].dept.Value,
                };
                if (_infoModel.assignments.Count >1)
                {
                    _infoModel.assignments.RemoveAt(1);
                }
                _infoModel.assignments.Add(s1);
                _infoModel.AssignName = model.name;
            }

            if(_type == 3){//子部门
                _subDic.Remove("toDept");
                _subDic.Add("toDept", model.id);
            }
            _titleLab.Text = model.name;
            Navigation.PopAsync();
            listView.SelectedItem = null;

           
        }
        public AssignPersonInfoPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskInfoModel">Task info model.</param>
        /// <param name="type">1.表示选择部门，2.表示选择部门人员，3.选择子部门</param>
        public AssignPersonInfoPage(GridTaskInfoModel taskInfoModel,int type,Label titleLab):this(){

            _titleLab = titleLab;
            _infoModel = taskInfoModel;
            listView.ItemsSource = gridList;
            _type = type;
            if (type == 1){
                gridList.Add(new UserDepartmentsModel
                {
                    name = "网格员",
                });
                getData();
            }

            if (type ==2)
            {
                reqDepartPerson(taskInfoModel.assignments[0].dept.Value);
            }
        }

        public AssignPersonInfoPage(ObservableCollection<UserDepartmentsModel> departList, int type, Label titleLab,Dictionary<string,object> subDic):this()
        {
            gridList = departList;
            _titleLab = titleLab;
            listView.ItemsSource = gridList;
            _type = type;
            _subDic = subDic;
        }


        private async void getData()
        {
                App.allDepartments =await (App.Current as App).GetDepartmentTree();
            getSubDeptsItems(App.allDepartments);
            //foreach (var item in App.allDepartments)
            //{
            //    reqGrid(item.id);
            //    foreach (var item1 in item.subDepts)
            //    {
            //        reqGrid(item.id);
            //    }
            //}
        }

        private async void getSubDeptsItems(ObservableCollection<GridAllDepartmentsModel> SubDepts)
        {
            foreach (var item in SubDepts)
            {
                UserDepartmentsModel model = new UserDepartmentsModel
                {
                    id = item.id,
                    name = item.name
                };
                gridList.Add(model);
                getSubDeptsItems(item.subDepts);
            }

        }


        //获取部门名称和id
        private async void reqGrid(Guid departId){
            string url = App.BasicDataModule.url + "/api/Modmanage/GetStaffDepartments";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("id",departId);
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var resultList = Tools.JsonUtils.DeserializeObject<ObservableCollection<UserDepartmentsModel>>(hTTPResponse.Results);
                    if (resultList == null) return;
                    foreach (var item in resultList)
                    {
                        gridList.Add(item);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        //获取部门人员id
        private async void reqDepartPerson(Guid departId)
        {
            string url = App.BasicDataModule.url + "/api/Modmanage/GetDepartmentUsers";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("id", departId);
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    ObservableCollection<string> departIds = new ObservableCollection<string>();
                    var resultList = Tools.JsonUtils.DeserializeObject<ObservableCollection<Dictionary<string, string>>>(hTTPResponse.Results);
                    if (resultList == null) return;
                    foreach (var item in resultList)
                    {
                        departIds.Add(item["userid"]);
                    }
                    reqDepartPersonTow(departIds);
                }
                catch (Exception ex)
                {
                }
            }
        }

        //获取部门人员名称
        private async void reqDepartPersonTow(ObservableCollection<string> departIds)
        {
            string url = App.FrameworkURL + "/api/fw/GetUserByArrUserid";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("items", departIds);
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string result = hTTPResponse.Results.Replace("username", "name");
                    var resultList = Tools.JsonUtils.DeserializeObject<ObservableCollection<UserDepartmentsModel>>(result);
                    if (resultList == null) return;
                    foreach (var item in resultList)
                    {
                        gridList.Add(item);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }






    }
}
