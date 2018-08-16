using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AepApp.iOS.Renderers;
using AepApp.MaterialForms;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessTimePicker), typeof(BorderlessTimePickerRenderer))]
namespace AepApp.iOS.Renderers
{
    public class BorderlessTimePickerRenderer : TimePickerRenderer
    {
        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
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
                case TextAlignment.Center:
                    Control.TextAlignment = UITextAlignment.Center;
                    break;
                case TextAlignment.End:
                    Control.TextAlignment = UITextAlignment.Right;
                    break;
                case TextAlignment.Start:
                    Control.TextAlignment = UITextAlignment.Left;
                    break;
            }
        }

        private void SetBorder(BorderlessTimePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
        }


        private void SetFontSize(BorderlessTimePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            UIFont font = Control.Font.WithSize(view.TSize);
            Control.Font = font;
        }
    }
}