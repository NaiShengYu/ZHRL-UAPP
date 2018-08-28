using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.iOS.Renderers;
using AepApp.MaterialForms;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessEditor), typeof(BorderlessEditorRenderer))]
namespace AepApp.iOS.Renderers
{
    public class BorderlessEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            var view = e.NewElement as BorderlessEditor;
            SetBorder(view);
        }


        private void SetBorder(BorderlessEditor view)
        {
            if (view == null || Control == null)
            {
                return;
            }
            Control.Layer.BorderWidth = view.HasBorder ? 1 : 0;
        }
    }
}