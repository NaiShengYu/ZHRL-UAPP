
using AepApp.Droid.Effects;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


//[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(BorderlessEntryEffect), "BorderlessEntryEffect")]
namespace AepApp.Droid.Effects
{
    public class BorderlessEntryEffect : PlatformEffect
    {

        protected override void OnAttached()
        {
            //var entry = Control as UITextField;
            //if (entry != null)
            //{
            //    entry.BorderStyle = UITextBorderStyle.None;
            //}
        }
        protected override void OnDetached()
        {

        }
    }
}