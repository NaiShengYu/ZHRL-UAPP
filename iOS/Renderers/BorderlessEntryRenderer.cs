using System;
using System.ComponentModel;
using AepApp.iOS.Renderers;
using AepApp.MaterialForms;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]

namespace AepApp.iOS.Renderers
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
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

            if (e.PropertyName == BorderlessPicker.HasBorderProperty.PropertyName)
                SetBorder(view);
        }

        private void SetBorder(BorderlessEntry view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
        }

    }
}
