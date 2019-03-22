using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class AddPlacementPage : ContentPage
    {
        ObservableCollection<string> _task = new ObservableCollection<string>();

        AddPlacementModel _placementModel = new AddPlacementModel();
        
        void Handle_Clicked(object sender, System.EventArgs e)
        {
        }

        //采样计划名称
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _placementModel.name = e.NewTextValue;
        }
        //编辑地址
        void Handle_editAddress(object sender, System.EventArgs e)
        {
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
            EditContentPage page = new EditContentPage("样品预处理",true, _placementModel.pretreatment);
            page.result += (object result, EventArgs even) => {
                _placementModel.pretreatment = result as string;
            };
            Navigation.PushAsync(page);
        }

        //质控说明
        void Handle_editQuality(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("质控说明", true, _placementModel.qctip);
            page.result += (object result, EventArgs even) => {
                _placementModel.qctip = result as string;
            };
            Navigation.PushAsync(page);
        }
        //备注信息
        void Handle_editRemarks(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("备注信息", true,_placementModel.remarks);
            page.result += (object result, EventArgs even) => {
                _placementModel.remarks = result as string;
            };
            Navigation.PushAsync(page);
        }
        //安全说明
        void Handle_editSafety(object sender, System.EventArgs e)
        {
            EditContentPage page = new EditContentPage("安全说明", true, _placementModel.security);
            page.result += (object result, EventArgs even) => {
                _placementModel.security = result as string;
            };
            Navigation.PushAsync(page);
        }
        //添加相关人员
        void Handle_AddPerson(object sender, System.EventArgs e)
        {
            PersonListPage page = new PersonListPage();
            bool isSame = false;
            page.SamplePerson += (object equipment, EventArgs even) => {
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
            page.SampleEquipment += (object equipment, EventArgs even) => {
                AddPlacement_Equipment result = equipment as AddPlacement_Equipment;
                foreach (AddPlacement_Equipment item in _placementModel.equips)
                {
                    if(item.equipid == result.equipid)
                    {
                        isSame = true;
                        break;
                    }
                }
                if (isSame ==false)
                    _placementModel.equips.Add(result);
            };
            Navigation.PushAsync(page);
        }
        void Handle_DeletEquipment(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _placementModel.equips.Remove(item.BindingContext as AddPlacement_Equipment);
        }
        void Handle_AddTask(object sender, System.EventArgs e)
        {
            _task.Add("瓶式深水采样器");
        }
        void Handle_DeletTask(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            _task.Remove(item.BindingContext as string);
        }


        public AddPlacementPage()
        {
            InitializeComponent();
            _placementModel.plantime =DateTime.Now;
            _placementModel.staffs = new ObservableCollection<AddPlacement_Staff>();
            _placementModel.equips = new ObservableCollection<AddPlacement_Equipment>();
            _placementModel.tasks = new ObservableCollection<AddPlacement_Task>();
            BindingContext = _placementModel;
            personLV.ItemsSource = _placementModel.staffs;
            PersonNumFrame.BindingContext = _placementModel.staffs;
            equipmentLV.ItemsSource = _placementModel.equips;
            equipmentNumFrame.BindingContext = _placementModel.equips;
            taskLV.ItemsSource = _task;
            taskNumFrame.BindingContext = _task;
        }



     
    }
}
