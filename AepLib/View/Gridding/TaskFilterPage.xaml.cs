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
		public TaskFilterPage ()
		{
			InitializeComponent ();
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