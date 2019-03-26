using AepApp.Models;
using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

//添加应急任务
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyAddTaskPage : ContentPage
    {
        private AddPlacement_Task mTask;
        private List<AddPlacement_Analysist> examineItems = new List<AddPlacement_Analysist>();
        private ObservableCollection<AddPlacement_Analysist> datas = new ObservableCollection<AddPlacement_Analysist>();

        public event EventHandler<EventArgs> SaveTask;

        public EmergencyAddTaskPage()
        {
            InitializeComponent();
            if (mTask == null)
            {
                mTask = new AddPlacement_Task();
                pickerType.SelectedIndex = 0;
                mTask.taskindex = "0";
                mTask.canEdit = true;
            }
            BindingContext = mTask;
        }

        public EmergencyAddTaskPage(AddPlacement_Task task) : this()
        {
            mTask = task;
            SetTaskInfo();
        }

        private void SetTaskInfo()
        {
            if (mTask == null) return;
            mTask.canEdit = false;
            int code = ConstConvertUtils.getTaskTypeCode(mTask.taskname);
            if (code >= 0)
            {
                pickerType.SelectedIndex = code;
            }
            datas = mTask.taskAnas;
            examineItems.Clear();
            foreach (var item in datas)
            {
                examineItems.Add(item);
            }
            LvItems.ItemsSource = datas;
            LbNum.Text = datas.Count + "";
            BindingContext = mTask;
        }

        //添加项目
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SampleItemsListPage());
            MessagingCenter.Unsubscribe<ContentPage, SampleExamineItem>(this, "selectItem");
            MessagingCenter.Subscribe<ContentPage, SampleExamineItem>(this, "selectItem", async (arg1, arg2) =>
            {
                var item = arg2 as SampleExamineItem;
                bool has = false;
                foreach (var s in examineItems)
                {
                    if (s.atid == item.id)
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    AddPlacement_Analysist anas = new AddPlacement_Analysist();
                    anas.atid = item.id;
                    anas.atname = item.name;
                    anas.attype = "0";
                    examineItems.Add(anas);
                }
                datas = new ObservableCollection<AddPlacement_Analysist>(examineItems);
                mTask.taskAnas = datas;
                LvItems.ItemsSource = datas;
                LbNum.Text = examineItems.Count + "";
            });
        }


        private void pickerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var typeName = picker.SelectedItem as string;
            int typeCode = picker.SelectedIndex;
            mTask.tasktype = typeCode + "";
        }

        private void Del_Clicked(object sender, System.EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            AddPlacement_Analysist s = item.BindingContext as AddPlacement_Analysist;
            examineItems.Remove(s);
            datas.Remove(s);
            LbNum.Text = examineItems.Count + "";
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!mTask.canEdit)
            {
                return;
            }
            mTask.taskname = EntryName.Text;
            if (string.IsNullOrWhiteSpace(mTask.taskname))
            {
                DisplayAlert("提示", "请输入任务名称", "确定");
                return;
            }
            SaveTask.Invoke(mTask, new EventArgs());
            Navigation.PopAsync();
        }
    }
}