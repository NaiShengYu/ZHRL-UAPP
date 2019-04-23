using System;
using System.Collections.Generic;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class AssignPersonPage : ContentPage
    {
        GridTaskInfoModel _taskInfomodel = null;
        void AssignPerson(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(departmentLab.Text))
            {
                return;
            }
            if (departmentLab.Text == "网格员")
            {
                if(_taskInfomodel != null)
                {
                    _taskInfomodel.IsToDepartment = false;
                }
                Navigation.PushAsync(new GridTreeWithUserPage(_taskInfomodel));
            }
            else
            {
                if (_taskInfomodel != null)
                {
                    _taskInfomodel.IsToDepartment = true;
                }
                Navigation.PushAsync(new AssignPersonInfoPage(_taskInfomodel, 2, personnelLab));
            }
        }

        void AssignDepartment(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AssignPersonInfoPage(_taskInfomodel,1,departmentLab));
        }

        public AssignPersonPage(GridTaskInfoModel taskInfoModel)
        {
            InitializeComponent();
            _taskInfomodel = taskInfoModel;
            ToolbarItems.Add(new ToolbarItem("确定","",() => {
                if(departmentLab.Text == "网格员" && personnelLab.Text == "网格员")
                {
                    DisplayAlert("提示", "请至少选择一个网格或网格员", "确定");
                    return;
                }
                Navigation.PopAsync();
            }));
            BindingContext = _taskInfomodel;
        }

    }
}
