using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.Droid;
using AepApp.View;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(TextMeterImplementation))]
namespace AepApp.Droid
{
    public class TextMeterImplementation : ITextMeter
    {
        private Typeface textTypeface;

        //public static Xamarin.Forms.Size MeasureTextSize(string text, double width, double fontSize, string fontName = null)
        public double MeasureTextHeightGivenMaxWidth(string text, double width, double fontSize, string fontName = null)
        {
            var textView = new TextView(global::Android.App.Application.Context);
            textView.Typeface = GetTypeface(fontName);
            textView.SetText(text, TextView.BufferType.Normal);
            textView.SetTextSize(ComplexUnitType.Px, (float)fontSize);

            int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
                (int)width, MeasureSpecMode.AtMost);
            int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
                0, MeasureSpecMode.Unspecified);

            textView.Measure(widthMeasureSpec, heightMeasureSpec);

            //return new Xamarin.Forms.Size((double)textView.MeasuredWidth, (double)textView.MeasuredHeight);
            return (double)textView.MeasuredHeight;
        }

        public double MeasureTextWidthGivenExactHeight(string text, double height, double fontSize, string fontName = null)
        {
            var textView = new TextView(global::Android.App.Application.Context);
            textView.Typeface = GetTypeface(fontName);
            textView.SetText(text, TextView.BufferType.Normal);
            textView.SetTextSize(ComplexUnitType.Px, (float)fontSize);

            int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
                0, MeasureSpecMode.Unspecified);
            int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
                (int)height, MeasureSpecMode.Exactly);

            textView.Measure(widthMeasureSpec, heightMeasureSpec);

            //return new Xamarin.Forms.Size((double)textView.MeasuredWidth, (double)textView.MeasuredHeight);
            return (double)textView.MeasuredWidth;
        }

        private Typeface GetTypeface(string fontName)
        {
            if (fontName == null)
            {
                return Typeface.Default;
            }

            if (textTypeface == null)
            {
                textTypeface = Typeface.Create(fontName, TypefaceStyle.Normal);
            }

            return textTypeface;
        }
    }
}