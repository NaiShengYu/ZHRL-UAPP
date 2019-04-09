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
                mTask.flag = "Add";
                mTask.taskstatus = "0";
                mTask.taskAnas = datas;
            }
            BindingContext = mTask;
            LvItems.ItemsSource = datas;

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
            LvItems.ItemsSource = datas;
            BindingContext = mTask;
        }

        //添加项目
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            int type = 1;
            var action =await DisplayActionSheet("检测项目", "取消", null,new string[]{ "因子组检测", "单因子检测" });
            switch (action)
            {
                case "因子组检测":
                        type = 1;
                    break;
                case "单因子检测":
                        type = 2;
                    break;
             
                default: {

                        return;
                }
            }
            SampleItemsListPage samepleOther = new SampleItemsListPage(type);
            await Navigation.PushAsync(samepleOther);
            samepleOther.SelectItem += (object item, EventArgs bbb) => {
                SampleExamineItem selectItem = item as SampleExamineItem;
                bool has = false;
                foreach (var s in datas)
                {
                    if (s.atid == selectItem.id)
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    AddPlacement_Analysist anas = new AddPlacement_Analysist();
                    anas.atid = selectItem.id;
                    anas.atname = selectItem.name;
                    anas.attype = "0";
                    datas.Add(anas);
                }
                mTask.taskAnas = datas;
            };
        }

        void taskBindingChanged(object sender, System.EventArgs e)
        {
            ViewCell cell = sender as ViewCell;
            if (cell ==null)
            {
                return;
            }
            if (mTask.canEdit == true)
            {
                var deletTaskAction = new MenuItem { Text = "删除", IsDestructive = true };
                deletTaskAction.Clicked += Del_Clicked;
                cell.ContextActions.Add(deletTaskAction);
            }
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
            datas.Remove(s);
            datas.Remove(s);

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