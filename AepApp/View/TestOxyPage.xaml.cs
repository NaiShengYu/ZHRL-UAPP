using AepApp.Models;
using Plugin.Media;
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

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }



            var file1 = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
            {
                DesiredLength = new TimeSpan(0, 0, 10),
                Name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4",
                Directory = "Video",
            });
            MV.Source = ImageSource.FromFile(file1.Path);



        }



		public TestOxyPage ()
		{
			InitializeComponent ();
            //var data = new OxyDataPageModle().AreaModel;
            //abc.Model = data;
        }
	}
}