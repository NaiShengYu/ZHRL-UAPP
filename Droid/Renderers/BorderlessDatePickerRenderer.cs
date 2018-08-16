using System.ComponentModel;
using AepApp.Droid.Renderers;
using AepApp.MaterialForms;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace AepApp.Droid.Renderers
{
    public class BorderlessDatePickerRenderer : DatePickerRenderer
    {
        public BorderlessDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as BorderlessDatePicker;
            SetBorder(view);
            SetTextAlignment(view);
            SetFontSize(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = (BorderlessDatePicker)Element;

            if (e.PropertyName == BorderlessDatePicker.XAlignProperty.PropertyName)
                SetTextAlignment(view);
            if (e.PropertyName == BorderlessDatePicker.HasBorderProperty.PropertyName)
                SetBorder(view);
            if (e.PropertyName == BorderlessDatePicker.TextSizeProperty.PropertyName)
                SetFontSize(view);
        }

        private void SetTextAlignment(BorderlessDatePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            switch (view.XAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.TextAlignment = Android.Views.TextAlignment.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.TextAlignment = Android.Views.TextAlignment.TextEnd;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.TextAlignment = Android.Views.TextAlignment.TextStart;
                    break;
            }
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

        private void SetFontSize(BorderlessDatePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            Control.TextSize = view.TSize;
        }
    }
}