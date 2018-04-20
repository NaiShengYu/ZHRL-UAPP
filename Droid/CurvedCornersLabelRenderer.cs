using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AepApp.Droid;
using AepApp.View;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CurvedCornersLabel), typeof(CurvedCornersLabelRenderer))]
namespace AepApp.Droid
{
    public class CurvedCornersLabelRenderer : LabelRenderer
    {
        public CurvedCornersLabelRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        private GradientDrawable _gradientBackground;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (CurvedCornersLabel)Element;
            if (view == null) return;
            if (Control != null)
            {
                float w = DpToPixels(Context, (float)view.CurvedCornerRadius);
                Control.SetPadding((int)w, 0, (int)w, 0);
            }
            Paint(view);
        }

        private void Paint(CurvedCornersLabel view)
        {
            // creating gradient drawable for the curved background
            _gradientBackground = new GradientDrawable();
            _gradientBackground.SetShape(ShapeType.Rectangle);
            _gradientBackground.SetColor(view.CurvedBackgroundColor.ToAndroid());

            // Thickness of the stroke line
            _gradientBackground.SetStroke(4, view.CurvedBackgroundColor.ToAndroid());

            // Radius for the curves
            _gradientBackground.SetCornerRadius(
                DpToPixels(this.Context,
                Convert.ToSingle(view.CurvedCornerRadius)));

            // set the background of the label
            Control.SetBackground(_gradientBackground);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // re-paint if these properties change at runtime
            if (e.PropertyName == CurvedCornersLabel.BackgroundColorProperty.PropertyName ||
                e.PropertyName == CurvedCornersLabel.CurvedCornerRadiusProperty.PropertyName)
            {
                if (Element != null)
                {
                    Paint((CurvedCornersLabel)Element);
                }
            }
        }

        /// <summary>
        /// Device Independent Pixels to Actual Pixles conversion
        /// </summary>
        /// <param name="context"></param>
        /// <param name="valueInDp"></param>
        /// <returns></returns>
        public static float DpToPixels(Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }
    }
}