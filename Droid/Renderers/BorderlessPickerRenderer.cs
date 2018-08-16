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

[assembly: ExportRenderer(typeof(BorderlessPicker), typeof(BorderlessPickerRenderer))]
namespace AepApp.Droid.Renderers
{
    public class BorderlessPickerRenderer : PickerRenderer
    {
        public BorderlessPickerRenderer(Context context) : base(context)
        {

        }

        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            //if (e.OldElement == null)
            //{
            //Control.Background = null;

            //var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
            //layoutParams.SetMargins(0, 0, 0, 0);
            //LayoutParameters = layoutParams;
            //Control.LayoutParameters = layoutParams;
            //Control.SetPadding(0, 0, 0, 0);
            //Control.TextAlignment = Android.Views.TextAlignment.TextEnd;
            //SetPadding(0, 0, 0, 0);
            //}

            var view = (BorderlessPicker)Element;
            SetBorder(view);
            SetTextAlignment(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var view = (BorderlessPicker)Element;
            if (e.PropertyName == BorderlessPicker.HasBorderProperty.PropertyName)
            {
                SetBorder(view);
            }
            else if (e.PropertyName == BorderlessPicker.XAlignProperty.PropertyName)
            {
                SetTextAlignment(view);
            }
        }

        private void SetTextAlignment(BorderlessPicker view)
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

        private void SetBorder(BorderlessPicker view)
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