using AepApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskFilterPage : ContentPage
    {
        TaskListPage.TaskFilterCondition conditions;
        public TaskFilterPage(TaskListPage.TaskFilterCondition filterCondition)
        {
            InitializeComponent();
            conditions = filterCondition;
            BindingContext = filterCondition;
            InitSource();
            InitFilter(filterCondition);
        }

        private void InitSource()
        {
            List<string> status = ConstConvertUtils.GetAllTaskState();
            pickerStatus.ItemsSource = status;
            List<string> type = ConstConvertUtils.GetAllTaskType();
            pickerType.ItemsSource = type;

        }

        private void InitFilter(TaskListPage.TaskFilterCondition conditions)
        {

            if (conditions == null)
            {
                return;
            }
            pickerType.SelectedItem = conditions.type;
            pickerStatus.SelectedItem = conditions.status;
            if (conditions.isTimeOn)
            {
                DatePickerStart.Date = conditions.dayStart == null ? DateTime.Now : conditions.dayStart;
                DatePickerEnd.Date = conditions.dayEnd == null ? DateTime.Now : conditions.dayEnd;
                TimePickerStart.Time = conditions.timeStart;
                TimePickerEnd.Time = conditions.timeEnd;
            }
        }

        private void Switch_Toggled_Title(object sender, ToggledEventArgs e)
        {

        }
        private void Switch_Toggled_Time(object sender, ToggledEventArgs e)
        {

        }

        private void Switch_Toggled_Status(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                pickerStatus.Focus();
            }
            return;
        }

        private void Switch_Toggled_Type(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                pickerType.Focus();
            }
            return;
        }

        private void Switch_Toggled_Griders(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
            Navigation.PushAsync(new GridTreeViewPage(true, false));
            MessagingCenter.Unsubscribe<ContentPage, TestTreeModel>(this, SubcriberConst.MSG_SELECT_GRIDER);
            MessagingCenter.Subscribe<ContentPage, TestTreeModel>(this, SubcriberConst.MSG_SELECT_GRIDER, (arg1, arg2) =>
            {
                TestTreeModel node = arg2 as TestTreeModel;
                if (node != null)
                {
                    conditions.griders = node.name;
                }
            });
        }

        private void Switch_Toggled_Address(object sender, ToggledEventArgs e)
        {

        }
        private void Switch_Toggled_Watchers(object sender, ToggledEventArgs e)
        {

        }

        private void pickerStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            conditions.status = picker.SelectedItem as string;
        }

        private void pickerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            conditions.type = picker.SelectedItem as string;
        }

        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            conditions.dayStart = e.NewDate;
        }

        private void DatePickerEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            conditions.dayEnd = e.NewDate;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            conditions.timeStart = TimePickerStart.Time;
            conditions.timeEnd = TimePickerEnd.Time;
            conditions.dayStart = DatePickerStart.Date;
            conditions.dayEnd = DatePickerEnd.Date;
            MessagingCenter.Send<ContentPage, TaskListPage.TaskFilterCondition>(this, TaskListPage.SUBSCRIBE_SEARCH, conditions);
            MessagingCenter.Unsubscribe<ContentPage, string>(this, TaskListPage.SUBSCRIBE_SEARCH);
            MessagingCenter.Unsubscribe<ContentPage, string>(this, SubcriberConst.MSG_SELECT_GRIDER);
        }
    }
}