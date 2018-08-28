using System.ComponentModel;
using AepApp.Droid.Renderers;
using AepApp.MaterialForms;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]

namespace AepApp.Droid.Renderers
{
    public interface BorderlessEntryRenderer:EntryRenderer
    {
        public BorderlessEntryRenderer(Context context) : base(context)
        {
        }

        public static void Init() { }


        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as BorderlessEntry;
            SetBorder(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = (BorderlessEntry)Element;

            if (e.PropertyName == BorderlessEntry.HasBorderProperty.PropertyName)
                SetBorder(view);
         
        }


        private void SetBorder(BorderlessEntry view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            if (!view.HasBorder)
            {
                Control.Background = null;
                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }
        }

    }
}
