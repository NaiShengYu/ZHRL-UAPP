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
	public partial class TaskFilterPage : ContentPage
	{

        private TaskListPage.TaskFilterCondition conditions;
        public TaskFilterPage (TaskListPage.TaskFilterCondition filterCondition)
		{
			InitializeComponent ();
            conditions = filterCondition;
            BindingContext = filterCondition;
            //switchKey.IsToggled = !string.IsNullOrWhiteSpace(filterCondition.SearchName);
            //switchStatus.IsToggled = !string.IsNullOrWhiteSpace(filterCondition.Status);
        }

        private void Switch_Toggled_Title(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
            {
                conditions.SearchName = "";
            }
        }
        private void Switch_Toggled_Time(object sender, ToggledEventArgs e)
        {

        }

        private void Switch_Toggled_Status(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                //pickerStatus.IsVisible = true;
                pickerStatus.Focus();
            }
            else
            {
                //pickerStatus.IsVisible = false;
                conditions.Status = "";
            }
            return;
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