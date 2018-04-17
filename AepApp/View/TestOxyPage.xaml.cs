using AepApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestOxyPage : ContentPage
	{
		public TestOxyPage ()
		{
			InitializeComponent ();
            //var data = new OxyDataPageModle().AreaModel;
            //abc.Model = data;
        }
	}
}