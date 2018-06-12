using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AepApp.iOS;
using AepApp.View;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MasterAndDetailPage), typeof(MasterAndDetailPageRenderer))]
namespace AepApp.iOS
{
    public class MasterAndDetailPageRenderer : MyPhoneMasterDetailRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);


        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            
        }
    }
}