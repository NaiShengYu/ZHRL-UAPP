﻿
using AepApp.Droid.Effects;
using AepApp.View;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessEntryEffect))]
namespace AepApp.Droid.Effects
{
    public class BorderlessEntryEffect : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            
            if (e.OldElement == null)
            {
                Control.Background = null;
                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }

            if (e.NewElement == null)
            {
                //Unwire events
            }
        }

    }
}