using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AepApp.iOS;
using AepApp.View;
using Foundation;
using CoreGraphics;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(TextMeterImplementation))]
namespace AepApp.iOS
{
    public class TextMeterImplementation : ITextMeter
    {
        //public static Xamarin.Forms.Size MeasureTextSize(string text, double width, double fontSize, string fontName = null)
        public double MeasureTextHeightGivenMaxWidth(string text, double width, double fontSize, string fontName = null)
        {
            var nsText = new NSString(text);
            var boundSize = new CGSize((float)width, float.MaxValue);
            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

            if (fontName == null)
            {
                fontName = "HelveticaNeue";
            }

            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName(fontName, (float)fontSize)
            };

            var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;

            //return new Xamarin.Forms.Size((double)sizeF.Width, (double)sizeF.Height);
            return (double)sizeF.Height + 5;
        }
        public double MeasureTextWidthGivenExactHeight(string text, double height, double fontSize, string fontName = null)
        {
            var nsText = new NSString(text);
            var boundSize = new CGSize(float.MaxValue, (float)height);
            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

            if (fontName == null)
            {
                fontName = "HelveticaNeue";
            }

            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName(fontName, (float)fontSize)
            };

            var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;

            //return new Xamarin.Forms.Size((double)sizeF.Width, (double)sizeF.Height);
            return (double)sizeF.Width + 5;
        }
    }
}