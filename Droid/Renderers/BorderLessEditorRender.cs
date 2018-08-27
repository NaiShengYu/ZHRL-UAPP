using System;
using System.Runtime.Remoting.Contexts;
using AepApp.Droid.Renderers;
using AepApp.MaterialForms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessEditor), typeof(BorderLessEditorRender))]

namespace AepApp.Droid.Renderers
{
    public class BorderLessEditorRender:EntryRenderer
    {

        public BorderLessEditorRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as BorderlessDatePicker;
            SetBorder(view);
        }

        private void SetBorder(BorderlessDatePicker view)
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
