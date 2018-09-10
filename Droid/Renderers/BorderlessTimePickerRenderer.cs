using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AepApp.Droid.Renderers;
using AepApp.MaterialForms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessTimePicker), typeof(BorderlessTimePickerRenderer))]
namespace AepApp.Droid.Renderers
{
    public class BorderlessTimePickerRenderer : TimePickerRenderer
    {
        public BorderlessTimePickerRenderer(Context context) : base(context)
        {
        }

        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as BorderlessTimePicker;
            SetBorder(view);
            SetTextAlignment(view);
            SetFontSize(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var view = (BorderlessTimePicker)Element;

            if (e.PropertyName == BorderlessTimePicker.XAlignProperty.PropertyName)
                SetTextAlignment(view);
            if (e.PropertyName == BorderlessTimePicker.HasBorderProperty.PropertyName)
                SetBorder(view);
            if (e.PropertyName == BorderlessTimePicker.TextSizeProperty.PropertyName)
                SetFontSize(view);
        }

        private void SetTextAlignment(BorderlessTimePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            switch (view.XAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.Gravity = Android.Views.GravityFlags.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.Gravity = Android.Views.GravityFlags.End;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.Gravity = Android.Views.GravityFlags.Start;
                    break;
            }
        }

        private void SetBorder(BorderlessTimePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            //Control.TextSize = 15;
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

        private void SetFontSize(BorderlessTimePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            Control.TextSize = view.TSize;
        }
    }
}