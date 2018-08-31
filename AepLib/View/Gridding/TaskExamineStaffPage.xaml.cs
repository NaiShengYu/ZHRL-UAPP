using AepApp.Models;
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
	public partial class TaskExamineStaffPage : ContentPage
	{

        TaskExamineStaffModel model;
		public TaskExamineStaffPage ()
		{
			InitializeComponent ();
            model = new TaskExamineStaffModel
            {
                date = DateTime.Now,
                total = 10,
                finished = 5,
                lastRatio = "70%",
                dispatchTotal = 2,
                dispatchFinished = 1,
                dispatchLastRatio = "86.9%",
            };
            BindingContext = model;

        }
	}
}