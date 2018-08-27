using System;
using Xamarin.Forms;
using AepApp.MaterialForms;
using Xamarin.Forms.Platform.iOS;
using static Java.Util.ResourceBundle;
using UIKit;
using AepApp.iOS.Renderers;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BorderlessEditor), typeof(BorderLessEditorRender))]

namespace AepApp.iOS.Renderers
{
    public class BorderLessEditorRender:EntryRenderer
    {

        public static void Init() { }

        //protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        //{
        //    base.OnElementChanged(e);
        //    var view = e.NewElement as BorderlessEditor;
        //    SetBorder(view);
        //}

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);

        //    var view = (BorderlessEditor)Element;

        //    if (e.PropertyName == BorderlessEditor.HasBorderProperty.PropertyName)
        //        SetBorder(view);
        //}

        //private void SetBorder(BorderlessEditor view)
        //{
        //    if (view == null || Control == null)
        //    {
        //        return;
        //    }
        //    Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
        //}
    }
}
