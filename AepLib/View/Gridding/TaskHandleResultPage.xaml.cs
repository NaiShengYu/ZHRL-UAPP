using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskHandleResultPage : ContentPage
    {
        public TaskHandleResultPage()
        {
            InitializeComponent();
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
                pickerStatus.IsVisible = true;
                pickerStatus.Focus();
            }
            else
            {
                pickerStatus.IsVisible = false;
            }
            return;
            List<string> _status = new List<string>();
            _status.Add("处理中");
            _status.Add("已上报");
            _status.Add("已处理");
            Picker picker = new Picker
            {
                Title = "请选择任务类型",
                HorizontalOptions = LayoutOptions.Center,
            };
            picker.ItemsSource = _status;
        }

        private void Switch_Toggled_Type(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                pickerType.IsVisible = true;
                pickerType.Focus();
            }
            else
            {
                pickerType.IsVisible = false;
            }
            return;

            List<string> _types = new List<string>();
            _types.Add("恶臭事件");
            _types.Add("污水偷排事件");
            Picker picker = new Picker
            {
                Title = "请选择任务类型",
                HorizontalOptions = LayoutOptions.Center,
            };
            picker.ItemsSource = _types;
            picker.IsVisible = true;
            picker.IsEnabled = true;
            picker.Focus();
        }

        private void Switch_Toggled_Griders(object sender, ToggledEventArgs e)
        {

        }

        private void Switch_Toggled_Address(object sender, ToggledEventArgs e)
        {

        }
        private void Switch_Toggled_Watchers(object sender, ToggledEventArgs e)
        {

        }
    }
}