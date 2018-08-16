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

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace AepApp.iOS.Renderers
{
    public class BorderlessDatePickerRenderer : DatePickerRenderer
    {
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

        private void SetBorder(BorderlessDatePicker view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
        }

        private void SetFontSize(BorderlessDatePicker view)
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