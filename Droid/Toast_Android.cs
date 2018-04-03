using Sample.Droid;

using Android.Widget;

using Xamarin.Forms;



[assembly: Dependency(typeof(Toast_Android))]

namespace Sample.Droid

{

    public class Toast_Android : IToast

    {

        public void LongAlert(string message)

        {

            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();

        }

        public void ShortAlert(string message)

        {

            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();

        }



    }

}